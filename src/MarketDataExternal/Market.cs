using MarketDataExternal.Providers;

namespace MarketDataExternal
{
    public class Market
    {
        public void Start()
        {
            bool flaky = false;

            ForexProvider forexProvider = new ForexProvider(8096, flaky);
            forexProvider.CreateServer().Start();

            StockQuoteProvider stockQuoteProvider = new StockQuoteProvider(8097, flaky);
            stockQuoteProvider.CreateServer().Start();

            TradeProvider tradeProvider = new TradeProvider(8098, flaky, stockQuoteProvider);
            tradeProvider.CreateServer().Start();

            StaticDataProvider staticDataProvider = new StaticDataProvider(8099);
            staticDataProvider.CreateServer().Start();
        }
    }
}


