using HslCommunication.ModBus;
using NT.SPC.DataBank;
using NT.SPC.ServerConfig;
using NT.Tools.Common;
using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT.SPC.OPCUA.Server
{
    public class NtCustomNodeManager : CustomNodeManager2
    {
        private readonly NT.Tools.Log.Log logger = NT.Tools.Log.LogFactory.GetLogger(typeof(NtCustomNodeManager));
        /// <summary>
        /// 设备配置
        /// </summary>
        public DeviceConfigDal DeviceConfigDal { get; private set; } = new DeviceConfigDal();
        /// <summary>
        /// 分组数据配置
        /// </summary>
        public GroupConfigDal GroupConfigDal { get; private set; } = new GroupConfigDal();
        /// <summary>
        /// 设备配置
        /// </summary>
        public List<DeviceConfigEntity> DeviceConfigs { get; private set; }
        /// <summary>
        /// 分组配置
        /// </summary>
        public List<GroupConfigEntity> GroupConfigs { get; private set; }
        /// <summary>
        /// 节点配置
        /// </summary>
        public List<NodeConfigEntity> NodeConfigs { get; private set; }
        /// <summary>
        /// 启动设备
        /// </summary>
        public List<IDeviceDriver> DeviceDrivers { get; private set; }
        /// <summary>
        /// 根节点名称
        /// </summary>
        public string RootName { get; private set; }
        /// <summary>
        /// 是否调试模式
        /// </summary>
        public bool IsDebug { get; private set; }
        /// <summary>
        /// 是否调试模式
        /// </summary>
        public ModbusTcpServer modbus { get; private set; }

        private ConcurrentDictionary<string, BaseDataVariableState> _Dict_BaseDataVariableState = new ConcurrentDictionary<string, BaseDataVariableState>();
        public NtCustomNodeManager(IServerInternal server, ApplicationConfiguration configuration, ModbusTcpServer modbus)
              : base(server, configuration, "http://localhost/UA/Server/v1.0")
        {
            this.modbus = modbus;
        }
        /// <summary>
        /// 在地址空间有用之前进行一些初始化的操作
        /// </summary>
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                LoadPredefinedNodes(SystemContext, externalReferences);
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out IList<IReference> references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }
                this.RootName = ConfigManage.GetValue("opcRootName");
                this.IsDebug = Convert.ToBoolean(ConfigManage.GetValue("isDebug"));
                this.GroupConfigs = this.GroupConfigDal.GetGroupConfig();
                this.NodeConfigs = new List<NodeConfigEntity>();
                if (this.GroupConfigs.Count > 0)
                {
                    foreach (var groupConfig in GroupConfigs)
                    {
                        NodeConfigs.AddRange(groupConfig.Nodes);
                    }
                }
                AddNodeClass(this.GroupConfigs, references);
                this.DeviceConfigs = this.DeviceConfigDal.GetDeviceConfig().Where(s => s.Enable).ToList();
                AddDevices(this.DeviceConfigs);

            }
        }
        #region INodeIdFactory Members

        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            return node.NodeId;
        }

        #endregion


        #region 事件处理
        /// <summary>
        /// 接收到采集数据
        /// </summary>
        /// <param name="dataMap">数据</param>
        /// <param name="dataTypeMap">数据类型</param>
        /// <param name="dateTime">接收时间</param>
        /// <param name="gid">设备连接Id</param>
        private void DataReceivedHandler(Dictionary<string, object> dataMap, Dictionary<string, string> dataTypeMap, DateTime dateTime, string gid)
        {
            DeviceConfigEntity deviceConfig = this.DeviceConfigs.Where(s => s.DeviceGuid.Equals(gid)).FirstOrDefault();
            foreach (var name in dataMap.Keys)
            {
                var nodes = this.NodeConfigs.Where(s => s.DeviceGuid.Equals(gid) && s.DataName.Equals(name));
                foreach (var node in nodes)
                {
                    string tag = BuildingTag(name, node);
                    string type = dataTypeMap[name];
                    object value = dataMap[name];
                    if (value != null)
                    {
                        WriteNodeValue(tag, value);
                        WriteModbusValue(node, value, type);
                    }
                }
            }
        }
        /// <summary>
        /// 发生错误
        /// </summary>
        /// <param name="ex">错误</param>
        /// <param name="level">错误级别(0.不需要记录与显示；1.仅记录；2.记录并显示)</param>
        /// <param name="gid">设备连接Id</param>
        private void ErrorOccurredHandler(Exception ex, int level, string gid)
        {
            Exception exception = ex.GetOriginalException();
            switch (level)
            {
                case 0:
                    break;
                case 1:
                    if (IsDebug)
                    {
                        logger.Error(exception);
                    }
                    break;
                case 2:
                    if (IsDebug)
                    {
                        logger.Error(exception);
                    }
                    DeviceConfigEntity deviceConfig = this.DeviceConfigs.Where(s => s.DeviceGuid.Equals(gid)).FirstOrDefault();
                    if (deviceConfig != null)
                    {
                        string message = string.Format("错误：设备-{0}，错误信息-{1}", deviceConfig.CustomName, ex.Message);
                        //throw new Exception(message);
                    }
                    break;
            }
        }
        /// <summary>
        /// 已建立连接
        /// </summary>
        /// <param name="gid">设备连接Id</param>
        private void ConnectedHandler(string gid)
        {

        }
        /// <summary>
        /// 已断开连接
        /// </summary>
        /// <param name="gid">设备连接Id</param>
        private void DisonnectedHandler(string gid)
        {

        }
        #endregion

        private void AddDevices(List<DeviceConfigEntity> deviceConfigs)
        {
            DeviceDrivers = new List<IDeviceDriver>();
            var runDeviceConfigs = deviceConfigs.Where(s => s.Enable);
            foreach (var runDeviceConfig in runDeviceConfigs)
            {
                IDeviceDriver device = DriverFactory.CreateDevice(runDeviceConfig.DriverName);
                device.SetConnectId(runDeviceConfig.DeviceGuid);
                device.LoadConfig();
                device.OnReceived += DataReceivedHandler;
                device.OnError += ErrorOccurredHandler;
                device.OnConnected += ConnectedHandler;
                device.OnDisonnected += DisonnectedHandler;
                //device.SetConfigString(deviceConfig.ConfigString);
                device.Start();
                DeviceDrivers.Add(device);
            }
        }
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="groupConfigs"></param>
        /// <param name="references"></param>
        public void AddNodeClass(List<GroupConfigEntity> groupConfigs, IList<IReference> references)
        {
            FolderState root = CreateFolder(null, RootName);
            root.Description = RootName;
            root.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
            references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, root.NodeId));
            root.EventNotifier = EventNotifiers.SubscribeToEvents;
            AddRootNotifier(root);
            foreach (var group in groupConfigs)
            {
                var groupFolder = CreateFolder(root, group.Name);
                foreach (var node in group.Nodes)
                {
                    var variableState = CreateBaseVariable(groupFolder, node);
                    _Dict_BaseDataVariableState.AddOrUpdate(variableState.NodeId.ToString(), variableState, (k, v) => variableState);
                }
            }
            AddPredefinedNode(SystemContext, root);
        }
        /// <summary>
        /// 创建一个新的节点，节点名称为字符串
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected FolderState CreateFolder(NodeState parent, string name)
        {
            return CreateFolder(parent, name, string.Empty);
        }
        /// <summary>
        /// 创建一个新的节点，节点名称为字符串
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        protected FolderState CreateFolder(NodeState parent, string name, string description)
        {
            FolderState folder = new FolderState(parent);

            folder.SymbolicName = name;
            folder.ReferenceTypeId = ReferenceTypes.Organizes;
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            folder.Description = description;
            if (parent == null)
            {
                folder.NodeId = new NodeId(name, NamespaceIndex);
            }
            else
            {
                folder.NodeId = new NodeId(parent.NodeId.ToString() + "/" + name);
            }
            folder.BrowseName = new QualifiedName(name, NamespaceIndex);
            folder.DisplayName = new LocalizedText(name);
            folder.WriteMask = AttributeWriteMask.None;
            folder.UserWriteMask = AttributeWriteMask.None;
            folder.EventNotifier = EventNotifiers.None;

            if (parent != null)
            {
                parent.AddChild(folder);
            }

            return folder;
        }
        /// <summary>
        /// 创建一个泛型值节点
        /// </summary>
        protected BaseDataVariableState CreateBaseVariable(NodeState parent, NodeConfigEntity nodeConfig)
        {
            BaseDataVariableState baseDataVariableState = null;
            switch (nodeConfig.DataType)
            {
                case "bool":
                    baseDataVariableState = CreateBaseVariable(parent, nodeConfig.Name, nodeConfig.DataName, DataTypeIds.Boolean, ValueRanks.Scalar, default(bool));
                    break;
                case "ushort":
                    baseDataVariableState = CreateBaseVariable(parent, nodeConfig.Name, nodeConfig.DataName, DataTypeIds.UInt16, ValueRanks.Scalar, default(ushort));
                    break;
                case "short":
                    baseDataVariableState = CreateBaseVariable(parent, nodeConfig.Name, nodeConfig.DataName, DataTypeIds.Int16, ValueRanks.Scalar, default(short));
                    break;
                case "uint":
                    baseDataVariableState = CreateBaseVariable(parent, nodeConfig.Name, nodeConfig.DataName, DataTypeIds.UInt32, ValueRanks.Scalar, default(uint));
                    break;
                case "int":
                    baseDataVariableState = CreateBaseVariable(parent, nodeConfig.Name, nodeConfig.DataName, DataTypeIds.Int32, ValueRanks.Scalar, default(int));
                    break;
                case "float":
                    baseDataVariableState = CreateBaseVariable(parent, nodeConfig.Name, nodeConfig.DataName, DataTypeIds.Float, ValueRanks.Scalar, default(float));
                    break;
                case "string":
                    baseDataVariableState = CreateBaseVariable(parent, nodeConfig.Name, nodeConfig.DataName, DataTypeIds.String, ValueRanks.Scalar, default(string));
                    break;
            }
            return baseDataVariableState;
        }


        /// <summary>
        /// 创建一个值节点，类型需要在创建的时候指定
        /// </summary>
        protected BaseDataVariableState CreateBaseVariable(NodeState parent, string name, string description, NodeId dataType, int valueRank, object defaultValue)
        {
            BaseDataVariableState variable = new BaseDataVariableState(parent);

            variable.SymbolicName = name;
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            if (parent == null)
            {
                variable.NodeId = new NodeId(name, NamespaceIndex);
            }
            else
            {
                variable.NodeId = new NodeId(parent.NodeId.ToString() + "/" + name);
            }
            variable.Description = description;
            variable.BrowseName = new QualifiedName(name, NamespaceIndex);
            variable.DisplayName = new LocalizedText(name);
            variable.WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.DataType = dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Historizing = false;
            variable.Value = defaultValue;
            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.Now;

            if (parent != null)
            {
                parent.AddChild(variable);
            }

            return variable;
        }
        public void WriteNodeValue(string node, object data)
        {
            if (_Dict_BaseDataVariableState.ContainsKey(node))
            {
                _Dict_BaseDataVariableState[node].Value = data;
                _Dict_BaseDataVariableState[node].ClearChangeMasks(SystemContext, false);
            }
        }
        public void WriteModbusValue(NodeConfigEntity node, object data, string type)
        {
            string address = node.RegAddress.ToString();
            if (modbus.IsStarted)
            {
                switch (type)
                {
                    case "bool":
                        bool boolValue = Convert.ToBoolean(data);
                        modbus.WriteCoil(address, boolValue);
                        break;
                    case "ushort":
                        ushort ushortValue = Convert.ToUInt16(data);
                        modbus.Write(address, ushortValue);
                        break;
                    case "short":
                        short shortValue = Convert.ToInt16(data);
                        modbus.Write(address, shortValue);
                        break;
                    case "uint":
                        uint uintValue = Convert.ToUInt32(data);
                        modbus.Write(address, uintValue);
                        break;
                    case "int":
                        int intValue = Convert.ToInt32(data);
                        modbus.Write(address, intValue);
                        break;
                    case "float":
                        float floatValue = Convert.ToSingle(data);
                        modbus.Write(address, floatValue);
                        break;
                    case "string":
                        string stringValue = Convert.ToString(data);
                        modbus.Write(address, stringValue);
                        break;
                }
            }
        }
        public object ReadNodeValue(string node)
        {
            object value = null;
            if (_Dict_BaseDataVariableState.ContainsKey(node))
            {
                value = _Dict_BaseDataVariableState[node].Value;
            }
            return value;
        }
        protected string BuildingTag(string name, NodeConfigEntity nodeConfig)
        {
            GroupConfigEntity groupConfig = this.GroupConfigs.Where(s => s.Id.Equals(nodeConfig.GroupId)).FirstOrDefault();
            return string.Format("ns=2;s={0}/{1}/{2}", RootName, groupConfig.Name, nodeConfig.Name);
        }
        /// <summary>
        /// 释放设备
        /// </summary>
        public void DisposeDevice()
        {
            if (DeviceDrivers != null)
            {
                foreach (var deviceDriver in DeviceDrivers)
                {
                    deviceDriver.Dispose();
                }
                DeviceDrivers.Clear();
            }
        }
    }
}
