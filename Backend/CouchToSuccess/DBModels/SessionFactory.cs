// Credits to Cole W at https://stackoverflow.com/questions/7454589/no-session-bound-to-the-current-context
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Context;
using System.Reflection;
using System.Web.Configuration;

namespace CouchToSuccess.DBModels
{
    public class SessionFactory
    {
        private static object _SessionFactoryLock = new object();
        private static ISessionFactory sessionFactory;

        private static ISessionFactory GetSessionFactory()
        {
            lock (_SessionFactoryLock)
            {
                if (sessionFactory == null)
                {
                    sessionFactory = Fluently.Configure().Database(
                        MySQLConfiguration.Standard.ConnectionString(
                            c => c.Server(WebConfigurationManager.AppSettings["DBServer"]).Database(WebConfigurationManager.AppSettings["DBName"]).Username(WebConfigurationManager.AppSettings["DBUsername"]).Password(WebConfigurationManager.AppSettings["DBPassword"])
                        )
                    ).ExposeConfiguration(c => c.SetProperty("current_session_context_class", "web")).Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly())).BuildSessionFactory();
                }
            }
            return sessionFactory;
        }

        public static ISession GetCurrentSession()
        {
            if (!CurrentSessionContext.HasBind(GetSessionFactory()))
            {
                CurrentSessionContext.Bind(GetSessionFactory().OpenSession());
            }

            return GetSessionFactory().GetCurrentSession();
        }

    }
}