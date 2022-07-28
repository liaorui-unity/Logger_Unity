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
    public class UISampleMgr : MonoBehaviour
    {
        static UISampleMgr instance;

        private void Awake()
        {
            instance = this;
            InitPanelFunc?.Invoke(this);
        }


        #region 特性数据

        internal static Dictionary<System.Type, Sample> samples = new Dictionary<System.Type, Sample>();

        public void AddLoaderInfo(Sample info)
        {
            if (!samples.ContainsKey(info.mScript))
            {
                samples.Add(info.mScript, info);
            }
        }
        public Sample GetLoaderInfo(System.Type type)
        {
            Sample info = null;
            if (!samples.TryGetValue(type, out info))
            {//可能没有提取特性
                this.ExtractAttribute(type.Assembly);
                if (!samples.TryGetValue(type, out info))
                {
                    return null;
                }
            }
            return info;
        }
        public void RemoveAllLoaderInfo()
        {
            samples.Clear();
        }

        /// <summary>
        /// 提取特性
        /// </summary>
        private void ExtractAttribute(System.Reflection.Assembly assembly)
        {
            float start_time = Time.realtimeSinceStartup;
            //外部程序集
            List<System.Type> types = AttributeUtils.FindType<SamplePanel>(assembly, true);


            if (types != null)
            {
                foreach (System.Type type in types)
                {
                    Sample ui_attr = AttributeUtils.GetClassAttribute<Sample>(type);
                    if (ui_attr == null) continue;
                    ui_attr.mScript = type;
                    this.AddLoaderInfo(ui_attr);
                }
            }
            Debuger.Log("UIManager:ExtractAttribute 提取特性用时:" + (Time.realtimeSinceStartup - start_time));
        }

        #endregion



        internal static Dictionary<System.Type, SamplePanel> panels = new Dictionary<System.Type, SamplePanel>();

        internal static UnityAction<UISampleMgr> InitPanelFunc;


        public static void AddPanel(System.Type type, SamplePanel sample)
        {
            panels[type] = sample;          
        }

        public static void Show<T>(params object[] vs) where T : SamplePanel
        {
            if (!panels.ContainsKey(typeof(T)))
            {
                var sample = instance.GetLoaderInfo(typeof(T));

                var samplePanel = Instantiate(Resources.Load(sample.path)) as GameObject;
                samplePanel.transform.SetParent(instance.transform);
                samplePanel.transform.localScale = Vector3.one;
                samplePanel.transform.localEulerAngles = Vector3.zero;
                samplePanel.transform.localPosition = Vector3.zero;


                var spl = samplePanel.GetComponent<T>();
                if (spl == null)
                    spl = samplePanel.AddComponent<T>();

                panels.Add(typeof(T), spl);

                spl.Setup(vs);
                spl.gameObject.SetActive(true);
            }
            else
            {
                panels[typeof(T)].Setup(vs);
                panels[typeof(T)].gameObject.SetActive(true);
            }
        }


        public static void Hide<T>()
        {
            panels[typeof(T)].gameObject.SetActive(false);
        }


        private void OnDestroy()
        {
            panels.Clear();
            samples.Clear();
      
            InitPanelFunc = null;
        }
    }
}
