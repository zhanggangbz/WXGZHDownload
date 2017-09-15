using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXGZHArticle
{
    /// <summary>
    /// 写日志类
    /// </summary>
    public class LogUtil
    {
        #region 字段
        public static object _lock = new object();
        public static string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "wx\\log";
        public static int fileSize = 10 * 1024 * 1024; //日志分隔文件大小
        #endregion

        #region 写文件
        /// <summary>
        /// 写文件
        /// </summary>
        public static void WriteFile(string log, string path)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }

                if (!File.Exists(path))
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create)) { fs.Close(); }
                }

                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        #region 日志内容
                        string value = string.Format(@"{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), log);
                        #endregion

                        sw.WriteLine(value);
                        sw.Flush();
                    }
                    fs.Close();
                }
            }
            catch { }
        }
        #endregion

        #region 生成日志文件路径
        /// <summary>
        /// 生成日志文件路径
        /// </summary>
        public static string CreateLogPath()
        {
            int index = 0;
            string logPath;
            bool bl = true;
            do
            {
                index++;
                logPath = Path.Combine(path, "Log" + DateTime.Now.ToString("yyyyMMdd") + (index == 1 ? "" : "_" + index.ToString()) + ".txt");
                if (File.Exists(logPath))
                {
                    FileInfo fileInfo = new FileInfo(logPath);
                    if (fileInfo.Length < fileSize)
                    {
                        bl = false;
                    }
                }
                else
                {
                    bl = false;
                }
            } while (bl);

            return logPath;
        }
        #endregion

        #region 写错误日志
        /// <summary>
        /// 写错误日志
        /// </summary>
        public static void LogError(string log)
        {
            Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    WriteFile("[Error] " + log, CreateLogPath());
                }
            });
        }
        #endregion

        #region 写操作日志
        /// <summary>
        /// 写操作日志
        /// </summary>
        public static void Log(string log)
        {
            Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    WriteFile("[Info]  " + log, CreateLogPath());
                }
            });
        }
        #endregion

    }
}
