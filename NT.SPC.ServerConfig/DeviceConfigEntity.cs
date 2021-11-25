using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT.SPC.ServerConfig
{
    /// <summary>
    /// 设备配置实体
    /// </summary>
    public class DeviceConfigEntity
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string DeviceGuid { get; set; }
        /// <summary>
        /// 驱动名称
        /// </summary>
        public string DriverName { get; set; }
        /// <summary>
        /// 自定义设备名称
        /// </summary>
        public string CustomName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }
    }
}
