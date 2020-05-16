namespace EMXFileManagement
{
    partial class MainFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDiskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createVolumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSampleFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.xoáToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xuấtFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.đặtPasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phụcHồiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkFlagDeletedShow = new System.Windows.Forms.CheckBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(-9, 83);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(380, 500);
            this.treeView1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Dtafalonso-Yosemite-Flat-Folder.ico");
            this.imageList1.Images.SetKeyName(1, "Firstfear-Whistlepuff-Documents.ico");
            this.imageList1.Images.SetKeyName(2, "Saki-NuoveXT-2-Folder-document-open.ico");
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(377, 84);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(911, 499);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "DateCreated";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Modified Date";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Size";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Trạng thái";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1300, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDiskToolStripMenuItem,
            this.createVolumnToolStripMenuItem,
            this.addSampleFileToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openDiskToolStripMenuItem
            // 
            this.openDiskToolStripMenuItem.Name = "openDiskToolStripMenuItem";
            this.openDiskToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.openDiskToolStripMenuItem.Text = "Open disk";
            this.openDiskToolStripMenuItem.Click += new System.EventHandler(this.openDiskToolStripMenuItem_Click);
            // 
            // createVolumnToolStripMenuItem
            // 
            this.createVolumnToolStripMenuItem.Name = "createVolumnToolStripMenuItem";
            this.createVolumnToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.createVolumnToolStripMenuItem.Text = "Create Volumn";
            this.createVolumnToolStripMenuItem.Click += new System.EventHandler(this.createVolumnToolStripMenuItem_Click);
            // 
            // addSampleFileToolStripMenuItem
            // 
            this.addSampleFileToolStripMenuItem.Name = "addSampleFileToolStripMenuItem";
            this.addSampleFileToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.addSampleFileToolStripMenuItem.Text = "Add Sample File";
            this.addSampleFileToolStripMenuItem.Click += new System.EventHandler(this.addSampleFileToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(133, 26);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xoáToolStripMenuItem,
            this.xuấtFileToolStripMenuItem,
            this.đặtPasswordToolStripMenuItem,
            this.phụcHồiToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(168, 100);
            // 
            // xoáToolStripMenuItem
            // 
            this.xoáToolStripMenuItem.Name = "xoáToolStripMenuItem";
            this.xoáToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.xoáToolStripMenuItem.Text = "Xoá";
            this.xoáToolStripMenuItem.Click += new System.EventHandler(this.xoáToolStripMenuItem_Click);
            // 
            // xuấtFileToolStripMenuItem
            // 
            this.xuấtFileToolStripMenuItem.Name = "xuấtFileToolStripMenuItem";
            this.xuấtFileToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.xuấtFileToolStripMenuItem.Text = "Xuất File";
            this.xuấtFileToolStripMenuItem.Click += new System.EventHandler(this.xuấtFileToolStripMenuItem_Click);
            // 
            // đặtPasswordToolStripMenuItem
            // 
            this.đặtPasswordToolStripMenuItem.Name = "đặtPasswordToolStripMenuItem";
            this.đặtPasswordToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.đặtPasswordToolStripMenuItem.Text = "Đặt Password";
            this.đặtPasswordToolStripMenuItem.Click += new System.EventHandler(this.đặtPasswordToolStripMenuItem_Click);
            // 
            // phụcHồiToolStripMenuItem
            // 
            this.phụcHồiToolStripMenuItem.Name = "phụcHồiToolStripMenuItem";
            this.phụcHồiToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.phụcHồiToolStripMenuItem.Text = "Phục hồi";
            this.phụcHồiToolStripMenuItem.Click += new System.EventHandler(this.phụcHồiToolStripMenuItem_Click);
            // 
            // checkFlagDeletedShow
            // 
            this.checkFlagDeletedShow.AutoSize = true;
            this.checkFlagDeletedShow.Checked = true;
            this.checkFlagDeletedShow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkFlagDeletedShow.Location = new System.Drawing.Point(1179, 45);
            this.checkFlagDeletedShow.Name = "checkFlagDeletedShow";
            this.checkFlagDeletedShow.Size = new System.Drawing.Size(109, 21);
            this.checkFlagDeletedShow.TabIndex = 4;
            this.checkFlagDeletedShow.Text = "Hiện đã xoá ";
            this.checkFlagDeletedShow.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = global::EMXFileManagement.Properties.Resources.direction__1_;
            this.btnRefresh.Location = new System.Drawing.Point(1042, 31);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(131, 47);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Cluster Start";
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 585);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.checkFlagDeletedShow);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainFrm";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createVolumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem xoáToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xuấtFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem đặtPasswordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem phụcHồiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSampleFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDiskToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkFlagDeletedShow;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
    }
}

