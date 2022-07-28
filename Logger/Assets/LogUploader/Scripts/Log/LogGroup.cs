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
	public class LogGroup : MonoBehaviour 
	{
        public GameObject bubblePrefab;

        private ScrollRect      scrollRect;
        private RectTransform   content;

        [SerializeField]
        private float stepVertical; //上下两个气泡的垂直间隔
        [SerializeField]
        private float stepHorizontal; //左右两个气泡的水平间隔

        private string lastStr;

        private       LogBubble  lastBubble;
        private Queue<LogBubble> bubbles = new Queue<LogBubble>();


        int rebuilderWait = 2;

        public void Init()
        {
            scrollRect = GetComponentInChildren<ScrollRect>();
            content    = transform.Find("Viewport/Content").GetComponent<RectTransform>();
        }

        public void AddBubble(string content)
        {
            if (lastStr == content)
            {
                lastBubble.ShowMultiple();
            }
            else
            { 
                lastStr = content;

                GameObject newBubble = GetBubble();
            
                //设置气泡内容
                var bubble = newBubble.GetComponent<LogBubble>();
                bubble.Init(content, this);
                bubbles.Enqueue(bubble);

                newBubble.transform.localPosition = new Vector3(-20000, 10000, 0);
                newBubble.SetActive(true);

                lastBubble = bubble;
                bubble.Rebuilder();

                scrollRect.verticalNormalizedPosition = 0;//使滑动条滚轮在最下方
            }
        }

        GameObject GetBubble()
        {
            if (bubbles.Count < 50)
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
            }
        }
    }
}
