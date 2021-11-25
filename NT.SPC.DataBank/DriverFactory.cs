using NT.Tools.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NT.SPC.DataBank
{
    public class DriverFactory
    {
        private static readonly Dictionary<string, Type> devcieTypes = new Dictionary<string, Type>();
        private static readonly string driverDllPath = "Resources\\DeviceDriver\\DLL";
        /// <summary>
        /// 获取所有支持的设备名
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSupportDevices()
        {
            return devcieTypes.Keys.ToList();
        }
        /// <summary>
        /// 根据生产厂家名与型号名获取设备接口
        /// </summary>
        /// <param name="deviceName">设备名</param>
        /// <returns></returns>
        public static IDeviceDriver CreateDevice(string deviceName)
        {
            if (devcieTypes.ContainsKey(deviceName))
            {
                Type type = devcieTypes[deviceName];
                IDeviceDriver stdDevice = (IDeviceDriver)Activator.CreateInstance(type);
                return stdDevice;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 加载支持的设备
        /// </summary>
        public static void LoadSupportDevice()
        {
            devcieTypes.Clear();
            List<Type> totalTypes = new List<Type>();
            //添加已知驱动类型
            #region 加载dll插件
            List<string> dllFiles = new List<string>();
            //扫描驱动目录
            string dllDir = DirFileHelper.GetCurrentDirectory() + driverDllPath;
            string[] dllPaths = DirFileHelper.GetFileNames(dllDir, "*.DLL");
            if (dllPaths != null)
            {
                dllFiles.AddRange(dllPaths);
            }
            #endregion
            //获取匹配的驱动类型
            
            foreach (var dllFile in dllFiles)
            {
                Assembly assembly = Assembly.LoadFrom(dllFile);
                var types = assembly.GetTypes()
                    .Where(type => !string.IsNullOrEmpty(type.Namespace))
                    .Where(type => type.GetInterface(typeof(IDeviceDriver).Name) != null);
                totalTypes.AddRange(types);
            }
            //添加扫描到的类型
            foreach (var type in totalTypes)
            {
                IDeviceDriver stdDevice = (IDeviceDriver)Activator.CreateInstance(type);
                string name = stdDevice.GetDriverName();
                if (!devcieTypes.ContainsKey(name))
                {
                    devcieTypes.Add(name, type);
                }
                stdDevice.Dispose();
            }
        }
    }
}
