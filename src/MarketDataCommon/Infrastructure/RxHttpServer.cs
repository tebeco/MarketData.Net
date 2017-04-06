using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace MarketDataCommon.Infrastructure
{
    public abstract class RxHttpServer : ICreateWebHost
    {
        public int Port { get; }

        public RxHttpServer(int port)
        {
            Port = port;
        }

        public IWebHost CreateServer()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.UseStartup<TStartup>()
                .Build();

            return host;
        }
    }
}
