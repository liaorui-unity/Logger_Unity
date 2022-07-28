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
    public class Only<T> : UpdateBase<T>
    {
        public override void Init(IPrompt<T> iPrompt)
        {
            base.Init(iPrompt);

            OnToFilter(iPrompt).Push(OnNext);
            cancal = iPrompt as IDisposable;
        }

        public override void OnNext(T value)
        {
            base.OnNext(value);
            OnNextStop();
        }

        public override void Release()
        {
            base.Release();
            ClassPool.Release(this);
        }
    }

    public static partial class PtExtend
    {
        public static IPrompt<T> PtOnly<T>(this IPrompt<T> target)
        {
            var prompt = ClassPool.Get<Only<T>>();
            prompt.Init(target);
            return prompt;
        }
    }
}
