namespace NT.Tools.Common
{
    /// <summary>
    /// 操作基类
    /// </summary>
    public class BaseOpModel
    {
        /// <summary>
        /// 错误代码(0为成功，1为失败)
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 操作代码
        /// </summary>
        public string OpCode { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 获取操作失败结果对象
        /// </summary>
        /// <param name="msg">返回信息</param>
        /// <param name="opCode">操作代码</param>
        /// <returns></returns>
        public static BaseOpModel Fail(string msg = "Fail", string opCode = null)
        {
            BaseOpModel baseOpModel = new BaseOpModel
            {
                Code = 1,
                Msg = msg,
                OpCode = opCode
            };
            return baseOpModel;
        }
        /// <summary>
        /// 获取操作成功结果对象
        /// </summary>
        /// <param name="msg">返回信息</param>
        /// <param name="opCode">操作代码</param>
        /// <returns></returns>
        public static BaseOpModel Success(string msg = "Success", string opCode = null)
        {
            BaseOpModel baseOpModel = new BaseOpModel
            {
                Code = 0,
                Msg = msg,
                OpCode = opCode
            };
            return baseOpModel;
        }
    }
    /// <summary>
    /// 操作基类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class BaseOpModel<T>
    {
        /// <summary>
        /// 结果代码(0为成功，1为失败)
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 操作代码
        /// </summary>
        public string OpCode { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 获取操作失败结果对象
        /// </summary>
        /// <param type="T">结果类型</param>
        /// <param name="msg">返回信息</param>
        /// <param name="data">结果数据</param>
        /// <param name="opCode">操作代码</param>
        /// <returns></returns>
        public static BaseOpModel<T> Fail(string msg, T data, string opCode = null)
        {
            BaseOpModel<T> baseOpModel = new BaseOpModel<T>
            {
                Code = 1,
                Msg = msg,
                Data = data,
                OpCode = opCode
            };
            return baseOpModel;
        }
        /// <summary>
        /// 获取操作成功结果对象
        /// </summary>
        /// <param type="T">结果类型</param>
        /// <param name="msg">返回信息</param>
        /// <param name="data">结果数据</param>
        /// <param name="opCode">操作代码</param>
        /// <returns></returns>
        public static BaseOpModel<T> Success(string msg, T data, string opCode = null)
        {
            BaseOpModel<T> baseOpModel = new BaseOpModel<T>
            {
                Code = 0,
                Msg = msg,
                Data = data,
                OpCode = opCode
            };
            return baseOpModel;
        }
    }
}
