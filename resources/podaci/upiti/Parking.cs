using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Newtonsoft.Json.Linq;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Parking
    {
        public static bool Spremi(string grad, ref _Opazanje opazanje, List<byte[]> slike, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    #region LOKACIJA

                    Lokacije lok = new Lokacije();

                    lok.Lat = (decimal)opazanje.Latitude;
                    lok.Long = (decimal)opazanje.Longitude;
                    lok.RegistracijskaPlocica = opazanje.Registracija == null ? null : opazanje.Registracija.Replace("-", "").Replace(" ", "").ToUpper();
                    lok.DatumVrijeme = ((DateTime)opazanje.Vrijeme);
                    lok.IDDjelatnika = opazanje.IDDjelatnika;
                    lok.IDNacinaPozicioniranja = 3;
                    lok.IDTerminala = opazanje.IDTerminala ?? 0;
                    lok.CellTowerID = null;
                    lok.SignalStrength = null;
                    lok.HDOP = 0;
                    lok.Brzina = 0;
                    //todo lok.Punjac = false;

                    db.Lokacijes.InsertOnSubmit(lok);
                    db.SubmitChanges();

                    #endregion


                    //if (slike != null)
                    //{
                    //    Prekrsaj.DodajSliku(grad, lok.IDLokacije, slike, idAplikacije);
                    //}
                   
                    PARKING_OPAZANJA po = new PARKING_OPAZANJA();

                    int id = 1;

                    if (db.PARKING_OPAZANJAs.Any())
                    {
                        id = db.PARKING_OPAZANJAs.Max(i => i.IDOpazanja) + 1;
                    }

                    po.IDOpazanja = id;
                    po.IDLokacije = lok.IDLokacije;
                    po.IDSektora = opazanje.IDSektora;
                    po.IDZone = db.PARKING_SEKTORIs.First(i => i.IDSektora == po.IDSektora).IDZone;
                    po.IDDjelatnika = (int)opazanje.IDDjelatnika;
                    po.IDStatusa = opazanje.IDStatusa;
                    po.Registracija = opazanje.Registracija.Replace("-", "").Replace(" ", "").ToUpper();
                    po.Drzava = opazanje.Drzava;
                    po.Vrijeme = opazanje.Vrijeme;
                    po.Latitude = opazanje.Latitude;
                    po.Longitude = opazanje.Longitude;
                    po.Kaznjen = opazanje.Kaznjen;

                    po.Otisao = false; // nema smisla spremati opažanje za vozilo koje je otišlo"?
                    po.PlacenoDo = null; //todo provjera plaćenosti ako je placeno zabilježiti do kada

                    db.PARKING_OPAZANJAs.InsertOnSubmit(po);
                    db.SubmitChanges();

                    if (slike != null)
                    {
                        DodajSlikuOpazanju(grad, id, slike, idAplikacije);
                    }

                    opazanje.IDOpazanja = id;
                    opazanje.IDLokacije = lok.IDLokacije;
                    opazanje.IDZone = po.IDZone;

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI OPAZANJE");
                return false;
            }
        }

        public static bool PromijeniPodatkeOpazanja(string grad, _Opazanje opazanje, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var op = db.PARKING_OPAZANJAs.First(i => i.IDOpazanja == opazanje.IDOpazanja);
                    if (op != null)
                    {
                        if (op.Registracija != null) op.Registracija = opazanje.Registracija;
                        if (op.Drzava != null) op.Drzava = opazanje.Drzava;
                        if (op.PlacenoDo != null) op.PlacenoDo = opazanje.PlacenoDo;
                        //op.Otisao = opazanje.Otisao;
                        db.SubmitChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI OPAZANJE");
                return false;
            }
        }

        public static bool VoziloOtislo(string grad, int idOpazanja, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var op = db.PARKING_OPAZANJAs.First(i => i.IDOpazanja == idOpazanja);
                    if (op != null)
                    {
                        op.Otisao = true;
                        op.IDStatusa = 5; // TODO stavio fiksno prema dogovoru s Pajom - razmisliti kasnije
                        db.SubmitChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI OPAZANJE");
                return false;
            }
        }

        public static List<_2DLista> DohvatiModele()
        {
            List<_2DLista> ret = new List<_2DLista>();
            using (PostavkeDataContext pb = new PostavkeDataContext())
            {
                var models = from m in pb.OPREMA_MODELs select new _2DLista(m.IDModela, m.NazivModela);
                if (models.Any()) ret.AddRange(models.ToArray());
            }
            return ret;
        }

        public static bool DodajSlikuOpazanju(string grad, int idOpazanja, List<byte[]> slike, int idAplikacije)
        {
            using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
            {
                PARKING_OPAZANJA op = db.PARKING_OPAZANJAs.First(i => i.IDOpazanja == idOpazanja);

                int idLokacije = op.IDLokacije;
                string uid = db.Djelatniks.First(i => i.IDDjelatnika == op.IDDjelatnika).UID;
                string lokacija = db.PARKING_SEKTORIs.First(i => i.IDSektora == op.IDSektora).NazivSektora;
                List<int> id = DodajSliku(grad, idLokacije, slike, uid, lokacija, idAplikacije);

                if (id.Any())
                {
                    return true;
                }

                return false;
            }
        }

        public static List<int> DodajSliku(string grad, int idLokacije, List<byte[]> slike, string uid, string lokacija, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List<int> id = new List<int>();
                    foreach (var sl in slike)
                    {
                        SlikaPrekrsaja slika = new SlikaPrekrsaja();

                        slika.IDLokacije = idLokacije;
                        slika.Slika = Slike.TimeStamp(sl, DateTime.Now, lokacija, uid);

                        db.SlikaPrekrsajas.InsertOnSubmit(slika);
                        db.SubmitChanges();

                        id.Add(slika.IDSlikePrekrsaja);
                    }

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dodaj Sliku");
                return new List<int>();
            }
        }

        public static List<_Opazanje> PretraziOpazanja(string grad, int idStatusa, int idDjelatnika, int idSektora, int idzone, DateTime? datumOd, DateTime? datumDo, string registracija,  int idAplikacije)
        {
            try
            {
                List<_2DLista> statusi = Statusi(false, idAplikacije);

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var op = from o in db.PARKING_OPAZANJAs
                             join r in db.RACUNIs on o.IDRacuna equals r.IDRacuna into racuni
                             from rr in racuni.DefaultIfEmpty()
                             join d in db.Djelatniks on o.IDDjelatnika equals d.IDDjelatnika into djelatnik
                             from dd in djelatnik.DefaultIfEmpty()
                             join s in db.PARKING_SEKTORIs on o.IDSektora equals s.IDSektora into sektori
                             from ss in sektori.DefaultIfEmpty()
                             join z in db.PARKING_ZONEs on ss.IDZone equals z.IDZone into zone
                             from zz in zone.DefaultIfEmpty()
                             where (datumOd.HasValue ? o.Vrijeme.Value.Date >= datumOd : datumOd == null)&&
                                   (datumDo.HasValue ? o.Vrijeme.Value.Date <= datumDo : datumDo == null) &&
                                   (idDjelatnika != 0 ? o.IDDjelatnika == idDjelatnika : true) &&
                                   (idStatusa != -2 ? (idStatusa == -1 ? o.IDStatusa == null : o.IDStatusa == idStatusa) : true) &&
                                   (idSektora != 0 ? o.IDSektora == idSektora : true) &&
                                   (idzone != 0 ? o.IDZone == idzone : true) &&
                                   (!string.IsNullOrEmpty(registracija) ? o.Registracija.ToLower().StartsWith(registracija.ToLower()) : registracija == "")
                             orderby o.Vrijeme descending
                             select new _Opazanje(o.IDOpazanja, o.IDLokacije, o.IDSektora, ss.IDZone, o.IDDjelatnika, 0, o.IDStatusa, o.IDRacuna, ss.NazivSektora, dd.ImePrezime, zz.NazivZone,
                                 Status(o.IDStatusa, statusi), o.Registracija, o.Drzava, (DateTime)o.Vrijeme, o.PlacenoDo, o.Latitude, o.Longitude, o.Iznos, o.Kaznjen.Value, (o.Otisao == null) ? false : o.Otisao.Value,
                                 rr.BrojRacuna,
                                 ""//broj kazni za registraciju
                                 );

                    return op.ToList();
                }

            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRETRAGA OPAZANJA");
                return new List<_Opazanje>();
            }
        }

        public static List<_Opazanje> TraziOpazanja(string grad, DateTime datum, string vrijeme, int? idDjelatnika, int idAplikacije)
        {

            try
            {
                List<_2DLista> statusi = Statusi(false, idAplikacije);

                string[] a = vrijeme.Split('-');
                int OdHH, OdMM, DoHH, DoMM;

                try
                {
                    OdHH = Convert.ToInt32(a.ElementAt(0).Split(':').ElementAt(0));
                    OdMM = Convert.ToInt32(a.ElementAt(0).Split(':').ElementAt(1));
                    DoHH = Convert.ToInt32(a.ElementAt(1).Split(':').ElementAt(0));
                    DoMM = Convert.ToInt32(a.ElementAt(1).Split(':').ElementAt(1));
                }
                catch
                {
                    OdHH = 6;
                    OdMM = 0;
                    DoHH = 23;
                    DoMM = 0;
                }

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var op = from o in db.PARKING_OPAZANJAs
                        join r in db.RACUNIs on o.IDRacuna equals r.IDRacuna into racuni
                        from rr in racuni.DefaultIfEmpty()
                             join d in db.Djelatniks on o.IDDjelatnika equals d.IDDjelatnika into djelatnik
                             from dd in djelatnik.DefaultIfEmpty()
                             join s in db.PARKING_SEKTORIs on o.IDSektora equals s.IDSektora into sektori
                             from ss in sektori.DefaultIfEmpty()
                             join z in db.PARKING_ZONEs on ss.IDZone equals z.IDZone into zone
                             from zz in zone.DefaultIfEmpty()
                             where o.Vrijeme.Value >= datum.Date.AddHours(OdHH).AddMinutes(OdMM) &&
                                   o.Vrijeme.Value <= datum.Date.AddHours(DoHH).AddMinutes(DoMM) &&
                                   (idDjelatnika != null && idDjelatnika > 0 ? o.IDDjelatnika == idDjelatnika : true)
                             orderby o.Vrijeme descending
                             select new _Opazanje(o.IDOpazanja, o.IDLokacije, o.IDSektora, ss.IDZone, o.IDDjelatnika, 0, o.IDStatusa, o.IDRacuna, ss.NazivSektora, dd.ImePrezime, zz.NazivZone, 
                                 Status(o.IDStatusa, statusi), o.Registracija, o.Drzava, (DateTime)o.Vrijeme, o.PlacenoDo, o.Latitude, o.Longitude, o.Iznos, o.Kaznjen.Value, (o.Otisao == null) ? false : o.Otisao.Value,
                                 rr.BrojRacuna, db.PARKING_OPAZANJAs.Count(m => m.Registracija == o.Registracija && m.Kaznjen == true).ToString());

                    return op.ToList();
                }

            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRETRAGA OPAZANJA");
                return new List<_Opazanje>();
            }

        }

        public static _Opazanje TraziOpazanje(string grad, int idOpazanja, int idAplikacije)
        {
            try
            {
                List<_2DLista> statusi = Statusi(false, idAplikacije);

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var op = from o in db.PARKING_OPAZANJAs
                             join d in db.Djelatniks on o.IDDjelatnika equals d.IDDjelatnika into djelatnik
                             from dd in djelatnik.DefaultIfEmpty()
                             join s in db.PARKING_SEKTORIs on o.IDSektora equals s.IDSektora into sektori
                             from ss in sektori.DefaultIfEmpty()
                             join z in db.PARKING_ZONEs on ss.IDZone equals z.IDZone into zone
                             from zz in zone.DefaultIfEmpty()
                             where (o.IDOpazanja == idOpazanja)
                             select new _Opazanje(o.IDOpazanja, o.IDLokacije, o.IDSektora, ss.IDZone, o.IDDjelatnika, 0, o.IDStatusa, o.IDRacuna, ss.NazivSektora, 
                                 Status(o.IDStatusa, statusi), dd.ImePrezime, zz.NazivZone, o.Registracija, o.Drzava, (DateTime)o.Vrijeme, o.PlacenoDo, o.Latitude, 
                                 o.Longitude, o.Iznos, o.Kaznjen.Value, (o.Otisao == null) ? false : o.Otisao.Value, "", "");


                    var opazanje = op.First();
                    if (opazanje != null)
                    {


                        List<string> slike64 = new List<string>();
                        var slike = from s in db.SlikaPrekrsajas
                                    where s.IDLokacije == opazanje.IDLokacije
                                    select s.Slika;
                        foreach (var slika in slike)
                        {
                            if (slika.Length > 0)
                            {
                                try
                                {
                                    slike64.Add(Convert.ToBase64String(slika.ToArray()));
                                   
                                    /*
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
                                    Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                                    Image myThumbnail = b.GetThumbnailImage(w, h, myCallback, IntPtr.Zero);
                                    var bytes = new MemoryStream();
                                    myThumbnail.Save(bytes, ImageFormat.Jpeg);
                                    slike64.Add(Convert.ToBase64String(bytes.ToArray()));
                                    */
                                    
                                }
                                catch (Exception ex)
                                {
                                    Sustav.SpremiGresku(grad, ex, idAplikacije, "Thumbnail slikice...");
                                }
                            }
                        }
                        opazanje.Slike = slike64.ToArray();
                    }
                    return opazanje;
                }

            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRETRAGA OPAZANJA");
                return null;
            }
        }

        public static List<_Opazanje> TraziOpazanje(string grad, string registracija, bool samoopazanja, int? idDjelatnika, int? idSektora, int idAplikacije)
        {
            int filter = (samoopazanja) ? 3 : 7;
            return TraziOpazanje(grad, registracija, filter, idDjelatnika, idSektora, idAplikacije);
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
        public static List<_Opazanje> TraziOpazanje(string grad, string registracija, int filter, int? idDjelatnika, int? idSektora, int idAplikacije)
        {
            int zastaraOpazanja = 24; // broj sati nakon kojih ne prikazuje opažanje
            bool op1, op2, kaznjeni;
            op1 = (filter & 0x01) != 0;
            op2 = (filter & 0x02) != 0;
            kaznjeni = (filter & 0x04) != 0;
            
            try
            {
                List<_2DLista> statusi = Statusi(false, idAplikacije);

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
                             select new _Opazanje(o.IDOpazanja, o.IDLokacije, o.IDSektora, ss.IDZone, o.IDDjelatnika, 0, o.IDStatusa, o.IDRacuna, ss.NazivSektora, dd.ImePrezime, zz.NazivZone,
                                 Status(o.IDStatusa, statusi), o.Registracija, o.Drzava, (DateTime)o.Vrijeme, o.PlacenoDo, o.Latitude, o.Longitude, o.Iznos, o.Kaznjen.Value, (o.Otisao == null) ? false : o.Otisao.Value, "", "");




                    return op.ToList();
                }

            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRETRAGA OPAZANJA");
                return new List<_Opazanje>();
            }

        }

        //public static string IspisKopijeRacunaParking(string grad, int idRacuna, int idAplikacije)
        //{
        //    try
        //    {
        //        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
        //        {
        //            _Racun racun = Naplata.DohvatiRacun(grad, idRacuna, idAplikacije);

        //            if (racun == null)
        //            {
        //                return "";
        //            }

        //            return Ispis.RacunParking(grad, racun, false, idAplikacije);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "KOPIJA RACUNA");
        //        return "";
        //    }
        //}

        public static string IspisKopijeRacunaParking(string grad, int idRacuna, int idAplikacije, int tipPrintera)
        {
            try
            {
                _Racun racun = Naplata.DohvatiRacun(grad, idRacuna, true, idAplikacije);


                if (racun == null)
                {
                    return "";
                }
                bool info;
                Naplata.VrstaPlacanja(grad, racun.IDVrste, idAplikacije, out info);

                return Ispis.RacunParking(grad, racun, info, idAplikacije, tipPrintera);
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "KOPIJA RACUNA");
                return "";
            }
        }

        public static JArray StavkeZone(string grad, int idStatusa, int? idZone, int idAplikacije)
        {
            JArray ret = new JArray();

            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var q = db.RACUNI_STAVKE_OPIs.Where(i => i.IDStatusa == idStatusa && i.Obrisan == false && i.IDZone == idZone && i.IDRedarstva == 4).Select(i => JObject.FromObject(new { IDOpisaStavke = i.IDOpisaStavke, Opis = i.NazivOpisaStavke, KratkiOpis = string.IsNullOrEmpty(i.KratkiOpis) ? (idStatusa == 0? "" : i.NazivOpisaStavke) : i.KratkiOpis, Cijena = i.Iznos }));
                    foreach (var s in q) ret.Add(s);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "STAVKE ZONE");
            }
            return ret;
        }

        public static Tuple<string, decimal, string> NaplatiParking(string grad, _Opazanje opazanje, int idStavke, int idVrstePlacanja, int kolicina, string poziv, int tipPrintera, int idAplikacije)
        {
            decimal iznos;
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    //todo zone
                    RACUNI_STAVKE_OPI st = db.RACUNI_STAVKE_OPIs.First(i => i.IDOpisaStavke == idStavke && i.IDRedarstva == 4);
                    if (st == null) return new Tuple<string, decimal, string>("", 0, "Ne postoji opis stavke računa!");

                    iznos = (decimal)st.Iznos * kolicina;
                    bool uplatnica;
                    string vrsta = Naplata.VrstaPlacanja(grad, idVrstePlacanja, idAplikacije, out uplatnica);
                    _PoslovniProstor pp = PoslovniProstor.DohvatiPoslovniProstor(grad, 4, idAplikacije);

                    if (pp == null)
                    {
                        return new Tuple<string, decimal, string>("", 0, "Niste definirali poslovni prostor!");
                    }

                    //provjera poziva
                    if (!string.IsNullOrEmpty(poziv))
                    {
                        Tuple<string, string> kontrola = Sustav.ProvjeraPoziva(grad, poziv, st.Iznos.Value, 4, idAplikacije);

                        if (!string.IsNullOrEmpty(kontrola.Item1))
                        {
                            return new Tuple<string, decimal, string>("", 0, kontrola.Item1);
                        }

                        poziv = kontrola.Item2;
                    }

                    _Djelatnik djel = Korisnici.DohvatiDjelatnika(grad, (int)opazanje.IDDjelatnika, idAplikacije);

                    List<_Stavka> stavke = new List<_Stavka>();

                    decimal ukupno = (decimal)(kolicina * st.Iznos);
                    decimal osnovica = Math.Round(ukupno / ((decimal)(100 + pp.PDV) / 100), 2);
                    decimal pdv = Math.Round(osnovica * pp.PDV / 100, 2);

                    _Stavka nova = new _Stavka(0, 0, st.IDOpisaStavke, st.NazivOpisaStavke, st.Lezarina, kolicina, (decimal)st.Iznos, pdv, osnovica, ukupno, pp.PDV, "");
                    stavke.Add(nova);

                    int blagajna = 1;

                    _Racun novi = new _Racun(0, -1, null, idVrstePlacanja, null, null, vrsta, "", (int)opazanje.IDDjelatnika, djel.ImeNaRacunu, 4, DateTime.Now, 0, 0, pdv, osnovica, ukupno,
                        pp.PDV, djel.OIB ?? "", blagajna, "", false, "", string.IsNullOrEmpty(opazanje.Registracija)? "":opazanje.Registracija + " (" + opazanje.Drzava + ")", true, "", "", "",
                        DateTime.Now, pp.Oznaka, poziv, "", "", opazanje.Registracija + " (" + opazanje.Drzava + ")", false, false, false, false, "", stavke, new List<_Osoba>());

                    if (opazanje.IDOpazanja == 0)
                    {
                        return new Tuple<string, decimal, string>(Naplata.NaplatiParking(grad, novi, uplatnica, idAplikacije, tipPrintera), iznos, "");
                    }

                    PARKING_OPAZANJA op = db.PARKING_OPAZANJAs.First(i => i.IDOpazanja == opazanje.IDOpazanja);
                    opazanje.IDLokacije = op.IDLokacije;

                    op.IDStatusa = st.IDStatusa;
                    op.Kaznjen = true;
                    op.Iznos = st.Iznos;

                    opazanje.IDStatusa = st.IDStatusa;

                    //ako je naplata (0) po satu preskoči ovo
                    if (st.IDStatusa != 0)
                    {
                        // TODO izbacio jer je suvišno nakon promjene?
                        /*
                        int idPrekrsaja = DodajPostupanje(grad, opazanje, iznos, idAplikacije);//todo iznos kazne

                        if (idPrekrsaja == -1)
                        {
                            iznos = 0;
                            return "";
                        }
                        */


                        novi.IDReference = op.IDOpazanja;
                    }

                    db.SubmitChanges();

                    return new Tuple<string, decimal, string>(Naplata.NaplatiParking(grad, novi, uplatnica, idAplikacije, tipPrintera), iznos, ""); //;Naplata.NaplatiParking(grad, novi, uplatnica, idAplikacije, tipPrintera);
                    //todo dodati record u akcije
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATA PARKINGA");
                iznos = 0;
                return new Tuple<string, decimal, string>("", 0, ex.Message);
            }
        }

        /*:: ZONE ::*/

        public static List<_Zone> Zone(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var zone = from z in db.PARKING_ZONEs
                               select new _Zone(z.IDZone, z.NazivZone, z.mParking, z.CijenaSata, z.GracePeriod);

                    return zone.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ZONE");
                return new List<_Zone>();
            }
        }

        public static int DodajZonu(string grad, _Zone zona, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PARKING_ZONE z = new PARKING_ZONE();

                    int id = 1;

                    if (db.PARKING_ZONEs.Any())
                    {
                        id = db.PARKING_ZONEs.Max(i => i.IDZone) + 1;
                    }

                    z.IDZone = id;
                    z.NazivZone = zona.NazivZone;
                    z.mParking = zona.mParking;
                    z.CijenaSata = zona.Cijena;
                    z.GracePeriod = zona.GracePeriod;

                    db.PARKING_ZONEs.InsertOnSubmit(z);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ ZONU");
                return -1;
            }
        }

        public static bool IzmjeniZonu(string grad, _Zone zona, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PARKING_ZONE z = db.PARKING_ZONEs.First(i => i.IDZone == zona.IDZone);

                    z.NazivZone = zona.NazivZone;
                    z.mParking = zona.mParking;
                    z.CijenaSata = zona.Cijena;
                    z.GracePeriod = zona.GracePeriod;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI ZONU");
                return false;
            }
        }

        public static bool ObrisiZonu(string grad, int idZone, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.PARKING_OPAZANJAs.Any(i => i.IDZone == idZone))
                    {
                        return false;
                    }

                    if (db.PARKING_SEKTORIs.Any(i => i.IDZone == idZone))
                    {
                        return false;
                    }

                    db.PARKING_ZONEs.DeleteOnSubmit(db.PARKING_ZONEs.First(i => i.IDZone == idZone));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OBRIŠI ZONU");
                return false;
            }
        }
        
        /*:: SEKTORI ::*/

        public static List<_Sektori> Sektori(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var sektori = from s in db.PARKING_SEKTORIs
                                  join z in db.PARKING_ZONEs on s.IDZone equals z.IDZone into zone
                                  from zz in zone.DefaultIfEmpty()
                                  select new _Sektori(s.IDSektora, s.IDZone, s.NazivSektora, s.OznakaSektora,
                                      s.IDZone == null ? s.mParking : zz.mParking, s.IDZone == null ? s.Cijena : zz.CijenaSata,
                                      s.Latitude, s.Longitude, zz.GracePeriod);

                    return sektori.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SEKTORI");
                return new List<_Sektori>();
            }
        }

        public static int DodajSektor(string grad, _Sektori sektor, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PARKING_SEKTORI z = new PARKING_SEKTORI();

                    int id = 1;

                    if (db.PARKING_SEKTORIs.Any())
                    {
                        id = db.PARKING_SEKTORIs.Max(i => i.IDSektora) + 1;
                    }

                    z.IDSektora = id;
                    z.IDZone = sektor.IDZone;
                    z.NazivSektora = sektor.NazivSektora;
                    z.OznakaSektora = sektor.OznakaSektora;
                    z.mParking = sektor.mParking;
                    z.Cijena = sektor.Cijena;
                    z.Longitude = sektor.Longitude;
                    z.Latitude = sektor.Latitude;

                    db.PARKING_SEKTORIs.InsertOnSubmit(z);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ SEKTOR");
                return -1;
            }
        }

        public static bool IzmjeniSektor(string grad, _Sektori sektor, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PARKING_SEKTORI z = db.PARKING_SEKTORIs.First(i => i.IDSektora == sektor.IDSektora);

                    z.IDZone = sektor.IDZone;
                    z.NazivSektora = sektor.NazivSektora;
                    z.OznakaSektora = sektor.OznakaSektora;
                    z.mParking = sektor.mParking;
                    z.Cijena = sektor.Cijena;
                    z.Longitude = sektor.Longitude;
                    z.Latitude = sektor.Latitude;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI SEKTOR");
                return false;
            }
        }

        public static bool ObrisiSektor(string grad, int idSektora, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.PARKING_OPAZANJAs.Any(i => i.IDSektora == idSektora))
                    {
                        return false;
                    }

                    db.PARKING_SEKTORIs.DeleteOnSubmit(db.PARKING_SEKTORIs.First(i => i.IDSektora == idSektora));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OBRIŠI SEKTOR");
                return false;
            }
        }

        /*:: HELPERS ::*/

        public static List<_2DLista> Statusi(bool naplacuje, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var drz = from d in db.PARKING_STATUS
                              where naplacuje ? d.Naplacuje : naplacuje == false
                              select new _2DLista(d.IDStatusa, d.Status);

                    return drz.ToList();
                }
            }
            catch
            {
                return new List<_2DLista>();
            }
        }

        public static string Status(int? idStatsua, List<_2DLista> Statusi)
        {
            try
            {
                if (idStatsua == null)
                {
                    return "Opažanje";
                }

                return Statusi.First(i => i.Value == idStatsua).Text;
            }
            catch
            {
                return "";
            }
        }
    }
}