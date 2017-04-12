using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
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
    public abstract class RxSseServer<T>
    {
        private IObservable<T> _events;
        private ISseChannel _sseChannel;

        public RxSseServer(int port, bool flaky)
        {
            Port = port;
            Flaky = flaky;
        }

        public void CreateServer()
        {
            if (Flaky)
            {
                _events = SubscriptionLimiter.LimitSubscriptions(1, InitializeEventStream());
            }
            else
            {
                _events = SubscriptionLimiter.LimitSubscriptions(1, InitializeEventStream());
                //_events = InitializeEventStream();
            }

            StartSseServer();
        }

        private void StartSseServer()
        {
            var factory = new SseChannelFactory();
            _sseChannel = factory.Create(IPAddress.Loopback, Port, 0, HandleRequest);
        }

        private void HandleRequest(IQueryCollection query)
        {
            Console.WriteLine("Subscribing ...");
            GetEvents(query).Subscribe(
                @event =>
                {
                    Console.WriteLine("Writing SSE event: " + @event);
                    ServerSentEvent sse = new ServerSentEvent(@event.ToString());

                    _sseChannel.SendAsync(sse, CancellationToken.None).Wait();
                },
                ex =>
                {
                    Console.WriteLine("Write to client failed, stopping response sending.");
                    _sseChannel.Dispose();
                    _sseChannel = null;
                },
                () =>
                {
                    Console.WriteLine("Stream completed, disposing channel");
                    _sseChannel.Dispose();
                    _sseChannel = null;
                }
            );
        }

        public IObservable<T> GetEvents(IQueryCollection httpRequest)
        {
            return _events;
        }

        protected abstract IObservable<T> InitializeEventStream();

        public int Port { get; }

        public bool Flaky { get; }

    }
}
