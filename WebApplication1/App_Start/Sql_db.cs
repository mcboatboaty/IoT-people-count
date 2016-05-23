using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace WebApplication1.App_Start
{
    public class Sql_db
    {
        private static SqlConnection sql = new SqlConnection("Server=tcp:mcboatboatydbserver.database.windows.net,1433;" +                //username of sql server
                                   "Database=mcboatboatyDB;" +                                                //username of sql database  
                                   "User ID=mcboatboaty@mcboatboatydbserver;" +                               //ID of database
                                   "Password=WhyNotBoth2;" +                                                  //Password of database               /*Connection String of DB*/
                                   "Trusted_Connection=False;" +                                              //Trusted connection flag
                                   "Encrypt=True;" +                                                          //Encryption flad
                                   "Connection Timeout=30;" +                                                 //Max timeout of connection
                                   "MultipleActiveResultSets=True;");
        private static Sql_db sql_DB;

        private Sql_db()
        {
        }

        public static Sql_db get_DBInstance
        {
            get
            {
                if(sql_DB == null)
                {
                    sql_DB = new Sql_db();
                }
                return sql_DB;
            }
        }

        public SqlConnection getDBConn()
        {
            sql.Open();
            return sql;
        }
    }
}