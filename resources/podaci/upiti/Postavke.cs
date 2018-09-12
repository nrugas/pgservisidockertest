using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Postavke
    {
        /* :: POSEBNA VOZILA ::*/

        #region POSEBNA VOZILA

        public static List<_Odobrenja> DohvatiOdobrenja(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var vozila = from p in db.ODOBRENJAs
                                 where p.IDRedarstva == idRedarstva
                                 select new _Odobrenja(p.IDOdobrenja, p.IDRedarstva, p.Naziv, p.Sjediste, p.Kontakt, p.Odobrenje, p.Registracija, p.Drzava, p.DatumOd, p.VrijemeOd, p.DatumDo, 
                                     p.VrijemeDo, p.Suspendirano, p.IDDjelatnika, p.IDSubjekta, p.Deaktiviran);

                    return vozila.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati Posebna Vozila");
                return new List<_Odobrenja>();
            }
        }

        public static bool ObrisiOdobrenje(string grad, int idOdobrenja, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.ODOBRENJAs.DeleteOnSubmit(db.ODOBRENJAs.First(i => i.IDOdobrenja == idOdobrenja));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Obrisi Posebno Vozilo");
                return false;
            }
        }

        public static int DodajOdobrenje(string grad, _Odobrenja odobrenje, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    ODOBRENJA p = new ODOBRENJA();

                    p.IDRedarstva = odobrenje.IDRedarstva;
                    p.Naziv = odobrenje.Naziv;
                    p.Sjediste = odobrenje.Sjediste;
                    p.Kontakt = odobrenje.Kontakt;
                    p.Odobrenje = odobrenje.Odobrenje;
                    p.Registracija = odobrenje.Registracija;
                    p.DatumOd = odobrenje.DatumOd;
                    p.VrijemeOd = odobrenje.VrijemeOd;
                    p.DatumDo = odobrenje.DatumDo;
                    p.VrijemeDo = odobrenje.VrijemeDo;
                    p.Suspendirano = odobrenje.Suspendirano;
                    p.IDDjelatnika = odobrenje.IDDjelatnika;
                    p.IDSubjekta = odobrenje.IDSubjekta;
                    p.Drzava = odobrenje.Drzava;
                    p.VrijemeUnosa = DateTime.Now;

                    db.ODOBRENJAs.InsertOnSubmit(p);
                    db.SubmitChanges();

                    return p.IDOdobrenja;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dodaj ODOBRENJE");
                return -1;
            }
        }

        public static bool IzmijeniOdobrenje(string grad, _Odobrenja odobrenje, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    ODOBRENJA p = db.ODOBRENJAs.First(i => i.IDOdobrenja == odobrenje.IDOdobrenja);

                    p.Naziv = odobrenje.Naziv;
                    p.Sjediste = odobrenje.Sjediste;
                    p.Kontakt = odobrenje.Kontakt;
                    p.Odobrenje = odobrenje.Odobrenje;
                    p.Registracija = odobrenje.Registracija;
                    p.DatumOd = odobrenje.DatumOd;
                    p.VrijemeOd = odobrenje.VrijemeOd;
                    p.DatumDo = odobrenje.DatumDo;
                    p.VrijemeDo = odobrenje.VrijemeDo;
                    p.Suspendirano = odobrenje.Suspendirano;
                    p.Drzava = odobrenje.Drzava;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ ODOBRENJE");
                return false;
            }
        }

        public static bool PromijeniStatusOdobrenja(string grad, int idOdobrenja, bool suspendirano, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    ODOBRENJA p = db.ODOBRENJAs.First(t => t.IDOdobrenja == idOdobrenja);
                    p.Suspendirano = suspendirano;

                    if (suspendirano)
                    {
                        p.Deaktiviran = DateTime.Now;
                    }
                    else
                    {
                        p.Deaktiviran = null;
                    }

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Promijeni Status ODOBRENJA");
                return false;
            }
        }

        public static _Odobrenja Odobrenje(string grad, string registracija, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {

                    var odobrenja = from p in db.ODOBRENJAs
                                    where p.Registracija.ToUpper() == registracija.Trim().ToUpper() &&
                                          p.IDRedarstva == idRedarstva
                                    select new _Odobrenja(p.IDOdobrenja, p.IDRedarstva, p.Naziv, p.Sjediste, p.Kontakt, p.Odobrenje, p.Registracija, p.Drzava, p.DatumOd, p.VrijemeOd, p.DatumDo,
                                        p.VrijemeDo, p.Suspendirano, p.IDDjelatnika, p.IDSubjekta, p.Deaktiviran);

                    if (odobrenja.Any())
                    {
                        return odobrenja.First();
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ODOBRENJE");
                return null;
            }
        }

        public static string PostojiOdobrenje(string grad, string registracija, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.ODOBRENJAs.Any(t => t.Registracija.ToUpper().Trim() == registracija.ToUpper().Trim() && !t.Suspendirano && t.IDRedarstva == idRedarstva))
                    {
                        ODOBRENJA odobrenje = db.ODOBRENJAs.First(t => t.Registracija.ToUpper().Trim() == registracija.ToUpper().Trim() && !t.Suspendirano && t.IDRedarstva == idRedarstva);

                        if (odobrenje != null)
                        {
                            string naslov = "";

                            //if (odobrenje.Registracija == "RI002ZL")//todo za sada nema naslova ako bude bilo potrebe definirati tu
                            //{
                            //    naslov = "Dozvola za parkiranje";
                            //}

                            string naziv = odobrenje.Naziv != "" ? odobrenje.Naziv + (odobrenje.Sjediste != "" ? " (" + odobrenje.Sjediste + ")" : "") : "";

                            if (!string.IsNullOrEmpty(naziv))
                            {
                                naziv += "\r\n";
                            }

                            string razlog = "";

                            if (!string.IsNullOrEmpty(odobrenje.Odobrenje))
                            {
                                razlog = "Razlog odobrenja:\r\n" + odobrenje.Odobrenje;
                            }

                            string datumOd = "", datumDo = "", vrijemeOd = "", vrijemeDo = "", datum = "", vrijeme = "";

                            #region DATUM

                            if (odobrenje.DatumOd != null)
                            {
                                datumOd = odobrenje.DatumOd.Value.ToString("dd.MM.yyyy");
                            }

                            if (odobrenje.DatumDo != null)
                            {
                                datumDo = odobrenje.DatumDo.Value.ToString("dd.MM.yyyy");
                            }

                            if (odobrenje.VrijemeOd != null)
                            {
                                vrijemeOd = odobrenje.VrijemeOd.Value.ToString("hh\\:mm");
                            }

                            if (odobrenje.VrijemeDo != null)
                            {
                                vrijemeDo = odobrenje.VrijemeDo.Value.ToString("hh\\:mm");
                            }

                            if (!string.IsNullOrEmpty(datumOd) && !string.IsNullOrEmpty(datumDo))
                            {
                                datum = datumOd + " - " + datumDo + "\r\n\r\n";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(datumOd))
                                {
                                    datum = datumOd;
                                }

                                if (!string.IsNullOrEmpty(datumDo))
                                {
                                    if (!string.IsNullOrEmpty(datum))
                                    {
                                        datum += " - " + datumDo;
                                    }
                                    else
                                    {
                                        datum = datumDo;
                                    }
                                }

                                if (string.IsNullOrEmpty(datum))
                                {
                                    datum += "Trajno\r\n";
                                }
                            }

                            #endregion

                            #region VRIJEME

                            if (!string.IsNullOrEmpty(vrijemeOd) && !string.IsNullOrEmpty(vrijemeDo))
                            {
                                vrijeme = vrijemeOd + " - " + vrijemeDo;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(vrijemeOd))
                                {
                                    vrijeme = vrijemeOd;
                                }

                                if (!string.IsNullOrEmpty(vrijemeDo))
                                {
                                    if (!string.IsNullOrEmpty(vrijeme))
                                    {
                                        vrijeme += " - " + vrijemeDo;
                                    }
                                    else
                                    {
                                        vrijeme = vrijemeDo;
                                    }
                                }
                            }

                            #endregion

                            string razdoblje = datum + vrijeme;

                            return naslov + "$" + naziv + "\r\n" + razlog + "\r\n" + razdoblje;
                        }
                    }

                    return "";
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POSEBNO VOZILO");
                return null;
            }
        }

        //obrisati kada uvedemo odobrenja
        public static bool? PosebnoVozilo(string grad, string registracija, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.PosebnaVozilas.Any(t => t.RegistracijskaOznaka == registracija))
                    {
                        PosebnaVozila p = db.PosebnaVozilas.Single(t => t.RegistracijskaOznaka == registracija);
                        p.BrojPrekrsaja += 1;
                        db.SubmitChanges();

                        return p.Sluzbeno;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "posebno vozilo?");
                return null;
            }
        }

        #endregion

        /*:: TERMINALI ::*/

        #region TERMINALI

        private static void AktivniNeaktivni(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    foreach (var t in db.Terminalis)
                    {
                        if (t.VrijemeZadnjegPristupa == null)
                        {
                            continue;
                        }

                        if (DateTime.Now.Subtract(t.VrijemeZadnjegPristupa.Value).Days > 30)
                        {
                            t.Aktivan = false;
                            db.SubmitChanges();
                        }
                        else
                        {
                            t.Aktivan = true;
                            db.SubmitChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POPIS TERMINALA AKTIVIRANJE DEAKTIVIRANJE");
            }
        }

        public static void IzmjeniTerminalS(string grad, int idTerminala, bool pauk, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Terminali t = db.Terminalis.First(p => p.IDTerminala == idTerminala);
                    t.Pauk = pauk;
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "TERMINAL JE PAUK");
            }
        }

        //stari
        public static _2DLista Terminal(string grad, string ib, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {

                    var ter = from t in db.Terminalis
                              where t.IdentifikacijskiBroj == ib
                              orderby t.NazivTerminala
                              select new _2DLista(t.IDTerminala, t.NazivTerminala);

                    return ter.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "TERMINAL STARI " + ib + " !");
                return new _2DLista(0, "...");
            }
        }

        public static List<_Terminal> PopisTerminalaS(string grad, bool neaktivni, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.Terminalis.Any(i => i.Aktivan && i.VrijemeZadnjegPristupa.Value > DateTime.Today.AddDays(-30)))
                    {
                        AktivniNeaktivni(grad, idAplikacije);
                    }

                    var ter = from t in db.Terminalis
                              where !neaktivni ? t.Aktivan : neaktivni
                              orderby t.NazivTerminala
                              select
                              new _Terminal(t.IDTerminala, null, "", t.IdentifikacijskiBroj, t.NazivTerminala, t.Verzija,
                                  t.Parametri, t.ResetRequest, t.RestartRequest, t.ProgramExit, t.TerminalSuspend, t.SelfDestruct,
                                  t.Aktivan, t.Pauk, t.VrijemeZadnjegPristupa);

                    return ter.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POPIS TERMINALA STARI");
                return new List<_Terminal>();
            }
        }

        public static List<_2DLista> DohvatiTerminale(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var ter = from t in db.Terminalis
                        where t.Aktivan
                        orderby t.NazivTerminala
                        select
                            new _2DLista(t.IDTerminala, t.NazivTerminala);

                    if (!ter.Any())
                    {
                        return new List<_2DLista>();
                    }

                    return ter.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POPIS TERMINALA");
                return new List<_2DLista>();
            }
        }

        public static bool IzmjeniTerminalS(string grad, _Terminal terminal, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Terminali t = db.Terminalis.Single(p => p.IDTerminala == terminal.IDTerminala);

                    //t.IDGrada = terminal.IDGrada;
                    t.NazivTerminala = terminal.Naziv;
                    t.Parametri = terminal.Parametri;
                    t.ResetRequest = terminal.ResetRequest;
                    t.RestartRequest = terminal.RestartRequest;
                    t.ProgramExit = terminal.ExitRequest;
                    t.TerminalSuspend = terminal.SuspendRequest;
                    t.Aktivan = terminal.Aktivan;
                    t.Pauk = (bool)terminal.Pauk;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Izmjeni Terminal");
                return false;
            }
        }

        public static int DodajTerminalS(string grad, _Terminal terminal, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.Terminalis.Any(i => i.IdentifikacijskiBroj == terminal.IdentifikacijskiBroj))
                    {
                        return
                            db.Terminalis.First(i => i.IdentifikacijskiBroj == terminal.IdentifikacijskiBroj)
                                .IDTerminala;
                    }

                    Terminali t = new Terminali();

                    //t.IDGrada = terminal.IDGrada;
                    t.NazivTerminala = terminal.Naziv;
                    t.IdentifikacijskiBroj = terminal.IdentifikacijskiBroj;
                    t.Parametri = terminal.Parametri;
                    t.ResetRequest = terminal.ResetRequest;
                    t.Pauk = (bool)terminal.Pauk;
                    t.VrijemeZadnjegPristupa = terminal.DatumSpajanja;
                    t.Verzija = terminal.Verzija;
                    t.RestartRequest = terminal.RestartRequest;
                    t.ProgramExit = terminal.ExitRequest;
                    t.TerminalSuspend = terminal.SuspendRequest;
                    t.Aktivan = true;

                    db.Terminalis.InsertOnSubmit(t);
                    db.SubmitChanges();

                    return t.IDTerminala;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Izmjeni Terminal");
                return -1;
            }
        }

        public static bool AkcijeNaTerminalima(string grad, _Terminal terminal, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Terminali t = db.Terminalis.Single(p => p.IDTerminala == terminal.IDTerminala);

                    t.ResetRequest = terminal.ResetRequest;
                    t.RestartRequest = terminal.RestartRequest;
                    t.ProgramExit = terminal.ExitRequest;
                    t.TerminalSuspend = terminal.SuspendRequest;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Akcije Na Terminalima");
                return false;
            }
        }

        public static _Terminal GetTerminala(string grad, string deviceId, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var ter = from t in db.Terminalis
                              where t.IdentifikacijskiBroj == deviceId
                              orderby t.NazivTerminala
                              select
                              new _Terminal(t.IDTerminala, null, "", t.IdentifikacijskiBroj, t.NazivTerminala, t.Verzija,
                                  t.Parametri, t.ResetRequest, t.RestartRequest, t.ProgramExit, t.TerminalSuspend, t.SelfDestruct,
                                  t.Aktivan, t.Pauk, t.VrijemeZadnjegPristupa);

                    if (!ter.Any())
                    {
                        return null;
                    }

                    return ter.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "TERMINAL");
                return null;
            }
        }

        //drasko
        public static bool ResetTerminals(string grad, string deviceId, int command, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (string.IsNullOrEmpty(deviceId))
                    {
                        foreach (Terminali r in db.Terminalis)
                        {
                            switch (command)
                            {
                                case 0:
                                    r.ResetRequest = true;
                                    break;
                                case 1:
                                    r.RestartRequest = true;
                                    break;
                                case 2:
                                    r.ProgramExit = true;
                                    break;
                                case 3:
                                    r.TerminalSuspend = true;
                                    break;
                            }

                            db.SubmitChanges();
                        }
                    }
                    else
                    {
                        //samo taj
                        Terminali r = db.Terminalis.First(i => i.IdentifikacijskiBroj == deviceId);
                        r.VrijemeZadnjegPristupa = DateTime.Now;

                        switch (command)
                        {
                            case 0:
                                r.ResetRequest = true;
                                break;
                            case 1:
                                r.RestartRequest = true;
                                break;
                            case 2:
                                r.ProgramExit = true;
                                break;
                            case 3:
                                r.TerminalSuspend = true;
                                break;
                        }

                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "AKCIJE TERMINALA");
                return false;
            }
        }

        public static string GetParametriTerminala(string grad, string deviceId, string naziv, bool startUp, int idAplikacije)
        {
            string par = "";
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Terminali r = db.Terminalis.First(i => i.IdentifikacijskiBroj == deviceId);

                    if (r == null)
                    {
                        _Terminal ter = new _Terminal(0, null, "", deviceId, naziv, "", null, false, false, false, false, false,
                            true, false, DateTime.Now);
                        DodajTerminalS(grad, ter, idAplikacije);
                    }
                    else
                    {
                        if (r.Parametri != null) par = r.Parametri.ToString();
                        if (startUp && r.ResetRequest)
                        {
                            r.ResetRequest = false;
                        }
                        r.RestartRequest = false;
                        r.ProgramExit = false;
                        r.TerminalSuspend = false;
                        r.VrijemeZadnjegPristupa = DateTime.Now;

                        db.SubmitChanges();
                    }

                    return par;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "TERMINAL");
                return "";
            }
        }

        public static int? SetTerminalAccessTime(string grad, string deviceId, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Terminali r = db.Terminalis.First(i => i.IdentifikacijskiBroj == deviceId);
                    r.VrijemeZadnjegPristupa = DateTime.Now;
                    db.SubmitChanges();

                    return r.IDTerminala;
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Set Terminal Access Time");
                return null;
            }
        }

        public static bool ClearTerminalStatus(string grad, string deviceId, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Terminali r = db.Terminalis.First(i => i.IdentifikacijskiBroj == deviceId);
                    r.TerminalSuspend = false;
                    r.ProgramExit = false;
                    r.ResetRequest = false;
                    r.RestartRequest = false;
                    r.SelfDestruct = false;
                    r.VrijemeZadnjegPristupa = DateTime.Now;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Set Terminal Access Time");
                return false;
            }
        }

        public static void UpdateVerzija(string grad, string progVer, string romVer, string deviceId, bool pauk, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Terminali r = db.Terminalis.First(i => i.IdentifikacijskiBroj == deviceId);
                    r.Verzija = string.Format("{0} ({1})", progVer, romVer);
                    r.VrijemeZadnjegPristupa = DateTime.Now;
                    //todo r.Pauk = pauk;

                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Update Verzije terminala, id:" + deviceId);
            }
        }

        //novi
        public static List<_Terminal> PopisTerminala(string grad, bool neaktivni, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var ter = from t in db.TERMINALIs
                              join g in db.GRADOVIs on t.IDGrada equals g.IDGrada into gra
                              from gg in gra.DefaultIfEmpty()
                              where !neaktivni ? t.Aktivan : neaktivni
                              orderby t.NazivTerminala
                              select
                              new _Terminal(t.IDTerminala, t.IDGrada, gg.NazivGrada ?? "SVI GRADOVI", t.IdentifikacijskiBroj,
                                  t.NazivTerminala, t.Verzija, t.Parametri, t.ResetRequest, t.RestartRequest, t.ProgramExit,
                                  t.TerminalSuspend, t.SelfDestruct,
                                  t.Aktivan, true, t.VrijemeZadnjegPristupa);

                    return ter.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POPIS TERMINALA");
                return new List<_Terminal>();
            }
        }

        public static bool IzmjeniTerminal(string grad, _Terminal terminal, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    TERMINALI t = db.TERMINALIs.Single(p => p.IDTerminala == terminal.IDTerminala);

                    //t.IDGrada = terminal.IDGrada;
                    t.NazivTerminala = terminal.Naziv;
                    t.Parametri = terminal.Parametri;
                    t.ResetRequest = terminal.ResetRequest;
                    t.RestartRequest = terminal.RestartRequest;
                    t.ProgramExit = terminal.ExitRequest;
                    t.TerminalSuspend = terminal.SuspendRequest;
                    t.Aktivan = terminal.Aktivan;
                    //t.Pauk = (bool)terminal.Pauk;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Izmjeni Terminal");
                return false;
            }
        }

        //status
        public static List<_StatusTerminala> StatusTerminala(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List <_StatusTerminala> s = new List<_StatusTerminala>();

                    foreach (var t in db.Terminalis.Where(i => i.Aktivan && i.IDTerminala > 0))
                    {
                        var ter = (from l in db.Lokacijes
                            where l.IDTerminala == t.IDTerminala &&
                                  l.DatumVrijeme.Date > DateTime.Today.AddDays(-1).Date &&
                                  l.RegistracijskaPlocica == ""
                            orderby l.DatumVrijeme descending
                            select
                                new _StatusTerminala(t.IDTerminala, t.NazivTerminala, t.Verzija, t.VrijemeZadnjegPristupa, l.GPSAcc, l.Battery, t.Pauk)).Take(1);

                        if (ter.Any())
                        {
                            s.Add(ter.First());
                        }
                        else
                        {
                            var ter1 = (from l in db.LokacijePaukas
                                where l.IDTerminala == t.IDTerminala &&
                                      l.DatumVrijemePauka.Date > DateTime.Today.AddDays(-1).Date 
                                        orderby l.DatumVrijemePauka descending
                                select
                                    new _StatusTerminala(t.IDTerminala, t.NazivTerminala, t.Verzija, t.VrijemeZadnjegPristupa, l.GPSAcc, l.Battery, t.Pauk)).Take(1);

                            if (ter1.Any())
                            {
                                s.Add(ter1.First());
                            }
                        }
                    }

                    return s;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POPIS TERMINALA STARI");
                return new List<_StatusTerminala>();
            }
        }

        #endregion

        /*:: PRINTERI ::*/

        #region PRINTERI

        public static List<_Printer> DohvatiPrintere(string grad, bool svi, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var predlozak = from p in db.PRINTERIs
                                    join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                                    where (!svi ? p.IDGrada == Sistem.IDGrada(grad) : svi) &&
                                          (idRedarstva != 0 ? p.IDRedarstva == idRedarstva : idRedarstva == 0)
                                    orderby p.NazivPrintera
                                    select
                                    new _Printer(p.IDPrintera, p.IDGrada, p.IDRedarstva, g.NazivGrada, p.PIN, p.Mac, p.NazivPrintera,
                                        p.InterniBroj, p.IDModela, p.SerijskiBroj, p.DatumDolaska, p.Jamstvo, p.JamstvoDo, p.Vlasnik,
                                        p.Ispravan);

                    return predlozak.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Lista Printera");
                return new List<_Printer>();
            }
        }

        public static bool ObrisiPrinter(string grad, int idPrintera, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    db.OPREMA_POVIJESTs.DeleteAllOnSubmit(db.OPREMA_POVIJESTs.Where(i => i.IDOpreme == idPrintera));
                    db.PRINTERIs.DeleteOnSubmit(db.PRINTERIs.First(i => i.IDPrintera == idPrintera));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Obrisi Printer");
                return false;
            }
        }

        public static int DodajPrinter(string grad, _Printer printer, string vlasnik, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (db.PRINTERIs.Any(i => i.Mac == printer.MAC))
                    {
                        return -2;
                    }

                    PRINTERI novi = new PRINTERI();

                    int id = 1;

                    if (db.PRINTERIs.Any())
                    {
                        id = db.PRINTERIs.Max(i => i.IDPrintera) + 1;
                    }

                    novi.IDPrintera = id;
                    novi.IDGrada = printer.IDGrada;
                    novi.IDRedarstva = printer.IDRedarstva;
                    novi.Mac = printer.MAC ?? "";
                    novi.PIN = printer.PIN;
                    novi.NazivPrintera = printer.Naziv;
                    novi.InterniBroj = printer.InterniBroj;
                    novi.IDModela = printer.IDModela;
                    novi.SerijskiBroj = printer.SerijskiBroj;
                    novi.DatumDolaska = printer.DatumUlaska;
                    novi.Jamstvo = printer.Jamstvo;
                    novi.JamstvoDo = printer.JamstvoDo;
                    novi.Vlasnik = printer.Vlasnik;
                    novi.Ispravan = printer.Ispravan;
                    novi.NazivVlasnika = vlasnik;

                    db.PRINTERIs.InsertOnSubmit(novi);
                    db.SubmitChanges();

                    Oprema.PovijestOpreme(printer.IDPrintera, 2, 2, "Dodao novi printer", DateTime.Now, idAplikacije);

                    return novi.IDPrintera;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dodaj Printer");
                return -1;
            }
        }

        public static bool IzmjeniPrinter(string grad, _Printer printer, string vlasnik, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    PRINTERI novi = db.PRINTERIs.Single(p => p.IDPrintera == printer.IDPrintera);

                    string napomena = "";
                    if (novi.IDGrada != printer.IDGrada)
                    {
                        napomena += string.Format("Izmjenio JLS printera: {0} => {1}", Grad(novi.IDGrada),
                            Grad(printer.IDGrada));
                    }

                    if (novi.IDRedarstva != printer.IDRedarstva)
                    {
                        string red = napomena != "" ? "\r\n" : "";
                        napomena += string.Format("{0}Izmjenio redarstvo printera: {1} => {2}", red,
                            Redarstvo(novi.IDRedarstva), Redarstvo(printer.IDRedarstva));
                    }

                    if (novi.NazivPrintera != printer.Naziv)
                    {
                        string red = napomena != "" ? "\r\n" : "";
                        napomena += string.Format("{0}Izmjenio naziv printera: {1} => {2}", red, novi.NazivPrintera,
                            printer.Naziv);
                    }

                    novi.IDGrada = printer.IDGrada;
                    novi.IDRedarstva = printer.IDRedarstva;
                    //novi.Mac = printer.MAC;
                    novi.PIN = printer.PIN;
                    novi.NazivPrintera = printer.Naziv;
                    novi.InterniBroj = printer.InterniBroj;

                    novi.IDModela = printer.IDModela;
                    novi.SerijskiBroj = printer.SerijskiBroj;
                    novi.DatumDolaska = printer.DatumUlaska;
                    novi.Jamstvo = printer.Jamstvo;
                    novi.JamstvoDo = printer.JamstvoDo;
                    novi.Vlasnik = printer.Vlasnik;
                    novi.Ispravan = printer.Ispravan;

                    if (vlasnik != null)
                    {
                        novi.NazivVlasnika = vlasnik;
                    }

                    db.SubmitChanges();

                    Oprema.PovijestOpreme(printer.IDPrintera, 2, 1, napomena, DateTime.Now, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Izmjeni Printer");
                return false;
            }
        }

        /*:: POVIJEST ::*/

        public static string Redarstvo(int? idRedarstva)
        {
            try
            {
                if (idRedarstva == null)
                {
                    return "";
                }

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.REDARSTVAs.First(i => i.IDRedarstva == idRedarstva).NazivRedarstva;
                }
            }
            catch (Exception)
            {

                return idRedarstva.ToString();
            }
        }

        public static string Grad(int? idGrada)
        {
            try
            {
                if (idGrada == null)
                {
                    return "";
                }

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.GRADOVIs.First(i => i.IDGrada == idGrada).NazivGrada;
                }
            }
            catch (Exception)
            {

                return idGrada.ToString();
            }
        }

        public static string Aplikacija(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.APLIKACIJEs.First(i => i.IDAplikacije == idAplikacije).NazivAplikacije;
                }
            }
            catch (Exception)
            {
                return idAplikacije.ToString();
            }
        }

        #endregion

        public static List<_2DLista> Redarstva(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var red = from r in db.REDARSTVAs
                              select new _2DLista(r.IDRedarstva, r.NazivRedarstva);

                    return red.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "LISTA REDARSTVA");
                return new List<_2DLista>();
            }
        }

        public static bool SpremiLog(string grad, DateTime vrijeme, string naziv, string uid, string service, string info, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {

                    Log l = new Log();

                    l.DatumVrijeme = vrijeme;
                    l.NazivTerminala = naziv;
                    l.UserID = uid;
                    l.Service = service;
                    l.Info = info;

                    db.Logs.InsertOnSubmit(l);
                    db.SubmitChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
