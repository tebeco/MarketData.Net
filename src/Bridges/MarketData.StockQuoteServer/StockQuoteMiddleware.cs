using MarketData.Dto;
using MarketData.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace MarketData.Bridge.StockQuote
{
    public class StockQuoteMiddleware
    {
        private IProduceObservable<string> _externalStockQuoteStream;
        private IProduceObservable<string> _externalForexStream;
        private IScheduler _scheduler;

        public StockQuoteMiddleware(
            IProduceObservable<string> externalStockQuoteStream,
            IProduceObservable<string> externalForexStream,
            IScheduler scheduler)
        {
            _externalStockQuoteStream = externalStockQuoteStream;
            _externalForexStream = externalForexStream;
            _scheduler = scheduler;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var observable = await GetEvents(httpContext.Request.Query["code"]);
        }

        public async Task<IObservable<Quote>> GetEvents(string stockCode)
        {
            var stockQuoteStream = await _externalStockQuoteStream.GetStream();
            //return stockQuoteStream.Select(Quote.FromJson);

            return stockQuoteStream
                .Select(Quote.FromJson)
                .Where(quote => string.Equals(stockCode, quote.Code, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
