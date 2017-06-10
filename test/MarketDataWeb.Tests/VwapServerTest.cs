using MMarketDataVwapServer;
using MarketDataCommon.Dto;
using MarketDataCommon.Infratructure.WebSocket;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MarketDataWeb.Tests
{
    public class VwapServerTest
    {
        public (Subject<string>, IProduceObservable<string>, TestScheduler, VwapServerMiddleware) Setup()
        {
            var tradeSourceSubject = new Subject<string>();
            var tradeEventStream = Substitute.For<IProduceObservable<string>>();
            tradeEventStream.GetStream().Returns(tradeSourceSubject);

            var scheduler = new TestScheduler();
            var vwapServer = new VwapServerMiddleware(tradeEventStream, scheduler);

            return (tradeSourceSubject, tradeEventStream, scheduler, vwapServer);
        }

        /**
        * Test 10
        */
        [Fact(Skip = "TEST TO VALIDATE")]
        public async Task should_generate_one_google_vwap_event_when_a_google_trade_is_done()
        {
            // given
            var (testSubject, tradeEventStream, scheduler, vwapServer) = Setup();
            List<Vwap> events = new List<Vwap>();
            var testSubscriber = new Subject<Vwap>();
            var vwapStream = await vwapServer.GetStream("GOOGL");
            vwapStream.Subscribe(@event => events.Add(@event));

            // when
            testSubject.OnNext(new Trade("GOOGL", 10, 7058.673).ToJson());
            testSubject.OnNext(new Trade("APPLE", 10, 981.8).ToJson());
            scheduler.AdvanceBy(1000);

            // then
            Assert.True(events.Count == 1);
            Vwap vwap = events.First();
            Assert.True(string.Equals(vwap.Code, "GOOGL"));
            Assert.True(Comparer<Double>.Default.Compare(vwap.VwapValue, 705.8673) == 0);
            Assert.True(vwap.Volume == 10);
        }

        /**
        * Test 11
        */
        [Fact(Skip = "TEST TO VALIDATE")]
        public async Task should_add_all_google_trades_to_generate_vwap_events()
        {
            // given
            var (testSubject, tradeEventStream, scheduler, vwapServer) = Setup();
            List<Vwap> events = new List<Vwap>();
            var testSubscriber = new Subject<Vwap>();
            var vwapStream = await vwapServer.GetStream("GOOGL");
            vwapStream.Subscribe(@event => events.Add(@event));

            // when
            testSubject.OnNext(new Trade("GOOGL", 10, 7058).ToJson());
            testSubject.OnNext(new Trade("GOOGL", 10, 7062).ToJson());
            scheduler.AdvanceBy(1000);

            // then
            Assert.True(events.Any());
            Vwap vwap = events.Last();
            Assert.True(string.Equals(vwap.Code, "GOOGL"));
            Assert.True(Comparer<Double>.Default.Compare(vwap.VwapValue, 706) == 0);
            Assert.True(vwap.Volume == 20);
        }

        /**
        * Test 12
        */
        [Fact(Skip = "TEST TO VALIDATE")]
        public async Task should_generate_at_most_one_event_per_sec()
        {
            //http://www.introtorx.com/content/v1.0.10621.0/16_TestingRx.html

            // given
            var (testSubject, tradeEventStream, scheduler, vwapServer) = Setup();
            List<Vwap> events = new List<Vwap>();
            var testSubscriber = new Subject<Vwap>();
            var vwapStream = await vwapServer.GetStream("GOOGL"); //<=== INJECT SUBJECT IN HERE 
            vwapStream.Subscribe(@event => events.Add(@event));

            // when
            testSubject.OnNext(new Trade("GOOGL", 10, 7000).ToJson());
            testSubject.OnNext(new Trade("GOOGL", 10, 7000).ToJson());
            testSubject.OnNext(new Trade("GOOGL", 10, 7000).ToJson());
            testSubject.OnNext(new Trade("GOOGL", 10, 7000).ToJson());
            testSubject.OnNext(new Trade("GOOGL", 10, 7000).ToJson());
            testSubject.OnNext(new Trade("GOOGL", 10, 7000).ToJson());
            testSubject.OnNext(new Trade("GOOGL", 10, 7000).ToJson());
            testSubject.OnNext(new Trade("GOOGL", 10, 7000).ToJson());
            testSubject.OnNext(new Trade("GOOGL", 10, 7000).ToJson());
            testSubject.OnNext(new Trade("GOOGL", 10, 7000).ToJson());
            testSubject.OnNext(new Trade("GOOGL", 10, 7000).ToJson());
            testSubject.OnCompleted();

            scheduler.AdvanceBy(2000);

            // then
            Assert.True(events.Count == 1);
            Vwap vwap = events.First();
            Assert.True(string.Equals(vwap.Code, "GOOGL"));
            Assert.True(Comparer<Double>.Default.Compare(vwap.VwapValue, 700) == 0);
            Assert.True(vwap.Volume == 100);
        }

    }
}
