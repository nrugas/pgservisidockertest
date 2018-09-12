using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Nalog
    {
        /*:: AKCIJE ::*/

        #region AKCIJE

        public static bool Storniraj(string grad, int idNaloga, int? idRazloga, bool redar, string napomena, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int idStatusa = redar ? 11 : 10;

                    NaloziPauku np = db.NaloziPaukus.Single(i => i.IDNaloga == idNaloga);

                    if (np.IDStatusa != 0 && np.IDStatusa != 8 && np.IDStatusa != 1 && redar)
                    {
                        return false;
                    }

                    np.Redoslijed = 0;
                    np.StornoRedara = true;
                    np.NalogZatvoren = true;
                    np.IDRazloga = idRazloga;
                    np.IDStatusa = idStatusa;
                    np.IDVozila = null;
                    np.Napomena = napomena;

                    #region Spremanje izmjena u tablicu PAUK

                    Pauk pa = db.Pauks.Single(r => r.IDNaloga == idNaloga);
                    pa.Status = idStatusa;
                    pa.NalogZatvoren = true;
                    pa.StornoRedara = redar;

                    #endregion

                    db.SubmitChanges();

                    if (np.IDVozila != null)
                    {
                        PosloziRedosljed(grad, (int)np.IDVozila, idAplikacije);
                    }

                    //todo obrisi 
                    //Financije.StornirajNalog(grad, idNaloga, idRazloga, np.IDVozila ?? 0, idStatusa, idAplikacije);
                    //

                    SpremiPovijest(grad, idNaloga, null, idStatusa, null, null, "", null, null, null, idRazloga, true, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "STORNIRAJ NALOG");
                return false;
            }
        }

        public static bool IzmjeniVoziloNaloga(string grad, int idNaloga, int idVozila, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    NaloziPauku np = db.NaloziPaukus.First(i => i.IDNaloga == idNaloga);

                    int redoslijed = 1;

                    if (db.NaloziPaukus.Any(i => i.DatumNaloga.Date == DateTime.Today && i.IDVozila == idVozila && i.NalogZatvoren == false))
                    {
                        redoslijed = db.NaloziPaukus.Where(i => i.DatumNaloga.Date == DateTime.Today && i.IDVozila == idVozila && i.NalogZatvoren == false).Max(i => i.Redoslijed) + 1;
                    }

                    np.IDStatusa = 8;
                    np.IDVozila = idVozila;
                    np.Redoslijed = redoslijed;
                    np.StornoRedara = false;
                    np.NalogZatvoren = false;
                    np.IDDjelatnika = null;

                    Pauk pa = db.Pauks.First(r => r.IDNaloga == idNaloga);
                    pa.Status = 8;
                    pa.StornoRedara = false;

                    db.SubmitChanges();

                    SpremiPovijest(grad, idNaloga, idVozila, 8, true, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODJELI PAUKU");
                return false;
            }
        }

        //dodijeli
        public static bool? Dodjeli(string grad, int idNaloga, int idVozila, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (idNaloga == -1)
                    {
                        return false;
                    }

                    NaloziPauku np = db.NaloziPaukus.First(i => i.IDNaloga == idNaloga);

                    if (np.StornoRedara)
                    {
                        //nalog je storniran, ali ga je pokusao opet dodjeliti..;
                        return null;
                    }

                    int redoslijed = 1;

                    if (db.NaloziPaukus.Any(i => i.DatumNaloga.Date == DateTime.Today && i.IDVozila == idVozila && i.NalogZatvoren == false))
                    {
                        redoslijed = db.NaloziPaukus.Where(i => i.DatumNaloga.Date == DateTime.Today && i.IDVozila == idVozila && i.NalogZatvoren == false).Max(i => i.Redoslijed) + 1;
                    }

                    np.IDStatusa = 8;
                    np.IDVozila = idVozila;
                    np.Redoslijed = redoslijed;
                    np.StornoRedara = false;
                    np.NalogZatvoren = false;

                    Pauk pa = db.Pauks.First(r => r.IDNaloga == idNaloga);
                    pa.Status = 8;
                    pa.StornoRedara = false;

                    db.SubmitChanges();

                    SpremiPovijest(grad, idNaloga, idVozila, 8, true, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODJELI PAUKU");
                return false;
            }
        }

        public static List<_Nalog> DodijeljeniNalozi(string grad, int idVozila, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var raz = from n in db.NaloziPaukus
                              join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa
                              join v in db.VozilaPaukas on n.IDVozila equals v.IDVozila
                              join r in db.RazloziNepodizanjaVozilas on n.IDRazloga equals r.IDRazloga into razlog
                              from rr in razlog.DefaultIfEmpty()
                              join b in db.RACUNIs on n.IDRacuna equals b.IDRacuna into rac
                              from bb in rac.DefaultIfEmpty()

                              where n.NalogZatvoren == false &&
                                    n.DatumNaloga.Date == DateTime.Today &&
                                    n.IDVozila == idVozila
                              orderby n.Redoslijed
                              select
                                  new _Nalog(n.IDNaloga, n.IDVozila, v.IDTerminala, v.NazivVozila, n.IDStatusa, s.NazivStatusa, n.IDRazloga, rr.NazivRazloga, n.DatumNaloga, null, null, null, false, false, s.Boja, n.IDRacuna,
                                      bb.BrojRacuna, Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije), n.Lisice, n.Napomena);

                    return raz.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODIJELJENI NALOZI");
                return new List<_Nalog>();
            }
        }

        public static bool UpDown(string grad, int idNaloga, int idVozila, bool up, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var nalozi = db.NaloziPaukus.Where(i => i.IDVozila == idVozila && i.NalogZatvoren == false && i.DatumNaloga.Date == DateTime.Today);

                    if (nalozi.OrderBy(i => i.Redoslijed).First().IDNaloga == idNaloga && up)
                    {
                        return true;
                    }

                    if (nalozi.OrderByDescending(i => i.Redoslijed).First().IDNaloga == idNaloga && !up)
                    {
                        return true;
                    }

                    foreach (var r in nalozi)
                    {
                        if (r.IDNaloga == idNaloga)
                        {
                            if (up)
                            {
                                r.Redoslijed -= 1;
                                db.SubmitChanges();
                            }
                            else
                            {
                                r.Redoslijed += 1;
                                db.SubmitChanges();
                            }
                        }
                        else
                        {
                            if (up)
                            {
                                r.Redoslijed += 1;
                                db.SubmitChanges();
                            }
                            else
                            {
                                r.Redoslijed -= 1;
                                db.SubmitChanges();
                            }
                        }
                    }

                    int x = 1;
                    foreach (var nalog in db.NaloziPaukus.Where(i => i.IDVozila == idVozila && i.NalogZatvoren == false && i.DatumNaloga.Date == DateTime.Today).OrderBy(i => i.Redoslijed))
                    {
                        nalog.Redoslijed = x++;
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODIJELJENI NALOZI");
                return false;
            }
        }

        //otkazi
        public static bool Otkazi(string grad, int idNaloga, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    NaloziPauku np = db.NaloziPaukus.First(i => i.IDNaloga == idNaloga);

                    int idVozila = np.IDVozila.Value;

                    np.IDStatusa = 0;
                    np.IDVozila = null;
                    np.Redoslijed = 0;
                    np.StornoRedara = false;
                    np.NalogZatvoren = false;
                    np.IDDjelatnika = null;

                    Pauk pa = db.Pauks.First(r => r.IDNaloga == idNaloga);
                    pa.Status = 0;
                    pa.StornoRedara = false;

                    db.SubmitChanges();

                    SpremiPovijest(grad, idNaloga, idVozila, 9, true, idAplikacije);
                }

                return true;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OTKAŽI NALOG PAUKU");
                return false;
            }
        }

        //deponiraj
        public static bool Deponiraj(string grad, int idNaloga, int idTerminala, int idAplikacije)
        {
            return IzmjeniStatusNaloga(grad, idNaloga, idTerminala, 0, 4, 0, DateTime.Now, idAplikacije);
        }

        //pokusaj
        public static bool PokusajPodizanja(string grad, int idNaloga, int idTerminala, int idAplikacije)
        {
            return IzmjeniStatusNaloga(grad, idNaloga, idTerminala, 0, 3, 0, DateTime.Now, idAplikacije);
        }

        //blokiraj
        public static bool Blokiraj(string grad, int idNaloga, int idTerminala, int idAplikacije)
        {
            return IzmjeniStatusNaloga(grad, idNaloga, idTerminala, 0, 22, 0, DateTime.Now, idAplikacije);
        }

        //naplati
        public static int? Pokusaj(string grad, _Racun racun, int idTerminala, ref string brrac, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (brrac == null)
                    {
                        int idRacuna;
                        string poziv;
                        //ne izdajem racun - rijeka
                        string tekst = Naplata.NaplatiPauk(grad, racun, 3, out idRacuna, out brrac, out poziv, idAplikacije);

                        if (tekst == "")
                        {
                            return -1;
                        }
                    }

                    int idVozila = -1;

                    if (db.VozilaPaukas.Any(i => i.IDTerminala == idTerminala))
                    {
                        idVozila = db.VozilaPaukas.First(i => i.IDTerminala == idTerminala).IDVozila;
                    }

                    #region Spremanje izmjena u tablicu PAUK

                    Pauk pa = db.Pauks.First(r => r.IDNaloga == racun.IDReference);
                    pa.Status = 3;
                    pa.NalogZatvoren = true;

                    db.SubmitChanges();

                    #endregion

                    #region Spremanje izmjena u tablicu NALOZI PAUKU

                    NaloziPauku np = db.NaloziPaukus.First(r => r.IDNaloga == racun.IDReference);

                    np.IDStatusa = 3;
                    np.NalogZatvoren = true;
                    np.IDRazloga = 0;
                    np.Redoslijed = 0;

                    db.SubmitChanges();

                    #endregion

                    db.SubmitChanges();

                    PosloziRedosljed(grad, idVozila, idAplikacije);
                    SpremiPovijest(grad, racun.IDReference, idVozila, 3, true, idAplikacije);

                    //todo obrisi
                    //if (grad.ToUpper() == "PROMETNIK_RIJEKA")
                    //{
                    //    if (brrac != null)
                    //    {
                    //        Financije.Naplati(grad, racun.IDReference, idTerminala, Convert.ToInt32(brrac), idAplikacije);
                    //    }
                    //    else
                    //    {
                    //        Financije.IzmjeniStatus(grad, racun.IDReference, 3, DateTime.Now, 0, idVozila, "", idAplikacije);
                    //    }
                    //}
                    //

                    return np.IDRacuna;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATI NALOG");
                return -1;
            }
        }

        //todo obrisi
        //public static bool Naplati(string grad, int idNaloga, int idTerminala, int brojRacuna, DateTime datum, decimal latPauka, decimal lngPauka, string adresa,
        //    decimal brzina, int idStatusaLokacije, int idCentralneLokacije, int idAplikacije)
        //{
        //    try
        //    {
        //        if (Financije.Naplati(grad, idNaloga, idTerminala, brojRacuna, idAplikacije))
        //        {
        //            IzmjeniStatusNaloga(grad, idNaloga, idTerminala, null, 3, null, datum, latPauka, lngPauka, adresa, brzina, idStatusaLokacije, idCentralneLokacije, idAplikacije);
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATI NALOG");
        //        return false;
        //    }
        //}
        //

        public static bool IzmjeniStatusNaloga(string grad, int idNaloga, int idTerminala, int idDjelatnika, int idStatusa, int? idRazloga, DateTime vrijeme, int idAplikacije)
        {
            return IzmjeniStatusNaloga(grad, idNaloga, idTerminala, idDjelatnika, idStatusa, idRazloga, DateTime.Now, 0, 0, "", 0, 0, 0, idAplikacije);
        }

        public static bool IzmjeniStatusNaloga(string grad, int idNaloga, int idTerminala, int? idDjelatnika, int idStatusa, int? idRazloga, DateTime vrijeme, decimal lat,
            decimal lng, string adresa, decimal brzina, int idStatusaLokacije, int idCentralneLokacije, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int idVozila = IDVozila(grad, idTerminala, idAplikacije);
                    bool statusZatvara = Zatvara(grad, idStatusa, idAplikacije);

                    if (!db.Pauks.Any(i => i.IDNaloga == idNaloga))
                    {
                        return false;
                    }

                    if (db.NaloziPaukus.Any(r => r.IDNaloga == idNaloga && r.IDStatusa == 22 && r.Lisice))
                    {
                        return true;
                    }

                    #region Spremanje izmjena u tablicu PAUK

                    Pauk pa = db.Pauks.Single(r => r.IDNaloga == idNaloga);

                    pa.Status = idStatusa;

                    if (idStatusa == 1)
                    {
                        pa.DatumZaprimanja = vrijeme;
                    }
                    if (idStatusa == 2)
                    {
                        pa.DatumPodizanja = vrijeme;
                    }
                    if (idStatusa == 4 || idStatusa == 22)
                    {
                        pa.DatumDeponija = vrijeme;
                    }

                    pa.NalogZatvoren = statusZatvara;

                    #endregion

                    #region Spremanje izmjena u tablicu NALOZI PAUKU

                    NaloziPauku np = db.NaloziPaukus.First(r => r.IDNaloga == idNaloga);

                    //zbog greške preuzimanja naloga iako je readar stornirao
                    if (np.StornoRedara && idStatusa == 1)
                    {
                        return false;
                    }

                    np.IDStatusa = idStatusa;
                    np.NalogZatvoren = statusZatvara;
                    np.IDRazloga = idRazloga;
                    np.StornoRedara = false;

                    if (idDjelatnika != 0)
                    {
                        np.IDDjelatnika = idDjelatnika;
                    }

                    //IDStatusa == 1 || IDStatusa == 6 ||
                    if (idStatusa == 5 || idStatusa == 10 || idStatusa == 11 || idStatusa == 16)
                    {
                        np.IDVozila = null;
                        np.Redoslijed = 0;
                    }

                    if (statusZatvara)
                    {
                        np.Redoslijed = 0;
                    }

                    #endregion

                    db.SubmitChanges();

                    if (statusZatvara)
                    {
                        PosloziRedosljed(grad, idVozila, idAplikacije);
                    }

                    SpremiPovijest(grad, idNaloga, idVozila, idStatusa, lat, lng, adresa, brzina, idStatusaLokacije, idCentralneLokacije, idRazloga, true, idAplikacije);

                    //PAUK ODBIO NALOG
                    if (idStatusa == 14)
                    {
                        NaloziPauku npi = db.NaloziPaukus.Single(r => r.IDNaloga == idNaloga);

                        npi.IDStatusa = 0;
                        npi.NalogZatvoren = false;
                        npi.IDRazloga = null;
                        npi.Redoslijed = 0;
                        npi.IDVozila = null;

                        db.SubmitChanges();
                    }

                    //todo obrisi
                    //if (grad.ToUpper() == "PROMETNIK_RIJEKA")
                    //{
                    //    string poziv = db.Prekrsajis.First(i => i.IDNaloga == idNaloga).BrojUpozorenja;
                    //    Financije.IzmjeniStatus(grad, idNaloga, idStatusa, vrijeme, idRazloga, idVozila, poziv, idAplikacije);
                    //}
                    //

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI STATUS NALOGA");
                return false;
            }
        }

        private static bool Zatvara(string grad, int idStatusa, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.StatusPaukas.First(i => i.IDStatusa == idStatusa).Zatvara;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool VratiNaPocetak(string grad, int idNaloga, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    NaloziPauku np = db.NaloziPaukus.Single(i => i.IDNaloga == idNaloga);

                    np.IDStatusa = 0;
                    np.IDVozila = null;
                    np.NalogZatvoren = false;
                    np.StornoRedara = false;
                    np.Redoslijed = 0;
                    np.IDRazloga = 0;
                    np.IDDjelatnika = null;
                    np.Napomena = "";

                    #region Spremanje izmjena u tablicu PAUK

                    Pauk pa = db.Pauks.Single(r => r.IDNaloga == idNaloga);
                    pa.Status = 0;
                    pa.NalogZatvoren = false;
                    pa.DatumZaprimanja = null;
                    pa.DatumPodizanja = null;
                    pa.DatumDeponija = null;

                    #endregion

                    db.SubmitChanges();

                    SpremiPovijest(grad, idNaloga, null, 21, true, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "STORNIRAJ NALOG");
                return false;
            }
        }

        public static bool NaplatioIzvanSustava(string grad, int idNaloga, string napomena, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    NaloziPauku np = db.NaloziPaukus.Single(i => i.IDNaloga == idNaloga);
                    np.IDRacuna = Sistem.IDGrada(grad) == 49 ?  -1 : -2;
                    np.Napomena = napomena;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATIO IZVAN SUSTAVA");
                return false;
            }
        }

        //naplati
        public static bool Napomena(string grad, int idNaloga, string napomena, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    NaloziPauku np = db.NaloziPaukus.Single(i => i.IDNaloga == idNaloga);
                    np.Napomena = napomena;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPOMENA");
                return false;
            }
        }

        #endregion

        /*:: IZMJENE STATUSA ::*/

        #region DOGAĐAJI

        //spremi povijest
        public static void SpremiPovijest(string grad, int idNaloga, int? idVozila, int idStatusa, bool nalog, int idAplikacije)
        {
            SpremiPovijest(grad, idNaloga, idVozila, idStatusa, null, null, "", null, null, null, null, nalog, idAplikacije);
        }

        public static void SpremiPovijest(string grad, int IDNaloga, int? IDVozila, int IDStatusa, decimal? Lat, decimal? Lon,
            string Adresa, decimal? Brzina, int? IDstatusaLokacije, int? IDCentralneLokacije, int? IDRazloga, bool nalog, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PovijestNaloga np = new PovijestNaloga();

                    np.IDNaloga = IDNaloga;
                    np.DatumVrijemeDogadjaja = IDStatusa == 18 || IDStatusa == 19 ? DateTime.Now.AddSeconds(-3) : DateTime.Now;
                    np.IDVozila = IDVozila == -1 ? null : IDVozila;
                    np.IDStatusa = IDStatusa;
                    np.LatDogadjaja = Lat;
                    np.LongDogadjaja = Lon;
                    np.AdresaDogadjaj = Adresa;
                    np.Brzina = (double?)Brzina;
                    np.IDStatusaLokacije = IDstatusaLokacije;
                    np.IDCentralneLokacije = IDCentralneLokacije;
                    np.IDRazloga = IDRazloga;
                    np.Nalog = nalog;

                    db.PovijestNalogas.InsertOnSubmit(np);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI POVIJEST");
            }
        }

        public static List<_Event> IzmjeneStatusa(string grad, int idVozila, DateTime datum, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var por = from n in db.PovijestNalogas
                              join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa
                              join v in db.VozilaPaukas on n.IDVozila equals v.IDVozila into Ima
                              from vv in Ima.DefaultIfEmpty()
                              join cl in db.CentralneLokacijes on n.IDCentralneLokacije equals cl.IDCentralneLokacije into cenlok
                              from cll in cenlok.DefaultIfEmpty()
                              join r in db.RazloziNepodizanjaVozilas on n.IDRazloga equals r.IDRazloga into raz
                              from rr in raz.DefaultIfEmpty()
                              where n.DatumVrijemeDogadjaja.Date == datum.Date &&
                                    ((idVozila != 0 && n.IDVozila == idVozila) || (idVozila == 0))
                              orderby n.DatumVrijemeDogadjaja descending
                              select
                                  new _Event
                                      (
                                          n.IDPovijestiNaloga,
                                          n.IDNaloga,
                                          n.IDStatusa,
                                          n.DatumVrijemeDogadjaja,
                                          s.IDStatusa == 17 ? (vv.ObradjujeNalog ? vv.NazivVozila : "ME (" + db.Zahtjevis.Join(db.Djelatniks, z => z.IDPrijaviteljaDjelatnik, d => d.IDDjelatnika, (z, d) => new { z.IDPrijave, d.ImePrezime }).FirstOrDefault(i => i.IDPrijave == n.IDNaloga).ImePrezime + ")") : vv.NazivVozila,
                                          s.IDStatusa == 12 ? s.NazivStatusa + " - " + cll.NazivCentralneLokacije :
                                          (s.IDStatusa == 19 ? s.NazivStatusa + " - " + db.Zahtjevis.Join(db.Djelatniks, z => z.IDOdobravatelja, d => d.IDDjelatnika, (z, d) => new { z.IDPrijave, d.ImePrezime }).First(i => i.IDPrijave == n.IDNaloga).ImePrezime + "\r\n(" + db.Zahtjevis.First(i => i.IDPrijave == n.IDNaloga).Poruka + ")" :
                                          (s.IDStatusa == 18 ? s.NazivStatusa + " - " + db.Zahtjevis.Join(db.Djelatniks, z => z.IDOdobravatelja, d => d.IDDjelatnika, (z, d) => new { z.IDPrijave, d.ImePrezime }).First(i => i.IDPrijave == n.IDNaloga).ImePrezime : (s.IDStatusa != 7 ? s.NazivStatusa : rr.NazivRazloga ?? "Ostalo"))),
                                          s.Boja,
                                          n.Nalog
                                      );

                    if (!por.Any())
                    {
                        return new List<_Event>();
                    }

                    return por.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI IZMJENE STATUSA, IDvozila: " + idVozila + ", Datum: " + datum);
                return new List<_Event>();
            }
        }

        public static bool ObrisiIzmjenuStatusa(string grad, int id, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.PovijestNalogas.DeleteOnSubmit(db.PovijestNalogas.First(i => i.IDPovijestiNaloga == id));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OBRISI IZMJENU STATUSA");
                return false;
            }
        }

        #endregion

        /*:: ::*/

        public static List<_2DLista> Razlozi(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var raz = from r in db.RazloziNepodizanjaVozilas
                              where r.IDStausa == 10
                              orderby r.NazivRazloga
                              select new _2DLista(r.IDRazloga, r.NazivRazloga);

                    return raz.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "RAZLOZI");
                return new List<_2DLista>();
            }
        }

        /*:: NALOG ::*/

        public static bool NoviNalog(string grad, int idNaloga, DateTime vrijeme, int idLokacije, bool lisice, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    try
                    {
                        Pauk pauk = new Pauk();

                        pauk.IDNaloga = idNaloga;
                        pauk.Status = 0;
                        pauk.NalogZatvoren = false;
                        pauk.DatumNaloga = vrijeme;
                        pauk.StornoRedara = false;

                        db.Pauks.InsertOnSubmit(pauk);
                        db.SubmitChanges();

                        NaloziPauku np = new NaloziPauku();

                        np.IDNaloga = idNaloga;
                        np.IDStatusa = 0;
                        np.NalogZatvoren = false;
                        np.DatumNaloga = vrijeme;
                        np.StornoRedara = false;
                        np.Lisice = lisice;

                        db.NaloziPaukus.InsertOnSubmit(np);
                        db.SubmitChanges();
                    }
                    catch
                    {
                        //ako ne prođe nalog pauku ignorirati ce ga i ponašati se kao da nije izdan, rijetko kada se dupliraju id-jevi pa je ovo najdenostavniji način bez da zaseremo sve
                        Prekrsaji prek1 = db.Prekrsajis.First(i => i.IDLokacije == idLokacije);
                        prek1.IDNaloga = null;
                        prek1.NalogPauka = false;
                        db.SubmitChanges();

                        return true;
                    }

                    new Thread(() => MailLista.PosaljiNaredbu(grad, idNaloga, idAplikacije)).Start();

                    SpremiPovijest(grad, idNaloga, null, 0, null, null, "", null, null, null, null, true, idAplikacije);

                    Prekrsaji prek = db.Prekrsajis.First(i => i.IDLokacije == idLokacije);
                    prek.IDNaloga = idNaloga;
                    prek.NalogPauka = true;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ NOVI NALOG");
                return false;
            }
        }

        public static int NoviID(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.NaloziPaukus.Any())
                    {
                        return db.NaloziPaukus.Max(i => i.IDNaloga) + 1;
                    }

                    return 1;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NOVI ID NALOGA");
                return -1;
            }
        }

        public static int IDDodjeljenogNaloga(string grad, _PozicijaPauka pozicija, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var nalog = from p in db.NaloziPaukus
                                where p.DatumNaloga.Date == DateTime.Today.Date &&
                                      p.IDVozila == pozicija.IDVozila &&
                                      p.NalogZatvoren == false &&
                                      p.StornoRedara == false
                                orderby p.Redoslijed ascending
                                select p.IDNaloga;

                    int idsLokacije;

                    try
                    {
                        idsLokacije = (int)db.LokacijePaukas.Where(i => i.IDVozila == pozicija.IDVozila && i.DatumVrijemePauka.Date == DateTime.Today.Date).
                            OrderByDescending(i => i.IDLokacijePauka).First().IDStatusaLokacija;
                    }
                    catch
                    {
                        idsLokacije = -1;
                    }

                    if (pozicija.IDStatusaLokacije != 0)
                    {
                        if (idsLokacije != pozicija.IDStatusaLokacije && idsLokacije != -1)
                        {
                            if (pozicija.IDStatusaLokacije == 2 && !db.PovijestNalogas.Any(i => i.IDNaloga == nalog.First() && i.IDStatusa == 13))
                            {
                                SpremiPovijest(grad, nalog.First(), pozicija.IDVozila, 13, pozicija.LatPauka, pozicija.LngPauka, "", null,
                                               pozicija.IDStatusaLokacije, pozicija.IDCentralneLokacije, null, true, idAplikacije);
                            }

                            if (pozicija.IDStatusaLokacije == 1 && !db.PovijestNalogas.Any(i => i.IDNaloga == nalog.First() && i.IDStatusa == 13))
                            {
                                SpremiPovijest(grad, nalog.First(), pozicija.IDVozila, 12, pozicija.LatPauka, pozicija.LngPauka, "", null,
                                               pozicija.IDStatusaLokacije, pozicija.IDCentralneLokacije, null, true, idAplikacije);
                            }
                        }
                    }

                    return nalog.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ID DODJELJENOG");
                return 0;
            }
        }

        public static int DohvatiIDNaloga(string grad, int idTerminala, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var nalog = from n in db.NaloziPaukus
                                join v in db.VozilaPaukas on n.IDVozila equals v.IDVozila
                                where v.IDTerminala == idTerminala &&
                                      n.DatumNaloga.Date == DateTime.Today.Date &&
                                      n.NalogZatvoren == false &&
                                      n.StornoRedara == false
                                orderby n.Redoslijed ascending
                                select n.IDNaloga;

                    if (nalog.Any())
                    {
                        return nalog.First();
                    }

                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public static _Nalog DetaljiNaloga(string grad, int idNaloga, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var nal = from n in db.NaloziPaukus
                              join h in db.Pauks on n.IDNaloga equals h.IDNaloga
                              join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa
                              join v in db.VozilaPaukas on n.IDVozila equals v.IDVozila into vozila
                              from v in vozila.DefaultIfEmpty()
                              join r in db.RazloziNepodizanjaVozilas on n.IDRazloga equals r.IDRazloga into raz
                              from r in raz.DefaultIfEmpty()
                              join b in db.RACUNIs on n.IDRacuna equals b.IDRacuna into rac
                              from bb in rac.DefaultIfEmpty()
                              where n.IDNaloga == idNaloga
                              orderby n.DatumNaloga ascending
                              select new _Nalog(
                                  n.IDNaloga,
                                  v.IDVozila,
                                  v.IDTerminala,
                                  v.NazivVozila,
                                  n.IDStatusa,
                                  s.NazivStatusa,
                                  n.IDRazloga,
                                  r.NazivRazloga,
                                  n.DatumNaloga,
                                  h.DatumZaprimanja,
                                  h.DatumPodizanja,
                                  h.DatumDeponija,
                                  n.StornoRedara,
                                  n.NalogZatvoren,
                                  s.Boja,
                                  n.IDRacuna,
                                  bb.BrojRacuna, 
                                  Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije),
                                  n.Lisice, 
                                  n.Napomena);

                    return nal.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DETALJI NALOGA");
                return null;
            }
        }

        public static List<_Nalog> DohvatiNalogeDjeatnika(string grad, int idDjelatnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var nalozi = from n in db.NaloziPaukus
                                 join h in db.Pauks on n.IDNaloga equals h.IDNaloga
                                 join p in db.Prekrsajis on n.IDNaloga equals p.IDNaloga
                                 join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa
                                 join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                                 join z in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals z.IDOpisa
                                 join o in db.PopisPrekrsajas on z.IDPrekrsaja equals o.IDPrekrsaja
                                 join v in db.VozilaPaukas on n.IDVozila equals v.IDVozila into vozila
                                 from v in vozila.DefaultIfEmpty()
                                 join t in db.Terminalis on v.IDTerminala equals t.IDTerminala into terminal
                                 from t in terminal.DefaultIfEmpty()
                                 join r in db.RazloziNepodizanjaVozilas on n.IDRazloga equals r.IDRazloga into raz
                                 from r in raz.DefaultIfEmpty()
                                 join b in db.RACUNIs on n.IDRacuna equals b.IDRacuna into rac
                                 from bb in rac.DefaultIfEmpty()
                                 where p.IDDjelatnika == idDjelatnika &&
                                       p.Vrijeme.Value.Date == DateTime.Today.Date &&
                                       n.NalogZatvoren == false &&
                                       n.IDStatusa != 2
                                 orderby n.DatumNaloga ascending
                                 select new _Nalog(
                                     n.IDNaloga,
                                     v.IDVozila,
                                     v.IDTerminala,
                                     v.NazivVozila,
                                     n.IDStatusa,
                                     s.NazivStatusa,
                                     n.IDRazloga,
                                     r.NazivRazloga,
                                     n.DatumNaloga,
                                     h.DatumZaprimanja,
                                     h.DatumPodizanja,
                                     h.DatumDeponija,
                                     n.StornoRedara,
                                     n.NalogZatvoren,
                                     s.Boja,
                                     n.IDRacuna,
                                     bb.BrojRacuna, 
                                     Naplata.VrstaPlacanja(grad, bb.IDVrstePlacanja, idAplikacije),
                                     n.Lisice, 
                                     n.Napomena);

                    return nalozi.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI NALOGE DJELATNIKA");
                return null;
            }
        }

        public static int ProvijeriNaplativost(string grad, string brojObavijesti, out string poruka, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var prek = from p in db.Prekrsajis
                               where p.BrojUpozorenja == brojObavijesti &&
                                     p.Test == false &&
                                     p.Status == false &&
                                     p.IDRacuna == null &&
                                     (p.BrojUpozorenja.StartsWith("03") || p.BrojUpozorenja.StartsWith("05") || p.BrojUpozorenja.StartsWith("07"))
                               select p.IDPrekrsaja;

                    try
                    {
                        poruka = "";
                        return prek.First();
                    }
                    catch
                    {
                        string razlozi = "";

                        var pre = from p in db.Prekrsajis
                                  where p.BrojUpozorenja == brojObavijesti
                                  select p;

                        try
                        {
                            if ((bool)pre.First().Test)
                            {
                                razlozi = " Prekršaj je postavljen kao test!";
                            }

                            if (pre.First().Status)
                            {
                                razlozi = " Prekršaj je storniran!";
                            }

                            if (pre.First().IDRacuna.HasValue)
                            {
                                razlozi = " Prekršaj je plaćen!";
                            }

                            if ((bool)pre.First().Poslano)
                            {
                                razlozi = " Prekršaj je proslijeđen u računovodstvo!";
                            }
                        }
                        catch (Exception)
                        {
                            razlozi = " Prekršaj ne postoji!";
                        }

                        poruka = "Prekršaj nije naplativ! - " + razlozi;
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                poruka = ex.Message;
                return -1;
            }
        }

        public static void PosloziRedosljed(string grad, int idVozila, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var dod = from n in db.NaloziPaukus
                              where n.IDVozila == idVozila &&
                                    n.IDStatusa == 8
                              orderby n.Redoslijed ascending
                              select n;

                    int x = 1;
                    foreach (var nalog in dod)
                    {
                        NaloziPauku np = nalog;// db.NaloziPaukus.First(i => i.IDNaloga == q.IDNaloga);
                        np.Redoslijed = x;
                        db.SubmitChanges();
                        x++;
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public static string NazivRazloga(string grad, int? IDStatusaRazloga, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return IDStatusaRazloga == null ? "" : db.RazloziNepodizanjaVozilas.First(i => i.IDRazloga == IDStatusaRazloga).NazivRazloga;
                }
            }
            catch
            {
                return "";
            }
        }

        public static string NazivStatusaNaloga(string grad, int IDStatusa, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.StatusPaukas.First(i => i.IDStatusa == IDStatusa).NazivStatusa;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static int IDVozila(string grad, int idTerminala, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.VozilaPaukas.First(i => i.IDTerminala == idTerminala).IDVozila;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int? IDTerminala(string grad, int idVozila, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.VozilaPaukas.First(i => i.IDVozila == idVozila).IDTerminala;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /*:: MOBILE ::*/

        public static List<_NalogMobile> DohvatiNalogeDjeatnikaMobile(string grad, int idDjelatnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var nalozi = from n in db.NaloziPaukus
                        join p in db.Prekrsajis on n.IDNaloga equals p.IDNaloga
                        join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa
                        join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                        join z in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals z.IDOpisa
                        join o in db.PopisPrekrsajas on z.IDPrekrsaja equals o.IDPrekrsaja
                        join v in db.VozilaPaukas on n.IDVozila equals v.IDVozila into vozila
                        from v in vozila.DefaultIfEmpty()
                        join t in db.Terminalis on v.IDTerminala equals t.IDTerminala into terminal
                        from t in terminal.DefaultIfEmpty()
                        join r in db.RazloziNepodizanjaVozilas on n.IDRazloga equals r.IDRazloga into raz
                        from r in raz.DefaultIfEmpty()
                        where p.IDDjelatnika == idDjelatnika &&
                              p.Vrijeme.Value.Date == DateTime.Today.Date &&
                              n.NalogZatvoren == false &&
                              (n.IDStatusa == 0 || n.IDStatusa == 1 || n.IDStatusa == 8)
                        //n.IDStatusa != 2 && n.IDStatusa != 1
                        orderby n.DatumNaloga ascending
                        select new _NalogMobile
                        (
                            p.IDPrekrsaja,
                            n.IDNaloga,
                            v.IDVozila,
                            t.IDTerminala,
                            n.IDStatusa,
                            t.NazivTerminala,
                            v.NazivVozila,
                            v.Registracija,
                            p.Vrijeme.Value,
                            p.KraticaDrzave != "??"
                                ? p.RegistracijskaPlocica + " (" + p.KraticaDrzave + ")"
                                : p.RegistracijskaPlocica,
                            d.ImePrezime,
                            d.UID,
                            d.BrojSI,
                            p.Adresa,
                            p.BrojUpozorenja,
                            z.OpisPrekrsaja,
                            z.ClanakPauka,
                            o.MaterijalnaKaznjivaNorma,
                            s.IDStatusa != 7 ? s.NazivStatusa : r.NazivRazloga,
                            s.NazivStatusa,
                            new _Koordinate(p.IDLokacije, d.IDDjelatnika, p.Lat, p.Long, p.Vrijeme.Value),
                            s.Boja,
                            n.NalogZatvoren,
                            n.StornoRedara,
                            s.IDStatusa == 8,
                            n.Lisice,
                            n.Redoslijed
                        );

                    return nalozi.ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public static _NalogMobile DetaljiNalogaMobile(string grad, int idNaloga, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var nal = from n in db.NaloziPaukus
                        join p in db.Prekrsajis on n.IDNaloga equals p.IDNaloga
                        join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa
                        join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                        join z in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals z.IDOpisa
                        join o in db.PopisPrekrsajas on z.IDPrekrsaja equals o.IDPrekrsaja
                        join v in db.VozilaPaukas on n.IDVozila equals v.IDVozila into vozila
                        from v in vozila.DefaultIfEmpty()
                        join t in db.Terminalis on v.IDTerminala equals t.IDTerminala into terminal
                        from t in terminal.DefaultIfEmpty()
                        join r in db.RazloziNepodizanjaVozilas on n.IDRazloga equals r.IDRazloga into raz
                        from r in raz.DefaultIfEmpty()
                        where n.IDNaloga == idNaloga
                        orderby n.DatumNaloga ascending
                        select new _NalogMobile
                        (
                            p.IDPrekrsaja,
                            n.IDNaloga,
                            v.IDVozila,
                            t.IDTerminala,
                            n.IDStatusa,
                            t.NazivTerminala,
                            v.NazivVozila,
                            v.Registracija,
                            p.Vrijeme.Value,
                            p.KraticaDrzave != "??"
                                ? p.RegistracijskaPlocica + " (" + p.KraticaDrzave + ")"
                                : p.RegistracijskaPlocica,
                            d.ImePrezime,
                            d.UID,
                            d.BrojSI,
                            p.Adresa,
                            p.BrojUpozorenja,
                            z.OpisPrekrsaja,
                            z.ClanakPauka,
                            o.MaterijalnaKaznjivaNorma,
                            s.IDStatusa != 7 ? s.NazivStatusa : r.NazivRazloga,
                            s.NazivStatusa,
                            new _Koordinate(p.IDLokacije, d.IDDjelatnika, p.Lat, p.Long, p.Vrijeme.Value),
                            s.Boja,
                            n.NalogZatvoren,
                            n.StornoRedara,
                            s.IDStatusa == 8,
                            n.Lisice,
                            n.Redoslijed
                        );

                    if (!nal.Any())
                    {
                        return null;
                    }

                    return nal.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DETALJI NALOGA MOBILE");
                return null;
            }
        }
    }
}