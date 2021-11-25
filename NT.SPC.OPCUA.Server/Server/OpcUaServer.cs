using HslCommunication.Core;
using HslCommunication.ModBus;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT.SPC.OPCUA.Server
{
    /// <summary>
    /// OpcUa服务端
    /// </summary>
    public class OpcUaServer
    {
        /// <summary>
        /// 配置文件
        /// </summary>
        public string ConfigFile { get; private set; } = "Resources\\Opc.Ua.Server.Config.xml";
        /// <summary>
        /// 是否启动服务
        /// </summary>
        public bool IsStart { get; private set; } = false;
        private ModbusTcpServer modbus = null;
        #region Constructor
        public OpcUaServer()
        {
            modbus = new ModbusTcpServer();
            modbus.Port = 502;
            modbus.DataFormat = DataFormat.CDAB;
            modbus.IsStringReverse = true;
            modbus.Station = 1;
        }
        #endregion

        #region Public Method
        public object ReadValue(string tag)
        {
            object value = null;
            if (IsStart)
            {
                value = StandardServer.DevicesNodeManager.ReadNodeValue(tag);
            }
            return value;

        }
        public void WriteValue(string tag, object value)
        {
            if (IsStart)
            {
                StandardServer.DevicesNodeManager.WriteNodeValue(tag, value);
            }
        }
        /// <summary>
        /// 开启服务
        /// </summary>
        public void StartServer()
        {
            StopServer();

            this.AppInstance = new ApplicationInstance();
            this.AppInstance.ApplicationType = ApplicationType.Server;
            // 加载配置
            string path = Path.GetFullPath(this.ConfigFile);
            this.AppInstance.LoadApplicationConfiguration(path, false).Wait();
            this.AppConfiguration = this.AppInstance.ApplicationConfiguration;
            // 检查证书
            bool certOK = this.AppInstance.CheckApplicationInstanceCertificate(false, 0).Result;
            if (!certOK)
            {
                throw new Exception("Application instance certificate invalid!");
            }
            // 开启服务
            this.StandardServer = new CustomStandardServer(modbus);
            this.modbus.ServerStart();
            this.AppInstance.Start(this.StandardServer);
            this.IsStart = true;
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void StopServer()
        {
            this.IsStart = false;
            if (this.AppInstance != null)
            {
                this.AppInstance.Stop();
            }
            if (this.StandardServer != null)
            {
                this.StandardServer.Dispose();
            }
            if (this.modbus != null)
            {
                this.modbus.ServerClose();
            }
        }
        #endregion

        #region 成员变量
        /// <summary>
        /// 应用实例
        /// </summary>
        private ApplicationInstance AppInstance { get; set; }
        /// <summary>
        /// 服务器的实例
        /// </summary>
        public CustomStandardServer StandardServer { get; private set; }
        /// <summary>
        /// 应用程序的配置实例
        /// </summary>
        public ApplicationConfiguration AppConfiguration { get; private set; }
        #endregion
    }
}
