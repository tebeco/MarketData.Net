using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
namespace NetCoreSse.Hosting
{
    public class SseHttpServer : IDisposable
    {
        private readonly Func<HttpContextChannel, Task<IDisposable>> _handler;
        private volatile bool _disposed;
        private IWebHost _host;

        public SseHttpServer(IPAddress ip, int port, Func<HttpContextChannel, Task<IDisposable>> handler)
        {
            _handler = handler;
            Port = port;
            Host = ip;
        }

        public IPAddress Host { get; }

        public int Port { get; }

        public string Url => $"http://{Host}:{Port}/";

        public void Run()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Cannot run disposed server");
            }

            _host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(Url)
                .Configure(app =>
                {
                    app.Run(async context =>
                    {
                        var token = context.RequestAborted;

                        var disposable = await _handler(new HttpContextChannel(context, token)).ConfigureAwait(false);

                        await token;
                        disposable.Dispose();
                    });
                })
                .Build();

            _host.Start();
        }

        public void Dispose()
        {
            _disposed = true;
            _host?.Dispose();
        }
    }
}
