using System;

namespace Monier.Models
{
    public class BankAccount
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PlaidAccountId { get; set; }
        public string InstitutionId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Mask { get; set; }
        public double AvailableBalance { get; set; }
        public double CurrentBalance { get; set; }
        public double? LimitBalance { get; set; }
    }
}