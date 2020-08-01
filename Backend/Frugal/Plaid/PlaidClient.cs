using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Frugal.Plaid
{
    public class PlaidClient
    {
        private static readonly string apiUrl = "https://sandbox.plaid.com";
        private static string plaidClientId;
        private static string plaidSecret;
        private static RestClient plaidClient;
        
        public PlaidClient()
        {
            plaidClientId = WebConfigurationManager.AppSettings["PlaidClientID"];
            plaidSecret = WebConfigurationManager.AppSettings["PlaidSecret"];
            plaidClient = new RestClient(apiUrl);
        }

        public List<Plaid.Models.BankAccount> GetInstitutionBankAccounts(string accessToken, out string institutionId)
        {
            var request = new RestRequest("/accounts/get", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new
            {
                client_id = plaidClientId,
                secret = plaidSecret,
                access_token = accessToken
            });
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(plaidClient.Execute(request).Content);
            institutionId = responseBodyJsonObj["item"]["institution_id"].ToString();
            return responseBodyJsonObj["accounts"].ToObject<List<Plaid.Models.BankAccount>>();
        }

        public JArray GetInstitutionBankAccountTransactions(string accessToken, string accountId, DateTime startDate, DateTime endDate)
        {
            var request = new RestRequest("/transactions/get", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new
            {
                client_id = plaidClientId,
                secret = plaidSecret,
                access_token = accessToken,
                start_date = startDate.ToString("yyyy-MM-dd"),
                end_date = endDate.ToString("yyyy-MM-dd"),
                options = new {
                    account_ids = new[] { accountId },
                    count = 500
                }
            });
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(plaidClient.Execute(request).Content);
            return responseBodyJsonObj["transactions"].ToObject<JArray>();
        }

        public string ExchangePublicTokenForAccessToken(string publicToken)
        {
            var request = new RestRequest("/item/public_token/exchange", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new
            {
                client_id = plaidClientId,
                secret = plaidSecret,
                public_token = publicToken
            });
            var responseBodyJsonObj = JsonConvert.DeserializeObject<JObject>(plaidClient.Execute(request).Content);
            return responseBodyJsonObj["access_token"].ToObject<string>();
        }
    }
}