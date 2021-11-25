namespace NT.SPC.DataServer
{
    /// <summary>
    /// 数据采集服务
    /// </summary>
    public class DeviceDataServer
    {
        #region OPC UA 服务
        /// <summary>
        /// 开启服务
        /// </summary>
        public void StartServer()
        {
            //OPC服务端开启
            Cache.OpcUaServer.StartServer();

        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void StopServer()
        {
            //OPC服务端停止
            Cache.OpcUaServer.StopServer();
        }
        #endregion
    }
}
