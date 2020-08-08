using System;

namespace Frugal.DBModels
{
    public partial class StockExchangeChartSync
    {
        public virtual Guid ID { get; set; }
        public virtual string Symbol { get; set; }
        public virtual string RangeFound { get; set; }
        public virtual DateTime LastUpdatedTimestamp { get; set; }
    }
}