using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using MarketDataCommon.Dto;
using Xunit;

namespace MarketDataCommon.Tests.Dto
{
    public class VwapTests
    {
        private const string MsftVwapJson = "{\"Code\":\"MSFT\",\"VwapValue\":1.1,\"Volume\":1000.1}";
        private static readonly Vwap MsftVwap = new Vwap("MSFT", 1.1, 1000.1);

        [Fact]
        public void Should_return_quote_as_json_When_ToJson()
        {
            //Given
            var msftQuote = MsftVwap;

            //When
            var json = msftQuote.ToJson();
            var toString = msftQuote.ToString();

            //Then
            json.Should().BeEquivalentTo(MsftVwapJson);
            toString.Should().BeEquivalentTo(MsftVwapJson);
        }

        [Fact]
        public void Should_return_stock_as_object_When_FromJson()
        {
            //Given
            var msftJson = MsftVwapJson;

            //When
            var msftStock = Vwap.FromJson(msftJson);

            //Then
            msftStock.ShouldBeEquivalentTo(MsftVwap);
        }

        [Fact]
        public void Should_match_trade_volume_and_nominal_When_only_one_trade()
        {
            //Given
            Vwap vwap = new Vwap("IBM", 0, 0);

            //When
            Vwap vwap2 = vwap.AddTrade(new Trade("IBM", 10, 420));

            //Then
            vwap2.Volume.Should().Be(10);
            vwap2.VwapValue.Should().Be(42.0d);
        }

        [Fact]
        public void should_compute_average_when_adding_a_trade()
        {
            //Given
            Vwap vwap = new Vwap("IBM", 700, 10);

            //When
            Vwap vwap2 = vwap.AddTrade(new Trade("IBM", 20, 15200));

            //Then
            vwap2.Volume.Should().Be(30);
            vwap2.VwapValue.Should().Be(740);
        }
    }
}
