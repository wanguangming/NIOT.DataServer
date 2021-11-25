namespace NT.SPC.DataServer
{
    partial class DeviceConfigForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnTestConnect = new System.Windows.Forms.Button();
            this.btnConfigFile = new System.Windows.Forms.Button();
            this.cbDriverName = new System.Windows.Forms.ComboBox();
            this.lblDriverName = new System.Windows.Forms.Label();
            this.tbDeviceName = new System.Windows.Forms.TextBox();
            this.lblDeviceName = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.gbAddressStr = new System.Windows.Forms.GroupBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dgvAddress = new System.Windows.Forms.DataGridView();
            this.DataName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btnSaveAddress = new System.Windows.Forms.Button();
            this.btnDelAddress = new System.Windows.Forms.Button();
            this.gbConnectStr = new System.Windows.Forms.GroupBox();
            this.pnlConnectStr = new System.Windows.Forms.Panel();
            this.gbDataDetail = new System.Windows.Forms.GroupBox();
            this.pnlAddressDetail = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.gbAddressStr.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAddress)).BeginInit();
            this.panel6.SuspendLayout();
            this.gbConnectStr.SuspendLayout();
            this.gbDataDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnCancel.Location = new System.Drawing.Point(347, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 35);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnConfirm.Location = new System.Drawing.Point(236, 3);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(88, 35);
            this.btnConfirm.TabIndex = 14;
            this.btnConfirm.Text = "确定";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnTestConnect);
            this.panel1.Controls.Add(this.btnConfigFile);
            this.panel1.Controls.Add(this.cbDriverName);
            this.panel1.Controls.Add(this.lblDriverName);
            this.panel1.Controls.Add(this.tbDeviceName);
            this.panel1.Controls.Add(this.lblDeviceName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(666, 78);
            this.panel1.TabIndex = 20;
            // 
            // btnTestConnect
            // 
            this.btnTestConnect.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnTestConnect.Location = new System.Drawing.Point(603, 12);
            this.btnTestConnect.Name = "btnTestConnect";
            this.btnTestConnect.Size = new System.Drawing.Size(52, 26);
            this.btnTestConnect.TabIndex = 21;
            this.btnTestConnect.Text = "测试";
            this.btnTestConnect.UseVisualStyleBackColor = true;
            this.btnTestConnect.Click += new System.EventHandler(this.btnTestConnect_Click);
            // 
            // btnConfigFile
            // 
            this.btnConfigFile.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnConfigFile.Location = new System.Drawing.Point(603, 43);
            this.btnConfigFile.Name = "btnConfigFile";
            this.btnConfigFile.Size = new System.Drawing.Size(52, 26);
            this.btnConfigFile.TabIndex = 23;
            this.btnConfigFile.Text = "文件";
            this.btnConfigFile.UseVisualStyleBackColor = true;
            this.btnConfigFile.Click += new System.EventHandler(this.btnConfigFile_Click);
            // 
            // cbDriverName
            // 
            this.cbDriverName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDriverName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDriverName.FormattingEnabled = true;
            this.cbDriverName.Location = new System.Drawing.Point(69, 45);
            this.cbDriverName.Name = "cbDriverName";
            this.cbDriverName.Size = new System.Drawing.Size(528, 24);
            this.cbDriverName.TabIndex = 16;
            this.cbDriverName.TextChanged += new System.EventHandler(this.cbDriverName_TextChanged);
            // 
            // lblDriverName
            // 
            this.lblDriverName.AutoSize = true;
            this.lblDriverName.Location = new System.Drawing.Point(12, 48);
            this.lblDriverName.Name = "lblDriverName";
            this.lblDriverName.Size = new System.Drawing.Size(56, 16);
            this.lblDriverName.TabIndex = 15;
            this.lblDriverName.Text = "设备：";
            // 
            // tbDeviceName
            // 
            this.tbDeviceName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDeviceName.Location = new System.Drawing.Point(69, 12);
            this.tbDeviceName.Name = "tbDeviceName";
            this.tbDeviceName.Size = new System.Drawing.Size(528, 26);
            this.tbDeviceName.TabIndex = 14;
            // 
            // lblDeviceName
            // 
            this.lblDeviceName.AutoSize = true;
            this.lblDeviceName.Location = new System.Drawing.Point(12, 16);
            this.lblDeviceName.Name = "lblDeviceName";
            this.lblDeviceName.Size = new System.Drawing.Size(56, 16);
            this.lblDeviceName.TabIndex = 13;
            this.lblDeviceName.Text = "名称：";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnConfirm);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 335);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(666, 40);
            this.panel2.TabIndex = 21;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.gbAddressStr);
            this.panel3.Controls.Add(this.gbConnectStr);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 78);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(666, 257);
            this.panel3.TabIndex = 22;
            // 
            // gbAddressStr
            // 
            this.gbAddressStr.Controls.Add(this.panel4);
            this.gbAddressStr.Controls.Add(this.gbDataDetail);
            this.gbAddressStr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbAddressStr.Location = new System.Drawing.Point(283, 0);
            this.gbAddressStr.Name = "gbAddressStr";
            this.gbAddressStr.Size = new System.Drawing.Size(383, 257);
            this.gbAddressStr.TabIndex = 2;
            this.gbAddressStr.TabStop = false;
            this.gbAddressStr.Text = "数据地址";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dgvAddress);
            this.panel4.Controls.Add(this.panel6);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 22);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(177, 232);
            this.panel4.TabIndex = 0;
            // 
            // dgvAddress
            // 
            this.dgvAddress.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAddress.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAddress.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAddress.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DataName});
            this.dgvAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAddress.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvAddress.Location = new System.Drawing.Point(0, 33);
            this.dgvAddress.Name = "dgvAddress";
            this.dgvAddress.RowHeadersVisible = false;
            this.dgvAddress.RowTemplate.Height = 23;
            this.dgvAddress.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAddress.Size = new System.Drawing.Size(177, 199);
            this.dgvAddress.TabIndex = 1;
            this.dgvAddress.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAddress_CellClick);
            this.dgvAddress.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAddress_CellDoubleClick);
            this.dgvAddress.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAddress_CellValueChanged);
            // 
            // DataName
            // 
            this.DataName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DataName.HeaderText = "数据名";
            this.DataName.Name = "DataName";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.btnSaveAddress);
            this.panel6.Controls.Add(this.btnDelAddress);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(177, 33);
            this.panel6.TabIndex = 0;
            // 
            // btnSaveAddress
            // 
            this.btnSaveAddress.Location = new System.Drawing.Point(17, 3);
            this.btnSaveAddress.Name = "btnSaveAddress";
            this.btnSaveAddress.Size = new System.Drawing.Size(59, 28);
            this.btnSaveAddress.TabIndex = 2;
            this.btnSaveAddress.Text = "保存";
            this.btnSaveAddress.UseVisualStyleBackColor = true;
            this.btnSaveAddress.Click += new System.EventHandler(this.btnSaveAddress_Click);
            // 
            // btnDelAddress
            // 
            this.btnDelAddress.Location = new System.Drawing.Point(99, 3);
            this.btnDelAddress.Name = "btnDelAddress";
            this.btnDelAddress.Size = new System.Drawing.Size(59, 28);
            this.btnDelAddress.TabIndex = 1;
            this.btnDelAddress.Text = "删除";
            this.btnDelAddress.UseVisualStyleBackColor = true;
            this.btnDelAddress.Click += new System.EventHandler(this.btnDelAddress_Click);
            // 
            // gbConnectStr
            // 
            this.gbConnectStr.Controls.Add(this.pnlConnectStr);
            this.gbConnectStr.Dock = System.Windows.Forms.DockStyle.Left;
            this.gbConnectStr.Location = new System.Drawing.Point(0, 0);
            this.gbConnectStr.Name = "gbConnectStr";
            this.gbConnectStr.Size = new System.Drawing.Size(283, 257);
            this.gbConnectStr.TabIndex = 3;
            this.gbConnectStr.TabStop = false;
            this.gbConnectStr.Text = "连接配置";
            // 
            // pnlConnectStr
            // 
            this.pnlConnectStr.AutoScroll = true;
            this.pnlConnectStr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlConnectStr.Location = new System.Drawing.Point(3, 22);
            this.pnlConnectStr.Name = "pnlConnectStr";
            this.pnlConnectStr.Size = new System.Drawing.Size(277, 232);
            this.pnlConnectStr.TabIndex = 2;
            // 
            // gbDataDetail
            // 
            this.gbDataDetail.Controls.Add(this.pnlAddressDetail);
            this.gbDataDetail.Dock = System.Windows.Forms.DockStyle.Right;
            this.gbDataDetail.Location = new System.Drawing.Point(180, 22);
            this.gbDataDetail.Name = "gbDataDetail";
            this.gbDataDetail.Size = new System.Drawing.Size(200, 232);
            this.gbDataDetail.TabIndex = 1;
            this.gbDataDetail.TabStop = false;
            this.gbDataDetail.Text = "配置";
            // 
            // pnlAddressDetail
            // 
            this.pnlAddressDetail.AutoScroll = true;
            this.pnlAddressDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAddressDetail.Location = new System.Drawing.Point(3, 22);
            this.pnlAddressDetail.Name = "pnlAddressDetail";
            this.pnlAddressDetail.Size = new System.Drawing.Size(194, 207);
            this.pnlAddressDetail.TabIndex = 2;
            // 
            // DeviceConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 375);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DeviceConfigForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "设备配置";
            this.Load += new System.EventHandler(this.DeviceConfigForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.gbAddressStr.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAddress)).EndInit();
            this.panel6.ResumeLayout(false);
            this.gbConnectStr.ResumeLayout(false);
            this.gbDataDetail.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cbDriverName;
        private System.Windows.Forms.Label lblDriverName;
        private System.Windows.Forms.TextBox tbDeviceName;
        private System.Windows.Forms.Label lblDeviceName;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnConfigFile;
        private System.Windows.Forms.Button btnTestConnect;
        private System.Windows.Forms.GroupBox gbAddressStr;
        private System.Windows.Forms.GroupBox gbConnectStr;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.DataGridView dgvAddress;
        private System.Windows.Forms.Button btnDelAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataName;
        private System.Windows.Forms.Button btnSaveAddress;
        private System.Windows.Forms.Panel pnlConnectStr;
        private System.Windows.Forms.GroupBox gbDataDetail;
        private System.Windows.Forms.Panel pnlAddressDetail;
    }
}