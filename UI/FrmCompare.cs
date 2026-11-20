using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>
    /// Compares two trees by their <see cref="ClassTree.CompareUrl"/>, highlighting files that
    /// are newer (green), older (red) or missing on the other side (khaki).
    /// </summary>
    public partial class FrmCompare : ParentForm
    {
        /// <summary>Snapshots of the caller's lists. The original sorted the caller's live
        /// lists in Load, so merely opening this window reordered the main form's data.</summary>
        private List<ClassTree> _left;
        private List<ClassTree> _right;

        private int _id;
        private int _id2;
        private ListViewColumnSorter _sorterLeft;
        private ListViewColumnSorter _sorterRight;
        private ClassLanguage _clsLanguage;

        public List<ClassTree> List { get; set; }

        public List<ClassTree> List2 { get; set; }

        public FrmCompare()
        {
            InitializeComponent();
        }

        private void FrmCompare_Load(object sender, EventArgs e)
        {
            _clsLanguage = new ClassLanguage(this, "frmCompare");
            _clsLanguage.SetControlLanguage(panel1);

            columnHeader1.Text = _clsLanguage.GetMessage("005");
            columnHeader2.Text = _clsLanguage.GetMessage("006");
            columnHeader3.Text = _clsLanguage.GetMessage("007");
            columnHeader7.Text = _clsLanguage.GetMessage("008");
            columnHeader9.Text = _clsLanguage.GetMessage("013");

            columnHeader4.Text = _clsLanguage.GetMessage("005");
            columnHeader5.Text = _clsLanguage.GetMessage("006");
            columnHeader6.Text = _clsLanguage.GetMessage("007");
            columnHeader8.Text = _clsLanguage.GetMessage("008");
            columnHeader10.Text = _clsLanguage.GetMessage("013");

            RefreshSnapshots();

            _sorterLeft = new ListViewColumnSorter();
            listView1.ListViewItemSorter = _sorterLeft;

            _sorterRight = new ListViewColumnSorter();
            listView2.ListViewItemSorter = _sorterRight;
        }

        /// <summary>Re-reads the caller's lists into private, stably ordered copies.</summary>
        public void RefreshSnapshots()
        {
            _left = List is null
                ? new List<ClassTree>()
                : List.OrderBy(x => x.OriginalUrl, StringComparer.CurrentCultureIgnoreCase).ToList();

            _right = List2 is null
                ? new List<ClassTree>()
                : List2.OrderBy(x => x.OriginalUrl, StringComparer.CurrentCultureIgnoreCase).ToList();
        }

        #region SQL menu integration

        private string GetAllExeNames()
        {
            var names = new List<string>();

            foreach (ListViewItem item in listView1.Items)
            {
                string url = item.SubItems.Count > 1 ? item.SubItems[1].Text : string.Empty;
                string exe = ExeReturn(url);

                // Was exe.IndexOf(".exe") > 0 - case sensitive, so ".EXE" was skipped, and
                // "IndexOf > 0" also rejected a value that *starts* with ".exe".
                if (exe.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    names.Add(exe);
                }
            }

            return string.Join(", ", names.Distinct().Select(x => "'" + x.Replace("'", "''") + "'"));
        }

        private static string ExeReturn(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            string normalised = url.Replace('\\', '/');
            string[] parts = normalised.Split('/');
            return parts[^1].Trim().Replace("}", string.Empty);
        }

        private void CmdMenuClick(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0 && listView2.Items.Count == 0)
            {
                return;
            }

            string exes = GetAllExeNames();
            if (exes.Length == 0)
            {
                MessageBox.Show(this, "No executables found in the comparison results.",
                    "Menu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var frmConnection = new FrmConnection { Owner = this };
            if (frmConnection.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            using var frmMenu = new FrmMenu
            {
                Owner = this,
                Exes = exes,
                Conn = frmConnection.Conn,
            };

            frmMenu.Show(this);
        }

        #endregion

        #region Comparison

        private void List1CompareList2()
        {
            // 1 = newer (green), 2 = older (red), 3 = not present in the other tree (khaki).
            var rightIndex = BuildCompareUrlIndex(_right);

            foreach (ClassTree left in _left)
            {
                bool matched = rightIndex.TryGetValue(NormaliseUrl(left.CompareUrl), out ClassTree other);

                if (matched && CompareTimestamps(left.LastWriteHourTime, other.LastWriteHourTime) > 0)
                {
                    AddRow(listView1, ref _id, left, other, _clsLanguage.GetMessage("009"), Color.PaleTurquoise);
                }
                else if (matched && CompareTimestamps(left.LastWriteHourTime, other.LastWriteHourTime) < 0)
                {
                    AddRow(listView1, ref _id, left, other, _clsLanguage.GetMessage("010"), Color.IndianRed);
                }
                else if (!matched)
                {
                    // Only file nodes carry a meaningful CompareUrl; skip folders.
                    if (string.IsNullOrEmpty(left.CompareUrl))
                    {
                        continue;
                    }

                    AddRow(listView1, ref _id, left, null, _clsLanguage.GetMessage("011"), Color.Khaki);
                }
            }
        }

        private void List2CompareList1()
        {
            var leftIndex = BuildCompareUrlIndex(_left);

            foreach (ClassTree right in _right)
            {
                if (string.IsNullOrEmpty(right.CompareUrl))
                {
                    continue;
                }

                if (!leftIndex.ContainsKey(NormaliseUrl(right.CompareUrl)))
                {
                    AddRow(listView2, ref _id2, right, null, _clsLanguage.GetMessage("011"), Color.Khaki);
                }
            }
        }

        private static Dictionary<string, ClassTree> BuildCompareUrlIndex(List<ClassTree> list)
        {
            var index = new Dictionary<string, ClassTree>(StringComparer.Ordinal);

            foreach (ClassTree node in list)
            {
                string key = NormaliseUrl(node.CompareUrl);
                if (key.Length > 0)
                {
                    index[key] = node;
                }
            }

            return index;
        }

        private static string NormaliseUrl(string url) => (url ?? string.Empty).Trim().ToLowerInvariant();

        /// <summary>
        /// Compares two "yyyyMMdd HH:mm:ss" stamps. Was <c>LastWriteHourTime.CompareTo(...)</c>
        /// on a possibly-null string, which threw NullReferenceException for any scanned file
        /// (the scanner stored "" there) and compared a raw string rather than a date.
        /// </summary>
        private static int CompareTimestamps(string left, string right)
        {
            bool hasLeft = TryParseTimestamp(left, out DateTime leftDate);
            bool hasRight = TryParseTimestamp(right, out DateTime rightDate);

            if (!hasLeft && !hasRight)
            {
                return 0;
            }

            if (!hasLeft)
            {
                return -1;
            }

            if (!hasRight)
            {
                return 1;
            }

            return leftDate.CompareTo(rightDate);
        }

        private static bool TryParseTimestamp(string value, out DateTime result)
        {
            return DateTime.TryParseExact(
                value,
                new[] { "yyyyMMdd HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyyMMdd", "yyyy-MM-dd" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result);
        }

        private static void AddRow(
            ListView target,
            ref int counter,
            ClassTree node,
            ClassTree other,
            string verdict,
            Color backColor)
        {
            counter++;

            string when = other is null
                ? $" ({node.LastWriteTime} --- / / / )"
                : $" ({node.LastWriteTime} --- {other.LastWriteTime})";

            var item = new ListViewItem(counter.ToString(CultureInfo.CurrentCulture))
            {
                BackColor = backColor,
            };

            item.SubItems.Add(node.NodeName);
            item.SubItems.Add(node.OriginalUrl);
            item.SubItems.Add(verdict);
            item.SubItems.Add(when);

            target.Items.Add(item);
        }

        private void CmdGoClick(object sender, EventArgs e)
        {
            RefreshSnapshots();

            // The old code reset the row counters but never cleared the views, so every click
            // appended a fresh set of duplicate rows.
            listView1.Items.Clear();
            listView2.Items.Clear();
            _id = 0;
            _id2 = 0;

            List1CompareList2();
            List2CompareList1();

            if (listView1.Items.Count == 0 && listView2.Items.Count == 0)
            {
                MessageBox.Show(this, _clsLanguage.GetMessage("012"), "Compare",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        private void CmdCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void CmdReportClick(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text file|*.txt";
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.FileName = string.Empty;

            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                // "using" so the file is flushed and closed even if a row is malformed.
                // Previously an exception from the out-of-range SubItems[4] left a truncated file.
                using var writer = new StreamWriter(saveFileDialog1.FileName, append: false, Encoding.UTF8);

                WriteSection(writer, "..............#1...........", listView1);
                writer.WriteLine();
                WriteSection(writer, "..............#2...........", listView2);
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write("Failed to write the comparison report.", ex);
                MessageBox.Show(this, ex.Message, "Report failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void WriteSection(StreamWriter writer, string header, ListView view)
        {
            writer.WriteLine(header);

            for (int i = 0; i < view.Items.Count; i++)
            {
                ListViewItem item = view.Items[i];

                string name = Sub(item, 0);
                string url = Sub(item, 1);
                string verdict = Sub(item, 2);
                string when = Sub(item, 3);

                writer.WriteLine($"{i + 1}. {verdict} {name} {url}{when}");
            }
        }

        private static string Sub(ListViewItem item, int index) =>
            item.SubItems.Count > index ? item.SubItems[index].Text : string.Empty;

        #region Drag and drop

        private static void Lv_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true
                ? DragDropEffects.All
                : DragDropEffects.None;
        }

        private static void Lv_MouseDown(object sender, MouseEventArgs e, ListView lv)
        {
            if (e.Button != MouseButtons.Left || lv.SelectedItems.Count == 0)
            {
                return;
            }

            var paths = new string[lv.SelectedItems.Count];
            for (int i = 0; i < lv.SelectedItems.Count; i++)
            {
                // Rows carry NodeName, OriginalUrl, verdict, dates. The path is sub-item 1;
                // the old code read sub-item 2, which is the verdict text.
                paths[i] = Sub(lv.SelectedItems[i], 1);
            }

            var data = new DataObject(DataFormats.FileDrop, paths);
            lv.DoDragDrop(data, DragDropEffects.Copy);
        }

        private static void Lv_DragDrop(object sender, DragEventArgs e, ListView lv)
        {
            if (e.Data?.GetData(DataFormats.FileDrop, true) is not string[] dropped)
            {
                return;
            }

            foreach (string path in dropped)
            {
                lv.Items.Add(path);
            }
        }

        private static void Lv_ColumnClick(object sender, ColumnClickEventArgs e, ListView lv, ListViewColumnSorter sorter)
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

        private void ListView1_MouseDown(object sender, MouseEventArgs e) => Lv_MouseDown(sender, e, listView1);

        private void ListView1_DragEnter(object sender, DragEventArgs e) => Lv_DragEnter(sender, e);

        private void ListView1_DragDrop(object sender, DragEventArgs e) => Lv_DragDrop(sender, e, listView1);

        private void ListView2_MouseDown(object sender, MouseEventArgs e) => Lv_MouseDown(sender, e, listView2);

        private void ListView2_DragEnter(object sender, DragEventArgs e) => Lv_DragEnter(sender, e);

        private void ListView2_DragDrop(object sender, DragEventArgs e) => Lv_DragDrop(sender, e, listView2);

        private void ListView1_ColumnClick(object sender, ColumnClickEventArgs e) => Lv_ColumnClick(sender, e, listView1, _sorterLeft);

        private void ListView2_ColumnClick(object sender, ColumnClickEventArgs e) => Lv_ColumnClick(sender, e, listView2, _sorterRight);

        #endregion
    }
}
