using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monier.Plaid.Models
{
    public class BankAccountBalance
    {
        public double available { get; set; }
        public double current { get; set; }
        public double? limit { get; set; }
    }
}