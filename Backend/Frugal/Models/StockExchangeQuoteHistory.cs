using System;

namespace Frugal.Models
{
    public class StockExchangeQuoteHistory
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}