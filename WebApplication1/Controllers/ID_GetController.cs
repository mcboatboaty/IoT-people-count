using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;

namespace WebApplication1.Controllers
{
    public class ID_GetController : ApiController
    {
        // GET: api/ID_Get
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ID_Get/5
        public string Get(string id)
        {
            SqlConnection sql = App_Start.Sql_db.get_DBInstance.getDBConn();
            SqlCommand sqlCmd = new SqlCommand("SELECT COUNT(*) from Counter where ID like @id", sql);
            sqlCmd.Parameters.AddWithValue("@id", id);

            //execute the search command
            int found = (int)sqlCmd.ExecuteScalar();
            sql.Close();
            sqlCmd.Dispose();

            if (found == 0)
            {
                sql = App_Start.Sql_db.get_DBInstance.getDBConn();
                sqlCmd = new SqlCommand("INSERT INTO Counter (ID, line) Values (@id, @count)", sql);
                sqlCmd.Parameters.AddWithValue("@id", id);
                sqlCmd.Parameters.AddWithValue("@count", "0");
                sqlCmd.ExecuteNonQuery();
                sql.Close();
            }
            return id;
        }
    }
}
