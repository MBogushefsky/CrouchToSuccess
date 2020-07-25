using System;

namespace Monier.DBModels
{
    public partial class User
    {
        public virtual Guid ID { get; set; }
        public virtual string Username { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string Email { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool Admin { get; set; }
    }
}