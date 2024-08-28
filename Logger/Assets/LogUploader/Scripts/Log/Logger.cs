/// <summary>
/// 实现日志上传到服务器的功能
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sailfish.Log
{
    public struct LogData
    {
        public LogType type;
        public string  log;
        public string  stackTrace;
        public string  timer;
    }


    public class Logger : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitLogger()
        {
            Application.logMessageReceived += Recv;
            Debug.s_debugLogEnable = true;
        }

        internal static Logger instance;
        internal static Queue<LogData> msgQues = new Queue<LogData>();

        public static UnityAction<bool> TriggerCall;

        private Transform     panel;
        private LogGroup      logGroup;
        private LogTrace      logTrace;
        private MouseGesture  gesture;
        private LoggerPanel   logPanel;

     
        void Awake()
        {   
            ReadConfig();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Debug.Init(Application.streamingAssetsPath);
#else
            Debug.Init(Application.persistentDataPath);
#endif

            panel    = transform.Find("MainPanel");
            logTrace = transform.GetComponentInChildren<LogTrace>(true);
            logGroup = transform.GetComponentInChildren<LogGroup>(true);
            logPanel = new LoggerPanel(panel);
            logGroup . Init();
            instance = this;
        }
        void ReadConfig()
        {
            try
            {
                Debug.uploadLAN = File.ReadAllText(Application.streamingAssetsPath + "/Logger/ip.txt");
            }
            catch
            {
                Debug.LogError("没有配置LoggerIP 默认自身IP");
            }
            Debug.Log("配置表IP:" + Debug.uploadLAN);
        }


        void Start()
        {
            gesture = new  MouseGesture();
            gesture . Call = logPanel . SetPanel;
        }


        static void Recv(string condition, string stackTrace, LogType type)
        {
            LogData data;
            data.log        = condition;
            data.type       = type;
            data.stackTrace = stackTrace;
            data.timer      = $"[<color=#51F7F8>{DateTime.Now.ToString("yy/MM/dd HH:mm:ss:ffff")}</color>]";
            msgQues.Enqueue(data);
        }


        private void Update()
        {
            if (gesture != null) gesture.Check();

            while (msgQues.Count > 0)
            {
                logGroup?.AddBubble(msgQues.Dequeue());
            }
        }


        public void SelectLog(LogData data)
        {
            logTrace?.ShowTrace(data);
            logTrace?.Rebuilder();
        }


        private void OnDestroy()
        {
            gesture  =  null;
            instance =  null;

            Application.logMessageReceived -= Recv;
        }
    }
}
