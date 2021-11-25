using NT.SPC.ServerConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT.SPC.DataServer
{
    /// <summary>
    /// 分组信息编辑
    /// </summary>
    public partial class GroupForm : Form
    {
        #region 字段、属性与构造方法
        public GroupForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 保存数据事件
        /// </summary>
        public event EventHandler OnSave;
        /// <summary>
        /// 分组对象实体
        /// </summary>
        public GroupConfigEntity GroupConfig { get; set; }
        /// <summary>
        /// 是否编辑
        /// </summary>
        public bool IsEdit { get; private set; }
        #endregion
        /// <summary>
        /// 窗口载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupForm_Load(object sender, EventArgs e)
        {
            if (GroupConfig == null)
            {
                this.Text = "添加分组";
                GroupConfig = new GroupConfigEntity();
                GroupConfig.Id = Guid.NewGuid().ToString();
                this.IsEdit = false;
            }
            else
            {
                this.Text = "修改分组";
                this.tbGroupName.Text = GroupConfig.Name;
                this.IsEdit = true;
            }
        }
        /// <summary>
        /// 点击确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tbGroupName.Text))
            {
                Cache.MessageForm.ShowDialogMessage("输入内容不能为空！", "错误");
            }
            else
            {
                var alike = Cache.GroupConfigs.Values.Where(s => s.Name.Equals(this.tbGroupName.Text)&&!s.Id.Equals(GroupConfig.Id));
                if (alike != null && alike.Count() > 0)
                {
                    Cache.MessageForm.ShowDialogMessage("已有相同的分组名称！", "错误");
                    return;
                }
                GroupConfig.Name = this.tbGroupName.Text;
                if (!IsEdit)
                {
                    Cache.GroupConfigs.TryAdd(GroupConfig.Id, GroupConfig);
                }
                OnSave?.Invoke(null, null);
                this.Close();
            }
        }
        /// <summary>
        /// 点击取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
