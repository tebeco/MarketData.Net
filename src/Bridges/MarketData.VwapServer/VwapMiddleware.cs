using MarketData.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace MarketData.VwapServer.Properties
{
    public class VwapMiddleware
    {
        private IObservable<string> _externalTradeStream;
        private IScheduler _scheduler;

        public VwapMiddleware(
            IObservable<string> externalTradeStream,
            IScheduler scheduler)
        {
            _externalTradeStream = externalTradeStream;
            _scheduler = scheduler;
        }

        public IObservable<Vwap> GetEvents(HttpContext httpContext)
        {
            var stockCode = httpContext.Request.Query["code"];

            return Observable.Never<Vwap>();
        }
    }
}
