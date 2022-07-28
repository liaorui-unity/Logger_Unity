//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sailfish
{
	public class LogBubble : MonoBehaviour
	{

		[Header("Log文本组件")]
		public int count;
		public Text logTxt;
		public LayoutElement layout;
		public RectTransform logTxtTransfrom;


		[Header("多次连续出现")]
		public Transform multiple;
		public   Text    multipleTxt;

		private LogGroup group;

		public void Init(string str, LogGroup group)
		{
			this.group = group;
			multiple.gameObject.SetActive(false);
			logTxt.text = str;
			count = 0;
		}



		public void ShowMultiple(int number = 1)
		{
			count += number;
			multiple.gameObject.SetActive(count > 1);
			multipleTxt.text = string.Format("x{0}", count);
		}


        private void LateUpdate()
        {
			if (logTxtTransfrom.sizeDelta.x > Screen.width)
			{
				layout.preferredWidth = Screen.width - 125;
				group.Rebuiler();
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
