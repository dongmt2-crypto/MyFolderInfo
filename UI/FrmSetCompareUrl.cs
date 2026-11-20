using System;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>
    /// Rewrites the compare-URL prefix for a subtree of the right-hand tree.
    /// </summary>
    /// <remarks>
    /// The old <c>CmdOK_Click</c> only set <c>DialogResult = Yes</c>: it never copied the
    /// edited text back onto <see cref="ClsTree"/>, so pressing OK threw the user's change away
    /// and the dialog looked like it had worked. The old <c>FolderInfo.SetCompareUrl</c> that
    /// was supposed to apply the change had an empty body.
    /// </remarks>
    public partial class FrmSetCompareUrl : ParentForm
    {
        /// <summary>The node the rewrite starts at. Never null while the dialog is shown.</summary>
        public ClassTree ClsTree { get; set; }

        /// <summary>Prefix to replace; empty when OK is pressed without a change.</summary>
        public string OldDirectory => txtfCompareURL.Text.Trim();

        /// <summary>Replacement prefix.</summary>
        public string NewDirectory => txtNewDir.Text.Trim();

        public FrmSetCompareUrl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ClsTree == null)
            {
                // Used to dereference this in Load with no guard.
                MessageBox.Show(this, "No node was selected.", "Set compare URL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            txtnodename.Text = ClsTree.NodeName;
            txtfCompareURL.Text = ClsTree.CompareUrl;

            // Derive the "to" field from the selected node's own URL so the user starts
            // from something sensible instead of an empty box.
            if (string.IsNullOrEmpty(txtNewDir.Text) && !string.IsNullOrEmpty(ClsTree.OriginalUrl))
            {
                txtNewDir.Text = ClsTree.OriginalUrl;
            }

            txtfCompareURL.Select();
        }

        private void CmdOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(OldDirectory))
            {
                MessageBox.Show(this, "Enter the URL prefix to replace.", "Set compare URL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Keep the node itself in sync, then hand the two prefixes back to the caller.
            ClsTree.CompareUrl = (ClsTree.CompareUrl ?? string.Empty).Replace(OldDirectory, NewDirectory);
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
