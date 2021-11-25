using NT.Tools.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT.SPC.ServerConfig
{
    /// <summary>
    /// 设备配置
    /// </summary>
    public class DeviceConfigDal
    {
        /// <summary>
        /// 配置文件
        /// </summary>
        public string ConfigFile { get; private set; } = "Resources\\Config\\DeviceConfig.json";
        public DeviceConfigDal()
        {

        }
        public DeviceConfigDal(string configFile)
        {
            this.ConfigFile = configFile;
        }
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <returns>数据分组配置</returns>
        public List<DeviceConfigEntity> GetDeviceConfig()
        {
            string path = DirFileHelper.GetAbsolutePath(ConfigFile);
            List<DeviceConfigEntity> groups = ConfigManage.GetConfig<List<DeviceConfigEntity>>(path);
            return groups;
        }
        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="groups">数据分组配置</param>
        /// <returns></returns>
        public void SaveDeviceConfig(List<DeviceConfigEntity> groups)
        {
            string path = DirFileHelper.GetAbsolutePath(ConfigFile);
            ConfigManage.SaveConfig(groups, path);
        }
    }
}
