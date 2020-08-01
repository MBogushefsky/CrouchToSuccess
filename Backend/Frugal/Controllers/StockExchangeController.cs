using Frugal.DBModels;
using Frugal.Handlers;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Frugal.ThirdParty.MarketStackClient;

namespace Frugal.Controllers
{
    [RoutePrefix("stock_exchange")]
    public class FakeStockExchangeController : ApiController
    {
        [Route("balance")]
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
                }
                return resultBalance;
            }
        }

        [Route("data/{Symbol}")]
        [HttpGet]
        public Dictionary<DateTime, double> GetDataBySymbol([FromUri] string Symbol)
        {
            MarketStackClient msClient = new MarketStackClient();
            return msClient.GetEODBySymbol(Symbol);
        }

        public static Models.StockExchangeTransaction StockExchangeTransactionDBModelToModel(DBModels.StockExchangeTransaction stockExchangeTransaction)
        {
            return new Models.StockExchangeTransaction()
            {
                ID = stockExchangeTransaction.ID,
                UserID = stockExchangeTransaction.UserID,
                Symbol = stockExchangeTransaction.Symbol,
                Type = stockExchangeTransaction.Type,
                CurrencyAmount = stockExchangeTransaction.CurrencyAmount,
                StockAmount = stockExchangeTransaction.StockAmount
            };
        }
    }
}
