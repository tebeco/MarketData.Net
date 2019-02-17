using MarketData.Backend.AllInOne.Startup;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace MarketData.Backend.AllInOne
{
    public class Market
    {
        public static async Task Main(string[] args)
        {
            var forexServer = StartForex();
            var stockQuoteServer = StartStockQuote();
            var tradeServer = StartTrade();
            var staticDataServer = StartStaticData();

            await Task.WhenAny(new[] { forexServer, stockQuoteServer, tradeServer, staticDataServer });
        }

        public static Task StartForex()
        {
            var forexHost = WebHost.CreateDefaultBuilder()
                .UseUrls("http://localhost:8096")
                .UseStartup<ForexStartup>()
                .Build();

            return forexHost.RunAsync();
        }

        public static Task StartStockQuote()
        {
            var stockQuoteHost = WebHost.CreateDefaultBuilder()
                .UseUrls("http://localhost:8097")
                .UseStartup<StockQuoteStartup>()
                .Build();

            return stockQuoteHost.RunAsync();
        }

        public static Task StartTrade()
        {
            var tradeHost = WebHost.CreateDefaultBuilder()
                .UseUrls("http://localhost:8098")
                .UseStartup<TradeStartup>()
                .Build();

            return tradeHost.RunAsync();
        }

        public static Task StartStaticData()
        {
            var staticDataHost = WebHost.CreateDefaultBuilder()
                .UseUrls("http://localhost:8099")
                .UseStartup<StaticDataStartup>()
                .Build();

            return staticDataHost.RunAsync();
        }
    }
}


