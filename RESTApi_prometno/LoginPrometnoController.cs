using PG.Servisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;


namespace PG.Servisi.RESTApi_prometno
{
    public class LoginPrometnoController : ApiController
    {
        // GET: api/Login
        [Route("Prometno/api/Login")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Login/5
        [Route("Prometno/api/Login/{aktivacijskiKod}")]
        [HttpGet]
        public _Grad Get(string aktivacijskiKod)
        {
            return MethodsPrometno.Aktivacija(aktivacijskiKod);
        }




        
        // POST: api/Login
        [Route("Prometno/api/Login")]
        [HttpPost]
        public Authorization Post([FromBody] Prijava value)
        {
            Authorization ret = null;
            bool blokiranaJLS;
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] originalBytes = Encoding.Default.GetBytes(value.Password);
                byte[] encodedBytes = md5.ComputeHash(originalBytes);
                var d = MethodsPrometno.Prijava(value.City_ID, value.UID, BitConverter.ToString(encodedBytes), out blokiranaJLS);
                if (d != null)
                {
                    ret = new Authorization();
                    ret.Djelatnik = d;
                    ret.Date = DateTime.Now.Date;
                    ret.GUID = Guid.NewGuid().ToString();
                    ret.City = MethodsPrometno.Grad(value.City_ID);
                    if (!string.IsNullOrEmpty(value.DeviceID))
                    {
                        var t = MethodsPrometno.GetTerminal(ret.City.Baza, value.DeviceID, value.DeviceName);
                        ret.TerminalName = t.Naziv;
                        ret.TerminalID = t.IDTerminala;
                        if (!string.IsNullOrEmpty(value.ProgramVersion))
                        {
                            MethodsPrometno.UpdateVerzija(ret.City.Baza, value.DeviceID, value.ProgramVersion, value.OSVersion);
                        }
                    }
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
                MethodsPrometno.SpremiGresku(city, ex, "Prijava - "+uid);
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
