using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT.SPC.OPCUA.Client
{
    /// <summary>
    /// OpcUa客户端
    /// </summary>
    public class OpcUaClient
    {
        #region 构造方法
        /// <summary>
        /// 无参构造
        /// </summary>
        public OpcUaClient()
        {
            dic_Subscriptions = new Dictionary<string, Subscription>();
        }
        #endregion

        #region 字段与属性
        #region 私有字段
        /// <summary>
        /// OPC应用实例
        /// </summary>
        private ApplicationInstance m_appInstance;
        private ApplicationConfiguration m_configuration;
        private Session m_Session;
        private bool m_IsConnected;                       //是否已经连接过
        private int m_ReconnectPeriod = 10;               //重连状态
        private bool m_UseSecurity;

        private EventHandler m_Connecting;
        private EventHandler m_Connected;
        private EventHandler m_Disconnected;
        private EventHandler m_KeepAlive;
        private EventHandler m_Reconnecting;
        private EventHandler m_Reconnected;
        private SessionReconnectHandler m_SessionReconnectHandler;

        private Dictionary<string, Subscription> dic_Subscriptions;        // 系统所有的节点信息

        #endregion

        #region 公有属性
        /// <summary>
        /// 配置文件
        /// </summary>
        public string ConfigFile { get; } = "Resources\\Opc.Ua.Client.Config.xml";
        /// <summary>
        /// 服务名称
        /// </summary>
        public string OpcUaName { get; set; } = "Opc Ua Client";
        /// <summary>
        /// 创建会话所使用的用户证明
        /// </summary>
        public IUserIdentity UserIdentity { get; set; }
        /// <summary>
        /// 配置信息
        /// </summary>
        public ApplicationConfiguration AppConfig
        {
            get
            {
                return m_configuration;
            }
        }
        /// <summary>
        /// 连接是否加密
        /// </summary>
        public bool UseSecurity
        {
            get { return m_UseSecurity; }
            set { m_UseSecurity = value; }
        }
        /// <summary>
        /// 当前活动的连接对象
        /// </summary>
        public Session Session
        {
            get { return m_Session; }
        }
        /// <summary>
        /// 连接状态标志
        /// </summary>
        public bool IsConnected
        {
            get { return m_IsConnected; }
        }
        /// <summary>
        /// 重连次数 (0代表禁用)
        /// </summary>
        public int ReconnectPeriod
        {
            get { return m_ReconnectPeriod; }
            set { m_ReconnectPeriod = value; }
        }
        /// <summary>
        /// 当客户端连接时触发
        /// </summary>
        public event EventHandler OnConnecting
        {
            add { m_Connecting += value; }
            remove { m_Connecting -= value; }
        }
        /// <summary>
        /// 当成功连接后触发
        /// </summary>
        public event EventHandler OnConnected
        {
            add { m_Connected += value; }
            remove { m_Connected -= value; }
        }
        /// <summary>
        /// 当断开连接后触发
        /// </summary>
        public event EventHandler OnDisconnected
        {
            add { m_Disconnected += value; }
            remove { m_Disconnected -= value; }
        }
        /// <summary>
        /// 当重连时触发
        /// </summary>
        public event EventHandler OnReconnecting
        {
            add { m_Reconnecting += value; }
            remove { m_Reconnecting -= value; }
        }
        /// <summary>
        /// 当重连完成后触发
        /// </summary>
        public event EventHandler OnReconnected
        {
            add { m_Reconnected += value; }
            remove { m_Reconnected -= value; }
        }
        /// <summary>
        /// 当服务有良好的心跳包到达时触发
        /// </summary>
        public event EventHandler OnKeepAlive
        {
            add { m_KeepAlive += value; }
            remove { m_KeepAlive -= value; }
        }
        #endregion
        #endregion

        #region 公有方法

        #region 连接与断开
        /// <summary>
        /// 连接服务
        /// </summary>
        /// <param name="serverUrl">remote url</param>
        public void ConnectServer(string serverUrl)
        {
            try
            {
                this.m_Session = Connect(serverUrl).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 创建一个新的会话
        /// </summary>
        /// <returns>会话实例</returns>
        private async Task<Session> Connect(string serverUrl)
        {
            //断开已存在的连接
            Disconnect();
            //创建新实例
            m_appInstance = new ApplicationInstance();
            m_appInstance.ApplicationType = ApplicationType.Client;
            //载入配置
            string file = Path.GetFullPath(this.ConfigFile);
            m_appInstance.LoadApplicationConfiguration(file, false).Wait();
            //检查证书
            bool certOK = m_appInstance.CheckApplicationInstanceCertificate(false, 0).Result;
            if (!certOK)
            {
                throw new Exception("Application instance certificate invalid!");
            }
            m_configuration = m_appInstance.ApplicationConfiguration;
            // 选择终端
            EndpointDescription endpointDescription = CoreClientUtils.SelectEndpoint(serverUrl, UseSecurity);
            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(this.AppConfig);
            ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
            //创建连接会话对象
            m_Connecting?.Invoke(this, null);
            m_Session = await Session.Create(
                this.AppConfig,
                endpoint,
                false,
                false,
                (string.IsNullOrEmpty(OpcUaName)) ? this.AppConfig.ApplicationName : OpcUaName,
                60000,
                UserIdentity,
                new string[] { });
            // 添加心跳回调事件
            m_Session.KeepAlive += new KeepAliveEventHandler(Session_KeepAlive);
            // 更新客户端状态
            m_IsConnected = true;
            // 触发连接完成事件
            m_Connected?.Invoke(this, null);
            //返回会话对象
            return m_Session;
        }
        /// <summary>
        /// 与服务端断开连接
        /// </summary>
        public void Disconnect()
        {
            // 停止重连处理任务
            if (m_SessionReconnectHandler != null)
            {
                m_SessionReconnectHandler.Dispose();
                m_SessionReconnectHandler = null;
            }
            // 断开会话
            if (m_Session != null)
            {
                m_Session.Close(5000);
                m_Session = null;
            }
            // 更新客户端状态
            m_IsConnected = false;
            // 触发断开事件
            m_Disconnected?.Invoke(this, null);
        }
        #endregion

        #region 节点读取、写入、删除
        #region 节点写入
        /// <summary>
        /// 写入一条记录到服务端
        /// </summary>
        /// <typeparam name="T">写入的数据类型</typeparam>
        /// <param name="tag">节点名称</param>
        /// <param name="value">值</param>
        /// <param name="dateTime">数据来源事件</param>
        /// <returns>是否成功</returns>
        public bool WriteValue<T>(string tag, T value, DateTime dateTime)
        {
            WriteValue valueToWrite = new WriteValue()
            {
                NodeId = new NodeId(tag),
                AttributeId = Attributes.Value
            };
            valueToWrite.Value.Value = value;
            valueToWrite.Value.StatusCode = StatusCodes.Good;
            valueToWrite.Value.ServerTimestamp = DateTime.Now;
            valueToWrite.Value.SourceTimestamp = dateTime;

            WriteValueCollection valuesToWrite = new WriteValueCollection
            {
                valueToWrite
            };

            // 写入当前的值
            m_Session.Write(
                null,
                valuesToWrite,
                out StatusCodeCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, valuesToWrite);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToWrite);

            if (StatusCode.IsBad(results[0]))
            {
                throw new ServiceResultException(results[0]);
            }
            return !StatusCode.IsBad(results[0]);
        }
        /// <summary>
        /// 异步写入一条记录到服务端
        /// </summary>
        /// <typeparam name="T">写入的数据类型</typeparam>
        /// <param name="tag">节点名称</param>
        /// <param name="value">值</param>
        /// <param name="dateTime">数据来源事件</param>
        public Task<bool> WriteValueAsync<T>(string tag, T value, DateTime dateTime)
        {
            WriteValue valueToWrite = new WriteValue()
            {
                NodeId = new NodeId(tag),
                AttributeId = Attributes.Value,
            };
            valueToWrite.Value.Value = value;
            valueToWrite.Value.StatusCode = StatusCodes.Good;
            valueToWrite.Value.ServerTimestamp = DateTime.Now;
            valueToWrite.Value.SourceTimestamp = dateTime;
            WriteValueCollection valuesToWrite = new WriteValueCollection
            {
                valueToWrite
            };

            var taskCompletionSource = new TaskCompletionSource<bool>();
            m_Session.BeginWrite(
                requestHeader: null,
                nodesToWrite: valuesToWrite,
                callback: ar =>
                {
                    var response = m_Session.EndWrite(
                      result: ar,
                      results: out StatusCodeCollection results,
                      diagnosticInfos: out DiagnosticInfoCollection diag);

                    try
                    {
                        ClientBase.ValidateResponse(results, valuesToWrite);
                        ClientBase.ValidateDiagnosticInfos(diag, valuesToWrite);
                        taskCompletionSource.SetResult(StatusCode.IsGood(results[0]));
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                },
                asyncState: null);
            return taskCompletionSource.Task;
        }
        /// <summary>
        /// 写入一组数据，所有的节点都写入成功，返回<c>True</c>，否则返回<c>False</c>
        /// </summary>
        /// <param name="tags">节点名称数组</param>
        /// <param name="values">节点的值数组</param>
        /// <returns>所有的是否都写入成功</returns>
        public List<bool> WriteValues(Dictionary<string, object> nodes)
        {
            List<bool> list = null;
            WriteValueCollection valuesToWrite = new WriteValueCollection();

            foreach (var tag in nodes.Keys)
            {
                WriteValue valueToWrite = new WriteValue()
                {
                    NodeId = new NodeId(tag),
                    AttributeId = Attributes.Value
                };
                valueToWrite.Value.Value = nodes[tag];
                valueToWrite.Value.StatusCode = StatusCodes.Good;
                valueToWrite.Value.ServerTimestamp = DateTime.Now;
                valueToWrite.Value.SourceTimestamp = DateTime.Now;
                valuesToWrite.Add(valueToWrite);
            }

            // 写入当前的值

            m_Session.Write(
                null,
                valuesToWrite,
                out StatusCodeCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, valuesToWrite);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToWrite);

            foreach (var r in results)
            {
                if (StatusCode.IsBad(r))
                {
                    list.Add(false);
                }
                else
                {
                    list.Add(true);
                }
            }
            return list;
        }

        #endregion
        #region 节点读取
        /// <summary>
        /// 从服务端读取节点的值
        /// </summary>
        /// <param name="nodeId">节点Id</param>
        /// <returns>DataValue</returns>
        public DataValue ReadValue(NodeId nodeId)
        {
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId( )
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.Value
                }
            };
            // 读取当前值
            m_Session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out DataValueCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            return results[0];
        }
        /// <summary>
        /// 从服务端读取节点的值
        /// </summary>
        /// <typeparam name="T">值的数据类型</typeparam>
        /// <param name="tag">节点Id</param>
        /// <returns>实际值</returns>
        public T ReadValue<T>(string tag)
        {
            DataValue dataValue = ReadValue(new NodeId(tag));
            return (T)dataValue.Value;
        }
        /// <summary>
        /// 异步从服务端读取标签的值
        /// </summary>
        /// <typeparam name="T">值的数据类型</typeparam>
        /// <param name="tag">tag值</param>
        /// <returns>从服务端取得的实际值</returns>
        public Task<T> ReadValueAsync<T>(string tag)
        {
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId()
                {
                    NodeId = new NodeId(tag),
                    AttributeId = Attributes.Value
                }
            };
            var taskCompletionSource = new TaskCompletionSource<T>();
            m_Session.BeginRead(
                requestHeader: null,
                maxAge: 0,
                timestampsToReturn: TimestampsToReturn.Neither,
                nodesToRead: nodesToRead,
                callback: ar =>
                {
                    DataValueCollection results;
                    DiagnosticInfoCollection diag;
                    var response = m_Session.EndRead(
                      result: ar,
                      results: out results,
                      diagnosticInfos: out diag);
                    try
                    {
                        CheckReturnValue(response.ServiceResult);
                        CheckReturnValue(results[0].StatusCode);
                        var val = results[0];
                        taskCompletionSource.TrySetResult((T)val.Value);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                },
                asyncState: null);

            return taskCompletionSource.Task;
        }
        /// <summary>
        /// 从服务端读取一组数据
        /// </summary>
        /// <param name="nodeIds">要读取的节点Id数组</param>
        /// <returns>节点值List</returns>
        public List<DataValue> ReadValues(List<NodeId> nodeIds)
        {
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();
            for (int i = 0; i < nodeIds.Count; i++)
            {
                nodesToRead.Add(new ReadValueId()
                {
                    NodeId = nodeIds[i],
                    AttributeId = Attributes.Value
                });
            }
            // 读取当前的值
            m_Session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out DataValueCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            return results.ToList();
        }
        /// <summary>
        /// 使用标签从服务端读取一组数据
        /// </summary>
        /// <typeparam name="T">值的数据类型</typeparam>
        /// <param name="tags">所以的节点数组信息</param>
        /// <returns>节点值List</returns>
        public List<T> ReadValues<T>(List<string> tags)
        {
            List<T> result = new List<T>();
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();
            for (int i = 0; i < tags.Count; i++)
            {
                nodesToRead.Add(new ReadValueId()
                {
                    NodeId = new NodeId(tags[i]),
                    AttributeId = Attributes.Value
                });
            }
            // 读取当前的值
            m_Session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out DataValueCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            foreach (var item in results)
            {
                result.Add((T)item.Value);
            }
            return result;
        }
        #endregion
        #region 节点删除
        /// <summary>
        /// 删除一个节点的操作，除非服务器配置允许，否则引发异常，成功返回<c>True</c>，否则返回<c>False</c>
        /// </summary>
        /// <param name="tag">节点文本描述</param>
        /// <returns></returns>
        public bool DeleteExsistNode(string tag)
        {
            DeleteNodesItemCollection waitDelete = new DeleteNodesItemCollection();

            DeleteNodesItem nodesItem = new DeleteNodesItem()
            {
                NodeId = new NodeId(tag),
            };

            m_Session.DeleteNodes(
                null,
                waitDelete,
                out StatusCodeCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, waitDelete);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, waitDelete);

            return !StatusCode.IsBad(results[0]);
        }
        #endregion
        #endregion

        #region 节点引用
        /// <summary>
        /// 浏览一个节点的引用
        /// </summary>
        /// <param name="tag">节点值</param>
        /// <returns>引用节点描述</returns>
        public List<ReferenceDescription> BrowseNode(string tag)
        {
            NodeId sourceId = new NodeId(tag);
            // 查找节点集合的所有节点
            BrowseDescription nodeToBrowse1 = new BrowseDescription();
            nodeToBrowse1.NodeId = sourceId;
            nodeToBrowse1.BrowseDirection = BrowseDirection.Forward;
            nodeToBrowse1.ReferenceTypeId = ReferenceTypeIds.Aggregates;
            nodeToBrowse1.IncludeSubtypes = true;
            nodeToBrowse1.NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable | NodeClass.Method);
            nodeToBrowse1.ResultMask = (uint)BrowseResultMask.All;

            // 查找按节点组织的所有节点
            BrowseDescription nodeToBrowse2 = new BrowseDescription();
            nodeToBrowse2.NodeId = sourceId;
            nodeToBrowse2.BrowseDirection = BrowseDirection.Forward;
            nodeToBrowse2.ReferenceTypeId = ReferenceTypeIds.Organizes;
            nodeToBrowse2.IncludeSubtypes = true;
            nodeToBrowse2.NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable);
            nodeToBrowse2.ResultMask = (uint)BrowseResultMask.All;

            BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();
            nodesToBrowse.Add(nodeToBrowse1);
            nodesToBrowse.Add(nodeToBrowse2);

            // 从服务器获取引用
            ReferenceDescriptionCollection references = Browse(m_Session, nodesToBrowse, false);

            return references.ToList();
        }
        #endregion

        #region 节点属性
        /// <summary>
        /// 读取一个节点的所有属性
        /// </summary>
        /// <param name="tag">节点信息</param>
        /// <returns>节点的特性值</returns>
        public OpcNodeAttribute[] ReadNoteAttributes(string tag)
        {
            NodeId sourceId = new NodeId(tag);
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

            // 尝试着去读取所有可能的特性
            for (uint ii = Attributes.NodeClass; ii <= Attributes.UserExecutable; ii++)
            {
                ReadValueId nodeToRead = new ReadValueId();
                nodeToRead.NodeId = sourceId;
                nodeToRead.AttributeId = ii;
                nodesToRead.Add(nodeToRead);
            }

            int startOfProperties = nodesToRead.Count;

            // 查找节点的所有属性
            BrowseDescription nodeToBrowse1 = new BrowseDescription();

            nodeToBrowse1.NodeId = sourceId;
            nodeToBrowse1.BrowseDirection = BrowseDirection.Forward;
            nodeToBrowse1.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            nodeToBrowse1.IncludeSubtypes = true;
            nodeToBrowse1.NodeClassMask = 0;
            nodeToBrowse1.ResultMask = (uint)BrowseResultMask.All;

            BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();
            nodesToBrowse.Add(nodeToBrowse1);

            // 从服务器获取引用
            ReferenceDescriptionCollection references = Browse(m_Session, nodesToBrowse, false);

            if (references == null)
            {
                return new OpcNodeAttribute[0];
            }

            for (int ii = 0; ii < references.Count; ii++)
            {
                // 忽略外部属性
                if (references[ii].NodeId.IsAbsolute)
                {
                    continue;
                }
                ReadValueId nodeToRead = new ReadValueId();
                nodeToRead.NodeId = (NodeId)references[ii].NodeId;
                nodeToRead.AttributeId = Attributes.Value;
                nodesToRead.Add(nodeToRead);
            }

            // 读取所有值
            DataValueCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            m_Session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out results,
                out diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            // 处理结果
            List<OpcNodeAttribute> nodeAttribute = new List<OpcNodeAttribute>();
            for (int ii = 0; ii < results.Count; ii++)
            {
                OpcNodeAttribute item = new OpcNodeAttribute();

                // 处理特性的值
                if (ii < startOfProperties)
                {
                    //忽略特性无效的节点
                    if (results[ii].StatusCode == StatusCodes.BadAttributeIdInvalid)
                    {
                        continue;
                    }
                    // 获取特性的名称
                    item.Name = Attributes.GetBrowseName(nodesToRead[ii].AttributeId);
                    // 显示异常错误
                    if (StatusCode.IsBad(results[ii].StatusCode))
                    {
                        item.Type = Utils.Format("{0}", Attributes.GetDataTypeId(nodesToRead[ii].AttributeId));
                        item.Value = Utils.Format("{0}", results[ii].StatusCode);
                    }
                    //显示值
                    else
                    {
                        TypeInfo typeInfo = TypeInfo.Construct(results[ii].Value);
                        item.Type = typeInfo.BuiltInType.ToString();
                        if (typeInfo.ValueRank >= ValueRanks.OneOrMoreDimensions)
                        {
                            item.Type += "[]";
                        }
                        item.Value = results[ii].Value;//Utils.Format("{0}", results[ii].Value);
                    }
                }
                // 处理属性的值
                else
                {
                    //忽略属性无效的节点
                    if (results[ii].StatusCode == StatusCodes.BadNodeIdUnknown)
                    {
                        continue;
                    }
                    // 获取属性的名称
                    item.Name = Utils.Format("{0}", references[ii - startOfProperties]);
                    // 显示异常错误
                    if (StatusCode.IsBad(results[ii].StatusCode))
                    {
                        item.Type = String.Empty;
                        item.Value = Utils.Format("{0}", results[ii].StatusCode);
                    }
                    // 显示值
                    else
                    {
                        TypeInfo typeInfo = TypeInfo.Construct(results[ii].Value);
                        item.Type = typeInfo.BuiltInType.ToString();
                        if (typeInfo.ValueRank >= ValueRanks.OneOrMoreDimensions)
                        {
                            item.Type += "[]";
                        }
                        item.Value = results[ii].Value;
                    }
                }
                nodeAttribute.Add(item);
            }
            return nodeAttribute.ToArray();
        }

        /// <summary>
        /// 读取一个节点的所有属性
        /// </summary>
        /// <param name="tag">节点值</param>
        /// <returns>所有的数据</returns>
        public DataValue[] ReadNoteDataValueAttributes(string tag)
        {
            NodeId sourceId = new NodeId(tag);
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

            // 尝试着去读取所有可能的特性
            for (uint ii = Attributes.NodeId; ii <= Attributes.UserExecutable; ii++)
            {
                ReadValueId nodeToRead = new ReadValueId();
                nodeToRead.NodeId = sourceId;
                nodeToRead.AttributeId = ii;
                nodesToRead.Add(nodeToRead);
            }

            int startOfProperties = nodesToRead.Count;

            // 查找节点的所有属性
            BrowseDescription nodeToBrowse1 = new BrowseDescription();

            nodeToBrowse1.NodeId = sourceId;
            nodeToBrowse1.BrowseDirection = BrowseDirection.Forward;
            nodeToBrowse1.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            nodeToBrowse1.IncludeSubtypes = true;
            nodeToBrowse1.NodeClassMask = 0;
            nodeToBrowse1.ResultMask = (uint)BrowseResultMask.All;

            BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();
            nodesToBrowse.Add(nodeToBrowse1);

            // 从服务器获取引用
            ReferenceDescriptionCollection references = Browse(m_Session, nodesToBrowse, false);

            if (references == null)
            {
                return new DataValue[0];
            }

            for (int ii = 0; ii < references.Count; ii++)
            {
                // 忽略外部属性
                if (references[ii].NodeId.IsAbsolute)
                {
                    continue;
                }

                ReadValueId nodeToRead = new ReadValueId();
                nodeToRead.NodeId = (NodeId)references[ii].NodeId;
                nodeToRead.AttributeId = Attributes.Value;
                nodesToRead.Add(nodeToRead);
            }

            // 读物所有值
            DataValueCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            m_Session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out results,
                out diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            return results.ToArray();
        }
        #endregion

        #region 数据订阅
        /// <summary>
        /// 新增一个订阅，需要指定订阅的关键字，订阅的tag名，以及回调方法
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="tag">tag</param>
        /// <param name="callback">回调方法</param>
        public void AddSubscription(string key, string tag, Action<string, MonitoredItem, MonitoredItemNotificationEventArgs> callback)
        {
            AddSubscription(key, new string[] { tag }, callback);
        }
        /// <summary>
        /// 新增一批订阅，需要指定订阅的关键字，订阅的tag名数组，以及回调方法
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="tags">节点名称数组</param>
        /// <param name="callback">回调方法</param>
        public void AddSubscription(string key, string[] tags, Action<string, MonitoredItem, MonitoredItemNotificationEventArgs> callback)
        {
            Subscription m_subscription = new Subscription(m_Session.DefaultSubscription);
            m_subscription.PublishingEnabled = true;
            m_subscription.PublishingInterval = 0;
            m_subscription.KeepAliveCount = 10000;
            m_subscription.LifetimeCount = uint.MaxValue;
            m_subscription.MaxNotificationsPerPublish = 1000;
            m_subscription.Priority = 100;
            m_subscription.DisplayName = key;

            for (int i = 0; i < tags.Length; i++)
            {
                var item = new MonitoredItem
                {
                    StartNodeId = new NodeId(tags[i]),
                    AttributeId = Attributes.Value,
                    DisplayName = tags[i],
                    SamplingInterval = 100,
                };
                item.Notification += (MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs args) =>
                {
                    callback?.Invoke(key, monitoredItem, args);
                };
                m_subscription.AddItem(item);
            }
            m_Session.AddSubscription(m_subscription);
            m_subscription.Create();
            if (dic_Subscriptions.ContainsKey(key))
            {
                dic_Subscriptions[key].Delete(true);
                m_Session.RemoveSubscription(dic_Subscriptions[key]);
                dic_Subscriptions[key].Dispose();
                dic_Subscriptions[key] = m_subscription;
            }
            else
            {
                dic_Subscriptions.Add(key, m_subscription);
            }
        }
        /// <summary>
        /// 移除订阅消息，如果该订阅消息是批量的，也直接移除
        /// </summary>
        /// <param name="key">订阅关键</param>
        public void RemoveSubscription(string key)
        {
            if (dic_Subscriptions.ContainsKey(key))
            {
                dic_Subscriptions[key].Delete(true);
                m_Session.RemoveSubscription(dic_Subscriptions[key]);
                dic_Subscriptions[key].Dispose();
                dic_Subscriptions.Remove(key);
            }
        }
        /// <summary>
        /// 移除所有的订阅消息
        /// </summary>
        public void RemoveAllSubscription()
        {
            foreach (var item in dic_Subscriptions)
            {
                item.Value.Delete(true);
                m_Session.RemoveSubscription(item.Value);
                item.Value.Dispose();
            }
            dic_Subscriptions.Clear();
        }

        #endregion

        #region 历史记录
        /// <summary>
        /// 读取历史数据
        /// </summary>
        /// <param name="tag">节点的索引</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="count">读取的个数</param>
        /// <param name="containBound">是否包含边界</param>
        /// <returns>读取的数据列表</returns>
        public IEnumerable<DataValue> ReadHistoryRawDataValues(string tag, DateTime start, DateTime end, uint count = 1, bool containBound = false)
        {
            HistoryReadValueId m_nodeToContinue = new HistoryReadValueId()
            {
                NodeId = new NodeId(tag),
            };

            ReadRawModifiedDetails m_details = new ReadRawModifiedDetails
            {
                StartTime = start,
                EndTime = end,
                NumValuesPerNode = count,
                IsReadModified = false,
                ReturnBounds = containBound
            };

            HistoryReadValueIdCollection nodesToRead = new HistoryReadValueIdCollection();
            nodesToRead.Add(m_nodeToContinue);

            m_Session.HistoryRead(
                null,
                new ExtensionObject(m_details),
                TimestampsToReturn.Both,
                false,
                nodesToRead,
                out HistoryReadResultCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            if (StatusCode.IsBad(results[0].StatusCode))
            {
                throw new ServiceResultException(results[0].StatusCode);
            }

            HistoryData values = ExtensionObject.ToEncodeable(results[0].HistoryData) as HistoryData;
            foreach (var value in values.DataValues)
            {
                yield return value;
            }
        }

        /// <summary>
        /// 读取一连串的历史数据，并将其转化成指定的类型
        /// </summary>
        /// <param name="tag">节点的索引</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="count">读取的个数</param>
        /// <param name="containBound">是否包含边界</param>
        /// <returns>读取的数据列表</returns>
        public IEnumerable<T> ReadHistoryRawDataValues<T>(string tag, DateTime start, DateTime end, uint count = 1, bool containBound = false)
        {
            HistoryReadValueId m_nodeToContinue = new HistoryReadValueId()
            {
                NodeId = new NodeId(tag),
            };

            ReadRawModifiedDetails m_details = new ReadRawModifiedDetails
            {
                StartTime = start.ToUniversalTime(),
                EndTime = end.ToUniversalTime(),
                NumValuesPerNode = count,
                IsReadModified = false,
                ReturnBounds = containBound
            };

            HistoryReadValueIdCollection nodesToRead = new HistoryReadValueIdCollection();
            nodesToRead.Add(m_nodeToContinue);

            m_Session.HistoryRead(
                null,
                new ExtensionObject(m_details),
                TimestampsToReturn.Both,
                false,
                nodesToRead,
                out HistoryReadResultCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            if (StatusCode.IsBad(results[0].StatusCode))
            {
                throw new ServiceResultException(results[0].StatusCode);
            }

            HistoryData values = ExtensionObject.ToEncodeable(results[0].HistoryData) as HistoryData;
            foreach (var value in values.DataValues)
            {
                yield return (T)value.Value;
            }
        }
        #endregion

        #endregion

        #region 私有方法
        /// <summary>
        /// 检查返回值状态
        /// </summary>
        /// <param name="status">状态码</param>
        private void CheckReturnValue(StatusCode status)
        {
            if (!StatusCode.IsGood(status))
                throw new Exception(string.Format("Invalid response from the server. (Response Status: {0})", status));
        }
        /// <summary>
        /// 浏览地址空间并返回引用集合
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <param name="nodesToBrowse">需要浏览的节点集合</param>
        /// <param name="throwOnError">是否抛出异常</param>
        /// <returns>查找到的引用集合，如果有异常发生则返回NULL </returns>
        public ReferenceDescriptionCollection Browse(Session session, BrowseDescriptionCollection nodesToBrowse, bool throwOnError)
        {
            try
            {
                ReferenceDescriptionCollection references = new ReferenceDescriptionCollection();
                BrowseDescriptionCollection unprocessedOperations = new BrowseDescriptionCollection();

                while (nodesToBrowse.Count > 0)
                {
                    // start the browse operation.
                    BrowseResultCollection results = null;
                    DiagnosticInfoCollection diagnosticInfos = null;

                    session.Browse(
                        null,
                        null,
                        0,
                        nodesToBrowse,
                        out results,
                        out diagnosticInfos);

                    ClientBase.ValidateResponse(results, nodesToBrowse);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToBrowse);

                    ByteStringCollection continuationPoints = new ByteStringCollection();

                    for (int ii = 0; ii < nodesToBrowse.Count; ii++)
                    {
                        // check for error.
                        if (StatusCode.IsBad(results[ii].StatusCode))
                        {
                            // this error indicates that the server does not have enough simultaneously active 
                            // continuation points. This request will need to be resent after the other operations
                            // have been completed and their continuation points released.
                            if (results[ii].StatusCode == StatusCodes.BadNoContinuationPoints)
                            {
                                unprocessedOperations.Add(nodesToBrowse[ii]);
                            }

                            continue;
                        }

                        // check if all references have been fetched.
                        if (results[ii].References.Count == 0)
                        {
                            continue;
                        }

                        // save results.
                        references.AddRange(results[ii].References);

                        // check for continuation point.
                        if (results[ii].ContinuationPoint != null)
                        {
                            continuationPoints.Add(results[ii].ContinuationPoint);
                        }
                    }

                    // process continuation points.
                    ByteStringCollection revisedContiuationPoints = new ByteStringCollection();

                    while (continuationPoints.Count > 0)
                    {
                        // continue browse operation.
                        session.BrowseNext(
                            null,
                            true,
                            continuationPoints,
                            out results,
                            out diagnosticInfos);

                        ClientBase.ValidateResponse(results, continuationPoints);
                        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, continuationPoints);

                        for (int ii = 0; ii < continuationPoints.Count; ii++)
                        {
                            // check for error.
                            if (StatusCode.IsBad(results[ii].StatusCode))
                            {
                                continue;
                            }

                            // check if all references have been fetched.
                            if (results[ii].References.Count == 0)
                            {
                                continue;
                            }

                            // save results.
                            references.AddRange(results[ii].References);

                            // check for continuation point.
                            if (results[ii].ContinuationPoint != null)
                            {
                                revisedContiuationPoints.Add(results[ii].ContinuationPoint);
                            }
                        }

                        // check if browsing must continue;
                        revisedContiuationPoints = continuationPoints;
                    }

                    // check if unprocessed results exist.
                    nodesToBrowse = unprocessedOperations;
                }

                // return complete list.
                return references;
            }
            catch (Exception exception)
            {
                if (throwOnError)
                {
                    throw new ServiceResultException(exception, StatusCodes.BadUnexpectedError);
                }

                return null;
            }
        }

        #endregion

        #region 事件处理
        /// <summary>
        /// 处理会话的心跳事件
        /// </summary>
        private void Session_KeepAlive(Session session, KeepAliveEventArgs e)
        {
            try
            {
                // 检查断开的会话来源是否一致
                if (!Object.ReferenceEquals(session, m_Session))
                {
                    return;
                }
                // 通信错误后开始重连处理
                if (ServiceResult.IsBad(e.Status))
                {
                    if (m_ReconnectPeriod <= 0)
                    {
                        return;
                    }
                    if (m_SessionReconnectHandler == null)
                    {
                        // 触发重连事件
                        m_Reconnecting?.Invoke(this, e);
                        m_SessionReconnectHandler = new SessionReconnectHandler();
                        m_SessionReconnectHandler.BeginReconnect(m_Session, m_ReconnectPeriod * 1000, Server_Reconnected);
                    }

                    return;
                }
                // 触发心跳事件
                m_KeepAlive?.Invoke(this, e);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// 在重连处理任务后执行重连完成事件
        /// </summary>
        private void Server_Reconnected(object sender, EventArgs e)
        {
            try
            {
                // 检查断开的会话来源是否一致
                if (!Object.ReferenceEquals(sender, m_SessionReconnectHandler))
                {
                    return;
                }
                m_Session = m_SessionReconnectHandler.Session;
                m_SessionReconnectHandler.Dispose();
                m_SessionReconnectHandler = null;
                // 触发重连完成事件
                m_Reconnected?.Invoke(this, e);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        #endregion
    }
}
