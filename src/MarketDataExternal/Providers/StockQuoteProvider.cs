using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MarketDataCommon.Dto;
using MarketDataCommon.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace MarketDataExternal.Providers
{
    public class StockQuoteProvider : RxSseServer<Quote>
    {
        private const double GoogleMin = 116;
        private const double GoogleMax = 136;
        private const double IbmMin = 120;
        private const double IbmMax = 135;
        private const double HpMin = 12.5;
        private const double HpMax = 14;
        private const double AppleMin = 94;
        private const double AppleMax = 99;
        private const double MicrosoftMin = 51;
        private const double MicrosoftMax = 53;

        public StockQuoteProvider(int port, bool flaky) : base(port, flaky)
        {

        }

        protected override IObservable<Quote> InitializeEventStream()
        {
            var googleStock = new RandomSequenceGenerator(GoogleMin, GoogleMax)
                .Create(TimeSpan.FromMilliseconds(200))
                .Select(s => new Quote("GOOGL", s));

            var ibmStock = new RandomSequenceGenerator(IbmMin, IbmMax)
                .Create(TimeSpan.FromMilliseconds(705))
                .Select(s => new Quote("IBM", s));

            var hpStock = new RandomSequenceGenerator(HpMin, HpMax)
                .Create(TimeSpan.FromMilliseconds(602))
                .Select(s => new Quote("HPQ", s));

            var appleStock = new RandomSequenceGenerator(AppleMin, AppleMax)
                .Create(TimeSpan.FromMilliseconds(253))
                .Select(s => new Quote("AAPL", s));

            var microsoftStock = new RandomSequenceGenerator(MicrosoftMin, MicrosoftMax)
                    .Create(TimeSpan.FromMilliseconds(407))
                    .Select(s => new Quote("MSFT", s));

            return Observable.Merge(googleStock, ibmStock, hpStock, appleStock, microsoftStock).Publish().RefCount();

        }
    }
}
