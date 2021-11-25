namespace NT.SPC.DataServer
{
    partial class NodeForm
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
            this.tbNodeName = new System.Windows.Forms.TextBox();
            this.lblNodeName = new System.Windows.Forms.Label();
            this.lblDeviceName = new System.Windows.Forms.Label();
            this.lblDataName = new System.Windows.Forms.Label();
            this.cbDeviceName = new System.Windows.Forms.ComboBox();
            this.cbDataName = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbNodeName
            // 
            this.tbNodeName.Location = new System.Drawing.Point(78, 22);
            this.tbNodeName.Name = "tbNodeName";
            this.tbNodeName.Size = new System.Drawing.Size(211, 26);
            this.tbNodeName.TabIndex = 5;
            // 
            // lblNodeName
            // 
            this.lblNodeName.AutoSize = true;
            this.lblNodeName.Location = new System.Drawing.Point(21, 26);
            this.lblNodeName.Name = "lblNodeName";
            this.lblNodeName.Size = new System.Drawing.Size(56, 16);
            this.lblNodeName.TabIndex = 4;
            this.lblNodeName.Text = "名称：";
            // 
            // lblDeviceName
            // 
            this.lblDeviceName.AutoSize = true;
            this.lblDeviceName.Location = new System.Drawing.Point(21, 61);
            this.lblDeviceName.Name = "lblDeviceName";
            this.lblDeviceName.Size = new System.Drawing.Size(56, 16);
            this.lblDeviceName.TabIndex = 8;
            this.lblDeviceName.Text = "设备：";
            // 
            // lblDataName
            // 
            this.lblDataName.AutoSize = true;
            this.lblDataName.Location = new System.Drawing.Point(21, 95);
            this.lblDataName.Name = "lblDataName";
            this.lblDataName.Size = new System.Drawing.Size(56, 16);
            this.lblDataName.TabIndex = 9;
            this.lblDataName.Text = "数据：";
            // 
            // cbDeviceName
            // 
            this.cbDeviceName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDeviceName.FormattingEnabled = true;
            this.cbDeviceName.Location = new System.Drawing.Point(78, 58);
            this.cbDeviceName.Name = "cbDeviceName";
            this.cbDeviceName.Size = new System.Drawing.Size(211, 24);
            this.cbDeviceName.TabIndex = 10;
            this.cbDeviceName.TextChanged += new System.EventHandler(this.cbDeviceName_TextChanged);
            // 
            // cbDataName
            // 
            this.cbDataName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataName.FormattingEnabled = true;
            this.cbDataName.Location = new System.Drawing.Point(78, 92);
            this.cbDataName.Name = "cbDataName";
            this.cbDataName.Size = new System.Drawing.Size(211, 24);
            this.cbDataName.TabIndex = 11;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(175, 134);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 35);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(64, 134);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(88, 35);
            this.btnConfirm.TabIndex = 12;
            this.btnConfirm.Text = "确定";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // NodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 186);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.cbDataName);
            this.Controls.Add(this.cbDeviceName);
            this.Controls.Add(this.lblDataName);
            this.Controls.Add(this.lblDeviceName);
            this.Controls.Add(this.tbNodeName);
            this.Controls.Add(this.lblNodeName);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "NodeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "数据节点";
            this.Load += new System.EventHandler(this.NodeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbNodeName;
        private System.Windows.Forms.Label lblNodeName;
        private System.Windows.Forms.Label lblDeviceName;
        private System.Windows.Forms.Label lblDataName;
        private System.Windows.Forms.ComboBox cbDeviceName;
        private System.Windows.Forms.ComboBox cbDataName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
    }
}