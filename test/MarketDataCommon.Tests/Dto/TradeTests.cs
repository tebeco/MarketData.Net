using FluentAssertions;
using MarketDataCommon.Dto;
using Xunit;

namespace MarketDataCommon.Tests.Dto
{
    public class TradeTests
    {
        private const string MsftJson = "{\"Code\":\"MSFT\",\"Quantity\":1,\"Nominal\":1.2}";
        private static readonly Trade MsftTrade = new Trade("MSFT", 1, 1.2);

        [Fact]
        public void Should_return_stock_as_json_When_ToJson()
        {
            //Given
            var msftStock = new Trade("MSFT", 1, 1.2);

            //When
            var json = msftStock.ToJson();
            var toString = msftStock.ToString();

            //Then
            json.Should().BeEquivalentTo(MsftJson);
            toString.Should().BeEquivalentTo(MsftJson);
        }

        [Fact]
        public void Should_return_stock_as_object_When_FromJson()
        {
            //Given
            var msftJson = MsftJson;

            //When
            var msftStock = Trade.FromJson(msftJson);

            //Then
            msftStock.ShouldBeEquivalentTo(MsftTrade);
        }
    }
}
