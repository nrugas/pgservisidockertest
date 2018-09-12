using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi
{
    public class PGMobile : IPGMobile
    {
        private const int idAplikacije = 1;

        public void Aktivan(string grad, int idKorisnika, string terminalid, string hostname, string verzija, string os)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Djelatnik d = db.Djelatniks.First(i => i.IDDjelatnika == idKorisnika);

                    if (db.Terminalis.Any(i => i.IdentifikacijskiBroj == terminalid))
                    {
                        Terminali t = db.Terminalis.First(i => i.IdentifikacijskiBroj == terminalid);

                        if (hostname == "")
                        {
                            hostname = t.NazivTerminala;
                        }

                        if (verzija == "")
                        {
                            verzija = t.Verzija;
                        }
                    }

                    Regex regex = new Regex(@"\s(.*)");
                    Match match = regex.Match(verzija);
                    if (match.Success)
                    {
                        verzija = verzija.Replace(match.Value, "");
                    }

                    os = Oprema.IzmjeniTerminal(grad, verzija + match.Value, hostname, terminalid, idAplikacije);
                    Sustav.Aktivnost(grad, idKorisnika, hostname, "v. " + verzija, d.UID, os, d.ObradjujeZahtjeve, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "AKTIVNOST MOBILE");
            }
        }

        public _Grad AktivacijaAplikacije(string aktivacijskiKod)
        {
            return Sustav.AktivacijaAplikacije(aktivacijskiKod, idAplikacije);
        }

        public List<_Printer> Printeri(string grad, int idRedarstva)
        {
            return Postavke.DohvatiPrintere(grad, false, idRedarstva, idAplikacije);
        }

        public List<_Drzava> Drzave()
        {
            return Sustav.Drzave(idAplikacije);
        }

        public List<_Zakon> DohvatiZakone(string grad, bool neaktivni)
        {
            return Zakoni.DohvatiZakoneS(grad, neaktivni, 1, false, idAplikacije);
        }

        public _Operater Djelatnik(string grad, string uid, string lozinka)
        {
            return Korisnici.PrijavaMobile(grad, uid, lozinka, idAplikacije);
        }

        public bool IzmjeniLozinku(string grad, int idKorisnika, string lozinka)
        {
            return Korisnici.IzmjeniZaporku(grad, idKorisnika, lozinka, true, idAplikacije);
        }

        public bool SpremiLog(string grad, DateTime vrijeme, string naziv, string uid, string service, string info)
        {
            return Postavke.SpremiLog(grad, vrijeme, naziv, uid, service, info, idAplikacije);
        }

        public _2DLista Terminal(string grad, string ib)
        {
            return Postavke.Terminal(grad, ib, idAplikacije);
        }

        public void SpremiError(string grad, string greska, string napomena, string korisnik)
        {
            Sustav.SpremiGresku(grad, new Exception(greska), idAplikacije, napomena, korisnik);
        }

        public void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena)
        {
            Sustav.SpremiAkciju(grad, idKorisnika, idAkcije, napomena, 1, idAplikacije);
        }

        /*:: OPREMA ::*/

        public void DodajBiljesku(string id, int idStatusa, string napomena)
        {
            Oprema.DodajBiljesku(id, idStatusa, napomena, idAplikacije);
        }

        public _Oprema Terminal(string id)
        {
            return Oprema.Terminal(id);
        }

        /*:: POSEBNA VOZILA ::*/

        public bool? PosebnoVozilo(string grad, string registracija)
        {
            return Postavke.PosebnoVozilo(grad, registracija, idAplikacije);
        }

        public string PostojiOdobrenje(string grad, string registracija)
        {
            return Postavke.PostojiOdobrenje(grad, registracija, 1, idAplikacije);
        }

        public _Odobrenja Odobrenje(string grad, string registracija, int idRedarstva)
        {
            return Postavke.Odobrenje(grad, registracija, idRedarstva, idAplikacije);
        }

        /*:: PREKRŠAJ ::*/

        public bool SpremiPrekrsajNovo(string grad, _Lokacija lokacija, ref _NoviPrekrsaj prekrsaj)
        {
            return Mobile.SpremiPrekrsajNovo(grad, lokacija, ref prekrsaj, 1, idAplikacije);
        }

        public int SpremiLokaciju(string grad, _Lokacija lokacija, bool pauk)
        {
            return Mobile.SpremiLokaciju(grad, lokacija, pauk, idAplikacije);
        }

        public bool SpremiFotografiju(string grad, int idLokacije, byte[] fotografija)
        {
            return Mobile.SpremiFotografiju(grad, idLokacije, fotografija, idAplikacije);
        }

        public int SpremiPrekrsaj(string grad, _NoviPrekrsaj prekrsaj)
        {
            return Mobile.NoviPrekrsaj(grad, prekrsaj, 1, idAplikacije);
        }

        public int PosaljiNalogPauku(string grad, int idPrekrsaja, DateTime datum, bool lisice)
        {
            return Mobile.PosaljiNalogPauku(grad, idPrekrsaja, datum, lisice, idAplikacije);
        }

        public _LokacijaPrekrsaja LokacijaPrekrsaja(string grad, int idLokacije)
        {
            return Mobile.LokacijaPrekrsaja(grad, idLokacije, idAplikacije);
        }

        public bool? IskoristenaUplatnica(string grad, string poziv)
        {
            return Mobile.IskoristenaUplatnica(grad, poziv, idAplikacije);
        }

        public bool? IskoristenaUplatnicaRacun(string grad, string poziv)
        {
            return Mobile.IskoristenaUplatnicaRacun(grad, poziv, idAplikacije);
        }

        public bool NoviNalog(string grad, int idNaloga, DateTime vrijeme, bool lisice, int idLokacije)
        {
            return Nalog.NoviNalog(grad, idNaloga, vrijeme, idLokacije, lisice, idAplikacije);
        }

        public _Prekrsaj GetPrekrsaj(string grad, int idLokacije)
        {
            return Prekrsaj.DetaljiPrekrsaja(grad, idLokacije, idAplikacije);
        }

        public _Prekrsaj GetPrekrsajBroj(string grad, string broj)
        {
            return Prekrsaj.DetaljiPrekrsajaBroj(grad, broj, idAplikacije);
        }

        /*:: ZAHTJEV ZA PODIZANJEM ::*/

        #region ZAHTJEV

        public int NoviZahtjev(string grad, _Zahtjev zahtjev, out bool aktivan)
        {
            return Mobile.NoviZahtjev(grad, zahtjev, out aktivan, idAplikacije);
        }

        public bool StatusZahtjeva(string grad, int idZahtjeva, ref int idStatusa, out string poruka, out int? idNaloga, out decimal kazna, out bool obavijest)
        {
            return Mobile.StatusZahtjeva(grad, idZahtjeva, ref idStatusa, out poruka, out idNaloga, out kazna, out obavijest, idAplikacije);
        }

        public _Operater PreuzeoZahtjev(string grad, int idZahtjeva, out string aplikacija)
        {
            return Mobile.PreuzeoZahtjev(grad, idZahtjeva, out aplikacija, idAplikacije);
        }

        public bool AktivniKorisnik(string grad)
        {
            return Mobile.AktivniKorisnik(grad, idAplikacije);
        }

        public int IDVozila(string grad, int idTerminala, out string vozilo)
        {
            return Mobile.IDVozila(grad, idTerminala, out vozilo, idAplikacije);
        }

        #endregion

        /*:: STATUS KAŽNJAVANJA VOZILA ::*/

        public List<_Kaznjavan> Kaznjavan(string grad, string registracija, string drzava, int dana)
        {
            if (registracija.ToUpper() == "IZVID")
            {
                return new List<_Kaznjavan>();
            }

            return Mobile.Kaznjavan(grad, registracija, drzava, dana, true, idAplikacije);
        }

        /*:: PAUK ::*/

        #region PAUK

        public string NazivVozila(string grad, int idTerminala)
        {
            return Spider.NazivVozila(grad, idTerminala, idAplikacije);
        }

        public _Vozilo Vozilo(string grad, int idTerminala)
        {
            return Spider.Vozilo(grad, idTerminala, idAplikacije);
        }

        public List<_2DLista> Razlozi(string grad, int idStatusa)
        {
            return Spider.Razlozi(grad, idStatusa, idAplikacije);
        }

        public int SpremiLokacijuPauka(string grad, _PozicijaPauka pozicija)
        {
            return Spider.SpremiLokacijuPauka(grad, pozicija, idAplikacije);
        }

        public List<_PozicijaDjelatnika> PozicijeRedara(string grad, int minuta)
        {
            return Mapa.PozicijeRedaraMobile(grad, minuta, idAplikacije);
        }

        public int DohvatiIDNaloga(string grad, int idTerminala)
        {
            return Nalog.DohvatiIDNaloga(grad, idTerminala, idAplikacije);
        }

        public _NalogMobile DetaljiNaloga(string grad, int idNaloga)
        {
            return Nalog.DetaljiNalogaMobile(grad, idNaloga, idAplikacije);
        }

        public List<_CentralnaLokacija> DohvatiCentralnuLokaciju(string grad, int idTerminala)
        {
            return Spider.DohvatiCentralnuLokaciju(grad, idTerminala, idAplikacije);
        }

        public List<_NalogMobile> DohvatiNalogeDjeatnika(string grad, int idDjelatnika)
        {
            return Nalog.DohvatiNalogeDjeatnikaMobile(grad, idDjelatnika, idAplikacije);
        }

        public bool StornoRedara(string grad, int idNaloga)
        {
            return Nalog.Storniraj(grad, idNaloga, null, true, "", idAplikacije);
        }

        //todo obrisi
        public int? DohvatiNoviBrojRacuna(string grad, int idNaloga, out decimal? iznos)
        {
            iznos = 0;
            return -1;
            //return Financije.NoviBrojRacuna(grad, idNaloga, out iznos, idAplikacije);
        }

        public void IzmjeniStatus(string grad, int idNaloga, int idTerminala, int idStatusa, int? idRazloga, DateTime datum, decimal lat, decimal lng, string adresa, decimal brzina,
            int idStatusaLokacije, int idCentralneLokacije)
        {
            Nalog.IzmjeniStatusNaloga(grad, idNaloga, idTerminala, null, idStatusa, idRazloga, datum, lat, lng, adresa, brzina, idStatusaLokacije, idCentralneLokacije, idAplikacije);
        }

        public void IzmjeniStatusNaloga(string grad, int idNaloga, int idTerminala, int? idDjelatnika, int idStatusa, int? idRazloga, DateTime datum, decimal lat, decimal lng, string adresa, decimal brzina,
            int idStatusaLokacije, int idCentralneLokacije)
        {
            Nalog.IzmjeniStatusNaloga(grad, idNaloga, idTerminala, idDjelatnika, idStatusa, idRazloga, datum, lat, lng, adresa, brzina, idStatusaLokacije, idCentralneLokacije, idAplikacije);
        }

        public int NoviID(string grad)
        {
            return Nalog.NoviID(grad, idAplikacije);
        }

        public int ProvijeriNaplativost(string grad, string brojObavijesti, out string poruka)
        {
            return Nalog.ProvijeriNaplativost(grad, brojObavijesti, out poruka, idAplikacije);
        }

        public _Zakon DetaljiPrekrsaja(string grad, int idOpisa)
        {
            return Zakoni.DohvatiZakonS(grad, idOpisa, false, idAplikacije);
        }

        //todo obrisi
        public bool NaplatiPauka(string grad, int idNaloga, int idTerminala, int brojRacuna, DateTime datum, decimal lat, decimal lng, string adresa, decimal brzina,
            int idStatusaLokacije, int idCentralneLokacije)
        {
            return false;
            //return Nalog.Naplati(grad, idNaloga, idTerminala, brojRacuna, datum, lat, lng, adresa, brzina, idStatusaLokacije, idCentralneLokacije, idAplikacije);
        }

        #endregion

        /*:: LOKACIJA ::*/

        public List<_DetaljiLokacije> DohvatiAdresu(double lat, double lng)
        {
            return Geocoordinates.DohvatiAdresu(lat, lng);
        }

        public bool NaplatiPrekrsaj(string grad, int idPrekrsaja, int idRedara, int idVrstePlacanja, out string poruka)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    //Prekrsaji pr = db.Prekrsajis.Single(i => i.IDPrekrsaja == idPrekrsaja);
                    //pr.Placeno = true;
                    //pr.Poslano = true;
                    //pr.IDRedarNaplate = idRedara;
                    //pr.IDVrstaPlacanja = idVrstePlacanja;

                    //db.SubmitChanges();

                    poruka = "";
                    return false;
                }
            }
            catch (Exception ex)
            {
                poruka = "Greška!!  " + ex.Message;
                return false;
            }
        }

        public bool DodajLokalnuAdresu(string grad, string adresa, string broj, decimal? hdop, decimal lat, decimal lng)
        {
            return Sustav.DodajLokalnuAdresu(grad, adresa, broj, hdop, lat, lng, idAplikacije);
        }

        /*:: ISPIS ::*/

        public bool IspisPredloska(string grad, string detaljiKazne, int qty, int idPredloska, int idJezika, out string Ispis)
        {
            return resources.podaci.upiti.Ispis.IspisPredloska(grad, detaljiKazne, qty, idPredloska, idJezika, out Ispis, false, idAplikacije);
        }

        public string IspisObavijestiPauk(string grad, int idLokacije, string broj)
        {
            return Ispis.IspisObavijestiPauk(grad, idLokacije, broj, idAplikacije);
        }

        public string IspisKopijeRacuna(string grad, int idRacuna)
        {
            return Naplata.IspisKopijeRacuna(grad, idRacuna, idAplikacije);
        }

        public string IspisKopijeRacunaBroj(string grad, string broj)
        {
            return Naplata.IspisKopijeRacunaBroj(grad, broj, idAplikacije);
        }

        public string IzvjestajSmjene(string grad, int idDjelatnika)
        {
            return Ispis.IzvjestajSmjene(grad, idDjelatnika, idAplikacije);
        }

        /*:: AUTO PILOT ::*/

        #region Auto Pilot

        public bool PromijeniStatusVozila(string grad, int idTerminala, bool aktivan)
        {
            return AutopilotPauk.PromijeniStatusVozila(grad, idTerminala, aktivan, idAplikacije);
        }

        public List<_NaloziPauku> IzdaniNalozi(string grad, DateTime datum, bool novi)
        {
            return AutopilotPauk.IzdaniNalozi(grad, datum, novi, idAplikacije);
        }

        public List<_StatusVozila> StatusVozila(string grad)
        {
            return AutopilotPauk.StatusVozila(grad, idAplikacije);
        }

        public bool UkljuciAutoPilot(string grad, bool ukljucen)
        {
            return AutopilotPauk.UkljuciAutoPilot(grad, ukljucen, idAplikacije);
        }

        public bool MozePrimitiNalog(string grad, int idVozila, int idNaloga)
        {
            return AutopilotPauk.MozePrimitiNalog(grad, idVozila, idNaloga, idAplikacije);
        }

        public bool SetAutopilotID(string grad, string ID, bool force, out string autopilotID)
        {
            return AutopilotPauk.SetAutopilotID(grad, ID, force, out autopilotID, idAplikacije);
        }

        public bool Autopilot(string grad)
        {
            return AutopilotPauk.Autopilot(grad, idAplikacije);
        }

        public void DodjeliPauku(string grad, int idNaloga, int idVozila)
        {
            Nalog.Dodjeli(grad, idNaloga, idVozila, idAplikacije);
            Sustav.SpremiAkciju(grad, -1, 85, "IDVozila: " + idVozila, 2, idAplikacije);
        }

        public bool PrivremenaObustava(string grad, int idVozila, bool obustavi)
        {
            return AutopilotPauk.PrivremenaObustava(grad, idVozila, obustavi, idAplikacije);
        }

        public DateTime? AutopilotUgasen(string grad)
        {
            return AutopilotPauk.AutopilotUgasen(grad, idAplikacije);
        }

        #endregion

        /*:: NAPLATA ::*/

        public List<_VrstaPlacanja> VrstePlacanja(string grad, int? idStatusa)
        {
            return Naplata.VrstePlacanja(grad, idStatusa, 2, idAplikacije);
        }

        public string Naplati(string grad, int idNaloga, int idStatusa, int idDjelatnika, int idVrstePlacanja, _Osoba osoba, decimal iznos, string poziv, out int idRacuna)
        {
            try
            {
                string vrsta = Naplata.VrstaPlacanja(grad, idVrstePlacanja, idAplikacije);
                _PoslovniProstor pp = PoslovniProstor.DohvatiPoslovniProstor(grad, 2, idAplikacije);
                _Djelatnik djel = Korisnici.DohvatiDjelatnika(grad, idDjelatnika, idAplikacije);

                decimal osnovica = Math.Round(iznos / ((decimal)(100 + pp.PDV) / 100), 2);
                decimal pdv = Math.Round(osnovica * pp.PDV / 100, 2);
                osnovica = iznos - pdv;
                decimal ukupno = iznos;

                List<_Stavka> stavke = new List<_Stavka>();
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RACUNI_STAVKE_OPI st = db.RACUNI_STAVKE_OPIs.First(i => i.IDStatusa == 3 && i.IDRedarstva == 2 && i.Obrisan == false);
                    _Stavka nova = new _Stavka(0, 0, st.IDOpisaStavke, st.NazivOpisaStavke, st.Lezarina, 1, iznos, pdv, osnovica, ukupno, pp.PDV, "");
                    stavke.Add(nova);

                    List<_Osoba> osobe = new List<_Osoba>();

                    if (!string.IsNullOrEmpty(osoba.Ime) || !string.IsNullOrEmpty(osoba.Prezime) || !string.IsNullOrEmpty(osoba.BrojDokumenta) || !string.IsNullOrEmpty(osoba.OIB) || !string.IsNullOrEmpty(osoba.Napomena) || !string.IsNullOrEmpty(osoba.Mjesto) || !string.IsNullOrEmpty(osoba.Posta) || !string.IsNullOrEmpty(osoba.Ulica))
                    {
                        osobe.Add(osoba);
                    }

                    string oznakaPP;
                    int blagajna = Naplata.Blagajna(grad, idNaloga, out oznakaPP, idAplikacije); //definirano na vozilu 

                    if (string.IsNullOrEmpty(oznakaPP))
                    {
                        oznakaPP = pp.Oznaka;
                    }

                    _Racun novi = new _Racun(0, idNaloga, null, idVrstePlacanja, null, null, vrsta, "", idDjelatnika, djel.ImeNaRacunu, 2, DateTime.Now, 0, 0, pdv, osnovica, ukupno,
                        pp.PDV, djel.OIB ?? "", blagajna == -1 ? 1 : blagajna, "", false, "", "", true, "", "", "", DateTime.Now, oznakaPP, poziv, "", "", "", false, false, false, false, "", stavke, osobe);

                    string brrac, pozivna;
                    return Naplata.NaplatiPauk(grad, novi, idStatusa, out idRacuna, out brrac, out pozivna, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATA S UREĐAJA");
                idRacuna = -1;
                return "";
            }
        }

        public bool SpremiDokument(string grad, int idRacuna, byte[] dokument)
        {
            return Osobe.SpremiDokument(grad, idRacuna, dokument, idAplikacije);
        }

        /*:: NEOČITANA REGISTRACIJA ::*/

        public bool NeocitanaRegistracija(string grad, _Neocitana registracija)
        {
            return Mobile.NeocitanaRegistracija(grad, registracija, idAplikacije);
        }

        public int DodanoLokacija(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Mobile.DodanoLokacija(grad, datumOd, datumDo, idAplikacije);
        }

        public int DodanoPrekrsaja(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Mobile.DodanoPrekrsaja(grad, datumOd, datumDo, idAplikacije);
        }

        /*:: OBRADA ZAHTJEVA ::*/

        #region OBRADA ZAHTJEVA

        /// <summary>
        /// Vraća broj neobrađenih zahtjeva i zahtjev koji je prvi na redu za preuzimanje
        /// </summary>
        /// <param name="grad">grad obrade</param>
        /// <param name="idDjelatnika">id prijavljenog djelatnika</param>
        /// <param name="uid">korisnik</param>
        /// <param name="prijava">detalji zahtjeva koji je prvi na redu za obradu</param>
        /// <returns>Broj neobrađenih zahtjeva</returns>
        public int Neobradjeni(string grad, int idDjelatnika, string uid, out _PrijavaPauk prijava)
        {
            if (!Korisnici.MozeObradjivati(grad, idDjelatnika, idAplikacije))
            {
                prijava = null;
                return -1;
            }

            using (PostavkeDataContext db = new PostavkeDataContext())
            {
                int zadrska = db.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad)).Zadrska;
                return Zahtjev.Neobradjeni(grad, idDjelatnika, zadrska, out prijava, idAplikacije);
            }
        }

        /// <summary>
        /// Slike zahtjeva za podizanjem
        /// </summary>
        /// <param name="grad">grad obrade</param>
        /// <param name="idLokacije">id lokacije preuzetog zahtjeva</param>
        /// <returns>Listu slika zahtjeva</returns>
        public List<_Slika> Slike(string grad, int idLokacije)
        {
            return Prekrsaj.SlikePrekrsaja(grad, idLokacije, idAplikacije);
        }

        /// <summary>
        /// Metoda za preuzimanje otvorenog zahtjeva
        /// </summary>
        /// <param name="grad">grad obrade</param>
        /// <param name="idZahtjeva">id zahtjeva koji se preuzima</param>
        /// <param name="idDjelatnika">iddjelatnika koji preuzima</param>
        /// <returns>True ako je uspješno preuzet, false ako je netko u međuvremenu preuzeo</returns>
        public bool Preuzmi(string grad, int idZahtjeva, int idDjelatnika)
        {
            return Zahtjev.Preuzmi(grad, idZahtjeva, idDjelatnika, idAplikacije);
        }

        /// <summary>
        /// Odustajanje od obrade zahtjeva (više neće dolaziti na obradu djelatniku koji je odustao)
        /// </summary>
        /// <param name="grad">grad obrade</param>
        /// <param name="idZahtjeva">id zahtjeva koji se obrađuje</param>
        /// <param name="idDjelatnika">id djelatnika koji obrađuje</param>
        public void Odustani(string grad, int idZahtjeva, int idDjelatnika)
        {
            Zahtjev.Odustani(grad, idZahtjeva, idDjelatnika, idAplikacije);
        }

        /// <summary>
        /// Kreira novi prekršaj i šalje nalog pauku
        /// </summary>
        /// <param name="grad">grad obrade</param>
        /// <param name="zahtjev">id zahtjeva koji se obrađuje</param>
        /// <param name="idOpisa">id opisa prekršaja</param>
        /// <param name="registracija">registracija</param>
        /// <param name="adresa">adresa prekršaja</param>
        /// <param name="drzava">država vozila prekršitelja</param>
        /// <param name="obavijest">true obavijest/false upozorenje</param>
        /// <param name="nalogPauku">true ide nalog pauku false ne ide</param>
        /// <returns>id novog prekršaja</returns>
        public int DodajPrekrsaj(string grad, _PrijavaPauk zahtjev, int idOpisa, string registracija, string adresa, string drzava, bool obavijest, bool nalogPauku)
        {
            try
            {
                decimal kazna = 0;

                try
                {
                    kazna = Zakoni.DohvatiZakonS(grad, idOpisa, false, idAplikacije).Kazna;
                }
                catch (Exception)
                {

                }

                //todo lisice
                int id = Zahtjev.DodajPrekrsaj(grad, zahtjev, idOpisa, kazna, registracija, adresa, drzava, obavijest, nalogPauku, false, 1, idAplikacije);
                try
                {
                    Sustav.SpremiAkciju(grad, zahtjev.IDOdobravatelja.Value, 103, "ID Zahtjeva: " + zahtjev.IDPrijave + ", ID Prekrsaja: " + id + ", Reg.: " + zahtjev.Registracija, 1, idAplikacije);
                }
                catch
                {
                }

                return id;

            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DODAJ PREKRSAJ - IZNOS KAZNE");
                return -1;
            }
        }

        /// <summary>
        /// Provjerava da li je zahtjev još uvijek aktivan
        /// </summary>
        /// <param name="grad">grad obrade</param>
        /// <param name="idZahtjeva">id zahtjeva koji se obrađuje</param>
        /// <returns>vraća true ako je pauk odustao od zahtjeva za vrijeme obrade</returns>
        public bool StatusZahtjevaPaukOdustao(string grad, int idZahtjeva)
        {
            return Zahtjev.StatusZahtjevaPaukOdustao(grad, idZahtjeva, idAplikacije);
        }

        public bool Odbij(string grad, int idZahtjeva, int idDjelatnika, string razlog)
        {
            try
            {
                Sustav.SpremiAkciju(grad, idDjelatnika, 222, "ID Zahtjeva: " + idZahtjeva, 2, idAplikacije);
            }
            catch
            {
            }

            return Zahtjev.Zatvori(grad, idZahtjeva, 2, idDjelatnika, null, null, razlog, 1, idAplikacije);
        }

        #endregion

        /*:: TERMINALI ::*/

        #region TERMINALI

        public _Terminal GetTerminala(string grad, string deviceId)
        {
            return Postavke.GetTerminala(grad, deviceId, idAplikacije);
        }

        public int InsertTerminal(string grad, string deviceId, string naziv)
        {
            try
            {
                _Terminal ter = new _Terminal(0, null, "", deviceId, naziv, "", null, false, false, false, false, false, true, false, DateTime.Now);
                return Postavke.DodajTerminalS(grad, ter, idAplikacije);
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "INSERT TERMINAL");
                return -1;
            }
        }

        public bool ResetTerminals(string grad, string deviceId, int command)
        {
            return Postavke.ResetTerminals(grad, deviceId, command, idAplikacije);
        }

        public string GetParametriTerminala(string grad, string deviceId, string naziv, bool startUp)
        {
            return Postavke.GetParametriTerminala(grad, deviceId, naziv, startUp, idAplikacije);
        }

        public int? SetTerminalAccessTime(string grad, string deviceId)
        {
            return Postavke.SetTerminalAccessTime(grad, deviceId, idAplikacije);
        }

        public bool ClearTerminalStatus(string grad, string deviceId)
        {
            return Postavke.ClearTerminalStatus(grad, deviceId, idAplikacije);
        }

        public void UpdateVerzija(string grad, string progVer, string romVer, string deviceId)//todo , bool pauk)
        {
            Postavke.UpdateVerzija(grad, progVer, romVer, deviceId, false, idAplikacije);
        }

        #endregion

        /*:: PORUKE ::*/

        public _Poruka DohvatiPoruku(string grad, int idDjelatnika, out int neprocitanih)
        {
            return Chat.DohvatiPoruku(grad, idDjelatnika, out neprocitanih, idAplikacije);
        }

        public bool ProcitaoPoruku(string grad, int idDjelatnika, int idPoruke)
        {
            return Chat.ProcitaoPoruku(grad, idDjelatnika, idPoruke, idAplikacije);
        }

        /*:: LISICE ::*/

        public List<_Prekrsaj> PretragaBlokiranih(string grad)
        {
            return Pretraga.PretragaNalogaZaNaplatu(grad, 22, false, idAplikacije);
        }

        public string DeblokirajVozilo(string grad, int idNaloga, int idDjelatnika, int idVrstePlacanja, _Osoba osoba, string poziv, out int idRacuna)
        {
            try
            {
                string vrsta = Naplata.VrstaPlacanja(grad, idVrstePlacanja, idAplikacije);
                _PoslovniProstor pp = PoslovniProstor.DohvatiPoslovniProstor(grad, 2, idAplikacije);

                _Djelatnik djel = Korisnici.DohvatiDjelatnika(grad, idDjelatnika, idAplikacije);

                List<_Stavka> stavke = new List<_Stavka>();
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RACUNI_STAVKE_OPI st = db.RACUNI_STAVKE_OPIs.First(i => i.IDStatusa == 22 && i.IDRedarstva == 2 && i.Obrisan == false);
                    RACUNI_STAVKE_OPI stl = db.RACUNI_STAVKE_OPIs.First(i => i.IDStatusa == 22 && i.IDRedarstva == 2 && i.Obrisan == false && i.Lezarina);

                    Pauk np = db.Pauks.First(i => i.IDNaloga == idNaloga);

                    int kolicina = DateTime.Today.Date.Subtract(np.DatumDeponija.Value.Date).Days;

                    decimal osnovicaD = Math.Round((decimal) (st.Iznos / ((decimal)(100 + pp.PDV) / 100)), 2);
                    decimal pdvD = Math.Round(osnovicaD * pp.PDV / 100, 2);
                    osnovicaD = (decimal) (st.Iznos - pdvD);
                    decimal ukupnoD = (decimal) st.Iznos;

                    decimal osnovicaL = Math.Round((decimal) (stl.Iznos / ((decimal)(100 + pp.PDV) / 100)), 2) * kolicina;
                    decimal pdvL = Math.Round(osnovicaL * pp.PDV / 100, 2);
                    osnovicaL = (decimal) (stl.Iznos * kolicina - pdvL);
                    decimal ukupnoL = (decimal) stl.Iznos * kolicina;

                    _Stavka nova = new _Stavka(0, 0, st.IDOpisaStavke, st.NazivOpisaStavke, st.Lezarina, 1, (decimal)st.Iznos, pdvD, osnovicaD, ukupnoD, pp.PDV, "");
                    _Stavka novaL = new _Stavka(0, 0, stl.IDOpisaStavke, stl.NazivOpisaStavke, stl.Lezarina, kolicina, (decimal)stl.Iznos, pdvL, osnovicaL, ukupnoL, pp.PDV, "");
                   
                    stavke.Add(nova);
                    
                    if (kolicina > 0)
                    {
                        stavke.Add(novaL);
                    }

                    List<_Osoba> osobe = new List<_Osoba>();
                    osobe.Add(osoba);

                    string oznakaPP;
                    int blagajna = Naplata.Blagajna(grad, idNaloga, out oznakaPP, idAplikacije); //definirano na vozilu 
                    if (string.IsNullOrEmpty(oznakaPP))
                    {
                        oznakaPP = pp.Oznaka;
                    }

                    _Racun novi = new _Racun(0, idNaloga, null, idVrstePlacanja, null, null, vrsta, "", idDjelatnika, djel.ImeNaRacunu, 2, DateTime.Now, 0, 0, pdvD + pdvL, osnovicaD + osnovicaL, ukupnoD + ukupnoL,
                        pp.PDV, djel.OIB ?? "", blagajna == -1 ? 1 : blagajna, "", false, "", "", true, "", "", "", DateTime.Now, oznakaPP, poziv, "", "", "", false, false, false, false, "", stavke, osobe);

                    string brrac, pozivna;
                    return Naplata.NaplatiPauk(grad, novi, 22, out idRacuna, out brrac, out pozivna, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATA S UREĐAJA");
                idRacuna = -1;
                return "";
            }
        }

        public decimal IznosDeblokade(string grad, int idNaloga)
        {
            try
            {

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RACUNI_STAVKE_OPI st = db.RACUNI_STAVKE_OPIs.First(i => i.IDStatusa == 22 && i.IDRedarstva == 2 && i.Obrisan == false);
                    RACUNI_STAVKE_OPI stl = db.RACUNI_STAVKE_OPIs.First(i => i.IDStatusa == 22 && i.IDRedarstva == 2 && i.Obrisan == false && i.Lezarina);//ležarina

                    Pauk np = db.Pauks.First(i => i.IDNaloga == idNaloga);

                    int kolicina = DateTime.Now.Subtract(np.DatumDeponija.Value).Days;//todo dana ležarine

                    decimal ukupno = (decimal)(st.Iznos + Math.Round((decimal)stl.Iznos * kolicina, 2));

                    return ukupno;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZNOS DEBLOKADE, IDNaloga: " + idNaloga);
                return 0;
            }
        }

        /*:: MOBILE ::*/

        //android
        public bool NoviZahtjevM(string grad, decimal lat, decimal lng, string adresa, List<byte[]> slike)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Lokacije lp = new Lokacije();

                    lp.Lat = lat;
                    lp.Long = lng;
                    lp.RegistracijskaPlocica = "???";
                    lp.DatumVrijeme = DateTime.Now;
                    lp.IDDjelatnika = null;
                    lp.IDNacinaPozicioniranja = 4;
                    lp.IDTerminala = 0;
                    lp.CellTowerID = null;
                    lp.SignalStrength = null;
                    lp.HDOP = 0;
                    lp.Brzina = 0;
                    lp.GPSAcc = 0;
                    lp.Battery = 0;

                    db.Lokacijes.InsertOnSubmit(lp);
                    db.SubmitChanges();

                    int idlokacije = lp.IDLokacije;

                    foreach (var s in slike)
                    {
                        SpremiFotografiju(grad, idlokacije, s);
                    }

                    Zahtjevi pri = new Zahtjevi();

                    pri.IDLokacije = idlokacije;
                    pri.IDPrijavitelja = null;
                    pri.DatumVrijeme = DateTime.Now;
                    pri.Registracija = "???";
                    pri.Lat = lat;
                    pri.Lng = lng;
                    pri.Adresa = adresa;
                    pri.KraticaDrzave = "??";
                    pri.IDStatusa = 0;

                    db.Zahtjevis.InsertOnSubmit(pri);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POŠALJI ZAHTJEV MOBILE");
                return false;
            }
        }

        /*:: PARKING ::*/

        public bool SpremiOpazanje(string grad, ref _Opazanje opazanje, List<byte[]> slike)
        {
            return Parking.Spremi(grad, ref opazanje, slike, idAplikacije);
        }

        public List<_Opazanje> TraziOpazanje(string grad, string registracija, bool samoopazanja, int? idDjelatnika, int? idSektora)
        {
            return Parking.TraziOpazanje(grad, registracija, samoopazanja, idDjelatnika, idSektora, idAplikacije);
        }

        public List<_Zone> Zone(string grad)
        {
            return Parking.Zone(grad, idAplikacije);
        }

        public List<_Sektori> Sektori(string grad)
        {
            return Parking.Sektori(grad, idAplikacije);
        }
    }
}