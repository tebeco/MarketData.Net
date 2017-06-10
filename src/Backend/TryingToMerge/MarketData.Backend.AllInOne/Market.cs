using MarketDataCommon.Dto;
using MarketDataExternal.Providers;
using MarketDataExternal.RxServers;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace MarketDataExternal
{
    public class Market
    {
        public static void Main(string[] args)
        {
            StartForex();
            StartStockQuote();
            StartTrade();
            StartStaticData();

            Console.ReadLine();   
        }

        public static void StartForex()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:8096")
                .UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration()
                .UseStartup<Startup<ForexProvider, Quote>>()
                .Build();

            host.Start();
        }

        public static void StartStockQuote()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:8097")
                .UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration()
                .UseStartup<Startup<StockQuoteProvider, Quote>>()
                .Build();

            host.Start();
        }

        public static void StartTrade()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://localhost:8098")
                //.UseIISIntegration()
                .UseStartup<Startup<TradeProvider, Trade>>()
                .Build();

            host.Start();
        }

        public static void StartStaticData()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:8099")
                .UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration()
                .UseStartup<StaticDataStartup>()
                .Build();

            host.Start();
        }
    }
}


