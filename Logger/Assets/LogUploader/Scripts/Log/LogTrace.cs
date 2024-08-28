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
	public class LogTrace : MonoBehaviour 
	{
		public ScrollRect scrollRect;
        public LogData selectData;
        public Text traceTxt;

        public void ShowTrace(LogData data)
        {
            if (string.IsNullOrEmpty(selectData.stackTrace))
                traceTxt.text = data.stackTrace;
        }

        public void SetSelect(LogData select)
        {
            selectData    = select;
            traceTxt.text = select.stackTrace;
        }

        public void Rebuilder()
        {
            scrollRect.SetLayoutVertical();
            scrollRect.SetLayoutHorizontal();//使滑动条滚轮在最下方
        }
    }
}
