using FluentAssertions;
using MarketDataCommon.Dto;
using Xunit;

namespace MarketDataCommon.Tests.Dto
{
    public class QuoteTests
    {
        private const string MsftQuoteJson = "{\"Code\":\"MSFT\",\"QuoteValue\":1.1}";
        private static readonly Quote MsftQuote = new Quote("MSFT", 1.1);

        [Fact]
        public void Should_return_quote_as_json_When_ToJson()
        {
            //Given
            var msftQuote = MsftQuote;

            //When
            var json = msftQuote.ToJson();
            var toString = msftQuote.ToString();

            //Then
            json.Should().BeEquivalentTo(MsftQuoteJson);
            toString.Should().BeEquivalentTo(MsftQuoteJson);
        }

        [Fact]
        public void Should_return_stock_as_object_When_FromJson()
        {
            //Given
            var msftJson = MsftQuoteJson;

            //When
            var msftStock = Quote.FromJson(msftJson);

            //Then
            msftStock.ShouldBeEquivalentTo(MsftQuote);
        }
    }
}
