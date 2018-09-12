using PG.Servisi;
using PG.Servisi.resources.podaci.upiti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace PG.Servisi.RESTApi
{
    public class LoginController : ApiController
    {
        // GET: api/Login
        public IEnumerable<string> Get()
        {
            return new string[] { "Parking", "LoginControler" };
        }

        // GET: api/Login/5
        [Route("api/Login/{aktivacijskiKod}")]
        [HttpGet]
        public _Grad Get(string aktivacijskiKod)
        {
            return Methods.Aktivacija(aktivacijskiKod);
        }

        [Route("api/Smjena/{guid}/{id}")]
        [HttpGet]
        public string Get(string guid, int id)
        {
            return Methods.IzvjestajSmjene(guid, id, 0);
        }

        [Route("api/Smjena/{guid}/{id}/{tipPrintera}")]
        [HttpGet]
        public string Get(string guid, int id, int tipPrintera)
        {
            return Methods.IzvjestajSmjene(guid, id, tipPrintera);
        }


        // POST: api/Login
        public Authorization Post([FromBody]Prijava value)
        {
            Authorization ret = null;
            bool blokiranaJLS;
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] originalBytes = Encoding.Default.GetBytes(value.Password);
                byte[] encodedBytes = md5.ComputeHash(originalBytes);
                var d = Methods.Prijava(value.City_ID, value.UID, BitConverter.ToString(encodedBytes), out blokiranaJLS);
                if (d != null)
                {
                    ret = new Authorization();
                    ret.Djelatnik = d;
                    ret.Date = DateTime.Now.Date;
                    ret.GUID = Guid.NewGuid().ToString();
                    ret.City = Methods.Grad(value.City_ID);
                    if (!string.IsNullOrEmpty(value.DeviceID))
                    {
                        var t = Methods.GetTerminal(ret.City.Baza, value.DeviceID, value.DeviceName);
                        ret.TerminalName = t.Naziv;
                        ret.TerminalID = t.IDTerminala;
                        if (!string.IsNullOrEmpty(value.ProgramVersion))
                        {
                            Methods.UpdateVerzija(ret.City.Baza, value.DeviceID, value.ProgramVersion, value.OSVersion);
                        }
                    }
                    //Methods.Authorized.Add(ret.GUID, ret);
                }
            }
            catch (Exception ex)
            {
                string city = "", uid = "";
                if(value != null)
                {
                    city = value.City_ID;
                    uid = value.UID;
                }
                Methods.SpremiGresku(city, ex, "Prijava - "+uid);
            }
            return ret;   
        }

        //// PUT: api/Login/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/Login/5
        //public void Delete(int id)
        //{
        //}
    }
}
