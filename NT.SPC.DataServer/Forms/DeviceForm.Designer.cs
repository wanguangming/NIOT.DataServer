namespace NT.SPC.DataServer
{
    partial class DeviceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tsMainForm = new System.Windows.Forms.ToolStrip();
            this.tssDeviceEnable = new System.Windows.Forms.ToolStripButton();
            this.tssDeviceDisable = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tssAddDevice = new System.Windows.Forms.ToolStripButton();
            this.tssEditDevice = new System.Windows.Forms.ToolStripButton();
            this.tssDelDevice = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tssCloseForm = new System.Windows.Forms.ToolStripButton();
            this.dgvDevice = new System.Windows.Forms.DataGridView();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeviceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DriverName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsEnable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsMainForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevice)).BeginInit();
            this.SuspendLayout();
            // 
            // tsMainForm
            // 
            this.tsMainForm.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tsMainForm.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsMainForm.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.tsMainForm.ImeMode = System.Windows.Forms.ImeMode.HangulFull;
            this.tsMainForm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssDeviceEnable,
            this.tssDeviceDisable,
            this.toolStripSeparator1,
            this.tssAddDevice,
            this.tssEditDevice,
            this.tssDelDevice,
            this.toolStripSeparator2,
            this.tssCloseForm});
            this.tsMainForm.Location = new System.Drawing.Point(0, 0);
            this.tsMainForm.Name = "tsMainForm";
            this.tsMainForm.Size = new System.Drawing.Size(720, 39);
            this.tsMainForm.TabIndex = 7;
            this.tsMainForm.Text = "主页工具栏";
            // 
            // tssDeviceEnable
            // 
            this.tssDeviceEnable.BackColor = System.Drawing.SystemColors.Control;
            this.tssDeviceEnable.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tssDeviceEnable.Image = ((System.Drawing.Image)(resources.GetObject("tssDeviceEnable.Image")));
            this.tssDeviceEnable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssDeviceEnable.Name = "tssDeviceEnable";
            this.tssDeviceEnable.Size = new System.Drawing.Size(101, 36);
            this.tssDeviceEnable.Text = "启用设备";
            this.tssDeviceEnable.Click += new System.EventHandler(this.tssDeviceEnable_Click);
            // 
            // tssDeviceDisable
            // 
            this.tssDeviceDisable.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tssDeviceDisable.Image = ((System.Drawing.Image)(resources.GetObject("tssDeviceDisable.Image")));
            this.tssDeviceDisable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssDeviceDisable.Name = "tssDeviceDisable";
            this.tssDeviceDisable.Size = new System.Drawing.Size(101, 36);
            this.tssDeviceDisable.Text = "停用设备";
            this.tssDeviceDisable.Click += new System.EventHandler(this.tssDeviceDisable_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator1.MergeIndex = 2;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // tssAddDevice
            // 
            this.tssAddDevice.Image = ((System.Drawing.Image)(resources.GetObject("tssAddDevice.Image")));
            this.tssAddDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssAddDevice.Name = "tssAddDevice";
            this.tssAddDevice.Size = new System.Drawing.Size(101, 36);
            this.tssAddDevice.Text = "添加设备";
            this.tssAddDevice.Click += new System.EventHandler(this.tssAddDevice_Click);
            // 
            // tssEditDevice
            // 
            this.tssEditDevice.Image = ((System.Drawing.Image)(resources.GetObject("tssEditDevice.Image")));
            this.tssEditDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssEditDevice.Name = "tssEditDevice";
            this.tssEditDevice.Size = new System.Drawing.Size(101, 36);
            this.tssEditDevice.Text = "编辑设备";
            this.tssEditDevice.Click += new System.EventHandler(this.tssEditDevice_Click);
            // 
            // tssDelDevice
            // 
            this.tssDelDevice.Image = ((System.Drawing.Image)(resources.GetObject("tssDelDevice.Image")));
            this.tssDelDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssDelDevice.Name = "tssDelDevice";
            this.tssDelDevice.Size = new System.Drawing.Size(101, 36);
            this.tssDelDevice.Text = "移除设备";
            this.tssDelDevice.Click += new System.EventHandler(this.tssDelDevice_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // tssCloseForm
            // 
            this.tssCloseForm.Image = ((System.Drawing.Image)(resources.GetObject("tssCloseForm.Image")));
            this.tssCloseForm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssCloseForm.Name = "tssCloseForm";
            this.tssCloseForm.Size = new System.Drawing.Size(101, 36);
            this.tssCloseForm.Text = "关闭窗口";
            this.tssCloseForm.Click += new System.EventHandler(this.tssCloseForm_Click);
            // 
            // dgvDevice
            // 
            this.dgvDevice.AllowUserToAddRows = false;
            this.dgvDevice.AllowUserToDeleteRows = false;
            this.dgvDevice.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDevice.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDevice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDevice.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.DeviceName,
            this.DriverName,
            this.IsEnable,
            this.State});
            this.dgvDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDevice.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvDevice.Location = new System.Drawing.Point(0, 39);
            this.dgvDevice.MultiSelect = false;
            this.dgvDevice.Name = "dgvDevice";
            this.dgvDevice.ReadOnly = true;
            this.dgvDevice.RowHeadersVisible = false;
            this.dgvDevice.RowTemplate.Height = 23;
            this.dgvDevice.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDevice.Size = new System.Drawing.Size(720, 309);
            this.dgvDevice.TabIndex = 8;
            // 
            // Index
            // 
            this.Index.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Index.DataPropertyName = "Index";
            this.Index.FillWeight = 40F;
            this.Index.HeaderText = "序号";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            // 
            // DeviceName
            // 
            this.DeviceName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DeviceName.DataPropertyName = "DeviceName";
            this.DeviceName.HeaderText = "设备名称";
            this.DeviceName.Name = "DeviceName";
            this.DeviceName.ReadOnly = true;
            // 
            // DriverName
            // 
            this.DriverName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DriverName.DataPropertyName = "DriverName";
            this.DriverName.HeaderText = "设备类型";
            this.DriverName.Name = "DriverName";
            this.DriverName.ReadOnly = true;
            // 
            // IsEnable
            // 
            this.IsEnable.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.IsEnable.DataPropertyName = "IsEnable";
            this.IsEnable.FillWeight = 60F;
            this.IsEnable.HeaderText = "是否启用";
            this.IsEnable.Name = "IsEnable";
            this.IsEnable.ReadOnly = true;
            // 
            // State
            // 
            this.State.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.State.DataPropertyName = "State";
            this.State.FillWeight = 60F;
            this.State.HeaderText = "连接状态";
            this.State.Name = "State";
            this.State.ReadOnly = true;
            // 
            // DeviceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 348);
            this.Controls.Add(this.dgvDevice);
            this.Controls.Add(this.tsMainForm);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DeviceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "设备管理";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DeviceForm_FormClosed);
            this.Load += new System.EventHandler(this.DeviceForm_Load);
            this.tsMainForm.ResumeLayout(false);
            this.tsMainForm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevice)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsMainForm;
        private System.Windows.Forms.ToolStripButton tssDeviceEnable;
        private System.Windows.Forms.ToolStripButton tssDeviceDisable;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tssAddDevice;
        private System.Windows.Forms.ToolStripButton tssDelDevice;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tssCloseForm;
        private System.Windows.Forms.DataGridView dgvDevice;
        private System.Windows.Forms.ToolStripButton tssEditDevice;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DriverName;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsEnable;
        private System.Windows.Forms.DataGridViewTextBoxColumn State;
    }
}