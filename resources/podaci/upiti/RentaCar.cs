using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using PG.Servisi.GOService;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class RentaCar
    {
        /*:: RENTACAR ::*/

        public static List<_RentaCar> DohvatiRentaCar(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rc = from r in db.RENTACARs
                             orderby r.Naziv
                             select
                             new _RentaCar(r.IDRentaCar, r.IDKlasifikacije, r.IDKorisnikaGO, r.Naziv, r.Email, r.Mobitel,
                                 r.Osoba, r.Telefon, r.Obavijesti, r.HUB, db.RENTACAR_VOZILAs.Where(i => i.IDRentaCar == r.IDRentaCar).Select(i => new _2DLista(i.IDVozila, i.Registracija)).ToList());

                    return rc.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "RENT A CAR");
                return new List<_RentaCar>();
            }
        }

        public static _RentaCar DodajRentaCar(string grad, _RentaCar renta, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    GRADOVI g;

                    using (PostavkeDataContext pdb = new PostavkeDataContext())
                    {
                        g = pdb.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad));
                    }

                    int idKorisnikaGO;
                    int idKlasifikacije;
                    using (GOPazigradClient sc = new GOPazigradClient())
                    {
                        idKlasifikacije = sc.DodajKlasifikaciju(g.GO, new _Klasifikacija() { IDGrupe = g.IDGrupePromet.Value, Naziv = renta.Naziv + " (Prometno redarstvo - postupanja)", Opis = "", Automatski = true, Vozilo = true, IDPredloska = null, NijeJavno = true, AutomatskiRijeseno = true, IDTipa = 1 });
                        idKorisnikaGO = sc.DodajKorisnika(g.GO, new _Korisnik() { Ime = renta.Naziv, Prezime = "", DatumRodenja = null, Email = renta.Email, Mobitel = renta.Mobitel, Lozinka = DateTime.Now.ToString("hhmmss"), IDPrivilegije = 4, SMS = true },
                            new ObservableCollection<int>() { idKlasifikacije });
                    }

                    RENTACAR rc = new RENTACAR();

                    int id = 1;

                    if (db.RENTACARs.Any())
                    {
                        id = db.RENTACARs.Max(i => i.IDRentaCar) + 1;
                    }

                    rc.IDRentaCar = id;
                    rc.IDKorisnikaGO = idKorisnikaGO;
                    rc.IDKlasifikacije = idKlasifikacije;
                    rc.Mobitel = renta.Mobitel;
                    rc.Naziv = renta.Naziv;
                    rc.Email = renta.Email;
                    rc.Osoba = renta.Osoba;
                    rc.Telefon = renta.Telefon;
                    rc.Obavijesti = renta.Aktivan;
                    rc.HUB = renta.HUB;

                    db.RENTACARs.InsertOnSubmit(rc);
                    db.SubmitChanges();

                    renta.IDKorisnikaGO = idKorisnikaGO;
                    renta.IDKlasifikacije = idKlasifikacije;
                    renta.IDRentaCar = id;
                    renta.Vozila = new List<_2DLista>();

                    return renta;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ RENT A CAR");
                return null;
            }
        }

        public static bool IzmjeniRentaCar(string grad, _RentaCar renta, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RENTACAR rc = db.RENTACARs.First(i => i.IDRentaCar == renta.IDRentaCar);

                    rc.Mobitel = renta.Mobitel;
                    rc.Naziv = renta.Naziv;
                    rc.Email = renta.Email;
                    rc.Osoba = renta.Osoba;
                    rc.Telefon = renta.Telefon;
                    rc.Obavijesti = renta.Aktivan;
                    rc.HUB = renta.HUB;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Izmjeni RENT A CAR");
                return false;
            }
        }

        public static bool ObrisiRentaCar(string grad, int idRente, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.RENTACAR_VOZILAs.Any(i => i.IDRentaCar == idRente))
                    {
                        return false;
                    }

                    db.RENTACARs.DeleteOnSubmit(db.RENTACARs.First(i => i.IDRentaCar == idRente));
                    db.SubmitChanges();

                    //todo blokiraj korisnika u GO??

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OBRISI RENT A CAR");
                return false;
            }
        }

        public static bool PoveziRentaCar(string grad, int idRente, string naziv, string email, string mobitel, out int idKorisnikaGO, out int idKlasifikacije, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    GRADOVI g;

                    using (PostavkeDataContext pdb = new PostavkeDataContext())
                    {
                        g = pdb.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad));
                    }

                    using (GOPazigradClient sc = new GOPazigradClient())
                    {
                        idKlasifikacije = sc.DodajKlasifikaciju(g.GO, new _Klasifikacija() { IDGrupe = g.IDGrupePromet.Value, Naziv = naziv + " (Prometno redarstvo - postupanja)", Opis = "", Automatski = true, Vozilo = true, IDPredloska = null, NijeJavno = true, AutomatskiRijeseno = true, IDTipa = 1 });
                        idKorisnikaGO = sc.DodajKorisnika(g.GO, new _Korisnik() { Ime = naziv, Prezime = "", DatumRodenja = null, Email = email, Mobitel = mobitel, Lozinka = DateTime.Now.ToString("hhmmss"), IDPrivilegije = 4, SMS = true },
                            new ObservableCollection<int>() { idKlasifikacije });
                    }

                    RENTACAR rc = db.RENTACARs.First(i => i.IDRentaCar == idRente);

                    rc.IDKorisnikaGO = idKorisnikaGO;
                    rc.IDKlasifikacije = idKlasifikacije;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POVEZI RENT A CAR");
                idKlasifikacije = 0;
                idKorisnikaGO = 0;
                return false;
            }
        }

        /*:: VOZILA ::*/

        public static int DodajRCVozilo(string grad, int idRentaCar, string registracija, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RENTACAR_VOZILA rc = new RENTACAR_VOZILA();

                    int id = 1;

                    if (db.RENTACAR_VOZILAs.Any())
                    {
                        id = db.RENTACAR_VOZILAs.Max(i => i.IDVozila) + 1;
                    }

                    rc.IDVozila = id;
                    rc.IDRentaCar = idRentaCar;
                    rc.Registracija = registracija;

                    db.RENTACAR_VOZILAs.InsertOnSubmit(rc);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ RENT A CAR VOZILO");
                return -1;
            }
        }

        public static bool DodajRCVozila(string grad, int idRentaCar, string[] registracije, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    foreach (var registracija in registracije)
                    {
                        if (db.RENTACAR_VOZILAs.Any(i => i.IDRentaCar == idRentaCar && i.Registracija == registracija))
                        {
                            continue;
                        }

                        RENTACAR_VOZILA rc = new RENTACAR_VOZILA();

                        int id = 1;

                        if (db.RENTACAR_VOZILAs.Any())
                        {
                            id = db.RENTACAR_VOZILAs.Max(i => i.IDVozila) + 1;
                        }

                        rc.IDVozila = id;
                        rc.IDRentaCar = idRentaCar;
                        rc.Registracija = registracija;

                        db.RENTACAR_VOZILAs.InsertOnSubmit(rc);
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ RENT A CAR VOZILO");
                return false;
            }
        }

        public static bool IzmjeniRCVozilo(string grad, int idVozila, string registracija, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RENTACAR_VOZILA rc = db.RENTACAR_VOZILAs.First(i => i.IDVozila == idVozila);

                    rc.Registracija = registracija;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI RENT A CAR VOZILO");
                return false;
            }
        }

        public static bool ObrisiRCVozilo(string grad, int idVozila, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.RENTACAR_VOZILAs.DeleteOnSubmit(db.RENTACAR_VOZILAs.First(i => i.IDVozila == idVozila));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OBRISI RENT A CAR VOZILO");
                return false;
            }
        }

        /*:: PROVJERA ::*/

        public static void PostojiRCVozilo(string grad, string registracija, int IDLokacije, int wait, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (!db.RENTACAR_VOZILAs.Any(i => i.Registracija == registracija))
                    {
                        return;
                    }

                    var rc = from v in db.RENTACAR_VOZILAs
                             join r in db.RENTACARs on v.IDRentaCar equals r.IDRentaCar
                             where r.Obavijesti && v.Registracija == registracija
                             select r;

                    if (!rc.Any())
                    {
                        return;
                    }

                    Thread.Sleep(wait);

                    GRADOVI g;
                    using (PostavkeDataContext p = new PostavkeDataContext())
                    {
                        g = p.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad));
                    }

                    _Prekrsaj prekrsaj = Prekrsaj.DetaljiPrekrsaja(grad, IDLokacije, idAplikacije);

                    _Predmet novi = new _Predmet();

                    novi.IDPrijavitelja = -1;
                    novi.IDIzvora = 5;
                    novi.IDGrupe = g.IDGrupePromet.Value;
                    novi.IDKlasifikacije = rc.First().IDKlasifikacije;
                    novi.IDTipa = 1; //informacija
                    novi.Grupa = "";
                    novi.Marker = "";
                    novi.Klasifikacija = "";
                    novi.Status = "";
                    novi.IDStatusa = 6;
                    novi.Ikona = "";
                    novi.TekstPrijave = prekrsaj.Registracija + "\r\n" + prekrsaj.OpisPrekrsaja + "\r\n" + prekrsaj.Kazna.Replace(".00", "").Replace(",00", "") + ",00 kn" + "\r\n" + prekrsaj.BrojDokumenta + "\r\n" + prekrsaj.ClanakPrekrsaja;
                    novi.DatumVrijeme = prekrsaj.DatumVrijeme;
                    novi.DatumPredmeta = novi.DatumVrijeme.ToString();
                    novi.NaslovPredmeta = prekrsaj.Registracija;
                    novi.Latitude = prekrsaj.Latitude;
                    novi.Longitude = prekrsaj.Longitude;
                    novi.Ulica = prekrsaj.Adresa.Replace("na ulici", "").Replace("u blizini kućnog broja", "").Replace("kod kućnog broja", "").Trim();
                    novi.Kbr = "";//todo?
                    novi.Posta = ""; //todo?
                    novi.Mjesto = "";//todo?
                    novi.Javno = false;
                    novi.Anonimno = false;
                    novi.DatumZatvaranja = null;
                    novi.Komentiranje = false;
                    novi.Tag = db.Djelatniks.First(i => i.IDDjelatnika == prekrsaj.IDDjelatnika).ImePrezime;
                    novi.Novi = false;
                    novi.Dokumenti = null;
                    novi.KomunalniObjekt = null;
                    novi.Registracija = prekrsaj.Registracija;
                    novi.IDLokacije = IDLokacije;
                    //kreiraj prijavu u GO te ju proslijedi korisniku

                    using (GOPazigradClient sc = new GOPazigradClient())
                    {
                        List<byte[]> slike = Prekrsaj.Slike(grad, IDLokacije, idAplikacije);
                        sc.NoviPredmet(g.GO, novi, new ObservableCollection<byte[]>(slike), true, rc.First().HUB);
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PROVIJERI RENT A CAR VOZILO");
            }
        }
    }
}