using MarketData.Common.Dto;
using System;
using System.Collections.Generic;

namespace MarketData.Backends.StaticData
{
    public class StaticDataProvider
    {
        private readonly Dictionary<string, Stock> _stocks = new Dictionary<string, Stock>();

        public StaticDataProvider()
        {
            _stocks.Add("GOOGL", new Stock("GOOGL", "Alphabet Inc", "NASDAQ"));
            _stocks.Add("IBM", new Stock("IBM", "International Business Machines Corp.", "NYSE"));
            _stocks.Add("AAPL", new Stock("AAPL", "Apple Inc.", "NASDAQ"));
            _stocks.Add("HPQ", new Stock("HPQ", "HP Inc", "NYSE"));
            _stocks.Add("MSFT", new Stock("MSFT", "Microsoft Corporation", "NASDAQ"));
            _stocks.Add("CRM", new Stock("CRM", "salesforce.com, inc.", "NYSE"));
            _stocks.Add("ADBE", new Stock("ADBE", "Adobe Systems Incorporated", "NASDAQ"));
            _stocks.Add("ORCL", new Stock("ORCL", "Oracle Corporation", "NYSE"));
        }

        public Stock GetStock(string queryCode)
        {
            if (queryCode == null || !_stocks.TryGetValue(queryCode, out var stock))
            {
                throw new Exception("Either no code in the URL or no match for the provided code");
            }
            else
            {
                return stock;
            }
        }
    }
}
