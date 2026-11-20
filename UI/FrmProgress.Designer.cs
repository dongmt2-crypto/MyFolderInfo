namespace MyFolderInfo.UI
{
    partial class FrmProgress
    {
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblCurrentFolder = new System.Windows.Forms.Label();
            this.lblNodeCount = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(18, 17);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Scanning...";
            //
            // lblCurrentFolder
            //
            this.lblCurrentFolder.AutoSize = false;
            this.lblCurrentFolder.Location = new System.Drawing.Point(20, 96);
            this.lblCurrentFolder.Name = "lblCurrentFolder";
            this.lblCurrentFolder.Size = new System.Drawing.Size(424, 32);
            this.lblCurrentFolder.TabIndex = 3;
            this.lblCurrentFolder.Text = "";
            //
            // lblNodeCount
            //
            this.lblNodeCount.AutoSize = true;
            this.lblNodeCount.Location = new System.Drawing.Point(20, 44);
            this.lblNodeCount.Name = "lblNodeCount";
            this.lblNodeCount.Size = new System.Drawing.Size(110, 15);
            this.lblNodeCount.TabIndex = 1;
            this.lblNodeCount.Text = "0 nodes";
            //
            // progressBar1
            //
            this.progressBar1.Location = new System.Drawing.Point(20, 66);
            this.progressBar1.Name = "progressBar1";
            // The node total is not known up front, so this is indeterminate rather than a
            // bar that sits at zero and looks hung.
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.MarqueeAnimationSpeed = 30;
            this.progressBar1.Size = new System.Drawing.Size(424, 14);
            this.progressBar1.TabIndex = 2;
            //
            // cmdCancel
            //
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(368, 134);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(76, 23);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.CmdCancel_Click);
            //
            // FrmProgress
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(460, 168);
            this.ControlBox = false;
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.lblNodeCount);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblCurrentFolder);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmProgress";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "My Folder Info";
            this.Load += new System.EventHandler(this.FrmProgress_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblCurrentFolder;
        private System.Windows.Forms.Label lblNodeCount;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button cmdCancel;
    }
}
