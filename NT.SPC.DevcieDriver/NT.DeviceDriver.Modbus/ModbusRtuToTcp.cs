using HslCommunication;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.ModBus;
using HslCommunication.Serial;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NT.SPC.DataBank;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NT.DeviceDriver.Modbus
{
    /// <summary>
    /// Modbus-RTU客户端协议，使用串口服务器转网口
    /// </summary>
    public class ModbusRtuNet : IDeviceDriver
    {
        public event SPC.DataBank.ReceivedEventHandler OnReceived;
        public event SPC.DataBank.ErrorOccurredEventHandler OnError;
        public event SPC.DataBank.ConnectedEventHandler OnConnected;
        public event SPC.DataBank.DisonnectedEventHandler OnDisonnected;
        #region 字段、属性与构造方法
        private readonly string _driverName = "Modbus RTU-NET";
        /// <summary>
        /// 配置文件
        /// </summary>
        private string _configFile = "Resources\\DeviceDriver\\{0}.json";
        /// <summary>
        /// 配置文件
        /// </summary>
        private string _defaultConfigFile = "Resources\\DeviceDriver\\Default\\Modbus-RTU-NET.json";
        /// <summary>
        /// 配置文件
        /// </summary>
        private string _connectId = string.Empty;
        /// <summary>
        /// 连接配置
        /// </summary>
        private DeviceConfig _deviceConfig = new DeviceConfig();
        /// <summary>
        /// 是否连接
        /// </summary>
        private bool _isConnected = false;
        /// <summary>
        /// modbus tcp 客户端
        /// </summary>
        private ModbusRtuToNet _modbus = null;
        /// <summary>
        /// 采集标志
        /// </summary>
        private bool _colloctMark = false;
        #endregion

        #region 内部类 - 配置
        /// <summary>
        /// 设备配置
        /// </summary>
        public class DeviceConfig
        {
            /// <summary>
            /// 设备连接配置
            /// </summary>
            public ConnectConfig ConnectConfig { get; set; }
            /// <summary>
            /// 设备数据地址配置
            /// </summary>
            public List<AddressConfig> AddressConfigs { get; set; }
            /// <summary>
            /// 连接ID
            /// </summary>
            public string ConnectId { get; set; } = "Default";
            /// <summary>
            /// 帮助文本
            /// </summary>
            public string HelpText { get; set; } = "未提供帮助文本";
        }
        /// <summary>
        /// 设备连接配置
        /// </summary>
        public class ConnectConfig
        {
            /// <summary>
            /// 服务器IP地址
            /// </summary>
            public string Host { get; set; } = "127.0.0.1";
            /// <summary>
            /// 服务器端口
            /// </summary>
            public int Port { get; set; } = 502;
            /// <summary>
            /// 从站地址
            /// </summary>
            public byte SlaveID { get; set; } = 1;
            /// <summary>
            /// 采集周期
            /// </summary>
            public int AcquisitionCycle { get; set; } = 1000;
            /// <summary>
            /// 通讯超时
            /// </summary>
            public int Timeout { get; set; } = 1000;
            /// <summary>
            /// 交换32位数据高低位
            /// </summary>
            public string Exchange32bit { get; set; } = "3412";
            /// <summary>
            /// 交换字符串数据高低位
            /// </summary>
            public string ExchangeString { get; set; } = "21";
        }
        /// <summary>
        /// 设备数据地址配置
        /// </summary>
        public class AddressConfig
        {
            /// <summary>
            /// 数据名称
            /// </summary>
            public string DataName { get; set; } = "Default";
            /// <summary>
            /// 站点号
            /// </summary>
            public byte SlaveId { get; set; } = 1;
            /// <summary>
            /// 寄存器区域
            /// </summary>
            public byte Area { get; set; } = 4;
            /// <summary>
            /// 起始地址
            /// </summary>
            public int Index { get; set; } = 0;
            /// <summary>
            /// 寄存器个数
            /// </summary>
            public ushort Length { get; set; } = 1;
            /// <summary>
            /// 数据类型
            /// </summary>
            public string DataType { get; set; } = "ushort";
            /// <summary>
            /// 数据转换类型，12先倍率再偏移，21先偏移再倍率
            /// </summary>
            public string ExchangeType { get; set; } = "21";
            /// <summary>
            /// 数据倍率
            /// </summary>
            public float Rate { get; set; } = 1;
            /// <summary>
            /// 数据偏移量
            /// </summary>
            public float Offset { get; set; } = 0;
            /// <summary>
            /// 变换后的类型 
            /// </summary>
            public string ExchangeData { get; set; } = "float";
        }
        #endregion

        #region 接口实现
        /// <summary>
        /// 获取驱动名称
        /// </summary>
        /// <returns></returns>
        public string GetDriverName()
        {
            return _driverName;
        }
        /// <summary>
        /// 获取可供采集的数据
        /// </summary>
        /// <returns></returns>
        public string[] GetColloctData()
        {
            List<AddressConfig> _addressConfigs = _deviceConfig.AddressConfigs;
            List<string> names = new List<string>();
            if (_addressConfigs != null && _addressConfigs.Count > 0)
            {
                names.AddRange(_addressConfigs.Select(s => s.DataName));
            }
            return names.ToArray();
        }
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return _isConnected;
        }
        /// <summary>
        /// 载入配置
        /// </summary>
        public void LoadConfig()
        {
            if (CheckConnectId(_connectId))
            {
                string configPath = AppDomain.CurrentDomain.BaseDirectory + string.Format(_configFile, _connectId);
                if (!File.Exists(configPath))
                {
                    OnError?.Invoke(new Exception("无效的配置文件：" + configPath), 2, _connectId);
                }
                else
                {
                    _deviceConfig = this.GetConfig<DeviceConfig>(configPath);
                }
                _deviceConfig.ConnectId = _connectId;
            }
        }
        /// <summary>
        /// 载入默认配置
        /// </summary>
        public void LoadDefaultConfig()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + string.Format(_defaultConfigFile);
            if (!File.Exists(configPath))
            {
                OnError?.Invoke(new Exception("无效的默认配置文件：" + configPath), 2, _connectId);
            }
            else
            {
                _deviceConfig = this.GetConfig<DeviceConfig>(configPath);
            }
        }
        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {
            if (_deviceConfig != null)
            {
                string json = FormatConfigToStr(_deviceConfig);
                if (CheckConnectId(_connectId))
                {
                    _deviceConfig.ConnectId = _connectId;
                    string configPath = AppDomain.CurrentDomain.BaseDirectory + string.Format(_configFile, _connectId);
                    this.SaveConfig<DeviceConfig>(_deviceConfig, configPath);
                }
            }
        }
        /// <summary>
        /// 删除配置
        /// </summary>
        public void DeleteConfig()
        {
            if (!string.IsNullOrWhiteSpace(_connectId))
            {
                string configPath = AppDomain.CurrentDomain.BaseDirectory + string.Format(_configFile, _connectId);
                if (File.Exists(configPath))
                {
                    File.Delete(configPath);
                }
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            if (!_isConnected)
            {
                Connect(_deviceConfig.ConnectConfig);
            }
            _colloctMark = true;
            Task.Factory.StartNew(CollectData);
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _colloctMark = false;
            Disconnect();
        }
        /// <summary>
        /// 启动连接
        /// </summary>
        public void Connect()
        {
            if (_deviceConfig.ConnectConfig != null)
            {
                Connect(_deviceConfig.ConnectConfig);
            }
            else
            {
                OnError?.Invoke(new Exception("无效的设备连接配置"), 2, _connectId);
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Disconnect()
        {
            _isConnected = false;
            if (this._modbus != null)
            {
                this._modbus.ConnectClose();
                this._modbus.Dispose();
                this._modbus = null;
            }
            OnDisonnected?.Invoke(_connectId);
        }
        /// <summary>
        /// 获取数据类型
        /// </summary>
        public string GetDataType(string dataName)
        {
            string dataType = "string";
            if (!string.IsNullOrEmpty(dataName) && _deviceConfig.AddressConfigs != null)
            {
                var addr = _deviceConfig.AddressConfigs.Where(s => s.DataName.Equals(dataName)).FirstOrDefault();
                if (addr != null)
                {
                    dataType = addr.DataType;
                }
            }
            return dataType;
        }
        /// <summary>
        /// 获取数据长度(每个16bit)
        /// </summary>
        public int GetDataLength(string dataName)
        {
            int dataLength = 2;
            if (!string.IsNullOrEmpty(dataName) && _deviceConfig.AddressConfigs != null)
            {
                var addr = _deviceConfig.AddressConfigs.Where(s => s.DataName.Equals(dataName)).FirstOrDefault();
                if (addr != null)
                {
                    dataLength = addr.Length;
                    switch (addr.ExchangeData)
                    {
                        case "bool":
                        case "ushort":
                        case "short":
                            dataLength = 1;
                            break;
                        case "uint":
                        case "int":
                        case "float":
                            dataLength = 2;
                            break;
                        case "double":
                            dataLength = 4;
                            break;
                        case "string":
                            dataLength = 16;
                            break;
                    }
                }
            }
            return dataLength;
        }
        /// <summary>
        /// 获取连接Id
        /// </summary>
        public string GetConnectId()
        {
            return _connectId;
        }
        /// <summary>
        /// 设置连接Id
        /// </summary>
        /// <param name="connectId">连接Id</param>
        public void SetConnectId(string connectId)
        {
            if (CheckConnectId(connectId))
            {
                _connectId = connectId;
                _deviceConfig.ConnectId = connectId;
            }
        }
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public string GetConnectString()
        {
            return FormatConfigToStr(_deviceConfig.ConnectConfig);
        }
        /// <summary>
        /// 设置连接字符串
        /// </summary>
        /// <param name="connectString">连接字符串</param>
        public void SetConnectString(string connectString)
        {
            if (CheckConnectId(_connectId))
            {
                _deviceConfig.ConnectConfig = FormatStrToConfig<ConnectConfig>(connectString);
                _deviceConfig.ConnectId = _connectId;
                SaveConfig();
            }
        }
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public string GetAddressString()
        {
            return FormatConfigToStr(_deviceConfig.AddressConfigs);
        }
        /// <summary>
        /// 设置连接字符串
        /// </summary>
        /// <param name="configString">连接字符串</param>
        public void SetAddressString(string addressString)
        {
            if (CheckConnectId(_connectId))
            {
                _deviceConfig.AddressConfigs = FormatStrToConfig<List<AddressConfig>>(addressString);
                _deviceConfig.ConnectId = _connectId;
                SaveConfig();
            }
        }
        /// <summary>
        /// 获取配置文件路径
        /// </summary>
        public string GetConfigPath()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + string.Format(_configFile, _connectId);
            if (!File.Exists(configPath))
            {
                configPath = string.Empty;
            }
            return configPath;
        }
        /// <summary>
        /// 获取帮助文本
        /// </summary>
        public string GetHelpText()
        {
            string helpText = "暂无帮助信息，请联系开发人员";
            if (_deviceConfig != null)
            {
                helpText = _deviceConfig.HelpText;
            }
            return helpText;
        }
        /// <summary>
        /// 检查连接
        /// </summary>
        /// <param name="connectString">连接字符串</param>
        /// <returns></returns>
        public bool CheckConnect(string connectString)
        {
            bool result = false;
            ConnectConfig connectConfig = FormatStrToConfig<ConnectConfig>(connectString);
            Connect(connectConfig);
            if (_isConnected)
            {
                result = true;
            }
            Disconnect();
            return result;
        }

        #region 私有方法
        /// <summary>
        /// 使用配置连接设备
        /// </summary>
        /// <param name="connectConfig"></param>
        private void Connect(ConnectConfig connectConfig)
        {
            _isConnected = false;
            if (this._modbus == null)
            {
                this._modbus = new ModbusRtuToNet(connectConfig.Host, connectConfig.Port, connectConfig.SlaveID);
                _modbus.ReceiveTimeOut = connectConfig.Timeout;
                _modbus.DataFormat = this.Get32bitDataFormat(connectConfig.Exchange32bit);
                _modbus.IsStringReverse = this.IsExchangeString(connectConfig.ExchangeString);
                _modbus.AddressStartWithZero = true;
            }
            OperateResult result = _modbus.ConnectServer();
            if (!result.IsSuccess)
            {
                string message = string.Format("设备连接失败：[主机：{0}:{1}，错误信息：{2}]", connectConfig.Host, connectConfig.Port, result.Message);
                OnError?.Invoke(new Exception(message), 2, _connectId);
                _isConnected = false;
            }
            else
            {
                _isConnected = true;
                OnConnected?.Invoke(_connectId);
            }
        }
        /// <summary>
        /// 获取32位数据解码顺序
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private DataFormat Get32bitDataFormat(string format)
        {
            DataFormat dataFormat = DataFormat.CDAB;
            switch (format)
            {
                case "1234":
                    dataFormat = DataFormat.ABCD;
                    break;
                case "2143":
                    dataFormat = DataFormat.BADC;
                    break;
                case "3412":
                    dataFormat = DataFormat.CDAB;
                    break;
                case "4321":
                    dataFormat = DataFormat.DCBA;
                    break;
            }
            return dataFormat;
        }
        /// <summary>
        /// 获去字符串解码是否需要交换
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private bool IsExchangeString(string format)
        {
            bool dataFormat = true;
            switch (format)
            {
                case "12":
                    dataFormat = false;
                    break;
                case "21":
                    dataFormat = true;
                    break;
            }
            return dataFormat;
        }
        /// <summary>
        /// 格式化连接字符串为配置对象
        /// </summary>
        /// <param name="configString"></param>
        /// <returns></returns>
        private T FormatStrToConfig<T>(string configString)
        {
            T t = default(T);
            if (!string.IsNullOrWhiteSpace(configString))
            {
                t = JsonConvert.DeserializeObject<T>(configString);
            }
            else
            {
                OnError?.Invoke(new Exception("无效的设备配置字符串"), 2, _connectId);
            }
            return t;
        }
        /// <summary>
        /// 序列化配置对象为连接字符串
        /// </summary>
        /// <param name="deviceConfig"></param>
        /// <returns></returns>
        private string FormatConfigToStr<T>(T t)
        {
            string json = string.Empty;
            if (t != null)
            {
                json = JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.Indented);
            }
            else
            {
                OnError?.Invoke(new Exception("无效的设备配置"), 2, _connectId);
            }
            return json;
        }
        /// <summary>
        /// 获取源异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private Exception GetOriginException(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return GetOriginException(ex.InnerException);
            }
            else
            {
                return ex;
            }
        }
        /// <summary>
        /// 采集事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void CollectData()
        {
            while (_colloctMark)
            {
                if (_deviceConfig.AddressConfigs == null)
                {
                    _colloctMark = false;
                    return;
                }
                Dictionary<string, object> dataMap = new Dictionary<string, object>();
                Dictionary<string, string> dataTypeMap = new Dictionary<string, string>();
                try
                {
                    foreach (var address in _deviceConfig.AddressConfigs)
                    {
                        var isValid = ReadDataFromAddress(address, out object value, out string errorMessage);
                        dataMap.Add(address.DataName, value);
                        dataTypeMap.Add(address.DataName, address.ExchangeData);
                        if (!isValid)
                        {
                            string message = string.Format("设备访问失败：[主机：{0}:{1}，错误信息：{2}]",
                                _deviceConfig.ConnectConfig.Host, _deviceConfig.ConnectConfig.Port, errorMessage);
                            OnError?.Invoke(new Exception(message), 1, _connectId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("设备访问失败：[主机：{0}:{1}，错误信息：{2}]",
                                _deviceConfig.ConnectConfig.Host, _deviceConfig.ConnectConfig.Port, ex.Message);
                    OnError?.Invoke(new Exception(message), 1, _connectId);
                }
                OnReceived?.Invoke(dataMap, dataTypeMap, DateTime.Now, _connectId);
                Thread.Sleep(_deviceConfig.ConnectConfig.AcquisitionCycle);
            }
        }
        /// <summary>
        /// 从地址读取数据
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">数据值</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>数据是否有效</returns>
        private bool ReadDataFromAddress(AddressConfig address, out object value, out string errorMessage)
        {
            bool isValid = false;
            value = null;
            errorMessage = string.Empty;
            byte code = 3;
            if (!_isConnected)
            {
                this.Connect();
            }
            switch (address.Area)
            {
                case 0:
                    code = 1;
                    break;
                case 1:
                    code = 2;
                    break;
                case 3:
                    code = 4;
                    break;
                case 4:
                    code = 3;
                    break;
            }
            string key = string.Format("s={0};x={1};{2}", address.SlaveId, code, address.Index);
            switch (address.DataType)
            {
                case "bool":
                    OperateResult<bool> boolResult = null;
                    if (address.Area == 1)
                    {
                        boolResult = _modbus.ReadDiscrete(key);

                    }
                    else
                    {
                        boolResult = _modbus.ReadCoil(key);
                    }
                    if (boolResult.IsSuccess)
                    {
                        isValid = true;
                        value = boolResult.Content;
                    }
                    else
                    {
                        isValid = false;
                        value = null;
                        errorMessage = boolResult.Message;
                    }
                    break;
                case "ushort":
                    OperateResult<ushort> ushortResult = _modbus.ReadUInt16(key);
                    if (ushortResult.IsSuccess)
                    {
                        isValid = true;
                        value = ushortResult.Content;
                    }
                    else
                    {
                        isValid = false;
                        value = null;
                        errorMessage = ushortResult.Message;
                    }
                    break;
                case "short":
                    OperateResult<short> shortResult = _modbus.ReadInt16(key);
                    if (shortResult.IsSuccess)
                    {
                        isValid = true;
                        value = shortResult.Content;
                    }
                    else
                    {
                        isValid = false;
                        value = null;
                        errorMessage = shortResult.Message;
                    }
                    break;
                case "uint":
                    OperateResult<uint> uintResult = _modbus.ReadUInt32(key);
                    if (uintResult.IsSuccess)
                    {
                        isValid = true;
                        value = uintResult.Content;
                    }
                    else
                    {
                        isValid = false;
                        value = null;
                        errorMessage = uintResult.Message;
                    }
                    break;
                case "int":
                    OperateResult<int> intResult = _modbus.ReadInt32(key);
                    if (intResult.IsSuccess)
                    {
                        isValid = true;
                        value = intResult.Content;
                    }
                    else
                    {
                        isValid = false;
                        value = null;
                        errorMessage = intResult.Message;
                    }
                    break;
                case "float":
                    OperateResult<float> floatResult = _modbus.ReadFloat(key);
                    if (floatResult.IsSuccess)
                    {
                        isValid = true;
                        value = floatResult.Content;
                    }
                    else
                    {
                        isValid = false;
                        value = null;
                        errorMessage = floatResult.Message;
                    }
                    break;
                case "string":
                    OperateResult<string> stringResult = _modbus.ReadString(key, address.Length);
                    if (stringResult.IsSuccess)
                    {
                        isValid = true;
                        value = stringResult.Content;
                    }
                    else
                    {
                        isValid = false;
                        value = null;
                        errorMessage = stringResult.Message;
                    }
                    break;
            }
            if (value != null)
            {
                value = ConvertValue(value, address);
            }
            return isValid;
        }
        /// <summary>
        /// 按照倍率和偏移量转换数据的值
        /// </summary>
        /// <returns></returns>
        private object ConvertValue(object value, AddressConfig address)
        {
            double temp = Convert.ToDouble(value);
            if (address.ExchangeType.Equals("21"))
            {
                temp = (temp + address.Offset) * address.Rate;
            }
            else
            {
                temp = temp * address.Rate + address.Offset;
            }
            return DataTypeConvert(temp, address.ExchangeData);
        }
        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private object DataTypeConvert(object data, string dataType)
        {
            object value = null;
            switch (dataType)
            {
                case "bool":
                    value = Convert.ToBoolean(data);
                    break;
                case "ushort":
                    value = Convert.ToUInt16(data);
                    break;
                case "short":
                    value = Convert.ToInt16(data);
                    break;
                case "uint":
                    value = Convert.ToUInt32(data);
                    break;
                case "int":
                    value = Convert.ToInt32(data);
                    break;
                case "float":
                    value = Convert.ToSingle(data);
                    break;
                case "double":
                    value = Convert.ToDouble(data);
                    break;
                case "string":
                    value = data.ToString();
                    break;
            }
            return value;
        }
        /// <summary>
        /// 检查设备连接Id
        /// </summary>
        /// <param name="connectId">设备连接Id</param>
        /// <returns></returns>
        private bool CheckConnectId(string connectId)
        {
            bool result = true;
            if (string.IsNullOrWhiteSpace(connectId))
            {
                OnError?.Invoke(new Exception("无效的设备Id"), 2, connectId);
                result = false;
            }
            return result;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    this.Stop();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~ModbusTcpClient()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
        #endregion

        #region 配置存取
        /// <summary>
        /// 读取配置实例
        /// </summary>
        /// <param name="path">文件绝对路径</param>
        /// <typeparam name="T">返回的参数对象类型</typeparam>
        /// <returns></returns>
        public T GetConfig<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default;
            }
            string json = File.ReadAllText(path, System.Text.Encoding.Default);
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }
            return ToObject<T>(json);

        }
        /// <summary>
        /// 保存配置实例
        /// </summary>
        /// <param name="entity">保存的对象</param>
        /// <param name="path">文件绝对路径</param>
        /// <typeparam name="T">返回的参数对象类型</typeparam>
        /// <returns></returns>
        public void SaveConfig<T>(T entity, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                string json = ToJson(entity);
                byte[] buff = System.Text.Encoding.Default.GetBytes(json);
                fs.Write(buff, 0, buff.Length);
                fs.Flush();
            }
        }
        public T ToObject<T>(string Json)
        {
            return Json == null ? default(T) : JsonConvert.DeserializeObject<T>(Json);
        }
        /// <summary>
        /// 转成json字串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public string ToJson(object obj)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        #endregion
    }
    public class ModbusRtuToNet : NetworkDeviceBase<ModbusRtuMessage, ReverseWordTransform>, IDisposable
    {
        #region Constructor

        /// <summary>
        /// 实例化一个MOdbus-Tcp协议的客户端对象
        /// </summary>
        public ModbusRtuToNet()
        {
            WordLength = 1;
            station = 1;
        }


        /// <summary>
        /// 指定服务器地址，端口号，客户端自己的站号来初始化
        /// </summary>
        /// <param name="ipAddress">服务器的Ip地址</param>
        /// <param name="port">服务器的端口号</param>
        /// <param name="station">客户端自身的站号</param>
        public ModbusRtuToNet(string ipAddress, int port = 502, byte station = 0x01)
        {
            IpAddress = ipAddress;
            Port = port;
            WordLength = 1;
            this.station = station;
        }

        #endregion

        #region Private Member
        private byte station = 0x01;                                // 本客户端的站号
        private bool isAddressStartWithZero = true;                 // 线圈值的地址值是否从零开始
        #endregion

        #region Public Member

        /// <summary>
        /// 获取或设置起始的地址是否从0开始，默认为True
        /// </summary>
        /// <remarks>
        /// <note type="warning">因为有些设备的起始地址是从1开始的，就要设置本属性为<c>True</c></note>
        /// </remarks>
        public bool AddressStartWithZero
        {
            get { return isAddressStartWithZero; }
            set { isAddressStartWithZero = value; }
        }

        /// <summary>
        /// 获取或者重新修改服务器的默认站号信息，当然，你可以再读写的时候动态指定，参见备注
        /// </summary>
        /// <remarks>
        /// 当你调用 ReadCoil("100") 时，对应的站号就是本属性的值，当你调用 ReadCoil("s=2;100") 时，就忽略本属性的值，读写寄存器的时候同理
        /// </remarks>
        public byte Station
        {
            get { return station; }
            set { station = value; }
        }

        /// <summary>
        /// 获取或设置数据解析的格式，默认ABCD，可选BADC，CDAB，DCBA格式
        /// </summary>
        /// <remarks>
        /// 对于Int32,UInt32,float,double,Int64,UInt64类型来说，存在多地址的电脑情况，需要和服务器进行匹配
        /// </remarks>
        public DataFormat DataFormat
        {
            get { return ByteTransform.DataFormat; }
            set { ByteTransform.DataFormat = value; }
        }

        /// <summary>
        /// 字符串数据是否按照字来反转
        /// </summary>
        /// <remarks>
        /// 字符串按照2个字节的排列进行颠倒，根据实际情况进行设置
        /// </remarks>
        public bool IsStringReverse
        {
            get { return ByteTransform.IsStringReverse; }
            set { ByteTransform.IsStringReverse = value; }
        }

        #endregion

        #region Build Command

        /// <summary>
        /// 生成一个读取线圈的指令头
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="length">长度</param>
        /// <returns>包含结果对象的报文</returns>
        public OperateResult<byte[]> BuildReadCoilCommand(string address, ushort length)
        {
            // 解析富地址
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress(address, isAddressStartWithZero, ModbusInfo.ReadCoil);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            // 生成最终rtu指令
            byte[] buffer = ModbusInfo.PackCommandToRtu(analysis.Content.CreateReadCoils(station, length));
            return OperateResult.CreateSuccessResult(buffer);
        }

        /// <summary>
        /// 生成一个读取离散信息的指令头
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="length">长度</param>
        /// <returns>包含结果对象的报文</returns>
        public OperateResult<byte[]> BuildReadDiscreteCommand(string address, ushort length)
        {
            // 解析富地址
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress(address, isAddressStartWithZero, ModbusInfo.ReadDiscrete);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            // 生成最终rtu指令
            byte[] buffer = ModbusInfo.PackCommandToRtu(analysis.Content.CreateReadDiscrete(station, length));
            return OperateResult.CreateSuccessResult(buffer);
        }

        /// <summary>
        /// 生成一个读取寄存器的指令头
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="length">长度</param>
        /// <returns>包含结果对象的报文</returns>
        public OperateResult<byte[]> BuildReadRegisterCommand(string address, ushort length)
        {
            // 解析富地址
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress(address, isAddressStartWithZero, ModbusInfo.ReadRegister);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            // 生成最终rtu指令
            byte[] buffer = ModbusInfo.PackCommandToRtu(analysis.Content.CreateReadRegister(station, length));
            return OperateResult.CreateSuccessResult(buffer);
        }

        /// <summary>
        /// 生成一个读取寄存器的指令头
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="length">长度</param>
        /// <returns>包含结果对象的报文</returns>
        private OperateResult<byte[]> BuildReadRegisterCommand(ModbusAddress address, ushort length)
        {
            // 生成最终rtu指令
            byte[] buffer = ModbusInfo.PackCommandToRtu(address.CreateReadRegister(station, length));
            return OperateResult.CreateSuccessResult(buffer);
        }

        /// <summary>
        /// 生成一个写入单线圈的指令头
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">长度</param>
        /// <returns>包含结果对象的报文</returns>
        public OperateResult<byte[]> BuildWriteOneCoilCommand(string address, bool value)
        {
            // 解析富地址
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress(address, isAddressStartWithZero, ModbusInfo.WriteOneCoil);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            // 生成最终rtu指令
            byte[] buffer = ModbusInfo.PackCommandToRtu(analysis.Content.CreateWriteOneCoil(station, value));
            return OperateResult.CreateSuccessResult(buffer);
        }

        /// <summary>
        /// 生成一个写入单个寄存器的报文
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="values">长度</param>
        /// <returns>包含结果对象的报文</returns>
        public OperateResult<byte[]> BuildWriteOneRegisterCommand(string address, byte[] values)
        {
            // 解析富地址
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress(address, isAddressStartWithZero, ModbusInfo.WriteOneRegister);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            // 生成最终tcp指令
            byte[] buffer = ModbusInfo.PackCommandToRtu(analysis.Content.CreateWriteOneRegister(station, values));
            return OperateResult.CreateSuccessResult(buffer);
        }

        /// <summary>
        /// 生成批量写入单个线圈的报文信息
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="values">实际数据值</param>
        /// <returns>包含结果对象的报文</returns>
        public OperateResult<byte[]> BuildWriteCoilCommand(string address, bool[] values)
        {
            // 解析富地址
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress(address, isAddressStartWithZero, ModbusInfo.WriteCoil);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            // 生成最终rtu指令
            byte[] buffer = ModbusInfo.PackCommandToRtu(analysis.Content.CreateWriteCoil(station, values));
            return OperateResult.CreateSuccessResult(buffer);
        }

        /// <summary>
        /// 生成批量写入寄存器的报文信息
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="values">实际值</param>
        /// <returns>包含结果对象的报文</returns>
        public OperateResult<byte[]> BuildWriteRegisterCommand(string address, byte[] values)
        {
            // 解析富地址
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress(address, isAddressStartWithZero, ModbusInfo.WriteRegister);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            // 生成最终rtu指令
            byte[] buffer = ModbusInfo.PackCommandToRtu(analysis.Content.CreateWriteRegister(station, values));
            return OperateResult.CreateSuccessResult(buffer);
        }

        #endregion

        #region Core Interative

        /// <summary>
        /// 检查当前的Modbus-Tcp响应是否是正确的
        /// </summary>
        /// <param name="send">发送的数据信息</param>
        /// <returns>带是否成功的结果数据</returns>
        private OperateResult<byte[]> CheckModbusTcpResponse(byte[] send)
        {
            OperateResult<byte[]> resultReceive = ReadFromCoreServer(send);

            if (!resultReceive.IsSuccess)
            {
                return resultReceive;
            }

            // 长度校验
            if (resultReceive.Content.Length < 5)
            {
                return new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort + "5");
            }

            // 检查crc
            if (!SoftCRC16.CheckCRC16(resultReceive.Content))
            {
                return new OperateResult<byte[]>(StringResources.Language.ModbusCRCCheckFailed +
                SoftBasic.ByteToHexString(resultReceive.Content, ' '));
            }
            // 发生了错误
            if ((send[1] + 0x80) == resultReceive.Content[1])
            {
                return new OperateResult<byte[]>(resultReceive.Content[2], ModbusInfo.GetDescriptionByErrorCode(resultReceive.Content[2]));
            }
            if (send[1] != resultReceive.Content[1])
            {
                return new OperateResult<byte[]>(resultReceive.Content[1], $"Receive Command Check Failed: ");
            }
            // 移除CRC校验
            byte[] buffer = new byte[resultReceive.Content.Length - 2];
            Array.Copy(resultReceive.Content, 0, buffer, 0, buffer.Length);
            return OperateResult.CreateSuccessResult(buffer);
        }
        #endregion

        #region Read Support

        /// <summary>
        /// 读取服务器的数据，需要指定不同的功能码
        /// </summary>
        /// <param name="code">指令</param>
        /// <param name="address">地址</param>
        /// <param name="length">长度</param>
        /// <returns>带是否成功的结果数据</returns>
        private OperateResult<byte[]> ReadModBusBase(byte code, string address, ushort length)
        {
            OperateResult<byte[]> command = null;
            switch (code)
            {
                case ModbusInfo.ReadCoil:
                    {
                        command = BuildReadCoilCommand(address, length);
                        break;
                    }
                case ModbusInfo.ReadDiscrete:
                    {
                        command = BuildReadDiscreteCommand(address, length);
                        break;
                    }
                case ModbusInfo.ReadRegister:
                    {
                        command = BuildReadRegisterCommand(address, length);
                        break;
                    }
                default: command = new OperateResult<byte[]>(StringResources.Language.ModbusTcpFunctionCodeNotSupport); break;
            }
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(command);

            OperateResult<byte[]> resultBytes = CheckModbusTcpResponse(command.Content);
            if (resultBytes.IsSuccess)
            {
                // 二次数据处理
                if (resultBytes.Content?.Length >= 3)
                {
                    byte[] buffer = new byte[resultBytes.Content.Length - 3];
                    Array.Copy(resultBytes.Content, 3, buffer, 0, buffer.Length);
                    resultBytes.Content = buffer;
                }
            }
            return resultBytes;
        }

        /// <summary>
        /// 读取服务器的数据，需要指定不同的功能码
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="length">长度</param>
        /// <returns>带是否成功的结果数据</returns>
        private OperateResult<byte[]> ReadModBusBase(ModbusAddress address, ushort length)
        {
            OperateResult<byte[]> command = BuildReadRegisterCommand(address, length);
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(command);

            OperateResult<byte[]> resultBytes = CheckModbusTcpResponse(command.Content);
            if (resultBytes.IsSuccess)
            {
                // 二次数据处理
                if (resultBytes.Content?.Length >= 3)
                {
                    byte[] buffer = new byte[resultBytes.Content.Length - 3];
                    Array.Copy(resultBytes.Content, 3, buffer, 0, buffer.Length);
                    resultBytes.Content = buffer;
                }
            }
            return resultBytes;
        }

        /// <summary>
        /// 读取线圈，需要指定起始地址
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的bool对象</returns>
        public OperateResult<bool> ReadCoil(string address)
        {
            var read = ReadCoil(address, 1);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>(read);

            return OperateResult.CreateSuccessResult(read.Content[0]);
        }

        /// <summary>
        /// 批量的读取线圈，需要指定起始地址，读取长度
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <param name="length">读取长度</param>
        /// <returns>带有成功标志的bool数组对象</returns>
        public OperateResult<bool[]> ReadCoil(string address, ushort length)
        {
            var read = ReadModBusBase(ModbusInfo.ReadCoil, address, length);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>(read);

            return OperateResult.CreateSuccessResult(SoftBasic.ByteToBoolArray(read.Content, length));
        }

        /// <summary>
        /// 读取输入线圈，需要指定起始地址
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <returns>带有成功标志的bool对象</returns>
        public OperateResult<bool> ReadDiscrete(string address)
        {
            var read = ReadDiscrete(address, 1);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>(read);

            return OperateResult.CreateSuccessResult(read.Content[0]);
        }

        /// <summary>
        /// 批量的读取输入点，需要指定起始地址，读取长度
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <param name="length">读取长度</param>
        /// <returns>带有成功标志的bool数组对象</returns>
        public OperateResult<bool[]> ReadDiscrete(string address, ushort length)
        {
            var read = ReadModBusBase(ModbusInfo.ReadDiscrete, address, length);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>(read);

            return OperateResult.CreateSuccessResult(SoftBasic.ByteToBoolArray(read.Content, length));
        }
        /// <summary>
        /// 从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"，或者是带功能码格式x=3;1234</param>
        /// <param name="length">读取的数量</param>
        /// <returns>带有成功标志的字节信息</returns>
        /// <remarks>
        /// 富地址格式，支持携带站号信息，功能码信息，具体参照类的示例代码
        /// </remarks>
        /// <example>
        /// 此处演示批量读取的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Modbus\Modbus.cs" region="ReadExample1" title="Read示例" />
        /// </example>
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress(address, isAddressStartWithZero, ModbusInfo.ReadRegister);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            List<byte> lists = new List<byte>();
            ushort alreadyFinished = 0;
            while (alreadyFinished < length)
            {
                ushort lengthTmp = (ushort)Math.Min((length - alreadyFinished), 120);
                OperateResult<byte[]> read = ReadModBusBase(analysis.Content.AddressAdd(alreadyFinished), lengthTmp);
                if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(read);

                lists.AddRange(read.Content);
                alreadyFinished += lengthTmp;
            }
            return OperateResult.CreateSuccessResult(lists.ToArray());
        }
        #endregion

        #region Write One Register

        /// <summary>
        /// 写一个寄存器数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="high">高位</param>
        /// <param name="low">地位</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteOneRegister(string address, byte high, byte low)
        {
            OperateResult<byte[]> command = BuildWriteOneRegisterCommand(address, new byte[] { high, low });
            if (!command.IsSuccess) return command;

            return CheckModbusTcpResponse(command.Content);
        }

        /// <summary>
        /// 写一个寄存器数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteOneRegister(string address, short value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteOneRegister(address, buffer[1], buffer[0]);
        }

        /// <summary>
        /// 写一个寄存器数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteOneRegister(string address, ushort value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteOneRegister(address, buffer[1], buffer[0]);
        }

        #endregion

        #region Write Coil

        /// <summary>
        /// 写一个线圈信息，指定是否通断
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteCoil(string address, bool value)
        {
            OperateResult<byte[]> command = BuildWriteOneCoilCommand(address, value);
            if (!command.IsSuccess) return command;

            return CheckModbusTcpResponse(command.Content);
        }

        /// <summary>
        /// 批量写线圈信息，指定是否通断
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>返回写入结果</returns>
        public OperateResult WriteCoil(string address, bool[] values)
        {
            OperateResult<byte[]> command = BuildWriteCoilCommand(address, values);
            if (!command.IsSuccess) return command;

            return CheckModbusTcpResponse(command.Content);
        }

        #endregion

        #region Write Base


        /// <summary>
        /// 将数据写入到Modbus的寄存器上去，需要指定起始地址和数据内容
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"</param>
        /// <param name="value">写入的数据，长度根据data的长度来指示</param>
        /// <returns>返回写入结果</returns>
        /// <remarks>
        /// 富地址格式，支持携带站号信息，功能码信息，具体参照类的示例代码
        /// </remarks>
        /// <example>
        /// 此处演示批量写入的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Modbus\Modbus.cs" region="WriteExample1" title="Write示例" />
        /// </example>
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<byte[]> command = BuildWriteRegisterCommand(address, value);
            if (!command.IsSuccess) return command;

            return CheckModbusTcpResponse(command.Content);
        }


        #endregion

        #region Bool Support

        /// <summary>
        /// 批量读取线圈或是离散的数据信息，需要指定地址和长度，具体的结果取决于实现
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>带有成功标识的bool[]数组</returns>
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress(address, isAddressStartWithZero, ModbusInfo.ReadCoil);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<bool[]>(analysis);

            var read = ReadModBusBase((byte)analysis.Content.Function, address, length);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>(read);

            return OperateResult.CreateSuccessResult(SoftBasic.ByteToBoolArray(read.Content, length));
        }

        /// <summary>
        /// 向线圈中写入bool数组，返回是否写入成功
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，长度为8的倍数</param>
        /// <returns>返回写入结果</returns>
        public override OperateResult Write(string address, bool[] values)
        {
            return WriteCoil(address, values);
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return $"ModbusRtuToNet[{IpAddress}:{Port}]";
        }

        #endregion
    }
    /// <summary>
    /// Modbus-Tcp协议支持的消息解析类
    /// </summary>
    public class ModbusRtuMessage : INetMessage
    {
        /// <summary>
        /// 消息头的指令长度
        /// </summary>
        public int ProtocolHeadBytesLength
        {
            get { return 0; }
        }


        /// <summary>
        /// 从当前的头子节文件中提取出接下来需要接收的数据长度
        /// </summary>
        /// <returns>返回接下来的数据内容长度</returns>
        public int GetContentLengthByHeadBytes()
        {
            /************************************************************************
             * 
             *    说明：为了应对有些特殊的设备，在整个指令的开端会增加一个额外的数据的时候
             * 
             ************************************************************************/

            if (SendBytes?.Length >= 8)
            {
                int length = SendBytes[4] * 256 + SendBytes[5];
                if (length > 0)
                {
                    return length * 2 + 5;
                }
            }
            return 0;
        }


        /// <summary>
        /// 检查头子节的合法性
        /// </summary>
        /// <param name="token">特殊的令牌，有些特殊消息的验证</param>
        /// <returns>是否成功的结果</returns>
        public bool CheckHeadBytesLegal(byte[] token)
        {
            return true;
        }


        /// <summary>
        /// 获取头子节里的消息标识
        /// </summary>
        /// <returns>消息标识</returns>
        public int GetHeadBytesIdentity()
        {
            return 0;
        }


        /// <summary>
        /// 消息头字节
        /// </summary>
        public byte[] HeadBytes { get; set; }


        /// <summary>
        /// 消息内容字节
        /// </summary>
        public byte[] ContentBytes { get; set; }


        /// <summary>
        /// 发送的字节信息
        /// </summary>
        public byte[] SendBytes { get; set; }


    }
}
