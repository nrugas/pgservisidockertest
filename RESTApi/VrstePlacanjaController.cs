using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi
{
    public class VrstePlacanjaController : ApiController
    {
        // GET: api/VrstePlacanja
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/VrstePlacanja/5
        [Route("api/VrstePlacanja/{guid}")]
        [HttpGet]
        public JArray Get(string guid)
        {
            var ret = new JArray();
            foreach (var vp in Methods.VrstePlacanja(guid)) ret.Add(JObject.FromObject(vp));
            return ret;
        }

        // POST: api/VrstePlacanja
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/VrstePlacanja/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/VrstePlacanja/5
        public void Delete(int id)
        {
        }
    }
}
