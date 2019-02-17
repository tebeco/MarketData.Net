using System;
using System.Threading.Tasks;

namespace MarketData.Common.Infrastructure
{
    public interface IProduceObservable<T>
    {
        Task<IObservable<T>> GetStream(string paramValue);
        Task<IObservable<T>> GetStream();
    }
}
