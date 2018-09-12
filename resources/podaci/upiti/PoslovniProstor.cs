using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class PoslovniProstor
    {
        public static _PoslovniProstor DohvatiPoslovniProstor(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var red = from r in db.POSLOVNI_PROSTORs
                              where r.IDRedarstva == idRedarstva
                              select
                                  new _PoslovniProstor(r.IDPoslovnogProstora, r.IDFiskaliziranog, r.IDVlasnikaFiskal, r.IDRedarstva, r.Naziv, r.Podnaslov, r.USustavu, r.Web,
                                      r.Email, r.Opis, r.Ulica, r.Broj, r.Dodatak, r.Posta, r.Mjesto, r.Tel, r.Fax, r.Banka, r.OIB, r.Oznaka, r.RadnoVrijeme,
                                      r.DatumPrimjene, r.PDV, r.Dospijece, r.Logo == null ? null : r.Logo.ToArray(),
                                      Gradovi.Uplatnica(grad, idRedarstva, idAplikacije),
                                      DohvatiPostavkeIspisa(grad, idRedarstva, idAplikacije));


                    if (!red.Any())
                    {
                        return null;
                    }

                    return red.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "POSLOVNI PROSTOR");
                return null;
            }
        }

        public static bool? IzmjeniPoslovniProstor(string grad, _PoslovniProstor prostor, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    POSLOVNI_PROSTOR pp;

                    if (db.POSLOVNI_PROSTORs.Any(i => i.IDRedarstva == prostor.IDRedarstva))
                    {
                        pp = db.POSLOVNI_PROSTORs.First(i => i.IDRedarstva == prostor.IDRedarstva);
                    }
                    else
                    {
                        pp = new POSLOVNI_PROSTOR();

                        int id = 1;

                        if (db.POSLOVNI_PROSTORs.Any())
                        {
                            id = db.POSLOVNI_PROSTORs.Max(i => i.IDRedarstva);
                        }

                        pp.IDPoslovnogProstora = id;
                        pp.IDRedarstva = prostor.IDRedarstva;
                    }

                    pp.Naziv = prostor.Naziv;
                    pp.Podnaslov = prostor.Podnaslov;
                    pp.USustavu = prostor.USustavu;
                    pp.Web = prostor.Web;
                    pp.Email = prostor.Email;
                    pp.Opis = prostor.Opis;
                    pp.Ulica = prostor.Ulica;
                    pp.Broj = prostor.Broj;
                    pp.Dodatak = prostor.Dodatak;
                    pp.Posta = prostor.Posta;
                    pp.Mjesto = prostor.Mjesto;
                    pp.Tel = prostor.Tel;
                    pp.Fax = prostor.Fax;
                    pp.Banka = prostor.Banka;
                    pp.OIB = prostor.OIB;
                    pp.Oznaka = prostor.Oznaka ?? ""; //todo obrisi
                    pp.RadnoVrijeme = prostor.RadnoVrijeme;
                    pp.DatumPrimjene = prostor.DatumPrimjene;
                    pp.PDV = prostor.PDV;
                    pp.Dospijece = prostor.Dosipijece;

                    if (!db.POSLOVNI_PROSTORs.Any(i => i.IDRedarstva == prostor.IDRedarstva))
                    {
                        prostor.IDPoslovnogProstora = pp.IDPoslovnogProstora;
                        db.POSLOVNI_PROSTORs.InsertOnSubmit(pp);
                    }

                    db.SubmitChanges();

                    int ok = Gradovi.IzmjeniUplatnicu(grad, prostor.Uplatnica, idAplikacije);

                    if (ok == -1)
                    {
                        return null;
                    }

                    if (pp.IDFiskaliziranog != null)
                    {
                        Fiskalizacija.IzmjeniPP(grad, prostor, idAplikacije);
                    }
                    else
                    {
                        Fiskalizacija.DodajPP(grad, prostor, idAplikacije);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI POSLOVNI PROSTOR");
                return false;
            }
        }

        public static bool SpremiLogo(string grad, byte[] logo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    POSLOVNI_PROSTOR pp;

                    if (db.POSLOVNI_PROSTORs.Any(i => i.IDRedarstva == idRedarstva))
                    {
                        pp = db.POSLOVNI_PROSTORs.First(i => i.IDRedarstva == idRedarstva);
                    }
                    else
                    {
                        pp = new POSLOVNI_PROSTOR();

                        int id = 1;

                        if (db.POSLOVNI_PROSTORs.Any())
                        {
                            id = db.POSLOVNI_PROSTORs.Max(i => i.IDRedarstva);
                        }

                        pp.IDPoslovnogProstora = id;
                        pp.IDRedarstva = idRedarstva;
                    }

                    pp.Logo = logo;

                    if (!db.POSLOVNI_PROSTORs.Any())
                    {
                        db.POSLOVNI_PROSTORs.InsertOnSubmit(pp);
                    }

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI LOGO POSLOVNOG PROSTORA");
                return false;
            }
        }

        /*:: NAPLATNA MJESTA ::*/

        public static _NaplatnoMjesto NaplatnoMjesto(string grad, string oznaka, int idProstora, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var red = from r in db.NAPLATNA_MJESTAs
                        where r.OznakaNaplatnogMjesta == oznaka &&
                              r.IDPoslovnogProstora == idProstora
                        select
                            new _NaplatnoMjesto(r.IDNaplatnogMjesta, r.IDPoslovnogProstora, r.OznakaNaplatnogMjesta, r.Naziv, r.Sifra, r.Adresa, r.Posta, r.Mjesto, r.Glavno);

                    if (!red.Any())
                    {
                        return null;
                    }

                    return red.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "NAPLATNO MJESTO");
                return null;
            }
        }

        public static List<_NaplatnoMjesto> NaplatnaMjesta(string grad, int idProstora, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var red = from r in db.NAPLATNA_MJESTAs
                        where r.IDPoslovnogProstora == idProstora
                        select
                            new _NaplatnoMjesto(r.IDNaplatnogMjesta, r.IDPoslovnogProstora, r.OznakaNaplatnogMjesta, r.Naziv, r.Sifra, r.Adresa, r.Posta, r.Mjesto, r.Glavno);

                    return red.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "NAPLATNA MJESTA");
                return new List<_NaplatnoMjesto>();
            }
        }

        /*:: POSTAVKE ISPISA RAČUNA ::*/

        public static List<_PostavkeIspisa> DohvatiPostavkeIspisa(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var red = from r in db.POSTAVKE_ISPISAs
                        where r.IDGrada == Sistem.IDGrada(grad) //&&
                              //r.IDRedarstva == idRedarstva //todo dodati idredarstva ispisa
                        select
                            new _PostavkeIspisa(r.IDPostavke, r.IDGrada, r.IDRedarstva, r.IDVrstePlacanja, r.IDStatusa,
                                r.ZalbaRedarstva, r.OdlukaGradaCjenik, r.OdlukaGradaLisice, r.Paragraf1, r.Boldano,
                                r.Primjeda, r.Naslov, r.Naredba,  r.TemeljniKapital, r.Mjesto, r.HUB, r.R1, r.ZapisnikRacun, r.ZapsinikNovaStrana, r.StavkeNaZapisniku, r.Privola);

                    if (!red.Any())
                    {
                        return null;
                    }

                    return red.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "POSTAVKE ISPISA");
                return null;
            }
        }

        public static bool IzmjeniPostavkeIspisa(string grad, _PostavkeIspisa postavke, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    POSTAVKE_ISPISA p = db.POSTAVKE_ISPISAs.First(r => r.IDPostavke == postavke.IDPostavke);
                    //r.IDGrada == Sistem.IDGrada(grad) && r.IDRedarstva == postavke.IDRedarstva && r.IDStatusa == postavke.IDStatusa && r.IDVrstePlacanja == postavke.IDVrstePlacanja);

                    p.ZalbaRedarstva = postavke.ZalbaRedarstva;
                    p.OdlukaGradaCjenik = postavke.OdlukaCjenik;
                    p.OdlukaGradaLisice = postavke.OdlukaLisice;
                    p.Paragraf1 = postavke.Paragraf1;
                    p.Boldano = postavke.Boldano;
                    p.Primjeda = postavke.Primjedba;
                    p.Naslov = postavke.Naslov;
                    p.Naredba = postavke.Naredba;
                    p.TemeljniKapital = postavke.TemeljniKapital;
                    p.Mjesto = postavke.Mjesto;
                    p.HUB = postavke.HUB;
                    p.R1 = postavke.R1;
                    p.ZapisnikRacun = postavke.ZapisnikRacun;
                    p.ZapsinikNovaStrana = postavke.ZapisnikNovaStrana;
                    p.StavkeNaZapisniku = postavke.StavkeNaZapisniku;
                    p.Privola = postavke.Privola;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI POSTAVKE ISPISA");
                return false;
            }
        }

        public static int DodajPostavkuIspisa(string grad, _PostavkeIspisa postavke, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    POSTAVKE_ISPISA p = new POSTAVKE_ISPISA();

                    p.IDGrada = Sistem.IDGrada(grad);
                    p.IDRedarstva = postavke.IDRedarstva;
                    p.IDStatusa = postavke.IDStatusa;
                    p.IDVrstePlacanja = postavke.IDVrstePlacanja;
                    p.ZalbaRedarstva = postavke.ZalbaRedarstva;
                    p.OdlukaGradaCjenik = postavke.OdlukaCjenik;
                    p.OdlukaGradaLisice = postavke.OdlukaLisice;
                    p.Paragraf1 = postavke.Paragraf1;
                    p.Boldano = postavke.Boldano;
                    p.Primjeda = postavke.Primjedba;
                    p.Naslov = postavke.Naslov;
                    p.Naredba = postavke.Naredba;
                    p.TemeljniKapital = postavke.TemeljniKapital;
                    p.Mjesto = postavke.Mjesto;
                    p.HUB = postavke.HUB;
                    p.R1 = postavke.R1;
                    p.ZapisnikRacun = postavke.ZapisnikRacun;
                    p.ZapsinikNovaStrana = postavke.ZapisnikNovaStrana;
                    p.StavkeNaZapisniku = postavke.StavkeNaZapisniku;
                    p.Privola = postavke.Privola;

                    db.POSTAVKE_ISPISAs.InsertOnSubmit(p);
                    db.SubmitChanges();

                    return p.IDPostavke;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "DODAJ POSTAVKU ISPISA");
                return -1;
            }
        }

        public static bool KopirajPostavkeIspisa(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var red = from r in db.POSTAVKE_ISPISAs
                        where r.IDGrada == 5 &&
                              r.IDRedarstva == idRedarstva
                        select
                            new _PostavkeIspisa(r.IDPostavke, r.IDGrada, r.IDRedarstva, r.IDVrstePlacanja, r.IDStatusa,
                                r.ZalbaRedarstva, r.OdlukaGradaCjenik, r.OdlukaGradaLisice, r.Paragraf1, r.Boldano,
                                r.Primjeda, r.Naslov, r.Naredba, r.TemeljniKapital, r.Mjesto, r.HUB, r.R1, r.ZapisnikRacun, r.ZapsinikNovaStrana, r.StavkeNaZapisniku, r.Privola);

                    foreach (var postavke in red)
                    {
                        POSTAVKE_ISPISA p = new POSTAVKE_ISPISA();

                        p.IDGrada = Sistem.IDGrada(grad);
                        p.IDRedarstva = idRedarstva;
                        p.IDStatusa = postavke.IDStatusa;
                        p.IDVrstePlacanja = postavke.IDVrstePlacanja;
                        p.ZalbaRedarstva = "";
                        p.OdlukaGradaCjenik = "";
                        p.OdlukaGradaLisice = "";
                        p.Paragraf1 = postavke.Paragraf1;
                        p.Boldano = postavke.Boldano;
                        p.Primjeda = postavke.Primjedba;
                        p.Naslov = postavke.Naslov;
                        p.Naredba = postavke.Naredba;
                        p.TemeljniKapital = "";
                        p.Mjesto = "";
                        p.HUB = postavke.HUB;
                        p.R1 = postavke.R1;
                        p.ZapisnikRacun = postavke.ZapisnikRacun;
                        p.ZapsinikNovaStrana = postavke.ZapisnikNovaStrana;
                        p.StavkeNaZapisniku = postavke.StavkeNaZapisniku;
                        p.Privola = postavke.Privola;

                        db.POSTAVKE_ISPISAs.InsertOnSubmit(p);
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "KOPIRAJ POSTAVKE ISPISA");
                return false;
            }
        }
    }
}