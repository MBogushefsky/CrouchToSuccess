// Credits to https://social.technet.microsoft.com/wiki/contents/articles/26805.c-calculating-percentage-similarity-of-2-strings.aspx
using Monier.DBModels;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
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
                IEnumerable<Models.Transaction> transactionsToAnalyze = TransactionController.GetTransactionsBySession(session, currentUser.Id);
                Dictionary<string, object> resultStatistics = new Dictionary<string, object>();
                //resultStatistics.Add("LargestCostAmountInPast30Days", GetLargestCostAmountInTransactions(transactionsToAnalyze));
                resultStatistics.Add("CurrentNetWorth", GetCurrentNetWorth(bankAccounts));
                resultStatistics.Add("NetWorthInPast30Days", NetWorthInPast30Days(transactionsToAnalyze, GetCurrentNetWorth(bankAccounts), DailyChangeInPast30Days(transactionsToAnalyze)));
                resultStatistics.Add("DailyCostInPast30Days", DailyCostInPast30Days(transactionsToAnalyze));
                resultStatistics.Add("TransactionsInPast30DaysPie", TransactionsInPast30DaysPie(transactionsToAnalyze));
                resultStatistics.Add("TypeOfPaymentsInPast30DaysPie", TypeOfPaymentsInPast30DaysPie(GetLast30DaysOfTransactionsFromAll(transactionsToAnalyze)));
                resultStatistics.Add("CurrentMonthlySubscriptions", CurrentMonthlySubscriptions(transactionsToAnalyze));
                return resultStatistics;
            }
        }

        private static IEnumerable<Models.Transaction> GetLast30DaysOfTransactionsFromAll(IEnumerable<Models.Transaction> transactions)
        {
            List<Models.Transaction> resultTransactions = new List<Models.Transaction>();
            DateTime currentDate = DateTime.Now;
            DateTime dateToCheck = DateTime.Now.AddDays(-30);
            while (dateToCheck.Year != currentDate.Year || dateToCheck.Month != currentDate.Month || dateToCheck.Day != currentDate.Day)
            {
                foreach (var transaction in transactions)
                {
                    if (dateToCheck.Year == transaction.TransactionDate.Year && dateToCheck.Month == transaction.TransactionDate.Month && dateToCheck.Day == transaction.TransactionDate.Day)
                    {
                        resultTransactions.Add(transaction);
                    }
                }
                dateToCheck = dateToCheck.AddDays(1);
            }
            return resultTransactions;
        }

        // Deprecated
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

        private static IEnumerable<KeyValuePair<string, double>> NetWorthInPast30Days(IEnumerable<Models.Transaction> transactions, double netWorthFrom30DaysAgo, IEnumerable<KeyValuePair<DateTime, double>> dailyChangeInPast30Days)
        {
            List<KeyValuePair<string, double>> resultDictionary = new List<KeyValuePair<string, double>>();
            double currentNetWorth = netWorthFrom30DaysAgo;
            foreach (KeyValuePair<DateTime, double> change in dailyChangeInPast30Days)
            {
                currentNetWorth -= change.Value;
            }
            foreach (KeyValuePair<DateTime, double> change in dailyChangeInPast30Days)
            {
                currentNetWorth += change.Value;
                resultDictionary.Add(new KeyValuePair<string, double>(change.Key.ToString("MM/dd"), Math.Truncate(100 * currentNetWorth) / 100));
            }
            return resultDictionary;
        }

        private static Dictionary<string, double> TypeOfPaymentsInPast30DaysPie(IEnumerable<Models.Transaction> last30DaysOfTransactions)
        {
            Dictionary<string, double> analyzeDictionary = new Dictionary<string, double>();
            foreach (Models.Transaction transaction in last30DaysOfTransactions)
            {
                if (transaction.CostAmount > 0)
                {
                    string transactionType = "Unknown";
                    if (transaction.PaymentChannel == "online")
                    {
                        transactionType = "Online";
                    }
                    else if (transaction.PaymentChannel == "other")
                    {
                        transactionType = "Other";
                    }
                    else if (transaction.PaymentChannel == "in store")
                    {
                        transactionType = "In Store";
                    }
                    else if (transaction.PaymentChannel == "in store")
                    {
                        transactionType = "In Store";
                    }
                    if (analyzeDictionary.ContainsKey(transactionType))
                    {
                        analyzeDictionary[transactionType] += transaction.CostAmount;
                    }
                    else
                    {
                        analyzeDictionary[transactionType] = transaction.CostAmount;
                    }
                }
            }
            Dictionary<string, double> resultDictionary = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> transactionType in analyzeDictionary)
            {
                resultDictionary[transactionType.Key] = GetTruncatedByTwoDecimals(transactionType.Value);
            }
            return resultDictionary;
        }

        private static Dictionary<string, double> CurrentMonthlySubscriptions(IEnumerable<Models.Transaction> transactions)
        {
            Dictionary<string, double> resultDictionary = new Dictionary<string, double>();
            Dictionary<string, List<Models.Transaction>> analyzeDictionary = new Dictionary<string, List<Models.Transaction>>();
            DateTime currentDate = DateTime.Now;
            DateTime dateToCheck = DateTime.Now.AddDays(-90);
            while (dateToCheck.Year != currentDate.Year || dateToCheck.Month != currentDate.Month || dateToCheck.Day != currentDate.Day)
            {
                foreach (var transaction in transactions)
                {
                    if (transaction.CostAmount > 0 && dateToCheck.Year == transaction.TransactionDate.Year && dateToCheck.Month == transaction.TransactionDate.Month && dateToCheck.Day == transaction.TransactionDate.Day)
                    {
                        string transactionName = GetNameOfTransaction(transaction);
                        Models.Transaction similarTransaction = IsSimilarTransactionByName(transaction, transactions);
                        if (similarTransaction != null && analyzeDictionary.ContainsKey(GetNameOfTransaction(similarTransaction)))
                        {
                            analyzeDictionary[GetNameOfTransaction(similarTransaction)].Add(transaction);
                        }
                        else
                        {
                            analyzeDictionary[transactionName] = new List<Models.Transaction>() { transaction };
                        }
                    }
                }
                dateToCheck = dateToCheck.AddDays(1);
            }
            foreach (KeyValuePair<string, List<Models.Transaction>> possibleSubscription in analyzeDictionary)
            {
                if (possibleSubscription.Value.Count == 3)
                {
                    if (IsAllTransactionPricesTheSame(possibleSubscription.Value) && IsAllTransactionDatesAroundTheSameTimeEachMonth(possibleSubscription.Value))
                    {
                        resultDictionary.Add(possibleSubscription.Key, possibleSubscription.Value[0].CostAmount);
                    }
                }
            }
            return resultDictionary;
        }

        private static bool IsAllTransactionDatesAroundTheSameTimeEachMonth(IEnumerable<Models.Transaction> transactions)
        {
            int dayOfTheMonth = -1;
            foreach (var transaction in transactions)
            {
                if (dayOfTheMonth == -1)
                {
                    dayOfTheMonth = transaction.TransactionDate.Day;
                }
                else if (dayOfTheMonth != transaction.TransactionDate.Day &&
                    dayOfTheMonth != (transaction.TransactionDate.Day - 2) &&
                    dayOfTheMonth != (transaction.TransactionDate.Day - 1) &&
                    dayOfTheMonth != (transaction.TransactionDate.Day + 1) &&
                    dayOfTheMonth != (transaction.TransactionDate.Day + 2))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsAllTransactionPricesTheSame(IEnumerable<Models.Transaction> transactions)
        {
            double priceToMatch = -1;
            foreach (var transaction in transactions)
            {
                if (priceToMatch == -1)
                {
                    priceToMatch = transaction.CostAmount;
                }
                else if (priceToMatch != transaction.CostAmount)
                {
                    return false;
                }
            }
            return true;
        }

        private static double GetCurrentNetWorth(IEnumerable<Models.BankAccount> bankAccounts)
        {
            double resultNetWorth = 0;
            foreach (var bankAccount in bankAccounts)
            {
                if (bankAccount.Type != "credit")
                {
                    resultNetWorth += bankAccount.CurrentBalance;
                }
                else
                {
                    resultNetWorth -= bankAccount.CurrentBalance;
                }
            }
            return Math.Truncate(100 * resultNetWorth) / 100;
        }

        private static IEnumerable<KeyValuePair<string, double>> DailyCostInPast30Days(IEnumerable<Models.Transaction> transactions)
        {
            List<KeyValuePair<string, double>> resultPair = new List<KeyValuePair<string, double>>();
            DateTime currentDate = DateTime.Now;
            DateTime dateToCheck = DateTime.Now.AddDays(-30);
            while (dateToCheck.Year != currentDate.Year || dateToCheck.Month != currentDate.Month || dateToCheck.Day != currentDate.Day)
            {
                double resultValue = 0;
                foreach (var transaction in transactions)
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

        private static IEnumerable<KeyValuePair<DateTime, double>> DailyChangeInPast30Days(IEnumerable<Models.Transaction> transactions)
        {
            List<KeyValuePair<DateTime, double>> resultPair = new List<KeyValuePair<DateTime, double>>();
            DateTime currentDate = DateTime.Now;
            DateTime dateToCheck = DateTime.Now.AddDays(-30);
            while (dateToCheck.Year != currentDate.Year || dateToCheck.Month != currentDate.Month || dateToCheck.Day != currentDate.Day)
            {
                double resultValue = 0;
                foreach (var transaction in transactions)
                {
                    if (dateToCheck.Year == transaction.TransactionDate.Year && dateToCheck.Month == transaction.TransactionDate.Month && dateToCheck.Day == transaction.TransactionDate.Day)
                    {
                        resultValue -= transaction.CostAmount;
                    }
                }
                resultPair.Add(new KeyValuePair<DateTime, double>(dateToCheck, Math.Truncate(100 * resultValue) / 100));
                dateToCheck = dateToCheck.AddDays(1);
            }
            return resultPair;
        }

        private static Dictionary<string, double> TransactionsInPast30DaysPie(IEnumerable<Models.Transaction> transactions)
        {
            Dictionary<string, double> resultDictionary = new Dictionary<string, double>();
            DateTime currentDate = DateTime.Now;
            DateTime dateToCheck = DateTime.Now.AddDays(-30);
            while (dateToCheck.Year != currentDate.Year || dateToCheck.Month != currentDate.Month || dateToCheck.Day != currentDate.Day)
            {
                foreach (var transaction in transactions)
                {
                    if (transaction.CostAmount > 0 && dateToCheck.Year == transaction.TransactionDate.Year && dateToCheck.Month == transaction.TransactionDate.Month && dateToCheck.Day == transaction.TransactionDate.Day)
                    {
                        string transactionName = GetNameOfTransaction(transaction);
                        if (resultDictionary.ContainsKey(transactionName))
                        {
                            resultDictionary[transactionName] += transaction.CostAmount;
                        }
                        else
                        {
                            resultDictionary[transactionName] = transaction.CostAmount;
                        }
                        resultDictionary[transactionName] = Math.Truncate(100 * resultDictionary[transactionName]) / 100;
                    }
                }
                dateToCheck = dateToCheck.AddDays(1);
            }
            return resultDictionary;
        }

        private static int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        private static Models.Transaction IsSimilarTransactionByName(Models.Transaction transactionToCheck, IEnumerable<Models.Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                if (transaction.Id != transactionToCheck.Id && CalculateSimilarity(GetNameOfTransaction(transaction), GetNameOfTransaction(transactionToCheck)) >= 0.5)
                {
                    return transaction;
                }
            }
            return null;
        }

        private static double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }

        public static double GetTruncatedByTwoDecimals(double number)
        {
            return Math.Truncate(100 * number) / 100;
        }

        private static string GetNameOfTransaction(Models.Transaction transaction)
        {
            string transactionName = "";
            if (string.IsNullOrEmpty(transaction.MerchantName))
            {
                transactionName = transaction.Name;
            }
            else
            {
                transactionName = transaction.MerchantName;
            }
            transactionName = Regex.Replace(transactionName, "[0-9][0-9]\\/[0-9][0-9]", "");
            return transactionName;
        }
    }
}
