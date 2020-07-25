using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monier.DBModels
{
    public class Transaction
    {
        public virtual Guid ID { get; set; }
        public virtual string PlaidTransactionID { get; set; }
        public virtual Guid UserID { get; set; }
        public virtual string PlaidAccountID { get; set; }
        public virtual string MerchantName { get; set; }
        public virtual string Name { get; set; }
        public virtual double CostAmount { get; set; }
        public virtual bool Pending { get; set; }
        public virtual string PaymentChannel { get; set; }
        public virtual string Categories { get; set; }
        public virtual DateTime TransactionDate { get; set; }
    }
}