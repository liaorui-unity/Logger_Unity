using System;
using System.Collections;
using System.Collections.Generic;
using LogInfo;
using Sailfish.Log;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sailfish.Log
{
    public class LogUIToolkit : MonoBehaviour,ILogUI
    {
        private Label fpsTxt;
        private Label titleTxt;
        private Label traceTxt;
        private TextField urlTxt;


        private Button uploadBtn;
        private Button uploadType;
        private Button closeBtn;
        private Button selectGo;

        private VisualElement traceElement;

        private VisualElement root;

        Color startColor = new Color(0.7058824f, 0.7058824f, 0.7058824f);

        private ScrollView contentScroll;


        public void AddLog(LogData data)
        {
            // 创建一个 VisualElement，用于显示日志信息
            var logElement = new Button();

            logElement.AddToClassList("item");
            logElement.text = data.ToContent();
            logElement.clicked += () =>
            {
                Click(logElement);
                SelectLog(data);
            };

            // 将日志信息添加到内容容器中
            contentScroll.Add(logElement);
            // 滚动到最底部
            contentScroll.scrollOffset = Vector2.zero;
        }


        public void Click(Button logElement)
        {
            if (selectGo != null)
            {
                selectGo.style.backgroundColor = startColor;
            }
            logElement.style.backgroundColor = Color.white;
            selectGo = logElement;
        }

        public void Show(LogData logItem)
        {
            if (traceElement.style.display == DisplayStyle.None)
            {
                traceElement.style.display = DisplayStyle.Flex;
                traceTxt.text = logItem.ToDataTrace();
            }
            else
            {
                traceTxt.text = logItem.ToDataTrace();
            }
        }


        private FPSDisplay display;
        private LogUploader uploader;


        public void Init(Transform panel)
        {
            uploader = new LogUploader();
            // 获取 UI 文档的根 VisualElement
            root = panel.GetComponent<UIDocument>().rootVisualElement;

            titleTxt = root.Q<Label>("IP");
            titleTxt.text = $"{Application.productName}_{LogInfo.Info.debugIp}";

            contentScroll = root.Q<ScrollView>("Scroll");
            contentScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            traceElement = root.Q<VisualElement>("Trace");

            fpsTxt     = root.Q<Label>("FPSValue");
            urlTxt     = root.Q<TextField>("Url");
            traceTxt   = root.Q<Label>("tValue");
            uploadBtn  = root.Q<Button>("UploadButton");
            uploadType = root.Q<Button>("UploadType");
            closeBtn   = root.Q<Button>("Close");

            urlTxt.value = Info.uploadLAN;
            urlTxt.RegisterValueChangedCallback((evt) =>
            {
                if (Info.debugTypeLAN)
                {
                    Info.SetUploadLAN(evt.newValue, Info.uploadPort);
                }
                else
                {
                    Info.SetUploadNET(evt.newValue, Info.uploadPort);
                }
            });

            uploadBtn.clicked += uploader.Upload;

            uploadType.clicked += () =>
            {
                SwitchType();
            };

            closeBtn.clicked += SetTrace;
            traceElement.style.display = DisplayStyle.None;


            if (display == null)
            {
                display = gameObject.AddComponent<FPSDisplay>();
                display.OnUpdateFPS += (fps) =>
                {
                    fpsTxt.text = fps;
                    fpsTxt.style.color = display.showColor;
                };
            }
        }



        private void SwitchType()
        {
            if (Info.debugTypeLAN)
            {
                Info.debugTypeLAN = false;
                uploadType.text = "互联网";
                urlTxt.value = Info.uploadNET;
            }
            else
            {
                Info.debugTypeLAN = true;
                uploadType.text = "局域网";
                urlTxt.value = Info.uploadLAN;
            }
        }

        public void SetTrace()
        {
            isRuning = !isRuning;

            if (isRuning)
            {
                traceElement.style.display = DisplayStyle.None;
                root.style.display = DisplayStyle.Flex;
            }
            else
            {
                root.style.display = DisplayStyle.None;
            }
        }

       
        public void SelectLog(LogData data)
        {
            Show(data);
        }

        public void Set(bool isOpen)
        {
            this.gameObject.SetActive(isOpen);
        }

        private bool isRuning = false;

    }
}
