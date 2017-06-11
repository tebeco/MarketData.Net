using MarketData.Infrastructure;
using Xunit;
using MarbleScheduler = MarbleTest.Net.MarbleScheduler;
using NSubstitute;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MarketData.Dto;
using System.Reactive.Concurrency;
using MarketData.Bridge.StockQuote;

namespace MarketDataWeb.Tests
{
    public class StockQuoteServerTest
    {
        /**
        * Test 3
         */
        [Fact(Skip = "Not done yet")]
        public async Task should_filter_quotes_for_requested_stock()
        {
            // given
            var scheduler = new MarbleScheduler();
            IObservable<String> quoteSource = scheduler.CreateHotObservable<string>("--(ga)--", new
            {
                g = new Quote("GOOGL", 705.8673).ToJson(),
                a = new Quote("APPLE", 98.18).ToJson()
            });
            IObservable<String> forexSource = scheduler.CreateHotObservable<string>("--f--", new { f = new Quote("EUR/USD", 1).ToJson() });

            // when
            var stockQuoteServer = CreateServer(quoteSource, forexSource, scheduler);
            var stockQuoteStream = await stockQuoteServer.GetEvents("GOOGL");

            // then
            scheduler.ExpectObservable(stockQuoteStream.Select(q => q.Code))
                     .ToBe("--v--", new { v = "GOOGL" });

            scheduler.Flush();
        }
        
        /**
        * Test 13
        */
        [Fact(Skip = "Not done yet")]
        public async Task should_generate_one_quote_in_euro_for_one_quote_in_dollar()
        {
            // given
            var scheduler = new MarbleScheduler();
            var quoteSource = scheduler.CreateHotObservable<string>("--g----", new { g = new Quote("GOOGL", 1300).ToJson() });
            var forexSource = scheduler.CreateHotObservable<string>("--f-x--", new
            {
                f = new Quote("EUR/USD", 1.3).ToJson(),
                x = new Quote("EUR/USD", 1.4).ToJson()
            });

            // when
            var stockQuoteServer = CreateServer(quoteSource, forexSource, scheduler);
            var stream = await stockQuoteServer.GetEvents("GOOGL");

            // then
            scheduler.ExpectObservable(stream.Select(q => q.QuoteValue))
                     .ToBe("--v----", new { v = 1000d });
            scheduler.Flush();
        }

        /**
        * Test 14
        */
        [Fact(Skip = "Not done yet")]
        public async Task should_generate_quotes_in_euro_using_latest_known_foreign_exchange_rate()
        {
            // given
            var scheduler = new MarbleScheduler();
            var quoteSource = scheduler.CreateHotObservable<string>("----g-", new { g = new Quote("GOOGL", 1300).ToJson() });
            var forexSource = scheduler.CreateHotObservable<string>("-f-x--", new
            {
                f = new Quote("EUR/USD", 1.2).ToJson(),
                x = new Quote("EUR/USD", 1.3).ToJson()
            });

            // when
            var stockQuoteServer = CreateServer(quoteSource, forexSource, scheduler);
            var stream = await stockQuoteServer.GetEvents("GOOGL");

            // then
            scheduler.ExpectObservable(stream.Select(q => q.QuoteValue))
                    .ToBe("----v-", new { v = 1000d });
            scheduler.Flush();
        }

        /**
        * Test 15
        */
        [Fact(Skip = "Not done yet")]
        public async Task should_unsubscribe_to_forex_stream_when_unscribing_to_quote()
        {
            // given
            var scheduler = new MarbleScheduler();
            var quoteSource = scheduler.CreateHotObservable<string>("----g---", new { g = new Quote("GOOGL", 1300).ToJson() });
            var forexSource = scheduler.CreateHotObservable<string>("---f----", new { f = new Quote("EUR/USD", 1.2).ToJson() });
            var stopSource = scheduler.CreateHotObservable<string>("------s-");

            // when
            var stockQuoteServer = CreateServer(quoteSource, forexSource, scheduler);
            var stream = await stockQuoteServer.GetEvents("GOOGL");
            stream.TakeUntil(stopSource).Subscribe();


            // then
            scheduler.ExpectSubscription(forexSource.Subscriptions)
                     .ToBe("^-----!-");
            scheduler.Flush();
        }

        /**
        * Test 16
        */
        [Fact(Skip = "Not done yet")]
        public async Task should_send_an_error_when_no_forex_data_after_five_seconds()
        {
            // given
            var scheduler = new MarbleScheduler();
            IObservable<String> quoteSource = scheduler.CreateHotObservable<string>("-g-----", new { g = new Quote("GOOGL", 1300).ToJson() });
            IObservable<String> forexSource = scheduler.CreateHotObservable<string>("-------");

            // when
            var stockQuoteServer = CreateServer(quoteSource, forexSource, scheduler);
            var stream = await stockQuoteServer.GetEvents("GOOGL");

            // then
            scheduler.ExpectObservable(stream)
                     .ToBe("------#");
            scheduler.Flush();
        }

        private StockQuoteMiddleware CreateServer(IObservable<string> quoteSource, IObservable<string> forexSource, IScheduler scheduler)
        {
            var mockStockQuoteProducer = Substitute.For<IProduceObservable<string>>();
            mockStockQuoteProducer.GetStream().Returns(Task.FromResult(quoteSource));

            var mockForexProducer = Substitute.For<IProduceObservable<string>>();
            mockForexProducer.GetStream().Returns(Task.FromResult(forexSource));

            return new StockQuoteMiddleware(mockStockQuoteProducer, mockForexProducer, scheduler);
        }
    }
}