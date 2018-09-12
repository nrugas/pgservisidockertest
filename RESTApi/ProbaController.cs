using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi
{
    public class ProbaController : ApiController
    {
        static PGParking metode = new PGParking();
        // GET: api/Proba
        public IEnumerable<string> Get()
        {
            List<string> ret = new List<string>();
            foreach (var s in metode.Sektori("Lokacije")) ret.Add(s.NazivSektora);
            return ret;
        }

        // GET: api/Proba/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Proba
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Proba/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Proba/5
        public void Delete(int id)
        {
        }
    }
}
