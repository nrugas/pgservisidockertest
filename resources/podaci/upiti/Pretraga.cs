using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.cs.sigurnost;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Pretraga
    {
        public static List<_Prekrsaj> PrekrsajiIzvoz(string grad, int idDjelatnika, int idPredloska, DateTime? datumOd, DateTime? datumDo, bool storno, int idRedarstva, int idAplikacije)
        {
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
                               join r in db.PopisPrekrsajas on oo.IDPrekrsaja equals r.IDPrekrsaja into popis
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
                               where (idDjelatnika != 0 ? p.IDDjelatnika == idDjelatnika : idDjelatnika == 0) &&
                                     (idPredloska != 0 ? p.IDPredloskaIspisa == idPredloska : idPredloska == 0) &&
                                     (datumOd != null ? p.Vrijeme.Value.Date >= datumOd : datumOd == null) &&
                                     (datumDo != null ? p.Vrijeme.Value.Date <= datumDo : datumDo == null) &&
                                     (!storno ? p.Status == false : storno) &&
                                     p.IDRedarstva == idRedarstva &&
                                     p.Test == false
                               orderby p.Vrijeme ascending
                               select new _Prekrsaj
                               (
                                   p.IDPrekrsaja,
                                   p.IDRedarstva,
                                   (int)l.IDTerminala,
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
                                   d.UID,
                                   p.Adresa,
                                   p.BrojUpozorenja,
                                   tt.NazivTerminala,
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
                                   "",
                                   p.StatusVPP ?? "",
                                   p.KraticaDrzave,
                                   p.IDRacuna,
                                   "",
                                   Priprema.Nalog(p.IDNaloga, xx, ss, vv, nn, zz, bb.BrojRacuna,
                                       Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije))
                               );

                    return Priprema.PripremiPodatke(grad, prek, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pretrazi Prekrsaje");
                return new List<_Prekrsaj>();
            }
        }

        /*:: PONAVLJACI ::*/

        public static List<_Prekrsaj> DetaljiPonavljaca(string grad, int idPredloska, DateTime? datumOd, DateTime? datumDo, string registracija, bool samoPauk, int idRedarstva, int idAplikacije)
        {
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
                               join r in db.PopisPrekrsajas on oo.IDPrekrsaja equals r.IDPrekrsaja into popis
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
                               where (idPredloska > 0 ? p.IDPredloskaIspisa == idPredloska : idPredloska == 0) &&
                                     (datumOd != null ? p.Vrijeme.Value.Date >= datumOd : datumOd == null) &&
                                     (datumDo != null ? p.Vrijeme.Value.Date <= datumDo : datumDo == null) &&
                                     p.RegistracijskaPlocica == registracija &&
                                     p.IDRedarstva == idRedarstva &&
                                     p.Test == false &&
                                     (samoPauk ? p.NalogPauka == true : samoPauk == false)
                               orderby p.Vrijeme.Value ascending
                               select new _Prekrsaj
                               (
                                   p.IDPrekrsaja,
                                   p.IDRedarstva,
                                   (int)l.IDTerminala,
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
                                   d.UID,
                                   p.Adresa,
                                   p.BrojUpozorenja,
                                   tt.NazivTerminala,
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
                                   "",
                                   p.StatusVPP ?? "",
                                   p.KraticaDrzave,
                                   p.IDRacuna,
                                   "",
                                   Priprema.Nalog(p.IDNaloga, xx, ss, vv, nn, zz, bb.BrojRacuna,
                                       Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije))
                               );

                    return Priprema.PripremiPodatke(grad, prek, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pretrazi Prekrsaje");
                return new List<_Prekrsaj>();
            }
        }

        public static List<_2DLista> Ponavljaci(string grad, int idPredloska, DateTime? datumOd, DateTime? datumDo, int broj, bool samoPauk, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var prek = (from p in db.Prekrsajis
                                where (idPredloska > 0 ? p.IDPredloskaIspisa == idPredloska : idPredloska == 0) &&
                                      (datumOd != null ? p.Vrijeme.Value.Date >= datumOd : datumOd == null) &&
                                      (datumDo != null ? p.Vrijeme.Value.Date <= datumDo : datumDo == null) &&
                                      (p.RegistracijskaPlocica != "IZVID" && p.RegistracijskaPlocica != "ZNAK") &&
                                      p.IDRedarstva == idRedarstva &&
                                      p.Test == false &&
                                      (samoPauk ? p.NalogPauka == true : samoPauk == false)
                                select p).GroupBy(i => i.RegistracijskaPlocica, (key, g) => new _2DLista(g.Count(), key));

                    return prek.ToList().Where(i => i.Value >= broj).ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pretrazi Ponavljace Prekrsaja");
                return new List<_2DLista>();
            }
        }

        public static List<_2DLista> PonavljaciParking(string grad, int idStatusa, DateTime? datumOd, DateTime? datumDo, int broj, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var prek = (from o in db.PARKING_OPAZANJAs
                                where (idStatusa != -2 ? (idStatusa == -1 ? o.IDStatusa == null : o.IDStatusa == idStatusa) : true) &&
                                      (datumOd.HasValue ? o.Vrijeme.Value.Date >= datumOd : datumOd == null) &&
                                      (datumDo.HasValue ? o.Vrijeme.Value.Date <= datumDo : datumDo == null) 
                                select o).GroupBy(i => i.Registracija, (key, g) => new _2DLista(g.Count(), key));

                    return prek.ToList().Where(i => i.Value >= broj).ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pretrazi Ponavljace Prekrsaja");
                return new List<_2DLista>();
            }
        }

        public static List<_Opazanje> DetaljiPonavljacaParking(string grad, int idStatusa, DateTime? datumOd, DateTime? datumDo, string registracija, bool kaznjeni, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List<_2DLista> statusi = Parking.Statusi(false, idAplikacije);

                    var prek = from o in db.PARKING_OPAZANJAs
                               join r in db.RACUNIs on o.IDRacuna equals r.IDRacuna into racuni
                               from rr in racuni.DefaultIfEmpty()
                               join d in db.Djelatniks on o.IDDjelatnika equals d.IDDjelatnika into djelatnik
                               from dd in djelatnik.DefaultIfEmpty()
                               join s in db.PARKING_SEKTORIs on o.IDSektora equals s.IDSektora into sektori
                               from ss in sektori.DefaultIfEmpty()
                               join z in db.PARKING_ZONEs on ss.IDZone equals z.IDZone into zone
                               from zz in zone.DefaultIfEmpty()
                               where (kaznjeni ? o.Kaznjen == true : (idStatusa != -2 ? (idStatusa == -1 ? o.IDStatusa == null : o.IDStatusa == idStatusa) : true)) &&
                                     (datumOd.HasValue ? o.Vrijeme.Value.Date >= datumOd : datumOd == null) &&
                                     (datumDo.HasValue ? o.Vrijeme.Value.Date <= datumDo : datumDo == null) &&
                                     o.Registracija.ToLower().StartsWith(registracija.ToLower()) 
                               orderby o.Vrijeme.Value ascending
                               select new _Opazanje(o.IDOpazanja, o.IDLokacije, o.IDSektora, ss.IDZone, o.IDDjelatnika, 0, o.IDStatusa, o.IDRacuna, ss.NazivSektora, dd.ImePrezime, zz.NazivZone,
                                   Parking.Status(o.IDStatusa, statusi), o.Registracija, o.Drzava, (DateTime)o.Vrijeme, o.PlacenoDo, o.Latitude, o.Longitude, o.Iznos, o.Kaznjen.Value, (o.Otisao == null) ? false : o.Otisao.Value,
                                   rr.BrojRacuna,
                                   ""//broj kazni za registraciju
                               );

                    return prek.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pretrazi opazanja recidivisti");
                return new List<_Opazanje>();
            }
        }

        /*:: VREMENA PAUKA ::*/

        public static List<_VremenaPauka> VremenaPauka(string grad, DateTime DatumOd, DateTime DatumDo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var vre = from p in db.Pauks
                              join s in db.StatusPaukas on p.Status equals s.IDStatusa
                              where p.DatumNaloga.Date >= DatumOd.Date &&
                                    p.DatumNaloga.Date <= DatumDo.Date
                              select
                                  new _VremenaPauka(p.DatumNaloga, p.DatumZaprimanja, p.DatumPodizanja, p.DatumDeponija,
                                      p.DatumZaprimanja != null
                                          ? p.DatumZaprimanja.Value.Subtract(p.DatumNaloga)
                                          : new TimeSpan(0),
                                      p.DatumPodizanja != null && p.DatumZaprimanja != null
                                          ? p.DatumPodizanja.Value.Subtract((DateTime)p.DatumZaprimanja)
                                          : new TimeSpan(0),
                                      p.DatumDeponija != null && p.DatumPodizanja != null
                                          ? p.DatumDeponija.Value.Subtract((DateTime)p.DatumPodizanja)
                                          : new TimeSpan(0),
                                      new TimeSpan(),
                                      s.NazivStatusa);

                    return vre.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VREMENA PAUKA");
                return new List<_VremenaPauka>();
            }
        }

        /*:: PRETRAGA ::*/

        public static List<_Prekrsaj> ZabiljezeniPrekrsaji(string grad, int idDjelatnika, int idPredloska, DateTime? datumOd, DateTime? datumDo,
            bool pauk, bool registracija, bool dokument, bool ulica, bool storno, char? tipStorna, string pojam,
            bool test, bool hr, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var pretraga = from p in db.Prekrsajis
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
                                   where (datumOd != null ? p.Vrijeme.Value.Date >= datumOd : datumOd == null) &&
                                         (datumDo != null ? p.Vrijeme.Value.Date <= datumDo : datumDo == null) &&
                                         (idDjelatnika != 0 ? p.IDDjelatnika == idDjelatnika : idDjelatnika == 0) &&
                                         (idPredloska != 0 ? p.IDPredloskaIspisa == idPredloska : idPredloska == 0) &&
                                         (pauk ? (bool)p.NalogPauka : !pauk) &&
                                         (registracija ? p.RegistracijskaPlocica.Contains(pojam.ToUpper()) : !registracija) &&
                                         (dokument ? p.BrojUpozorenja.Contains(pojam) : !dokument) &&
                                         (ulica ? p.Adresa.Contains(pojam) : !ulica) &&
                                         (storno
                                             ? (tipStorna != 'z'
                                                 ? ((p.Status && (tipStorna == 'i'
                                                         ? p.Tekst.Contains(EncryptDecrypt.Encrypt(pojam))
                                                         : tipStorna != 'i')))
                                                 : tipStorna == 'z')
                                             : p.Status == false) &&
                                         p.IDRedarstva == idRedarstva &&
                                         (!test ? p.Test == false : test) &&
                                         (hr ? p.KraticaDrzave == "HR" : !hr)
                                   orderby p.Vrijeme.Value ascending
                                   select new _Prekrsaj
                                   (p.IDPrekrsaja,
                                       p.IDRedarstva,
                                       (int)l.IDTerminala,
                                       ss.IDOpisa,
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
                                       "",
                                       p.StatusVPP ?? "",
                                       p.KraticaDrzave,
                                       p.IDRacuna,
                                       qq.BrojRacuna,
                                       Priprema.Nalog(p.IDNaloga, xx, zz, vv, nn, yy, rr.BrojRacuna,
                                           Naplata.VrstaPlacanja(grad, rr.IDVrstePlacanja, idAplikacije)));

                    return Priprema.PripremiPodatke(grad, pretraga, idAplikacije);
                }
            }
            catch
            {
                return new List<_Prekrsaj>();
            }
        }

        public static List<_Prekrsaj> PretragaNaloga(string grad, int idStatusa, int idVozila, DateTime? datumOd, DateTime? datumDo,
            bool registracija, bool dokument, bool ulica, string pojam, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var pretraga = from n in db.NaloziPaukus
                                   join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa
                                   join p in db.Prekrsajis on n.IDNaloga equals p.IDNaloga
                                   join l in db.Lokacijes on p.IDLokacije equals l.IDLokacije
                                   join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa
                                   join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska into predlozak
                                   from ii in predlozak.DefaultIfEmpty()
                                   join v in db.PopisPrekrsajas on o.IDPrekrsaja equals v.IDPrekrsaja
                                   join t in db.Terminalis on l.IDTerminala equals t.IDTerminala into term
                                   from tt in term.DefaultIfEmpty()
                                   join r in db.RazloziNepodizanjaVozilas on n.IDRazloga equals r.IDRazloga into raz
                                   from rr in raz.DefaultIfEmpty()
                                   join y in db.VozilaPaukas on n.IDVozila equals y.IDVozila into voz
                                   from yy in voz.DefaultIfEmpty()
                                   join b in db.RACUNIs on n.IDRacuna equals b.IDRacuna into rac
                                   from bb in rac.DefaultIfEmpty()
                                   join x in db.Pauks on p.IDNaloga equals x.IDNaloga into pau
                                   from xx in pau.DefaultIfEmpty()
                                   where p.NalogPauka == true &&
                                         p.IDNaloga > 0 &&
                                         (datumOd != null ? p.Vrijeme.Value.Date >= datumOd : datumOd == null) &&
                                         (datumDo != null ? p.Vrijeme.Value.Date <= datumDo : datumDo == null) &&
                                         (idStatusa != 0 ? n.IDStatusa == idStatusa : idStatusa == 0) &&
                                         (idVozila != 0 ? n.IDVozila == idVozila : idVozila == 0) &&
                                         (registracija ? p.RegistracijskaPlocica.Contains(pojam.ToUpper()) : !registracija) &&
                                         (dokument ? p.IDNaloga.ToString() == pojam : !dokument) &&
                                         (ulica ? p.Adresa.Contains(pojam) : !ulica)
                                   orderby p.Vrijeme.Value ascending
                                   select new _Prekrsaj
                                   (p.IDPrekrsaja,
                                       p.IDRedarstva,
                                       (int)l.IDTerminala,
                                       o.IDOpisa,
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
                                       d.UID,
                                       p.Adresa,
                                       p.BrojUpozorenja,
                                       tt.IDTerminala == 0 ? "RUČNI UNOS" : tt.NazivTerminala,
                                       ii.NazivPredloska,
                                       o.OpisPrekrsaja,
                                       o.KratkiOpis,
                                       "",
                                       v.MaterijalnaKaznjivaNorma,
                                       "",
                                       o.ClanakPauka,
                                       v.Kazna + ",00 kn", //todo
                                       p.NalogPauka,
                                       p.Zahtjev,
                                       p.Status,
                                       p.Test,
                                       p.TrajanjePostupka,
                                       Priprema.Ocitanje(p.StatusOcitanja),
                                       "",
                                       "",
                                       null,
                                       "",
                                       p.StatusVPP ?? "",
                                       p.KraticaDrzave,
                                       p.IDRacuna,
                                       "",
                                       Priprema.Nalog(p.IDNaloga, yy, s, rr, n, xx, bb.BrojRacuna,
                                           Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije))
                                   );

                    return Priprema.PripremiPodatke(grad, pretraga, idAplikacije);
                }
            }
            catch
            {
                return new List<_Prekrsaj>();
            }
        }

        public static List<_Prekrsaj> PretragaNalogaZaNaplatu(string grad, int idStatusa, bool drzava, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    DateTime pocetak;
                    using (PostavkeDataContext pb = new PostavkeDataContext())
                    {
                        pocetak = pb.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad)).NaplataPauk.Value;
                    }

                    var pretraga = from n in db.NaloziPaukus
                                   join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa
                                   join p in db.Prekrsajis on n.IDNaloga equals p.IDNaloga
                                   join l in db.Lokacijes on p.IDLokacije equals l.IDLokacije
                                   join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa into opis
                                   from oo in opis.DefaultIfEmpty()
                                   join v in db.PopisPrekrsajas on oo.IDPrekrsaja equals v.IDPrekrsaja into popis
                                   from vv in popis.DefaultIfEmpty()
                                   join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska into predlozak
                                   from ii in predlozak.DefaultIfEmpty()
                                   join t in db.Terminalis on l.IDTerminala equals t.IDTerminala into term
                                   from tt in term.DefaultIfEmpty()
                                   join r in db.RazloziNepodizanjaVozilas on n.IDRazloga equals r.IDRazloga into raz
                                   from rr in raz.DefaultIfEmpty()
                                   join y in db.VozilaPaukas on n.IDVozila equals y.IDVozila into voz
                                   from yy in voz.DefaultIfEmpty()
                                   join b in db.RACUNIs on n.IDRacuna equals b.IDRacuna into rac
                                   from bb in rac.DefaultIfEmpty()
                                   join x in db.Pauks on p.IDNaloga equals x.IDNaloga into pau
                                   from xx in pau.DefaultIfEmpty()
                                   where p.NalogPauka == true &&
                                         p.IDNaloga > 0 &&
                                         n.IDStatusa == idStatusa &&
                                         n.IDRacuna == null &&
                                         n.DatumNaloga.Date >= pocetak.Date
                                   orderby p.Vrijeme.Value ascending
                                   select new _Prekrsaj
                                   (p.IDPrekrsaja,
                                       p.IDRedarstva,
                                       (int)l.IDTerminala,
                                       oo.IDOpisa,
                                       p.IDOpisaZakona,
                                       p.IDLokacije,
                                       (int)p.IDDjelatnika,
                                       p.IDPredloskaIspisa ?? -1,
                                       p.Lat,
                                       p.Long,
                                       p.Vrijeme.Value,
                                       drzava
                                           ? Priprema.Registracija(p.RegistracijskaPlocica, p.KraticaDrzave)
                                           : p.RegistracijskaPlocica,
                                       d.ImePrezime,
                                       d.BrojSI,
                                       d.UID,
                                       p.Adresa,
                                       p.BrojUpozorenja,
                                       tt.IDTerminala == 0 ? "RUČNI UNOS" : tt.NazivTerminala,
                                       ii.NazivPredloska,
                                       oo.OpisPrekrsaja,
                                       oo.KratkiOpis,
                                       "",
                                       vv.MaterijalnaKaznjivaNorma,
                                       "",
                                       oo.ClanakPauka,
                                       p.Kazna.ToString(),
                                       p.NalogPauka,
                                       p.Zahtjev,
                                       p.Status,
                                       p.Test,
                                       p.TrajanjePostupka,
                                       Priprema.Ocitanje(p.StatusOcitanja),
                                       "",
                                       "",
                                       null,
                                       "",
                                       p.StatusVPP ?? "",
                                       p.KraticaDrzave,
                                       p.IDRacuna,
                                       "",
                                       Priprema.Nalog(p.IDNaloga, yy, s, rr, n, xx, bb.BrojRacuna,
                                           Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije))
                                   );

                    return Priprema.PripremiPodatke(grad, pretraga, idAplikacije);
                }
            }
            catch
            {
                return new List<_Prekrsaj>();
            }
        }
    }
}