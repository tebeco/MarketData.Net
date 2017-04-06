using System;
using System.Reactive.Linq;
using MarketDataCommon.Dto;
using MarketDataCommon.Infrastructure;

namespace MarketDataExternal.Providers
{
    public class ForexProvider : RxServerEventBroadcasterBase<Quote>
    {
        private readonly TimeSpan _interval = TimeSpan.FromMilliseconds(500);

        public ForexProvider(int port, bool flaky) : base(port, flaky)
        {
        }

        protected override IObservable<Quote> InitializeEventStream()
        {
            return new RandomSequenceGenerator(1.2, 1.3)
                .Create(_interval)
                .Select(q => new Quote("EUR/USD", q));
        }
    }
}
