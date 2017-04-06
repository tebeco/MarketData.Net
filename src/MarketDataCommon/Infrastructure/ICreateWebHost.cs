using Microsoft.AspNetCore.Hosting;

namespace MarketDataCommon.Infrastructure
{
    public interface ICreateWebHost
    {
        IWebHost CreateServer();
    }
}
