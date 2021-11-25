using NT.SPC.DataBank;
using NT.SPC.ServerConfig;
using NT.Tools.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT.SPC.DataServer
{
    public partial class MainForm : Form
    {
        #region 字段、属性与构造方法
        private readonly NT.Tools.Log.Log logger = NT.Tools.Log.LogFactory.GetLogger(typeof(MainForm));
        private Task _refreshTask = null;
        private bool _refreshMark = false;
        private DataDetailModel dataDetail = null;
        public MainForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 设备配置
        /// </summary>
        private DeviceConfigDal deviceConfigDal = Cache.DeviceConfigDal;
        /// <summary>
        /// 分组数据配置
        /// </summary>
        private GroupConfigDal groupConfigDal = Cache.GroupConfigDal;
        #endregion

        #region 窗口事件
        /// <summary>
        /// 窗口载入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadConfig();
                tssStartServer_Click(sender, e);
                this.Hide();
                this.ShowInTaskbar = false;
                this.ntfBackIcon.Visible = true;
                this.pnlDataDetail.Visible = false;
            }
            catch (Exception ex)
            {
                throw ex;
                //logger.Error(ex.GetOriginalException());
            }
        }
        /// <summary>
        /// 窗口将要关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                //应用程序要求关闭窗口
                case CloseReason.ApplicationExitCall:
                    e.Cancel = false; //不拦截，响应操作
                    break;
                //自身窗口上的关闭按钮
                case CloseReason.FormOwnerClosing:
                    e.Cancel = true;//拦截，不响应操作
                    break;
                //MDI窗体关闭事件
                case CloseReason.MdiFormClosing:
                    e.Cancel = true;//拦截，不响应操作
                    break;
                //不明原因的关闭
                case CloseReason.None:
                    break;
                //任务管理器关闭进程
                case CloseReason.TaskManagerClosing:
                    e.Cancel = false;//不拦截，响应操作
                    break;
                //用户通过UI关闭窗口或者通过Alt+F4关闭窗口
                case CloseReason.UserClosing:
                    e.Cancel = true;//拦截，不响应操作
                    break;
                //操作系统准备关机
                case CloseReason.WindowsShutDown:
                    e.Cancel = false;//不拦截，响应操作
                    break;
                default:
                    break;
            }
            if (e.Cancel)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                this.ntfBackIcon.Visible = true;
            }
        }
        /// <summary>
        /// 窗口已关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //tssStopServer_Click(sender, e);
        }
        #endregion

        #region 读取配置
        /// <summary>
        /// 获取配置
        /// </summary>
        private void LoadConfig()
        {
            var deviceConfigs = this.deviceConfigDal.GetDeviceConfig();
            if (deviceConfigs != null)
            {
                Cache.DeviceConfigs.Clear();
                foreach (var deviceConfig in deviceConfigs)
                {
                    Cache.DeviceConfigs.TryAdd(deviceConfig.DeviceGuid, deviceConfig);
                }
            }
            var groupConfigs = this.groupConfigDal.GetGroupConfig();
            if (groupConfigs != null)
            {
                Cache.GroupConfigs.Clear();
                foreach (var groupConfig in groupConfigs)
                {
                    Cache.GroupConfigs.TryAdd(groupConfig.Id, groupConfig);
                }
            }
            Cache.IsDebug = Convert.ToBoolean(ConfigManage.GetValue("isDebug"));
            Cache.OpcRootName = ConfigManage.GetValue("opcRootName");
            Cache.OpcServerUrl = ConfigManage.GetValue("opcServerUrl");
            DriverFactory.LoadSupportDevice();
            this.ShowTreeView(Cache.GroupConfigs.Values.ToList());
        }

        #endregion

        #region 控件数据
        /// <summary>
        /// 显示详情
        /// </summary>
        /// <param name="details"></param>
        private void ShowDetail(DataDetailModel dataDetail)
        {
            if (this.gbDataDetail.InvokeRequired)
            {
                // 很帅的调自己
                this.gbDataDetail.Invoke(new Action(() => { this.ShowDetail(dataDetail); }));
            }
            else
            {
                if (dataDetail == null)
                {
                    this.pnlDataDetail.Visible = false;
                    return;
                }
                this.pnlDataDetail.Visible = true;
                this.tbName.Text = dataDetail.Name;
                this.tbDataName.Text = dataDetail.DataName;
                this.tbDeviceName.Text = dataDetail.DeviceName;
                this.tbDeviceState.Text = dataDetail.DeviceState;
                this.tbValue.Text = dataDetail.Value;
                this.tbAddress.Text = dataDetail.Address;
                this.tbMbArea.Text = dataDetail.Area.ToString();
                this.tbMbAddress.Text = dataDetail.RegAddress.ToString();
            }
        }
        /// <summary>
        /// 展示树形图
        /// </summary>
        /// <param name="groups">分组数据</param>
        private void ShowTreeView(List<GroupConfigEntity> groups)
        {
            if (this.tvDataMap.InvokeRequired)
            {
                // 很帅的调自己
                this.tvDataMap.Invoke(new Action(() => { this.ShowTreeView(groups); }));
            }
            else
            {
                TreeNode selectNode = this.tvDataMap.SelectedNode;
                this.tvDataMap.Nodes.Clear();
                TreeNode root = new TreeNode()
                {
                    Name = "root",
                    Tag = "root",
                    Text = Cache.OpcRootName,
                    ImageIndex = 0,
                    SelectedImageIndex = 0
                };
                this.tvDataMap.Nodes.Add(root);
                if (groups == null || groups.Count == 0)
                {
                    TreeNode noNode = new TreeNode()
                    {
                        Name = Guid.NewGuid().ToString(),
                        Tag = "root",
                        Text = "暂无数据，请添加",
                        ImageIndex = 0,
                        SelectedImageIndex = 0
                    };
                    root.Nodes.Add(noNode);
                }
                else
                {
                    groups = groups.OrderBy(s => s.Name).ToList();
                    foreach (var group in groups)
                    {
                        TreeNode groupNode = new TreeNode()
                        {
                            Name = group.Id,
                            Tag = group.Tag,
                            Text = group.Name,
                            ImageIndex = 1,
                            SelectedImageIndex = 1
                        };
                        root.Nodes.Add(groupNode);
                        if (selectNode != null && selectNode.Name.Equals(groupNode.Name) && selectNode.Tag.Equals(groupNode.Tag) && selectNode.Text.Equals(groupNode.Text))
                        {
                            this.tvDataMap.SelectedNode = groupNode;
                        }
                        //添加子节点
                        var nodes = group.Nodes.OrderBy(s => s.Name).ToList();
                        foreach (var node in nodes)
                        {
                            TreeNode dataNode = new TreeNode()
                            {
                                Name = node.Id,
                                Tag = node.Tag,
                                Text = node.Name,
                                ImageIndex = 2,
                                SelectedImageIndex = 2
                            };
                            groupNode.Nodes.Add(dataNode);
                            if (selectNode != null && selectNode.Name.Equals(dataNode.Name) && selectNode.Tag.Equals(dataNode.Tag) && selectNode.Text.Equals(dataNode.Text))
                            {
                                this.tvDataMap.SelectedNode = dataNode;
                            }
                        }
                    }
                }
                this.tvDataMap.ExpandAll();
            }
        }
        #endregion

        #region 按钮操作
        /// <summary>
        /// 点击启动服务按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssStartServer_Click(object sender, EventArgs e)
        {
            if (Cache.GroupConfigs == null || Cache.GroupConfigs.Count == 0)
            {
                Cache.MessageForm.ShowMessage("未配置数据", "信息", 3000);
                return;
            }
            Cache.DeviceDataServer.StartServer();
            var runDevices = Cache.DeviceConfigs.Values.Where(s => s.Enable);
            foreach (var runDevice in runDevices)
            {
                Cache.RunDevices.AddOrUpdate(runDevice.DeviceGuid, runDevice, (k, v) => runDevice);
            }
            this.tssAddGroup.Enabled = false;
            this.tssAddMember.Enabled = false;
            this.tssEditSelect.Enabled = false;
            this.tssDelSelect.Enabled = false;
            //Cache.MessageForm.ShowMessage("启动服务成功", "信息", 3000);
            _refreshMark = true;
            _refreshTask = Task.Run(RefreshDataThread);
        }
        /// <summary>
        /// 点击停止服务按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssStopServer_Click(object sender, EventArgs e)
        {
            _refreshMark = false;
            _refreshTask = null;
            try
            {
                Cache.DeviceDataServer.StopServer();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            Cache.RunDevices.Clear();
            this.tssAddGroup.Enabled = true;
            this.tssAddMember.Enabled = true;
            this.tssEditSelect.Enabled = true;
            this.tssDelSelect.Enabled = true;
            //Cache.MessageForm.ShowMessage("停止服务成功", "信息", 3000);
        }
        /// <summary>
        /// 点击添加成员按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssAddMember_Click(object sender, EventArgs e)
        {
            var selectedNode = this.GetValidSelectNode();
            if (selectedNode != null)
            {
                GroupConfigEntity groupConfig = null;
                if (selectedNode.Tag.Equals("Group"))
                {
                    groupConfig = Cache.GroupConfigs.Values.Where(s => s.Id.Equals(selectedNode.Name)).FirstOrDefault();
                }
                //编辑数据节点
                else if (selectedNode.Tag.Equals("Data"))
                {
                    var parentNode = selectedNode.Parent;
                    if (parentNode != null)
                    {
                        groupConfig = Cache.GroupConfigs.Values.Where(s => s.Id.Equals(parentNode.Name)).FirstOrDefault();
                    }
                }
                //无效的目标
                else
                {
                    Cache.MessageForm.ShowDialogMessage("请选择分组或者分组下的成员！", "错误");
                    return;
                }
                //打开新增数据
                if (groupConfig != null)
                {
                    NodeForm nodeForm = new NodeForm(groupConfig);
                    nodeForm.OnSave += this.RefreshTreeHandler;
                    nodeForm.ShowDialog();
                }
                else
                {
                    Cache.MessageForm.ShowDialogMessage("请选择分组或者分组下的成员！", "错误");
                    return;
                }
            }
        }
        /// <summary>
        /// 点击添加分组按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssAddGroup_Click(object sender, EventArgs e)
        {
            GroupForm groupForm = new GroupForm();
            groupForm.OnSave += this.RefreshTreeHandler;
            groupForm.ShowDialog();
        }
        /// <summary>
        /// 点击编辑选中按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssEditSelect_Click(object sender, EventArgs e)
        {
            var selectedNode = this.GetValidSelectNode();
            if (selectedNode != null)
            {
                //编辑分组
                if (selectedNode.Tag.Equals("Group"))
                {
                    var item = Cache.GroupConfigs.Values.Where(s => s.Id.Equals(selectedNode.Name)).FirstOrDefault();
                    GroupForm groupForm = new GroupForm();
                    groupForm.OnSave += this.RefreshTreeHandler;
                    groupForm.GroupConfig = item;
                    groupForm.ShowDialog();
                }
                //编辑数据节点
                else if (selectedNode.Tag.Equals("Data"))
                {
                    var parentNode = selectedNode.Parent;
                    var parent = Cache.GroupConfigs.Values.Where(s => s.Id.Equals(parentNode.Name)).FirstOrDefault();
                    if (parent != null)
                    {
                        var item = parent.Nodes.Where(s => s.Id.Equals(selectedNode.Name)).FirstOrDefault();
                        NodeForm nodeForm = new NodeForm(parent);
                        nodeForm.OnSave += this.RefreshTreeHandler;
                        nodeForm.NodeConfig = item;
                        nodeForm.ShowDialog();
                    }
                }
                else
                {
                    Cache.MessageForm.ShowDialogMessage("选中的为根节点，请选择一个有效目标！", "错误");
                }
            }
        }
        /// <summary>
        /// 点击移除选中按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssDelSelect_Click(object sender, EventArgs e)
        {
            var selectedNode = this.GetValidSelectNode();
            if (selectedNode != null)
            {
                //删除分组
                if (selectedNode.Tag.Equals("Group"))
                {
                    if (Cache.MessageForm.ShowOkCancelDialog("将删除分组以及其下节点，是否继续？") == DialogResult.Cancel)
                    {
                        return;
                    }
                    var item = Cache.GroupConfigs.Values.Where(s => s.Id.Equals(selectedNode.Name)).FirstOrDefault();
                    if (item != null)
                    {
                        Cache.GroupConfigs.TryRemove(item.Id, out item);
                    }
                }
                //删除数据节点
                else if (selectedNode.Tag.Equals("Data"))
                {
                    var parentNode = selectedNode.Parent;
                    var parent = Cache.GroupConfigs.Values.Where(s => s.Id.Equals(parentNode.Name)).FirstOrDefault();
                    if (parent != null)
                    {
                        var item = parent.Nodes.Where(s => s.Id.Equals(selectedNode.Name)).FirstOrDefault();
                        if (item != null)
                        {
                            parent.Nodes.Remove(item);
                        }
                    }
                }
                tvDataMap.Nodes.Remove(selectedNode);
                this.RefreshTreeHandler(null, null);
            }
        }
        /// <summary>
        /// 点击设备管理按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tssDeviceManage_Click(object sender, EventArgs e)
        {
            DeviceForm deviceForm = new DeviceForm();
            deviceForm.OnSave += this.RefreshTreeHandler;
            deviceForm.ShowDialog();
        }

        #endregion

        #region 私有调用
        /// <summary>
        /// 获取有效的选中目标(非根节点)
        /// </summary>
        /// <returns></returns>
        private TreeNode GetValidSelectNode()
        {
            var selectedNode = tvDataMap.SelectedNode;
            if (selectedNode == null)
            {
                Cache.MessageForm.ShowDialogMessage("未选择有效目标！", "错误");
                return null;
            }
            else
            {
                if (selectedNode.Tag.Equals("root"))
                {
                    Cache.MessageForm.ShowDialogMessage("选中的为根节点，请选择一个有效目标！", "错误");
                    return null;
                }
                else
                {
                    return selectedNode;
                }
            }
        }
        /// <summary>
        /// 修改、编辑后刷新数据节点树
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshTreeHandler(object sender, EventArgs e)
        {
            var list = Cache.GroupConfigs.Values.ToList();
            if (list.Count > 0)
            {
                list = list.OrderBy(s => s.Name).ToList();
            }
            this.ShowTreeView(list);
            groupConfigDal.SaveGroupConfig(list);
        }
        #endregion
        /// <summary>
        /// 点击通知图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ntfBackIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.Activate();
            this.WindowState = FormWindowState.Normal;
            //this.notifyIcon1.Visible = false;
        }
        /// <summary>
        /// 显示主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmShowMain_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }
        /// <summary>
        /// 退出应用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmExitApp_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.ntfBackIcon.Visible = false;

            tssStopServer_Click(sender, e);

            Application.Exit();
        }
        /// <summary>
        /// 点击节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvDataMap_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag.ToString() == "Data")
            {
                GroupConfigEntity groupConfig = Cache.GroupConfigs[e.Node.Parent.Name];
                NodeConfigEntity nodeConfig = groupConfig.Nodes.Where(s => s.Name.Equals(e.Node.Text)).FirstOrDefault();
                DeviceConfigEntity deviceConfig = Cache.DeviceConfigs[nodeConfig.DeviceGuid];
                this.dataDetail = new DataDetailModel();
                this.dataDetail.Name = nodeConfig.Name;
                this.dataDetail.DataName = nodeConfig.DataName;
                this.dataDetail.DeviceName = deviceConfig.CustomName;
                this.dataDetail.DeviceState = deviceConfig.Enable.ToString();
                this.dataDetail.Address = string.Format(@"ns={0};s={1}/{2}/{3}", Cache.OpcNamespace, Cache.OpcRootName, groupConfig.Name, nodeConfig.Name);
                this.dataDetail.Area = nodeConfig.Area;
                this.dataDetail.RegAddress = nodeConfig.RegAddress;
                object value = Cache.OpcUaServer.ReadValue(this.dataDetail.Address);
                if (value != null)
                {
                    this.dataDetail.Value = value.ToString();
                }
                this.dataDetail.DeviceState = deviceConfig.Enable.ToString();
                this.ShowDetail(this.dataDetail);
            }
            else
            {
                this.dataDetail = null;
                this.ShowDetail(null);
            }
        }
        /// <summary>
        /// 刷新选择的数据
        /// </summary>
        private void RefreshDataThread()
        {
            while (_refreshMark)
            {
                Thread.Sleep(500);
                DataDetailModel dataDetail = null;
                if (this.dataDetail != null)
                {
                    dataDetail = this.dataDetail;
                }
                try
                {
                    if (dataDetail != null && Cache.OpcUaServer.IsStart)
                    {
                        object value = Cache.OpcUaServer.ReadValue(dataDetail.Address);
                        dataDetail.Value = value.ToString();
                        this.ShowDetail(this.dataDetail);
                    }
                }
                catch (Exception  ex)
                {
                    continue;
                }
                Thread.Sleep(500);
            }
        }
    }
}
