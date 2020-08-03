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
                resultDictionary.Add("Symbol", Symbol);
                MarketStackClient msClient = new MarketStackClient();
                resultDictionary.Add("Day", msClient.GetIntradayRaw(Symbol, "15min", null, null, 96));
                resultDictionary.Add("Week", msClient.GetIntradayRaw(Symbol, "3hour", null, null, 56));
                resultDictionary.Add("Month", msClient.GetIntradayRaw(Symbol, "12hour", null, null, 60));
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
                if (!portfolio.ContainsKey(transactionRequest.Symbol) || (portfolio.ContainsKey(transactionRequest.Symbol) && portfolio[transactionRequest.Symbol] < transactionRequest.StockAmount))
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Don't have this stock to sell") });
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

        [Route("eod/{Symbol}")]
        [HttpGet]
        public Dictionary<DateTime, double> GetEODBySymbol([FromUri] string Symbol)
        {
            MarketStackClient msClient = new MarketStackClient();
            return msClient.GetEODBySymbol(Symbol);
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
