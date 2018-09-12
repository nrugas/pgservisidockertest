using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using PG.Servisi.FiskalizacijaSR;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Fiskalizacija
    {
        public static void Fiskaliziraj(string grad, int idRacuna, int idRedarstva, int idAplikacije)
        {
            try
            {
                _Racun racun = Naplata.DohvatiRacunLight(grad, idRacuna, idAplikacije);

                _FiskalOdgovor fo;

                _PoslovniProstor pp = PoslovniProstor.DohvatiPoslovniProstor(grad, idRedarstva, idAplikacije);

                if (pp.IDFisklaizacije == null)
                {
                    return;
                }

                bool demo = grad == "Lokacije";

                using (FiskalServiceClient sc = new FiskalServiceClient())
                {
                    List<_RacunPDV> obrpdv = new List<_RacunPDV>();
                    obrpdv.Add(new _RacunPDV { Iznos = racun.PDV, Osnovica = racun.Osnovica, Stopa = racun.PDVPosto });

                    string vpk = Naplata.VrstaPlacanjaKratica(grad, racun.IDVrste, idAplikacije);

                    fo = sc.FiskalizirajRacunSVE(pp.IDVlasnikaFiskal.Value, pp.IDFisklaizacije.Value, racun.Blagajna, racun.Godina, racun.RedniBroj, racun.OIB, racun.DatumVrijeme,
                            racun.Ukupno, obrpdv, new _RacunPDV(), 0, Convert.ToChar(vpk), demo, false);

                    sc.Close();
                }

                if (fo != null)
                {
                    using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                    {
                        RACUNI r = db.RACUNIs.First(i => i.IDRacuna == idRacuna);

                        r.JIR = fo.Jir;
                        r.ZKI = fo.Zki;
                        r.UUID = fo.Uuid;

                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "FISKALIZACIJA RAČUNA");
            }
        }

        public static string PonovnaFiskalizacija(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    _PoslovniProstor pp = PoslovniProstor.DohvatiPoslovniProstor(grad, idRedarstva, idAplikacije);
                    bool demo = grad == "Lokacije";

                    int broj, x = 0;
                    foreach (var racun in DohvatiRacuneZaFiskalizaciju(grad, idRedarstva, out broj, idAplikacije))
                    {
                        try
                        {
                            _FiskalOdgovor fo;

                            using (FiskalServiceClient sc = new FiskalServiceClient())
                            {
                                List<_RacunPDV> obrpdv = new List<_RacunPDV>();
                                obrpdv.Add(new _RacunPDV { Iznos = racun.PDV, Osnovica = racun.Osnovica, Stopa = racun.PDVPosto });

                                string vpk = Naplata.VrstaPlacanjaKratica(grad, racun.IDVrste, idAplikacije);

                                fo = sc.FiskalizirajRacunSVE(pp.IDVlasnikaFiskal.Value, pp.IDFisklaizacije.Value, racun.Blagajna, racun.Godina, racun.RedniBroj, racun.OIB, racun.DatumVrijeme,
                                    racun.Ukupno, obrpdv, null, 0, Convert.ToChar(vpk), demo, true);

                                sc.Close();
                            }

                            if (fo != null)
                            {
                                RACUNI r = db.RACUNIs.First(i => i.IDRacuna == racun.IDRacuna);

                                r.JIR = fo.Jir;
                                r.ZKI = fo.Zki;
                                r.UUID = fo.Uuid;

                                db.SubmitChanges();

                                if (fo.Jir != "")
                                {
                                    x++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Sustav.SpremiGresku(grad, ex, idAplikacije, "FISKALIZACIJA RAČUNA: " + racun.IDRacuna);
                        }
                    }

                    return x + "/" + broj;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "FISKALIZACIJA RAČUNA");
                return null;
            }
        }

        public static List<_Racun> DohvatiRacuneZaFiskalizaciju(string grad, int idRedarstva, out int broj, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              join d in db.Djelatniks on r.IDDjelatnika equals d.IDDjelatnika
                              where r.JIR == "" &&
                                    r.IDRedarstva == idRedarstva &&
                                    (r.IDVrstePlacanja != 4 && r.IDVrstePlacanja != 5)//ne fiskaliziram transakcijski racun
                              select new _Racun(r.IDRacuna, r.IDReference, r.IDZakljucenja, r.IDVrstePlacanja, r.IDVrsteKartice, r.IDBanke, Naplata.VrstaPlacanja(grad, r.IDVrstePlacanja, idAplikacije), Naplata.VrstaBanke(grad, r.IDBanke, idAplikacije), r.IDDjelatnika,
                                      d.ImeNaRacunu, r.IDRedarstva, r.Datum, r.RB, r.Godina, r.PDV, r.Osnovica, r.Ukupno, r.PDVPosto, r.OIB, r.Blagajna, r.BrojRacuna,
                                      r.Storniran, r.Orginal, r.Napomena, r.Uplacen, r.JIR, r.ZKI, r.UUID, r.DatumPreuzimanja, r.PoslovniProstor, r.PozivNaBroj, r.BrojOdobrenja,
                                      Naplata.VrstaKartice(grad, r.IDVrsteKartice, idAplikacije), Naplata.RegistracijaRacuna(grad, r.IDReference, r.IDRedarstva, idAplikacije), r.Prenesen, r.Rate, d.Blagajna, false, "", null, null);

                    broj = rac.Count();
                    return rac.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI RACUN");
                broj = 0;
                return null;
            }
        }

        /*:: POSLOVNI PROSTOR ::*/

        public static bool DodajPP(string grad, _PoslovniProstor pos, int idAplikacije)
        {
            try
            {
                int idPoste;

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    idPoste = db.POSTEs.First(i => i.Posta == pos.Posta).IDPoste;
                }

                using (FiskalServiceClient sc = new FiskalServiceClient())
                {
                    _Vlasnik vl = new _Vlasnik()
                    {
                        Id = pos.IDVlasnikaFiskal ?? 0,
                        Naziv = pos.Naziv,
                        OIB = pos.OIB,
                        SjedisteDodKBR = pos.Dodatak,
                        SjedisteKBR = pos.Broj,
                        SjedistePosta = idPoste,
                        SjedisteUlica = pos.Ulica,
                        TipOsobe = 'P',
                        USustavuPDVa = true //todo
                    };

                    int idVlasnika = pos.IDVlasnikaFiskal == null ? sc.DodajVlasnika(vl) : pos.IDVlasnikaFiskal.Value;

                    if (idVlasnika == -1)
                    {
                        return false;
                    }

                    FiskalizacijaSR._PoslovniProstor pp = new FiskalizacijaSR._PoslovniProstor
                    {
                        Id = -1,
                        JLSNaziv = pos.Mjesto,
                        KBR = pos.Broj,
                        Naziv = pos.Naziv,
                        NazivRed1 = pos.Podnaslov,
                        NazivRed2 = "",
                        DodKBR = pos.Dodatak,
                        OznakaNaRacunu = pos.Oznaka,
                        Posta = idPoste,
                        Ulica = pos.Ulica,
                        Vlasnik = idVlasnika,
                        SljednostRacuna = "P",
                        Zatvoren = ""
                    };

                    int idpp = sc.DodajPoslovniProstor(pp);

                    if (idpp != -1)
                    {
                        _RadnoVrijeme rv = new _RadnoVrijeme()
                        {
                            PoslovniProstor = idpp,
                            Opis = pos.RadnoVrijeme,
                            PocetakPrimjene = DateTime.Now
                        };

                        int idrv = sc.DodajRadnoVrijeme(rv);
                    }

                    sc.Close();

                    using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                    {
                        POSLOVNI_PROSTOR poslpro = db.POSLOVNI_PROSTORs.First(i => i.IDPoslovnogProstora == pos.IDPoslovnogProstora);

                        poslpro.IDVlasnikaFiskal = idVlasnika;
                        poslpro.IDFiskaliziranog = idpp == -1 ? (int?)null : idpp;

                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "REGISTRIRAJ POSLOVNI PROSTOR");
                return false;
            }
        }

        public static bool IzmjeniPP(string grad, _PoslovniProstor pos, int idAplikacije)
        {
            try
            {
                int idPoste;

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    idPoste = db.POSTEs.First(i => i.Posta == pos.Posta).IDPoste;
                }
                using (FiskalServiceClient sc = new FiskalServiceClient())
                {
                    _Vlasnik vl = new _Vlasnik
                    {
                        Id = pos.IDVlasnikaFiskal.Value,
                        Naziv = pos.Naziv,
                        OIB = pos.OIB,
                        SjedisteDodKBR = pos.Dodatak,
                        SjedisteKBR = pos.Broj,
                        SjedistePosta = idPoste,
                        SjedisteUlica = pos.Ulica,
                        TipOsobe = 'P',
                        USustavuPDVa = true //todo
                    };

                    bool Vlasnik = sc.IzmjeniVlasnika(vl);

                    if (Vlasnik)
                    {
                        FiskalizacijaSR._PoslovniProstor pp = new FiskalizacijaSR._PoslovniProstor()
                        {
                            Id = pos.IDFisklaizacije.Value,
                            JLSNaziv = pos.Mjesto,
                            KBR = pos.Broj,
                            Naziv = pos.Naziv,
                            NazivRed1 = pos.Podnaslov,
                            NazivRed2 = "",
                            DodKBR = pos.Dodatak,
                            OznakaNaRacunu = pos.Oznaka,
                            Posta = idPoste,
                            Ulica = pos.Ulica,
                            Vlasnik = pos.IDVlasnikaFiskal.Value,
                            SljednostRacuna = "P",
                            Zatvoren = ""
                        };

                        sc.IzmjeniPoslovniProstor(pp);

                        _RadnoVrijeme rv = new _RadnoVrijeme
                        {
                            PoslovniProstor = pos.IDFisklaizacije.Value,
                            Opis = pos.RadnoVrijeme,
                            PocetakPrimjene = DateTime.Now,
                            Zatvaranje = "",
                            Pon = "",
                            Uto = "",
                            Sri = "",
                            Cet = "",
                            Pet = "",
                            Sub = "",
                            Ned = "",
                            Praznik = "",
                            Prijavljen = false
                        };

                        bool idrv = sc.IzmjeniRadnoVrijeme(rv);
                    }
                    else
                    {
                        sc.Close();
                        return false;
                    }

                    sc.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "REGISTRIRAJ POSLOVNI PROSTOR - IZMJENA");
                return false;
            }
        }

        public static bool RegistrirajPP(string grad, _PoslovniProstor pos, int idAplikacije)
        {
            try
            {
                if (pos.IDFisklaizacije == null)
                {
                    return false;
                }

                bool demo = grad == "Lokacije";

                using (FiskalServiceClient sc = new FiskalServiceClient())
                {
                    bool ok = sc.PrijaviPoslovniProstor(pos.IDFisklaizacije.Value, demo);
                    sc.Close();

                    using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                    {
                        POSLOVNI_PROSTOR poslpro = db.POSLOVNI_PROSTORs.First(i => i.IDPoslovnogProstora == pos.IDPoslovnogProstora);
                        poslpro.DatumPrimjene = DateTime.Now;
                        db.SubmitChanges();
                    }

                    return ok;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "REGISTRIRAJ POSLOVNI PROSTOR");
                return false;
            }
        }

        /*:: POSLOVNI PROSTOR ::*/

        public static bool DodajCertifikat(string grad, string sifra, byte[] certifikat, int idVlasnika, int idAplikacije)
        {
            //Console.WriteLine("{0}Subject: {1}{0}", Environment.NewLine, x509.Subject);
            //Console.WriteLine("{0}Issuer: {1}{0}", Environment.NewLine, x509.Issuer);
            //Console.WriteLine("{0}Version: {1}{0}", Environment.NewLine, x509.Version);
            //Console.WriteLine("{0}Valid Date: {1}{0}", Environment.NewLine, x509.NotBefore);
            //Console.WriteLine("{0}Expiry Date: {1}{0}", Environment.NewLine, x509.NotAfter);
            //Console.WriteLine("{0}Thumbprint: {1}{0}", Environment.NewLine, x509.Thumbprint);
            //Console.WriteLine("{0}Serial Number: {1}{0}", Environment.NewLine, x509.SerialNumber);
            //Console.WriteLine("{0}Friendly Name: {1}{0}", Environment.NewLine, x509.PublicKey.Oid.FriendlyName);
            //Console.WriteLine("{0}Public Key Format: {1}{0}", Environment.NewLine, x509.PublicKey.EncodedKeyValue.Format(true));
            //Console.WriteLine("{0}Raw Data Length: {1}{0}", Environment.NewLine, x509.RawData.Length);
            //Console.WriteLine("{0}Certificate to string: {1}{0}", Environment.NewLine, x509.ToString(true));

            //Console.WriteLine("{0}Certificate to XML String: {1}{0}", Environment.NewLine, x509.PublicKey.Key.ToXmlString(false));

            try
            {
                X509Certificate2 x509 = new X509Certificate2();
                x509.Import(certifikat, sifra, X509KeyStorageFlags.DefaultKeySet);
                int id;

                using (FiskalServiceClient sc = new FiskalServiceClient())
                {
                    _Certifikat cr = new _Certifikat();
                    cr.Certifikat = certifikat;
                    cr.Demo = grad == "Lokacije" || grad == "PROMETNIK_OTOCAC";
                    cr.Lozinka = sifra;
                    cr.IDVlasnika = idVlasnika;
                    cr.VrijediOd = x509.NotBefore;
                    cr.VrijediDo = x509.NotAfter;

                    id = sc.DodajCertifikat(cr);
                }

                return id > 0;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ CERTIFIKAT");
                return false;
            }
        }
    }
}