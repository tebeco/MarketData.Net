using Microsoft.AspNetCore.Hosting;
using NetCoreSse;

namespace MarketDataCommon.Infrastructure
{
    public interface ICreateWebHost
    {
        ISseChannel CreateServer();
    }
}
