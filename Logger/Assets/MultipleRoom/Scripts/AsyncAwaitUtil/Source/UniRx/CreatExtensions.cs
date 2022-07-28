using Boo.Lang;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using UniRx.Operators;

public class CreatExtensions : MonoBehaviour
{

    public bool isOn;

    public Button mButton;
    private void Start()
    {
        //var createStream = Observable.EveryUpdate()
        //    .Where(_ => isOn)
        //    .ThrottleFirst(TimeSpan.FromSeconds(3f)).
        //     Subscribe((_) =>
        //    {
        //        Debug.Log("间隔5秒可以触发一次A");

        //    }, () =>
        //    {
        //        Debug.Log("间隔5秒可以触发一次A111");
        //    });


        //createStream.Dispose();

      



      var t=  mButton.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(3f))
            .GH()
            .Subscribe
            (_ =>
            {
                Debug.Log("On Button Clicked:" + Time.time);
            });

      

    }



    IEnumerator gt()
    {
        yield return "fg";
    }

    async Task<string> Wait()
    {

        var value = await gt();


        Debug.Log(value);

        await new WaitForSeconds(2.0f);
        Debug.Log("dengdai");
        return "dengdai";
    }

    // Update is called once per frame
    void Update()
    {

    }


 
}

public class OnlyEx<T> : OperatorObservableBase<T>
{
    readonly IObservable<T> source;
    readonly IObservable<T> second;

    public OnlyEx(IObservable<T> source)
        : base(source.IsRequiredSubscribeOnCurrentThread())
    {
        this.source = source;
    }

    protected override System.IDisposable SubscribeCore(IObserver<T> observer, System.IDisposable cancel)
    {
        //cancel.Dispose();
        //observer.OnCompleted();
        return cancel;
    }
}

public static class FF
{
    public static UniRx.IObservable<T> GH<T>(this UniRx.IObservable<T> observer)
    {
        Observable.Timer(System.TimeSpan.FromTicks(1)).Subscribe(_ =>
        {

        }, () =>
        {
            Debug.Log("OnCompleted");
        });


        return new OnlyEx<T>(observer);
    }
}


