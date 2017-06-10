using MarketDataCommon.Dto;
using MarketDataCommon.Infratructure.WebSocket;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace MarketData.StockServer
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

        public Task<IObservable<Stock>> GetEvents()
        {
            return Task.FromResult(Observable.Return(new Stock("SOME CODE", "SOME COMPANY NAME", "SOME MARKET")));
        }
    }
}
