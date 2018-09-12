using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Posade
    {
        public static bool Ukloni(string grad, _Posada posada, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (!db.Posadas.Any(i => i.DatumVrijeme.Date == posada.DatumVrijeme.Date && i.IDVozila == posada.IDVozila && i.JutarnjaSmjena.Value == posada.JutarnjaSmjena))
                    {
                        return false;
                    }

                    Posada p = db.Posadas.First(i => i.DatumVrijeme.Date == posada.DatumVrijeme.Date && i.IDVozila == posada.IDVozila && i.JutarnjaSmjena.Value == posada.JutarnjaSmjena);

                    p.IDDjelatnika1 = posada.IDDjelatnika1;
                    p.IDDjelatnika2 = posada.IDDjelatnika2;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "UKLONI POSADU");
                return false;
            }
        }

        public static bool Dodaj(string grad, _Posada posada, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.Posadas.Any(i => i.DatumVrijeme.Date == posada.DatumVrijeme.Date && i.IDVozila == posada.IDVozila && i.JutarnjaSmjena.Value == posada.JutarnjaSmjena))
                    {
                        Posada p = db.Posadas.First(i => i.DatumVrijeme.Date == posada.DatumVrijeme.Date && i.IDVozila == posada.IDVozila && i.JutarnjaSmjena.Value == posada.JutarnjaSmjena);

                        p.IDDjelatnika1 = posada.IDDjelatnika1;
                        p.IDDjelatnika2 = posada.IDDjelatnika2;

                        db.SubmitChanges();

                        return true;
                    }

                    Posada po = new Posada();

                    int id = 1;

                    if (db.Posadas.Any())
                    {
                        id = db.Posadas.Max(i => i.IDPosade) + 1;
                    }

                    po.IDPosade = id;
                    po.DatumVrijeme = posada.DatumVrijeme;
                    po.IDVozila = posada.IDVozila;
                    po.IDDjelatnika1 = posada.IDDjelatnika1;
                    po.IDDjelatnika2 = posada.IDDjelatnika2;
                    po.IDTerminala = posada.IDTerminala;
                    po.JutarnjaSmjena = posada.JutarnjaSmjena;

                    db.Posadas.InsertOnSubmit(po);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ POSADU");
                return false;
            }
        }

        public static List<_Posada> DohvatiPosadu(string grad, DateTime datum, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var pos = from p in db.Posadas
                              where p.DatumVrijeme == datum
                              select new _Posada(
                                  p.IDPosade,
                                  p.DatumVrijeme,
                                  p.IDVozila,
                                  p.IDDjelatnika1,
                                  p.IDDjelatnika2,
                                  p.IDTerminala,
                                  p.JutarnjaSmjena
                                  );

                    return pos.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI POSADU");
                return new List<_Posada>();
            }
        }
    }
}