//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
    [Sample(path = "Backpack")]
    public class UIPanelSample : SamplePanel
    {
       public bool isUnilt = false;
        public override void OnStart()
        {
            AddUIMsg("GUI", UIMsg_GUI);



        }



        public override void Register()
        {
          
        }

        public override void Setup(params object[] vs)
        {
      
        }

        public override void UnRegister()
        {
          
        }


        [FuncMethod]
        public int YUi()
        {
            Debug.Log("df");
            return 50;
        }



        public void UIMsg_GUI(UIMsg msg)
        {
            string arg = msg.m_Args[0] as string;
            Debug.Log(arg);
        }
    }
}
