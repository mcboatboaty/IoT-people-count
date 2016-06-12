using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PeopleCount.Controllers
{
    public class GetAllController : ApiController
    {
        // GET: api/GetAll
        //This API call retrieves all active Raspberry Pi devices ID and returns a list object containing all the ID's
        public Dictionary<string,string> Get()
        {
            //dynamic list to hold all retrieved ID's from the database
            List<string> allIDs = new List<string>();
            Dictionary<string, string> AllIds = new Dictionary<string, string>();

            //get an SQL connection instance
            SqlConnection sql = WebApiApplication.getSQL();

            //try and read the entries from the table
            SqlDataReader myReader = null;
            try
            {
                //get everything in the Counter tables
                SqlCommand myCmd = new SqlCommand("select * from CounterU", sql);

                try
                {
                    myReader = myCmd.ExecuteReader();
                }
                catch (Exception e)
                {
                    if (myReader != null)
                    {
                        myReader.Close();
                    }
                    myCmd.Dispose();
                    return new Dictionary<string,string>() { { e.Message, e.Message } };
                }

                //read every entry from the reader
                while (myReader.Read())
                {
                    //append the found id in the output list
                    //allIDs.Add(myReader["ID"].ToString());
                    AllIds.Add(myReader["ID"].ToString(), myReader["Label"].ToString());
                }

                //dispose all used resources
                myReader.Close();
                myCmd.Dispose();

                return AllIds;
            }

            //An error occured while retrieving data from sql table
            catch (Exception e)
            {
                if (myReader != null)
                {
                    myReader.Close();
                }

                //return the error message
                return new Dictionary<string, string>() { { e.Message, e.Message } };
            }
        }
    }
}
