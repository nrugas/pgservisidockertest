using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.cs.email;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Gradovi
    {
        public static List<_Grad> GradoviKorisnika(string grad, int idKorisnika, int idPrivilegije, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (idPrivilegije == 1)
                    {
                        var svi = from p in db.GRADOVIs
                                  join c in db.CERTIFIKATI_MUPs on p.IDGrada equals c.IDGrada into mup
                                  from cc in mup.DefaultIfEmpty()
                                  orderby p.NazivGrada
                                  select new _Grad(p.IDGrada, p.Baza, p.NazivGrada, (double)p.Latitude, (double)p.Longitude, p.IznosNaloga, p.Zoom, p.Pauk, 
                                  p.Aktivan, p.Vpp, p.Odvjetnici, p.AktivacijskiKod, p.Adresa, p.Grb, p.NaplataPauk != null, p.NaplataPauk, p.DOF, p.PazigradNaIzvjestaju, 
                                  p.ZalbaRedarstva, p.Zadrska, p.Chat, cc.SmijePitati, p.Mapa, p.GO, p.IDGrupePromet, p.Tip, p.Lisice, p.OdlukaLisice, p.VanjskePrijave, 
                                  p.PrilogObavijest, p.RacunHUB, p.NakonDanaLezarina, p.Parking);

                        return svi.ToList();
                    }

                    var gradovi = from p in db.GRADOVIs
                                  join k in db.KORISNICI_GRADOVIs on p.IDGrada equals k.IDGrada
                                  join c in db.CERTIFIKATI_MUPs on p.IDGrada equals c.IDGrada into mup
                                  from cc in mup.DefaultIfEmpty()
                                  where p.Aktivan &&
                                        k.IDKorisnika == idKorisnika &&
                                        (idPrivilegije > 2 ? k.IDOriginalnog == Sistem.IDGrada(grad) : k.IDOriginalnog == -1)
                                  orderby p.NazivGrada
                                  select new _Grad(p.IDGrada, p.Baza, p.NazivGrada, (double)p.Latitude, (double)p.Longitude, p.IznosNaloga, p.Zoom, p.Pauk, 
                                  p.Aktivan, p.Vpp, p.Odvjetnici, p.AktivacijskiKod, p.Adresa, p.Grb, p.NaplataPauk != null, p.NaplataPauk, p.DOF, p.PazigradNaIzvjestaju, 
                                  p.ZalbaRedarstva, p.Zadrska, p.Chat, cc.SmijePitati, p.Mapa, p.GO, p.IDGrupePromet, p.Tip, p.Lisice, p.OdlukaLisice, p.VanjskePrijave,
                                  p.PrilogObavijest, p.RacunHUB, p.NakonDanaLezarina, p.Parking);

                    return gradovi.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Gradovi Korisnika");
                return new List<_Grad>();
            }
        }

        public static List<_Grad> GradoviOdvjetnika(bool obradjeni, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    List<_Grad> grad = new List<_Grad>();

                    foreach (var g in db.GRADOVIs.Where(i => i.Aktivan && i.Odvjetnici).OrderBy(i => i.NazivGrada))
                    {
                        using (PazigradDataContext pg = new PazigradDataContext(Sistem.ConnectionString(g.Baza, idAplikacije)))
                        {
                            int br;
                            if (obradjeni)
                            {

                                var obr = from vpp in pg.VppVanjskoPostupcis
                                    join s in pg.VppStatus on vpp.IDStatusaVP equals s.IDStatusaVP into sta
                                    from ss in sta.DefaultIfEmpty()
                                              //join p in pg.Prekrsajis on vpp.IDPrekrsaja equals p.IDPrekrsaja
                                          where vpp.RegistracijaOK &&
                                                vpp.dozvola == "DA" &&
                                                vpp.IDStatusaVP != 1 &&
                                                vpp.IDStatusaVP != null &&
                                                ss.Zatvara == false &&
                                                vpp.Preuzeto == false &&
                                                ss.Zatvara == false
                                          select vpp;

                                //foreach (var v in obr)
                                //{
                                //    if (!new PostavkeDataContext().DRZAVAs.First(i => i.Kratica == v.KraticaDrzave).NaplataVP)
                                //    {
                                //        continue;
                                //    }

                                //    br++;
                                //}

                                br = obr.Count();
                            }
                            else
                            {
                                br = pg.VppVanjskoPostupcis.Count(i => i.RegistracijaOK == false && i.dozvola == "DA" && i.IDStatusaVP != null && i.Preuzeto == false);
                            }

                            grad.Add(new _Grad(g.IDGrada, g.Baza, g.NazivGrada, (double)g.Latitude, (double)g.Longitude, g.IznosNaloga, br, g.Pauk, g.Aktivan, g.Vpp, g.Odvjetnici,
                                g.AktivacijskiKod, g.Adresa, g.Grb, g.NaplataPauk != null, g.NaplataPauk, g.DOF, g.PazigradNaIzvjestaju, g.ZalbaRedarstva, g.Zadrska, g.Chat, false, 
                                g.Mapa, g.GO, g.IDGrupePromet, g.Tip, g.Lisice, g.OdlukaLisice, g.VanjskePrijave, g.PrilogObavijest, g.RacunHUB, g.NakonDanaLezarina, g.Parking));
                        }
                    }

                    return grad;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Gradovi odvjetnika");
                return new List<_Grad>();
            }
        }

        public static List<_2DLista> PopisGradova(int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var gradovi = from p in db.GRADOVIs
                                  where p.Aktivan &&
                                  (idRedarstva == 4 ? p.Parking : idRedarstva != 4)
                                  orderby p.NazivGrada
                                  select new _2DLista(p.IDGrada, p.NazivGrada);

                    return gradovi.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Gradovi");
                return new List<_2DLista>();
            }
        }

        public static List<_JLS> GradoviWeb(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var gradovi = from p in db.GRADOVIs
                                  where p.Aktivan
                                  orderby p.NazivGrada
                                  select new _JLS(p.Baza, p.NazivGrada);

                    return gradovi.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Gradovi");
                return new List<_JLS>();
            }
        }

        /*:: POSTAVKE JLS ::*/

        public static bool PostavkeGrada(string grad, _Grad postavke, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    GRADOVI p = db.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad));

                    bool odv = p.Odvjetnici;
                    //decimal iznos = p.IznosNaloga;

                    p.NazivGrada = postavke.Naziv;
                    p.Latitude = (decimal)postavke.Latitude;
                    p.Longitude = (decimal)postavke.Longitude;
                    p.IznosNaloga = postavke.IznosNaloga;
                    p.Zoom = postavke.Zoom;
                    p.Pauk = postavke.Pauk;
                    p.Aktivan = postavke.Aktivan;
                    p.Vpp = postavke.Vpp;
                    p.Odvjetnici = postavke.Odvjetnici;
                    p.Adresa = postavke.Adresa;
                    p.Grb = postavke.Grb;
                    p.DOF = postavke.DOF;
                    p.Chat = postavke.Chat;
                    p.Mapa = postavke.Mapa;
                    p.GO = postavke.GO;
                    p.IDGrupePromet = postavke.IDGrupePredmet;
                    p.Lisice = postavke.Lisice;

                    db.SubmitChanges();

                    if (odv != postavke.Odvjetnici)
                    {
                        string poruka = p.NazivGrada + " - isključeno je vanjsko postupanje!";

                        if (postavke.Odvjetnici)
                        {
                            poruka = p.NazivGrada + " - uključeno je vanjsko postupanje!";
                        }

                        Posalji.Email(grad, poruka, p.NazivGrada + "Promjena postavki", new List<string> { "marko.andric@ri-ing.net" }, null, false, idAplikacije);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "POSTAVKE GRADA");
                return false;
            }
        }

        public static bool IzmjeniMapu(string grad, string mapa, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    GRADOVI p = db.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad));
                    p.Mapa = mapa;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "POSTAVKE GRADA - MAPA");
                return false;
            }
        }

        public static _Grad Grad(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var gradovi = from p in db.GRADOVIs
                                  join c in db.CERTIFIKATI_MUPs on p.IDGrada equals c.IDGrada into mup
                                  from cc in mup.DefaultIfEmpty()
                                  where p.IDGrada == Sistem.IDGrada(grad)
                                  select new _Grad(p.IDGrada, p.Baza, p.NazivGrada, (double)p.Latitude, (double)p.Longitude, p.IznosNaloga, p.Zoom, p.Pauk, 
                                  p.Aktivan, p.Vpp, p.Odvjetnici, p.AktivacijskiKod, p.Adresa, p.Grb, p.NaplataPauk != null, p.NaplataPauk, p.DOF, p.PazigradNaIzvjestaju, 
                                  p.ZalbaRedarstva, p.Zadrska, p.Chat, cc.SmijePitati, p.Mapa, p.GO, p.IDGrupePromet, p.Tip, p.Lisice, p.OdlukaLisice, p.VanjskePrijave, 
                                  p.PrilogObavijest, p.RacunHUB, p.NakonDanaLezarina, p.Parking);

                    return gradovi.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Grad");
                return null;
            }
        }

        /*:: NALOG ZA PLACANJE ::*/

        public static _Uplatnica Uplatnica(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var nalog = from n in db.UPLATNICEs
                                where n.IDGrada == Sistem.IDGrada(grad) &&
                                      n.IDRedarstva == idRedarstva
                                select new _Uplatnica(n.IDUplatnice, n.IDGrada, n.IDRedarstva, n.Adresa, n.Model, n.IBAN, n.Poziv1, n.Poziv2,
                                n.Opis, n.SWIFT, n.Sifra, n.Naziv, n.UlicaBroj, n.Posta, n.Mjesto);

                    if (!nalog.Any())
                    {
                        return null;
                    }

                    return nalog.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "UPLATNICA");
                return null;
            }
        }

        public static _Uplatnica Uplatnica(string grad, int idGrada, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var nalog = from n in db.UPLATNICEs
                                where n.IDGrada == idGrada &&
                                      n.IDRedarstva == idRedarstva
                                select new _Uplatnica(n.IDUplatnice, n.IDGrada, n.IDRedarstva, n.Adresa, n.Model, n.IBAN, n.Poziv1, n.Poziv2,
                                n.Opis, n.SWIFT, n.Sifra, n.Naziv, n.UlicaBroj, n.Posta, n.Mjesto);

                    if (!nalog.Any())
                    {
                        return null;
                    }

                    return nalog.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "UPLATNICA");
                return null;
            }
        }

        public static int IzmjeniUplatnicu(string grad, _Uplatnica uplatnica, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (!db.UPLATNICEs.Any(i => i.IDGrada == Sistem.IDGrada(grad) && i.IDRedarstva == uplatnica.IDRedarstva))
                    {
                        UPLATNICE novi = new UPLATNICE();

                        novi.IDUplatnice = db.UPLATNICEs.Max(i => i.IDUplatnice) + 1;
                        novi.IDGrada = uplatnica.IDGrada < 1 ? Sistem.IDGrada(grad) : uplatnica.IDGrada;
                        novi.IDRedarstva = uplatnica.IDRedarstva;
                        novi.Adresa = string.Format("{0}; {1}; {2} {3}", uplatnica.Naziv, uplatnica.UlicaBroj, uplatnica.Posta, uplatnica.Mjesto);
                        novi.Model = uplatnica.Model ?? "";
                        novi.IBAN = uplatnica.IBAN;
                        novi.Poziv1 = uplatnica.Poziv1 ?? "";
                        novi.Poziv2 = uplatnica.Poziv2 ?? "";
                        novi.Opis = uplatnica.Opis;
                        novi.SWIFT = uplatnica.Swift ?? "";
                        novi.Sifra = uplatnica.Sifra ?? "";
                        novi.Naziv = uplatnica.Naziv;
                        novi.UlicaBroj = uplatnica.UlicaBroj;
                        novi.Posta = uplatnica.Posta;
                        novi.Mjesto = uplatnica.Mjesto;

                        db.UPLATNICEs.InsertOnSubmit(novi);
                        db.SubmitChanges();

                        if (uplatnica.IDRedarstva == 1)
                        {
                            PostaviZaVpp(grad, uplatnica, idAplikacije);
                        }

                        return novi.IDUplatnice;
                    }

                    UPLATNICE n;

                    if (uplatnica.IDUplatnice != 0)
                    {
                       n = db.UPLATNICEs.First(i => i.IDUplatnice == uplatnica.IDUplatnice);
                    }
                    else
                    {
                        n = db.UPLATNICEs.First(i => i.IDGrada == Sistem.IDGrada(grad) && i.IDRedarstva == uplatnica.IDRedarstva);
                    }

                    //n.IDGrada = uplatnica.IDGrada < 0 ? Sistem.IDGrada(grad) : uplatnica.IDGrada;
                    //n.IDRedarstva = uplatnica.IDRedarstva;
                    n.Adresa = uplatnica.Adresa ?? "";
                    n.Model = uplatnica.Model ?? "";
                    n.IBAN = uplatnica.IBAN ?? "";
                    n.Poziv1 = uplatnica.Poziv1 ?? "";
                    n.Poziv2 = uplatnica.Poziv2 ?? "";
                    n.Opis = uplatnica.Opis ?? "";
                    n.SWIFT = uplatnica.Swift ?? "";
                    n.Sifra = uplatnica.Sifra ?? "";
                    n.Naziv = uplatnica.Naziv ?? "";
                    n.UlicaBroj = uplatnica.UlicaBroj ?? "";
                    n.Posta = uplatnica.Posta ?? "";
                    n.Mjesto = uplatnica.Mjesto ?? "";

                    db.SubmitChanges();

                    if (uplatnica.IDRedarstva == 1)
                    {
                        PostaviZaVpp(grad, uplatnica, idAplikacije);
                    }

                    return n.IDUplatnice;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NALOG ZA PLAĆANJE");
                return -1;
            }
        }

        private static void PostaviZaVpp(string grad, _Uplatnica u, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.NALOZI_Predlozaks.Any())
                    {
                        NALOZI_Predlozak np = db.NALOZI_Predlozaks.First();

                        np.Grad = u.Mjesto;
                        np.Adresa = u.Adresa;
                        np.Model = u.Model;
                        np.BrojRacuna = "";
                        np.PozivNaBroj1 = u.Poziv1;
                        np.PozivNaBroj2 = u.Poziv2;
                        np.OpisPlacanja = u.Opis;
                        np.Sifra = u.Sifra;
                        np.IBAN = u.IBAN;

                        db.NALOZI_Predlozaks.InsertOnSubmit(np);
                        db.SubmitChanges();
                    }
                    else
                    {
                        NALOZI_Predlozak np = new NALOZI_Predlozak();

                        np.IDPredloska = 1;
                        np.Grad = u.Mjesto;
                        np.Adresa = u.Adresa;
                        np.Model = u.Model;
                        np.BrojRacuna = "";
                        np.PozivNaBroj1 = u.Poziv1;
                        np.PozivNaBroj2 = u.Poziv2;
                        np.OpisPlacanja = u.Opis;
                        np.Sifra = u.Sifra;
                        np.IBAN = u.IBAN;

                        db.NALOZI_Predlozaks.InsertOnSubmit(np);
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NALOZI_Predlozak");
            }
        }

        /*:: NALOG ZA PLACANJE ::*/

        public static decimal IznosNaUplatnici(int idGrada, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.GRADOVIs.First(i => i.IDGrada == idGrada).IznosNaloga;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Iznosa na uplatnici");
                return -1;
            }
        }

        public static List<_Iznos> Iznosi(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.UPLATNICE_IZNOSIs.OrderBy(i => i.Iznos).Select(i => new _Iznos(i.IDIznosa, i.IDRedarstva, i.Iznos)).ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Iznosi");
                return new List<_Iznos>();
            }
        }

        public static List<_Iznos> IznosPokusaja(int idGrada, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext pdb = new PostavkeDataContext())
                {
                    string grad = pdb.GRADOVIs.First(i => i.IDGrada == idGrada).Baza;

                    using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                    {
                        var izn = from i in db.RACUNI_STAVKE_OPIs
                                  where i.IDStatusa == 3
                                  orderby i.Iznos
                                  select i;//new _Iznos(i.IDIznosa, 2, (decimal) i.Iznos);

                        if (!izn.Any())
                        {
                            return null;
                        }

                        List<_Iznos> pokusaj = new List<_Iznos>();

                        foreach (var q in izn)
                        {
                            int id;
                            if (pdb.UPLATNICE_IZNOSIs.Any(i => i.Iznos == q.Iznos.Value && i.IDRedarstva == 2))
                            {
                                id = pdb.UPLATNICE_IZNOSIs.First(i => i.Iznos == q.Iznos.Value && i.IDRedarstva == 2).IDIznosa;
                            }
                            else
                            {
                                UPLATNICE_IZNOSI ui = new UPLATNICE_IZNOSI();

                                ui.IDIznosa = pdb.UPLATNICE_IZNOSIs.Max(i => i.IDIznosa) + 1;
                                ui.IDIznosa = 2;
                                ui.Iznos = q.Iznos.Value;

                                pdb.UPLATNICE_IZNOSIs.InsertOnSubmit(ui);
                                db.SubmitChanges();

                                id = ui.IDIznosa;
                            }

                            pokusaj.Add(new _Iznos(id, 2, q.Iznos.Value));
                        }

                        return pokusaj;
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Iznosi");
                return new List<_Iznos>();
            }
        }

        /*:: POVIJEST ::*/

        public static int ZadnjiNalog(int idGrada, int? idIznosa, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (db.UPLATNICE_POVIJESTs.Any(i => i.IDGrada == idGrada && i.IDRedarstva == idRedarstva && i.IDIznosa == idIznosa && i.Nalog != null))
                    {
                        //nova godina - reset na nulu
                        if (db.UPLATNICE_POVIJESTs.Where(i => i.IDGrada == idGrada && i.IDRedarstva == idRedarstva && i.IDIznosa == idIznosa).OrderByDescending(i => i.Datum).First().Datum.Year != DateTime.Now.Year)
                        {
                            return 0;
                        }

                        //zadnji broj naloga
                        return (int)(db.UPLATNICE_POVIJESTs.Where(i => i.IDGrada == idGrada && i.IDRedarstva == idRedarstva && i.IDIznosa == idIznosa && i.Nalog != null && i.Datum.Year == DateTime.Now.Year)
                                .Max(i => i.Nalog) + 1);
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "ZADNJI NALOG");
                return -1;
            }
        }

        public static List<_Povijest> PosvijestIspisaNaloga(int idGrada, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var pov = from p in db.UPLATNICE_POVIJESTs
                              join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                              join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                              join i in db.UPLATNICE_IZNOSIs on p.IDIznosa equals i.IDIznosa into iznos
                              from ii in iznos.DefaultIfEmpty()
                              orderby p.Datum descending
                              where p.IDGrada == idGrada
                              select
                                  new _Povijest(p.IDPovijesti, p.IDGrada, g.NazivGrada, p.IDRedarstva, r.NazivRedarstva,
                                      p.IDIznosa, ii.Iznos, p.Stranica, p.Datum, p.Nalog, p.Ispis);

                    return pov.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "POVIJEST ISPISA NALOGA");
                return new List<_Povijest>();
            }
        }

        public static bool SpremiPovijestIspisa(_Povijest povijest, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {

                    if (povijest.IDPovijesti == -1)
                    {

                        if (db.UPLATNICE_POVIJESTs.Any(i => i.IDGrada == povijest.IDGrada && i.IDRedarstva == povijest.IDRedarstva && i.IDIznosa == povijest.IDIznosa && i.Nalog != null && i.Datum.Year == DateTime.Now.Year))
                        {
                            UPLATNICE_POVIJEST upi = db.UPLATNICE_POVIJESTs.Where(i => i.IDGrada == povijest.IDGrada && i.IDRedarstva == povijest.IDRedarstva && i.IDIznosa == povijest.IDIznosa && i.Nalog != null && i.Datum.Year == DateTime.Now.Year).OrderByDescending(i => i.Nalog).First();

                            upi.Nalog = upi.Nalog + 1;
                            db.SubmitChanges();

                            return true;
                        }

                        UPLATNICE_POVIJEST upin = new UPLATNICE_POVIJEST();

                        int idi = 1;

                        if (db.UPLATNICE_POVIJESTs.Any())
                        {
                            idi = db.UPLATNICE_POVIJESTs.Max(i => i.IDPovijesti) + 1;
                        }

                        upin.IDPovijesti = idi;
                        upin.IDGrada = povijest.IDGrada;
                        upin.IDRedarstva = povijest.IDRedarstva;
                        upin.IDIznosa = povijest.IDIznosa;
                        upin.Stranica = povijest.Stranica;
                        upin.Datum = povijest.Datum;
                        upin.Nalog = 1;
                        upin.Ispis = povijest.Ispis;

                        db.UPLATNICE_POVIJESTs.InsertOnSubmit(upin);
                        db.SubmitChanges();

                        return true;
                    }

                    UPLATNICE_POVIJEST up = new UPLATNICE_POVIJEST();

                    int id = 1;

                    if (db.UPLATNICE_POVIJESTs.Any())
                    {
                        id = db.UPLATNICE_POVIJESTs.Max(i => i.IDPovijesti) + 1;
                    }

                    up.IDPovijesti = id;
                    up.IDGrada = povijest.IDGrada;
                    up.IDRedarstva = povijest.IDRedarstva;
                    up.IDIznosa = povijest.IDIznosa;
                    up.Stranica = povijest.Stranica;
                    up.Datum = povijest.Datum;
                    up.Nalog = povijest.Nalog - 1;
                    up.Ispis = povijest.Ispis;

                    db.UPLATNICE_POVIJESTs.InsertOnSubmit(up);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "SPREMI POVIJEST ISPISA NALOGA");
                return false;
            }
        }

        public static bool? ProvijeriGodinu(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.UPLATNICE_POVIJESTs.OrderByDescending(i => i.Datum).First().Datum.Year != DateTime.Today.Year;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "NOVA GODINA");
                return null;
            }
        }

        /*:: DODAJ GRAD ::*/

        public static int KreirajNoviGrad(_Grad grad, int idAplikacije)
        {
            try
            {
                using (DbConnection connection = new SqlConnection(Sistem._master))
                {
                    connection.Open();

                    StringBuilder sbpg = new StringBuilder();

                    sbpg.AppendLine("CREATE DATABASE " + grad.Baza);
                    sbpg.AppendLine("ON");
                    sbpg.AppendLine("(NAME = " + grad.Baza + ",");
                    //sbpg.AppendLine("FILENAME = 'E:\\BAZE\" + grad.Baza + ".mdf',");
                    sbpg.AppendLine("FILENAME = 'D:\\DATA\\" + grad.Baza + ".mdf',");
                    sbpg.AppendLine("SIZE = 50MB,");
                    sbpg.AppendLine("MAXSIZE = UNLIMITED,");
                    sbpg.AppendLine("FILEGROWTH = 10MB)");
                    sbpg.AppendLine("LOG ON");
                    sbpg.AppendLine("(NAME = " + grad.Baza + "_log,");
                    //sbpg.AppendLine("FILENAME = 'E:\\BAZE\\" + grad.Baza + "_log.ldf',");
                    sbpg.AppendLine("FILENAME = 'D:\\DATA\\" + grad.Baza + "_log.ldf',");
                    sbpg.AppendLine("SIZE = 10MB,");
                    sbpg.AppendLine("MAXSIZE = UNLIMITED,");
                    sbpg.AppendLine("FILEGROWTH = 10%);");

                    using (DbCommand command = new SqlCommand(sbpg.ToString()))
                    {
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }

                    string collation = "ALTER DATABASE " + grad.Baza + " COLLATE Croatian_100_CI_AS";

                    using (DbCommand command = new SqlCommand(collation))
                    {
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }

                    int id = DodajGrad(grad, idAplikacije);

                    if (id != -1)
                    {
                        using (DbConnection newconnection = new SqlConnection(Sistem._master.Replace("master", grad.Baza)))//ConnectionString(grad.Baza, 1)))
                        {
                            newconnection.Open();

                            string owner = "EXEC sp_changedbowner 'pazigrad';";

                            using (DbCommand command = new SqlCommand(owner))
                            {
                                command.Connection = newconnection;
                                command.ExecuteNonQuery();
                            }

                            ////todo
                            //try
                            //{
                            //    string vpp = "EXEC sp_adduser 'vpp', 'vpp', 'dbo';";

                            //    using (DbCommand command = new SqlCommand(vpp))
                            //    {
                            //        command.Connection = newconnection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception)
                            //{
                            //}

                            newconnection.Close();
                        }

                        //Posalji.Email(grad.Baza, Pripremi.PopulateBodyJLS(grad.Naziv, korisnik, grad.AktivacijskiKod), "Pazigrad - NOVA JLS!", new List<string>() { "support@pazigrad.com" }, null, true, idAplikacije);
                    }

                    return id;
                }

            }
            catch
            {
                return -1;
            }
        }

        public static int DodajGrad(_Grad grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    GRADOVI gr = new GRADOVI();

                    int id = 1;

                    if (db.GRADOVIs.Any())
                    {
                        id = db.GRADOVIs.Max(i => i.IDGrada) + 1;
                    }

                    gr.IDGrada = id;
                    gr.NazivGrada = grad.Naziv;
                    gr.Latitude = (decimal)grad.Latitude;
                    gr.Longitude = (decimal)grad.Longitude;
                    gr.IznosNaloga = 1;
                    gr.Instanca = "10.0.1.243";
                    gr.Baza = grad.Baza;
                    gr.Korisnik = "pazigrad";
                    gr.Lozinka = "p4z1gr4d";
                    gr.Zoom = grad.Zoom;
                    gr.Pauk = false;
                    gr.Aktivan = false;
                    gr.Vpp = true;
                    gr.Odvjetnici = false;
                    gr.AktivacijskiKod = grad.AktivacijskiKod;
                    gr.Adresa = "";
                    gr.Grb = grad.Grb;
                    gr.NaplataPauk = null;
                    gr.DOF = false;
                    gr.PazigradNaIzvjestaju = true;
                    gr.ZalbaRedarstva = "";
                    gr.NaplataPauk = null;
                    gr.Chat = false;
                    gr.Mapa = "GoogleMap";
                    gr.GO = null;
                    gr.IDGrupePromet = null;
                    gr.Tip = "";
                    gr.Lisice = false;
                    gr.OdlukaLisice = "";
                    gr.VanjskePrijave = false;
                    gr.PrilogObavijest = false;
                    gr.DohvatVlasnikaMUP = false;
                    gr.RacunHUB = false;
                    gr.NakonDanaLezarina = 0;

                    db.GRADOVIs.InsertOnSubmit(gr);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "DODAJ GRAD");
                return -1;
            }
        }
    }
}