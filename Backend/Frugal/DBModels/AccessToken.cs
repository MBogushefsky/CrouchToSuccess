using System;

namespace Frugal.DBModels
{
    public partial class AccessToken
    {
        public virtual Guid ID { get; set; }
        public virtual Guid UserID { get; set; }
        public virtual string Token { get; set; }
    }
}