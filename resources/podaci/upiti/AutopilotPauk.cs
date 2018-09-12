using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class AutopilotPauk
    {
        public static bool PromijeniStatusVozila(string grad, int idTerminala, bool aktivan, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VozilaPauka vp = db.VozilaPaukas.First(i => i.IDTerminala == idTerminala);
                    vp.AP = aktivan;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PROMIJENI STATUS VOZILA");
                return false;
            }
        }

        public static List<_NaloziPauku> IzdaniNalozi(string grad, DateTime datum, bool novi, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var nal = from n in db.NaloziPaukus
                              join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa
                              where n.DatumNaloga.Date == datum.Date &&
                                    (novi ? n.IDStatusa == 0 : novi == false)
                              select new _NaloziPauku
                                         (
                                             n.IDNaloga,
                                             n.IDStatusa,
                                             s.NazivStatusa,
                                             n.DatumNaloga
                                         );

                    return nal.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PROMIJENI STATUS VOZILA");
                return null;
            }
        }

        public static List<_StatusVozila> StatusVozila(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List<_StatusVozila> sv = new List<_StatusVozila>();

                    foreach (var v in db.VozilaPaukas.Where(i => i.IDVozila != 0))
                    {
                        try
                        {
                            if (!v.IDTerminala .HasValue)
                            {
                                continue;
                            }

                            decimal? lat = null, lng = null;
                            if (db.LokacijePaukas.Any(i => i.DatumVrijemePauka.Date == DateTime.Today.Date && i.IDVozila == v.IDVozila))
                            {
                                LokacijePauka lp = db.LokacijePaukas.Where(i => i.DatumVrijemePauka.Date == DateTime.Today.Date && i.IDVozila == v.IDVozila)
                                        .OrderByDescending(i => i.DatumVrijemePauka).First();

                                lat = lp.LatPauka;
                                lng = lp.LongPauka;
                            }

                            int br = db.NaloziPaukus.Count(i =>
                                        i.DatumNaloga.Date == DateTime.Today.Date && i.IDVozila == v.IDVozila &&
                                        i.NalogZatvoren == false);

                            DateTime? vr = db.Terminalis.First(i => i.IDTerminala == v.IDTerminala.Value).VrijemeZadnjegPristupa;

                            sv.Add(new _StatusVozila(v.IDVozila, v.IDTerminala, v.NazivVozila, v.AP, lat, lng, br, v.PrivremenaObustava, v.Lisice, vr));
                        }
                        catch 
                        {

                        }
                    }

                    return sv;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI STATUSE VOZILA");
                return null;
            }
        }

        public static bool UkljuciAutoPilot(string grad, bool ukljucen, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PostavkePrograma pp = db.PostavkeProgramas.First();
                    pp.AutoPilot = ukljucen;
                    if (!ukljucen)
                    {
                        pp.AutoPilotOff = DateTime.Now;
                    }
                    db.SubmitChanges();

                    Sustav.SpremiAkciju(grad, -1, 86, "Ukljucen: " + ukljucen, 2, idAplikacije);
                    return pp.AutoPilot;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "UKLJUČI AUTO PILOTA");
                return false;
            }
        }

        public static bool MozePrimitiNalog(string grad, int idVozila, int idNaloga, int idAplikacije)
        {
            try
            {
                const int idStatusa = 14; //14 je vozilo odbilo nalog - prije je bilo 1 ako ne radi vrati jedan

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    NaloziPauku np = db.NaloziPaukus.First(i => i.IDNaloga == idNaloga);
                    VozilaPauka vp = db.VozilaPaukas.First(i => i.IDVozila == idVozila);

                    if (!vp.AP)
                    {
                        return false;
                    }

                    if (np.Lisice)
                    {
                        if (vp.Lisice)
                        {
                            //ako pauk obrađuje lisice i nalog je za blokadu vozila provijeri da li je pauk već odbio
                            return !db.PovijestNalogas.Any(i => i.IDNaloga == idNaloga && i.IDVozila == idVozila && i.IDStatusa == idStatusa);
                        }

                        //pauk ne obrađuje lisice
                        return false;
                    }

                    if (!np.Lisice)
                    {
                        if (vp.ObradjujeNalog)
                        {
                            //ako pauk obrađuje naloge i nalog je za podizanje vozila provijeri da li je pauk već odbio
                            return !db.PovijestNalogas.Any(i => i.IDNaloga == idNaloga && i.IDVozila == idVozila && i.IDStatusa == idStatusa);
                        }

                        //pauk ne obrađuje naloge
                        return false;
                    }

                    return !db.PovijestNalogas.Any(i => i.IDNaloga == idNaloga && i.IDVozila == idVozila && i.IDStatusa == idStatusa);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "MOŽE PRIMITI NALOG");
                return false;
            }
        }

        public static bool SetAutopilotID(string grad, string ID, bool force, out string autopilotID, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    bool autopilot = db.PostavkeProgramas.First().AutoPilot;
                    string id = db.PostavkeProgramas.First().AutopilotID;

                    if (autopilot && force)
                    {
                        PostavkePrograma pp = db.PostavkeProgramas.First();
                        pp.AutopilotID = ID;
                        db.SubmitChanges();

                        var idAuto = from p in db.PostavkeProgramas
                                     select p.AutopilotID;

                        autopilotID = idAuto.First();
                        return idAuto.First() == ID;
                    }

                    if (autopilot && string.IsNullOrEmpty(id))
                    {
                        PostavkePrograma pp = db.PostavkeProgramas.First();
                        pp.AutopilotID = ID;
                        db.SubmitChanges();

                        var idAuto = from p in db.PostavkeProgramas
                                     select p.AutopilotID;

                        autopilotID = idAuto.First();
                        return idAuto.First() == ID;
                    }

                    if (autopilot && id == ID)
                    {
                        autopilotID = id;
                        return true;
                    }

                    if (autopilot && id != ID)
                    {
                        autopilotID = id;
                        return false;
                    }

                    autopilotID = id;
                    return false;
                }
            }
            catch
            {
                autopilotID = "";
                return false;
            }
        }

        public static bool Autopilot(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.PostavkeProgramas.First().AutoPilot;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static DateTime? AutopilotUgasen(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    return db.PostavkeProgramas.First().AutoPilotOff;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool PrivremenaObustava(string grad, int idVozila, bool obustavi, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (!db.VozilaPaukas.Any(i => i.IDVozila == idVozila))
                    {
                        //Sustav.SpremiGresku(grad, new ApplicationException(idVozila.ToString()), idAplikacije, "UKLJUČI AUTO PILOTA");
                        return true;
                    }

                    VozilaPauka vp = db.VozilaPaukas.First(i => i.IDVozila == idVozila);
                    vp.PrivremenaObustava = obustavi;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "UKLJUČI AUTO PILOTA");
                return false;
            }
        }
    }
}