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

        public List<Intraday> GetIntradayRaw(string symbol, string interval, string dateFrom, string dateTo, int? limit)
        {
            string endpointUrl = "/intraday?access_key=" + apiKey + "&symbols=" + symbol + "&interval=" + interval;
            if (limit != null)
            {
                endpointUrl += "&limit=" + limit;
            }
            if (dateFrom != null)
            {
                endpointUrl += "&date_from=" + dateFrom;
            }
            if (dateTo != null)
            {
                endpointUrl += "&date_to=" + dateTo;
            }
            var request = new RestRequest(endpointUrl, Method.GET);
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(restClient.Execute(request).Content);
            List<Intraday> intradays = responseBodyJsonObj["data"].ToObject<List<Intraday>>();
            return intradays;
        }

        public Dictionary<DateTime, double> GetIntradayBySymbol(string symbol)
        {
            Dictionary<DateTime, double> resultDictionary = new Dictionary<DateTime, double>();
            List<Intraday> intradays = GetIntradayBySymbolRaw(symbol);
            foreach (var intraday in intradays)
            {
                resultDictionary.Add(DateTime.Parse(intraday.date), intraday.close);
            }
            return resultDictionary;
        }

        public double GetCurrentPriceBySymbol(string symbol)
        {
            var request = new RestRequest("/intraday/latest?access_key=" + apiKey + "&symbols=" + symbol, Method.GET);
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(restClient.Execute(request).Content);
            List<Intraday> intradays = responseBodyJsonObj["data"].ToObject<List<Intraday>>();
            return intradays[0].close;
        }

        public static List<Intraday> GetIntradayBySymbolRaw(string symbol)
        {
            var request = new RestRequest("/intraday?access_key=" + apiKey + "&symbols=" + symbol, Method.GET);
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(restClient.Execute(request).Content);
            List<Intraday> intradays = responseBodyJsonObj["data"].ToObject<List<Intraday>>();
            return intradays;
        }
    }
}