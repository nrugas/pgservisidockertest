using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi_prometno
{
    public class LokacijeController : ApiController
    {
        /*
        // GET: api/Lokacije
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Lokacije/5
        public string Get(int id)
        {
            return "value";
        }
        */

        // POST: api/Lokacije
        [Route("Prometno/api/Lokacije/{guid}")]
        [HttpPost]
        public int Post(string guid, [FromBody] JObject value)
        {
            _Lokacija lok = value.ToObject<_Lokacija>();
            return MethodsPrometno.SpremiLokaciju(guid, lok);
        }


        /*
        // PUT: api/Lokacije/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Lokacije/5
        public void Delete(int id)
        {
        }
        */
    }
}
