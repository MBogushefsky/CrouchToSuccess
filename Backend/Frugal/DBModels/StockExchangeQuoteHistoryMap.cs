using FluentNHibernate.Mapping;
using System;

namespace Frugal.DBModels
{
    public partial class StockExchangeQuoteHistoryMap : ClassMap<StockExchangeQuoteHistory>
    {
        public StockExchangeQuoteHistoryMap()
        {
            Table("stock_exchange_quote_history");
            LazyLoad();
            Id(x => x.ID).GeneratedBy.Assigned().Column("ID").Unique();
            Map(x => x.Symbol).Column("Symbol").Not.Nullable();
            Map(x => x.Price).Column("Price").Not.Nullable();
            Map(x => x.Timestamp).Column("Timestamp").Not.Nullable();
        }
    }
}