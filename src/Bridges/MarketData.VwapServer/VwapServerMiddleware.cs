using MarketData.Dto;
using MarketData.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace MarketData.VwapServer
{
    public class VwapServerMiddleware
    {
        private IProduceObservable<string> _tradeServerStream;
        private IScheduler _scheduler;

        public VwapServerMiddleware(IProduceObservable<string> tradeServerStream, IScheduler scheduler)
        {
            _tradeServerStream = tradeServerStream;
            _scheduler = scheduler;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var stream = await GetStream(httpContext.Request.Query["code"]);
        }

        public Task<IObservable<Vwap>> GetStream(string stockCode)
        {
            return Task.FromResult(Observable.Never<Vwap>());

        }
    }
}
