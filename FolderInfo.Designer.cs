namespace MyFolderInfo
{
    partial class FolderInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderInfo));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripExecution = new System.Windows.Forms.ToolStripMenuItem();
            this.lengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nodeInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchDetailToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.cmdLoad = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.lblMyFolderInfo = new System.Windows.Forms.Label();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdText = new System.Windows.Forms.Button();
            this.chkWriteFile = new System.Windows.Forms.CheckBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.cmdOpen = new System.Windows.Forms.Button();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cLengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cNodeInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cCompareURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.csearchDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdSave2 = new System.Windows.Forms.Button();
            this.cmdLoadXML = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.directSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdClose = new System.Windows.Forms.Button();
            this.cmdSearch = new System.Windows.Forms.Button();
            this.txtNodeSearch = new System.Windows.Forms.TextBox();
            this.chbTree1 = new System.Windows.Forms.CheckBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.cmdText2 = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripExecution,
            this.lengthToolStripMenuItem,
            this.nodeInfoToolStripMenuItem,
            this.searchDetailToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(140, 92);
            // 
            // toolStripExecution
            // 
            this.toolStripExecution.Name = "toolStripExecution";
            this.toolStripExecution.Size = new System.Drawing.Size(139, 22);
            this.toolStripExecution.Tag = "L020";
            this.toolStripExecution.Text = "Execution";
            this.toolStripExecution.Click += new System.EventHandler(this.ExecutionMenu_Click);
            // 
            // lengthToolStripMenuItem
            // 
            this.lengthToolStripMenuItem.Name = "lengthToolStripMenuItem";
            this.lengthToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.lengthToolStripMenuItem.Tag = "L025";
            this.lengthToolStripMenuItem.Text = "Length";
            // 
            // nodeInfoToolStripMenuItem
            // 
            this.nodeInfoToolStripMenuItem.Name = "nodeInfoToolStripMenuItem";
            this.nodeInfoToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.nodeInfoToolStripMenuItem.Tag = "L023";
            this.nodeInfoToolStripMenuItem.Text = "NodeInfo";
            // Was commented out, so the left tree's "NodeInfo" context item did nothing.
            this.nodeInfoToolStripMenuItem.Click += new System.EventHandler(this.NodeInfoMenu_Click);
            // 
            // searchDetailToolStripMenuItem1
            // 
            this.searchDetailToolStripMenuItem1.Name = "searchDetailToolStripMenuItem1";
            this.searchDetailToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            this.searchDetailToolStripMenuItem1.Tag = "L026";
            this.searchDetailToolStripMenuItem1.Text = "SearchDetail";
            // Was commented out, so the left tree's "SearchDetail" context item did nothing.
            this.searchDetailToolStripMenuItem1.Click += new System.EventHandler(this.SearchDetailMenu2_Click);
            // 
            // txtAddress
            // 
            // Was Anchor = Top|Left|Right. FolderInfo.LayoutPanes() places every control in
            // this form from ClientSize, and the anchor pass then moved the same controls
            // again: the search row came out 2 px out of line and txtAddress overlapped
            // txtNodeSearch by 134 px on first open. Nothing here is anchored any more.
            this.txtAddress.BackColor = System.Drawing.SystemColors.Window;
            this.txtAddress.Location = new System.Drawing.Point(12, 42);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.ReadOnly = true;
            this.txtAddress.Size = new System.Drawing.Size(369, 20);
            this.txtAddress.TabIndex = 6;
            // 
            // cmdLoad
            // 
            this.cmdLoad.BackColor = System.Drawing.SystemColors.Control;
            this.cmdLoad.Location = new System.Drawing.Point(12, 65);
            this.cmdLoad.Name = "cmdLoad";
            this.cmdLoad.Size = new System.Drawing.Size(75, 23);
            this.cmdLoad.TabIndex = 0;
            this.cmdLoad.Tag = "L001";
            this.cmdLoad.Text = "Load";
            this.cmdLoad.UseVisualStyleBackColor = false;
            this.cmdLoad.Click += new System.EventHandler(this.CmdLoad_Click);
            // 
            // lblMyFolderInfo
            // 
            this.lblMyFolderInfo.AutoSize = true;
            this.lblMyFolderInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMyFolderInfo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblMyFolderInfo.Location = new System.Drawing.Point(284, 9);
            this.lblMyFolderInfo.Name = "lblMyFolderInfo";
            this.lblMyFolderInfo.Size = new System.Drawing.Size(220, 26);
            this.lblMyFolderInfo.TabIndex = 4;
            this.lblMyFolderInfo.Tag = "L000";
            this.lblMyFolderInfo.Text = "MY FOLDER INFO ";
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(162, 65);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(75, 23);
            this.cmdSave.TabIndex = 2;
            this.cmdSave.Tag = "L003";
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.CmdSave_Click);
            // 
            // cmdText
            // 
            this.cmdText.Location = new System.Drawing.Point(237, 65);
            this.cmdText.Name = "cmdText";
            this.cmdText.Size = new System.Drawing.Size(75, 23);
            this.cmdText.TabIndex = 3;
            this.cmdText.Tag = "L004";
            this.cmdText.Text = "Write text";
            this.cmdText.UseVisualStyleBackColor = true;
            this.cmdText.Click += new System.EventHandler(this.CmdText_Click);
            // 
            // chkWriteFile
            // 
            this.chkWriteFile.AutoSize = true;
            this.chkWriteFile.Checked = true;
            this.chkWriteFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWriteFile.Location = new System.Drawing.Point(312, 69);
            this.chkWriteFile.Name = "chkWriteFile";
            this.chkWriteFile.Size = new System.Drawing.Size(85, 17);
            this.chkWriteFile.TabIndex = 4;
            this.chkWriteFile.Tag = "L005";
            this.chkWriteFile.Text = "Include Files";
            this.chkWriteFile.UseVisualStyleBackColor = true;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "XML file|*.xml";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 551);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(792, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(152, 17);
            this.toolStripStatusLabel1.Text = "Created by DongMT @2009";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(103, 17);
            this.toolStripStatusLabel.Tag = "L008";
            this.toolStripStatusLabel.Text = "................................";
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(100, 17);
            this.toolStripStatusLabel2.Tag = "L009";
            this.toolStripStatusLabel2.Text = "...............................";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "XML file|*.xml";
            // 
            // cmdOpen
            // 
            this.cmdOpen.Location = new System.Drawing.Point(493, 65);
            this.cmdOpen.Name = "cmdOpen";
            this.cmdOpen.Size = new System.Drawing.Size(75, 23);
            this.cmdOpen.TabIndex = 8;
            this.cmdOpen.Tag = "L007";
            this.cmdOpen.Text = "Open";
            this.cmdOpen.UseVisualStyleBackColor = true;
            // Was commented out: the "Open" button rendered and did nothing.
            this.cmdOpen.Click += new System.EventHandler(this.CmdOpen_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.cLengthToolStripMenuItem,
            this.cNodeInfoToolStripMenuItem,
            this.cCompareURLToolStripMenuItem,
            this.csearchDetailToolStripMenuItem});
            // Was "contextMenuStrip1", which collided with the real contextMenuStrip1 above
            // and made Controls["contextMenuStrip1"] / the designer ambiguous.
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(151, 136);
            this.contextMenuStrip2.Tag = "L023";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.addToolStripMenuItem.Tag = "L021";
            this.addToolStripMenuItem.Text = "Add";
             this.addToolStripMenuItem.Click += new System.EventHandler(this.AddMenu_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.removeToolStripMenuItem.Tag = "L017";
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveMenu_Click);
            // 
            // cLengthToolStripMenuItem
            // 
            this.cLengthToolStripMenuItem.Name = "cLengthToolStripMenuItem";
            this.cLengthToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.cLengthToolStripMenuItem.Tag = "L025";
            this.cLengthToolStripMenuItem.Text = "cLength";
            // 
            // cNodeInfoToolStripMenuItem
            // 
            this.cNodeInfoToolStripMenuItem.Name = "cNodeInfoToolStripMenuItem";
            this.cNodeInfoToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.cNodeInfoToolStripMenuItem.Tag = "L023";
            this.cNodeInfoToolStripMenuItem.Text = "cNodeInfo";
             this.cNodeInfoToolStripMenuItem.Click += new System.EventHandler(this.RightNodeInfoMenu_Click);
            // 
            // cCompareURLToolStripMenuItem
            // 
            this.cCompareURLToolStripMenuItem.Name = "cCompareURLToolStripMenuItem";
            this.cCompareURLToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.cCompareURLToolStripMenuItem.Tag = "L034";
            this.cCompareURLToolStripMenuItem.Text = "cCompareURL";
            this.cCompareURLToolStripMenuItem.Click += new System.EventHandler(this.CompareUrlMenu_Click);
            // 
            // csearchDetailToolStripMenuItem
            // 
            this.csearchDetailToolStripMenuItem.Name = "csearchDetailToolStripMenuItem";
            this.csearchDetailToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.csearchDetailToolStripMenuItem.Tag = "L026";
            this.csearchDetailToolStripMenuItem.Text = "cSearchDetail";
            this.csearchDetailToolStripMenuItem.Click += new System.EventHandler(this.SearchDetailMenu_Click);
            // 
            // cmdSave2
            // 
            this.cmdSave2.Location = new System.Drawing.Point(568, 65);
            this.cmdSave2.Name = "cmdSave2";
            this.cmdSave2.Size = new System.Drawing.Size(75, 23);
            this.cmdSave2.TabIndex = 9;
            this.cmdSave2.Tag = "L006";
            this.cmdSave2.Text = "Save";
            this.cmdSave2.UseVisualStyleBackColor = true;
            this.cmdSave2.Click += new System.EventHandler(this.CmdSave2_Click);
            // 
            // cmdLoadXML
            // 
            this.cmdLoadXML.Location = new System.Drawing.Point(87, 65);
            this.cmdLoadXML.Name = "cmdLoadXML";
            this.cmdLoadXML.Size = new System.Drawing.Size(75, 23);
            this.cmdLoadXML.TabIndex = 1;
            this.cmdLoadXML.Tag = "L002";
            this.cmdLoadXML.Text = "Load XML";
            this.cmdLoadXML.UseVisualStyleBackColor = true;
            this.cmdLoadXML.Click += new System.EventHandler(this.CmdLoadXml_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionToolStripMenuItem,
            this.toolToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(792, 24);
            this.menuStrip1.TabIndex = 21;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionToolStripMenuItem
            // 
            this.optionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.colorToolStripMenuItem});
            this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
            this.optionToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.optionToolStripMenuItem.Tag = "L015";
            this.optionToolStripMenuItem.Text = "Option";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.optionsToolStripMenuItem.Tag = "L016";
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItem_Click);
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.colorToolStripMenuItem.Tag = "L027";
            this.colorToolStripMenuItem.Text = "Color";
            this.colorToolStripMenuItem.Click += new System.EventHandler(this.ColorToolStripMenuItem_Click);
            // 
            // toolToolStripMenuItem
            // 
            this.toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compareToolStripMenuItem,
            this.directSearchToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.eventsToolStripMenuItem});
            this.toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            this.toolToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.toolToolStripMenuItem.Tag = "L031";
            this.toolToolStripMenuItem.Text = "Tool";
            // 
            // compareToolStripMenuItem
            // 
            this.compareToolStripMenuItem.Name = "compareToolStripMenuItem";
            this.compareToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.compareToolStripMenuItem.Tag = "L032";
            this.compareToolStripMenuItem.Text = "Compare";
            this.compareToolStripMenuItem.Click += new System.EventHandler(this.CompareToolStripMenuItem_Click);
            // 
            // directSearchToolStripMenuItem
            // 
            this.directSearchToolStripMenuItem.Name = "directSearchToolStripMenuItem";
            this.directSearchToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.directSearchToolStripMenuItem.Tag = "L033";
            this.directSearchToolStripMenuItem.Text = "DirectSearch";
            this.directSearchToolStripMenuItem.Click += new System.EventHandler(this.DirectSearchToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Tag = "L030";
            this.aboutToolStripMenuItem.Text = "Gallery";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // eventsToolStripMenuItem
            // 
            this.eventsToolStripMenuItem.Name = "eventsToolStripMenuItem";
            this.eventsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.eventsToolStripMenuItem.Tag = "L039";
            this.eventsToolStripMenuItem.Text = "Events";
            this.eventsToolStripMenuItem.Click += new System.EventHandler(this.EventsMenu_Click);
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(718, 65);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(60, 23);
            this.cmdClose.TabIndex = 10;
            this.cmdClose.Tag = "L012";
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.CmdClose_Click);
            // 
            // cmdSearch
            // 
            this.cmdSearch.Location = new System.Drawing.Point(703, 41);
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.Size = new System.Drawing.Size(75, 23);
            this.cmdSearch.TabIndex = 6;
            this.cmdSearch.Tag = "L024";
            this.cmdSearch.Text = "Search";
            this.cmdSearch.UseVisualStyleBackColor = true;
            this.cmdSearch.Click += new System.EventHandler(this.CmdSearch_Click);
            // 
            // txtNodeSearch
            // 
            this.txtNodeSearch.Location = new System.Drawing.Point(399, 42);
            this.txtNodeSearch.Name = "txtNodeSearch";
            this.txtNodeSearch.Size = new System.Drawing.Size(304, 20);
            this.txtNodeSearch.TabIndex = 5;
            this.txtNodeSearch.TextChanged += new System.EventHandler(this.TxtNodeSearch_TextChanged);
            this.txtNodeSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtNodeSearch_KeyDown);
            // 
            // chbTree1
            // 
            this.chbTree1.AutoSize = true;
            this.chbTree1.Checked = true;
            this.chbTree1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbTree1.Location = new System.Drawing.Point(399, 68);
            this.chbTree1.Name = "chbTree1";
            this.chbTree1.Size = new System.Drawing.Size(94, 17);
            this.chbTree1.TabIndex = 11;
            this.chbTree1.Tag = "L022";
            this.chbTree1.Text = "Search Tree 1";
            this.chbTree1.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.BackColor = System.Drawing.SystemColors.Window;
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Location = new System.Drawing.Point(12, 93);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(369, 445);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1_AfterSelect);
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView1_BeforeExpand);
            // Was NodeMouseClick. The target node is now resolved in MouseDown so the right
            // tree and the left tree behave identically.
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeView1_MouseDown);
            this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.TreeView1_DragOver);
            // 
            // treeView2
            // 
            this.treeView2.AllowDrop = true;
            this.treeView2.ContextMenuStrip = this.contextMenuStrip2;
            this.treeView2.Location = new System.Drawing.Point(397, 91);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(381, 445);
            this.treeView2.TabIndex = 1;
            this.treeView2.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.TreeView2_ItemDrag);
            this.treeView2.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView2_AfterSelect);
            this.treeView2.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView2_BeforeExpand);
            this.treeView2.DragDrop += new System.Windows.Forms.DragEventHandler(this.TreeView2_DragDrop);
            this.treeView2.DragEnter += new System.Windows.Forms.DragEventHandler(this.TreeView2_DragEnter);
            this.treeView2.DragOver += new System.Windows.Forms.DragEventHandler(this.TreeView2_DragOver);
            this.treeView2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeView2_MouseDown);
            // 
            // cmdText2

            this.cmdText2.Location = new System.Drawing.Point(643, 65);
            this.cmdText2.Name = "cmdText2";
            this.cmdText2.Size = new System.Drawing.Size(75, 23);
            this.cmdText2.TabIndex = 22;
            this.cmdText2.Tag = "L029";
            this.cmdText2.Text = "Write text";
            this.cmdText2.UseVisualStyleBackColor = true;
            this.cmdText2.Click += new System.EventHandler(this.CmdText2_Click);
            // 
            // FolderInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.cmdText2);
            this.Controls.Add(this.treeView2);
            this.Controls.Add(this.cmdSearch);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.txtNodeSearch);
            this.Controls.Add(this.chbTree1);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.lblMyFolderInfo);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.cmdLoadXML);
            this.Controls.Add(this.chkWriteFile);
            this.Controls.Add(this.cmdOpen);
            this.Controls.Add(this.cmdText);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.cmdSave2);
            this.Controls.Add(this.cmdLoad);
            this.Controls.Add(this.txtAddress);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FolderInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MyFolderInfo";
            this.Load += new System.EventHandler(this.FolderInfo_Load);
            this.Resize += new System.EventHandler(this.FolderInfo_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Button cmdLoad;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label lblMyFolderInfo;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdText;
        private System.Windows.Forms.CheckBox chkWriteFile;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button cmdOpen;
        private System.Windows.Forms.Button cmdSave2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.Button cmdLoadXML;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripExecution;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lengthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cLengthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nodeInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cNodeInfoToolStripMenuItem;
        private System.Windows.Forms.Button cmdSearch;
        private System.Windows.Forms.TextBox txtNodeSearch;
        private System.Windows.Forms.CheckBox chbTree1;
        private System.Windows.Forms.ToolStripMenuItem searchDetailToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem csearchDetailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.Button cmdText2;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem directSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cCompareURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eventsToolStripMenuItem;
       
    }
}

