using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi
{
    public class SektoriController : ApiController
    {
        // GET: api/Sektori
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("api/Sektori/{guid}")]
        [HttpGet]
        public JArray Get(string guid)
        {
            var ret = new JArray();
            foreach (var vp in Methods.Sektori(guid)) ret.Add(JObject.FromObject(vp));
            return ret;
        }

        // POST: api/Sektori
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Sektori/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Sektori/5
        public void Delete(int id)
        {
        }
    }
}
