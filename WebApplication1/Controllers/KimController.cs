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

namespace WebApplication1.Controllers
{
    public class KimController : ApiController
    { 
        // GET: api/Kim
        //This API call is currently not used in the protocol
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Kim/5
        //This API call is client oriented: calls for the deployed Web App and manages a read only request.
        //This API is responsible for returning the counter size to the requesting client.
        public string Get(string id)
        {
            string output = "";
            SqlConnection sql = App_Start.Sql_db.get_DBInstance.getDBConn();
            //try and read the entries from the table
            try
            {
                SqlDataReader myReader = null;

                //get relevant row from table according to given ID
                SqlCommand myCmd = new SqlCommand("select * from Counter where ID = @id", sql);
                myCmd.Parameters.AddWithValue("@id", id);

                try
                {
                    myReader = myCmd.ExecuteReader();
                }
                catch
                {
                    return "Error reading from table, terminating.";
                }

                myReader.Read();
                output = myReader["ID"].ToString() + " : " + myReader["line"].ToString();
                sql.Close();
                return output;
            }

            //An error occured while retrieving data from sql table
            catch
            {
                return "Error retrieving data from table, terminating.";
            }
        }

        // POST: api/Kim
        //This API call is RaspberryPi oriented: calls for deployed Web App and edits entries in database.
        //this POST controller lets a RaspberryPi send a body containing a <string, int> pair representing ID of line and count of people, updates database accordingly.
        public string Post([FromBody]JObject value)
        {
            SqlConnection sql = App_Start.Sql_db.get_DBInstance.getDBConn();

            //container for value to update
            int line_update=0;

            SqlDataReader myReader = null;

            //Deserialize POST request packet from json to dictionary
            var values = JsonConvert.DeserializeObject<Dictionary<string, int>>(value.ToString());

            //iterate over each entry in this dictionary
            foreach(KeyValuePair<string,int> entry in values)
            {
                //retrieve the current people in line based on given device ID
                SqlCommand myCmd = new SqlCommand("select * from Counter where ID = @id", sql);
                myCmd.Parameters.AddWithValue("@id", entry.Key);

                try
                {
                    myReader = myCmd.ExecuteReader();
                }
                catch
                {
                    return ("an error has occured while reading from table");
                }
                myReader.Read();
                line_update = int.Parse(myReader["line"].ToString());

                //calculate the new count of people in line
                line_update = line_update + entry.Value;

                //must close sql connection to reset
                sql.Close();

                //open connection
                sql = App_Start.Sql_db.get_DBInstance.getDBConn();

                //update entry
                myCmd = new SqlCommand("UPDATE Counter SET line = @ln Where ID = @id", sql);
                myCmd.Parameters.AddWithValue("@ln", line_update.ToString());
                myCmd.Parameters.AddWithValue("@id", entry.Key);
                try
                {
                    myCmd.ExecuteNonQuery();
                }
                catch(SqlException ex)
                {
                    return ex.Message;
                }

                //end of update
                sql.Close();
            }

            //returns the final updates value
            return line_update.ToString();
        }


        //sql_handler(string cmd): simple sql execute method << receives an sql command >> executes command on open sql connection
        public void sql_handler(string cmd)
        {
            SqlCommand myCommand = new SqlCommand(cmd, App_Start.Sql_db.get_DBInstance.getDBConn());
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
