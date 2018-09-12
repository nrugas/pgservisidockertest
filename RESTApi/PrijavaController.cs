using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi
{
    public class PrijavaController : ApiController
    {

        // POST: api/Opazanja
        [Route("api/Prijava/{guid}")]
        [HttpPost]
        public int Post(string guid, [FromBody] JObject value)
        {
            try
            {
                __Opazanje o = value.ToObject<__Opazanje>();
                List<string> slike64 = value["Slike"].ToObject<List<string>>();
                List<byte[]> slike = new List<byte[]>();
                foreach (var s in slike64)
                {
                    try { slike.Add(Convert.FromBase64String(s)); } catch { }
                }
                return Methods.Prijava(guid, o, slike);
            }
            catch (Exception ex)
            {
                Methods.SpremiGresku(guid, ex, "PRIJAVE CONTROLLER (vanjsko postupanje)");
                return -1;
            }

        }

    }
}
