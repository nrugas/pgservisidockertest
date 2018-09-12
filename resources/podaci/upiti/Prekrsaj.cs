using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.cs.email;
using PG.Servisi.resources.cs.sigurnost;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Prekrsaj
    {
        /*:: PREKRŠAJ GLAVNI MENU ::*/

        public static List<_Prekrsaj> PretraziPrekrsaje(string grad, int idDjelatnika, DateTime datum, int idRedarstva, int idAplikacije)
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
                               where (idDjelatnika != 0 ? p.IDDjelatnika == idDjelatnika : idDjelatnika == 0) &&
                                     //(!obavijesti ? p.IDPredloskaIspisa != 2 : obavijesti) &&
                                     //(!upozorenja ? p.IDPredloskaIspisa != 1 : upozorenja) &&
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

                    return Priprema.PripremiPodatke(grad, prek, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Pretrazi Prekrsaje");
                return new List<_Prekrsaj>();
            }
        }

        public static _Prekrsaj DetaljiPrekrsaja(string grad, int idLokacije, int idAplikacije)
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
                               join q in db.RACUNIs on p.IDRacuna equals q.IDRacuna into racP
                               from qq in racP.DefaultIfEmpty()
                               where p.IDLokacije == idLokacije
                               select new _Prekrsaj
                                   (
                                   p.IDPrekrsaja,
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
                                   "",
                                   p.StatusVPP ?? "",
                                   p.KraticaDrzave,
                                   p.IDRacuna,
                                   qq.BrojRacuna,
                                   Priprema.Nalog(p.IDNaloga, xx, ss, vv, nn, zz, bb.BrojRacuna, Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije)));

                    return Priprema.PripremiPodatke(grad, prek, idAplikacije).First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Detalji Prekrsaja");
                return null;
            }
        }

        public static _Prekrsaj DetaljiPrekrsajaNalog(string grad, int idNaloga, int idAplikacije)
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
                               where p.IDNaloga == idNaloga
                               select new _Prekrsaj
                                   (
                                   p.IDPrekrsaja,
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
                                   "",
                                   p.StatusVPP ?? "",
                                   p.KraticaDrzave,
                                   p.IDRacuna,
                                   "",
                                   Priprema.Nalog(p.IDNaloga, xx, ss, vv, nn, zz, bb.BrojRacuna, Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije)));

                    if (!prek.Any())
                    {
                        return null;
                    }

                    return Priprema.PripremiPodatke(grad, prek, idAplikacije).First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Detalji Prekrsaja - nalog");
                return null;
            }
        }

        public static _Prekrsaj DetaljiPrekrsajaBroj(string grad, string broj, int idAplikacije)
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
                               join q in db.RACUNIs on p.IDRacuna equals q.IDRacuna into racP
                               from qq in racP.DefaultIfEmpty()
                               where p.BrojUpozorenja == broj
                               select new _Prekrsaj
                                   (
                                   p.IDPrekrsaja,
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
                                   "",
                                   p.StatusVPP ?? "",
                                   p.KraticaDrzave,
                                   p.IDRacuna,
                                   qq.BrojRacuna,
                                   Priprema.Nalog(p.IDNaloga, xx, ss, vv, nn, zz, bb.BrojRacuna, Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije)));

                    return Priprema.PripremiPodatke(grad, prek, idAplikacije).First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Detalji Prekrsaja");
                return null;
            }
        }

        //todo !!! obrisati nakon usklađivanja baza
        public static _Prekrsaj DetaljiPrekrsajaO(string grad, int idLokacije, int idAplikacije)
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
                               where p.IDLokacije == idLokacije
                               select new _Prekrsaj
                                   (
                                   p.IDPrekrsaja,
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
                                   p.KraticaDrzave != "??" ? p.RegistracijskaPlocica + " (" + p.KraticaDrzave + ")" : p.RegistracijskaPlocica,
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
                                   (p.StatusOcitanja & 3) == 3 ? "I" : (p.StatusOcitanja & 3) == 1 ? "O" : "R",
                                   EncryptDecrypt.Decrypt(p.Tekst),
                                   EncryptDecrypt.Decrypt(p.Napomena),
                                   null,//new _KomentarPostupanja(kk.IDKomentara, kk.IDPrekrsaja, kk.Primjedba, kk.Obrazlozenje, kk.IspravnoPostupanje),
                                   "",
                                   p.StatusVPP ?? "",
                                   p.KraticaDrzave,
                                   p.IDRacuna,
                                   "",
                                   Priprema.Nalog(p.IDNaloga, xx, ss, vv, nn, zz, "", ""));

                    return Priprema.PripremiPodatke(grad, prek, idAplikacije).First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Detalji Prekrsaja");
                return null;
            }
        }

        /*:: SLIKE ::*/

        public static List<byte[]> Slike(string grad, int idLokacije, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var slike = from s in db.SlikaPrekrsajas
                                where s.IDLokacije == idLokacije
                                select s.Slika.ToArray();

                    return slike.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Slike");
                return new List<byte[]>();
            }
        }

        public static List<int> DodajSliku(string grad, int idLokacije, List<byte[]> slike, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    string uid = "", ulica = "";
                    DateTime d = DateTime.Now;

                    if (idRedarstva == 3)
                    {
                        try
                        {
                            Prekrsaji p = db.Prekrsajis.First(i => i.IDLokacije == idLokacije);
                            ulica = p.Adresa;
                            d = p.Vrijeme.Value;

                            uid = db.Djelatniks.First(i => i.IDDjelatnika == p.IDDjelatnika.Value).UID;
                        }
                        catch
                        {

                        }
                    }

                    List<int> id = new List<int>();
                    foreach (var sl in slike)
                    {
                        SlikaPrekrsaja slika = new SlikaPrekrsaja();

                        slika.IDLokacije = idLokacije;

                        if (idRedarstva == 3)
                        {
                            slika.Slika = cs.Slike.TimeStamp(sl, d, ulica, uid);
                        }
                        else
                        {
                            slika.Slika = sl;
                        }

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

        public static bool ObrisiSliku(string grad, int idSlike, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.SlikaPrekrsajas.DeleteOnSubmit(db.SlikaPrekrsajas.First(i => i.IDSlikePrekrsaja == idSlike));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "");
                return false;
            }
        }

        public static List<_Slika> SlikePrekrsaja(string grad, int idLokacije, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.SlikaPrekrsajas.Where(i => i.IDLokacije == idLokacije).Select(i => new _Slika(i.IDSlikePrekrsaja, i.Slika.ToArray())).ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "");
                return new List<_Slika>();
            }
        }

        public static int RotirajSliku(string grad, int idSlike, byte[] slika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    SlikaPrekrsaja sp = db.SlikaPrekrsajas.First(i => i.IDSlikePrekrsaja == idSlike);
                    sp.Slika = slika;
                    db.SubmitChanges();

                    return sp.IDLokacije;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "");
                return -1;
            }
        }

        public static void DodajFotografijuGO(string grad, int idLokacije, byte[] s, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    string uid = "", ulica = "";
                    DateTime d = DateTime.Now;

                    try
                    {
                        Prekrsaji p = db.Prekrsajis.First(i => i.IDLokacije == idLokacije);
                        ulica = p.Adresa;
                        d = p.Vrijeme.Value;

                        uid = db.Djelatniks.First(i => i.IDDjelatnika == p.IDDjelatnika.Value).UID;
                    }
                    catch
                    {

                    }

                    SlikaPrekrsaja slika = new SlikaPrekrsaja();
                    slika.IDLokacije = idLokacije;
                    slika.Slika = cs.Slike.TimeStamp(s, d, ulica, uid);

                    db.SlikaPrekrsajas.InsertOnSubmit(slika);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dodaj Sliku");
            }
        }

        /*:: RUČNO DODAVANJE PREKRSAJA ::*/

        public static List<string> BrojPrekrsaja(string grad, string registracija, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var zak = from p in db.Prekrsajis
                              where p.RegistracijskaPlocica == registracija.Replace("-", "").Replace(" ", "")
                              group p by p.IDPredloskaIspisa
                                  into x
                              select new
                              {
                                  ID = x.Key,
                                  broj = x.Count(),
                                  sum = x.Sum(i => i.Kazna)
                              };

                    List<string> novi = new List<string>();

                    foreach (var q in zak)
                    {
                        string upob = "OBAVIJEST";

                        if (q.ID == 1 || q.ID == 14)
                        {
                            upob = "UPOZORENJE";
                        }

                        novi.Add(upob + "     " + q.broj + " (" + q.sum + ")");
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Broj Prekrsaja za registraciju: " + registracija);
                return null;
            }
        }

        public static int DodajRucniPrekrsaj(string grad, _Prekrsaj prekrsaj, bool kaznjava, List<byte[]> slike, int idRedarstva, bool lisice, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (!db.Terminalis.Any(i => i.IDTerminala == 0))
                    {
                        DodajRucniTerminal(grad, idAplikacije);
                    }

                    #region LOKACIJA

                    Lokacije lok = new Lokacije();

                    lok.Lat = prekrsaj.Latitude;
                    lok.Long = prekrsaj.Longitude;
                    lok.RegistracijskaPlocica = prekrsaj.Registracija;
                    lok.DatumVrijeme = prekrsaj.DatumVrijeme;
                    lok.IDDjelatnika = prekrsaj.IDDjelatnika;
                    lok.IDNacinaPozicioniranja = 3;
                    lok.IDTerminala = 0;
                    lok.CellTowerID = null;
                    lok.SignalStrength = null;
                    lok.HDOP = 0;
                    lok.Brzina = 0;
                    lok.Punjac = false;

                    db.Lokacijes.InsertOnSubmit(lok);
                    db.SubmitChanges();

                    #endregion

                    string brojUp;

                    if (!string.IsNullOrEmpty(prekrsaj.BrojDokumenta))
                    {
                        brojUp = prekrsaj.BrojDokumenta;
                    }
                    else
                    {
                        brojUp = GenerirajPozivNaBroj(grad, kaznjava, prekrsaj.DatumVrijeme, Convert.ToDecimal(prekrsaj.Kazna), idAplikacije);
                    }

                    int idPredloska;

                    if (prekrsaj.IDDokumenta != 0)
                    {
                        idPredloska = prekrsaj.IDDokumenta;
                    }
                    else
                    {
                        if (db.PredlosciIspisas.Any(i => i.IDRedarstva == idRedarstva && i.Pauk && i.Kaznjava == kaznjava))
                        {
                            idPredloska = db.PredlosciIspisas.First(i => i.IDRedarstva == idRedarstva && i.Pauk && i.Kaznjava == kaznjava).IDPRedloska;
                        }
                        else
                        {
                            idPredloska = kaznjava ? IDPredloskaObavijesti(grad, idAplikacije) : IDPredloskaUpozorenja(grad, idAplikacije);
                        }
                    }

                    #region PREKRSAJ

                    Prekrsaji prek = new Prekrsaji();

                    prek.IDLokacije = lok.IDLokacije;
                    prek.IDSkracenogOpisa = prekrsaj.IDOpisaPrekrsaja;
                    prek.IDDjelatnika = prekrsaj.IDDjelatnika;
                    prek.Vrijeme = prekrsaj.DatumVrijeme;
                    prek.RegistracijskaPlocica = prekrsaj.Registracija;
                    prek.BrojUpozorenja = brojUp;
                    prek.Lat = prekrsaj.Latitude;
                    prek.Long = prekrsaj.Longitude;
                    prek.IDNacinaPozicioniranja = 3;
                    prek.Adresa = prekrsaj.Adresa;
                    prek.Kazna = Convert.ToInt32(prekrsaj.Kazna);
                    prek.IDPredloskaIspisa = idPredloska;
                    prek.PozivNaBroj = kaznjava ? brojUp : "";
                    prek.Test = prekrsaj.Test;
                    prek.Poslano = false;
                    prek.Status = false;
                    prek.Napomena = null;
                    prek.Tekst = null;
                    prek.GMapsUlica = null;
                    prek.NalogPauka = prekrsaj.Pauk;
                    prek.IDNaloga = null;
                    prek.Zakljucan = false;
                    prek.IDRacuna = null;
                    prek.KraticaDrzave = prekrsaj.Drzava;
                    prek.StatusOcitanja = 1;
                    prek.Ocitanja = "";
                    prek.TrajanjePostupka = 0;
                    prek.Rucno = true;
                    prek.IDOpisaZakona = Zakoni.DohvatiIDNovogZakona(grad, prekrsaj.IDOpisaPrekrsaja, idAplikacije);
                    prek.IDRedarstva = idRedarstva;

                    db.Prekrsajis.InsertOnSubmit(prek);
                    db.SubmitChanges();

                    //todo - trenutno je samo za lokacije, kad zavrsi testno razdoblje to makni
                    if (grad == "Lokacije")
                    {
                        //todo gledati po idredarstva i statusu predloska kaznjava
                        if (prek.IDPredloskaIspisa == 15 || prek.IDPredloskaIspisa == 2)
                        {
                            Vpp._VppPrijenos prijenos = new Vpp._VppPrijenos(prek.IDPrekrsaja, 1, prek.Kazna, prek.PozivNaBroj, "Obavijest");
                            bool ok = Vpp.DodajVPP(grad, prijenos, idAplikacije);

                            if (ok)
                            {
                                prek.Poslano = true;
                                db.SubmitChanges();
                            }
                        }
                    }

                    #endregion

                    #region PAUK

                    if (prekrsaj.Pauk == true)
                    {
                        NalogPauku(grad, prek.IDPrekrsaja, prekrsaj.DatumVrijeme, lisice, prekrsaj.NapomenaStorna, idAplikacije);
                    }

                    #endregion

                    #region SLIKE

                    DodajSliku(grad, lok.IDLokacije, slike, idRedarstva, idAplikacije);

                    #endregion

                    if (kaznjava)
                    {
                        new Thread(() => RentaCar.PostojiRCVozilo(grad, prek.RegistracijskaPlocica, prek.IDLokacije, 0, idAplikacije)).Start();
                    }

                    return lok.IDLokacije;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dodao Rucni Prekrsaj");
                return -1;
            }
        }

        public static string GenerirajPozivNaBroj(string grad, bool obavijest, DateTime datum, decimal kazna, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    string k = kazna.ToString().Substring(0, 3);

                    if (obavijest)
                    {
                        string sifra = Gradovi.Uplatnica(grad, 1, idAplikacije).Sifra;

                        string l = sifra + DateTime.Now.Year.ToString().Remove(0, 3) + "8";

                        var brob = from p in db.Prekrsajis
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                   where p.Rucno &&
                                         p.Kazna == Convert.ToDecimal(k) &&
                                         i.NazivPredloska == "OBAVIJEST" &&
                                         p.Vrijeme.Value.Year == DateTime.Now.Year &&
                                         p.PozivNaBroj.Contains(l)
                                   select p;


                        //todo ne valja jer kada dodje do 9999 doda novu znamenku tj produzi se poziv na broj
                        string bo = "0" + k.Remove(2) + sifra + DateTime.Now.Year.ToString().Remove(0, 3) + "8";

                        int y = 1;
                        string broj = (brob.Any() ? brob.Count() + y : 1).ToString("0000");

                        while (db.Prekrsajis.Any(i => i.BrojUpozorenja == bo + broj))
                        {
                            y++;
                            broj = (brob.Count() + y).ToString("0000");
                        }

                        return bo + broj;
                    }

                    var brup = from p in db.Prekrsajis
                               join l in db.Lokacijes on p.IDLokacije equals l.IDLokacije
                               join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                               where p.Vrijeme.Value.Date == datum.Date &&
                                     i.NazivPredloska == "UPOZORENJE" &&
                                     p.BrojUpozorenja.StartsWith("0")
                               select new { p, l };

                    int br;

                    if (brup.Any())
                    {
                        br = brup.Count() + 1;
                    }
                    else
                    {
                        br = 1;
                    }

                    string up = "0-" + datum.Day.ToString("00") + datum.Month.ToString("00") + datum.Year.ToString("00");

                    while (db.Prekrsajis.Any(i => i.BrojUpozorenja == up + br))
                    {
                        br++;
                    }

                    return up + br;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Generiraj Poziv Na Broj");
                return "";
            }
        }

        public static bool DodijeliPozivNaBroj(string grad, _Prekrsaj prekrsaj, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    bool obavijest = prekrsaj.Dokument == "OBAVIJEST";

                    string broj = GenerirajPozivNaBroj(grad, obavijest, DateTime.Now, Convert.ToDecimal(prekrsaj.Kazna), idAplikacije);

                    Prekrsaji prek = db.Prekrsajis.First(i => i.IDPrekrsaja == prekrsaj.IDPrekrsaja);
                    prek.BrojUpozorenja = broj;
                    prek.PozivNaBroj = obavijest ? broj : "";

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Generiraj Poziv Na Broj");
                return false;
            }
        }

        public static int IDPredloskaUpozorenja(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.PredlosciIspisas.First(i => i.NazivPredloska == "UPOZORENJE").IDPRedloska;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int IDPredloskaObavijesti(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.PredlosciIspisas.First(i => i.NazivPredloska == "OBAVIJEST").IDPRedloska;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static void DodajRucniTerminal(string grad, int idAplikacije)
        {
            try
            {
                //using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                //{
                using (DbConnection connection = new SqlConnection(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    connection.Open();
                    using (DbCommand command = new SqlCommand("SET IDENTITY_INSERT Terminali ON"))
                    {
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = new SqlCommand("INSERT INTO Terminali (IDTerminala,NazivTerminala)VALUES(0, 'Ručno');"))
                    {
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }

                    using (DbCommand command = new SqlCommand("SET IDENTITY_INSERT Terminali OFF"))
                    {
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dodao Rucni Prekrsaj");
            }
        }

        /*:: IZMJENE PREKRSAJA ::*/

        public static bool Prenesen(string grad, int idPrekrsaja, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return (bool)db.Prekrsajis.First(i => i.IDPrekrsaja == idPrekrsaja).Poslano;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool RelokacijaPrekrsaja(string grad, int id, decimal latitude, decimal longitude, bool opazanje, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (opazanje)
                    {
                        PARKING_OPAZANJA pr = db.PARKING_OPAZANJAs.First(i => i.IDOpazanja == id);
                        pr.Latitude = latitude;
                        pr.Longitude = longitude;

                        db.SubmitChanges();

                    }
                    else
                    {
                        Prekrsaji pr = db.Prekrsajis.First(i => i.IDPrekrsaja == id);
                        pr.Lat = latitude;
                        pr.Long = longitude;

                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Relokacija Prekrsaja");
                return false;
            }
        }

        public static bool Storniraj(string grad, int idPrekrsaja, string napomena, string osoba, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Prekrsaji p = db.Prekrsajis.Single(s => s.IDPrekrsaja == idPrekrsaja);

                    p.Status = true;
                    p.Napomena = EncryptDecrypt.Encrypt(napomena);
                    p.Tekst = EncryptDecrypt.Encrypt(osoba.ToUpper());

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Storniraj");
                return false;
            }
        }

        public static bool Test(string grad, int idPrekrsaja, bool test, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Prekrsaji p = db.Prekrsajis.First(s => s.IDPrekrsaja == idPrekrsaja);
                    p.Test = test;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Test");
                return false;
            }
        }

        public static bool Registracija(string grad, int id, string registracija, string kratica, bool opazanje, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (opazanje)
                    {
                        PARKING_OPAZANJA p = db.PARKING_OPAZANJAs.First(s => s.IDOpazanja == id);
                        p.Registracija = registracija;
                        p.Drzava = kratica;
                        db.SubmitChanges();

                        Lokacije l = db.Lokacijes.First(i => i.IDLokacije == p.IDLokacije);
                        l.RegistracijskaPlocica = registracija;
                        db.SubmitChanges();
                    }
                    else
                    {
                        Prekrsaji p = db.Prekrsajis.First(s => s.IDPrekrsaja == id);
                        p.RegistracijskaPlocica = registracija;
                        p.KraticaDrzave = kratica;
                        db.SubmitChanges();

                        Lokacije l = db.Lokacijes.First(i => i.IDLokacije == p.IDLokacije);
                        l.RegistracijskaPlocica = registracija;
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Registracija");
                return false;
            }
        }

        public static bool Adresa(string grad, int idPrekrsaja, string adresa, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Prekrsaji p = db.Prekrsajis.First(s => s.IDPrekrsaja == idPrekrsaja);
                    p.Adresa = adresa;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Adresa");
                return false;
            }
        }

        public static string BrojDokumenta(string grad, _Prekrsaj prekrsaj, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Prekrsaji p = db.Prekrsajis.First(s => s.IDPrekrsaja == prekrsaj.IDPrekrsaja);

                    string broj;
                    if (prekrsaj.Dokument == "OBAVIJEST")
                    {
                        broj = GenerirajPozivNaBroj(grad, false, prekrsaj.DatumVrijeme, Convert.ToDecimal(prekrsaj.Kazna.Replace(",00 kn", "")), idAplikacije);
                    }
                    else
                    {
                        broj = p.PozivNaBroj != "" && !p.PozivNaBroj.Contains("-") ? p.PozivNaBroj : GenerirajPozivNaBroj(grad, true, prekrsaj.DatumVrijeme, Convert.ToDecimal(prekrsaj.Kazna.Replace(",00 kn", "")), idAplikacije);

                        if (db.Prekrsajis.Any(i => i.BrojUpozorenja == broj))
                        {
                            broj = GenerirajPozivNaBroj(grad, true, prekrsaj.DatumVrijeme, Convert.ToDecimal(prekrsaj.Kazna.Replace(",00 kn", "")), idAplikacije);
                        }
                    }

                    return broj;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VRSTA - OBAVIJEST -> UPOZORENJE");
                return "";
            }
        }

        public static int Vrsta(string grad, _Prekrsaj prekrsaj, string broj, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.Prekrsajis.Any(i => i.PozivNaBroj == broj && i.IDPrekrsaja != prekrsaj.IDPrekrsaja))
                    {
                        return -2;
                    }

                    Prekrsaji p = db.Prekrsajis.First(s => s.IDPrekrsaja == prekrsaj.IDPrekrsaja);

                    p.IDPredloskaIspisa = prekrsaj.Dokument == "OBAVIJEST" ? IDPredloskaUpozorenja(grad, idAplikacije) : IDPredloskaObavijesti(grad, idAplikacije);
                    p.BrojUpozorenja = broj;

                    db.SubmitChanges();

                    return (int)p.IDPredloskaIspisa;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VRSTA - OBAVIJEST -> UPOZORENJE");
                return -1;
            }
        }

        public static string ObrisiSveStornirane(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var stornirani = from p in db.Prekrsajis
                                     where p.Status
                                     select p;

                    string akc = "Obrisano je " + stornirani.Count() + " prekršaja!\r\n";
                    foreach (var p in stornirani)
                    {
                        akc += string.Format("R: {0}, BD: {1}, A:{2}, IDL:{3}, IDO: {4}\r\n", p.RegistracijskaPlocica, p.BrojUpozorenja, p.Adresa, p.IDLokacije, p.IDSkracenogOpisa);
                    }

                    db.Prekrsajis.DeleteAllOnSubmit(stornirani);
                    db.SubmitChanges();

                    Posalji.Email(grad, akc, "Korisnik je obrisao stornirane prekršaje!",
                        new List<string> { "daniel.pajalic@ri-ing.net" }, null, false, idAplikacije);

                    return "Obrisano je " + stornirani.Count() + " prekršaja!";
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VRSTA - OBAVIJEST -> UPOZORENJE");
                return "";
            }
        }

        public static bool IzmjeniZakonPrekrsaja(string grad, int idPrekrsaja, int idOpisa, decimal kazna, int idRedarstva, out bool dodan, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Prekrsaji stari = db.Prekrsajis.First(i => i.IDPrekrsaja == idPrekrsaja);

                    if (stari.Kazna != kazna && stari.Poslano == true)
                    {
                        stari.Status = true;
                        stari.Napomena = EncryptDecrypt.Encrypt("Storniran te dodan novi, zbog promjene zakon postupnja i iznosa kazne.");
                        stari.Tekst = EncryptDecrypt.Encrypt("");

                        #region PREKRSAJ

                        Prekrsaji novi = new Prekrsaji();

                        novi.IDLokacije = stari.IDLokacije;
                        novi.IDSkracenogOpisa = idOpisa;
                        novi.IDDjelatnika = stari.IDDjelatnika;
                        novi.Vrijeme = stari.Vrijeme;
                        novi.RegistracijskaPlocica = stari.RegistracijskaPlocica;
                        novi.BrojUpozorenja = stari.BrojUpozorenja;
                        novi.Lat = stari.Lat;
                        novi.Long = stari.Long;
                        novi.IDNacinaPozicioniranja = stari.IDNacinaPozicioniranja;
                        novi.Adresa = stari.Adresa;
                        novi.Kazna = kazna;
                        novi.IDPredloskaIspisa = stari.IDPredloskaIspisa;
                        novi.PozivNaBroj = stari.PozivNaBroj;
                        novi.Test = false;
                        novi.Poslano = false;
                        novi.Status = false;
                        novi.Napomena = null;
                        novi.Tekst = null;
                        novi.GMapsUlica = stari.GMapsUlica;
                        novi.NalogPauka = stari.NalogPauka;
                        novi.IDNaloga = stari.IDNaloga;
                        novi.Zakljucan = false;
                        novi.IDRacuna = stari.IDRacuna;
                        //obrisi
                        //novi.IDRedarNaplate = null;
                        //novi.IDVrstaPlacanja = null;
                        //novi.Placeno = false;
                        //novi.DatumPlacanja = null;
                        //novi.PlacanjePreneseno = false;
                        novi.KraticaDrzave = stari.KraticaDrzave;
                        novi.StatusOcitanja = stari.StatusOcitanja;
                        novi.Ocitanja = stari.Ocitanja;
                        novi.TrajanjePostupka = stari.TrajanjePostupka;
                        novi.Rucno = stari.Rucno;
                        novi.Zahtjev = stari.Zahtjev;
                        novi.IDOpisaZakona = Zakoni.DohvatiIDNovogZakona(grad, idOpisa, idAplikacije);
                        novi.IDRedarstva = idRedarstva;

                        db.Prekrsajis.InsertOnSubmit(novi);
                        db.SubmitChanges();

                        #endregion

                        dodan = true;

                        try
                        {
                            OpisiPrekrsaja op = db.OpisiPrekrsajas.First(i => i.IDOpisa == idOpisa);

                            string poruka = "Izmjenjen je zakon postupanja i iznos kazne na prekršaju s pozivom na broj " + stari.BrojUpozorenja +
                                            ". Preneseni prekršaj je storniran i kreiran je identičan prekršaj sa navedenim izmjenama.\r\n" +
                                            "\r\nNovi opis: " + op.OpisPrekrsaja +
                                            "\r\nNovi članak: " + db.PopisPrekrsajas.First(i => i.IDPrekrsaja == op.IDPrekrsaja).MaterijalnaKaznjivaNorma +
                                            "\r\nNovi iznos: " + kazna.ToString("n2") + " kn";

                            Sustav.ZahtjevVPP(grad, poruka, idAplikacije);
                        }
                        catch (Exception ex)
                        {
                            Sustav.SpremiGresku(grad, ex, idAplikacije, "POSALJI LAZOTU EMAIL");
                        }
                    }
                    else
                    {
                        dodan = false;

                        stari.IDSkracenogOpisa = idOpisa; //todo - promijeni na novo ako ikad bude
                        stari.IDOpisaZakona = Zakoni.DohvatiIDNovogZakona(grad, idOpisa, idAplikacije);

                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI OPIS ZAKONA PREKRŠAJA");
                dodan = false;
                return false;
            }
        }

        /*::  PAUK ::*/

        public static int NalogPauku(string grad, int idPrekrsaja, DateTime datum, bool lisice, string napomena, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int idNaloga = 1;

                    Prekrsaji prek = db.Prekrsajis.First(i => i.IDPrekrsaja == idPrekrsaja);

                    //todo obrisi
                    //if (grad.ToUpper().Contains("RIJEKA"))
                    //{
                    //    _Prekrsaj p = DetaljiPrekrsaja(grad, prek.IDLokacije, idAplikacije);

                    //    idNaloga = Financije.SpremiNalog(grad, p.Registracija, p.BrojDokumenta, p.IDDjelatnika.ToString(), p.Redar,
                    //        p.DatumVrijeme, p.IDPrekrsaja, p.OpisPrekrsaja, p.Clanak, prek.Adresa, idAplikacije);

                    //    if (idNaloga == -1)
                    //    {
                    //        return -1;
                    //    }

                    //    new Thread(() => Financije.Slike(grad, idNaloga, prek.IDLokacije, idAplikacije)).Start();
                    //}
                    //else
                    //{
                    //    if (db.NaloziPaukus.Any())
                    //    {
                    //        idNaloga = db.NaloziPaukus.Max(i => i.IDNaloga) + 1;
                    //    }
                    //}
                    //

                    if (db.NaloziPaukus.Any())
                    {
                        idNaloga = db.NaloziPaukus.Max(i => i.IDNaloga) + 1;
                    }

                    NaloziPauku nal = new NaloziPauku();

                    nal.IDNaloga = idNaloga;
                    nal.IDStatusa = 0;
                    nal.IDVozila = null;
                    nal.NalogZatvoren = false;
                    nal.DatumNaloga = datum;
                    nal.StornoRedara = false;
                    nal.Redoslijed = 0;
                    nal.IDRazloga = 0;
                    nal.Lisice = lisice;
                    nal.Napomena = napomena;

                    db.NaloziPaukus.InsertOnSubmit(nal);
                    db.SubmitChanges();

                    Pauk pau = new Pauk();
                    pau.IDNaloga = idNaloga;
                    pau.Status = 0;
                    pau.NalogZatvoren = false;
                    pau.DatumNaloga = datum;
                    pau.StornoRedara = false;

                    db.Pauks.InsertOnSubmit(pau);
                    db.SubmitChanges();

                    prek.NalogPauka = true;
                    prek.IDNaloga = idNaloga;
                    db.SubmitChanges();

                    Nalog.SpremiPovijest(grad, idNaloga, null, 0, true, idAplikacije);

                    new Thread(() => MailLista.PosaljiNaredbu(grad, idNaloga, idAplikacije)).Start();

                    return idNaloga;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ NALOG PAUKU");
                return -1;
            }
        }

        /*::  PARKING ::*/

        public static bool VoziloOtislo(string grad, int idOpazanja, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PARKING_OPAZANJA pr = db.PARKING_OPAZANJAs.First(i => i.IDOpazanja == idOpazanja);

                    pr.IDStatusa = 5;
                    pr.Otisao = true;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Test");
                return false;
            }
        }

    }
}