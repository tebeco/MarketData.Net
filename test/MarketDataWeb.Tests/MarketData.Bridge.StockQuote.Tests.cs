using Xunit;
using MarbleScheduler = MarbleTest.Net.MarbleScheduler;
using NSubstitute;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MarketData.Dto;
using MarketData.Bridge.Stock;

namespace MarketDataWeb.Tests
{
    public class StockServerTests
    {
        /**
        * Test 7
         */
        [Fact(Skip = "Not done yet")]
        public async Task should_send_a_stock_message_when_receiving_a_quote()
        {
            // given
            var scheduler = new MarbleScheduler();
            var quoteSource = scheduler.CreateHotObservable<string>("--q--", new { q = new Quote("GOOGL", 705.8673).ToJson() });

            // when
            var server = CreateServer(quoteSource, scheduler);
            var stream = await server.GetEvents();

            // then
            scheduler.ExpectObservable(stream.Select(s => s.CompanyName).Concat(Observable.Never<string>()))
                    .ToBe("--s--", new { s = "Alphabet Inc" });

            scheduler.Flush();
        }

        /**
         * Test 8
         */
        [Fact]
        public async Task should_send_a_stock_message_only_once_when_receiving_two_quotes_for_the_same_stock()
        {
            // given
            var scheduler = new MarbleScheduler();
            var quoteSource = scheduler.CreateHotObservable<string>("--f-s-t--", new
            {
                f = new Quote("GOOGL", 705.8673).ToJson(),
                s = new Quote("GOOGL", 705.8912).ToJson(),
                t = new Quote("IBM", 106.344).ToJson()
            });

            // when
            var server = CreateServer(quoteSource, scheduler);
            var stream = await server.GetEvents();

            // then
            scheduler.ExpectObservable(stream.Select(s=>s.CompanyName).Concat(Observable.Never<string>()))
                    .ToBe("--g---i--", new
                    {
                        g = "Alphabet Inc",
                        i = "International Business Machines Corp."
                    });
            scheduler.Flush();
        }

        /**
        * Test 9
        */
        [Fact]
        public async Task should_stop_stream_after_10_seconds()
        {
            // given
            var scheduler = new MarbleScheduler();
            var quoteSource = Observable.Never<string>();

            // when
            var server = CreateServer(quoteSource, scheduler);
            var stream = await server.GetEvents();

            // then
            scheduler.ExpectObservable(stream).ToBe("----------|");
            scheduler.Flush();
        }

        public StockServerStreamProducer CreateServer(IObservable<String> quoteSource, MarbleScheduler scheduler)
        {
            var stockClient = Substitute.For<IProduceStaticDataObservable>();
            var stockQuoteClient = Substitute.For<IProduceStockObservable>();
            stockQuoteClient.GetStream().Returns(quoteSource);

            stockClient.GetStream("GOOGL").Returns(
                Observable.Return(new Stock("GOOGL", "Alphabet Inc", "NASDAQ").ToJson())
            );

            stockClient.GetStream("IBM").Returns(
                Observable.Return(new Stock("IBM", "International Business Machines Corp.", "NYSE").ToJson())
            );

            return new StockServerStreamProducer(stockClient, stockQuoteClient, scheduler);
        }
    }
}
