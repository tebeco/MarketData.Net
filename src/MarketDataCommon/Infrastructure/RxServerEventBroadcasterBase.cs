using System;
using System.Reactive.Concurrency;
using Microsoft.AspNetCore.Hosting;

namespace MarketDataCommon.Infrastructure
{
    public abstract class RxServerEventBroadcasterBase<T> : ICreateWebHost
    {
        private IObservable<T> _events;

        public RxServerEventBroadcasterBase(int port, bool flaky)
        {
            Port = port;
            Flaky = flaky;
        }

        public IWebHost CreateServer()
        {
            if (Flaky)
            {
                _events = SubscriptionLimiter.LimitSubscriptions(1, InitializeEventStream());
            }
            else
            {
                _events = InitializeEventStream();
            }
            return InternalCreateServer();
        }

        private IWebHost InternalCreateServer()
        {
            throw new NotImplementedException();
        }

        public IObservable<T> GetEvents(object httpRequest)
        {
            return _events;
        }

        protected abstract IObservable<T> InitializeEventStream();

        public int Port { get; }

        public bool Flaky { get; }
    }
}
