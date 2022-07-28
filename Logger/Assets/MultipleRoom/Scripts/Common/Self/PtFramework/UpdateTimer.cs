//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using fs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sailfish
{
    public class UpdateTimer : UpdateBase<float>, IState
    {   /// <summary>
        /// 创建者
        /// </summary>
        public static UpdateTimer Creater
        {
            get
            {
                var update = ClassPool.Get<UpdateTimer>();
                update.Init();
                PtFramework.CreatDisposable(update.CodeID, update);
                return update;
            }
        }

        /// <summary>
        /// 回收者
        /// </summary>
        public static UpdateTimer Recycler
        {
            set
            {
                PtFramework.RemoveDisposable(value.CodeID);
                ClassPool.Release(value);
            }
        }




        bool isStop = false;
        float startTime = 0;
        void Update()
        {
            if (!isStop) call?.Invoke(Time.time - startTime);
        }

        public void Stop()
        {
            isStop = true;
            CallUnit.updateCall.RemoveListener(Update);
        }

        public void ReStart()
        {
            isStop = false;
            startTime = Time.time;
            CallUnit.updateCall.AddListener(Update);
        }


        public void CollectDisposable(IDisposable disposable)
        {
            disposables.Add(disposable);
        }


        public override int Code()
        {
            return this.GetHashCode();
        }


        public override void Release()
        {
            base.Release();

            Recycler = this;
            CallUnit.updateCall.RemoveListener(Update);

            foreach (var item in disposables)
            {
                item.Release();
            }
            disposables.Clear();
        }
    }
}
