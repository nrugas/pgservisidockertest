using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.cs.sigurnost;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Mapa
    {
        /*:: ADMINISTRACIJA ::*/

        public static List<_Tocka> StvaranjePutanjeRedara(string grad, DateTime datum, string vrijeme, int idRedara, int gpsAcc, int speed, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
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

                    var tocke = from d in db.Djelatniks
                                join l in db.Lokacijes on d.IDDjelatnika equals l.IDDjelatnika
                                where d.IDDjelatnika == idRedara &&
                                      l.DatumVrijeme >= datum.Date.AddHours(OdHH).AddMinutes(OdMM) &&
                                      l.DatumVrijeme <= datum.Date.AddHours(DoHH).AddMinutes(DoMM) &&
                                      (gpsAcc > 0 && l.IDNacinaPozicioniranja == 1 ? l.GPSAcc < gpsAcc : gpsAcc == 0) &&
                                      (speed != -1 ? l.Brzina > speed : speed == -1)
                                //(d.IDRedarstva != 4 ? (l.GPSAcc < 30 && l.Brzina > 0) : l.GPSAcc < 50)
                                orderby l.DatumVrijeme ascending
                                select new _Tocka
                                    (
                                    (double)l.Lat,
                                    (double)l.Long,
                                    l.DatumVrijeme
                                    );

                    if (tocke.Any())
                    {
                        return tocke.ToList();
                    }

                    return new List<_Tocka>();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PUTANJA REDARA");
                return new List<_Tocka>();
            }
        }

        public static List<_Prekrsaj> PozicijePrekrsaja(string grad, int idZaposlenika, DateTime datum, string vrijeme, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
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

                    var tocke = from p in db.Prekrsajis
                                join l in db.Lokacijes on p.IDLokacije equals l.IDLokacije
                                join s in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals s.IDOpisa into opis
                                from ss in opis.DefaultIfEmpty()
                                join o in db.PopisPrekrsajas on ss.IDPrekrsaja equals o.IDPrekrsaja into popis
                                from oo in popis.DefaultIfEmpty()
                                join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                                join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska into predlozak
                                from ii in predlozak.DefaultIfEmpty()
                                join t in db.Terminalis on l.IDTerminala equals t.IDTerminala into term
                                from tt in term.DefaultIfEmpty()
                                join n in db.NaloziPaukus on p.IDNaloga equals n.IDNaloga into nalozi
                                from nn in nalozi.DefaultIfEmpty()
                                join z in db.StatusPaukas on nn.IDStatusa equals z.IDStatusa into statusi
                                from zz in statusi.DefaultIfEmpty()
                                join v in db.RazloziNepodizanjaVozilas on nn.IDRazloga equals v.IDRazloga into raz
                                from vv in raz.DefaultIfEmpty()
                                join y in db.Pauks on p.IDNaloga equals y.IDNaloga into pau
                                from yy in pau.DefaultIfEmpty()
                                join x in db.VozilaPaukas on nn.IDVozila equals x.IDVozila into voz
                                from xx in voz.DefaultIfEmpty()
                                join r in db.RACUNIs on nn.IDRacuna equals r.IDRacuna into rac
                                from rr in rac.DefaultIfEmpty()
                                join q in db.RACUNIs on p.IDRacuna equals q.IDRacuna into racP
                                from qq in racP.DefaultIfEmpty()
                                where (idZaposlenika != 0 ? p.IDDjelatnika == idZaposlenika : idZaposlenika == 0) &&
                                      p.Vrijeme.Value >= datum.Date.AddHours(OdHH).AddMinutes(OdMM) &&
                                      p.Vrijeme.Value <= datum.Date.AddHours(DoHH).AddMinutes(DoMM) &&
                                      p.IDRedarstva == idRedarstva &&
                                      p.Test == false &&
                                      p.Status == false
                                orderby p.Vrijeme.Value
                                select new _Prekrsaj
                                    (
                                    p.IDPrekrsaja,
                                    p.IDRedarstva,
                                    tt.IDTerminala,
                                    ss.IDOpisa,
                                    p.IDOpisaZakona,
                                    p.IDLokacije,
                                    (int)p.IDDjelatnika,
                                    p.IDPredloskaIspisa ?? -1,
                                    p.Lat,
                                    p.Long,
                                    p.Vrijeme.Value,
                                    p.KraticaDrzave != "??" ? p.RegistracijskaPlocica + " (" + p.KraticaDrzave + ")" : p.RegistracijskaPlocica,
                                    d.ImePrezime,
                                    d.BrojSI,
                                    d.UID,
                                    p.Adresa,
                                    p.BrojUpozorenja,
                                    tt.IDTerminala == 0 ? "RUČNI UNOS" : tt.NazivTerminala,
                                    ii.NazivPredloska,
                                    ss.OpisPrekrsaja,
                                    ss.KratkiOpis,
                                    "",
                                    oo.MaterijalnaKaznjivaNorma,
                                    "",
                                    ss.ClanakPauka,
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
                                    p.RegistracijskaPlocica == "IZVID" ? "0" : db.Prekrsajis.Count(m => m.RegistracijskaPlocica == p.RegistracijskaPlocica && m.Test == false && m.Status == false && ii.IDRedarstva == idRedarstva).ToString(),//broj kazni za registraciju
                                    p.StatusVPP ?? "",
                                    p.KraticaDrzave,
                                    p.IDRacuna,
                                    qq.BrojRacuna,
                                    Priprema.Nalog(p.IDNaloga, xx, zz, vv, nn, yy, rr.BrojRacuna, Naplata.VrstaPlacanja(grad, rr.IDVrstePlacanja, idAplikacije)));

                    return Priprema.PripremiPodatke(grad, tocke, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pozicije Prekrsaja: " + vrijeme);
                return new List<_Prekrsaj>();
            }
        }

        public static List<_Pozicija> PozicijeRedara(string grad, int minuta, int idRedarstva, int idAplikacije)
        {
            try
            {
                List<_Pozicija> nova = new List<_Pozicija>();

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    //var x = db.Djelatniks.Where(i => i.PrometniRedar && i.IDRedarstva == 1);

                    foreach (var d in db.Djelatniks.Where(i => i.PrometniRedar && i.IDRedarstva == idRedarstva))
                    {
                        var tocke = (from l in db.Lokacijes
                                     join t in db.Terminalis on l.IDTerminala equals t.IDTerminala
                                     where l.DatumVrijeme >= DateTime.Now.AddMinutes(-20) &&
                                           l.IDDjelatnika == d.IDDjelatnika
                                     orderby l.DatumVrijeme descending
                                     select new
                                     {
                                         l.DatumVrijeme,
                                         l.Lat,
                                         l.Long,
                                         t.NazivTerminala,
                                         t.VrijemeZadnjegPristupa,
                                         t.TelefonskiBroj,
                                         l.Battery,
                                         l.GPSAcc
                                     }).Take(1);

                        foreach (var t in tocke)
                        {
                            if (t.VrijemeZadnjegPristupa == null)
                            {
                                continue;
                            }

                            if (t.VrijemeZadnjegPristupa <= DateTime.Now.Subtract(new TimeSpan(0, 0, minuta, 0)))
                            {
                                continue;
                            }

                            List<_Tocka> toc = new List<_Tocka>();

                            toc.Add(new _Tocka((double)t.Lat, (double)t.Long, t.DatumVrijeme));

                            if (toc.Any())
                            {
                                nova.Add(new _Pozicija(d.IDDjelatnika, d.ImePrezime, tocke.First().NazivTerminala,
                                    tocke.First().GPSAcc == null ? 100 : (int)tocke.First().GPSAcc,
                                    tocke.First().Battery == null ? "-" : tocke.First().Battery + "%",
                                    0, d.Mobitel, "", t.VrijemeZadnjegPristupa, false, toc));
                            }
                        }
                    }

                    return nova;
                }

            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pozicije Redara");
                return new List<_Pozicija>();
            }
        }

        public static _Pozicija PozicijaOdabranogRedara(string grad, int idDjelatnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var tocke = (from l in db.Lokacijes
                                 join d in db.Djelatniks on l.IDDjelatnika equals d.IDDjelatnika
                                 join t in db.Terminalis on l.IDTerminala equals t.IDTerminala
                                 where l.DatumVrijeme >= DateTime.Now.AddMinutes(-20) &&
                                       l.IDDjelatnika == d.IDDjelatnika &&
                                       t.Pauk != true
                                 orderby l.DatumVrijeme descending
                                 select new
                                 {
                                     d.IDDjelatnika,
                                     d.ImePrezime,
                                     d.Mobitel,
                                     l.DatumVrijeme,
                                     l.Lat,
                                     l.Long,
                                     t.NazivTerminala,
                                     t.VrijemeZadnjegPristupa,
                                     t.TelefonskiBroj,
                                     l.Battery,
                                     l.GPSAcc
                                 }).Take(1);

                    foreach (var t in tocke)
                    {
                        if (t.VrijemeZadnjegPristupa == null)
                        {
                            continue;
                        }

                        if (t.VrijemeZadnjegPristupa <= DateTime.Now.Subtract(new TimeSpan(0, 0, 10, 0)))
                        {
                            continue;
                        }

                        List<_Tocka> toc = new List<_Tocka>();

                        toc.Add(new _Tocka((double)t.Lat, (double)t.Long, t.DatumVrijeme));

                        if (toc.Any())
                        {
                            return new _Pozicija(t.IDDjelatnika, t.ImePrezime, tocke.First().NazivTerminala,
                                tocke.First().GPSAcc == null ? 100 : (int)tocke.First().GPSAcc,
                                tocke.First().Battery == null ? "-" : tocke.First().Battery + "%",
                                0, t.Mobitel, "", t.VrijemeZadnjegPristupa, false, toc);
                        }
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pozicija Odabranog Redara");
                return null;
            }
        }

        public static int ImaKazni(string grad, string registracija, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return
                        db.Prekrsajis.Count(
                            i => i.RegistracijskaPlocica == registracija.Replace("-", "").Replace(" ", ""));
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Broj Prekrsaja za registraciju: " + registracija);
                return 0;
            }
        }

        /*:: PAUK ::*/

        public static List<_Prekrsaj> PozicijeNaloga(string grad, int idVozila, DateTime datum, bool sviNalozi, string vrijeme, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    string[] y = vrijeme.Split('-');

                    int OdHH = Convert.ToInt32(y.ElementAt(0).Split(':').ElementAt(0));
                    int OdMM = Convert.ToInt32(y.ElementAt(0).Split(':').ElementAt(1));
                    int DoHH = Convert.ToInt32(y.ElementAt(1).Split(':').ElementAt(0));
                    int DoMM = Convert.ToInt32(y.ElementAt(1).Split(':').ElementAt(1));

                    var nal = from n in db.NaloziPaukus
                              join p in db.Prekrsajis on n.IDNaloga equals p.IDNaloga
                              join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska into predlozak
                              from ii in predlozak.DefaultIfEmpty()
                              join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa
                              join l in db.Lokacijes on p.IDLokacije equals l.IDLokacije
                              join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                              join z in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals z.IDOpisa into opis
                              from zz in opis.DefaultIfEmpty()
                              join o in db.PopisPrekrsajas on zz.IDPrekrsaja equals o.IDPrekrsaja into popis
                              from oo in popis.DefaultIfEmpty()
                              join a in db.Pauks on n.IDNaloga equals a.IDNaloga
                              join t in db.Terminalis on l.IDTerminala equals t.IDTerminala into term
                              from tt in term.DefaultIfEmpty()
                              join v in db.VozilaPaukas on n.IDVozila equals v.IDVozila into vozila
                              from vv in vozila.DefaultIfEmpty()
                              join r in db.RazloziNepodizanjaVozilas on n.IDRazloga equals r.IDRazloga into raz
                              from rr in raz.DefaultIfEmpty()
                              join x in db.VozilaPaukas on n.IDVozila equals x.IDVozila into voz
                              from xx in voz.DefaultIfEmpty()
                              join b in db.RACUNIs on n.IDRacuna equals b.IDRacuna into rac
                              from bb in rac.DefaultIfEmpty()
                              join q in db.RACUNIs on p.IDRacuna equals q.IDRacuna into racP
                              from qq in racP.DefaultIfEmpty()
                              where n.DatumNaloga >= datum.Date.AddHours(OdHH).AddMinutes(OdMM) &&
                                    n.DatumNaloga <= datum.Date.AddHours(DoHH).AddMinutes(DoMM) &&
                                    (!sviNalozi ? n.NalogZatvoren == false : sviNalozi) &&
                                    (idVozila != 0 ? n.IDVozila == idVozila : idVozila == 0) &&
                                    p.IDNaloga > 0
                              orderby n.IDStatusa, n.DatumNaloga ascending
                              select new _Prekrsaj
                                  (
                                  p.IDPrekrsaja,
                                  p.IDRedarstva,
                                  tt.IDTerminala,
                                  zz.IDOpisa,
                                  p.IDOpisaZakona,
                                  p.IDLokacije,
                                  (int)p.IDDjelatnika,
                                  p.IDPredloskaIspisa ?? -1,
                                  p.Lat,
                                  p.Long,
                                  p.Vrijeme.Value,
                                  p.KraticaDrzave != "??" ? p.RegistracijskaPlocica + " (" + p.KraticaDrzave + ")" : p.RegistracijskaPlocica,
                                  d.ImePrezime,
                                  d.BrojSI,
                                  string.IsNullOrEmpty(d.ImeNaRacunu) ? d.UID : d.ImeNaRacunu,
                                  p.Adresa,
                                  p.BrojUpozorenja,
                                  tt.IDTerminala == 0 ? "RUČNI UNOS" : tt.NazivTerminala,
                                  ii.NazivPredloska,
                                  zz.OpisPrekrsaja,
                                  zz.KratkiOpis,
                                  "",
                                  oo.MaterijalnaKaznjivaNorma,
                                  "",
                                  zz.ClanakPauka,
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
                                  "",
                                  p.StatusVPP ?? "",
                                  p.KraticaDrzave,
                                  p.IDRacuna,
                                  qq.BrojRacuna,
                                  Priprema.Nalog(p.IDNaloga, xx, s, rr, n, a, bb.BrojRacuna, Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije)));

                    return nal.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pozicije Naloga");
                return new List<_Prekrsaj>();
            }
        }

        public static List<_Tocka> StvaranjePutanjePauka(string grad, DateTime datum, string vrijeme, int idVozila, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    string[] x = vrijeme.Split('-');

                    int OdHH = Convert.ToInt32(x.ElementAt(0).Split(':').ElementAt(0));
                    int OdMM = Convert.ToInt32(x.ElementAt(0).Split(':').ElementAt(1));
                    int DoHH = Convert.ToInt32(x.ElementAt(1).Split(':').ElementAt(0));
                    int DoMM = Convert.ToInt32(x.ElementAt(1).Split(':').ElementAt(1));

                    var tocke = from p in db.LokacijePaukas
                                join t in db.Terminalis on p.IDTerminala equals t.IDTerminala
                                join v in db.VozilaPaukas on p.IDVozila equals v.IDVozila
                                where v.IDVozila == idVozila &&
                                      p.DatumVrijemePauka >= datum.Date.AddHours(OdHH).AddMinutes(OdMM) &&
                                      p.DatumVrijemePauka <= datum.Date.AddHours(DoHH).AddMinutes(DoMM) &&
                                      p.Brzina > 1.1 && p.GPSAcc < 30
                                orderby p.DatumVrijemePauka ascending
                                select new _Tocka
                                (
                                    (double)p.LatPauka,
                                    (double)p.LongPauka,
                                    p.DatumVrijemePauka
                               );

                    if (tocke.Any())
                    {
                        return tocke.ToList();
                    }

                    //mobilna ekipa
                    var tockeme = from p in db.Lokacijes
                                  join t in db.Terminalis on p.IDTerminala equals t.IDTerminala
                                  join v in db.VozilaPaukas on t.IDTerminala equals v.IDTerminala.Value
                                  where p.DatumVrijeme >= datum.Date.AddHours(OdHH).AddMinutes(OdMM) &&
                                        p.DatumVrijeme <= datum.Date.AddHours(DoHH).AddMinutes(DoMM) &&
                                        p.Brzina > 1.1 && p.GPSAcc < 30 &&
                                        v.IDVozila == idVozila
                                  orderby p.DatumVrijeme ascending
                                  select new _Tocka
                                  (
                                      (double)p.Lat,
                                      (double)p.Long,
                                      p.DatumVrijeme
                                 );

                    return tockeme.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PUTANJA PAUKA");
                return new List<_Tocka>();
            }
        }

        public static List<_Pozicija> TrenutnePozicijeVozila(string grad, int minuta, int? idVozila, int idAplikacije)
        {
            try
            {
                List<_Pozicija> nova = new List<_Pozicija>();

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    foreach (var v in db.VozilaPaukas.Where(i => i.IDTerminala != 0 && i.IDTerminala != null && (idVozila != null ? i.IDVozila == idVozila : idVozila == null)))
                    {
                        if (v.ObradjujeNalog)
                        {
                            var tocke = (from t in db.Terminalis
                                         join l in db.LokacijePaukas on t.IDTerminala equals l.IDTerminala
                                         where l.DatumVrijemePauka >= DateTime.Now.AddMinutes(-10) &&
                                               l.IDVozila == v.IDVozila
                                         orderby l.DatumVrijemePauka descending
                                         select new
                                         {
                                             l.DatumVrijemePauka,
                                             l.LatPauka,
                                             l.LongPauka,
                                             t.NazivTerminala,
                                             t.VrijemeZadnjegPristupa,
                                             t.IDTerminala,
                                             l.Battery,
                                             l.Brzina,
                                             l.GPSAcc
                                         }).Take(1); //5 ako je ona putanja s repoma ali ionako nes ne radi

                            List<_Tocka> toc = new List<_Tocka>();

                            foreach (var q in tocke)
                            {
                                toc.Add(new _Tocka((double)q.LatPauka, (double)q.LongPauka, q.DatumVrijemePauka));
                            }

                            if (toc.Any())
                            {
                                nova.Add(new _Pozicija(v.IDVozila, v.NazivVozila, tocke.First().NazivTerminala, tocke.First().GPSAcc == null ? 100 : (int)tocke.First().GPSAcc,
                                             tocke.First().Battery == null ? "-" : tocke.First().Battery + "%", tocke.First().Brzina, v.Kontakt, v.Registracija, tocke.First().VrijemeZadnjegPristupa, v.AP, toc));
                            }
                        }
                        else
                        {
                            //mobilna ekipa
                            var tocke = (from t in db.Terminalis
                                         join l in db.LokacijePaukas on t.IDTerminala equals l.IDTerminala
                                         where l.DatumVrijemePauka >= DateTime.Now.AddMinutes(-10) &&
                                               t.IDTerminala == v.IDTerminala
                                         orderby l.DatumVrijemePauka descending
                                         select new
                                         {
                                             l.DatumVrijemePauka,
                                             l.LatPauka,
                                             l.LongPauka,
                                             t.NazivTerminala,
                                             t.VrijemeZadnjegPristupa,
                                             t.IDTerminala,
                                             l.Battery,
                                             l.Brzina,
                                             l.GPSAcc
                                         }).Take(1);

                            List<_Tocka> toc = new List<_Tocka>();

                            foreach (var q in tocke)
                            {
                                toc.Add(new _Tocka((double)q.LatPauka, (double)q.LongPauka, q.DatumVrijemePauka));
                            }

                            if (toc.Any())
                            {
                                nova.Add(new _Pozicija(v.IDVozila, v.NazivVozila, tocke.First().NazivTerminala, tocke.First().GPSAcc == null ? 100 : (int)tocke.First().GPSAcc,
                                             tocke.First().Battery == null ? "-" : tocke.First().Battery + "%", tocke.First().Brzina, v.Kontakt, v.Registracija, tocke.First().VrijemeZadnjegPristupa, false, toc));
                            }
                        }
                    }
                }

                return nova;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pozicije Vozila");
                return new List<_Pozicija>();
            }
        }

        public static List<_PutanjaVozila> PutanjaObradeNaloga(string grad, _Nalog nalog, out List<_PutanjaVozila> putanjaDoPrekrsaja, out List<_DogadjajiNaloga> dogadaji, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    DateTime izdavanja = db.PovijestNalogas.OrderByDescending(i => i.DatumVrijemeDogadjaja).FirstOrDefault(i => i.IDNaloga == nalog.IDNaloga && i.IDStatusa == 0).DatumVrijemeDogadjaja;
                    DateTime preuzimanje = db.PovijestNalogas.OrderByDescending(i => i.DatumVrijemeDogadjaja).FirstOrDefault(i => i.IDNaloga == nalog.IDNaloga && i.IDStatusa == 1).DatumVrijemeDogadjaja;
                    DateTime kraj = db.PovijestNalogas.OrderByDescending(i => i.DatumVrijemeDogadjaja).FirstOrDefault(i => i.IDNaloga == nalog.IDNaloga).DatumVrijemeDogadjaja;

                    var OdIzdavanja = from l in db.LokacijePaukas
                                      join t in db.Terminalis on l.IDTerminala equals t.IDTerminala
                                      join v in db.VozilaPaukas on l.IDVozila equals v.IDVozila
                                      where v.IDVozila == nalog.IDVozila &&
                                            l.DatumVrijemePauka >= izdavanja &&
                                            l.DatumVrijemePauka <= preuzimanje
                                      orderby l.DatumVrijemePauka ascending
                                      select new _PutanjaVozila(
                                          l.IDLokacijePauka,
                                          v.NazivVozila,
                                          (double)l.LatPauka,
                                          (double)l.LongPauka,
                                          t.NazivTerminala,
                                          l.DatumVrijemePauka
                                          );

                    var tocke = from l in db.LokacijePaukas
                                join t in db.Terminalis on l.IDTerminala equals t.IDTerminala
                                join v in db.VozilaPaukas on l.IDVozila equals v.IDVozila
                                where v.IDVozila == nalog.IDVozila &&
                                      l.DatumVrijemePauka >= preuzimanje &&
                                      l.DatumVrijemePauka <= kraj
                                orderby l.DatumVrijemePauka ascending
                                select new _PutanjaVozila(
                                    l.IDLokacijePauka,
                                    v.NazivVozila,
                                    (double)l.LatPauka,
                                    (double)l.LongPauka,
                                    t.NazivTerminala,
                                    l.DatumVrijemePauka
                                    );

                    var dog = from p in db.PovijestNalogas
                              join s in db.StatusPaukas on p.IDStatusa equals s.IDStatusa
                              where p.IDNaloga == nalog.IDNaloga
                              select new
                              {
                                  p.IDStatusa,
                                  p.LatDogadjaja,
                                  p.LongDogadjaja,
                                  s.NazivStatusa,
                                  p.DatumVrijemeDogadjaja
                              };

                    dogadaji = new List<_DogadjajiNaloga>();

                    decimal lat = 0;

                    foreach (var q in dog)
                    {
                        if (q.LatDogadjaja != null)
                        {
                            decimal lat1 = (decimal)q.LatDogadjaja;

                            if (q.LatDogadjaja == lat)
                            {
                                lat1 += (decimal)0.0001;
                            }

                            dogadaji.Add(new _DogadjajiNaloga(q.IDStatusa, lat1, (decimal)q.LongDogadjaja, q.NazivStatusa, q.DatumVrijemeDogadjaja));

                            lat = (decimal)q.LatDogadjaja;
                        }
                    }

                    putanjaDoPrekrsaja = tocke.ToList();
                    return OdIzdavanja.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Putanja Prekrsaja");
                dogadaji = new List<_DogadjajiNaloga>();
                putanjaDoPrekrsaja = new List<_PutanjaVozila>();
                return new List<_PutanjaVozila>();
            }
        }

        /*:: OSTALO ::*/

        public static List<_WebKamere> PozicijeKamera(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var kamere = from i in db.PozicijeKameras
                                 select new _WebKamere(i.IDPozicije, i.NazivKamere, i.LatKamere, i.LonKamere,
                                     i.WebAdresa, i.OpisKamere, i.Prikazi);

                    return kamere.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pozicije Kamera");
                return new List<_WebKamere>();
            }
        }

        /*:: MOBILE ::*/

        class AktivniDjelatnici
        {
            public int IDDjelatnika { get; set; }
            public string ImePrezime { get; set; }
            public DateTime VrijemeZadnjegPristupa { get; set; }
            public string NazivTerminala { get; set; }
            public string TelefonskiBroj { get; set; }
            public double Lat { get; set; }
            public double Long { get; set; }
            public int? Battery { get; set; }
            public int? GPSAcc { get; set; }

        }

        public static List<_PozicijaDjelatnika> PozicijeRedaraMobile(string grad, int minuta, int idAplikacije)
        {
            try
            {
                List<_PozicijaDjelatnika> nova = new List<_PozicijaDjelatnika>();

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    string query = string.Format(@"SELECT MAX([IDLokacije]) as IDLokacije
                                          , Lokacije.IDDjelatnika FROM Lokacije
                                        where Lokacije.DatumVrijeme >= DateAdd(minute, -{0}, GETDATE()) 
                                        group by Lokacije.IDDjelatnika", minuta);
                    
                    
                    var lokDjel = db.ExecuteQuery<Lokacije>(query);
                    
                    foreach(var ld in lokDjel)
                    {
                        /*
                       foreach (var d in db.Djelatniks.Where(i => i.PrometniRedar))
                       {

                           var tocke = (from l in db.Lokacijes
                                        join t in db.Terminalis on l.IDTerminala equals t.IDTerminala
                                        where l.DatumVrijeme.Date == DateTime.Today.Date &&
                                              l.IDDjelatnika == d.IDDjelatnika
                                        orderby l.DatumVrijeme descending
                                        select new
                                        {
                                            l.DatumVrijeme,
                                            l.Lat,
                                            l.Long,
                                            t.NazivTerminala,
                                            t.VrijemeZadnjegPristupa,
                                            t.TelefonskiBroj,
                                            l.Battery,
                                            l.GPSAcc
                                        }).Take(1);

                           foreach (var q in tocke)
                           {
                           */
                        var d = db.Djelatniks.First(i => i.IDDjelatnika == ld.IDDjelatnika);


                        var q = (from l in db.Lokacijes where ld.IDLokacije == l.IDLokacije
                                          join t in db.Terminalis on l.IDTerminala equals t.IDTerminala
                                          
                                          select new
                                          {
                                              l.DatumVrijeme,
                                              l.Lat,
                                              l.Long,
                                              t.NazivTerminala,
                                              t.VrijemeZadnjegPristupa,
                                              t.TelefonskiBroj,
                                              l.Battery,
                                              l.GPSAcc
                                          }).First();

                        if (q.VrijemeZadnjegPristupa == null)
                            {
                                continue;
                            }

                            if (q.VrijemeZadnjegPristupa <= DateTime.Now.Subtract(new TimeSpan(0, 0, minuta, 0)))
                            {
                                continue;
                            }

                            nova.Add(new _PozicijaDjelatnika(d.IDDjelatnika, d.ImePrezime, (double)q.Lat, (double)q.Long, q.NazivTerminala,
                                                             q.VrijemeZadnjegPristupa.Value, 0, 0, 0,
                                                             q.GPSAcc == null ? "-" : q.GPSAcc.ToString(),
                                                             q.Battery == null ? "-" : q.Battery + "%",
                                                             !string.IsNullOrEmpty(q.TelefonskiBroj) ? q.TelefonskiBroj : "-", true));
                        //}
                    }
                    
                    return nova;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pozicije Redara MOBILE");
                return new List<_PozicijaDjelatnika>();
            }
        }
    }
}