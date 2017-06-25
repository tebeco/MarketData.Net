using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Text;

namespace MarketData.Infrastructure

{
    // See code from https://github.com/aspnet/SignalR/blob/eee2683b5ede1f10d85e78f29c11c4d8f70ef330/src/Microsoft.AspNetCore.Sockets.Http/Transports/WebSocketsTransport.cs#L124-L183
    public class RxClientWebSocket : IProduceObservable<string>
    {
        protected Uri _uri;
        protected CancellationToken _cancellationToken;

        public RxClientWebSocket(Uri uri, CancellationToken cancellationToken)
        {
            _uri = uri;
            _cancellationToken = cancellationToken;
        }

        private async Task<IObservable<Message>> FromWebSocketUri(Uri uri, CancellationToken cancellationToken)
        {
            var client = new ClientWebSocket();
            await client.ConnectAsync(uri, cancellationToken);

            return FromClientWebSocket(client, cancellationToken);
        }

        private IObservable<Message> FromClientWebSocket(ClientWebSocket webSocket, CancellationToken cancellationToken)
        {
            if (webSocket == null)
                throw new ArgumentNullException(nameof(webSocket));

            return Observable.Create<Message>(async (observer, cts) =>
            {
                await StartPolling(webSocket, observer, cts);
                return (IDisposable)observer;
            });
        }

        private async Task StartPolling(ClientWebSocket webSocket, IObserver<Message> pushToSubject, CancellationToken cts)
        {
            try
            {
                var incomingMessage = new List<ArraySegment<byte>>();
                while (cts.IsCancellationRequested)
                {
                    var totalBytes = 0;
                    WebSocketReceiveResult receiveResult;
                    do
                    {
                        var buffer = new ArraySegment<byte>(new byte[4096]);

                        // Exceptions are handled above where the send and receive tasks are being run.
                        receiveResult = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                        if (receiveResult.MessageType == WebSocketMessageType.Close)
                        {
                            pushToSubject.OnCompleted();
                            return;
                        }

                        var truncBuffer = new ArraySegment<byte>(buffer.Array, 0, receiveResult.Count);
                        incomingMessage.Add(truncBuffer);
                        totalBytes += receiveResult.Count;

                    } while (!receiveResult.EndOfMessage);

                    Message message;
                    var messageType = receiveResult.MessageType == WebSocketMessageType.Binary ? MessageType.Binary : MessageType.Text;
                    if (incomingMessage.Count > 1)
                    {
                        var messageBuffer = new byte[totalBytes];
                        var offset = 0;
                        for (var i = 0; i < incomingMessage.Count; i++)
                        {
                            Buffer.BlockCopy(incomingMessage[i].Array, 0, messageBuffer, offset, incomingMessage[i].Count);
                            offset += incomingMessage[i].Count;
                        }

                        message = new Message(messageBuffer, messageType, receiveResult.EndOfMessage);
                    }
                    else
                    {
                        var buffer = new byte[incomingMessage[0].Count];
                        Buffer.BlockCopy(incomingMessage[0].Array, incomingMessage[0].Offset, buffer, 0, incomingMessage[0].Count);
                        message = new Message(buffer, messageType, receiveResult.EndOfMessage);
                    }

                    incomingMessage.Clear();
                    pushToSubject.OnNext(message);
                }
            }
            catch (Exception ex)
            {
                pushToSubject.OnError(ex);
            }
        }

        public Task<IObservable<string>> GetStream(string _) { throw new NotImplementedException(); }

        public async Task<IObservable<string>> GetStream()
        {
            var x = await FromWebSocketUri(_uri, _cancellationToken);
            return x.Select(msg => Encoding.UTF8.GetString(msg.Payload));
        }
    }
}