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

        //initiate a new connection to an existing sql data base 
        SqlConnection sql = new SqlConnection("Server=tcp:mcboatboatydbserver.database.windows.net,1433;" +                //username of sql server
                                                "Database=mcboatboatyDB;" +                                                //username of sql database  
                                                "User ID=mcboatboaty@mcboatboatydbserver;" +                               //ID of database
                                                "Password=WhyNotBoth2;" +                                                  //Password of database               /*Connection String of DB*/
                                                "Trusted_Connection=False;" +                                              //Trusted connection flag
                                                "Encrypt=True;" +                                                          //Encryption flad
                                                "Connection Timeout=30;");                                                 //Max timeout of connection

        // GET: api/Kim
        //This API call is currently not used in the protocol
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Kim/5
        //This API call is client oriented: calls for the deployed Web App and manages a read only request.
        //This API is responsible for returning the counter size to the requesting client.
        public string Get(int id)
        {
            //open the sql connection
            sql.Open();

            //define the sql command to create a 'Counter' table with ID and line as two column entries (in case the table doesn't exist, otherwise, do nothing.
            String cmd_create = "If not exists(select name from sysobjects where name = 'Counter') CREATE TABLE Counter(ID varchar(50), line int)";

            //execute the sql command
            sql_handler(cmd_create);

            /*String cmd_check = "INSERT INTO Counter(ID, line) Values ('Pi01', 5)";
            SqlCommand myCommand = new SqlCommand(cmd_check
                                     , sql);
            try
            {
                myCommand.ExecuteNonQuery();
            }
            catch
            {
                return Marshal.GetLastWin32Error().ToString();
            }*/

            //try to read entries from the table
            try
            {
                SqlDataReader myReader = null;
                SqlCommand myCmd = new SqlCommand("select * from Counter",
                                                         sql);
                try
                {
                    myReader = myCmd.ExecuteReader();
                }
                catch
                {
                    return "Error reading from table, terminating.";
                }
                myReader.Read();
                return myReader["ID"].ToString()+" : "+myReader["line"].ToString();
            }

            //An error occured while retrieving data from sql table
            catch (Exception e)
            {
                return "Error retrieving data from table, terminating.";
            }
        }

        // POST: api/Kim
        //This API call is RaspberryPi oriented: calls for deployed Web App and edits entries in database.
        //this POST controller lets a RaspberryPi send a body containing a <string, int> pair representing ID of line and count of people, updates database accordingly.
        public string Post([FromBody]JObject value)
        {
            //container for value to update
            int line_update=0;

            SqlDataReader myReader = null;

            //Deserialize POST request packet from json to dictionary
            var values = JsonConvert.DeserializeObject<Dictionary<string, int>>(value.ToString());

            //iterate over each entry in this dictionary
            foreach(KeyValuePair<string,int> entry in values)
            {
                //open sql connection
                sql.Open();

                //retrieve the current people in line
                SqlCommand myCmd = new SqlCommand("select * from Counter", sql);
                try
                {
                    myReader = myCmd.ExecuteReader();
                }
                catch
                {

                }
                myReader.Read();
                line_update = int.Parse(myReader["line"].ToString());

                //calculate the new count of people in line
                line_update = line_update + entry.Value;

                //must close sql connection to reset
                sql.Close();

                //reopen sql connection to update new value
                sql.Open();

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
            SqlCommand myCommand = new SqlCommand(cmd, sql);
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
