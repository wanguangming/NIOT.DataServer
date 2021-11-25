using System;

namespace NT.Tools.Common
{
    /// <summary>
    /// 数据转换
    /// </summary>
    public class DataConvert
    {
        /// <summary>
        /// 数据库int转bool型
        /// </summary>
        /// <param name="num">整数</param>
        /// <returns></returns>
        public static bool IntToBool(int num)
        {
            if (num <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 将DateTime类型转换为时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static double GetTimeStamp(System.DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            double unixTime = System.Math.Round((time - startTime).TotalMilliseconds);
            return unixTime;
        }
        /// <summary>
        /// 将时间戳类型转换为DateTime
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(double timeStamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            TimeSpan ts = TimeSpan.FromMilliseconds(timeStamp);
            DateTime nowTime = startTime + ts;
            return nowTime;
        }
    }
}
