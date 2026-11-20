using System;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>
    /// Asks for the options used when the tree is written out as a text file.
    /// </summary>
    public partial class FrmTextExport : ParentForm
    {
        private ClassLanguage _clsLan;

        /// <summary>True when folder and file names should keep their sequence number.</summary>
        public bool NumberNames { get; private set; } = true;

        public FrmTextExport()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _clsLan = new ClassLanguage(this, "frmTextExport");
        }

        private void CmdOK_Click(object sender, EventArgs e)
        {
            NumberNames = !chkNoNumber.Checked;

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
