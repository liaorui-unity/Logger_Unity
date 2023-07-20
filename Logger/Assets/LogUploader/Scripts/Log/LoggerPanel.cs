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

namespace Sailfish.Log
{
	public class LoggerPanel
	{
        /// <summary>
        /// 打印日志按钮
        /// </summary>
        public Toggle logBtnType;
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


        public LogUploader uploader;

        public Transform panel;

        private SetIP   setIP;
       
        public LoggerPanel(Transform target )
		{
            panel = target;

            logBtnType   =   target.Find("bottom/typeToggle").  GetComponent<Toggle>();
            uploadBtn    =   target.Find("bottom/uploadButton").GetComponent<Button>();
            closeBtn     =   target.Find("top/closeButton"). GetComponent<Button>();

            ipTxt    =   target.Find("top/account/Ip").GetComponent<Text>(); 
            proTxt   =   target.Find("top/account/Project").GetComponent<Text>();

            ipTxt. text  =  Debug.debugIp;
            proTxt.text  =  Debug.debugName;

            setIP   = new SetIP(logBtnType.transform);

            uploader = new LogUploader();

            uploadBtn.onClick.AddListener(() => { uploader.Upload();  });
            closeBtn .onClick.AddListener(() => { SetPanel(false, true); });

            logBtnType.  onValueChanged.AddListener(SwitchType);
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


        private void SwitchType(bool isValue)
        {
            setIP.ExitSelect(setIP.value.text);
            Debug.debugTypeLAN = isValue;
            setIP.Switch(isValue);
        }
    }

    class SetIP
    {
        Text  typeTxt;
        Image input;

        public InputField value;

        public SetIP(Transform target)
        {
            var temp = target.Find("ipField");
            typeTxt  = target.Find("Label").GetComponent<Text>();

            input = temp.GetComponent<Image>();
            value = temp.GetComponent<InputField>();
     

            value.text = Debug.debugTypeLAN ? Debug.uploadLAN : Debug.uploadNET;
            value.onEndEdit.AddListener(ExitSelect);

            UnityAction<BaseEventData> selectEvent = InputFieldClick;
            var eventTrigger = value.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerClick,
            };
            entry.callback.AddListener(selectEvent);
            eventTrigger.triggers.Add(entry);

            Set(false);
        }

        void InputFieldClick(BaseEventData data)
        {
           Set(true);
        }

        public void ExitSelect(string ip)
        {
            Set(false);
            value.text = Debug.debugTypeLAN ? Debug.uploadLAN = ip : Debug.uploadNET = ip;
        }

        public void Switch(bool isValue)
        {
            typeTxt.text = isValue ? "局域网" : "互联网";
            value.  text = isValue ? Debug.uploadLAN : Debug.uploadNET;
        }

        public void Set(bool isOn)
        {
            input.enabled = isOn;
        }
    }
}
