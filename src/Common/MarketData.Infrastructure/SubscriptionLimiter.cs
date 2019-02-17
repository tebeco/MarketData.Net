using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace MarketData.Infrastructure
{
    public class SubscriptionLimiter
    {
        public static IObservable<T> LimitSubscriptions<T>(int maxNumber, IObservable<T> source)
        {
            int subscriptionsCount = 0;
            var observable = Observable.Create<T>(observer =>
            {
                if (subscriptionsCount >= maxNumber)
                {
                    var subject = new Subject<T>();
                    subject.Subscribe(observer);
                    subject.OnError(new Exception($"Max concurrent subscription reached. Max is : {maxNumber}"));
                    return subject;
                }
                else
                {
                    Interlocked.Increment(ref subscriptionsCount);
                    var x = source.Subscribe(observer);
                    var spy = new SubscriptionsSpy(x, () => Interlocked.Decrement(ref subscriptionsCount));
                    return spy;
                }
            });

            return observable;
        }

        public class SubscriptionsSpy : IDisposable
        {
            private readonly IDisposable _disposable;
            private readonly Action _onDispose;

            public SubscriptionsSpy(IDisposable disposable, Action onDispose)
            {
                _disposable = disposable;
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                _onDispose();
                _disposable.Dispose();
            }
        }

    }
}