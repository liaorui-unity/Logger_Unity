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
	public class Cycle<T> :UpdateBase<T> 
	{
		float startTime = 0;
		public void Init(IPrompt<T> iPrompt,float time)
		{
			base.Init(iPrompt as IDisposable);

			startTime = Time.time;
			OnToFilter(iPrompt).Push((_) =>
			{
				if (Time.time - startTime > time)
				{
					startTime = Time.time;
					OnNext(_);
				}
			});
		}

        public override void Release()
        {
            base.Release();
			ClassPool.Release(this);
        }
    }


	public static partial class PtExtend
	{
		public static IPrompt<T> PtCycle<T>(this IPrompt<T> target, float time)
		{
			var prompt = ClassPool.Get<Cycle<T>>();
			prompt.Init(target, time);
			return prompt;
		}
	}
}
