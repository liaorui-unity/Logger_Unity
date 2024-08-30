/// <summary>
/// 封装Debug日志打印，实现日志开关控制、彩色日志打印、日志文件存储等功能
/// </summary>

using UnityEngine;
using System.Text;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace LogInfo
{
    public class Info : UnityEngine.Debug
    {
        //项目名
        public static string debugName;
        //项目文件夹日期
        public static string debugDate;
        //项目文件名
        public static string debugFileDate;

        public static string debugIp
        {
            get => GetLocalIPv4();
        }

#region 设置服务端IP
        /// <summary>
        /// 互联网地址
        /// </summary>
        public static string uploadNET { get; private set; } = "101.33.207.93";
        /// <summary>
        /// 本地局域网地址
        /// </summary>
        public static string uploadLAN { get; private set; } = "127.0.0.1";
        /// <summary>
        /// 链接端口
        /// </summary>
        public static int    uploadPort { get; private set; } = 8765;
        #endregion

        // 日志文件存储位置
        public static string logFileSavePath { get; private set; }

        /// <summary>
        /// 项目是否提交到局域网
        /// </summary>
        public static bool debugTypeLAN = true;

      
        // 保留日志文件夹数量
        public static int keepFolder { get; private set; }= 2;

        // 使用StringBuilder来优化字符串的重复构造
        private static StringBuilder s_logStr = new StringBuilder();


        internal static Thread saveThread;


        public static void SetUploadNET(string ip, int port)
        {
            uploadNET  = ip;
            uploadPort = port;
        }
        public static void SetUploadLAN(string ip, int port)
        {
            uploadLAN  = ip;
            uploadPort = port;
        }

        /// <summary>
        /// 初始化，在游戏启动的入口脚本的Awake函数中调用Debug.Init
        /// </summary>
        public static void Init(string savePath)
        {
            var date  = System.DateTime.Now.ToString("yyyyMMdd-hhmmss");
            var dates = date.Split('-');

            debugName     = Application.productName;
            debugDate     = dates.First();
            debugFileDate = dates.Last();


            var logFolder   = string.Format("{0}/{1}", savePath, "Log");
            var dubugFolder = string.Format("{0}/{1}", logFolder, debugDate);
            logFileSavePath = string.Format("{0}/{1}.log", dubugFolder, debugFileDate);

            if (!Directory.Exists(dubugFolder))      Directory.CreateDirectory(dubugFolder);

            Application.logMessageReceivedThreaded += OnLogCallBack;

            SortAndDestroy(logFolder);

            Log("项目名字："       +  debugName);
            Log("日志时间："       +  debugDate);

            Log("本地日志地址："    +  logFileSavePath);
            Log("服务器日志地址："  +  uploadNET);

            saveThread = new Thread(Save);
            saveThread.IsBackground = true;
            saveThread.Start();

            Log("开启日志线程");
        }


        public static void Abort()
        {
            saveThread.Abort();
            Application.logMessageReceivedThreaded -= OnLogCallBack;
        }


        static void SortAndDestroy(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            var dires = info.GetDirectories().ToList();

            dires.Sort((x, y) =>
            {
                if (int.Parse(x.Name) > int.Parse(y.Name))
                    return -1;
                else
                    return 1;
            });

            for (int i = keepFolder; i < dires.Count; i++)
            {
                Directory.Delete(dires[i].FullName,true);
            }
        }


        public static string GetLocalIPv4()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();
        }


        /// <summary>
        /// 打印日志回调
        /// </summary>
        /// <param name="condition">日志文本</param>
        /// <param name="stackTrace">调用堆栈</param>
        /// <param name="type">日志类型</param>
        private static void OnLogCallBack(string condition, string stackTrace, LogType type)
        {
            condition = string.Format("[{0}]{1}", System.DateTime.Now.ToString("hh:mm:ss"), condition);

            s_logStr.AppendLine(condition);

            if (!string.IsNullOrEmpty(stackTrace))
            {
                var stacks = stackTrace.Split('\n');
                foreach (var item in stacks)
                {
                    s_logStr.AppendLine(item);
                }
            }
        }


        public static void Save()
        {
            while (true)
            {
                if (s_logStr.Length > 0)
                {
                    if (!File.Exists(logFileSavePath))
                    {
                        var fs = File.Create(logFileSavePath);
                        fs.Close();
                    }
                    using (var sw = File.AppendText(logFileSavePath))
                    {
                        sw.WriteLine(s_logStr.ToString());
                    }

                    s_logStr.Clear();
                }

                Thread.Sleep(2000);
            }
        }

        public static void Log(string str)
        {
            Log(str,null);
        }

        /// <summary>
        /// 带颜色的日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color">颜色值，例：green, yellow，#ff0000</param>
        /// <param name="context">上下文对象</param>
        public static void LogWithColor(object message, string color, Object context = null)
        {
            Log(FmtColor(color, message), context);
        }

        /// <summary>
        /// 红色日志
        /// </summary>
        public static void LogRed(object message, Object context = null)
        {
            Log(FmtColor("red", message), context);
        }

        /// <summary>
        /// 绿色日志
        /// </summary>
        public static void LogGreen(object message, Object context = null)
        {
            Log(FmtColor("green", message), context);
        }

        /// <summary>
        /// 黄色日志
        /// </summary>
        public static void LogYellow(object message, Object context = null)
        {
            Log(FmtColor("yellow", message), context);
        }

        /// <summary>
        /// 青蓝色日志
        /// </summary>
        public static void LogCyan(object message, Object context = null)
        {
            UnityEngine.Debug.Log(FmtColor("#00ffff", message), context);
        }

        /// <summary>
        /// 格式化颜色日志
        /// </summary>
        private static object FmtColor(string color, object obj)
        {
            if (obj is string value)
            {
#if !UNITY_EDITOR
                return value;
#else
                return FmtColor(color, value);
#endif
            }
            else
            {
#if !UNITY_EDITOR
                return obj;
#else
                return string.Format("<color={0}>{1}</color>", color, obj);
#endif
            }
        }

        /// <summary>
        /// 格式化颜色日志
        /// </summary>
        private static object FmtColor(string color, string msg)
        {
#if !UNITY_EDITOR
            return msg;
#else
            int p = msg.IndexOf('\n');
            if (p >= 0) p = msg.IndexOf('\n', p + 1);// 可以同时显示两行
            if (p < 0 || p >= msg.Length - 1) return string.Format("<color={0}>{1}</color>", color, msg);
            if (p > 2 && msg[p - 1] == '\r') p--;
            return string.Format("<color={0}>{1}</color>{2}", color, msg.Substring(0, p), msg.Substring(p));
#endif
        }

#region 解决日志双击溯源问题
#if Test
    [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
    static bool OnOpenAsset(int instanceID, int line)
    {
        string stackTrace = GetStackTrace();
        if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains("GameLogger:Log"))
        {
            // 使用正则表达式匹配at的哪个脚本的哪一行
            var matches = System.Text.RegularExpressions.Regex.Match(stackTrace, @"\(at (.+)\)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string pathLine = "";
            while (matches.Success)
            {
                pathLine = matches.Groups[1].Value;

                if (!pathLine.Contains("GameLogger.cs"))
                {
                    int splitIndex = pathLine.LastIndexOf(":");
                    // 脚本路径
                    string path = pathLine.Substring(0, splitIndex);
                    // 行号
                    line = System.Convert.ToInt32(pathLine.Substring(splitIndex + 1));
                    string fullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                    fullPath = fullPath + path;
                    // 跳转到目标代码的特定行
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                    break;
                }
                matches = matches.NextMatch();
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取当前日志窗口选中的日志的堆栈信息
    /// </summary>
    /// <returns></returns>
    static string GetStackTrace()
    {
        // 通过反射获取ConsoleWindow类
        var ConsoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
        // 获取窗口实例
        var fieldInfo = ConsoleWindowType.GetField("ms_ConsoleWindow",
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.NonPublic);
        var consoleInstance = fieldInfo.GetValue(null);
        if (consoleInstance != null)
        {
            if ((object)UnityEditor.EditorWindow.focusedWindow == consoleInstance)
            {
                // 获取m_ActiveText成员
                fieldInfo = ConsoleWindowType.GetField("m_ActiveText",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);
                // 获取m_ActiveText的值
                string activeText = fieldInfo.GetValue(consoleInstance).ToString();
                return activeText;
            }
        }
        return null;
    }
#endif
#endregion 解决日志双击溯源问题
    }
}