using Xunit;
using NSubstitute;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MarketData.Common.Dto;
using MarketData.Common.Infrastructure;
using MarketData.Bridge.Forex;
using MarbleTest.Net;

namespace MarketDataWeb.Tests
{
    public class ForexServerTests
    {
        /*
         * Test 1
         */
        [Fact(Skip = "Not done yet")]
        public async Task should_forward_forex_data()
        {
            // given
            var scheduler = new MarbleScheduler();
            var forexSource = scheduler.CreateHotObservable<string>("--f--", new { f = new Quote("EUR/USD", 1.4).ToJson() });

            // when
            var forexStream = await Create(forexSource).GetEvents();

            // then
            scheduler.ExpectObservable(forexStream.Take(1))
                     .ToBe("--(v|)", new { v = 1.4 });

            scheduler.Flush();
        }

        /*
         * Test 2
         */
        [Fact(Skip = "Not done yet")]
        public async Task should_forward_only_one_forex_data()
        {
            // given
            var scheduler = new MarbleScheduler();
            var forexSource = scheduler.CreateHotObservable<string>("--f-x-", new
            {
                f = new Quote("EUR/USD", 1.4).ToJson(),
                x = new Quote("EUR/USD", 1.5).ToJson()
            });

            // when
            var forexStream = await Create(forexSource).GetEvents();

            // then
            scheduler.ExpectObservable(forexStream)
                     .ToBe("--(v|)", new { v = 1.4 });

            scheduler.Flush();
        }

        private ForexMiddleware Create(IObservable<String> forexSource)
        {
            var mock = Substitute.For<IProduceObservable<string>>();
            mock.GetStream().Returns(Task.FromResult(forexSource));

            return new ForexMiddleware(mock);
        }
    }
}
