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
    [RoutePrefix("bankAccount")]
    public class BankAccountController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IEnumerable<Models.BankAccount> GetBankAccounts()
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                return GetBankAccountsBySession(session, currentUser.Id);
            }
        }

        [Route("{ID}")]
        [HttpGet]
        public Models.BankAccount GetBankAccountByID([FromUri] Guid ID)
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                return GetBankAccountByIDAndSession(session, ID);
            }
        }

        public static Models.BankAccount GetBankAccountByIDAndSession(ISession session, Guid bankAccountId)
        {
            DBModels.BankAccount dbBankAccount = session.Get<DBModels.BankAccount>(bankAccountId);
            return DBModelBankAccountToModelBankAccount(dbBankAccount);
        }

        public static IEnumerable<Models.BankAccount> GetBankAccountsBySession(ISession session, Guid currentUserId)
        {
            IEnumerable<DBModels.BankAccount> dbBankAccounts = session.Query<DBModels.BankAccount>().Where(x => x.UserID == currentUserId);
            List<Models.BankAccount> bankAccountsToReturn = new List<Models.BankAccount>();
            foreach (DBModels.BankAccount dbBankAccount in dbBankAccounts)
            {
                bankAccountsToReturn.Add(DBModelBankAccountToModelBankAccount(dbBankAccount));
            }
            return bankAccountsToReturn.ToList();
        }

        private static Models.BankAccount DBModelBankAccountToModelBankAccount(DBModels.BankAccount dbBankAccount)
        {
            return new Models.BankAccount()
            {
                Id = dbBankAccount.ID,
                UserId = dbBankAccount.UserID,
                PlaidAccountId = dbBankAccount.PlaidAccountID,
                InstitutionId = dbBankAccount.InstitutionID,
                Name = dbBankAccount.Name,
                FullName = dbBankAccount.FullName,
                Type = dbBankAccount.Type,
                SubType = dbBankAccount.SubType,
                Mask = dbBankAccount.Mask,
                AvailableBalance = dbBankAccount.AvailableBalance,
                CurrentBalance = dbBankAccount.CurrentBalance,
                LimitBalance = dbBankAccount.LimitBalance
            };
        }
    }
}
