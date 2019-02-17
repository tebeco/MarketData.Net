using MarketData.Dto;
using MarketData.Infrastructure;
using System;
using System.Reactive.Linq;

namespace MarketData.Backend.Forex
{
    public class ForexProvider
    {
        private readonly TimeSpan _interval = TimeSpan.FromMilliseconds(500);

        public IObservable<Quote> GetEventStream()
        {
            return new RandomSequenceGenerator(1.2, 1.3)
                .Create(_interval)
                .Select(q => new Quote("EUR/USD", q));
        }
    }
}
