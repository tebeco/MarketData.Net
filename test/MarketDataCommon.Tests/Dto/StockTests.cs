using FluentAssertions;
using MarketDataCommon.Dto;
using Xunit;

namespace MarketDataCommon.Tests.Dto
{
    public class StockTests
    {
        private const string MsftJson = "{\"Code\":\"MSFT\",\"CompanyName\":\"Microsoft Corporation\",\"Market\":\"NASDAQ\"}";
        private static readonly Stock MsftStock = new Stock("MSFT", "Microsoft Corporation", "NASDAQ");

        [Fact]
        public void Should_return_stock_as_json_When_ToJson()
        {
            //Given
            var msftStock = new Stock("MSFT", "Microsoft Corporation", "NASDAQ");

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
            var msftStock = Stock.FromJson(msftJson);

            //Then
            msftStock.ShouldBeEquivalentTo(MsftStock);
        }
    }
}
