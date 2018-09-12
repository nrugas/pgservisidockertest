using Newtonsoft.Json.Linq;
using PG.Servisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi
{
    public class OpazanjaController : ApiController
    {

        [Route("api/Opazanja/{guid}/{idOpazanja}")]
        [HttpGet]
        public _Opazanje Get(string guid, int idOpazanja)
        {
            return Methods.TraziOpazanje(guid, idOpazanja);
        }

        // GET: api/Opazanja/5
        [Route("api/Opazanja/{guid}/{id}/{filter}/{idsek}/{index}/{count}")]
        [HttpGet]
        public IEnumerable<_Opazanje> Get(string guid, int id, int filter, int idSek, int index, int count)
        {
            return Methods.TraziOpazanje(guid, idSek, filter, id, index, count, "");
        }

        // GET: api/Opazanja/5
        [Route("api/Opazanja/{guid}/{id}/{filter}/{idsek}/{page}/{pagesize}/{reg}")]
        [HttpGet]
        public IEnumerable<_Opazanje> Get(string guid, int id, int filter, int idSek, string reg, int page, int pagesize)
        {
            return Methods.TraziOpazanje(guid, idSek, filter, id, page, pagesize, reg);
        }

        // GET: api/Opazanja/5
        [Route("api/Opazanja/{guid}/{id}/{filter}/{idsek}/{page}/{pagesize}/{reg}/{latitude}/{longitude}/{zastara}")]
        [HttpGet]
        public IEnumerable<_Opazanje> Get(string guid, int id, int filter, int idSek, string reg, int page, int pagesize, double latitude, double longitude, int zastara)
        {
            return Methods.TraziOpazanje(guid, idSek, filter, id, page, pagesize, reg, latitude, longitude, zastara);
        }




        // POST: api/Opazanja
        [Route("api/Opazanja/{guid}")]
        [HttpPost]
        public int Post(string guid, [FromBody] JObject value)
        {
            try
            {
                _Opazanje o = value.ToObject<_Opazanje>();
                List<string> slike64 = value["Slike"].ToObject<List<string>>();
                List<byte[]> slike = new List<byte[]>();
                foreach (var s in slike64)
                {
                    try { slike.Add(Convert.FromBase64String(s)); } catch { }
                }
                bool ret = Methods.SpremiOpazanje(guid, ref o, slike);
                return (ret) ? 1 : 0;
            }
            catch (Exception ex)
            {
                Methods.SpremiGresku(guid, ex, "SPREMI OPAZANJE CONTROLLER");
                return -1;
            }

        }

        // PUT: api/Opazanja/5
        [Route("api/Opazanja/{guid}/{id}")]
        [HttpPut]
        public int Put(string guid, int id, [FromBody]List<string> value)
        {
            int i = 0;
            List<byte[]> slike = new List<byte[]>();
            foreach (var s in value)
            {
                try { slike.Add(Convert.FromBase64String(s));
                    i++;
                } catch { }
            }
            if (i == 0) return 0;
            else if (Methods.DodajSlikuOpazanju(guid, id, slike))
            {
                return i;
            }
            else return 0;
        }

        // PUT: api/Opazanja/
        [Route("api/Opazanja/{guid}")]
        [HttpPut]
        public bool Put(string guid, [FromBody] _Opazanje value)
        {
            return Methods.PromijeniPodatkeOpazanja(guid, value);
        }

        // DELETE: api/Opazanja/5
        [Route("api/Opazanja/{guid}/{idOpazanja}")]
        [HttpDelete]
        public bool Delete(string guid, int idOpazanja)
        {
            return Methods.VoziloOtislo(guid, idOpazanja);
        }
    }
}
