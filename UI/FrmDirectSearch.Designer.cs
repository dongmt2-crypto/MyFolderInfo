namespace MyFolderInfo.UI
{
    partial class FrmDirectSearch
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.chkWriteFile = new System.Windows.Forms.CheckBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.cmdSearch = new System.Windows.Forms.Button();
            this.cmdLoad = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblPath2 = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.txtPath);
            this.panel1.Controls.Add(this.chkWriteFile);
            this.panel1.Controls.Add(this.lblPath);
            this.panel1.Controls.Add(this.cmdSearch);
            this.panel1.Controls.Add(this.cmdLoad);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtSearch);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(651, 56);
            this.panel1.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(386, 31);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(90, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Tag = "L003";
            this.checkBox1.Text = "Search in List";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllSystemSources;
            this.txtPath.BackColor = System.Drawing.SystemColors.Window;
            this.txtPath.Location = new System.Drawing.Point(81, 28);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(283, 20);
            this.txtPath.TabIndex = 2;
            // 
            // chkWriteFile
            // 
            this.chkWriteFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkWriteFile.AutoSize = true;
            this.chkWriteFile.Checked = true;
            this.chkWriteFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWriteFile.Location = new System.Drawing.Point(482, 31);
            this.chkWriteFile.Name = "chkWriteFile";
            this.chkWriteFile.Size = new System.Drawing.Size(85, 17);
            this.chkWriteFile.TabIndex = 4;
            this.chkWriteFile.Tag = "L004";
            this.chkWriteFile.Text = "Include Files";
            this.chkWriteFile.UseVisualStyleBackColor = true;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(3, 31);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(29, 13);
            this.lblPath.TabIndex = 12;
            this.lblPath.Tag = "L002";
            this.lblPath.Text = "Path";
            // 
            // cmdSearch
            // 
            this.cmdSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSearch.BackColor = System.Drawing.SystemColors.Control;
            this.cmdSearch.Location = new System.Drawing.Point(573, 27);
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.Size = new System.Drawing.Size(75, 23);
            this.cmdSearch.TabIndex = 5;
            this.cmdSearch.Tag = "L006";
            this.cmdSearch.Text = "Search";
            this.cmdSearch.UseVisualStyleBackColor = false;
            this.cmdSearch.Click += new System.EventHandler(this.CmdSearch_Click);
            // 
            // cmdLoad
            // 
            this.cmdLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdLoad.BackColor = System.Drawing.SystemColors.Control;
            this.cmdLoad.Location = new System.Drawing.Point(573, 5);
            this.cmdLoad.Name = "cmdLoad";
            this.cmdLoad.Size = new System.Drawing.Size(75, 23);
            this.cmdLoad.TabIndex = 1;
            this.cmdLoad.Tag = "L005";
            this.cmdLoad.Text = "Load";
            this.cmdLoad.UseVisualStyleBackColor = false;
            this.cmdLoad.Click += new System.EventHandler(this.CmdLoad_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 9;
            this.label1.Tag = "L001";
            this.label1.Text = "Search";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.BackColor = System.Drawing.SystemColors.Window;
            this.txtSearch.Location = new System.Drawing.Point(81, 7);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(486, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblPath2);
            this.panel2.Controls.Add(this.cmdClose);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 375);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(651, 29);
            this.panel2.TabIndex = 1;
            // 
            // lblPath2
            // 
            this.lblPath2.AutoSize = true;
            this.lblPath2.Location = new System.Drawing.Point(105, 8);
            this.lblPath2.Name = "lblPath2";
            this.lblPath2.Size = new System.Drawing.Size(31, 13);
            this.lblPath2.TabIndex = 1;
            this.lblPath2.Tag = "L002";
            this.lblPath2.Text = "L002";
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(3, 3);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 0;
            this.cmdClose.Tag = "L007";
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.CmdClose_Click);
            // 
            // listView1
            // 
            this.listView1.AllowColumnReorder = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 56);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(651, 319);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView1_DragDrop);
            this.listView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListView1_MouseDown);
            this.listView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListView1_DragEnter);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "No";
            this.columnHeader1.Width = 50;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "URL";
            this.columnHeader3.Width = 390;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(112, 26);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.openToolStripMenuItem.Tag = "L005";
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenMenu_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 500;
            this.timer2.Tick += new System.EventHandler(this.Timer2_Tick);
            // 
            // frmDirectSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 404);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmDirectSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "L000";
            this.Text = "frmDirectSearch";
            this.Load += new System.EventHandler(this.FrmDirectSearch_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdSearch;
        private System.Windows.Forms.Button cmdLoad;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkWriteFile;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblPath2;
        private System.Windows.Forms.Timer timer2;
    }
}