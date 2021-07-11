using System;

namespace CouchToSuccess.DBModels
{
    public partial class StockExchangeTransaction
    {
        public virtual Guid ID { get; set; }
        public virtual Guid UserID { get; set; }
        public virtual string Symbol { get; set; }
        public virtual string Type { get; set; }
        public virtual double? CurrencyAmount { get; set; }
        public virtual int? StockAmount { get; set; }
        public virtual double? StockRate { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}