using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi_prometno
{
    public class PrekrsajiController : ApiController
    {

        // GET: api/Prekrsaji
        [Route("Prometno/api/Prekrsaji/Zakoni/{grad}")]
        [HttpGet]
        public IEnumerable<_Zakon> Get(string grad)
        {
            //return MethodsPrometno.DohvatiPrekrsaje("PROMETNIK_RIJEKA", "", 7, 0, 0, 40);
            return MethodsPrometno.DohvatiZakone(grad);
        }

        [Route("Prometno/api/Prekrsaji/{guid}/{idLokacije}")]
        [HttpGet]
        public _Prekrsaj Get(string guid, int idLokacije)
        {
            return MethodsPrometno.DohvatiPrekrsaj(guid, idLokacije);
        }


        // GET: api/Prekrsaji/5
        //[Route("Prometno/api/Prekrsaji/{id}")]
        //[HttpGet]
        //public Prekrsaj[] Get(string id)
        //{
        //    return MethodsPrometno.PretraziPrekrsaje(id, "", 7, 0, DateTime.Now.Date.AddDays(-1), 1, 0, 40);
        //}



        [Route("Prometno/api/Prekrsaji/{guid}/{id}/{filter}/{index}/{pagesize}/{reg}")]
        [HttpGet]
        public __Prekrsaj[] Get(string guid, int id, int filter, string reg, int index, int pagesize)
        {
            return MethodsPrometno.DohvatiPrekrsaje(guid, reg, filter, id, index, pagesize);
        }


        [Route("Prometno/api/Prekrsaji/{guid}/{id}/{filter}/{index}/{pagesize}")]
        [HttpGet]
        public __Prekrsaj[] Get(string guid, int id, int filter, int index, int pagesize)
        {
            return MethodsPrometno.DohvatiPrekrsaje(guid, null, filter, id, index, pagesize);
        }

        /*
        // POST: api/Opazanja
        [Route("Prometno/api/Prekrsaji/{guid}")]
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
                bool ret = true; // Methods.SpremiOpazanje(guid, ref o, slike);
                return (ret) ? 1 : 0;
            }
            catch (Exception ex)
            {
                MethodsPrometno.SpremiGresku(guid, ex, "SPREMI OPAZANJE CONTROLLER");
                return -1;
            }

        }
        */


        
        [Route("Prometno/api/Prekrsaji/{guid}")]
        [HttpPost]
        public JObject Post(string guid, [FromBody] JObject value)
        {
            JObject ret = new JObject();
            try
            {
                __Prekrsaj p = value.ToObject<__Prekrsaj>();
                List<string> slike64 = p.Slike; // value["Slike"].ToObject<List<string>>();
                List<byte[]> slike = new List<byte[]>();
                if (slike64 != null)
                {
                    foreach (var s in slike64)
                    {
                        try { slike.Add(Convert.FromBase64String(s)); } catch { }
                    }
                }
                
                var ispis = MethodsPrometno.SpremiPrekrsaj(guid, p, slike, out int idLokacije);
                ret.Add("Ispis", ispis);
                ret.Add("IDLokacije", idLokacije);
            }
            catch (Exception ex)
            {
                MethodsPrometno.SpremiGresku(guid, ex, "SPREMI PREKRSAJ CONTROLLER");
            }
            return ret;
        }
        

        // POST: api/Prekrsaji
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Prekrsaji/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Prekrsaji/5
        public void Delete(int id)
        {
        }
    }
}
