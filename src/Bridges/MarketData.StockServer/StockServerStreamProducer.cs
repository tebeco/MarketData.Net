using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using StockDto = MarketData.Dto.Stock;

namespace MarketData.Bridge.Stock
{
    public class StockServerStreamProducer
    {
        private IScheduler _scheduler;
        private IProduceStockObservable _externalStockEventStream;
        private IProduceStaticDataObservable _stockStaticDataStream;

        public StockServerStreamProducer(
            IProduceStaticDataObservable stockStaticDataStream,
            IProduceStockObservable externalStockEventStream,
            IScheduler scheduler)
        {
            _stockStaticDataStream = stockStaticDataStream;
            _externalStockEventStream = externalStockEventStream;
            _scheduler = scheduler;
        }

        public Task<IObservable<StockDto>> GetEvents()
        {
            return Task.FromResult(Observable.Return(new StockDto("SOME CODE", "SOME COMPANY NAME", "SOME MARKET")));
        }
    }
}
