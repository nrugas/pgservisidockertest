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

namespace PG.Servisi.RESTApi
{
    public class LPRNoviController : ApiController
    {
        // GET: api/LPR
        [Route("api/LPRNovi/{guid}/{reg}")]
        [HttpGet]
        public Object Get(string guid, string reg)
        {
            return AnalyzeLPR(reg, "");
        }

        // GET: api/LPR
        [Route("api/LPRNovi/{guid}/{reg}/{drz}")]
        [HttpGet]
        public Object Get(string guid, string reg, string drz)
        {
            return AnalyzeLPR(reg, drz);
        }

        //// GET: api/LPR/5
        //[Route("api/LPR/{image}")]
        //[HttpGet]
        //public JObject Get(string image)
        //{
        //    var ret = new JObject();
        //    //var lprs = Methods.LPR(image);
        //    //if (lprs != null && lprs.Length > 0)
        //    //{
        //    //    PlateDefinition.CountryResultSet[] res = PlateDefinition.SearchResults(lprs[0]);
        //    //    foreach (var r in res)
        //    //    {

        //    //        JArray plates = new JArray();
        //    //        foreach (var p in r.Plates)
        //    //        {
        //    //            plates.Add(p.Plate.ToString());
        //    //        }
        //    //        JProperty jp = new JProperty(r.Country, plates);
        //    //        ret.Add(jp);
        //    //    }
        //    //}
        //    return ret;
        //}

        /*
    // POST: api/LPR
    [Route("api/LPROLD/{guid}")]
    [HttpPost]
    public JObject Post(string guid, [FromBody]string value)
    {
        var ret = new JObject();
        var lprs = Methods.LPR(value);
        if (lprs != null && lprs.Length > 0)
        {
            PlateDefinition.CountryResultSet[] res = PlateDefinition.SearchResults(lprs[0]);
            foreach (var r in res)
            {

                JArray plates = new JArray();
                foreach (var p in r.Plates)
                {
                    plates.Add(p.Plate.ToString());
                }
                JProperty jp = new JProperty(r.Country, plates);
                ret.Add(jp);
            }
        }
        return ret;
    }
    */
        JProperty CountryPlates(PlateDefinition.CountryResultSet r)
        {
            JArray plates = new JArray();
            
            foreach (var p in r.Plates)
            {
                //plates.Add(p.Plate.ToString());
                plates.Add(JObject.FromObject(p));
            }
            return new JProperty(r.Country, plates);
        }

        JObject Recognize(string pic64, bool prepoznajVozilo, bool koordinateTablice)
        {
            var ret = new JObject();
            try
            {
                var drzave = Methods.Drzave();
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
            }catch(Exception ex)
            {
                Methods.SpremiGresku("", ex, "LPR CONTROLLER");
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
                Methods.SpremiGresku("", ex, "LPR CONTROLLER");
                throw ex;
            }
            return ret;
        }



        // POST: api/LPR
        [Route("api/LPRNovi/{guid}")]
        [HttpPost]
        public JObject Post(string guid, [FromBody]string value)
        {

            var ret = Recognize(value, true, false);
            /*
            var ret = new JObject();
            string secretKey = "sk_b1572dbc0c19945677074b5e";
            //Client openALPRClient = new Client();
            //Response x = await openALPRClient.RecognizeBytesAsync(value, secretKey, Recognize_vehicle._1, "eu", null, Return_image._0, 10, null);
            JObject vozilo;
            var lprs = PG.Servisi.RESTApi.OpenALRPApi.Recognize(value, true, out vozilo);
            //var lprs = Methods.LPR(value);
            if (lprs != null && lprs.Length > 0)
            {
                PlateDefinition.CountryResultSet[] res = PlateDefinition.SearchResults(lprs[0]);
                foreach (var r in res)
                {

                    JArray plates = new JArray();
                    foreach (var p in r.Plates)
                    {   
                        plates.Add(p.Plate.ToString());
                    }
                    JProperty jp = new JProperty(r.Country, plates);
                    ret.Add(jp);
                }
                ret.Add(new JProperty("vozilo", vozilo));
            }
            */
            
            return ret;
        }

        // POST: api/LPR
        [Route("api/LPRNovi/{guid}/{recognizeVehicle}")]
        [HttpPost]
        public JObject Post(string guid, int recognizeVehicle, [FromBody]string value)
        {
            var ret = Recognize(value, recognizeVehicle > 0, false);
            return ret;
        }



        // POST: api/LPR
        [Route("api/LPRNovi/{guid}/{recognizeVehicle}/{tablica}")]
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
