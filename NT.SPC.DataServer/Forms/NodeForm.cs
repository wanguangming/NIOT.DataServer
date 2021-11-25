using NT.SPC.DataBank;
using NT.SPC.ServerConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT.SPC.DataServer
{
    public partial class NodeForm : Form
    {
        #region 字段、属性与构造方法
        public NodeForm(GroupConfigEntity groupConfig)
        {
            InitializeComponent();
            this.GroupConfig = groupConfig;
        }
        //private Dictionary<string, IDeviceDriver> DeviceDirver = new Dictionary<string, IDeviceDriver>();
        /// <summary>
        /// 保存数据事件
        /// </summary>
        public event EventHandler OnSave;
        /// <summary>
        /// 数据节点对象实体
        /// </summary>
        public NodeConfigEntity NodeConfig { get; set; }
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
        private void NodeForm_Load(object sender, EventArgs e)
        {
            if (Cache.DeviceConfigs.Count == 0)
            {
                this.cbDeviceName.Text = "请添加设备";
                this.cbDataName.Text = "";
                this.btnConfirm.Enabled = false;
            }
            else
            {
                this.cbDeviceName.Items.Clear();
                this.cbDeviceName.Items.AddRange(Cache.DeviceConfigs.Values.Where(s => s.Enable).Select(s => s.CustomName).ToArray());
                this.cbDeviceName.SelectedIndex = 0;
                this.btnConfirm.Enabled = true;
            }
            if (NodeConfig == null)
            {
                this.Text = "添加数据节点";
                NodeConfig = new NodeConfigEntity();
                NodeConfig.Id = Guid.NewGuid().ToString();
                NodeConfig.GroupId = this.GroupConfig.Id;
                GroupConfig.Nodes.Add(NodeConfig);
                this.IsEdit = false;
            }
            else
            {
                this.Text = "修改数据节点";
                this.tbNodeName.Text = NodeConfig.Name;
                var deviceConfig = Cache.DeviceConfigs.Values.Where(s => s.DeviceGuid.Equals(NodeConfig.DeviceGuid)).FirstOrDefault();
                if (deviceConfig != null)
                {
                    this.cbDeviceName.Text = deviceConfig.CustomName;
                    this.cbDataName.Text = NodeConfig.DataName;
                    this.IsEdit = true;
                }
            }
        }
        /// <summary>
        /// 点击确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tbNodeName.Text))
            {
                Cache.MessageForm.ShowDialogMessage("数据名称不能为空！", "错误");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.cbDeviceName.Text) || this.cbDeviceName.Text.Equals("请添加设备"))
            {
                Cache.MessageForm.ShowDialogMessage("请选择有效的来源设备！", "错误");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.cbDataName.Text))
            {
                Cache.MessageForm.ShowDialogMessage("请选择有效的数据源！", "错误");
                return;
            }
            var alike = GroupConfig.Nodes.Where(s => s.Name != null && s.Name.Equals(this.tbNodeName.Text) && !s.Id.Equals(this.NodeConfig.Id));
            if (alike != null && alike.Count() > 0)
            {
                Cache.MessageForm.ShowDialogMessage("已有相同的节点名称！", "错误");
                return;
            }
            NodeConfig.Name = this.tbNodeName.Text;
            string deviceName = this.cbDeviceName.Text;
            var deviceConfig = Cache.DeviceConfigs.Values.Where(s => s.CustomName.Equals(deviceName)).FirstOrDefault();
            if (deviceConfig == null)
            {
                Cache.MessageForm.ShowDialogMessage("选择的设备无效！", "错误");
                return;
            }
            NodeConfig.DeviceGuid = deviceConfig.DeviceGuid;
            NodeConfig.DataName = this.cbDataName.Text;
            IDeviceDriver device = DriverFactory.CreateDevice(deviceConfig.DriverName);
            device.SetConnectId(deviceConfig.DeviceGuid);
            device.LoadConfig();
            NodeConfig.DataType = device.GetDataType(NodeConfig.DataName);
            NodeConfig.RegLength = device.GetDataLength(NodeConfig.DataName);
            device.Dispose();
            if (!IsEdit)
            {
                List<NodeConfigEntity> nodes = new List<NodeConfigEntity>();
                foreach (var group in Cache.GroupConfigs.Values)
                {
                    nodes.AddRange(group.Nodes);
                }
                if (NodeConfig.DataType.Equals("bool"))
                {
                    NodeConfig.Area = 0;
                    var configs = nodes.Where(t => t.Area == 0).OrderBy(t => t.RegAddress);
                    if (configs != null && configs.Count() > 0)
                    {
                        var lastConfig = configs.Last();
                        NodeConfig.RegAddress = lastConfig.RegAddress + lastConfig.RegLength;
                    }
                    else
                    {
                        NodeConfig.RegAddress = 1;
                    }
                }
                else
                {
                    NodeConfig.Area = 4;
                    var configs = nodes.Where(t => t.Area == 4).OrderBy(t => t.RegAddress);
                    if (configs != null && configs.Count() > 0)
                    {
                        var lastConfig = configs.Last();
                        NodeConfig.RegAddress = lastConfig.RegAddress + lastConfig.RegLength;
                    }
                    else
                    {
                        NodeConfig.RegAddress = 1;
                    }
                }

            }
            GroupConfig.Nodes = GroupConfig.Nodes.Where(s => !string.IsNullOrWhiteSpace(s.Name) && s.GroupId.Equals(GroupConfig.Id)).ToList();
            OnSave?.Invoke(null, null);
            this.Close();
        }
        /// <summary>
        /// 点击取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.OnSave = null;
            this.Close();
        }
        /// <summary>
        /// 变更设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbDeviceName_TextChanged(object sender, EventArgs e)
        {
            string deviceName = this.cbDeviceName.Text;
            if (!string.IsNullOrWhiteSpace(deviceName))
            {
                var deviceConfig = Cache.DeviceConfigs.Values.Where(s => s.CustomName.Equals(deviceName)).FirstOrDefault();
                if (deviceConfig != null)
                {
                    IDeviceDriver device = DriverFactory.CreateDevice(deviceConfig.DriverName);
                    device.SetConnectId(deviceConfig.DeviceGuid);
                    device.LoadConfig();
                    var dataNames = device.GetColloctData();
                    this.cbDataName.Items.Clear();
                    this.cbDataName.Items.AddRange(dataNames);
                    this.cbDataName.SelectedIndex = 0;
                    device.Dispose();
                }
            }
            else
            {
                if (Cache.DeviceConfigs.Count != 0)
                {
                    Cache.MessageForm.ShowDialogMessage("选择的设备无效！", "错误");
                }
                else
                {
                    this.cbDeviceName.Text = "请添加设备";
                    this.cbDataName.Text = "";
                }
            }
        }
    }
}
