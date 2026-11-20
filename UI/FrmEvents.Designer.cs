namespace MyFolderInfo.UI
{
    partial class FrmEvents
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdClose = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmdImage = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdSave);
            this.panel1.Controls.Add(this.cmdImage);
            this.panel1.Controls.Add(this.cmdClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 357);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(654, 29);
            this.panel1.TabIndex = 0;
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(6, 3);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(75, 23);
            this.cmdSave.TabIndex = 5;
            this.cmdSave.Tag = "L001";
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(81, 3);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 4;
            this.cmdClose.Tag = "L002";
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column6,
            this.Column5});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(654, 357);
            this.dataGridView1.TabIndex = 1;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "stt";
            this.Column1.HeaderText = "Stt";
            this.Column1.Name = "Column1";
            this.Column1.Width = 50;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "ngay1";
            this.Column2.HeaderText = "Tu ngay";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "ngay2";
            this.Column3.HeaderText = "Den ngay";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "message";
            this.Column4.HeaderText = "Noi dung";
            this.Column4.Name = "Column4";
            this.Column4.Width = 200;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "message2";
            this.Column6.HeaderText = "Noi Dung 2";
            this.Column6.Name = "Column6";
            this.Column6.Width = 200;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "image_url";
            this.Column5.HeaderText = "Hinh anh";
            this.Column5.Name = "Column5";
            this.Column5.Width = 150;
            // 
            // cmdImage
            // 
            this.cmdImage.Location = new System.Drawing.Point(156, 3);
            this.cmdImage.Name = "cmdImage";
            this.cmdImage.Size = new System.Drawing.Size(75, 23);
            this.cmdImage.TabIndex = 4;
            this.cmdImage.Tag = "L010";
            this.cmdImage.Text = "Images";
            this.cmdImage.UseVisualStyleBackColor = true;
            this.cmdImage.Click += new System.EventHandler(this.cmdImage_Click);
            // 
            // frmEvents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 386);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Name = "frmEvents";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "L000";
            this.Text = "frmEvents";
            // Escape now runs cmdClose_Click, which already asks before discarding edits.
            this.AcceptButton = this.cmdSave;
            this.CancelButton = this.cmdClose;
            this.Load += new System.EventHandler(this.FrmEvents_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.Button cmdImage;
    }
}