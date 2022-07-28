//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：2021-06-23 16:40:27
//=======================================================
using fs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sailfish
{

    public static partial class Extend
    {
        public static TimeExtend UpdateCall<T>(this T target)
        {
            int id = target.GetHashCode();
            TimeExtend timeClass = ClassPool.Get<TimeExtend>();
            timeClass.Init(target.GetHashCode(), 0.001f);
            TimeMgr.AddTimer(id, timeClass);
            return timeClass;
        }

        public static TimeExtend UpdateUnilt<T>(this T target)
        {
            int id = target.GetHashCode();
            TimeExtend timeClass = ClassPool.Get<TimeExtend>();
            timeClass.Init(target.GetHashCode(), int.MaxValue);
            TimeMgr.AddTimer(id, timeClass);
            return timeClass;
        }

        public static TimeExtend UpdateTime<T>(this T target, float duration)
        {
            int id = target.GetHashCode();
            TimeExtend timeClass = ClassPool.Get<TimeExtend>();
            timeClass.Init(target.GetHashCode(), duration);
            TimeMgr.AddTimer(id, timeClass);
            return timeClass;
        }


        public static TimeExtend LateUpdateTime<T>(this T target, float duration)
        {
            int id = target.GetHashCode();
            TimeExtend timeClass = ClassPool.Get<TimeExtend>();
            timeClass.Init(target.GetHashCode(), duration, true);

            TimeMgr.AddTimer(id, timeClass);
            return timeClass;
        }


        public static TimeExtend UpdateUntil<T>(this T target, float duration)
        {
            int id = target.GetHashCode();
            TimeExtend timeClass = ClassPool.Get<TimeExtend>();
            timeClass.Init(target.GetHashCode(), duration, true);
            TimeMgr.AddTimer(id, timeClass);
            return timeClass;
        }


        public static void StopThisOfTime<T>(this T target)
        {
            TimeMgr.Stop(target.GetHashCode());
        }

        public static void Stop(this TimeExtend target)
        {
            target.Clear();
        }
    }





    public class TimeMgr 
    {
        public static Dictionary<int, List<TimeExtend>> timerValues = new Dictionary<int, List<TimeExtend>>(); //记录所有的 ID 对应 时间延时脚本的数据类 


        public static void AddTimer(int typeID, TimeExtend extend)
        {
            if (timerValues.Count == 0)
                CallUnit.destroyCall.RemoveListener(Clear);


            if (timerValues.ContainsKey(typeID))
            {
                timerValues[typeID].Add(extend);
            }
            else
            {
                var extends = new List<TimeExtend>();
                extends.Add(extend);
                timerValues.Add(typeID, extends);
            }
        }


        public static void ClearTimer(int id, TimeExtend extend)
        {
            if (timerValues.ContainsKey(id))
            {
                timerValues[id].Remove(extend);
                extend = null;
            }
        }


        public static void Stop(int id)
        {
            if (timerValues.ContainsKey(id))
            {
                var item = timerValues[id];

                for (int i = 0; i < item.Count; i++)
                {
                    item[i].Clear();
                    item[i] = null;
                }

                timerValues.Remove(id);
                item = null;

                if (timerValues.Count == 0)
                {
                    CallUnit.destroyCall.RemoveListener(Clear);
                }
            }
        }

        public static void Clear()
        {
            Debug.Log("清空Time数据");

            CallUnit.destroyCall.RemoveListener(Clear);
            timerValues.Clear();
        }
    }




    /// <summary>
    /// 时间延迟类
    /// </summary>
    public class TimeExtend
    {
        private System.Func<float, bool> actionProcess;
        private System.Action actionDone;


        int hashCode = 0;
        bool isLate = false;

        float time;
        float delayTime;


        public void Init(int id, float delayT, bool isLate=false)
        {
            time = 0;
            hashCode = id;
            delayTime = delayT;
            this.isLate = isLate;

            if (isLate) CallUnit.lateUpdateCall.AddListener(Update);
            else CallUnit.updateCall.AddListener(Update);

            CallUnit.destroyCall.AddListener(Clear);
        }

        public void Update()
        {
            try
            {
                if ((bool)actionProcess?.Invoke(Mathf.InverseLerp(0, delayTime, time)))
                {
                    time = delayTime;
                }

                if (time >= delayTime)
                {
                    time = 0;
                    actionDone?.Invoke();
                    Clear();
                }

                time += Time.deltaTime;
            }
            catch (System.Exception e)
            {
                Clear();
                Debug.Log(string.Format("Timer报错：{0}————清空事件", e.Message));
            }
        }


        public void Clear()
        {
            time = 0;
            actionDone = null;
            actionProcess = null;

            ClassPool.Release(this);
            TimeMgr.ClearTimer(hashCode, this);


            if (isLate) CallUnit.lateUpdateCall.RemoveListener(Update);
            else CallUnit.updateCall.RemoveListener(Update);

            CallUnit.destroyCall.RemoveListener(Clear);
        }



        #region 事件委托回调
        /// <summary>
        /// 时间延时完成后执行
        /// </summary>
        public TimeExtend OnComelete(System.Action action)
        {
            actionDone = action;
            return this;
        }

        /// <summary>
        /// 时间延时过程中执行
        /// </summary>
        public TimeExtend OnProcess(System.Action<float> action)
        {
            actionProcess = (time) =>
            {
                action?.Invoke(time);
                return false;
            };
            return this;
        }

        /// <summary>
        /// 时间延时过程中执行
        /// </summary>
        public TimeExtend OnUnity(System.Func<float, bool> action)
        {
            actionProcess = action;
            return this;
        }

        #endregion
    }
}
