using System;

namespace Monier.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string PlaidTransactionId { get; set; }
        public Guid UserId { get; set; }
        public string PlaidAccountId { get; set; }
        public string MerchantName { get; set; }
        public string Name { get; set; }
        public double CostAmount { get; set; }
        public bool Pending { get; set; }
        public string PaymentChannel { get; set; }
        public string[] Categories { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}