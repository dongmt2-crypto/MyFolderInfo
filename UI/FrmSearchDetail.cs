using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>
    /// Flat search over a loaded tree: match on name and/or a size range, then reveal or open
    /// the selected result.
    /// </summary>
    public partial class FrmSearchDetail : ParentForm
    {
        public TreeView Tv { get; set; }

        public List<ClassTree> List { get; set; }

        /// <summary>
        /// Pre-fills the search box. This is a property rather than a field because a
        /// modeless window's Load runs only once: the caller re-opens the same instance every
        /// time the user picks "Search Detail" on a different node, and a plain field left the
        /// previously searched name in the box.
        /// </summary>
        public string NodeName
        {
            get => _nodeName;
            set
            {
                _nodeName = value ?? string.Empty;
                if (IsHandleCreated)
                {
                    txtNodeSearch.Text = _nodeName;
                }
            }
        }

        private string _nodeName = string.Empty;

        private readonly TreeLib _treeLib = new();
        private bool _isNewSearch = true;
        private List<int> _searchList;
        private ClassLanguage _clsLan;
        private ListViewColumnSorter _lvwColumnSorter;

        public FrmSearchDetail()
        {
            InitializeComponent();
            _treeLib.AttachLog(ErrorLog.Current);
        }

        private void FrmSearchDetail_Load(object sender, EventArgs e)
        {
            txtNodeSearch.Text = NodeName;

            _clsLan = new ClassLanguage(this, "frmSearchDetail");
            _clsLan.SetControlLanguage(panel1);
            _clsLan.SetContextMenuLanguage(contextMenuStrip1);

            columnHeader1.Text = _clsLan.GetMessage("007");
            columnHeader2.Text = _clsLan.GetMessage("008");
            columnHeader3.Text = _clsLan.GetMessage("010");

            _lvwColumnSorter = new ListViewColumnSorter();
            listView1.ListViewItemSorter = _lvwColumnSorter;

            ResizeColumns();
        }

        private void FrmSearchDetail_Resize(object sender, EventArgs e) => ResizeColumns();

        /// <summary>
        /// The third column used to be sized once, at Load, with a hard-coded "Width - header -
        /// 20" expression, so it never matched the window again.
        /// </summary>
        private void ResizeColumns()
        {
            int available = listView1.ClientSize.Width - 4;
            if (available <= 0)
            {
                return;
            }

            columnHeader1.Width = Math.Max(60, (int)(available * 0.45));
            columnHeader2.Width = Math.Max(60, (int)(available * 0.35));
            columnHeader3.Width = Math.Max(40, available - columnHeader1.Width - columnHeader2.Width);
        }

        #region Search

        private void CmdSearch_Click(object sender, EventArgs e)
        {
            if (txtNodeSearch.Text.Trim().Length <= 2
                && numericUpDown1.Value == decimal.Zero
                && numericUpDown2.Value == decimal.Zero)
            {
                return;
            }

            RunSearch();
        }

        private void TxtNodeSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                RunSearch();
            }
        }

        private void TxtNodeSearch_TextChanged(object sender, EventArgs e) => _isNewSearch = true;

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e) => _isNewSearch = true;

        private void NumericUpDown2_ValueChanged(object sender, EventArgs e) => _isNewSearch = true;

        private void RunSearch()
        {
            if (!_isNewSearch)
            {
                return;
            }

            if (List is null || List.Count == 0)
            {
                return;
            }

            _searchList = BuildResultList(List);
            _isNewSearch = false;

            if (_searchList.Count == 0)
            {
                MessageBox.Show(this, _clsLan.GetMessage("009"), "Search",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            listView1.Items.Clear();

            Color backA = GetColor("MauNenLV");
            Color backB = GetColor("MauNenLV2");
            Color fore = GetColor("MauChuLV");

            for (int i = 0; i < _searchList.Count; i++)
            {
                if (!_treeLib.TryGetTreeByID(_searchList[i], List, out ClassTree node))
                {
                    continue;
                }

                var item = new ListViewItem((i + 1).ToString(CultureInfo.CurrentCulture))
                {
                    Tag = node.NodeCode,
                    BackColor = i % 2 == 0 ? backA : backB,
                    ForeColor = fore,
                };

                item.SubItems.Add(node.NodeName);
                item.SubItems.Add(node.OriginalUrl);
                item.SubItems.Add(node.Length);

                listView1.Items.Add(item);
            }
        }

        /// <summary>
        /// Builds the result set. The original evaluated the name filter and the size filter in
        /// two independent passes, so a node could be added twice, and the two branches had the
        /// size bounds the wrong way round relative to each other. It also called
        /// <c>Convert.ToInt32(Length)</c> on a free-form string loaded from XML, which threw
        /// <see cref="FormatException"/> and aborted the whole search.
        /// </summary>
        private List<int> BuildResultList(List<ClassTree> list)
        {
            var results = new List<int>();

            string raw = txtNodeSearch.Text;
            string needle = raw.Replace("\"", string.Empty).ToLowerInvariant();
            bool wholeWords = raw.IndexOf('"') >= 0;

            bool useName = needle.Length > 0;
            decimal minSize = numericUpDown1.Value;
            decimal maxSize = numericUpDown2.Value;
            bool useSize = minSize != decimal.Zero || maxSize != decimal.Zero;

            if (minSize > maxSize && maxSize != decimal.Zero)
            {
                (minSize, maxSize) = (maxSize, minSize);
            }

            foreach (ClassTree node in list)
            {
                if (node == null)
                {
                    continue;
                }

                if (useName)
                {
                    string name = node.NodeName?.ToLowerInvariant() ?? string.Empty;

                    bool nameHit = wholeWords
                        ? _treeLib.IsExactlyMap(name, needle)
                        : _treeLib.IsNormalMap(name, needle);

                    if (!nameHit)
                    {
                        continue;
                    }
                }

                if (useSize && !InSizeRange(node, minSize, maxSize))
                {
                    continue;
                }

                results.Add(node.NodeCode);
            }

            return results;
        }

        private static bool InSizeRange(ClassTree node, decimal minSize, decimal maxSize)
        {
            if (!long.TryParse(node.Length, NumberStyles.Integer, CultureInfo.InvariantCulture, out long size))
            {
                // Folders store "0" and hand-edited files can hold anything; treat an
                // unparseable size as "no size information" rather than throwing.
                return false;
            }

            if (minSize != decimal.Zero && size < (long)minSize)
            {
                return false;
            }

            if (maxSize != decimal.Zero && size > (long)maxSize)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Results actions

        private ClassTree SelectedNode()
        {
            if (List is null || listView1.SelectedIndices.Count == 0)
            {
                return null;
            }

            var tag = listView1.Items[listView1.SelectedIndices[0]].Tag;
            return tag is int code && _treeLib.TryGetTreeByID(code, List, out ClassTree node) ? node : null;
        }

        private void ShowInTreeViewMenu_Click(object sender, EventArgs e)
        {
            if (Tv is null || Tv.Nodes.Count == 0)
            {
                return;
            }

            ClassTree node = SelectedNode();
            if (node is null)
            {
                return;
            }

            // Was: a hard-coded Color.White, which both ignored the theme and cleared the
            // highlight even when nothing was selected.
            if (Tv.SelectedNode is not null)
            {
                Tv.SelectedNode.BackColor = Tv.BackColor;
            }

            _treeLib.DoSearch(node.NodeCode, Tv, List);
        }

        private void OpenMenu_Click(object sender, EventArgs e)
        {
            ClassTree node = SelectedNode();
            if (node is null || string.IsNullOrWhiteSpace(node.OriginalUrl))
            {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(node.OriginalUrl) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write($"Could not open '{node.OriginalUrl}'.", ex);
                MessageBox.Show(this, ex.Message, "Open failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Lv_ColumnClick(object sender, ColumnClickEventArgs e, ListView lv, ListViewColumnSorter sorter)
        {
            if (e.Column == sorter.SortColumn)
            {
                sorter.Order = sorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                sorter.SortColumn = e.Column;
                sorter.Order = SortOrder.Ascending;
            }

            lv.Sort();
        }

        private void ListView1_ColumnClick(object sender, ColumnClickEventArgs e) =>
            Lv_ColumnClick(sender, e, listView1, _lvwColumnSorter);

        #endregion

        #region Drag and drop

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

        private void ListView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || listView1.SelectedItems.Count == 0)
            {
                return;
            }

            var paths = new string[listView1.SelectedItems.Count];
            for (int i = 0; i < listView1.SelectedItems.Count; i++)
            {
                // Rows are (no., name, url, size) - the path is sub-item 2. The old code read
                // sub-item 2 on a list that only had 0 and 1, so dragging any row threw
                // ArgumentOutOfRangeException.
                ListViewItem item = listView1.SelectedItems[i];
                paths[i] = item.SubItems.Count > 2 ? item.SubItems[2].Text : string.Empty;
            }

            var data = new DataObject(DataFormats.FileDrop, paths);
            listView1.DoDragDrop(data, DragDropEffects.Copy);
        }

        #endregion
    }
}
