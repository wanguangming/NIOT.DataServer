using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT.Tools.Common
{
    /// <summary>
    /// 异常处理
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 递归获取源异常
        /// </summary>
        /// <param name="ex">抛出的异常对象</param>
        /// <returns></returns>
        public static Exception GetOriginalException(this Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex;
            }
            else
            {
                return ex.InnerException.GetOriginalException();
            }
        }
    }
}
