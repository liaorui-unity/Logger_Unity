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
	public class AsIEnumerator<T>:UpdateBase<T>		
	{

		bool isCompleted = false;

		public override void Init(IPrompt<T> iPrompt) 
		{
			base.Init(iPrompt);
			iPrompt.OnComplete(OnNext);
		}

		public void Init(IPrompt<T> iPrompt,System.Action<T> action)
		{
			base.Init(iPrompt);
			iPrompt.OnComplete((_)=>
			{
				OnNext(_);
				action?.Invoke(_);
			});
		}

		public override void OnNext(T value)
        {
            base.OnNext(value);
			isCompleted = true;
        }

        public override void Release()
        {
			base.Release();
			ClassPool.Release(this);
        }


		public IEnumerator IETask()
		{
			while (!isCompleted)
			{
				yield return 0;
			}
			isCompleted = false;
		}
	}


	public static partial class PtExtend
	{
		public static IEnumerator PtAsIEnumerator<T>(this IPrompt<T> target)
		{
			var prompt = ClassPool.Get<AsIEnumerator<T>>();
			prompt.Init(target);
			yield return prompt.IETask();
		}

		public static IEnumerator PtAsIEnumerator<T>(this IPrompt<T> target,System.Action<T> action)
		{
			var prompt = ClassPool.Get<AsIEnumerator<T>>();
			prompt.Init(target, action);
			yield return prompt.IETask();
		}
	}
}
