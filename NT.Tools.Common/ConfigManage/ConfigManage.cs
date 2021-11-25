using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace NT.Tools.Common
{
    /// <summary>
    /// 配置文件操作类
    /// </summary>
    public class ConfigManage
    {
        #region appSettings配置节
        /// <summary>
        /// 根据Key取Value值
        /// </summary>
        /// <param name="key">appSettings</param>
        public static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key].ToString().Trim();
        }
        /// <summary>
        /// 根据所有AppSetting键值对
        /// </summary>
        public static Dictionary<string, string> GetAllPairs()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                var value = ConfigurationManager.AppSettings[key].ToString().Trim();
                keyValuePairs.Add(key, value);
            }
            return keyValuePairs;
        }
        /// <summary>
        /// 根据Key修改Value值
        /// </summary>
        /// <param name="key">要修改的Key</param>
        /// <param name="value">要修改为的值</param>
        public static void SetValue(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            bool exist = false;
            foreach (string exKey in config.AppSettings.Settings.AllKeys)
            {
                if (key == exKey)
                {
                    exist = true;
                }
            }
            if (exist)
            {
                config.AppSettings.Settings[key].Value = value;
            }
            else
            {
                config.AppSettings.Settings.Add(key, value);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        #endregion

        #region 读取Json配置文件
        /// <summary>
        /// 读取配置实例
        /// </summary>
        /// <param name="name">配置类名</param>
        /// <param name="path">文件绝对路径</param>
        /// <typeparam name="T">返回的参数对象类型</typeparam>
        /// <returns></returns>
        public static T GetConfig<T>(string name, string path)
        {
            if (!File.Exists(path))
            {
                return default;
            }
            string json = File.ReadAllText(path, System.Text.Encoding.Default);
            var allConfig = json.ToJObject();
            if (!allConfig.ContainsKey(name))
            {
                return default;
            }
            var partConfig = allConfig[name];
            var result = partConfig.ToObject<T>();
            return result;
        }
        /// <summary>
        /// 读取配置实例
        /// </summary>
        /// <param name="path">文件绝对路径</param>
        /// <typeparam name="T">返回的参数对象类型</typeparam>
        /// <returns></returns>
        public static T GetConfig<T>(string path)
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
            return json.ToObject<T>();

        }
        /// <summary>
        /// 保存配置实例
        /// </summary>
        /// <param name="entity">保存的对象</param>
        /// <param name="path">文件绝对路径</param>
        /// <typeparam name="T">返回的参数对象类型</typeparam>
        /// <returns></returns>
        public static void SaveConfig<T>(T entity, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                string json = JsonConvert.SerializeObject(entity, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                byte[] buff = System.Text.Encoding.Default.GetBytes(json);
                fs.Write(buff, 0, buff.Length);
                fs.Flush();
            }
        }
        /// <summary>
        /// 保存配置实例
        /// </summary>
        /// <param name="name">配置节名</param>
        /// <param name="entity">保存的对象</param>
        /// <param name="path">文件绝对路径</param>
        /// <typeparam name="T">返回的参数对象类型</typeparam>
        /// <returns></returns>
        public static void SaveConfig<T>(string name, T entity, string path)
        {
            //string text = File.ReadAllText(path, System.Text.Encoding.Default);

            Newtonsoft.Json.Linq.JObject allConfig = new Newtonsoft.Json.Linq.JObject();
            using (System.IO.StreamReader file = System.IO.File.OpenText(path))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    allConfig = (JObject)JToken.ReadFrom(reader);
                }
            }

            var item = Newtonsoft.Json.Linq.JToken.FromObject(entity);
            if (!allConfig.ContainsKey(name))
            {
                allConfig.Add(name, item);
            }
            else
            {
                allConfig[name] = item;
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                string json = allConfig.ToJson();
                byte[] buff = System.Text.Encoding.Default.GetBytes(json);
                fs.Write(buff, 0, buff.Length);
                fs.Flush();
            }
        }
        #endregion

        #region 读取 ini 配置文件
        /// <summary>
        /// 从ini文件获取数据
        /// </summary>
        /// <param name="iniFile">ini格式配置文件</param>
        /// <param name="section">应用程序</param>
        /// <param name="key">键的名称</param>
        /// <returns>键的值</returns>
        public static string GetProfileString(string iniFile, string section, string key)
        {
            if (!DirFileHelper.IsExistsFile(iniFile))
            {
                return "";
            }
            int nCapacity = 255;
            StringBuilder temp = new StringBuilder(nCapacity);
            int i = GetPrivateProfileString(section, key, "", temp, nCapacity, iniFile);

            if (i < 0)
                return "";

            return temp.ToString();
        }
        /// <summary>
        /// 向ini文件中写入值
        /// </summary>
        /// <param name="iniFile">ini格式配置文件</param>
        /// <param name="section">应用程序</param>
        /// <param name="key">键的名称</param>
        /// <param name="value">键的值</param>
        /// <returns>执行成功为1，失败为0。</returns>
        public static long WriteProfileString(string iniFile, string section, string key, string value)
        {
            if (!DirFileHelper.IsExistsFile(iniFile))
            {
                return 0;
            }
            if (section.Trim().Length <= 0 || key.Trim().Length <= 0 || value.Trim().Length <= 0)
                return 0;

            return WritePrivateProfileString(section, key, value, iniFile);
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        #endregion
    }
}
