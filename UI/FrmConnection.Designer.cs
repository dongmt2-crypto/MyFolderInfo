namespace MyFolderInfo.UI
{
    partial class FrmConnection
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
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtuser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdLogin = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(132, 38);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(155, 20);
            this.txtDatabase.TabIndex = 1;
            // Was pre-filled with a real internal server/database name, so the dialog shipped
            // someone else's connection details.
            this.txtDatabase.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 1;
            this.label1.Tag = "L002";
            this.label1.Text = "Database";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 3;
            this.label2.Tag = "L001";
            this.label2.Text = "Server";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(132, 12);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(155, 20);
            this.txtServer.TabIndex = 0;
            this.txtServer.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 5;
            this.label3.Tag = "L003";
            this.label3.Text = "User Name";
            // 
            // txtuser
            // 
            this.txtuser.Location = new System.Drawing.Point(132, 64);
            this.txtuser.Name = "txtuser";
            this.txtuser.Size = new System.Drawing.Size(155, 20);
            this.txtuser.TabIndex = 2;
            this.txtuser.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 7;
            this.label4.Tag = "L004";
            this.label4.Text = "Password";
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(132, 90);
            this.txtPass.Name = "txtPass";
            this.txtPass.PasswordChar = '*';
            this.txtPass.Size = new System.Drawing.Size(155, 20);
            this.txtPass.TabIndex = 3;
            this.txtPass.Text = "";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(83, 127);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 5;
            this.cmdCancel.Tag = "L006";
            this.cmdCancel.Text = "Close";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.CmdCancelClick);
            // 
            // cmdLogin
            // 
            this.cmdLogin.Location = new System.Drawing.Point(8, 127);
            this.cmdLogin.Name = "cmdLogin";
            this.cmdLogin.Size = new System.Drawing.Size(75, 23);
            this.cmdLogin.TabIndex = 4;
            this.cmdLogin.Tag = "L005";
            this.cmdLogin.Text = "Login";
            this.cmdLogin.UseVisualStyleBackColor = true;
            this.cmdLogin.Click += new System.EventHandler(this.CmdLoginClick);
            // 
            // frmConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 152);
            this.Controls.Add(this.cmdLogin);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtuser);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDatabase);
            this.Name = "frmConnection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "L000";
            this.Text = "frmConnection";
            this.AcceptButton = this.cmdLogin;
            this.CancelButton = this.cmdCancel;
            this.Load += new System.EventHandler(this.FrmConnectionLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdLogin;
        internal System.Windows.Forms.TextBox txtDatabase;
        internal System.Windows.Forms.TextBox txtServer;
        internal System.Windows.Forms.TextBox txtuser;
        internal System.Windows.Forms.TextBox txtPass;
    }
}