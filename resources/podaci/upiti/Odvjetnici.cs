using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Odvjetnici
    {
        public static List<_VppPostupanja> DohvatiPostupanja(string grad, bool nepreuzeti, string drzava, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List<_Drzava> drzave = Sustav.Drzave(idAplikacije);
                    List<_2DLista> marke = MarkaVozila(idAplikacije);

                    _Uplatnica np = Gradovi.Uplatnica(grad, 1, idAplikacije);

                    var pos = from vpp in db.VppVanjskoPostupcis
                              join p in db.Prekrsajis on vpp.IDPrekrsaja equals p.IDPrekrsaja
                              join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa
                              join k in db.PopisPrekrsajas on o.IDPrekrsaja equals k.IDPrekrsaja
                              join s in db.VppStatus on vpp.IDStatusaVP equals s.IDStatusaVP into sta
                              from ss in sta.DefaultIfEmpty()
                              where vpp.dozvola == "DA" &&
                                    vpp.RegistracijaOK &&
                                    vpp.IDStatusaVP != 1 &&
                                    vpp.IDStatusaVP != null &&
                                    ss.Zatvara == false &&
                                    (nepreuzeti ? vpp.Preuzeto == false : nepreuzeti == false) &&
                                    (drzava != "??" ? p.KraticaDrzave == drzava : drzava == "??")
                              select new { vpp, p, o, k, ss };

                    List<_VppPostupanja> nova = new List<_VppPostupanja>();

                    foreach (var q in pos)
                    {
                        //if (!new PostavkeDataContext().DRZAVAs.First(i => i.Kratica == q.p.KraticaDrzave).NaplataVP)
                        //{
                        //    continue;
                        //}

                        string mv = "???";

                        if (q.vpp.IDMarkeVozila != null)
                        {
                            mv = marke.First(i => i.Value == q.vpp.IDMarkeVozila).Text;
                        }
                        nova.Add(new _VppPostupanja(q.vpp.IDVanjskoPostupci, q.p.IDPrekrsaja, q.p.IDSkracenogOpisa.Value , q.vpp.IDMarkeVozila, q.vpp.IDStatusaVP, q.p.Vrijeme.Value, q.vpp.datumvrijeme,
                             q.p.RegistracijskaPlocica, q.p.Adresa, np.Poziv1 + "-" + q.p.BrojUpozorenja + "-" + np.Poziv2, q.o.OpisPrekrsaja, q.k.MaterijalnaKaznjivaNorma,
                             q.p.Kazna.ToString(), mv, string.Format("{0} ({1})", drzave.First(i => i.Kratica == q.p.KraticaDrzave).Drzava, q.p.KraticaDrzave), q.ss.NazivStatusa,
                             new _Koordinate(q.p.IDLokacije, (int)q.p.IDDjelatnika, q.p.Lat, q.p.Long, q.p.Vrijeme.Value), q.vpp.Preuzeto, q.ss.Zatvara,
                             q.vpp.Preuzeto == true ? @"\resources\images\icons\kvacica.png" : @"\resources\images\icons\kriz.png", q.vpp.status.ToUpper() == "P", q.vpp.Napomena, q.vpp.Prilog != null));
                    }

                    return nova;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati Postupanja");
                return new List<_VppPostupanja>();
            }
        }

        public static bool OznaciPreuzete(string grad, List<int> preuzeti, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    foreach (var q in preuzeti)
                    {
                        VppVanjskoPostupci vpp = db.VppVanjskoPostupcis.First(i => i.IDVanjskoPostupci == q);
                        vpp.IDStatusaVP = 3;
                        vpp.Preuzeto = true;
                        db.SubmitChanges();

                        SpremiAkcijuPostupanja(grad, q, "Odvjetničko društvo je preuzelo prekršaj!", "Prekršaj je izvezen u CSV format.", idAplikacije);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Oznaci preuzete");
                return false;
            }
        }

        public static List<_VppPostupanja> PretraziPostupanja(string grad, string registracija, string poziv, int idStatusa, string drzava, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {

                    List<_Drzava> drzave = Sustav.Drzave(idAplikacije);
                    List<_2DLista> marke = MarkaVozila(idAplikacije);

                    _Uplatnica np = Gradovi.Uplatnica(grad, 1, idAplikacije);

                    var pos = from vpp in db.VppVanjskoPostupcis
                              join p in db.Prekrsajis on vpp.IDPrekrsaja equals p.IDPrekrsaja
                              join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa
                              join k in db.PopisPrekrsajas on o.IDPrekrsaja equals k.IDPrekrsaja
                              join s in db.VppStatus on vpp.IDStatusaVP equals s.IDStatusaVP into sta
                              from ss in sta.DefaultIfEmpty()
                              where vpp.dozvola == "DA" &&
                                    vpp.RegistracijaOK &&
                                    vpp.Preuzeto == true &&
                                    (registracija != "" ? p.RegistracijskaPlocica.Contains(registracija) : registracija == "") &&
                                    (poziv != "" ? p.BrojUpozorenja == poziv : poziv == "") &&
                                    (idStatusa != 0 ? vpp.IDStatusaVP == idStatusa : idStatusa == 0) &&
                                    (drzava != "??" ? p.KraticaDrzave == drzava : drzava == "??")
                              select new { vpp, p, k, o, ss };

                    List<_VppPostupanja> nova = new List<_VppPostupanja>();

                    foreach (var q in pos)
                    {
                        string mv = "???";

                        if (q.vpp.IDMarkeVozila != null)
                        {
                            mv = marke.First(i => i.Value == q.vpp.IDMarkeVozila).Text;
                        }
                        nova.Add(new _VppPostupanja(q.vpp.IDVanjskoPostupci, q.p.IDPrekrsaja, q.p.IDSkracenogOpisa.Value, q.vpp.IDMarkeVozila, q.vpp.IDStatusaVP, q.p.Vrijeme.Value, q.vpp.datumvrijeme,
                             q.p.RegistracijskaPlocica, q.p.Adresa, np.Poziv1 + "-" + q.p.BrojUpozorenja + "-" + np.Poziv2, q.o.OpisPrekrsaja, q.k.MaterijalnaKaznjivaNorma,
                             q.p.Kazna.ToString(), mv, string.Format("{0} ({1})", drzave.First(i => i.Kratica == q.p.KraticaDrzave).Drzava, q.p.KraticaDrzave), q.ss.NazivStatusa,
                             new _Koordinate(q.p.IDLokacije, (int)q.p.IDDjelatnika, q.p.Lat, q.p.Long, q.p.Vrijeme.Value), q.vpp.Preuzeto, q.ss.Zatvara,
                             q.vpp.Preuzeto == true ? @"\resources\images\icons\kvacica.png" : @"\resources\images\icons\kriz.png", q.vpp.status.ToUpper() == "P", q.vpp.Napomena, q.vpp.Prilog != null));
                    }

                    return nova;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati Postupanja");
                return new List<_VppPostupanja>();
            }
        }

        public static List<_VppPostupanja> PretraziPostupanjaZatvaranje(string grad, DateTime? datumOd, DateTime? datumDo, string drzava, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List<_Drzava> drzave = Sustav.Drzave(idAplikacije);
                    List<_2DLista> marke = MarkaVozila(idAplikacije);

                    _Uplatnica np = Gradovi.Uplatnica(grad, 1, idAplikacije);

                    var pos = from vpp in db.VppVanjskoPostupcis
                              join p in db.Prekrsajis on vpp.IDPrekrsaja equals p.IDPrekrsaja
                              join o in db.OpisiPrekrsajas on p.IDSkracenogOpisa equals o.IDOpisa
                              join k in db.PopisPrekrsajas on o.IDPrekrsaja equals k.IDPrekrsaja
                              join s in db.VppStatus on vpp.IDStatusaVP equals s.IDStatusaVP into sta
                              from ss in sta.DefaultIfEmpty()
                              where vpp.dozvola == "DA" &&
                                    vpp.RegistracijaOK &&
                                    vpp.Preuzeto == true &&
                                    (vpp.IDStatusaVP == null || ss.Zatvara == false) &&
                                    (datumOd != null ? p.Vrijeme.Value.Date >= datumOd : datumOd == null) &&
                                    (datumDo != null ? p.Vrijeme.Value.Date <= datumDo : datumDo == null) &&
                                    (drzava != "??" ? p.KraticaDrzave == drzava : drzava == "??")
                              select new { vpp, p, k, o, ss };

                    List<_VppPostupanja> nova = new List<_VppPostupanja>();

                    foreach (var q in pos)
                    {
                        string mv = "???";

                        if (q.vpp.IDMarkeVozila != null)
                        {
                            mv = marke.First(i => i.Value == q.vpp.IDMarkeVozila).Text;
                        }
                        nova.Add(new _VppPostupanja(q.vpp.IDVanjskoPostupci, q.p.IDPrekrsaja, q.p.IDSkracenogOpisa.Value, q.vpp.IDMarkeVozila, q.vpp.IDStatusaVP, q.p.Vrijeme.Value, q.vpp.datumvrijeme,
                             q.p.RegistracijskaPlocica, q.p.Adresa, np.Poziv1 + "-" + q.p.BrojUpozorenja + "-" + np.Poziv2, q.o.OpisPrekrsaja, q.k.MaterijalnaKaznjivaNorma,
                             q.p.Kazna.ToString(), mv, string.Format("{0} ({1})", drzave.First(i => i.Kratica == q.p.KraticaDrzave).Drzava, q.p.KraticaDrzave), q.ss.NazivStatusa,
                             new _Koordinate(q.p.IDLokacije, (int)q.p.IDDjelatnika, q.p.Lat, q.p.Long, q.p.Vrijeme.Value), q.vpp.Preuzeto, q.ss.Zatvara,
                             q.vpp.Preuzeto == true ? @"\resources\images\icons\kvacica.png" : @"\resources\images\icons\kriz.png", q.vpp.status.ToUpper() == "P", q.vpp.Napomena, q.vpp.Prilog != null));
                    }

                    return nova;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati Postupanja");
                return new List<_VppPostupanja>();
            }
        }

        /*:: KONTROLA ::*/

        public static List<_VppRegistracije> DohvatiProvjeruRegistracije(string grad, int max, out int br, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var pos = from vpp in db.VppVanjskoPostupcis
                              join p in db.Prekrsajis on vpp.IDPrekrsaja equals p.IDPrekrsaja
                              where vpp.dozvola == "DA" && vpp.RegistracijaOK == false && vpp.status.ToUpper() == "A"
                              select new _VppRegistracije(vpp.IDVanjskoPostupci, vpp.IDPrekrsaja, p.IDLokacije, p.RegistracijskaPlocica, p.KraticaDrzave);

                    if (!pos.Any())
                    {
                        br = 0;
                        return new List<_VppRegistracije>();
                    }

                    if (max == 0)
                    {
                        br = pos.Count();
                        return pos.ToList();
                    }

                    br = pos.Count();
                    return pos.Take(max).ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dohvati Postupanja");
                br = 0;
                return new List<_VppRegistracije>();
            }
        }

        public static bool PromijeniRegistraciju(string grad, int idPrekrsaja, int idVppVanjsko, string registracija, string kratica, int idMarke, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Prekrsaji p = db.Prekrsajis.First(s => s.IDPrekrsaja == idPrekrsaja);
                    string stara = p.RegistracijskaPlocica;

                    bool ok = Prekrsaj.Registracija(grad, idPrekrsaja, registracija, kratica, false, idAplikacije);

                    if (!ok)
                    {
                        return false;
                    }

                    VppVanjskoPostupci vppvp = db.VppVanjskoPostupcis.First(i => i.IDVanjskoPostupci == idVppVanjsko);
                    vppvp.RegistracijaOK = true;
                    vppvp.IDStatusaVP = 2;
                    vppvp.IDMarkeVozila = idMarke;
                    db.SubmitChanges();

                    return SpremiAkcijuPostupanja(grad, idVppVanjsko, "Prekontrolirana registracija", "Stara registracija: " + stara, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PROMJENI REGISTRACIJU");
                return false;
            }
        }

        public static bool OdbijRegistraciju(string grad, int idVppVanjsko, string status, string napomena, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VppVanjskoPostupci vppvp = db.VppVanjskoPostupcis.First(i => i.IDVanjskoPostupci == idVppVanjsko);
                    vppvp.RegistracijaOK = true;
                    vppvp.IDStatusaVP = 1;
                    db.SubmitChanges();

                    return SpremiAkcijuPostupanja(grad, idVppVanjsko, status, napomena, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ODBIJ PREKRSAJ");
                return false;
            }
        }

        public static bool OdobriSveRegistracije(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    int br;

                    foreach (var postupanja in DohvatiProvjeruRegistracije(grad, 0, out br, idAplikacije))
                    {
                        VppVanjskoPostupci vppvp = db.VppVanjskoPostupcis.First(i => i.IDVanjskoPostupci == postupanja.IDVppPostupanja);
                        vppvp.RegistracijaOK = true;
                        db.SubmitChanges();
                    }

                    return SpremiAkcijuPostupanja(grad, 0, "Odobrene sve registracije (" + br + ")", "", idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Odobri Sve Registracije");
                return false;
            }
        }

        /*:: AKCIJE LOG ::*/

        public static bool SpremiAkcijuPostupanja(string grad, int idVppVanjsko, string akcija, string napomena, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VppVanjskoAkcije va = new VppVanjskoAkcije();
                    va.IDVanjskoPostupci = idVppVanjsko;
                    va.datum = DateTime.Now;
                    va.akcija = akcija;
                    va.napomena = napomena;
                    va.Poslano = false;

                    db.VppVanjskoAkcijes.InsertOnSubmit(va);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "akcija vpp Postupanja");
                return false;
            }
        }

        /*:: MARKA ::*/

        public static List<_2DLista> MarkaVozila(int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var voz = from m in db.MARKA_VOZILAs
                              orderby m.NazivMarke
                              select new _2DLista(m.IDMarke, m.NazivMarke);

                    return voz.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Dohvati MARKE VOZILA");
                return new List<_2DLista>();
            }
        }

        public static bool DodajMarkuVozila(string marka, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    MARKA_VOZILA mv = new MARKA_VOZILA();

                    mv.IDMarke = db.MARKA_VOZILAs.Max(i => i.IDMarke) + 1;
                    mv.NazivMarke = marka;

                    db.MARKA_VOZILAs.InsertOnSubmit(mv);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Dohvati MARKE VOZILA");
                return false;
            }
        }

        /*:: STATUSI ::*/

        public static List<_2DLista> Statusi(string grad, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var voz = from m in db.VppStatus
                              orderby m.NazivStatusa
                              //where m.Rucno
                              select new _2DLista(m.IDStatusaVP, m.NazivStatusa);

                    return voz.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Dohvati STATUSE");
                return new List<_2DLista>();
            }
        }

        public static bool IzmjeniStatus(string grad, List<_VppPostupanja> postupanja, int idStatusa, string napomena, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    foreach (var p in postupanja)
                    {
                        VppVanjskoPostupci vpp = db.VppVanjskoPostupcis.First(i => i.IDVanjskoPostupci == p.IDPreuzimanja);
                        vpp.IDStatusaVP = idStatusa;
                        db.SubmitChanges();

                        string status = db.VppStatus.First(i => i.IDStatusaVP == idStatusa).NazivStatusa;

                        SpremiAkcijuPostupanja(grad, p.IDPreuzimanja, status, napomena, idAplikacije);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "IZMJENI STATUS");
                return false;
            }
        }

        /*:: UPLATE ::*/

        public static bool ZavediUplatu(string grad, _VppPostupanja postupanje, string napomena, string poziv, DateTime datum, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VppVanjskoUplate vpp = new VppVanjskoUplate();
                    vpp.IDVanjskoPostupci = postupanje.IDPreuzimanja;
                    vpp.datum = datum;
                    vpp.platitelj = "";
                    vpp.iznos = Convert.ToDecimal(postupanje.Kazna);
                    vpp.pnb_odobrenja = poziv;
                    vpp.tip = "KAZNA";
                    vpp.napomena = napomena;
                    vpp.Poslano = false;

                    db.VppVanjskoUplates.InsertOnSubmit(vpp);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "UPLATA");
                return false;
            }
        }

        public static bool UkupnaUplata(string grad, string poziv, DateTime datum, decimal iznos, int br, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    UPLATE_ODVJETNIKA uo = new UPLATE_ODVJETNIKA();

                    uo.IDGrada = Sistem.IDGrada(grad);
                    uo.Poziv = poziv;
                    uo.Datum = datum;
                    uo.Iznos = iznos;
                    uo.BrojPrekrsaja = br;

                    db.UPLATE_ODVJETNIKAs.InsertOnSubmit(uo);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "SPREMI UKUPNU UPLATU");
                return false;
            }
        }

        public static List<_Uplata> Uplate(int idGrada, DateTime? datumOd, DateTime? datumDo, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var upl = from u in db.UPLATE_ODVJETNIKAs
                              join g in db.GRADOVIs on u.IDGrada equals g.IDGrada
                              where (datumOd != null ? u.Datum.Date >= datumOd : datumOd == null) &&
                                    (datumDo != null ? u.Datum.Date <= datumDo : datumDo == null) &&
                                    (idGrada != 0 ? u.IDGrada == idGrada : idGrada == 0)
                              select new _Uplata(u.IDUplate, u.IDGrada, g.NazivGrada, u.Poziv, u.Datum, u.Iznos, u.BrojPrekrsaja);

                    return upl.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "SPREMI UKUPNU UPLATU");
                return new List<_Uplata>();
            }
        }

        public static List<_VppPostupanja> UplacenaPostupanja(string grad, string poziv, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var upl = from u in db.VppVanjskoUplates
                              join q in db.VppVanjskoPostupcis on u.IDVanjskoPostupci equals q.IDVanjskoPostupci
                              join p in db.Prekrsajis on q.IDPrekrsaja equals p.IDPrekrsaja
                              where u.pnb_odobrenja == poziv
                              select new _VppPostupanja(q.IDVanjskoPostupci, q.IDPrekrsaja, 0, 0, 7, p.Vrijeme.Value, q.datumvrijeme,
                             string.Format("{0} ({1})", p.RegistracijskaPlocica, p.KraticaDrzave), "", p.PozivNaBroj, "", "", 
                             p.Kazna.ToString(), "", "", "", null, null, true, "", q.status.ToUpper() == "P", q.Napomena, q.Prilog != null);

                    return upl.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "DOHVATI DETALJE UPLATE");
                return new List<_VppPostupanja>();
            }
        }

        /*:: PRILOG ::*/

        public static byte[] Prilog(string grad, int idPostupanja, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    VppVanjskoPostupci vpp = db.VppVanjskoPostupcis.First(i => i.IDVanjskoPostupci == idPostupanja);

                    return vpp.Prilog.ToArray();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRILOG POSTUPANJU");
                return null;
            }
        }

    }
}