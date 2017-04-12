using MarketDataExternal.Providers;

namespace MarketDataExternal
{
    public class Market
    {
        public void Start()
        {
            bool flaky = false;

            ForexProvider forexProvider = new ForexProvider(7096, flaky);
            forexProvider.CreateServer();

            StockQuoteProvider stockQuoteProvider = new StockQuoteProvider(7097, flaky);
            stockQuoteProvider.CreateServer();

            TradeProvider tradeProvider = new TradeProvider(7098, flaky, stockQuoteProvider);
            tradeProvider.CreateServer();

            StaticDataProvider staticDataProvider = new StaticDataProvider(7099);
            staticDataProvider.CreateServer().Start();
        }
    }
}


