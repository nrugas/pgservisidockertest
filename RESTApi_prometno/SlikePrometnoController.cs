using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi_prometno
{
    public class SlikePrometnoController : ApiController
    {
        // GET: api/Slike
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("Prometno/api/Slike/{guid}/{idLokacije}")]
        [HttpGet]
        public JArray Get(string guid, int idLokacije)
        {
            return MethodsPrometno.DohvatiSlike(guid, idLokacije);
        }

        // POST: api/Slike
        public void Post([FromBody]string value)
        {
        }

        
        [Route("Prometno/api/Slike/{guid}/{idLokacije}")]
        [HttpPut]
        public int Put(string guid, int idLokacije, [FromBody]List<string> value)
        {
            int i = 0;
            List<byte[]> slike = new List<byte[]>();
            foreach (var s in value)
            {
                try
                {
                    slike.Add(Convert.FromBase64String(s));
                    i++;
                }
                catch { }
            }
            if (i == 0) return 0;
            else return MethodsPrometno.DodajSliku(guid, idLokacije, slike);
        }


        // DELETE: api/Slike/5
        public void Delete(int id)
        {
        }
    }
}
