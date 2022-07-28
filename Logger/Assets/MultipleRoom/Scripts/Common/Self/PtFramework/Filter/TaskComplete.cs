//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Sailfish
{

    public class ToTask<T,U> : UpdateBase<T>
    {
        internal System.Func<T,U> resulf;

        public override void Init(IPrompt<T> iPrompt) 
        {
            base.Init(iPrompt);
            OnToFilter(iPrompt).Push(OnNext);
        }

        public override void OnNext(T value)
        {
            base.OnNext(value);
            resulf?.Invoke(value);
        }

        public override void Release()
        {
            resulf = null;
            base.Release();
            ClassPool.Release(this);
        }
    }

    public class ToTask<T> : UpdateBase<T>
    {

       internal System.Action<T> resulf;

        public override void Init(IPrompt<T> iPrompt)
        {
            base.Init(iPrompt);
            OnToFilter(iPrompt).Push(OnNext);
        }

        public override void OnNext(T value)
        {
            base.OnNext(value);
            resulf?.Invoke(value);
        }

        public override void Release()
        {
            resulf = null;
            base.Release();
            ClassPool.Release(this);
        }
    }
    public static partial class PtExtend
    {
        public static ToTask<T> TaskComplete<T>(this IPrompt<T> target, System.Action<T> action)
        {
            var prompt = ClassPool.Get<ToTask<T>>();
            prompt.Init(target);
            prompt.resulf = action;
            return prompt;
        }

        public static ToTask<T,U> TaskComplete<T,U>(this IPrompt<T> target, System.Func<T,U> action)
        {
            var prompt = ClassPool.Get<ToTask<T,U>>();
            prompt.Init(target);
            prompt.resulf = action;
            return prompt;
        }
    }
}
