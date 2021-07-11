using System;

namespace CouchToSuccess.DBModels
{
    public partial class BankAccount
    {
        public virtual Guid ID { get; set; }
        public virtual Guid UserID { get; set; }
        public virtual string PlaidAccountID { get; set; }
        public virtual string InstitutionID { get; set; }
        public virtual string Name { get; set; }
        public virtual string FullName { get; set; }
        public virtual string Type { get; set; }
        public virtual string SubType { get; set; }
        public virtual string Mask { get; set; }
        public virtual double? AvailableBalance { get; set; }
        public virtual double? CurrentBalance { get; set; }
        public virtual double? LimitBalance { get; set; }
    }
}