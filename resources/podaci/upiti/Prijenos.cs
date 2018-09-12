using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.cs.email;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Prijenos
    {
        #region SPLIT

        public static string url = "http://ritamapi.dyndns.ws/rest/api/v1";
        public static string key = "ritam";
        public static string username = "test";

        /*:: POZIVI ::*/

        public static bool Placeni(string grad, DateTime datumOd, DateTime datumDo, out string poruka, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    #region PODACI

                    var rac =
                            db.RACUNIs.Where(r => r.Datum >= datumOd && r.Datum <= datumDo && r.IDVrstePlacanja != 4 && r.IDVrstePlacanja != 5);
                    // && r.Prenesen == false

                    if (!rac.Any())
                    {
                        PovijestPrijenosa(grad, new List<int>(), "Nema ne prenesenih računa za odabrani datum!", "",
                            true, idAplikacije);
                        poruka = "Nema ne prenesenih računa za odabrani datum!";
                        return true;
                    }

                    List<_StavkeP> stavke = new List<_StavkeP>();
                    List<_VrstaPla> vrstea = new List<_VrstaPla>();
                    //treba grupirati kartice u jedno i odgode ako ih bude trebalo
                    List<_VrstaPla> vrste = new List<_VrstaPla>();

                    foreach (var r in rac.GroupBy(i => i.IDVrstePlacanja))
                    {
                        vrstea.Add(new _VrstaPla(Convert.ToChar(Naplata.VrstaPlacanjaKratica(grad, r.Key, idAplikacije)), "",
                            r.Sum(i => i.Ukupno).ToString("n2")));//.Replace(".", ",")
                    }

                    foreach (var r in vrstea.GroupBy(i => i.vp_tip))
                    {
                        decimal ukupno = 0;

                        foreach (var u in r)
                        {
                            ukupno += Convert.ToDecimal(u.vp_iznos);
                        }

                        vrste.Add(new _VrstaPla(r.Key, "", ukupno.ToString("n2").Replace(".", "")));
                    }

                    foreach (var r in rac.GroupBy(i => i.PDVPosto))
                    {
                        stavke.Add(new _StavkeP(r.Key.ToString("n1").Replace(".", ""), "0,0",
                            r.Sum(i => i.Osnovica).ToString().Replace(".", "").Replace(",0000", ",00"), "0,0", "0,0", "0,0",
                            r.Sum(i => i.PDV).ToString().Replace(".", "").Replace(",0000", ",00"), "0,0", "0,0", "0,0", "0,0", "0,0", "0,0"));
                    }

                    string pp = db.POSLOVNI_PROSTORs.First(i => i.IDRedarstva == 2).Oznaka;

                    _Placeni plac = new _Placeni(new _Zaglavlje("", pp, datumOd.ToString("dd.MM.yyyy"), "", ""), stavke, vrste);

                    #endregion

                    #region POZIV

                    string datum = DateToString(DateTime.Now);

                    string metoda = "/sale/dailysales/create";
                    string tip = "POST";

                    string signature = CalculateSignature(tip, "/rest/api/v1" + metoda, datum);

                    string adresa = metoda + "?username=" + username + "&date=" + datum + "&signature=" + signature;

                    string response = MakeRequest(url + adresa, plac, tip, "application/json", typeof(string));

                    #endregion

                    if (response.Contains("uspješno spremljena"))
                    {
                        try
                        {
                            List<int> preneseni = rac.Select(i => i.IDRacuna).ToList(); //and idvrste placanja != 4,5 

                            foreach (var r in rac)
                            {
                                r.Prenesen = true;
                                db.SubmitChanges();
                            }

                            PovijestPrijenosa(grad, preneseni, response, JsonConvert.SerializeObject(plac), true, idAplikacije);

                            poruka = "Svi računi za odabrani datum su uspješno preneseni!";
                            return true;
                        }
                        catch (Exception ex)
                        {
                            PovijestPrijenosa(grad, rac.Select(i => i.IDRacuna).ToList(), response, JsonConvert.SerializeObject(plac), false, idAplikacije);
                            poruka = "Došlo je do greške prilikom prijenosa računa, računi su preneseni ali nisu označeni kao preneseni!";
                            Posalji.Email(grad, poruka, "GREŠKA PRIJENOS - " + grad.Replace("PROMETNIK_", "").Replace("_", " "),
                                new List<string> { "daniel.pajalic@ri-ing.net" }, null, false, idAplikacije);
                            Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIJENOS RACUNA U RAČUNOVODSTVO");
                            return false;
                        }
                    }

                    Sustav.SpremiGresku(grad, new Exception(response), idAplikacije, "PRIJENOS RACUNA U RAČUNOVODSTVO");
                    PovijestPrijenosa(grad, rac.Select(i => i.IDRacuna).ToList(), response,
                        JsonConvert.SerializeObject(plac), false, idAplikacije);
                    poruka = "Došlo je do greške prilikom prijenosa računa!";
                    return false;
                }
            }
            catch (Exception ex)
            {
                PovijestPrijenosa(grad, new List<int>(), "", "", false, idAplikacije);
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIJENOS RACUNA U RAČUNOVODSTVO");
                poruka = "Došlo je do greške prilikom prijenosa računa!";
                return false;
            }
        }

        public static bool Odgode(string grad, DateTime datumOd, DateTime datumDo, out string poruka, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    #region PODACI

                    var racuni = from r in db.RACUNIs
                                 where r.Datum >= datumOd &&
                                       r.Datum <= datumDo &&
                                       (r.IDVrstePlacanja == 4 || r.IDVrstePlacanja == 5) &&
                                       r.Prenesen == false
                                 select r;

                    if (!racuni.Any())
                    {
                        PovijestPrijenosa(grad, new List<int>(), "Nema ne prenesenih odgoda za odabrani datum!", "", true, idAplikacije);
                        poruka = "Nema ne prenesenih odgoda za odabrani datum!";
                        return true;
                    }

                    #endregion

                    poruka = "";
                    foreach (var r in racuni)
                    {
                        List<_Osoba> oso = (from ro in db.RACUNI_OSOBE_RELACIJEs
                                            join o in db.RACUNI_OSOBEs on ro.IDOsobe equals o.IDOsobe
                                            where ro.IDRacuna == r.IDRacuna &&
                                                  !ro.Vlasnik
                                            select new _Osoba(o.IDOsobe, o.Ime, o.Prezime, o.Ulica, o.KucniBroj, o.Posta, o.Mjesto, o.Drzava, o.OIB, o.Napomena, o.BrojDokumenta, o.Rodjen, ro.Vlasnik, o.MUP)).ToList();


                        if (!oso.Any())
                        {
                            oso.Add(new _Osoba(0, "", "", "", "", "", "", "", "", "", "", null, false, false));
                        }

                        var os = oso.First();

                        if (oso.Count > 1)
                        {
                            os = oso.First(i => i.Vlasnik == false);
                        }

                        List<_StavkeOdgoda> stavke = new List<_StavkeOdgoda>();
                        List<_StopeOdgoda> po_stopama = new List<_StopeOdgoda>();

                        po_stopama.Add(new _StopeOdgoda(r.PDVPosto.ToString("n1").Replace(".", ","), "0,0", "0,0",
                            r.Osnovica.ToString().Replace(".", ",").Replace(",0000", ",00"), r.PDV.ToString().Replace(".", ",").Replace(",0000", ",00"), "0,0", "0,0"));
                        stavke.Add(new _StavkeOdgoda("0,0", "0,0", "0,0", "0,0", "0,0", "0,0", "0,0", "0,0",
                            r.PDV.ToString().Replace(".", ",").Replace(",0000", ",00"), r.Ukupno.ToString().Replace(".", ",").Replace(",0000", ",00")));

                        string adresaO = os.Ulica + " " + os.KBr;
                        // string oi = string.IsNullOrEmpty(r.o.OIB) ? r.o.BrojDokumenta : "";

                        int dospijece = db.POSLOVNI_PROSTORs.First(i => i.IDRedarstva == 2).Dospijece;

                        _ZaglavljeOdgoda zaglavlje = new _ZaglavljeOdgoda(r.PoslovniProstor, datumOd.ToString("dd.MM.yyyy"),//datum od jer zele da im bude sve sa istim datumom (Pauk - noćna blagajna 05.04.2018 - email ivan matana)
                            r.Datum.AddDays(dospijece).ToString("dd.MM.yyyy"), r.BrojRacuna, "V62", "E0", os.OIB,
                            os.Prezime + " " + os.Ime, adresaO, os.Posta, os.Mjesto, r.PozivNaBroj, "", os.Napomena,
                            r.IDRacuna.ToString());
                        _Odgoda plac = new _Odgoda(zaglavlje, stavke, po_stopama);

                        #region POZIV

                        string datum = DateToString(DateTime.Now);
                        string metoda = "/fin/knjizenje/racun/create";
                        string tip = "POST";

                        string signature = CalculateSignature(tip, "/rest/api/v1" + metoda, datum);

                        string adresa = metoda + "?username=" + username + "&date=" + datum + "&signature=" + signature;

                        string response = MakeRequest(url + adresa, plac, tip, "application/json", typeof(string));

                        #endregion

                        #region ODGOVOR

                        List<int> preneseni = new List<int> { r.IDRacuna };

                        if (response.Contains("uspješno izvršeno"))
                        {
                            try
                            {
                                db.RACUNIs.First(i => i.IDRacuna == r.IDRacuna).Prenesen = true;
                                db.SubmitChanges();

                                PovijestPrijenosa(grad, preneseni, response, JsonConvert.SerializeObject(plac), true, idAplikacije);
                                //poruka += "Račun br. " + r.BrojRacuna + " - " + response;
                            }
                            catch (Exception ex)
                            {
                                PovijestPrijenosa(grad, preneseni, response, JsonConvert.SerializeObject(plac), false, idAplikacije);
                                poruka =
                                    "Došlo je do greške prilikom prijenosa računa, računi su preneseni ali nisu označeni kao preneseni!";
                                Posalji.Email(grad, poruka,
                                    "GREŠKA PRIJENOS - " + grad.Replace("PROMETNIK_", "").Replace("_", " "),
                                    new List<string> { "daniel.pajalic@ri-ing.net" }, null, false, idAplikacije);
                                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIJENOS RACUNA U RAČUNOVODSTVO");
                            }
                        }
                        else
                        {
                            poruka += "Račun br. " + r.BrojRacuna + " - " + response + "\r\n";
                            PovijestPrijenosa(grad, preneseni, response, JsonConvert.SerializeObject(plac), false,
                                idAplikacije);
                        }

                        #endregion
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                PovijestPrijenosa(grad, new List<int>(), "", "", false, idAplikacije);
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIJENOS RACUNA U RAČUNOVODSTVO");
                poruka = "Došlo je do greške prilikom prijenosa računa!";
                return false;
            }
        }

        /*:: JSON - REST ::*/

        public static string MakeRequest(string requestUrl, object JSONRequest, string JSONmethod, string JSONContentType, Type JSONResponseType)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Method = JSONmethod;
                request.ContentType = JSONContentType;

                if (JSONRequest != null)
                {
                    string sb = JsonConvert.SerializeObject(JSONRequest) + "}";

                    byte[] bt = Encoding.UTF8.GetBytes(sb);

                    Stream requestStream = request.GetRequestStream();

                    requestStream.Write(bt, 0, bt.Length);
                    requestStream.Close();
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(string.Format("Server error (HTTP {0}: {1}).", response.StatusCode,
                            response.StatusDescription));

                    Stream st = response.GetResponseStream();
                    StreamReader sr = new StreamReader(st);
                    return sr.ReadToEnd();
                    //object objResponse = JsonConvert.DeserializeObject(strsb, JSONResponseType);
                    //return objResponse.ToString();
                }
            }
            catch (WebException ex)
            {
                Sustav.SpremiGresku("", ex, 1, "Make Request");

                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, 10, "JSON REQUEST");
                return ex.Message;
            }
        }

        public static string CalculateSignature(string HTTPVerb, string filename, string dateTime)
        {
            try
            {
                string signature = HTTPVerb + Environment.NewLine + filename + Environment.NewLine + dateTime;
                return Encode(signature);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string Encode(string input)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            byte[] keyByte = Encoding.ASCII.GetBytes(key);
            using (var myhmacsha1 = new HMACSHA1(keyByte))
            {
                var hashArray = myhmacsha1.ComputeHash(byteArray);
                return hashArray.Aggregate("", (s, e) => s + string.Format("{0:x2}", e), s => s);
            }
        }

        public static string DateToString(DateTime datum)
        {
            return datum.ToString("dd.MM.yyyy HH:mm:ss");
        }

        /*:: POVIJEST ::*/

        public static void PovijestPrijenosa(string grad, List<int> preneseni, string response, string request,
            bool uspjesno, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    PRIJENOS_RACUNOVODSTVO pr = new PRIJENOS_RACUNOVODSTVO();

                    int id = 1;

                    if (db.PRIJENOS_RACUNOVODSTVOs.Any())
                    {
                        id = db.PRIJENOS_RACUNOVODSTVOs.Max(i => i.IDPrijenosa) + 1;
                    }

                    pr.IDPrijenosa = id;
                    pr.IDGrada = Sistem.IDGrada(grad);
                    pr.Datum = DateTime.Now;
                    pr.IDPrenesenih = JsonConvert.SerializeObject(preneseni);
                    pr.BrojPrenesenih = preneseni.Count;
                    pr.Response = response;
                    pr.Request = request;
                    pr.Uspjesno = uspjesno;

                    db.PRIJENOS_RACUNOVODSTVOs.InsertOnSubmit(pr);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIJENOS RACUNA U RAČUNOVODSTVO LOG");
            }
        }

        public static List<_PovijestPrijenosa> DohvatiPovijestPrijenosa(string grad, DateTime? datumOd, DateTime? datumDo, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var pov = from p in db.PRIJENOS_RACUNOVODSTVOs
                              where p.IDGrada == Sistem.IDGrada(grad) &&
                                    (datumOd != null ? p.Datum.Date >= datumOd : datumOd == null) &&
                                    (datumDo != null ? p.Datum.Date <= datumDo : datumDo == null)
                              select new _PovijestPrijenosa(p.IDPrijenosa, p.Datum, p.IDPrenesenih, p.BrojPrenesenih, p.Response, p.Uspjesno);

                    if (pov.Any())
                    {
                        return pov.ToList();
                    }

                    return new List<_PovijestPrijenosa>();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POVIJEST PRIJENOSA");
                return new List<_PovijestPrijenosa>();
            }
        }

        /*:: PRIPREMA ::*/

        public static _Prijenos Pripremi(string grad, DateTime datumOd, DateTime datumDo, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var rac = from r in db.RACUNIs
                              where r.Datum >= datumOd &&
                                    r.Datum <= datumDo
                              select r;

                    if (!rac.Any(r => !r.Prenesen))
                    {
                        return null;
                    }

                    int brRac = rac.Count(), brKar = 0, brGot = 0, brOdg = 0;
                    decimal uku = rac.Sum(i => i.Ukupno), got = 0, kar = 0, odg = 0;

                    foreach (var r in rac.GroupBy(i => i.IDVrstePlacanja))
                    {
                        if (r.Key == 1)
                        {
                            brGot = r.Count();
                            got = r.Sum(i => i.Ukupno);
                        }

                        if (r.Key == 2 || r.Key == 3)
                        {
                            brKar += r.Count();
                            kar += r.Sum(i => i.Ukupno);
                        }

                        foreach (var q in r.Where(i => (i.IDVrstePlacanja == 4 || i.IDVrstePlacanja == 5) && i.Prenesen == false))
                        {
                            brOdg++;
                            odg += q.Ukupno;
                        }

                        //if ((r.Key == 4 || r.Key == 5) && r.First().Prenesen == false)
                        //{
                        //    brOdg += r.Count();
                        //    odg += r.Sum(i => i.Ukupno);
                        //}
                    }

                    return new _Prijenos(brRac, brKar, brGot, brOdg, uku, kar, got, odg);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIJENOS RACUNA U RAČUNOVODSTVO PRIPREMA");
                return null;
            }
        }

        #endregion

        #region RIJEKA

        public static string putanja = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"resources\pdf\temp\";

        public static bool OdgodeR(string grad, DateTime datumOd, DateTime datumDo, List<string> primatelji, out string poruka, int idAplikacije)
        {
            try
            {
                //poruka = "";
                //return true;
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    #region PODACI

                    var racuni = from r in db.RACUNIs
                                 where r.Datum >= datumOd &&
                                       r.Datum <= datumDo &&
                                       (r.IDVrstePlacanja == 4 || r.IDVrstePlacanja == 5) &&
                                       r.Prenesen == false
                                 select r;

                    if (!racuni.Any())
                    {
                        PovijestPrijenosa(grad, new List<int>(), "Nema ne prenesenih odgoda za odabrani datum!", "", true, idAplikacije);
                        poruka = "Nema ne prenesenih odgoda za odabrani datum!";
                        return true;
                    }

                    #endregion

                    List<_2DLista> sifrastavke = db.RACUNI_STAVKE_OPIs.Select(i => new _2DLista(i.IDOpisaStavke, i.Sifra)).ToList();

                    poruka = "";
                    string partneri = "partneri_" + datumOd.ToString("dd_MM_yy") + ".csv";
                    string stavke = "odgode_" + datumOd.ToString("dd_MM_yy") + ".csv";

                    string putanjaP = Putanja(putanja + partneri);
                    string putanjaO = Putanja(putanja + stavke);

                    int dospijece = db.POSLOVNI_PROSTORs.First(i => i.IDRedarstva == 2).Dospijece;

                    using (StreamWriter tw = new StreamWriter(putanjaP, false, Encoding.Default))
                    {
                        using (StreamWriter rac = new StreamWriter(putanjaO, false, Encoding.Default))
                        {
                            foreach (var r in racuni)
                            {
                                var oso = from ro in db.RACUNI_OSOBE_RELACIJEs
                                          join o in db.RACUNI_OSOBEs on ro.IDOsobe equals o.IDOsobe
                                          where ro.IDRacuna == r.IDRacuna
                                          select new { o, ro.Vlasnik };

                                string registracija = "", oib = "00000000000";

                                try
                                {
                                    registracija = db.Prekrsajis.First(i => i.IDNaloga == r.IDReference).RegistracijskaPlocica;
                                }
                                catch 
                                {
                                    
                                }

                                if (!oso.Any())
                                {                                  
                                    tw.WriteLine(oib + "#" +  "#" + "#" + "#" + "#" + "#"  + "#");
                                }
                                else
                                {
                                    var os = oso.First();

                                    if (oso.Count() > 1)
                                    {
                                        if (oso.Any(i => i.Vlasnik))
                                        {
                                            os = oso.First(i => i.Vlasnik);
                                        }
                                    }

                                    oib = os.o.OIB;
                                    tw.WriteLine(os.o.OIB + "#" + os.o.Ime + " " + os.o.Prezime + "#" + os.o.Mjesto + "#" + os.o.Ulica + " " + os.o.KucniBroj + "#" + os.o.Posta + "#" + os.o.Drzava + "#" + "" + "#" + "");
                                }

                                foreach (var stavka in db.RACUNI_STAVKEs.Where(i => i.IDRacuna == r.IDRacuna))
                                {
                                    string kol = stavka.Kolicina.ToString("N3");
                                    string cijena = stavka.Cijena.ToString("N4"); //.Replace(".", "");

                                    if (stavka.Ukupno < 0)
                                    {
                                        kol = "-" + kol;
                                    }

                                    if (stavka.Cijena < 0)
                                    {
                                        cijena = (stavka.Cijena * -1).ToString("N4"); //.Replace(".", "");
                                    }

                                    if (cijena.Contains(".") && cijena.Contains(","))
                                    {
                                        cijena = cijena.Replace(".", "");
                                    }
                                    else if (cijena.Contains(".") && !cijena.Contains(","))
                                    {
                                        cijena = cijena.Replace(".", ",");
                                    }

                                    rac.WriteLine("05" + "#" + "7P" + "#" + oib + "#" + r.PozivNaBroj + "#" + r.Datum.ToString("yyMMdd") + "#" + dospijece + "#" + sifrastavke.First(i => i.Value == stavka.IDOpisaStavke).Text + "#" + kol + 
                                                  "#" + cijena + "#300000#300020" + "#" + r.BrojRacuna + " " + r.Godina.ToString().Replace("20", "") + " " + registracija);
                                }
                            }

                            rac.Close();
                        }

                        tw.Close();
                    }

                    //saljem na email
                    List<string> prilozi = new List<string>();
                    prilozi.Add(putanjaP);
                    prilozi.Add(putanjaO);

                    string body = cs.email.Pripremi.PopulateBodyPrijenos("Point", datumOd.ToString("dd.MM.yy"));

                    bool poslano = Posalji.EmailPrilozi(grad, body, "Prijenos podataka - ODGODE (" + datumOd.ToString("dd.MM.yy") + ")", primatelji, prilozi, true, idAplikacije);

                    if (poslano)
                    {
                        //foreach (RACUNI r in racuni)
                        //{
                        //    r.Prenesen = true;
                        //    db.SubmitChanges();
                        //}

                        poruka = "Prijenos podataka je uspješno izvršen";
                        return true;
                    }

                    poruka = "Došlo je do greške prilikom slanja podataka";
                    return false;
                }
            }
            catch (Exception ex)
            {
                PovijestPrijenosa(grad, new List<int>(), "", "", false, idAplikacije);
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIJENOS RACUNA U RAČUNOVODSTVO");
                poruka = "Došlo je do greške prilikom prijenosa računa!";
                return false;
            }
        }

        public static bool TemeljnicaR(string grad, DateTime datum, List<string> primatelji, out string poruka, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    #region PODACI

                    var racuni = from r in db.BLAGAJNICKI_DNEVNIKs
                                 join s in db.BLAGAJNICKI_DNEVNIK_STAVKEs on r.IDDnevnika equals s.IDDnevnika
                                 where r.Datum.Date == datum.Date
                                 select new { r, s };

                    if (!racuni.Any())
                    {
                        PovijestPrijenosa(grad, new List<int>(), "Nema ne prenesenih temeljnica za odabrani datum!", "", true, idAplikacije);
                        poruka = "Nema ne prenesenih podataka za odabrani datum!";
                        return true;
                    }

                    #endregion

                    POSLOVNI_PROSTOR pp = db.POSLOVNI_PROSTORs.First(i => i.IDRedarstva == 2);

                    poruka = "";
                    string temeljnica = "temeljnica_" + datum.ToString("dd_MM_yy") + ".csv";
                    string putanjaO = Putanja(putanja + temeljnica);

                    using (StreamWriter rac = new StreamWriter(putanjaO, false, Encoding.Default))
                    {
                        foreach (var r in racuni)
                        {
                            decimal iznos = r.s.Primitak;

                            if (r.s.Izdatak != 0)
                            {
                                iznos = r.s.Izdatak;
                            }

                            if (iznos == 0)
                            {
                                continue;
                            }

                            string iznos1 = iznos.ToString("N2");
                            if (iznos1.Contains(".") && iznos1.Contains(","))
                            {
                                iznos1 = iznos1.Replace(".", "");
                            }
                            else if (iznos1.Contains(".") && !iznos1.Contains(","))
                            {
                                iznos1 = iznos1.Replace(".", ",");
                            }

                            string objektTroska = "RPP01", nm = "";

                            if (!string.IsNullOrEmpty(r.s.NaplatnoMjesto))
                            {
                                if (r.s.NaplatnoMjesto == "JT")
                                {
                                    objektTroska = "RPP02";
                                }

                                nm = r.s.NaplatnoMjesto + "-";
                            }

                            if (r.s.Opis == "DNEVNI UTRŽAK")
                            {
                                decimal osnovica = Math.Round(iznos / ((decimal)(100 + pp.PDV) / 100), 2);
                                decimal pdv = Math.Round(osnovica * pp.PDV / 100, 2);

                                string osnovica1 = osnovica.ToString("N2");
                                if (osnovica1.Contains(".") && osnovica1.Contains(","))
                                {
                                    osnovica1 = osnovica1.Replace(".", "");
                                }
                                else if (osnovica1.Contains(".") && !osnovica1.Contains(","))
                                {
                                    osnovica1 = osnovica1.Replace(".", ",");
                                }

                                string pdv1 = pdv.ToString("N2");
                                if (pdv1.Contains(".") && pdv1.Contains(","))
                                {
                                    pdv1 = pdv1.Replace(".", "");
                                }
                                else if (pdv1.Contains(".") && !pdv1.Contains(","))
                                {
                                    pdv1 = pdv1.Replace(".", ",");
                                }

                                //ukupno
                                rac.WriteLine("102306" + "#D#" + iznos1 + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + r.r.Datum.ToString("dd.MM.yyyy") + "#" + "#" + "#" + "#" + "#" + "#" + nm + r.s.Opis + "/" + r.s.Tip + "-" + r.s.RB);

                                //pdv
                                rac.WriteLine("240011" + "#P#" + pdv1 + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + r.r.Datum.ToString("dd.MM.yyyy") + "#" + "#" + "#" + "#" + "#" + "#" + nm + r.s.Opis + "/" + r.s.Tip + "-" + r.s.RB);

                                //osnovica
                                rac.WriteLine("751005" + "#P#" + osnovica1 + "#" + "#" + "#" + "#" + "#" + "#" + "300020#" + "300020#" + objektTroska + "#" + "#" + "#" + r.r.Datum.ToString("dd.MM.yyyy") + "#" + "#" + "#" + "#" + "#" + "#" + nm + r.s.Opis + "/" + r.s.Tip + "-" + r.s.RB);
                            }

                            if (r.s.Opis == "POLOG I SMJENA")
                            {
                                rac.WriteLine("102306" + "#P#" + iznos1 + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + objektTroska + "#" + "#" + "#" + r.r.Datum.ToString("dd.MM.yyyy") + "#" + "#" + "#" + "#" + "#" + "#" + nm + r.s.Opis + "/" + r.s.Tip + "-" + r.s.RB);
                                rac.WriteLine("100906" + "#D#" + iznos1 + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + objektTroska + "#" + "#" + "#" + r.r.Datum.ToString("dd.MM.yyyy") + "#" + "#" + "#" + "#" + "#" + "#" + nm + r.s.Opis + "/" + r.s.Tip + "-" + r.s.RB);
                            }

                            if (r.s.Opis == "POLOG II SMJENA")
                            {
                                rac.WriteLine("102306" + "#P#" + iznos1 + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + objektTroska + "#" + "#" + "#" + r.r.Datum.ToString("dd.MM.yyyy") + "#" + "#" + "#" + "#" + "#" + "#" + nm + r.s.Opis + "/" + r.s.Tip + "-" + r.s.RB);
                                rac.WriteLine("100906" + "#D#" + iznos1 + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + objektTroska + "#" + "#" + "#" + r.r.Datum.ToString("dd.MM.yyyy") + "#" + "#" + "#" + "#" + "#" + "#" + nm + r.s.Opis + "/" + r.s.Tip + "-" + r.s.RB);
                            }
                        }

                        foreach (var r in racuni.Where(i => i.s.IDBanke.HasValue).GroupBy(i => i.s.IDBanke))
                        {
                            string konto = "117800"; //PBZ

                            if (r.Key == 1)
                            {
                                konto = "117700"; //ERSTE
                            }

                            string izdatak = r.Sum(i => i.s.Izdatak).ToString("N2");
                            if (izdatak.Contains(".") && izdatak.Contains(","))
                            {
                                izdatak = izdatak.Replace(".", "");
                            }
                            else if (izdatak.Contains(".") && !izdatak.Contains(","))
                            {
                                izdatak = izdatak.Replace(".", ",");
                            }

                            rac.WriteLine("102306" + "#P#" + izdatak + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + r.First().r.Datum.ToString("dd.MM.yyyy") + "#" + "#" + "#" + "#" + "#" + "#" + "KARTICE " + Naplata.VrstaBanke(grad, r.Key, idAplikacije));

                            foreach (var s in r)
                            {
                                string izdatak2 = s.s.Izdatak.ToString("N2");
                                if (izdatak2.Contains(".") && izdatak2.Contains(","))
                                {
                                    izdatak2 = izdatak2.Replace(".", "");
                                }
                                else if (izdatak2.Contains(".") && !izdatak2.Contains(","))
                                {
                                    izdatak2 = izdatak2.Replace(".", ",");
                                }

                                rac.WriteLine(konto + "#D#" + izdatak2 + "#" + "#" + "#" + "#" + "#" + "#" + "#" + "#" + Naplata.VrstaKarticeKratica(grad, s.s.IDVrsteKartice, idAplikacije) + "#" + "#" + "#" + r.First().r.Datum.ToString("dd.MM.yyyy") + "#" + "#" + "#" + "#" + "#" + "#" + s.s.NaplatnoMjesto + "-" + s.s.Opis + "/" + s.s.Tip + "-" + s.s.RB);
                            }
                        }

                        rac.Close();
                    }

                    //saljem na email
                    List<string> prilozi = new List<string>();
                    prilozi.Add(putanjaO);

                    string body = cs.email.Pripremi.PopulateBodyPrijenos("Point", datum.ToString("dd.MM.yy"));

                    bool poslano = Posalji.EmailPrilozi(grad, body, "Prijenos podataka - TEMELJNICA (" + datum.ToString("dd.MM.yy") + ")", primatelji, prilozi, true, idAplikacije);

                    if (poslano)
                    {
                        poruka = "Prijenos podataka je uspješno izvršen";
                        return true;
                    }

                    poruka = "Došlo je do greške prilikom slanja podataka";
                    return false;
                }
            }
            catch (Exception ex)
            {
                PovijestPrijenosa(grad, new List<int>(), "", "", false, idAplikacije);
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIJENOS RACUNA U RAČUNOVODSTVO");
                poruka = "Došlo je do greške prilikom prijenosa računa!";
                return false;
            }
        }

        #endregion

        private static string Putanja(string putanjax, int x = 1)
        {
            if (File.Exists(putanjax))
            {
                var novaPutanja = putanjax.Replace(".csv", "") + " (" + x + ").csv";
                x++;
                return Putanja(novaPutanja, x);
            }

            return putanjax;
        }
    }
}

