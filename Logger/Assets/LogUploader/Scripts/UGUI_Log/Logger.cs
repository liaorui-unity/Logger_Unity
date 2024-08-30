/// <summary>
/// 实现日志上传到服务器的功能
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Sailfish;
using Sailfish.Log;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LogInfo;

namespace Sailfish.Log
{
    public struct LogData
    {
        public LogType type;
        public string  log;
        public string  stackTrace;
        public string  timer;
    }

    public enum LogMode
    { 
        UGUI,
        UIToolKit
    }

    public class Logger : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitLogger()
        {
            if (instance == null)
            {
                instance = new GameObject("Logger").AddComponent<Logger>();
                Application.logMessageReceivedThreaded += Recv;
                CreatMode(LogMode.UIToolKit);
            }
        }

        internal static Logger instance;
        public static UnityAction<bool> TriggerCall;
        private Transform     panel;
        private MouseGesture  gesture;

        private ILogUI logUI;

        private LogMode logMode;

        public static void CreatMode(LogMode mode)
        {
            if (mode == LogMode.UGUI)
            {
                var go = (GameObject)Instantiate(Resources.Load("UGUILogger"));
                instance.panel = go.transform.Find("MainPanel");
                instance.logUI = go.transform.GetComponent<ILogUI>();
                instance.logUI?.Init(instance.panel);
            }
            else if (mode == LogMode.UIToolKit)
            {
                var go = (GameObject)Instantiate(Resources.Load("UIToolKitLogger"));
                instance.logUI = go.transform.GetComponent<ILogUI>();
                instance.logUI?.Init(null);
            }
        }

     
        void Awake()
        {
            instance = this;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Info.Init(System.Environment.CurrentDirectory);
#else
            Info.Init(Application.persistentDataPath);
#endif
 
        }


        void Start()
        {
            gesture = new MouseGesture()
            {
                Call = Set
            };
        }

        public void Set(bool isOpen)
        {
            logUI?.Set(isOpen);
        }


        static void Recv(string condition, string stackTrace, LogType type)
        {
            LogData data;
            data.log        = condition;
            data.type       = type;
            data.stackTrace = stackTrace;
            data.timer      = $"[<color=#51F7F8>{DateTime.Now.ToString("HH:mm:ss:ffff")}</color>]";
            instance.logUI?.AddLog(data);
        }


        private void Update()
        {
            if (gesture != null) gesture.Check();
            if(Input.GetKeyDown(KeyCode.F1))
            {
                Info.Log("Time:"+Time.time);
            }
        }


        public void SelectLog(LogData data)
        {
            logUI?.SelectLog(data);
        }


        private void OnDestroy()
        {
            gesture  =  null;
            instance =  null;

            Application.logMessageReceivedThreaded -= Recv;
        }
    }

    public static class LogExtend
    {
        public static string ToContent(this LogData data)
        {
            return $"{data.timer} {data.log}";
        }
        public static string ToDataTrace(this LogData data)
        {
            return $"{data.timer} {data.log} \r\n {data.stackTrace}";
        }
    }
}
