using MarketDataCommon.Dto;
using System;

namespace MarketDataExternal.Providers
{
    public interface IStreamProvider<T> where T : IJsonable
    {
        IObservable<T> GetEventStream();
    }
}
