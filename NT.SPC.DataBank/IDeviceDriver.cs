using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT.SPC.DataBank
{
    /// <summary>
    /// 接收到数据
    /// </summary>
    /// <param name="dataMap">数据</param>
    /// <param name="dataTypeMap">数据类型</param>
    /// <param name="dateTime">接收时间</param>
    /// <param name="gid">设备连接Id</param>
    public delegate void ReceivedEventHandler(Dictionary<string, object> dataMap, Dictionary<string, string> dataTypeMap, DateTime dateTime, string gid);
    /// <summary>
    /// 发生错误
    /// </summary>
    /// <param name="ex">错误</param>
    /// <param name="level">错误级别(0.不需要记录与显示；1.仅记录；2.记录并显示)</param>
    /// <param name="gid">设备连接Id</param>
    public delegate void ErrorOccurredEventHandler(Exception ex, int level, string gid);
    /// <summary>
    /// 已建立连接
    /// </summary>
    /// <param name="gid">设备连接Id</param>
    public delegate void ConnectedEventHandler(string gid);
    /// <summary>
    /// 已断开连接
    /// </summary>
    /// <param name="gid">设备连接Id</param>
    public delegate void DisonnectedEventHandler(string gid);
    /// <summary>
    /// 数据采集驱动
    /// </summary>
    public interface IDeviceDriver : IDisposable
    {
        event ReceivedEventHandler OnReceived;
        event ErrorOccurredEventHandler OnError;
        event ConnectedEventHandler OnConnected;
        event DisonnectedEventHandler OnDisonnected;
        /// <summary>
        /// 获取驱动名称
        /// </summary>
        /// <returns></returns>
        string GetDriverName();
        /// <summary>
        /// 获取可供采集的数据
        /// </summary>
        /// <returns></returns>
        string[] GetColloctData();
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
        /// <summary>
        /// 载入配置
        /// </summary>
        void LoadConfig();
        /// <summary>
        /// 载入默认配置
        /// </summary>
        void LoadDefaultConfig();
        /// <summary>
        /// 保存配置
        /// </summary>
        void SaveConfig();
        /// <summary>
        /// 保存配置
        /// </summary>
        void DeleteConfig();
        /// <summary>
        /// 启动服务
        /// </summary>
        void Start();
        /// <summary>
        /// 停止服务
        /// </summary>
        void Stop();
        /// <summary>
        /// 启动连接
        /// </summary>
        void Connect();
        /// <summary>
        /// 关闭连接
        /// </summary>
        void Disconnect();
        /// <summary>
        /// 获取数据类型
        /// </summary>
        string GetDataType(string dataName);
        /// <summary>
        /// 获取数据长度(以16bit为单位)
        /// </summary>
        int GetDataLength(string dataName);
        /// <summary>
        /// 获取连接Id
        /// </summary>
        string GetConnectId();
        /// <summary>
        /// 设置连接Id
        /// </summary>
        /// <param name="connectId">连接Id</param>
        void SetConnectId(string connectId);
        /// <summary>
        /// 获取配置字符串
        /// </summary>
        string GetConnectString();
        /// <summary>
        /// 设置配置字符串
        /// </summary>
        /// <param name="connectString">连接字符串</param>
        void SetConnectString(string connectString);
        /// <summary>
        /// 获取数据地址字符串
        /// </summary>
        string GetAddressString();
        /// <summary>
        /// 设置数据地址字符串
        /// </summary>
        /// <param name="addressString">数据地址字符串</param>
        void SetAddressString(string addressString);
        /// <summary>
        /// 获取配置文件路径
        /// </summary>
        string GetConfigPath();
        /// <summary>
        /// 获取帮助文本
        /// </summary>
        string GetHelpText();
        /// <summary>
        /// 检查连接
        /// </summary>
        /// <param name="configString">配置字符串</param>
        /// <returns></returns>
        bool CheckConnect(string configString);
    }
}
