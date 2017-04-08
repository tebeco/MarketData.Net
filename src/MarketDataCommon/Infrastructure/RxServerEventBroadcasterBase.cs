using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace MarketDataCommon.Infrastructure
{
    public abstract class RxServerEventBroadcasterBase<T> : ICreateWebHost
    {
        private IObservable<T> _events;

        public RxServerEventBroadcasterBase(int port, bool flaky)
        {
            Port = port;
            Flaky = flaky;
        }

        public IWebHost CreateServer()
        {
            if (Flaky)
            {
                _events = SubscriptionLimiter.LimitSubscriptions(1, InitializeEventStream());
            }
            else
            {
                _events = InitializeEventStream();
            }
            return InternalCreateServer();
        }

        private IWebHost InternalCreateServer()
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

        public IObservable<T> GetEvents(object httpRequest)
        {
            return _events;
        }

        protected abstract IObservable<T> InitializeEventStream();
        public abstract Task ProcessHttpContextAsync(HttpContext httpContext);

        public int Port { get; }

        public bool Flaky { get; }

        #region From Startup
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
                await ProcessHttpContextAsync(context);
            });
        }


        #endregion
    }
}
