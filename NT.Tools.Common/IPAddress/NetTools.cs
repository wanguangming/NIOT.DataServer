using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace NT.Tools.Common
{
    /// <summary>
    /// IP地址工具
    /// </summary>
    public class NetTools
    {
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLocalIP()
        {
            List<string> ipList = new List<string>();
            string HostName = Dns.GetHostName(); //得到主机名
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
            for (int i = 0; i < IpEntry.AddressList.Length; i++)
            {
                //从IP地址列表中筛选出IPv4类型的IP地址
                //AddressFamily.InterNetwork表示此IP为IPv4,
                //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ipList.Add(IpEntry.AddressList[i].ToString());
                }
            }
            return ipList;
        }
        /// <summary>
        /// IP地址验证
        /// </summary>
        /// <param name="ipaddr">IP地址字符串</param>
        /// <returns></returns>
        public static bool IsIPAddress(string ipaddr)
        {
            if (string.IsNullOrWhiteSpace(ipaddr))
            {
                return false;
            }
            //判断是否为xxx.xxx.xxx.xxx
            string[] ipSpan = ipaddr.Split('.');
            if (ipSpan.Length != 4)
            {
                return false;
            }
            //检查每一段是否为正整数且在0-255之间
            foreach (var span in ipSpan)
            {
                if (IsUnsignInt(span))
                {
                    int spanNum = int.Parse(span);
                    if (spanNum > 255)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 判断字符串是否为有效端口号
        /// </summary>
        /// <param name="port"></param>
        /// <param name="portNum"></param>
        /// <returns></returns>
        public static bool IsPort(string port, out int portNum)
        {
            portNum = 0;
            if (string.IsNullOrWhiteSpace(port))
            {
                return false;
            }
            if (!IsInt(port))
            {
                return false;
            }
            portNum = Convert.ToInt32(port);
            if (portNum < 0 || portNum > 65535)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 生成两个IP之间的所有IP集合
        /// </summary>
        /// <param name="startIp"></param>
        /// <param name="endIp"></param>
        /// <returns></returns>
        public static List<string> IpSegmentGenerate(string startIp, string endIp)
        {
            List<string> ipList = new List<string>();
            uint iStartip = IpToInt(startIp);
            uint iEndIp = IpToInt(endIp);
            StringBuilder ip_result = new StringBuilder();
            if (iEndIp >= iStartip)
            {
                for (uint ip = iStartip; ip <= iEndIp; ip++)
                {
                    ipList.Add(IntToIp(ip));
                }
            }
            return ipList;
        }
        /// <summary>
        /// 将IP字符串转化为数字
        /// </summary>
        /// <param name="ipStr">IP字符串</param>
        /// <returns></returns>
        public static uint IpToInt(string ipStr)
        {
            string[] ip = ipStr.Split('.');
            uint ipcode = 0xFFFFFF00 | byte.Parse(ip[3]);
            ipcode = ipcode & 0xFFFF00FF | (uint.Parse(ip[2]) << 0x8);
            ipcode = ipcode & 0xFF00FFFF | (uint.Parse(ip[1]) << 0xF);
            ipcode = ipcode & 0x00FFFFFF | (uint.Parse(ip[0]) << 0x18);
            return ipcode;
        }
        /// <summary>
        /// 将数字转化为IP字符串
        /// </summary>
        /// <param name="ipcode">IP对应的数字</param>
        /// <returns></returns>
        public static string IntToIp(uint ipcode)
        {
            byte a = (byte)((ipcode & 0xFF000000) >> 0x18);
            byte b = (byte)((ipcode & 0x00FF0000) >> 0xF);
            byte c = (byte)((ipcode & 0x0000FF00) >> 0x8);
            byte d = (byte)(ipcode & 0x000000FF);
            string ipStr = string.Format("{0}.{1}.{2}.{3}", a, b, c, d);
            return ipStr;
        }
        #region 判断字符串是否为数字
        /// <summary>
        /// 判断字符串是否为数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }
        /// <summary>
        /// 判断字符串是否为整型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }
        /// <summary>
        /// 判断字符串是否为无符号整型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUnsignInt(string value)
        {
            return Regex.IsMatch(value, @"^\d*$");
        }
        /// <summary>
        /// 判断字符串是否为无符号数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUnsign(string value)
        {
            return Regex.IsMatch(value, @"^\d*[.]?\d*$");
        }
        #endregion
    }
}
