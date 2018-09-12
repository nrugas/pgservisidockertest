using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PG.Servisi.RESTApi_prometno
{
    public class __Prekrsaj : _Prekrsaj
    {
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public int Total { get; set; }
        [DataMember]
        public List<string> Slike { get; set; }
        [DataMember]
        public _Lokacija Lokacija { get; set; }
        




        public __Prekrsaj(int idp, int idr, int idt, int? idopp, int? idoz, int idlok, int iddjel, int iddok, decimal lat, decimal lng, DateTime dv, string reg, string red, string bi, string uid, string adr,
            string bd, string ter, string dok, string op, string ko, string oz, string cp, string c, string cpauka, string kaz, bool? pa, bool zahtjev, bool? sto, bool? te, int? trajanje, string so, string os, string ns,
            _KomentarPostupanja kom, string voz, string svpp, string drzava, int? idRacuna, string racun, _Nalog nalog, byte[] slika, bool zakonskaSankcija):
            base(idp, idr, idt, idopp, idoz, idlok, iddjel, iddok, lat, lng, dv, reg, red, bi, uid, adr,
            bd, ter, dok, op, ko, oz, cp, c, cpauka, kaz,  pa,  zahtjev,  sto,  te,  trajanje, so, os, ns,
            kom, voz, svpp, drzava,  idRacuna, racun, nalog)
        {

            ZakonskaSankcija = zakonskaSankcija;
            Lokacija = null;

            if (slika != null && slika.Length > 0)
            {
                Slike = new List<string>();
                
                try
                {
                    Bitmap b = new Bitmap(new MemoryStream(slika.ToArray()));
                    int maxPixelDimension = 240;
                    int w = b.Width, h = b.Height;

                    if (w > h)
                    {
                        h = h * maxPixelDimension / w;
                        w = maxPixelDimension;
                    }
                    else
                    {
                        w = w * maxPixelDimension / h;
                        h = maxPixelDimension;
                    }
                    Image.GetThumbnailImageAbort myCallback = ThumbnailCallback;
                    Image myThumbnail = b.GetThumbnailImage(w, h, myCallback, IntPtr.Zero);
                    var bytes = new MemoryStream();
                    myThumbnail.Save(bytes, ImageFormat.Jpeg);
                    Slike.Add(Convert.ToBase64String(bytes.ToArray()));
                }
                catch (Exception ex)
                {
                    //Sustav.SpremiGresku(grad, ex, idAplikacije, "Thumbnail slikice...");
                }
            }
        }

        private bool ThumbnailCallback()
        {
            return false;
        }
    }

}