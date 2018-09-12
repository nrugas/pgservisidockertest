using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Vozila
    {
        #region VOZILA

        public static List<_2DLista> PopisVozila(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var voz = from p in db.VozilaPaukas
                              where p.Obrisan == false
                              orderby p.NazivVozila ascending
                              select new _2DLista(p.IDVozila, p.NazivVozila);

                    return voz.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POPIS VOZILA");
                return new List<_2DLista>();
            }
        }

        public static List<_Vozilo> VozilaPauka(string grad, bool obrisana, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var voz = from p in db.VozilaPaukas
                              join t in db.Terminalis on p.IDTerminala equals t.IDTerminala into term
                              from tt in term.DefaultIfEmpty()
                              where obrisana == false ? p.Obrisan == false : obrisana
                              orderby p.NazivVozila ascending
                              select new _Vozilo(p.IDVozila, p.NazivVozila, tt.IDTerminala, tt.NazivTerminala, p.Registracija, p.Kontakt, p.Napomena, p.AP, p.Oznaka, p.OznakaPP, p.ObradjujeNalog, p.Lisice, p.Obrisan);

                    return voz.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VOZILA");
                return new List<_Vozilo>();
            }
        }

        public static _Vozilo Vozilo(string grad, int idVozila, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var voz = from p in db.VozilaPaukas
                              join t in db.Terminalis on p.IDTerminala equals t.IDTerminala into term
                              from tt in term.DefaultIfEmpty()
                              where p.IDVozila == idVozila
                              select new _Vozilo(p.IDVozila, p.NazivVozila, tt.IDTerminala, tt.NazivTerminala, p.Registracija, p.Kontakt, p.Napomena, p.AP, p.Oznaka, p.OznakaPP, p.ObradjujeNalog, p.Lisice, p.Obrisan);

                    if (!voz.Any())
                    {
                        return null;
                    }

                    return voz.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "VOZILA");
                return null;
            }
        }

        //public static List<_2DLista> Terminali(string grad, int idAplikacije)
        //{
        //    try
        //    {
        //        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
        //        {
        //            List<_2DLista> ter = new List<_2DLista>();

        //            ter.Add(new _2DLista(0, "Nije dodijeljen!"));

        //            //todo - iz tablice terminali u novom pazigradu
        //            var voz = from p in db.Terminalis
        //                      where p.Pauk == true
        //                      orderby p.NazivTerminala ascending
        //                      select p;

        //            foreach (var q in voz)
        //            {
        //                ter.Add(new _2DLista(q.IDTerminala, q.NazivTerminala));
        //            }

        //            return ter;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "TERMINALI PAUKA");
        //        return new List<_2DLista>();
        //    }
        //}

        public static bool ObrisiVozilo(string grad, int idVozila, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    //db.VozilaCentralnelokacijes.DeleteOnSubmit(db.VozilaCentralnelokacijes.First(i => i.IDVozila == idVozila));
                    //db.VozilaPaukas.DeleteOnSubmit(db.VozilaPaukas.First(i => i.IDVozila == idVozila));

                    VozilaPauka vp = db.VozilaPaukas.First(i => i.IDVozila == idVozila);
                    vp.Obrisan = true;
                    vp.IDTerminala = null;
                    vp.AP = false;
                    vp.Lisice = false;
                    vp.ObradjujeNalog = false;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "OBRISI VOZILO");
                return false;
            }
        }

        public static bool IzmijeniVozilo(string grad, _Vozilo vozilo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VozilaPauka vp = db.VozilaPaukas.First(i => i.IDVozila == vozilo.IDVozila);

                    vp.NazivVozila = vozilo.NazivVozila;
                    vp.IDTerminala = vozilo.IDTerminala == 0 ? null : vozilo.IDTerminala;
                    vp.Registracija = vozilo.Registracija;
                    vp.Kontakt = vozilo.Kontakt;
                    vp.Napomena = vozilo.Napomena;
                    vp.Oznaka = vozilo.Oznaka;
                    vp.OznakaPP = vozilo.OznakaPP;
                    vp.AP = vozilo.Autopilot;
                    vp.ObradjujeNalog = vozilo.Obradjuje;
                    vp.Lisice = vozilo.Blokira;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "TERMINALI PAUKA");
                return false;
            }
        }

        public static int DodajVozilo(string grad, _Vozilo vozilo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VozilaPauka vp = new VozilaPauka();

                    int id = 1;

                    if (db.VozilaPaukas.Any())
                    {
                        id = db.VozilaPaukas.Max(i => i.IDVozila) + 1;
                    }

                    vp.IDVozila = id;
                    vp.NazivVozila = vozilo.NazivVozila;
                    vp.IDTerminala = vozilo.IDTerminala == 0 ? null : vozilo.IDTerminala;
                    vp.Registracija = vozilo.Registracija;
                    vp.Kontakt = vozilo.Kontakt;
                    vp.Napomena = vozilo.Napomena;
                    vp.Oznaka = vozilo.Oznaka;
                    vp.OznakaPP = vozilo.OznakaPP;
                    vp.AP = vozilo.IDTerminala != 0;
                    vp.ObradjujeNalog = vozilo.Obradjuje;
                    vp.Lisice = vozilo.Blokira;
                    vp.Obrisan = false;

                    db.VozilaPaukas.InsertOnSubmit(vp);
                    db.SubmitChanges();

                    VozilaCentralnelokacije vcl = new VozilaCentralnelokacije();

                    vcl.IDVozila = id;
                    vcl.IDCentralneLokacije = 2;

                    db.VozilaCentralnelokacijes.InsertOnSubmit(vcl);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ VOZILO");
                return -1;
            }
        }

        public static bool AktivirajVozilo(string grad, int idVozila, bool aktivno, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VozilaPauka vp = db.VozilaPaukas.First(i => i.IDVozila == idVozila);

                    vp.AP = aktivno;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "AKTIVNO VOZILO");
                return false;
            }
        }

        public static bool VoziloObradjujeNaloge(string grad, int idVozila, bool obradjuje, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VozilaPauka vp = db.VozilaPaukas.First(i => i.IDVozila == idVozila);

                    vp.ObradjujeNalog = obradjuje;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "AKTIVNO VOZILO - OBRAĐUJE NALOGE");
                return false;
            }
        }

        public static bool VoziloObradjujeLisice(string grad, int idVozila, bool obradjuje, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VozilaPauka vp = db.VozilaPaukas.First(i => i.IDVozila == idVozila);

                    vp.Lisice = obradjuje;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "AKTIVNO VOZILO - OBRAĐUJE LISICE");
                return false;
            }
        }

        #endregion
    }
}