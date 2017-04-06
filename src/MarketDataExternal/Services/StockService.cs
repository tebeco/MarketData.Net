using MarketDataCommon.Dto;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MarketDataExternal.Services
{
    public interface IStockService
    {
        bool TryGetFromCode(string code, out Stock stock);
        IEnumerable<Stock> GetAllStock();
    }

    public class StockService : IStockService
    {
        private readonly Dictionary<string, Stock> _stocks = new Dictionary<string, Stock>();

        public StockService()
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

        public bool TryGetFromCode(string code, out Stock stock)
        {
            var found = _stocks.TryGetValue(code, out var cachedStock);
            stock = cachedStock;

            return found;
        }

        public IEnumerable<Stock> GetAllStock()
        {
            return _stocks.Values.ToArray();
        }
    }
}
