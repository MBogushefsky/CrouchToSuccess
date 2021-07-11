using System;

namespace CouchToSuccess.DBModels
{
    public partial class StockExchangeChart
    {
        public virtual Guid ID { get; set; }
        public virtual string Symbol { get; set; }
        public virtual DateTime Timestamp { get; set; }
        public virtual string RangeFound { get; set; }
        public virtual double? Open { get; set; }
        public virtual double? Close { get; set; }
        public virtual double? High { get; set; }
        public virtual double? Low { get; set; }
        public virtual long Volume { get; set; }
    }
}