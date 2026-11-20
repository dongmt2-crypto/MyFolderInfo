using System;
using System.Drawing;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>
    /// Asks how deep the folder that is about to be scanned should be indexed.
    /// </summary>
    /// <remarks>
    /// The picked folder counts as level 1, so "3" means the picked folder plus two more
    /// generations. "All levels" ignores the number, and "Only that level" keeps just the
    /// picked folder and the folders at the chosen level.
    /// </remarks>
    public partial class FrmScanMode : ParentForm
    {
        private ClassLanguage _clsLan;

        /// <summary>Folder that is about to be scanned, shown to the user.</summary>
        public string FolderPath { get; set; }

        /// <summary>True when the user asked for an unlimited scan.</summary>
        public bool AllLevels { get; private set; }

        /// <summary>Number of generations to index, counting the picked folder as 1.</summary>
        public int Levels { get; private set; }

        /// <summary>True when only the folders at <see cref="Levels"/> are to be indexed.</summary>
        public bool OnlyDeepest { get; private set; }

        public FrmScanMode()
        {
            InitializeComponent();

            numLevels.Minimum = 1;
            numLevels.Maximum = 999;
            numLevels.Value = 3;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _clsLan = new ClassLanguage(this, "frmScanMode");

            lblPath.Text = FolderPath ?? string.Empty;
            SyncEnabledState();
        }

        private void ChkAllLevels_CheckedChanged(object sender, EventArgs e)
        {
            // "All levels" and "only the chosen level" cannot both apply.
            if (chkAllLevels.Checked)
            {
                chkOnlyDeepest.Checked = false;
            }

            SyncEnabledState();
        }

        private void ChkOnlyDeepest_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOnlyDeepest.Checked)
            {
                chkAllLevels.Checked = false;
            }

            SyncEnabledState();
        }

        private void SyncEnabledState()
        {
            bool limited = !chkAllLevels.Checked;
            numLevels.Enabled = limited;
            chkOnlyDeepest.Enabled = limited;
        }

        private void CmdOK_Click(object sender, EventArgs e)
        {
            AllLevels = chkAllLevels.Checked;
            Levels = (int)numLevels.Value;
            OnlyDeepest = chkOnlyDeepest.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
