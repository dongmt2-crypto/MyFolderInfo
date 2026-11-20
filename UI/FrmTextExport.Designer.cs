namespace MyFolderInfo.UI
{
    partial class FrmTextExport
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
            this.chkNoNumber = new System.Windows.Forms.CheckBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblQuestion
            // 
            this.lblQuestion.Location = new System.Drawing.Point(12, 12);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(336, 32);
            this.lblQuestion.TabIndex = 0;
            this.lblQuestion.Tag = "L001";
            this.lblQuestion.Text = "How should the text file be written?";
            // 
            // chkNoNumber
            // 
            this.chkNoNumber.AutoSize = true;
            this.chkNoNumber.Location = new System.Drawing.Point(12, 54);
            this.chkNoNumber.Name = "chkNoNumber";
            this.chkNoNumber.Size = new System.Drawing.Size(220, 17);
            this.chkNoNumber.TabIndex = 1;
            this.chkNoNumber.Tag = "L002";
            this.chkNoNumber.Text = "Do not number folder and file names";
            this.chkNoNumber.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(192, 90);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Tag = "L003";
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.CmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(273, 90);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Tag = "L004";
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.CmdCancel_Click);
            // 
            // FrmTextExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 125);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.chkNoNumber);
            this.Controls.Add(this.lblQuestion);
            this.AcceptButton = this.cmdOK;
            this.CancelButton = this.cmdCancel;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Name = "FrmTextExport";
            this.Tag = "L000";
            this.Text = "Export text";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.CheckBox chkNoNumber;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
    }
}
