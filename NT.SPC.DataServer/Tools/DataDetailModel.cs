using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT.SPC.DataServer
{
    /// <summary>
    /// 数据详情
    /// </summary>
    public class DataDetailModel
    {
        /// <summary>
        /// 自定名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据名称
        /// </summary>
        public string DataName { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        public string DeviceState { get; set; }
        /// <summary>
        /// 数据值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// OpcUa地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Modbus区域
        /// </summary>
        public byte Area { get; set; }
        /// <summary>
        /// Modbus寄存器地址
        /// </summary>
        public int RegAddress { get; set; }
    }
}
