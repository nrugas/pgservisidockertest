using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using PG.Servisi.MUPParking;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Naplata
    {
        public static List<_VrstaPlacanja> VrstePlacanja(string grad, int? idStatusa, int idRedarstva, int idAplikacije)
        {
            try
            {
                //Uplatitelj - 0=NE, 1=OPCIONALNO, 2=OBAVEZNO
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (idStatusa == null)
                    {
                        var vp = from v in db.VRSTE_PLACANJAs
                                 select new _VrstaPlacanja(v.IDVrstePlacanja, v.NazivVrstePlacanja, v.Uplatnica, v.Ispis, v.Uplatitelj, 0,
                                 v.NaRacunu, v.Kratica);

                        return vp.ToList();
                    }

                    var vps = from v in db.VRSTE_PLACANJAs
                              join vs in db.VRSTE_PLACANJA_STATUS on v.IDVrstePlacanja equals vs.IDVrste
                              where vs.IDStatusa == idStatusa.Value &&
                                    vs.IDGrada == Sistem.IDGrada(grad) &&
                                    vs.IDRedarstva == idRedarstva
                              select new _VrstaPlacanja(v.IDVrstePlacanja, v.NazivVrstePlacanja, v.Uplatnica, v.Ispis, v.Uplatitelj, vs.Iznos,
                              v.NaRacunu, v.Kratica);

                    return vps.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VRSTE PLAĆANJA");
                return new List<_VrstaPlacanja>();
            }
        }

        public static List<_VrstaPlacanjaStatus> VrstePlacanjaStatusi(string grad, int idRedarstva, out bool definiranIznos, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext pb = new PostavkeDataContext())
                {
                    List<_VrstaPlacanjaStatus> vrste = new List<_VrstaPlacanjaStatus>();

                    foreach (var v in pb.VRSTE_PLACANJAs)
                    {
                        var sp = from vps in pb.VRSTE_PLACANJA_STATUS
                                 where vps.IDGrada == Sistem.IDGrada(grad) &&
                                       vps.IDVrste == v.IDVrstePlacanja &&
                                       vps.IDRedarstva == idRedarstva
                                 select vps;

                        if (!sp.Any())
                        {
                            vrste.Add(new _VrstaPlacanjaStatus(v.IDVrstePlacanja, v.NazivVrstePlacanja, Sistem.IDGrada(grad), -1, 0));
                            continue;
                        }

                        vrste.Add(new _VrstaPlacanjaStatus(v.IDVrstePlacanja, v.NazivVrstePlacanja, Sistem.IDGrada(grad), sp.First().IDStatusa, sp.First().Iznos));
                    }

                    using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                    {
                        if (idRedarstva == 2)
                        {
                            //ako je pauk mora biti definran iznos pokušaja podizanja da bi mogli dodavati vrste plaćanja
                            definiranIznos = db.RACUNI_STAVKE_OPIs.Any(i => i.IDStatusa == 3 && i.IDRedarstva == 2);
                        }
                        else
                        {
                            definiranIznos = true;
                        }
                        return vrste;
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VRSTE PLAĆANJA");
                definiranIznos = false;
                return new List<_VrstaPlacanjaStatus>();
            }
        }

        public static bool IzmjeniStstusVrstePlacanja(string grad, int idVrste, bool ukljuci, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext pb = new PostavkeDataContext())
                {
                    if (ukljuci)
                    {
                        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                        {
                            if (idRedarstva == 2)
                            {
                                VRSTE_PLACANJA_STATUS ss = new VRSTE_PLACANJA_STATUS();

                                ss.IDGrada = Sistem.IDGrada(grad);
                                ss.IDVrste = idVrste;
                                ss.IDStatusa = 3;
                                ss.Iznos = (decimal)db.RACUNI_STAVKE_OPIs.First(i => i.IDStatusa == 3 && i.IDRedarstva == 2).Iznos;
                                ss.IDRedarstva = idRedarstva;

                                pb.VRSTE_PLACANJA_STATUS.InsertOnSubmit(ss);
                                pb.SubmitChanges();

                            }
                            else
                            {
                                VRSTE_PLACANJA_STATUS ss = new VRSTE_PLACANJA_STATUS();

                                ss.IDGrada = Sistem.IDGrada(grad);
                                ss.IDVrste = idVrste;
                                ss.IDStatusa = 0;
                                ss.Iznos = 0;
                                ss.IDRedarstva = idRedarstva;

                                pb.VRSTE_PLACANJA_STATUS.InsertOnSubmit(ss);
                                pb.SubmitChanges();
                            }
                        }
                    }
                    else
                    {
                        pb.VRSTE_PLACANJA_STATUS.DeleteOnSubmit(pb.VRSTE_PLACANJA_STATUS.First(i => i.IDGrada == Sistem.IDGrada(grad) && i.IDVrste == idVrste && i.IDRedarstva == idRedarstva));
                        pb.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI VRSTU PLAĆANJA STATUS");
                return false;
            }
        }

        public static List<_2DLista> VrsteKartica(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var vps = from v in db.VRSTE_KARTICAs
                              select new _2DLista(v.IDVrsteKartice, v.NazivVrsteKartice);

                    return vps.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VRSTE KARTICA");
                return new List<_2DLista>();
            }
        }

        public static List<_2DLista> VrsteBanaka(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var vps = from v in db.VRSTE_BANAKAs
                              select new _2DLista(v.IDBanke, v.Kratica);

                    return vps.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VRSTE BANAKA");
                return new List<_2DLista>();
            }
        }

        public static string NaplatiPauk(string grad, _Racun racun, int idStatusa, out int idRacuna, out string brrac, out string poziv, int idAplikacije)
        {
            try
            {
                //todo: provjera da li je storniran
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Prekrsaji p = db.Prekrsajis.First(i => i.IDNaloga == racun.IDReference);
                    if (idStatusa == -10)
                    {
                        idStatusa = db.NaloziPaukus.First(i => i.IDNaloga == p.IDNaloga).IDStatusa;
                    }

                    string registracija = p.RegistracijskaPlocica, clanak = db.OpisiPrekrsajas.First(i => i.IDOpisa == p.IDSkracenogOpisa).ClanakPauka;

                    //račun postoji za nalog
                    if (db.RACUNIs.Any(i => i.IDReference == racun.IDReference && i.IDRedarstva == racun.IDRedarstva && i.Storniran == false))
                    {
                        RACUNI r = db.RACUNIs.First(i => i.IDReference == racun.IDReference && i.IDRedarstva == racun.IDRedarstva);
                        brrac = r.BrojRacuna;
                        idRacuna = r.IDRacuna;
                        poziv = r.PozivNaBroj;

                        return Ispis.Racun(grad, DohvatiRacun(grad, idRacuna, false, idAplikacije), registracija, clanak, idStatusa, p.Vrijeme.Value, idAplikacije);
                    }

                    idRacuna = NoviRacun(grad, racun, out brrac, out poziv, idAplikacije);

                    if (idRacuna == -1)
                    {
                        return "";
                    }

                    return Ispis.Racun(grad, DohvatiRacun(grad, idRacuna, false, idAplikacije), registracija, clanak, idStatusa, p.Vrijeme.Value, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATA RAČUNA");
                idRacuna = -1;
                brrac = null;
                poziv = "";
                return "";
            }
        }

        public static string NaplatiParking(string grad, _Racun racun, bool info, int idAplikacije)
        {
            return NaplatiParking(grad, racun, info, idAplikacije, 0);
        }

        public static string NaplatiParking(string grad, _Racun racun, bool info, int idAplikacije, int tipPrintera)
        {
            try
            {
                string brrac, poziv;
                int idRacuna = NoviRacun(grad, racun, out brrac, out poziv, idAplikacije);

                if (idRacuna == -1)
                {
                    return "";
                }

                return Ispis.RacunParking(grad, DohvatiRacun(grad, idRacuna, false, idAplikacije), info, idAplikacije, tipPrintera);
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATA RAČUNA");
                return "";
            }
        }

        public static int NoviRacun(string grad, _Racun racun, out string brrac, out string poziv, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int id = 1, rb = 1;

                    if (db.RACUNIs.Any())
                    {
                        id = db.RACUNIs.Max(i => i.IDRacuna) + 1;
                    }

                    if (db.RACUNIs.Any(i => i.Godina == racun.DatumVrijeme.Year && i.IDRedarstva == racun.IDRedarstva && i.PoslovniProstor == racun.OznakaPP))
                    {
                        rb = db.RACUNIs.Where(i => i.Godina == racun.DatumVrijeme.Year && i.IDRedarstva == racun.IDRedarstva && i.PoslovniProstor == racun.OznakaPP).Max(i => i.RB) + 1;
                    }

                    RACUNI rac = new RACUNI();

                    rac.IDRacuna = id;
                    rac.IDVrstePlacanja = racun.IDVrste;
                    rac.IDDjelatnika = racun.IDDjelatnika;
                    rac.IDRedarstva = racun.IDRedarstva;
                    rac.IDVrsteKartice = racun.IDVrsteKartice;
                    rac.IDBanke = racun.IDVrsteBanke;
                    rac.Datum = racun.DatumVrijeme;
                    rac.RB = rb;
                    rac.Godina = racun.DatumVrijeme.Year;
                    rac.PDV = racun.PDV;
                    rac.Osnovica = racun.Osnovica;
                    rac.Ukupno = racun.Ukupno;
                    rac.PDVPosto = racun.PDVPosto;
                    rac.PoslovniProstor = racun.OznakaPP;
                    rac.Blagajna = racun.Blagajna;
                    rac.BrojRacuna = rac.RB + "/" + rac.PoslovniProstor + "/" + rac.Blagajna;
                    rac.OIB = racun.OIB;
                    rac.Storniran = racun.Storniran;
                    rac.Orginal = racun.Orginal;
                    rac.Napomena = racun.Napomena;
                    rac.Uplacen = racun.IDVrste != 4;
                    rac.IDReference = racun.IDReference;
                    rac.Operater = racun.Operater;
                    rac.DatumPreuzimanja = racun.DatumPreuzimanja;
                    rac.PozivNaBroj = string.IsNullOrEmpty(racun.PozivNaBr) ? PozivNaBroj(grad, racun.Ukupno, rac.IDRedarstva, idAplikacije) : racun.PozivNaBr;
                    rac.BrojOdobrenja = racun.BrojOdobrenja;
                    rac.Rate = racun.Rate;
                    rac.JIR = "";
                    rac.ZKI = "";
                    rac.UUID = "";

                    db.RACUNIs.InsertOnSubmit(rac);
                    db.SubmitChanges();

                    bool ok = SpremiStavke(grad, racun.Stavke, id, false, idAplikacije);
                    bool oks = Osobe.SpremiOsobe(grad, racun.Osobe, id, false, rac.IDVrstePlacanja == 4 || rac.IDVrstePlacanja == 5, idAplikacije);

                    if (!ok || !oks)
                    {
                        try
                        {
                            db.RACUNI_OSOBE_RELACIJEs.DeleteAllOnSubmit(db.RACUNI_OSOBE_RELACIJEs.Where(i => i.IDRacuna == rac.IDRacuna));
                            db.RACUNI_STAVKEs.DeleteAllOnSubmit(db.RACUNI_STAVKEs.Where(i => i.IDRacuna == rac.IDRacuna));
                            db.RACUNIs.DeleteOnSubmit(db.RACUNIs.First(i => i.IDRacuna == rac.IDRacuna));
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATA RAČUNA");
                        }

                        Sustav.SpremiGresku(grad, new ApplicationException("Greška kod spremanja stavki, obrisao sam račun i unesene stavke"), idAplikacije, "NAPLATA RAČUNA");

                        brrac = "";
                        poziv = "";
                        return -1;
                    }

                    //ovisno o tome za koga je naplata
                    if (rac.IDRedarstva == 2)
                    {
                        db.NaloziPaukus.First(i => i.IDNaloga == racun.IDReference).IDRacuna = id;
                        db.SubmitChanges();
                    }

                    if (rac.IDRedarstva == 4)
                    {
                        if (racun.IDReference > 0)
                        {
                            //todo idrefernece mora biti id opazanja
                            var o = db.PARKING_OPAZANJAs.First(i => i.IDOpazanja == racun.IDReference);
                            o.IDRacuna = id;
                            o.PlacenoDo = racun.DatumVrijeme.AddDays(1); //TODO vidjeti ubuduće da li uvijek važi 24h!???
                            o.Kaznjen = true;
                            //o.IDStatusa = idStatusa; //todo??
                            o.Iznos = racun.Ukupno;

                            db.SubmitChanges();
                        }
                    }

                    if (rac.IDVrstePlacanja != 4 && rac.IDVrstePlacanja != 5)
                    {
                        Fiskalizacija.Fiskaliziraj(grad, id, rac.IDRedarstva, idAplikacije);
                    }
                    else
                    {
                        //todo - trenutno je samo za lokacije, kad zavrsi testno razdoblje to makni
                        if (grad == "Lokacije")
                        {
                            Vpp._VppPrijenos prijenos = new Vpp._VppPrijenos(rac.IDRacuna, rac.IDRedarstva, rac.Ukupno, rac.PozivNaBroj, "Transakcijski račun");
                            bool okp = Vpp.DodajVPP(grad, prijenos, idAplikacije);

                            if (okp)
                            {
                                //todo uvedi neki status na račune
                                //pre.Poslano = true;
                                //db.SubmitChanges();
                            }
                        }
                    }

                    brrac = rac.BrojRacuna;
                    poziv = rac.PozivNaBroj;
                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NOVI RAČUN");
                brrac = "";
                poziv = "";
                return -1;
            }
        }

        private static string PozivNaBroj(string grad, decimal ukupno, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    string increment = "00001";

                    if (db.RACUNIs.Any(i => i.PozivNaBroj.Contains("-" + DateTime.Today.ToString("yy") + "9") && i.IDRedarstva == idRedarstva))
                    {
                        int id = db.RACUNIs.Count(i => i.PozivNaBroj.Contains("-" + DateTime.Today.ToString("yy") + "9") && i.IDRedarstva == idRedarstva) + 1;
                        increment = id.ToString("00000");
                    }

                    _Uplatnica n = Gradovi.Uplatnica(grad, idRedarstva, idAplikacije);

                    if (n == null)
                    {
                        return "";
                    }

                    string iznos = "000";

                    try
                    {
                        PostavkeDataContext pdb = new PostavkeDataContext();

                        if (pdb.UPLATNICE_IZNOSIs.Any(i => i.Iznos == ukupno))
                        {
                            iznos = pdb.UPLATNICE_IZNOSIs.First(i => i.Iznos == ukupno).IDIznosa.ToString("000");
                        }
                    }
                    catch (Exception)
                    {
                        iznos = "000";
                    }

                    string idjls = n.IDGrada.ToString("000");

                    string godina = DateTime.Today.ToString("yy");
                    string rucno = "9";

                    string prvidio = iznos + idjls + idRedarstva;
                    string drugidio = godina + rucno + increment;

                    string poziv;

                    if (n.Model == "HR01")
                    {
                        poziv = prvidio + "-" + drugidio + KontrolniBroj.f_kontrolni("HUBM11", prvidio + drugidio, "", (prvidio + drugidio).Length);
                    }
                    else
                    {
                        poziv = prvidio + KontrolniBroj.f_kontrolni("HUBM11", prvidio, "", prvidio.Length) + "-" + drugidio + KontrolniBroj.f_kontrolni("HUBM11", drugidio, "", drugidio.Length);
                    }

                    return (n.Poziv1 + "-" + poziv + "-" + n.Poziv2).Trim('-');
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static int Blagajna(string grad, int idNaloga, out string oznakaPP, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int idVozila = db.NaloziPaukus.First(i => i.IDNaloga == idNaloga).IDVozila.Value;

                    VozilaPauka v = db.VozilaPaukas.First(i => i.IDVozila == idVozila);

                    oznakaPP = "";
                    if (!string.IsNullOrEmpty(v.OznakaPP))
                    {
                        oznakaPP = v.OznakaPP;
                    }

                    return Convert.ToInt32(v.Oznaka);
                }
            }
            catch (Exception)
            {
                oznakaPP = "";
                return -1;
            }
        }

        public static bool SpremiStavke(string grad, List<_Stavka> stavke, int idRacuna, bool storno, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    foreach (var s in stavke)
                    {
                        RACUNI_STAVKE sta = new RACUNI_STAVKE();

                        int ids = 1;

                        if (db.RACUNI_STAVKEs.Any())
                        {
                            ids = db.RACUNI_STAVKEs.Max(i => i.IDStavke) + 1;
                        }

                        sta.IDStavke = ids;
                        sta.IDRacuna = idRacuna;
                        sta.IDOpisaStavke = s.IDOpisaStavke;
                        sta.Kolicina = s.Kolicina;
                        sta.Cijena = storno ? s.Cijena * -1 : s.Cijena;
                        sta.Pdv = storno ? s.PDV * -1 : s.PDV;
                        sta.Osnovica = storno ? s.Osnovica * -1 : s.Osnovica;
                        sta.Ukupno = storno ? s.Ukupno * -1 : s.Ukupno;
                        sta.PdvPosto = s.PdvPosto;
                        sta.Napomena = s.Napomena;

                        db.RACUNI_STAVKEs.InsertOnSubmit(sta);
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "STAVKE RAČUNA");
                return false;
            }
        }

        /*:: STORNO ::*/

        public static int StornirajRacun(string grad, _Racun racun, int idStatusa, byte[] prilog, string filename, out string brrac, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int id = 1, rb = 1;

                    if (db.RACUNIs.Any())
                    {
                        id = db.RACUNIs.Max(i => i.IDRacuna) + 1;
                    }

                    if (db.RACUNIs.Any())
                    {
                        rb = db.RACUNIs.Where(i => i.Godina == racun.DatumVrijeme.Year && i.IDRedarstva == racun.IDRedarstva && i.PoslovniProstor == racun.OznakaPP).Max(i => i.RB) + 1;
                    }

                    RACUNI rac = new RACUNI();

                    rac.IDRacuna = id;
                    rac.IDVrstePlacanja = racun.IDVrste;
                    rac.IDDjelatnika = racun.IDDjelatnika;
                    rac.IDRedarstva = racun.IDRedarstva;
                    rac.IDVrsteKartice = racun.IDVrsteKartice;
                    rac.IDBanke = racun.IDVrsteBanke;
                    rac.Datum = racun.DatumVrijeme;
                    rac.RB = rb;
                    rac.Godina = racun.DatumVrijeme.Year;
                    rac.PDV = racun.PDV;
                    rac.Osnovica = racun.Osnovica;
                    rac.Ukupno = racun.Ukupno;
                    rac.PDVPosto = racun.PDVPosto;
                    rac.PoslovniProstor = racun.OznakaPP;
                    rac.Blagajna = racun.Blagajna;
                    rac.BrojRacuna = rac.RB + "/" + rac.PoslovniProstor + "/" + rac.Blagajna;
                    rac.OIB = racun.OIB;
                    rac.Storniran = racun.Storniran;
                    rac.Orginal = racun.Orginal;
                    if (string.IsNullOrEmpty(rac.Napomena))
                    {
                        rac.Napomena = racun.Napomena;
                    }
                    else
                    {
                        rac.Napomena += " || " + racun.Napomena;
                    }
                    rac.Uplacen = false;
                    rac.JIR = "";
                    rac.ZKI = "";
                    rac.UUID = "";
                    rac.IDReference = racun.IDReference;
                    rac.Operater = racun.Operater;
                    rac.DatumPreuzimanja = racun.DatumPreuzimanja;
                    rac.PozivNaBroj = racun.PozivNaBr;
                    rac.BrojOdobrenja = racun.BrojOdobrenja;
                    rac.Rate = racun.Rate;

                    db.RACUNIs.InsertOnSubmit(rac);
                    db.SubmitChanges();

                    bool ok = SpremiStavke(grad, DohvatiStavkeRacuna(grad, racun.IDRacuna, idAplikacije), id, true, idAplikacije);
                    bool oks = Osobe.SpremiOsobe(grad, racun.Osobe, id, true, false, idAplikacije); //todo - ne kopirati samo id-eve

                    if (!ok || !oks)
                    {
                        db.RACUNI_OSOBE_RELACIJEs.DeleteAllOnSubmit(db.RACUNI_OSOBE_RELACIJEs.Where(i => i.IDRacuna == rac.IDRacuna));
                        db.RACUNI_STAVKEs.DeleteAllOnSubmit(db.RACUNI_STAVKEs.Where(i => i.IDRacuna == rac.IDRacuna));
                        db.RACUNIs.DeleteOnSubmit(db.RACUNIs.First(i => i.IDRacuna == rac.IDRacuna));
                        db.SubmitChanges();

                        Sustav.SpremiGresku(grad, new ApplicationException("greška kod spremanja stavki, obrisao sam račun i unesene stavke"), idAplikacije, "NAPLATA RAČUNA");

                        brrac = "";
                        return -1;
                    }

                    db.RACUNIs.First(i => i.IDRacuna == racun.IDRacuna).Storniran = true;
                    db.SubmitChanges();

                    //ovisno o tome za koga je naplata
                    if (rac.IDRedarstva == 2)
                    {
                        db.NaloziPaukus.First(i => i.IDNaloga == racun.IDReference).IDRacuna = null;
                        db.SubmitChanges();
                    }

                    if (rac.IDRedarstva == 4)
                    {
                        if (racun.IDReference > 0)
                        {

                            PARKING_OPAZANJA po = db.PARKING_OPAZANJAs.First(i => i.IDOpazanja == racun.IDReference);
                            po.Kaznjen = false;
                            po.Iznos = null;
                            po.IDStatusa = null;
                            po.IDRacuna = null;
                            po.PlacenoDo = null;
                            db.SubmitChanges();

                        }
                    }

                    if (rac.IDVrstePlacanja != 4 && rac.IDVrstePlacanja != 5)
                    {
                        Fiskalizacija.Fiskaliziraj(grad, id, rac.IDRedarstva, idAplikacije);
                    }

                    Storno(grad, racun.IDRacuna, rac.IDRacuna, idStatusa, racun.IDDjelatnika, racun.Napomena, prilog, filename, idAplikacije);

                    brrac = rac.BrojRacuna;
                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "STORNIRAJ RAČUN");
                brrac = "";
                return -1;
            }
        }

        public static string StornirajRacunParking(string grad, int idRacuna, int idDjelatnika, string napomena, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    _Racun racun = DohvatiRacun(grad, idRacuna, true, idAplikacije);

                    Djelatnik d = db.Djelatniks.First(i => i.IDDjelatnika == idDjelatnika);

                    racun.IDDjelatnika = idDjelatnika;
                    racun.DatumVrijeme = DateTime.Now;
                    racun.OIB = d.OIB;
                    racun.Operater = d.ImeNaRacunu;
                    racun.Orginal = racun.BrojRacuna;
                    racun.Storniran = true;
                    racun.Osnovica = racun.Osnovica * -1;
                    racun.Ukupno = racun.Ukupno * -1;
                    racun.PDV = racun.PDV * -1;
                    racun.Godina = DateTime.Now.Year;
                    //racun.OznakaPP = stari.OznakaPP; //todo da li oznaka sa originala ili sa kase? novi.OznakaPP = Settings.Default.OznakaPP;
                    string broj;
                    int id = StornirajRacun(grad, racun, 5, null, null, out broj, idAplikacije);

                    if (id > 0)
                    {
                        return "OK";
                        //IspisKopijeRacuna(grad, id, idAplikacije);Pla
                    }

                    return "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        internal static string StornirajRacun(string grad, int idReference, int idDjelatnika, string napomena, int idStatusa, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    _Racun racun = DohvatiRacunReference(grad, idReference, idAplikacije);

                    Djelatnik d = db.Djelatniks.First(i => i.IDDjelatnika == idDjelatnika);

                    racun.IDDjelatnika = idDjelatnika;
                    racun.DatumVrijeme = DateTime.Now;
                    racun.OIB = d.OIB;
                    racun.Operater = d.ImeNaRacunu;
                    racun.Orginal = racun.BrojRacuna;
                    racun.Storniran = true;
                    racun.Osnovica = racun.Osnovica * -1;
                    racun.Ukupno = racun.Ukupno * -1;
                    racun.PDV = racun.PDV * -1;
                    racun.Godina = DateTime.Now.Year;
                    //racun.OznakaPP = stari.OznakaPP; //todo da li oznaka sa originala ili sa kase? novi.OznakaPP = Settings.Default.OznakaPP;

                    string broj;
                    int id = StornirajRacun(grad, racun, idStatusa, null, null, out broj, idAplikacije);

                    if (id > 0)
                    {
                        return IspisKopijeRacuna(grad, id, idAplikacije);
                    }

                    return "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static bool Storno(string grad, int idOriginala, int idStorniranog, int idStatusa, int idDjelatnika, string napomena, byte[] prilog, string filename, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RACUNI_STORNA rs = new RACUNI_STORNA();

                    int id = 1;

                    if (db.RACUNI_STORNAs.Any())
                    {
                        id = db.RACUNI_STORNAs.Max(i => i.IDStorna) + 1;
                    }

                    rs.IDStorna = id;
                    rs.IDOriginala = idOriginala;
                    rs.IDStorniranog = idStorniranog;
                    rs.IDStatusa = idStatusa;
                    rs.IDDjelatnika = idDjelatnika;
                    rs.Datum = DateTime.Now;
                    rs.Napomena = napomena;
                    rs.Prilog = prilog;
                    rs.PrilogNaziv = filename;

                    db.RACUNI_STORNAs.InsertOnSubmit(rs);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "STORNIRAJ RAČUN REKAPITULACIJA");
                return false;
            }
        }

        public static List<_2DLista> StatusiStorna(int idRedarstva)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var st = from s in db.STATUSI_STORNAs
                             where s.IDRedarstva == idRedarstva
                             select new _2DLista(s.IDStatusStorna, s.Naziv);

                    return st.ToList();
                }
            }
            catch (Exception e)
            {
                return new List<_2DLista>();
            }
        }

        public static string StatusStorna(int? idStatusa)
        {
            try
            {
                if (idStatusa == null)
                {
                    return "";
                }

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var st = from s in db.STATUSI_STORNAs
                        where s.IDStatusStorna == idStatusa
                        select s.Naziv;

                    return st.First();
                }
            }
            catch
            {
                return "";
            }
        }

        public static byte[] PregledajPrilogStornu(string grad, int idRacuna, out string filename, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var p = from s in db.RACUNI_STORNAs
                        where s.IDStorniranog == idRacuna
                        select s;

                    if (p.Any())
                    {
                        filename = p.First().PrilogNaziv;
                        return p.First().Prilog.ToArray();
                    }

                    filename = "";
                    return null;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PREGLED PRILOGA STORNU");
                filename = "";
                return null;
            }
        }

        /*:: ISPIS ::*/

        public static string IspisKopijeRacuna(string grad, int idRacuna, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    _Racun racun = DohvatiRacun(grad, idRacuna, true, idAplikacije);

                    if (racun == null)
                    {
                        return "";
                    }

                    Prekrsaji p = db.Prekrsajis.First(i => i.IDNaloga == racun.IDReference);
                    string registracija = p.RegistracijskaPlocica, clanak = db.OpisiPrekrsajas.First(i => i.IDOpisa == p.IDSkracenogOpisa).ClanakPauka;
                    int idStatusa = db.NaloziPaukus.First(i => i.IDNaloga == p.IDNaloga).IDStatusa;

                    return Ispis.Racun(grad, racun, registracija, clanak, idStatusa, p.Vrijeme.Value, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "KOPIJA RACUNA");
                return "";
            }
        }

        public static string IspisKopijeRacunaBroj(string grad, string broj, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (!db.RACUNIs.Any(i => i.BrojRacuna == broj))
                    {
                        return "";
                    }

                    int id = db.RACUNIs.Where(i => i.BrojRacuna == broj).OrderByDescending(i => i.Datum).First().IDRacuna;

                    _Racun racun = DohvatiRacun(grad, id, true, idAplikacije);

                    if (racun == null)
                    {
                        return "";
                    }

                    Prekrsaji p = db.Prekrsajis.First(i => i.IDNaloga == racun.IDReference);
                    string registracija = p.RegistracijskaPlocica, clanak = db.OpisiPrekrsajas.First(i => i.IDOpisa == p.IDSkracenogOpisa).ClanakPauka;
                    int idStatusa = db.NaloziPaukus.First(i => i.IDNaloga == p.IDNaloga).IDStatusa;

                    return Ispis.Racun(grad, racun, registracija, clanak, idStatusa, p.Vrijeme.Value, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "KOPIJA RACUNA");
                return "";
            }
        }

        public static string VrstaPlacanja(string grad, int? idVrste, int idAplikacije)
        {
            bool uplatnica;
            return VrstaPlacanja(grad, idVrste, idAplikacije, out uplatnica);
        }

        public static string VrstaPlacanja(string grad, int? idVrste, int idAplikacije, out bool uplatnica)
        {
            uplatnica = false;
            try
            {
                if (idVrste == null)
                {
                    return "-";
                }

                if (idVrste == 0)
                {
                    return "-";
                }

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var q = db.VRSTE_PLACANJAs.First(i => i.IDVrstePlacanja == idVrste);
                    uplatnica = q.Uplatnica;
                    return q.NaRacunu;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "BR RACUNA I VRSTA RACUNA");
                return "-";
            }
        }

        public static string VrstaBanke(string grad, int? idVrste, int idAplikacije)
        {
            try
            {
                if (idVrste == null)
                {
                    return "-";
                }

                if (idVrste == 0)
                {
                    return "-";
                }

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.VRSTE_BANAKAs.First(i => i.IDBanke == idVrste).Kratica;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "BR RACUNA I VRSTA RACUNA");
                return "-";
            }
        }

        public static string VrstaPlacanjaDetaljno(string grad, int? idVrste, int idAplikacije)
        {
            try
            {
                if (idVrste == null)
                {
                    return "-";
                }
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.VRSTE_PLACANJAs.First(i => i.IDVrstePlacanja == idVrste).NazivVrstePlacanja;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "BR RACUNA I VRSTA RACUNA");
                return "-";
            }
        }

        public static string VrstaKartice(string grad, int? idVrste, int idAplikacije)
        {
            try
            {
                if (idVrste == null)
                {
                    return "";
                }

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.VRSTE_KARTICAs.First(i => i.IDVrsteKartice == idVrste).NazivVrsteKartice;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "BR RACUNA I VRSTA RACUNA");
                return "-";
            }
        }

        public static string VrstaKarticeKratica(string grad, int? idVrste, int idAplikacije)
        {
            try
            {
                if (idVrste == null)
                {
                    return "";
                }

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.VRSTE_KARTICAs.First(i => i.IDVrsteKartice == idVrste).Sifra;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "BR RACUNA I VRSTA RACUNA");
                return "-";
            }
        }

        public static string VrstaPlacanjaKratica(string grad, int? idVrste, int idAplikacije)
        {
            try
            {
                if (idVrste == null)
                {
                    return "-";
                }
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.VRSTE_PLACANJAs.First(i => i.IDVrstePlacanja == idVrste).Kratica;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "BR RACUNA I VRSTA RACUNA");
                return "-";
            }
        }

        /*:: OPISI STAVKI ::*/

        public static List<_2DLista> Stavka(string grad, int idStatusa, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.RACUNI_STAVKE_OPIs.Where(i => i.IDStatusa == idStatusa && i.Obrisan == false).Select(i => new _2DLista((int)i.Iznos, i.NazivOpisaStavke)).ToList();
                }
            }
            catch (Exception)
            {
                return new List<_2DLista>();
            }
        }

        public static List<_2DLista> StatusiKojiNaplacuju(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var stat = from s in db.StatusPaukas
                               where s.Naplacuje
                               select new _2DLista(s.IDStatusa, s.NazivStatusa);

                    return stat.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "STATUSI KOJI SE NAPLAČUJU");
                return new List<_2DLista>();
            }
        }

        public static bool? ObrisiOpisStavke(string grad, int idOpisa, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.RACUNI_STAVKE_OPIs.First(i => i.IDOpisaStavke == idOpisa).Obrisan = true;
                    db.SubmitChanges();

                    try
                    {
                        db.RACUNI_STAVKE_OPIs.DeleteOnSubmit(db.RACUNI_STAVKE_OPIs.First(i => i.IDOpisaStavke == idOpisa));
                        db.SubmitChanges();

                        return true;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "KOPIJA RACUNA");
                return false;
            }
        }

        public static bool IzmjeniOpisStavke(string grad, _OpisiStavki opis, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RACUNI_STAVKE_OPI op = db.RACUNI_STAVKE_OPIs.First(i => i.IDOpisaStavke == opis.IDOpisaStavke);

                    op.IDStatusa = opis.IDStatusa;
                    op.IDZone = opis.IDZone;
                    op.IDRedarstva = opis.IDRedarstva;
                    op.Iznos = opis.Iznos;
                    op.NazivOpisaStavke = opis.NazivOpisaStavke;
                    op.Sifra = opis.Sifra;
                    op.Lezarina = opis.Lezarina;
                    op.KratkiOpis = opis.KratkiOpis;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJNEI OPIS STAVKE");
                return false;
            }
        }

        public static int DodajOpisStavke(string grad, _OpisiStavki opis, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RACUNI_STAVKE_OPI op = new RACUNI_STAVKE_OPI();

                    int id = 1;

                    if (db.RACUNI_STAVKE_OPIs.Any())
                    {
                        id = db.RACUNI_STAVKE_OPIs.Max(i => i.IDOpisaStavke) + 1;
                    }

                    op.IDOpisaStavke = id;
                    op.IDStatusa = opis.IDStatusa;
                    op.IDZone = opis.IDZone;
                    op.IDRedarstva = opis.IDRedarstva;
                    op.Iznos = opis.Iznos;
                    op.NazivOpisaStavke = opis.NazivOpisaStavke;
                    op.Sifra = opis.Sifra;
                    op.Lezarina = opis.Lezarina;
                    op.KratkiOpis = opis.KratkiOpis;

                    db.RACUNI_STAVKE_OPIs.InsertOnSubmit(op);
                    db.SubmitChanges();

                    return op.IDOpisaStavke;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ OPIS STAVKE");
                return -1;
            }
        }

        public static List<_OpisiStavki> DohvatiOpiseStavki(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (idRedarstva == 4)
                    {
                        List<_2DLista> statusi = Parking.Statusi(true, idAplikacije);

                        var os = from p in db.RACUNI_STAVKE_OPIs
                                 join z in db.PARKING_ZONEs on p.IDZone equals z.IDZone into zone
                                 from zz in zone.DefaultIfEmpty()
                                 where p.IDRedarstva == idRedarstva
                                 orderby p.NazivOpisaStavke
                                 select
                                     new _OpisiStavki(p.IDOpisaStavke, p.IDStatusa, p.IDZone, p.IDRedarstva, p.NazivOpisaStavke, Parking.Status(p.IDStatusa, statusi), p.Iznos, p.Lezarina, p.Obrisan, p.Sifra, p.KratkiOpis, zz.NazivZone);

                        return os.ToList();
                    }
                    else
                    {
                        var os = from p in db.RACUNI_STAVKE_OPIs
                                 join s in db.StatusPaukas on p.IDStatusa equals s.IDStatusa into stat
                                 from ss in stat.DefaultIfEmpty()
                                 where p.IDRedarstva == idRedarstva
                                 orderby p.NazivOpisaStavke
                                 select
                                     new _OpisiStavki(p.IDOpisaStavke, p.IDStatusa, p.IDZone, p.IDRedarstva, p.NazivOpisaStavke, ss.NazivStatusa, p.Iznos, p.Lezarina, p.Obrisan, p.Sifra, p.KratkiOpis, "");

                        return os.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI OPISE STAVKI");
                return new List<_OpisiStavki>();
            }
        }

        /*:: RACUNI ::*/

        public static _Racun DohvatiRacun(string grad, int idRacuna, bool mup, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              join d in db.Djelatniks on r.IDDjelatnika equals d.IDDjelatnika
                              join s in db.RACUNI_STORNAs on r.IDRacuna equals s.IDStorniranog into storno
                              from ss in storno.DefaultIfEmpty()
                              where r.IDRacuna == idRacuna
                              select new _Racun(r.IDRacuna, r.IDReference, r.IDZakljucenja, r.IDVrstePlacanja, r.IDVrsteKartice, r.IDBanke, VrstaPlacanja(grad, r.IDVrstePlacanja, idAplikacije), VrstaBanke(grad, r.IDBanke, idAplikacije),//
                                      r.IDDjelatnika, d.ImeNaRacunu, r.IDRedarstva, r.Datum, r.RB, r.Godina, r.PDV, r.Osnovica, r.Ukupno, r.PDVPosto, r.OIB, r.Blagajna, r.BrojRacuna,
                                      r.Storniran, r.Orginal, r.Napomena, r.Uplacen, r.JIR, r.ZKI, r.UUID, r.DatumPreuzimanja, r.PoslovniProstor, r.PozivNaBroj,
                                      r.BrojOdobrenja, VrstaKartice(grad, r.IDVrsteKartice, idAplikacije),
                                      RegistracijaRacuna(grad, r.IDReference, r.IDRedarstva, idAplikacije), r.Prenesen, r.Rate, d.Blagajna, ss != null ? (ss.Prilog != null ? ss.Prilog.Length != 0 : false) : false, StatusStorna(ss.IDStatusa),
                                      DohvatiStavkeRacuna(grad, idRacuna, idAplikacije),
                                      Osobe.DohvatiOsobeRacuna(grad, idRacuna, mup, idAplikacije));

                    return rac.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI RACUN");
                return null;
            }
        }

        public static _Racun DohvatiRacunReference(string grad, int idRefernece, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              join d in db.Djelatniks on r.IDDjelatnika equals d.IDDjelatnika
                              join s in db.RACUNI_STORNAs on r.IDRacuna equals s.IDStorniranog into storno
                              from ss in storno.DefaultIfEmpty()
                              where r.IDReference == idRefernece
                              select new _Racun(r.IDRacuna, r.IDReference, r.IDZakljucenja, r.IDVrstePlacanja, r.IDVrsteKartice, r.IDBanke, VrstaPlacanja(grad, r.IDVrstePlacanja, idAplikacije), VrstaBanke(grad, r.IDBanke, idAplikacije),//
                                  r.IDDjelatnika, d.ImeNaRacunu, r.IDRedarstva, r.Datum, r.RB, r.Godina, r.PDV, r.Osnovica, r.Ukupno, r.PDVPosto, r.OIB, r.Blagajna, r.BrojRacuna,
                                  r.Storniran, r.Orginal, r.Napomena, r.Uplacen, r.JIR, r.ZKI, r.UUID, r.DatumPreuzimanja, r.PoslovniProstor, r.PozivNaBroj,
                                  r.BrojOdobrenja, VrstaKartice(grad, r.IDVrsteKartice, idAplikacije),
                                  RegistracijaRacuna(grad, r.IDReference, r.IDRedarstva, idAplikacije), r.Prenesen, r.Rate, d.Blagajna, ss != null ? (ss.Prilog != null ? ss.Prilog.Length != 0 : false) : false, StatusStorna(ss.IDStatusa),
                                  DohvatiStavkeRacuna(grad, r.IDRacuna, idAplikacije),
                                  Osobe.DohvatiOsobeRacuna(grad, r.IDRacuna, true, idAplikacije));

                    return rac.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI RACUN");
                return null;
            }
        }

        public static _Racun DohvatiRacunLight(string grad, int idRacuna, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              join d in db.Djelatniks on r.IDDjelatnika equals d.IDDjelatnika
                              where r.IDRacuna == idRacuna
                              select new _Racun(r.IDRacuna, r.IDReference, r.IDZakljucenja, r.IDVrstePlacanja, r.IDVrsteKartice, r.IDBanke, VrstaPlacanja(grad, r.IDVrstePlacanja, idAplikacije), VrstaBanke(grad, r.IDBanke, idAplikacije), r.IDDjelatnika,
                                      d.ImeNaRacunu, r.IDRedarstva, r.Datum, r.RB, r.Godina, r.PDV, r.Osnovica, r.Ukupno, r.PDVPosto, r.OIB, r.Blagajna, r.BrojRacuna,
                                      r.Storniran, r.Orginal, r.Napomena, r.Uplacen, r.JIR, r.ZKI, r.UUID, r.DatumPreuzimanja, r.PoslovniProstor, r.PozivNaBroj, r.BrojOdobrenja,
                                      VrstaKartice(grad, r.IDVrsteKartice, idAplikacije), RegistracijaRacuna(grad, r.IDReference, r.IDRedarstva, idAplikacije), r.Prenesen, r.Rate, d.Blagajna, false, "",
                                      null, null);

                    return rac.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI RACUN");
                return null;
            }
        }

        public static _Racun DohvatiRacunPoziv(string grad, string poziv, out int idStatusa, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              join d in db.Djelatniks on r.IDDjelatnika equals d.IDDjelatnika
                              join s in db.RACUNI_STORNAs on r.IDRacuna equals s.IDStorniranog into storno
                              from ss in storno.DefaultIfEmpty()
                              where r.PozivNaBroj == poziv && 
                                    r.Storniran == false
                              select new _Racun(r.IDRacuna, r.IDReference, r.IDZakljucenja, r.IDVrstePlacanja, r.IDVrsteKartice, r.IDBanke, VrstaPlacanja(grad, r.IDVrstePlacanja, idAplikacije), VrstaBanke(grad, r.IDBanke, idAplikacije),//
                                  r.IDDjelatnika, d.ImeNaRacunu, r.IDRedarstva, r.Datum, r.RB, r.Godina, r.PDV, r.Osnovica, r.Ukupno, r.PDVPosto, r.OIB, r.Blagajna, r.BrojRacuna,
                                  r.Storniran, r.Orginal, r.Napomena, r.Uplacen, r.JIR, r.ZKI, r.UUID, r.DatumPreuzimanja, r.PoslovniProstor, r.PozivNaBroj,
                                  r.BrojOdobrenja, VrstaKartice(grad, r.IDVrsteKartice, idAplikacije),
                                  RegistracijaRacuna(grad, r.IDReference, r.IDRedarstva, idAplikacije), r.Prenesen, r.Rate, d.Blagajna, false, StatusStorna(ss.IDStatusa),
                                  DohvatiStavkeRacuna(grad, r.IDRacuna, idAplikacije),
                                  Osobe.DohvatiOsobeRacuna(grad, r.IDRacuna, true, idAplikacije));

                    if (!rac.Any())
                    {
                        idStatusa = -1;
                        return null;
                    }

                    idStatusa = db.NaloziPaukus.First(i => i.IDNaloga == rac.First().IDReference).IDStatusa;
                    return rac.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI RACUN");
                idStatusa = -1;
                return null;
            }
        }

        public static List<_Stavka> DohvatiStavkeRacuna(string grad, int idRacuna, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var sta = from s in db.RACUNI_STAVKEs
                              join o in db.RACUNI_STAVKE_OPIs on s.IDOpisaStavke equals o.IDOpisaStavke
                              where s.IDRacuna == idRacuna
                              select new _Stavka(s.IDStavke, s.IDRacuna, s.IDOpisaStavke, o.NazivOpisaStavke, o.Lezarina, (int)s.Kolicina, s.Cijena, s.Pdv, s.Osnovica, s.Ukupno, s.PdvPosto, s.Napomena);

                    return sta.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI STAVKE RACUNA");
                return new List<_Stavka>();
            }
        }

        public static _Prekrsaj DetaljiPrekrsaja(string grad, int idReference, int idRedarstva, int idAplikacije)
        {
            try
            {
                if (idRedarstva == 1)
                {
                    return Prekrsaj.DetaljiPrekrsaja(grad, idReference, idAplikacije);
                }

                if (idRedarstva == 2)
                {
                    return Prekrsaj.DetaljiPrekrsajaNalog(grad, idReference, idAplikacije);
                }

                return null;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DETALJI PREKRSAJA");
                return null;
            }
        }

        /*:: ZAKLJUCENJE ::*/

        public static int ZakljuciBlagajnu(string grad, _Zakljucenje zakljucenje, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RACUNI_ZAKLJUCENJA rz = new RACUNI_ZAKLJUCENJA();

                    int id = 1;

                    if (db.RACUNI_ZAKLJUCENJAs.Any())
                    {
                        id = db.RACUNI_ZAKLJUCENJAs.Max(i => i.IDZakljucenja) + 1;
                    }

                    rz.IDZakljucenja = id;
                    rz.IDDjelatnika = zakljucenje.IDDjelatnika;
                    rz.IDRedarstva = zakljucenje.IDRedarstva;
                    rz.Vrijeme = zakljucenje.Vrijeme;
                    rz.Ukupno = zakljucenje.Ukupno;
                    rz.Broj = zakljucenje.Broj;
                    rz.Oznaka = zakljucenje.Oznaka;

                    db.RACUNI_ZAKLJUCENJAs.InsertOnSubmit(rz);
                    db.SubmitChanges();

                    foreach (var r in zakljucenje.Racuni)
                    {
                        try
                        {
                            db.RACUNIs.First(i => i.IDRacuna == r).IDZakljucenja = id;
                            db.SubmitChanges();
                        }
                        catch (Exception)
                        {
                            Sustav.SpremiGresku(grad, new ArgumentNullException("NIJE ZAKLJUCEN RAČUN ID: " + r), idAplikacije, "SPREMI ZAKLJUCENJE");
                        }
                    }

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI DOKUMENT");
                return -1;
            }
        }

        public static List<_Zakljucenje> DohvatiZakljucenja(string grad, DateTime? datumOd, DateTime? datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var zaklj = from z in db.RACUNI_ZAKLJUCENJAs
                                join d in db.Djelatniks on z.IDDjelatnika equals d.IDDjelatnika into djelatnik
                                from dd in djelatnik.DefaultIfEmpty()
                                where z.IDRedarstva == idRedarstva &&
                                    (datumOd != null ? z.Vrijeme.Date >= datumOd : datumOd == null) &&
                                    (datumDo != null ? z.Vrijeme.Date <= datumDo : datumDo == null)
                                select new _Zakljucenje(z.IDZakljucenja, z.IDDjelatnika, z.IDRedarstva, dd.ImePrezime, z.Vrijeme, z.Ukupno, z.Broj, z.Oznaka, null);

                    return zaklj.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI ZAKLJUCENJA");
                return new List<_Zakljucenje>();
            }
        }

        public static List<_Racun> DohvatiPopisRacunaZakljucenja(string grad, int idZakljucenja, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              join d in db.Djelatniks on r.IDDjelatnika equals d.IDDjelatnika
                              where r.IDZakljucenja == idZakljucenja
                              select new _Racun(r.IDRacuna, r.IDReference, r.IDZakljucenja, r.IDVrstePlacanja, r.IDVrsteKartice, r.IDBanke, VrstaPlacanja(grad, r.IDVrstePlacanja, idAplikacije), VrstaBanke(grad, r.IDBanke, idAplikacije), r.IDDjelatnika,
                                      d.ImeNaRacunu, r.IDRedarstva, r.Datum, r.RB, r.Godina, r.PDV, r.Osnovica, r.Ukupno, r.PDVPosto, r.OIB, r.Blagajna, r.BrojRacuna,
                                      r.Storniran, r.Orginal, r.Napomena, r.Uplacen, r.JIR, r.ZKI, r.UUID, r.DatumPreuzimanja, r.PoslovniProstor, r.PozivNaBroj, r.BrojOdobrenja,
                                      VrstaKartice(grad, r.IDVrsteKartice, idAplikacije), RegistracijaRacuna(grad, r.IDReference, r.IDRedarstva, idAplikacije), r.Prenesen, r.Rate, d.Blagajna, false, "",
                                      DohvatiStavkeRacuna(grad, r.IDRacuna, idAplikacije), null);

                    return rac.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI RACUN");
                return new List<_Racun>();
            }
        }

        public static bool ZakljuciZaostatke(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              where r.IDZakljucenja == null &&
                                    r.Datum.Date < DateTime.Today &&
                                    r.IDRedarstva == idRedarstva
                              select r;

                    foreach (var r in rac.GroupBy(i => new { i.Datum.Date, i.IDDjelatnika }))
                    {
                        List<int> racuni = new List<int>();

                        foreach (var q in r)
                        {
                            racuni.Add(q.IDRacuna);
                        }

                        _Zakljucenje zak = new _Zakljucenje(0, r.Key.IDDjelatnika, idRedarstva, "", r.Key.Date, r.Sum(i => i.Ukupno), r.Count(), r.First().PoslovniProstor, racuni);

                        ZakljuciBlagajnu(grad, zak, idAplikacije);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI DOKUMENT");
                return false;
            }
        }

        /*:: BLAGAJNIČKI DNEVNIK :: */

        public static void BlagajnickiDnevnik(string grad, DateTime datum, int idDjelatnika, int idRedarstva, decimal polog, int idAplikacije)
        {
            try
            {
                //todo šta ako postoji za datum? - pregaziti ga s novim podacima - update
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {

                    BLAGAJNICKI_DNEVNIK bd = new BLAGAJNICKI_DNEVNIK();

                    int id = 1;

                    if (db.BLAGAJNICKI_DNEVNIKs.Any())
                    {
                        id = db.BLAGAJNICKI_DNEVNIKs.Max(i => i.IDDnevnika) + 1;
                    }

                    bd.IDDnevnika = id;
                    bd.IDRedarstva = idRedarstva;
                    bd.IDDjelatnika = idDjelatnika;
                    bd.Datum = datum;
                    bd.Polog = polog;
                    bd.Promet = 0;// dnevniUtrzak;

                    db.BLAGAJNICKI_DNEVNIKs.InsertOnSubmit(bd);
                    db.SubmitChanges();

                    //ovo je ok dok ne počnu naplačivati gotovinu na terenu
                    foreach (var nm in db.NAPLATNA_MJESTAs.Where(i => !i.Neaktivno))
                    {
                        decimal dnevniUtrzak = 0;
                        if (db.RACUNIs.Any(i => i.IDVrstePlacanja != 4 && i.IDVrstePlacanja != 5 && i.Datum.Date == datum.Date && i.PoslovniProstor == nm.OznakaNaplatnogMjesta))
                        {
                            dnevniUtrzak = db.RACUNIs.Where(i => i.IDVrstePlacanja != 4 && i.IDVrstePlacanja != 5 && i.Datum.Date == datum.Date && i.PoslovniProstor == nm.OznakaNaplatnogMjesta).Sum(i => i.Ukupno);
                        }

                        BlagajnickiDnevnikStavka(grad, id, null, null, "DNEVNI UTRŽAK", "U", nm.Sifra, dnevniUtrzak, 0, 10, false, idAplikacije);

                        //Smjene
                        foreach (var z in db.RACUNI_ZAKLJUCENJAs.Where(i => i.Vrijeme.Date == datum.Date && i.Oznaka == nm.OznakaNaplatnogMjesta).OrderBy(i => i.Vrijeme))
                        {
                            string opis;
                            int red;

                            if (z.Vrijeme.Hour > 17)
                            {
                                opis = "POLOG II SMJENA";
                                red = 12;
                            }
                            else
                            {
                                opis = "POLOG I SMJENA";
                                red = 11;
                            }

                            decimal smjena = 0;

                            if (db.RACUNIs.Any(i => i.IDZakljucenja == z.IDZakljucenja && i.IDVrstePlacanja == 1))
                            {
                                smjena = db.RACUNIs.Where(i => i.IDZakljucenja == z.IDZakljucenja && i.IDVrstePlacanja == 1)
                                    .Sum(i => i.Ukupno);
                            }

                            BlagajnickiDnevnikStavka(grad, id, null, null, opis, "I", nm.Sifra, 0, smjena, red, false, idAplikacije);
                        }

                        //kartice
                        if (db.RACUNIs.Any(i => (i.IDVrstePlacanja == 2 || i.IDVrstePlacanja == 3) && i.Datum.Date == datum.Date && i.PoslovniProstor == nm.OznakaNaplatnogMjesta))
                        {
                            foreach (var r in db.RACUNIs.Where(i => (i.IDVrstePlacanja == 2 || i.IDVrstePlacanja == 3) && i.Datum.Date == datum.Date && i.PoslovniProstor == nm.OznakaNaplatnogMjesta).GroupBy(i => i.IDBanke))
                            {
                                foreach (var k in r.GroupBy(i => i.IDVrsteKartice))
                                {
                                    foreach (var g in k.GroupBy(i => i.Rate))
                                    {
                                        int red = r.Key == 1 ? 30 : 20;

                                        string opis = string.Format("KARTICE {0} ({1}{2})", VrstaBanke(grad, r.Key, idAplikacije), VrstaKarticeKratica(grad, k.Key, idAplikacije), g.Key == true ? "R" : "");
                                        BlagajnickiDnevnikStavka(grad, id, r.Key, k.Key, opis, "I", nm.Sifra, 0, g.Sum(i => i.Ukupno), red, g.Key == true, idAplikacije);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "BLAGAJNICKI DNEVNIK");
            }
        }

        public static bool BlagajnickiDnevnikStavka(string grad, int idDnevnika, int? idBanke, int? idKartice, string opis, string tip, string naplatno, decimal primitak, decimal izdatak, int redoslijed, bool rate, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.BLAGAJNICKI_DNEVNIK_STAVKEs.Any(i => i.IDDnevnika == idDnevnika && i.Opis == opis && i.IDBanke == idBanke))
                    {
                        //zao jer imaju mastercared i maestro zajedno, tko zna zasto..
                        BLAGAJNICKI_DNEVNIK_STAVKE s = db.BLAGAJNICKI_DNEVNIK_STAVKEs.First(i => i.IDDnevnika == idDnevnika && i.Opis == opis && i.IDBanke == idBanke);

                        s.Primitak += primitak;
                        s.Izdatak += izdatak;

                        db.SubmitChanges();

                        return true;
                    }

                    BLAGAJNICKI_DNEVNIK_STAVKE bds = new BLAGAJNICKI_DNEVNIK_STAVKE();

                    int id = 1;

                    if (db.BLAGAJNICKI_DNEVNIK_STAVKEs.Any())
                    {
                        id = db.BLAGAJNICKI_DNEVNIK_STAVKEs.Max(i => i.IDStavkeDnevnika) + 1;
                    }

                    int rb = 1;

                    if (db.BLAGAJNICKI_DNEVNIK_STAVKEs.Any(i => i.Tip == tip && i.NaplatnoMjesto == naplatno))
                    {
                        rb = db.BLAGAJNICKI_DNEVNIK_STAVKEs.Where(i => i.Tip == tip && i.NaplatnoMjesto == naplatno).Max(i => i.RB) + 1;
                    }

                    bds.IDStavkeDnevnika = id;
                    bds.IDDnevnika = idDnevnika;
                    bds.IDBanke = idBanke;
                    bds.IDVrsteKartice = idKartice;
                    bds.RB = rb;
                    bds.Tip = tip;
                    bds.Opis = opis;
                    bds.Primitak = primitak;
                    bds.Izdatak = izdatak;
                    bds.Redosljed = redoslijed;
                    bds.Rate = rate;
                    bds.NaplatnoMjesto = naplatno;

                    db.BLAGAJNICKI_DNEVNIK_STAVKEs.InsertOnSubmit(bds);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "BLAGAJNICKI DNEVNIK STAVKE");
                return false;
            }
        }

        /*:: PRETRAGA ::*/

        public static List<_Racun> DohvatiPopisRacuna(string grad, DateTime? datumOd, DateTime? datumDo, int idDjelatnika, bool fisk, string brrac, string poziv, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              join d in db.Djelatniks on r.IDDjelatnika equals d.IDDjelatnika
                              join s in db.RACUNI_STORNAs on r.IDRacuna equals s.IDStorniranog into storno
                              from ss in storno.DefaultIfEmpty()
                              where r.IDRedarstva == idRedarstva &&
                                    (idDjelatnika != 0 ? r.IDDjelatnika == idDjelatnika : idDjelatnika == 0) &&
                                    (datumOd != null ? r.Datum.Date >= datumOd : datumOd == null) &&
                                    (datumDo != null ? r.Datum.Date <= datumDo : datumDo == null) &&
                                    (brrac != "" ? r.BrojRacuna.ToUpper().Contains(brrac.ToUpper()) : brrac == "") &&
                                    (poziv != "" ? r.PozivNaBroj.ToUpper().Contains(poziv) : poziv == "") &&
                                    (fisk ? r.JIR == "" && (r.IDVrstePlacanja != 4 && r.IDVrstePlacanja != 5) : fisk == false)
                              select
                                  new _Racun(r.IDRacuna, r.IDReference, r.IDZakljucenja, r.IDVrstePlacanja, r.IDVrsteKartice, r.IDBanke,
                                      VrstaPlacanja(grad, r.IDVrstePlacanja, idAplikacije), VrstaBanke(grad, r.IDBanke, idAplikacije), r.IDDjelatnika,
                                      d.ImeNaRacunu, r.IDRedarstva, r.Datum, r.RB, r.Godina, r.PDV, r.Osnovica, r.Ukupno,
                                      r.PDVPosto, r.OIB, r.Blagajna, r.BrojRacuna,
                                      r.Storniran, r.Orginal, r.Napomena, r.Uplacen, r.JIR, r.ZKI, r.UUID, r.DatumPreuzimanja,
                                      r.PoslovniProstor, r.PozivNaBroj, r.BrojOdobrenja, VrstaKartice(grad, r.IDVrsteKartice, idAplikacije),
                                      RegistracijaRacuna(grad, r.IDReference, r.IDRedarstva, idAplikacije), r.Prenesen, r.Rate, d.Blagajna, ss != null ? (ss.Prilog != null ? ss.Prilog.Length != 0 : false) : false, StatusStorna(ss.IDStatusa),
                                      null,//DohvatiStavkeRacuna(grad, r.IDRacuna, idAplikacije),
                                      Osobe.DohvatiOsobeRacuna(grad, r.IDRacuna, true, idAplikacije));//DetaljiPrekrsaja(grad, r.IDReference, idRedarstva, idAplikacije));

                    return rac.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI RACUN");
                return new List<_Racun>();
            }
        }

        public static List<_Racun> DohvatiPopisRacunaOsoba(string grad, string ime, string prezime, string oib, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var osobe = (from o in db.RACUNI_OSOBEs
                                 join or in db.RACUNI_OSOBE_RELACIJEs on o.IDOsobe equals or.IDOsobe
                                 where (ime != "" ? o.Ime.ToUpper().Contains(ime) : ime == "") &&
                                       (prezime != "" ? o.Prezime.ToUpper().Contains(prezime) : prezime == "") &&
                                       (oib != "" ? o.OIB.Contains(oib) : oib == "")
                                 select or.IDRacuna).Distinct();

                    List<_Racun> racuni = new List<_Racun>();

                    foreach (var id in osobe)
                    {
                        var rac = from r in db.RACUNIs
                                  join d in db.Djelatniks on r.IDDjelatnika equals d.IDDjelatnika
                                  join s in db.RACUNI_STORNAs on r.IDRacuna equals s.IDStorniranog into storno
                                  from ss in storno.DefaultIfEmpty()
                                  where r.IDRacuna == id &&
                                        r.IDRedarstva == idRedarstva
                                  select new _Racun(r.IDRacuna, r.IDReference, r.IDZakljucenja, r.IDVrstePlacanja, r.IDVrsteKartice, r.IDBanke, VrstaPlacanja(grad, r.IDVrstePlacanja, idAplikacije), VrstaBanke(grad, r.IDBanke, idAplikacije), r.IDDjelatnika,
                                          d.ImeNaRacunu, r.IDRedarstva, r.Datum, r.RB, r.Godina, r.PDV, r.Osnovica, r.Ukupno, r.PDVPosto, r.OIB, r.Blagajna, r.BrojRacuna,
                                          r.Storniran, r.Orginal, r.Napomena, r.Uplacen, r.JIR, r.ZKI, r.UUID, r.DatumPreuzimanja, r.PoslovniProstor, r.PozivNaBroj, r.BrojOdobrenja,
                                          VrstaKartice(grad, r.IDVrsteKartice, idAplikacije), RegistracijaRacuna(grad, r.IDReference, r.IDRedarstva, idAplikacije), r.Prenesen, r.Rate, d.Blagajna, ss != null ? (ss.Prilog != null ? ss.Prilog.Length != 0 : false) : false, StatusStorna(ss.IDStatusa),
                                          DohvatiStavkeRacuna(grad, r.IDRacuna, idAplikacije),
                                          Osobe.DohvatiOsobeRacuna(grad, r.IDRacuna, true, idAplikacije));

                        racuni.Add(rac.First());
                    }

                    return racuni;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI RACUN PO OSOBI");
                return new List<_Racun>();
            }
        }

        public static List<_Racun> DohvatiPopisRacunaStorno(string grad, DateTime? datumOd, DateTime? datumDo, int idStatusa, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              join d in db.Djelatniks on r.IDDjelatnika equals d.IDDjelatnika
                              join s in db.RACUNI_STORNAs on r.IDRacuna equals s.IDStorniranog into storno
                              from ss in storno.DefaultIfEmpty()
                              where r.IDRedarstva == idRedarstva &&
                                    (datumOd != null ? r.Datum.Date >= datumOd : datumOd == null) &&
                                    (datumDo != null ? r.Datum.Date <= datumDo : datumDo == null) &&
                                    (idStatusa != 0 ? ss.IDStatusa == idStatusa : idStatusa == 0) 
                              select
                                  new _Racun(r.IDRacuna, r.IDReference, r.IDZakljucenja, r.IDVrstePlacanja, r.IDVrsteKartice, r.IDBanke,
                                      VrstaPlacanja(grad, r.IDVrstePlacanja, idAplikacije), VrstaBanke(grad, r.IDBanke, idAplikacije), r.IDDjelatnika,
                                      d.ImeNaRacunu, r.IDRedarstva, r.Datum, r.RB, r.Godina, r.PDV, r.Osnovica, r.Ukupno,
                                      r.PDVPosto, r.OIB, r.Blagajna, r.BrojRacuna,
                                      r.Storniran, r.Orginal, r.Napomena, r.Uplacen, r.JIR, r.ZKI, r.UUID, r.DatumPreuzimanja,
                                      r.PoslovniProstor, r.PozivNaBroj, r.BrojOdobrenja, VrstaKartice(grad, r.IDVrsteKartice, idAplikacije),
                                      RegistracijaRacuna(grad, r.IDReference, r.IDRedarstva, idAplikacije), r.Prenesen, r.Rate, d.Blagajna, ss != null ? (ss.Prilog != null ? ss.Prilog.Length != 0 : false) : false, StatusStorna(ss.IDStatusa),
                                      null,//DohvatiStavkeRacuna(grad, r.IDRacuna, idAplikacije),
                                      Osobe.DohvatiOsobeRacuna(grad, r.IDRacuna, true, idAplikacije));//DetaljiPrekrsaja(grad, r.IDReference, idRedarstva, idAplikacije));

                    return rac.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI RACUN");
                return new List<_Racun>();
            }
        }

        public static List<_Racun> DohvatiPopisRacunaZakljucenje(string grad, int? idDjelatnika, string oznaka, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              join d in db.Djelatniks on r.IDDjelatnika equals d.IDDjelatnika
                              where r.IDRedarstva == idRedarstva &&
                                    r.IDZakljucenja == null &&
                                    (idDjelatnika != null ? r.IDDjelatnika == idDjelatnika : idDjelatnika == null) &&
                                    (oznaka != "" ? r.PoslovniProstor == oznaka : oznaka == "")
                              select new _Racun(r.IDRacuna, r.IDReference, r.IDZakljucenja, r.IDVrstePlacanja, r.IDVrsteKartice, r.IDBanke, VrstaPlacanja(grad, r.IDVrstePlacanja, idAplikacije), VrstaBanke(grad, r.IDBanke, idAplikacije), r.IDDjelatnika,
                                      d.ImeNaRacunu, r.IDRedarstva, r.Datum, r.RB, r.Godina, r.PDV, r.Osnovica, r.Ukupno, r.PDVPosto, r.OIB, r.Blagajna, r.BrojRacuna,
                                      r.Storniran, r.Orginal, r.Napomena, r.Uplacen, r.JIR, r.ZKI, r.UUID, r.DatumPreuzimanja, r.PoslovniProstor, r.PozivNaBroj, r.BrojOdobrenja,
                                      VrstaKartice(grad, r.IDVrsteKartice, idAplikacije), RegistracijaRacuna(grad, r.IDReference, r.IDRedarstva, idAplikacije), r.Prenesen, r.Rate, d.Blagajna, false, "",
                                      null, null);

                    return rac.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI POPIS ZAKLJUCENJA");
                return new List<_Racun>();
            }
        }

        public static int Nezakljuceni(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.RACUNIs.Count(i => i.IDZakljucenja == null && i.IDRedarstva == idRedarstva);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NEZAKLJUCENI");
                return -1;
            }
        }

        public static bool ProgramskoZakljucivanjeZaostalih(string grad, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              where r.IDRedarstva == idRedarstva &&
                                    r.IDZakljucenja == null &&
                                    r.Datum.Date < datumDo.Date
                              select new { r.IDRacuna, r.Ukupno, r.Datum };

                    foreach (var r in rac.GroupBy(i => i.Datum.Date))
                    {
                        _Zakljucenje zak = new _Zakljucenje(0, 16, 2, "Ivan Varvodić", DateTime.Now, r.Sum(i => i.Ukupno), r.Count(), null, r.Select(i => i.IDRacuna).ToList());
                        ZakljuciBlagajnu(grad, zak, idAplikacije);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, " PROGRAMSKA ZAKLJUCENJA");
                return false;
            }
        }

        /*:: KARTICA ::*/

        public static bool IzmjeniKarticuPlacanja(string grad, int idRacuna, int? idBanke, int idKartice, string odobrenje, bool? rate, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RACUNI r = db.RACUNIs.First(i => i.IDRacuna == idRacuna);

                    r.IDBanke = idBanke;
                    r.IDVrsteKartice = idKartice;
                    r.BrojOdobrenja = odobrenje;
                    r.Rate = rate;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI KARTICU PLACANJA");
                return false;
            }
        }

        /*:: REGISTRACIJA ::*/

        public static string RegistracijaRacuna(string grad, int id, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    switch (idRedarstva)
                    {
                        case 1:
                            return db.Prekrsajis.First(i => i.IDPrekrsaja == id).RegistracijskaPlocica;
                        case 2:
                            return db.Prekrsajis.First(i => i.IDNaloga == id).RegistracijskaPlocica;
                        case 4:
                            if (db.PARKING_OPAZANJAs.Any(i => i.IDOpazanja == id))
                            {
                                return db.PARKING_OPAZANJAs.First(i => i.IDOpazanja == id).Registracija;
                            }
                            return "";
                    }

                    return "";
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "REGISTRACIJA RACUNA");
                return "";
            }
        }

        /*:: PRIJENOS U RACUNOVODSTVO ::*/

        public static bool? Prenesi(string grad, DateTime datumOd, DateTime datumDo, out string poruka, int idAplikacije)
        {
            poruka = "";

            try
            {
                if (grad == "Lokacije")
                {
                    //Prijenos.url = "http://ritamapi.dyndns.ws:8920/rest/api/v1";
                    //Prijenos.key = "ritam";
                    //Prijenos.username = "test";

                    //string placeni, odgoda;
                    //bool okplac = Prijenos.Placeni(grad, datumOd, datumDo, out placeni, idAplikacije);
                    //bool okodg = Prijenos.Odgode(grad, datumOd, datumDo, out odgoda, idAplikacije);

                    //poruka = "GOTOVINA, KARTICE\r\n" + placeni + "\r\n\r\nODGODE\r\n" + odgoda;

                    //if (okplac && okodg)
                    //{
                    //    return true;
                    //}

                    //return false;

                    string odgoda, temeljnica;
                    bool okodg = Prijenos.OdgodeR(grad, datumOd, datumDo, new List<string>() { "daniel.pajalic@ri-ing.net" }, out odgoda, idAplikacije);

                    poruka = "ODGODE\r\n" + odgoda;

                    bool oktem = Prijenos.TemeljnicaR(grad, datumOd, new List<string>() { "daniel.pajalic@ri-ing.net" }, out temeljnica, idAplikacije);

                    poruka += "\r\n\r\nTEMELJNICA\r\n" + temeljnica;

                    if (okodg && oktem)
                    {
                        return true;
                    }

                    return false;
                }

                if (grad.ToUpper() == "PROMETNIK_SPLIT")
                {
                    Prijenos.url = "http://ritamapi.dyndns.ws:8920/rest/api/v1";
                    Prijenos.key = "15hvi25";
                    Prijenos.username = "hvidra15_ws";

                    string placeni, odgoda;
                    bool okplac = Prijenos.Placeni(grad, datumOd, datumDo, out placeni, idAplikacije);
                    bool okodg = Prijenos.Odgode(grad, datumOd, datumDo, out odgoda, idAplikacije);

                    if (string.IsNullOrEmpty(odgoda))
                    {
                        odgoda = "podaci su uspješno preseni.";
                    }

                    poruka = "GOTOVINA, KARTICE\r\n" + placeni + "\r\n\r\nODGODE\r\n" + odgoda;

                    if (okplac && okodg)
                    {
                        return true;
                    }

                    return false;
                }

                if (grad.ToUpper() == "PROMETNIK_RIJEKA")
                {
                    string odgoda, temeljnica;

                    bool okodg = Prijenos.OdgodeR(grad, datumOd, datumDo, new List<string>() { "vladimir.simunic@rijeka-plus.hr" }, out odgoda, idAplikacije);
                    //bool okodg = Prijenos.OdgodeR(grad, datumOd, datumDo, new List<string>() { "daniel.pajalic@ri-ing.net" }, out odgoda, idAplikacije);

                    poruka = "ODGODE\r\n" + odgoda;

                    bool oktem = Prijenos.TemeljnicaR(grad, datumOd, new List<string>() { "vladimir.simunic@rijeka-plus.hr" }, out temeljnica, idAplikacije);
                    //bool oktem = Prijenos.TemeljnicaR(grad, datumOd, new List<string>() { "daniel.pajalic@ri-ing.net" }, out temeljnica, idAplikacije);

                    poruka += "\r\n\r\nTEMELJNICA\r\n" + temeljnica;

                    if (okodg && oktem)
                    {
                        return true;
                    }

                    return false;
                }

                return null;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIJENOS U RAČUNOVODSTVO");
                return false;
            }
        }

        /*:: DOHVAT PODATAKA OD MUPA ::*/

        public static List<_Racun> DohvatiPopisRacunaMUP(string grad, DateTime? datumOd, DateTime? datumDo, string brrac, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              join d in db.Djelatniks on r.IDDjelatnika equals d.IDDjelatnika
                              where r.IDRedarstva == idRedarstva &&
                                    (r.IDVrstePlacanja == 4 || r.IDVrstePlacanja == 5) &&
                                    (datumOd != null ? r.Datum.Date >= datumOd : datumOd == null) &&
                                    (datumDo != null ? r.Datum.Date <= datumDo : datumDo == null) &&
                                    (brrac != "" ? r.BrojRacuna.ToUpper().Contains(brrac.ToUpper()) : brrac == "")
                              select
                                  new _Racun(r.IDRacuna, r.IDReference, r.IDZakljucenja, r.IDVrstePlacanja, r.IDVrsteKartice, r.IDBanke,
                                      VrstaPlacanja(grad, r.IDVrstePlacanja, idAplikacije), VrstaBanke(grad, r.IDBanke, idAplikacije), r.IDDjelatnika,
                                      d.ImeNaRacunu, r.IDRedarstva, r.Datum, r.RB, r.Godina, r.PDV, r.Osnovica, r.Ukupno,
                                      r.PDVPosto, r.OIB, r.Blagajna, r.BrojRacuna,
                                      r.Storniran, r.Orginal, r.Napomena, r.Uplacen, r.JIR, r.ZKI, r.UUID, r.DatumPreuzimanja,
                                      r.PoslovniProstor, r.PozivNaBroj, r.BrojOdobrenja, VrstaKartice(grad, r.IDVrsteKartice, idAplikacije),
                                      RegistracijaRacuna(grad, r.IDReference, r.IDRedarstva, idAplikacije), r.Prenesen, r.Rate, d.Blagajna, false, "",
                                      null, DohvatiOsobeMup(grad, r.IDReference, r.IDRedarstva, idAplikacije));

                    return rac.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI RACUN MUP");
                return new List<_Racun>();
            }
        }

        private static List<_Osoba> DohvatiOsobeMup(string grad, int idReference, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int idPrekrsaja;

                    switch (idRedarstva)
                    {
                        case 1://todo
                            idPrekrsaja = idReference;
                            break;
                        case 2:
                            idPrekrsaja = db.Prekrsajis.First(i => i.IDNaloga == idReference).IDPrekrsaja;
                            break;
                        default:
                            idPrekrsaja = -1;
                            break;
                    }

                    var mup = from m in db.MUPs
                              where m.IDPrekrsaja == idPrekrsaja &&
                                    m.IDRedarstva == idRedarstva
                              select m;

                    List<_Osoba> osobe = new List<_Osoba>();

                    foreach (var m in mup)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(m.Ime);
                        sb.AppendLine(m.Prezime);
                        sb.AppendLine(m.Naziv);
                        sb.AppendLine(m.OIB);
                        sb.AppendLine(m.Adresa);
                        sb.AppendLine(m.Mjesto);
                        sb.AppendLine(m.Opcina);
                        sb.AppendLine(m.Drzava);
                        sb.AppendLine("----------------");
                        sb.AppendLine(m.Registracija);
                        sb.AppendLine(m.Vrsta);
                        sb.AppendLine(m.Marka);

                        osobe.Add(new _Osoba(0, "", "", "", "", "", "", "", "", sb.ToString(), "", null, m.Vlasnik, true));
                    }

                    return osobe;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI OSOBE MUP");
                return new List<_Osoba>();
            }
        }

        public static List<_Osoba> DohvatMUPa(string grad, _Racun racun, int idKorisnika, int idAplikacije)
        {
            try
            {
                List<_Osoba> osoba = new List<_Osoba>();
                List<_Osoba> vk = new List<_Osoba>();

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    int idPrekrsaja;
                    string registracija = "";
                    DateTime datum = new DateTime();
                    bool direktniupis = false;
                    using (PazigradDataContext pdb = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                    {
                        GRADOVI g = db.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad));
                        POSLOVNI_PROSTOR prostor = pdb.POSLOVNI_PROSTORs.First(i => i.IDRedarstva == racun.IDRedarstva);

                        if (prostor == null)
                        {
                            return null;
                        }

                        string oib = prostor.OIB;
                        string lokacija = string.Format("{0}, {1}", prostor.Mjesto, prostor.Naziv);

                        switch (racun.IDRedarstva)
                        {
                            case 1:
                                Prekrsaji p = pdb.Prekrsajis.First(i => i.IDPrekrsaja == racun.IDReference);
                                idPrekrsaja = p.IDPrekrsaja;
                                registracija = p.RegistracijskaPlocica;
                                datum = p.Vrijeme.Value;
                                //lokacija = g.NazivGrada;
                                break;
                            case 2:
                                Prekrsaji p1 = pdb.Prekrsajis.First(i => i.IDNaloga == racun.IDReference);
                                idPrekrsaja = p1.IDPrekrsaja;
                                registracija = p1.RegistracijskaPlocica;
                                datum = p1.Vrijeme.Value;
                                direktniupis = g.DohvatVlasnikaMUP;
                                break;
                            default:
                                idPrekrsaja = -1;
                                break;
                        }

                        if (idPrekrsaja == -1)
                        {
                            return null;
                        }

                        _OdgovorMUPVozilo osobe = MUPParkingVozilo(oib, registracija, datum, lokacija, idAplikacije);

                        if (osobe == null)
                        {
                            return null;
                        }

                        if (direktniupis)
                        {
                            foreach (var v in osobe.Vlasnik)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(v.ImeVlasnika);
                                sb.AppendLine(v.PrezimeVlasnika);
                                sb.AppendLine(v.NazivVlasnika);
                                sb.AppendLine(v.OIBVlasnika);
                                sb.AppendLine(v.UlicaBroj);
                                sb.AppendLine(v.Mjesto);
                                sb.AppendLine(v.Opcina);
                                sb.AppendLine(v.Drzava);
                                sb.AppendLine("----------------");
                                sb.AppendLine(osobe.Vozilo.RegistarskaOznaka);
                                sb.AppendLine(osobe.Vozilo.VrstaVozila);
                                sb.AppendLine(osobe.Vozilo.MarkaVozila);

                                vk.Add(new _Osoba(0, string.IsNullOrEmpty(v.ImeVlasnika) ? v.NazivVlasnika : v.ImeVlasnika, v.PrezimeVlasnika, v.UlicaBroj, "", "", v.Mjesto, v.Drzava, v.OIBVlasnika, sb.ToString(), "", null, true, true));
                            }

                            Osobe.SpremiOsobe(grad, vk, racun.IDRacuna, true, true, idAplikacije);
                        }

                        SpremiMUP(grad, idPrekrsaja, racun.IDRedarstva, osobe, idAplikacije);

                        foreach (var v in osobe.Vlasnik)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(v.ImeVlasnika);
                            sb.AppendLine(v.PrezimeVlasnika);
                            sb.AppendLine(v.NazivVlasnika);
                            sb.AppendLine(v.OIBVlasnika);
                            sb.AppendLine(v.UlicaBroj);
                            sb.AppendLine(v.Mjesto);
                            sb.AppendLine(v.Opcina);
                            sb.AppendLine(v.Drzava);
                            sb.AppendLine("----------------");
                            sb.AppendLine(osobe.Vozilo.RegistarskaOznaka);
                            sb.AppendLine(osobe.Vozilo.VrstaVozila);
                            sb.AppendLine(osobe.Vozilo.MarkaVozila);

                            osoba.Add(new _Osoba(0, "", "", "", "", "", "", "", "", sb.ToString(), "", null, true, true));
                        }
                    }

                    //spremi u tablicu prema cemu je isao upit - dodati testni upit bool polje?
                    try
                    {
                        DOHVAT_MUP dm = new DOHVAT_MUP();

                        int id = 1;

                        if (db.DOHVAT_MUPs.Any())
                        {
                            id = db.DOHVAT_MUPs.Max(i => i.IDDohvata) + 1;
                        }

                        dm.IDDohvata = id;
                        dm.IDGrada = Sistem.IDGrada(grad);
                        dm.IDRedarstva = racun.IDRedarstva;
                        dm.IDKorisnika = idKorisnika;
                        dm.IDPrekrsaja = idPrekrsaja;
                        dm.Vrijeme = DateTime.Now;
                        dm.Registracija = racun.Registracija;

                        db.DOHVAT_MUPs.InsertOnSubmit(dm);
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVAT OD MUPA - SPREMI U TABLICU");
                    }

                    if (direktniupis)
                    {
                        return vk;
                    }

                    return osoba; //vratit osobu
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVAT OD MUPA");
                return null;
            }
        }

        public static bool SpremiMUP(string grad, int idPrekrsaja, int idRedarstva, _OdgovorMUPVozilo osobe, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    foreach (var m in osobe.Vlasnik)
                    {
                        MUP dm = new MUP();

                        int id = 1;

                        if (db.MUPs.Any())
                        {
                            id = db.MUPs.Max(i => i.IDMUP) + 1;
                        }

                        dm.IDMUP = id;
                        dm.IDPrekrsaja = idPrekrsaja;
                        dm.IDRedarstva = idRedarstva;
                        dm.Ime = m.ImeVlasnika;
                        dm.Prezime = m.PrezimeVlasnika;
                        dm.Naziv = m.NazivVlasnika;
                        dm.OIB = m.OIBVlasnika;
                        dm.Drzava = m.Drzava;
                        dm.Opcina = m.Opcina;
                        dm.Mjesto = m.Mjesto;
                        dm.Adresa = m.UlicaBroj;
                        dm.Registracija = osobe.Vozilo.RegistarskaOznaka;
                        dm.Vrsta = osobe.Vozilo.VrstaVozila;
                        dm.Marka = osobe.Vozilo.MarkaVozila;
                        dm.Vlasnik = m.IndikatorVK.ToUpper() == "V";

                        db.MUPs.InsertOnSubmit(dm);
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI DOHVAT OD MUPA");
                return false;
            }
        }

        public static _OdgovorMUPVozilo MUPParkingVozilo(string oibustanove, string registracija, DateTime datumpreksaja, string adresaprekrsaja, int idAplikacije)
        {
            _OdgovorMUPVozilo ret = new _OdgovorMUPVozilo();

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (MUPParkingWebServiceInterfaceClient cc = new MUPParkingWebServiceInterfaceClient())
                {
                    cc.Open();

                    UpitIdentifikacija ui = new UpitIdentifikacija();
                    ui.DatumPrekrsaja = datumpreksaja;
                    ui.InstitucijaOIB = oibustanove;
                    ui.LokacijaPrekrsaja = adresaprekrsaja;
                    ui.SvrhaPretrage = 40;

                    PretragaParams pp = new PretragaParams();
                    pp.RegistarskaOznaka = registracija;

                    int status;
                    string message;

                    Odgovor odgovor = cc.Vozilo(ui, pp, out status, out message);

                    if (status != 0)
                    {
                        Sustav.SpremiGresku("", new Exception(message), idAplikacije, "POZIV MUPU: " + oibustanove + ", reg:" + registracija);
                        return ret;
                    }

                    _VoziloMUP vozilo = new _VoziloMUP(odgovor.OsnovnoVozilo.RegistarskaOznaka, odgovor.OsnovnoVozilo.MarkaVozila, odgovor.OsnovnoVozilo.VrstaVozila);

                    ret.Vozilo = vozilo;
                    ret.Vlasnik = new List<_VlasnikMUP>();

                    _VlasnikMUP vl = new _VlasnikMUP();

                    foreach (OdgovorVlasnik o in odgovor.VlasnikVozilo)
                    {
                        vl.IndikatorVK = o.IndikatorVK;
                        vl.OIBVlasnika = o.OIBVlasnika;
                        vl.ImeVlasnika = o.ImeVlasnika;
                        vl.PrezimeVlasnika = o.PrezimeVlasnika;
                        vl.NazivVlasnika = o.NazivVlasnika;
                        vl.Drzava = o.AdresaVlasnika.Drzava.ToUpper() == "HRVATSKA" ? "HR" : o.AdresaVlasnika.Drzava;
                        vl.Opcina = o.AdresaVlasnika.Opcina;
                        vl.Mjesto = o.AdresaVlasnika.Mjesto;
                        vl.UlicaBroj = o.AdresaVlasnika.UlicaBroj;
                        ret.Vlasnik.Add(vl);
                    }

                    cc.Close();

                    return ret;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "POZIV MUPU: " + oibustanove + ", reg:" + registracija);
                return null;
            }
        }
    }
}

