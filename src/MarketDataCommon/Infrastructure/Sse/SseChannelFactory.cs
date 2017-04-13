using System;
using System.Net;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NetCoreSse.Hosting;

namespace NetCoreSse
{
    public class SseChannelFactory
    {
        private IPAddress _host;
        private int _port;

        public virtual ISseChannel Create(string host, int port, int bufferSize, Func<IQueryCollection, IDisposable> handleRequestAsync)
        {
            var ipAddress = IPAddress.Parse(host);
            return Create(ipAddress, port, bufferSize, handleRequestAsync);
        }

        public virtual ISseChannel Create(IPAddress host, int port, int bufferSize, Func<IQueryCollection, IDisposable> handleRequestAsync)
        {
            _host = host;
            _port = port;
            var channel = new MulticastChannel(bufferSize);

            var httpServer = new SseHttpServer(_host, _port, async contextChannel =>
            {
                await PrepareSseStreamAsync(contextChannel).ConfigureAwait(false);
                await channel.AddChannel(contextChannel, contextChannel.Token).ConfigureAwait(false);

                return handleRequestAsync(contextChannel.HttpContext.Request.Query);
            });
            channel.AttachDisposable(httpServer);
            httpServer.Run();

            return channel;
        }

        private async Task PrepareSseStreamAsync(HttpContextChannel httpContextChannel)
        {
            var httpResponse = httpContextChannel.HttpContext.Response;

            httpResponse.StatusCode = 200;
            httpResponse.Headers.Add("Connection", "keep-alive");
            httpResponse.Headers.Add("Content-Type", "text/event-stream");
            httpResponse.Headers.Add("Cache-Control", "no-cache");
            httpResponse.Headers.Add("Access-Control-Allow-Origin", "*");

            await httpResponse.Body.FlushAsync().ConfigureAwait(false);
        }
    }
}
