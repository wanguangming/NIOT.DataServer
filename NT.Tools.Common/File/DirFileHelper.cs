using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace NT.Tools.Common
{
    public class DirFileHelper
    {
        #region 文件或文件夹是否存在
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool IsExistsFile(string filePath)
        {
            return File.Exists(filePath);
        }
        /// <summary>
        /// 检测指定目录是否存在
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        #endregion

        #region 创建文件和文件夹
        /// <summary>
        /// 创建文件夹（仅当文件夹不存在）
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        public static void CreateDirectory(string directoryPath)
        {
            if (!IsExistDirectory(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
        /// <summary>
        /// 创建文件（仅当文件不存在）
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateFile(string filePath)
        {
            if (!IsExistsFile(filePath))
            {
                File.Create(filePath);
            }
        }
        #endregion

        #region 删除文件和文件夹
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void DeleteFile(string filePath)
        {
            if (IsExistsFile(filePath))
            {
                File.Delete(filePath);
            }
        }
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        public static void DeleteDirectory(string dirPath)
        {
            if (IsExistDirectory(dirPath))
            {
                Directory.Delete(dirPath);
            }
        }
        #endregion

        #region 重命名文件和文件夹
        /// <summary>
        /// 文件重命名
        /// </summary>
        /// <param name="path">文件全路径</param>
        /// <param name="newName">新文件名</param>
        /// <returns>新文件全路径</returns>
        public static string RenameFile(string path, string newName)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistsFile(path))
            {
                throw new FileNotFoundException();
            }
            string newPath = GetParentDirectory(path) + "\\" + newName;
            File.Move(path, newPath);
            return newPath;
        }
        /// <summary>
        /// 文件夹重命名
        /// </summary>
        /// <param name="path">文件夹全路径</param>
        /// <param name="newName">新文件夹名</param>
        /// <returns>新文件夹全路径</returns>
        public static string RenameDirectory(string path, string newName)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(path))
            {
                throw new FileNotFoundException();
            }
            string newPath = GetParentDirectory(path) + "\\" + newName;
            Directory.Move(path, newPath);
            return newPath;
        }
        #endregion

        #region 获取文件列表
        /// <summary>
        /// 获取指定目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetFileNames(string directoryPath)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }
            //获取文件列表
            return Directory.GetFiles(directoryPath);
        }
        /// <summary>
        /// 获取指定目录中所有子目录列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetDirectories(string directoryPath)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }
            return Directory.GetDirectories(directoryPath);
        }
        /// <summary>
        /// 获取指定目录及子目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetFileNames(string directoryPath, string searchPattern = "*", bool isSearchChild = false)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                return null;
            }
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 从文件的绝对路径中获取文件信息
        /// <summary>
        /// 从文件的绝对路径中获取父目录
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetParentDirectory(string filePath)
        {
            //获取文件的名称
            return Path.GetDirectoryName(filePath);
        }
        /// <summary>
        /// 从文件的绝对路径中获取文件名( 包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetFileName(string filePath)
        {
            //获取文件的名称
            return Path.GetFileName(filePath);
        }
        /// <summary>
        /// 从文件的绝对路径中获取扩展名
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetExtension(string filePath)
        {
            //获取文件的名称
            return Path.GetExtension(filePath);
        }
        /// <summary>
        /// 从文件的绝对路径中获取文件名(不包含扩展名)
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetFileNameNoExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }
        /// <summary>
        /// 获取文件的绝对路径
        /// </summary>
        /// <param name="filePath">文件的路径</param>        
        public static string GetAbsolutePath(string filePath)
        {
            return Path.GetFullPath(filePath);
        }
        #endregion

        #region 移动文件(剪贴--粘贴)
        /// <summary>
        /// 移动文件(剪贴--粘贴)
        /// </summary>
        /// <param name="sourceFile">要移动的文件的路径及全名(包括后缀)</param>
        /// <param name="targetFile">文件移动到新的位置,并指定新的文件名</param>
        public static void MoveFile(string sourceFile, string targetFile)
        {
            if (IsExistsFile(sourceFile))
            {
                File.Move(sourceFile, targetFile);
            }
        }
        #endregion

        #region 复制与备份
        /// <summary>
        /// 复制文件到指定全路径（包括文件名）
        /// </summary>
        /// <param name="sourceFile">源文件</param>
        /// <param name="targetFile">目标文件</param>
        /// <param name="overwrite">是否覆盖</param>
        public static void CopyFile(string sourceFile, string targetFile, bool overwrite = false)
        {
            if (IsExistsFile(sourceFile))
            {
                File.Copy(sourceFile, targetFile, overwrite);
            }
        }
        /// <summary>
        /// 复制文件到目标文件夹
        /// </summary>
        /// <param name="sourceFile">源文件</param>
        /// <param name="targetFile">目标文件</param>
        /// <param name="overwrite">是否覆盖</param>
        public static void CopyFileToDir(string sourceFile, string targetDir, bool overwrite = false)
        {
            if (IsExistsFile(sourceFile))
            {
                string fileName = GetFileName(sourceFile);
                File.Copy(sourceFile, targetDir + fileName, overwrite);
            }
        }
        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="filePath">源文件</param>
        /// <param name="suffix">备份后缀</param>
        /// <param name="overwrite">是否覆盖</param>
        public static string BackupFile(string filePath, string suffix = ".bak", bool overwrite = false)
        {
            string backupFile = filePath + suffix;
            File.Copy(filePath, backupFile, overwrite);
            return backupFile;
        }
        #endregion

        #region 获得程序路径
        /// <summary>
        /// 获得程序路径
        /// </summary>
        public static string GetCurrentDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
        #endregion

        #region 打开目录
        /// <summary>
        /// 打开路径并定位文件
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        public static void ExplorerFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;
            else
                Process.Start(@"explorer.exe", "/select,\"" + filePath + "\"");

        }
        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="dirPath">目录绝对路径</param>
        public static void ExplorerDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                return;
            else
                Process.Start(@"explorer.exe", dirPath);
        }
        #endregion
    }
}
