using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Zakoni
    {
        /*:: ZAKONI ::*/

        //stari
        public static _Zakon DohvatiZakonS(string grad, int idOpisa, bool obrisan, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List<_Opis> opisi = (from o in db.OpisiPrekrsajas
                                         where o.IDOpisa == idOpisa &&
                                           (!obrisan ? o.Obrisan == false : obrisan)
                                         orderby o.KratkiOpis
                                         select new _Opis(o.IDOpisa, o.IDPrekrsaja, o.OpisPrekrsaja, o.KratkiOpis, o.ClanakPauka, o.Obrisan,
                                             DohvatiPrijevodeS(grad, o.IDOpisa, false, idAplikacije))).ToList();

                    var zak = from p in db.PopisPrekrsajas
                              where p.IDPrekrsaja == opisi.First().IDZakona //&&
                              //p.IDRedarstva == idRedarstva
                              orderby p.SkraceniOpis
                              select new _Zakon(p.IDPrekrsaja, p.IDRedarstva, p.SkraceniOpis, p.Kazna, p.MaterijalnaKaznjivaNorma, p.Neaktivan,
                              opisi, DohvatiPrijevodeS(grad, p.IDPrekrsaja, true, idAplikacije));

                    return zak.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati Zakone stari");
                return null;
            }
        }

        public static List<_Zakon> DohvatiZakoneS(string grad, bool neaktivni, int idredarstva, bool obrisan, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var zak = from p in db.PopisPrekrsajas
                              where (!neaktivni ? p.Neaktivan == neaktivni : neaktivni) &&
                                    (idredarstva != -1 ? p.IDRedarstva == idredarstva : idredarstva == -1)
                              orderby p.SkraceniOpis
                              select new _Zakon(p.IDPrekrsaja, p.IDRedarstva, p.SkraceniOpis, p.Kazna, p.MaterijalnaKaznjivaNorma, p.Neaktivan,
                              DohvatiOpiseZakonaS(grad, p.IDPrekrsaja, obrisan, idAplikacije),
                              DohvatiPrijevodeS(grad, p.IDPrekrsaja, true, idAplikacije));

                    if (zak.Any())
                    {
                        return zak.ToList();
                    }

                    return new List<_Zakon>();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati Zakone stari");
                return new List<_Zakon>();
            }
        }

        public static List<_Opis> DohvatiOpiseZakonaS(string grad, int idZakona, bool obrisan, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var zak = from o in db.OpisiPrekrsajas
                              where o.IDPrekrsaja == idZakona &&
                                    (!obrisan ? o.Obrisan == false : obrisan)
                              orderby o.KratkiOpis
                              select new _Opis(o.IDOpisa, idZakona, o.OpisPrekrsaja, o.KratkiOpis, o.ClanakPauka, o.Obrisan,
                                  DohvatiPrijevodeS(grad, o.IDOpisa, false, idAplikacije));

                    return zak.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI OPISE ZAKONA");
                return new List<_Opis>();
            }
        }

        public static List<_3DLista> DohvatiOpise(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var zak = from o in db.OpisiPrekrsajas
                              orderby o.KratkiOpis
                              select new _3DLista(o.IDOpisa, o.OpisPrekrsaja, o.KratkiOpis);

                    return zak.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI OPISE ZAKONA");
                return new List<_3DLista>();
            }
        }

        private static List<_Prijevod> DohvatiPrijevodeS(string grad, int idZakona, bool zakon, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (zakon)
                    {
                        var pri = from o in db.PopisPrekrsajaPrijevodis
                                  where o.IDPrekrsaja == idZakona
                                  select new _Prijevod(o.IDPrijevodaPopisa, o.IDPrekrsaja, o.IDJezika, o.PrijevodClanka);

                        return pri.ToList();
                    }

                    var pri1 = from o in db.OpisiPrekrsajaPrijevodis
                               where o.IDOpisa == idZakona
                               select new _Prijevod(o.IDPrijevoda, o.IDOpisa, o.IDJezika, o.Prijevod);

                    return pri1.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI PRIJEVODE ZAKONA");
                return new List<_Prijevod>();
            }
        }

        //novi
        public static List<_Zakon> DohvatiZakone(string grad, bool neaktivni, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var zak = from p in db.ZAKONIs
                              where !neaktivni ? p.Neaktivan == neaktivni : neaktivni
                              select new _Zakon(p.IDZakona, 1, p.SkracenOpis, p.Kazna, p.Clanak, p.Neaktivan,
                                  DohvatiOpiseZakona(grad, p.IDZakona, idAplikacije),
                                  DohvatiPrijevode(grad, p.IDZakona, true, idAplikacije));

                    return zak.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI ZAKONE");
                return new List<_Zakon>();
            }
        }

        public static List<_Opis> DohvatiOpiseZakona(string grad, int idZakona, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var zak = from o in db.ZAKONI_OPISIs
                              where o.IDZakona == idZakona
                              select new _Opis(o.IDOpisaZakona, idZakona, o.Opis, o.KratkiOpis, o.ClanakPauka, false,
                                  DohvatiPrijevode(grad, o.IDOpisaZakona, false, idAplikacije));

                    return zak.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI OPISE ZAKONA");
                return new List<_Opis>();
            }
        }

        private static List<_Prijevod> DohvatiPrijevode(string grad, int idZakona, bool zakon, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (zakon)
                    {
                        var pri = from o in db.ZAKONI_PRIJEVODIs
                                  where o.IDZakona == idZakona
                                  select new _Prijevod(o.IDPrijevoda, o.IDZakona, o.IDJezika, o.Clanak);

                        return pri.ToList();
                    }

                    var pri1 = from o in db.ZAKONI_OPISI_PRIJEVODIs
                               where o.IDOpisaZakona == idZakona
                               select new _Prijevod(o.IDPrijevodaOpisa, o.IDOpisaZakona, o.IDJezika, o.Opis);

                    return pri1.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI PRIJEVODE ZAKONA");
                return new List<_Prijevod>();
            }
        }

        public static int? DohvatiIDNovogZakona(string grad, int? idOpisaS, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (!idOpisaS.HasValue)
                    {
                        return null;
                    }

                    var opisi = from o in db.OpisiPrekrsajas
                                where o.IDOpisa == idOpisaS
                                select o.IDNovog;

                    if (!opisi.Any())
                    {
                        return null;
                    }

                    return (int)opisi.First();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI ID NOVOG ZAKONA - KONVERZIJA");
                return null;
            }
        }

        /*:: DODAJ ::*/

        //stari
        public static int DodajZakonS(string grad, _Zakon zakon, int idRedarstva, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PopisPrekrsaja pp = new PopisPrekrsaja();
                    pp.Kazna = (int)zakon.Kazna;
                    pp.MaterijalnaKaznjivaNorma = zakon.Clanak;
                    pp.Neaktivan = zakon.Neaktivan;
                    pp.SkraceniOpis = zakon.SkraceniOpis;
                    pp.IDRedarstva = idRedarstva;

                    db.PopisPrekrsajas.InsertOnSubmit(pp);
                    db.SubmitChanges();

                    return pp.IDPrekrsaja;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ NOVI ZAKON STARI");
                return -1;
            }
        }

        public static int DodajKratkiOpisS(string grad, _Opis opis, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    OpisiPrekrsaja op = new OpisiPrekrsaja();

                    op.IDPrekrsaja = opis.IDZakona;
                    op.OpisPrekrsaja = opis.OpisPrekrsaja;
                    op.KratkiOpis = opis.KratkiOpis;
                    op.ClanakPauka = opis.ClanakPauka;
                    op.Obrisan = opis.Obrisan;

                    db.OpisiPrekrsajas.InsertOnSubmit(op);
                    db.SubmitChanges();

                    return op.IDOpisa;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ KRATKI OPIS STARI");
                return -1;
            }
        }

        //novi
        public static int DodajZakon(string grad, _Zakon novi, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    ZAKONI zakon = new ZAKONI();

                    zakon.IDZakona = db.ZAKONIs.Any() ? db.ZAKONIs.Max(i => i.IDZakona) + 100 : 100;
                    zakon.SkracenOpis = novi.SkraceniOpis;
                    zakon.Clanak = novi.Clanak;
                    zakon.Kazna = novi.Kazna;

                    db.ZAKONIs.InsertOnSubmit(zakon);
                    db.SubmitChanges();

                    return zakon.IDZakona;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ NOVI ZAKON");
                return -1;
            }
        }

        public static int DodajKratkiOpis(string grad, _Opis opis, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    ZAKONI_OPISI o = new ZAKONI_OPISI();

                    o.IDOpisaZakona = db.ZAKONI_OPISIs.Any() ? db.ZAKONI_OPISIs.Max(i => i.IDOpisaZakona) + 10 : 10;
                    o.IDZakona = opis.IDZakona;
                    o.KratkiOpis = opis.KratkiOpis;
                    o.Opis = opis.OpisPrekrsaja;
                    o.ClanakPauka = opis.ClanakPauka;
                    //todo o.Obrisan = opis.Obrisan;

                    db.ZAKONI_OPISIs.InsertOnSubmit(o);
                    db.SubmitChanges();

                    return o.IDOpisaZakona;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ KRATKI OPIS");
                return -1;
            }
        }

        /*:: IZMJENI ::*/

        //stari
        public static bool IzmjeniZakonS(string grad, _Zakon zakon, _Opis opis, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    PopisPrekrsaja pp = db.PopisPrekrsajas.First(i => i.IDPrekrsaja == zakon.IDZakona);
                    pp.Kazna = (int)zakon.Kazna;
                    pp.MaterijalnaKaznjivaNorma = zakon.Clanak;
                    pp.Neaktivan = zakon.Neaktivan;

                    db.SubmitChanges();

                    if (opis != null)
                    {
                        OpisiPrekrsaja op = db.OpisiPrekrsajas.First(i => i.IDOpisa == opis.IDOpisa);
                        op.KratkiOpis = opis.KratkiOpis;
                        op.OpisPrekrsaja = opis.OpisPrekrsaja;
                        op.ClanakPauka = opis.ClanakPauka;
                        op.Obrisan = opis.Obrisan;

                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI ZAKON I OPIS STARI");
                return false;
            }
        }

        //novi
        public static bool IzmjeniZakon(string grad, _Zakon zakon, _Opis opis, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    ZAKONI z = db.ZAKONIs.First(i => i.IDZakona == zakon.IDZakona);

                    z.SkracenOpis = zakon.SkraceniOpis;
                    z.Clanak = zakon.Clanak;
                    z.Kazna = zakon.Kazna;
                    z.Neaktivan = zakon.Neaktivan;

                    db.SubmitChanges();

                    if (opis != null)
                    {
                        ZAKONI_OPISI o = db.ZAKONI_OPISIs.First(i => i.IDOpisaZakona == opis.IDOpisa);

                        o.KratkiOpis = opis.KratkiOpis;
                        o.Opis = opis.OpisPrekrsaja;
                        o.ClanakPauka = opis.ClanakPauka;
                        //o.Obrisan = opis.Obrisan;

                        db.SubmitChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI ZAKON I OPIS");
                return false;
            }
        }

        /*:: JEZICI ::*/

        public static List<_2DLista> DohvatiJezike(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var zak = from j in db.JEZICIs
                              select new _2DLista(j.IDJezika, j.NazivJezika);

                    return zak.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI JEZIKE");
                return new List<_2DLista>();
            }
        }

        public static bool SpremiPrijevod(string grad, int idJezika, int idZakona, int idOpisa, string clanak, string opis, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    if (db.ZAKONI_PRIJEVODIs.Any(i => i.IDJezika == idJezika && i.IDZakona == idZakona))
                    {
                        ZAKONI_PRIJEVODI zp = db.ZAKONI_PRIJEVODIs.First(i => i.IDJezika == idJezika && i.IDZakona == idZakona);
                        zp.Clanak = clanak;
                        db.SubmitChanges();
                    }
                    else
                    {
                        ZAKONI_PRIJEVODI zp = new ZAKONI_PRIJEVODI();

                        zp.IDPrijevoda = db.ZAKONI_PRIJEVODIs.Any() ? db.ZAKONI_PRIJEVODIs.Max(i => i.IDPrijevoda) + 1 : 1;
                        zp.IDZakona = idZakona;
                        zp.IDJezika = idJezika;
                        zp.Clanak = clanak;

                        db.ZAKONI_PRIJEVODIs.InsertOnSubmit(zp);
                        db.SubmitChanges();
                    }

                    if (opis != "")
                    {
                        if (db.ZAKONI_OPISI_PRIJEVODIs.Any(i => i.IDJezika == idJezika && i.IDOpisaZakona == idOpisa))
                        {
                            ZAKONI_OPISI_PRIJEVODI zp = db.ZAKONI_OPISI_PRIJEVODIs.First(i => i.IDJezika == idJezika && i.IDOpisaZakona == idOpisa);
                            zp.Opis = opis;
                            db.SubmitChanges();
                        }
                        else
                        {
                            ZAKONI_OPISI_PRIJEVODI zp = new ZAKONI_OPISI_PRIJEVODI();

                            zp.IDPrijevodaOpisa = db.ZAKONI_OPISI_PRIJEVODIs.Any() ? db.ZAKONI_OPISI_PRIJEVODIs.Max(i => i.IDPrijevodaOpisa) + 1 : 1;
                            zp.IDOpisaZakona = idOpisa;
                            zp.IDJezika = idJezika;
                            zp.Opis = opis;

                            db.ZAKONI_OPISI_PRIJEVODIs.InsertOnSubmit(zp);
                            db.SubmitChanges();
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIJEVOD ZAKONA");
                return false;
            }
        }

        //stari
        public static bool SpremiPrijevodS(string grad, int idJezika, int idZakona, int idOpisa, string clanak, string opis, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (db.PopisPrekrsajaPrijevodis.Any(i => i.IDJezika == idJezika && i.IDPrekrsaja == idZakona))
                    {
                        PopisPrekrsajaPrijevodi zp = db.PopisPrekrsajaPrijevodis.First(i => i.IDJezika == idJezika && i.IDPrekrsaja == idZakona);
                        zp.PrijevodClanka = clanak;
                        db.SubmitChanges();
                    }
                    else
                    {
                        PopisPrekrsajaPrijevodi zp = new PopisPrekrsajaPrijevodi();

                        zp.IDPrijevodaPopisa = db.PopisPrekrsajaPrijevodis.Any() ? db.PopisPrekrsajaPrijevodis.Max(i => i.IDPrijevodaPopisa) + 1 : 1;
                        zp.IDPrekrsaja = idZakona;
                        zp.IDJezika = idJezika;
                        zp.PrijevodClanka = clanak;

                        db.PopisPrekrsajaPrijevodis.InsertOnSubmit(zp);
                        db.SubmitChanges();
                    }

                    if (opis != "")
                    {
                        if (db.OpisiPrekrsajaPrijevodis.Any(i => i.IDJezika == idJezika && i.IDOpisa == idOpisa))
                        {
                            OpisiPrekrsajaPrijevodi zp = db.OpisiPrekrsajaPrijevodis.First(i => i.IDJezika == idJezika && i.IDOpisa == idOpisa);
                            zp.Prijevod = opis;
                            db.SubmitChanges();
                        }
                        else
                        {
                            OpisiPrekrsajaPrijevodi zp = new OpisiPrekrsajaPrijevodi();

                            zp.IDPrijevoda = db.OpisiPrekrsajaPrijevodis.Any() ? db.OpisiPrekrsajaPrijevodis.Max(i => i.IDPrijevoda) + 1 : 1;
                            zp.IDOpisa = idOpisa;
                            zp.IDJezika = idJezika;
                            zp.Prijevod = opis;

                            db.OpisiPrekrsajaPrijevodis.InsertOnSubmit(zp);
                            db.SubmitChanges();
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIJEVOD ZAKONA");
                return false;
            }
        }

        /*:: GO ::*/

        public static int IznosKazneS(string grad, int idOpisa, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var zak = from o in db.OpisiPrekrsajas
                        join p in db.PopisPrekrsajas on o.IDPrekrsaja equals p.IDPrekrsaja
                        where o.IDOpisa == idOpisa
                        select p.Kazna;

                    if (zak.Any())
                    {
                        return zak.First();
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati iznos Zakona");
                return 0;
            }
        }

        public static List<_Opis> DohvatiOpiseZakonaGOS(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var zak = from o in db.OpisiPrekrsajas
                        join p in db.PopisPrekrsajas on o.IDPrekrsaja equals p.IDPrekrsaja
                        where p.IDRedarstva == 3 &&
                              o.Obrisan == false
                        orderby o.KratkiOpis
                        select new _Opis(o.IDOpisa, 0, o.OpisPrekrsaja, o.KratkiOpis, o.ClanakPauka, o.Obrisan,
                            DohvatiPrijevodeS(grad, o.IDOpisa, false, idAplikacije));

                    return zak.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI OPISE ZAKONA GO");
                return new List<_Opis>();
            }
        }
    }
}