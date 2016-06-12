using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace PeopleCount.Controllers
{
    public class IdGetController : ApiController
    {
        // GET: api/ID_Get
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ID_Get/5
        public string Post([FromBody]JObject value)
        {
            SqlConnection sql = WebApiApplication.getSQL();
            int found = 0;
            String id = value.Property("ID").Value.ToString();
            String label = value.Property("Label").Value.ToString();

            //count the number of rows containing the specific ID - we are expecting 0 or 1 assuming correctness
            SqlCommand sqlCmd = new SqlCommand("SELECT COUNT(*) from CounterU where ID like @id", sql);
            sqlCmd.Parameters.AddWithValue("@id", id);

            try
            {
                //execute the search command
                found = (int)sqlCmd.ExecuteScalar();
            }
            catch
            {
                return null;
            }

            //release the cmd resource
            sqlCmd.Dispose();

            //in short - if the ID wasn't found in the table, then we must initialize
            if (found == 0)
            {
                sqlCmd = new SqlCommand("INSERT INTO CounterU (ID, line, Label) Values (@id, @count, @label)", sql);
                sqlCmd.Parameters.AddWithValue("@id", id);
                sqlCmd.Parameters.AddWithValue("@count", "0");
                sqlCmd.Parameters.AddWithValue("@label", label);

                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
            }

            //return the ID as confirmation
            return id;
        }
    }
}
