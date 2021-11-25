namespace NT.SPC.DataServer
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tsMainForm = new System.Windows.Forms.ToolStrip();
            this.tssStartServer = new System.Windows.Forms.ToolStripButton();
            this.tssStopServer = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tssAddGroup = new System.Windows.Forms.ToolStripButton();
            this.tssAddMember = new System.Windows.Forms.ToolStripButton();
            this.tssEditSelect = new System.Windows.Forms.ToolStripButton();
            this.tssDelSelect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tssDeviceManage = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gbDataDetail = new System.Windows.Forms.GroupBox();
            this.pnlDataDetail = new System.Windows.Forms.Panel();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbDeviceState = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbDeviceName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDataName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbTreeView = new System.Windows.Forms.GroupBox();
            this.tvDataMap = new System.Windows.Forms.TreeView();
            this.imglTreeIcom = new System.Windows.Forms.ImageList(this.components);
            this.ntfBackIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmShowMain = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmExitApp = new System.Windows.Forms.ToolStripMenuItem();
            this.tbMbArea = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbMbAddress = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tsMainForm.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbDataDetail.SuspendLayout();
            this.pnlDataDetail.SuspendLayout();
            this.gbTreeView.SuspendLayout();
            this.cmsIcon.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsMainForm
            // 
            this.tsMainForm.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tsMainForm.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsMainForm.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.tsMainForm.ImeMode = System.Windows.Forms.ImeMode.HangulFull;
            this.tsMainForm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssStartServer,
            this.tssStopServer,
            this.toolStripSeparator1,
            this.tssAddGroup,
            this.tssAddMember,
            this.tssEditSelect,
            this.tssDelSelect,
            this.toolStripSeparator3,
            this.tssDeviceManage});
            this.tsMainForm.Location = new System.Drawing.Point(0, 0);
            this.tsMainForm.Name = "tsMainForm";
            this.tsMainForm.Size = new System.Drawing.Size(767, 39);
            this.tsMainForm.TabIndex = 6;
            this.tsMainForm.Text = "主页工具栏";
            // 
            // tssStartServer
            // 
            this.tssStartServer.BackColor = System.Drawing.SystemColors.Control;
            this.tssStartServer.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tssStartServer.Image = ((System.Drawing.Image)(resources.GetObject("tssStartServer.Image")));
            this.tssStartServer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssStartServer.Name = "tssStartServer";
            this.tssStartServer.Size = new System.Drawing.Size(101, 36);
            this.tssStartServer.Text = "启动服务";
            this.tssStartServer.Click += new System.EventHandler(this.tssStartServer_Click);
            // 
            // tssStopServer
            // 
            this.tssStopServer.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tssStopServer.Image = ((System.Drawing.Image)(resources.GetObject("tssStopServer.Image")));
            this.tssStopServer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssStopServer.Name = "tssStopServer";
            this.tssStopServer.Size = new System.Drawing.Size(101, 36);
            this.tssStopServer.Text = "停止服务";
            this.tssStopServer.Click += new System.EventHandler(this.tssStopServer_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator1.MergeIndex = 2;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // tssAddGroup
            // 
            this.tssAddGroup.Image = ((System.Drawing.Image)(resources.GetObject("tssAddGroup.Image")));
            this.tssAddGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssAddGroup.Name = "tssAddGroup";
            this.tssAddGroup.Size = new System.Drawing.Size(101, 36);
            this.tssAddGroup.Text = "添加分组";
            this.tssAddGroup.Click += new System.EventHandler(this.tssAddGroup_Click);
            // 
            // tssAddMember
            // 
            this.tssAddMember.Image = ((System.Drawing.Image)(resources.GetObject("tssAddMember.Image")));
            this.tssAddMember.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssAddMember.Name = "tssAddMember";
            this.tssAddMember.Size = new System.Drawing.Size(101, 36);
            this.tssAddMember.Text = "添加成员";
            this.tssAddMember.Click += new System.EventHandler(this.tssAddMember_Click);
            // 
            // tssEditSelect
            // 
            this.tssEditSelect.Image = ((System.Drawing.Image)(resources.GetObject("tssEditSelect.Image")));
            this.tssEditSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssEditSelect.Name = "tssEditSelect";
            this.tssEditSelect.Size = new System.Drawing.Size(101, 36);
            this.tssEditSelect.Text = "编辑选中";
            this.tssEditSelect.Click += new System.EventHandler(this.tssEditSelect_Click);
            // 
            // tssDelSelect
            // 
            this.tssDelSelect.Image = ((System.Drawing.Image)(resources.GetObject("tssDelSelect.Image")));
            this.tssDelSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssDelSelect.Name = "tssDelSelect";
            this.tssDelSelect.Size = new System.Drawing.Size(101, 36);
            this.tssDelSelect.Text = "移除选中";
            this.tssDelSelect.Click += new System.EventHandler(this.tssDelSelect_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 39);
            // 
            // tssDeviceManage
            // 
            this.tssDeviceManage.Image = ((System.Drawing.Image)(resources.GetObject("tssDeviceManage.Image")));
            this.tssDeviceManage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssDeviceManage.Name = "tssDeviceManage";
            this.tssDeviceManage.Size = new System.Drawing.Size(101, 36);
            this.tssDeviceManage.Text = "设备管理";
            this.tssDeviceManage.Click += new System.EventHandler(this.tssDeviceManage_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gbDataDetail);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(423, 39);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(344, 416);
            this.panel1.TabIndex = 7;
            // 
            // gbDataDetail
            // 
            this.gbDataDetail.Controls.Add(this.pnlDataDetail);
            this.gbDataDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbDataDetail.Location = new System.Drawing.Point(0, 0);
            this.gbDataDetail.Name = "gbDataDetail";
            this.gbDataDetail.Size = new System.Drawing.Size(344, 416);
            this.gbDataDetail.TabIndex = 0;
            this.gbDataDetail.TabStop = false;
            this.gbDataDetail.Text = "详情";
            // 
            // pnlDataDetail
            // 
            this.pnlDataDetail.Controls.Add(this.tbMbAddress);
            this.pnlDataDetail.Controls.Add(this.label8);
            this.pnlDataDetail.Controls.Add(this.tbMbArea);
            this.pnlDataDetail.Controls.Add(this.label7);
            this.pnlDataDetail.Controls.Add(this.tbAddress);
            this.pnlDataDetail.Controls.Add(this.label6);
            this.pnlDataDetail.Controls.Add(this.tbValue);
            this.pnlDataDetail.Controls.Add(this.label5);
            this.pnlDataDetail.Controls.Add(this.tbDeviceState);
            this.pnlDataDetail.Controls.Add(this.label4);
            this.pnlDataDetail.Controls.Add(this.tbDeviceName);
            this.pnlDataDetail.Controls.Add(this.label3);
            this.pnlDataDetail.Controls.Add(this.tbDataName);
            this.pnlDataDetail.Controls.Add(this.label2);
            this.pnlDataDetail.Controls.Add(this.tbName);
            this.pnlDataDetail.Controls.Add(this.label1);
            this.pnlDataDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDataDetail.Location = new System.Drawing.Point(3, 22);
            this.pnlDataDetail.Name = "pnlDataDetail";
            this.pnlDataDetail.Size = new System.Drawing.Size(338, 391);
            this.pnlDataDetail.TabIndex = 0;
            // 
            // tbAddress
            // 
            this.tbAddress.Location = new System.Drawing.Point(114, 239);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.ReadOnly = true;
            this.tbAddress.Size = new System.Drawing.Size(215, 26);
            this.tbAddress.TabIndex = 23;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 243);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 16);
            this.label6.TabIndex = 22;
            this.label6.Text = "OPCUA地址：";
            // 
            // tbValue
            // 
            this.tbValue.Location = new System.Drawing.Point(114, 194);
            this.tbValue.Name = "tbValue";
            this.tbValue.ReadOnly = true;
            this.tbValue.Size = new System.Drawing.Size(215, 26);
            this.tbValue.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 16);
            this.label5.TabIndex = 20;
            this.label5.Text = "当前值：";
            // 
            // tbDeviceState
            // 
            this.tbDeviceState.Location = new System.Drawing.Point(114, 149);
            this.tbDeviceState.Name = "tbDeviceState";
            this.tbDeviceState.ReadOnly = true;
            this.tbDeviceState.Size = new System.Drawing.Size(215, 26);
            this.tbDeviceState.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 16);
            this.label4.TabIndex = 18;
            this.label4.Text = "设备状态：";
            // 
            // tbDeviceName
            // 
            this.tbDeviceName.Location = new System.Drawing.Point(114, 104);
            this.tbDeviceName.Name = "tbDeviceName";
            this.tbDeviceName.ReadOnly = true;
            this.tbDeviceName.Size = new System.Drawing.Size(215, 26);
            this.tbDeviceName.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 16;
            this.label3.Text = "来源设备：";
            // 
            // tbDataName
            // 
            this.tbDataName.Location = new System.Drawing.Point(114, 59);
            this.tbDataName.Name = "tbDataName";
            this.tbDataName.ReadOnly = true;
            this.tbDataName.Size = new System.Drawing.Size(215, 26);
            this.tbDataName.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 14;
            this.label2.Text = "数据名称：";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(114, 14);
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(215, 26);
            this.tbName.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 12;
            this.label1.Text = "自定名称：";
            // 
            // gbTreeView
            // 
            this.gbTreeView.Controls.Add(this.tvDataMap);
            this.gbTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbTreeView.Location = new System.Drawing.Point(0, 39);
            this.gbTreeView.Name = "gbTreeView";
            this.gbTreeView.Size = new System.Drawing.Size(423, 416);
            this.gbTreeView.TabIndex = 8;
            this.gbTreeView.TabStop = false;
            this.gbTreeView.Text = "采集的数据";
            // 
            // tvDataMap
            // 
            this.tvDataMap.BackColor = System.Drawing.SystemColors.Control;
            this.tvDataMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvDataMap.ImageIndex = 0;
            this.tvDataMap.ImageList = this.imglTreeIcom;
            this.tvDataMap.Location = new System.Drawing.Point(3, 22);
            this.tvDataMap.Name = "tvDataMap";
            this.tvDataMap.SelectedImageIndex = 0;
            this.tvDataMap.Size = new System.Drawing.Size(417, 391);
            this.tvDataMap.TabIndex = 0;
            this.tvDataMap.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvDataMap_NodeMouseClick);
            // 
            // imglTreeIcom
            // 
            this.imglTreeIcom.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglTreeIcom.ImageStream")));
            this.imglTreeIcom.TransparentColor = System.Drawing.Color.Transparent;
            this.imglTreeIcom.Images.SetKeyName(0, "Home.ico");
            this.imglTreeIcom.Images.SetKeyName(1, "Tag.ico");
            this.imglTreeIcom.Images.SetKeyName(2, "Rss.ico");
            // 
            // ntfBackIcon
            // 
            this.ntfBackIcon.ContextMenuStrip = this.cmsIcon;
            this.ntfBackIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("ntfBackIcon.Icon")));
            this.ntfBackIcon.Text = "OPCUA";
            this.ntfBackIcon.Visible = true;
            this.ntfBackIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ntfBackIcon_MouseDoubleClick);
            // 
            // cmsIcon
            // 
            this.cmsIcon.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F);
            this.cmsIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmShowMain,
            this.tsmExitApp});
            this.cmsIcon.Name = "cmsIcon";
            this.cmsIcon.ShowImageMargin = false;
            this.cmsIcon.Size = new System.Drawing.Size(136, 56);
            // 
            // tsmShowMain
            // 
            this.tsmShowMain.Name = "tsmShowMain";
            this.tsmShowMain.Size = new System.Drawing.Size(135, 26);
            this.tsmShowMain.Text = "显示主界面";
            this.tsmShowMain.Click += new System.EventHandler(this.tsmShowMain_Click);
            // 
            // tsmExitApp
            // 
            this.tsmExitApp.Name = "tsmExitApp";
            this.tsmExitApp.Size = new System.Drawing.Size(135, 26);
            this.tsmExitApp.Text = "退出程序";
            this.tsmExitApp.Click += new System.EventHandler(this.tsmExitApp_Click);
            // 
            // tbMbArea
            // 
            this.tbMbArea.Location = new System.Drawing.Point(114, 283);
            this.tbMbArea.Name = "tbMbArea";
            this.tbMbArea.ReadOnly = true;
            this.tbMbArea.Size = new System.Drawing.Size(58, 26);
            this.tbMbArea.TabIndex = 25;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 288);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 16);
            this.label7.TabIndex = 24;
            this.label7.Text = "MODBUS地址：";
            // 
            // tbMbAddress
            // 
            this.tbMbAddress.Location = new System.Drawing.Point(205, 283);
            this.tbMbAddress.Name = "tbMbAddress";
            this.tbMbAddress.ReadOnly = true;
            this.tbMbAddress.Size = new System.Drawing.Size(124, 26);
            this.tbMbAddress.TabIndex = 27;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(177, 288);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 16);
            this.label8.TabIndex = 26;
            this.label8.Text = "区";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 455);
            this.Controls.Add(this.gbTreeView);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tsMainForm);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OPCUA";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tsMainForm.ResumeLayout(false);
            this.tsMainForm.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.gbDataDetail.ResumeLayout(false);
            this.pnlDataDetail.ResumeLayout(false);
            this.pnlDataDetail.PerformLayout();
            this.gbTreeView.ResumeLayout(false);
            this.cmsIcon.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsMainForm;
        private System.Windows.Forms.ToolStripButton tssStartServer;
        private System.Windows.Forms.ToolStripButton tssStopServer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tssAddMember;
        private System.Windows.Forms.ToolStripButton tssEditSelect;
        private System.Windows.Forms.ToolStripButton tssAddGroup;
        private System.Windows.Forms.ToolStripButton tssDelSelect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tssDeviceManage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox gbTreeView;
        private System.Windows.Forms.TreeView tvDataMap;
        private System.Windows.Forms.ImageList imglTreeIcom;
        private System.Windows.Forms.NotifyIcon ntfBackIcon;
        private System.Windows.Forms.ContextMenuStrip cmsIcon;
        private System.Windows.Forms.ToolStripMenuItem tsmShowMain;
        private System.Windows.Forms.ToolStripMenuItem tsmExitApp;
        private System.Windows.Forms.GroupBox gbDataDetail;
        private System.Windows.Forms.Panel pnlDataDetail;
        private System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbDeviceState;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbDeviceName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDataName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbMbAddress;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbMbArea;
        private System.Windows.Forms.Label label7;
    }
}

