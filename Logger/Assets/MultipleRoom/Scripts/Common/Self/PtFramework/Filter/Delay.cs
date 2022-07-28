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
    public class Delay : UpdateBase<float>
    {
        float runTime = 0;
        public void Init(IPrompt<float> iPrompt, float time) 
        {
            base.Init(iPrompt);

            runTime = time;
            OnToFilter(iPrompt).Push(OnNext);
        }


        public override void OnNext(float value)
        {
            if (value >= runTime)
            {
                base.OnNext(value);
                OnNextDone();
            }
        }

        public override void Release()
        {
            base.Release();
            ClassPool.Release(this);
        }
    }

    public static partial class PtExtend
    {
        public static IPrompt<float> PtDelayTimer(this IPrompt<float> target, float time=0.01f)
        {
            var prompt = ClassPool.Get<Delay>();
            prompt.Init(target, time);
            return prompt;
        }
    }
}
