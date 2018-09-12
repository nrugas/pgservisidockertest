using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Mobile
    {
        /*:: PREKRŠAJ ::*/

        public static bool SpremiPrekrsajNovo(string grad, _Lokacija lokacija, ref _NoviPrekrsaj prekrsaj, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    string bu = prekrsaj.BrojUpozorenja;
                    if (db.Prekrsajis.Any(i => i.BrojUpozorenja == bu))
                    {
                        prekrsaj.IDPrekrsaja = db.Prekrsajis.First(i => i.BrojUpozorenja == bu).IDPrekrsaja;
                        prekrsaj.IDLokacije = db.Prekrsajis.First(i => i.BrojUpozorenja == bu).IDLokacije;
                        return true;
                    }

                    Lokacije lok = new Lokacije();

                    lok.Lat = lokacija.Latitude;
                    lok.Long = lokacija.Longitude;
                    lok.RegistracijskaPlocica = lokacija.Registracija;
                    lok.DatumVrijeme = lokacija.DatumVrijeme.ToLocalTime();
                    lok.IDDjelatnika = lokacija.IDDjelatnika;
                    lok.IDNacinaPozicioniranja = lokacija.IDPozicioniranja;
                    lok.IDTerminala = lokacija.IDTerminala;
                    lok.CellTowerID = lokacija.CellTowerID;
                    lok.SignalStrength = lokacija.SignalStrength;
                    lok.HDOP = lokacija.HDOP;
                    lok.Brzina = lokacija.Brzina;
                    lok.GPSAcc = lokacija.Preciznost;
                    lok.Battery = lokacija.Baterija;
                    lok.Punjac = lokacija.Punjac;

                    db.Lokacijes.InsertOnSubmit(lok);
                    db.SubmitChanges();

                    prekrsaj.IDLokacije = lok.IDLokacije;

                    int id = NoviPrekrsaj(grad, prekrsaj, idRedarstva, idAplikacije);

                    prekrsaj.IDPrekrsaja = id;

                    if (id != -1)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI LOKACIJU i PREKRSAJ");
                return false;
            }
        }

        public static int SpremiLokaciju(string grad, _Lokacija lokacija, bool pauk, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int id;

                    if (pauk)
                    {
                        LokacijePauka lok = new LokacijePauka();

                        lok.LatPauka = lokacija.Latitude;
                        lok.LongPauka = lokacija.Longitude;
                        lok.DatumVrijemePauka = lokacija.DatumVrijeme.ToLocalTime();
                        lok.IDVozila = lokacija.IDDjelatnika;
                        lok.IDNacinaPozicioniranja = lokacija.IDPozicioniranja;
                        lok.IDTerminala = lokacija.IDTerminala;
                        lok.Brzina = lokacija.Brzina;
                        lok.GPSAcc = lokacija.Preciznost;
                        lok.Battery = lokacija.Baterija;
                        //lok.Punjac = lokacija.Punjac;

                        db.LokacijePaukas.InsertOnSubmit(lok);
                        db.SubmitChanges();

                        id = lok.IDLokacijePauka;
                    }
                    else
                    {
                        Lokacije lok = new Lokacije();

                        lok.Lat = lokacija.Latitude;
                        lok.Long = lokacija.Longitude;
                        lok.RegistracijskaPlocica = lokacija.Registracija;
                        lok.DatumVrijeme = lokacija.DatumVrijeme.ToLocalTime();
                        lok.IDDjelatnika = lokacija.IDDjelatnika;
                        lok.IDNacinaPozicioniranja = lokacija.IDPozicioniranja;
                        lok.IDTerminala = lokacija.IDTerminala;
                        lok.CellTowerID = lokacija.CellTowerID;
                        lok.SignalStrength = lokacija.SignalStrength;
                        lok.HDOP = lokacija.HDOP;
                        lok.Brzina = lokacija.Brzina;
                        lok.GPSAcc = lokacija.Preciznost;
                        lok.Battery = lokacija.Baterija;
                        lok.Punjac = lokacija.Punjac;

                        db.Lokacijes.InsertOnSubmit(lok);
                        db.SubmitChanges();

                        id = lok.IDLokacije;
                    }
                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI LOKACIJU");
                return -1;
            }
        }

        public static bool SpremiFotografiju(string grad, int idLokacije, byte[] fotografija, int idAplikacije)
        {
            try
            {
                if (fotografija == null || fotografija.Length == 0)
                {
                    Sustav.SpremiGresku(grad, new Exception("Byte[] je prazan, id lokacije: " + idLokacije),
                        idAplikacije, "SPREMI FOTOGRAFIJU");
                    return false;
                }

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    SlikaPrekrsaja slika = new SlikaPrekrsaja();

                    slika.IDLokacije = idLokacije;
                    slika.Slika = fotografija;

                    db.SlikaPrekrsajas.InsertOnSubmit(slika);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI FOTOGRAFIJU");
                return false;
            }
        }

        public static int NoviPrekrsaj(string grad, _NoviPrekrsaj prekrsaj, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int idPredloska = prekrsaj.IDPredloska;

                    if (idPredloska <= 0)
                    {
                        idPredloska = db.PredlosciIspisas.First(i => i.IDRedarstva == idRedarstva && i.Kaznjava == prekrsaj.ZakonskaSankcija).IDPRedloska;
                    }

                    decimal kazna = prekrsaj.Kazna;

                    if (kazna <= 0)
                    {
                        kazna = Zakoni.IznosKazneS(grad, prekrsaj.IDOpisaPrekrsaja, idAplikacije);
                    }

                    Prekrsaji pre = new Prekrsaji();

                    pre.IDLokacije = prekrsaj.IDLokacije;
                    pre.IDSkracenogOpisa = prekrsaj.IDOpisaPrekrsaja; // s vremenom ukloniti - todo!
                    pre.IDNacinaPozicioniranja = prekrsaj.IDPozicioniranja;
                    pre.IDPredloskaIspisa = idPredloska;
                    pre.IDDjelatnika = prekrsaj.IDDjelatnika;
                    pre.Vrijeme = prekrsaj.Vrijeme;
                    pre.RegistracijskaPlocica = prekrsaj.Registracija.Replace("-", "").Replace(" ", "");
                    pre.BrojUpozorenja = prekrsaj.BrojUpozorenja;
                    pre.Lat = prekrsaj.Latitude;
                    pre.Long = prekrsaj.Longitude;
                    pre.Adresa = prekrsaj.Adresa;
                    pre.Kazna = kazna;
                    pre.PozivNaBroj = prekrsaj.PozivNaBroj;
                    pre.NalogPauka = prekrsaj.Nalog;
                    pre.IDNaloga = null;
                    pre.KraticaDrzave = prekrsaj.Drzava;

                    pre.Test = false;
                    pre.Poslano = false;
                    pre.Zakljucan = false;
                    pre.IDRacuna = null;

                    pre.StatusOcitanja = (byte?)prekrsaj.StatusOcitanja;
                    pre.Ocitanja = prekrsaj.Ocitanja;
                    pre.TrajanjePostupka = prekrsaj.Trajanje;
                    pre.IDRedarstva = idRedarstva;

                    try
                    {
                        if (prekrsaj.IDZakona == 0)
                        {
                            pre.IDOpisaZakona =
                                db.OpisiPrekrsajas.First(i => i.IDOpisa == prekrsaj.IDOpisaPrekrsaja).IDNovog;
                        }
                        else
                        {
                            pre.IDOpisaZakona = prekrsaj.IDZakona;
                        }
                    }
                    catch (Exception ex)
                    {
                        Sustav.SpremiGresku(grad, ex, idAplikacije, "NOVI PREKRSAJ - unificirani zakon");
                    }

                    string napomena =
                        string.Format("IDLokacije: {0}, IDOpisa: {1}, Registracija: {2}, Broj: {3}, Adresa: {4}",
                            prekrsaj.IDLokacije, prekrsaj.IDOpisaPrekrsaja, prekrsaj.Registracija, prekrsaj.BrojUpozorenja, prekrsaj.Adresa);

                    Sustav.SpremiAkciju(grad, prekrsaj.IDDjelatnika, 92, napomena, 1, idAplikacije);

                    db.Prekrsajis.InsertOnSubmit(pre);
                    db.SubmitChanges();

                    //todo - trenutno je samo za lokacije, kad zavrsi testno razdoblje to makni
                    if (grad == "Lokacije")
                    {
                        if (pre.IDPredloskaIspisa == 15 || pre.IDPredloskaIspisa == 2)
                        {
                            Vpp._VppPrijenos prijenos = new Vpp._VppPrijenos(pre.IDPrekrsaja, 1, pre.Kazna, pre.PozivNaBroj, "Obavijest");
                            bool ok = Vpp.DodajVPP(grad, prijenos, idAplikacije);

                            if (ok)
                            {
                                pre.Poslano = true;
                                db.SubmitChanges();
                            }
                        }
                    }

                    if (db.PredlosciIspisas.First(i => i.IDPRedloska == idPredloska).NazivPredloska == "OBAVIJEST")
                    {
                        new Thread(() => RentaCar.PostojiRCVozilo(grad, pre.RegistracijskaPlocica, pre.IDLokacije, 60000, idAplikacije)).Start();
                    }

                    return pre.IDPrekrsaja;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NOVI PREKRSAJ");
                return -1;
            }
        }

        public static int PosaljiNalogPauku(string grad, int idPrekrsaja, DateTime datum, bool lisice, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int id = db.Pauks.Any() ? db.Pauks.Max(i => i.IDNaloga) + 1 : 1;

                    #region PAUK

                    Pauk pauk = new Pauk();

                    pauk.IDNaloga = id;
                    pauk.Status = 0;
                    pauk.NalogZatvoren = false;
                    pauk.DatumNaloga = datum; // todo now ili isto vrijeme kao i prekrsaj?
                    pauk.DatumZaprimanja = null;
                    pauk.DatumPodizanja = null;
                    pauk.DatumDeponija = null;
                    pauk.StornoRedara = false;

                    db.Pauks.InsertOnSubmit(pauk);
                    db.SubmitChanges();

                    #endregion

                    #region NALOZI PAUKU

                    NaloziPauku np = new NaloziPauku();

                    np.IDNaloga = id;
                    np.IDStatusa = 0;
                    np.IDVozila = null;
                    np.NalogZatvoren = false;
                    np.DatumNaloga = datum;
                    np.StornoRedara = true;
                    np.Redoslijed = 0;
                    np.IDRazloga = null;
                    np.Lisice = lisice;

                    db.NaloziPaukus.InsertOnSubmit(np);
                    db.SubmitChanges();

                    #endregion

                    #region PREKRSAJ

                    Prekrsaji prek = db.Prekrsajis.First(i => i.IDPrekrsaja == idPrekrsaja);
                    prek.IDNaloga = id;
                    db.SubmitChanges();

                    #endregion

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NOVI NALOG PAUKU");
                return -1;
            }
        }

        public static _LokacijaPrekrsaja LokacijaPrekrsaja(string grad, int idLokacije, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (!db.Prekrsajis.Any(i => i.IDLokacije == idLokacije))
                    {
                        return null;
                    }

                    var lok = from p in db.Prekrsajis
                              join l in db.Lokacijes on p.IDLokacije equals l.IDLokacije
                              join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika into djel
                              from dd in djel.DefaultIfEmpty()
                              join t in db.Terminalis on l.IDTerminala equals t.IDTerminala into ter
                              from tt in ter.DefaultIfEmpty()
                              where p.IDLokacije == idLokacije
                              select
                              new _LokacijaPrekrsaja(p.IDLokacije, p.IDPrekrsaja, p.IDNaloga, l.IDNacinaPozicioniranja, p.IDDjelatnika, l.IDTerminala,
                            p.Lat, p.Long, l.Lat, l.Long, l.DatumVrijeme, p.Adresa, tt.NazivTerminala, dd.ImePrezime, dd.UID, l.Brzina);

                    if (!lok.Any())
                    {
                        return null;
                    }

                    return lok.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "LOKACIJA PREKRSAJA, IDLokacije: " + idLokacije);
                return null;
            }
        }

        public static bool? IskoristenaUplatnica(string grad, string poziv, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.Prekrsajis.Any(i => i.PozivNaBroj == poziv))
                    {
                        return true;
                    }

                    Sustav.SpremiAkciju(grad, -1, 106, "Broj uplatnice: " + poziv, 1, idAplikacije);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POZIV NA BROJ, poziv: " + poziv);
                return null;
            }
        }

        public static bool? IskoristenaUplatnicaRacun(string grad, string poziv, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.RACUNIs.Any(i => i.PozivNaBroj == poziv))
                    {
                        return true;
                    }

                    Sustav.SpremiAkciju(grad, -1, 106, "RAČUN - Broj uplatnice: " + poziv, 2, idAplikacije);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POZIV NA BROJ, poziv: " + poziv);
                return null;
            }
        }

        /*:: ZAHTJEV ZA PODIZANJEM ::*/

        public static int NoviZahtjev(string grad, _Zahtjev zahtjev, out bool aktivan, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Zahtjevi pri = new Zahtjevi();

                    pri.IDLokacije = zahtjev.IDLokacije;
                    pri.IDPrijavitelja = zahtjev.IDVozila;
                    pri.DatumVrijeme = zahtjev.Vrijeme;
                    pri.Registracija = zahtjev.Registracija;
                    pri.Lat = zahtjev.Latitude;
                    pri.Lng = zahtjev.Longitude;
                    pri.Adresa = zahtjev.Adresa;
                    pri.KraticaDrzave = zahtjev.Drzava;
                    pri.IDStatusa = -1;
                    pri.IDOpisa = zahtjev.IDOpisa;
                    pri.TipOcitanja = zahtjev.TipOcitanja;
                    pri.Trajanje = zahtjev.Trajanje;
                    pri.Ocitanja = zahtjev.Ocitanja;
                    pri.IDPrijaviteljaDjelatnik = zahtjev.IDDjelatnika;

                    db.Zahtjevis.InsertOnSubmit(pri);
                    db.SubmitChanges();

                    aktivan = AktivniKorisnik(grad, idAplikacije);

                    Nalog.SpremiPovijest(grad, pri.IDPrijave, zahtjev.IDVozila, 17, false, idAplikacije);

                    return pri.IDPrijave;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NOVI ZAHTJEV PAUKA");
                aktivan = false;
                return -1;
            }
        }

        public static bool StatusZahtjeva(string grad, int idZahtjeva, ref int idStatusa, out string poruka, out int? idNaloga, out decimal kazna, out bool obavijest, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (!db.Zahtjevis.Any(i => i.IDPrijave == idZahtjeva))
                    {
                        poruka = "";
                        idNaloga = -1;
                        kazna = 0;
                        obavijest = false;
                        return false;
                    }

                    Zahtjevi pri = db.Zahtjevis.First(i => i.IDPrijave == idZahtjeva);

                    if (idStatusa >= 0)
                    {
                        pri.IDStatusa = idStatusa;
                        db.SubmitChanges();
                    }
                    else
                    {
                        idStatusa = pri.IDStatusa;
                    }

                    idNaloga = pri.IDNaloga;
                    poruka = pri.Poruka;
                    kazna = 0;
                    obavijest = false;

                    if (idStatusa == 4)
                    {
                        Nalog.SpremiPovijest(grad, pri.IDPrijave, pri.IDPrijavitelja.Value, 20, false, idAplikacije);
                    }

                    try
                    {
                        if (pri.IDPrekrsaja == null)
                        {
                            if (pri.IDNaloga != null)
                            {
                                if (db.Prekrsajis.Any(i => i.IDPrekrsaja == pri.IDNaloga.Value))
                                {
                                    obavijest =
                                        Predlosci.Obavijest(grad,
                                            db.Prekrsajis.First(i => i.IDNaloga == pri.IDNaloga).IDPredloskaIspisa.Value,
                                            idAplikacije).Value;
                                    kazna = db.Prekrsajis.First(i => i.IDNaloga == pri.IDNaloga).Kazna;
                                }
                            }

                            return true;
                        }

                        if (db.Prekrsajis.Any(i => i.IDPrekrsaja == pri.IDPrekrsaja.Value))
                        {
                            obavijest = Predlosci.Obavijest(grad, db.Prekrsajis.First(i => i.IDPrekrsaja == pri.IDPrekrsaja).IDPredloskaIspisa.Value, idAplikacije).Value;
                            kazna = db.Prekrsajis.First(i => i.IDPrekrsaja == pri.IDPrekrsaja).Kazna;
                        }
                    }
                    catch (Exception ex)
                    {
                        Sustav.SpremiGresku(grad, ex, idAplikacije, "STATUS - KAZNA");
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "STATUS ZAHTJEVA, grad:" + grad + ", idz: " + idZahtjeva + ", ids: " + idStatusa);

                poruka = "";
                idNaloga = -1;
                kazna = 0;
                obavijest = false;
                return false;
            }
        }

        public static _Operater PreuzeoZahtjev(string grad, int idZahtjeva, out string aplikacija, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    aplikacija = "";
                    if (!db.Zahtjevis.Any(i => i.IDPrijave == idZahtjeva))
                    {
                        return null;
                    }

                    Zahtjevi pri = db.Zahtjevis.First(i => i.IDPrijave == idZahtjeva);

                    if (pri.IDOdobravatelja != null)
                    {
                        var djel = from p in db.Djelatniks
                                   where p.IDDjelatnika == pri.IDOdobravatelja.Value
                                   select new _Operater(
                                       p.IDDjelatnika,
                                       p.IDRedarstva,
                                       Korisnici.IDPrijaviteljaGo(p),
                                       p.ImePrezime,
                                       p.UID,
                                       p.BrojSI,
                                       p.IDPrivilegija,
                                       p.Parametri,
                                       p.Blokiran,
                                       Korisnici.MozeNaplacivati(p),
                                       //da bi moga vrsiti naplatu moraju biti upisani ovi detalji
                                       p.TraziOdobrenje ?? false,
                                       p.ObradjujeZahtjeve,
                                       p.GOGrad
                                   );

                        using (PostavkeDataContext pdb = new PostavkeDataContext())
                        {
                            var ima = from a in pdb.AKTIVNE_APLIKACIJEs
                                      join ap in pdb.APLIKACIJEs on a.IDAplikacije equals ap.IDAplikacije into aplikacije
                                      from app in aplikacije.DefaultIfEmpty()
                                      where a.IDGrada == Sistem.IDGrada(grad) &&
                                            a.IDDjelatnika == pri.IDOdobravatelja &&
                                            a.OdobravaZahtjeve
                                      orderby a.ZadnjaAktivnost descending
                                      select app.IDAplikacije;

                            if (ima.Any())
                            {
                                int id = ima.First();
                                aplikacija = id == 1 ? "na terenu" : "u uredu";
                            }
                            else
                            {
                                return null;
                            }
                        }

                        if (djel.Any())
                        {
                            return djel.First();
                        }
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Preuzeo Zahtjev");

                aplikacija = "";
                return null;
            }
        }

        public static bool AktivniKorisnik(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext po = new PostavkeDataContext())
                {
                    int id = po.GRADOVIs.First(i => i.Baza == grad).IDGrada;

                    DateTime dat = DateTime.Now.Subtract(new TimeSpan(0, 0, 5, 0)); //todo ubaciti u bazu?

                    var ap = from a in po.AKTIVNE_APLIKACIJEs
                             where a.IDGrada == id &&
                                   a.OdobravaZahtjeve &&
                                   //(a.IDAplikacije == 6 || a.IDAplikacije == 1) &&
                                   //(a.IDAplikacije == 4 || a.IDAplikacije == 6) &&
                                   a.ZadnjaAktivnost >= dat
                             select a;

                    if (ap.Any())
                    {
                        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                        {
                            foreach (var q in ap)
                            {
                                //samo Admin JLS i Djelatnik (Redar) mogu odobravati
                                if (db.Djelatniks.Any(i => i.IDDjelatnika == q.IDDjelatnika))
                                //&& (i.IDPrivilegija == 3 || i.IDPrivilegija == 4))) 
                                {
                                    //mozda vratiti popis pa da odabere koje želi ili mu inicijalno dati popis pa da pauk odabere kome šalje?
                                    return true;
                                }
                            }

                            return false;
                        }
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "AKTIVNI KORISNIK");
                return false;
            }
        }

        public static int IDVozila(string grad, int idTerminala, out string vozilo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (!db.VozilaPaukas.Any(i => i.IDTerminala == idTerminala))
                    {
                        vozilo = null;
                        return -1;
                    }

                    var voz = db.VozilaPaukas.First(i => i.IDTerminala == idTerminala);

                    if (voz == null)
                    {
                        vozilo = null;
                        return -1;
                    }

                    vozilo = voz.NazivVozila;
                    return voz.IDVozila;
                }
            }
            catch
            {
                vozilo = null;
                return -1;
            }
        }

        /*:: STATUS KAŽNJAVANJA VOZILA ::*/

        public static List<_Kaznjavan> Kaznjavan(string grad, string registracija, string drzava, int dana, bool slike,  int idAplikacije)
        {
            DateTime dt = DateTime.Now;

            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var pr = from p in db.Prekrsajis
                             join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska into predlozak
                             from ii in predlozak.DefaultIfEmpty()
                             join r in db.Djelatniks on p.IDDjelatnika equals r.IDDjelatnika
                             join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa
                             //todo
                             join d in db.PopisPrekrsajas on o.IDPrekrsaja equals d.IDPrekrsaja
                             //todo
                             where p.RegistracijskaPlocica.Trim() == registracija.Trim().Replace(" ", "").Replace("-", "") &&
                                   p.KraticaDrzave == drzava &&
                                   p.Vrijeme.Value.Date >= DateTime.Today.AddDays(-dana).Date &&
                                   p.Test == false && p.Status == false
                             select new _Kaznjavan(
                                 p.IDLokacije,
                                 ii.NazivPredloska,
                                 p.Vrijeme.Value,
                                 p.Kazna,
                                 p.Lat,
                                 p.Long,
                                 p.Adresa,
                                 r.ImePrezime,
                                 string.Format("{0} ({1})", o.OpisPrekrsaja, d.MaterijalnaKaznjivaNorma),
                                 slike
                                     ? db.SlikaPrekrsajas.Where(z => z.IDLokacije == p.IDLokacije)
                                         .Select(z => z.IDSlikePrekrsaja)
                                         .ToList()
                                     : null
                             );

                    return pr.ToList();
                }
            }
            catch (Exception ex)
            {
                TimeSpan ts = DateTime.Now.Subtract(dt);

                Sustav.SpremiGresku(grad, ex, idAplikacije, "STATUS KAŽNJAVANJA VOZILA, Trajanje: " + ts.TotalSeconds);
                return new List<_Kaznjavan>();
            }
        }

        /*:: NEOČITANA REGISTRACIJA ::*/

        public static bool NeocitanaRegistracija(string grad, _Neocitana registracija, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    NeocitaneRegistracije reg = new NeocitaneRegistracije();

                    reg.Slika = registracija.Slika;
                    reg.DatumVrijeme = registracija.Datum;
                    reg.TrajanjePrijenosa = registracija.TrajanjePrijenosa;
                    reg.Duljina = registracija.Duljina;
                    reg.TrajanjeOcitanja = registracija.TrajanjeOcitanja;
                    reg.Ocitanja = registracija.Ocitanja;
                    reg.Status = registracija.Status;
                    reg.IDDjelatnika = registracija.IDDjelatnika;
                    reg.XMLData = registracija.XMLData;

                    db.NeocitaneRegistracijes.InsertOnSubmit(reg);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NEOČITANA REGISTRACIJA");
                return false;
            }
        }

        /*::  ::*/

        public static int DodanoLokacija(string grad, DateTime datumOd, DateTime datumDo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.Lokacijes.Count(i => i.DatumVrijeme >= datumOd && i.DatumVrijeme <= datumDo);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODANO LOKACIJA");
                return -1;
            }
        }

        public static int DodanoPrekrsaja(string grad, DateTime datumOd, DateTime datumDo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var ukupno = from p in db.Prekrsajis
                                 where p.Vrijeme.Value >= datumOd && p.Vrijeme.Value <= datumDo
                                 select p;

                    return ukupno.Count();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODANO PREKRSAJA");
                return -1;
            }
        }
    }
}