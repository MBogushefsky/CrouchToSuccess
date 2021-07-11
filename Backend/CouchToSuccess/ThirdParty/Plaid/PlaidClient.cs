using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace CouchToSuccess.ThirdParty.Plaid
{
    public class PlaidClient
    {
        private static readonly string apiUrl = "https://sandbox.plaid.com";
        private static string clientId;
        private static string secret;
        private static RestClient restClient;
        
        public PlaidClient()
        {
            clientId = WebConfigurationManager.AppSettings["PlaidClientID"];
            secret = WebConfigurationManager.AppSettings["PlaidSecret"];
            restClient = new RestClient(apiUrl);
        }

        public List<Plaid.Models.BankAccount> GetInstitutionBankAccounts(string accessToken, out string institutionId)
        {
            var request = new RestRequest("/accounts/get", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new
            {
                client_id = clientId,
                secret = secret,
                access_token = accessToken
            });
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(restClient.Execute(request).Content);
            institutionId = responseBodyJsonObj["item"]["institution_id"].ToString();
            return responseBodyJsonObj["accounts"].ToObject<List<Plaid.Models.BankAccount>>();
        }

        public JArray GetInstitutionBankAccountTransactions(string accessToken, string accountId, DateTime startDate, DateTime endDate)
        {
            var request = new RestRequest("/transactions/get", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new
            {
                client_id = clientId,
                secret = secret,
                access_token = accessToken,
                start_date = startDate.ToString("yyyy-MM-dd"),
                end_date = endDate.ToString("yyyy-MM-dd"),
                options = new {
                    account_ids = new[] { accountId },
                    count = 500
                }
            });
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(restClient.Execute(request).Content);
            return responseBodyJsonObj["transactions"].ToObject<JArray>();
        }

        public string ExchangePublicTokenForAccessToken(string publicToken)
        {
            var request = new RestRequest("/item/public_token/exchange", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new
            {
                client_id = clientId,
                secret = secret,
                public_token = publicToken
            });
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(restClient.Execute(request).Content);
            return responseBodyJsonObj["access_token"].ToObject<string>();
        }
    }
}