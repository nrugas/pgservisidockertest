using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using PG.Servisi.resources.podaci.upiti;
using TestPlateDefinition;
using Newtonsoft.Json.Linq;
using PG.Servisi;

namespace PG.Servisi.RESTApi
{
    public class TestController : ApiController
    {
        // GET: api/Test
        [Route("api/Test/LPR/{plate}")]
        [HttpGet]
        public Object Get(string plate)
        {
            return Mapa.PozicijeRedaraMobile(plate, 5, 1);
        }

        // GET: api/Test/5
        public string Get(int id)
        {
            decimal izn;
            string pred = "TESTING...";
            //_Opazanje op = new _Opazanje(0, 0, null, 109, null, null, "", "", "RI123AA", "HR", DateTime.Now, null, null, null, false);
            //string pred = Methods.Kazni(Methods.grad, op, null, 1, id, out izn);
            return pred;
        }

        // POST: api/Test
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Test/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Test/5
        public void Delete(int id)
        {
        }
    }
}
