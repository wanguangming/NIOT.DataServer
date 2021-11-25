using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT.SPC.DataServer
{
    /// <summary>
    /// 消息提示窗口
    /// </summary>
    public partial class MessageForm : Form
    {
        public MessageForm()
        {
            InitializeComponent();
        }
        #region 公有调用
        /// <summary>
        /// 展示消息
        /// </summary>
        public void ShowMessage(string message, string title = "信息", int closeDelay = -1)
        {
            this.SetType(0);
            this.lblMessage.Text = message;
            this.Text = title;
            this.Show();
            if (closeDelay > 0)
            {
                Task.Run(new Action(() =>
                {
                    Thread.Sleep(closeDelay);
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => { this.Hide(); }));
                    }
                }));
            }
        }
        /// <summary>
        /// 展示消息对话框
        /// </summary>
        public void ShowDialogMessage(string message, string title = "信息")
        {
            this.SetType(0);
            this.lblMessage.Text = message;
            this.Text = title;
            this.ShowDialog();
        }
        /// <summary>
        /// 展示消息对话框
        /// </summary>
        public DialogResult ShowOkCancelDialog(string message, string title = "信息")
        {
            this.DialogResult = DialogResult.Cancel;
            this.SetType(1);
            this.lblMessage.Text = message;
            this.Text = title;
            return this.ShowDialog();
        }
        #endregion

        #region 按钮操作
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        /// <summary>
        /// 点击取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        /// <summary>
        /// 点击确认按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion

        #region 私有调用
        /// <summary>
        /// 设置窗口类型
        /// </summary>
        /// <param name="type"></param>
        private void SetType(int type = 0)
        {
            switch (type)
            {
                case 0:
                    this.btnClose.Show();
                    this.btnConfirm.Hide();
                    this.btnCancel.Hide();
                    break;
                case 1:
                    this.btnClose.Hide();
                    this.btnConfirm.Show();
                    this.btnCancel.Show();
                    break;
                default:
                    this.btnClose.Show();
                    this.btnConfirm.Hide();
                    this.btnCancel.Hide();
                    break;
            }
        }
        #endregion
    }
}
