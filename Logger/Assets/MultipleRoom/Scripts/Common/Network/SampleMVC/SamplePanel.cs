//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：#CreateTime#
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace fs
{

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class Sample : Attribute
    {
        public System.Type mScript;
        public string path;
    }


	public abstract class SamplePanel : MonoBehaviour 
    {
        public SamplePanel()
        {
            UISampleMgr.InitPanelFunc += (_) =>
            {
                if (this)
                {
                    uIMgr = _;
                    UISampleMgr.AddPanel(this.GetType(), this);
                }
            };
        }

        internal UISampleMgr uIMgr;

        public T GetView<T>() where T: SampleView
        {
            if (m_View == null)
            {
                m_View = new SampleView(this.transform) as T;
            }
            return m_View as T;
        }
        SampleView m_View;




        #region UI Panel和Panel之间的事件传输
        public void AddUIMsg(string msgKey, System.Action<UIMsg> msgCall)
        {
            this.AddTypeMsg(msgKey, msgCall);
        }

        public void RemoveUIMsg(string msgKey)
        {
            this.RemoveTypeMsg(msgKey);
        }

        public void RemoveAllMsg()
        {
            this.RemoveAllTypeMsg();
        }


        public void SendUIMsg(string msgKey, UIMsg uIMsg)
        {
            this.TriggerUIMsg(msgKey, uIMsg);
        }
        #endregion


        #region UI Panel与外部的事件传输
        public void InitMethod()
        {
            this.AddMethodFuncs();
        }

        public void RemoveMethod()
        {
            if (MethodCall.GetInfoCount(this.GetHashCode()) > 0)
            {
                this.RemoveMethodFuncs();
            }
        } 
        #endregion


        void Start()
        {    //--Panel自带配置--
            InitMethod();
            //--Panel自带配置--

            OnStart();
        }

        private void OnDisable()
        {
            UnRegister();
        }

        private void OnEnable()
        {
            Register();
        }

        private void OnDestroy()
        {
            //--移除Panel自带配置--
            RemoveAllMsg();
            RemoveMethod();
            //--移除Panel自带配置--

            OnDestroyed();
        }


        public virtual void OnStart() { }
        public virtual void OnDestroyed() { }
        public abstract void Setup(params object[] vs);
        public abstract void Register();
        public abstract void UnRegister();
    }
}
