using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Osobe
    {
        public static bool SpremiOsobe(string grad, List<_Osoba> osobe, int idRacuna, bool mupIzvor, bool odgoda, int idAplikacije)
        {

            //todo da li je odgoda odmah dostaviti
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int x = 0;
                    foreach (var s in osobe)
                    {
                        if (s == null)
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(s.Ime) && string.IsNullOrEmpty(s.Prezime) && string.IsNullOrEmpty(s.Napomena) && string.IsNullOrEmpty(s.OIB) && string.IsNullOrEmpty(s.BrojDokumenta) && string.IsNullOrEmpty(s.Ulica))
                        {
                            continue;
                        }

                        RACUNI_OSOBE oso = new RACUNI_OSOBE();

                        var idosobe = 1;

                        if (db.RACUNI_OSOBEs.Any())
                        {
                            idosobe = db.RACUNI_OSOBEs.Max(i => i.IDOsobe) + 1;
                        }

                        oso.IDOsobe = idosobe;
                        oso.Ime = s.Ime ?? "";
                        oso.Prezime = s.Prezime ?? "";
                        oso.Ulica = s.Ulica ?? "";
                        oso.KucniBroj = s.KBr ?? "";
                        oso.Posta = s.Posta ?? "";
                        oso.Mjesto = s.Mjesto ?? "";
                        oso.Drzava = s.Drzava ?? "";
                        oso.OIB = s.OIB ?? "";
                        oso.Napomena = s.Napomena ?? "";
                        oso.BrojDokumenta = s.BrojDokumenta ?? "";
                        oso.Rodjen = s.Rodjen;
                        oso.MUP = mupIzvor;

                        db.RACUNI_OSOBEs.InsertOnSubmit(oso);
                        db.SubmitChanges();

                        RACUNI_OSOBE_RELACIJE rel = new RACUNI_OSOBE_RELACIJE();

                        rel.IDOsobe = idosobe;
                        rel.IDRacuna = idRacuna;
                        rel.Vlasnik = s.Vlasnik ?? false;

                        db.RACUNI_OSOBE_RELACIJEs.InsertOnSubmit(rel);
                        db.SubmitChanges();

                        x++;
                    }

                    //ima uključen dohvat od mupa i nije proslijeđeno ništa od draška i idvrste je odgoda
                    if (!mupIzvor && odgoda && !osobe.Any(i => i.Vlasnik == true))
                    {
                        try
                        {
                            using (PostavkeDataContext pdb = new PostavkeDataContext())
                            {
                                bool mup = pdb.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad)).DohvatVlasnikaMUP;

                                if (mup) // uključen dohvat od mupa i nije dodana niti jedna osoba
                                {
                                    _Racun rac = Naplata.DohvatiRacun(grad, idRacuna, true, idAplikacije);
                                    Naplata.DohvatMUPa(grad, rac, rac.IDDjelatnika, idAplikacije);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Sustav.SpremiGresku(grad, ex, idAplikacije, "OSOBE RAČUNA - DOHVAT MUP");
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OSOBE RAČUNA");
                return false;
            }
        }

        public static List<_Osoba> DohvatiOsobeRacuna(string grad, int idRacuna, bool mup, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var sta = from s in db.RACUNI_OSOBEs
                              join r in db.RACUNI_OSOBE_RELACIJEs on s.IDOsobe equals r.IDOsobe
                              where r.IDRacuna == idRacuna &&
                                    (!mup ? s.MUP == false : mup == true)
                              select new _Osoba(s.IDOsobe, s.Ime, s.Prezime, s.Ulica, s.KucniBroj, s.Posta, s.Mjesto, s.Drzava, s.OIB, s.Napomena, s.BrojDokumenta, s.Rodjen, r.Vlasnik, s.MUP);

                    return sta.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI OSOBE");
                return new List<_Osoba>();
            }
        }

        /*:: DOKUMENTI ::*/

        public static bool SpremiDokument(string grad, int idRacuna, byte[] dokument, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int idOsobe;

                    if (db.RACUNI_OSOBE_RELACIJEs.Any(i => i.IDRacuna == idRacuna))
                    {
                        idOsobe = db.RACUNI_OSOBE_RELACIJEs.First(i => i.IDRacuna == idRacuna).IDOsobe;
                    }
                    else
                    {
                        string putanja = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\" + grad + "_" + idRacuna + "_" + DateTime.Now.ToString("dd.MM.yy") + "_" + DateTime.Now.Ticks +".jpg";
                        Sustav.SpremiGresku(grad, new Exception(putanja), idAplikacije, "SPREMI DOKUMENT");

                        Slike.SpremiNaDisk(putanja, dokument);
                        return true;
                    }

                    RACUNI_OSOBE_DOKUMENTI rod = new RACUNI_OSOBE_DOKUMENTI();

                    int id = 1;

                    if (db.RACUNI_OSOBE_DOKUMENTIs.Any())
                    {
                        id = db.RACUNI_OSOBE_DOKUMENTIs.Max(i => i.IDDokumenta) + 1;
                    }

                    rod.IDDokumenta = id;
                    rod.IDOsobe = idOsobe;
                    rod.Dokument = dokument;
                    rod.Vrijeme = DateTime.Now;
                    rod.IDRacuna = idRacuna;

                    db.RACUNI_OSOBE_DOKUMENTIs.InsertOnSubmit(rod);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (SqlException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI DOKUMENT");
                return false;
            }
        }

        public static List<_Dokument> Dokumenti(string grad, int idOsobe, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var dok = from d in db.RACUNI_OSOBE_DOKUMENTIs
                              where d.IDOsobe == idOsobe
                              select new _Dokument(d.IDDokumenta, d.IDOsobe, d.Dokument.ToArray());

                    return dok.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI DOKUMENTE");
                return null;
            }
        }

        /*:: OSOBE ::*/

        public static List<_Osoba> DohvatiOsobe(string grad, bool oib, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var sta = from s in db.RACUNI_OSOBEs
                              where oib ? s.OIB.Trim() != "" : oib == false
                              select new _Osoba(s.IDOsobe, s.Ime, s.Prezime, s.Ulica, s.KucniBroj, s.Posta, s.Mjesto, s.Drzava, s.OIB, s.Napomena, s.BrojDokumenta, s.Rodjen, false, s.MUP);

                    return sta.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI STAVKE RACUNA");
                return new List<_Osoba>();
            }
        }

        public static _Osoba DohvatiOsobu(string grad, int idOsobe, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var sta = from s in db.RACUNI_OSOBEs
                              select new _Osoba(s.IDOsobe, s.Ime, s.Prezime, s.Ulica, s.KucniBroj, s.Posta, s.Mjesto, s.Drzava, s.OIB, s.Napomena, s.BrojDokumenta, s.Rodjen, false, s.MUP);

                    return sta.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI OSOBU");
                return null;
            }
        }

        public static bool IzmjeniOsobu(string grad, _Osoba osoba, int idRacuna, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RACUNI_OSOBE s;
                    int idosobe = 1;

                    if (osoba.IDOsobe == -1)
                    {
                        s = new RACUNI_OSOBE();

                        if (db.RACUNI_OSOBEs.Any())
                        {
                            idosobe = db.RACUNI_OSOBEs.Max(i => i.IDOsobe) + 1;
                        }

                        s.IDOsobe = idosobe;
                    }
                    else
                    {
                        s = db.RACUNI_OSOBEs.First(i => i.IDOsobe == osoba.IDOsobe);
                    }

                    s.Ime = osoba.Ime;
                    s.Prezime = osoba.Prezime;
                    s.Ulica = osoba.Ulica;
                    s.KucniBroj = osoba.KBr;
                    s.Posta = osoba.Posta;
                    s.Mjesto = osoba.Mjesto;
                    s.Drzava = osoba.Drzava ?? "";
                    s.OIB = osoba.OIB;
                    s.Napomena = osoba.Napomena ?? "";
                    s.BrojDokumenta = osoba.BrojDokumenta;
                    s.Rodjen = osoba.Rodjen;

                    if (osoba.IDOsobe == -1)
                    {
                        db.RACUNI_OSOBEs.InsertOnSubmit(s);
                    }
                    db.SubmitChanges();

                    if (osoba.IDOsobe == -1)
                    {
                        RACUNI_OSOBE_RELACIJE rel = new RACUNI_OSOBE_RELACIJE();

                        rel.IDOsobe = idosobe;
                        rel.IDRacuna = idRacuna;
                        rel.Vlasnik = (bool) osoba.Vlasnik;

                        db.RACUNI_OSOBE_RELACIJEs.InsertOnSubmit(rel);
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI OSOBU");
                return false;
            }
        }

        public static bool ObrisiOsobu(string grad, int idOsobe, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.RACUNI_OSOBEs.DeleteOnSubmit(db.RACUNI_OSOBEs.First(i => i.IDOsobe == idOsobe));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OBRISI OSOBU");
                return false;
            }
        }
    }
}