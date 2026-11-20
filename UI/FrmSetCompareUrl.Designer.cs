namespace MyFolderInfo.UI
{
    partial class FrmSetCompareUrl
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
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtfCompareURL = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtnodename = new System.Windows.Forms.TextBox();
            this.lblTen = new System.Windows.Forms.Label();
            this.txtNewDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.Location = new System.Drawing.Point(5, 113);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Tag = "L004";
            this.cmdOK.Text = "Save";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.CmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.Location = new System.Drawing.Point(80, 113);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Tag = "L005";
            this.cmdCancel.Text = "Close";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.CmdCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(10, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 98);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            // 
            // txtfCompareURL
            // 
            this.txtfCompareURL.Location = new System.Drawing.Point(149, 44);
            this.txtfCompareURL.Name = "txtfCompareURL";
            this.txtfCompareURL.Size = new System.Drawing.Size(313, 20);
            this.txtfCompareURL.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 20;
            this.label1.Tag = "L002";
            this.label1.Text = "Dia chi goc so sanh";
            // 
            // txtnodename
            // 
            this.txtnodename.Location = new System.Drawing.Point(149, 21);
            this.txtnodename.Name = "txtnodename";
            this.txtnodename.Size = new System.Drawing.Size(313, 20);
            this.txtnodename.TabIndex = 0;
            // 
            // lblTen
            // 
            this.lblTen.AutoSize = true;
            this.lblTen.Location = new System.Drawing.Point(19, 25);
            this.lblTen.Name = "lblTen";
            this.lblTen.Size = new System.Drawing.Size(55, 13);
            this.lblTen.TabIndex = 18;
            this.lblTen.Tag = "L001";
            this.lblTen.Text = "Ten Node";
            // 
            // txtNewDir
            // 
            this.txtNewDir.Location = new System.Drawing.Point(149, 69);
            this.txtNewDir.Name = "txtNewDir";
            this.txtNewDir.Size = new System.Drawing.Size(313, 20);
            this.txtNewDir.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 13);
            this.label2.TabIndex = 22;
            this.label2.Tag = "L003";
            this.label2.Text = "Dia chi goc so sanh moi";
            // 
            // FrmSetCompareUrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 139);
            this.Controls.Add(this.txtNewDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtfCompareURL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtnodename);
            this.Controls.Add(this.lblTen);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox1);
            this.AcceptButton = cmdOK;
            this.CancelButton = cmdCancel;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Name = "FrmSetCompareUrl";
            this.Tag = "L000";
            this.Text = "Set compare URL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtnodename;
        private System.Windows.Forms.Label lblTen;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox txtfCompareURL;
        internal System.Windows.Forms.TextBox txtNewDir;
    }
}