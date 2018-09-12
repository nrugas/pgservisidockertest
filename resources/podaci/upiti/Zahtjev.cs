using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Zahtjev
    {
        /* -2 odustao od klasifikacije
         * -1 neaktivan
         *  0 aktivan
         *  1 preuzet
         *  2 odbijen
         *  3 prihvacen
         *  4 storniran
         *  5 izdao nalog pauku
         */

        public static int Neobradjeni(string grad, int idDjelatnika, int zadrska, out _PrijavaPauk prijava, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    prijava = null;

                    if (idDjelatnika == 0)
                    {
                        return db.Zahtjevis.Count(i => i.IDStatusa == 0 && i.DatumVrijeme.Date == DateTime.Today.Date);
                    }

                    var pri = from p in db.Zahtjevis
                              where p.IDStatusa == 0 && p.DatumVrijeme.Date == DateTime.Today.Date
                              orderby p.DatumVrijeme descending
                              select new _PrijavaPauk(
                                  p.IDPrijave,
                                  p.IDLokacije,
                                  p.IDPrijavitelja,
                                  p.IDOdobravatelja,
                                  p.DatumVrijeme,
                                  p.Registracija,
                                  p.Lat,
                                  p.Lng,
                                  p.Adresa,
                                  p.KraticaDrzave,
                                  p.IDStatusa,
                                  p.IDOpisa,
                                  db.SlikaPrekrsajas.Where(i => i.IDLokacije == p.IDLokacije).Select(i => new _Slika(i.IDSlikePrekrsaja, idAplikacije == 1 ? null : i.Slika.ToArray())).ToList());

                    //if (!pri.Any())
                    //{
                    //    //provjerava da li je neki zahtjev ostao visiti ako je vraća ga u obradu
                    //    new Thread(() => Preuzeti(grad, idDjelatnika, idAplikacije)).Start();
                    //}

                    int x = 0;
                    foreach (var q in pri)
                    {
                        if (db.ZahtjeviAkcijes.Any(i => i.IDZahtjeva == q.IDPrijave && i.IDAkcije == -2 && i.IDDjelatnika == idDjelatnika))
                        {
                            x++;
                            continue;
                        }

                        //ako je zadrska razlicito od nula, sustav ceka određeno virjeme prije nego dodijeli zahtjev redarima na terenu
                        if (zadrska != 0)
                        {
                            if (DateTime.Now.Subtract(q.DatumVrijeme).TotalSeconds < zadrska)
                            {
                                continue;
                            }
                        }

                        prijava = q;
                        break;
                    }

                    return pri.Count() - x;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NEOBRAĐENI ZAHTJEVI");
                prijava = null;
                return -1;
            }
        }

        public static bool Preuzmi(string grad, int idZahtjeva, int idDjelatnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Zahtjevi pri = db.Zahtjevis.First(i => i.IDPrijave == idZahtjeva);

                    //ako je vec netko preuzeo
                    if (pri.IDStatusa != 0)
                    {
                        return false;
                    }

                    pri.IDOdobravatelja = idDjelatnika;
                    pri.IDStatusa = 1;
                    db.SubmitChanges();

                    SpremiAkcijuZahtjeva(grad, idZahtjeva, idDjelatnika, 1, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PREUZMI ZAHTJEV");
                return false;
            }
        }

        public static void Odustani(string grad, int idZahtjeva, int idDjelatnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Zahtjevi pri = db.Zahtjevis.First(i => i.IDPrijave == idZahtjeva);
                    pri.IDOdobravatelja = null;
                    pri.IDStatusa = 0;
                    db.SubmitChanges();

                    SpremiAkcijuZahtjeva(grad, idZahtjeva, idDjelatnika, -2, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ODUSTANI ZAHTJEV");
            }
        }

        public static int BrojSlikaZahtjeva(string grad, int idLokacije, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.SlikaPrekrsajas.Count(i => i.IDLokacije == idLokacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "BR SLIKA ZAHTJEV");
                return 0;
            }
        }

        public static bool StatusZahtjeva(string grad, int idZahtjeva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Zahtjevi pri = db.Zahtjevis.First(i => i.IDPrijave == idZahtjeva);

                    if (pri.IDStatusa != 0)
                    {
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "STATUS ZAHTJEVA");
                return false;
            }
        }

        public static bool StatusZahtjevaPaukOdustao(string grad, int idZahtjeva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.Zahtjevis.First(i => i.IDPrijave == idZahtjeva).IDStatusa == 4)
                    {
                        //storniran od strane pauka
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "STATUS ZAHTJEVA PAUK ODUSTAO");
                return false;
            }
        }

        public static void SpremiAkcijuZahtjeva(string grad, int idZahtjeva, int idDjelatnika, int idAkcije, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    ZahtjeviAkcije za = new ZahtjeviAkcije();

                    za.IDZahtjeva = idZahtjeva;
                    za.IDDjelatnika = idDjelatnika;
                    za.IDAkcije = idAkcije;

                    db.ZahtjeviAkcijes.InsertOnSubmit(za);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PREUZMI ZAHTJEV");
            }
        }

        public static int DodajPrekrsaj(string grad, _PrijavaPauk zahtjev, int idOpisa, decimal kazna, string registracija, string adresa, string drzava, bool kaznjava, bool nalogPauku, bool lisice, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.Zahtjevis.First(i => i.IDPrijave == zahtjev.IDPrijave).IDStatusa == 4)
                    {
                        //storniran od strane pauka
                        return -2;
                    }

                    if (db.Prekrsajis.Any(i => i.IDLokacije == zahtjev.IDLokacije && i.Test == false))
                    {
                        return db.Prekrsajis.First(i => i.IDLokacije == zahtjev.IDLokacije).IDPrekrsaja;
                    }

                    string brojUp = Prekrsaj.GenerirajPozivNaBroj(grad, kaznjava, zahtjev.DatumVrijeme, kazna, idAplikacije);

                    //int idPredloska = obavijest ? Prekrsaj.IDPredloskaObavijesti(grad, idAplikacije) : Prekrsaj.IDPredloskaUpozorenja(grad, idAplikacije); //todo ovisno o redarstvu i kaznjava (obavijest) i pauk

                    int idPredloska = db.PredlosciIspisas.First(i => i.IDRedarstva == idRedarstva && i.Pauk && i.Kaznjava == kaznjava).IDPRedloska;

                    Zahtjevi z = db.Zahtjevis.First(i => i.IDPrijave == zahtjev.IDPrijave);

                    #region PREKRSAJ

                    Prekrsaji prek = new Prekrsaji();

                    prek.IDLokacije = zahtjev.IDLokacije;
                    prek.IDSkracenogOpisa = idOpisa;
                    prek.IDDjelatnika = zahtjev.IDOdobravatelja;
                    prek.Vrijeme = zahtjev.DatumVrijeme;
                    prek.RegistracijskaPlocica = registracija.Replace(" ", "").Replace("-", "").ToUpper();
                    prek.BrojUpozorenja = brojUp;
                    prek.Lat = zahtjev.Lat;
                    prek.Long = zahtjev.Lng;
                    prek.IDNacinaPozicioniranja = 3;
                    prek.Adresa = adresa;
                    prek.Kazna = kazna;
                    prek.IDPredloskaIspisa = idPredloska;
                    prek.PozivNaBroj = brojUp;
                    prek.Test = false; 
                    prek.Poslano = false;
                    prek.Status = false;
                    prek.Napomena = null;
                    prek.Tekst = null;
                    prek.GMapsUlica = null;
                    prek.NalogPauka = nalogPauku;
                    prek.IDNaloga = 0;
                    prek.Zakljucan = false;
                    prek.IDRacuna = null;
                    //todo obrisi
                    //prek.IDRedarNaplate = null;
                    //prek.IDVrstaPlacanja = null;
                    //prek.Placeno = false;
                    //prek.DatumPlacanja = null;
                    //prek.PlacanjePreneseno = false;
                    prek.KraticaDrzave = drzava;
                    prek.StatusOcitanja = z.TipOcitanja;
                    prek.Ocitanja = z.Ocitanja;
                    prek.TrajanjePostupka = z.Trajanje;
                    prek.Rucno = true;
                    prek.Zahtjev = true;
                    prek.IDRedarstva = idRedarstva;
                    prek.IDOpisaZakona = Zakoni.DohvatiIDNovogZakona(grad, prek.IDSkracenogOpisa, idAplikacije);

                    db.Prekrsajis.InsertOnSubmit(prek);
                    db.SubmitChanges();

                    #endregion

                    Lokacije lok = db.Lokacijes.First(i => i.IDLokacije == zahtjev.IDLokacije);
                    lok.IDDjelatnika = zahtjev.IDOdobravatelja;
                    db.SubmitChanges();

                    //todo - trenutno je samo za lokacije, kad zavrsi testno razdoblje to makni
                    if (grad == "Lokacije")
                    {
                        if (prek.IDPredloskaIspisa == 15 || prek.IDPredloskaIspisa == 2)
                        {
                            Vpp._VppPrijenos prekrsaj = new Vpp._VppPrijenos(prek.IDPrekrsaja, 1, prek.Kazna, prek.PozivNaBroj, "Obavijest");
                            bool ok = Vpp.DodajVPP(grad, prekrsaj, idAplikacije);

                            if (ok)
                            {
                                prek.Poslano = true;
                                db.SubmitChanges();
                            }
                        }
                    }

                    if (nalogPauku)
                    {
                        bool ok = KreirajNalog(grad, prek.IDPrekrsaja, zahtjev.IDLokacije, zahtjev, lisice, idRedarstva, idAplikacije);

                        if (!ok)
                        {
                            prek.Test = true;
                            db.SubmitChanges();

                            return -1;
                        }
                    }
                    else
                    {
                        //todo ako nije nalog pauku razlikovati odobreno jer ce drasku doci odobreno ali bez naloga pauku!
                        Zatvori(grad, zahtjev.IDPrijave, 3, zahtjev.IDOdobravatelja.Value, null, prek.IDPrekrsaja, "ODOBRENO", idRedarstva, idAplikacije);
                    }

                    if (kaznjava)
                    {
                        new Thread(() => RentaCar.PostojiRCVozilo(grad, prek.RegistracijskaPlocica, prek.IDLokacije, 0, idAplikacije)).Start();
                    }

                    return prek.IDPrekrsaja;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ PREKRŠAJ");
                return -1;
            }
        }

        public static bool KreirajNalog(string grad, int idPrekrsaja, int idLokacije, _PrijavaPauk zahtjev, bool lisice, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int idNaloga = 1;

                    if (db.NaloziPaukus.Any())
                    {
                        idNaloga = db.NaloziPaukus.Max(i => i.IDNaloga) + 1;
                    }

                    bool ok = DodajNalog(grad, idPrekrsaja, idNaloga, zahtjev, lisice, idAplikacije);

                    if (ok)
                    {
                        int idOdobravatelja = 0;
                        if (zahtjev.IDOdobravatelja != null)
                        {
                            idOdobravatelja = zahtjev.IDOdobravatelja.Value;
                        }

                        SpremiAkcijuZahtjeva(grad, zahtjev.IDPrijave, idOdobravatelja, 5, idAplikacije);
                        Zatvori(grad, zahtjev.IDPrijave, 3, idOdobravatelja, idNaloga, idPrekrsaja, "ODOBRENO", idRedarstva, idAplikacije);

                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ NALOG");
                return false;
            }
        }

        public static bool DodajNalog(string grad, int idPrekrsaja, int idNaloga, _PrijavaPauk zahtjev, bool lisice, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VozilaPauka vp = db.VozilaPaukas.First(i => i.IDVozila == zahtjev.IDPrijavitelja);

                    Prekrsaji pre = db.Prekrsajis.First(i => i.IDPrekrsaja == idPrekrsaja);
                    pre.IDNaloga = idNaloga;
                    pre.NalogPauka = true;
                    db.SubmitChanges();

                    NaloziPauku nal = new NaloziPauku();
                    nal.IDNaloga = idNaloga;
                    
                    //todo - ako obradjuje lisice / obradjuje nalog
                    if (vp != null)
                    {
                        if (vp.ObradjujeNalog)
                        {
                            nal.IDStatusa = 8;
                            nal.IDVozila = zahtjev.IDPrijavitelja;
                            nal.Redoslijed = 1;
                        }
                        else
                        {
                            nal.IDStatusa = 0;
                            nal.IDVozila = null;
                            nal.Redoslijed = 0;
                        }
                    }
                    else
                    {
                        nal.IDStatusa = 0;
                        nal.IDVozila = null;
                        nal.Redoslijed = 0;
                    }

                    nal.NalogZatvoren = false;
                    nal.DatumNaloga = zahtjev.DatumVrijeme;
                    nal.StornoRedara = false;
                    nal.IDRazloga = 0;
                    nal.Lisice = lisice;

                    db.NaloziPaukus.InsertOnSubmit(nal);
                    db.SubmitChanges();

                    Pauk pau = new Pauk();
                    pau.IDNaloga = idNaloga;
                    if (vp != null)
                    {
                        pau.Status = vp.ObradjujeNalog ? 8 : 0;
                    }
                    else
                    {
                        pau.Status = 0;
                    }
                    pau.NalogZatvoren = false;
                    pau.DatumNaloga = zahtjev.DatumVrijeme;
                    pau.StornoRedara = false;

                    db.Pauks.InsertOnSubmit(pau);
                    db.SubmitChanges();

                    if (vp.ObradjujeNalog)
                    {
                        Redoslijed(grad, zahtjev.IDPrijavitelja.Value, idNaloga, idAplikacije);
                    }

                    Nalog.SpremiPovijest(grad, idNaloga, zahtjev.IDPrijavitelja, 0, true, idAplikacije); //novi

                    if (vp.ObradjujeNalog)
                    {
                        Nalog.SpremiPovijest(grad, idNaloga, zahtjev.IDPrijavitelja, 8, true, idAplikacije); //dodijeljen
                    }

                    new Thread(() => MailLista.PosaljiNaredbu(grad, idNaloga, idAplikacije)).Start();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ NALOG I DODIJELI");
                return false;
            }
        }

        public static bool Zatvori(string grad, int idZahtjeva, int idStatusa, int idDjelatnika, int? idNaloga, int? idPrekrsaja, string razlog, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Zahtjevi pri = db.Zahtjevis.First(i => i.IDPrijave == idZahtjeva);

                    pri.IDStatusa = idStatusa;
                    pri.IDOdobravatelja = idDjelatnika;
                    pri.Poruka = razlog;
                    pri.IDNaloga = idNaloga;
                    pri.IDPrekrsaja = idPrekrsaja;
                    pri.IDAplikacije = idAplikacije;
                    pri.IDRedarstva = idRedarstva;

                    db.SubmitChanges();

                    SpremiAkcijuZahtjeva(grad, idZahtjeva, idDjelatnika, idStatusa, idAplikacije);

                    if (idStatusa == 3) //todo and nalog razlicit od null else dodan prekrsaja
                    {
                        Nalog.SpremiPovijest(grad, pri.IDPrijave, -1, 18, false, idAplikacije);
                    }

                    if (idStatusa == 2)
                    {
                        Nalog.SpremiPovijest(grad, pri.IDPrijave, -1, 19, false, idAplikacije);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ZATVORI PRIJAVU");
                return false;
            }
        }

        public static bool AktivniKorisnik(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    DateTime dat = DateTime.Now.Subtract(new TimeSpan(0, 0, 2, 0));

                    var ap = from a in db.AKTIVNE_APLIKACIJEs
                             where a.IDGrada == Sistem.IDGrada(grad) &&
                                   //(a.IDAplikacije == 1 || a.IDAplikacije == 6) &&
                                   a.ZadnjaAktivnost >= dat &&
                                   a.OdobravaZahtjeve
                             select a;

                    if (ap.Any())
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "AKTIVNI KORISNIK MUP");
                return false;
            }
        }

        public static List<_ZahtjevPauka> Zahtjevi(string grad, int idVozila, int idStatusa, DateTime? datumOd, DateTime? datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var zah = from z in db.Zahtjevis
                              join v in db.VozilaPaukas on z.IDPrijavitelja equals v.IDVozila into voz
                              from vv in voz.DefaultIfEmpty()
                              join d in db.Djelatniks on z.IDOdobravatelja equals d.IDDjelatnika into djel
                              from dd in djel.DefaultIfEmpty()
                              join k in db.Djelatniks on z.IDPrijaviteljaDjelatnik equals k.IDDjelatnika into djelPrij
                              from kk in djelPrij.DefaultIfEmpty()
                              join o in db.OpisiPrekrsajas on z.IDOpisa equals o.IDOpisa into opis
                              from oo in opis.DefaultIfEmpty()
                              where (idVozila != 0 ? z.IDPrijavitelja == idVozila : idVozila == 0) &&
                                    (idStatusa != -3 ? z.IDStatusa == idStatusa : idStatusa == -3) &&
                                    (idRedarstva != -1 ? z.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                    (datumOd != null ? z.DatumVrijeme.Date >= datumOd : datumOd == null) &&
                                    (datumDo != null ? z.DatumVrijeme.Date <= datumDo : datumDo == null)
                              select
                            new _ZahtjevPauka(z.IDPrijave, z.IDLokacije, z.IDPrijaviteljaDjelatnik, kk.ImePrezime, z.IDNaloga, z.IDPrijavitelja, vv.NazivVozila ?? "ME", z.IDOdobravatelja,
                            dd.ImePrezime, z.IDStatusa, Status(z.IDStatusa), z.IDOpisa, oo.OpisPrekrsaja, z.DatumVrijeme, z.Registracija, z.Adresa, z.Poruka == "ODOBRENO" ? "" : z.Poruka.Replace("\r\n", ""),
                            Trajanje(grad, z.IDPrijave, idAplikacije), z.IDAplikacije, Postavke.Aplikacija(z.IDAplikacije));

                    return zah.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ZAHTJEVI");
                return new List<_ZahtjevPauka>();
            }
        }

        public static _ZahtjevPauka DohvatiZahtjev(string grad, int idZahtjeva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var zah = from z in db.Zahtjevis
                              join v in db.VozilaPaukas on z.IDPrijavitelja equals v.IDVozila into voz
                              from vv in voz.DefaultIfEmpty()
                              join d in db.Djelatniks on z.IDOdobravatelja equals d.IDDjelatnika into djel
                              from dd in djel.DefaultIfEmpty()
                              join k in db.Djelatniks on z.IDPrijaviteljaDjelatnik equals k.IDDjelatnika into djelPrij
                              from kk in djelPrij.DefaultIfEmpty()
                              join o in db.OpisiPrekrsajas on z.IDOpisa equals o.IDOpisa into opis
                              from oo in opis.DefaultIfEmpty()
                              where z.IDPrijave == idZahtjeva
                              select
                            new _ZahtjevPauka(z.IDPrijave, z.IDLokacije, z.IDPrijaviteljaDjelatnik, kk.ImePrezime, z.IDNaloga, z.IDPrijavitelja, vv.NazivVozila ?? "ME", z.IDOdobravatelja,
                            dd.ImePrezime, z.IDStatusa, Status(z.IDStatusa), z.IDOpisa, oo.OpisPrekrsaja, z.DatumVrijeme, z.Registracija, z.Adresa, z.Poruka == "ODOBRENO" ? "" : z.Poruka.Replace("\r\n", ""),
                            Trajanje(grad, z.IDPrijave, idAplikacije), z.IDAplikacije, Postavke.Aplikacija(z.IDAplikacije));

                    if (!zah.Any())
                    {
                        return null;
                    }

                    return zah.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ZAHTJEV");
                return null;
            }
        }

        public string IspisZahtjeva(string grad, int idLokacije, string broj, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Prekrsaji p = db.Prekrsajis.First(i => i.IDLokacije == idLokacije);

                    if (!string.IsNullOrEmpty(broj))
                    {
                        p.BrojUpozorenja = broj;
                        p.PozivNaBroj = broj;

                        db.SubmitChanges();
                    }

                    string ispis;
                    Ispis.IspisPredloska(grad, ObavijestOPrekrsaju(Prekrsaj.DetaljiPrekrsaja(grad, idLokacije, idAplikacije)), 1, p.IDPredloskaIspisa.Value, 0, out ispis, string.IsNullOrEmpty(broj), idAplikacije);

                    return ispis;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ZAHTJEVI");
                return null;
            }
        }

        private static string ObavijestOPrekrsaju(_Prekrsaj prekrsaj)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("<Elements>");
            sb.Append("<DanasnjiDatum>" + prekrsaj.DatumVrijeme + "</DanasnjiDatum>");
            sb.Append("<BrojUpozorenja>" + prekrsaj.BrojDokumenta + "</BrojUpozorenja>");
            sb.Append("<Datum>" + prekrsaj.DatumVrijeme.ToString("dd.MM.yy") + "</Datum>");
            sb.Append("<Vrijeme>" + prekrsaj.DatumVrijeme.ToString("HH:mm") + "</Vrijeme>");
            sb.Append("<Registracija>" + prekrsaj.Registracija + "</Registracija>");
            sb.Append("<Ulica>" + prekrsaj.Adresa + "</Ulica>");
            sb.Append("<KucniBroj></KucniBroj>");
            sb.Append("<Clanak>" + prekrsaj.ClanakPrekrsaja + "</Clanak>");
            sb.Append("<Kazna>" + prekrsaj.Kazna + "</Kazna>");
            sb.Append("<Djelatnik>" + prekrsaj.Redar + "</Djelatnik>");
            sb.Append("<Prekrsaj>" + prekrsaj.OpisPrekrsaja + "</Prekrsaj>");
            sb.Append("</Elements>");

            return sb.ToString();
        }

        private static void Redoslijed(string grad, int idVozila, int idNaloga, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    foreach (var n in db.NaloziPaukus.Where(i => i.DatumNaloga.Date == DateTime.Today && i.IDVozila == idVozila && i.NalogZatvoren == false))
                    {
                        if (n.IDNaloga == idNaloga)
                        {
                            continue;
                        }

                        n.Redoslijed = n.Redoslijed + 1;
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "REDOSLIJED");
            }
        }

        /**/

        public static TimeSpan Trajanje(string grad, int idZahtjeva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var st = from s in db.PovijestNalogas
                             where s.IDNaloga == idZahtjeva &&
                                   s.Nalog == false
                             orderby s.DatumVrijemeDogadjaja ascending
                             select s;

                    return st.Max(i => i.DatumVrijemeDogadjaja).Subtract(st.Min(i => i.DatumVrijemeDogadjaja));
                }
            }
            catch (Exception)
            {
                return new TimeSpan(0, 0, 0, 0);
            }
        }

        public static string Status(int id)
        {
            switch (id)
            {
                case -2:
                    return "Odustao od klasifikacije";
                case -1:
                    return "Nije aktiviran";//"Nema aktivnih odobravatelja";
                case 0:
                    return "Novi zahtjev";
                case 1:
                    return "Preuzet na obradu";
                case 2:
                    return "Odbijen";
                case 3:
                    return "Odobren";
                case 4:
                    return "Odustao od zahtjeva";
            }

            return "";
        }

        /*:: PONOVI ::*/

        //public static void Preuzeti(string grad, int idDjelatnika, int idAplikacije)
        //{
        //    try
        //    {
        //        return; //todo
        //        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
        //        {
        //            DateTime dat = DateTime.Now.Subtract(new TimeSpan(0, 0, 4, 0));

        //            foreach (var u in db.Zahtjevis.Where(i => i.IDStatusa == 1 && i.DatumVrijeme.Date == DateTime.Today.Date))
        //            {
        //                bool aktivan;

        //                using (PostavkeDataContext pb = new PostavkeDataContext())
        //                {
        //                    aktivan = pb.AKTIVNE_APLIKACIJEs.Any(a =>
        //                                        a.IDGrada == Sistem.IDGrada(grad) &&
        //                                        a.IDDjelatnika == u.IDOdobravatelja &&
        //                                        a.IDDjelatnika == idDjelatnika &&
        //                                        (a.IDAplikacije == 1 || a.IDAplikacije == 6) &&
        //                                        a.ZadnjaAktivnost >= dat);
        //                }

        //                if (!aktivan)
        //                {
        //                    Sustav.SpremiGresku(grad, new Exception(idDjelatnika.ToString()), idAplikacije, "AUTOMATSKO VRAĆANJE U OBRADU");
        //                    PonoviZahtjev(grad, u.IDPrijave, idAplikacije);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "AUTOMATSKO VRAĆANJE U OBRADU");
        //    }
        //}

        public static bool PonoviZahtjev(string grad, int idZahtjeva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.ZahtjeviAkcijes.DeleteAllOnSubmit(db.ZahtjeviAkcijes.Where(i => i.IDZahtjeva == idZahtjeva));
                    db.SubmitChanges();

                    Zahtjevi za = db.Zahtjevis.First(i => i.IDPrijave == idZahtjeva);

                    za.IDOdobravatelja = null;
                    za.IDStatusa = 0;
                    za.Poruka = "";
                    za.IDRedarstva = null; //todo mozda ne jer salje istom

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PONOVI ZAHTJEV");
                return false;
            }
        }
    }
}
