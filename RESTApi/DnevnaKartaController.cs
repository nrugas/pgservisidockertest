using PG.Servisi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi.RESTApi
{
    public class DnevnaKartaController : ApiController
    {

        /*
        // GET: api/DnevnaKarta
        [Route("api/VrijemeDolaska/{idGrada}/{idSektora}/{cijenaSat}/{cijenaDnevna}/{radnoVrijeme}")]
        [HttpGet]
        
        public string Get(int idGrada, int idSektora, string cijenaSat, string cijenaDnevna, string radnoVrijeme)
        {
            return Methods.VrijemeDolaska(idGrada, idSektora, cijenaSat, cijenaDnevna, radnoVrijeme);
        }
        */

        // GET: api/DnevnaKarta
        [Route("api/VrijemeDolaska/{idGrada}/{idSektora}/{cijenaSat}/{cijenaDnevna}/{radnoVrijeme}/{tipPrintera}")]
        [HttpGet]

        public string Get(int idGrada, int idSektora, string cijenaSat, string cijenaDnevna, string radnoVrijeme, int tipPrintera)
        {
            if(tipPrintera == 0) return Methods.VrijemeDolaska(idGrada, idSektora, cijenaSat, cijenaDnevna, radnoVrijeme);
            else return Methods.VrijemeDolaskaESC(idGrada, idSektora, cijenaSat, cijenaDnevna, radnoVrijeme);
        }


        


        // GET: api/DnevnaKarta/5
        [Route("api/Kazni/{guid}/{idracuna}")]
        [HttpGet]
        public string Get(string guid, int idracuna)
        {
            return Methods.KopijaRacuna(guid, idracuna, 0);
        }


        // GET: api/DnevnaKarta/5
        [Route("api/PK/{guid}/{idracuna}/{tipPrintera}")]
        [HttpGet]
        public string Get(string guid, int idracuna, int tipPrintera)
        {
            return Methods.KopijaRacuna(guid, idracuna, tipPrintera);
        }


        // POST: api/DnevnaKarta
        [Route("api/Kazni/{guid}/{idStavke}/{kolicina}/{vp}")]
        [HttpPost]
        public String Post(string guid, int idStavke, int kolicina, int vp, [FromBody] JObject value)
        {
            _Opazanje o = value.ToObject<_Opazanje>();
            string poziv = "";
            var p = value.GetValue("PozivNaBroj");
            if (p != null && !string.IsNullOrEmpty(p.ToString())) poziv = p.ToString();
            decimal iznos = 0;
            if (value["Slike"] != null)
            {
                List<string> slike64 = value["Slike"].ToObject<List<string>>();
                List<byte[]> slike = new List<byte[]>();
                foreach (var s in slike64)
                {
                    try { slike.Add(Convert.FromBase64String(s)); } catch { }
                }
                Methods.DodajSlikuOpazanju(guid, o.IDOpazanja, slike);
            }
            var resp = Methods.Kazni(guid, o, idStavke, vp, kolicina, poziv, 0);
            return resp.Item1;
            
        }

        // POST: api/DnevnaKarta
        [Route("api/PK/{guid}/{idStavke}/{kolicina}/{vp}/{tipPrintera}")]
        [HttpPost]
        public String Post(string guid, int idStavke, int kolicina, int vp, int tipPrintera, [FromBody] JObject value)
        {
            JToken p = null;
            try
            {
                _Opazanje o = value.ToObject<_Opazanje>();
                string poziv = "";
                p = value.GetValue("PozivNaBroj");
                
                if (p != null && !string.IsNullOrEmpty(p.ToString())) poziv = p.ToString();
                
                if (value["Slike"] != null)
                {
                    List<string> slike64 = value["Slike"].ToObject<List<string>>();
                    List<byte[]> slike = new List<byte[]>();
                    foreach (var s in slike64)
                    {
                        try { slike.Add(Convert.FromBase64String(s)); } catch { }
                    }
                    if(slike.Count > 0 && o!= null && o.IDOpazanja > 0) Methods.DodajSlikuOpazanju(guid, o.IDOpazanja, slike);
                }
                var resp = Methods.Kazni(guid, o, idStavke, vp, kolicina, poziv, 0);
                if(p != null && !string.IsNullOrEmpty(resp.Item3))
                {
                    return "ERROR:"+resp.Item3;
                }
                else return resp.Item1;
            }
            catch(Exception ex)
            {
                Methods.SpremiGresku(guid, ex, "DNEVNA KARTA CONTROLLER");
                return (p == null)? "":"ERROR:"+ex.Message;
            }
            

        }

        // PUT: api/DnevnaKarta/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/DnevnaKarta/5
        [Route("api/Kazni/{guid}/{idRacuna}/{idDjelatnika}")]
        [HttpDelete]
        public string Delete(string guid, int idRacuna, int idDjelatnika)
        {
            return Methods.StornirajKaznu(guid, idRacuna, idDjelatnika);
        }


        // DELETE: api/DnevnaKarta/5
        [Route("api/PK/{guid}/{idRacuna}/{idDjelatnika}/{tipPrintera}")]
        [HttpDelete]
        public string Delete(string guid, int idRacuna, int idDjelatnika, int tipPrintera)
        {
            return Methods.StornirajKaznu(guid, idRacuna, idDjelatnika);
        }
    }
}
