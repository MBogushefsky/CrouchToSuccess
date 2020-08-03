using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frugal.ThirdParty.Models
{
    public class Intraday
    {
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double? last { get; set; }
        public double close { get; set; }
        public double? volume { get; set; }
        public string date { get; set; }
        public string symbol { get; set; }
        public string exchange { get; set; }
    }
}