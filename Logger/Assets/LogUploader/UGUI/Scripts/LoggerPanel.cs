//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LogInfo;

namespace Sailfish.Log
{
	public class LoggerPanel
	{
        /// <summary>
        /// 上传日志按钮
        /// </summary>
        public Button uploadBtn;
        /// <summary>
        /// 关闭日志按钮
        /// </summary>
        public Button closeBtn;
        /// <summary>
        /// 日志文本Text
        /// </summary>
        public Text logTxt;

        /// <summary>
        /// 本项目名称
        /// </summary>
        public Text proTxt;

        /// <summary>
        /// 本机IP地址
        /// </summary>
        public Text ipTxt;

        /// <summary>
        /// 显示FPS
        /// </summary>
        public Text fpsTxt;


        public LogUploader uploader;

        public Transform panel;

        public Transform ipSetting;

        private SetIP   setIP;

        private FPSDisplay display;

        public LoggerPanel(Transform target )
		{
            panel = target;

            ipSetting    =   target.Find("bottom/type");
            uploadBtn    =   target.Find("bottom/upload").GetComponent<Button>();
            closeBtn     =   target.Find("top/closeButton"). GetComponent<Button>();

            fpsTxt   =  target.Find("top/Fps/value").GetComponent<Text>();
            ipTxt    =   target.Find("top/account/Ip").GetComponent<Text>(); 
            proTxt   =   target.Find("top/account/Project").GetComponent<Text>();

            ipTxt. text  = Info.debugIp;
            proTxt.text  = Info.debugName;

            setIP   = new SetIP(ipSetting.transform);

            uploader = new LogUploader();
            display = target.gameObject.AddComponent<FPSDisplay>();
            display.OnUpdateFPS += (fps) =>
            {
                if (fpsTxt != null)
                {
                    fpsTxt.color = display.showColor;
                    fpsTxt.text  = fps;
                }
            };

            uploadBtn.onClick.AddListener(() => { uploader.Upload();  });
            closeBtn .onClick.AddListener(() => { SetPanel(false, true); });
        }

        void SetPanel(bool isOpen, bool isQuit = false)
        {
            if (isQuit)
            {
                panel.gameObject.SetActive(isOpen);
                return;
            }
            panel.gameObject.SetActive(!panel.gameObject.activeSelf);
        }


        public void SetPanel(bool isOpen)
        {
            SetPanel(isOpen, false);
        }
    }

    class SetIP
    {
        Dropdown dropType;

        public InputField value;

        public SetIP(Transform target)
        {
            var temp = target.Find("ipField");
            dropType = target.Find("Select").GetComponent<Dropdown>();

            value = temp.GetComponent<InputField>();
            dropType.onValueChanged.AddListener(SwitchType);

            value.text = Info.debugTypeLAN ? Info.uploadLAN : Info.uploadNET;
            value.onEndEdit.AddListener(ExitSelect);
        }

        public void SwitchType(int value)
        {
            Info.debugTypeLAN = value == 0;
            var str = Info.debugTypeLAN ? Info.uploadLAN : Info.uploadNET;
            ExitSelect(str);
        }

        public void ExitSelect(string ip)
        {


            if (Info.debugTypeLAN)
            {
                Info.SetUploadLAN(ip, Info.uploadPort);
            }
            else
            { 
                Info.SetUploadNET(ip, Info.uploadPort);
            }
            value.text =  ip;
        }

        public void Switch(bool isValue)
        {
            value.  text = isValue ? Info.uploadLAN : Info.uploadNET;
        }

    
    }
}
