using MarketData.Dto;
using System;

namespace MarketData.Backend.AllInOne.Providers
{
    public interface IStreamProvider<T> where T : IProduceJson
    {
        IObservable<T> GetEventStream();
    }
}
