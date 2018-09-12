using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Predlosci
    {
        /*:: PREDLOŠCI ::*/

        #region PREDLOŠCI

        public static List<_2DLista> JeziciPredlozaka(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var jez = from p in db.JEZICIs
                              select new _2DLista(p.IDJezika, p.NazivJezika);

                    return jez.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Jezici Predlozaka");
                return new List<_2DLista>();
            }

        }

        public static List<_2DLista> PopisPredlozaka(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List<_2DLista> nova = new List<_2DLista>();
                    nova.Add(new _2DLista(0, "Svi dokumenti"));

                    var popis = from p in db.PredlosciIspisas
                                where p.IDRedarstva == idRedarstva
                                select new _2DLista(p.IDPRedloska, p.NazivPredloska);

                    nova.AddRange(popis.Select(q => new _2DLista(q.Value, q.Text)));

                    return nova.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POPIS Predlozaka");
                return new List<_2DLista>();
            }
        }

        public static List<_Predlozak> PredlosciIspisaLight(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var pred = from p in db.PredlosciIspisas
                        where idRedarstva != -1 ? p.IDRedarstva == idRedarstva : idRedarstva == -1
                        select new _Predlozak(p.IDPRedloska, p.IDRedarstva, p.NazivPredloska, p.Pauk, p.Kaznjava, null, new List<_TekstPredloska>());

                    return pred.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Predlosci Ispisa");
                return new List<_Predlozak>();
            }
        }

        public static List<_Predlozak> PredlosciIspisa(string grad, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var pred = from p in db.PredlosciIspisas
                               where idRedarstva != 0 ? p.IDRedarstva == idRedarstva : idRedarstva == 0
                               select new _Predlozak(p.IDPRedloska, p.IDRedarstva, p.NazivPredloska, p.Pauk, p.Kaznjava, p.Predlozak,
                                   (from pp in db.PrevedeniPredloscis
                                    join j in db.Jezicis on pp.IDJezika equals j.IDJezika
                                    where pp.IDPredloska == p.IDPRedloska
                                    select new _TekstPredloska(pp.IDPredloska, (int)pp.IDJezika, j.Nazivjezika, pp.Predlozak)).ToList()
                                   );

                    return pred.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Predlosci Ispisa");
                return new List<_Predlozak>();
            }
        }

        internal static _Predlozak DohvatiPredlozakIspisa(string grad, int idPredloska, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var pred = from p in db.PredlosciIspisas
                        where p.IDPRedloska == idPredloska
                        select new _Predlozak(p.IDPRedloska, p.IDRedarstva, p.NazivPredloska, p.Pauk, p.Kaznjava, p.Predlozak,
                            (from pp in db.PrevedeniPredloscis
                                join j in db.Jezicis on pp.IDJezika equals j.IDJezika
                                where pp.IDPredloska == p.IDPRedloska
                                select new _TekstPredloska(pp.IDPredloska, (int)pp.IDJezika, j.Nazivjezika, pp.Predlozak)).ToList()
                        );

                    return pred.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Predlosci Ispisa");
                return null;
            }
        }

        public static _Predlozak DohvatiPredlozakIspisa(string grad, string koji, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var pred = from p in db.PredlosciIspisas
                               where p.NazivPredloska == koji
                               select new _Predlozak(p.IDPRedloska, p.IDRedarstva, p.NazivPredloska, p.Pauk, p.Kaznjava, p.Predlozak,
                                   (from pp in db.PrevedeniPredloscis
                                    join j in db.Jezicis on pp.IDJezika equals j.IDJezika
                                    where pp.IDPredloska == p.IDPRedloska
                                    select new _TekstPredloska(pp.IDPredloska, (int)pp.IDJezika, j.Nazivjezika, pp.Predlozak)).ToList()
                                   );

                    return pred.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Predlosci Ispisa");
                return null;
            }
        }

        public static bool ObrisiPredlozak(string grad, int idPredloska, int idJezika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (idJezika == 0)
                    {
                        db.PrevedeniPredloscis.DeleteAllOnSubmit(db.PrevedeniPredloscis.Where(p => p.IDPredloska == idPredloska));
                        db.PredlosciIspisas.DeleteOnSubmit(db.PredlosciIspisas.First(p => p.IDPRedloska == idPredloska));
                        db.SubmitChanges();
                    }
                    else
                    {
                        db.PrevedeniPredloscis.DeleteOnSubmit(db.PrevedeniPredloscis.First(p => p.IDJezika == idJezika && p.IDPredloska == idPredloska));
                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Obrisi Predlozak");
                return false;
            }
        }

        public static bool IzmijeniPredlozak(string grad, int idPredloska, int idJezika, string nazivPredloska, bool pauk, bool kaznjava, XElement tekstPredloska, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (idJezika == 0)
                    {
                        PredlosciIspisa pi = db.PredlosciIspisas.First(i => i.IDPRedloska == idPredloska);

                        pi.NazivPredloska = nazivPredloska;
                        pi.Predlozak = tekstPredloska;
                        pi.Pauk = pauk;
                        pi.Kaznjava = kaznjava;

                        db.SubmitChanges();
                    }
                    else
                    {
                        if (db.PrevedeniPredloscis.Any(i => i.IDPredloska == idPredloska && i.IDJezika == idJezika))
                        {
                            db.PrevedeniPredloscis.First(i => i.IDPredloska == idPredloska && i.IDJezika == idJezika).Predlozak = tekstPredloska;
                            db.SubmitChanges();
                        }
                        else
                        {
                            PrevedeniPredlosci pi = new PrevedeniPredlosci();

                            pi.IDPredloska = idPredloska;
                            pi.Predlozak = tekstPredloska;
                            pi.IDJezika = idJezika;

                            db.PrevedeniPredloscis.InsertOnSubmit(pi);
                            db.SubmitChanges();
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Izmijeni Predlozak");
                return false;
            }
        }

        public static int DodajPredlozak(string grad, string nazivPredloska, bool pauk, bool kaznjava, XElement tekstPredloska, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PredlosciIspisa pi = new PredlosciIspisa();

                    pi.NazivPredloska = nazivPredloska;
                    pi.Predlozak = tekstPredloska;
                    pi.IDRedarstva = idRedarstva;
                    pi.Pauk = pauk;
                    pi.Kaznjava = kaznjava;

                    db.PredlosciIspisas.InsertOnSubmit(pi);
                    db.SubmitChanges();

                    return pi.IDPRedloska;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dodaj Predlozak");
                return -1;
            }
        }

        public static bool? Obavijest(string grad, int idPredloska, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var pr = from p in db.PredlosciIspisas
                        where p.IDPRedloska == idPredloska
                        select p;

                    return pr.First().Kaznjava;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VRSTA PREDLOSKA");
                return null;
            }
        }

        #endregion
    }
}