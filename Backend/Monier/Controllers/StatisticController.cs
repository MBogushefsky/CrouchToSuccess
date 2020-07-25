using Monier.DBModels;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Monier.Controllers
{
    [RoutePrefix("statistics")]
    public class StatisticController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IDictionary<string, object> GetStatistics()
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                IEnumerable<Models.BankAccount> bankAccounts = BankAccountController.GetBankAccountsBySession(session, currentUser.Id);
                IEnumerable<Models.Transaction> transactionsPast30DaysToAnalyze = TransactionController.GetTransactionsBySession(session, currentUser.Id);
                Dictionary<string, object> resultStatistics = new Dictionary<string, object>();
                resultStatistics.Add("LargestCostAmountInPast30Days", GetLargestCostAmountInTransactions(transactionsPast30DaysToAnalyze));
                resultStatistics.Add("CurrentNetWorth", GetCurrentNetWorth(bankAccounts));
                resultStatistics.Add("DailyCostInPast30Days", DailyCostInPast30Days(transactionsPast30DaysToAnalyze));
                return resultStatistics;
            }
        }

        private static Models.Transaction GetLargestCostAmountInTransactions(IEnumerable<Models.Transaction> transactions)
        {
            Models.Transaction largestTransaction = null;
            double largestCostAmount = 0;
            foreach (var transaction in transactions)
            {
                if (transaction.CostAmount > largestCostAmount)
                {
                    largestTransaction = transaction;
                    largestCostAmount = transaction.CostAmount;
                }
            }
            return largestTransaction;
        }

        private static string GetCurrentNetWorth(IEnumerable<Models.BankAccount> bankAccounts)
        {
            double resultNetWorth = 0;
            foreach (var bankAccount in bankAccounts)
            {
                if (bankAccount.Type != "credit")
                {
                    resultNetWorth += bankAccount.CurrentBalance;
                }
            }
            return resultNetWorth.ToString("C");
        }

        // NOT DONE
        private static IEnumerable<KeyValuePair<string, double>> DailyCostInPast30Days(IEnumerable<Models.Transaction> transactionsPast30Days)
        {
            List<KeyValuePair<string, double>> resultPair = new List<KeyValuePair<string, double>>();
            DateTime currentDate = DateTime.Now;
            DateTime dateToCheck = DateTime.Now.AddDays(-30);
            while (dateToCheck.Year != currentDate.Year || dateToCheck.Month != currentDate.Month || dateToCheck.Day != currentDate.Day)
            {
                double resultValue = 0;
                foreach (var transaction in transactionsPast30Days)
                {
                    if (transaction.CostAmount > 0 && dateToCheck.Year == transaction.TransactionDate.Year && dateToCheck.Month == transaction.TransactionDate.Month && dateToCheck.Day == transaction.TransactionDate.Day)
                    {
                        resultValue += transaction.CostAmount;
                    }
                }
                resultPair.Add(new KeyValuePair<string, double>(dateToCheck.ToString("MM/dd"), Math.Truncate(100 * resultValue) / 100));
                dateToCheck = dateToCheck.AddDays(1);
            }
            return resultPair;
        }
    }
}
