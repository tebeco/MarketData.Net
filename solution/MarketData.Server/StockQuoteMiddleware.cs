using MarketDataCommon.Dto;
using MarketDataCommon.Infratructure;
using MarketDataCommon.Infratructure.WebSocket;
using Microsoft.AspNetCore.Http;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace MarketData.StockQuoteServer
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
            var forexStream = await _externalForexStream.GetStream();
            //return forexStream.Select(Quote.FromJson);
            
        }
    }
}
