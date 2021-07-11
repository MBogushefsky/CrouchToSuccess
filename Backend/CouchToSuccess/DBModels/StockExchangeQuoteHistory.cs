using System;

namespace CouchToSuccess.DBModels
{
    public partial class StockExchangeQuoteHistory
    {
        public virtual Guid ID { get; set; }
        public virtual string Symbol { get; set; }
        public virtual double Price { get; set; }
        public virtual DateTime Timestamp { get; set; }
    }
}