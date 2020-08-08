using FluentNHibernate.Mapping;
using System;

namespace Frugal.DBModels
{
    public partial class StockExchangeChartSyncMap : ClassMap<StockExchangeChartSync>
    {
        public StockExchangeChartSyncMap()
        {
            Table("stock_exchange_chart_sync");
            LazyLoad();
            Id(x => x.ID).GeneratedBy.Assigned().Column("ID").Unique();
            Map(x => x.Symbol).Column("Symbol").Not.Nullable();
            Map(x => x.RangeFound).Column("RangeFound").Not.Nullable();
            Map(x => x.LastUpdatedTimestamp).Column("LastUpdatedTimestamp").Not.Nullable();
        }
    }
}