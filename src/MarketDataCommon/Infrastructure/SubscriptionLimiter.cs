using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading;

namespace MarketDataCommon.Infrastructure
{
    public class SubscriptionLimiter
    {
        private int subscriptionsCount = 0;

        public static IObservable<T> LimitSubscriptions<T>(int maxNumber, IObservable<T> source, IScheduler scheduler)
        {
            var observable = Observable.Create<T>(observer =>
            {
                //TODO: Add counter to limit max subscriptions
                //if( count > maxNumber){
                //observer.OnError(new Exception($"Max concurrent subscription reached. Max is : {maxNumber}"));
                //}else{
                return source.Subscribe(observer);
                //}
            });

            return observable;
        }

    }
}