using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Chat
    {
        /*:: PORUKE ::*/

        #region PORUKE

        public static List<_Poruka> DohvatiPoruke(string grad, int idDjelatnika, bool pauk, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var poruke = from p in db.Porukes
                                 join pos in db.Djelatniks on p.IDPosiljatelja equals pos.IDDjelatnika
                                 where (p.IDPosiljatelja == idDjelatnika || p.IDPrimatelja == idDjelatnika) &&
                                       p.Pauk == pauk
                                 orderby p.DatumVrijeme descending
                                 select new _Poruka
                                     (
                                     p.IDPoruke,
                                     p.IDPrimatelja,
                                     p.IDPosiljatelja,
                                     p.IDLokacije,
                                     "", //prim.ImePrezime,
                                     pos.ImePrezime,
                                     p.DatumVrijeme,
                                     p.TekstPoruke,
                                     p.Procitano,
                                     p.Odlazna,
                                     p.Vazno,
                                     p.IDLokacije != null
                                     );

                    return poruke.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati Poruke");
                return new List<_Poruka>();
            }
        }

        public static bool PromijeniStatusPoruke(string grad, int idPrimatelja, int idPosiljatelja, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    foreach (Poruke poruka in db.Porukes.Where(i => i.IDPosiljatelja == idPosiljatelja && i.IDPrimatelja == idPrimatelja))
                    {
                        poruka.Procitano = true;
                        db.SubmitChanges();
                    }

                    return true;

                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Promijeni Status poruke");
                return false;
            }
        }

        public static bool ImaNeprocitanihPoruka(string grad, int idPrimatelja, out bool vazno, out int brojPoruka, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var por = from p in db.Porukes
                              where p.IDPrimatelja == idPrimatelja &&
                                    !p.Procitano && !p.ObrisanoPrimljena && !p.Pauk
                              select p;

                    vazno = por.Any(i => i.Vazno);
                    brojPoruka = por.Count();

                    return por.Count() != 0;
                }

            }
            catch
            {
                vazno = false;
                brojPoruka = 0;
                return false;
            }
        }

        public static int PosaljiPoruku(string grad, _Poruka poruka, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (poruka.IDPrimatelja == 0)
                    {
                        foreach (var djel in db.Djelatniks.Where(i => i.Blokiran == false && i.PrometniRedar && i.IDRedarstva == idRedarstva))
                        {
                            if (djel.IDDjelatnika == poruka.IDPosiljatelja)
                            {
                                continue;
                            }

                            Poruke poru = new Poruke();

                            poru.IDPrimatelja = djel.IDDjelatnika;
                            poru.IDPosiljatelja = poruka.IDPosiljatelja;
                            poru.DatumVrijeme = DateTime.Now;
                            poru.TekstPoruke = poruka.Poruka;
                            poru.Vazno = poruka.Vazno;
                            poru.Odlazna = true;
                            poru.IDLokacije = poruka.IDLokacije;

                            db.Porukes.InsertOnSubmit(poru);
                            db.SubmitChanges();
                        }

                        return 0;
                    }

                    Poruke por = new Poruke();

                    por.IDPrimatelja = poruka.IDPrimatelja;
                    por.IDPosiljatelja = poruka.IDPosiljatelja;
                    por.DatumVrijeme = DateTime.Now;
                    por.TekstPoruke = poruka.Poruka;
                    por.Vazno = poruka.Vazno;
                    por.Odlazna = true;
                    por.IDLokacije = poruka.IDLokacije;

                    db.Porukes.InsertOnSubmit(por);
                    db.SubmitChanges();

                    return por.IDPoruke;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Posalji Poruku");
                return -1;
            }
        }

        public static bool ObrisiPoruku(string grad, int IDPoruku, bool Primljena, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Poruke por = db.Porukes.Single(i => i.IDPoruke == IDPoruku);

                    if (Primljena)
                    {
                        por.ObrisanoPrimljena = true;
                    }
                    else
                    {
                        por.ObrisanoPoslana = true;
                    }

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Obrisi Poruku");
                return false;
            }

        }

        public static int Neprocitanih(string grad, int idPrimatelja, int idPosiljatelja, out bool aktivan, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    aktivan = Sustav.Aktivan(grad, idPosiljatelja, idAplikacije);
                    return db.Porukes.Count(i => i.IDPosiljatelja == idPosiljatelja && i.IDPrimatelja == idPrimatelja && i.Procitano == false);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Promijeni Status poruke");
                aktivan = false;
                return -1;
            }
        }

        //drasko
        public static _Poruka DohvatiPoruku(string grad, int idDjelatnika, out int neprocitanih, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var poruke = from p in db.Porukes
                                 join pos in db.Djelatniks on p.IDPosiljatelja equals pos.IDDjelatnika
                                 where p.IDPrimatelja == idDjelatnika &&
                                       p.Procitano == false
                                 orderby p.DatumVrijeme descending
                                 select new _Poruka
                                     (
                                     p.IDPoruke,
                                     p.IDPrimatelja,
                                     p.IDPosiljatelja,
                                     p.IDLokacije,
                                     "", //prim.ImePrezime,
                                     pos.ImePrezime,
                                     p.DatumVrijeme,
                                     p.TekstPoruke,
                                     p.Procitano,
                                     p.Odlazna,
                                     p.Vazno,
                                     p.IDLokacije != null
                                     );

                    if (!poruke.Any())
                    {
                        neprocitanih = 0;
                        return null;
                    }

                    neprocitanih = poruke.Count();
                    return poruke.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati Poruku");
                neprocitanih = 0;
                return null;
            }
        }

        public static bool ProcitaoPoruku(string grad, int idDjelatnika, int idPoruke, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Poruke poruka = db.Porukes.First(i => i.IDPrimatelja == idDjelatnika && i.IDPoruke == idPoruke);
                    poruka.Procitano = true;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Promijeni Status poruke");
                return false;
            }
        }

        #endregion
    }
}