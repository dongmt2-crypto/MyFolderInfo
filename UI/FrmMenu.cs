using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>Loads the start-menu tree for the executables found in a comparison.</summary>
    public partial class FrmMenu : ParentForm
    {
        private readonly ClassDataDb _db = new(ErrorLog.Current);
        private ClassLanguage _clsLan;

        public SqlConnection Conn { get; set; }

        /// <summary>Comma-separated, single-quoted, escaped list of executable file names.</summary>
        public string Exes { get; set; }

        public FrmMenu()
        {
            InitializeComponent();
        }

        private void FrmMenuLoad(object sender, EventArgs e)
        {
            _clsLan = new ClassLanguage(this, "frmMenu");
            _clsLan.SetControlLanguage(panel1);

            columnHeader1.Text = _clsLan.GetMessage("003");
            columnHeader2.Text = _clsLan.GetMessage("004");
            columnHeader3.Text = _clsLan.GetMessage("005");
            columnHeader4.Text = _clsLan.GetMessage("006");

            if (string.IsNullOrWhiteSpace(Exes))
            {
                MessageBox.Show(this, "No menu found.", "Menu",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (Conn is null)
            {
                MessageBox.Show(this, "No database connection.", "Menu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataSet ds;
            try
            {
                ds = _db.Query(BuildQuery(), Conn, "tc");
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write("Could not read the start menu from SQL Server.", ex);
                MessageBox.Show(this, ex.Message, "Menu",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!ds.Tables.Contains("tc"))
            {
                return;
            }

            for (int i = 0; i < ds.Tables["tc"].Rows.Count; i++)
            {
                DataRow row = ds.Tables["tc"].Rows[i];

                string bar = row["bar"].ToString();
                string exe = row["exe"].ToString();
                string full = row["bar0"].ToString();

                var item = new ListViewItem((i + 1).ToString(CultureInfo.CurrentCulture));
                item.SubItems.Add(bar);
                item.SubItems.Add(exe);
                item.SubItems.Add(full);

                // The view has four columns (No, Menu name, EXE, Menu cap1) but the old code
                // only ever added three sub-items, so the "Menu cap1" column stayed blank and
                // the report's SubItems[3] access threw ArgumentOutOfRangeException.
                if (full.Length > 0)
                {
                    item.SubItems.Add(full.Replace('\\', '\u2502'));
                }
                else
                {
                    item.SubItems.Add(string.Empty);
                }

                listView1.Items.Add(item);
            }
        }

        private string BuildQuery()
        {
            // Vietnamese uses the unaccented bar column, everything else uses bar2.
            string bar = ClassLanguage.StrLanguage.StartsWith("V", StringComparison.Ordinal)
                ? "bar"
                : "bar2";

            // Exes is already a quoted, comma-separated literal list built by FrmCompare.
            return $@"
select a.{bar} as bar,
       substring(a.exe, 1, charindex(' ', a.exe + ' ')) as exe,
       RTRIM(c.{bar}) + '\' + RTRIM(b.{bar}) as bar0
from command a
left join command b on a.menu_id0 = b.menu_id
left join command c on b.menu_id0 = c.menu_id
where substring(a.exe, 1, charindex(' ', a.exe + ' ')) in ({Exes})
order by a.menu_id";
        }

        private void CmdCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void CmdReportClick(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                // using, so a mid-write failure cannot leave a half-written report.
                using var writer = new StreamWriter(saveFileDialog1.FileName, append: false, Encoding.UTF8);

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    ListViewItem item = listView1.Items[i];

                    string name = Sub(item, 1);
                    string exe = Sub(item, 2);
                    string path = Sub(item, 3);

                    writer.WriteLine($"{i + 1}. {name} {exe} ({path})");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write("Failed to write the menu report.", ex);
                MessageBox.Show(this, ex.Message, "Report failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string Sub(ListViewItem item, int index) =>
            item.SubItems.Count > index ? item.SubItems[index].Text : string.Empty;
    }
}
