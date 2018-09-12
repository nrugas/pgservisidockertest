using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Spider
    {
        public static string NazivVozila(string grad, int idTerminala, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.VozilaPaukas.Any(i => i.IDTerminala == idTerminala))
                    {
                        return db.VozilaPaukas.First(i => i.IDTerminala == idTerminala).NazivVozila;
                    }

                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static _Vozilo Vozilo(string grad, int idTerminala, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var voz = from p in db.VozilaPaukas
                              join t in db.Terminalis on p.IDTerminala equals t.IDTerminala into term
                              from tt in term.DefaultIfEmpty()
                              where p.IDTerminala == idTerminala
                              select new _Vozilo(p.IDVozila, p.NazivVozila, idTerminala, tt.NazivTerminala, p.Registracija, p.Kontakt, p.Napomena, p.AP, p.Oznaka, p.OznakaPP, p.ObradjujeNalog, p.Lisice, p.Obrisan);

                    if (!voz.Any())
                    {
                        return null;
                    }

                    return voz.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VOZILO");
                return null;
            }
        }

        public static List<_2DLista> Razlozi(string grad, int idStatusa, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var raz = from p in db.RazloziNepodizanjaVozilas
                              where p.IDStausa == idStatusa
                              select new _2DLista(p.IDRazloga, p.NazivRazloga);

                    return raz.ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public static int SpremiLokacijuPauka(string grad, _PozicijaPauka pozicija, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (pozicija.IDVozila == null)
                    {
                        try
                        {
                            pozicija.IDVozila = db.VozilaPaukas.First(i => i.IDTerminala == pozicija.IDTerminala).IDVozila;
                        }
                        catch
                        {
                            pozicija.IDVozila = 0;
                        }
                    }

                    if (pozicija.IDVozila < 0 || pozicija.IDVozila == null)
                    {
                        Sustav.SpremiGresku(grad, new ApplicationException(pozicija.IDVozila + " " + pozicija.IDTerminala), idAplikacije, "SPREMI LOKACIJU PAUKA");
                        return 0;
                    }

                    LokacijePauka lp = new LokacijePauka();

                    lp.IDVozila = pozicija.IDVozila;
                    lp.IDNacinaPozicioniranja = pozicija.IDNacinaPozicioniranja;
                    lp.IDTerminala = pozicija.IDTerminala;
                    lp.LatPauka = pozicija.LatPauka;
                    lp.LongPauka = pozicija.LngPauka;
                    lp.DatumVrijemePauka = pozicija.DatumVrijeme;
                    lp.IDStatusaLokacija = pozicija.IDStatusaLokacije;
                    lp.IDCentralneLokacije = pozicija.IDCentralneLokacije;
                    lp.GPSAcc = pozicija.Preciznost;
                    lp.Brzina = pozicija.Brzina;
                    lp.Battery = pozicija.Baterija;

                    db.LokacijePaukas.InsertOnSubmit(lp);
                    db.SubmitChanges();

                    return Nalog.IDDodjeljenogNaloga(grad, pozicija, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI LOKACIJU PAUKA");
                return 0;
            }
        }

        public static List<_CentralnaLokacija> DohvatiCentralnuLokaciju(string grad, int idTerminala, int idAplikacije)
        {
            using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
            {
                try
                {
                    if (idTerminala == -1)
                    {
                        var poss = from v in db.VozilaPaukas
                                   join cv in db.VozilaCentralnelokacijes on v.IDVozila equals cv.IDVozila
                                   join c in db.CentralneLokacijes on cv.IDCentralneLokacije equals c.IDCentralneLokacije
                                   select new _CentralnaLokacija(c.IDCentralneLokacije, c.LatitudeCL, c.LongitudeCL);

                        return poss.ToList();
                    }

                    var pos = from v in db.VozilaPaukas
                              join cv in db.VozilaCentralnelokacijes on v.IDVozila equals cv.IDVozila
                              join c in db.CentralneLokacijes on cv.IDCentralneLokacije equals c.IDCentralneLokacije
                              where v.IDTerminala == idTerminala
                              select new _CentralnaLokacija(c.IDCentralneLokacije, c.LatitudeCL, c.LongitudeCL);

                    return pos.ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        public static List<_2DLista> Statusi(string grad, bool zatvara, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var raz = from p in db.StatusPaukas
                              where zatvara ? p.Zatvara : !zatvara
                              orderby p.NazivStatusa
                              select new _2DLista(p.IDStatusa, p.NazivStatusa);

                    return raz.ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        /*:: POSTAVKE JLS ::*/

        public static _CentralnaLokacija Deponij(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var dep = from c in db.CentralneLokacijes
                              where c.NazivCentralneLokacije == "Deponij"
                              select new _CentralnaLokacija(c.IDCentralneLokacije, c.LatitudeCL, c.LongitudeCL);

                    if (dep.Any())
                    {
                        return dep.First();
                    }

                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool LokacijaDeponija(string grad, _CentralnaLokacija deponij, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.CentralneLokacijes.Any(i => i.NazivCentralneLokacije == "Deponij"))
                    {
                        CentralneLokacije cl = db.CentralneLokacijes.First(i => i.NazivCentralneLokacije == "Deponij");

                        cl.LatitudeCL = deponij.Latitude;
                        cl.LongitudeCL = deponij.Longitude;

                        db.SubmitChanges();
                    }
                    else
                    {
                        CentralneLokacije cl = new CentralneLokacije();

                        cl.NazivCentralneLokacije = "Deponij";
                        cl.IDCentralneLokacije = 2;
                        cl.LatitudeCL = deponij.Latitude;
                        cl.LongitudeCL = deponij.Longitude;
                        cl.Ikonica = "";

                        db.CentralneLokacijes.InsertOnSubmit(cl);
                        db.SubmitChanges();

                        foreach (var vozilo in db.VozilaPaukas)
                        {
                            if (vozilo.IDVozila == 0)
                            {
                                continue;
                            }

                            VozilaCentralnelokacije vcl = new VozilaCentralnelokacije();

                            vcl.IDVozila = vozilo.IDVozila;
                            vcl.IDCentralneLokacije = 2;

                            db.VozilaCentralnelokacijes.InsertOnSubmit(vcl);
                            db.SubmitChanges();
                        }
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool StatusLokacije(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.StatusLokacijePaukas.Any();
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DodajStatusLokacije(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    StatusLokacijePauka slp = new StatusLokacijePauka();

                    slp.IDStatusaLokacija = 1;
                    slp.NazivStatusaLokacija = "Na centralnoj lokaciji";

                    db.StatusLokacijePaukas.InsertOnSubmit(slp);
                    db.SubmitChanges();

                    StatusLokacijePauka slp1 = new StatusLokacijePauka();

                    slp1.IDStatusaLokacija = 2;
                    slp1.NazivStatusaLokacija = "Na mjestu prekršaja";

                    db.StatusLokacijePaukas.InsertOnSubmit(slp1);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /*:: REDARSTVO PAUK ::*/

        public static List<Tuple<int, int, int>> RedarstvaPauka(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    IQueryable<Tuple<int, int, int>> red = from r in db.REDARSTVA_PAUKs
                                                           where r.IDGrada == Sistem.IDGrada(grad)
                                                           select new Tuple<int, int, int>(r.IDGrada, r.IDRedarstva, r.IDPredloska);

                    return red.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "REDARSTVA PAUKA");
                return null;
            }
        }

        public static List<Tuple<string, int, int>> DohvatiRedarstvaPauka(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var red = from rp in db.REDARSTVA_PAUKs
                        join r in db.REDARSTVAs on rp.IDRedarstva equals r.IDRedarstva 
                        where rp.IDGrada == Sistem.IDGrada(grad)
                        select new Tuple<string, int, int>(r.NazivRedarstva, rp.IDRedarstva, rp.IDPredloska);

                    return red.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "REDARSTVA PAUKA");
                return null;
            }
        }

        public static bool ObrisiRedarstvoPauk(string grad, Tuple<int, int, int> podaci, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    db.REDARSTVA_PAUKs.DeleteOnSubmit(db.REDARSTVA_PAUKs.First(i => i.IDGrada == Sistem.IDGrada(grad) && i.IDRedarstva == podaci.Item2 && i.IDPredloska == podaci.Item3));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OBRISI REDARSTVO PAUKA");
                return false;
            }
        }

        public static bool DodajRedarstvoPauk(string grad, Tuple<int, int, int> podaci, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    REDARSTVA_PAUK rp = new REDARSTVA_PAUK();

                    rp.IDGrada = Sistem.IDGrada(grad);
                    rp.IDRedarstva = podaci.Item2;
                    rp.IDPredloska = podaci.Item3;

                    db.REDARSTVA_PAUKs.InsertOnSubmit(rp);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ REDARSTVO PAUKA");
                return false;
            }
        }
    }
}