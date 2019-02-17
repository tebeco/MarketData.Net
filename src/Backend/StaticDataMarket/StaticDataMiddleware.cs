using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MarketData.Backend.StaticData
{
    public class StaticDataMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly StaticDataProvider _staticDataProvider;

        public StaticDataMiddleware(RequestDelegate next, StaticDataProvider staticDataProvider)
        {
            _next = next;
            _staticDataProvider = staticDataProvider;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var queryCode = httpContext.Request.Query["code"];

            var stock = _staticDataProvider.GetStock(queryCode);

            if (stock == null)
            {
                httpContext.Response.StatusCode = 404;
                await httpContext.Response.WriteAsync($"Either no code in the URL or no match for the provided code. Query was : ({httpContext.Request.Query})").ConfigureAwait(false);
                await httpContext.Response.Body.FlushAsync().ConfigureAwait(false);
            }
            else
            {
                httpContext.Response.StatusCode = 200;
                await httpContext.Response.WriteAsync(stock.ToJson()).ConfigureAwait(false);
                await httpContext.Response.Body.FlushAsync().ConfigureAwait(false);
            }
        }
    }
}
