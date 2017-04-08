using System;
using System.Linq;
using System.Threading.Tasks;
using MarketDataCommon.Infrastructure;
using MarketDataExternal.Services;
using Microsoft.AspNetCore.Http;

namespace MarketDataExternal.Providers
{
    public class StaticDataProvider : RxHttpServer
    {
        private readonly StockService _stockService;

        public StaticDataProvider(int port) : base(port)
        {
            _stockService = new StockService();

        }

        public override async Task ProcessHttpContextAsync(HttpContext httpContext)
        {
            var queryCode = httpContext.Request.Query
                .Where(qp => string.Equals(qp.Key, "code", StringComparison.CurrentCultureIgnoreCase))
                .Select(qp => qp.Value.ToString()).FirstOrDefault();

            if (queryCode == null || !_stockService.TryGetFromCode(queryCode, out var stock))
            {
                httpContext.Response.StatusCode = 404;
                await httpContext.Response.WriteAsync("Either no code in the URL or no match for the provided code");
                await httpContext.Response.Body.FlushAsync();
            }
            else
            {
                httpContext.Response.StatusCode = 200;
                await httpContext.Response.WriteAsync(stock.ToJson());
                await httpContext.Response.Body.FlushAsync();
            }
        }
    }
}
