using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using MarketDataExternal.Providers;

namespace MarketDataExternal.RxServers
{
    public class StaticDataMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly StaticDataProvider _staticDataProvider;

        public StaticDataMiddleware(RequestDelegate next)
        {
            _next = next;
            _staticDataProvider = new StaticDataProvider();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var queryCode = httpContext.Request.Query
                    .Where(qp => string.Equals(qp.Key, "code", StringComparison.CurrentCultureIgnoreCase))
                    .Select(qp => qp.Value.ToString()).FirstOrDefault();

            var stock = _staticDataProvider.GetStock(queryCode);

            if (queryCode == null)
            {
                httpContext.Response.StatusCode = 404;
                await httpContext.Response.WriteAsync("Either no code in the URL or no match for the provided code").ConfigureAwait(false);
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
