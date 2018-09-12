using PG.Servisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;



namespace PG.Servisi.RESTApi
{
    public class DrzaveController : ApiController
    {
        // GET: api/Drzave
        //public IEnumerable<_Drzava> Get()
        //{
        //    var l = Sustav.Drzave(mParking.idAplikacije);
        //    return l;
        //}

        public IEnumerable<_Drzava> Get()
        {
            var ret = new List<_Drzava>();
            var l = Methods.Drzave();
            foreach (var d in l)
            {
                ret.Add(d);
            }
            return ret;
        }

        // GET: api/Drzave/5
        [Route("api/Kratica/{kratica}")]
        [HttpGet]
        public string Get(string kratica)
        {
            string ret = null;
            var l = Methods.Drzave();
            foreach (var d in l)
            {
                if (d.Kratica.ToUpper() == kratica.ToString().ToUpper())
                {
                    ret = d.Drzava;
                    break;
                }
            }
            return ret;
        }


        // POST: api/Drzave
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Drzave/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Drzave/5
        public void Delete(int id)
        {
        }
    }
}
