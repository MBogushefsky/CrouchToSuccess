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
        private static string apiUrl = "http://api.marketstack.com/v1";
        private static string apiKey;
        private static RestClient restClient;
        
        public MarketStackClient()
        {
            apiKey = WebConfigurationManager.AppSettings["MarketStackApiKey"];
            restClient = new RestClient(apiUrl);
        }

        public Dictionary<DateTime, double> GetEODBySymbol(string symbol)
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
        }
    }
}