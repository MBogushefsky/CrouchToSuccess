using Frugal.DBModels;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Frugal.Controllers
{
    [RoutePrefix("transaction")]
    public class TransactionController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IEnumerable<Models.Transaction> GetTransactions()
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                return GetTransactionsBySession(session, currentUser.Id);
            }
        }

        [Route("{ID}")]
        [HttpGet]
        public IEnumerable<Models.Transaction> GetTransactionsByPlaidBankAccountID([FromUri] string ID)
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                return GetTransactionsByPlaidBankAccountIDAndSession(session, ID);
            }
        }

        public static IEnumerable<Models.Transaction> GetTransactionsByPlaidBankAccountIDAndSession(ISession session, string plaidBankAccountId)
        {
            var dbTransactions = session.QueryOver<DBModels.Transaction>().Where(x => x.PlaidAccountID == plaidBankAccountId).OrderBy(x => x.TransactionDate).Desc.List<DBModels.Transaction>();
            List<Models.Transaction> transactionToReturn = new List<Models.Transaction>();
            foreach (var dbTransaction in dbTransactions)
            {
                transactionToReturn.Add(DBModelTransactionToModelTransaction(dbTransaction));
            }
            return transactionToReturn;
        }

        public static IEnumerable<Models.Transaction> GetTransactionsBySession(ISession session, Guid currentUserId)
        {
            var dbTransactions = session.Query<DBModels.Transaction>().Where(x => x.UserID == currentUserId).ToList();
            List<Models.Transaction> transactionToReturn = new List<Models.Transaction>();
            foreach (var dbTransaction in dbTransactions)
            {
                transactionToReturn.Add(DBModelTransactionToModelTransaction(dbTransaction));
            }
            return transactionToReturn;
        }

        public static Models.Transaction DBModelTransactionToModelTransaction(DBModels.Transaction dbTransaction)
        {
            string[] categoriesSplit = dbTransaction.Categories.Split('|');
            List<string> categoriesToAdd = new List<string>();
            foreach (string category in categoriesSplit)
            {
                if (!string.IsNullOrWhiteSpace(category))
                {
                    categoriesToAdd.Add(category);
                }
            }
            
            return new Models.Transaction()
            {
                Id = dbTransaction.ID,
                PlaidTransactionId = dbTransaction.PlaidTransactionID,
                PlaidAccountId = dbTransaction.PlaidAccountID,
                UserId = dbTransaction.UserID,
                Name = dbTransaction.Name,
                MerchantName = dbTransaction.MerchantName,
                Categories = categoriesToAdd.ToArray(),
                CostAmount = dbTransaction.CostAmount,
                PaymentChannel = dbTransaction.PaymentChannel,
                Pending = dbTransaction.Pending,
                TransactionDate = dbTransaction.TransactionDate
            };
        }
    }
}
