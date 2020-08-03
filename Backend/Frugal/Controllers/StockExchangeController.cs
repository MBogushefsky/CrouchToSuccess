using Frugal.DBModels;
using Frugal.Handlers;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Frugal.ThirdParty.MarketStackClient;
using System.Net;
using System.Net.Http;
using Frugal.ThirdParty.Models;
using Newtonsoft.Json.Linq;

namespace Frugal.Controllers
{
    [RoutePrefix("stock-exchange")]
    public class StockExchangeController : ApiController
    {
        [Route("buying-power")]
        [HttpGet]
        public double GetCurrentBalanceOfUser()
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                IEnumerable<DBModels.StockExchangeTransaction> dbStockExchangeTransactions = session.Query<DBModels.StockExchangeTransaction>().Where(x => x.UserID == currentUser.Id);
                double resultBalance = 0.00;
                foreach (var stockExchangeTran in dbStockExchangeTransactions)
                {
                    if (stockExchangeTran.Type == "ADD")
                    {
                        resultBalance += NullHandler.GetZeroIfNull(stockExchangeTran.CurrencyAmount);
                    }
                    else if (stockExchangeTran.Type == "BUY")
                    {
                        resultBalance -= NullHandler.GetZeroIfNull(stockExchangeTran.StockRate) * NullHandler.GetZeroIfNull(stockExchangeTran.StockAmount);
                    }
                    else if (stockExchangeTran.Type == "SELL")
                    {
                        resultBalance += NullHandler.GetZeroIfNull(stockExchangeTran.StockRate) * NullHandler.GetZeroIfNull(stockExchangeTran.StockAmount);
                    }
                }
                return resultBalance;
            }
        }

        [Route("equity/history")]
        [HttpGet]
        public Dictionary<string, List<Models.StockExchangeTransaction>> GetStockHistoryOfUser()
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                IEnumerable<DBModels.StockExchangeTransaction> dbStockExchangeTransactions = session.QueryOver<DBModels.StockExchangeTransaction>().Where(x => x.UserID == currentUser.Id).OrderBy(x => x.CreatedDate).Asc.List();
                
                Dictionary<string, List<Models.StockExchangeTransaction>> resultDictionary = new Dictionary<string, List<Models.StockExchangeTransaction>>();
                List<Models.StockExchangeTransaction> dayTransactions = new List<Models.StockExchangeTransaction>();
                List<Models.StockExchangeTransaction> weekTransactions = new List<Models.StockExchangeTransaction>();
                List<Models.StockExchangeTransaction> monthTransactions = new List<Models.StockExchangeTransaction>();
                List<Models.StockExchangeTransaction> threeMonthTransactions = new List<Models.StockExchangeTransaction>();
                List<Models.StockExchangeTransaction> yearTransactions = new List<Models.StockExchangeTransaction>();
                List<Models.StockExchangeTransaction> allTransactions = new List<Models.StockExchangeTransaction>();
                DateTime currentDateTime = DateTime.Now;
                DateTime dayAgo = DateTime.Now.AddDays(-1);
                DateTime weekAgo = DateTime.Now.AddDays(-7);
                DateTime monthAgo = DateTime.Now.AddMonths(-1);
                DateTime threeMonthAgo = DateTime.Now.AddMonths(-3);
                DateTime yearAgo = DateTime.Now.AddYears(-1);
                foreach (var transaction in dbStockExchangeTransactions)
                {
                    Models.StockExchangeTransaction transactionToAdd = StockExchangeTransactionDBModelToModel(transaction);
                    if (DateTime.Compare(dayAgo, transaction.CreatedDate) <= 0)
                    {
                        dayTransactions.Add(transactionToAdd);
                    }
                    if (DateTime.Compare(weekAgo, transaction.CreatedDate) <= 0)
                    {
                        weekTransactions.Add(transactionToAdd);
                    }
                    if (DateTime.Compare(monthAgo, transaction.CreatedDate) <= 0)
                    {
                        monthTransactions.Add(transactionToAdd);
                    }
                    if (DateTime.Compare(threeMonthAgo, transaction.CreatedDate) <= 0)
                    {
                        threeMonthTransactions.Add(transactionToAdd);
                    }
                    if (DateTime.Compare(yearAgo, transaction.CreatedDate) <= 0)
                    {
                        yearTransactions.Add(transactionToAdd);
                    }
                    allTransactions.Add(transactionToAdd);
                }
                resultDictionary.Add("Day", dayTransactions);
                resultDictionary.Add("Week", weekTransactions);
                resultDictionary.Add("Month", monthTransactions);
                resultDictionary.Add("3Month", threeMonthTransactions);
                resultDictionary.Add("Year", yearTransactions);
                resultDictionary.Add("All", allTransactions);
                return resultDictionary;
            }
        }

        [Route("portfolio")]
        [HttpGet]
        public Dictionary<string, int> GetCurrentPortfolio()
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                return GetStockPortfolio(session, currentUser.Id);
            }
        }

        [Route("data/{Symbol}")]
        [HttpGet]
        public Dictionary<string, object> GetDataOfSymbol([FromUri] string Symbol)
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                Dictionary<string, object> resultDictionary = new Dictionary<string, object>();
                MarketStackClient msClient = new MarketStackClient();
                JObject symbolQuote = msClient.GetSymbolQuote(Symbol);
                resultDictionary.Add("Symbol", symbolQuote["symbol"]);
                resultDictionary.Add("CompanyName", symbolQuote["companyName"]);
                resultDictionary.Add("CurrentPrice", msClient.GetCurrentPriceBySymbol(Symbol));
                resultDictionary.Add("Quote", symbolQuote);
                resultDictionary.Add("Day", msClient.GetIntradayRaw(Symbol));
                resultDictionary.Add("Week", msClient.GetHistoricalDataRaw(Symbol, "5dm"));
                resultDictionary.Add("Month", msClient.GetHistoricalDataRaw(Symbol, "1mm"));
                return resultDictionary;
            }
        }

        [Route("buy")]
        [HttpPost]
        public void BuyStock([FromBody] Models.StockExchangeTransaction transactionRequest)
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                double buyingPower = GetBuyingPowerByUser(session, currentUser.Id);
                double priceOfStockSymbol = GetCurrentSymbolPrice(transactionRequest.Symbol);
                if ((priceOfStockSymbol * transactionRequest.StockAmount) > buyingPower)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Not enough buying power") });
                }
                if (transactionRequest.StockRate != priceOfStockSymbol)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Price stock changed") });
                }
                try
                {
                    session.Save(new DBModels.StockExchangeTransaction()
                    {
                        ID = Guid.NewGuid(),
                        UserID = currentUser.Id,
                        Type = "BUY",
                        StockRate = priceOfStockSymbol,
                        Symbol = transactionRequest.Symbol,
                        CurrencyAmount = null,
                        StockAmount = transactionRequest.StockAmount,
                        CreatedDate = DateTime.Now
                    });
                    trans.Commit();
                }
                catch (Exception e)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
                }
            }
        }

        [Route("sell")]
        [HttpPost]
        public void SellStock([FromBody] Models.StockExchangeTransaction transactionRequest)
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                Dictionary<string, int> portfolio = GetStockPortfolio(session, currentUser.Id);
                double priceOfStockSymbol = GetCurrentSymbolPrice(transactionRequest.Symbol);
                if (!portfolio.ContainsKey(transactionRequest.Symbol))
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Don't have this stock to sell") });
                }
                if (portfolio.ContainsKey(transactionRequest.Symbol) && portfolio[transactionRequest.Symbol] < transactionRequest.StockAmount)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Don't have enough stock to sell this amount") });
                }
                if (transactionRequest.StockRate != priceOfStockSymbol)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Price stock changed") });
                }
                try
                {
                    session.Save(new DBModels.StockExchangeTransaction()
                    {
                        ID = Guid.NewGuid(),
                        UserID = currentUser.Id,
                        Type = "SELL",
                        StockRate = priceOfStockSymbol,
                        Symbol = transactionRequest.Symbol,
                        CurrencyAmount = null,
                        StockAmount = transactionRequest.StockAmount,
                        CreatedDate = DateTime.Now
                    });
                    trans.Commit();
                }
                catch (Exception e)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
                }
            }
        }

        [Route("intraday/{Symbol}")]
        [HttpGet]
        public Dictionary<DateTime, double> GetIntradayBySymbol([FromUri] string Symbol)
        {
            MarketStackClient msClient = new MarketStackClient();
            return msClient.GetIntradayBySymbol(Symbol);
        }

        /*[Route("eod/{Symbol}")]
        [HttpGet]
        public Dictionary<DateTime, double> GetEODBySymbol([FromUri] string Symbol)
        {
            MarketStackClient msClient = new MarketStackClient();
            return msClient.GetEODBySymbol(Symbol);
        }*/


        // NOT DONE YET
        public static Dictionary<DateTime, double> GetEquityHistoryOfTransactions(List<Models.StockExchangeTransaction> transactions)
        {
            Dictionary<DateTime, double> resultDictionary = new Dictionary<DateTime, double>();
            double currentEquity = 0.00;
            foreach (var transaction in transactions)
            {
                double changeFromTransaction = 0.00;
                if (resultDictionary.ContainsKey(transaction.CreatedDate))
                {
                    resultDictionary[transaction.CreatedDate] += currentEquity;
                }
                else
                {
                    resultDictionary[transaction.CreatedDate] = currentEquity;
                }
                if (transaction.Type == "ADD")
                {
                    changeFromTransaction += NullHandler.GetZeroIfNull(transaction.CurrencyAmount);
                }
                else if (transaction.Type == "BUY")
                {
                    changeFromTransaction -= NullHandler.GetZeroIfNull(transaction.StockRate) * NullHandler.GetZeroIfNull(transaction.StockAmount);
                }
                else if (transaction.Type == "SELL")
                {
                    changeFromTransaction += NullHandler.GetZeroIfNull(transaction.StockRate) * NullHandler.GetZeroIfNull(transaction.StockAmount);
                }
            }
            return null;
        }

        public static Dictionary<DateTime, double> AppendOnToKeyOrCreate(Dictionary<DateTime, double> dictionary, DateTime key, double number, bool subtract)
        {
            if (dictionary.ContainsKey(key))
            {
                if (subtract)
                {
                    dictionary[key] -= number;
                }
                else
                {
                    dictionary[key] += number;
                }
            }
            else
            {
                if (subtract)
                {
                    dictionary[key] = number;
                }
                else
                {
                    dictionary[key] = -1 * number;
                }
            }
            return dictionary;
        }

        public static double GetCurrentSymbolPrice(string symbol)
        {
            MarketStackClient msClient = new MarketStackClient();
            return msClient.GetCurrentPriceBySymbol(symbol);
        }

        public static Dictionary<string, int> GetStockPortfolio(ISession session, Guid userId)
        {
            IEnumerable<DBModels.StockExchangeTransaction> dbStockExchangeTransactions = session.Query<DBModels.StockExchangeTransaction>().Where(x => x.UserID == userId);
            Dictionary<string, int> resultPortfolio = new Dictionary<string, int>();
            foreach (var stockExchangeTran in dbStockExchangeTransactions)
            {
                if (stockExchangeTran.Type == "BUY" || stockExchangeTran.Type == "SELL")
                {
                    if (!resultPortfolio.ContainsKey(stockExchangeTran.Symbol))
                    {
                        resultPortfolio[stockExchangeTran.Symbol] = 0;
                    }
                    if (stockExchangeTran.Type == "BUY")
                    {
                        resultPortfolio[stockExchangeTran.Symbol] += NullHandler.GetZeroIfNull(stockExchangeTran.StockAmount);
                    }
                    else if (stockExchangeTran.Type == "SELL")
                    {
                        resultPortfolio[stockExchangeTran.Symbol] -= NullHandler.GetZeroIfNull(stockExchangeTran.StockAmount);
                    }
                }
            }
            return resultPortfolio;
        }

        public static double GetBuyingPowerByUser(ISession session, Guid userId)
        {
            IEnumerable<DBModels.StockExchangeTransaction> dbStockExchangeTransactions = session.Query<DBModels.StockExchangeTransaction>().Where(x => x.UserID == userId);
            double resultBuyingPower = 0.00;
            foreach (var stockExchangeTran in dbStockExchangeTransactions)
            {
                if (stockExchangeTran.Type == "ADD")
                {
                    resultBuyingPower += NullHandler.GetZeroIfNull(stockExchangeTran.CurrencyAmount);
                }
            }
            return resultBuyingPower;
        }

        public static Models.StockExchangeTransaction StockExchangeTransactionDBModelToModel(DBModels.StockExchangeTransaction stockExchangeTransaction)
        {
            return new Models.StockExchangeTransaction()
            {
                Id = stockExchangeTransaction.ID,
                UserId = stockExchangeTransaction.UserID,
                Symbol = stockExchangeTransaction.Symbol,
                Type = stockExchangeTransaction.Type,
                CurrencyAmount = stockExchangeTransaction.CurrencyAmount,
                StockAmount = stockExchangeTransaction.StockAmount,
                StockRate = stockExchangeTransaction.StockRate,
                CreatedDate = stockExchangeTransaction.CreatedDate
            };
        }
    }
}
