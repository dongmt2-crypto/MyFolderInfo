using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>
    /// Modal progress dialog shown while the tree is being scanned.
    /// <para>
    /// The old version (<c>FrmWait</c>) polled a static counter from a
    /// <see cref="System.Windows.Forms.Timer"/> and was only ever shown from code that had
    /// been commented out, so scans ran on the UI thread with no feedback at all. This one is
    /// driven by real progress callbacks and can cancel a long scan.
    /// </para>
    /// </summary>
    internal partial class FrmProgress : ParentForm
    {
        private readonly string _title;

        public FrmProgress(string title = null)
        {
            InitializeComponent();
            _title = string.IsNullOrWhiteSpace(title) ? ClassData.PleaseWait : title;
        }

        /// <summary>Set to true to abort a scan in progress.</summary>
        public bool Cancelled { get; private set; }

        private void FrmProgress_Load(object sender, EventArgs e)
        {
            lblTitle.Text = _title;
        }

        /// <summary>Marshals a scan progress callback onto the UI thread.</summary>
        public void Report(int nodeCount, string currentFolder)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }

            void Apply()
            {
                lblNodeCount.Text = nodeCount.ToString("N0");
                lblCurrentFolder.Text = currentFolder ?? string.Empty;
            }

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke((Action)Apply);
                }
                catch (ObjectDisposedException)
                {
                    // The dialog closed while the scan was posting progress.
                }
            }
            else
            {
                Apply();
            }
        }

        private void CmdCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            lblTitle.Text = "Cancelling...";
            cmdCancel.Enabled = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Cancelled = true;
            }

            base.OnFormClosing(e);
        }
    }
}
