using Frugal.ThirdParty.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Frugal.ThirdParty.MarketStackClient
{
    public class MarketStackClient
    {
        private static string apiUrl = "https://cloud.iexapis.com/stable";
        private static string apiKey;
        private static RestClient restClient;
        
        public MarketStackClient()
        {
            apiKey = WebConfigurationManager.AppSettings["IEXCloudApiKey"];
            restClient = new RestClient(apiUrl);
        }

        /*public Dictionary<DateTime, double> GetEODBySymbol(string symbol)
        {
            Dictionary<DateTime, double> resultDictionary = new Dictionary<DateTime, double>();
            var request = new RestRequest("/eod?access_key=" + apiKey + "&symbols=" + symbol, Method.GET);
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(restClient.Execute(request).Content);
            List<EOD> eods = responseBodyJsonObj["data"].ToObject<List<EOD>>();
            foreach (var eod in eods)
            {
                resultDictionary.Add(DateTime.Parse(eod.date), eod.close);
            }
            return resultDictionary;
        }*/

        public JObject GetSymbolQuote(string symbol)
        {
            var request = new RestRequest("/stock/" + symbol + "/quote?token=" + apiKey, Method.GET);
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(restClient.Execute(request).Content);
            return responseBodyJsonObj;
        }

        public List<PriceInstance> GetIntradayRaw(string symbol)
        {
            var request = new RestRequest("/stock/" + symbol + "/intraday-prices?token=" + apiKey, Method.GET);
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JArray>(restClient.Execute(request).Content);
            List<PriceInstance> intradays = responseBodyJsonObj.ToObject<List<PriceInstance>>();

            return intradays;
        }

        public List<PriceInstance> GetHistoricalDataRaw(string symbol, string range)
        {
            var request = new RestRequest("/stock/" + symbol + "/chart/" + range + "?token=" + apiKey, Method.GET);
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JArray>(restClient.Execute(request).Content);
            List<PriceInstance> intradays = responseBodyJsonObj.ToObject<List<PriceInstance>>();
            return intradays;
        }

        public Dictionary<DateTime, double> GetIntradayBySymbol(string symbol)
        {
            Dictionary<DateTime, double> resultDictionary = new Dictionary<DateTime, double>();
            List<PriceInstance> intradays = GetIntradayBySymbolRaw(symbol);
            double previousKnownValue = 0.00;
            foreach (var intraday in intradays)
            {
                resultDictionary.Add(DateTime.Parse(intraday.date), intraday.close == null ? previousKnownValue : (double) intraday.close);
            }
            return resultDictionary;
        }

        public double GetCurrentPriceBySymbol(string symbol)
        {
            var request = new RestRequest("/stock/" + symbol + "/price?token=" + apiKey, Method.GET);
            var responseBodyJsonObj = JsonConvert.DeserializeObject<double>(restClient.Execute(request).Content);
            return responseBodyJsonObj;
        }

        public static List<PriceInstance> GetIntradayBySymbolRaw(string symbol)
        {
            var request = new RestRequest("/intraday?access_key=" + apiKey + "&symbols=" + symbol, Method.GET);
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(restClient.Execute(request).Content);
            List<PriceInstance> intradays = responseBodyJsonObj["data"].ToObject<List<PriceInstance>>();
            return intradays;
        }
    }
}