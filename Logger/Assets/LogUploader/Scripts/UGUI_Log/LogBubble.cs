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
        public Text multipleTxt;
		public Button selectBtn;

        LogData data;

        public void Awake()
        {
            selectBtn.onClick.AddListener(select);
        }


        public string Show(LogData log)
        {
            return $"{log.timer} {log.log}";
        }

        public void Init(LogData log)
		{
            logTxt.text = Show(log);
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
            multipleTxt.text = count > 0 ? $"x{count}" : "";
        }

        public void Rebuilder()
		{
            ShowMultiple();
        }
	}
}
