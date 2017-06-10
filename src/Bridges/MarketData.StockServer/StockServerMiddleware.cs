using MarketDataCommon.Dto;
using MarketDataCommon.Infratructure.WebSocket;
using System.Reactive.Concurrency;
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

namespace MarketData.StockServer
{
    public class StockServerMiddleware
    {
        private StockServerStreamProducer _stockServerStreamProducer;
        private RequestDelegate _next;
        private ILogger<StockServerMiddleware> _logger;

        public StockServerMiddleware(RequestDelegate next, ILogger<StockServerMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;

        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var stockCode = context.Request.Query["code"];
                IProduceStaticDataObservable stockStaticDataStream = new StaticDataStreamProducer(new Uri($"http://localhost:8097?code={stockCode}"));
                IProduceStockObservable externalStockEventStream = new StockServerClientWebSocket(new Uri($"ws://localhost:8097"), context.RequestAborted);

                _stockServerStreamProducer = new StockServerStreamProducer(stockStaticDataStream, externalStockEventStream, Scheduler.Default);

                await StartSending(context);
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private async Task StartSending(HttpContext context)
        {
            var stockServerStream = await _stockServerStreamProducer.GetEvents();
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var webSocketClosedStream = StartPolling(webSocket).ToObservable();

            await stockServerStream.Select(stock => (webSocket, stock))
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
