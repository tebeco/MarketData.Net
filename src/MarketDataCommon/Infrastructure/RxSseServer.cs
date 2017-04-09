using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NetCoreSse;

namespace MarketDataCommon.Infrastructure
{
    public abstract class RxSseServer<T> : ICreateWebHost
    {
        private IObservable<T> _events;

        public RxSseServer(int port, bool flaky)
        {
            Port = port;
            Flaky = flaky;
        }

        public IWebHost CreateServer()
        //{
        //    if (Flaky)
        //    {
        //        _events = SubscriptionLimiter.LimitSubscriptions(1, InitializeEventStream());
        //    }
        //    else
        //    {
        //        _events = InitializeEventStream();
        //    }
        //    return InternalCreateServer();
        //}

        //private IWebHost InternalCreateServer()
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

        public abstract Task ProcessHttpContextAsync(HttpContext httpContext);

        protected abstract IObservable<T> GetEvents(IQueryCollection query);

        public int Port { get; }

        public bool Flaky { get; }

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

            app.Use((httpContext, next) =>
            {
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                httpContext.Response.Headers.Add("Cache-Control", "no-cache");
                httpContext.Response.Headers.Add("Cononection", "keep-alive");
                httpContext.Response.Headers.Add("Content-Type", "text /event-stream");

                GetIntervalObservable(httpContext);

                return Task.CompletedTask;
            });
        }

        private IObservable<Unit> GetIntervalObservable(HttpContext httpContext)
        {
            return GetEvents(httpContext.Request.Query)
                .SelectMany(async @event =>
                {
                    Console.WriteLine("Writing SSE event: " + @event);
                    ServerSentEvent sse = new ServerSentEvent("", @event.ToString());
                    await httpContext.Response.WriteAsync(sse.ToString());
                    return Observable.Empty<T>();
                })
                .Materialize()
                .TakeWhile(notification =>
                {
                    if (notification.Kind == NotificationKind.OnError)
                    {
                        Console.WriteLine("Write to client failed, stopping response sending.");
                        Console.WriteLine(notification.Exception);
                    }
                    return notification.Kind != NotificationKind.OnError;
                })
                .Select(x => Unit.Default);
        }
    }
}
