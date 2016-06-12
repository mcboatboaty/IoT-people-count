using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PeopleCount.Controllers
{
    public class InfoGetController : ApiController
    { 
        // GET: api/Kim
        //This API call is currently not used in the protocol
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/InfoGet/{id}
        //This API call is client oriented: calls for the deployed Web App and manages a read only request.
        //This API is responsible for returning the counter size to the requesting client.
        public string Get(string id)
        {
            //string representation of output
            string output = "";

            //get an SQL connection instance
            SqlConnection sql = WebApiApplication.getSQL();

            //try and read the entries from the table
            SqlDataReader myReader = null;
            try
            {
                //get relevant row from table according to given ID
                SqlCommand myCmd = new SqlCommand("select * from CounterU where ID = @id", sql);
                myCmd.Parameters.AddWithValue("@id", id);

                try
                {
                    myReader = myCmd.ExecuteReader();
                }
                catch(Exception e)
                {
                    if (myReader != null)
                    {
                        myReader.Close();
                    }
                    myCmd.Dispose();
                    return e.Message;
                }

                //read the content of the data reader
                myReader.Read();

                //format the output string to contain Pi ID and counter value
                output = myReader["Label"].ToString() + " : " + myReader["line"].ToString();

                //dispose all used resources
                myReader.Close();
                myCmd.Dispose();

                return output;
            }

            //An error occured while retrieving data from sql table
            catch(Exception e)
            {
                if (myReader!=null)
                {
                    myReader.Close();
                }
                return e.Message;
            }
        }

        // POST: api/InfoGet
        //This API call is RaspberryPi oriented: calls for deployed Web App and edits entries in database.
        //this POST controller lets a RaspberryPi send a body containing a <string, int> pair representing ID of line and count of people, updates database accordingly.
        public string Post([FromBody]JObject value)
        {
            //get an SQL connection instance
            SqlConnection sql = WebApiApplication.getSQL();

            //container for value to update
            int line_update=0;

            SqlDataReader myReader = null;

            //Deserialize POST request
            String id = value.Property("ID").Value.ToString();
            int newArrivals = 0;
            try {
                newArrivals = int.Parse(value.Property("line").Value.ToString());
            }
            catch(Exception e)
            {
                return e.Message;
            }

            /*Manipulate DB according to receive parameters*/

            //retrieve the current people in line based on given device ID
            SqlCommand myCmd = new SqlCommand("select * from CounterU where ID = @id", sql);
            myCmd.Parameters.AddWithValue("@id", id);

            try
            {
                myReader = myCmd.ExecuteReader();
            }
            catch
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                myCmd.Dispose();
                return ("an error has occured while reading from table");
            }

            //read content of the data reader
            myReader.Read();

            //retrieve current counter on the current queried line
            line_update = int.Parse(myReader["line"].ToString());

            //calculate the new count of people in line
            line_update = line_update + newArrivals;

            //dispose the reader before moving up
            myReader.Close();

            //update entry
            myCmd = new SqlCommand("UPDATE CounterU SET line = @ln Where ID = @id", sql);
            myCmd.Parameters.AddWithValue("@ln", line_update.ToString());
            myCmd.Parameters.AddWithValue("@id", id);
            try
            {
                myCmd.ExecuteNonQuery();
            }
            catch(SqlException ex)
            {
                return ex.Message;
            }

            //end of update
            myCmd.Dispose();

            //returns the final updates value
            return line_update.ToString();
        }


        //sql_handler(string cmd): simple sql execute method << receives an sql command >> executes command on open sql connection
        public void sql_handler(string cmd)
        {
            SqlCommand myCommand = new SqlCommand(cmd, WebApiApplication.getSQL());
            try
            {
                myCommand.ExecuteNonQuery();
            }
            catch
            {
                return;
            }
        }
    }
}
