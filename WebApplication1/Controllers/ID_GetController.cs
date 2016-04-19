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
            SqlConnection sql = WebApiApplication.getSQL();

            //count the number of rows containing the specific ID - we are expecting 0 or 1 assuming correctness
            SqlCommand sqlCmd = new SqlCommand("SELECT COUNT(*) from Counter where ID like @id", sql);
            sqlCmd.Parameters.AddWithValue("@id", id);

            //execute the search command
            int found = (int)sqlCmd.ExecuteScalar();

            //release the cmd resource
            sqlCmd.Dispose();

            //in short - if the ID wasn't found in the table, then we must initialize
            if (found == 0)
            {
                sqlCmd = new SqlCommand("INSERT INTO Counter (ID, line) Values (@id, @count)", sql);
                sqlCmd.Parameters.AddWithValue("@id", id);
                sqlCmd.Parameters.AddWithValue("@count", "0");

                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
            }

            //return the ID as confirmation
            return id;
        }
    }
}
