using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Data.SqlClient;

namespace PeopleCount
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static SqlConnection sql;

        protected void Application_Start()
        {
            sql = App_Start.Sql_db.get_DBInstance.getDBConn();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        public static SqlConnection getSQL()
        {
            return sql;
        }
    }
}
