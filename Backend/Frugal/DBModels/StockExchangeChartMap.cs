using FluentNHibernate.Mapping;
using System;

namespace Frugal.DBModels
{
    public partial class StockExchangeChartMap : ClassMap<StockExchangeChart>
    {
        public StockExchangeChartMap()
        {
            Table("stock_exchange_chart");
            LazyLoad();
            Id(x => x.ID).GeneratedBy.Assigned().Column("ID").Unique();
            Map(x => x.Symbol).Column("Symbol").Not.Nullable();
            Map(x => x.Timestamp).Column("Timestamp").Not.Nullable();
            Map(x => x.RangeFound).Column("RangeFound").Not.Nullable();
            Map(x => x.Open).Column("Open").Nullable();
            Map(x => x.Close).Column("Close").Nullable();
            Map(x => x.High).Column("High").Nullable();
            Map(x => x.Low).Column("Low").Not.Nullable();
            Map(x => x.Volume).Column("Volume").Not.Nullable();
        }
    }
}