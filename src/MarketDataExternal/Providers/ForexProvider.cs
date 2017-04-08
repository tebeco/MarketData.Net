using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MarketDataCommon.Dto;
using MarketDataCommon.Infrastructure;
using Microsoft.AspNetCore.Http;

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

        public override async Task ProcessHttpContextAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 200;
            await httpContext.Response.WriteAsync("Not implemented yet");
            await httpContext.Response.Body.FlushAsync();
        }
    }
}
