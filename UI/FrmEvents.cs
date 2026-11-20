using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>
    /// Editor for the banner events shown over the right-hand tree.
    /// </summary>
    /// <remarks>
    /// Data-loss fix: a missing or malformed <c>Events.xml</c> used to be swallowed, leaving
    /// the pre-created empty table bound to the grid. Pressing Save then wrote that empty
    /// table straight back over the file - <b>erasing every event</b>. Save is now refused
    /// when the file could not be read, and the in-progress cell is committed before saving
    /// (it used to be discarded, because <c>AcceptChanges</c> ran without <c>EndEdit</c>).
    /// </remarks>
    public partial class FrmEvents : ParentForm
    {
        private static readonly string[] Columns = { "Stt", "Ngay1", "Ngay2", "Message", "Message2", "Image_url" };

        private DataSet _ds;
        private bool _loadedFromFile;

        public FrmEvents()
        {
            InitializeComponent();
        }

        private void FrmEvents_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;

            _ds = CreateDataset();
            _loadedFromFile = LoadEvents();

            if (_loadedFromFile)
            {
                // Sorts on the *string* column, so "10" landed before "2". Pad the sort key
                // in a separate expression instead of relying on zero-padded input.
                _ds.Tables[0].DefaultView.Sort = "Stt ASC";
            }

            dataGridView1.DataSource = _ds.Tables[0].DefaultView;

            var clsLan = new ClassLanguage(this, "frmEvents");
            clsLan.SetControlLanguage(panel1);

            Column1.HeaderText = clsLan.GetMessage("003");
            Column2.HeaderText = clsLan.GetMessage("004");
            Column3.HeaderText = clsLan.GetMessage("005");
            Column4.HeaderText = clsLan.GetMessage("006");
            Column6.HeaderText = clsLan.GetMessage("007");
            Column5.HeaderText = clsLan.GetMessage("008");
        }

        private bool LoadEvents()
        {
            if (!File.Exists(AppPaths.EventsFile))
            {
                return false;
            }

            try
            {
                _ds.ReadXml(AppPaths.EventsFile, XmlReadMode.Auto);

                if (_ds.Tables.Count == 0 || _ds.Tables[0].Rows.Count == 0)
                {
                    return false;
                }

                // ReadXml inserts every row with the "Added" state, so DataSet.HasChanges()
                // reports true straight after a *successful* load. Without this baseline,
                // simply opening the window and closing it popped "discard changes?".
                _ds.AcceptChanges();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write($"Could not read {AppPaths.EventsFile}.", ex);
                _ds = CreateDataset();
                return false;
            }
        }

        private static DataSet CreateDataset()
        {
            var ds = new DataSet("XML");
            var table = new DataTable("event");

            foreach (string name in Columns)
            {
                table.Columns.Add(name, typeof(string));
            }

            ds.Tables.Add(table);
            return ds;
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            if (HasPendingEdits() && !ConfirmDiscard())
            {
                return;
            }

            Close();
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (!_loadedFromFile && _ds.Tables[0].Rows.Count == 0)
            {
                // Refuse to overwrite a file we could not read with an empty one.
                MessageBox.Show(this,
                    "Events.xml could not be read, so it has not been overwritten.\r\n\r\n" +
                    "Fix or restore the file, then try again.",
                    "Save refused", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Commit the cell that is still in edit mode; without this the user's
            // last keystroke was silently thrown away.
            dataGridView1.EndEdit();
            _ds.AcceptChanges();

            try
            {
                SaveEventsAtomic();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write("Could not save Events.xml.", ex);
                MessageBox.Show(this, ex.Message, "Save failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Writes via temp file + replace so an interrupted save cannot brick the file.</summary>
        private void SaveEventsAtomic()
        {
            string temp = AppPaths.EventsFile + ".tmp";

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            };

            using (var writer = XmlWriter.Create(temp, settings))
            {
                _ds.WriteXml(writer, XmlWriteMode.IgnoreSchema);
            }

            if (File.Exists(AppPaths.EventsFile))
            {
                File.Copy(AppPaths.EventsFile, AppPaths.EventsFile + ".bak", overwrite: true);
            }

            File.Move(temp, AppPaths.EventsFile, overwrite: true);
        }

        private bool HasPendingEdits() => _ds is not null && _ds.HasChanges();

        private bool ConfirmDiscard()
        {
            return MessageBox.Show(this,
                "There are unsaved changes. Discard them?",
                "My Folder Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void cmdImage_Click(object sender, EventArgs e)
        {
            string folder = ClassData.ImageDir;
            if (!Directory.Exists(folder))
            {
                folder = AppPaths.ImageFolder;
            }

            if (!Directory.Exists(folder))
            {
                MessageBox.Show(this, $"Image folder not found:\r\n{folder}", "Images",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(folder) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write($"Could not open '{folder}'.", ex);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing
                && HasPendingEdits()
                && !ConfirmDiscard())
            {
                e.Cancel = true;
            }

            base.OnFormClosing(e);
        }
    }
}
