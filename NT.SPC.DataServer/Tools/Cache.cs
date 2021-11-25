using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NT.SPC.DataBank;
using NT.SPC.ServerConfig;
using NT.SPC.OPCUA.Server;

namespace NT.SPC.DataServer
{
    /// <summary>
    /// 全局缓存
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// 消息提示窗口
        /// </summary>
        public static MessageForm MessageForm { get; private set; } = new MessageForm();
        /// <summary>
        /// 是否调试模式
        /// </summary>
        public static bool IsDebug { get; set; } = false;
        /// <summary>
        /// 根节点名称
        /// </summary>
        public static string OpcRootName { get; set; } = "SPC Data Server";
        /// <summary>
        /// 根节点名称
        /// </summary>
        public static int OpcNamespace { get; set; } = 2;
        /// <summary>
        /// 根节点名称
        /// </summary>
        public static string OpcServerUrl { get; set; } = "opc.tcp://localhost:51210/UA/Server";
        /// <summary>
        /// OPC UA 服务端
        /// </summary>
        public static OpcUaServer OpcUaServer { get; private set; } = new OpcUaServer();
        /// <summary>
        /// 设备数据服务
        /// </summary>
        public static DeviceDataServer DeviceDataServer { get; private set; } = new DeviceDataServer();
        /// <summary>
        /// 设备配置
        /// </summary>
        public static DeviceConfigDal DeviceConfigDal { get; private set; } = new DeviceConfigDal();
        /// <summary>
        /// 分组数据配置
        /// </summary>
        public static GroupConfigDal GroupConfigDal { get; private set; } = new GroupConfigDal();
        /// <summary>
        /// 设备配置
        /// </summary>
        public static ConcurrentDictionary<string, DeviceConfigEntity> DeviceConfigs { get; set; } = new ConcurrentDictionary<string, DeviceConfigEntity>();
        /// <summary>
        /// 分组配置
        /// </summary>
        public static ConcurrentDictionary<string, GroupConfigEntity> GroupConfigs { get; set; } = new ConcurrentDictionary<string, GroupConfigEntity>();
        /// 运行中的设备驱动
        /// </summary>
        public static ConcurrentDictionary<string, DeviceConfigEntity> RunDevices { get; set; } = new ConcurrentDictionary<string, DeviceConfigEntity>();
    }
}
