using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frugal.ThirdParty.Models
{
    public class PriceInstance
    {
        public string date { get; set; }
        public string minute { get; set; }
        public string label { get; set; }
        public double? marketOpen { get; set; }
        public double? marketClose { get; set; }
        public double? marketHigh { get; set; }
        public double? marketLow { get; set; }
        public double? marketAverage { get; set; }
        public int marketVolume { get; set; }
        public double marketNotional { get; set; }
        public int marketNumberOfTrades { get; set; }
        public double? marketChangeOverTime { get; set; }
        public double? high { get; set; }
        public double? low { get; set; }
        public double? open { get; set; }
        public double? close { get; set; }
        public double? average { get; set; }
        public int volume { get; set; }
        public double notional { get; set; }
        public int numberOfTrades { get; set; }
        public double? changeOverTime { get; set; }
    }
}