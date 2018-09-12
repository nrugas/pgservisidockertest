using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi
{
    public class SlikeController : ApiController
    {
        // GET: api/Slike
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("api/Slike/{guid}/{idLokacije}")]
        [HttpGet]
        public JArray Get(string guid, int idLokacije)
        {
            return Methods.DohvatiSlike(guid, idLokacije);
        }

        // POST: api/Slike
        public void Post([FromBody]string value)
        {
        }

        
        [Route("api/Slike/{guid}/{idOpazanja}")]
        [HttpPut]
        public int Put(string guid, int idOpazanja, [FromBody]List<string> value)
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
            else if (Methods.DodajSlikuOpazanju(guid, idOpazanja, slike))
            {
                return i;
            }
            else return 0;
        }


        // DELETE: api/Slike/5
        public void Delete(int id)
        {
        }
    }
}
