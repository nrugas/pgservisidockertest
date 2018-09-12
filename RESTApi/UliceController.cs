using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi
{
    public class UliceController : ApiController
    {
        // GET: api/Ulice
        [Route("api/Ulice/{guid}/{lat}/{lon}")]
        [HttpGet]
        public Object Get(string guid, double lat, double lon)
        {
            return Methods.PopisUlica(guid, lat, lon).ToArray();
        }

       
        // POST: api/Ulice
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Ulice/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Ulice/5
        public void Delete(int id)
        {
        }
    }
}
