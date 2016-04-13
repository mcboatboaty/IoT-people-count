using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    /*This is a test controller ONLY and has nothing to do with the functionality of the client-server protocol
    feel free to play around with it to test some results*/

    public class SivanController : ApiController
    {
        // GET: api/Sivan
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Sivan/5
        public string Get(int id)
        {
            if(id<0 || id > 10)
            {
                return "$COUNTER";
            }
            return "N/A";
        }

        // POST: api/Sivan
        public string Post(String id,[FromBody] string value)
        {
            return id;
        }

        // PUT: api/Sivan/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Sivan/5
        public void Delete(int id)
        {
        }
    }
}
