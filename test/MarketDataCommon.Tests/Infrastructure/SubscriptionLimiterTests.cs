//using System;
//using System.Collections.Generic;
//using System.Reactive.Subjects;
//using System.Text;
//using MarketDataCommon.Infrastructure;
//using Microsoft.Reactive.Testing;
//using Xunit;

//namespace MarketDataCommon.Tests.Infrastructure
//{
//    public class SubscriptionLimiterTests
//    {
//        [Fact]

//        public void should_allow_one_subscription()
//        {
//            // given
//            var scheduler = new TestScheduler();
//            var subject = new Subject<int>();
//            IObservable<int> limitedObservable = SubscriptionLimiter.LimitSubscriptions(1, subject, scheduler);

//            //When
//            scheduler.Start()
//            limitedObservable.Subscribe(subscriber);
//            subject.OnNext(123);
//            // then

//            subscriber.
//            assertThat(subscriber.getOnNextEvents()).hasSize(1).contains(123);
//        }
//    }
//}
