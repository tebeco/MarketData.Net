using MarketDataCommon.Dto;
using MarketDataCommon.Infrastructure;
using System;
using System.Reactive.Linq;

namespace MarketDataExternalCommon
{
    public class TradeProvider
    {
        private readonly StockQuoteProvider _stockQuoteProvider;

        public TradeProvider()
        {
            _stockQuoteProvider = new StockQuoteProvider();
        }

        public IObservable<Trade> GetEventStream()
        {
            var quotes = _stockQuoteProvider.GetEventStream();

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
