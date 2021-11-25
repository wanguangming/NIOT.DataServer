using NT.Tools.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT.SPC.DataServer
{
    static class Program
    {
        private static readonly NT.Tools.Log.Log logger = NT.Tools.Log.LogFactory.GetLogger(typeof(Program));
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            #region 异常捕获
            //设置应用程序处理异常方式：ThreadException处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //处理非UI线程异常
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //确保只有一个程序运行
            Process instance = RunningInstance();
            if (instance == null)
            {
                Application.Run(new MainForm());
            }
            else
            {
                HandleRunningInstance(instance);
            }
        }

        #region 异常处理
        /// <summary>
        /// 处理UI线程异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ShowMsgBox(e.Exception);
        }
        /// <summary>
        /// 处理未捕获的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            ShowMsgBox(ex);
        }
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="ex"></param>
        static void ShowMsgBox(Exception ex)
        {
            Exception exception = ex.GetOriginalException();
            logger.Error(exception);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("应用程序发生了错误，信息如下：");
            sb.AppendLine();
            sb.AppendLine(exception.Message);
            sb.AppendLine();
            sb.AppendFormat(@"详情请查看错误日志 => ~\Log\{0}", DateTime.Now.ToString("yyyy-MM-dd"));
            MessageBox.Show(sb.ToString(), "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region 检查程序是否运行
        /// <summary>
        /// 检查程序是否运行
        /// </summary>
        private static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            //遍历与当前进程名称相同的进程列表
            foreach (Process process in processes)
            {
                //如果实例已经存在则忽略当前进程
                if (process.Id != current.Id)
                {
                    //保证要打开的进程同已经存在的进程来自同一文件路径
                    if (process.MainModule.FileName == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 展示已运行的程序
        /// </summary>
        /// <param name="instance"></param>
        private static void HandleRunningInstance(Process instance)
        {
            IntPtr intPtr = instance.MainWindowHandle;
            if (intPtr == IntPtr.Zero)
            {
                intPtr = FindWindow(null, "OPCUA");
            }
            ShowWindowAsync(intPtr, 1); //调用api函数，正常显示窗口
            SetForegroundWindow(instance.MainWindowHandle); //将窗口放置最前端
        }
        /// <summary>
        /// 打开指定句柄的窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="cmdShow"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        /// <summary>
        /// 将指定句柄的窗口激活到最前
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        /// <summary>
        /// 根据窗口标题查找窗体
        /// </summary>
        /// <param name="lpClassName">窗体的类</param>
        /// <param name="lpWindowName">窗体的标题</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        #endregion
    }
}
