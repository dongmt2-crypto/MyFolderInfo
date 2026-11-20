using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>
    /// Direct filesystem search (does not need a tree to be loaded first).
    /// </summary>
    /// <remarks>
    /// The original ran the scan on a raw <see cref="Thread"/> and had all of these problems:
    /// <list type="bullet">
    /// <item><description>it read <c>txtSearch.Text</c> and <c>txtPath.Text</c> from the
    /// background thread (illegal cross-thread access) and showed <c>MessageBox</c>s from it;</description></item>
    /// <item><description>the worker appended to a <see cref="List{T}"/> that the UI thread
    /// was simultaneously reading by index in a timer, so the count could grow between the
    /// bounds check and the access - a guaranteed <c>ArgumentOutOfRangeException</c>;</description></item>
    /// <item><description><c>Thread.Abort()</c> was called on the worker from inside itself,
    /// and only ever referenced the newest thread, so repeated searches leaked live threads;</description></item>
    /// <item><description>both timers were left permanently enabled;</description></item>
    /// <item><description>result rows had two sub-items but the code read sub-item 2, so
    /// "Open" silently did nothing and dragging a row threw;</description></item>
    /// <item><description><c>dir.GetDirectories()</c> sat outside the try/catch, so one
    /// access-denied subfolder killed the process mid-scan.</description></item>
    /// </list>
    /// This version is a cancellable <c>async</c> walk: the worker owns its own result list and
    /// hands completed chunks to the UI thread.
    /// </remarks>
    public partial class FrmDirectSearch : ParentForm
    {
        /// <summary>Result rows are flushed to the grid in batches of this size.</summary>
        private const int BatchSize = 200;

        private readonly TreeLib _treeLib = new();
        private ClassLanguage _clsLan;
        private Color _backRowA;
        private Color _backRowB;
        private Color _foreRow;

        private CancellationTokenSource _cts;
        private int _rowsShown;

        public FrmDirectSearch()
        {
            InitializeComponent();
            _treeLib.AttachLog(ErrorLog.Current);
        }

        private void FrmDirectSearch_Load(object sender, EventArgs e)
        {
            _clsLan = new ClassLanguage(this, "frmDirectSearch");
            _clsLan.SetControlLanguage(panel1);
            _clsLan.SetControlLanguage(panel2);

            columnHeader1.Text = _clsLan.GetMessage("010");
            columnHeader2.Text = _clsLan.GetMessage("011");
            columnHeader3.Text = _clsLan.GetMessage("012");

            _backRowA = GetColor("MauNenLV");
            _backRowB = GetColor("MauNenLV2");
            _foreRow = GetColor("MauChuLV");

            // The designer left both timers enabled; they ticked forever even when idle.
            timer1.Enabled = false;
            timer2.Enabled = false;
        }

        /// <summary>
        /// The designer wires both timers, but progress is now pushed from the worker instead
        /// of polled, so the ticks are deliberately inert. They stay disabled rather than being
        /// unwired so the timer controls keep compiling.
        /// </summary>
        private void Timer1_Tick(object sender, EventArgs e)
        {
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
        }

        #region Search

        private async void CmdSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (keyword.Length < 3)
            {
                lblPath2.Text = "Search keyword must be at least 3 characters.";
                return;
            }

            string root = PathProbe.Normalize(txtPath.Text);
            txtPath.Text = root;

            // Directory.Exists() returns false for every failure, so an unreachable mapped
            // network drive looked like a bad path. Report which one it actually is.
            PathVerdict verdict = PathProbe.Probe(root, out int win32Code);
            if (root.Length < 3 || verdict != PathVerdict.Ok)
            {
                ErrorLog.Current?.Write($"Cannot search '{root}': {verdict} (Win32 {win32Code}).");
                lblPath2.Text = verdict == PathVerdict.Ok
                    ? _clsLan.GetMessage("008")
                    : DescribeUnreadableFolder(root, verdict);
                return;
            }

            // Cancel any in-flight search before starting a new one.
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            bool wholeWords = txtSearch.Text.Contains('"');
            string needle = keyword.Replace("\"", string.Empty).ToLowerInvariant();
            bool includeFiles = chkWriteFile.Checked;

            listView1.Items.Clear();
            _rowsShown = 0;
            lblPath2.Text = string.Empty;
            cmdSearch.Enabled = false;
            UseWaitCursor = true;

            var batch = new List<ClassTree>(BatchSize);
            string currentFolder = root;
            int matches = 0;

            void Flush(bool force)
            {
                if (batch.Count == 0 || (!force && batch.Count < BatchSize))
                {
                    return;
                }

                // Snapshot and clear on the worker; only the UI thread touches the ListView.
                ClassTree[] pending = batch.ToArray();
                batch.Clear();

                void Apply()
                {
                    foreach (ClassTree node in pending)
                    {
                        var item = new ListViewItem((_rowsShown + 1).ToString(CultureInfo.CurrentCulture))
                        {
                            Tag = node.OriginalUrl,
                            BackColor = _rowsShown % 2 == 0 ? _backRowA : _backRowB,
                            ForeColor = _foreRow,
                        };

                        item.SubItems.Add(node.NodeName);
                        item.SubItems.Add(node.OriginalUrl);

                        listView1.Items.Add(item);
                        _rowsShown++;
                    }
                }

                if (IsDisposed || Disposing)
                {
                    return;
                }

                if (InvokeRequired)
                {
                    BeginInvoke((Action)Apply);
                }
                else
                {
                    Apply();
                }
            }

            try
            {
                await Task.Run(
                    () =>
                    {
                        Walk(
                            root,
                            needle,
                            wholeWords,
                            includeFiles,
                            node =>
                            {
                                matches++;
                                batch.Add(node);
                                Flush(force: false);
                            },
                            folder => currentFolder = folder,
                            token);
                    },
                    token);

                Flush(force: true);

                if (matches == 0)
                {
                    MessageBox.Show(this, _clsLan.GetMessage("009"), "Search",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblPath2.Text = $"{matches:N0} result(s)";
                }
            }
            catch (OperationCanceledException)
            {
                lblPath2.Text = "Search cancelled.";
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write("Direct search failed.", ex);
                MessageBox.Show(this, ex.Message, "Search failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cmdSearch.Enabled = true;
                UseWaitCursor = false;
            }
        }

        /// <summary>
        /// Iterative depth-first walk. Iterative rather than recursive so a pathological or
        /// symlink-y tree cannot blow the stack, and cancellation-aware at every level.
        /// </summary>
        private void Walk(
            string root,
            string needle,
            bool wholeWords,
            bool includeFiles,
            Action<ClassTree> onMatch,
            Action<string> onFolder,
            CancellationToken token)
        {
            var pending = new Stack<string>();
            pending.Push(root);

            var skips = new SkipReport(ErrorLog.Current);

            while (pending.Count > 0)
            {
                token.ThrowIfCancellationRequested();

                string current = pending.Pop();

                // The same malformed-name and disconnected-share faults the tree scanner
                // guards against apply here; a bad entry must not end the search.
                if (!GetMyFiles.TryCreateDirectory(current, out DirectoryInfo dir, out Exception dirError))
                {
                    skips.Folder(current, dirError);
                    continue;
                }

                onFolder(current);

                if (Matches(dir.Name, needle, wholeWords))
                {
                    onMatch(new ClassTree { NodeName = dir.Name, OriginalUrl = dir.FullName });
                }

                if (includeFiles)
                {
                    foreach (FileInfo file in GetMyFiles.GetFiles(dir.FullName, skips))
                    {
                        token.ThrowIfCancellationRequested();

                        if (Matches(file.Name, needle, wholeWords))
                        {
                            onMatch(new ClassTree { NodeName = file.Name, OriginalUrl = file.FullName });
                        }
                    }
                }

                // Push children in reverse so they pop in sorted order.
                List<DirectoryInfo> children = GetMyFiles.GetDirectories(dir.FullName, skips);
                children.Sort((a, b) => string.Compare(b.Name, a.Name, StringComparison.CurrentCultureIgnoreCase));

                foreach (DirectoryInfo child in children)
                {
                    pending.Push(child.FullName);
                }
            }

            skips.WriteSummary();
        }

        private bool Matches(string name, string needle, bool wholeWords)
        {
            string lower = name?.ToLowerInvariant() ?? string.Empty;

            return wholeWords
                ? _treeLib.IsExactlyMap(lower, needle)
                : _treeLib.IsNormalMap(lower, needle);
        }

        #endregion

        #region Results

        private string SelectedPath()
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return null;
            }

            ListViewItem item = listView1.SelectedItems[0];

            // Rows are (no., name, url) - the path is sub-item 2, which the old code never
            // populated, so Open did nothing and drag threw.
            if (item.SubItems.Count > 2 && item.SubItems[2].Text.Length > 0)
            {
                return item.SubItems[2].Text;
            }

            return item.Tag as string;
        }

        private void OpenMenu_Click(object sender, EventArgs e)
        {
            string path = SelectedPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write($"Could not open '{path}'.", ex);
                MessageBox.Show(this, ex.Message, "Open failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || listView1.SelectedItems.Count == 0)
            {
                return;
            }

            var paths = new string[listView1.SelectedItems.Count];
            for (int i = 0; i < listView1.SelectedItems.Count; i++)
            {
                ListViewItem item = listView1.SelectedItems[i];
                paths[i] = item.SubItems.Count > 2 ? item.SubItems[2].Text : item.Tag as string;
            }

            var data = new DataObject(DataFormats.FileDrop, paths);
            listView1.DoDragDrop(data, DragDropEffects.Copy);
        }

        private void ListView1_DragEnter(object sender, DragEventArgs e) => SetDropEffect(e);

        private static void SetDropEffect(DragEventArgs e)
        {
            e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true
                ? DragDropEffects.All
                : DragDropEffects.None;
        }

        private void ListView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop, true) is not string[] dropped)
            {
                return;
            }

            foreach (string path in dropped)
            {
                listView1.Items.Add(path);
            }
        }

        private void CmdLoad_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void CmdClose_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            Close();
        }

        #endregion

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            base.OnFormClosed(e);
        }
    }
}
