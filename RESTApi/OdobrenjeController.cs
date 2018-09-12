using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi
{
    public class OdobrenjeController : ApiController
    {
        [Route("api/Odobrenje/{guid}/{reg}")]
        [HttpGet]

        public string Get(string guid, string reg)
        {
            return Methods.PostojiOdobrenje(guid, reg);
        }
    }
}
