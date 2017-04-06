using System;
using System.Reactive.Linq;
using MarketDataCommon.Dto;
using MarketDataCommon.Infrastructure;

namespace MarketDataExternal.Providers
{
    public class TradeProvider : RxServerEventBroadcasterBase<Trade>
    {
        private readonly StockQuoteProvider _stockQuoteProvider;

        public TradeProvider(int port, bool flaky, StockQuoteProvider stockQuoteProvider) : base(port, flaky)
        {
            _stockQuoteProvider = stockQuoteProvider;
        }

        protected override IObservable<Trade> InitializeEventStream()
        {
            var quotes = _stockQuoteProvider.GetEvents(null);

            var quantities = new RandomSequenceGenerator(10, 1000).CreateIntegerSequence(TimeSpan.FromMilliseconds(50)).Publish().RefCount();

            return quotes.SelectMany(quote =>
            {
                return quantities
                    .Take(1)
                    .Select(qty => new Trade(quote.Code, qty, quote.QuoteValue * qty));
            });
        }

    }
}
