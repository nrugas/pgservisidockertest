using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class VanjskaPrijava
    {
        public static List<_Prijava> Prijave(string grad, DateTime datumOd, DateTime datumDo, bool nepregledane, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List<_2DLista> izvor = new PostavkeDataContext().IZVOR_VPs.Select(i => new _2DLista(i.IDIzvora, i.Naziv)).ToList();

                    var pri = from vp in db.VANJSKE_PRIJAVEs
                              join n in db.NaloziPaukus on vp.IDNaloga equals n.IDNaloga into nalog
                              from nn in nalog.DefaultIfEmpty()
                              join s in db.StatusPaukas on nn.IDStatusa equals s.IDStatusa into status
                              from ss in status.DefaultIfEmpty()
                              where vp.IDRedarstva == idRedarstva &&
                                  (nepregledane ? vp.Odbijen != true && (!vp.IDNaloga.HasValue || !vp.IDPrekrsaja.HasValue) : vp.Vrijeme.Date >= datumOd.Date && vp.Vrijeme.Date <= datumDo.Date)
                              orderby vp.Vrijeme descending
                              select new _Prijava(vp.IDVanjskePrijave, vp.IDIzvora, Izvor(izvor, vp.IDIzvora), vp.IDRedarstva, vp.IDLokacije, vp.IDNaloga, 
                                  vp.IDPrekrsaja, ss.NazivStatusa, vp.Lattitude, vp.Longitude, vp.Adresa, vp.Opis, vp.Registracija.ToUpper().Replace("-", "").Replace(" ", "").Trim(), 
                                  vp.Vrijeme, vp.Pregledano, vp.Odbijen, vp.Napomena);

                    if (!pri.Any())
                    {
                        return new List<_Prijava>();
                    }

                    return pri.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VANJSKE PRIJAVE");
                return new List<_Prijava>();
            }
        }

        private static string Izvor(List<_2DLista> izvor, int id)
        {
            return izvor.First(i => i.Value == id).Text;
        }

        public static void PregledaoPrijavu(string grad, int idPrijave, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VANJSKE_PRIJAVE vp = db.VANJSKE_PRIJAVEs.First(i => i.IDVanjskePrijave == idPrijave);

                    vp.Pregledano = true;

                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VANJSKE PRIJAVE");
            }
        }

        public static bool NalogPrijave(string grad, int idPrijave, int? idNaloga, int idPrekrsaja, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VANJSKE_PRIJAVE vp = db.VANJSKE_PRIJAVEs.First(i => i.IDVanjskePrijave == idPrijave);

                    vp.IDNaloga = idNaloga;
                    vp.IDPrekrsaja = idPrekrsaja;
                    vp.Odbijen = false;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VANJSKE PRIJAVE");
                return false;
            }
        }

        public static int NepregledanePrijave(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.VANJSKE_PRIJAVEs.Count(i => i.Odbijen != true && (!i.IDNaloga.HasValue || !i.IDPrekrsaja.HasValue));// && i.Vrijeme.Date == DateTime.Today);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VANJSKE PRIJAVE");
                return 0;
            }
        }

        public static void OdbijPrijavu(string grad, int idPrijave, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VANJSKE_PRIJAVE vp = db.VANJSKE_PRIJAVEs.First(i => i.IDVanjskePrijave == idPrijave);

                    vp.Pregledano = true;
                    vp.Odbijen = !vp.Odbijen;

                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VANJSKE PRIJAVE - ODBIJANJE");
            }
        }

        public static bool ObradiPrijavu(string grad, int idPrijave, string napomena, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VANJSKE_PRIJAVE vp = db.VANJSKE_PRIJAVEs.First(i => i.IDVanjskePrijave == idPrijave);

                    vp.Pregledano = true;
                    vp.Odbijen = null;
                    vp.Napomena = napomena;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VANJSKE PRIJAVE - ODBIJANJE");
                return false;
            }
        }

        public static List<_2DLista> StatusiVP(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.STATUS_VANJSKE_PRIJAVEs.Select(i => new _2DLista(i.IDStatusa, i.Napomena)).ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<_2DLista>();
            }
        }

        public static int NovaPrijava(string grad, _Prijava prijava, List<byte[]> slike, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    #region LOKACIJA

                    Lokacije lok = new Lokacije();

                    lok.Lat = prijava.Latitude;
                    lok.Long = prijava.Longitude;
                    lok.RegistracijskaPlocica = prijava.Registracija;
                    lok.DatumVrijeme = prijava.Vrijeme;
                    lok.IDDjelatnika = null;//prijava.IDDjelatnika;
                    lok.IDNacinaPozicioniranja = 3;
                    lok.IDTerminala = 0;
                    lok.CellTowerID = null;
                    lok.SignalStrength = null;
                    lok.HDOP = 0;
                    lok.Brzina = 0;

                    db.Lokacijes.InsertOnSubmit(lok);
                    db.SubmitChanges();

                    #endregion

                    #region SLIKE

                    Prekrsaj.DodajSliku(grad, lok.IDLokacije, slike, 1, idAplikacije);

                    #endregion

                    VANJSKE_PRIJAVE vp = new VANJSKE_PRIJAVE();

                    int id = 1;

                    if (db.VANJSKE_PRIJAVEs.Any())
                    {
                        id = db.VANJSKE_PRIJAVEs.Max(i => i.IDVanjskePrijave) + 1;
                    }

                    vp.IDVanjskePrijave = id;
                    vp.IDLokacije = lok.IDLokacije;
                    vp.IDRedarstva = prijava.IDRedarstva; // bilo fixirano na 4 pa sam promijenio i šaljem 2 u prijavi
                    vp.IDIzvora = 1;
                    vp.Lattitude = prijava.Latitude;
                    vp.Longitude = prijava.Longitude;
                    vp.Adresa = prijava.Adresa;
                    vp.Opis = prijava.Opis;
                    vp.Vrijeme = prijava.Vrijeme;
                    vp.Pregledano = false;
                    vp.Registracija = prijava.Registracija; // stavio jer je pucalo...

                    db.VANJSKE_PRIJAVEs.InsertOnSubmit(vp);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VANJSKA PRIJAVA");
                return -1;
            }
        }

    }
}