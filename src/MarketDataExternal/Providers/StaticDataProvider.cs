using MarketDataCommon.Infrastructure;

namespace MarketDataExternal.Providers
{
    public class StaticDataProvider : RxHttpServer
    {
        public StaticDataProvider(int port) : base(port)
        {

        }
    }
}
