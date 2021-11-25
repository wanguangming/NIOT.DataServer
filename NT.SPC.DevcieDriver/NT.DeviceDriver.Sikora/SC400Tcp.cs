using HslCommunication.BasicFramework;
using HslCommunication.Serial;
using HslCommunication.Enthernet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication;
using HslCommunication.ModBus;
using System.Text.RegularExpressions;
using HslCommunication.Core.Net;
using HslCommunication.Core.IMessage;

namespace NT.DeviceDriver.Beta
{
    /// <summary>
    /// 通讯协议的类库
    /// </summary>
    public class SC400Tcp : NetworkDeviceBase<ModbusTcpMessage, ReverseWordTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个Modbus-Rtu协议的客户端对象
        /// </summary>
        public SC400Tcp()
        {
            ByteTransform = new ReverseWordTransform();
        }
        #endregion

        #region Private Member

        private byte station = ModbusInfo.ReadCoil;                  // 本客户端的站号
        private bool isAddressStartWithZero = true;                  // 线圈值的地址值是否从零开始

        #endregion

        #region Build Command
        /// <summary>
        /// 生成一个写入单个寄存器的报文
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="data">长度</param>
        /// <returns>包含结果对象的报文</returns>
        public OperateResult<byte[]> BuildWriteOneRegisterCommand(string address, byte[] data)
        {
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress(address, isAddressStartWithZero, ModbusInfo.WriteOneRegister);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            // 生成最终rtu指令
            byte[] buffer = ModbusInfo.PackCommandToRtu(analysis.Content.CreateWriteOneRegister(station, data));
            return OperateResult.CreateSuccessResult(buffer);
        }
        #endregion

        #region Core Interative

        /// <summary>
        /// 检查当前的Modbus-Rtu响应是否是正确的
        /// </summary>
        /// <param name="send">发送的数据信息</param>
        /// <returns>带是否成功的结果数据</returns>
        protected virtual OperateResult<byte[]> CheckModbusTcpResponse(byte[] send)
        {
            byte[] sendbuffer = new byte[send.Length + 3];
            sendbuffer[0] = 02;
            Array.Copy(send, 0, sendbuffer, 1, send.Length);
            sendbuffer[send.Length + 1] = 03;
            sendbuffer[send.Length + 2] = BCCCheck(send);
            // 核心交互
            OperateResult<byte[]> result = ReadFromCoreServer(sendbuffer);
            if (!result.IsSuccess) return result;

            // 长度校验
            if (result.Content.Length < 5) return new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort + "5");

            // 检查BCC
            if (!CheckReceiveBytes(result.Content)) return new OperateResult<byte[]>(StringResources.Language.ModbusCRCCheckFailed +
                SoftBasic.ByteToHexString(result.Content, ' '));

            // 移除BCC校验
            byte[] buffer = new byte[result.Content.Length - 3];
            Array.Copy(result.Content, 1, buffer, 0, buffer.Length);
            return OperateResult.CreateSuccessResult(buffer);
        }

        #endregion

        #region Protect Override

        /// <summary>
        /// 检查当前接收的字节数据是否正确的
        /// </summary>
        /// <param name="rBytes">从设备反馈回来的数据</param>
        /// <returns>是否校验成功</returns>
        protected bool CheckReceiveBytes(byte[] rBytes)
        {
            byte[] check = new byte[rBytes.Length - 2];
            Array.Copy(rBytes, 1, check, 0, check.Length);
            var r = check.Aggregate(0, (a, b) => b ^ a);
            if (r < 0x20) r += 0x20;
            return (byte)r == rBytes[rBytes.Length - 1];
        }

        /// <summary>
        /// 检查当前接收的字节数据是否正确的
        /// </summary>
        /// <param name="rBytes">从设备反馈回来的数据</param>
        /// <returns>是否校验成功</returns>
        protected byte BCCCheck(byte[] rBytes)
        {
            var r = rBytes.Aggregate(0, (a, b) => b ^ a);
            r ^= 0x03;
            if (r < 0x20) r += 0x20;
            return (byte)r;
        }

        #endregion

        #region Read Support
        /// <summary>
        /// 从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"，或者是带功能码格式x=3;1234</param>
        /// <param name="length">读取的数量</param>
        /// <returns>带有成功标志的字节信息</returns>
        public OperateResult<byte[]> ReadDB(string SendChar)
        {
            OperateResult<byte[]> read = CheckModbusTcpResponse(Encoding.Default.GetBytes(SendChar));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(read);
            return OperateResult.CreateSuccessResult(read.Content);
        }
        /// <summary>
        /// 从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"，或者是带功能码格式x=3;1234</param>
        /// <param name="length">读取的数量</param>
        /// <returns>带有成功标志的字节信息</returns>
        public OperateResult<short> ReadShort(string SendChar)
        {
            OperateResult<byte[]> read = CheckModbusTcpResponse(Encoding.Default.GetBytes(SendChar));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<short>(read);
            return OperateResult.CreateSuccessResult(Convert.ToInt16(Encoding.Default.GetString(read.Content)));
        }

        /// <summary>
        /// 从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"，或者是带功能码格式x=3;1234</param>
        /// <param name="length">读取的数量</param>
        /// <returns>带有成功标志的字节信息</returns>
        public OperateResult<float> ReadFloats(string SendChar, ushort length)
        {
            OperateResult<byte[]> read = CheckModbusTcpResponse(Encoding.Default.GetBytes(SendChar));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<float>(read);
            string amount = string.Empty;
            string aaa = Encoding.Default.GetString(read.Content);
            if (IsNumeric(Encoding.Default.GetString(read.Content)))
                amount = Convert.ToDecimal(aaa).ToString(string.Format("F{0}", length));
            else
            {
                amount = "0.00";
            }
            return OperateResult.CreateSuccessResult((float)(Convert.ToDouble(amount) / Math.Pow(10, (double)length)));
        }

        public bool IsNumeric(string str) //接收一个string类型的参数,保存到str里
        {
            if (str == null || str.Length == 0)    //验证这个参数是否为空
                return false;                           //是，就返回False
            ASCIIEncoding ascii = new ASCIIEncoding();//new ASCIIEncoding 的实例
            byte[] bytestr = ascii.GetBytes(str);         //把string类型的参数保存到数组里

            foreach (byte c in bytestr)                   //遍历这个数组里的内容
            {
                if (c < 48 || c > 57)                          //判断是否为数字
                {
                    return false;                              //不是，就返回False
                }
            }
            return true;                                        //是，就返回True
        }
        #endregion

        #region Write

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return $"ModbusTcp[{IpAddress}:{Port}]";
        }

        #endregion

    }
}
