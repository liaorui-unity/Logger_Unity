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
    public class Logger : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitLogger()
        {
            Application.logMessageReceived += Recv;
            if (instance == null)
            {
                var logger = Instantiate(Resources.Load("Logger")) as GameObject;
                logger.gameObject.name = "Logger";
                instance = logger.GetComponent<Logger>();

                instance.logGroup.Init();
                instance.logPanel = new LoggerPanel(instance.panel);
                TriggerCall += instance.logPanel.SetPanel;
            }
        }

        internal static Logger instance;
        internal static Queue<string> msgQues = new Queue<string>();


        public static UnityAction<bool> TriggerCall;


        public  Transform   panel;
        public  LogGroup logGroup;

        private MouseGesture  gesture;
        private LoggerPanel   logPanel;


        Thread threadwhat;

     

        void Awake()
        {
            ReadConfig();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Debug.Init(Application.streamingAssetsPath);
#else
            Debug.Init(Application.persistentDataPath);
#endif
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
            gesture = new MouseGesture();
            StartThread();
        }

        void StartThread()
        {
            threadwhat = new Thread(ThreadVerify);//创建一个接受信息的进程
            threadwhat.IsBackground = true;//后台启动
            threadwhat.Start();//有传入参数的线程
        }

        void ThreadVerify()
        {
            logPanel.uploader.VerifyCall = Verify;
            logPanel.uploader.Verify();
        }

        void Verify(bool isSuccess)
        {
            logPanel.uploader.VerifyCall = null;

            if (isSuccess)
            {
                Debug.Log("连接本地服务器成功！");
                gesture.Call = logPanel.SetPanel;
            }
            else
            {
                Debug.Log("连接本地服务器失败！");     
                gesture.Call = (value) =>
                {
                    gesture.Call = null;
                    StartThread();
                };
            }

            threadwhat.Abort();
        }



        static void Recv(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                    {
                        if (!Debug.s_debugLogEnable) return;
                    }
                    break;
                case LogType.Warning:
                    {
                        if (!Debug.s_warningLogEnable) return;
                    }
                    break;
                case LogType.Error:
                    {
                        if (!Debug.s_errorLogEnable) return;
                    }
                    break;
            }

            msgQues.Enqueue(string.Format("[<color=#FF0000>{0}</color>] {1}", DateTime.Now.ToString("yy/MM/dd HH:mm:ss:ffff"), condition));
        }



        private void Update()
        {
            if (gesture != null) gesture.Check();

            while (msgQues.Count > 0)
            {
                logGroup?.AddBubble(msgQues.Dequeue());
            }
        }



        private void OnDestroy()
        {
            gesture  =  null;
            instance =  null;

            threadwhat.Abort();
            Application.logMessageReceived -= Recv;
        }
    }
}
