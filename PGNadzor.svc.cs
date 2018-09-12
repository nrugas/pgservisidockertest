using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi
{
    public class PGNadzor : IPGNadzor
    {
        private const int idAplikacije = 10;

        public bool Dostupan()
        {
            return true;
        }

        public void SpremiError(string grad, Exception greska, string napomena, string korisnik)
        {
            Sustav.SpremiGresku(grad, greska, idAplikacije, napomena, korisnik);
        }

        public List<_2DLista> PopisGradova()
        {
            return Gradovi.PopisGradova(0, idAplikacije);
        }

        public List<_2DLista> Redarstva()
        {
            return Postavke.Redarstva(idAplikacije);
        }

        public List<_Iznos> Iznosi()
        {
            return Gradovi.Iznosi(idAplikacije);
        }

        public _Uplatnica Uplatnica(string grad, int idGrada, int idRedarstva)
        {
            return Gradovi.Uplatnica(grad, idGrada, idRedarstva, idAplikacije);
        }

        public decimal IznosNaUplatnici(int idGrada)
        {
            return Gradovi.IznosNaUplatnici(idGrada, idAplikacije);
        }

        public List<_Iznos> IznosPokusaja(int idGrada)
        {
            return Gradovi.IznosPokusaja(idGrada, idAplikacije);
        }

        public List<_Projekt> DohvatiProjekte()
        {
            List<_Projekt> projekti = new List<_Projekt>();

            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    foreach (var q in db.GRADOVIs.Where(i => i.Aktivan == true))
                    {
                        bool? status = null;
                        string postupanja = "Upozorenja: 0 <br/>Obavijesti: 0 <br>Naloga: 0";
                        DateTime? zadnje = null;

                        if (q.Aktivan)
                        {
                            PazigradDataContext pg = new PazigradDataContext(Sistem.ConnectionString(q.Baza, idAplikacije));

                            try
                            {
                                if (!pg.Prekrsajis.OrderByDescending(i => i.Vrijeme.Value).Any())
                                {
                                    postupanja = "Upozorenja: 0 <br/>Obavijesti: 0 <br>Naloga: 0";
                                    zadnje = null;
                                    projekti.Add(new _Projekt(q.IDGrada, q.NazivGrada, q.Latitude, q.Longitude, q.Baza, "PG", status, postupanja, zadnje));
                                    continue;
                                }

                                status = pg.Prekrsajis.OrderByDescending(i => i.Vrijeme.Value).First().Vrijeme.Value > DateTime.Now.AddMinutes(-15);

                                var pos = from p in pg.Prekrsajis
                                          where p.Vrijeme.Value.Date == DateTime.Today.Date
                                          select p;

                                postupanja = string.Format("Upozorenja: {0}<br/>Obavijesti: {1} <br>Naloga: {2}",
                                    pos.Count(i => i.IDPredloskaIspisa == 1 || i.IDPredloskaIspisa == 14),
                                    pos.Count(i => i.IDPredloskaIspisa == 2 || i.IDPredloskaIspisa == 15),
                                    pos.Count(i => i.NalogPauka == true));

                                zadnje = pg.Prekrsajis.OrderByDescending(i => i.Vrijeme.Value).First(i => i.RegistracijskaPlocica != "").Vrijeme.Value;
                            }
                            catch (Exception)
                            {
                                postupanja = "Upozorenja: 0 <br/>Obavijesti: 0 <br>Naloga: 0";
                                zadnje = null;
                            }
                        }

                        projekti.Add(new _Projekt(q.IDGrada, q.NazivGrada, q.Latitude, q.Longitude, q.Baza, "PG", status, postupanja, zadnje));
                    }

                    using (GODataContext go = new GODataContext())
                    {
                        foreach (var q in go.GRADOVI1s.Where(i => i.Aktivan))
                        {
                            bool? status = null;
                            string postupanja = "Predmeta: 0";
                            DateTime? zadnje = null;

                            if (q.Aktivan)
                            {
                                try
                                {
                                    GO_GRADDataContext pg = new GO_GRADDataContext(Sistem.ConnectionStringGO(q.Baza, idAplikacije));

                                    status = pg.PREDMETIs.Any(i => i.DatumPrijave.Date == DateTime.Today.Date);

                                    postupanja = "Predmeta: " + pg.PREDMETIs.Count(i => i.DatumPrijave.Date == DateTime.Today.Date);

                                    if (pg.PREDMETIs.OrderByDescending(i => i.DatumPrijave).Any())
                                    {
                                        zadnje = pg.PREDMETIs.OrderByDescending(i => i.DatumPrijave).First().DatumPrijave;
                                    }
                                }
                                catch (Exception)
                                {
                                    postupanja = "Predmeta: 0";
                                    zadnje = null;
                                }
                            }

                            projekti.Add(new _Projekt(q.IDGrada, q.NazivGrada, q.Lattitude, q.Longitude, q.Baza, "GO", status, postupanja, zadnje));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Dohvati Projekte");
            }

            return projekti.OrderBy(i => i.NazivGrada).ToList();
        }

        /*:: POVIJEST ::*/

        public int ZadnjiNalog(int idGrada, int? idIznosa, int idRedarstva)
        {
            return Gradovi.ZadnjiNalog(idGrada, idIznosa, idRedarstva, idAplikacije);
        }

        public List<_Povijest> PosvijestIspisaNaloga(int idGrada)
        {
            return Gradovi.PosvijestIspisaNaloga(idGrada, idAplikacije);
        }

        public bool SpremiPovijestIspisa(_Povijest povijest)
        {
            return Gradovi.SpremiPovijestIspisa(povijest, idAplikacije);
        }

        public bool? ProvijeriGodinu()
        {
            return Gradovi.ProvijeriGodinu(idAplikacije);
        }
    }
}
