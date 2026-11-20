namespace MyFolderInfo.UI
{
    partial class FrmMenu
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
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdReport = new System.Windows.Forms.Button();
            this.cmdClose = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.AllowColumnReorder = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(654, 376);
            this.listView1.TabIndex = 0;
            this.listView1.Tag = "L000";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "No";
            this.columnHeader1.Width = 36;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Menu name";
            this.columnHeader2.Width = 250;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "EXE";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Menu cap1";
            this.columnHeader4.Width = 270;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdReport);
            this.panel1.Controls.Add(this.cmdClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 347);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(654, 29);
            this.panel1.TabIndex = 4;
            // 
            // cmdReport
            // 
            this.cmdReport.Location = new System.Drawing.Point(78, 3);
            this.cmdReport.Name = "cmdReport";
            this.cmdReport.Size = new System.Drawing.Size(75, 23);
            this.cmdReport.TabIndex = 1;
            this.cmdReport.Tag = "L002";
            this.cmdReport.Text = "Report";
            this.cmdReport.UseVisualStyleBackColor = true;
            this.cmdReport.Click += new System.EventHandler(this.CmdReportClick);
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(3, 3);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 0;
            this.cmdClose.Tag = "L001";
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.CmdCloseClick);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "report.txt";
            // 
            // frmMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 376);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.listView1);
            this.Name = "frmMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "L000";
            this.Text = "frmMenu";
            this.Load += new System.EventHandler(this.FrmMenuLoad);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Button cmdReport;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}