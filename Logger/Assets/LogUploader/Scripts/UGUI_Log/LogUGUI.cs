using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sailfish.Log
{

    public class LogUGUI : MonoBehaviour,ILogUI
    {
        private LogGroup logGroup;
        private LogTrace logTrace;
        private LoggerPanel logPanel;
        public void AddLog(LogData data)
        {
            logGroup.AddBubble(data);
        }

        public void Init(Transform panel)
        {
            logTrace = transform.GetComponentInChildren<LogTrace>(true);
            logGroup = transform.GetComponentInChildren<LogGroup>(true);
            logPanel = new LoggerPanel(panel);
            logGroup.Init();
        }

        public void SelectLog(LogData data)
        {
            logTrace?.ShowTrace(data);
        }

        public void Set(bool isOpen)
        {
            logPanel.SetPanel(isOpen);
        }
    }
}