using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Oprema
    {
        public static List<_2DLista> VrstaOpreme(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var op = from o in db.OPREMA_VRSTAs
                             orderby o.VrstaOpreme
                             select new _2DLista(o.IDVrsteOpreme, o.VrstaOpreme);

                    return op.ToList();
                }
            }
            catch (Exception)
            {
                return new List<_2DLista>();
            }
        }

        public static List<_Stanje> Stanje(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    List<_Stanje> nova = new List<_Stanje>();

                    foreach (var o in db.OPREMA_VRSTAs)
                    {
                        if (o.IDVrsteOpreme == 1)
                        {
                            int term = db.TERMINALIs.Count(i => i.IDGrada == 1);
                            int neisp = db.TERMINALIs.Count(i => i.IDGrada == 1 && i.Ispravan == false);
                            int servis = db.OPREMA_SERVIs.Count(i => i.IDVrsteOpreme == 1 && i.Vracen == false);

                            nova.Add(new _Stanje(o.IDVrsteOpreme, o.VrstaOpreme, term - neisp, neisp - servis, servis));
                        }

                        if (o.IDVrsteOpreme == 2)
                        {
                            int term = db.PRINTERIs.Count(i => i.IDGrada == 1);
                            int neisp = db.PRINTERIs.Count(i => i.IDGrada == 1 && i.Ispravan == false);
                            int servis = db.OPREMA_SERVIs.Count(i => i.IDVrsteOpreme == 2 && i.Vracen == false);

                            nova.Add(new _Stanje(o.IDVrsteOpreme, o.VrstaOpreme, term - neisp, neisp - servis, servis));
                        }

                        if (o.IDVrsteOpreme == 3)
                        {
                            int term = db.SIMOVIs.Count(i => i.IDGrada == 1);
                            int neisp = db.SIMOVIs.Count(i => i.IDGrada == 1 && i.Ispravan == false);
                            int servis = db.OPREMA_SERVIs.Count(i => i.IDVrsteOpreme == 3 && i.Vracen == false);

                            nova.Add(new _Stanje(o.IDVrsteOpreme, o.VrstaOpreme, term - neisp, neisp - servis, servis));
                        }

                        if (o.IDVrsteOpreme > 3 && o.IDVrsteOpreme < 9)
                        {
                            int term = db.OPREMA_OSTALOs.Count(i => i.IDVrste == o.IDVrsteOpreme && i.IDGrada == 1);
                            int neisp = db.OPREMA_OSTALOs.Count(i => i.IDVrste == o.IDVrsteOpreme && i.IDGrada == 1 && i.Ispravan == false);
                            int servis = db.OPREMA_SERVIs.Count(i => i.IDVrsteOpreme == o.IDVrsteOpreme && i.Vracen == false);

                            nova.Add(new _Stanje(o.IDVrsteOpreme, o.VrstaOpreme, term - neisp, neisp - servis, servis));
                        }

                        if (o.IDVrsteOpreme == 9)
                        {
                            int term = db.KAMEREs.Count(i => i.IDGrada == 1);
                            int neisp = db.KAMEREs.Count(i => i.IDGrada == 1 && i.Ispravan == false);
                            int servis = db.OPREMA_SERVIs.Count(i => i.IDVrsteOpreme == 9 && i.Vracen == false);

                            nova.Add(new _Stanje(o.IDVrsteOpreme, o.VrstaOpreme, term - neisp, neisp - servis, servis));
                        }
                    }

                    return nova;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "");
                return new List<_Stanje>();
            }
        }

        //model
        public static int DodajModel(string naziv, int idVrste, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    OPREMA_MODEL om = new OPREMA_MODEL();

                    int id = 1;

                    if (db.OPREMA_MODELs.Any())
                    {
                        id = db.OPREMA_MODELs.Max(i => i.IDModela) + 1;
                    }

                    om.IDModela = id;
                    om.IDVrsteOpreme = idVrste;
                    om.NazivModela = naziv;

                    db.OPREMA_MODELs.InsertOnSubmit(om);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "");
                return -1;
            }
        }

        public static bool IzmjeniModel(int idModela, string naziv, int idVrste, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    OPREMA_MODEL om = db.OPREMA_MODELs.First(i => i.IDModela == idModela);

                    //om.IDVrsteOpreme = idVrste;
                    om.NazivModela = naziv;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "");
                return false;
            }
        }

        public static bool? ObrisiModel(int idModela, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (db.OPREMA_OSTALOs.Any(i => i.IDModela == idModela))
                    {
                        return null;
                    }

                    if (db.TERMINALIs.Any(i => i.IDModela == idModela))
                    {
                        return null;
                    }

                    if (db.PRINTERIs.Any(i => i.IDModela == idModela))
                    {
                        return null;
                    }

                    if (db.SIMOVIs.Any(i => i.IDModela == idModela))
                    {
                        return null;
                    }

                    db.OPREMA_MODELs.DeleteOnSubmit(db.OPREMA_MODELs.First(i => i.IDModela == idModela));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "");
                return false;
            }
        }

        public static List<_Model> Modeli(int? idVrste, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var op = from o in db.OPREMA_MODELs
                             where idVrste != null ? o.IDVrsteOpreme == idVrste : idVrste == null
                             orderby o.NazivModela
                             select new _Model(o.IDModela, o.IDVrsteOpreme, o.NazivModela);

                    return op.ToList();
                }
            }
            catch (Exception)
            {
                return new List<_Model>();
            }
        }

        //os
        public static int DodajOS(string naziv, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    OPREMA_O om = new OPREMA_O();

                    int id = 1;

                    if (db.OPREMA_Os.Any())
                    {
                        id = db.OPREMA_Os.Max(i => i.IDOsa) + 1;
                    }

                    om.IDOsa = id;
                    om.NazivOsa = naziv;

                    db.OPREMA_Os.InsertOnSubmit(om);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "");
                return -1;
            }
        }

        public static bool IzmjeniOS(int idOs, string naziv, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    OPREMA_O om = db.OPREMA_Os.First(i => i.IDOsa == idOs);
                    om.NazivOsa = naziv;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "");
                return false;
            }
        }

        public static bool? ObrisiOs(int idOs, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (db.TERMINALIs.Any(i => i.IDOsa == idOs))
                    {
                        return null;
                    }

                    db.OPREMA_Os.DeleteOnSubmit(db.OPREMA_Os.First(i => i.IDOsa == idOs));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "");
                return false;
            }
        }

        public static List<_2DLista> OS(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var op = from o in db.OPREMA_Os
                             orderby o.NazivOsa
                             select new _2DLista(o.IDOsa, o.NazivOsa);

                    return op.ToList();
                }
            }
            catch (Exception)
            {
                return new List<_2DLista>();
            }
        }

        //tarifa
        public static int DodajTarifu(string naziv, string opis, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    SIMOVI_TARIFA om = new SIMOVI_TARIFA();

                    int id = 1;

                    if (db.SIMOVI_TARIFAs.Any())
                    {
                        id = db.SIMOVI_TARIFAs.Max(i => i.IDTarife) + 1;
                    }

                    om.IDTarife = id;
                    om.NazivTarife = naziv;
                    om.OpisTarife = opis;

                    db.SIMOVI_TARIFAs.InsertOnSubmit(om);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "");
                return -1;
            }
        }

        public static bool IzmjeniTarifu(int idTarife, string naziv, string opis, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    SIMOVI_TARIFA om = db.SIMOVI_TARIFAs.First(i => i.IDTarife == idTarife);
                    om.NazivTarife = naziv;
                    om.OpisTarife = opis;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "");
                return false;
            }
        }

        public static bool? ObrisiTarifu(int idTarife, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (db.SIMOVIs.Any(i => i.IDTarife == idTarife))
                    {
                        return null;
                    }

                    db.SIMOVI_TARIFAs.DeleteOnSubmit(db.SIMOVI_TARIFAs.First(i => i.IDTarife == idTarife));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "");
                return false;
            }
        }

        public static List<_3DLista> Tarife(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var op = from o in db.SIMOVI_TARIFAs
                             orderby o.NazivTarife
                             select new _3DLista(o.IDTarife, o.NazivTarife, o.OpisTarife);

                    return op.ToList();
                }
            }
            catch (Exception)
            {
                return new List<_3DLista>();
            }
        }

        /*:: PRETRAGA ::*/

        public static List<_Oprema> SvaOprema(string broj, bool? tip)
        {
            try
            {
                List<_Oprema> sve = new List<_Oprema>();

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var terminali = from p in db.TERMINALIs
                                    join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                                    join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                                    join m in db.OPREMA_MODELs on p.IDModela equals m.IDModela into model
                                    from mm in model.DefaultIfEmpty()
                                    join o in db.OPREMA_Os on p.IDOsa equals o.IDOsa into os
                                    from oo in os.DefaultIfEmpty()
                                    where broj != "" ? (tip == true ? p.InterniBroj.ToString() == broj : p.SerijskiBroj.Contains(broj)) : broj == ""
                                    orderby g.NazivGrada, r.NazivRedarstva
                                    select
                                        new _Oprema(p.IDTerminala, 1, IDStavke(1), "TERMINAL", p.IDGrada, g.NazivGrada, p.IDRedarstva, r.NazivRedarstva, p.IDModela, mm.NazivModela,
                                        p.IDOsa, oo.NazivOsa, 0, "", p.Firmware, p.IdentifikacijskiBroj, p.NazivTerminala, p.InterniBroj, p.SerijskiBroj, p.DatumDolaska,
                                        p.Jamstvo, p.JamstvoDo, p.Vlasnik, p.NazivVlasnika, Ispravan(1, p.IDTerminala, p.Ispravan), "", "");

                    foreach (_Oprema term in terminali)
                    {
                        sve.Add(term);
                    }

                    var printeri = from p in db.PRINTERIs
                                   join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                                   join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                                   join m in db.OPREMA_MODELs on p.IDModela equals m.IDModela into model
                                   from mm in model.DefaultIfEmpty()
                                   where broj != "" ? (tip == true ? p.InterniBroj.ToString() == broj : p.SerijskiBroj.Contains(broj)) : broj == ""
                                   orderby g.NazivGrada, r.NazivRedarstva
                                   select
                                       new _Oprema(p.IDPrintera, 2, IDStavke(2), "PRINTER", p.IDGrada, g.NazivGrada, p.IDRedarstva, r.NazivRedarstva, p.IDModela, mm.NazivModela,
                                       0, "", 0, "", p.PIN, p.Mac, p.NazivPrintera, p.InterniBroj, p.SerijskiBroj, p.DatumDolaska, p.Jamstvo, p.JamstvoDo, p.Vlasnik, p.NazivVlasnika,
                                       Ispravan(2, p.IDPrintera, p.Ispravan), "", "");

                    foreach (_Oprema term in printeri)
                    {
                        sve.Add(term);
                    }

                    var simovi = from p in db.SIMOVIs
                                 join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                                 join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                                 join m in db.OPREMA_MODELs on p.IDModela equals m.IDModela into model
                                 from mm in model.DefaultIfEmpty()
                                 join t in db.SIMOVI_TARIFAs on p.IDTarife equals t.IDTarife into tarifa
                                 from tt in tarifa.DefaultIfEmpty()
                                 where broj != "" ? (tip == true ? p.Interni.ToString() == broj : (tip == false ? p.SerijskiBroj.Contains(broj) : p.Broj.Contains(broj))) : broj == ""
                                 orderby g.NazivGrada, r.NazivRedarstva
                                 select
                                     new _Oprema(p.IDSima, 3, IDStavke(3), "SIM", p.IDGrada ?? 1, g.NazivGrada, p.IDRedarstva ?? 1, r.NazivRedarstva, p.IDModela, mm.NazivModela,
                                     p.IDTarife ?? 1, tt.NazivTarife, 0, "", p.Pin, p.Puk, p.Imsi, p.Interni, p.SerijskiBroj, p.DatumUlaska, null, null, p.Vlasnik, p.NazivVlasnika,
                                     Ispravan(3, p.IDSima, p.Ispravan), p.Broj, p.VPN);

                    foreach (_Oprema term in simovi)
                    {
                        sve.Add(term);
                    }

                    var kamere = from p in db.KAMEREs
                                 join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                                 join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                                 join m in db.OPREMA_MODELs on p.IDModela equals m.IDModela into model
                                 from mm in model.DefaultIfEmpty()
                                 join o in db.OPREMA_MEMORIJAs on p.IDMemorije equals o.IDMemorije into mem
                                 from oo in mem.DefaultIfEmpty()
                                 where broj != "" ? (tip == true ? p.InterniBroj.ToString() == broj : p.SerijskiBroj.Contains(broj)) : broj == ""
                                 orderby g.NazivGrada, r.NazivRedarstva
                                 select
                                     new _Oprema(p.IDKamere, 9, IDStavke(9), "KAMERA", p.IDGrada, g.NazivGrada, p.IDRedarstva, r.NazivRedarstva, p.IDModela, mm.NazivModela,
                                     0, "", p.IDMemorije, oo.KolicinaMemorije, p.Firmware, "", p.NazivKamere, p.InterniBroj, p.SerijskiBroj, p.DatumDolaska,
                                     p.Jamstvo, p.JamstvoDo, p.Vlasnik, p.NazivVlasnika, Ispravan(9, p.IDKamere, p.Ispravan), "", "");

                    foreach (_Oprema term in kamere)
                    {
                        sve.Add(term);
                    }

                    var ostalo = from p in db.OPREMA_OSTALOs
                                 join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                                 join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                                 join m in db.OPREMA_MODELs on p.IDModela equals m.IDModela into model
                                 from mm in model.DefaultIfEmpty()
                                 join o in db.OPREMA_VRSTAs on p.IDVrste equals o.IDVrsteOpreme
                                 where broj != "" ? (tip == true ? p.InterniBroj.ToString() == broj : p.SerijskiBroj.Contains(broj)) : broj == ""
                                 orderby g.NazivGrada, r.NazivRedarstva
                                 select
                                     new _Oprema(p.IDOpreme, p.IDVrste, IDStavke(p.IDVrste), o.VrstaOpreme.ToUpper(), p.IDGrada, g.NazivGrada, p.IDRedarstva, r.NazivRedarstva, p.IDModela, mm.NazivModela,
                                     0, "", 0, "", "", "", "", p.InterniBroj, p.SerijskiBroj, p.DatumDolaska, p.Jamstvo, p.JamstvoDo, p.Vlasnik, p.NazivVlasnika, Ispravan(p.IDVrste, p.IDOpreme, p.Ispravan), "", "");

                    foreach (_Oprema term in ostalo)
                    {
                        sve.Add(term);
                    }

                    return sve;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static _Oprema Terminal(string id)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var terminali = from p in db.TERMINALIs
                                    join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                                    join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                                    join m in db.OPREMA_MODELs on p.IDModela equals m.IDModela into model
                                    from mm in model.DefaultIfEmpty()
                                    join o in db.OPREMA_Os on p.IDOsa equals o.IDOsa into os
                                    from oo in os.DefaultIfEmpty()
                                    where p.IdentifikacijskiBroj == id
                                    orderby g.NazivGrada, r.NazivRedarstva
                                    select
                                        new _Oprema(p.IDTerminala, 1, IDStavke(1), "TERMINAL", p.IDGrada, g.NazivGrada, p.IDRedarstva, r.NazivRedarstva, p.IDModela, mm.NazivModela,
                                        p.IDOsa, oo.NazivOsa, 0, "", p.Firmware, p.IdentifikacijskiBroj, p.NazivTerminala, p.InterniBroj, p.SerijskiBroj, p.DatumDolaska,
                                        p.Jamstvo, p.JamstvoDo, p.Vlasnik, p.NazivVlasnika, Ispravan(1, p.IDTerminala, p.Ispravan), "", "");

                    if (terminali.Any())
                    {
                        return terminali.First();
                    }

                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static List<_Oprema> PretraziOpremu(int idVrste, string broj, bool? tip)
        {
            try
            {
                if (idVrste == 0)
                {
                    return SvaOprema(broj, tip);
                }

                if (idVrste == 1)
                {
                    using (PostavkeDataContext db = new PostavkeDataContext())
                    {
                        var prin = from p in db.TERMINALIs
                                   join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                                   join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                                   join m in db.OPREMA_MODELs on p.IDModela equals m.IDModela into model
                                   from mm in model.DefaultIfEmpty()
                                   join o in db.OPREMA_Os on p.IDOsa equals o.IDOsa into os
                                   from oo in os.DefaultIfEmpty()
                                   where broj != "" ? (tip == true ? p.InterniBroj.ToString() == broj : p.SerijskiBroj.ToUpper().Contains(broj.ToUpper())) : broj == ""
                                   orderby g.NazivGrada, r.NazivRedarstva
                                   select
                                       new _Oprema(p.IDTerminala, idVrste, IDStavke(1), "TERMINAL", p.IDGrada, g.NazivGrada, p.IDRedarstva, r.NazivRedarstva, p.IDModela, mm.NazivModela,
                                       p.IDOsa, oo.NazivOsa, 0, "", p.Firmware, p.IdentifikacijskiBroj, p.NazivTerminala, p.InterniBroj, p.SerijskiBroj, p.DatumDolaska,
                                       p.Jamstvo, p.JamstvoDo, p.Vlasnik, p.NazivVlasnika, Ispravan(1, p.IDTerminala, p.Ispravan), "", "");

                        return prin.ToList();
                    }
                }
                if (idVrste == 2)
                {
                    using (PostavkeDataContext db = new PostavkeDataContext())
                    {
                        var prin = from p in db.PRINTERIs
                                   join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                                   join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                                   join m in db.OPREMA_MODELs on p.IDModela equals m.IDModela into model
                                   from mm in model.DefaultIfEmpty()
                                   where broj != "" ? (tip == true ? p.InterniBroj.ToString() == broj : p.SerijskiBroj.Contains(broj)) : broj == ""
                                   orderby g.NazivGrada, r.NazivRedarstva
                                   select
                                       new _Oprema(p.IDPrintera, idVrste, IDStavke(2), "PRINTER", p.IDGrada, g.NazivGrada, p.IDRedarstva, r.NazivRedarstva, p.IDModela, mm.NazivModela,
                                       0, "", 0, "", p.PIN, p.Mac, p.NazivPrintera, p.InterniBroj, p.SerijskiBroj, p.DatumDolaska, p.Jamstvo, p.JamstvoDo, p.Vlasnik, p.NazivVlasnika,
                                       Ispravan(2, p.IDPrintera, p.Ispravan), "", "");

                        return prin.ToList();
                    }

                }

                if (idVrste == 3)
                {
                    using (PostavkeDataContext db = new PostavkeDataContext())
                    {
                        var prin = from p in db.SIMOVIs
                                   join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                                   join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                                   join m in db.OPREMA_MODELs on p.IDModela equals m.IDModela into model
                                   from mm in model.DefaultIfEmpty()
                                   join t in db.SIMOVI_TARIFAs on p.IDTarife equals t.IDTarife into tarifa
                                   from tt in tarifa.DefaultIfEmpty()
                                   where broj != "" ? (tip == true ? p.Interni.ToString() == broj : (tip == false ? p.SerijskiBroj.Contains(broj) : p.Broj.Contains(broj))) : broj == ""
                                   orderby g.NazivGrada, r.NazivRedarstva
                                   select
                                       new _Oprema(p.IDSima, idVrste, IDStavke(3), "SIM", p.IDGrada ?? 1, g.NazivGrada, p.IDRedarstva ?? 1, r.NazivRedarstva, p.IDModela, mm.NazivModela,
                                       p.IDTarife ?? 1, tt.NazivTarife, 0, "", p.Pin, p.Puk, p.Imsi, p.Interni, p.SerijskiBroj, p.DatumUlaska, null, null, p.Vlasnik, p.NazivVlasnika,
                                       Ispravan(3, p.IDSima, p.Ispravan), p.Broj, p.VPN);

                        return prin.ToList();
                    }
                }

                if (idVrste == 9)
                {
                    using (PostavkeDataContext db = new PostavkeDataContext())
                    {
                        var kamere = from p in db.KAMEREs
                                     join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                                     join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                                     join m in db.OPREMA_MODELs on p.IDModela equals m.IDModela into model
                                     from mm in model.DefaultIfEmpty()
                                     join o in db.OPREMA_MEMORIJAs on p.IDMemorije equals o.IDMemorije into mem
                                     from oo in mem.DefaultIfEmpty()
                                     where broj != "" ? (tip == true ? p.InterniBroj.ToString() == broj : p.SerijskiBroj.Contains(broj)) : broj == ""
                                     orderby g.NazivGrada, r.NazivRedarstva
                                     select
                                         new _Oprema(p.IDKamere, 9, IDStavke(9), "KAMERA", p.IDGrada, g.NazivGrada, p.IDRedarstva, r.NazivRedarstva, p.IDModela, mm.NazivModela,
                                         0, "", p.IDMemorije, oo.KolicinaMemorije, p.Firmware, "", p.NazivKamere, p.InterniBroj, p.SerijskiBroj, p.DatumDolaska,
                                         p.Jamstvo, p.JamstvoDo, p.Vlasnik, p.NazivVlasnika, Ispravan(9, p.IDKamere, p.Ispravan), "", "");

                        return kamere.ToList();
                    }
                }

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var prin = from p in db.OPREMA_OSTALOs
                               join g in db.GRADOVIs on p.IDGrada equals g.IDGrada
                               join r in db.REDARSTVAs on p.IDRedarstva equals r.IDRedarstva
                               join m in db.OPREMA_MODELs on p.IDModela equals m.IDModela into model
                               from mm in model.DefaultIfEmpty()
                               join v in db.OPREMA_VRSTAs on p.IDVrste equals v.IDVrsteOpreme into vrsta
                               from vv in vrsta.DefaultIfEmpty()
                               where broj != "" ? (tip == true ? p.InterniBroj.ToString() == broj : p.SerijskiBroj.Contains(broj)) : broj == "" &&
                                     p.IDVrste == idVrste
                               orderby g.NazivGrada, r.NazivRedarstva
                               select
                                   new _Oprema(p.IDOpreme, p.IDVrste, IDStavke(p.IDVrste), vv.VrstaOpreme.ToUpper(), p.IDGrada, g.NazivGrada, p.IDRedarstva, r.NazivRedarstva, p.IDModela, mm.NazivModela,
                                   0, "", 0, "", "", "", "", p.InterniBroj, p.SerijskiBroj, p.DatumDolaska, p.Jamstvo, p.JamstvoDo, p.Vlasnik, p.NazivVlasnika, Ispravan(p.IDVrste, p.IDOpreme, p.Ispravan), "", "");

                    return prin.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int IDStavke(int idVrste)
        {
            using (PostavkeDataContext db = new PostavkeDataContext())
            {
                return db.OPREMA_VRSTAs.First(i => i.IDVrsteOpreme == idVrste).IDStavke.Value;
            }
        }

        /*:: SERVIS ::*/

        public static bool Servis(_Servis servis)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    OPREMA_SERVI s;
                    int id = 1;

                    if (servis.Vracen)
                    {
                        s = db.OPREMA_SERVIs.First(i => i.Vracen == false && i.IDOpreme == servis.IDOpreme);
                    }
                    else
                    {
                        s = new OPREMA_SERVI();

                        if (db.OPREMA_SERVIs.Any())
                        {
                            id = db.OPREMA_SERVIs.Max(i => i.IDServisa) + 1;
                        }

                        s.IDServisa = id;
                    }

                    s.IDVrsteOpreme = servis.IDVrste;
                    s.IDOpreme = servis.IDOpreme;
                    s.Vracen = servis.Vracen;
                    s.Napomena = "";

                    if (servis.Vracen)
                    {
                        s.Napomena = servis.Napomena;
                        s.DatumPovratka = servis.DatumPovratka;
                    }
                    else
                    {
                        s.Datum = servis.Datum;
                        s.OpisProblema = servis.OpisProblema;
                    }

                    if (servis.Vracen == false)
                    {
                        db.OPREMA_SERVIs.InsertOnSubmit(s);
                    }

                    db.SubmitChanges();

                    if (servis.IDVrste == 1)
                    {
                        db.TERMINALIs.First(i => i.IDTerminala == servis.IDOpreme).Ispravan = servis.Vracen;
                        db.SubmitChanges();
                    }

                    if (servis.IDVrste == 2)
                    {
                        db.PRINTERIs.First(i => i.IDPrintera == servis.IDOpreme).Ispravan = servis.Vracen;
                        db.SubmitChanges();
                    }

                    if (servis.IDVrste == 3)
                    {
                        db.SIMOVIs.First(i => i.IDSima == servis.IDOpreme).Ispravan = servis.Vracen;
                        db.SubmitChanges();
                    }

                    if (servis.IDVrste > 3)
                    {
                        db.OPREMA_OSTALOs.First(i => i.IDOpreme == servis.IDOpreme).Ispravan = servis.Vracen;
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool? Ispravan(int idVrste, int idOpreme, bool ispravan)
        {
            using (PostavkeDataContext db = new PostavkeDataContext())
            {
                if (db.OPREMA_SERVIs.Any(i => i.IDOpreme == idOpreme && i.IDVrsteOpreme == idVrste && i.Vracen == false))
                {
                    return null;
                }

                return ispravan;
            }
        }

        /*:: IZMJENA ::*/

        public static bool Izmjeni(_Oprema oprema, int idAplikacije)
        {
            switch (oprema.IDVrsteOpreme)
            {
                case 1:
                    return IzmjeniTerminal(oprema, idAplikacije);
                case 2:
                    return IzmjeniPrinter(oprema, idAplikacije);
                case 3:
                    return IzmjeniSim(oprema, idAplikacije);
                case 9:
                    return IzmjeniKameru(oprema, idAplikacije);
            }

            return IzmjeniOpremu(oprema, idAplikacije);
        }

        private static bool IzmjeniKameru(_Oprema oprema, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    KAMERE ter = db.KAMEREs.First(i => i.IDKamere == oprema.IDOpreme);

                    string napomena = "";
                    if (ter.IDGrada != oprema.IDGrada)
                    {
                        napomena += string.Format("Izmjenio JLS: {0} => {1}", Postavke.Grad(ter.IDGrada), Postavke.Grad(oprema.IDGrada));
                    }

                    if (ter.IDRedarstva != oprema.IDRedarstva)
                    {
                        string red = napomena != "" ? "\r\n" : "";
                        napomena += string.Format("{0}Izmjenio redarstvo: {1} => {2}", red, Postavke.Redarstvo(ter.IDRedarstva), Postavke.Redarstvo(oprema.IDRedarstva));
                    }

                    if (ter.NazivKamere != oprema.Naziv)
                    {
                        string red = napomena != "" ? "\r\n" : "";
                        napomena += string.Format("{0}Izmjenio naziv: {1} => {2}", red, ter.NazivKamere, oprema.Naziv);
                    }

                    if (ter.Ispravan != oprema.Ispravan)
                    {
                        string red = napomena != "" ? "\r\n" : "";
                        napomena += string.Format("{0}Izmjenio ispravnost: {1} => {2}", red, ter.Ispravan, oprema.Ispravan);
                    }

                    ter.IDGrada = oprema.IDGrada;
                    ter.IDRedarstva = oprema.IDRedarstva;
                    ter.IDModela = oprema.IDModela.Value;
                    ter.IDMemorije = oprema.IDMemorije;
                    ter.IDModela = oprema.IDModela.Value;
                    ter.NazivKamere = oprema.Naziv;
                    ter.NazivVlasnika = oprema.NazivVlasnika;
                    ter.InterniBroj = oprema.InterniBroj.Value;
                    ter.SerijskiBroj = oprema.SerijskiBroj;
                    ter.DatumDolaska = oprema.DatumUlaska;
                    ter.Jamstvo = oprema.Jamstvo;
                    ter.JamstvoDo = oprema.DatumJamstva;
                    ter.Vlasnik = oprema.Vlasnik.Value;
                    ter.Firmware = oprema.PinFirmware;
                    ter.Ispravan = oprema.Ispravan.Value;

                    db.SubmitChanges();

                    PovijestOpreme(oprema.IDOpreme, oprema.IDVrsteOpreme, 1, napomena, DateTime.Now, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (TERMINAL)");
                return false;
            }
        }

        private static bool IzmjeniTerminal(_Oprema oprema, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    TERMINALI ter = db.TERMINALIs.First(i => i.IDTerminala == oprema.IDOpreme);

                    string napomena = "";
                    if (ter.IDGrada != oprema.IDGrada)
                    {
                        napomena += string.Format("Izmjenio JLS: {0} => {1}", Postavke.Grad(ter.IDGrada), Postavke.Grad(oprema.IDGrada));
                    }

                    if (ter.IDRedarstva != oprema.IDRedarstva)
                    {
                        string red = napomena != "" ? "\r\n" : "";
                        napomena += string.Format("{0}Izmjenio redarstvo: {1} => {2}", red, Postavke.Redarstvo(ter.IDRedarstva), Postavke.Redarstvo(oprema.IDRedarstva));
                    }

                    if (ter.NazivTerminala != oprema.Naziv)
                    {
                        string red = napomena != "" ? "\r\n" : "";
                        napomena += string.Format("{0}Izmjenio naziv: {1} => {2}", red, ter.NazivTerminala, oprema.Naziv);
                    }

                    if (ter.Ispravan != oprema.Ispravan)
                    {
                        string red = napomena != "" ? "\r\n" : "";
                        napomena += string.Format("{0}Izmjenio ispravnost: {1} => {2}", red, ter.Ispravan, oprema.Ispravan);
                    }

                    ter.IDGrada = oprema.IDGrada;
                    ter.IDRedarstva = oprema.IDRedarstva;
                    ter.NazivTerminala = oprema.Naziv;
                    ter.IdentifikacijskiBroj = oprema.MacIDB;
                    ter.IDModela = oprema.IDModela.Value;
                    ter.IDOsa = oprema.IDOsa;
                    ter.SerijskiBroj = oprema.SerijskiBroj;
                    ter.DatumDolaska = oprema.DatumUlaska;
                    ter.Jamstvo = oprema.Jamstvo;
                    ter.JamstvoDo = oprema.DatumJamstva;
                    ter.Vlasnik = oprema.Vlasnik.Value;
                    ter.Firmware = oprema.PinFirmware;
                    ter.Ispravan = oprema.Ispravan.Value;
                    ter.NazivVlasnika = oprema.NazivVlasnika;

                    db.SubmitChanges();

                    PovijestOpreme(oprema.IDOpreme, oprema.IDVrsteOpreme, 1, napomena, DateTime.Now, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (TERMINAL)");
                return false;
            }
        }

        private static bool IzmjeniPrinter(_Oprema oprema, int idAplikacije)
        {
            try
            {
                _Printer printer = new _Printer(oprema.IDOpreme, oprema.IDGrada, oprema.IDRedarstva, "", oprema.PinFirmware, oprema.MacIDB, oprema.Naziv,
                    oprema.InterniBroj.Value, oprema.IDModela.Value, oprema.SerijskiBroj, oprema.DatumUlaska, oprema.Jamstvo, oprema.DatumJamstva, oprema.Vlasnik,
                    oprema.Ispravan.Value);

                return Postavke.IzmjeniPrinter("", printer, oprema.NazivVlasnika, idAplikacije);
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (PINTER)");
                return false;
            }
        }

        private static bool IzmjeniSim(_Oprema oprema, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    SIMOVI ter = db.SIMOVIs.First(i => i.IDSima == oprema.IDOpreme);

                    string napomena = "";
                    if (ter.IDGrada != oprema.IDGrada)
                    {
                        napomena += string.Format("Izmjenio JLS: {0} => {1}", Postavke.Grad(ter.IDGrada), Postavke.Grad(oprema.IDGrada));
                    }

                    if (ter.IDRedarstva != oprema.IDRedarstva)
                    {
                        string red = napomena != "" ? "\r\n" : "";
                        napomena += string.Format("{0}Izmjenio redarstvo: {1} => {2}", red, Postavke.Redarstvo(ter.IDRedarstva), Postavke.Redarstvo(oprema.IDRedarstva));
                    }

                    ter.IDGrada = oprema.IDGrada;
                    ter.IDRedarstva = oprema.IDRedarstva;
                    ter.IDTarife = oprema.IDOsa;
                    ter.IDModela = oprema.IDModela.Value;
                    ter.SerijskiBroj = oprema.SerijskiBroj;
                    ter.Imsi = oprema.Naziv;
                    ter.Pin = oprema.PinFirmware;
                    ter.Puk = oprema.MacIDB;
                    ter.VPN = oprema.VPN;
                    ter.Broj = oprema.Broj;
                    ter.DatumUlaska = oprema.DatumUlaska;
                    ter.Vlasnik = oprema.Vlasnik.Value;
                    ter.Ispravan = oprema.Ispravan.Value;
                    ter.NazivVlasnika = oprema.NazivVlasnika;

                    db.SubmitChanges();

                    PovijestOpreme(oprema.IDOpreme, oprema.IDVrsteOpreme, 1, napomena, DateTime.Now, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (TERMINAL)");
                return false;
            }
        }

        private static bool IzmjeniOpremu(_Oprema oprema, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    OPREMA_OSTALO ter = db.OPREMA_OSTALOs.First(i => i.IDOpreme == oprema.IDOpreme);

                    string napomena = "";
                    if (ter.IDGrada != oprema.IDGrada)
                    {
                        napomena += string.Format("Izmjenio JLS: {0} => {1}", Postavke.Grad(ter.IDGrada), Postavke.Grad(oprema.IDGrada));
                    }

                    if (ter.IDRedarstva != oprema.IDRedarstva)
                    {
                        string red = napomena != "" ? "\r\n" : "";
                        napomena += string.Format("{0}Izmjenio redarstvo: {1} => {2}", red, Postavke.Redarstvo(ter.IDRedarstva), Postavke.Redarstvo(oprema.IDRedarstva));
                    }

                    ter.IDGrada = oprema.IDGrada;
                    ter.IDRedarstva = oprema.IDRedarstva;
                    ter.IDModela = oprema.IDModela.Value;
                    ter.SerijskiBroj = oprema.SerijskiBroj;
                    ter.DatumDolaska = oprema.DatumUlaska;
                    ter.Jamstvo = oprema.Jamstvo;
                    ter.JamstvoDo = oprema.DatumJamstva;
                    ter.Vlasnik = oprema.Vlasnik.Value;
                    ter.Ispravan = oprema.Ispravan.Value;
                    ter.NazivVlasnika = oprema.NazivVlasnika;

                    db.SubmitChanges();

                    PovijestOpreme(oprema.IDOpreme, oprema.IDVrsteOpreme, 1, napomena, DateTime.Now, idAplikacije);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (OSTALO)");
                return false;
            }
        }

        /*:: IZMJENA INTERNOG ::*/

        public static bool IzmjeniInterni(int idVrsteOpreme, int idOpreme, int interniBroj, int idAplikacije)
        {
            switch (idVrsteOpreme)
            {
                case 1:
                    return IzmjeniTerminalInterni(idOpreme, interniBroj, idAplikacije);
                case 2:
                    return IzmjeniPrinterInterni(idOpreme, interniBroj, idAplikacije);
                case 3:
                    return IzmjeniSimInterni(idOpreme, interniBroj, idAplikacije);
            }

            return IzmjeniOpremuInterni(idOpreme, interniBroj, idAplikacije);
        }

        private static bool IzmjeniTerminalInterni(int idOpreme, int interniBroj, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    TERMINALI ter = db.TERMINALIs.First(i => i.IDTerminala == idOpreme);
                    ter.InterniBroj = interniBroj;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (TERMINAL)- INTERNI");
                return false;
            }
        }

        private static bool IzmjeniPrinterInterni(int idOpreme, int interniBroj, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    PRINTERI novi = db.PRINTERIs.Single(p => p.IDPrintera == idOpreme);
                    novi.InterniBroj = interniBroj;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (PINTER)- INTERNI");
                return false;
            }
        }

        private static bool IzmjeniSimInterni(int idOpreme, int interniBroj, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    SIMOVI ter = db.SIMOVIs.First(i => i.IDSima == idOpreme);
                    ter.Interni = interniBroj;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (TERMINAL)- INTERNI");
                return false;
            }
        }

        private static bool IzmjeniOpremuInterni(int idOpreme, int interniBroj, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    OPREMA_OSTALO ter = db.OPREMA_OSTALOs.First(i => i.IDOpreme == idOpreme);
                    ter.InterniBroj = interniBroj;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (OSTALO) - INTERNI");
                return false;
            }
        }

        /*:: POVJEST ::*/

        public static void PovijestOpreme(int idOpreme, int idVrste, int idStatusa, string napomena, DateTime datum, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    OPREMA_POVIJEST pp = new OPREMA_POVIJEST();

                    pp.IDOpreme = idOpreme;
                    pp.IDVrsteOpreme = idVrste;
                    pp.Datum = datum;
                    pp.IDStatusa = idStatusa;
                    pp.Status = "";
                    pp.Napomena = napomena;

                    db.OPREMA_POVIJESTs.InsertOnSubmit(pp);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "POVIJEST PRINTERA");
            }
        }

        public static void DodajBiljesku(string id, int idStatusa, string napomena, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    OPREMA_POVIJEST pp = new OPREMA_POVIJEST();

                    pp.IDOpreme = db.TERMINALIs.First(i => i.IdentifikacijskiBroj == id).IDTerminala;
                    pp.IDVrsteOpreme = 1;
                    pp.Datum = DateTime.Now;
                    pp.IDStatusa = idStatusa;
                    pp.Status = "";
                    pp.Napomena = napomena;

                    db.OPREMA_POVIJESTs.InsertOnSubmit(pp);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "POVIJEST PRINTERA");
            }
        }

        public static bool ObrisiPovijestOpreme(int idPovijesti, bool servis, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (servis)
                    {
                        db.OPREMA_SERVIs.DeleteOnSubmit(db.OPREMA_SERVIs.First(i => i.IDServisa == idPovijesti));
                        db.SubmitChanges();
                    }
                    else
                    {
                        db.OPREMA_POVIJESTs.DeleteOnSubmit(db.OPREMA_POVIJESTs.First(i => i.IDPovijesti == idPovijesti));
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "POVIJEST PRINTERA");
                return false;
            }
        }

        public static List<_PovijestOpreme> DohvatiPovijestOpreme(int idOpreme, int idVrste, int idStatusa, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (idVrste == 1)
                    {
                        var pov = from p in db.OPREMA_POVIJESTs
                            join s in db.OPREMA_POVIJEST_STATUS on p.IDStatusa equals s.IDStatusa
                            join v in db.OPREMA_VRSTAs on p.IDVrsteOpreme equals v.IDVrsteOpreme
                            join t in db.TERMINALIs on p.IDOpreme equals t.IDTerminala
                            where (idOpreme != 0 ? p.IDOpreme == idOpreme : idOpreme == 0) &&
                                  (idVrste != 0 ? p.IDVrsteOpreme == idVrste : idVrste == 0) &&
                                  (idStatusa != 0 ? p.IDStatusa == idStatusa : idStatusa == 0)
                            select new _PovijestOpreme(p.IDPovijesti, p.IDOpreme, p.IDVrsteOpreme, v.VrstaOpreme, s.IDStatusa, p.Datum, s.NazivStatusa, p.Napomena, t.NazivTerminala + " (" + t.InterniBroj + ")", false);

                        return pov.ToList();
                    }
                    if (idVrste == 2)
                    {
                        var pov = from p in db.OPREMA_POVIJESTs
                            join s in db.OPREMA_POVIJEST_STATUS on p.IDStatusa equals s.IDStatusa
                            join v in db.OPREMA_VRSTAs on p.IDVrsteOpreme equals v.IDVrsteOpreme
                            join t in db.PRINTERIs on p.IDOpreme equals t.IDPrintera
                            where (idOpreme != 0 ? p.IDOpreme == idOpreme : idOpreme == 0) &&
                                  (idVrste != 0 ? p.IDVrsteOpreme == idVrste : idVrste == 0) &&
                                  (idStatusa != 0 ? p.IDStatusa == idStatusa : idStatusa == 0)
                            select new _PovijestOpreme(p.IDPovijesti, p.IDOpreme, p.IDVrsteOpreme, v.VrstaOpreme, s.IDStatusa, p.Datum, s.NazivStatusa, p.Napomena, t.NazivPrintera + " (" + t.InterniBroj + ")", false);

                        return pov.ToList();
                    }

                    if (idVrste == 3)
                    {
                        var pov = from p in db.OPREMA_POVIJESTs
                            join s in db.OPREMA_POVIJEST_STATUS on p.IDStatusa equals s.IDStatusa
                            join v in db.OPREMA_VRSTAs on p.IDVrsteOpreme equals v.IDVrsteOpreme
                            join t in db.SIMOVIs on p.IDOpreme equals t.IDSima
                            where (idOpreme != 0 ? p.IDOpreme == idOpreme : idOpreme == 0) &&
                                  (idVrste != 0 ? p.IDVrsteOpreme == idVrste : idVrste == 0) &&
                                  (idStatusa != 0 ? p.IDStatusa == idStatusa : idStatusa == 0)
                            select new _PovijestOpreme(p.IDPovijesti, p.IDOpreme, p.IDVrsteOpreme, v.VrstaOpreme, s.IDStatusa, p.Datum, s.NazivStatusa, p.Napomena, "INT.:" + t.Interni, false);

                        return pov.ToList();

                    }

                        var pov1 = from p in db.OPREMA_POVIJESTs
                            join s in db.OPREMA_POVIJEST_STATUS on p.IDStatusa equals s.IDStatusa
                            join v in db.OPREMA_VRSTAs on p.IDVrsteOpreme equals v.IDVrsteOpreme
                            join t in db.OPREMA_OSTALOs on p.IDOpreme equals t.IDOpreme
                            where (idOpreme != 0 ? p.IDOpreme == idOpreme : idOpreme == 0) &&
                                  (idVrste != 0 ? p.IDVrsteOpreme == idVrste : idVrste == 0) &&
                                  (idStatusa != 0 ? p.IDStatusa == idStatusa : idStatusa == 0)
                            select new _PovijestOpreme(p.IDPovijesti, p.IDOpreme, p.IDVrsteOpreme, v.VrstaOpreme, s.IDStatusa, p.Datum, s.NazivStatusa, p.Napomena, "INT.:" + t.InterniBroj, false);

                        return pov1.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "POVIJEST PRINTERA");
                return new List<_PovijestOpreme>();
            }
        }

        public static int NoviStatus(string status, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    OPREMA_POVIJEST_STATUS ops = new OPREMA_POVIJEST_STATUS();

                    int id = 1;

                    if (db.OPREMA_POVIJEST_STATUS.Any())
                    {
                        id = db.OPREMA_POVIJEST_STATUS.Max(i => i.IDStatusa) + 1;
                    }

                    ops.IDStatusa = id;
                    ops.NazivStatusa = status;

                    db.OPREMA_POVIJEST_STATUS.InsertOnSubmit(ops);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static List<_2DLista> StatusiPovijesti()
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var op = from o in db.OPREMA_POVIJEST_STATUS
                             orderby o.NazivStatusa
                             select new _2DLista(o.IDStatusa, o.NazivStatusa);

                    return op.ToList();
                }
            }
            catch (Exception)
            {
                return new List<_2DLista>();
            }
        }

        /*:: DODAJ ::*/

        public static int InterniBroj(int idAplikacije)
        {
            try
            {
                List<int> broj = new List<int>();

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (db.PRINTERIs.Any())
                    {
                        broj.Add(db.PRINTERIs.Max(i => i.InterniBroj) + 1);
                    }
                    if (db.TERMINALIs.Any())
                    {
                        broj.Add(db.TERMINALIs.Max(i => i.InterniBroj) + 1);
                    }
                    if (db.SIMOVIs.Any())
                    {
                        broj.Add(db.SIMOVIs.Max(i => i.Interni) + 1);
                    }
                    if (db.OPREMA_OSTALOs.Any())
                    {
                        broj.Add(db.OPREMA_OSTALOs.Max(i => i.InterniBroj) + 1);
                    }
                    if (db.KAMEREs.Any())
                    {
                        broj.Add(db.KAMEREs.Max(i => i.InterniBroj) + 1);
                    }

                    return broj.Max();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "");
                return -1;
            }
        }

        public static int Dodaj(_Oprema oprema, int idAplikacije)
        {
            switch (oprema.IDVrsteOpreme)
            {
                case 1:
                    return DodajTerminal(oprema, idAplikacije);
                case 2:
                    return DodajPrinter(oprema, idAplikacije);
                case 3:
                    return DodajSim(oprema, idAplikacije);
                case 9:
                    return DodajKameru(oprema, idAplikacije);
            }

            return DodajOpremu(oprema, idAplikacije);
        }

        private static int DodajKameru(_Oprema oprema, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    KAMERE ter = new KAMERE();

                    ter.IDGrada = oprema.IDGrada;
                    ter.IDRedarstva = oprema.IDRedarstva;
                    ter.IDModela = oprema.IDModela.Value;
                    ter.IDMemorije = oprema.IDMemorije;
                    ter.IDModela = oprema.IDModela.Value;
                    ter.NazivKamere = oprema.Naziv;
                    ter.NazivVlasnika = oprema.NazivVlasnika;
                    ter.InterniBroj = oprema.InterniBroj.Value;
                    ter.SerijskiBroj = oprema.SerijskiBroj;
                    ter.DatumDolaska = oprema.DatumUlaska;
                    ter.Jamstvo = oprema.Jamstvo;
                    ter.JamstvoDo = oprema.DatumJamstva;
                    ter.Vlasnik = oprema.Vlasnik.Value;
                    ter.Firmware = oprema.PinFirmware;
                    ter.Ispravan = oprema.Ispravan.Value;

                    db.KAMEREs.InsertOnSubmit(ter);
                    db.SubmitChanges();

                    PovijestOpreme(oprema.IDOpreme, oprema.IDVrsteOpreme, 2, "Dodao novu kameru", DateTime.Now, idAplikacije);

                    return ter.IDKamere;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "DODAJ OPREMU (KAMERA)");
                return -1;
            }
        }

        private static int DodajTerminal(_Oprema oprema, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    TERMINALI ter = new TERMINALI();

                    int id = 1;

                    if (db.TERMINALIs.Any())
                    {
                        id = db.TERMINALIs.Max(i => i.IDTerminala) + 1;
                    }

                    ter.IDTerminala = id;
                    ter.IDGrada = oprema.IDGrada;
                    ter.IDRedarstva = oprema.IDRedarstva;
                    ter.InterniBroj = oprema.InterniBroj.Value;
                    ter.NazivTerminala = oprema.Naziv;
                    ter.IdentifikacijskiBroj = oprema.MacIDB;
                    ter.IDModela = oprema.IDModela.Value;
                    ter.IDOsa = oprema.IDOsa;
                    ter.SerijskiBroj = oprema.SerijskiBroj;
                    ter.DatumDolaska = oprema.DatumUlaska;
                    ter.Jamstvo = oprema.Jamstvo;
                    ter.JamstvoDo = oprema.DatumJamstva;
                    ter.Vlasnik = oprema.Vlasnik.Value;
                    ter.Firmware = oprema.PinFirmware;
                    ter.Ispravan = oprema.Ispravan.Value;
                    ter.Verzija = "";
                    ter.NazivVlasnika = oprema.NazivVlasnika;

                    db.TERMINALIs.InsertOnSubmit(ter);
                    db.SubmitChanges();

                    PovijestOpreme(oprema.IDOpreme, oprema.IDVrsteOpreme, 2, "Dodao novi terminal", DateTime.Now, idAplikacije);

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "DODAJ OPREMU (TERMINAL)");
                return -1;
            }
        }

        private static int DodajPrinter(_Oprema oprema, int idAplikacije)
        {
            try
            {
                _Printer printer = new _Printer(oprema.IDOpreme, oprema.IDGrada, oprema.IDRedarstva, "", oprema.PinFirmware, oprema.MacIDB, oprema.Naziv,
                    oprema.InterniBroj.Value, oprema.IDModela.Value, oprema.SerijskiBroj, oprema.DatumUlaska, oprema.Jamstvo, oprema.DatumJamstva, oprema.Vlasnik,
                    oprema.Ispravan.Value);

                return Postavke.DodajPrinter("", printer, oprema.NazivVlasnika, idAplikacije);
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (PINTER)");
                return -1;
            }
        }

        private static int DodajSim(_Oprema oprema, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    SIMOVI ter = new SIMOVI();

                    int id = 1;

                    if (db.SIMOVIs.Any())
                    {
                        id = db.SIMOVIs.Max(i => i.IDSima) + 1;
                    }

                    ter.IDSima = id;
                    ter.Interni = oprema.InterniBroj.Value;
                    ter.IDGrada = oprema.IDGrada;
                    ter.IDRedarstva = oprema.IDRedarstva;
                    ter.IDTarife = oprema.IDOsa;
                    ter.IDModela = oprema.IDModela.Value;
                    ter.SerijskiBroj = oprema.SerijskiBroj;
                    ter.Imsi = oprema.Naziv;
                    ter.Pin = oprema.PinFirmware;
                    ter.Puk = oprema.MacIDB;
                    ter.VPN = oprema.VPN;
                    ter.Broj = oprema.Broj;
                    ter.DatumUlaska = oprema.DatumUlaska;
                    ter.Vlasnik = oprema.Vlasnik.Value;
                    ter.Ispravan = oprema.Ispravan.Value;
                    ter.NazivVlasnika = oprema.NazivVlasnika;

                    db.SIMOVIs.InsertOnSubmit(ter);
                    db.SubmitChanges();

                    PovijestOpreme(oprema.IDOpreme, oprema.IDVrsteOpreme, 2, "Dodao novi SIM", DateTime.Now, idAplikacije);

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (TERMINAL)");
                return -1;
            }
        }

        private static int DodajOpremu(_Oprema oprema, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    OPREMA_OSTALO ter = new OPREMA_OSTALO();

                    int id = 1;

                    if (db.OPREMA_OSTALOs.Any())
                    {
                        id = db.OPREMA_OSTALOs.Max(i => i.IDOpreme) + 1;
                    }

                    ter.IDVrste = oprema.IDVrsteOpreme;
                    ter.IDOpreme = id;
                    ter.IDGrada = oprema.IDGrada;
                    ter.InterniBroj = oprema.InterniBroj.Value;
                    ter.IDRedarstva = oprema.IDRedarstva;
                    ter.IDModela = oprema.IDModela.Value;
                    ter.SerijskiBroj = oprema.SerijskiBroj;
                    ter.DatumDolaska = oprema.DatumUlaska;
                    ter.Jamstvo = oprema.Jamstvo;
                    ter.JamstvoDo = oprema.DatumJamstva;
                    ter.Vlasnik = oprema.Vlasnik.Value;
                    ter.Ispravan = oprema.Ispravan.Value;
                    ter.NazivVlasnika = oprema.NazivVlasnika;

                    db.OPREMA_OSTALOs.InsertOnSubmit(ter);
                    db.SubmitChanges();

                    PovijestOpreme(oprema.IDOpreme, oprema.IDVrsteOpreme, 2, "Dodao novu opremu", DateTime.Now, idAplikacije);

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI OPREMU (OSTALO)");
                return -1;
            }
        }

        public static string IzmjeniTerminal(string grad, string verzija, string naziv, string id, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    TERMINALI ter = db.TERMINALIs.First(i => i.IdentifikacijskiBroj == id);

                    if (ter == null)
                    {
                        return "";
                    }

                    if (!string.IsNullOrEmpty(naziv))
                    {
                        ter.NazivTerminala = naziv;
                    }

                    ter.Verzija = verzija;
                    ter.VrijemeZadnjegPristupa = DateTime.Now;
                    db.SubmitChanges();

                    if (db.OPREMA_Os.Any(i => i.IDOsa == ter.IDOsa))
                    {
                        return db.OPREMA_Os.First(i => i.IDOsa == ter.IDOsa).NazivOsa;
                    }

                    return "";
                }
            }
            catch (InvalidOperationException)
            {
                return "";
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI OPREMU (TERMINAL) - AKTIVNOST: " + id);
                return "";
            }
        }

        /*:: ::*/

        public static bool DodajPrilog(List<_Prilog> prilog, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    foreach (var p in prilog)
                    {
                        OPREMA_PRILOG ter = new OPREMA_PRILOG();

                        ter.IDVrsteOpreme = p.IDVrsteOpreme;
                        ter.IDOpreme = p.IDOpreme;
                        ter.Vrijeme = p.Vrijeme;
                        ter.Opis = p.Opis;
                        ter.Prilog = p.Prilog;
                        ter.Ekstenzija = p.Ekstenzija;

                        db.OPREMA_PRILOGs.InsertOnSubmit(ter);
                        db.SubmitChanges();

                        PovijestOpreme(p.IDOpreme, p.IDVrsteOpreme, 2, "Dodao novi prilog", DateTime.Now, idAplikacije);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "DODAO PRILOG");
                return false;
            }
        }

        public static List<_Prilog> Prilozi(int idVrste, int idOpreme, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var pril = from p in db.OPREMA_PRILOGs
                               where p.IDVrsteOpreme == idVrste &&
                                     p.IDOpreme == idOpreme
                               select new _Prilog(p.IDPriloga, p.IDVrsteOpreme, p.IDOpreme, p.Vrijeme, p.Opis, null, p.Ekstenzija);

                    return pril.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "DOHVATIO PRILOGE");
                return new List<_Prilog>();
            }
        }

        public static _Prilog Prilog(int idPriloga, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var pril = from p in db.OPREMA_PRILOGs
                               where p.IDPriloga == idPriloga
                               select new _Prilog(p.IDPriloga, p.IDVrsteOpreme, p.IDOpreme, p.Vrijeme, p.Opis, p.Prilog.ToArray(), p.Ekstenzija);

                    return pril.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "PRILOG");
                return null;
            }
        }

        public static bool ObrisiPrilog(int idPriloga, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    db.OPREMA_PRILOGs.DeleteOnSubmit(db.OPREMA_PRILOGs.First(i => i.IDPriloga == idPriloga));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "OBRISI PRILOG");
                return false;
            }
        }

        public static List<_2DLista> Memorija(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var op = from o in db.OPREMA_MEMORIJAs
                             select new _2DLista(o.IDMemorije, o.KolicinaMemorije);

                    return op.ToList();
                }
            }
            catch (Exception)
            {
                return new List<_2DLista>();
            }
        }
    }
}