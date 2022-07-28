//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：2021-06-23 16:40:27
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{

    #region 特性事件委托

    //-----------------特性事件委托---------------start

    /// <summary>
    /// UI与外部事件特性
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class FuncMethod : System.Attribute {}




    /// <summary>
    /// 触发事件委托函数
    /// </summary>
    public class MethodCall
    {
        public static Dictionary<string, object> keyInfos = new Dictionary<string, object>();
        public static Dictionary<object, EventInfo> eventInfos = new Dictionary<object, EventInfo>();


        public static int GetInfoCount(int hode)
        {
            EventInfo info = null;
            if (eventInfos.TryGetValue(hode, out info))
            {
                return info.InfoCounts;
            }
            return 0;
        }

        public static void TriggerCall(string key, params object[] values)
        {
            if (keyInfos.ContainsKey(key))
            {
                eventInfos[keyInfos[key]].InvokeMethod(key, values);
            }
        }

        internal static object TriggerFunc(string key, params object[] values)
        {
            if (keyInfos.ContainsKey(key))
            {
                return eventInfos[keyInfos[key]].InvokeMethod(key, values);
            }
            return null;
        }
    }



    /// <summary>
    /// 事件容器
    /// </summary>
    public class EventInfo
    {
        public object instance;
        public Dictionary<string, MethodInfo> methodInfos = new Dictionary<string, MethodInfo>();


        public int InfoCounts
        {
            get => methodInfos.Count;
        }

        public object InvokeMethod(string key, params object[] values)
        {
            return methodInfos[key].Invoke(instance, values);
        }

        public void RemoveMethod(string key)
        {
            if (methodInfos.ContainsKey(key)) methodInfos.Remove(key);
            if (MethodCall.keyInfos.ContainsKey(key)) MethodCall.keyInfos.Remove(key);
        }

        public void RemoveAllMethod()
        {
            foreach (var item in methodInfos.Keys)
            {
                MethodCall.keyInfos.Remove(item);
            }
            methodInfos.Clear();
        }
    }



    /// <summary>
    /// 特性数据处理
    /// </summary>
    public static class ExtendAttributes
    {
        public static void AddMethodFuncs(this object target)
        {
            var fields = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            EventInfo tempEvent = new EventInfo();

            for (int i = 0; i < fields.Length; i++)
            {
                FuncMethod[] attrs = fields[i].GetCustomAttributes(typeof(FuncMethod), false) as FuncMethod[];
                if (attrs != null && attrs.Length > 0)
                {
                    MethodInfo info = fields[i];
                    try
                    {
                        tempEvent.instance = target;
                        tempEvent.methodInfos.Add(info.Name, info);
                        MethodCall.keyInfos.Add(info.Name, target);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Key值：{info.Name}____{e.Message}");
                    }
                }
            }

            if (tempEvent.instance != null)
                MethodCall.eventInfos.Add(target, tempEvent);
        }

        public static void RemoveMethodFuncs(this object target)
        {
            if (MethodCall.eventInfos.ContainsKey(target))
            {
                MethodCall.eventInfos[target].RemoveAllMethod();
                MethodCall.eventInfos.Remove(target);
            }
        }

        public static void RemoveMethodFuncs(this object target, string key)
        {
            if (MethodCall.eventInfos.ContainsKey(target))
            {
                MethodCall.eventInfos[target].RemoveMethod(key);
                MethodCall.keyInfos.Remove(key);
            }
        }
    }

    //-----------------特性事件委托---------------end
    #endregion
}

