using NT.Tools.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT.SPC.ServerConfig
{
    /// <summary>
    /// 节点配置
    /// </summary>
    public class GroupConfigDal
    {
        /// <summary>
        /// 配置文件
        /// </summary>
        public string ConfigFile { get; private set; } = "Resources\\Config\\GroupConfig.json";
        public GroupConfigDal()
        {

        }
        public GroupConfigDal(string configFile)
        {
            this.ConfigFile = configFile;
        }
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <returns>数据分组配置</returns>
        public List<GroupConfigEntity> GetGroupConfig()
        {
            string path = DirFileHelper.GetAbsolutePath(ConfigFile);
            List<GroupConfigEntity> groups = ConfigManage.GetConfig<List<GroupConfigEntity>>(path);
            return groups;
        }
        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="groups">数据分组配置</param>
        /// <returns></returns>
        public void SaveGroupConfig(List<GroupConfigEntity> groups)
        {
            string path = DirFileHelper.GetAbsolutePath(ConfigFile);
            ConfigManage.SaveConfig(groups, path);
        }
    }
}
