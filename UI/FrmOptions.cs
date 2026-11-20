using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>Editor for Options.xml (language, sort order, level, image folder).</summary>
    /// <remarks>
    /// Fixes over the original:
    /// <list type="bullet">
    /// <item><description>it wrote option values by child <i>position</i>
    /// (<c>ChildNodes[2]</c>), so a reformat of the hand-edited file put values in the wrong
    /// element. It now goes through <see cref="OptionsStore"/>, which addresses elements by
    /// name and saves atomically;</description></item>
    /// <item><description>it persisted <c>cmbSort.SelectedIndex</c> /
    /// <c>cmbLevel.SelectedIndex</c> - indexes into <i>translated</i> display strings. Changing
    /// the UI language therefore silently changed what the stored number meant. The stable
    /// option value is stored now, and the combo is re-selected by value;</description></item>
    /// <item><description><c>Convert.ToInt32(ClassData.Load2level)</c> threw a
    /// <see cref="FormatException"/> on a hand-edited file, taking the app down on open;</description></item>
    /// <item><description><c>ChkDir_CheckedChanged</c> hard-coded
    /// <c>Application.StartupPath + @"\Images"</c> instead of the shared image-folder path;</description></item>
    /// <item><description>the language list was every file in the Languages folder, including
    /// any stray readme or backup, and each selection was matched by raw index.</description></item>
    /// </list>
    /// </remarks>
    public partial class FrmOptions : ParentForm
    {
        private const string SortKey = "Sort_No";
        private const string LanguageKey = "Language";
        private const string LevelKey = "Load2level";
        private const string ImageDirKey = "Image_Dir";

        private ClassLanguage _clsLan;
        private string _currentLanguage;
        private string _currentLevel;

        public FrmOptions()
        {
            InitializeComponent();
        }

        private void FrmOptions_Load(object sender, EventArgs e)
        {
            _clsLan = new ClassLanguage(this, "frmOption");

            _currentLanguage = GetOption(LanguageKey);
            _currentLevel = GetOption(LevelKey);

            LoadLanguageList();
            LoadSortList();
            LoadLevelList();
        }

        #region Populate

        private static string GetOption(string key)
        {
            return FolderInfo.ClsOptions?
                .FirstOrDefault(o => o.ThuocTinh == key)?
                .GiaTri ?? string.Empty;
        }

        private void LoadLanguageList()
        {
            cmbLanguage.Items.Clear();

            if (!Directory.Exists(AppPaths.LanguageFolder))
            {
                return;
            }

            // *.xml only - the old code listed every file in the folder.
            foreach (string path in Directory.GetFiles(AppPaths.LanguageFolder, "*.xml").OrderBy(x => x))
            {
                cmbLanguage.Items.Add(Path.GetFileName(path));
            }

            SelectByValue(cmbLanguage, _currentLanguage);
        }

        private void LoadSortList()
        {
            cmbSort.Items.Clear();

            foreach (string label in Split(_clsLan.GetMessage("003")))
            {
                cmbSort.Items.Add(label.Trim());
            }

            // The stored value is the stable "0 = by name, 1 = by extension" flag, not a
            // position in this translated list.
            SelectByValue(cmbSort, ClassData.SortNo);
        }

        private void LoadLevelList()
        {
            cmbLevel.Items.Clear();

            foreach (string label in Split(_clsLan.GetMessage("008")))
            {
                cmbLevel.Items.Add(label.Trim());
            }

            // Level entries are the numbers themselves, so select the item that *is* the
            // stored level rather than assuming the index matches.
            SelectByValue(cmbLevel, _currentLevel);
        }

        private static string[] Split(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return Array.Empty<string>();
            }

            return message.Split(',');
        }

        private static void SelectByValue(ComboBox combo, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (string.Equals(combo.Items[i]?.ToString(), value, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        #endregion

        #region Save

        private void CmdSave_Click(object sender, EventArgs e)
        {
            string language = cmbLanguage.Text.Trim();
            string sortNo = (cmbSort.SelectedIndex >= 0 ? cmbSort.SelectedIndex : 0)
                .ToString(CultureInfo.InvariantCulture);
            string level = (cmbLevel.SelectedIndex >= 0 ? cmbLevel.SelectedIndex : 0)
                .ToString(CultureInfo.InvariantCulture);
            string imageDir = txtImageDir.Text.Trim();

            var values = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [LanguageKey] = language,
                [SortKey] = sortNo,
                [LevelKey] = level,
                [ImageDirKey] = imageDir,
            };

            if (!OptionsStore.Save(values, ErrorLog.Current))
            {
                MessageBox.Show(this,
                    "Options.xml could not be read, so it has not been overwritten.\r\n\r\n" +
                    "Fix or restore the file, then try again.",
                    "Save refused", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Update the in-memory copies too, so the change takes effect without a restart.
            ClassData.ApplyOptions(values
                .Select(pair => new ClassOptions(pair.Key, string.Empty, pair.Value)));

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CmdCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CmdBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                txtImageDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void ChkDir_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkDir.Checked)
            {
                // Was Application.StartupPath + @"\Images", which diverged from the path the
                // rest of the app resolves.
                txtImageDir.Text = AppPaths.ImageFolder;
            }
        }

        #endregion
    }
}
