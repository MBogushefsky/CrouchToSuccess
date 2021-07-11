using FluentNHibernate.Mapping;
using System;

namespace CouchToSuccess.DBModels
{
    public partial class StockExchangeTransactionMap : ClassMap<StockExchangeTransaction>
    {
        public StockExchangeTransactionMap()
        {
            Table("stock_exchange_transaction");
            LazyLoad();
            Id(x => x.ID).GeneratedBy.Assigned().Column("ID").Unique();
            Map(x => x.UserID).Column("UserID").Not.Nullable();
            Map(x => x.Symbol).Column("Symbol").Nullable();
            Map(x => x.Type).Column("Type").Not.Nullable();
            Map(x => x.CurrencyAmount).Column("CurrencyAmount").Nullable();
            Map(x => x.StockAmount).Column("StockAmount").Nullable();
            Map(x => x.StockRate).Column("StockRate").Nullable();
            Map(x => x.CreatedDate).Column("CreatedDate").Not.Nullable();
        }
    }
}