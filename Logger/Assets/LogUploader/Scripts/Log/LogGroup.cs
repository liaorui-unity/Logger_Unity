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
	public class LogGroup : MonoBehaviour 
	{
        public GameObject bubblePrefab;

        private ScrollRect      scrollRect;
        private RectTransform   content;

     
        private string lastStr;

        private       LogBubble  lastBubble;
        private Queue<LogBubble> bubbles = new Queue<LogBubble>();


        int rebuilderWait = 2;

        public void Init()
        {
            scrollRect = GetComponentInChildren<ScrollRect>();
            content    = transform.Find("Viewport/Content").GetComponent<RectTransform>();
        }

        public void AddBubble(LogData content)
        {
            if (lastStr == content.log)
            {
                lastBubble.ShowMultiple();
            }
            else
            { 
                lastStr = content.log;

                GameObject newBubble = GetBubble();
            
                //设置气泡内容
                var bubble = newBubble . GetComponent<LogBubble>();
                bubble     . Init(content);
                bubble     . Rebuilder();
                bubbles    . Enqueue(bubble);
                lastBubble = bubble;

                newBubble.transform.localPosition = new Vector3(-20000, 10000, 0);
                newBubble.SetActive(true);

                scrollRect.verticalNormalizedPosition = 0;//使滑动条滚轮在最下方
                scrollRect.SetLayoutVertical();
                scrollRect.SetLayoutHorizontal();
               // Rebuiler();
            }
        }

        GameObject GetBubble()
        {
            if (bubbles.Count < 500)
            {
                GameObject newBubble = Instantiate(bubblePrefab, this.content);
                return newBubble;
            }

               var temp = bubbles.Dequeue();
                   temp.transform.SetAsLastSibling();
            return temp.gameObject;
        }


        public void Rebuiler()
        {
            rebuilderWait = 2;
        }

        private void LateUpdate()
        {
            if (rebuilderWait == 0) return;
                rebuilderWait -= 1;
            if (rebuilderWait == 0)
            {
                scrollRect.verticalNormalizedPosition = 0;//使滑动条滚轮在最下方
                scrollRect.SetLayoutVertical();
                scrollRect.SetLayoutHorizontal();
            }
        }
    }
}
