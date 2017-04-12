using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace MarketDataCommon.Infrastructure
{
    public abstract class StaticHttpServer
    {
        public int Port { get; }

        public StaticHttpServer(int port)
        {
            Port = port;
        }

        public IWebHost CreateServer()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://localhost:{Port}")
                .Configure(appBuilder =>
                {
                    var loggerFactory = appBuilder.ApplicationServices.GetService<ILoggerFactory>();
                    Configure(appBuilder, loggerFactory);
                })
                .Build();

            return host;
        }

        public abstract Task ProcessHttpContextAsync(HttpContext httpContext);

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var loggerSettings = new ConsoleLoggerSettings()
            {
                IncludeScopes = false,
                Switches = new Dictionary<string, LogLevel>
                {
                    {"Default", LogLevel.Debug},
                    {"System", LogLevel.Information},
                    {"Microsoft", LogLevel.Information}
                }
            };

            loggerFactory.AddConsole(loggerSettings);
            loggerFactory.AddDebug();

            app.Use(async (context, next) =>
            {
                await ProcessHttpContextAsync(context).ConfigureAwait(false);
            });
        }

    }
}
