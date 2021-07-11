using FluentNHibernate.Mapping;
using System;

namespace CouchToSuccess.DBModels
{
    public partial class BankAccountMap : ClassMap<BankAccount>
    {
        public BankAccountMap()
        {
            Table("bank_account");
            LazyLoad();
            Id(x => x.ID).GeneratedBy.Assigned().Column("ID").Unique();
            Map(x => x.UserID).Column("UserID").Not.Nullable();
            Map(x => x.PlaidAccountID).Column("PlaidAccountID").Not.Nullable().Unique();
            Map(x => x.InstitutionID).Column("InstitutionID").Not.Nullable();
            Map(x => x.Name).Column("Name").Not.Nullable();
            Map(x => x.FullName).Column("FullName").Nullable();
            Map(x => x.Type).Column("Type").Not.Nullable();
            Map(x => x.SubType).Column("SubType").Not.Nullable();
            Map(x => x.Mask).Column("Mask").Not.Nullable();
            Map(x => x.AvailableBalance).Column("AvailableBalance").Nullable();
            Map(x => x.CurrentBalance).Column("CurrentBalance").Nullable();
            Map(x => x.LimitBalance).Column("LimitBalance").Nullable();
        }
    }
}