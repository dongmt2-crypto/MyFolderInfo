namespace MyFolderInfo.UI
{
    partial class FrmScanMode
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblQuestion = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.lblLevels = new System.Windows.Forms.Label();
            this.numLevels = new System.Windows.Forms.NumericUpDown();
            this.chkAllLevels = new System.Windows.Forms.CheckBox();
            this.chkOnlyDeepest = new System.Windows.Forms.CheckBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numLevels)).BeginInit();
            this.SuspendLayout();
            // 
            // lblQuestion
            // 
            this.lblQuestion.Location = new System.Drawing.Point(12, 12);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(336, 32);
            this.lblQuestion.TabIndex = 0;
            this.lblQuestion.Tag = "L001";
            this.lblQuestion.Text = "How should this folder be scanned?";
            // 
            // lblPath
            // 
            this.lblPath.AutoEllipsis = true;
            this.lblPath.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPath.Location = new System.Drawing.Point(12, 48);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(336, 15);
            this.lblPath.TabIndex = 1;
            // 
            // lblLevels
            // 
            this.lblLevels.AutoSize = true;
            this.lblLevels.Location = new System.Drawing.Point(12, 78);
            this.lblLevels.Name = "lblLevels";
            this.lblLevels.Size = new System.Drawing.Size(90, 13);
            this.lblLevels.TabIndex = 2;
            this.lblLevels.Tag = "L003";
            this.lblLevels.Text = "Number of levels:";
            // 
            // numLevels
            // 
            this.numLevels.Location = new System.Drawing.Point(150, 74);
            this.numLevels.Name = "numLevels";
            this.numLevels.Size = new System.Drawing.Size(60, 20);
            this.numLevels.TabIndex = 3;
            this.numLevels.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // chkAllLevels
            // 
            this.chkAllLevels.AutoSize = true;
            this.chkAllLevels.Location = new System.Drawing.Point(220, 75);
            this.chkAllLevels.Name = "chkAllLevels";
            this.chkAllLevels.Size = new System.Drawing.Size(74, 17);
            this.chkAllLevels.TabIndex = 4;
            this.chkAllLevels.Tag = "L002";
            this.chkAllLevels.Text = "All levels";
            this.chkAllLevels.UseVisualStyleBackColor = true;
            this.chkAllLevels.CheckedChanged += new System.EventHandler(this.ChkAllLevels_CheckedChanged);
            // 
            // chkOnlyDeepest
            // 
            this.chkOnlyDeepest.AutoSize = true;
            this.chkOnlyDeepest.Location = new System.Drawing.Point(12, 100);
            this.chkOnlyDeepest.Name = "chkOnlyDeepest";
            this.chkOnlyDeepest.Size = new System.Drawing.Size(150, 17);
            this.chkOnlyDeepest.TabIndex = 5;
            this.chkOnlyDeepest.Tag = "L004";
            this.chkOnlyDeepest.Text = "Only that level";
            this.chkOnlyDeepest.UseVisualStyleBackColor = true;
            this.chkOnlyDeepest.CheckedChanged += new System.EventHandler(this.ChkOnlyDeepest_CheckedChanged);
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(192, 130);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 6;
            this.cmdOK.Tag = "L005";
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.CmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(273, 130);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 7;
            this.cmdCancel.Tag = "L006";
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.CmdCancel_Click);
            // 
            // FrmScanMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 165);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.chkOnlyDeepest);
            this.Controls.Add(this.chkAllLevels);
            this.Controls.Add(this.numLevels);
            this.Controls.Add(this.lblLevels);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.lblQuestion);
            this.AcceptButton = this.cmdOK;
            this.CancelButton = this.cmdCancel;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Name = "FrmScanMode";
            this.Tag = "L000";
            this.Text = "Scan folder";
            ((System.ComponentModel.ISupportInitialize)(this.numLevels)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Label lblLevels;
        private System.Windows.Forms.NumericUpDown numLevels;
        private System.Windows.Forms.CheckBox chkAllLevels;
        private System.Windows.Forms.CheckBox chkOnlyDeepest;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
    }
}
