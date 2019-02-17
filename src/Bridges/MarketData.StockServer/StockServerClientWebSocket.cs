using MarketData.Common.Infrastructure;
using System;
using System.Threading;

namespace MarketData.Bridge.Stock
{
    public interface IProduceStockObservable : IProduceObservable<string>
    {

    }

    public class StockServerClientWebSocket : RxClientWebSocket, IProduceStockObservable
    {
        public StockServerClientWebSocket(Uri uri, CancellationToken cancellationToken) : base(uri, cancellationToken)
        {
        }
    }
}
