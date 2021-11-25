using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT.SPC.ServerConfig
{
    /// <summary>
    /// 分组配置实体
    /// </summary>
    public class GroupConfigEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 节点标签
        /// </summary>
        public string Tag { get; set; } = "Group";
        /// <summary>
        /// 分组中的节点
        /// </summary>
        public List<NodeConfigEntity> Nodes { get; set; } = new List<NodeConfigEntity>();
    }
    /// <summary>
    /// 节点配置实体
    /// </summary>
    public class NodeConfigEntity
    {
        /// <summary>
        /// 所属设备Id
        /// </summary>
        public string DeviceGuid { get; set; }
        /// <summary>
        /// 所属分组Id
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 数据名称
        /// </summary>
        public string DataName { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 节点标签
        /// </summary>
        public string Tag { get; set; } = "Data";
        /// <summary>
        /// Modbus区域
        /// </summary>
        public byte Area { get; set; }
        /// <summary>
        /// Modbus寄存器地址
        /// </summary>
        public int RegAddress { get; set; }
        /// <summary>
        /// Modbus寄存器个数
        /// </summary>
        public int RegLength { get; set; }
    }
}
