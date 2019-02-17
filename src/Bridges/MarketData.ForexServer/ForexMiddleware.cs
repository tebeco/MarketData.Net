using MarketData.Common.Dto;
using MarketData.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace MarketData.Bridge.Forex
{
    public class ForexMiddleware
    {
        private IProduceObservable<string> _externalForexStream;

        public ForexMiddleware(IProduceObservable<string> externalForexStream)
        {
            _externalForexStream = externalForexStream;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var forexStream = await GetEvents();
        }

        public async Task<IObservable<double>> GetEvents()
        {
            var stream = await _externalForexStream.GetStream();
            //return Task.FromResult(Observable.Never<double>());

            return stream
                .Select(quoteAsString => Quote.FromJson(quoteAsString).QuoteValue)
                .Take(1);

        }
    }
}
