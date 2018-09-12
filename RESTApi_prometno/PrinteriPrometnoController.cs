using Newtonsoft.Json.Linq;
using PG.Servisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi_prometno
{
    public class PrinteriPrometnoController : ApiController
    {
        
        // GET: api/Printeri/5
        [Route("Prometno/api/Printeri/{grad}/{id}")]
        [HttpGet]
        public JArray Get(string grad, int id)
        {
            JArray ret = new JArray();
            var ps = MethodsPrometno.Printeri(grad, id);
            var ms = MethodsPrometno.Modeli();
            var q = from p in ps join m in ms on p.IDModela equals m.Value select JObject.FromObject(new { IDPrintera = p.IDPrintera, Naziv = p.Naziv, MAC = p.MAC, PIN = p.PIN, InterniBroj = p.InterniBroj, Model = m.Text });
            foreach (var r in q) ret.Add(r);
            //foreach(var r in q)
            //{
            //    JObject printer = new JObject();
            //    printer.Add(new JProperty("IDPrintera", r.IDPrintera));
            //    printer.Add(new JProperty("Naziv", r.Naziv));
            //    printer.Add(new JProperty("InterniBroj", r.InterniBroj));
            //    printer.Add(new JProperty("MAC", r.MAC));
            //    printer.Add(new JProperty("Model", r.Model));
            //    ret.Add(printer);
            //}
            return ret;
        }

        //// POST: api/Printeri
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/Printeri/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/Printeri/5
        //public void Delete(int id)
        //{
        //}
    }
}
