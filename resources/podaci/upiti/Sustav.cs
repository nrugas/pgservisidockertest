using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.cs.email;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Sustav
    {
        public static _Grad AktivacijaAplikacije(string aktivacijskiKod, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var gra = from g in db.GRADOVIs
                              join c in db.CERTIFIKATI_MUPs on g.IDGrada equals c.IDGrada into mup
                              from cc in mup.DefaultIfEmpty()
                              where g.AktivacijskiKod.ToUpper() == aktivacijskiKod.ToUpper()
                              select new _Grad(g.IDGrada, g.Baza, g.NazivGrada, (double)g.Latitude, (double)g.Longitude, g.IznosNaloga, g.Zoom, g.Pauk, g.Aktivan,
                              g.Vpp, g.Odvjetnici, g.AktivacijskiKod, g.Adresa, g.Grb, g.NaplataPauk != null, g.NaplataPauk, g.DOF, g.PazigradNaIzvjestaju, g.ZalbaRedarstva,
                              g.Zadrska, g.Chat, cc.SmijePitati, g.Mapa, g.GO, g.IDGrupePromet, g.Tip, g.Lisice, g.OdlukaLisice, g.VanjskePrijave, g.PrilogObavijest, g.RacunHUB,
                                  g.NakonDanaLezarina, g.Parking);

                    if (gra.Any())
                    {
                        return gra.First();
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku("", ex, idAplikacije, "Aktivacija Aplikacije");
                return null;
            }
        }

        public static List<_2DLista> Aplikacije(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var drz = from d in db.APLIKACIJEs
                              where d.Prikazi
                              orderby d.NazivAplikacije
                              select new _2DLista(d.IDAplikacije, d.NazivAplikacije);

                    return drz.ToList();
                }
            }
            catch (Exception ex)
            {
                SpremiGresku("", ex, idAplikacije, "drzave");
                return new List<_2DLista>();
            }
        }

        /*:: GRESKE ::*/

        public static void SpremiGresku(string grad, Exception greska, int idAplikacije, string napomena)
        {
            SpremiGresku(grad, greska, idAplikacije, napomena, "");
        }

        public static void SpremiGresku(string grad, Exception greska, int idAplikacije, string napomena, string korisnik)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (napomena.ToUpper().Contains("UPDATE CHECK"))
                    {
                        return;
                    }

                    if (napomena.ToUpper().Contains("Zvonimir Nikolić".ToUpper()))
                    {
                        return;
                    }

                    GRESKE gr = new GRESKE();

                    gr.IDGrada = grad != "" ? Sistem.IDGrada(grad) : (int?)null;
                    gr.IDAplikacije = idAplikacije;
                    gr.Datum = DateTime.Now;
                    gr.Napomena = napomena.ToUpper();
                    gr.Greska = greska.ToString();
                    gr.Korisnik = korisnik;

                    db.GRESKEs.InsertOnSubmit(gr);
                    db.SubmitChanges();
                }
            }
            catch
            {
            }
        }

        /*:: PODRSKA ::*/

        public static bool PrijaviProblem(string grad, _Problem problem, List<byte[]> slike, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    _Djelatnik osoba = Korisnici.DohvatiDjelatnika(grad, problem.IDDjelatnika, idAplikacije);
                    var gr = db.GRADOVIs.First(i => i.Baza == grad);
                    string aplikacija = db.APLIKACIJEs.First(i => i.IDAplikacije == problem.IDAplikacije).NazivAplikacije;

                    PROBLEMI pk = new PROBLEMI();

                    pk.IDGrada = gr.IDGrada;
                    pk.IDDjelatnika = problem.IDDjelatnika;
                    pk.IDAplikacije = problem.IDAplikacije;
                    pk.OpsiProblema = problem.Opis;
                    pk.Ponavljanje = problem.Interval;
                    pk.NakonRadnje = problem.Radnja;
                    pk.IDRedara = problem.IDRedara;
                    pk.IDTerminala = problem.IDTerminala;
                    pk.DatumVrijeme = DateTime.Now;
                    pk.Rijeseno = false;

                    db.PROBLEMIs.InsertOnSubmit(pk);
                    db.SubmitChanges();

                    foreach (var b in slike)
                    {
                        PROBLEMI_SLIKE ps = new PROBLEMI_SLIKE();
                        ps.IDProblema = pk.IDProblema;
                        ps.Problem = true;
                        ps.Slika = b;

                        db.PROBLEMI_SLIKEs.InsertOnSubmit(ps);
                        db.SubmitChanges();
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(problem.Radnja);
                    sb.AppendLine(problem.Interval);
                    sb.AppendLine(problem.Opis);
                    sb.AppendLine();
                    if (problem.IDTerminala != null)
                    {
                        sb.AppendLine(string.Format("Terminal: {0} (ID: {1})", problem.Terminal, problem.IDTerminala));
                        sb.AppendLine();
                    }
                    if (problem.IDRedara != null)
                    {
                        sb.AppendLine(string.Format("Redar: {0} (ID: {1})", problem.Redar, problem.IDRedara));
                        sb.AppendLine();
                    }

                    //sb.AppendLine("#assign Ivo");
                    sb.AppendLine("#category Problem");
                    sb.AppendLine("#set Aplikacija=" + aplikacija);
                    sb.AppendLine("#set Sustav=PAZIGRAD");
                    sb.AppendLine("#set Osoba=" + osoba.ImePrezime);
                    sb.AppendLine("#set JLS=" + gr.NazivGrada);
                    sb.AppendLine("# created by " + (!string.IsNullOrEmpty(osoba.Email) ? osoba.Email : osoba.ImePrezime));//mora biti razmak da bi bilo ok

                    Posalji.EmailHelpDesk(grad, sb.ToString(), "Prijava problema - " + gr.NazivGrada, slike, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Prijavi problem");
                return false;
            }
        }

        public static bool PostaviPitanje(string grad, int idKorisnika, int idPrograma, string poruka, List<byte[]> slike, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    _Djelatnik osoba = Korisnici.DohvatiDjelatnika(grad, idKorisnika, idAplikacije);
                    var gr = db.GRADOVIs.First(i => i.Baza == grad);
                    string aplikacija = db.APLIKACIJEs.First(i => i.IDAplikacije == idPrograma).NazivAplikacije;

                    PITANJE_ZAHTJEV pz = new PITANJE_ZAHTJEV();

                    pz.IDGrada = gr.IDGrada;
                    pz.IDDjelatnika = idKorisnika;
                    pz.IDAplikacije = idPrograma;
                    pz.Datum = DateTime.Now;
                    pz.Pitanje = poruka;
                    pz.Rijesen = false;

                    db.PITANJE_ZAHTJEVs.InsertOnSubmit(pz);
                    db.SubmitChanges();

                    foreach (var b in slike)
                    {
                        PROBLEMI_SLIKE ps = new PROBLEMI_SLIKE();
                        ps.IDProblema = pz.IDPitanja;
                        ps.Problem = false;
                        ps.Slika = b;

                        db.PROBLEMI_SLIKEs.InsertOnSubmit(ps);
                        db.SubmitChanges();
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(poruka);
                    sb.AppendLine();
                    //sb.AppendLine("#assign Ivo");
                    sb.AppendLine("#category Pitanje");
                    sb.AppendLine("#set Aplikacija=" + aplikacija);
                    sb.AppendLine("#set Osoba=" + osoba.ImePrezime);
                    sb.AppendLine("#set Sustav=PAZIGRAD");
                    sb.AppendLine("#set JLS=" + gr.NazivGrada);
                    sb.AppendLine("# created by " + (!string.IsNullOrEmpty(osoba.Email) ? osoba.Email : osoba.ImePrezime));//mora biti razmak da bi bilo ok

                    Posalji.EmailHelpDesk(grad, sb.ToString(), "Pitanje / zahtjev - " + gr.NazivGrada, slike, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Postavi Pitanje");
                return false;
            }
        }

        public static bool ZahtjevVPP(string grad, string poruka, int idAplikacije, int idPrograma = 3)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var gr = db.GRADOVIs.First(i => i.Baza == grad);
                    string aplikacija = db.APLIKACIJEs.First(i => i.IDAplikacije == idPrograma).NazivAplikacije;

                    PITANJE_ZAHTJEV pz = new PITANJE_ZAHTJEV();

                    pz.IDGrada = gr.IDGrada;
                    pz.IDDjelatnika = 1;
                    pz.IDAplikacije = idPrograma;
                    pz.Datum = DateTime.Now;
                    pz.Pitanje = poruka;
                    pz.Rijesen = false;

                    db.PITANJE_ZAHTJEVs.InsertOnSubmit(pz);
                    db.SubmitChanges();

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(poruka);
                    sb.AppendLine();
                    sb.AppendLine("#assign Nenad");
                    sb.AppendLine("#category Pitanje");
                    sb.AppendLine("#set Aplikacija=" + aplikacija);
                    sb.AppendLine("#set Osoba= Ivo Opančar");
                    sb.AppendLine("#set Sustav=PAZIGRAD");
                    sb.AppendLine("#set JLS=" + gr.NazivGrada);
                    sb.AppendLine("# created by ivo.opancar@ri-ing.net");//mora biti razmak da bi bilo ok

                    Posalji.EmailHelpDesk(grad, sb.ToString(), "Interni zahtjev za: " + gr.NazivGrada, null, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Postavi Pitanje");
                return false;
            }
        }

        public static bool NaruciMaterijal(string grad, _Narudzba narudzba, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    _Djelatnik osoba = Korisnici.DohvatiDjelatnika(grad, narudzba.IDKorisnika, idAplikacije);
                    GRADOVI gr = db.GRADOVIs.First(i => i.Baza == grad);
                    string aplikacija = db.APLIKACIJEs.First(i => i.IDAplikacije == idAplikacije).NazivAplikacije;

                    NARUDZBE n = new NARUDZBE();

                    n.IDGrada = gr.IDGrada;
                    n.IDRedarstva = narudzba.IDRedarstva;
                    n.IDDjelatnika = narudzba.IDKorisnika;
                    n.Datum = DateTime.Now;
                    n.Tristo = narudzba.Tristo;
                    n.Petsto = narudzba.Petsto;
                    n.Sedamsto = narudzba.Sedamsto;
                    n.Etuia = narudzba.Etuia;
                    n.Traka = narudzba.Traka;
                    n.Napomena = narudzba.Napomena;
                    n.Dostava = narudzba.Dostava;
                    n.Rijeseno = false;
                    
                    db.NARUDZBEs.InsertOnSubmit(n);
                    db.SubmitChanges();

                    #region SLANJE E-MAILA

                    StringBuilder sb = new StringBuilder();

                    if (idAplikacije == 4)
                    {
                        sb.AppendLine("Naloga za pokušaj (pola iznosa): " + narudzba.Tristo);
                        sb.AppendLine("Naloga za naplatu (puni iznos): " + narudzba.Petsto);
                    }
                    else
                    {
                        if ((int)gr.IznosNaloga == 1)
                        {
                            sb.AppendLine("Naloga od 300 kn: " + narudzba.Tristo);
                            sb.AppendLine("Naloga od 500 kn: " + narudzba.Petsto);
                            sb.AppendLine("Naloga od 700 kn: " + narudzba.Sedamsto);
                        }
                        else if ((int)gr.IznosNaloga == 2)
                        {
                            sb.AppendLine("Naloga od 150 kn: " + narudzba.Tristo);
                            sb.AppendLine("Naloga od 250 kn: " + narudzba.Petsto);
                            sb.AppendLine("Naloga od 350 kn: " + narudzba.Sedamsto);
                        }
                        else if ((int)gr.IznosNaloga == 0)
                        {
                            sb.AppendLine("Naloga bez iznosa: " + narudzba.Tristo);
                        }
                        //else
                        //{
                        //    Fraction frac = new Fraction((double)iznos);
                        //    lblIznos.Content = "Na nalozima se ispisuje " + frac.Inverse().ToString() + " iznosa!";

                        //    lblTristo.Content = string.Format("Naloga od {0} kn:", Math.Round(300 / iznos, 2));
                        //    lblPetsto.Content = string.Format("Naloga od {0} kn:", Math.Round(500 / iznos, 2));
                        //    lblSedamsto.Content = string.Format("Naloga od {0} kn:", Math.Round(700 / iznos, 2));
                        //}
                    }

                    sb.AppendLine("");
                    sb.AppendLine("Etuia: " + (narudzba.Etuia == 0 ? "Bez etuia!" : narudzba.Etuia.ToString()));
                    sb.AppendLine("");
                    sb.AppendLine("Traka za printer: " + narudzba.Traka);
                    sb.AppendLine("");
                    sb.AppendLine("Način isporuke: " + narudzba.Dostava);

                    if (!string.IsNullOrEmpty(narudzba.Napomena))
                    {
                        sb.AppendLine("");
                        sb.AppendLine("Napomena:");
                        sb.AppendLine(narudzba.Napomena);
                    }

                    sb.AppendLine("");
                    sb.AppendLine("ADRESA ZA ISPORUKU:");
                    sb.AppendLine(gr.Adresa.Replace(",", "\r\n"));
                    sb.AppendLine("");

                    sb.AppendLine("#assign Nevija");
                    sb.AppendLine("#cc nevija.samardzic@ri-ing.net");
                    sb.AppendLine("#category Narudžba materijala");
                    sb.AppendLine("#set Aplikacija=" + aplikacija);
                    sb.AppendLine("#set Sustav=PAZIGRAD");
                    sb.AppendLine("#set Osoba=" + osoba.ImePrezime);
                    sb.AppendLine("#set JLS=" + gr.NazivGrada);
                    sb.AppendLine("# created by " + (!string.IsNullOrEmpty(osoba.Email) ? osoba.Email : osoba.ImePrezime));//mora biti razmak da bi bilo ok

                    Posalji.EmailHelpDesk(grad, sb.ToString(), "Narudžba materijala - " + gr.NazivGrada, null, idAplikacije);

                    #endregion

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Naruci Materijal");
                return false;
            }
        }

        public static List<_Narudzba> DohvatiNarudzbe(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {

                    var nar = from n in db.NARUDZBEs
                        where n.IDGrada == Sistem.IDGrada(grad) &&
                              n.IDRedarstva == 1
                        select new _Narudzba(n.IDNarudzbe, n.IDGrada, n.IDDjelatnika, n.IDRedarstva, n.Datum, n.Tristo,
                            n.Petsto, n.Sedamsto, n.Etuia, n.Traka, n.Napomena, "", n.Dostava, "", n.Rijeseno);

                    return nar.ToList();
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "DOHVATI NARUDZBE");
                return null;
            }
        }

        public static bool IspunjeniPodaci(string grad, int idKorisnika, int idAplikacije)
        {
            try
            {
                //using (PostavkeDataContext db = new PostavkeDataContext())
                //{

                //}

                //var nal = from n in db.NALOZIs
                //          join g in db.GRADOVIs on n.IDGrada equals g.IDGrada
                //          where g.Baza == grad
                //          select n;

                //if (!nal.Any())
                //{
                //    return false;
                //}

                //if (string.IsNullOrEmpty(nal.First().Adresa))
                //{
                //    return false;
                //}

                //if (string.IsNullOrEmpty(nal.First().Model))
                //{
                //    return false;
                //}

                //if (string.IsNullOrEmpty(nal.First().IBAN))
                //{
                //    return false;
                //}

                //if (string.IsNullOrEmpty(nal.First().Poziv1))
                //{
                //    return false;
                //}

                //if (string.IsNullOrEmpty(nal.First().Poziv2))
                //{
                //    return false;
                //}

                //if (string.IsNullOrEmpty(nal.First().Opis))
                //{
                //    return false;
                //}

                return true;
            }
            catch (Exception ex)
            {
                SpremiGresku("", ex, idAplikacije, "Ispunjeni Podaci");
                return false;
            }
        }

        /*:: AKTIVNOST ::*/

        public static bool Aktivnost(string grad, int idDjelatnika, string racunalo, string verzija, string korisnik, string os, bool odobrava, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {

                    if (db.AKTIVNE_APLIKACIJEs.Any(i => i.Racunalo == racunalo && i.IDAplikacije == idAplikacije))
                    {
                        try
                        {
                            AKTIVNE_APLIKACIJE aas = db.AKTIVNE_APLIKACIJEs.First(i => i.Racunalo == racunalo && i.IDAplikacije == idAplikacije);

                            aas.ZadnjaAktivnost = DateTime.Now;
                            aas.Verzija = verzija;
                            aas.OS = os;
                            aas.IDGrada = db.GRADOVIs.First(i => i.Baza == grad).IDGrada;
                            aas.Grad = grad;
                            aas.IDDjelatnika = idDjelatnika;
                            aas.Korisnik = korisnik;
                            aas.OdobravaZahtjeve = odobrava;
                            db.SubmitChanges();

                            return aas.Restart;
                        }
                        catch
                        {
                            return false;
                        }
                    }

                    AKTIVNE_APLIKACIJE aa = new AKTIVNE_APLIKACIJE();

                    aa.IDGrada = db.GRADOVIs.First(i => i.Baza == grad).IDGrada;
                    aa.IDAplikacije = idAplikacije;
                    aa.IDDjelatnika = idDjelatnika;
                    aa.Racunalo = racunalo;
                    aa.ZadnjaAktivnost = DateTime.Now;
                    aa.Verzija = verzija;
                    aa.Grad = grad;
                    aa.Korisnik = korisnik;
                    aa.OS = os;
                    aa.OdobravaZahtjeve = odobrava;

                    db.AKTIVNE_APLIKACIJEs.InsertOnSubmit(aa);
                    db.SubmitChanges();

                    return aa.Restart;
                }
            }
            catch
            {
                //SpremiGresku(grad, ex, idAplikacije, "Aktivnost");
                return false;
            }
        }

        public static void AktivnostReset(string grad, string racunalo, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    AKTIVNE_APLIKACIJE aas = db.AKTIVNE_APLIKACIJEs.First(i => i.Racunalo == racunalo && i.IDAplikacije == idAplikacije);

                    aas.Restart = false;
                    db.SubmitChanges();
                }
            }
            catch
            {
                //SpremiGresku(grad, ex, idAplikacije, "Aktivnost");
            }
        }

        public static bool ObrisiAktivnost(string grad, int idAktivnosti, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    db.AKTIVNE_APLIKACIJEs.DeleteOnSubmit(db.AKTIVNE_APLIKACIJEs.First(i => i.IDAktive == idAktivnosti));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "OBRISI AKTIVNE APLIKACIJE");
                return false;
            }
        }

        public static void Reset(string grad, int idAktivnosti, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    AKTIVNE_APLIKACIJE aas = db.AKTIVNE_APLIKACIJEs.First(i => i.IDAktive == idAktivnosti);
                    aas.Restart = true;
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "OBRISI AKTIVNE APLIKACIJE");
            }
        }

        public static List<_AktivneAplikacije> DohvatiAktivne(string grad, bool aktivni, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var kor = from a in db.AKTIVNE_APLIKACIJEs
                              join g in db.GRADOVIs on a.IDGrada equals g.IDGrada
                              join z in db.APLIKACIJEs on a.IDAplikacije equals z.IDAplikacije
                              where a.Korisnik != "Administrator" &&
                                    (grad != "" ? a.Grad == grad : grad == "") &&
                                    (aktivni ? a.ZadnjaAktivnost > DateTime.Now.AddDays(-15) : aktivni == false)
                              orderby a.ZadnjaAktivnost, a.Racunalo, a.IDAplikacije, a.Korisnik descending
                              select
                                  new _AktivneAplikacije(a.IDAktive, a.ZadnjaAktivnost, g.NazivGrada, a.Korisnik, a.Racunalo, z.NazivAplikacije, a.Verzija, a.OS,
                                      ZadnjaAktivnost(a.ZadnjaAktivnost), Aktivan(a.ZadnjaAktivnost), a.OdobravaZahtjeve, a.Restart);

                    return kor.ToList();
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "AKTIVNE APLIKACIJE");
                return new List<_AktivneAplikacije>();
            }
        }

        private static string ZadnjaAktivnost(DateTime zadnjaAktivnost)
        {
            string vri = "";

            TimeSpan ts = DateTime.Now.Subtract(zadnjaAktivnost);

            if (ts.Days > 0)
            {
                vri += ts.Days + " dana, ";
            }

            if (ts.TotalHours > 1)
            {
                vri += ts.Hours + " h, ";
            }

            if (ts.TotalMinutes > 0)
            {
                vri += ts.Minutes + " min.";
            }

            return string.Format("{0}", vri);
        }

        private static bool Aktivan(DateTime zadnjaAktivnost)
        {
            TimeSpan ts = DateTime.Now.Subtract(zadnjaAktivnost);

            if (ts.TotalMinutes < 5)
            {
                return true;
            }

            return false;
        }

        public static bool Aktivan(string grad, int idKorisnika, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.AKTIVNE_APLIKACIJEs.Any(a => a.IDDjelatnika == idKorisnika && a.IDGrada == Sistem.IDGrada(grad) &&
                                    a.ZadnjaAktivnost > DateTime.Now.AddSeconds(-120) && a.IDAplikacije != 6);
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "AKTIVNi KORISNIK");
                return false;
            }
        }

        /*:: AKCIJE KORISNIKA ::*/

        #region AKCIJE KORISNIKA

        public static void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    AKCIJE_DJELATNIKA ak = new AKCIJE_DJELATNIKA();

                    ak.IDAkcije = idAkcije;
                    ak.IDDjelatnika = idKorisnika;
                    ak.IDAplikacije = idAplikacije;
                    ak.Datum = DateTime.Now;
                    ak.IDGrada = grad == "" ? 1 : Sistem.IDGrada(grad);
                    ak.Detalji = napomena;
                    ak.IDRedarstva = idRedarstva;

                    db.AKCIJE_DJELATNIKAs.InsertOnSubmit(ak);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Spremi Akciju");
            }
        }

        public static List<_Akcija> DohvatiAkcije(string grad, int? idDjelatnika, int? idAkcije, DateTime? odDatum, DateTime? doDatum, int idprivilegije, int? idRedarstva, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var akc = from a in db.AKCIJE_DJELATNIKAs
                              join k in db.AKCIJEs on a.IDAkcije equals k.IDAkcije
                              where a.IDGrada == Sistem.IDGrada(grad) &&
                                    (idDjelatnika != null ? a.IDDjelatnika == idDjelatnika : idDjelatnika == null) &&
                                    (idAkcije != null ? a.IDAkcije == idAkcije : idAkcije == null) &&
                                    (idRedarstva != null ? a.IDRedarstva == idRedarstva : idRedarstva == null) &&
                                    (odDatum != null ? a.Datum.Date >= odDatum : odDatum == null) &&
                                    (doDatum != null ? a.Datum.Date <= doDatum : doDatum == null)
                              orderby a.Datum
                              select new { a, k };

                    List<_Akcija> nova = new List<_Akcija>();

                    using (PazigradDataContext pb = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                    {
                        foreach (var q in akc)
                        {
                            if (q.a.IDDjelatnika == -1)
                            {
                                nova.Add(new _Akcija(q.a.IDAkcijeKorisnika, q.a.IDAkcije, q.a.IDDjelatnika, q.a.Datum.Date, q.a.Datum, q.a.Detalji, q.k.NazivAkcije, "Automatska akcija"));
                                continue;
                            }

                            if (idprivilegije == 1 && db.KORISNICIs.Any(i => i.IDKorisnika == q.a.IDDjelatnika))
                            {
                                KORISNICI kor = db.KORISNICIs.First(i => i.IDKorisnika == q.a.IDDjelatnika);

                                if (kor != null)
                                {
                                    nova.Add(new _Akcija(q.a.IDAkcijeKorisnika, q.a.IDAkcije, q.a.IDDjelatnika, q.a.Datum.Date, q.a.Datum, q.a.Detalji,
                                        q.k.NazivAkcije, kor.ImePrezime));
                                    continue;
                                }
                            }

                            Djelatnik djel = pb.Djelatniks.FirstOrDefault(i => i.IDDjelatnika == q.a.IDDjelatnika);

                            if (djel == null)
                            {
                                nova.Add(new _Akcija(q.a.IDAkcijeKorisnika, q.a.IDAkcije, q.a.IDDjelatnika, q.a.Datum.Date, q.a.Datum, q.a.Detalji, q.k.NazivAkcije, ""));
                                continue;
                            }

                            nova.Add(new _Akcija(q.a.IDAkcijeKorisnika, q.a.IDAkcije, q.a.IDDjelatnika, q.a.Datum.Date, q.a.Datum, q.a.Detalji, q.k.NazivAkcije, djel.ImePrezime));
                        }
                    }

                    return nova;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "DOHVATI AKCIJE KORISNIKA");
                return new List<_Akcija>();
            }
        }

        public static List<_2DLista> DohvatiVrsteAkcija(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    List<_2DLista> Nova = new List<_2DLista>();

                    Nova.Add(new _2DLista(0, "..."));
                    Nova.AddRange(db.AKCIJEs.Select(a => new _2DLista(a.IDAkcije, a.NazivAkcije)));

                    return Nova.OrderBy(i => i.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Dohvati Vrste Akcije");
                return new List<_2DLista>();
            }
        }

        public static bool ObrisiAkciju(string grad, int idAkcije, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    db.AKCIJE_DJELATNIKAs.DeleteOnSubmit(db.AKCIJE_DJELATNIKAs.First(i => i.IDAkcijeKorisnika == idAkcije));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Obrisi Akciju");
                return false;
            }
        }

        #endregion

        /*:: ADMINISTRACIJA ULICA ::*/

        #region ADMINISTRACIJA ULICA

        public static List<_LokalneAdrese> PopisUlica(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List<_LokalneAdrese> nova = new List<_LokalneAdrese>();

                    foreach (var q in db.LokalneAdreses.OrderBy(i => i.Ulica).GroupBy(i => i.Ulica))
                    {
                        List<_DetaljiLokalneAdrese> det = new List<_DetaljiLokalneAdrese>();

                        foreach (var adr in q)
                        {
                            det.Add(new _DetaljiLokalneAdrese(adr.ID, adr.Lat, adr.Long, adr.Kbr));
                        }

                        nova.Add(new _LokalneAdrese(q.Key, q.Count(), det));
                    }

                    return nova;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Popis Ulica");
                return null;
            }
        }

        public static bool ObrisiLokalnuAdresu(string grad, int idAdrese, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.LokalneAdreses.DeleteOnSubmit(db.LokalneAdreses.First(i => i.ID == idAdrese));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Obrisi Lokalnu Adresu");
                return false;
            }
        }

        public static bool ObrisiGrupuUlica(string grad, string ulica, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.LokalneAdreses.DeleteAllOnSubmit(db.LokalneAdreses.Where(i => i.Ulica == ulica));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Obrisi sve Lokalne Adrese");
                return false;
            }
        }

        public static bool IzmjeniLokalnuAdresu(string grad, int idAdrese, string adresa, string kbr, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    LokalneAdrese la = db.LokalneAdreses.First(i => i.ID == idAdrese);

                    la.Ulica = adresa;
                    la.Kbr = kbr;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Izmjeni Lokalnu Adresu");
                return false;
            }
        }

        public static bool IzmjeniGrupuUlica(string grad, string ulica, string novaulica, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    foreach (var ul in db.LokalneAdreses.Where(i => i.Ulica == ulica))
                    {
                        ul.Ulica = novaulica;
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "IZMJENI GRUPU ULICA");
                return false;
            }
        }

        public static bool DodajLokalnuAdresu(string grad, string adresa, string broj, decimal? hdop, decimal lat, decimal lng, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    LokalneAdrese la = new LokalneAdrese();

                    la.Ulica = adresa;
                    la.Kbr = broj;
                    la.HDOP = hdop;
                    la.Lat = lat;
                    la.Long = lng;

                    db.LokalneAdreses.InsertOnSubmit(la);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "DODAJ Lokalnu Adresu");
                return false;
            }
        }

        #endregion

        /*:: KAMERE ::*/

        #region WEB KAMERE

        public static bool ImaKamere(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.PozicijeKameras.Any(i => i.Prikazi);
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Obrisi Kameru");
                return true;
            }
        }

        public static int DodajKameru(string grad, _WebKamere kamera, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PozicijeKamera pk = new PozicijeKamera();

                    int id = 1;

                    if (db.PozicijeKameras.Any())
                    {
                        id = db.PozicijeKameras.Max(i => i.IDPozicije) + 1;
                    }

                    pk.IDPozicije = id;
                    pk.NazivKamere = kamera.NazivKamere;
                    pk.LatKamere = kamera.LatKamere;
                    pk.LonKamere = kamera.LngKamere;
                    pk.WebAdresa = kamera.AdresaKamere;
                    pk.OpisKamere = kamera.OpisKamere;
                    pk.Prikazi = true;

                    db.PozicijeKameras.InsertOnSubmit(pk);
                    db.SubmitChanges();

                    return pk.IDPozicije;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Dodaj Kameru");
                return -1;
            }
        }

        public static bool IzmijeniKameru(string grad, _WebKamere kamera, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PozicijeKamera pk = db.PozicijeKameras.First(i => i.IDPozicije == kamera.IDKamere);
                    pk.NazivKamere = kamera.NazivKamere;
                    pk.LatKamere = kamera.LatKamere;
                    pk.LonKamere = kamera.LngKamere;
                    pk.WebAdresa = kamera.AdresaKamere;
                    pk.OpisKamere = kamera.OpisKamere;
                    pk.Prikazi = kamera.PrikaziKamere;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Izmijeni Kameru");
                return true;
            }
        }

        public static bool ObrisiKameru(string grad, int idKamere, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.PozicijeKameras.DeleteOnSubmit(db.PozicijeKameras.First(i => i.IDPozicije == idKamere));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Obrisi Kameru");
                return true;
            }
        }

        public static List<_WebKamere> DohvatiKamere(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var kam = from k in db.PozicijeKameras
                              select new _WebKamere(k.IDPozicije, k.NazivKamere, k.LatKamere, k.LonKamere,
                                                 k.WebAdresa, k.OpisKamere, k.Prikazi);
                    return kam.ToList();
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "Dohvati Kamere");
                return new List<_WebKamere>();
            }
        }

        #endregion

        /*:: DRZAVE ::*/

        public static List<_Drzava> Drzave(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var drz = from d in db.DRZAVAs
                              orderby d.Naziv
                              select new _Drzava(d.Naziv, d.Kratica, d.ISO_Kratica);

                    return drz.ToList();
                }
            }
            catch (Exception ex)
            {
                SpremiGresku("", ex, idAplikacije, "drzave");
                return new List<_Drzava>();
            }
        }

        public static bool DodajDrzavu(string kratica, string naziv, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (db.DRZAVAs.Any(i => i.Kratica == kratica))
                    {
                        return false;
                    }

                    DRZAVA dr = new DRZAVA();

                    dr.Kratica = kratica;
                    dr.Naziv = naziv;

                    db.DRZAVAs.InsertOnSubmit(dr);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku("", ex, idAplikacije, "dodaj drzavu");
                return false;
            }
        }

        /*:: POSTE ::*/

        public static List<_Posta> Poste(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var drz = from d in db.POSTEs
                              select new _Posta(d.IDPoste, d.Posta, d.Mjesto);

                    return drz.ToList();
                }
            }
            catch (Exception ex)
            {
                SpremiGresku("", ex, idAplikacije, "POSTE");
                return new List<_Posta>();
            }
        }

        /*:: KONTAKTIRANJE KORISNIKA ::*/

        public static bool DodajPredlozak(string grad, string naziv, string predlozak, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    PREDLOSCI_EMAILOVA pe = new PREDLOSCI_EMAILOVA();

                    int id = 1;

                    if (db.PREDLOSCI_EMAILOVAs.Any())
                    {
                        id = db.PREDLOSCI_EMAILOVAs.Max(i => i.IDPredloska) + 1;
                    }

                    pe.IDPredloska = id;
                    pe.NazivPredloska = naziv;
                    pe.Predlozak = predlozak;

                    db.PREDLOSCI_EMAILOVAs.InsertOnSubmit(pe);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "DODAJ PREDLOZAK");
                return false;
            }
        }

        public static List<_3DLista> DohvatiPredloskeEmaila(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.PREDLOSCI_EMAILOVAs.Select(i => new _3DLista(i.IDPredloska, i.NazivPredloska, i.Predlozak)).ToList();
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "DOHVATI PREDLOSKE");
                return new List<_3DLista>();
            }
        }

        public static _EmailPostavke PostavkeEmaila(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    POSTAVKE_EMAILA po = db.POSTAVKE_EMAILAs.First();
                    GRADOVI g = db.GRADOVIs.First(i => i.Baza == grad);

                    return new _EmailPostavke(po.Timeout, po.Host, po.DefaultCredentials, po.EnableSsl, po.Port, po.UserName, po.Lozinka, "support@pazigrad.com  ", po.Naziv, g.Grb, "http://www.pazigrad.net");
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "POSTAVKE EMAILA");
                return null;
            }
        }

        public static bool ObrisiPredlozakEmaila(string grad, int idPredloska, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    db.PREDLOSCI_EMAILOVAs.DeleteOnSubmit(db.PREDLOSCI_EMAILOVAs.First(i => i.IDPredloska == idPredloska));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "OBRISI PREDLOZAK");
                return false;
            }
        }

        /*:: POSTAVKE ::*/

        public static bool PostavkePauka(string grad, bool dof, bool naplata, bool lisice, bool prijave, bool prilog, bool mup, string odlukaLisice, bool hub, string zalbaPrometnog, int danaLezarina, DateTime? datum, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    GRADOVI g = db.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad));

                    g.NaplataPauk = datum;
                    g.DOF = dof;
                    g.Lisice = lisice;
                    g.VanjskePrijave = prijave;
                    g.OdlukaLisice = odlukaLisice;
                    g.PrilogObavijest = prilog;
                    g.DohvatVlasnikaMUP = mup;
                    g.RacunHUB = hub;
                    g.ZalbaRedarstva = zalbaPrometnog;
                    g.NakonDanaLezarina = danaLezarina;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "POSTAVKE PAUKA JLS");
                return false;
            }
        }

        /*:: POZIV*/

        public static Tuple<string, string> ProvjeraPoziva(string grad, string poziv, decimal kazna, int idRedarstva, int idAplikacije)
        {
            try
            {
                //_Uplatnica u = Gradovi.Uplatnica(grad, idRedarstva, idAplikacije);
                //_Zakon z = Zakoni.DohvatiZakonS(grad, idOpisa, false, idAplikacije);
                List<_Iznos> iznosi = Gradovi.Iznosi(idAplikacije);

                if (idRedarstva == 4)
                {
                    using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                    {
                        if (db.RACUNIs.Any(i => i.PozivNaBroj == poziv && i.IDRedarstva == idRedarstva))
                        {
                            return new Tuple<string, string>("Uplatnica je iskorištena!", "");
                        }
                    }

                    string[] p = poziv.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                    if (p.Length == 1)
                    {
                        //nema crtice pa ju dodajem
                        if (poziv.Length == 16)
                        {
                            poziv = poziv.Insert(7, "-");
                        }
                        else if (poziv.Length == 15)
                        {
                            poziv = poziv.Insert(6, "-");
                        }
                        else 
                        {
                            return new Tuple<string, string>("Pogrešno unesen poziv na broj!", ""); //bolje definirati tekst
                        }

                        return ProvjeraPoziva(grad, poziv, kazna, idRedarstva, idAplikacije);
                    }

                    int idIznosa = Convert.ToInt32(p.ElementAt(0).Substring(0, 2));
                    string jls = p.ElementAt(0).Substring(2, 3);
                    string redarstvo = p.ElementAt(0).Substring(5,1); //tu treba uzeti samo jedan znak jer u slučaju duljine 16 slijedeći je kontrolni broj 

                    //string godina = p.ElementAt(1).Substring(0,1);

                    if (jls != Sistem.IDGrada(grad).ToString("000"))
                    {
                        return new Tuple<string, string>("Očitana je uplatnica za pogrešan grad!", "");//bolje definirati tekst
                    }

                    if (redarstvo != idRedarstva.ToString())
                    {
                        return new Tuple<string, string>("Očitana je uplatnica za pogrešnu redarstvenu službu!", ""); //bolje definirati tekst
                    }

                    //ako je uplatnica bez iznosa
                    if (idIznosa != 0)
                    {
                        if (iznosi.First(i => i.IDIznosa == idIznosa).Iznos != kazna)
                        {
                            return new Tuple<string, string>("Očitani iznos uplatnice ne odgovara iznosu kazne!", ""); //bolje definirati tekst
                        }
                    }

                    return new Tuple<string, string>("", poziv);
                }

                return new Tuple<string, string>("KONTROLA NIJE PROŠLA", ""); //todo 
                //if (poziv.StartsWith("030") || poziv.StartsWith("050") || poziv.StartsWith("070"))
                //{

                //}
                //else
                //{

                //}
            }
            catch (Exception ex)
            {
                SpremiGresku(grad, ex, idAplikacije, "PROVJERA POZIVA: " + poziv);
                return new Tuple<string, string>(ex.Message, "");
            }
        }

    }
}