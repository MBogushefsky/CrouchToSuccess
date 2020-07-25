using FluentNHibernate.Mapping;
using System;

namespace Monier.DBModels
{
    public partial class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            Table("transaction");
            LazyLoad();
            Id(x => x.ID).GeneratedBy.Assigned().Column("ID").Unique();
            Map(x => x.PlaidTransactionID).Column("PlaidTransactionID").Not.Nullable().Unique();
            Map(x => x.UserID).Column("UserID").Not.Nullable();
            Map(x => x.PlaidAccountID).Column("PlaidAccountID").Not.Nullable();
            Map(x => x.MerchantName).Column("MerchantName").Nullable();
            Map(x => x.Name).Column("Name").Not.Nullable();
            Map(x => x.CostAmount).Column("CostAmount").Nullable();
            Map(x => x.Pending).Column("Pending").Not.Nullable();
            Map(x => x.PaymentChannel).Column("PaymentChannel").Not.Nullable();
            Map(x => x.Categories).Column("Categories").Nullable();
            Map(x => x.TransactionDate).Column("TransactionDate").Not.Nullable();
        }
    }
}