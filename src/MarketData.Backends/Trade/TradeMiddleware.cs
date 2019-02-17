using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketData.Backends.Trade.Middlewares
{
    public class TradeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly TradeProvider _tradeProvider;

        public TradeMiddleware(RequestDelegate next, ILogger<TradeMiddleware> logger, TradeProvider tradeProvider)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
            _tradeProvider = tradeProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                await StartSending(context);
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private async Task StartSending(HttpContext context)
        {
            var tradeStream = _tradeProvider.GetEventStream();
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var webSocketClosedStream = StartPolling(webSocket).ToObservable();

            await tradeStream.Select(quote => (webSocket, quote))
                .TakeWhile(socketAndQuote => !socketAndQuote.Item1.CloseStatus.HasValue)
                .TakeUntil(webSocketClosedStream)
                .Select(socketAndQuote => (socketAndQuote.Item1, socketAndQuote.Item2.ToJson()))
                .SelectMany(socketAndQuoteAsJson =>
                {
                    var socket = socketAndQuoteAsJson.Item1;
                    var quoteAsJson = socketAndQuoteAsJson.Item2;
                    var quoteBuffer = Encoding.UTF8.GetBytes(quoteAsJson);

                    if (socket.CloseStatus.HasValue || socket.State != WebSocketState.Open)
                    {
                        _logger.LogError("WebSocket crashed");
                        return Observable.Empty<Unit>();
                    }

                    return Observable
                        .FromAsync(async () => await socket.SendAsync(new ArraySegment<byte>(quoteBuffer, 0, quoteBuffer.Length), WebSocketMessageType.Text, true, context.RequestAborted))
                        .Do(_ => _logger.LogDebug($"Sent Frame : {quoteAsJson}"))
                        .Catch<Unit, Exception>(ex =>
                        {
                            _logger.LogError(ex.Message);
                            return Observable.Empty<Unit>();
                        });
                })
                .ToTask(context.RequestAborted);
        }

        private async Task<WebSocketReceiveResult> StartPolling(WebSocket webSocket)
        {
            var incomingMessage = new List<ArraySegment<byte>>();
            while (true)
            {
                const int bufferSize = 4096;
                WebSocketReceiveResult receiveResult;
                do
                {
                    var buffer = new ArraySegment<byte>(new byte[bufferSize]);

                    // Exceptions are handled above where the send and receive tasks are being run.
                    receiveResult = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        return receiveResult;
                    }

                } while (!receiveResult.EndOfMessage);
            }
        }
    }
}