using System;

namespace CouchToSuccess.Models
{
    public class StockExchangeTransaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public double? CurrencyAmount { get; set; }
        public int? StockAmount { get; set; }
        public double? StockRate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}