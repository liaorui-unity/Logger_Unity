//=======================================================
// 作者：LR
// 公司：广州旗博士科技有限公司
// 描述：工具人
// 创建时间：#CreateTime#
//=======================================================
using fs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sailfish
{

    public interface IDisposable
    {
        int Code();
        void Release();
    }


    public interface IFilter<T>
    {
        void OnNext(T value);
        void Push(Action<T> action);
    }


    public interface IPrompt<T>
	{
        IDisposable OnComplete(Action<T> action);

        IPrompt<T> OnTaskComplete(Action<T> action);
    }


    public interface IState
    {
        void Stop();
        void ReStart();
        void CollectDisposable(IDisposable disposable);
    }


    public class Prompt<T> : IPrompt<T>, IFilter<T>
    {
        internal Action<T> call;
        internal IDisposable cancal;

        public virtual void Init(IPrompt<T> iPrompt)
        {
            cancal = iPrompt as IDisposable;
        }
        public virtual void Init(IDisposable disposable) 
        {
            cancal = disposable;
        }


        public virtual IPrompt<T> OnTaskComplete(Action<T> action)
        {
            call = action;
            return this;
        }

        public virtual IDisposable OnComplete(Action<T> action)
        {
            call = action;
            return cancal; 
        }

        public virtual void OnNext(T value)
        {
            call?.Invoke(value);
        }

        public virtual void Release()
        {
            call   = null;
            cancal = null;
        }

        public void Push(Action<T> action)
        {
            call = action;
        }
    }



    public class UpdateBase<T> : Prompt<T>, IDisposable
    {
        internal int CodeID;
        internal List<IDisposable> disposables;

        public override void Init(IPrompt<T> iPrompt) 
        {
            base.Init(iPrompt);
            PtFramework.CollectDisposable(cancal.Code(), this);
        }
        public override void Init(IDisposable disposable)
        {
            base.Init(disposable);
            PtFramework.CollectDisposable(cancal.Code(), this);
        }
        public virtual void Init()
        {
            CodeID = this.GetHashCode();
            disposables = new List<IDisposable>();
            PtFramework.CollectDisposable(CodeID, this);
        }


        public override IDisposable OnComplete(Action<T> action)
        {
            PtFramework.ReTakeDisposable(Code());
            return base.OnComplete(action);
        }



        public void OnNextDone()
        {
            PtFramework.ReleaseDisposable(Code());
        }


        public void OnNextStop()
        {
            PtFramework.StopTakeDisposable(Code());
        }


        public virtual int Code()
        {
            return CodeID = cancal.Code();
        }

        public IFilter<T> OnToFilter(IPrompt<T> prompt)
        {
            return prompt as IFilter<T>;
        }
    }


   


    public class PtFramework : MonoBehaviour
    {

        public static Dictionary<int, IState> updates = new Dictionary<int, IState>();

        public static void CollectDisposable(int code, IDisposable disposable)
        {
            IState state = null;
            if (updates.TryGetValue(code, out state))
            {
                state.CollectDisposable(disposable);
            }
        }

        public static void CreatDisposable(int code, IState state)
        {
            if (!updates.ContainsKey(code))
            {
                updates[code] = state;
            }
        }


        public static void ReTakeDisposable(int code)
        {
            IState state = null;

            if (updates.TryGetValue(code, out state))
            {
                state.ReStart();
            }
        }


        public static void StopTakeDisposable(int code)
        {
            IState state = null;
            if (updates.TryGetValue(code, out state))
            {
                state.Stop();
            }
        }


        public static void ReleaseDisposable(int code)
        {
            IState state = null;
            if (updates.TryGetValue(code, out state))
            {
                ((IDisposable)state).Release();
            }
        }

        public static void RemoveDisposable(int code)
        {
            if (updates.ContainsKey(code))
            {
                updates.Remove(code);
            }
        }


    }

    public static partial class PtExtend
    {
        public static TaskAwaiter<T> GetAwaiter<T>(this IPrompt<T> Prompt)
        {
            var task = new TaskCompletionSource<T>();

            Prompt.OnComplete(_=>
            {
                task.TrySetResult(_);
                task.TrySetCanceled();
            });

            return task.Task.GetAwaiter();
        }


        public static TaskAwaiter<T> GetAwaiter<T>(this ToTask<T> Prompt)
        {
            var task = new TaskCompletionSource<T>();

            Prompt.OnComplete((_)=> 
            {
                task.SetResult(_);
                task.TrySetCanceled();
            });

            return task.Task.GetAwaiter();
        }

        public static TaskAwaiter<U> GetAwaiter<T,U>(this ToTask<T,U> Prompt)
        {
            var task = new TaskCompletionSource<U>();

            Prompt.OnComplete((_) =>
            {
                var resulf = Prompt.resulf.Invoke(_);
                task.SetResult(resulf);
                task.TrySetCanceled();
            });

            return task.Task.GetAwaiter();
        }
    }
}
