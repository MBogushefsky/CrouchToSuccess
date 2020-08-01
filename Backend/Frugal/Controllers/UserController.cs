// Credits to https://stackoverflow.com/questions/25855698/how-can-i-retrieve-basic-authentication-credentials-from-the-header
using Frugal.DBModels;
using Frugal.Models;
using Frugal.Plaid;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Frugal.Controllers
{
    [RoutePrefix("user")]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        [Route("")]
        [HttpGet]
        public Models.User LoginUser([FromUri] string Username, [FromUri] string PasswordHash)
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            {
                var dbUser = session.Query<DBModels.User>().Where(x => x.Username == Username && x.PasswordHash == HttpUtility.UrlDecode(PasswordHash)).ToList().FirstOrDefault();
                if (dbUser == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                return DBModelUserToModelUser(dbUser);
            }
        }

        [Route("")]
        [HttpPost]
        public void CreateUser([FromBody] Models.User user)
        {
            UserToken userToken = (UserToken)HttpContext.Current.Items["UserToken"];
            using (ISession session = SessionFactory.GetCurrentSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                try
                {
                    session.Save(new DBModels.User() { 
                        ID = Guid.NewGuid(),
                        Username = user.Username,
                        PasswordHash = user.PasswordHash,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = null,
                        Admin = false
                    });
                    trans.Commit();
                }
                catch (Exception e)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message)} );
                }
            }
        }

        [Route("linkBank")]
        [HttpPost]
        public void LinkBankAccount([FromBody] string publicToken)
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                PlaidClient plaidClient = new PlaidClient();
                string accessToken = plaidClient.ExchangePublicTokenForAccessToken(publicToken);
                session.Save(new DBModels.AccessToken()
                {
                    ID = Guid.NewGuid(),
                    UserID = currentUser.Id,
                    Token = accessToken
                });
                trans.Commit();
            }
        }

        [Route("linkBank/{BankAccountID}")]
        [HttpDelete]
        public void DeleteLinkedBankAccount([FromUri] Guid BankAccountID)
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                Models.User currentUser = UserController.CheckUserCredentials(session, HttpContext.Current.Request.Headers["Authorization"]);
                DBModels.BankAccount dbBankAccountToDelete = session.Query<DBModels.BankAccount>().Where(x => x.UserID == currentUser.Id && x.ID == BankAccountID).FirstOrDefault();
                session.Query<DBModels.BankAccount>().Where(x => x.UserID == currentUser.Id && x.ID == BankAccountID).Delete();
                session.Query<DBModels.Transaction>().Where(x => x.UserID == currentUser.Id && x.PlaidAccountID == dbBankAccountToDelete.PlaidAccountID).Delete();
                trans.Commit();
            }
        }

        public static Models.User CheckUserCredentials(ISession session, string authorization)
        {
            if (authorization != null && authorization.StartsWith("Basic"))
            {
                string encodedUsernamePassword = authorization.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                int seperatorIndex = usernamePassword.IndexOf(':');
                string username = usernamePassword.Substring(0, seperatorIndex);
                string password = usernamePassword.Substring(seperatorIndex + 1);
                var dbUser = session.Query<DBModels.User>().Where(x => x.Username == username && x.PasswordHash == HttpUtility.UrlDecode(password)).ToList().FirstOrDefault();
                if (dbUser == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                return DBModelUserToModelUser(dbUser);
            }
            else
            {
                throw new Exception("The authorization header is either empty or isn't Basic.");
            }
            return null;
        }

        /*[Route("getUser/{Username}/{PasswordHash}")]
        [HttpGet]
        public Models.UserAccount GetUser([FromUri] string Username, [FromUri] string PasswordHash)
        {
            using (ISession session = SessionFactory.GetCurrentSession())
            {
                var userAccount = session.Query<DBModels.UserAccount>().Where(x => x.Username == Username && x.Password == PasswordHash).ToList().FirstOrDefault();
                if (userAccount != null)
                {
                    return DBModelUserAccountToModelUserAccount(userAccount);
                }
                else
                {
                    return null;
                }
            }
        }*/

        private static Models.User DBModelUserToModelUser(DBModels.User dbUser)
        {
            return new Models.User()
            {
                Id = dbUser.ID,
                Username = dbUser.Username,
                PasswordHash = dbUser.PasswordHash,
                Email = dbUser.Email,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                PhoneNumber = dbUser.PhoneNumber,
                Admin = dbUser.Admin
            };
        }
    }
}
