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

namespace Sailfish.Log
{
	public class LogTrace : MonoBehaviour 
	{
		public ScrollRect scrollRect;
        public Text      traceTxt;
        int count = 1;
        public void ShowTrace(LogData data)
        {
            traceTxt.text = data.ToDataTrace();
            traceTxt.gameObject.SetActive(true);
            count = 2;
        }


        void Update()
        {
            if (count > 0)
            {
                count -= 1;
                if (count == 1)
                {
                    traceTxt.gameObject.SetActive(false);
                }
                if (count == 0)
                {
                    traceTxt.gameObject.SetActive(true);
                    Rebuilder();
                }
            }
        }

        public void Rebuilder()
        {
            scrollRect.verticalNormalizedPosition = 0;//使滑动条滚轮在最下方
            scrollRect.SetLayoutVertical();
            scrollRect.SetLayoutHorizontal();//使滑动条滚轮在最下方
        }
    }
}
