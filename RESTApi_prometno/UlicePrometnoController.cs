using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi_prometno
{
    public class UlicePrometnoController : ApiController
    {
        // GET: api/Ulice
        [Route("Prometno/api/Ulice/{guid}/{lat}/{lon}")]
        [HttpGet]
        public Object Get(string guid, double lat, double lon)
        {
            return MethodsPrometno.PopisUlica(guid, lat, lon).ToArray();
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
