using System;

namespace CouchToSuccess.Models
{
    public class StockExchangeChartSync
    {
        public Guid ID { get; set; }
        public string Symbol { get; set; }
        public string RangeFound { get; set; }
        public DateTime LastUpdatedTimestamp { get; set; }
    }
}