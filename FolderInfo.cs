using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyFolderInfo.Model;
using MyFolderInfo.UI;

namespace MyFolderInfo
{
    /// <summary>
    /// Main window: scans a folder into a tree, lets you open a saved tree, drag a subtree
    /// into a second "compare" tree, and search both.
    /// </summary>
    public partial class FolderInfo : ParentForm
    {
        /// <summary>
        /// Options loaded from Options.xml. This used to be a static list that was appended
        /// to on every "Options -&gt; Save", so the list grew without bound and the language
        /// loader walked an ever-larger collection.
        /// </summary>
        internal static IList<ClassOptions> ClsOptions { get; private set; } = new List<ClassOptions>();

        private List<ClassTree> _listTree1;
        private List<ClassTree> _listTree2;
        private int _maxNodeCode;
        private ClassLanguage _clsLanguage;
        private readonly TreeLib _treeLib = new();
        private string _oldDir = string.Empty;
        private string _newDir = string.Empty;
        private MediaUserControl _userMediaUC;
        private TreeNode _rightSelectNode;
        private ClassTree _clsTree2;
        private ClassTree _clsTree1;
        private bool _isNewSearch = true;
        private List<int> _searchList;
        private int _times;

        // Modeless child windows - keep one of each so repeated menu clicks cannot stack up
        // dozens of copies, each holding its own view of the same data. They are fetched
        // through Reusable(), because a cached instance is disposed as soon as the user
        // closes it.
        private FrmCompare _frmCompare;
        private FrmDirectSearch _frmDirectSearch;
        private FrmEvents _frmEvents;
        private FrmPictures _frmPictures;
        private FrmSearchDetail _frmSearchDetail;

        /// <summary>Untranslated "Length" caption, restored when a right-click misses a node.</summary>
        private string _lengthLabel = "Length";

        private CancellationTokenSource _scanCts;

        /// <summary>
        /// Returns a still-usable instance of a cached modeless window, or a fresh one.
        /// <c>_frmX ??= new FrmX()</c> handed the <em>disposed</em> instance back after the
        /// user closed the window, and the next "right-click a node -&gt; pick a menu item"
        /// died with <see cref="ObjectDisposedException"/> out of <c>Form.Show()</c>.
        /// </summary>
        private static T Reusable<T>(T cached, Func<T> create) where T : Form =>
            cached is { IsDisposed: false } ? cached : create();

        /// <summary>
        /// True when <paramref name="url"/> points at something the shell can open. The tree
        /// holds folders as well as files and <see cref="File.Exists(string)"/> answers false
        /// for every directory, so a check that used it alone reported "this node no longer
        /// exists on disk" for every folder the user right-clicked.
        /// </summary>
        internal static bool CanOpenInShell(string url) =>
            !string.IsNullOrWhiteSpace(url) && (File.Exists(url) || Directory.Exists(url));

        public FolderInfo()
        {
            InitializeComponent();
            _treeLib.AttachLog(ErrorLog.Current);
        }

        #region Left pane: scanning and loading

        private async void CmdLoad_Click(object sender, EventArgs e)
        {
            while (true)
            {
                if (folderBrowserDialog1.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                // The picker can hand back a shell-namespace or extended-length path, and a
                // mapped drive that is merely unreachable used to be reported as "not a folder"
                // because Directory.Exists() cannot fail loudly. Probe it properly instead.
                string path = PathProbe.Normalize(folderBrowserDialog1.SelectedPath);
                PathVerdict verdict = PathProbe.Probe(path, out int win32Code);

                if (verdict == PathVerdict.Ok)
                {
                    (int Levels, bool OnlyDeepest)? mode = AskScanLevels(path);
                    if (mode == null)
                    {
                        return; // the user cancelled the depth prompt
                    }

                    await ScanAsync(path, mode.Value.Levels, mode.Value.OnlyDeepest);
                    return;
                }

                ErrorLog.Current?.Write(
                    $"Cannot scan '{path}': {verdict} (Win32 {win32Code}).");

                MessageBoxIcon icon = verdict == PathVerdict.IsFile
                    ? MessageBoxIcon.Warning
                    : MessageBoxIcon.Error;

                string text = DescribeUnreadableFolder(path, verdict, _clsLanguage);

                // A dropped WebDAV/SharePoint mapping usually comes back by itself, so let the
                // user retry the same folder instead of having to re-pick it every time.
                if (verdict is PathVerdict.Unreachable or PathVerdict.Busy or PathVerdict.DeviceNotReady)
                {
                    if (MessageBox.Show(this, text + Environment.NewLine + Environment.NewLine + "Try again?",
                        "My Folder Info", MessageBoxButtons.YesNo, icon) == DialogResult.Yes)
                    {
                        continue;
                    }
                }
                else
                {
                    MessageBox.Show(this, text, "My Folder Info", MessageBoxButtons.OK, icon);
                }

                return;
            }
        }

        private (int Levels, bool OnlyDeepest)? AskScanLevels(string path)
        {
            using var frm = new FrmScanMode { Owner = this, FolderPath = path };

            if (frm.ShowDialog(this) != DialogResult.OK)
            {
                return null;
            }

            return frm.AllLevels
                ? (int.MaxValue, false)
                : (frm.Levels, frm.OnlyDeepest);
        }

        private async Task ScanAsync(string path, int levels, bool onlyDeepest)
        {
            bool includeFiles = chkWriteFile.Checked;
            var collected = new List<ClassTree>();

            _treeLib.ResetScan();
            _scanCts?.Cancel();
            _scanCts = new CancellationTokenSource();
            CancellationToken token = _scanCts.Token;

            toolStripStatusLabel.Text = "Scanning...";
            treeView1.Nodes.Clear();
            UseWaitCursor = true;
            cmdLoad.Enabled = false;

            var progress = new FrmProgress("Scanning " + path);
            try
            {
                progress.Show(this);

                // Report from the worker; Report() marshals onto the UI thread itself.
                // Throttled so a fast local SSD does not flood the message queue with
                // thousands of BeginInvoke calls per second.
                int lastReported = 0;
                void OnProgress(int count, string current)
                {
                    if (count - lastReported < 50)
                    {
                        return;
                    }

                    lastReported = count;
                    progress.Report(count, current);
                }

                await Task.Run(
                    () =>
                    {
                        if (includeFiles)
                        {
                            _treeLib.GetFolderWithFiles(path, 0, 0, collected, token, OnProgress, levels, onlyDeepest);
                        }
                        else
                        {
                            _treeLib.GetFolder(path, 0, 0, collected, token, OnProgress, levels, onlyDeepest);
                        }
                    },
                    token);

                if (token.IsCancellationRequested || progress.Cancelled)
                {
                    _log("Scan cancelled.");
                    return;
                }

                collected.Sort(new ClassTreeComparer());
                _treeLib.InvalidateIndex();

                _listTree1 = collected;
                _treeLib.AddTreeView(treeView1, _listTree1);
                ResetSearch();

                if (treeView1.Nodes.Count > 0)
                {
                    treeView1.SelectedNode = treeView1.Nodes[0];
                    treeView1.SelectedNode.Expand();
                }

                _log($"Loaded {_listTree1.Count:N0} nodes.");

                // Whatever the drive would not let us read - a denied system folder, a
                // SharePoint entry that vanished mid-walk, a name Windows rejects - is now
                // skipped instead of aborting the scan, so say how much was skipped.
                _treeLib.WriteScanSummary();

                // Everything below the root failed to enumerate (a SharePoint drive that
                // dropped mid-scan, for instance). The scan "succeeded" with a single node,
                // which reads exactly like a bug, so say what happened.
                if (_listTree1.Count <= 1)
                {
                    ErrorLog.Current?.Write(
                        $"Scan of '{path}' produced {_listTree1.Count} node(s): nothing below the root could be listed.");
                }
            }
            catch (OperationCanceledException)
            {
                _log("Scan cancelled.");
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write("Folder scan failed.", ex);
                MessageBox.Show(this, ex.Message, "Scan failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Cancel button sets progress.Cancelled; make that reach the walker.
                if (progress.Cancelled && !token.IsCancellationRequested)
                {
                    _scanCts?.Cancel();
                }

                if (!progress.IsDisposed)
                {
                    progress.Close();
                    progress.Dispose();
                }

                UseWaitCursor = false;
                cmdLoad.Enabled = true;
            }
        }

        private void CmdSave_Click(object sender, EventArgs e)
        {
            if (_listTree1 == null)
            {
                return;
            }

            ConfigureSaveDialog("XML file|*.xml", "*.xml");
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            WriteXmlSafe(_listTree1, saveFileDialog1.FileName);
        }

        private void CmdText_Click(object sender, EventArgs e)
        {
            if (_listTree1 == null || _listTree1.Count == 0)
            {
                return;
            }

            if (!AskTextExportOptions(out bool numberNames))
            {
                return;
            }

            ConfigureSaveDialog("Text file|*.txt", "*.txt");
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            WriteTextSafe(_listTree1, saveFileDialog1.FileName, numberNames);
        }

        private bool AskTextExportOptions(out bool numberNames)
        {
            using var frm = new FrmTextExport { Owner = this };

            numberNames = true;
            if (frm.ShowDialog(this) != DialogResult.OK)
            {
                return false;
            }

            numberNames = frm.NumberNames;
            return true;
        }

        private void CmdLoadXml_Click(object sender, EventArgs e)
        {
            // Bind the backing list up front: LoadTreeFile sets tv.SelectedNode, which raises
            // AfterSelect synchronously, and the handler materialises children through _listTree1.
            // Assigning it only after LoadTreeFile returned left the freshly loaded nodes
            // invisible - the tree showed just the root. Restore the old list on failure so a
            // cancelled or corrupt load does not orphan the tree from its data.
            List<ClassTree> previous = _listTree1;
            var list = new List<ClassTree>();
            _listTree1 = list;

            if (!LoadTreeFile(treeView1, list, out string file))
            {
                _listTree1 = previous;
                return;
            }

            _log($"Loaded {_listTree1.Count:N0} nodes from {Path.GetFileName(file)}.");
        }

        private void CmdOpen_Click(object sender, EventArgs e)
        {
            // See CmdLoadXml_Click: the list must be live before LoadTreeFile selects the root.
            List<ClassTree> previous = _listTree2;
            var list = new List<ClassTree>();
            _listTree2 = list;

            if (!LoadTreeFile(treeView2, list, out _))
            {
                _listTree2 = previous;
                return;
            }

            // Was list.Max() on a possibly-empty sequence, which threw InvalidOperationException
            // as soon as you opened a legitimately empty (or cancelled) tree file.
            _maxNodeCode = _treeLib.GetMaxNodeCode(_listTree2);
            ResetSearch();
        }

        /// <summary>Opens a tree file, streaming it, and repopulates <paramref name="tv"/>.</summary>
        private bool LoadTreeFile(TreeView tv, List<ClassTree> list, out string fileName)
        {
            fileName = null;
            toolStripStatusLabel.Text = "Loading...";

            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Filter = "XML file|*.xml|All files|*.*";

            if (openFileDialog1.ShowDialog(this) != DialogResult.OK)
            {
                toolStripStatusLabel.Text = "Ready";
                return false;
            }

            fileName = openFileDialog1.FileName;

            UseWaitCursor = true;
            try
            {
                if (!_treeLib.ReadXml(fileName, list))
                {
                    MessageBox.Show(this,
                        $"'{Path.GetFileName(fileName)}' could not be read. It may be corrupt.",
                        "Load failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                list.Sort(new ClassTreeComparer());
                _treeLib.InvalidateIndex();

                tv.Nodes.Clear();
                _treeLib.AddTreeView(tv, list);

                if (list.Count > 0 && tv.Nodes.Count > 0)
                {
                    tv.SelectedNode = tv.Nodes[0];
                    tv.SelectedNode.Expand();
                }

                toolStripStatusLabel2.Text = $"{list.Count:N0} nodes";
                toolStripStatusLabel.Text = "Ready";
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write($"Failed to load {fileName}.", ex);
                MessageBox.Show(this, ex.Message, "Load failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _treeLib.TV1Select(treeView1, _listTree1);
        }

        /// <summary>
        /// Fills in the node's children the moment its "+" is clicked. Without this the only
        /// way to see a folder's contents was to click the label, because children were only
        /// ever created from AfterSelect.
        /// </summary>
        private void TreeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            _treeLib.BeforeExpand(e.Node, _listTree1);
        }

        /// <summary>
        /// Resolves the node a right-click landed on and remembers it, so every item in the
        /// context menu acts on <em>that</em> node. Only the right-hand tree used to do this
        /// (in MouseDown); the left tree relied on NodeMouseClick and never selected the node,
        /// so the menu acted on whatever was selected before the right-click and the tree
        /// showed no indication of which node the menu belonged to.
        /// </summary>
        private void TreeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            TreeNode node = treeView1.GetNodeAt(e.X, e.Y);

            // Right-clicking the empty space under the tree must not leave the previous node
            // remembered, so the state is cleared rather than left stale.
            if (!TryGetRightClicked(node, _listTree1, out ClassTree clsTree))
            {
                _rightSelectNode = null;
                _clsTree1 = null;
                lengthToolStripMenuItem.Text = _lengthLabel;
                return;
            }

            treeView1.SelectedNode = node;
            _rightSelectNode = node;
            _clsTree1 = clsTree;
            lengthToolStripMenuItem.Text = clsTree.Length;
        }

        /// <summary>
        /// True when <paramref name="node"/> is a real node of <paramref name="list"/>;
        /// false for null, for the expander placeholder, and for anything the model no
        /// longer knows about.
        /// </summary>
        private bool TryGetRightClicked(TreeNode node, List<ClassTree> list, out ClassTree clsTree)
        {
            clsTree = null;
            return node != null
                && !TreeLib.IsPlaceholder(node)
                && _treeLib.TryGetTreeByID(TreeLib.GetNodeCode(node), list, out clsTree);
        }

        private void TreeView1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data?.GetDataPresent("System.Windows.Forms.TreeNode", false) == true
                ? DragDropEffects.Move
                : DragDropEffects.None;
        }

        #endregion

        #region Right pane: the compare tree

        private void TreeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _treeLib.TV1Select(treeView2, _listTree2);
        }

        private void TreeView2_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            _treeLib.BeforeExpand(e.Node, _listTree2);
        }

        private void TreeView2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data?.GetDataPresent("System.Windows.Forms.TreeNode", false) == true
                ? DragDropEffects.Move
                : DragDropEffects.None;
        }

        private void TreeView2_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void TreeView2_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void TreeView2_DragDrop(object sender, DragEventArgs e)
        {
            // Dragging a node *from the left tree* onto the right tree adds it.
            if (e.Data?.GetDataPresent("System.Windows.Forms.TreeNode", false) != true)
            {
                return;
            }

            if (treeView1.SelectedNode == null)
            {
                return;
            }

            TreeNode target = treeView2.GetNodeAt(treeView2.PointToClient(new Point(e.X, e.Y)));
            if (target == null)
            {
                return;
            }

            AddSubtreeToRightTree(target, treeView1.SelectedNode);
        }

        /// <summary>
        /// Right-hand tree equivalent of <see cref="TreeView1_MouseDown"/>. The "Length"
        /// caption is filled in here too, so the whole context menu is driven from one event.
        /// </summary>
        private void TreeView2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            TreeNode node = treeView2.GetNodeAt(e.X, e.Y);

            if (!TryGetRightClicked(node, _listTree2, out ClassTree clsTree))
            {
                _clsTree2 = null;
                cLengthToolStripMenuItem.Text = _lengthLabel;
                return;
            }

            treeView2.SelectedNode = node;
            _clsTree2 = clsTree;
            cLengthToolStripMenuItem.Text = clsTree.Length;
        }

        private void AddMenu_Click(object sender, EventArgs e)
        {
            if (treeView2.SelectedNode == null || treeView1.SelectedNode == null)
            {
                return;
            }

            AddSubtreeToRightTree(treeView2.SelectedNode, treeView1.SelectedNode);
        }

        private void AddSubtreeToRightTree(TreeNode target, TreeNode source)
        {
            if (_listTree1 == null || _listTree2 == null || target == null || source == null)
            {
                return;
            }

            _treeLib.CopySubtree(
                _listTree1,
                TreeLib.GetNodeCode(target),
                TreeLib.GetNodeCode(source),
                _listTree2,
                ref _maxNodeCode);

            var added = new TreeNode(source.Text) { Tag = _maxNodeCode };
            target.Nodes.Add(added);

            // The node was appended by hand rather than by AddTreeView, so give it its own
            // "+" if the copied subtree root is a folder with children.
            _treeLib.RefreshExpander(added, _listTree2);
            ResetSearch();
        }

        private void RemoveMenu_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView2.SelectedNode;
            if (node == null || _listTree2 == null)
            {
                return;
            }

            int code = TreeLib.GetNodeCode(node);

            _treeLib.RemoveTreeNode(code, _listTree2);
            node.Remove();
            ResetSearch();
        }

        private void CmdSave2_Click(object sender, EventArgs e)
        {
            if (_listTree2 == null)
            {
                return;
            }

            ConfigureSaveDialog("XML file|*.xml", "*.xml");
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            WriteXmlSafe(_listTree2, saveFileDialog1.FileName);
        }

        private void CmdText2_Click(object sender, EventArgs e)
        {
            if (_listTree2 == null || _listTree2.Count == 0)
            {
                return;
            }

            if (!AskTextExportOptions(out bool numberNames))
            {
                return;
            }

            ConfigureSaveDialog("Text file|*.txt", "*.txt");
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            _listTree2.Sort(new ClassTreeComparer2());
            WriteTextSafe(_listTree2, saveFileDialog1.FileName, numberNames);
        }

        #endregion

        #region Search

        private void ResetSearch()
        {
            _isNewSearch = true;
            _times = 0;
            _searchList = null;
        }

        private void CmdSearch_Click(object sender, EventArgs e)
        {
            if (txtNodeSearch.Text.Trim().Length <= 2)
            {
                return;
            }

            SearchPrepare();
        }

        private void TxtNodeSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SearchPrepare();
            }
        }

        private void TxtNodeSearch_TextChanged(object sender, EventArgs e)
        {
            _isNewSearch = true;
        }

        /// <summary>
        /// Steps to the next match. Pressing Enter repeatedly walks the result set; the
        /// original kept this counter in a field that was never reset when the underlying
        /// tree changed, so it ran off the end of the result list.
        /// </summary>
        public void SearchPrepare()
        {
            List<ClassTree> list = chbTree1.Checked ? _listTree1 : _listTree2;
            TreeView tv = chbTree1.Checked ? treeView1 : treeView2;

            if (list == null || tv == null || tv.Nodes.Count == 0)
            {
                return;
            }

            if (_isNewSearch || _searchList == null)
            {
                _searchList = CreateSearchList(list);
                if (_searchList.Count == 0)
                {
                    string message = _clsLanguage?.GetMessage("028");
                    MessageBox.Show(this,
                        string.IsNullOrEmpty(message) ? "No matches found." : message,
                        "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _times = 0;
                _isNewSearch = false;
            }

            if (_times >= _searchList.Count)
            {
                _times = 0;
            }

            _treeLib.DoSearch(_searchList[_times], tv, list);
            _times++;
        }

        public List<int> CreateSearchList(List<ClassTree> list)
        {
            var matches = new List<int>();
            string raw = txtNodeSearch.Text;

            // A quoted term means "match whole words only".
            string needle = raw.Replace("\"", string.Empty).ToLowerInvariant();
            bool exact = raw.IndexOf('"') >= 0;

            foreach (ClassTree node in list)
            {
                string name = node.NodeName?.ToLowerInvariant() ?? string.Empty;

                bool hit = exact
                    ? _treeLib.IsExactlyMap(name, needle)
                    : _treeLib.IsNormalMap(name, needle);

                if (hit)
                {
                    matches.Add(node.NodeCode);
                }
            }

            return matches;
        }

        #endregion

        #region Menu commands

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var frmOptions = new FrmOptions();
            if (frmOptions.ShowDialog(this) == DialogResult.OK)
            {
                LoadOptions();
                ApplyTheme();
            }
        }

        private void ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var frmColor = new FrmColor();
            if (frmColor.ShowDialog(this) == DialogResult.OK)
            {
                ApplyTheme();
            }
        }

        private void CompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_listTree1 == null || _listTree2 == null)
            {
                return;
            }

            _frmCompare = Reusable(_frmCompare, () => new FrmCompare { Owner = this });
            _frmCompare.List = _listTree1;
            _frmCompare.List2 = _listTree2;

            if (!_frmCompare.Visible)
            {
                _frmCompare.Show();
            }

            _frmCompare.BringToFront();
        }

        private void DirectSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _frmDirectSearch = Reusable(_frmDirectSearch, () => new FrmDirectSearch { Owner = this });

            if (!_frmDirectSearch.Visible)
            {
                _frmDirectSearch.Show();
            }

            _frmDirectSearch.BringToFront();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _frmPictures = Reusable(_frmPictures, () => new FrmPictures { Owner = this });

            if (!_frmPictures.Visible)
            {
                _frmPictures.Show();
            }

            _frmPictures.BringToFront();
        }

        private void EventsMenu_Click(object sender, EventArgs e)
        {
            _frmEvents = Reusable(_frmEvents, () => new FrmEvents { Owner = this });

            if (!_frmEvents.Visible)
            {
                _frmEvents.Show();
            }

            _frmEvents.BringToFront();
        }

        private void ExecutionMenu_Click(object sender, EventArgs e)
        {
            if (_rightSelectNode == null
                || !_treeLib.TryGetTreeByID(TreeLib.GetNodeCode(_rightSelectNode), _listTree1, out ClassTree node))
            {
                return;
            }

            string url = node.OriginalUrl;

            // The tree holds both folders and files, so the shell target has to accept both.
            if (!CanOpenInShell(url))
            {
                MessageBox.Show(this, "This node no longer exists on disk.", "Open",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write($"Could not open '{url}'.", ex);
                MessageBox.Show(this, ex.Message, "Open failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NodeInfoMenu_Click(object sender, EventArgs e)
        {
            if (_rightSelectNode == null || _clsTree1 == null)
            {
                return;
            }

            // ShowDialog, not Show: the window was opened modeless inside a `using`, so it was
            // disposed the instant the handler returned and the item looked like it did nothing.
            // It is an editor - it has Save/Close, an AcceptButton and sets DialogResult - so it
            // is modal by design.
            using var frmNodeInfo = new FrmNodeInfo
            {
                Owner = this,
                CurrNode = _rightSelectNode,
                ClsTree = _clsTree1,
            };

            frmNodeInfo.ShowDialog(this);
        }

        private void RightNodeInfoMenu_Click(object sender, EventArgs e)
        {
            if (treeView2.SelectedNode == null || _clsTree2 == null)
            {
                return;
            }

            using var frmNodeInfo = new FrmNodeInfo
            {
                Owner = this,
                CurrNode = treeView2.SelectedNode,
                ClsTree = _clsTree2,
            };

            frmNodeInfo.ShowDialog(this);
        }

        private void SearchDetailMenu_Click(object sender, EventArgs e)
        {
            if (_listTree2 == null || treeView2.SelectedNode == null)
            {
                return;
            }

            if (!_treeLib.TryGetTreeByID(TreeLib.GetNodeCode(treeView2.SelectedNode), _listTree2, out ClassTree node))
            {
                return;
            }

            ShowSearchDetail(treeView2, _listTree2, node.NodeName, dockLeft: false);
        }

        private void SearchDetailMenu2_Click(object sender, EventArgs e)
        {
            if (_listTree1 == null)
            {
                return;
            }

            ShowSearchDetail(
                treeView1,
                _listTree1,
                _rightSelectNode?.Text,
                dockLeft: true);
        }

        private void ShowSearchDetail(TreeView tv, List<ClassTree> list, string nodeName, bool dockLeft)
        {
            _frmSearchDetail = Reusable(_frmSearchDetail, () => new FrmSearchDetail { Owner = this });
            _frmSearchDetail.Tv = tv;
            _frmSearchDetail.List = list;
            _frmSearchDetail.NodeName = nodeName;

            if (!_frmSearchDetail.Visible)
            {
                _frmSearchDetail.Show(this);
            }

            // Size to half the main window and park it on the matching edge.
            _frmSearchDetail.Width = Math.Max(320, Width / 2);
            _frmSearchDetail.Location = dockLeft
                ? new Point(this.Right - _frmSearchDetail.Width, this.Bottom - _frmSearchDetail.Height - 30)
                : new Point(this.Left, this.Bottom - _frmSearchDetail.Height - 30);

            _frmSearchDetail.BringToFront();
        }

        private void CompareUrlMenu_Click(object sender, EventArgs e)
        {
            if (treeView2.SelectedNode == null || _listTree2 == null)
            {
                return;
            }

            if (!_treeLib.TryGetTreeByID(TreeLib.GetNodeCode(treeView2.SelectedNode), _listTree2, out ClassTree node))
            {
                return;
            }

            using var frm = new FrmSetCompareUrl { Owner = this, ClsTree = node };
            if (frm.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            _oldDir = frm.OldDirectory;
            _newDir = frm.NewDirectory;

            int changed = ReplaceCompareUrl(node.NodeCode, _listTree2, _oldDir, _newDir);
            _log($"Updated {changed:N0} compare URLs.");
        }

        private void CmdClose_Click(object sender, EventArgs e)
        {
            _scanCts?.Cancel();
            Close();
        }

        #endregion

        #region Compare-URL rewriting

        /// <summary>
        /// Rewrites <see cref="ClassTree.CompareUrl"/> for a node and everything under it.
        /// The original method had an <em>empty body</em> (and a working helper,
        /// <c>SetCompareUrl2</c>, that nothing called), so the "Set compare URL" menu item
        /// silently did nothing.
        /// </summary>
        private int ReplaceCompareUrl(int nodeCode, List<ClassTree> list, string oldDir, string newDir)
        {
            if (list == null || string.IsNullOrEmpty(oldDir))
            {
                return 0;
            }

            int changed = 0;

            foreach (ClassTree node in list)
            {
                if (node.NodeCode == nodeCode)
                {
                    node.CompareUrl = (node.CompareUrl ?? string.Empty).Replace(oldDir, newDir);
                    changed++;
                }
            }

            foreach (ClassTree node in list)
            {
                if (node.ParentCode == nodeCode && node.NodeCode != node.ParentCode)
                {
                    changed += ReplaceCompareUrl(node.NodeCode, list, oldDir, newDir);
                }
            }

            return changed;
        }

        #endregion

        #region Form lifecycle and layout

        private void FolderInfo_Load(object sender, EventArgs e)
        {
            LoadOptions();

            removeToolStripMenuItem.Text = "Remove";
            toolStripStatusLabel.Text = "Ready";
            toolStripStatusLabel2.Text = string.Empty;

            ApplyTheme();
            LoadMediaUserControl();

            // The Resize event also fires while the handle is being created, i.e. before the
            // form has its final size, so the first layout it produced was stale and left the
            // two top text boxes overlapping. Laying out once more here, after the language
            // has sized the auto-sized title label, fixes the first paint.
            LayoutPanes();
        }

        private void ApplyTheme()
        {
            ClassData.SelectNodeColor = GetColor("MauNodeSelect");
            ClassData.NodeColor = GetColor("MauChuTV");
            SetColor();
        }

        private void LoadMediaUserControl()
        {
            try
            {
                _userMediaUC = new MediaUserControl();
                _userMediaUC.LoadData();
                _userMediaUC.Dock = DockStyle.Fill;

                if (_userMediaUC.Index >= 0)
                {
                    treeView2.Controls.Add(_userMediaUC);
                }
                else
                {
                    _userMediaUC.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write("Could not initialise the event banner.", ex);
            }
        }

        private void FolderInfo_Resize(object sender, EventArgs e)
        {
            LayoutPanes();
        }

        /// <summary>
        /// Lays the whole form out from <see cref="ClientSize"/>. This is the only thing that
        /// decides where a control goes: the per-control anchors have been removed, because
        /// the two systems used to fight. The anchor pass kept the design-time gap between
        /// txtAddress and txtNodeSearch while this method stretched txtAddress across the
        /// left pane, so on first open the two boxes overlapped by 134 px and sat 2 px out of
        /// line; maximising happened to produce a width where they lined up again, which is
        /// why the bug looked intermittent.
        ///
        /// The original subtracted magic numbers from <c>Form.Height</c> and set a status
        /// label's width to <c>Width - 442</c>, which went negative (and threw) on a small
        /// window; everything here is derived from the real client area instead.
        /// </summary>
        private void LayoutPanes()
        {
            const int Gap = 16;
            const int Margin = 12;
            const int ButtonGap = 2;

            int right = ClientSize.Width - Margin;
            int paneWidth = Math.Max(120, (ClientSize.Width - (Margin * 2) - Gap) / 2);
            int secondPane = Margin + paneWidth + Gap;
            int secondRight = Math.Min(right, secondPane + paneWidth);

            // Two rows of controls, both positioned off the controls that are already
            // measured, so a taller font (or a higher DPI) pushes row 2 down instead of
            // making row 1 overlap it.
            int searchTop = Math.Max(cmdSearch.Top, menuStrip1.Bottom + 8);
            int textTop = searchTop + Math.Max(0, (cmdSearch.Height - txtAddress.Height) / 2);
            int buttonTop = Math.Max(cmdLoad.Top, cmdSearch.Bottom + 4);
            int checkTop = buttonTop + ((cmdLoad.Height - chkWriteFile.Height) / 2);

            // Search row: address box, node search box, then the Search button on the right.
            cmdSearch.SetBounds(right - cmdSearch.Width, searchTop, cmdSearch.Width, cmdSearch.Height);
            txtNodeSearch.SetBounds(
                secondPane,
                textTop,
                Math.Max(40, cmdSearch.Left - secondPane - 4),
                txtNodeSearch.Height);
            txtAddress.SetBounds(Margin, textTop, paneWidth, txtAddress.Height);

            // Toolbar row, left to right.
            cmdLoad.SetBounds(Margin, buttonTop, cmdLoad.Width, cmdLoad.Height);
            cmdLoadXML.SetBounds(cmdLoad.Right + ButtonGap, buttonTop, cmdLoadXML.Width, cmdLoadXML.Height);
            cmdSave.SetBounds(cmdLoadXML.Right + ButtonGap, buttonTop, cmdSave.Width, cmdSave.Height);
            cmdText.SetBounds(cmdSave.Right + ButtonGap, buttonTop, cmdText.Width, cmdText.Height);
            chkWriteFile.SetBounds(cmdText.Right + 6, checkTop, chkWriteFile.Width, chkWriteFile.Height);

            // Toolbar row, right to left, anchored to the same right edge as the trees.
            cmdClose.SetBounds(secondRight - cmdClose.Width, buttonTop, cmdClose.Width, cmdClose.Height);
            cmdOpen.SetBounds(cmdClose.Left - ButtonGap - cmdOpen.Width, buttonTop, cmdOpen.Width, cmdOpen.Height);
            cmdText2.SetBounds(cmdOpen.Left - ButtonGap - cmdText2.Width, buttonTop, cmdText2.Width, cmdText2.Height);
            cmdSave2.SetBounds(cmdText2.Left - ButtonGap - cmdSave2.Width, buttonTop, cmdSave2.Width, cmdSave2.Height);
            chbTree1.SetBounds(secondPane, checkTop, chbTree1.Width, chbTree1.Height);

            // The checkboxes share the toolbar row with the buttons. On a window narrower than
            // the design width the left row (4 buttons + checkbox) is wider than a half pane,
            // so hide a checkbox rather than let it collide with the other pane's checkbox.
            chkWriteFile.Visible = chkWriteFile.Right <= secondPane - 4;
            chbTree1.Visible = chbTree1.Right <= secondRight - 4;

            int treeTop = cmdLoad.Bottom + 8;
            int treeHeight = Math.Max(120, statusStrip1.Top - treeTop - 8);
            treeView1.SetBounds(Margin, treeTop, paneWidth, treeHeight);
            treeView2.SetBounds(secondPane, treeTop, Math.Max(40, secondRight - secondPane), treeHeight);

            lblMyFolderInfo.SetBounds(
                (ClientSize.Width - lblMyFolderInfo.Width) / 2,
                lblMyFolderInfo.Top,
                lblMyFolderInfo.Width,
                lblMyFolderInfo.Height);

            toolStripStatusLabel2.Spring = true;
            toolStripStatusLabel2.TextAlign = ContentAlignment.MiddleRight;

            if (_userMediaUC is { IsDisposed: false })
            {
                _userMediaUC.Bounds = treeView2.ClientRectangle;
            }
        }

        #endregion

        #region Options / logging helpers

        private void LoadOptions()
        {
            // Replaced the unbounded Append to a static list.
            ClsOptions = OptionsStore.Load(ErrorLog.Current);

            ClassData.ApplyOptions(ClsOptions);

            _clsLanguage = new ClassLanguage(this, "frmMain", menuStrip1);

            ClassData.PleaseWait = _clsLanguage.GetMessage("018");
            if (ClassData.PleaseWait.Length == 0)
            {
                ClassData.PleaseWait = "Please wait...";
            }

            toolStripStatusLabel.Text = "Ready";
            _clsLanguage.SetContextMenuLanguage(contextMenuStrip1);
            _clsLanguage.SetContextMenuLanguage(contextMenuStrip2);

            // The "Length" caption is overwritten with the size of whatever node was
            // right-clicked, so keep the translated label to put back when a right-click
            // misses a node.
            string lengthCaption = _clsLanguage.GetMessage("025");
            _lengthLabel = string.IsNullOrWhiteSpace(lengthCaption) ? "Length" : lengthCaption;
            lengthToolStripMenuItem.Text = _lengthLabel;
            cLengthToolStripMenuItem.Text = _lengthLabel;

            try
            {
                _userMediaUC?.LoadEvent();
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write("Could not refresh the event banner.", ex);
            }
        }

        private void _log(string message)
        {
            toolStripStatusLabel.Text = message;
        }

        private void ConfigureSaveDialog(string filter, string defaultExtension)
        {
            saveFileDialog1.Filter = filter;
            saveFileDialog1.DefaultExt = defaultExtension.TrimStart('*');
            saveFileDialog1.FileName = string.Empty;
            saveFileDialog1.OverwritePrompt = true;
        }

        private void WriteXmlSafe(List<ClassTree> list, string fileName)
        {
            try
            {
                _treeLib.WriteXml(list, fileName);
                _log($"Saved {list.Count:N0} nodes to {Path.GetFileName(fileName)}.");
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write($"Failed to write {fileName}.", ex);
                MessageBox.Show(this, ex.Message, "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WriteTextSafe(List<ClassTree> list, string fileName, bool numberNames)
        {
            try
            {
                _treeLib.WriteText2(fileName, list, numberNames);
                _log($"Wrote {Path.GetFileName(fileName)}.");
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write($"Failed to write {fileName}.", ex);
                MessageBox.Show(this, ex.Message, "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _scanCts?.Cancel();
            _scanCts?.Dispose();
            _userMediaUC?.Dispose();
            base.OnFormClosed(e);
        }

        #endregion
    }
}
