using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs.email;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Net
    {
        //public static List<_Narudzba> DohvatiNarudzbe(string grad, int idAplikacije)
        //{
        //    try
        //    {
        //        PostavkeDataContext db = new PostavkeDataContext();

        //        var nar = from n in db.NARUDZBEs
        //                  where n.IDGrada == db.GRADOVIs.First(i => i.NazivGrada == grad).IDGrada
        //                  orderby n.Datum
        //                  select new _Narudzba(n.IDNarudzbe, n.IDGrada, n.IDDjelatnika, n.Datum, n.Datum.ToShortDateString(), n.Datum.ToShortTimeString(), 
        //                      n.Datum.Year, n.Tristo, n.Petsto, n.Sedamsto, n.Etuia, n.Traka, n.Napomena, 
        //                      n.Rijeseno ? "resources/images/ikone/accept.png" : "resources/images/ikone/cross.png", n.Rijeseno);

        //        return nar.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "Prijava");
        //        return new List<_Narudzba>();
        //    }
        //}

        //public static bool NovaNarudzba(string grad, _Narudzba narudzba, int idAplikacije)
        //{
        //    try
        //    {
        //        PostavkeDataContext db = new PostavkeDataContext();

        //        NARUDZBE n = new NARUDZBE();

        //        n.IDGrada = db.GRADOVIs.First(i => i.NazivGrada == grad).IDGrada;
        //        n.IDDjelatnika = narudzba.IDKorisnika;
        //        n.Datum = narudzba.DatumVrijeme;
        //        n.Tristo = narudzba.Tristo;
        //        n.Petsto = narudzba.Petsto;
        //        n.Sedamsto = narudzba.Sedamsto;
        //        n.Etuia = narudzba.Etuia;
        //        n.Traka = narudzba.Traka;
        //        n.Napomena = narudzba.Napomena;
        //        n.Rijeseno = false;

        //        db.NARUDZBEs.InsertOnSubmit(n);
        //        db.SubmitChanges();

        //        _Djelatnik kor = Korisnici.DohvatiDjelatnika(grad, narudzba.IDKorisnika, idAplikacije);

        //        string korisnik = string.Format("{0}", kor.ImePrezime);

        //        string poruka = Pripremi.PopulateBodyNarudzba(grad, korisnik, n.IDNarudzbe, "PAZIGRAD.NET", narudzba.Tristo.ToString(),
        //            narudzba.Petsto.ToString(), narudzba.Sedamsto.ToString(), narudzba.Etuia.ToString(), narudzba.Traka.ToString(), "", narudzba.Napomena, "");

        //        //todo Posalji.Email(grad, poruka, "Narudžba materijala", , null, true, idAplikacije);
                
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "Prijava");
        //        return false;
        //    }
        //}
    }
}