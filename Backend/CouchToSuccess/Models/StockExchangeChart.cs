using System;

namespace CouchToSuccess.Models
{
    public class StockExchangeChart
    {
        public Guid ID { get; set; }
        public string Symbol { get; set; }
        public DateTime Timestamp { get; set; }
        public string RangeFound { get; set; }
        public double? Open { get; set; }
        public double? Close { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public long Volume { get; set; }
    }
}