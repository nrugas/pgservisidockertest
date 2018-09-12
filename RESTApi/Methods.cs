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
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using PG.Servisi.resources.podaci.baze;
using PG.Servisi.resources.cs;
using System.Globalization;

namespace PG.Servisi.RESTApi
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

    public static class Methods
    {
        static PGParking servisPGParking = new PGParking();

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



        private const int idAplikacije = 13; //todo
        private const int idRedarstva = 4; //todo

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
                    List<LokalneAdrese> nova = new List<LokalneAdrese>();

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
                        foreach (var u in nova) if (u.Ulica == q.Ulica && u.Kbr == q.Kbr) { ok = false; break; }
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


        public static __Opazanje[] TraziOpazanje(string grad, string registracija, bool samoopazanja, int? idDjelatnika, int? idSektora, int idAplikacije)
        {
            int filter = (samoopazanja) ? 3 : 7;
            return TraziOpazanje(grad, registracija, filter, idDjelatnika, idSektora, 0, -1, idAplikacije);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grad"></param>
        /// <param name="registracija"></param>
        /// <param name="filter">
        /// bit 0 -> opažanja kojima NIJE prošao "grace period"
        /// bit 1 -> opažanja kojima JE prošao "grace period"
        /// bit 2 -> izdane parkirne karte
        /// </param>
        /// <param name="idDjelatnika"></param>
        /// <param name="idSektora"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="idAplikacije"></param>
        /// <returns></returns>
        public static __Opazanje[] TraziOpazanje(string grad, string registracija, int filter, int? idDjelatnika, int? idSektora, int index, int count, int idAplikacije)
        {
            int zastaraOpazanja = 4; // broj sati nakon kojih ne prikazuje opažanje
            bool op1, op2, kaznjeni;
            op1 = (filter & 0x01) != 0;
            op2 = (filter & 0x02) != 0;
            kaznjeni = (filter & 0x04) != 0;
            if (count <= 0) count = int.MaxValue;

            try
            {
                List<_2DLista> statusi = Parking.Statusi(false, idAplikacije);

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

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var op = from o in db.PARKING_OPAZANJAs
                             join d in db.Djelatniks on o.IDDjelatnika equals d.IDDjelatnika into djelatnik
                             from dd in djelatnik.DefaultIfEmpty()
                             join s in db.PARKING_SEKTORIs on o.IDSektora equals s.IDSektora into sektori
                             from ss in sektori.DefaultIfEmpty()
                             join z in db.PARKING_ZONEs on ss.IDZone equals z.IDZone into zone
                             from zz in zone.DefaultIfEmpty()
                             where (!string.IsNullOrEmpty(registracija) ? ((!contains) ? o.Registracija.Replace("-", "").Replace(" ", "").ToUpper().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D") == registracija
                             : o.Registracija.Replace("-", "").Replace(" ", "").ToUpper().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D").Contains(registracija)) : registracija == "") &&
                                   ((DateTime.Now - o.Vrijeme.Value).TotalHours <= zastaraOpazanja || (o.PlacenoDo != null && o.PlacenoDo.Value > DateTime.Now)) &&
                                   (o.Otisao == null || o.Otisao.Value != true) &&
                                   //(o.PlacenoDo == null || DateTime.Now < o.PlacenoDo) &&
                                   (
                                   (kaznjeni && o.Kaznjen == true) ||
                                   (op1 && o.Kaznjen != true && (DateTime.Now - o.Vrijeme.Value).TotalMinutes < zz.GracePeriod) ||
                                   (op2 && o.Kaznjen != true && (DateTime.Now - o.Vrijeme.Value).TotalMinutes >= zz.GracePeriod)
                                   ) &&
                                   (idDjelatnika != null && idDjelatnika > 0 ? o.IDDjelatnika == idDjelatnika : true) &&
                                   (idSektora == null || idSektora <= 0 ? true : o.IDSektora == idSektora)
                             orderby o.Vrijeme descending
                             select new __Opazanje(o.IDOpazanja, o.IDLokacije, o.IDSektora, ss.IDZone, o.IDDjelatnika, 0, o.IDStatusa, o.IDRacuna, ss.NazivSektora, dd.ImePrezime, zz.NazivZone,
                                 Parking.Status(o.IDStatusa, statusi), o.Registracija, o.Drzava, (DateTime)o.Vrijeme, o.PlacenoDo, o.Latitude, o.Longitude, o.Iznos, o.Kaznjen.Value, (o.Otisao == null) ? false : o.Otisao.Value, "", "");




                    __Opazanje[] opazanja;
                    __Opazanje[] ops = op.ToArray();

                   

                    int len = (count > 0) ? Math.Min(count, ops.Length - index) : Math.Max(0, ops.Length - index);
                    opazanja = new __Opazanje[len];
                    Array.Copy(ops, index, opazanja, 0, len);

                    for (int i = 0; i < opazanja.Length; i++)
                    {

                        __Opazanje o = opazanja[i];
                        o.Index = i + index;
                        o.Total = ops.Length;
                    }

                    foreach (var opazanje in opazanja)
                    {
                        List<string> slike64 = new List<string>();
                        var slika = (from s in db.SlikaPrekrsajas
                                    where s.IDLokacije == opazanje.IDLokacije
                                    select s.Slika).FirstOrDefault();
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
                        
                        opazanje.Slike =  slike64;
                    }
                    return opazanja;
                }

            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRETRAGA OPAZANJA");
                return new __Opazanje[0];
            }

        }

        private static bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grad"></param>
        /// <param name="registracija"></param>
        /// <param name="filter">
        /// bit 0 -> opažanja kojima NIJE prošao "grace period"
        /// bit 1 -> opažanja kojima JE prošao "grace period"
        /// bit 2 -> izdane parkirne karte
        /// </param>
        /// <param name="idDjelatnika"></param>
        /// <param name="idSektora"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="idAplikacije"></param>
        /// <returns></returns>
        public static __Opazanje[] TraziOpazanje(string grad, string registracija, int filter, int? idDjelatnika, int? idSektora, int index, int count, double latitude, double longitude, int zastara, int idAplikacije)
        {
            int zastaraOpazanja = zastara>0? zastara:4; // broj sati nakon kojih ne prikazuje opažanje
            bool op1, op2, kaznjeni;
            op1 = (filter & 0x01) != 0;
            op2 = (filter & 0x02) != 0;
            kaznjeni = (filter & 0x04) != 0;
            if (count <= 0) count = int.MaxValue;

            try
            {
                List<_2DLista> statusi = Parking.Statusi(false, idAplikacije);

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

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var op = from o in db.PARKING_OPAZANJAs
                             join d in db.Djelatniks on o.IDDjelatnika equals d.IDDjelatnika into djelatnik
                             from dd in djelatnik.DefaultIfEmpty()
                             join s in db.PARKING_SEKTORIs on o.IDSektora equals s.IDSektora into sektori
                             from ss in sektori.DefaultIfEmpty()
                             join z in db.PARKING_ZONEs on ss.IDZone equals z.IDZone into zone
                             from zz in zone.DefaultIfEmpty()
                             where (!string.IsNullOrEmpty(registracija) ? ((!contains) ? o.Registracija.Replace("-", "").Replace(" ", "").ToUpper().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D") == registracija
                             : o.Registracija.Replace("-", "").Replace(" ", "").ToUpper().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D").Contains(registracija)) : registracija == "") &&
                                   ((DateTime.Now - o.Vrijeme.Value).TotalHours <= zastaraOpazanja || (o.PlacenoDo != null && o.PlacenoDo.Value > DateTime.Now)) &&
                                   (o.Otisao == null || o.Otisao.Value != true) &&
                                   //(o.PlacenoDo == null || DateTime.Now < o.PlacenoDo) &&
                                   (
                                   (kaznjeni && o.Kaznjen == true) ||
                                   (op1 && o.Kaznjen != true && (DateTime.Now - o.Vrijeme.Value).TotalMinutes < zz.GracePeriod) ||
                                   (op2 && o.Kaznjen != true && (DateTime.Now - o.Vrijeme.Value).TotalMinutes >= zz.GracePeriod)
                                   ) &&
                                   (idDjelatnika != null && idDjelatnika > 0 ? o.IDDjelatnika == idDjelatnika : true) &&
                                   (idSektora == null || idSektora <= 0 ? true : o.IDSektora == idSektora)
                             select new __Opazanje(o.IDOpazanja, o.IDLokacije, o.IDSektora, ss.IDZone, o.IDDjelatnika, 0, o.IDStatusa, o.IDRacuna, ss.NazivSektora, dd.ImePrezime, zz.NazivZone,
                                 Parking.Status(o.IDStatusa, statusi), o.Registracija, o.Drzava, (DateTime)o.Vrijeme, o.PlacenoDo, o.Latitude, o.Longitude, o.Iznos, o.Kaznjen.Value, (o.Otisao == null) ? false : o.Otisao.Value, "", "");




                    __Opazanje[] opazanja;
                    __Opazanje[] ops = op.ToArray();

                    Array.Sort(ops, delegate (__Opazanje o1, __Opazanje o2) {
                        double d1 = Udaljenost(latitude, longitude, (double)o1.Latitude, (double)o1.Longitude);
                        double d2 = Udaljenost(latitude, longitude, (double)o2.Latitude, (double)o2.Longitude);
                        return d1.CompareTo(d2);
                    });

                    int len = (count > 0) ? Math.Min(count, ops.Length - index) : Math.Max(0, ops.Length - index);
                    opazanja = new __Opazanje[len];
                    Array.Copy(ops, index, opazanja, 0, len);

                    for (int i = 0; i < opazanja.Length; i++)
                    {

                        __Opazanje o = opazanja[i];
                        o.Index = i + index;
                        o.Total = ops.Length;
                    }

                    foreach (var opazanje in opazanja)
                    {
                        List<string> slike64 = new List<string>();
                        var slika = (from s in db.SlikaPrekrsajas
                                     where s.IDLokacije == opazanje.IDLokacije
                                     select s.Slika).FirstOrDefault();
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

                        opazanje.Slike = slike64;
                    }
                    return opazanja;
                }

            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRETRAGA OPAZANJA");
                return new __Opazanje[0];
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

        public static bool SpremiOpazanje(string grad, ref _Opazanje opazanje, List<byte[]> slike)
        {
            return Parking.Spremi(grad, ref opazanje, slike, idAplikacije);
        }

        public static bool DodajSlikuOpazanju(string grad, int idOpazanja, List<byte[]> slike)
        {
            return Parking.DodajSlikuOpazanju(grad, idOpazanja, slike, idAplikacije);
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

        public static __Opazanje[] TraziOpazanje(string grad, int idSektora, int filter, int idDjelatnika)
        {
            return TraziOpazanje(grad, "", filter, idDjelatnika, idSektora, 0, -1, idAplikacije);
        }

        public static bool PromijeniPodatkeOpazanja(string grad, _Opazanje opazanje)
        {
            return Parking.PromijeniPodatkeOpazanja(grad, opazanje, idAplikacije);
        }

        public static bool VoziloOtislo(string grad, int idOpazanja)
        {
            return Parking.VoziloOtislo(grad, idOpazanja, idAplikacije);
        }

        public static _Opazanje TraziOpazanje(string grad, int idOpazanja)
        {
            return Parking.TraziOpazanje(grad, idOpazanja, idAplikacije);
        }

        public static __Opazanje[] TraziOpazanje(string grad, int idSektora, int filter, int idDjelatnika, int index, int count, string reg)
        {
            var ret = TraziOpazanje(grad, reg, filter, idDjelatnika, idSektora, index, count, idAplikacije);
            return ret;
        }

        public static __Opazanje[] TraziOpazanje(string grad, int idSektora, int filter, int idDjelatnika, int index, int count, string reg, double latitude, double longitude, int zastara)
        {
            var ret = TraziOpazanje(grad, reg, filter, idDjelatnika, idSektora, index, count, latitude, longitude, zastara, idAplikacije);
            return ret;
        }

        public static int Prijava(string grad, __Opazanje op, List<byte[]> slike)
        {
            _Prijava prijava = new _Prijava(0, 0, "", 2, 0, null, null, "", (decimal) op.Latitude, (decimal) op.Longitude, op.Adresa, "", op.Registracija + " (" + op.Drzava + ")", (DateTime) op.Vrijeme, false, null, "PARKING TEREN");
            return VanjskaPrijava.NovaPrijava(grad, prijava, slike, idAplikacije);
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

        public static string KopijaRacuna(string grad, int idRacuna, int tipPrintera)
        {
            return Parking.IspisKopijeRacunaParking(grad, idRacuna, idAplikacije, tipPrintera);
        }


        public static string UlazniTicket(int idGrada, int idSektora, string cijenaSat, string cijenaDnevna, string radnoVrijeme)
        {
            return Ispis.VrijemeDolaska(idGrada, idSektora, cijenaSat, cijenaDnevna, radnoVrijeme, idAplikacije);
        }

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

        public static List<_Printer> Printeri(string grad, int idRedarstva)
        {
            return servisPGParking.DohvatiPrintere(grad, false, idRedarstva);
        }

        public static List<_2DLista> Modeli()
        {
            return Parking.DohvatiModele();
        }

        /*:: NAPLATA ::*/

        public static List<_VrstaPlacanja> VrstePlacanja(string grad)
        {
            return servisPGParking.VrstePlacanja(grad);
        }

        public static void Fiskaliziraj(string grad, int idRacuna, int idRedarstva)
        {
            Fiskalizacija.Fiskaliziraj(grad, idRacuna, idRedarstva, idAplikacije);
        }

        public static string IzvjestajSmjene(string grad, int idKorisnika, int tipPrintera)
        {
            return Ispis.IzvjestajSmjeneParking(grad, idKorisnika, idRedarstva, DateTime.Today, idAplikacije, tipPrintera);
            //return servisPGParking.IzvjestajSmjene(grad, idKorisnika);
        }

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