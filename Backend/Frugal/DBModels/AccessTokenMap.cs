using FluentNHibernate.Mapping;
using System;

namespace Frugal.DBModels
{
    public partial class AccessTokenMap : ClassMap<AccessToken>
    {
        public AccessTokenMap()
        {
            Table("access_token");
            LazyLoad();
            Id(x => x.ID).GeneratedBy.Assigned().Column("ID").Unique();
            Map(x => x.UserID).Column("UserID").Not.Nullable();
            Map(x => x.Token).Column("Token").Not.Nullable().Unique();
        }
    }
}