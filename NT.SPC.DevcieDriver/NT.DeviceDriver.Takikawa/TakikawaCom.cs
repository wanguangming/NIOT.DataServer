using HslCommunication.BasicFramework;
using HslCommunication.Serial;
using System;
using System.Text;
using HslCommunication.Core;
using HslCommunication;

namespace NT.DeviceDriver.Takikawa
{
    /// <summary>
    /// 通讯协议的类库，多项式码0xA001
    /// </summary>
    public class TakikawaCom : SerialDeviceBase<ReverseWordTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个协议的客户端对象
        /// </summary>
        public TakikawaCom()
        {
            ByteTransform = new ReverseWordTransform();
        }
        #endregion

        #region Build Command
        /// <summary>
        /// 生成一个写入单个寄存器的报文
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="data">长度</param>
        /// <returns>包含结果对象的报文</returns>
        //public OperateResult<byte[]> BuildWriteOneRegisterCommand( string address, byte[] data )
        //{
        //    OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress( address, isAddressStartWithZero, ModbusInfo.WriteOneRegister );
        //    if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

        //    // 生成最终rtu指令
        //    byte[] buffer = ModbusInfo.PackCommandToRtu( analysis.Content.CreateWriteOneRegister( station, data ) );
        //    return OperateResult.CreateSuccessResult( buffer );
        //}
        #endregion

        #region Core Interative

        /// <summary>
        /// 检查当前的Modbus-Rtu响应是否是正确的
        /// </summary>
        /// <param name="send">发送的数据信息</param>
        /// <returns>带是否成功的结果数据</returns>
        protected virtual OperateResult<byte[]> CheckModbusTcpResponse(byte[] send)
        {
            // 核心交互
            OperateResult<byte[]> result = ReadBase(send);
            if (!result.IsSuccess) return result;

            // 长度校验
            if (result.Content.Length < 5) return new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort + "5");

            // 检查crc
            if (result.Content[0] != send[0]) return new OperateResult<byte[]>(StringResources.Language.ModbusCRCCheckFailed +
                SoftBasic.ByteToHexString(result.Content, ' '));

            // 移除CRC校验
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
        protected override bool CheckReceiveBytes(byte[] rBytes)
        {
            return SoftCRC16.CheckCRC16(rBytes);
        }

        #endregion

        #region Read Support

        /// <summary>
        /// 从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"，或者是带功能码格式x=3;1234</param>
        /// <param name="length">读取的数量</param>
        /// <returns>带有成功标志的字节信息</returns>
        public OperateResult<short> ReadShort(string SendChar)
        {
            OperateResult<byte[]> read = ReadBase(Encoding.Default.GetBytes(SendChar));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<short>(read);
            return OperateResult.CreateSuccessResult(Convert.ToInt16(Encoding.Default.GetString(read.Content).Trim()));
        }

        /// <summary>
        /// 从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度
        /// </summary>
        /// <param name="address">起始地址，格式为"1234"，或者是带功能码格式x=3;1234</param>
        /// <param name="length">读取的数量</param>
        /// <returns>带有成功标志的字节信息</returns>
        public OperateResult<float> ReadFloats(string SendChar, ushort length)
        {
            OperateResult<byte[]> read = ReadBase(Encoding.Default.GetBytes(SendChar));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<float>(read);
            string amount = string.Empty;
            string trimStr = Encoding.Default.GetString(read.Content).Trim().TrimStart('\r', '\n').TrimEnd('\r', '\n').Trim();
            if (IsNumeric(trimStr))
                amount = Convert.ToDecimal(trimStr).ToString(string.Format("F{0}", length));
            else
            {
                amount = "0.00";
            }
            return OperateResult.CreateSuccessResult(Convert.ToSingle(amount));
        }

        public bool IsNumeric(string str) //接收一个string类型的参数,保存到str里
        {
            if (str == null || str.Length == 0)    //验证这个参数是否为空
                return false;                           //是，就返回False
            ASCIIEncoding ascii = new ASCIIEncoding();//new ASCIIEncoding 的实例
            byte[] bytestr = ascii.GetBytes(str);         //把string类型的参数保存到数组里

            foreach (byte c in bytestr)                   //遍历这个数组里的内容
            {
                if ((c < 48 || c > 57) && c != 46 && c != 32)                          //判断是否为数字以及小数点
                {
                    return false;                              //不是，就返回False
                }
            }
            return true;                                        //是，就返回True
        }
        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return $"Beta[{PortName}:{BaudRate}]";
        }

        #endregion

    }
}
