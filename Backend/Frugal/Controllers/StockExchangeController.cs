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
        public Dictionary<string, Dictionary<DateTime, double>> GetStockHistoryOfUser()
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                IEnumerable<DBModels.StockExchangeTransaction> dbStockExchangeTransactions = session.QueryOver<DBModels.StockExchangeTransaction>().Where(x => x.UserID == currentUser.Id).OrderBy(x => x.CreatedDate).Asc.List();
                
                Dictionary<string, int> allEverOwnedPortfolio = GetAllEverOwnerStockPortfolio(session, currentUser.Id);
                IEnumerable<DBModels.StockExchangeQuoteHistory> dbStockExchangeQuoteHistories = session.Query<DBModels.StockExchangeQuoteHistory>().Where(x => allEverOwnedPortfolio.ContainsKey(x.Symbol));
                List<Models.StockExchangeQuoteHistory> stockExchangeQuoteHistories = DBModelToModel(dbStockExchangeQuoteHistories);

                Dictionary<string, Dictionary<DateTime, double>> resultDictionary = new Dictionary<string, Dictionary<DateTime, double>>();

                List<Models.StockExchangeTransaction> dayTransactions = new List<Models.StockExchangeTransaction>();
                List<Models.StockExchangeTransaction> weekTransactions = new List<Models.StockExchangeTransaction>();
                List<Models.StockExchangeTransaction> monthTransactions = new List<Models.StockExchangeTransaction>();
                List<Models.StockExchangeTransaction> allTransactions = new List<Models.StockExchangeTransaction>();

                List<Models.StockExchangeQuoteHistory> dayQuotes = new List<Models.StockExchangeQuoteHistory>();
                List<Models.StockExchangeQuoteHistory> weekQuotes = new List<Models.StockExchangeQuoteHistory>();
                List<Models.StockExchangeQuoteHistory> monthQuotes = new List<Models.StockExchangeQuoteHistory>();
                List<Models.StockExchangeQuoteHistory> allQuotes = new List<Models.StockExchangeQuoteHistory>();

                DateTime currentDateTime = DateTime.Now;
                DateTime dayAgo = DateTime.Now.AddDays(-1);
                DateTime weekAgo = DateTime.Now.AddDays(-7);
                DateTime monthAgo = DateTime.Now.AddMonths(-1);
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
                    allTransactions.Add(transactionToAdd);
                }

                foreach (var quote in stockExchangeQuoteHistories)
                {
                    if (DateTime.Compare(dayAgo, quote.Timestamp) <= 0)
                    {
                        dayQuotes.Add(quote);
                    }
                    if (DateTime.Compare(weekAgo, quote.Timestamp) <= 0)
                    {
                        weekQuotes.Add(quote);
                    }
                    if (DateTime.Compare(monthAgo, quote.Timestamp) <= 0)
                    {
                        monthQuotes.Add(quote);
                    }
                    allQuotes.Add(quote);
                }

                resultDictionary.Add("Day", GetEquityHistoryOfTransactions(dayQuotes, dayTransactions));
                resultDictionary.Add("Week", GetEquityHistoryOfTransactions(weekQuotes, weekTransactions));
                resultDictionary.Add("Month", GetEquityHistoryOfTransactions(monthQuotes, monthTransactions));
                resultDictionary.Add("All", GetEquityHistoryOfTransactions(allQuotes, allTransactions));
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
                Dictionary<string, int> portfolio = GetCurrentStockPortfolio(session, currentUser.Id);
                
                trans.Commit();
                return portfolio;
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
                double currentPriceOfSymbol = msClient.GetCurrentPriceBySymbol(Symbol);
                resultDictionary.Add("CurrentPrice", currentPriceOfSymbol);
                SaveSymbolQuote(session, new DBModels.StockExchangeQuoteHistory() { 
                    ID = Guid.NewGuid(),
                    Symbol = (string) symbolQuote["symbol"],
                    Price = currentPriceOfSymbol,
                    Timestamp = DateTime.Now
                });
                resultDictionary.Add("Quote", symbolQuote);
                List<PriceInstance> intradayData = msClient.GetIntradayRaw(Symbol);
                List<PriceInstance> weekData = msClient.GetHistoricalDataRaw(Symbol, "5dm");
                List<PriceInstance> monthData = msClient.GetHistoricalDataRaw(Symbol, "1mm");
                resultDictionary.Add("Day", intradayData);
                resultDictionary.Add("Week", weekData);
                resultDictionary.Add("Month", monthData);
                trans.Commit();
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
                    SaveSymbolQuote(session, new DBModels.StockExchangeQuoteHistory()
                    {
                        ID = Guid.NewGuid(),
                        Symbol = transactionRequest.Symbol,
                        Price = priceOfStockSymbol,
                        Timestamp = DateTime.Now
                    });
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
                Dictionary<string, int> portfolio = GetCurrentStockPortfolio(session, currentUser.Id);
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
                    SaveSymbolQuote(session, new DBModels.StockExchangeQuoteHistory()
                    {
                        ID = Guid.NewGuid(),
                        Symbol = transactionRequest.Symbol,
                        Price = priceOfStockSymbol,
                        Timestamp = DateTime.Now
                    });
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

        /*public static List<PriceInstance> StoreHistoricalDataOfSymbolIfNeeded(ISession session, string symbol)
        {
            MarketStackClient msClient = new MarketStackClient();
            DBModels.StockExchangeChartSync dbStockExchangeChartSyncs = session.Query<DBModels.StockExchangeChartSync>().Where(x => x.Symbol == symbol && x.RangeFound == "intraday").FirstOrDefault();
            if (dbStockExchangeChartSyncs == null)
            {
                List<PriceInstance> intradayData = msClient.GetIntradayRaw(symbol);
                foreach (var data in intradayData)
                {
                    session.SaveOrUpdate(new StockExchangeChart() { 
                        ID = Guid.NewGuid(),
                        Symbol = symbol,
                        RangeFound = "intraday",
                        Open = data.open,
                        Close = data.close,
                        High = data.high,
                        Low = data.low,
                        Volume = data.volume,
                        Timestamp = DateTime.Parse(data.date + " " + data.minute)
                    });
                }
                session.SaveOrUpdate(new DBModels.StockExchangeChartSync() {
                    ID = Guid.NewGuid(),
                    Symbol = symbol,
                    RangeFound = "intraday",
                    LastUpdatedTimestamp = DateTime.Now
                });
            }
            else
            {
                if (latestUpdate < DateTimeHandler.GetEpochOfDateTime(dbStockExchangeChartSyncs.LastUpdatedTimestamp))
                {
                    dbStockExchangeChartSyncs.LastUpdatedTimestamp = DateTime.Now;
                    session.SaveOrUpdate(dbStockExchangeChartSyncs);
                }
                else
                {
                    DateTime latestChartOfSymbol = session.QueryOver<DBModels.StockExchangeChart>().Where(x => x.Symbol == symbol && x.RangeFound == "intraday").OrderBy(x => x.Timestamp).Desc.List().FirstOrDefault().Timestamp;
                    if (latestChartOfSymbol == dbStockExchangeChartSyncs.LastUpdatedTimestamp)
                    {

                    }
                    IEnumerable<DBModels.StockExchangeChart> dbStockExchangeCharts = session.QueryOver<DBModels.StockExchangeChart>().Where(x => x.Symbol == symbol && x.RangeFound == "intraday").OrderBy(x => x.Timestamp).Asc.List();
                    
                    List<PriceInstance> intradayData = msClient.GetIntradayRaw(symbol);
                }
            }
        }*/

        public static void StoreQuoteToHistory(ISession session, DBModels.StockExchangeQuoteHistory quote)
        {
            session.SaveOrUpdate(quote);
        }

        // NOT DONE YET
        public static Dictionary<DateTime, double> GetEquityHistoryOfTransactions(List<Models.StockExchangeQuoteHistory> quotes, List<Models.StockExchangeTransaction> transactions)
        {
            Dictionary<DateTime, double> resultDictionary = new Dictionary<DateTime, double>();

            Dictionary<DateTime, Models.StockExchangePortfolio> equityDictionary = new Dictionary<DateTime, Models.StockExchangePortfolio>();
            Models.StockExchangePortfolio previousPortfolio = null;
            foreach (var transaction in transactions)
            {
                if (previousPortfolio == null)
                {
                    previousPortfolio = new Models.StockExchangePortfolio()
                    {
                        CurrencyAmount = 0,
                        Stocks = new Dictionary<string, int>(),
                        StockPrices = new Dictionary<string, double>(),
                        Timestamp = DateTime.Now
                    };
                }
                Models.StockExchangePortfolio currentPortfolio = previousPortfolio;
                if (transaction.Type == "ADD")
                {
                    currentPortfolio.CurrencyAmount += NullHandler.GetZeroIfNull(transaction.CurrencyAmount);
                }
                else if (transaction.Type == "BUY")
                {
                    currentPortfolio.CurrencyAmount -= (NullHandler.GetZeroIfNull(transaction.StockAmount) * NullHandler.GetZeroIfNull(transaction.StockRate));
                    if (currentPortfolio.Stocks.ContainsKey(transaction.Symbol))
                    {
                        currentPortfolio.Stocks[transaction.Symbol] += NullHandler.GetZeroIfNull(transaction.StockAmount);
                    }
                    else
                    {
                        currentPortfolio.Stocks[transaction.Symbol] = NullHandler.GetZeroIfNull(transaction.StockAmount);
                    }
                    currentPortfolio.StockPrices[transaction.Symbol] = NullHandler.GetZeroIfNull(transaction.StockRate);
                }
                else if (transaction.Type == "SELL")
                {
                    if (previousPortfolio.Stocks.ContainsKey(transaction.Symbol))
                    {
                        currentPortfolio.Stocks[transaction.Symbol] -= NullHandler.GetZeroIfNull(transaction.StockAmount);
                    }
                    else
                    {
                        currentPortfolio.Stocks[transaction.Symbol] = -1 * NullHandler.GetZeroIfNull(transaction.StockAmount);
                    }
                    currentPortfolio.CurrencyAmount -= (NullHandler.GetZeroIfNull(transaction.StockAmount) * NullHandler.GetZeroIfNull(transaction.StockRate));
                    currentPortfolio.StockPrices[transaction.Symbol] = NullHandler.GetZeroIfNull(transaction.StockRate);
                }
                if (equityDictionary.ContainsKey(transaction.CreatedDate))
                {
                    equityDictionary[transaction.CreatedDate] = new Models.StockExchangePortfolio() { 
                        CurrencyAmount = equityDictionary[transaction.CreatedDate].CurrencyAmount + NullHandler.GetZeroIfNull(transaction.CurrencyAmount),
                        Stocks = DictionaryHandler.AddTwoDictionaries(equityDictionary[transaction.CreatedDate].Stocks, currentPortfolio.Stocks),
                        StockPrices = currentPortfolio.StockPrices,
                        Timestamp = transaction.CreatedDate
                    };
                }
                else
                {
                    equityDictionary[transaction.CreatedDate] = currentPortfolio;
                }
                previousPortfolio = currentPortfolio;
            }
            foreach (var quote in quotes)
            {
                if (equityDictionary.ContainsKey(quote.Timestamp))
                {
                    equityDictionary[quote.Timestamp].StockPrices[quote.Symbol] = quote.Price;
                }
                else
                {
                    equityDictionary[quote.Timestamp] = new Models.StockExchangePortfolio()
                    {
                        
                    };
                }
            }
            return resultDictionary;
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

        public static void SaveSymbolQuote(ISession session, DBModels.StockExchangeQuoteHistory quote)
        {
            DBModels.StockExchangeQuoteHistory dbStockExchangeQuoteHistory = session.QueryOver<DBModels.StockExchangeQuoteHistory>().Where(x => x.Symbol == quote.Symbol).OrderBy(x => x.Timestamp).Desc.List().FirstOrDefault();
            if (dbStockExchangeQuoteHistory == null || dbStockExchangeQuoteHistory.Price != quote.Price)
            {
                session.SaveOrUpdate(quote);
            }
        }

        public static double GetCurrentSymbolPrice(string symbol)
        {
            MarketStackClient msClient = new MarketStackClient();
            return msClient.GetCurrentPriceBySymbol(symbol);
        }

        public static Dictionary<string, int> GetCurrentStockPortfolio(ISession session, Guid userId)
        {
            Dictionary<string, int> resultDictionary = new Dictionary<string, int>();
            Dictionary<string, int> allPortfolioOwned = GetAllEverOwnerStockPortfolio(session, userId);
            foreach (KeyValuePair<string, int> portfolioPair in allPortfolioOwned)
            {
                if (portfolioPair.Value != 0)
                {
                    resultDictionary.Add(portfolioPair.Key, portfolioPair.Value);
                    SaveSymbolQuote(session, new DBModels.StockExchangeQuoteHistory()
                    {
                        ID = Guid.NewGuid(),
                        Symbol = portfolioPair.Key,
                        Price = GetCurrentSymbolPrice(portfolioPair.Key),
                        Timestamp = DateTime.Now
                    });
                }
            }
            return resultDictionary;
        }

        public static Dictionary<string, int> GetAllEverOwnerStockPortfolio(ISession session, Guid userId)
        {
            IEnumerable<DBModels.StockExchangeTransaction> dbStockExchangeTransactions = session.Query<DBModels.StockExchangeTransaction>().Where(x => x.UserID == userId);
            Dictionary<string, int> allPortfolioOwned = new Dictionary<string, int>();
            foreach (var stockExchangeTran in dbStockExchangeTransactions)
            {
                if (stockExchangeTran.Type == "BUY" || stockExchangeTran.Type == "SELL")
                {
                    if (!allPortfolioOwned.ContainsKey(stockExchangeTran.Symbol))
                    {
                        allPortfolioOwned[stockExchangeTran.Symbol] = 0;
                    }
                    if (stockExchangeTran.Type == "BUY")
                    {
                        allPortfolioOwned[stockExchangeTran.Symbol] += NullHandler.GetZeroIfNull(stockExchangeTran.StockAmount);
                    }
                    else if (stockExchangeTran.Type == "SELL")
                    {
                        allPortfolioOwned[stockExchangeTran.Symbol] -= NullHandler.GetZeroIfNull(stockExchangeTran.StockAmount);
                    }
                }
            }
            return allPortfolioOwned;
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

        public static List<Models.StockExchangeQuoteHistory> DBModelToModel(IEnumerable<DBModels.StockExchangeQuoteHistory> dbQuoteHistories)
        {
            List<Models.StockExchangeQuoteHistory> resultQuoteHistory = new List<Models.StockExchangeQuoteHistory>();
            foreach (var quoteHistory in dbQuoteHistories)
            {
                resultQuoteHistory.Add(new Models.StockExchangeQuoteHistory()
                {
                    Id = quoteHistory.ID,
                    Symbol = quoteHistory.Symbol,
                    Price = quoteHistory.Price,
                    Timestamp = quoteHistory.Timestamp
                });
            }
            return resultQuoteHistory;
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
