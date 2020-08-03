using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frugal.ThirdParty.Models
{
    public class EOD
    {
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public double volume { get; set; }
        public double adj_high { get; set; }
        public double adj_low { get; set; }
        public double adj_close { get; set; }
        public double adj_open { get; set; }
        public double adj_volume { get; set; }
        public string symbol { get; set; }
        public string exchange { get; set; }
        public string date { get; set; }
    }
}