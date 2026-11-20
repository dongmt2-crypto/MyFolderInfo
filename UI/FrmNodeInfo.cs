using System;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>Viewer/editor for a single tree node's metadata.</summary>
    public partial class FrmNodeInfo : ParentForm
    {
        public ClassTree ClsTree { get; set; }

        public TreeNode CurrNode { get; set; }

        /// <summary>True when <see cref="CmdSave_Click"/> actually applied the edit.</summary>
        public bool Saved { get; private set; }

        public FrmNodeInfo()
        {
            InitializeComponent();
        }

        private void FrmNodeInfo_Load(object sender, EventArgs e)
        {
            if (ClsTree is null)
            {
                MessageBox.Show(this, "No node was selected.", "Node info",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            txtnodename.Text = ClsTree.NodeName;
            txtoriginalUrl.Text = ClsTree.OriginalUrl;
            txtfLength.Text = ClsTree.Length;
            txtfCreationTime.Text = ClsTree.CreationTime;
            txtfLastWriteTime.Text = ClsTree.LastWriteTime;
            txtfLastAccessTime.Text = ClsTree.LastAccessTime;
        }

        private void CmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CmdSave_Click(object sender, EventArgs e)
        {
            if (ClsTree is null)
            {
                // The old Save dereferenced ClsTree/CurrNode inside a try with an empty catch,
                // so it closed the dialog as if the edit had been saved.
                MessageBox.Show(this, "No node is loaded, so there is nothing to save.",
                    "Node info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ClsTree.NodeName = txtnodename.Text.Trim();
            ClsTree.OriginalUrl = txtoriginalUrl.Text.Trim();
            ClsTree.Length = txtfLength.Text.Trim();
            ClsTree.CreationTime = txtfCreationTime.Text.Trim();
            ClsTree.LastWriteTime = txtfLastWriteTime.Text.Trim();
            ClsTree.LastAccessTime = txtfLastAccessTime.Text.Trim();

            if (CurrNode is not null)
            {
                CurrNode.Text = ClsTree.NodeName;
            }

            Saved = true;
            DialogResult = DialogResult.OK;

            // The edit lives in memory only - the tree still has to be written out with
            // "Save XML", which the old dialog never told the user.
            MessageBox.Show(this,
                "Node updated. Use \"Save XML\" on the main window to keep the change.",
                "Node info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Close();
        }
    }
}
