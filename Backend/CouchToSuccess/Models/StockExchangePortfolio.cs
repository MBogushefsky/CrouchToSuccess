using System;
using System.Collections.Generic;

namespace CouchToSuccess.Models
{
    public class StockExchangePortfolio
    {
        public double CurrencyAmount { get; set; }
        public Dictionary<string, int> Stocks { get; set; }
        public Dictionary<string, double> StockPrices { get; set; }
        public DateTime Timestamp { get; set; }
    }
}