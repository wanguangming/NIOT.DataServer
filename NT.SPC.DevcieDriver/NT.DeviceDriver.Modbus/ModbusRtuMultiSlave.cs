using Newtonsoft.Json;
using NT.SPC.DataBank;
using HslCommunication;
using HslCommunication.Core;
using HslCommunication.ModBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;
using System.Threading;

namespace NT.DeviceDriver.Modbus
{
    /// <summary>
    /// Modbus-RTU客户端（区分从站地址数据高低位）
    /// </summary>
    public class ModbusRtuMultiSlave : IDeviceDriver
    {
        public event SPC.DataBank.ReceivedEventHandler OnReceived;
        public event SPC.DataBank.ErrorOccurredEventHandler OnError;
        public event SPC.DataBank.ConnectedEventHandler OnConnected;
        public event SPC.DataBank.DisonnectedEventHandler OnDisonnected;
        #region 字段、属性与构造方法
        private readonly string _driverName = "Modbus Rtu MultiSlave";
        /// <summary>
        /// 配置文件
        /// </summary>
        private string _configFile = "Resources\\DeviceDriver\\{0}.json";
        /// <summary>
        /// 配置文件
        /// </summary>
        private string _defaultConfigFile = "Resources\\DeviceDriver\\Default\\Modbus-RTU-MultiSlave.json";
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
        private ModbusRtu _modbus = null;
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
            /// 串口名
            /// </summary>
            public string PortName { get; set; } = "COM1";
            /// <summary>
            /// 波特率
            /// </summary>
            public int BaudRate { get; set; }
            /// <summary>
            /// 数据位
            /// </summary>
            public int DataBits { get; set; }
            /// <summary>
            /// 停止位
            /// </summary>
            public decimal StopBits { get; set; }
            /// <summary>
            /// 校验位
            /// </summary>
            public string Parity { get; set; }
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
            /// 原始数据类型
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
            /// <summary>
            /// 交换32位数据高低位
            /// </summary>
            public string Endian { get; set; } = "";
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
            if (this._modbus != null)
            {
                this._modbus.Close();
                this._modbus.Dispose();
                this._modbus = null;
            }
            _isConnected = false;
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
                this._modbus = new ModbusRtu(connectConfig.SlaveID);
                SerialInit(_modbus, connectConfig);
                //_modbus.SerialPortInni(connectConfig.PortName, connectConfig.BaudRate, connectConfig.DataBits, connectConfig.StopBits, connectConfig.Parity);
            }
            if (!_modbus.IsOpen())
            {
                _modbus.Open();
            }
            if (!_modbus.IsOpen())
            {
                string message = string.Format("串口连接失败：[端口：{0},{1},{2},{3},{4}]", connectConfig.PortName, connectConfig.BaudRate, connectConfig.DataBits, connectConfig.StopBits, connectConfig.Parity);
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
                json = JsonConvert.SerializeObject(t, Formatting.Indented);
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
                if (!_modbus.IsOpen())
                {
                    _colloctMark = false;
                    string message = string.Format("串口连接失败：[端口：{0}]]",
                            _deviceConfig.ConnectConfig.PortName);
                    OnError?.Invoke(new Exception(message), 2, _connectId);
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
                            string message = string.Format("数据读取失败：[端口：{0},数据：{1}]，错误信息：{2}]",
                                _deviceConfig.ConnectConfig.PortName, address.DataName, errorMessage);
                            OnError?.Invoke(new Exception(message), 1, _connectId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("数据读取失败：[端口：{0}，错误信息：{1}]",
                                _deviceConfig.ConnectConfig.PortName, ex.Message);
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
            if (!string.IsNullOrWhiteSpace(address.Endian))
            {
                value = ExchangeValue(value, address);
            }
            if (value != null)
            {
                value = ConvertValue(value, address);
            }
            return isValid;
        }
        /// <summary>
        /// 交换高低位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        private object ExchangeValue(object value, AddressConfig address)
        {
            switch (address.DataType)
            {
                case "uint":
                    var uintValue = Convert.ToUInt32(value);
                    var uintSource = BitConverter.GetBytes(uintValue);
                    var uintTarget = NumbericExchange(uintSource, address);
                    value = BitConverter.ToUInt32(uintTarget, 0);
                    break;
                case "int":
                    var intValue = Convert.ToInt32(value);
                    var intSource = BitConverter.GetBytes(intValue);
                    var intTarget = NumbericExchange(intSource, address);
                    value = BitConverter.ToInt32(intTarget, 0);
                    break;
                case "float":
                    var floatValue = Convert.ToSingle(value);
                    var floatSource = BitConverter.GetBytes(floatValue);
                    var floatTarget = NumbericExchange(floatSource, address);
                    value = BitConverter.ToUInt32(floatTarget, 0);
                    break;
                case "string":
                    var stringValue = Convert.ToString(value);
                    var stringSource = Encoding.ASCII.GetBytes(stringValue);
                    var stringTarget = StringExchange(stringSource, address);
                    value = Encoding.ASCII.GetString(stringTarget);
                    break;
            }
            return value;
        }
        /// <summary>
        /// 数值高低位交换
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="addressConfig"></param>
        /// <returns></returns>
        private byte[] NumbericExchange(byte[] buff, AddressConfig addressConfig)
        {
            var oldDataFormat = Get32bitDataFormat(_deviceConfig.ConnectConfig.Exchange32bit);
            var tempList = buff.ToList();
            if (oldDataFormat == DataFormat.ABCD)
            {
                tempList = new List<byte>() { tempList[0], tempList[1], tempList[2], tempList[3] };
            }
            else if (oldDataFormat == DataFormat.BADC)
            {
                tempList = new List<byte>() { tempList[1], tempList[0], tempList[3], tempList[2] };
            }
            else if (oldDataFormat == DataFormat.DCBA)
            {
                tempList = new List<byte>() { tempList[3], tempList[2], tempList[1], tempList[0] };
            }
            else
            {
                tempList = new List<byte>() { tempList[2], tempList[3], tempList[0], tempList[1] };
            }
            DataFormat dataFormat = Get32bitDataFormat(addressConfig.Endian);
            if (dataFormat == DataFormat.ABCD)
            {
                return new byte[] { tempList[0], tempList[1], tempList[2], tempList[3] };
            }
            else if (dataFormat == DataFormat.BADC)
            {
                return new byte[] { tempList[1], tempList[0], tempList[3], tempList[2] };
            }
            else if (dataFormat == DataFormat.DCBA)
            {
                return new byte[] { tempList[3], tempList[2], tempList[1], tempList[0] };
            }
            else
            {
                return new byte[] { tempList[2], tempList[3], tempList[0], tempList[1] };
            }

        }
        /// <summary>
        /// 字符高低位交换
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="addressConfig"></param>
        /// <returns></returns>
        private byte[] StringExchange(byte[] buff, AddressConfig addressConfig)
        {
            var tempList = buff.ToList();
            if (tempList.Count % 2 != 0)
            {
                tempList.Add(0);
            }
            var oldList = new List<byte>();
            var oldIsExchange = IsExchangeString(_deviceConfig.ConnectConfig.ExchangeString);
            if (oldIsExchange)
            {
                for (int i = 0; i < tempList.Count; i = i + 2)
                {
                    oldList.Add(tempList[i + 1]);
                    oldList.Add(tempList[i]);
                }
            }
            else
            {
                oldList.AddRange(tempList);
            }
            var newList = new List<byte>();
            bool isExchange = IsExchangeString(addressConfig.Endian);
            if (isExchange)
            {
                for (int i = 0; i < oldList.Count; i = i + 2)
                {
                    newList.Add(oldList[i + 1]);
                    newList.Add(oldList[i]);
                }
            }
            else
            {
                newList.AddRange(oldList);
            }
            return newList.ToArray();
        }
        /// <summary>
        /// 转换数据的值
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
        /// 转换数据
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
        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <param name="modbus"></param>
        /// <param name="connectConfig"></param>
        private void SerialInit(ModbusRtu modbus, ConnectConfig connectConfig)
        {
            StopBits stopBits = StopBits.One;
            switch (connectConfig.StopBits)
            {
                case 1:
                    stopBits = StopBits.One;
                    break;
                case 1.5M:
                    stopBits = StopBits.OnePointFive;
                    break;
                case 2:
                    stopBits = StopBits.Two;
                    break;
                case 0:
                    stopBits = StopBits.None;
                    break;
            }
            Parity parity = Parity.None;
            switch (connectConfig.Parity)
            {
                case "None":
                    parity = Parity.None;
                    break;
                case "Odd":
                    parity = Parity.Odd;
                    break;
                case "Even":
                    parity = Parity.Even;
                    break;
                case "Mark":
                    parity = Parity.Mark;
                    break;
                case "Space":
                    parity = Parity.Space;
                    break;
            }
            modbus.Station = 1;
            modbus.DataFormat = this.Get32bitDataFormat(connectConfig.Exchange32bit);
            modbus.IsStringReverse = this.IsExchangeString(connectConfig.ExchangeString);
            modbus.AddressStartWithZero = true;
            modbus.SerialPortInni(connectConfig.PortName, connectConfig.BaudRate, connectConfig.DataBits, stopBits, parity);
            modbus.ReceiveTimeout = connectConfig.Timeout;
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
}
