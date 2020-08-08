using System;

namespace Frugal.Models
{
    public class StockExchangeChartSync
    {
        public Guid ID { get; set; }
        public string Symbol { get; set; }
        public string RangeFound { get; set; }
        public DateTime LastUpdatedTimestamp { get; set; }
    }
}