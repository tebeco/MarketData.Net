using MarketData.Backend.AllInOne.Startup;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace MarketData.Backend.AllInOne
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
            var forexHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment("Development")
                .UseUrls("http://localhost:8096")
                .UseStartup<ForexStartup>()
                .Build();

            forexHost.Start();
        }

        public static void StartStockQuote()
        {
            var stockQuoteHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment("Development")
                .UseUrls("http://localhost:8097")
                .UseStartup<StockQuoteStartup>()
                .Build();

            stockQuoteHost.Start();
        }

        public static void StartTrade()
        {
            var tradeHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment("Development")
                .UseUrls("http://localhost:8098")
                .UseStartup<TradeStartup>()
                .Build();

            tradeHost.Start();
        }

        public static void StartStaticData()
        {
            var staticDataHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment("Development")
                .UseUrls("http://localhost:8099")
                .UseStartup<StaticDataStartup>()
                .Build();

            staticDataHost.Start();
        }
    }
}


