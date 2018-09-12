using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Statistika
    {
        #region POSTAVKE

        public static List<_2DLista> PopisNaglasenihUlica(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var Ulice = from p in db.NaglaseneUlices
                                select new _2DLista(p.IDUlice, p.NazivUlice);

                    return Ulice.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Popis Naglasenih Ulica");
                return new List<_2DLista>();
            }
        }

        public static bool ObrisiNaglasenuUlicu(string grad, int idUlice, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.NaglaseneUlices.DeleteOnSubmit(db.NaglaseneUlices.First(i => i.IDUlice == idUlice));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Obrisi Naglasenu Ulicu");
                return false;
            }
        }

        public static int DodajNaglasenuUlicu(string grad, string ulica, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    NaglaseneUlice n = new NaglaseneUlice();
                    n.NazivUlice = ulica.ToUpper();

                    db.NaglaseneUlices.InsertOnSubmit(n);
                    db.SubmitChanges();

                    return n.IDUlice;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dodaj Naglasenu Ulicu");
                return -1;
            }
        }

        private static string Mjesec(int mjesec, int godina)
        {
            switch (mjesec)
            {
                case 1:
                    return string.Format("Siječanj, {0}", godina);
                case 2:
                    return string.Format("Veljača, {0}", godina);
                case 3:
                    return string.Format("Ožujak, {0}", godina);
                case 4:
                    return string.Format("Travanj, {0}", godina);
                case 5:
                    return string.Format("Svibanj, {0}", godina);
                case 6:
                    return string.Format("Lipanj, {0}", godina);
                case 7:
                    return string.Format("Srpanj, {0}", godina);
                case 8:
                    return string.Format("Kolovoz, {0}", godina);
                case 9:
                    return string.Format("Rujan, {0}", godina);
                case 10:
                    return string.Format("Listopad, {0}", godina);
                case 11:
                    return string.Format("Studeni, {0}", godina);
                case 12:
                    return string.Format("Prosinac, {0}", godina);
            }

            return "";
        }

        private static string Drzava(string kratica, out bool VP)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var dr = from d in db.DRZAVAs
                             where d.Kratica == kratica
                             select d;

                    VP = dr.First().NaplataVP;
                    return dr.First().Naziv;
                }
            }
            catch (Exception)
            {
                VP = false;
                return kratica;
            }
        }

        #endregion

        #region DOHVAT PODATAKA

        //data
        public static _Aktivnost Aktivnost(string grad, DateTime datum, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                   where p.Vrijeme.Value.Date == datum.Date &&
                                         p.Test == false &&
                                         p.Status == false &&
                                         (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1)
                                   select new { p, i.NazivPredloska };

                    int obavijesti = rezultat.Count(i => i.NazivPredloska == "OBAVIJEST");
                    int upozorenje = rezultat.Count(i => i.NazivPredloska == "UPOZORENJE");

                    bool pauk, lisice;

                    using (PostavkeDataContext pb = new PostavkeDataContext())
                    {
                        pauk = pb.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad)).Pauk;
                        lisice = pb.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad)).Lisice;
                    }

                    int? lisica = lisice ? db.NaloziPaukus.Count(i => i.Lisice && i.DatumNaloga.Date == datum.Date) : 0;

                    int? naloga = pauk ? rezultat.Count(i => i.p.NalogPauka == true) - lisica : null;

                    int? ukupno = pauk ? db.Zahtjevis.Count(i => i.DatumVrijeme.Date == datum.Date) : (int?)null;
                    int? odobrenih = pauk
                        ? db.Zahtjevis.Count(i => i.DatumVrijeme.Date == datum.Date && i.IDStatusa == 3)
                        : (int?)null;

                    return new _Aktivnost(obavijesti, upozorenje, naloga, ukupno, odobrenih, lisica);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Izdanih Obavijesti");
                return null;
            }
        }

        //pie
        public static List<_Statistika> IzdanihObavijesti(string grad, DateTime datumOd, DateTime datumDo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         i.NazivPredloska == "OBAVIJEST" &&
                                         p.Test == false &&
                                         p.Status == false
                                   select p;

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => i.Kazna))
                    {
                        try
                        {
                            if (q.Key == 0)
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            novi.Add(new _Statistika(q.Key + " kn", q.Count(), posto, (q.Key * q.Count()).ToString("n"), DateTime.Today, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Izdanih Obavijesti");
                return null;
            }
        }

        public static List<_Statistika> NaloziPauku(string grad, DateTime datumOd, DateTime datumDo, int IDVozila, bool zahtjevi, int idOpisa, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Pauks
                                   join z in db.Prekrsajis on p.IDNaloga equals z.IDNaloga
                                   join s in db.StatusPaukas on p.Status equals s.IDStatusa
                                   join n in db.NaloziPaukus on p.IDNaloga equals n.IDNaloga
                                   join x in db.Prekrsajis on p.IDNaloga equals x.IDNaloga
                                   where p.DatumNaloga.Date >= datumOd &&
                                         p.DatumNaloga.Date <= datumDo &&
                                         (idRedarstva != -1 ? z.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                         ((IDVozila == 0) || (IDVozila != 0 && n.IDVozila == IDVozila)) &&
                                         (zahtjevi ? x.Zahtjev : zahtjevi == false) &&
                                         (idOpisa != 0 ? x.IDSkracenogOpisa == idOpisa : idOpisa == 0)
                                   select new { p, s };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => i.s.NazivStatusa))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            novi.Add(new _Statistika(q.Key, q.Count(), posto, "", DateTime.Today, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Nalozi Pauku");
                return null;
            }
        }

        public static List<_Statistika> UpozorenjaObavijesti(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                         p.Test == false &&
                                         p.Status == false
                                   select new { p, i };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => i.i.NazivPredloska))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            novi.Add(new _Statistika(q.Key, q.Count(), posto, "", DateTime.Today, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, " Upozorenja Obavijesti");
                return null;
            }
        }

        public static List<_Statistika> StatusVPP(string grad, DateTime datumOd, DateTime datumDo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 240;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         p.Test == false &&
                                         p.StatusVPP != null &&
                                         i.NazivPredloska == "OBAVIJEST"
                                   select new { p, i };

                    List<_Statistika> novi = new List<_Statistika>();

                    var nepreneseni = from p in db.Prekrsajis
                                      join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                      where p.Vrijeme.Value.Date >= datumOd.Date &&
                                            p.Vrijeme.Value.Date <= datumDo.Date &&
                                            p.Test == false &&
                                            p.Status == false &&
                                            p.Poslano.Value == false &&
                                            p.StatusVPP == null &&
                                            i.NazivPredloska == "OBAVIJEST"
                                      select new { p, i };

                    if (nepreneseni.Any())
                    {
                        decimal poston = Math.Round((decimal)nepreneseni.Count() / (nepreneseni.Count() + rezultat.Count()) * 100, 2);
                        novi.Add(new _Statistika("Nepreneseni", nepreneseni.Count(), poston, "", DateTime.Today, null));
                    }

                    foreach (var q in rezultat.GroupBy(i => i.p.StatusVPP))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                            novi.Add(new _Statistika(q.Key, q.Count(), posto, "", DateTime.Today, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Status VPP");
                return null;
            }
        }

        public static List<_Statistika> PoslanihZahtjeva(string grad, DateTime datumOd, DateTime datumDo, int IDVozila, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from z in db.Zahtjevis
                                   join v in db.VozilaPaukas on z.IDPrijavitelja equals v.IDVozila
                                   where z.DatumVrijeme.Date >= datumOd &&
                                         z.DatumVrijeme.Date <= datumDo &&
                                         (idRedarstva != -1 ? z.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                         ((IDVozila == 0) || (IDVozila != 0 && z.IDPrijavitelja == IDVozila))
                                   select new { z, v };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => i.z.IDStatusa))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            if (q.Key == -1)
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            novi.Add(new _Statistika(Zahtjev.Status(q.Key), q.Count(), posto, "", DateTime.Today, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ZAHTJEVI PAUKA");
                return null;
            }
        }

        //bar
        public static List<_Statistika> UpozorenjaObavijestiMjesecno(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 240;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                         p.Test == false &&
                                         p.Status == false
                                   orderby i.IDPRedloska descending
                                   select new { p, i };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => new { i.p.Vrijeme.Value.Date.Year, i.p.Vrijeme.Value.Date.Month }))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            List<_Statistika> detalji = new List<_Statistika>();

                            foreach (var dok in q.GroupBy(i => i.i.NazivPredloska))
                            {
                                decimal posto1 = Math.Round((decimal)dok.Count() / q.Count() * 100, 2);
                                detalji.Add(new _Statistika(dok.Key, dok.Count(), posto1,
                                    Mjesec(q.Key.Month, q.First().p.Vrijeme.Value.Year), DateTime.Today, null));
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                            novi.Add(new _Statistika(Mjesec(q.Key.Month, q.First().p.Vrijeme.Value.Year), q.Count(),
                                posto, "", new DateTime(q.Key.Year, q.Key.Month, 1), detalji));
                        }
                        catch
                        {
                        }
                    }

                    return novi.OrderBy(i => i.Datum).ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Upozorenja Obavijesti Mjesecno");
                return null;
            }
        }

        public static List<_Statistika> UpozorenjaObavijestiPoPrekrsajima(string grad, DateTime datumOd, DateTime datumDo, bool nalozi, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                   join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         (nalozi ? p.NalogPauka == true : !nalozi) &&
                                         (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                         p.Test == false &&
                                         p.Status == false
                                   orderby i.IDPRedloska descending
                                   select new { p, i, o };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => new { i.o.KratkiOpis }))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            List<_Statistika> detalji = new List<_Statistika>();

                            foreach (var dok in q.GroupBy(i => i.i.NazivPredloska))
                            {
                                decimal posto1 = Math.Round((decimal)dok.Count() / q.Count() * 100, 2);
                                detalji.Add(new _Statistika(dok.Key, dok.Count(), posto1, q.Key.KratkiOpis, DateTime.Today, null));
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                            novi.Add(new _Statistika(q.Key.KratkiOpis, q.Count(), posto, "", DateTime.Today, detalji));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Upozorenja Obavijesti Po Prekrsajima");
                return null;
            }
        }

        public static List<_Statistika> RedariPrekrsaji(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije));
                db.CommandTimeout = 60;
                db.ObjectTrackingEnabled = false;

                var rezultat = from p in db.Prekrsajis
                               join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                               join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                               where p.Vrijeme.Value.Date >= datumOd.Date &&
                                     p.Vrijeme.Value.Date <= datumDo.Date &&
                                     (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                     p.Test == false &&
                                     p.Status == false &&
                                     d.PrikaziStatistika
                               select new { p, i, d };

                List<_Statistika> novi = new List<_Statistika>();

                foreach (var q in rezultat.GroupBy(i => new { i.d.ImePrezime }))
                {
                    try
                    {
                        if (!q.Any())
                        {
                            continue;
                        }

                        List<_Statistika> detalji = new List<_Statistika>();

                        foreach (var dok in q.GroupBy(i => i.i.NazivPredloska))
                        {
                            decimal posto1 = Math.Round((decimal)dok.Count() / q.Count() * 100, 2);
                            detalji.Add(new _Statistika(dok.Key, dok.Count(), posto1, q.Key.ImePrezime, DateTime.Today, null));
                        }

                        if (detalji.Count == 1)
                        {
                            if (detalji.First().Naziv == "Obavijest".ToUpper())
                            {
                                detalji.Add(new _Statistika("Upozorenje".ToUpper(), 0, 0, q.Key.ImePrezime, DateTime.Today, null));
                            }
                            else
                            {
                                detalji.Add(new _Statistika("Obavijest".ToUpper(), 0, 0, q.Key.ImePrezime, DateTime.Today, null));
                            }
                        }

                        decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                        novi.Add(new _Statistika(q.Key.ImePrezime, q.Count(), posto, q.Key.ImePrezime, DateTime.Today, detalji.OrderBy(i => i.Naziv).ToList()));
                    }
                    catch
                    {
                    }
                }

                return novi.OrderBy(i => i.Naziv).ToList();
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Redari Prekrsaji");
                return null;
            }
        }

        public static List<_Statistika> NalogaPoPrekrsajima(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                         p.NalogPauka == true
                                   select new { p, o };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => new { i.o.KratkiOpis }))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            List<_Statistika> detalji = new List<_Statistika>();
                            detalji.Add(new _Statistika(q.Key.KratkiOpis, q.Count(), posto, q.Key.KratkiOpis, DateTime.Today, null));

                            novi.Add(new _Statistika(q.Key.KratkiOpis, q.Count(), posto, "", DateTime.Today, detalji));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Upozorenja Obavijesti Po Prekrsajima");
                return null;
            }
        }

        public static List<_Statistika> NalogaPoPrekrsajimaSatus(string grad, DateTime datumOd, DateTime datumDo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa
                                   join b in db.NaloziPaukus on p.IDNaloga equals b.IDNaloga
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         p.NalogPauka == true &&
                                         (b.IDStatusa == 3 || b.IDStatusa == 4)
                                   select new { p, o, b };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => new { i.o.KratkiOpis }))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            List<_Statistika> detalji = new List<_Statistika>();

                            foreach (var dok in q.GroupBy(i => i.b.IDStatusa))
                            {
                                decimal posto1 = Math.Round((decimal)dok.Count() / q.Count() * 100, 2);
                                detalji.Add(new _Statistika(dok.Key == 3 ? "Pokušaj podizanja" : "Stavio vozilo na deponij", dok.Count(), posto1, q.Key.KratkiOpis, DateTime.Today, null));
                            }

                            novi.Add(new _Statistika(q.Key.KratkiOpis, q.Count(), posto, "", DateTime.Today, detalji));
                        }
                        catch
                        {

                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "nalozi Po Prekrsajima i statusu");
                return null;
            }
        }

        public static List<_Statistika> NaglaseneUlice(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    List<_Statistika> novi = new List<_Statistika>();

                    var Ulice = from p in db.NaglaseneUlices
                                where p.NazivUlice.Trim() != ""
                                select p.NazivUlice;

                    var uk = from p in db.Prekrsajis
                             join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                             where p.Vrijeme.Value.Date >= datumOd.Date &&
                                   p.Vrijeme.Value.Date <= datumDo.Date &&
                                   (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                   p.Test == false &&
                                   p.Status == false
                             select p;

                    foreach (var ul in Ulice)
                    {
                        var rezultat = from p in db.Prekrsajis
                                       join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                       where p.Vrijeme.Value.Date >= datumOd.Date &&
                                             p.Vrijeme.Value.Date <= datumDo.Date &&
                                             (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                             p.Adresa.ToUpper().Contains(ul.ToUpper()) &&
                                             p.Test == false &&
                                             p.Status == false
                                       orderby i.IDPRedloska descending
                                       select new { p, i };

                        List<_Statistika> detalji = new List<_Statistika>();

                        foreach (var q in rezultat.GroupBy(i => i.i.NazivPredloska))
                        {
                            try
                            {
                                if (!q.Any())
                                {
                                    continue;
                                }

                                decimal posto1 = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                                detalji.Add(new _Statistika(q.Key, q.Count(), posto1, ul.ToUpper(), DateTime.Today, null));
                            }
                            catch
                            {
                            }
                        }

                        decimal posto = Math.Round((decimal)rezultat.Count() / uk.Count() * 100, 2);
                        novi.Add(new _Statistika(ul.ToUpper(), rezultat.Count(), posto, "", DateTime.Today, detalji.Count == 0 ? null : detalji));
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Naglasene Ulice");
                return null;
            }
        }

        public static List<_Statistika> PrekrsajaPoDrzavi(string grad, DateTime datumOd, DateTime datumDo, bool izuzmihr, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                   join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                         (izuzmihr ? p.KraticaDrzave != "HR" : izuzmihr == false) &&
                                         p.Test == false &&
                                         p.Status == false
                                   orderby i.IDPRedloska descending
                                   select new { p, i, o };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => i.p.KraticaDrzave))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            List<_Statistika> detalji = new List<_Statistika>();

                            foreach (var dok in q.GroupBy(i => i.i.NazivPredloska))
                            {
                                decimal posto1 = Math.Round((decimal)dok.Count() / q.Count() * 100, 2);
                                detalji.Add(new _Statistika(dok.Key, dok.Count(), posto1, q.Key, DateTime.Today, null));
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                            novi.Add(new _Statistika(q.Key, q.Count(), posto, "", DateTime.Today, detalji));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Prekrsaja Po Drzavi");
                return null;
            }
        }

        public static List<_Statistika> ObavijestiNaloziPauku(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska into naziv
                                   from i in naziv.DefaultIfEmpty()
                                   join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa
                                   join n in db.NaloziPaukus on p.IDNaloga equals n.IDNaloga into ima
                                   from n in ima.DefaultIfEmpty()
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                         p.Test == false &&
                                         p.Status == false
                                   orderby i.IDPRedloska descending
                                   select new { u = 1, p, i, o, n };

                    List<_Statistika> novi = new List<_Statistika>();

                    int kazni = 0, mogucnosti = 0, izdanih = 0;
                    foreach (var q in rezultat.GroupBy(i => i.u))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            List<_Statistika> detalji = new List<_Statistika>();

                            foreach (var dok in q.GroupBy(i => i.i.NazivPredloska))
                            {
                                decimal posto1 = Math.Round((decimal)dok.Count() / q.Count() * 100, 2);
                                detalji.Add(new _Statistika(dok.Key, dok.Count(), posto1,
                                    "Izdanih kazni (" + q.Count() + ")", DateTime.Today, null));
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                            novi.Add(new _Statistika("Izdanih kazni (" + q.Count() + ")", q.Count(), posto, "",
                                DateTime.Today, detalji));
                            kazni = q.Count();
                        }
                        catch
                        {
                        }
                    }

                    foreach (var q in rezultat.Where(i => i.o.ClanakPauka != "").GroupBy(i => i.u))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            List<_Statistika> detalji = new List<_Statistika>();
                            decimal postoIzdanih = Math.Round((decimal)q.Count() / kazni * 100, 2);

                            foreach (var dok in q.GroupBy(i => i.i.NazivPredloska))
                            {
                                decimal posto1 = Math.Round((decimal)dok.Count() / q.Count() * 100, 2);
                                detalji.Add(new _Statistika(dok.Key, dok.Count(), posto1,
                                    "Postoji pravna osnova (" + postoIzdanih + "%)", DateTime.Today, null));
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                            novi.Add(new _Statistika("Postoji pravna osnova (" + postoIzdanih + "%)", q.Count(), posto,
                                "", DateTime.Today, detalji));
                            mogucnosti = q.Count();
                        }
                        catch
                        {
                        }
                    }

                    foreach (var q in rezultat.Where(i => i.p.NalogPauka == true).GroupBy(i => i.u))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            List<_Statistika> detalji = new List<_Statistika>();
                            decimal postoIzdanih = Math.Round((decimal)q.Count() / mogucnosti * 100, 2);

                            foreach (var dok in q.GroupBy(i => i.i.NazivPredloska))
                            {
                                decimal posto1 = Math.Round((decimal)dok.Count() / q.Count() * 100, 2);
                                detalji.Add(new _Statistika(dok.Key, dok.Count(), posto1,
                                    "Izdanih naloga (" + postoIzdanih + "%)", DateTime.Today, null));
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                            novi.Add(new _Statistika("Izdanih naloga (" + postoIzdanih + "%)", q.Count(), posto, "",
                                DateTime.Today, detalji));
                            izdanih = q.Count();
                        }
                        catch
                        {
                        }
                    }

                    foreach (var q in rezultat.Where(i => i.n.IDStatusa == 4 || i.n.IDStatusa == 3).GroupBy(i => i.u))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            List<_Statistika> detalji = new List<_Statistika>();
                            decimal postoIzdanih = Math.Round((decimal)q.Count() / izdanih * 100, 2);

                            foreach (var dok in q.GroupBy(i => i.i.NazivPredloska))
                            {
                                decimal posto1 = Math.Round((decimal)dok.Count() / q.Count() * 100, 2);
                                detalji.Add(new _Statistika(dok.Key, dok.Count(), posto1,
                                    "Uspješno izvršenih naloga (" + postoIzdanih + "%)", DateTime.Today, null));
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                            novi.Add(new _Statistika("Uspješno izvršenih naloga (" + postoIzdanih + "%)", q.Count(),
                                posto, "", DateTime.Today, detalji));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Obavijesti Nalozi Pauku");
                return null;
            }
        }

        public static List<_Statistika> RazloziOstalog(string grad, DateTime datumOd, DateTime datumDo, int IDVozila, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Pauks
                                   join s in db.StatusPaukas on p.Status equals s.IDStatusa
                                   join n in db.NaloziPaukus on p.IDNaloga equals n.IDNaloga
                                   join r in db.RazloziNepodizanjaVozilas on n.IDRazloga equals r.IDRazloga
                                   where n.IDStatusa == 7 &&
                                         p.DatumNaloga.Date >= datumOd &&
                                         p.DatumNaloga.Date <= datumDo &&
                                         ((IDVozila == 0) || (IDVozila != 0 && n.IDVozila == IDVozila))
                                   select new { p, s, r };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => i.r.NazivRazloga))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            novi.Add(new _Statistika(q.Key, q.Count(), posto, "", DateTime.Today, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Nalozi Pauku");
                return null;
            }
        }

        public static List<_Statistika> IzdanihRacuna(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from r in db.RACUNIs
                                   where (idRedarstva != -1 ? r.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                         r.Datum.Date >= datumOd &&
                                         r.Datum.Date <= datumDo
                                   select r;

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => i.IDVrstePlacanja))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            decimal ukupno = q.Sum(i => i.Ukupno);

                            novi.Add(new _Statistika(Naplata.VrstaPlacanja(grad, q.Key, idAplikacije), q.Count(), posto,
                                Naplata.VrstaPlacanjaDetaljno(grad, q.Key, idAplikacije) + " (" + ukupno.ToString("n2") +
                                " kn)", DateTime.Today, new List<_Statistika>()));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZDANIH RACUNA");
                return null;
            }
        }

        public static List<_Statistika> VPStatusi(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from r in db.VppVanjskoPostupcis
                                   join s in db.VppStatus on r.IDStatusaVP equals s.IDStatusaVP
                                   select new { r, s };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => i.r.IDStatusaVP))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                            novi.Add(new _Statistika(q.First().s.NazivStatusa, q.Count(), posto, "", DateTime.Now,
                                new List<_Statistika>()));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZDANIH RACUNA");
                return null;
            }
        }

        public static List<_Statistika> VPNaplaceni(string grad, int idAplikacije)
        {
            try
            {
                List<_Statistika> novi = new List<_Statistika>();

                using (PostavkeDataContext pd = new PostavkeDataContext())
                {
                    foreach (var g in pd.GRADOVIs.Where(i => i.Odvjetnici && i.Aktivan))
                    {
                        if (g.IDGrada == 1 || g.IDGrada == 6)
                        {
                            continue;
                        }

                        List<_Statistika> detalji = new List<_Statistika>();

                        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(g.Baza, idAplikacije)))
                        {
                            db.CommandTimeout = 60;
                            db.ObjectTrackingEnabled = false;

                            int uplate = db.VppVanjskoUplates.Count();

                            int ukupnoprekrsaja = db.Prekrsajis.Count(i => i.Poslano == true); //obavijesti - gledam prema onima koji su preneseni u vpp
                            int ukupnostranaca = db.Prekrsajis.Count(i => i.Poslano == true && i.KraticaDrzave != "HR" && i.KraticaDrzave != "??"); //obavijesti - gledam prema onima koji su preneseni u vpp
                            int ukupnostranacaplaceno = db.Prekrsajis.Count(i => i.Poslano == true && i.KraticaDrzave != "HR" && i.KraticaDrzave != "??" && i.StatusVPP == "Plaćeno") - uplate; //obavijesti - gledam prema onima koji su preneseni u vpp
                            int ukupnovp = db.VppVanjskoPostupcis.Count();

                            decimal novaca;

                            if (ukupnovp == 0)
                            {
                                continue;
                            }

                            try
                            {
                                novaca = db.VppVanjskoUplates.Sum(i => i.iznos);
                            }
                            catch (Exception)
                            {
                                novaca = 0;
                            }

                            detalji.Add(new _Statistika("Ukupno stranaca", ukupnostranaca, Math.Round((decimal)ukupnostranaca / ukupnoprekrsaja * 100, 2), "Ukupno izdanih kazni strancima", DateTime.Now, new List<_Statistika>()));
                            detalji.Add(new _Statistika("Plaćeni stranci", ukupnostranacaplaceno, Math.Round((decimal)ukupnostranacaplaceno / ukupnostranaca * 100, 2), "Ukupno plaćenih kazni stranaca", DateTime.Now, new List<_Statistika>()));
                            detalji.Add(new _Statistika("Ne plaćeni stranci", ukupnostranaca - ukupnostranacaplaceno, Math.Round((decimal)(ukupnostranaca - ukupnostranacaplaceno) / ukupnostranaca * 100, 2), "Ne plaćene kazne stranaca", DateTime.Now, new List<_Statistika>()));

                            detalji.Add(new _Statistika("Poslanih na vanjsko postupanje", ukupnovp, Math.Round((decimal)ukupnovp / ukupnostranaca * 100, 2), "Poslanih kazni na vanjsko postupanje", DateTime.Now, new List<_Statistika>()));
                            detalji.Add(new _Statistika("Ne poslanih na vanjsko postupanje", ukupnostranaca - ukupnovp, Math.Round((decimal)(ukupnostranaca - ukupnovp - ukupnostranacaplaceno) / ukupnostranaca * 100, 2), "Ne poslanih kazni na vanjsko postupanje", DateTime.Now, new List<_Statistika>()));

                            detalji.Add(new _Statistika("Uspješno naplaćenih", uplate, Math.Round((decimal)uplate / ukupnovp * 100, 2), novaca.ToString(), DateTime.Now, new List<_Statistika>()));

                            novi.Add(new _Statistika(g.NazivGrada, ukupnoprekrsaja, 100, "Ukupno izdanih prekršaja", DateTime.Now, detalji));
                        }
                    }
                }

                return novi;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZDANIH RACUNA");
                return null;
            }
        }

        public static List<_Stranci> VPRazradaDrzave(string grad, int idAplikacije)
        {
            try
            {
                List<_Stranci> novi = new List<_Stranci>();

                using (PostavkeDataContext pd = new PostavkeDataContext())
                {
                    foreach (var g in pd.GRADOVIs.Where(i => i.Odvjetnici && i.Aktivan))
                    {
                        if (g.IDGrada == 1 || g.IDGrada == 6)
                        {
                            continue;
                        }

                        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(g.Baza, idAplikacije)))
                        {
                            db.CommandTimeout = 60;
                            db.ObjectTrackingEnabled = false;

                            var dr = from p in db.Prekrsajis
                                     where p.KraticaDrzave != "HR" &&
                                           p.KraticaDrzave != "??" &&
                                           p.Poslano == true
                                     select p;

                            List<_Stranci> detalji = new List<_Stranci>();

                            foreach (var q in dr.GroupBy(i => i.KraticaDrzave))
                            {
                                var vp = from v in db.VppVanjskoPostupcis
                                         join p in db.Prekrsajis on v.IDPrekrsaja equals p.IDPrekrsaja
                                         where p.KraticaDrzave == q.Key &&
                                               v.status == "A"
                                         select v;

                                var vpu = from vu in db.VppVanjskoUplates
                                          join v in db.VppVanjskoPostupcis on vu.IDVanjskoPostupci equals v.IDVanjskoPostupci
                                          join p in db.Prekrsajis on v.IDPrekrsaja equals p.IDPrekrsaja
                                          where p.KraticaDrzave == q.Key
                                          select v;

                                string drzava = Drzava(q.Key, out var VP);

                                if (!VP)
                                {
                                    drzava += "*";
                                }

                                int poslanoVPD = vp.Count();

                                int ukupnoD = q.Count();
                                int uplateVPD = vpu.Count();
                                int placenihD = q.Count(i => i.Poslano == true && i.StatusVPP == "Plaćeno") - uplateVPD;
                                int obustavljenoD = q.Count(i => i.Poslano == true && i.StatusVPP == "Obustavljeno");
                                int djelomicnoD = q.Count(i => i.Poslano == true && i.StatusVPP == "Djelomično plaćeno");
                                int nenaplacenoD = q.Count(i => i.Poslano == true && i.StatusVPP == "Nije plaćeno") - (poslanoVPD - uplateVPD);
                                int ostaloD = ukupnoD - obustavljenoD - djelomicnoD - nenaplacenoD - placenihD - poslanoVPD;

                                detalji.Add(new _Stranci(drzava, ukupnoD, placenihD, obustavljenoD, djelomicnoD, ostaloD, poslanoVPD, uplateVPD, nenaplacenoD, null));
                            }

                            int poslanoVP = db.VppVanjskoPostupcis.Count(i => i.status == "A");
                            int ukupno = db.Prekrsajis.Count(i => i.Poslano == true && i.KraticaDrzave != "HR" && i.KraticaDrzave != "??");
                            int uplateVP = db.VppVanjskoUplates.Count();
                            int placenih = db.Prekrsajis.Count(i => i.Poslano == true && i.KraticaDrzave != "HR" && i.KraticaDrzave != "??" && i.StatusVPP == "Plaćeno") - uplateVP;
                            int obustavljeno = db.Prekrsajis.Count(i => i.Poslano == true && i.KraticaDrzave != "HR" && i.KraticaDrzave != "??" && i.StatusVPP == "Obustavljeno");
                            int djelomicno = db.Prekrsajis.Count(i => i.Poslano == true && i.KraticaDrzave != "HR" && i.KraticaDrzave != "??" && i.StatusVPP == "Djelomično plaćeno");
                            int nenaplaceno = db.Prekrsajis.Count(i => i.Poslano == true && i.KraticaDrzave != "HR" && i.KraticaDrzave != "??" && i.StatusVPP == "Nije plaćeno") - (poslanoVP - uplateVP);
                            int ostalo = ukupno - obustavljeno - djelomicno - nenaplaceno - placenih - poslanoVP;

                            novi.Add(new _Stranci(g.NazivGrada, ukupno, placenih, obustavljeno, djelomicno, ostalo, poslanoVP, uplateVP, nenaplaceno, detalji.OrderByDescending(i => i.NaplacenihVP).ToList()));
                            //break;
                        }
                    }
                }

                return novi;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZDANIH RACUNA");
                return null;
            }
        }

        //bar - line
        public static List<_Statistika> ProsjekPoDanu2(string grad, DateTime datumOd, DateTime datumDo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                   join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         d.PrikaziStatistika &&
                                         p.Test == false &&
                                         p.Status == false
                                   orderby p.IDPredloskaIspisa descending
                                   select new { p, d, i };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => new { i.d.ImePrezime }))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            List<_Statistika> detalji = new List<_Statistika>();

                            foreach (var dok in q.GroupBy(i => i.i.NazivPredloska))
                            {
                                decimal posto1 = Math.Round((decimal)dok.Count() / q.Count() * 100, 2);
                                detalji.Add(new _Statistika(dok.Key, dok.Count(), posto1, q.Key.ImePrezime, DateTime.Today, null));
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);
                            novi.Add(new _Statistika(q.Key.ImePrezime, q.Count(), posto, "", DateTime.Today, detalji));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Prosjek Po Danu");
                return null;
            }
        }

        public static List<_Statistika> ProsjekPoDanu(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var res = from x in
                             (
                                 from p in db.Prekrsajis
                                 join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                                 where p.Vrijeme.Value.Date >= datumOd.Date &&
                                       p.Vrijeme.Value.Date <= datumDo.Date &&
                                       (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                       d.PrikaziStatistika &&
                                       p.Test == false &&
                                       p.Status == false
                                 group p by new { d.ImePrezime, p.Vrijeme.Value.Date } into g
                                 orderby g.Key.ImePrezime, g.Key.Date
                                 select new { g.Key.ImePrezime, g.Key.Date, br = g.Count() }).AsEnumerable()
                              group x by new { x.ImePrezime }
                             into y
                              select
                                  new _Statistika
                                  (
                                      y.Key.ImePrezime,
                                      (int)y.Average(aa => aa.br),
                                      y.Count(), //Broj dana
                                      "",
                                      new DateTime(),
                                      null //Prosjek prekrsaja
                                  );

                    return res.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Prosjek Po Danu");
                return null;
            }
        }

        public static List<_Statistika> TrajanjePostupka(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join d in db.Djelatniks on p.IDDjelatnika equals d.IDDjelatnika
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         p.TrajanjePostupka != null &&
                                         p.TrajanjePostupka != 0 &&
                                         (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                         d.PrikaziStatistika &&
                                         p.Test == false
                                   orderby p.IDPredloskaIspisa descending
                                   select new { p, d, i };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => new { i.d.ImePrezime }))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            novi.Add(new _Statistika(q.Key.ImePrezime, (int)q.Average(i => i.p.TrajanjePostupka).Value / 1000, q.Count(), "", DateTime.Today, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Trajanje Postupka");
                return null;
            }
        }

        public static List<_Statistika> Kilometri(string grad, DateTime datumOd, DateTime datumDo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var tocke = from p in db.LokacijePaukas
                                join t in db.Terminalis on p.IDTerminala equals t.IDTerminala
                                join v in db.VozilaPaukas on p.IDVozila equals v.IDVozila
                                where p.DatumVrijemePauka.Date >= datumOd.Date &&
                                      p.DatumVrijemePauka.Date <= datumDo.Date &&
                                      p.Brzina > 1.1 && p.GPSAcc < 35
                                orderby p.DatumVrijemePauka ascending
                                select new
                                {
                                    p.LatPauka,
                                    p.LongPauka,
                                    v.IDVozila,
                                    v.NazivVozila,
                                    p.DatumVrijemePauka
                                };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var v in tocke.GroupBy(i => new { i.IDVozila, i.DatumVrijemePauka.Date }))
                    {
                        if (v.Key.IDVozila == 0)
                        {
                            continue;
                        }

                        double ukupno = 0;
                        var x = v.ToList();
                        int to = v.Count();

                        for (int i = 0; i < to - 1; i++)
                        {
                            if (i + 1 > to - 1)
                            {
                                break;
                            }

                            double udaljenost = Geocoordinates.distance((double)x[i].LatPauka, (double)x[i].LongPauka,
                                (double)x[i + 1].LatPauka, (double)x[i + 1].LongPauka, 'K');

                            if (udaljenost < 1)
                            {
                                ukupno += udaljenost;
                            }
                        }

                        novi.Add(new _Statistika(v.First().NazivVozila, (int)ukupno, 0, v.First().DatumVrijemePauka.Date.ToString("dd.MM.yy"), v.First().DatumVrijemePauka.Date, null));
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PUTANJA PAUKA");
                return new List<_Statistika>();
            }
        }

        public static List<_Statistika> IntenzitetRada(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.Prekrsajis
                                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1) &&
                                         p.Test == false &&
                                         p.Status == false
                                   select new { p, i };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => i.p.Vrijeme.Value.Date))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            novi.Add(new _Statistika("Postupanja", q.Count(), posto, q.Key.ToString("dd.MM.yy"), q.Key, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi.OrderBy(i => i.Datum).ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "INTENZITET");
                return null;
            }
        }

        public static List<_Statistika> AktivnostTerminal(string grad, DateTime datumOd, DateTime datumDo, int idDjelatnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from l in db.Lokacijes
                                   where l.IDDjelatnika == idDjelatnika &&
                                         l.DatumVrijeme >= datumOd &&
                                         l.DatumVrijeme <= datumDo
                                   select l.DatumVrijeme;

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (IGrouping<DateTime, DateTime> vrijeme in rezultat.GroupBy(i => i.Date))
                    {
                        try
                        {
                            if (!vrijeme.Any())
                            {
                                continue;
                            }

                            List<DateTime> vremena = new List<DateTime>();

                            foreach (var dateTime in vrijeme)
                            {
                                if (dateTime > vrijeme.Key.AddHours(5))
                                {
                                    vremena.Add(dateTime);
                                }
                            }

                            if (!vremena.Any())
                            {
                                continue;
                            }

                            double min = 0;
                            DateTime prije = vremena.First();

                            DateTime pocetak = vremena.First();
                            DateTime kraj = vremena.OrderByDescending(i => i).First();

                            TimeSpan ukupno = kraj.Subtract(pocetak);

                            List<_Statistika> pauze = new List<_Statistika>();
                            for (int i = 0; i < vremena.Count - 1; i++)
                            {
                                try
                                {
                                    DateTime poslije = vremena.ElementAt(i);
                                    if (poslije.Subtract(prije).TotalMinutes > 10)
                                    {
                                        double pauza = poslije.Subtract(prije).TotalMinutes;
                                        min += pauza;

                                        decimal posto = (decimal)Math.Round(pauza / ukupno.TotalMinutes * 100, 2);
                                        pauze.Add(new _Statistika(string.Format("Mirovanje: ~ {0} min", (int)pauza), (int)pauza, posto, string.Format("Mirovanje od {0:HH:mm} do {1:HH:mm} ({2} min)", prije, poslije, (int)pauza), vrijeme.Key, null));
                                    }

                                    prije = vremena.ElementAt(i);
                                }
                                catch
                                {

                                }
                            }

                            if (ukupno.TotalMinutes > 0)
                            {
                                novi.Add(new _Statistika(string.Format("{0:dd.MM.yy} ({1:hh\\:mm})", pocetak, ukupno), (int)ukupno.TotalMinutes, (int)min, string.Format("Počeo u {0:HH:mm} završio u {1:HH:mm} (ukupno: {2:hh\\:mm\\:ss}, Mirovanje: {3} min)", pocetak, kraj, ukupno, (int)min), vrijeme.Key, pauze));
                            }
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "AKTIVNOST NATERMINALU");
                return null;
            }
        }

        public static List<_Statistika> TrajanjeBaterije(string grad, DateTime datum, int idTerminala, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from l in db.Lokacijes
                                   join t in db.Terminalis on l.IDTerminala equals t.IDTerminala
                                   where l.DatumVrijeme.Date == datum &&
                                         l.IDTerminala == idTerminala
                                   select new { l, t.NazivTerminala };

                    List<_Statistika> novi = new List<_Statistika>();
                    double? b = 0;

                    foreach (var q in rezultat)
                    {
                        try
                        {
                            if (q.l.Battery == b)
                            {
                                continue;
                            }

                            b = q.l.Battery;
                            novi.Add(new _Statistika(q.NazivTerminala, (int)q.l.Battery, (decimal)q.l.Battery, q.l.DatumVrijeme.ToString("HH:mm:ss"), q.l.DatumVrijeme, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "TRAJANJE BATERIJE");
                return null;
            }
        }

        public static List<_Statistika> GPS(string grad, DateTime datum, int idTerminala, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from l in db.Lokacijes
                                   join t in db.Terminalis on l.IDTerminala equals t.IDTerminala
                                   where l.DatumVrijeme.Date == datum &&
                                         l.IDTerminala == idTerminala
                                   select new { l, t.NazivTerminala };

                    List<_Statistika> novi = new List<_Statistika>();
                    double? b = 0;

                    foreach (var q in rezultat)
                    {
                        try
                        {
                            if (q.l.GPSAcc == b)
                            {
                                continue;
                            }

                            b = q.l.GPSAcc;
                            novi.Add(new _Statistika(q.NazivTerminala, (int)q.l.GPSAcc, (decimal)q.l.GPSAcc, q.l.DatumVrijeme.ToString("HH:mm:ss"), q.l.DatumVrijeme, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "TRAJANJE BATERIJE");
                return null;
            }
        }

        ////todo usporedba na grafu svih djelatnika tjedan/dan/mjesec - GO
        //line
        //public static List<_IzdanihKazni> RedariPrekrsaji(string grad, DateTime datumOd, DateTime datumDo, int idAplikacije)
        //{
        //    try
        //    {
        //        PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije));
        //        db.CommandTimeout = 60;

        //        var izdanih = from x in
        //                          (
        //                              from p in db.Prekrsajis
        //                              join l in db.Lokacijes on p.IDLokacije equals l.IDLokacije
        //                              join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
        //                              join d in db.Djelatniks on l.IDDjelatnika equals d.IDDjelatnika
        //                              where l.RegistracijskaPlocica != "" &&
        //                                    l.DatumVrijeme.Date >= datumOd.Date &&
        //                                    l.DatumVrijeme.Date <= datumDo.Date &&
        //                                    p.Test == false &&
        //                                    p.Status == false &&
        //                                    d.PrikaziStatistika
        //                              select new { p, l, i, d })
        //                      group x by new { x.d.ImePrezime }
        //                          into grupirano
        //                          select new _IzdanihKazni(
        //                              grupirano.Key.ImePrezime,
        //                              grupirano.Count(i => i.i.NazivPredloska == "UPOZORENJE"),
        //                              grupirano.Count(i => i.i.NazivPredloska == "OBAVIJEST")
        //                              );

        //        return izdanih.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "Redari Prekrsaji");
        //        return null;
        //    }
        //}

        //public static List<_UspjesnoOcitan> UspjesnostOcitanja(string grad, DateTime datumOd, DateTime datumDo, out int ocitanih, out int izmjenjenih, out int rucnih, int idAplikacije)
        //{
        //    try
        //    {
        //        PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije));
        //        db.CommandTimeout = 60;

        //        var ocitanja = from x in
        //                           (

        //                               from l in db.Lokacijes
        //                               join p in db.Prekrsajis on l.IDLokacije equals p.IDLokacije
        //                               join d in db.Djelatniks on l.IDDjelatnika equals d.IDDjelatnika
        //                               where l.RegistracijskaPlocica != "" &&
        //                                     l.DatumVrijeme.Date >= datumOd.Date &&
        //                                     l.DatumVrijeme.Date <= datumDo.Date.AddDays(1) &&
        //                                     p.StatusOcitanja != null &&
        //                                     d.PrikaziStatistika &&
        //                                     p.Test == false
        //                               orderby d.ImePrezime
        //                               select new { d.ImePrezime, p.StatusOcitanja, p.IDPredloskaIspisa }).AsEnumerable()
        //                       group x by new { x.ImePrezime }
        //                           into y
        //                           select
        //                               new _UspjesnoOcitan
        //                                   (
        //                                       y.Key.ImePrezime,
        //                                       y.Count(aa => ((byte)aa.StatusOcitanja & 3) == 1),
        //                                       y.Count(aa => ((byte)aa.StatusOcitanja & 3) == 3),
        //                                       y.Count(aa => ((byte)aa.StatusOcitanja & 3) != 1 && (aa.StatusOcitanja & 3) != 3)
        //                                   );

        //        ocitanih = ocitanja.Sum(i => i.Ocitan);
        //        izmjenjenih = ocitanja.Sum(i => i.Izmijenjen);
        //        rucnih = ocitanja.Sum(i => i.Rucni);

        //        return ocitanja.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "Uspjesnost Ocitanja");
        //        ocitanih = 0;
        //        izmjenjenih = 0;
        //        rucnih = 0;
        //        return null;
        //    }
        //}


        //public static List<_Prekrsaj> KomentariPostupanja(string grad, int idAplikacije)
        //{
        //    try
        //    {
        //        PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije));
        //        db.CommandTimeout = 60;

        //        var prek = from p in db.Prekrsajis
        //                   join l in db.Lokacijes on p.IDLokacije equals l.IDLokacije
        //                   join d in db.Djelatniks on l.IDDjelatnika equals d.IDDjelatnika
        //                   join t in db.Terminalis on l.IDTerminala equals t.IDTerminala
        //                   join i in db.PredlosciIspisas on p.IDPredloskaIspisa equals i.IDPRedloska
        //                   join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa
        //                   join r in db.PopisPrekrsajas on o.IDPrekrsaja equals r.IDPrekrsaja
        //                   join n in db.NaloziPaukus on p.IDNaloga equals n.IDNaloga into nalozi
        //                   from n in nalozi.DefaultIfEmpty()
        //                   join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa into statusi
        //                   from s in statusi.DefaultIfEmpty()
        //                   join k in db.KomentariPostupanjas on p.IDPrekrsaja equals k.IDPrekrsaja
        //                   orderby l.DatumVrijeme ascending
        //                   select new _Prekrsaj
        //                              (
        //                                  p.IDPrekrsaja,
        //                                  t.IDTerminala,
        //                                  o.IDOpisa,
        //                                  p.KraticaDrzave != "??" ? p.RegistracijskaPlocica + " (" + p.KraticaDrzave + ")" : p.RegistracijskaPlocica,
        //                                  l.DatumVrijeme,
        //                                  d.ImePrezime,
        //                                  d.UID,
        //                                  p.Adresa,
        //                                  p.BrojUpozorenja,
        //                                  t.NazivTerminala,
        //                                  i.NazivPredloska,
        //                                  o.OpisPrekrsaja,
        //                                  r.MaterijalnaKaznjivaNorma,
        //                                  r.Kazna.ToString(),
        //                       //s.NazivStatusa,
        //                                  p.NalogPauka,
        //                       //p.IDNaloga,
        //                                  p.Status,
        //                                  p.Test,
        //                                  (p.StatusOcitanja & 3) == 3 ? "I" : (p.StatusOcitanja & 3) == 1 ? "O" : "R",
        //                                  new _Koordinate(p.IDLokacije, (int)l.IDDjelatnika, p.Lat, p.Long, l.DatumVrijeme),
        //                                  new _KomentarPostupanja(k.IDKomentara, k.IDPrekrsaja, k.Primjedba, k.Obrazlozenje, k.IspravnoPostupanje),
        //                                  "",
        //                                  p.StatusVPP ?? "",
        //                                  p.KraticaDrzave,
        //                                  null
        //                              );

        //        return prek.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "Komentari Postupanja");
        //        return null;
        //    }
        //}

        /*:: PARKING ::*/

        public static List<_Statistika> ObavljenihOpazanja(string grad, DateTime datumOd, DateTime datumDo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.PARKING_OPAZANJAs
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date
                                   select p;

                    List<_Statistika> novi = new List<_Statistika>();

                    List<_2DLista> statusi = Parking.Statusi(false, idAplikacije);

                    foreach (var q in rezultat.GroupBy(i => i.IDStatusa))
                    {
                        try
                        {
                            if (q.Key == 0)
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            novi.Add(new _Statistika(Parking.Status(q.Key, statusi), q.Count(), posto, q.Count().ToString(), DateTime.Today, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OPAŽANJA");
                return null;
            }
        }

        public static List<_Statistika> IzdaneKazne(string grad, DateTime datumOd, DateTime datumDo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.PARKING_OPAZANJAs
                        where p.Vrijeme.Value.Date >= datumOd.Date &&
                              p.Vrijeme.Value.Date <= datumDo.Date &&
                              p.IDStatusa.HasValue &&
                              (p.IDStatusa != 4 && p.IDStatusa != 5)
                        select p;

                    List<_Statistika> novi = new List<_Statistika>();

                    List<_2DLista> statusi = Parking.Statusi(true, idAplikacije);

                    foreach (var q in rezultat.GroupBy(i => i.IDStatusa))
                    {
                        try
                        {
                            if (q.Key == 0)
                            {
                                continue;
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            novi.Add(new _Statistika(Parking.Status(q.Key, statusi), q.Count(), posto, q.Sum(i => i.Iznos).ToString(), DateTime.Today, null));
                        }
                        catch
                        {
                        }
                    }

                    return novi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OPAŽANJA");
                return null;
            }
        }

        public static List<_Statistika> IzdanePretplate(string grad, DateTime datumOd, DateTime datumDo, int idSubjekta, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 240;
                    db.ObjectTrackingEnabled = false;

                    var rezultat = from p in db.ODOBRENJAs
                                   join s in db.POSLOVNI_SUBJEKTIs on p.IDSubjekta equals s.IDPoslovnogSubjekta
                                   where p.VrijemeUnosa.Value.Date >= datumOd.Date &&
                                         p.VrijemeUnosa.Value.Date <= datumDo.Date &&
                                         (idSubjekta != -1 ? p.IDSubjekta == idSubjekta : idSubjekta == -1) &&
                                         (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1)
                                   //orderby i.IDPRedloska descending
                                   select new { p, s.NazivSubjekta, t = Traje(p.DatumOd, p.DatumDo) };

                    List<_Statistika> novi = new List<_Statistika>();

                    foreach (var q in rezultat.GroupBy(i => new { i.NazivSubjekta }))
                    {
                        try
                        {
                            if (!q.Any())
                            {
                                continue;
                            }

                            List<_Statistika> detalji = new List<_Statistika>();

                            foreach (var dok in q.GroupBy(i => i.t))
                            {
                                decimal posto1 = Math.Round((decimal)dok.Count() / q.Count() * 100, 2);
                                detalji.Add(new _Statistika(dok.Key, dok.Count(), posto1, dok.Key, DateTime.Today, null));
                            }

                            decimal posto = Math.Round((decimal)q.Count() / rezultat.Count() * 100, 2);

                            novi.Add(new _Statistika(q.Key.NazivSubjekta, q.Count(), posto, "", q.First().p.VrijemeUnosa.Value, detalji));
                        }
                        catch
                        {
                        }
                    }

                    return novi.OrderBy(i => i.Datum).ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Upozorenja Obavijesti Mjesecno");
                return null;
            }
        }

        private static string Traje(DateTime? datumOd, DateTime? datumDo)
        {
            try
            {
                if (!datumDo.HasValue)
                {
                    return "NEOGRANIČENO";
                }

                int dana = datumDo.Value.Subtract(datumOd.Value).Days;

                if (dana == 364 || dana == 365 || dana == 366)
                {
                    return "GODIŠNJA";
                }

                if (dana == 182 || dana == 183 || dana == 184)
                {
                    return "POLUGODIŠNJA";
                }

                if (dana == 28 || dana == 29 || dana == 30 || dana == 31)
                {
                    return "MJESEČNA";
                }

                if (dana == 1)
                {
                    return "DNEVNA";
                }

                return dana.ToString();
            }
            catch
            {
                return "??";
            }
        }

        #endregion

        public static List<_EvidencijaPlacanja> EvidencijaPlacanja(string grad, DateTime datum, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    Table<VRSTE_PLACANJA> vp = new PostavkeDataContext().VRSTE_PLACANJAs;

                    var evidencije = from r in db.RACUNIs
                                     join n in db.NaloziPaukus on r.IDReference equals n.IDNaloga
                                     join s in db.RACUNI_STAVKEs on r.IDRacuna equals s.IDRacuna
                                     where r.Datum >= datum.Date.AddHours(6) && //todo od 6
                                           r.Datum <= datum.Date.AddDays(1).AddHours(6) &&//do 6
                                           r.Storniran == false
                                     select new { n.IDStatusa, r.IDVrstePlacanja, s, K = VrstaPlacanja(vp, r.IDVrstePlacanja) };

                    List<_EvidencijaPlacanja> placanja = new List<_EvidencijaPlacanja>();

                    foreach (var r in evidencije.Distinct().GroupBy(i => new { i.IDStatusa }))
                    {
                        if (r.Key.IDStatusa == 4)
                        {
                            List<_EvidencijaPlacanja> detalji = new List<_EvidencijaPlacanja>();

                            foreach (var x in evidencije.Where(i => i.IDStatusa == r.Key.IDStatusa).GroupBy(i => i.IDVrstePlacanja))
                            {
                                foreach (var o in db.RACUNI_STAVKE_OPIs.Where(i => i.IDStatusa == r.Key.IDStatusa))
                                {
                                    int br = r.Count(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key);

                                    if (o.Lezarina)
                                    {
                                        br = (int)r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Kolicina);
                                    }

                                    decimal iznos = r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Ukupno);

                                    if (br == 0)
                                    {
                                        continue;
                                    }

                                    if (detalji.Any(i => i.Opis == string.Format("{0} ({1}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K)))
                                    {
                                        _EvidencijaPlacanja ep = detalji.First(i => i.Opis == string.Format("{0} ({1}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K));
                                        ep.Iznos += iznos;
                                        ep.Broj += br;
                                    }
                                    else
                                    {
                                        detalji.Add(new _EvidencijaPlacanja(1, x.Key, "", x.First().K, string.Format("{0} ({1}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K), br, iznos, null));
                                    }
                                }
                            }

                            placanja.Add(new _EvidencijaPlacanja(1, 4, "", "", "Naplata na odlagalištu", r.Count(), detalji.Sum(i => i.Iznos), detalji));
                        }

                        if (r.Key.IDStatusa == 3)
                        {
                            List<_EvidencijaPlacanja> detalji = new List<_EvidencijaPlacanja>();

                            foreach (var x in evidencije.Where(i => i.IDStatusa == r.Key.IDStatusa).GroupBy(i => i.IDVrstePlacanja))
                            {
                                foreach (var o in db.RACUNI_STAVKE_OPIs.Where(i => i.IDStatusa == r.Key.IDStatusa))
                                {
                                    int br = r.Count(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key);

                                    if (o.Lezarina)
                                    {
                                        br = (int)r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Kolicina);
                                    }

                                    decimal iznos = r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Ukupno);

                                    if (br == 0)
                                    {
                                        continue;
                                    }

                                    if (detalji.Any(i => i.Opis == string.Format("{0} ({1}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K)))
                                    {
                                        _EvidencijaPlacanja ep = detalji.First(i => i.Opis == string.Format("{0} ({1:N}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K));
                                        ep.Iznos += iznos;
                                        ep.Broj += br;
                                    }
                                    else
                                    {
                                        detalji.Add(new _EvidencijaPlacanja(1, x.Key, "", x.First().K, string.Format("{0} ({1:N}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K), br, iznos, null));
                                    }
                                }
                            }

                            placanja.Add(new _EvidencijaPlacanja(2, 3, "", "", "Naplata na terenu", r.Count(), detalji.Sum(i => i.Iznos), detalji));
                        }

                        if (r.Key.IDStatusa == 22)
                        {
                            List<_EvidencijaPlacanja> detalji = new List<_EvidencijaPlacanja>();

                            foreach (var x in evidencije.Where(i => i.IDStatusa == r.Key.IDStatusa).GroupBy(i => i.IDVrstePlacanja))
                            {
                                foreach (var o in db.RACUNI_STAVKE_OPIs.Where(i => i.IDStatusa == r.Key.IDStatusa))
                                {
                                    int br = r.Count(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key);

                                    if (o.Lezarina)
                                    {
                                        br = (int)r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Kolicina);
                                    }

                                    decimal iznos = r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Ukupno);

                                    if (br == 0)
                                    {
                                        continue;
                                    }

                                    if (detalji.Any(i => i.Opis == string.Format("{0} ({1}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K)))
                                    {
                                        _EvidencijaPlacanja ep = detalji.First(i => i.Opis == string.Format("{0} ({1}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K));
                                        ep.Iznos += iznos;
                                        ep.Broj += br;
                                    }
                                    else
                                    {
                                        detalji.Add(new _EvidencijaPlacanja(1, x.Key, "", x.First().K, string.Format("{0} ({1}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K), br, iznos, null));
                                    }
                                }
                            }

                            placanja.Add(new _EvidencijaPlacanja(3, 22, "", "", "Naplata blokiranja", r.Count(), detalji.Sum(i => i.Iznos), detalji));
                        }
                    }

                    if (!placanja.Any(i => i.IDVrstePlacanja == 4))
                    {
                        placanja.Add(new _EvidencijaPlacanja(1, 4, "", "", "Naplata na odlagalištu", 0, 0, new List<_EvidencijaPlacanja>()));
                    }

                    if (!placanja.Any(i => i.IDVrstePlacanja == 3))
                    {
                        placanja.Add(new _EvidencijaPlacanja(2, 3, "", "", "Naplata na terenu", 0, 0, new List<_EvidencijaPlacanja>()));
                    }

                    if (!placanja.Any(i => i.IDVrstePlacanja == 22))
                    {
                        placanja.Add(new _EvidencijaPlacanja(3, 22, "", "", "Naplata blokiranja", 0, 0, new List<_EvidencijaPlacanja>()));
                    }

                    return placanja.OrderBy(i => i.Redoslijed).ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "EVIDENCIJA PLACANJA");
                return new List<_EvidencijaPlacanja>();
            }
        }

        public static List<_EvidencijaPlacanja> EvidencijaPlacanjaUkupno(string grad, DateTime datum, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    Table<VRSTE_PLACANJA> vp = new PostavkeDataContext().VRSTE_PLACANJAs;

                    DateTime datumOd = new DateTime(datum.Year, datum.Month, 1, 6, 0, 0);
                    DateTime datumDo = new DateTime(datum.Month == 12 ? datum.Year + 1 : datum.Year, datum.Month == 12 ? 1 : datum.Month + 1, 1, 6, 0, 0).AddDays(-1);

                    var evidencije = from r in db.RACUNIs
                                     join n in db.NaloziPaukus on r.IDReference equals n.IDNaloga
                                     join s in db.RACUNI_STAVKEs on r.IDRacuna equals s.IDRacuna
                                     where r.Datum >= datumOd && //todo od 6
                                           r.Datum <= datumDo //do 6
                                     orderby r.Datum
                                     select new { n.IDStatusa, r.IDVrstePlacanja, r.Datum, s, K = VrstaPlacanja(vp, r.IDVrstePlacanja) };

                    List<_EvidencijaPlacanja> placanja = new List<_EvidencijaPlacanja>();

                    foreach (var r in evidencije.GroupBy(i => new { i.IDStatusa }))
                    {
                        if (r.Key.IDStatusa == 4)
                        {
                            List<_EvidencijaPlacanja> detalji = new List<_EvidencijaPlacanja>();

                            foreach (var x in evidencije.Where(i => i.IDStatusa == r.Key.IDStatusa).GroupBy(i => i.IDVrstePlacanja))
                            {
                                foreach (var o in db.RACUNI_STAVKE_OPIs.Where(i => i.IDStatusa == r.Key.IDStatusa))
                                {
                                    int br = r.Count(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key);

                                    if (o.Lezarina)
                                    {
                                        br = (int)r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Kolicina);
                                    }

                                    decimal iznos = r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Ukupno);

                                    if (br == 0)
                                    {
                                        continue;
                                    }

                                    if (detalji.Any(i => i.Opis == string.Format("{0} ({1:N}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K)))
                                    {
                                        _EvidencijaPlacanja ep = detalji.First(i => i.Opis == string.Format("{0} ({1:N}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K));
                                        ep.Iznos += iznos;
                                        ep.Broj += br;
                                    }
                                    else
                                    {
                                        detalji.Add(new _EvidencijaPlacanja(1, x.Key, "", x.First().K, string.Format("{0} ({1:N}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K), br, iznos, null));
                                    }
                                }
                            }

                            placanja.Add(new _EvidencijaPlacanja(1, 4, "", "", "Naplata na odlagalištu", r.Count(), detalji.Sum(i => i.Iznos), detalji.OrderBy(i => i.Opis).ToList()));
                        }

                        if (r.Key.IDStatusa == 3)
                        {
                            List<_EvidencijaPlacanja> detalji = new List<_EvidencijaPlacanja>();

                            foreach (var x in evidencije.Where(i => i.IDStatusa == r.Key.IDStatusa).GroupBy(i => i.IDVrstePlacanja))
                            {
                                foreach (var o in db.RACUNI_STAVKE_OPIs.Where(i => i.IDStatusa == r.Key.IDStatusa))
                                {
                                    int br = r.Count(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key);

                                    if (o.Lezarina)
                                    {
                                        br = (int)r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Kolicina);
                                    }

                                    decimal iznos = r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Ukupno);

                                    if (br == 0)
                                    {
                                        continue;
                                    }

                                    if (detalji.Any(i => i.Opis == string.Format("{0} ({1:N}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K)))
                                    {
                                        _EvidencijaPlacanja ep = detalji.First(i => i.Opis == string.Format("{0} ({1:N}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K));
                                        ep.Iznos += iznos;
                                        ep.Broj += br;
                                    }
                                    else
                                    {
                                        detalji.Add(new _EvidencijaPlacanja(1, x.Key, "", x.First().K, string.Format("{0} ({1:N}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K), br, iznos, null));
                                    }
                                }
                            }

                            placanja.Add(new _EvidencijaPlacanja(2, 3, "", "", "Naplata na terenu", r.Count(), detalji.Sum(i => i.Iznos), detalji));
                        }

                        if (r.Key.IDStatusa == 22)
                        {
                            List<_EvidencijaPlacanja> detalji = new List<_EvidencijaPlacanja>();

                            foreach (var x in evidencije.Where(i => i.IDStatusa == r.Key.IDStatusa).GroupBy(i => i.IDVrstePlacanja))
                            {
                                foreach (var o in db.RACUNI_STAVKE_OPIs.Where(i => i.IDStatusa == r.Key.IDStatusa))
                                {
                                    int br = r.Count(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key);

                                    if (o.Lezarina)
                                    {
                                        br = (int)r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Kolicina);
                                    }

                                    decimal iznos = r.Where(i => i.s.IDOpisaStavke == o.IDOpisaStavke && i.IDVrstePlacanja == x.Key).Sum(i => i.s.Ukupno);

                                    if (br == 0)
                                    {
                                        continue;
                                    }

                                    if (detalji.Any(i => i.Opis == string.Format("{0} ({1:N}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K)))
                                    {
                                        _EvidencijaPlacanja ep = detalji.First(i => i.Opis == string.Format("{0} ({1:N}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K));
                                        ep.Iznos += iznos;
                                        ep.Broj += br;
                                    }
                                    else
                                    {
                                        detalji.Add(new _EvidencijaPlacanja(1, x.Key, "", x.First().K, string.Format("{0} ({1:N}) - {2}", o.NazivOpisaStavke, o.Iznos, x.First().K), br, iznos, null));
                                    }
                                }
                            }

                            placanja.Add(new _EvidencijaPlacanja(3, 22, "", "", "Naplata blokiranja", r.Count(), detalji.Sum(i => i.Iznos), detalji));
                        }
                    }

                    if (!placanja.Any(i => i.IDVrstePlacanja == 4))
                    {
                        placanja.Add(new _EvidencijaPlacanja(1, 4, "", "", "Naplata na odlagalištu", 0, 0, new List<_EvidencijaPlacanja>()));
                    }

                    if (!placanja.Any(i => i.IDVrstePlacanja == 3))
                    {
                        placanja.Add(new _EvidencijaPlacanja(2, 3, "", "", "Naplata na terenu", 0, 0, new List<_EvidencijaPlacanja>()));
                    }

                    if (!placanja.Any(i => i.IDVrstePlacanja == 22))
                    {
                        placanja.Add(new _EvidencijaPlacanja(3, 22, "", "", "Naplata blokiranja", 0, 0, new List<_EvidencijaPlacanja>()));
                    }

                    return placanja.OrderBy(i => i.Redoslijed).ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "EVIDENCIJA PLACANJA MJESEC");
                return new List<_EvidencijaPlacanja>();
            }
        }

        public static string VrstaPlacanja(Table<VRSTE_PLACANJA> vp, int idVrste)
        {
            return vp.First(i => i.IDVrstePlacanja == idVrste).Kratica;
        }

        /*:: DNEVNI UTRŽAK ::*/

        public static List<_DnevniUtrzak> BlagajnickiIzvjestaj(string grad, DateTime datum, int idDjelatnika, int idRedarstva, int idAplikacije)
        {
            try
            {
                List<_DnevniUtrzak> novi = new List<_DnevniUtrzak>();

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (!db.BLAGAJNICKI_DNEVNIKs.Any(i => i.Datum.Date == datum.Date && i.IDRedarstva == idRedarstva))
                    {
                        Naplata.BlagajnickiDnevnik(grad, datum, idDjelatnika, idRedarstva, 500, idAplikacije);
                    }

                    BLAGAJNICKI_DNEVNIK bd = db.BLAGAJNICKI_DNEVNIKs.First(i => i.Datum.Date == datum.Date && i.IDRedarstva == idRedarstva);

                    foreach (var b in db.BLAGAJNICKI_DNEVNIK_STAVKEs.Where(i => i.IDDnevnika == bd.IDDnevnika))
                    {
                        string sifra = string.Format("{0}-{1} {2}", b.Tip, b.RB, b.NaplatnoMjesto);
                        novi.Add(new _DnevniUtrzak(bd.IDDnevnika, sifra, b.Opis, b.Primitak, b.Izdatak));
                    }
                }

                return novi;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DNEVNI UTRŽAK");
                return new List<_DnevniUtrzak>();
            }
        }

        public static List<_DnevniUtrzak> BlagajnickiIzvjestajSintetika(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, int idAplikacije)
        {
            try
            {
                List<_DnevniUtrzak> novi = new List<_DnevniUtrzak>();

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {

                    var dnevnik = from b in db.BLAGAJNICKI_DNEVNIKs
                                  join s in db.BLAGAJNICKI_DNEVNIK_STAVKEs on b.IDDnevnika equals s.IDDnevnika
                                  where b.Datum >= datumOd &&
                                        b.Datum <= datumDo
                                  select s;

                    foreach (var b in dnevnik.GroupBy(i => i.Opis))
                    {
                        int redoslijed = 0;
                        if (b.First().Redosljed.HasValue)
                        {
                            redoslijed = b.First().Redosljed.Value;
                        }

                        novi.Add(new _DnevniUtrzak(redoslijed, "", b.Key, b.Sum(i => i.Primitak), b.Sum(i => i.Izdatak)));
                    }
                }

                return novi.OrderBy(i => i.IDDnevnika).ToList();
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DNEVNI UTRŽAK");
                return new List<_DnevniUtrzak>();
            }
        }

        public static DateTime? ZadnjiBlagajnickiIzvjestaj(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.BLAGAJNICKI_DNEVNIKs.Any(i => i.IDRedarstva == idRedarstva))
                    {
                        return db.BLAGAJNICKI_DNEVNIKs.OrderByDescending(i => i.Datum).First(i => i.IDRedarstva == idRedarstva).Datum;
                    }

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /**/

        public static List<_CentralnaLokacija> IntenzitetPostupanja(string grad, DateTime datumOd, DateTime datumDo, int idRedarstva, bool nalog,
            bool zahtjevi, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    db.CommandTimeout = 60;
                    db.ObjectTrackingEnabled = false;

                    if (zahtjevi)
                    {
                        var zah = from p in db.Zahtjevis
                                  where p.DatumVrijeme.Date >= datumOd.Date &&
                                        p.DatumVrijeme.Date <= datumDo.Date &&
                                        (idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1)
                                  select new _CentralnaLokacija(p.IDLokacije, p.Lat, p.Lng);

                        return zah.ToList();
                    }

                    var rezultat = from p in db.Prekrsajis
                                   where p.Vrijeme.Value.Date >= datumOd.Date &&
                                         p.Vrijeme.Value.Date <= datumDo.Date &&
                                         (nalog ? p.NalogPauka == true : nalog == false) &&
                                         p.Test == false &&
                                         p.Status == false
                                   select new _CentralnaLokacija(p.IDLokacije, p.Lat, p.Long);

                    return rezultat.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "INTENZITET POSTUPANJA");
                return null;
            }
        }
    }
}