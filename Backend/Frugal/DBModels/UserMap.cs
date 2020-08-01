using FluentNHibernate.Mapping;
using System;

namespace Frugal.DBModels
{
    public partial class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("user");
            LazyLoad();
            Id(x => x.ID).GeneratedBy.Assigned().Column("ID").Unique();
            Map(x => x.Username).Column("Username").Not.Nullable().Unique();
            Map(x => x.PasswordHash).Column("PasswordHash").Not.Nullable();
            Map(x => x.Email).Column("Email").Nullable().Unique();
            Map(x => x.FirstName).Column("FirstName").Not.Nullable();
            Map(x => x.LastName).Column("LastName").Not.Nullable();
            Map(x => x.PhoneNumber).Column("PhoneNumber").Nullable().Unique();
            Map(x => x.Admin).Column("Admin").Not.Nullable();
        }
    }
}