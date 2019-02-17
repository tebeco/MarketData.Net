using MarketData.Backends.StockQuote;
using TradeDto = MarketData.Dto.Trade;
using MarketData.Common.Infrastructure;
using System;
using System.Reactive.Linq;

namespace MarketData.Backends.Trade
{
    public class TradeProvider
    {
        private readonly StockQuoteProvider _stockQuoteProvider;

        public TradeProvider()
        {
            _stockQuoteProvider = new StockQuoteProvider();
        }

        public IObservable<TradeDto> GetEventStream()
        {
            var quotes = _stockQuoteProvider.GetEventStream();

            var quantities = new RandomSequenceGenerator(10, 1000).CreateIntegerSequence(TimeSpan.FromMilliseconds(50)).Publish().RefCount();

            return quotes.SelectMany(quote =>
            {
                return quantities
                    .Take(1)
                    .Select(qty => new TradeDto(quote.Code, qty, quote.QuoteValue * qty));
            });
        }
    }
}
