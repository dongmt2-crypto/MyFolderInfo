namespace MyFolderInfo.UI
{
    partial class FrmCompare
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdClose = new System.Windows.Forms.Button();
            this.cmdReport = new System.Windows.Forms.Button();
            this.cmdMenu = new System.Windows.Forms.Button();
            this.cmdGo = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.AllowColumnReorder = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader7,
            this.columnHeader9});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(654, 167);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView1_DragDrop);
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListView1_ColumnClick);
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
            this.columnHeader3.Width = 320;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Status";
            this.columnHeader7.Width = 80;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdClose);
            this.panel1.Controls.Add(this.cmdReport);
            this.panel1.Controls.Add(this.cmdMenu);
            this.panel1.Controls.Add(this.cmdGo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 344);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(654, 32);
            this.panel1.TabIndex = 3;
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(228, 6);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 3;
            this.cmdClose.Tag = "L004";
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.CmdCloseClick);
            // 
            // cmdReport
            // 
            this.cmdReport.Location = new System.Drawing.Point(153, 6);
            this.cmdReport.Name = "cmdReport";
            this.cmdReport.Size = new System.Drawing.Size(75, 23);
            this.cmdReport.TabIndex = 2;
            this.cmdReport.Tag = "L003";
            this.cmdReport.Text = "Report";
            this.cmdReport.UseVisualStyleBackColor = true;
            this.cmdReport.Click += new System.EventHandler(this.CmdReportClick);
            // 
            // cmdMenu
            // 
            this.cmdMenu.Location = new System.Drawing.Point(78, 6);
            this.cmdMenu.Name = "cmdMenu";
            this.cmdMenu.Size = new System.Drawing.Size(75, 23);
            this.cmdMenu.TabIndex = 1;
            this.cmdMenu.Tag = "L002";
            this.cmdMenu.Text = "Go to Menu";
            this.cmdMenu.UseVisualStyleBackColor = true;
            this.cmdMenu.Click += new System.EventHandler(this.CmdMenuClick);
            // 
            // cmdGo
            // 
            this.cmdGo.Location = new System.Drawing.Point(3, 6);
            this.cmdGo.Name = "cmdGo";
            this.cmdGo.Size = new System.Drawing.Size(75, 23);
            this.cmdGo.TabIndex = 0;
            this.cmdGo.Tag = "L001";
            this.cmdGo.Text = "Go";
            this.cmdGo.UseVisualStyleBackColor = true;
            this.cmdGo.Click += new System.EventHandler(this.CmdGoClick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitter1);
            this.panel2.Controls.Add(this.listView2);
            this.panel2.Controls.Add(this.listView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(654, 344);
            this.panel2.TabIndex = 5;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 167);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(654, 4);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // listView2
            // 
            this.listView2.AllowColumnReorder = true;
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader8,
            this.columnHeader10});
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(0, 167);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(654, 177);
            this.listView2.TabIndex = 3;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView2_DragDrop);
            this.listView2.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListView2_ColumnClick);
            this.listView2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListView2_MouseDown);
            this.listView2.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListView2_DragEnter);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "No";
            this.columnHeader4.Width = 50;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Name";
            this.columnHeader5.Width = 200;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "URL";
            this.columnHeader6.Width = 320;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Status";
            this.columnHeader8.Width = 80;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "report.txt";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Date";
            this.columnHeader9.Width = 120;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Date";
            this.columnHeader10.Width = 120;
            // 
            // frmCompare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 376);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmCompare";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "L000";
            this.Text = "frmCompare";
            this.Load += new System.EventHandler(this.FrmCompare_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdGo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button cmdMenu;
        private System.Windows.Forms.Button cmdReport;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
    }
}