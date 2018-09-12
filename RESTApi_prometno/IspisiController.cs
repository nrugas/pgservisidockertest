using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PG.Servisi.RESTApi_prometno
{
    public class IspisiController : ApiController
    {

       
        [Route("Prometno/api/Ispisi/Kopija/{guid}/{idLokacije}/{tipPrintera}")]
        [HttpGet]
        public string Post(string guid, int idLokacije, int tipPrintera)
        {
            return MethodsPrometno.KopijaDokumenta(guid, idLokacije, tipPrintera);
        }


    }
}
