﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace MarketData.Backend.StockQuote
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
