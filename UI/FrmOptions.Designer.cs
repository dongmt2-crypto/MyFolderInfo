using MyFolderInfo.UI;

namespace MyFolderInfo.UI
{
    partial class FrmOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmOptions));
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSort = new System.Windows.Forms.ComboBox();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbLevel = new System.Windows.Forms.ComboBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.ChkDir = new System.Windows.Forms.CheckBox();
            this.txtImageDir = new System.Windows.Forms.TextBox();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmdSave
            // 
            this.cmdSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdSave.Location = new System.Drawing.Point(12, 188);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(93, 23);
            this.cmdSave.TabIndex = 3;
            this.cmdSave.Tag = "L005";
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.CmdSave_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdCancel.Location = new System.Drawing.Point(105, 188);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(93, 23);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Tag = "L006";
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.CmdCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(355, 159);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = "L001";
            this.groupBox1.Text = "Options";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 3;
            this.label1.Tag = "L004";
            this.label1.Text = "Language File";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 5;
            this.label2.Tag = "L002";
            this.label2.Text = "Sort Files by";
            // 
            // cmbSort
            // 
            this.cmbSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSort.FormattingEnabled = true;
            this.cmbSort.Items.AddRange(new object[] {
            "Name",
            "Extention"});
            this.cmbSort.Location = new System.Drawing.Point(133, 58);
            this.cmbSort.Name = "cmbSort";
            this.cmbSort.Size = new System.Drawing.Size(198, 21);
            this.cmbSort.TabIndex = 1;
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Items.AddRange(new object[] {
            "Name",
            "Extention"});
            this.cmbLanguage.Location = new System.Drawing.Point(133, 31);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(198, 21);
            this.cmbLanguage.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 8;
            this.label3.Tag = "L007";
            this.label3.Text = "Load 2 level";
            // 
            // cmbLevel
            // 
            this.cmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLevel.FormattingEnabled = true;
            this.cmbLevel.Items.AddRange(new object[] {
            "Name",
            "Extention"});
            this.cmbLevel.Location = new System.Drawing.Point(133, 86);
            this.cmbLevel.Name = "cmbLevel";
            this.cmbLevel.Size = new System.Drawing.Size(198, 21);
            this.cmbLevel.TabIndex = 2;
            // 
            // ChkDir
            // 
            this.ChkDir.AutoSize = true;
            this.ChkDir.Location = new System.Drawing.Point(133, 114);
            this.ChkDir.Name = "ChkDir";
            this.ChkDir.Size = new System.Drawing.Size(151, 17);
            this.ChkDir.TabIndex = 9;
            this.ChkDir.Tag = "L010";
            this.ChkDir.Text = "Same as program directory";
            this.ChkDir.UseVisualStyleBackColor = true;
            this.ChkDir.CheckedChanged += new System.EventHandler(this.ChkDir_CheckedChanged);
            // 
            // txtImageDir
            // 
            this.txtImageDir.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtImageDir.Location = new System.Drawing.Point(133, 138);
            this.txtImageDir.Name = "txtImageDir";
            this.txtImageDir.ReadOnly = true;
            this.txtImageDir.Size = new System.Drawing.Size(198, 20);
            this.txtImageDir.TabIndex = 10;
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Location = new System.Drawing.Point(290, 110);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(41, 23);
            this.cmdBrowse.TabIndex = 11;
            this.cmdBrowse.Tag = "L011";
            this.cmdBrowse.Text = "...";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.CmdBrowse_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 12;
            this.label4.Tag = "L009";
            this.label4.Text = "Image Dir";
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 214);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmdBrowse);
            this.Controls.Add(this.txtImageDir);
            this.Controls.Add(this.ChkDir);
            this.Controls.Add(this.cmbLevel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbLanguage);
            this.Controls.Add(this.cmbSort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "L000";
            this.Text = "frmOptions";
            this.AcceptButton = this.cmdSave;
            this.CancelButton = this.cmdCancel;
            this.Load += new System.EventHandler(this.FrmOptions_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSort;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbLevel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox ChkDir;
        private System.Windows.Forms.TextBox txtImageDir;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.Label label4;
    }
}