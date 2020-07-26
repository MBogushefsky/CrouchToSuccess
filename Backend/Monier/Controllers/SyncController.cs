using Monier.DBModels;
using Monier.Helper;
using Monier.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using NHibernate.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Monier.Controllers
{
    [RoutePrefix("sync")]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SyncController : ApiController
    {
        [Route("everything")]
        [HttpPut] 
        public void SyncEverything()
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                DeleteExistingBankAccountsByUser(session, currentUser.Id);
                DeleteExistingTransactionsByUser(session, currentUser.Id);
                IEnumerable<DBModels.AccessToken> dbAccessTokens = session.Query<DBModels.AccessToken>().Where(x => x.UserID == currentUser.Id);
                foreach (var dbAccessToken in dbAccessTokens)
                {
                    PlaidClient plaidClient = new PlaidClient();
                    string institutionId = null;
                    List<Plaid.Models.BankAccount> pBankAccounts = plaidClient.GetInstitutionBankAccounts(dbAccessToken.Token, out institutionId);

                    foreach (var pBankAccount in pBankAccounts)
                    {
                        DBModels.BankAccount dbBankAccount = session.Query<DBModels.BankAccount>().Where(x => x.PlaidAccountID == pBankAccount.account_id).FirstOrDefault();
                        if (dbBankAccount == null)
                        {
                            session.SaveOrUpdate(new DBModels.BankAccount()
                            {
                                ID = Guid.NewGuid(),
                                PlaidAccountID = pBankAccount.account_id,
                                UserID = currentUser.Id,
                                InstitutionID = institutionId,
                                Name = pBankAccount.name,
                                FullName = pBankAccount.official_name,
                                Mask = pBankAccount.mask,
                                Type = pBankAccount.type,
                                SubType = pBankAccount.subtype,
                                AvailableBalance = pBankAccount.balances.available,
                                CurrentBalance = pBankAccount.balances.current,
                                LimitBalance = pBankAccount.balances.limit
                            });
                        }
                        else
                        {
                            dbBankAccount.PlaidAccountID = pBankAccount.account_id;
                            dbBankAccount.UserID = currentUser.Id;
                            dbBankAccount.InstitutionID = institutionId;
                            dbBankAccount.Name = pBankAccount.name;
                            dbBankAccount.FullName = pBankAccount.official_name;
                            dbBankAccount.Mask = pBankAccount.mask;
                            dbBankAccount.Type = pBankAccount.type;
                            dbBankAccount.SubType = pBankAccount.subtype;
                            dbBankAccount.AvailableBalance = pBankAccount.balances.available;
                            dbBankAccount.CurrentBalance = pBankAccount.balances.current;
                            dbBankAccount.LimitBalance = pBankAccount.balances.limit;
                            session.SaveOrUpdate(dbBankAccount);
                        }
                        DateTime ninetyDaysPrior = DateTime.Now.AddDays(-90);
                        JArray pBankAccountTransactions = plaidClient.GetInstitutionBankAccountTransactions(dbAccessToken.Token, pBankAccount.account_id, ninetyDaysPrior, DateTime.Now);
                        foreach (JObject pBankAccountTransaction in pBankAccountTransactions)
                        {
                            DBModels.Transaction dbBankAccountTransaction = session.Query<DBModels.Transaction>().Where(x => x.PlaidTransactionID == pBankAccountTransaction["transaction_id"].ToString()).FirstOrDefault();
                            string categoriesToAdd = "";
                            foreach (string category in ((JArray)pBankAccountTransaction["category"]))
                            {
                                categoriesToAdd += category + "|";
                            }
                            if (dbBankAccountTransaction == null)
                            {
                                session.SaveOrUpdate(new DBModels.Transaction()
                                {
                                    ID = Guid.NewGuid(),
                                    PlaidTransactionID = pBankAccountTransaction["transaction_id"].ToString(),
                                    UserID = currentUser.Id,
                                    PlaidAccountID = pBankAccount.account_id,
                                    MerchantName = pBankAccountTransaction["merchant_name"].ToString(),
                                    Name = pBankAccountTransaction["name"].ToString(),
                                    CostAmount = (double)pBankAccountTransaction["amount"],
                                    Pending = (bool)pBankAccountTransaction["pending"],
                                    PaymentChannel = pBankAccountTransaction["payment_channel"].ToString(),
                                    Categories = categoriesToAdd,
                                    TransactionDate = DateTime.ParseExact(pBankAccountTransaction["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture)
                                });
                            }
                            else
                            {
                                dbBankAccountTransaction.PlaidTransactionID = pBankAccountTransaction["transaction_id"].ToString();
                                dbBankAccountTransaction.UserID = currentUser.Id;
                                dbBankAccountTransaction.PlaidAccountID = pBankAccount.account_id;
                                dbBankAccountTransaction.MerchantName = pBankAccountTransaction["merchant_name"].ToString();
                                dbBankAccountTransaction.Name = pBankAccountTransaction["name"].ToString();
                                dbBankAccountTransaction.CostAmount = (double)pBankAccountTransaction["amount"];
                                dbBankAccountTransaction.Pending = (bool)pBankAccountTransaction["pending"];
                                dbBankAccountTransaction.PaymentChannel = pBankAccountTransaction["payment_channel"].ToString();
                                dbBankAccountTransaction.Categories = categoriesToAdd;
                                dbBankAccountTransaction.TransactionDate = DateTime.ParseExact(pBankAccountTransaction["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                session.SaveOrUpdate(dbBankAccount);
                            }
                        }
                    }
                }
                trans.Commit();
            }
        }

        private static void DeleteExistingBankAccountsByUser(ISession session, Guid userId)
        {
            session.Query<DBModels.BankAccount>().Where(x => x.UserID == userId).Delete();
        }

        private static void DeleteExistingTransactionsByUser(ISession session, Guid userId)
        {
            session.Query<DBModels.Transaction>().Where(x => x.UserID == userId).Delete();
        }
    }
}
