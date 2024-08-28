//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using Sailfish.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Sailfish
{
	[ExecuteInEditMode]
	public class LogBubble : MonoBehaviour
	{

		[Header("Log文本组件")]
		public int count;
		public Text logTxt;
        public Text timeTxt;
        public Text multipleTxt;

		public Button selectBtn;

		public LayoutElement m_Layout;
		public RectTransform m_CountRect;
        public RectTransform m_TimerRect;

		public Transform multiple;

        

        private RectTransform parentRect;

        float width = 0;
        int heigth = 0;
        LogData data;


        public void Awake()
        {
            parentRect = this.transform.parent.parent.GetComponent<RectTransform>();
            selectBtn.onClick.AddListener(select);
        }


        public void Init(LogData log)
		{
			multiple.gameObject.SetActive(false);
			logTxt  . text = log . log;
            timeTxt . text = log . timer;
            count   = 0;
            data    = log;

            switch (log.type)
            { 
				case LogType . Log      : logTxt . color = Color . white; break;
				case LogType . Warning  : logTxt . color = Color . yellow; break;
				case LogType . Error    : logTxt . color = Color . red; break;
				case LogType . Exception: logTxt . color = Color . red; break;
			}

        }


        public void select()
        {
           Sailfish.Log.Logger.instance.SelectLog(data);
        }


        public void ShowMultiple(int number = 1)
		{
			count += number;
			multiple.gameObject.SetActive(count > 1);
			multipleTxt.text = string.Format("x{0}", count);
		}


        private void Update()
        {
            var layaout = Screen.width - m_TimerRect.sizeDelta.x - m_CountRect.sizeDelta.x - 30;

            if (width != layaout)
            {
                width = layaout;
                m_Layout.preferredWidth = (int)layaout;
            }
        }

        public void Rebuilder()
		{
			if (count <= 0)
			{
				ShowMultiple();
			}
		}
	}
}
