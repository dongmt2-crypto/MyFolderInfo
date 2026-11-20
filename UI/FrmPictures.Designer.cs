namespace MyFolderInfo.UI
{
    partial class FrmPictures
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
            this.lblMemory = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.cmdOpen = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setWallpaperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setWallpaper2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setWallpaper3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.rotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblMemory);
            this.panel1.Controls.Add(this.cmdClose);
            this.panel1.Controls.Add(this.cmdOpen);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 434);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(602, 35);
            this.panel1.TabIndex = 0;
            // 
            // lblMemory
            // 
            this.lblMemory.AutoSize = true;
            this.lblMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMemory.Location = new System.Drawing.Point(165, 8);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(96, 15);
            this.lblMemory.TabIndex = 11;
            this.lblMemory.Tag = "L003";
            this.lblMemory.Text = "My Memory....";
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdClose.Location = new System.Drawing.Point(84, 6);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 1;
            this.cmdClose.Tag = "L002";
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.CmdClose_Click);
            // 
            // cmdOpen
            // 
            this.cmdOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdOpen.Location = new System.Drawing.Point(9, 6);
            this.cmdOpen.Name = "cmdOpen";
            this.cmdOpen.Size = new System.Drawing.Size(75, 23);
            this.cmdOpen.TabIndex = 0;
            this.cmdOpen.Tag = "L001";
            this.cmdOpen.Text = "Open Folder";
            this.cmdOpen.UseVisualStyleBackColor = true;
            this.cmdOpen.Click += new System.EventHandler(this.CmdOpen_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.listBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(602, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(190, 469);
            this.panel2.TabIndex = 1;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(190, 469);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.ListBox1_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(602, 469);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setWallpaperToolStripMenuItem,
            this.setWallpaper2ToolStripMenuItem,
            this.setWallpaper3ToolStripMenuItem,
            this.rotateToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(162, 114);
            // 
            // setWallpaperToolStripMenuItem
            // 
            this.setWallpaperToolStripMenuItem.Name = "setWallpaperToolStripMenuItem";
            this.setWallpaperToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.setWallpaperToolStripMenuItem.Text = "Set Wallpaper 1";
            this.setWallpaperToolStripMenuItem.Click += new System.EventHandler(this.SetWallpaperToolStripMenuItem_Click);
            // 
            // setWallpaper2ToolStripMenuItem
            // 
            this.setWallpaper2ToolStripMenuItem.Name = "setWallpaper2ToolStripMenuItem";
            this.setWallpaper2ToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.setWallpaper2ToolStripMenuItem.Text = "Set Wallpaper 2";
            this.setWallpaper2ToolStripMenuItem.Click += new System.EventHandler(this.SetWallpaper2ToolStripMenuItem_Click);
            // 
            // setWallpaper3ToolStripMenuItem
            // 
            this.setWallpaper3ToolStripMenuItem.Name = "setWallpaper3ToolStripMenuItem";
            this.setWallpaper3ToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.setWallpaper3ToolStripMenuItem.Text = "Set Wallpaper 3";
            this.setWallpaper3ToolStripMenuItem.Click += new System.EventHandler(this.SetWallpaper3ToolStripMenuItem_Click);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(599, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 434);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // rotateToolStripMenuItem
            // 
            this.rotateToolStripMenuItem.Name = "rotateToolStripMenuItem";
            this.rotateToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.rotateToolStripMenuItem.Text = "Rotate Image";
            this.rotateToolStripMenuItem.Click += new System.EventHandler(this.RotateMenu_Click);
            // 
            // frmPictures
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 469);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel2);
            this.Name = "frmPictures";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "L000";
            this.Text = "Gallery";
            this.Load += new System.EventHandler(this.FrmPictures_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Button cmdOpen;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem setWallpaperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setWallpaper2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setWallpaper3ToolStripMenuItem;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripMenuItem rotateToolStripMenuItem;
    }
}