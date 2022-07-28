//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sailfish
{
	public class Select<T,U> :UpdateBase<U> 
	{
        public void Init(IPrompt<T> iPrompt, Func<T,U> func) 
        {
            base.Init(iPrompt as IDisposable);
            ((IFilter<T>)iPrompt).Push(_ => OnNext(func.Invoke(_)));
        }


        public override void OnNext(U value)
        {
            base.OnNext(value);
        }

        public override void Release()
        {
            base.Release();
            ClassPool.Release(this);
        }
    }

    public static partial class PtExtend
    {
        public static IPrompt<U> PtSelect<T, U>(this IPrompt<T> target, Func<T, U> func)
        {
            var prompt = ClassPool.Get<Select<T, U>>();
            prompt.Init(target, func);
            return prompt;
        }
    }
}
