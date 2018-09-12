using PG.Servisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Text;
using PG.Servisi.resources.podaci.upiti;
using PG.Servisi.PazigradPCService;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using PG.Servisi.resources.podaci.baze;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.cs.sigurnost;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.Globalization;
using PG.Servisi.resources.cs.ispis;

namespace PG.Servisi.RESTApi_prometno
{
    
    public class Envelope
    {
        public string GUID { get; set; }
    }

    public class Authorization : Envelope
    {
        public _Djelatnik Djelatnik { get; set; }
        public int TerminalID { get; set; }
        public string TerminalName { get; set; }
        public _Grad City { get; set; }
        public DateTime Date { get; set; }
    }

    public class Prijava : Envelope
    {
        public string UID { get; set; }
        public string Password { get; set; }
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string ProgramVersion { get; set; }
        public string OSVersion { get; set; }
        public string City_ID { get; set; }
        public DateTime Date { get; set; }

    }


    public static class MethodsPrometno
    {
        static PGMobile servisPGMobile = new PGMobile();

        static Dictionary<string, Envelope> authorized = new Dictionary<string, Envelope>();

        public static Dictionary<string, Envelope> Authorized
        {
            get { return authorized; }
        }



        /// <summary>
        /// Udaljenost dvije točke
        /// </summary>
        /// <param name="lat1">Latitude 1. točke</param>
        /// <param name="lon1">Longitude 1. točke</param>
        /// <param name="lat2">Latitude 2. točke</param>
        /// <param name="lon2">Longitude 2. točke</param>
        /// <returns>udaljenost u metrima</returns>
        /// 
        public static double Udaljenost(double lat1, double lon1, double lat2, double lon2)
        {
            return Rez(lat1, lon1, lat2, lon2, 'K') * 1000;
        }

        private static double Rez(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }




        private const int idAplikacije = 1; //todo
        private const int idRedarstva = 1; //todo

        internal static string grad = "Lokacije";


        public static List<LokalneAdrese> PopisUlica(string grad, double latitude, double longitude)
        {
            return PopisUlica(grad, latitude, longitude, idAplikacije);
        }

        public static List<LokalneAdrese> PopisUlica(string grad, double latitude, double longitude, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    NumberFormatInfo nf = new NumberFormatInfo();
                    nf.NumberDecimalSeparator = ".";
                    string query = string.Format(@"SELECT MIN(ID) as ID, Ulica, lat, long, MAX(Kbr) as Kbr
  FROM LokalneAdrese
  where ROUND(Lat, 3) = {0} and  ROUND(Long, 3) = {1} 
  group by Ulica, lat, long
  order by Ulica", Math.Round(latitude, 3).ToString(nf), Math.Round(longitude, 3).ToString(nf));
                    var result =
                    
                        db.ExecuteQuery<LokalneAdrese>(query);
                    /*
  db.LokalneAdreses.OrderBy(i => i.Ulica).GroupBy(i => i.Ulica);
                    */
                    List< LokalneAdrese> nova = new List<LokalneAdrese>();

                    //foreach (var qq in result)
                    //{
                    //List<_DetaljiLokalneAdrese> det = new List<_DetaljiLokalneAdrese>();

                    foreach (var q in result)
                    {
                        // det.Add(new _DetaljiLokalneAdrese(adr.ID, adr.Lat, adr.Long, adr.Kbr));

                        //if (Udaljenost((double)q.Lat, (double)q.Long, 45.334649, 14.432134) < 50)
                        //{
                            bool ok = true;
                            if (q.Ulica.Contains("KRIŽANJE")) q.Kbr = "";
                            foreach (var u in nova) if (u.Ulica  == q.Ulica && u.Kbr == q.Kbr) { ok = false; break; }
                            if (ok)
                            {
                                var a = new LokalneAdrese();
                                a.ID = q.ID;
                                a.Lat = q.Lat;
                                a.Long = q.Long;
                                a.Ulica = q.Ulica;
                                a.Kbr = q.Kbr;
                                nova.Add(a);
                            }
                        //}
                    }
                    //}

                    return nova;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, "Popis Ulica");
                return new List<LokalneAdrese>();
            }
        }

        public static _Grad Aktivacija(string aktivacijskiKod)
        {
            return Sustav.AktivacijaAplikacije(aktivacijskiKod, idAplikacije);
        }

        public static _Grad Grad(string cityID)
        {
            return Gradovi.Grad(cityID, idAplikacije);
        }

        public static void SpremiGresku(string grad, Exception ex, string msg)
        {
            Sustav.SpremiGresku(grad, ex, idAplikacije, msg);
        }


        public static _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS)
        {
            return Korisnici.Prijava(grad, korisnickoIme, zaporka, idRedarstva, out blokiranaJLS, idAplikacije);
        }

        public static List<_Drzava> Drzave()
        {
            return Sustav.Drzave(idAplikacije);
        }

        public static string SpremiPrekrsaj(string grad, __Prekrsaj prekrsaj, List<byte[]> slike, out int idLokacije)
        {
            idLokacije = -1;
            try
            {
                if (prekrsaj.Lokacija != null)
                {
                    idLokacije = prekrsaj.IDLokacije = SpremiLokaciju(grad, prekrsaj.Lokacija);
                }
                int idp = -1;
                _NoviPrekrsaj np = KreirajNoviPrekrsaj(prekrsaj);
                if (prekrsaj.IDLokacije > 0 && np != null)
                {
                    idp = Mobile.NoviPrekrsaj(grad, np, idRedarstva, idAplikacije);
                }
                if (idp > 0)
                {
                    Prekrsaj.DodajSliku(grad, np.IDLokacije, slike, idRedarstva, idAplikacije);
                    return Ispis.Predlozak(grad, np.IDLokacije, 1, 0, idAplikacije);
                }
            }catch(Exception ex)
            {
                SpremiGresku(grad, ex, "SPREMI PREKRSAJ");
            }
            return "";
        }

        static _NoviPrekrsaj KreirajNoviPrekrsaj(__Prekrsaj p)
        {
            _NoviPrekrsaj ret = null;
            try
            {
                int statusOcitanja = 0;
                switch (p.StatusOcitanja)
                {
                    case "I":
                        statusOcitanja = 3;
                        break;
                    case "O":
                        statusOcitanja = 1;
                        break;
                    case "R":
                        statusOcitanja = 2;
                        break;
                }
                ret = new _NoviPrekrsaj(0, p.IDLokacije, (int)p.IDOpisaPrekrsaja, p.IDOpisaZakona == null ? 0 : (int)p.IDOpisaZakona, p.Lokacija != null? p.Lokacija.IDPozicioniranja:0, 0,
                    p.IDDjelatnika, p.DatumVrijeme, p.Registracija, "", p.Latitude, p.Longitude, p.Adresa, 0 , p.BrojDokumenta, p.Nalog != null, p.Nalog != null ? (int)p.Nalog.IDNaloga : 0,
                    p.Drzava, statusOcitanja, "", p.Trajanje != null ? (int)p.Trajanje : 0);
                ret.ZakonskaSankcija = p.ZakonskaSankcija;

            } catch (Exception ex)
            {
                SpremiGresku("", ex, "KREIRAJ NOVI PREKRSAJ");
            }
            return ret;

        }




        public static int DodajSliku(string grad, int idLokacije, List<byte[]> slike)
        {
            return Prekrsaj.DodajSliku(grad, idLokacije, slike, idRedarstva, idAplikacije).Count;
        }

        public static JArray DohvatiSlike(string grad, int idLokacije)
        {
            
            JArray ret = new JArray();
            
            foreach (var slika in Prekrsaj.Slike(grad, idLokacije, idAplikacije))
            {
                JObject slikaZaSlanje = new JObject();
                var s = Convert.ToBase64String(slika);
                slikaZaSlanje.Add("Slika", s);
                slikaZaSlanje.Add("ID", s.GetHashCode());

                ret.Add(slikaZaSlanje);
            }
            
            return ret;
        }

        public static _Prekrsaj DohvatiPrekrsaj(string grad, int idLokacije)
        {
            return Prekrsaj.DetaljiPrekrsaja(grad, idLokacije, idAplikacije);
        }

        public static __Prekrsaj[] DohvatiPrekrsaje(string grad, string registracija, int filter, int idDjelatnika, int index, int count)
        {
            return PretraziPrekrsaje(grad, registracija, filter, idDjelatnika, DateTime.Now.Date, idRedarstva, index, count);
        }

        /*
        public static JArray PretraziPrekrsaje(string grad, string registracija, int filter, int idDjelatnika, DateTime datum, int idRedarstva, int index, int count)
        {
            bool nalogpauku, obavijesti, upozorenja;
            obavijesti = (filter & 0x01) != 0;
            upozorenja = (filter & 0x02) != 0;
            nalogpauku = (filter & 0x04) != 0;
            
            if (count <= 0) count = int.MaxValue;
            bool contains = false;
            if (registracija != null && registracija.StartsWith("~"))
            {
                contains = true;
                registracija = registracija.Replace("~", "");
            }
            if (registracija == null)
            {
                registracija = "";
            }
            else registracija = registracija.Replace("-", "").Replace(" ", "").ToUpper().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D");

            try
            {
                JArray prekrsaji = new JArray();
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var prek = from p in db.Prekrsajis
                               join l in db.Lokacijes on p.IDLokacije equals l.IDLokacije
                               join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                               join t in db.Terminalis on l.IDTerminala equals t.IDTerminala into term
                               from tt in term.DefaultIfEmpty()
                               join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska into predlozak
                               from ii in predlozak.DefaultIfEmpty()
                               join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa into opis
                               from oo in opis.DefaultIfEmpty()
                               join r in db.PopisPrekrsajas on p.IDPrekrsaja equals r.IDPrekrsaja into popis
                               from rr in popis.DefaultIfEmpty()
                               join n in db.NaloziPaukus on p.IDNaloga equals n.IDNaloga into nalozi
                               from nn in nalozi.DefaultIfEmpty()
                               join s in db.StatusPaukas on nn.IDStatusa equals s.IDStatusa into statusi
                               from ss in statusi.DefaultIfEmpty()
                               join v in db.RazloziNepodizanjaVozilas on nn.IDRazloga equals v.IDRazloga into raz
                               from vv in raz.DefaultIfEmpty()
                               join z in db.Pauks on p.IDNaloga equals z.IDNaloga into pau
                               from zz in pau.DefaultIfEmpty()
                               join x in db.VozilaPaukas on nn.IDVozila equals x.IDVozila into voz
                               from xx in voz.DefaultIfEmpty()
                               join b in db.RACUNIs on nn.IDRacuna equals b.IDRacuna into rac
                               from bb in rac.DefaultIfEmpty()
                               join q in db.RACUNIs on p.IDRacuna equals q.IDRacuna into racP
                               from qq in racP.DefaultIfEmpty()
                               where (!string.IsNullOrEmpty(registracija) ? ((!contains) ? p.RegistracijskaPlocica.Replace("-", "").Replace(" ", "").ToUpper().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D") == registracija
                                     : p.RegistracijskaPlocica.Replace("-", "").Replace(" ", "").ToUpper().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D").Contains(registracija)) : registracija == "") &&
                                     (idDjelatnika > 0 ? p.IDDjelatnika == idDjelatnika : true) &&
                                     //(!obavijesti ? p.IDPredloskaIspisa != 2 : obavijesti) &&
                                     //(!upozorenja ? p.IDPredloskaIspisa != 1 : upozorenja) &&
                                     (
                                     (nalogpauku && p.NalogPauka.Value) ||
                                     (obavijesti && (ii.IDPRedloska == 15 || ii.IDPRedloska == 2)) ||
                                     (upozorenja && (ii.IDPRedloska == 14 || ii.IDPRedloska == 1))
                                     ) &&
                                     p.Vrijeme.Value.Date == datum.Date &&
                                     p.IDRedarstva == idRedarstva &&
                                     p.Status == false && p.Test == false
                               orderby p.Vrijeme.Value ascending
                               select new _Prekrsaj
                               (p.IDPrekrsaja,
                                   p.IDRedarstva,
                                   tt.IDTerminala,
                                   oo.IDOpisa,
                                   p.IDOpisaZakona,
                                   p.IDLokacije,
                                   (int)p.IDDjelatnika,
                                   p.IDPredloskaIspisa ?? -1,
                                   p.Lat,
                                   p.Long,
                                   p.Vrijeme.Value,
                                   Priprema.Registracija(p.RegistracijskaPlocica, p.KraticaDrzave),
                                   d.ImePrezime,
                                   d.BrojSI,
                                   string.IsNullOrEmpty(d.ImeNaRacunu) ? d.UID : d.ImeNaRacunu,
                                   p.Adresa,
                                   p.BrojUpozorenja,
                                   tt.IDTerminala == 0 ? "RUČNI UNOS" : tt.NazivTerminala,
                                   ii.NazivPredloska,
                                   oo.OpisPrekrsaja,
                                   oo.KratkiOpis,
                                   "",
                                   rr.MaterijalnaKaznjivaNorma,
                                   "",
                                   oo.ClanakPauka,
                                   p.Kazna.ToString(),
                                   p.NalogPauka,
                                   p.Zahtjev,
                                   p.Status,
                                   p.Test,
                                   p.TrajanjePostupka,
                                   Priprema.Ocitanje(p.StatusOcitanja),
                                   EncryptDecrypt.Decrypt(p.Tekst),
                                   EncryptDecrypt.Decrypt(p.Napomena),
                                   null,
                                   "", //ImaKazni(grad, p.RegistracijskaPlocica, idAplikacije).ToString(),
                                   p.StatusVPP ?? "",
                                   p.KraticaDrzave,
                                   p.IDRacuna,
                                   qq.BrojRacuna,
                                   Priprema.Nalog(p.IDNaloga, xx, ss, vv, nn, zz, bb.BrojRacuna, Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije))
                               );

                    //return Priprema.PripremiPodatke(grad, prek, idAplikacije); => NE KUŽIM ČEMU - VIDJETI S PAJOM...
                    
                    _Prekrsaj[] pks = prek.ToArray();

                     

                    for (int i = 0; i < prek.Count(); i++)
                    {
                        if (count > 0 && i >= index + count) break;
                        if (i >= index)
                        {
                            var p = JObject.FromObject(pks[i], new JsonSerializer
                            {
                                DateParseHandling = DateParseHandling.DateTime
                            });
                            p.Add("Index", i);
                            p.Add("Total", pks.Length);

                            prekrsaji.Add(p);
                        }

                    }

                    foreach (JObject p in prekrsaji)
                    {
                       JArray slike64 = new JArray();
                        var slike = from s in db.SlikaPrekrsajas
                                    where s.IDLokacije == (int) p.GetValue("IDLokacije")
                                    select s.Slika;
                        foreach (var slika in slike)
                        {
                            if (slika.Length > 0)
                            {
                                try
                                {
                                    //Bitmap b = new Bitmap(new MemoryStream(slika.ToArray()));
                                    //Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                                    //Image myThumbnail = b.GetThumbnailImage(64, 64, myCallback, IntPtr.Zero);
                                    //var bytes = new MemoryStream();
                                    //myThumbnail.Save(bytes, ImageFormat.Jpeg);
                                    //slike64.Add(Convert.ToBase64String(bytes.ToArray()));
                                    //break;
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
                                    slike64.Add(Convert.ToBase64String(bytes.ToArray()));
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Sustav.SpremiGresku(grad, ex, idAplikacije, "Thumbnail slikice...");
                                }
                            }
                        }
                        p.Add("Slike", slike64);
                    }
                }
                return prekrsaji;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pretrazi Prekrsaje (MethodsPrometno)");
                return new JArray();
            }
        }
        */


            public static __Prekrsaj[] PretraziPrekrsaje(string grad, string registracija, int filter, int idDjelatnika, DateTime datum, int idRedarstva, int index, int count)
        {
            bool nalogpauku, obavijesti, upozorenja;
            obavijesti = (filter & 0x01) != 0;
            upozorenja = (filter & 0x02) != 0;
            nalogpauku = (filter & 0x04) != 0;

            __Prekrsaj[] prekrsaji = new __Prekrsaj[0];
            System.Diagnostics.Debug.Print(DateTime.Now.ToString());
            if (count <= 0) count = int.MaxValue;
            bool contains = false;
            if (registracija != null && registracija.StartsWith("~"))
            {
                contains = true;
                registracija = registracija.Replace("~", "");
            }
            if (registracija == null)
            {
                registracija = "";
            }
            else registracija = registracija.Replace("-", "").Replace(" ", "").ToUpper().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D");

            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var prek = from p in db.Prekrsajis
                               join l in db.Lokacijes on p.IDLokacije equals l.IDLokacije
                               join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                               join t in db.Terminalis on l.IDTerminala equals t.IDTerminala into term
                               from tt in term.DefaultIfEmpty()
                               join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska into predlozak
                               from ii in predlozak.DefaultIfEmpty()
                               join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa into opis
                               from oo in opis.DefaultIfEmpty()
                               join r in db.PopisPrekrsajas on p.IDPrekrsaja equals r.IDPrekrsaja into popis
                               from rr in popis.DefaultIfEmpty()
                               join n in db.NaloziPaukus on p.IDNaloga equals n.IDNaloga into nalozi
                               from nn in nalozi.DefaultIfEmpty()
                               join s in db.StatusPaukas on nn.IDStatusa equals s.IDStatusa into statusi
                               from ss in statusi.DefaultIfEmpty()
                               join v in db.RazloziNepodizanjaVozilas on nn.IDRazloga equals v.IDRazloga into raz
                               from vv in raz.DefaultIfEmpty()
                               join z in db.Pauks on p.IDNaloga equals z.IDNaloga into pau
                               from zz in pau.DefaultIfEmpty()
                               join x in db.VozilaPaukas on nn.IDVozila equals x.IDVozila into voz
                               from xx in voz.DefaultIfEmpty()
                               join b in db.RACUNIs on nn.IDRacuna equals b.IDRacuna into rac
                               from bb in rac.DefaultIfEmpty()
                               join q in db.RACUNIs on p.IDRacuna equals q.IDRacuna into racP
                               from qq in racP.DefaultIfEmpty()
                               //join slike in db.SlikaPrekrsajas on p.IDLokacije equals slike.IDLokacije into slikeP
                               //from slikes in slikeP.Take(1).DefaultIfEmpty()
                               where (!string.IsNullOrEmpty(registracija) ? ((!contains) ? p.RegistracijskaPlocica.Replace("-", "").Replace(" ", "").ToUpper().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D") == registracija
                                     : p.RegistracijskaPlocica.Replace("-", "").Replace(" ", "").ToUpper().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D").Contains(registracija)) : registracija == "") &&
                                     (idDjelatnika > 0 ? p.IDDjelatnika == idDjelatnika : true) &&
                                     //(!obavijesti ? p.IDPredloskaIspisa != 2 : obavijesti) &&
                                     //(!upozorenja ? p.IDPredloskaIspisa != 1 : upozorenja) &&
                                     (
                                     (nalogpauku && p.NalogPauka.Value) ||
                                     (obavijesti && ii.Kaznjava) ||
                                     (upozorenja && !ii.Kaznjava)
                                     ) &&
                                     p.Vrijeme.Value.Date == datum.Date &&
                                     p.IDRedarstva == idRedarstva &&
                                     p.Status == false && p.Test == false
                               orderby p.Vrijeme.Value descending
                               select new __Prekrsaj
                               (p.IDPrekrsaja,
                                   p.IDRedarstva,
                                   tt.IDTerminala,
                                   oo.IDOpisa,
                                   p.IDOpisaZakona,
                                   p.IDLokacije,
                                   (int)p.IDDjelatnika,
                                   p.IDPredloskaIspisa ?? -1,
                                   p.Lat,
                                   p.Long,
                                   p.Vrijeme.Value,
                                   Priprema.Registracija(p.RegistracijskaPlocica, p.KraticaDrzave),
                                   d.ImePrezime,
                                   d.BrojSI,
                                   string.IsNullOrEmpty(d.ImeNaRacunu) ? d.UID : d.ImeNaRacunu,
                                   p.Adresa,
                                   p.BrojUpozorenja,
                                   tt.IDTerminala == 0 ? "RUČNI UNOS" : tt.NazivTerminala,
                                   ii.NazivPredloska,
                                   oo.OpisPrekrsaja,
                                   oo.KratkiOpis,
                                   "",
                                   rr.MaterijalnaKaznjivaNorma,
                                   "",
                                   oo.ClanakPauka,
                                   p.Kazna.ToString(),
                                   p.NalogPauka,
                                   p.Zahtjev,
                                   p.Status,
                                   p.Test,
                                   p.TrajanjePostupka,
                                   Priprema.Ocitanje(p.StatusOcitanja),
                                   EncryptDecrypt.Decrypt(p.Tekst),
                                   EncryptDecrypt.Decrypt(p.Napomena),
                                   null,
                                   "", //ImaKazni(grad, p.RegistracijskaPlocica, idAplikacije).ToString(),
                                   p.StatusVPP ?? "",
                                   p.KraticaDrzave,
                                   p.IDRacuna,
                                   qq.BrojRacuna,
                                   Priprema.Nalog(p.IDNaloga, xx, ss, vv, nn, zz, bb.BrojRacuna, Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije)),
                                   null, //(slikes.Slika != null)? slikes.Slika.ToArray():null
                                   ii.Kaznjava
                               );

                    //return Priprema.PripremiPodatke(grad, prek, idAplikacije); => NE KUŽIM ČEMU - VIDJETI S PAJOM...

                    __Prekrsaj[] pks = prek.ToArray();
                    System.Diagnostics.Debug.Print(DateTime.Now.ToString());
                    int len = (count > 0) ? Math.Min(count, pks.Length - index) : Math.Max(0, pks.Length - index);
                    prekrsaji = new __Prekrsaj[len];
                    Array.Copy(pks, index, prekrsaji, 0, len);

                    for (int i = 0; i < prekrsaji.Length; i++)
                    {
                        
                            __Prekrsaj p = prekrsaji[i];
                            p.Index = i+index;
                            p.Total = pks.Length;
                    }
                    System.Diagnostics.Debug.Print(DateTime.Now.ToString());
                   
                    foreach (__Prekrsaj p in prekrsaji)
                    {
                        List<string> slike64 = new List<string>();
                        var slika = (from s in db.SlikaPrekrsajas
                                    where s.IDLokacije == p.IDLokacije
                                    select s.Slika).FirstOrDefault();
                        //foreach (var slika in slike)
                        //{
                            if (slika != null && slika.Length > 0)
                            {
                                try
                                {
                                    //Bitmap b = new Bitmap(new MemoryStream(slika.ToArray()));
                                    //Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                                    //Image myThumbnail = b.GetThumbnailImage(64, 64, myCallback, IntPtr.Zero);
                                    //var bytes = new MemoryStream();
                                    //myThumbnail.Save(bytes, ImageFormat.Jpeg);
                                    //slike64.Add(Convert.ToBase64String(bytes.ToArray()));
                                    //break;
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
                                    slike64.Add(Convert.ToBase64String(bytes.ToArray()));
                                    //break;
                                }
                                catch (Exception ex)
                                {
                                    Sustav.SpremiGresku(grad, ex, idAplikacije, "Thumbnail slikice...");
                                }
                            }
                        //}
                        p.Slike = slike64;
                    }
                }
                System.Diagnostics.Debug.Print(DateTime.Now.ToString());
                return prekrsaji;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pretrazi Prekrsaje (MethodsPrometno)");
                return new __Prekrsaj[0];
            }
        }


        private static bool ThumbnailCallback()
        {
            return false;
        }

        public static List<_Zakon> DohvatiZakone(string grad)
        {
            return Zakoni.DohvatiZakoneS(grad, false, 1, false, idAplikacije);
        }





        public static bool PromijeniPodatkeOpazanja(string grad, _Opazanje opazanje)
        {
            return Parking.PromijeniPodatkeOpazanja(grad, opazanje, idAplikacije);
        }

        public static bool VoziloOtislo(string grad, int idOpazanja)
        {
            return Parking.VoziloOtislo(grad, idOpazanja, idAplikacije);
        }

        
        public static Tuple<string, decimal, string> Kazni(string grad, _Opazanje opazanje, int idStavke, int idVrstePlacanja, int kolicina, string poziv, int tipPrintera)
        {
            return Parking.NaplatiParking(grad, opazanje, idStavke, idVrstePlacanja, kolicina, poziv, tipPrintera, idAplikacije);
        }

        public static string StornirajKaznu(string grad, int idRacuna, int idDjelatnika)
        {
            return Naplata.StornirajRacunParking(grad, idRacuna, idDjelatnika, "STORNIRANO NA TERENU!", idAplikacije);
        }

        /*
      public static int StornirajZadnjiRacun(string grad, int idDjelatnika)
       {
           return servisPGParking.StornirajZadnjiRacun(grad, idDjelatnika, 4, idAplikacije);
       }
       */

        public static string KopijaDokumenta(string grad, int idLokacije, int tipPrintera)
        {
            return Ispis.Predlozak(grad, idLokacije, 1, 0, idAplikacije);
        }


        public static string UlazniTicket(int idGrada, int idSektora, string cijenaSat, string cijenaDnevna, string radnoVrijeme)
        {
            return Ispis.VrijemeDolaska(idGrada, idSektora, cijenaSat, cijenaDnevna, radnoVrijeme, idAplikacije);
        }

        /*
        public static List<_Zone> Zone(string grad)
        {
            return servisPGParking.Zone(grad);
        }

        public static JArray Sektori(string grad)
        {
            JArray ret = new JArray();
            foreach (var sek in servisPGParking.Sektori(grad))
            {
                JObject sektor = JObject.FromObject(sek);

                JArray cijenaSat = Parking.StavkeZone(grad, 0, sek.IDZone, idAplikacije);
                sektor.Add(new JProperty("CijenaSat", cijenaSat));
                JArray cijenaDnevna = Parking.StavkeZone(grad, 1, sek.IDZone, idAplikacije);
                sektor.Add(new JProperty("CijenaDnevna", cijenaDnevna));
                JArray cijenaPoludnevna = Parking.StavkeZone(grad, 2, sek.IDZone, idAplikacije);
                sektor.Add(new JProperty("CijenaPoludnevna", cijenaPoludnevna));
                JArray cijenaPretplata = Parking.StavkeZone(grad, -1, sek.IDZone, idAplikacije);
                sektor.Add(new JProperty("CijenaPretplata", cijenaPretplata));
                //JArray arr = new JArray();
                //foreach(var item in Parking.StavkeZone(grad, 0, (int) sek.IDZone, idAplikacije).Children())
                //{
                //    arr.Add(item); ;
                //}
                ret.Add(sektor);
            }
            return ret;
        }
        */

        public static List<_Printer> Printeri(string grad, int idRedarstva)
        {
            return servisPGMobile.Printeri(grad, idRedarstva);
        }

        public static List<_2DLista> Modeli()
        {
            return Parking.DohvatiModele();
        }

        /*:: NAPLATA ::*/

        public static List<_VrstaPlacanja> VrstePlacanja(string grad)
        {
            return servisPGMobile.VrstePlacanja(grad, null);
        }

        public static void Fiskaliziraj(string grad, int idRacuna, int idRedarstva)
        {
            Fiskalizacija.Fiskaliziraj(grad, idRacuna, idRedarstva, idAplikacije);
        }
/*
        public static string IzvjestajSmjene(string grad, int idKorisnika, int tipPrintera)
        {
            return Ispis.IzvjestajSmjeneParking(grad, idKorisnika, idRedarstva, DateTime.Today, idAplikacije, tipPrintera);
            //return servisPGParking.IzvjestajSmjene(grad, idKorisnika);
        }
*/
        public static string VrijemeDolaskaESC(int idGrada, int idSektora, string cijenaSat, string cijenaDnevna, string radnoVrijeme)
        {
            return Ispis.VrijemeDolaskaESC(idGrada, idSektora, cijenaSat, cijenaDnevna, radnoVrijeme, idAplikacije);
        }

        public static string VrijemeDolaska(int idGrada, int idSektora, string cijenaSat, string cijenaDnevna, string radnoVrijeme)
        {
            return Ispis.VrijemeDolaska(idGrada, idSektora, cijenaSat, cijenaDnevna, radnoVrijeme, idAplikacije);
        }

        public static string PostojiOdobrenje(string grad, string registracija)
        {
            return Postavke.PostojiOdobrenje(grad, registracija, idRedarstva, idAplikacije);
        }


        public static int SpremiLokaciju(string grad, _Lokacija lokacija)
        {
            SetTerminalAccessTime(grad, lokacija.IDTerminala);
            return Mobile.SpremiLokaciju(grad, lokacija, false, idAplikacije);
        }

        public static int? SetTerminalAccessTime(string grad, int terminalId)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Terminali r = db.Terminalis.First(i => i.IDTerminala == terminalId);
                    r.VrijemeZadnjegPristupa = DateTime.Now;
                    db.SubmitChanges();

                    return r.IDTerminala;
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Set Terminal Access Time in Methods Parking");
                return null;
            }
        }

        public static void UpdateVerzija(string grad, string deviceId, string progVer, string osVer)
        {
            Postavke.UpdateVerzija(grad, progVer, osVer, deviceId, false, idAplikacije);
        }

        public static _Terminal GetTerminal(string grad, string deviceID, string naziv)
        {
            var t = Postavke.GetTerminala(grad, deviceID, idAplikacije);
            if (t == null)
            {
                try
                {
                    _Terminal ter = new _Terminal(0, null, "", deviceID, naziv, "", null, false, false, false, false, false, true, false, DateTime.Now);
                    if (Postavke.DodajTerminalS(grad, ter, idAplikacije) > 0)
                    {
                        t = ter;
                    }
                }
                catch (Exception ex)
                {
                    Sustav.SpremiGresku(grad, ex, idAplikacije, "INSERT TERMINAL");
                    return null;
                }
            }
            return t;
        }
    }
}