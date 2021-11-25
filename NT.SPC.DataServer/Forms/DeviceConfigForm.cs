using NT.SPC.DataBank;
using NT.SPC.ServerConfig;
using NT.Tools.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT.SPC.DataServer
{
    public partial class DeviceConfigForm : Form
    {
        #region 字段、属性与构造方法
        public DeviceConfigForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 设备配置对象实体
        /// </summary>
        public DeviceConfigEntity DeviceConfig { get; set; }
        /// <summary>
        /// 是否编辑
        /// </summary>
        public bool IsEdit { get; private set; }
        /// <summary>
        /// 保存数据事件
        /// </summary>
        public event EventHandler OnSave;
        private Dictionary<string, string> _ConnectString { get; set; }

        private List<Dictionary<string, string>> _AddressString { get; set; }

        private List<string> _RowAddrs { get; set; }
        #endregion

        #region 窗口事件
        /// <summary>
        /// 窗口载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceConfigForm_Load(object sender, EventArgs e)
        {
            //检查亦有驱动
            List<string> drivers = DriverFactory.GetSupportDevices();
            if (drivers == null || drivers.Count == 0)
            {
                this.cbDriverName.Text = "请添加设备驱动";
                this.btnConfirm.Enabled = false;
            }
            else
            {
                this.cbDriverName.Items.Clear();
                this.cbDriverName.Items.AddRange(drivers.ToArray());
                this.btnConfirm.Enabled = true;
            }
            //初始化表单
            if (DeviceConfig == null)
            {
                this.IsEdit = false;
                this.Text = "添加设备";
                DeviceConfig = new DeviceConfigEntity();
                DeviceConfig.DeviceGuid = Guid.NewGuid().ToString();
                DeviceConfig.CustomName = string.Empty;
                this.cbDriverName.SelectedIndex = 0;
            }
            else
            {
                this.IsEdit = true;
                this.Text = "修改设备";
                this.tbDeviceName.Text = DeviceConfig.CustomName;
                this.cbDriverName.Text = DeviceConfig.DriverName;
                this.cbDriverName.Enabled = false;
            }
        }
        /// <summary>
        /// 更换设备驱动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbDriverName_TextChanged(object sender, EventArgs e)
        {
            string driverName = this.cbDriverName.Text;
            if (!string.IsNullOrWhiteSpace(driverName) && !driverName.Equals("请添加设备驱动"))
            {
                IDeviceDriver deviceDriver = DriverFactory.CreateDevice(driverName);
                if (deviceDriver != null)
                {
                    if (IsEdit)
                    {
                        deviceDriver.SetConnectId(DeviceConfig.DeviceGuid);
                        deviceDriver.LoadConfig();
                    }
                    else
                    {
                        deviceDriver.LoadDefaultConfig();
                    }
                    string connectStr = deviceDriver.GetConnectString();
                    if (!string.IsNullOrWhiteSpace(connectStr))
                    {
                        this._ConnectString = connectStr.ToObject<Dictionary<string, string>>();
                        this.ShowConnectString();
                    }
                    else
                    {
                        Cache.MessageForm.ShowDialogMessage("该驱动配置文件异常！");
                    }
                    string addressStr = deviceDriver.GetAddressString();
                    if (!string.IsNullOrWhiteSpace(addressStr))
                    {
                        this._AddressString = addressStr.ToObject<List<Dictionary<string, string>>>();
                        this.ShowAddressString();
                    }
                    else
                    {
                        Cache.MessageForm.ShowDialogMessage("该驱动配置文件异常！");
                    }
                }
            }
        }
        /// <summary>
        /// 单击展示数据地址配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAddress_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string value = GetSelectCellValue();
            if (string.IsNullOrWhiteSpace(value))
            {
                this.pnlAddressDetail.Visible = false;
                return;
            }
            if (AddressContains(value))
            {
                var dict = GetAddressByName(value);
                if (dict.Count <= 1)
                {
                    InitAddressConfig();
                }
                else
                {
                    ShowAddressConfig(dict);
                }
            }
            else
            {
                InitAddressConfig();
            }
        }
        /// <summary>
        /// 双击修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAddress_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvAddress.BeginEdit(false);
            if (this.dgvAddress.CurrentCell.Value == null)
            {
                InitAddressConfig();
            }
        }
        /// <summary>
        /// 修改值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAddress_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            if (dgvAddress.Rows[e.RowIndex].Cells["DataName"].Value != null)
            {
                if (dgvAddress.Rows.Count - 1 > _RowAddrs.Count)
                {
                    if (e.RowIndex == dgvAddress.Rows.Count - 2)
                    {
                        string name = dgvAddress.Rows[e.RowIndex].Cells["DataName"].Value.ToString();
                        _RowAddrs.Add(name);
                        Dictionary<string, string> addressConfig = new Dictionary<string, string>();
                        addressConfig["DataName"] = name;
                        _AddressString.Add(addressConfig);
                    }
                }
                else
                {
                    string oldName = _RowAddrs[e.RowIndex];
                    string newName = dgvAddress.Rows[e.RowIndex].Cells["DataName"].Value.ToString();
                    _RowAddrs[e.RowIndex] = newName;
                    var addressConfig = GetAddressByName(oldName);
                    addressConfig["DataName"] = newName;
                }
            }
        }
        #endregion

        #region 按钮操作
        /// <summary>
        /// 打开配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfigFile_Click(object sender, EventArgs e)
        {
            IDeviceDriver deviceDriver = this.GetSelectDriver();
            deviceDriver.SetConnectId(DeviceConfig.DeviceGuid);
            string filePath = deviceDriver.GetConfigPath();
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                System.Diagnostics.Process.Start("notepad.exe", filePath);
            }
            else
            {
                Cache.MessageForm.ShowDialogMessage("此设备未保存配置文件！", "错误");
            }
        }
        /// <summary>
        /// 点击测试按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestConnect_Click(object sender, EventArgs e)
        {
            IDeviceDriver deviceDriver = this.GetSelectDriver();
            if (deviceDriver != null)
            {
                bool result = deviceDriver.CheckConnect(_ConnectString.ToString());
                if (result)
                {
                    Cache.MessageForm.ShowDialogMessage("连接成功！", "信息");
                }
                else
                {
                    Cache.MessageForm.ShowDialogMessage("连接失败！", "错误");
                }
            }
            deviceDriver.Dispose();
        }
        /// <summary>
        /// 点击确认按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tbDeviceName.Text))
            {
                Cache.MessageForm.ShowDialogMessage("设备名称不能为空！", "错误");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.cbDriverName.Text) || this.cbDriverName.Text.Equals("请添加设备驱动"))
            {
                Cache.MessageForm.ShowDialogMessage("请选择有效的驱动程序！", "错误");
                return;
            }
            var alike = Cache.DeviceConfigs.Values.Where(s => s.CustomName.Equals(this.tbDeviceName.Text) && !s.DeviceGuid.Equals(DeviceConfig.DeviceGuid));
            if (alike != null && alike.Count() > 0)
            {
                Cache.MessageForm.ShowDialogMessage("已有相同的设备名称！", "错误");
                return;
            }
            DeviceConfig.CustomName = this.tbDeviceName.Text;
            DeviceConfig.DriverName = this.cbDriverName.Text;
            DeviceConfig.Enable = true;
            //保存配置
            GetConnectConfigForm();
            IDeviceDriver deviceDriver = this.GetSelectDriver();
            deviceDriver.SetConnectId(DeviceConfig.DeviceGuid);
            if (!IsEdit)
            {
                Cache.DeviceConfigs.TryAdd(this.DeviceConfig.DeviceGuid, this.DeviceConfig);
            }
            //保存设备配置
            deviceDriver.SetConnectString(this._ConnectString.ToJson());
            deviceDriver.SetAddressString(this._AddressString.ToJson());
            deviceDriver.SaveConfig();
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
            this.Close();
        }
        /// <summary>
        /// 删除数据项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelAddress_Click(object sender, EventArgs e)
        {
            if (dgvAddress.CurrentRow != null)
            {
                int rowIndex = dgvAddress.CurrentRow.Index;
                string key = this.GetSelectCellValue();
                if (AddressContains(key))
                {
                    _RowAddrs.Remove(key);
                    _AddressString.Remove(GetAddressByName(key));
                }
                dgvAddress.Rows.RemoveAt(rowIndex);
            }
        }
        /// <summary>
        /// 保存数据项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveAddress_Click(object sender, EventArgs e)
        {
            string prefix = "ast_";
            string name = this.GetSelectCellValue();
            if (!string.IsNullOrWhiteSpace(name))
            {
                var addressConfig = GetAddressByName(name);
                foreach (Control control in pnlAddressDetail.Controls)
                {
                    if (control is TextBox)
                    {
                        TextBox tb = control as TextBox;
                        string field = tb.Name.Replace(prefix, "");
                        addressConfig[field] = tb.Text;
                    }
                }
            }
        }
        #endregion

        #region 私有调用
        /// <summary>
        /// 获取表单中的连接字符串
        /// </summary>
        /// <returns></returns>
        private void GetConnectConfigForm()
        {
            string prefix = "cst_";
            foreach (Control control in pnlConnectStr.Controls)
            {
                if (control is TextBox)
                {
                    TextBox tb = control as TextBox;
                    string field = tb.Name.Replace(prefix, "");
                    _ConnectString[field] = tb.Text;
                }
            }
        }
        /// <summary>
        /// 获得选择的设备驱动实例
        /// </summary>
        /// <returns></returns>
        private IDeviceDriver GetSelectDriver()
        {
            IDeviceDriver deviceDriver = null;
            string driverName = this.cbDriverName.Text;
            if (string.IsNullOrWhiteSpace(driverName))
            {
                Cache.MessageForm.ShowDialogMessage("无效的设备驱动！", "错误");
            }
            else
            {
                deviceDriver = DriverFactory.CreateDevice(driverName);
                if (deviceDriver == null)
                {
                    Cache.MessageForm.ShowDialogMessage("未注册设备驱动程序！", "错误");
                }
            }
            return deviceDriver;
        }
        /// <summary>
        /// 显示连接字符串
        /// </summary>
        /// <param name="connectString"></param>
        private void ShowConnectString()
        {
            this.pnlConnectStr.Controls.Clear();
            if (_ConnectString != null)
            {
                int beginHeight = 10;
                string prefix = "cst_";
                foreach (var key in _ConnectString.Keys)
                {
                    Label lblKey = new Label();
                    lblKey.Text = key;
                    lblKey.Size = new Size(140, 28);
                    lblKey.Location = new Point(10, beginHeight + 5);

                    TextBox tbKey = new TextBox();
                    tbKey.Name = prefix + key;
                    tbKey.Text = _ConnectString[key];
                    tbKey.Size = new Size(100, 28);
                    tbKey.Location = new Point(150, beginHeight);

                    this.pnlConnectStr.Controls.Add(lblKey);
                    this.pnlConnectStr.Controls.Add(tbKey);

                    beginHeight += 30;
                }
            }
        }
        /// <summary>
        /// 显示采集数据
        /// </summary>
        private void ShowAddressString()
        {
            _RowAddrs = new List<string>();
            this.dgvAddress.Rows.Clear();
            if (_AddressString != null && _AddressString.Count > 0)
            {
                foreach (var addressConfig in _AddressString)
                {
                    _RowAddrs.Add(addressConfig["DataName"]);
                    int rowIndex = this.dgvAddress.Rows.Add();
                    this.dgvAddress.Rows[rowIndex].Cells["DataName"].Value = addressConfig["DataName"];
                }
                this.dgvAddress.Rows[0].Selected = true;
                this.ShowAddressConfig(_AddressString[0]);
            }
        }
        /// <summary>
        /// 新的数据配置
        /// </summary>
        private void InitAddressConfig()
        {
            this.pnlAddressDetail.Controls.Clear();
            IDeviceDriver device = this.GetSelectDriver();
            if (device != null)
            {
                device.LoadDefaultConfig();
                var addressString = device.GetAddressString();
                if (!string.IsNullOrWhiteSpace(addressString))
                {
                    var addressConfigs = addressString.ToObject<List<Dictionary<string, string>>>();
                    this.ShowAddressConfig(addressConfigs.First());
                }
                device.Dispose();
            }
        }
        /// <summary>
        /// 显示数据配置
        /// </summary>
        /// <param name="configs"></param>
        private void ShowAddressConfig(Dictionary<string, string> configs)
        {
            this.pnlAddressDetail.Controls.Clear();
            if (configs != null)
            {
                this.pnlAddressDetail.Visible = true;
                int beginHeight = 10;
                string prefix = "ast_";
                foreach (var key in configs.Keys)
                {
                    if (key.Equals("DataName"))
                    {
                        continue;
                    }
                    Label lblKey = new Label();
                    lblKey.Text = key;
                    lblKey.Size = new Size(100, 28);
                    lblKey.Location = new Point(10, beginHeight + 5);

                    TextBox tbKey = new TextBox();
                    tbKey.Name = prefix + key;
                    tbKey.Text = configs[key];
                    tbKey.Size = new Size(80, 28);
                    tbKey.Location = new Point(110, beginHeight);

                    this.pnlAddressDetail.Controls.Add(lblKey);
                    this.pnlAddressDetail.Controls.Add(tbKey);

                    beginHeight += 30;
                }
            }
        }
        /// <summary>
        /// 获得选择单元格的值
        /// </summary>
        /// <returns></returns>
        private string GetSelectCellValue()
        {
            string value = string.Empty;
            if (dgvAddress.CurrentRow != null)
            {
                int rowIndex = dgvAddress.CurrentRow.Index;
                var cellValue = dgvAddress.Rows[rowIndex].Cells["DataName"].Value;
                if (cellValue != null)
                {
                    value = cellValue.ToString();
                }
            }
            return value;
        }
        /// <summary>
        /// 检查是否包含数据项
        /// </summary>
        /// <param name="dataName"></param>
        /// <returns></returns>
        private bool AddressContains(string dataName)
        {
            bool result = false;
            if (_AddressString != null)
            {
                foreach (var address in _AddressString)
                {
                    if (address["DataName"].Equals(dataName))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 获取数据项的配置
        /// </summary>
        /// <param name="dataName"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetAddressByName(string dataName)
        {
            Dictionary<string, string> result = null;
            if (_AddressString != null)
            {
                foreach (var address in _AddressString)
                {
                    if (address["DataName"].Equals(dataName))
                    {
                        result = address;
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
