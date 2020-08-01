using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frugal.ThirdParty.Plaid.Models
{
    public class BankAccount
    {
        public string account_id { get; set; }
        public BankAccountBalance balances { get; set; }
        public string mask { get; set; }
        public string name { get; set; }
        public string official_name { get; set; }
        public string subtype { get; set; }
        public string type { get; set; }
    }
}