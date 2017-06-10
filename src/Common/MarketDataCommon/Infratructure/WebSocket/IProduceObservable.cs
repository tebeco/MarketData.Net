using System;
using System.Threading.Tasks;

namespace MarketDataCommon.Infratructure.WebSocket
{
    public interface IProduceObservable<T>
    {
        Task<IObservable<T>> GetStream(string paramValue);
        Task<IObservable<T>> GetStream();
    }
}
