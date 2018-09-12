using PG.Servisi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestPlateDefinition;
using System.Threading.Tasks;
using System.Drawing;

namespace PG.Servisi.RESTApi_prometno
{
    public class LPRPrometnoController : ApiController
    {
       
        
        JProperty CountryPlates(PlateDefinition.CountryResultSet r)
        {
            JArray plates = new JArray();
            foreach (var p in r.Plates)
            {
                plates.Add(p.Plate.ToString());
            }
            return new JProperty(r.Country, plates);
        }

        JObject Recognize(string pic64, bool prepoznajVozilo, bool koordinateTablice)
        {
            var ret = new JObject();
            try
            {
                var drzave = MethodsPrometno.Drzave();
                string secretKey = "sk_b1572dbc0c19945677074b5e";
                //Client openALPRClient = new Client();
                //Response x = await openALPRClient.RecognizeBytesAsync(value, secretKey, Recognize_vehicle._1, "eu", null, Return_image._0, 10, null);
                JObject vozilo;
                Rectangle plateRect = new Rectangle();
                var lprs = PG.Servisi.RESTApi.OpenALRPApi.Recognize(pic64, prepoznajVozilo, ref plateRect, out vozilo);
                //var lprs = Methods.LPR(value);
                if (!string.IsNullOrEmpty(lprs.Item1))
                {
                    String drz = "";
                    if (!string.IsNullOrEmpty(lprs.Item2))
                    {
                        string dd = lprs.Item2;
                        if (lprs.Item2.Trim().Length > 2) dd = lprs.Item2.Substring(lprs.Item2.Length - 2).ToUpper();
                        foreach (var d in drzave)
                        {
                            if (d.ISOKratica == dd)
                            {
                                drz = d.Kratica;
                            }
                        }
                    }
                    ret = AnalyzeLPR(lprs.Item1, drz);
                    if (vozilo != null && vozilo.Count > 0) ret.Add(new JProperty("vozilo", vozilo));
                    if (koordinateTablice && !plateRect.IsEmpty)
                    {
                        JObject tablica = new JObject();
                        tablica.Add("x", plateRect.X);
                        tablica.Add("y", plateRect.Y);
                        tablica.Add("width", plateRect.Width);
                        tablica.Add("height", plateRect.Height);
                        ret.Add(new JProperty("tablica", tablica));
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPrometno.SpremiGresku("", ex, "LPR CONTROLLER");
                throw ex;
            }
            return ret;
        }

        JObject AnalyzeLPR(string reg, string drz)
        {
            var ret = new JObject();
            try
            {
                if (!string.IsNullOrEmpty(reg))
                {
                    if (drz == null) drz = "";
                    PlateDefinition.CountryResultSet[] res = PlateDefinition.SearchResults(reg);
                    foreach (var r in res)
                    {
                        if (r.Country.ToUpper() == drz.ToUpper()) ret.Add(CountryPlates(r));
                    }
                    foreach (var r in res)
                    {
                        if (r.Country.ToUpper() != drz.ToUpper()) ret.Add(CountryPlates(r));
                    }
                }
            }
            catch (Exception ex)
            {
                MethodsPrometno.SpremiGresku("", ex, "LPR CONTROLLER");
                throw ex;
            }
            return ret;
        }


        // GET: api/LPR
        [Route("Prometno/api/LPR/{guid}/{reg}")]
        [HttpGet]
        public Object Get(string guid, string reg)
        {
            return AnalyzeLPR(reg, "");
        }

        // GET: api/LPR
        [Route("Prometno/api/LPR/{guid}/{reg}/{drz}")]
        [HttpGet]
        public Object Get(string guid, string reg, string drz)
        {
            return AnalyzeLPR(reg, drz);
        }


        // POST: api/LPR
        [Route("Prometno/api/LPR/{guid}")]
        [HttpPost]
        public JObject Post(string guid, [FromBody]string value)
        {
            var ret = Recognize(value, true, false);
            return ret;
        }

        // POST: api/LPR
        [Route("Prometno/api/LPR/{guid}/{recognizeVehicle}")]
        [HttpPost]
        public JObject Post(string guid, int recognizeVehicle, [FromBody]string value)
        {
            var ret = Recognize(value, recognizeVehicle > 0, false);
            return ret;
        }



        // POST: api/LPR
        [Route("Prometno/api/LPR/{guid}/{recognizeVehicle}/{tablica}")]
        [HttpPost]
        public JObject Post(string guid, int recognizeVehicle, int tablica, [FromBody]string value)
        {
            var ret = Recognize(value, recognizeVehicle > 0, tablica > 0);
            return ret;
        }



        // PUT: api/LPR/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/LPR/5
        public void Delete(int id)
        {
        }
    }
}
