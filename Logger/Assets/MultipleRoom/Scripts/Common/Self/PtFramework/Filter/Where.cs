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
    public class Where<T> : UpdateBase<bool>
    {
        public void Init(IPrompt<T> iPrompt, Func<T, bool> func)
        {
            base.Init(iPrompt as IDisposable);
            ((IFilter<T>)iPrompt).Push(_ => OnNext((bool)func?.Invoke(_)));
        }



        public override void OnNext(bool value)
        {
            if (value)
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
        public static IPrompt<bool> PtWhere<T>(this IPrompt<T> target, Func<T, bool> func)
        {
            var prompt = ClassPool.Get<Where<T>>();
            prompt.Init(target, func);
            return prompt;
        }
    }

}
