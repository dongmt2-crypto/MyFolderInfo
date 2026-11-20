using MyFolderInfo.UI;

namespace MyFolderInfo.UI
{
    partial class FrmNodeInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmNodeInfo));
            this.cmdClose = new System.Windows.Forms.Button();
            this.lblTen = new System.Windows.Forms.Label();
            this.txtnodename = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtoriginalUrl = new System.Windows.Forms.TextBox();
            this.txtfLength = new System.Windows.Forms.TextBox();
            this.txtfCreationTime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtfLastWriteTime = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtfLastAccessTime = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdClose.Location = new System.Drawing.Point(85, 117);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(76, 23);
            this.cmdClose.TabIndex = 7;
            this.cmdClose.Tag = "L007";
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.CmdClose_Click);
            // 
            // lblTen
            // 
            this.lblTen.AutoSize = true;
            this.lblTen.Location = new System.Drawing.Point(15, 19);
            this.lblTen.Name = "lblTen";
            this.lblTen.Size = new System.Drawing.Size(55, 13);
            this.lblTen.TabIndex = 1;
            this.lblTen.Tag = "L001";
            this.lblTen.Text = "Ten Node";
            // 
            // txtnodename
            // 
            this.txtnodename.Location = new System.Drawing.Point(107, 15);
            this.txtnodename.Name = "txtnodename";
            this.txtnodename.Size = new System.Drawing.Size(347, 20);
            this.txtnodename.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 3;
            this.label1.Tag = "L002";
            this.label1.Text = "Dia chi Node";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 8;
            this.label3.Tag = "L003";
            this.label3.Text = "Size Node";
            // 
            // txtoriginalUrl
            // 
            this.txtoriginalUrl.Location = new System.Drawing.Point(107, 38);
            this.txtoriginalUrl.Name = "txtoriginalUrl";
            this.txtoriginalUrl.Size = new System.Drawing.Size(347, 20);
            this.txtoriginalUrl.TabIndex = 1;
            // 
            // txtfLength
            // 
            this.txtfLength.Location = new System.Drawing.Point(107, 61);
            this.txtfLength.Name = "txtfLength";
            this.txtfLength.Size = new System.Drawing.Size(108, 20);
            this.txtfLength.TabIndex = 2;
            // 
            // txtfCreationTime
            // 
            this.txtfCreationTime.Location = new System.Drawing.Point(344, 61);
            this.txtfCreationTime.Name = "txtfCreationTime";
            this.txtfCreationTime.Size = new System.Drawing.Size(110, 20);
            this.txtfCreationTime.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(246, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 10;
            this.label4.Tag = "L005";
            this.label4.Text = "Created Time";
            // 
            // txtfLastWriteTime
            // 
            this.txtfLastWriteTime.Location = new System.Drawing.Point(107, 84);
            this.txtfLastWriteTime.Name = "txtfLastWriteTime";
            this.txtfLastWriteTime.Size = new System.Drawing.Size(108, 20);
            this.txtfLastWriteTime.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 12;
            this.label5.Tag = "L004";
            this.label5.Text = "ModifiedTime";
            // 
            // txtfLastAccessTime
            // 
            this.txtfLastAccessTime.Location = new System.Drawing.Point(344, 84);
            this.txtfLastAccessTime.Name = "txtfLastAccessTime";
            this.txtfLastAccessTime.Size = new System.Drawing.Size(110, 20);
            this.txtfLastAccessTime.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(246, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 14;
            this.label6.Tag = "L006";
            this.label6.Text = "Last AccessTime";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(10, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 108);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            // 
            // cmdSave
            // 
            this.cmdSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdSave.Location = new System.Drawing.Point(10, 117);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(76, 23);
            this.cmdSave.TabIndex = 6;
            this.cmdSave.Tag = "L008";
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.CmdSave_Click);
            // 
            // frmNodeInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 143);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtfLastAccessTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtfLastWriteTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtfLength);
            this.Controls.Add(this.txtfCreationTime);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtoriginalUrl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtnodename);
            this.Controls.Add(this.lblTen);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmNodeInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "L000";
            this.Text = "frmNodeInfo";
            this.AcceptButton = this.cmdSave;
            this.CancelButton = this.cmdClose;
            this.Load += new System.EventHandler(this.FrmNodeInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Label lblTen;
        private System.Windows.Forms.TextBox txtnodename;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtoriginalUrl;
        private System.Windows.Forms.TextBox txtfLength;
        private System.Windows.Forms.TextBox txtfCreationTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtfLastWriteTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtfLastAccessTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdSave;
    }
}