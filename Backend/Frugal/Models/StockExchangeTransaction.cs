using System;

namespace Frugal.Models
{
    public class StockExchangeTransaction
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public double? CurrencyAmount { get; set; }
        public int? StockAmount { get; set; }
    }
}