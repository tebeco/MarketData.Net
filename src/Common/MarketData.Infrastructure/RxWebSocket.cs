using System;
using System.Net.WebSockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace MarketData.Common.Infrastructure
{
    public class RxWebSocket<TReceived>
    {
        private readonly Uri _uri;
        private readonly CancellationTokenSource _cts;

        public RxWebSocket(string uri, CancellationTokenSource cts) : this(new Uri(uri))
        {
            _cts = cts;
        }

        public RxWebSocket(Uri uri)
        {
            _uri = uri;
        }

        public IObservable<TReceived> GetObservables(IScheduler scheduler)
        {
            ClientWebSocket webSocket = new ClientWebSocket();

            var o = Observable.CombineLatest(
                Observable.FromAsync(async (cts) => await webSocket.ConnectAsync(_uri, _cts.Token), scheduler),
                Observable.Return(default(TReceived)),
                (ws, result) => ws
            );

            return Observable.Create<TReceived>(async (observer, cts) =>
            {
                while (!cts.IsCancellationRequested)
                {
                    var buffer = new ArraySegment<byte>(new byte[4096]);
                    var receiveResult = await webSocket.ReceiveAsync(buffer, cts);
                    if (receiveResult.MessageType == WebSocketMessageType.Close && !cts.IsCancellationRequested)
                    {
                        observer.OnCompleted();
                    }

                }
            });
        }
    }
}
