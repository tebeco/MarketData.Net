using MarketData.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Bridge.Stock
{
    public interface IProduceStaticDataObservable : IProduceObservable<string> { }

    public class StaticDataStreamProducer : IProduceStaticDataObservable
    {
        public StaticDataStreamProducer(Uri uri)
        {

        }

        public Task<IObservable<string>> GetStream(string paramValue)
        {
            throw new NotImplementedException();
        }

        public Task<IObservable<string>> GetStream()
        {
            throw new NotImplementedException();
        }
    }
}
