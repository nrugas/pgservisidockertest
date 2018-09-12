using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PG.Servisi.GOService;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.cs.email;
using PG.Servisi.resources.cs.sigurnost;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Korisnici
    {
        private static int IDPrivilegije(int id)
        {
            if (id == 1)
            {
                return 1;
            }

            return id + 1;
        }

        public static bool PosaljiEmailDdobrodoslice(string grad, int idKorisnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    string lozinka = EncryptDecrypt.CreatePassword(4);

                    if (db.Djelatniks.Any(p => p.IDDjelatnika == idKorisnika))
                    {
                        Djelatnik novi = db.Djelatniks.First(p => p.IDDjelatnika == idKorisnika);
                        novi.Lozinka = EncryptDecrypt.EncodePassword(lozinka);
                        db.SubmitChanges();

                        return Posalji.Email(grad, Pripremi.PopulateBodyDobrodoslica(grad, novi.ImePrezime, novi.UID, lozinka), "Dobrodošli u sustav Pazigrad", new List<string> { novi.Email }, null, true, idAplikacije);
                    }

                    using (PostavkeDataContext pb = new PostavkeDataContext())
                    {
                        if (pb.KORISNICIs.Any(i => i.IDKorisnika == idKorisnika))
                        {
                            KORISNICI novi = pb.KORISNICIs.First(p => p.IDKorisnika == idKorisnika);
                            novi.Lozinka = EncryptDecrypt.EncodePassword(lozinka);
                            db.SubmitChanges();

                            return Posalji.Email(grad, Pripremi.PopulateBodyDobrodoslica(grad, novi.ImePrezime, novi.UID, lozinka), "Dobrodošli u sustav Pazigrad", new List<string> { novi.Email }, null, true, idAplikacije);
                        }
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "E-MAIL DOBRODOŠLICE");
                return false;
            }
        }

        /*:: PRIJAVA ::*/

        public static _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, int idRedarstva, out bool blokiranaJLS, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext pb = new PostavkeDataContext())
                {
                    if (grad != "")
                    {
                        blokiranaJLS = !pb.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad)).Aktivan;
                    }
                    else
                    {
                        blokiranaJLS = false;
                    }

                    var kor = from p in pb.KORISNICIs
                              where p.UID == korisnickoIme &&
                                    p.Lozinka.Replace("-", "") == zaporka.Replace("-", "")
                                   select new _Djelatnik(
                                  p.IDKorisnika,
                                  0,
                                  null, //todo centralizirati?
                                  p.ImePrezime,
                                  p.UID,
                                  "",
                                  p.Email,
                                  p.Mobitel,
                                  p.Telefon,
                                  p.Lozinka,
                                  p.IDPrivilegije,
                                  NazivPrivilegije(grad, p.IDPrivilegije, idAplikacije),
                                  "",
                                  "",
                                  false,
                                  false,
                                  false,
                                  p.Blokiran,
                                  p.Obrisan,
                                  false,
                                  false,
                                  false,
                                  false,
                                  false,
                                  "",
                                  null,
                                  Gradovi.GradoviKorisnika(grad, p.IDKorisnika, p.IDPrivilegije, idAplikacije),
                                  PrivilegijeDjelatnika(grad, p.IDKorisnika, p.IDPrivilegije, idAplikacije)
                                  );

                    if (kor.Any())
                    {
                        if (kor.First().Obrisan)
                        {
                            return null;
                        }

                        return kor.First();
                    }
                }

                if (grad == "")
                {
                    return null;
                }

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var djel = from p in db.Djelatniks
                               where p.UID == korisnickoIme &&
                                     p.Lozinka.Replace("-", "") == zaporka.Replace("-", "") &&
                                     (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1)
                               select new _Djelatnik(
                                   p.IDDjelatnika,
                                   p.IDRedarstva,
                                   p.IDPoslovnogSubjekta,
                                   p.ImePrezime,
                                   p.UID,
                                   p.BrojSI,
                                   p.Email,
                                   p.Mobitel,
                                   p.Telefon,
                                   p.Lozinka,
                                   IDPrivilegije(p.IDPrivilegija),
                                   NazivPrivilegije(grad, IDPrivilegije(p.IDPrivilegija), idAplikacije),
                                   p.OIB,
                                   p.ImeNaRacunu,
                                   p.PrometniRedar,
                                   p.Pauk,
                                   p.PrikaziStatistika,
                                   p.Blokiran,
                                   p.Obrisan,
                                   p.TraziOdobrenje ?? false,
                                   p.Mup,
                                   p.ObradjujeZahtjeve,
                                   p.Blagajna,
                                   p.Pretplate,
                                   p.GOGrad,
                                   p.IDGO,
                                   Gradovi.GradoviKorisnika(grad, p.IDDjelatnika, IDPrivilegije(p.IDPrivilegija), idAplikacije),
                                   PrivilegijeDjelatnika(grad, p.IDDjelatnika, IDPrivilegije(p.IDPrivilegija), idAplikacije)
                                   );

                    if (djel.Any())
                    {
                        if (djel.First().Obrisan)
                        {
                            return null;
                        }

                        if (idAplikacije == 11)
                        {
                            Sustav.SpremiAkciju(grad, djel.First().IDDjelatnika, 2, "Prijava u android aplikaciju za naplatu parkinga", 4, idAplikacije);
                        }

                        return djel.First();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Prijava");
                blokiranaJLS = false;
                return null;
            }
        }

        public static _Operater PrijavaMobile(string grad, string korisnickoIme, string zaporka, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var djel = from p in db.Djelatniks
                               where p.UID == korisnickoIme &&
                                     p.Lozinka.Replace("-", "") == zaporka.Replace("-", "")
                               select new _Operater(
                                   p.IDDjelatnika,
                                   p.IDRedarstva,
                                   IDPrijaviteljaGo(p),
                                   //p.IDGO ?? IDPrijaviteljaGo(p.IDRedarstva),
                                   p.ImePrezime,
                                   p.UID,
                                   p.BrojSI,
                                   p.IDPrivilegija,
                                   p.Parametri,
                                   p.Blokiran,
                                   MozeNaplacivati(p),//da bi moga vrsiti naplatu moraju biti upisani ovi detalji
                                   p.TraziOdobrenje ?? false,
                                   p.ObradjujeZahtjeve,
                                   p.GOGrad
                                   );

                    if (djel.Any())
                    {
                        //Sustav.SpremiAkciju(grad, djel.First().IDRedara, 2, "Prijava na terminalu", idAplikacije); // draško stalno proziva izgleda kao da se stalno prijavljuje
                        return djel.First();
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Prijava");
                return null;
            }
        }

        public static int IDPrijaviteljaGo(Djelatnik djelatnik)
        {
            if (djelatnik.IDGO.HasValue)
            {
                return djelatnik.IDGO.Value;
            }

            switch (djelatnik.IDRedarstva)
            {
                case 1:
                    return -1;
                case 2:
                    return -3;
                case 3:
                    return -2;
                default:
                    return -1;
            }
        }

        public static bool MozeNaplacivati(Djelatnik djelatnik)
        {
            return !string.IsNullOrEmpty(djelatnik.OIB) && !string.IsNullOrEmpty(djelatnik.ImeNaRacunu);
        }

        public static bool MozeObradjivati(string grad, int idKorisnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.Djelatniks.Any(i => i.IDDjelatnika == idKorisnika))
                    {
                        return db.Djelatniks.First(i => i.IDDjelatnika == idKorisnika).ObradjujeZahtjeve;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "MOZE OBRADJIVATI");
                return false;
            }
        }

        /*:: KORISNICI ::*/

        public static int DodajNovogDjelatnika(string grad, _Djelatnik korisnik, bool email, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext pb = new PostavkeDataContext())
                {
                    if (pb.KORISNICIs.Any(i => i.UID == korisnik.UID))
                    {
                        return -2;
                    }

                    if (korisnik.IDPrivilegije == 1 || korisnik.IDPrivilegije == 2)
                    {
                        KORISNICI kor = new KORISNICI();

                        kor.IDPrivilegije = korisnik.IDPrivilegije;
                        kor.ImePrezime = korisnik.ImePrezime;
                        kor.UID = korisnik.UID;
                        kor.Lozinka = EncryptDecrypt.EncodePassword(korisnik.Zaporka);
                        kor.Email = korisnik.Email;
                        kor.Mobitel = korisnik.Mobitel;
                        kor.Telefon = korisnik.Telefon;
                        kor.Blokiran = korisnik.Blokiran;
                        kor.Obrisan = false;

                        pb.KORISNICIs.InsertOnSubmit(kor);
                        pb.SubmitChanges();

                        return kor.IDKorisnika;
                    }

                    using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                    {

                        if (db.Djelatniks.Any(i => i.UID == korisnik.UID && i.IDRedarstva == korisnik.IDRedarstva))
                        {
                            return -2;
                        }

                        Djelatnik novi = new Djelatnik();

                        novi.ImePrezime = korisnik.ImePrezime;
                        novi.Lozinka = EncryptDecrypt.EncodePassword(korisnik.Zaporka);
                        novi.UID = korisnik.UID;
                        novi.IDPrivilegija = korisnik.IDPrivilegije == 1 ? 1 : korisnik.IDPrivilegije - 1; //todo obrati paznju
                        novi.IDPoslovnogSubjekta = korisnik.IDSubjekta;
                        novi.IDRedarstva = korisnik.IDRedarstva;
                        novi.Email = korisnik.Email;
                        novi.Mobitel = korisnik.Mobitel;
                        novi.Telefon = korisnik.Telefon;
                        novi.PrikaziStatistika = korisnik.Statistika;
                        novi.BrojSI = korisnik.BrojIskaznice;
                        novi.Otisak = null;
                        novi.Blokiran = korisnik.Blokiran;
                        novi.PrometniRedar = korisnik.Prometni;
                        novi.Pauk = korisnik.Pauk;
                        novi.OIB = korisnik.OIB;
                        novi.ImeNaRacunu = korisnik.ImeNaRacunu;
                        novi.Obrisan = false;
                        novi.TraziOdobrenje = korisnik.TraziOdobrenje;
                        novi.ObradjujeZahtjeve = korisnik.ObradaZahtjeva;
                        novi.Mup = korisnik.MUP;
                        novi.GOGrad = korisnik.GOGradID;
                        novi.IDGO = korisnik.IDGO;
                        novi.Pretplate = korisnik.Pretplate;

                        db.Djelatniks.InsertOnSubmit(novi);
                        db.SubmitChanges();

                        if (novi.Email != "" && email)
                        {
                            Posalji.Email(grad, Pripremi.PopulateBodyDobrodoslica(grad, novi.ImePrezime, novi.UID, korisnik.Zaporka), "Dobrodošli u sustav Pazigrad", new List<string> { novi.Email }, null, true, idAplikacije);
                        }

                        return novi.IDDjelatnika;
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dodaj Novog Korisnika");
                return -1;
            }
        }

        public static bool ObrisiDjelatnika(string grad, int idDjelatnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.Djelatniks.Any(i => i.IDDjelatnika == idDjelatnika))
                    {
                        Djelatnik d = db.Djelatniks.First(i => i.IDDjelatnika == idDjelatnika);
                        d.Obrisan = true;
                        d.UID = "_" + d.UID;

                        db.SubmitChanges();

                        return true;
                    }
                }

                using (PostavkeDataContext pb = new PostavkeDataContext())
                {
                    if (pb.KORISNICIs.Any(i => i.IDKorisnika == idDjelatnika))
                    {
                        KORISNICI k = pb.KORISNICIs.First(i => i.IDKorisnika == idDjelatnika);
                        k.Obrisan = true;
                        k.UID = "_" + k.UID;

                        pb.SubmitChanges();

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Obrisi Djelatnika");
                return false;
            }
        }

        public static bool IzmjeniDjelatnika(string grad, _Djelatnik korisnik, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.Djelatniks.Any(p => p.IDDjelatnika == korisnik.IDDjelatnika))
                    {
                        if (db.Djelatniks.Any(i => i.UID == korisnik.UID && i.IDDjelatnika != korisnik.IDDjelatnika && i.IDRedarstva == korisnik.IDRedarstva))
                        {
                            return false;
                        }

                        Djelatnik novi = db.Djelatniks.First(p => p.IDDjelatnika == korisnik.IDDjelatnika);

                        novi.ImePrezime = korisnik.ImePrezime;
                        novi.UID = korisnik.UID;
                        novi.IDPrivilegija = korisnik.IDPrivilegije == 1 ? 1 : korisnik.IDPrivilegije - 1; //todo obrati paznju
                        novi.IDPoslovnogSubjekta = korisnik.IDSubjekta;
                        novi.Email = korisnik.Email;
                        novi.Mobitel = korisnik.Mobitel;
                        novi.Telefon = korisnik.Telefon;
                        novi.PrikaziStatistika = korisnik.Statistika;
                        novi.BrojSI = korisnik.BrojIskaznice;
                        novi.Otisak = null;
                        novi.Blokiran = korisnik.Blokiran;
                        novi.PrometniRedar = korisnik.Prometni;
                        novi.Pauk = korisnik.Pauk;
                        novi.OIB = korisnik.OIB;
                        novi.ImeNaRacunu = korisnik.ImeNaRacunu;
                        novi.TraziOdobrenje = korisnik.TraziOdobrenje;
                        novi.ObradjujeZahtjeve = korisnik.ObradaZahtjeva;
                        novi.Mup = korisnik.MUP;
                        novi.GOGrad = korisnik.GOGradID;
                        novi.Pretplate = korisnik.Pretplate;

                        if (korisnik.IDRedarstva != null)
                        {
                            novi.IDRedarstva = korisnik.IDRedarstva;
                        }

                        db.SubmitChanges();

                        return true;
                    }
                }

                using (PostavkeDataContext pb = new PostavkeDataContext())
                {
                    if (pb.KORISNICIs.Any(i => i.IDKorisnika == korisnik.IDDjelatnika))
                    {
                        KORISNICI novi = pb.KORISNICIs.First(p => p.IDKorisnika == korisnik.IDDjelatnika);

                        novi.ImePrezime = korisnik.ImePrezime;
                        novi.UID = korisnik.UID;
                        novi.IDPrivilegije = korisnik.IDPrivilegije;
                        novi.Email = korisnik.Email;
                        novi.Mobitel = korisnik.Mobitel;
                        novi.Telefon = korisnik.Telefon;
                        novi.Blokiran = korisnik.Blokiran;

                        pb.SubmitChanges();

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Izmjeni Djelatnika");
                return false;
            }
        }

        public static List<_Djelatnik> DohvatiDjelatnike(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                List<_Djelatnik> svi = new List<_Djelatnik>();

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var djel = from p in db.Djelatniks
                               where p.Obrisan == false &&
                                     (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1)
                               select new _Djelatnik(
                                   p.IDDjelatnika,
                                   p.IDRedarstva,
                                   p.IDPoslovnogSubjekta,
                                   p.ImePrezime,
                                   p.UID,
                                   p.BrojSI,
                                   p.Email,
                                   p.Mobitel,
                                   p.Telefon,
                                   p.Lozinka,
                                   IDPrivilegije(p.IDPrivilegija),
                                   NazivPrivilegije(grad, IDPrivilegije(p.IDPrivilegija), idAplikacije),
                                   p.OIB,
                                   p.ImeNaRacunu,
                                   p.PrometniRedar,
                                   p.Pauk,
                                   p.PrikaziStatistika,
                                   p.Blokiran,
                                   p.Obrisan,
                                   p.TraziOdobrenje ?? false,
                                   p.Mup,
                                   p.ObradjujeZahtjeve,
                                   p.Blagajna,
                                   p.Pretplate,
                                   p.GOGrad,
                                   p.IDGO,
                                   Gradovi.GradoviKorisnika(grad, p.IDDjelatnika, IDPrivilegije(p.IDPrivilegija), idAplikacije),
                                   PrivilegijeDjelatnika(grad, p.IDDjelatnika, IDPrivilegije(p.IDPrivilegija), idAplikacije));

                    foreach (var djelatnik in djel)
                    {
                        svi.Add(djelatnik);
                    }
                }

                using (PostavkeDataContext pb = new PostavkeDataContext())
                {
                    var kor = from p in pb.KORISNICIs
                              where p.Obrisan == false
                              select new _Djelatnik(
                                   p.IDKorisnika,
                                   0,
                                   null, //todo
                                   p.ImePrezime,
                                   p.UID,
                                   "",
                                   p.Email,
                                   p.Mobitel,
                                   p.Telefon,
                                   p.Lozinka,
                                   p.IDPrivilegije,
                                   NazivPrivilegije(grad, p.IDPrivilegije, idAplikacije),
                                   "",
                                   "",
                                   false,
                                   false,
                                   false,
                                   p.Blokiran,
                                   p.Obrisan,
                                   false,
                                   false,
                                   false,
                                   false,
                                   false,
                                   "",
                                   null,
                                   Gradovi.GradoviKorisnika(grad, p.IDKorisnika, p.IDPrivilegije, idAplikacije),
                                   PrivilegijeDjelatnika(grad, p.IDKorisnika, p.IDPrivilegije, idAplikacije));

                    foreach (var djelatnik in kor)
                    {
                        svi.Add(djelatnik);
                    }
                }

                return svi;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI DJELATNIKE");
                return new List<_Djelatnik>();
            }
        }

        public static List<_Kontakt> DohvatiKontakte(string grad, int idAplikacije)
        {
            try
            {
                List<_Kontakt> svi = new List<_Kontakt>();

                using (PostavkeDataContext pdb = new PostavkeDataContext())
                {
                    foreach (var g in pdb.GRADOVIs.Where(i => i.Aktivan))
                    {
                        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(g.Baza, idAplikacije)))
                        {
                            var djel = from p in db.Djelatniks
                                       where p.Obrisan == false &&
                                             p.Blokiran == false &&
                                             p.Email != "" &&
                                             p.IDPrivilegija > 1 &&
                                             p.IDRedarstva != null
                                       orderby p.ImePrezime
                                       select new _Kontakt(
                                           p.IDDjelatnika,
                                           IDPrivilegije(p.IDPrivilegija),
                                           p.IDRedarstva.Value,
                                           g.IDGrada,
                                           p.ImePrezime,
                                           p.Email,
                                           g.AktivacijskiKod); //neki tekst koji zelimo ubaciti u meilove koje se šalju

                            svi.InsertRange(0, djel.ToList());
                        }
                    }

                    return svi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI SVE DJELATNIKE");
                return new List<_Kontakt>();
            }
        }

        public static List<_Kontakt> DohvatiSMS(string grad, int idAplikacije)
        {
            try
            {
                List<_Kontakt> svi = new List<_Kontakt>();

                using (PostavkeDataContext pdb = new PostavkeDataContext())
                {
                    foreach (var g in pdb.GRADOVIs.Where(i => i.Aktivan))
                    {
                        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(g.Baza, idAplikacije)))
                        {
                            var djel = from p in db.Djelatniks
                                       where p.Obrisan == false &&
                                             p.Blokiran == false &&
                                             p.Mobitel != "" &&
                                             p.Mobitel != null &&
                                             p.IDPrivilegija > 1 &&
                                             p.IDRedarstva != null
                                       orderby p.ImePrezime
                                       select new _Kontakt(
                                           p.IDDjelatnika,
                                           IDPrivilegije(p.IDPrivilegija),
                                           p.IDRedarstva.Value,
                                           g.IDGrada,
                                           p.ImePrezime,
                                           p.Mobitel.Replace("/","").Replace("+", "").Replace("-", ""),
                                           "");

                            svi.InsertRange(0, djel.ToList());
                        }
                    }

                    return svi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI SVE DJELATNIKE");
                return new List<_Kontakt>();
            }
        }

        public static List<_Chat> DohvatiDjelatnikeChat(string grad, int idDjelatnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var djel = from p in db.Djelatniks
                               where p.Obrisan == false &&
                                     p.Blokiran == false &&
                                     p.IDDjelatnika != idDjelatnika &&
                                     p.IDRedarstva != null
                               select new _Chat(
                                   p.IDDjelatnika,
                                   p.IDRedarstva.Value,
                                   p.ImePrezime,
                                   IDPrivilegije(p.IDPrivilegija),
                                   NazivPrivilegije(grad, IDPrivilegije(p.IDPrivilegija), idAplikacije),
                                   Sustav.Aktivan(grad, p.IDDjelatnika, idAplikacije),
                                   db.Porukes.Count(i => i.IDPosiljatelja == p.IDDjelatnika && i.IDPrimatelja == idDjelatnika && i.Procitano == false));

                    return djel.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI DJELATNIKE CHAT");
                return new List<_Chat>();
            }
        }

        public static _Djelatnik DohvatiDjelatnika(string grad, int idDjelatnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var djel = from p in db.Djelatniks
                               where p.IDDjelatnika == idDjelatnika
                               select new _Djelatnik(
                                   p.IDDjelatnika,
                                   p.IDRedarstva,
                                   p.IDPoslovnogSubjekta,
                                   p.ImePrezime,
                                   p.UID,
                                   p.BrojSI,
                                   p.Email,
                                   p.Mobitel,
                                   p.Telefon,
                                   p.Lozinka,
                                   IDPrivilegije(p.IDPrivilegija),
                                   NazivPrivilegije(grad, IDPrivilegije(p.IDPrivilegija), idAplikacije),
                                   p.OIB,
                                   p.ImeNaRacunu,
                                   p.PrometniRedar,
                                   p.Pauk,
                                   p.PrikaziStatistika,
                                   p.Blokiran,
                                   p.Obrisan,
                                   p.TraziOdobrenje ?? false,
                                   p.Mup,
                                   p.ObradjujeZahtjeve,
                                   p.Blagajna,
                                   p.Pretplate,
                                   p.GOGrad,
                                   p.IDGO,
                                   Gradovi.GradoviKorisnika(grad, p.IDDjelatnika, IDPrivilegije(p.IDPrivilegija), idAplikacije),
                                   PrivilegijeDjelatnika(grad, p.IDDjelatnika, IDPrivilegije(p.IDPrivilegija), idAplikacije));

                    if (djel.Any())
                    {
                        return djel.First();
                    }
                }

                using (PostavkeDataContext pb = new PostavkeDataContext())
                {
                    var kor = from p in pb.KORISNICIs
                              where p.IDKorisnika == idDjelatnika
                              select new _Djelatnik(
                                   p.IDKorisnika,
                                   0,
                                   null,
                                   p.ImePrezime,
                                   p.UID,
                                   "",
                                   p.Email,
                                   p.Mobitel,
                                   p.Telefon,
                                   p.Lozinka,
                                   p.IDPrivilegije,
                                   NazivPrivilegije(grad, p.IDPrivilegije, idAplikacije),
                                   "",
                                   "",
                                   false,
                                   false,
                                   false,
                                   p.Blokiran,
                                   p.Obrisan,
                                   false,
                                   false,
                                   false,
                                   false,
                                   false,
                                   "",
                                   null,
                                   Gradovi.GradoviKorisnika(grad, p.IDKorisnika, p.IDPrivilegije, idAplikacije),
                                   PrivilegijeDjelatnika(grad, p.IDKorisnika, p.IDPrivilegije, idAplikacije));

                    if (kor.Any())
                    {
                        return kor.First();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI DJELATNIKE");
                return null;
            }
        }

        /*:: PRIVILEGIJE ::*/

        public static List<_2DLista> DohvatiPrivilegije(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var priv = from p in db.PRIVILEGIJEs
                               select new _2DLista(p.IDPrivilegije, p.Naziv);

                    return priv.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati Privilegije");
                return new List<_2DLista>();
            }
        }

        private static string NazivPrivilegije(string grad, int idPrivilegije, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var priv = from p in db.PRIVILEGIJEs
                               where p.IDPrivilegije == idPrivilegije
                               select p.Naziv;

                    if (!priv.Any())
                    {
                        return "";
                    }

                    return priv.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati naziv Privilegije");
                return "";
            }
        }

        /*:: LOZINKA ::*/

        public static bool IzmjeniZaporku(string grad, int idDjelatnika, string lozinka, bool mobile, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.Djelatniks.Any(i => i.IDDjelatnika == idDjelatnika))
                    {
                        Djelatnik d = db.Djelatniks.First(i => i.IDDjelatnika == idDjelatnika);
                        d.Lozinka = mobile ? lozinka : EncryptDecrypt.EncodePassword(lozinka);
                        db.SubmitChanges();

                        if (d.Email != "" && !mobile)
                        {
                            Posalji.Email(grad, Pripremi.PopulateBodyLozinka(d.ImePrezime, d.UID, lozinka), "Izmjena lozinke - Pazigrad", new List<string> { d.Email }, null, true, idAplikacije);
                        }

                        return true;
                    }

                    using (PostavkeDataContext pb = new PostavkeDataContext())
                    {
                        if (pb.KORISNICIs.Any(i => i.IDKorisnika == idDjelatnika))
                        {
                            KORISNICI novi = pb.KORISNICIs.First(p => p.IDKorisnika == idDjelatnika);
                            novi.Lozinka = EncryptDecrypt.EncodePassword(lozinka);
                            pb.SubmitChanges();

                            return true;
                        }

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Izmjeni Zaporku");
                return false;
            }
        }

        public static bool IzmjeniOtisak(string grad, int idDjelatnika, byte[] otisak, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Djelatnik d = db.Djelatniks.First(i => i.IDDjelatnika == idDjelatnika);
                    d.Otisak = otisak;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Izmjeni Otisak");
                return false;
            }
        }

        /*:: RADNICI ::*/

        public static List<_3DLista> Redari(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var korisnik = from p in db.Djelatniks
                                   where p.PrometniRedar &&
                                         p.IDRedarstva == idRedarstva &&
                                         !p.Obrisan 
                                         orderby p.ImePrezime
                                   select new _3DLista(p.IDDjelatnika, p.ImePrezime, p.UID);

                    List<_3DLista> nova = new List<_3DLista>();

                    if (idRedarstva == 1)
                    {
                        nova.Add(new _3DLista(0, "Svi redari", ""));
                    }

                    if (idRedarstva == 4)
                    {
                        nova.Add(new _3DLista(0, "Svi naplatničari", ""));
                    }

                    foreach (var k in korisnik)
                    {
                        nova.Add(k);
                    }

                    return nova; 
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Redari");
                return new List<_3DLista>();
            }
        }

        public static List<_2DLista> Pauk(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var korisnik = from p in db.Djelatniks
                                   where p.Pauk &&
                                         !p.Obrisan
                                   orderby p.ImePrezime
                                   select new _2DLista(p.IDDjelatnika, p.ImePrezime);

                    return korisnik.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Redari");
                return new List<_2DLista>();
            }
        }

        /*:: PRIVILEGIJE ::*/

        public static List<_Privilegije> Privilegije(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    List<_Privilegije> privilegije = new List<_Privilegije>();

                    foreach (var p in db.PRIVILEGIJEs)
                    {
                        var ap = from d in db.DOZVOLEs
                                 join g in db.GRUPE_PRIVILEGIJAs on d.IDDozvole equals g.IDDozvole
                                 where g.IDPrivilegije == p.IDPrivilegije &&
                                       d.IDAplikacije == idAplikacije
                                 select new _Dozvola(d.IDDozvole, d.IDAplikacije, "", d.NazivAkcije, d.OpisAkcije, true, true, false); //zabrana je false ? da li je to ok

                        List<_Dozvola> akcije = new List<_Dozvola>();

                        foreach (var a in ap)
                        {
                            string aplikacija = db.APLIKACIJEs.First(i => i.IDAplikacije == a.IDAplikacije).NazivAplikacije;

                            akcije.Add(new _Dozvola(a.IDAkcije, a.IDAplikacije, aplikacija, a.Opis, a.Naziv, a.GrupaPrivilegija, a.Odabran, a.Dozvoljeno));
                        }

                        privilegije.Add(new _Privilegije(p.IDPrivilegije, p.Naziv, p.Opis, akcije));
                    }

                    return privilegije;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Privilegije");
                return null;
            }
        }

        public static List<_Dozvola> DetaljiPrivilegije(string grad, int idKorisnika, int idPrivilegije, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    List<_Dozvola> akcije = new List<_Dozvola>();

                    var ap = from d in db.DOZVOLEs
                             select new _Dozvola(d.IDDozvole, d.IDAplikacije, "", d.NazivAkcije, d.OpisAkcije, true, true, false);

                    foreach (var a in ap)
                    {
                        var gp = from g in db.GRUPE_PRIVILEGIJAs
                                 where g.IDPrivilegije == idPrivilegije
                                 select g.IDDozvole;

                        string aplikacija = db.APLIKACIJEs.First(i => i.IDAplikacije == a.IDAplikacije).NazivAplikacije;
                        bool grupa = gp.Contains(a.IDAkcije);
                        bool odabran = grupa ? true : db.DJELATNICI_DOZVOLEs.Any(i => i.IDDjelatnika == idKorisnika && i.IDDozvole == a.IDAkcije && i.IDGrada == Sistem.IDGrada(grad));//false - ako nije grupa onda treba provijeiti ako je dodijljen korisniku extra
                        bool dozvoljeno = true;//false - ako nije grupa onda treba provijeiti ako je dodijljen korisniku extra

                        if (db.DJELATNICI_DOZVOLEs.Any(i => i.IDDjelatnika == idKorisnika && i.IDDozvole == a.IDAkcije && i.IDGrada == Sistem.IDGrada(grad)))
                        {
                            dozvoljeno = db.DJELATNICI_DOZVOLEs.First(i => i.IDDjelatnika == idKorisnika && i.IDDozvole == a.IDAkcije && i.IDGrada == Sistem.IDGrada(grad)).Dozvoljeno;
                        }

                        akcije.Add(new _Dozvola(a.IDAkcije, a.IDAplikacije, aplikacija, a.Opis, a.Naziv, grupa, odabran, dozvoljeno));
                    }

                    return akcije;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "detalji privilegija korisnika");
                return null;
            }
        }

        public static bool IzmjeniDetaljePrivilegija(string grad, int idDjelatnika, List<int> dodijeljene, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    db.DJELATNICI_DOZVOLEs.DeleteAllOnSubmit(db.DJELATNICI_DOZVOLEs.Where(i => i.IDDjelatnika == idDjelatnika && i.IDGrada == Sistem.IDGrada(grad)));
                    db.SubmitChanges();

                    foreach (var i in dodijeljene)
                    {
                        DJELATNICI_DOZVOLE ka = new DJELATNICI_DOZVOLE();
                        ka.IDDjelatnika = idDjelatnika;
                        ka.IDDozvole = i;
                        ka.IDGrada = Sistem.IDGrada(grad);
                        ka.Dozvoljeno = true;

                        db.DJELATNICI_DOZVOLEs.InsertOnSubmit(ka);
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI detalje privilegija korisnika");
                return false;
            }
        }

        public static bool ResetirajPrivilegije(string grad, int idDjelatnika, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    db.DJELATNICI_DOZVOLEs.DeleteAllOnSubmit(db.DJELATNICI_DOZVOLEs.Where(i => i.IDDjelatnika == idDjelatnika));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "resetriaj privilegija korisnika");
                return false;
            }
        }

        private static List<int> PrivilegijeDjelatnika(string grad, int idDjelatnika, int idPrivilegije, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    List<int> privilegije = new List<int>();

                    if (grad == "")
                    {
                        return privilegije;
                    }

                    foreach (var q in db.DJELATNICI_DOZVOLEs.Where(i => i.IDDjelatnika == idDjelatnika && i.Dozvoljeno && i.IDGrada == Sistem.IDGrada(grad)))
                    {
                        privilegije.Add(q.IDDozvole);
                    }

                    foreach (var q in db.GRUPE_PRIVILEGIJAs.Where(i => i.IDPrivilegije == idPrivilegije))
                    {
                        privilegije.Add(q.IDDozvole);
                    }

                    foreach (var q in db.DJELATNICI_DOZVOLEs.Where(i => i.IDDjelatnika == idDjelatnika && !i.Dozvoljeno && i.IDGrada == Sistem.IDGrada(grad)))
                    {
                        privilegije.Remove(q.IDDozvole);
                    }

                    return privilegije;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIVILEGIJE KORISNIKA");
                return new List<int>();
            }
        }

        public static bool Zabrani(string grad, int idDjelatnika, int idDozvole, bool dozvoljeno, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (dozvoljeno)
                    {
                        db.DJELATNICI_DOZVOLEs.DeleteOnSubmit(db.DJELATNICI_DOZVOLEs.First(i => i.IDDjelatnika == idDjelatnika && i.IDDozvole == idDozvole && i.IDGrada == Sistem.IDGrada(grad)));
                        db.SubmitChanges();
                    }
                    else
                    {
                        DJELATNICI_DOZVOLE ka = new DJELATNICI_DOZVOLE();
                        ka.IDDjelatnika = idDjelatnika;
                        ka.IDDozvole = idDozvole;
                        ka.IDGrada = Sistem.IDGrada(grad);
                        ka.Dozvoljeno = false;

                        db.DJELATNICI_DOZVOLEs.InsertOnSubmit(ka);
                        db.SubmitChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ZABRANI / DOZVOLI");
                return false;
            }
        }

        /*:: POSLOVNI SUBJEKTI ::*/

        public static List<_2DLista> DohvatiPopisSubjekta(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var subjekti = from p in db.POSLOVNI_SUBJEKTIs
                        orderby p.NazivSubjekta
                        select new _2DLista(p.IDPoslovnogSubjekta, p.NazivSubjekta);

                    return subjekti.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Redari");
                return new List<_2DLista>();
            }
        }

        public static List<_PoslovniSubjekt> DohvatiPoslovneSubjekte(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var subjekti = from p in db.POSLOVNI_SUBJEKTIs
                        orderby p.NazivSubjekta
                        select new _PoslovniSubjekt(p.IDPoslovnogSubjekta, p.NazivSubjekta, p.OdgovornaOsoba, p.Mobitel, p.Email, p.OIB, p.Adresa);

                    return subjekti.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Redari");
                return new List<_PoslovniSubjekt>();
            }
        }

        public static int DodajPoslovnogSubjekta(string grad, _PoslovniSubjekt subjekt, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    POSLOVNI_SUBJEKTI ps = new POSLOVNI_SUBJEKTI();

                    int id = 1;

                    if (db.POSLOVNI_SUBJEKTIs.Any())
                    {
                        id = db.POSLOVNI_SUBJEKTIs.Max(i => i.IDPoslovnogSubjekta) + 1;
                    }

                    ps.IDPoslovnogSubjekta = id;
                    ps.NazivSubjekta = subjekt.Naziv;
                    ps.OdgovornaOsoba = subjekt.Osoba;
                    ps.Mobitel = subjekt.Mobitel;
                    ps.Email = subjekt.Email;
                    ps.OIB = subjekt.OIB;
                    ps.Adresa = subjekt.Adresa;

                    db.POSLOVNI_SUBJEKTIs.InsertOnSubmit(ps);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ POSLOVNI SUBJEKT");
                return -1;
            }
        }

        public static bool IzmjeniPoslovnogSubjekta(string grad, _PoslovniSubjekt subjekt, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    POSLOVNI_SUBJEKTI ps = db.POSLOVNI_SUBJEKTIs.First(i => i.IDPoslovnogSubjekta == subjekt.IDSubjekta);

                    ps.NazivSubjekta = subjekt.Naziv;
                    ps.OdgovornaOsoba = subjekt.Osoba;
                    ps.Mobitel = subjekt.Mobitel;
                    ps.Email = subjekt.Email;
                    ps.OIB = subjekt.OIB;
                    ps.Adresa = subjekt.Adresa;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI POSLOVNI SUBJEKT");
                return false;
            }
        }

        public static bool ObrisPoslovnogSubjekta(string grad, int idSubjekta, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {

                    if (db.Djelatniks.Any(i => i.IDPoslovnogSubjekta == idSubjekta))
                    {
                        return false;
                    }

                    if (db.ODOBRENJAs.Any(i => i.IDSubjekta == idSubjekta))
                    {
                        return false;
                    }

                    db.POSLOVNI_SUBJEKTIs.DeleteOnSubmit(db.POSLOVNI_SUBJEKTIs.First(i => i.IDPoslovnogSubjekta == idSubjekta));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OBRISI POSLOVNI SUBJEKT");
                return false;
            }
        }

        /*:: DODAJ GRADOVE KORISNIKU ::*/

        public static bool IzmjeniGradoveDjelatnika(string grad, int idKorisnika, List<int> gradovi, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (idKorisnika >= 1000)
                    {
                        db.KORISNICI_GRADOVIs.DeleteAllOnSubmit(db.KORISNICI_GRADOVIs.Where(i => i.IDKorisnika == idKorisnika && i.IDOriginalnog == -1));
                        db.SubmitChanges();
                    }
                    else
                    {
                        db.KORISNICI_GRADOVIs.DeleteAllOnSubmit(db.KORISNICI_GRADOVIs.Where(i => i.IDKorisnika == idKorisnika && i.IDOriginalnog == Sistem.IDGrada(grad)));
                        db.SubmitChanges();
                    }

                    foreach (var g in gradovi)
                    {
                        KORISNICI_GRADOVI kg = new KORISNICI_GRADOVI();

                        kg.IDKorisnika = idKorisnika;
                        kg.IDGrada = g;

                        if (idKorisnika < 1000)
                        {
                            kg.IDOriginalnog = Sistem.IDGrada(grad);
                        }
                        else
                        {
                            kg.IDOriginalnog = -1;
                        }

                        db.KORISNICI_GRADOVIs.InsertOnSubmit(kg);
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI GRADOVE KORISNIKA");
                return false;
            }
        }

        //public static void Kontakti()
        //{
        //    try
        //    {
        //        using (PostavkeDataContext pb = new PostavkeDataContext())
        //        {
        //            StringBuilder sb = new StringBuilder();

        //            foreach (var g in pb.GRADOVIs)
        //            {
        //                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(g.Baza, 1)))
        //                {
        //                    var x = from d in db.Djelatniks
        //                            join p in db.Privilegijes on d.IDPrivilegija equals p.IDPrivilegije
        //                            where d.Obrisan == false && d.Blokiran == false && d.IDPrivilegija != 1
        //                            select new { d, p };

        //                    foreach (var z in x)
        //                    {
        //                        sb.AppendLine(z.d.ImePrezime + ";" + z.d.Email + ";" + g.NazivGrada + ";" + z.p.NazivPrivilegije);
        //                    }
        //                }
        //            }

        //            StreamWriter file = new StreamWriter("C:\\Users\\dpajalic\\Desktop\\kontkti.csv");
        //            file.WriteLine(sb.ToString());
        //            file.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku("", ex, 1, "KONTAKTI");
        //    }
        //}

        /**/

        public static List<_Djelatnik> DohvatiDjelatnikeNET(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List<_Djelatnik> svi = new List<_Djelatnik>();

                    var djel = from p in db.Djelatniks
                               where p.Obrisan == false
                               select new _Djelatnik(
                                   p.IDDjelatnika,
                                   p.IDRedarstva,
                                   p.IDPoslovnogSubjekta,
                                   p.ImePrezime,
                                   p.UID,
                                   p.BrojSI,
                                   p.Email,
                                   p.Mobitel,
                                   p.Telefon,
                                   p.Lozinka,
                                   IDPrivilegije(p.IDPrivilegija),
                                   NazivPrivilegije(grad, IDPrivilegije(p.IDPrivilegija), idAplikacije),
                                   p.OIB,
                                   p.ImeNaRacunu,
                                   p.PrometniRedar,
                                   p.Pauk,
                                   p.PrikaziStatistika,
                                   p.Blokiran,
                                   p.Obrisan,
                                   p.TraziOdobrenje ?? false,
                                   p.Mup,
                                   p.ObradjujeZahtjeve,
                                   p.Blagajna,
                                   p.Pretplate,
                                   p.GOGrad,
                                   p.IDGO,
                                   Gradovi.GradoviKorisnika(grad, p.IDDjelatnika, IDPrivilegije(p.IDPrivilegija), idAplikacije),
                                   PrivilegijeDjelatnika(grad, p.IDDjelatnika, IDPrivilegije(p.IDPrivilegija), idAplikacije));

                    foreach (var djelatnik in djel)
                    {
                        svi.Add(djelatnik);
                    }

                    return svi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI DJELATNIKE");
                return new List<_Djelatnik>();
            }
        }

        /*:: GO ::*/

        public static int PoveziSaGO(string grad, int idKorisnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    GRADOVI g;

                    using (PostavkeDataContext pdb = new PostavkeDataContext())
                    {
                        g = pdb.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad));
                    }

                    _Djelatnik d = DohvatiDjelatnika(grad, idKorisnika, idAplikacije);

                    string baza = string.IsNullOrEmpty(d.GOGradID) ? g.GO : d.GOGradID;

                    if (string.IsNullOrEmpty(baza))
                    {
                        return -1;
                    }

                    using (GOPazigradClient sc = new GOPazigradClient())
                    {
                        int idGO = sc.DodajKorisnika(baza,
                            new _Korisnik()
                            {
                                Ime = d.ImePrezime,
                                Prezime = "",
                                DatumRodenja = null,
                                Email = d.Email,
                                Mobitel = d.Mobitel,
                                Lozinka = DateTime.Now.ToString("hhmmss"),
                                IDPrivilegije = 4,
                                SMS = true,
                                IDPazigrad = idKorisnika
                            },
                            new ObservableCollection<int>());

                        Djelatnik dj = db.Djelatniks.First(i => i.IDDjelatnika == idKorisnika);
                        dj.IDGO = idGO;
                        db.SubmitChanges();

                        return idGO;
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI DJELATNIKE");
                return -1;
            }
        }
    }
}