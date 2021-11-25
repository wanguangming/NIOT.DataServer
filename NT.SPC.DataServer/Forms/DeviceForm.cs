using NT.SPC.DataBank;
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
    public partial class DeviceForm : Form
    {
        #region 字段、属性与构造方法
        public DeviceForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 设备配置
        /// </summary>
        private DeviceConfigDal deviceConfigDal = Cache.DeviceConfigDal;
        /// <summary>
        /// 保存数据事件
        /// </summary>
        public event EventHandler OnSave;
        #endregion

        #region 窗口事件
        /// <summary>
        /// 串口载入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceForm_Load(object sender, EventArgs e)
        {
            this.ShowDataGrid(Cache.DeviceConfigs.Values.ToList());
        }
        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cache.MessageForm.ShowDialogMessage("改动需要重启服务才能生效！", "信息");
            OnSave?.Invoke(null, null);
        }
        #endregion

        #region 数据

        /// <summary>
        /// 展示树形图
        /// </summary>
        /// <param name="groups">分组数据</param>
        private void ShowDataGrid(List<DeviceConfigEntity> devices)
        {
            if (devices == null)
            {
                return;
            }
            if (this.dgvDevice.InvokeRequired)
            {
                // 很帅的调自己
                this.dgvDevice.Invoke(new Action(() => { this.ShowDataGrid(devices); }));
            }
            else
            {
                string selectName = this.dgvDevice.CurrentRow == null ? string.Empty : this.dgvDevice.CurrentRow.Cells["DeviceName"].Value.ToString();
                devices = devices.OrderBy(s => s.CustomName).ToList();
                this.dgvDevice.Rows.Clear();
                foreach (var device in devices)
                {
                    int index = this.dgvDevice.Rows.Add();
                    this.dgvDevice.Rows[index].Cells["Index"].Value = index + 1;
                    this.dgvDevice.Rows[index].Cells["DeviceName"].Value = device.CustomName;
                    if (device.CustomName.Equals(selectName))
                    {
                        this.dgvDevice.CurrentCell = this.dgvDevice.Rows[index].Cells["Index"];
                        this.dgvDevice.Rows[index].Selected = true;
                    }
                    this.dgvDevice.Rows[index].Cells["DriverName"].Value = device.DriverName;
                    if (device.Enable)
                    {
                        this.dgvDevice.Rows[index].Cells["IsEnable"].Value = "已启用";
                    }
                    else
                    {
                        this.dgvDevice.Rows[index].Cells["IsEnable"].Value = "停用中";
                    }
                    if (Cache.RunDevices.ContainsKey(device.DeviceGuid))
                    {
                        this.dgvDevice.Rows[index].Cells["State"].Value = "使用中";
                    }
                    else
                    {
                        IDeviceDriver runDevice = DriverFactory.CreateDevice(device.DriverName);
                        bool isConnect = false;
                        try
                        {
                            runDevice.SetConnectId(device.DeviceGuid);
                            runDevice.LoadConfig();
                            isConnect = runDevice.CheckConnect(runDevice.GetConnectString());
                            runDevice.Dispose();
                        }
                        catch { }
                        if (isConnect)
                        {
                            this.dgvDevice.Rows[index].Cells["State"].Value = "连接可用";
                        }
                        else
                        {
                            this.dgvDevice.Rows[index].Cells["State"].Value = "连接异常";
                        }
                    }
                }
            }
        }
        #endregion

        #region 按钮操作
        /// <summary>
        /// 点击启用设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssDeviceEnable_Click(object sender, EventArgs e)
        {
            if (this.dgvDevice.CurrentRow == null)
            {
                Cache.MessageForm.ShowDialogMessage("请选择表格中有效的行！", "错误");
                return;
            }
            var deviceConfig = this.GetSelectDevice();
            if (deviceConfig != null)
            {
                //启用连接
                IDeviceDriver deviceDriver = DriverFactory.CreateDevice(deviceConfig.DriverName);
                deviceDriver.SetConnectId(deviceConfig.DeviceGuid);
                deviceDriver.LoadConfig();
                deviceConfig.Enable = true;
                this.RefreshDataGridHandler(null, null);
            }
            else
            {
                Cache.MessageForm.ShowDialogMessage("无效的设备配置！", "错误");
            }
        }
        /// <summary>
        /// 点击停用设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssDeviceDisable_Click(object sender, EventArgs e)
        {
            if (this.dgvDevice.CurrentRow == null)
            {
                Cache.MessageForm.ShowDialogMessage("请选择表格中有效的行！", "错误");
                return;
            }
            var result = Cache.MessageForm.ShowOkCancelDialog("确定要停用选中设备吗？", "提示");
            if (result == DialogResult.OK)
            {
                var deviceConfig = this.GetSelectDevice();
                if (deviceConfig != null)
                {
                    //停用连接
                    deviceConfig.Enable = false;
                    this.RefreshDataGridHandler(null, null);
                }
                else
                {
                    Cache.MessageForm.ShowDialogMessage("无效的设备配置！", "错误");
                }
            }
        }
        /// <summary>
        /// 点击添加设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssAddDevice_Click(object sender, EventArgs e)
        {
            DeviceConfigForm deviceConfigForm = new DeviceConfigForm();
            deviceConfigForm.OnSave += this.RefreshDataGridHandler;
            deviceConfigForm.ShowDialog();
        }
        /// <summary>
        /// 点击编辑设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssEditDevice_Click(object sender, EventArgs e)
        {
            DeviceConfigEntity deviceConfigEntity = this.GetSelectDevice();
            if (deviceConfigEntity == null)
            {
                Cache.MessageForm.ShowDialogMessage("无效的选中行！", "错误");
            }
            else
            {
                DeviceConfigForm deviceConfigForm = new DeviceConfigForm();
                deviceConfigForm.OnSave += this.RefreshDataGridHandler;
                deviceConfigForm.DeviceConfig = deviceConfigEntity;
                deviceConfigForm.ShowDialog();
            }
        }
        /// <summary>
        /// 点击删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssDelDevice_Click(object sender, EventArgs e)
        {
            DeviceConfigEntity deviceConfigEntity = this.GetSelectDevice();
            if (deviceConfigEntity == null)
            {
                Cache.MessageForm.ShowDialogMessage("无效的选中行！", "错误");
                return;
            }
            var result = Cache.MessageForm.ShowOkCancelDialog("确定要删除选中设备吗？", "提示");
            if (result == DialogResult.OK)
            {
                Cache.DeviceConfigs.TryRemove(deviceConfigEntity.DeviceGuid, out deviceConfigEntity);
                this.ShowDataGrid(Cache.DeviceConfigs.Values.ToList());
            }
        }
        /// <summary>
        /// 点击关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssCloseForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 私有调用
        /// <summary>
        /// 获取选中行的设备配置对象
        /// </summary>
        /// <returns></returns>
        private DeviceConfigEntity GetSelectDevice()
        {
            DeviceConfigEntity deviceConfig = null;
            if (this.dgvDevice.CurrentRow != null)
            {
                //int index = this.dgvDevice.CurrentRow.Index;
                var deviceName = this.dgvDevice.CurrentRow.Cells["DeviceName"].Value;
                if (deviceName != null)
                {
                    deviceConfig = Cache.DeviceConfigs.Values.Where(s => s.CustomName.Equals(deviceName.ToString())).FirstOrDefault();
                }
            }
            return deviceConfig;
        }
        /// <summary>
        /// 修改、编辑后刷新数据节点树
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDataGridHandler(object sender, EventArgs e)
        {
            var list = Cache.DeviceConfigs.Values.ToList();
            if (list.Count > 0)
            {
                list = list.OrderBy(s=>s.CustomName).ToList();
            }
            this.ShowDataGrid(list);
            deviceConfigDal.SaveDeviceConfig(list);
        }

        #endregion
    }
}
