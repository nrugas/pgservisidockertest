using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Linq;

namespace PG.Servisi
{
    [ServiceContract]
    public interface IPGParking
    {
        [OperationContract]
        bool Dostupan();

        [OperationContract]
        _Grad Aktivacija(string aktivacijskiKod);

        [OperationContract]
        List<_2DLista> Aplikacije();

        [OperationContract]
        List<_2DLista> PopisGradova();

        [OperationContract]
        List<_3DLista> Redari(string grad);

        [OperationContract]
        List<_2DLista> Redarstva();

        [OperationContract]
        List<_Drzava> Drzave();

        [OperationContract]
        List<_2DLista> PopisPredlozaka(string grad);

        [OperationContract]
        _Grad Grad(string grad);

        [OperationContract]
        bool PostavkeGrada(string grad, _Grad postavke);

        [OperationContract]
        bool IzmjeniMapu(string grad, string mapa);

        /*:: NALOG ZA PLACANJE ::*/

        #region NALOG

        [OperationContract]
        _Uplatnica Uplatnica(string grad, int idRedarstva);

        [OperationContract]
        int IzmjeniUplatnicu(string grad, _Uplatnica nalog);

        #endregion

        /*:: AKTIVNOST ::*/

        [OperationContract]
        bool Aktivan(string grad, string korisnik, string racunalo, string verzija, string os, int idKorisnika);

        [OperationContract]
        bool TrenutnoAktivan(string grad, int idKorisnika);

        [OperationContract]
        List<_AktivneAplikacije> DohvatiAktivne(string grad, bool aktivni);

        [OperationContract]
        bool ObrisiAktivnost(string grad, int idAktivnosti);

        [OperationContract]
        void Reset(string grad, int idAktivnosti);

        /*:: AKCIJE KORISNIKA ::*/

        #region AKCIJE KORISNIKA

        [OperationContract]
        void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena);

        [OperationContract]
        List<_Akcija> DohvatiAkcije(string grad, int? idDjelatnika, int? idAkcije, DateTime? odDatum, DateTime? doDatum, int idprivilegije, int? idRedarstva);

        [OperationContract]
        List<_2DLista> DohvatiVrsteAkcija(string grad);

        [OperationContract]
        bool ObrisiAkciju(string grad, int idAkcije);

        #endregion

        /*:: PODRSKA ::*/

        #region PODRSKA

        [OperationContract]
        bool PrijaviProblem(string grad, _Problem problem, List<byte[]> slike);

        [OperationContract]
        bool NaruciMaterijal(string grad, _Narudzba narudzba);

        [OperationContract]
        bool PostaviPitanje(string grad, int idKorisnika, int idPodrucja, string poruka, List<byte[]> slike);

        [OperationContract]
        List<_Prekrsaj> PrekrsajiIzvoz(string grad, int idDjelatnika, int idPredloska, DateTime? datumOd,
            DateTime? datumDo, bool storno);

        #endregion

        #region KONTAKTIRANJE KORISNIKA

        [OperationContract]
        bool DodajPredlozakEmaila(string grad, string naziv, string predlozak);

        [OperationContract]
        bool ObrisiPredlozakEmaila(string grad, int idPredloska);

        [OperationContract]
        List<_3DLista> DohvatiPredloskeEmaila(string grad);

        [OperationContract]
        _EmailPostavke PostavkeEmaila(string grad);

        #endregion

        /*:: DJELATNICI ::*/

        #region DJELATNICI

        [OperationContract]
        bool PosaljiEmailDdobrodoslice(string grad, int idKorisnika);

        [OperationContract]
        int DodajNovogDjelatnika(string grad, _Djelatnik korisnik, bool email);

        [OperationContract]
        bool ObrisiDjelatnika(string grad, int idKorisnika);

        [OperationContract]
        bool IzmjeniDjelatnika(string grad, _Djelatnik korisnik);

        [OperationContract]
        List<_Djelatnik> DohvatiDjelatnike(string grad);

        [OperationContract]
        List<_Chat> DohvatiDjelatnikeChat(string grad, int idDjelatnika);

        [OperationContract]
        List<_2DLista> DohvatiPrivilegije(string grad);

        [OperationContract]
        bool IzmjeniZaporku(string grad, int idKorisnika, string zaporka);

        [OperationContract]
        bool IzmjeniOtisak(string grad, int idKorisnika, byte[] otisak);

        [OperationContract]
        bool IzmjeniGradoveDjelatnika(string grad, int idKorisnika, List<int> gradovi);

        //[OperationContract]
        //void Kontakti();

        [OperationContract]
        List<_Kontakt> DohvatiKontakte(string grad);

        #endregion

        #region PRIVILEGIJE

        [OperationContract]
        List<_Privilegije> Privilegije(string grad);

        [OperationContract]
        List<_Dozvola> DetaljiPrivilegije(string grad, int idKorisnika, int idPrivilegije);

        [OperationContract]
        bool IzmjeniDetaljePrivilegija(string grad, int idKorisnika, List<int> dodijeljene);

        [OperationContract]
        bool ResetirajPrivilegije(string grad, int idKorisnika);

        #endregion

        #region POSLOVNI SUBJEKTI

        [OperationContract]
        List<_2DLista> DohvatiPopisSubjekta(string grad);

        [OperationContract]
        List<_PoslovniSubjekt> DohvatiPoslovneSubjekte(string grad);

        [OperationContract]
        int DodajPoslovnogSubjekta(string grad, _PoslovniSubjekt subjekt);

        [OperationContract]
        bool IzmjeniPoslovnogSubjekta(string grad, _PoslovniSubjekt subjekt);

        [OperationContract]
        bool ObrisPoslovnogSubjekta(string grad, int idSubjekta);

        #endregion

        /*:: RACUNI ::*/

        #region RACUNI

        /*:: NAPLATA ::*/

        //[OperationContract]
        //string Naplati(string grad, _Racun racun, out int idRacuna, out string poziv);

        [OperationContract]
        List<_Posta> Poste();

        [OperationContract]
        List<_2DLista> Stavka(string grad, int idStatusa);

        [OperationContract]
        string IspisKopijeRacuna(string grad, int idRacuna);

        [OperationContract]
        List<_2DLista> VrstaKartica(string grad);

        [OperationContract]
        List<_2DLista> VrsteBanaka(string grad);

        [OperationContract]
        List<_2DLista> Statusi();

        /*:: VRSTE PLACANJA ::*/

        [OperationContract]
        List<_VrstaPlacanja> VrstePlacanja(string grad);

        [OperationContract]
        List<_VrstaPlacanjaStatus> VrstePlacanjaStatusi(string grad, out bool definiranIznos);

        [OperationContract]
        bool IzmjeniStstusVrstePlacanja(string grad, int idVrste, bool ukljuci);

        /*:: NAPLATA ::*/

        [OperationContract]
        int NoviRacun(string grad, _Racun racun, out string brrac, out string poziv);

        [OperationContract]
        List<_2DLista> StatusiStorna();

        [OperationContract]
        int StornirajRacun(string grad, _Racun racun, int idStatusa, byte[] prilog, string filename, out string brrac);

        [OperationContract]
        byte[] PregledajPrilogStornu(string grad, int idRacuna, out string filename);

        [OperationContract]
        string StornirajKaznu(string grad, int idPrekrsaja, int idDjelatnika, string napomena, int idStatusa);

        [OperationContract]
        _PoslovniProstor DohvatiPoslovniProstor(string grad, int idRedarstva);

        [OperationContract]
        bool? IzmjeniPoslovniProstor(string grad, _PoslovniProstor prostor);

        [OperationContract]
        bool SpremiLogo(string grad, byte[] logo, int idRedarstva);

        [OperationContract]
        bool RegistrirajPoslovniProstor(string grad, _PoslovniProstor prostor);

        [OperationContract]
        bool DodajCertifikat(string grad, string sifra, byte[] certifikat, int idVlasnika);

        /*:: OPISI STAVKI ::*/

        [OperationContract]
        List<_2DLista> StatusiKojiNaplacuju();

        [OperationContract]
        bool? ObrisiOpisStavke(string grad, int idOpisa);

        [OperationContract]
        bool IzmjeniOpisStavke(string grad, _OpisiStavki opis);

        [OperationContract]
        int DodajOpisStavke(string grad, _OpisiStavki opis);

        [OperationContract]
        List<_OpisiStavki> DohvatiOpiseStavki(string grad, int idRedarstva);

        [OperationContract]
        _Racun DohvatiRacun(string grad, int idRacuna);

        [OperationContract]
        List<_Racun> DohvatiPopisRacuna(string grad, DateTime? datumOd, DateTime? datumDo, int idDjelatnika, bool fisk,
            string brrac, string poziv, int idRedarstva);

        [OperationContract]
        List<_Racun> DohvatiPopisRacunaOsoba(string grad, string ime, string prezime, string oib, int idRedarstva);

        /*:: OSOBE ::*/

        [OperationContract]
        bool SpremiOsobe(string grad, List<_Osoba> osobe, int idRacuna);

        [OperationContract]
        List<_Osoba> DohvatiOsobe(string grad, bool oib);

        [OperationContract]
        List<_Dokument> Dokumenti(string grad, int idOsobe);

        [OperationContract]
        bool IzmjeniOsobu(string grad, _Osoba osoba, int idRacuna);

        [OperationContract]
        bool ObrisiOsobu(string grad, int idOsobe);

        [OperationContract]
        _Osoba DohvatiOsobu(string grad, int idOsobe);

        /*:: KARTICE ::*/

        [OperationContract]
        bool IzmjeniKarticuPlacanja(string grad, int idRacuna, int? idBanke, int idKartice, string odobrenje, bool? rate);

        /*:: ZAKLJUCENJE ::*/

        [OperationContract]
        int ZakljuciBlagajnu(string grad, _Zakljucenje zakljucenje);

        [OperationContract]
        bool ProgramskoZakljucivanjeZaostalih(string grad, DateTime datumDo, int idRedarstva);

        [OperationContract]
        List<_Zakljucenje> DohvatiZakljucenja(string grad, DateTime? datumOd, DateTime? datumDo, int idRedarstva);

        [OperationContract]
        List<_Racun> DohvatiPopisRacunaZakljucenja(string grad, int idZakljucenja);

        [OperationContract]
        int Nezakljuceni(string grad, int idRedarstva);

        [OperationContract]
        List<_Racun> DohvatiPopisRacunaZakljucenje(string grad, int? idDjelatnika, string oznaka, int idRedarstva);

        [OperationContract]
        bool ZakljuciZaostatke(string grad, int idRedarstva);

        [OperationContract]
        bool Prenesi(string grad, List<DateTime> datumi);

        /*:: FISKALIZACIJA ::*/

        [OperationContract]
        string PonovnaFiskalizacija(string grad, int idRedarstva);

        [OperationContract]
        string IzvjestajSmjene(string grad, int idKorisnika);

        [OperationContract]
        string IzvjestajZakljucenja(string grad, int idKorisnika, DateTime datum);

        /*:: MUP ::*/

        [OperationContract]
        List<_Osoba> DohvatMUPa(string grad, _Racun racun, int idKorisnika);

        #endregion

        /*:: SUSTAV ::*/

        #region PRINTERI

        [OperationContract]
        List<_Printer> DohvatiPrintere(string grad, bool svi, int idRedarstva);

        [OperationContract]
        bool ObrisiPrinter(string grad, int idPrintera);

        [OperationContract]
        int DodajPrinter(string grad, _Printer printer);

        [OperationContract]
        bool IzmjeniPrinter(string grad, _Printer printer);

        /*:: POVIJEST ::*/

        [OperationContract]
        void PovijestPrintera(int idPrintera, string status, string napomena);

        [OperationContract]
        List<_PovijestOpreme> DohvatiPovijestPrintera(int idPrintera);

        #endregion

        #region TERMINALI

        [OperationContract]
        List<_Terminal> PopisTerminala(string grad, bool neaktivni);

        [OperationContract]
        bool IzmjeniTerminal(string grad, _Terminal terminal);

        [OperationContract]
        bool AkcijeNaTerminalima(string grad, _Terminal terminal);

        #endregion

        #region POSEBNA VOZILA

        [OperationContract]
        List<_Odobrenja> DohvatiOdobrenja(string grad);

        [OperationContract]
        bool ObrisiOdobrenje(string grad, int idOdobrenja);

        [OperationContract]
        int DodajOdobrenje(string grad, _Odobrenja vozilo);

        [OperationContract]
        bool PromijeniStatusOdobrenja(string grad, int idOdobrenja, bool suspendirano);

        [OperationContract]
        bool IzmijeniOdobrenje(string grad, _Odobrenja odobrenje);
        //[OperationContract]
        //bool PokusajKaznjavanja(string grad, string registracija);

        #endregion

        /*:: PREDLOŠCI ::*/

        #region PREDLOŠCI

        [OperationContract]
        List<_2DLista> JeziciPredlozaka(string grad);

        [OperationContract]
        List<_Predlozak> PredlosciIspisa(string grad);

        [OperationContract]
        bool ObrisiPredlozak(string grad, int idPredloska, int idJezika);

        [OperationContract]
        bool IzmijeniPredlozak(string grad, int idPredloska, int idJezika, string nazivPredloska, bool pauk, bool kaznjava, XElement tekstPredloska);

        [OperationContract]
        int DodajPredlozak(string grad, string nazivPredloska, bool pauk, bool kaznjava, XElement tekstPredloska);

        [OperationContract]
        _Predlozak DohvatiPredlozakIspisa(string grad, string koji);

        #endregion

        #region ISPIS PREDLOŽAKA

        //todo naziv
        [OperationContract]
        bool IspisPredloska(string grad, string detaljiKazne, int idPredloska, out string ispis);

        [OperationContract]
        bool IspisPredloska1(string baza, string detaljiKazne, int qty, int idPredloska, int idJezika, out string ispis);

        #endregion

        /*:: ADMINISTRACIJA ULICA ::*/

        #region ADMINISTRACIJA ULICA

        [OperationContract]
        List<_LokalneAdrese> PopisUlica(string grad);

        [OperationContract]
        bool ObrisiLokalnuAdresu(string grad, int idAdrese);

        [OperationContract]
        bool ObrisiGrupuUlica(string grad, string ulica);

        [OperationContract]
        bool IzmjeniLokalnuAdresu(string grad, int idAdrese, string adresa, string kbr);

        [OperationContract]
        bool IzmjeniGrupuUlica(string grad, string ulica, string novaulica);

        #endregion

        /*:: PREKRSAJ ::*/

        #region PREKRSAJ

        //[OperationContract]
        //List<_Prekrsaj> PretraziPrekrsaje(string grad, int idDjelatnika, DateTime datum, bool obavijesti, bool upozorenja);

        //[OperationContract]
        //_Prekrsaj DetaljiPrekrsaja(string grad, int idLokacije);

        [OperationContract]
        _Aktivnost Aktivnost(string grad, DateTime datum);

        /*:: SLIKE ::*/

        [OperationContract]
        List<byte[]> Slike(string grad, int idLokacije);

        [OperationContract]
        List<int> DodajSliku(string grad, int idLokacije, List<byte[]> slike);

        [OperationContract]
        bool ObrisiSliku(string grad, int idSlike);

        [OperationContract]
        List<_Slika> SlikePrekrsaja(string grad, int idLokacije);

        [OperationContract]
        int RotirajSliku(string grad, int idLokacije, byte[] slike);

        /*:: MAPA GLAVNI MENU ::*/

        [OperationContract]
        _Opazanje TraziOpazanje(string grad, int idOpazanja);

        [OperationContract]
        List<_Opazanje> TraziOpazanja(string grad, DateTime datum, string vrijeme, int? idDjelatnika);

        [OperationContract]
        List<_Opazanje> PretraziOpazanja(string grad, int idStatusa, int idDjelatnika, int idSektora,
            int idzone, DateTime? datumOd, DateTime? datumDo, string registracija);

        [OperationContract]
        List<_Tocka> StvaranjePutanjeRedara(string grad, DateTime datum, string vrijeme, int idRedara, int gpsAcc, int speed);

        //[OperationContract]
        //List<_Prekrsaj> PozicijePrekrsaja(string baza, int idZaposlenika, DateTime datum, string vrijeme);

        [OperationContract]
        List<_Pozicija> PozicijeRedara(string baza, int minuta);

        [OperationContract]
        _Pozicija PozicijaOdabranogRedara(string baza, int idDjelatnika);

        /*:: RUČNO DODAVANJE PREKRSAJA ::*/

        [OperationContract]
        List<string> BrojPrekrsaja(string grad, string registracija);

        [OperationContract]
        int DodajRucniPrekrsaj(string grad, _Prekrsaj prekrsaj, bool obavijest, List<byte[]> slike);

        #region AKCIJE

        [OperationContract]
        bool RelokacijaPrekrsaja(string grad, int idPrekrsaja, decimal latitude, decimal longitude);

        [OperationContract]
        bool Registracija(string grad, int idOpazanja, string registracija, string kratica);

        [OperationContract]
        bool Adresa(string grad, int idPrekrsaja, string adresa);

        [OperationContract]
        bool VoziloOtislo(string grad, int idOpazanja);

        #endregion

        #endregion

        /*:: PRETRAGA ::*/

        #region PRETRAGA

        [OperationContract]
        List<_Prekrsaj> ZabiljezeniPrekrsaji(string grad, int idDjelatnika, int idPredloska, DateTime? datumOd, DateTime? datumDo, bool pauk,
            bool registracija, bool dokument, bool ulica, bool storno, char? tipStorna, string pojam, bool test, bool hr);

        /*:: PONAVLJACI ::*/

        [OperationContract]
        List<_Opazanje> DetaljiPonavljaca(string grad, int idPredloska, DateTime? datumOd, DateTime? datumDo, string registracija, bool kaznjeni);

        [OperationContract]
        List<_2DLista> Ponavljaci(string grad, int idPredloska, DateTime? datumOd, DateTime? datumDo, int broj);

        #endregion

        /*:: STATISTIKA ::*/

        [OperationContract]
        List<_Statistika> ObavljenihOpazanja(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> IzdaneKazne(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> IzdanePretplate(string grad, DateTime datumOd, DateTime datumDo, int idSubjekta);

        /**/

        [OperationContract]
        _Aktiviran AktivacijaAplikacije(string aktivacijskiKod);

        [OperationContract]
        void SpremiError(string grad, Exception greska, string napomena, string korisnik);

        [OperationContract]
        _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS);

        [OperationContract]
        string Naplati(string grad, int idKorisnika, int kolicina);

        [OperationContract]
        string DnevnaKarta(string grad, int idKorisnika, string registracija, int? idLokacije, int idVrste);

        [OperationContract]
        string VrijemeDolaska();

        /**/

        [OperationContract]
        int SpremiLokaciju(string grad, string registracija, int idDjelatnika);

        [OperationContract]
        bool SpremiFotografiju(string grad, int idLokacije, byte[] fotografija);

        /**/

        [OperationContract]
        bool Storno(string grad, int idKorisnkika);

        /*:: ZONE ::*/

        [OperationContract]
        List<_Zone> Zone(string grad);

        [OperationContract]
        int DodajZonu(string grad, _Zone zona);

        [OperationContract]
        bool IzmjeniZonu(string grad, _Zone zona);

        [OperationContract]
        bool ObrisiZonu(string grad, int idZone);

        /*:: SEKTORI ::*/

        [OperationContract]
        List<_Sektori> Sektori(string grad);

        [OperationContract]
        int DodajSektor(string grad, _Sektori sektor);

        [OperationContract]
        bool IzmjeniSektor(string grad, _Sektori sektor);

        [OperationContract]
        bool ObrisiSektor(string grad, int idSektora);
    }

    [DataContract]
    public class _Aktiviran
    {
        [DataMember]
        public string Baza { get; set; }
        [DataMember]
        public string Naziv { get; set; }

        public _Aktiviran(string val, string txt)
        {
           Baza = val;
           Naziv = txt;
        }
    }

    [DataContract]
    public class _Sektori
    {
        [DataMember]
        public int IDSektora { get; set; }
        [DataMember]
        public int? IDZone { get; set; }
        [DataMember]
        public string NazivSektora { get; set; }
        [DataMember]
        public string OznakaSektora { get; set; }
        [DataMember]
        public string mParking { get; set; }
        [DataMember]
        public decimal? Cijena { get; set; }
        [DataMember]
        public int GracePeriod { get; set; }
        [DataMember]
        public decimal? Longitude { get; set; }
        [DataMember]
        public decimal? Latitude { get; set; }

        public _Sektori(int idsektora, int? idzone, string naziv, string oznaka, string mparking, decimal? cijena, decimal? latitude, decimal? longitude, int? gracePeriod)
        {
            IDSektora = idsektora;
            IDZone = idzone;
            NazivSektora = naziv;
            OznakaSektora = oznaka;
            mParking = mparking;
            Cijena = cijena;
            Longitude = latitude;
            Latitude = longitude;
            GracePeriod = (gracePeriod == null) ? 15 : (int)gracePeriod;
        }
    }

    [DataContract]
    public class _Zone
    {
        [DataMember]
        public int IDZone { get; set; }
        [DataMember]
        public string NazivZone { get; set; }
        [DataMember]
        public string mParking { get; set; }
        [DataMember]
        public decimal Cijena { get; set; }
        [DataMember]
        public int? GracePeriod { get; set; }

        public _Zone(int id, string naziv, string mparking, decimal cijena, int? gracePeriod)
        {
            IDZone = id;
            NazivZone = naziv;
            mParking = mparking;
            Cijena = cijena;
            GracePeriod = gracePeriod;
        }
    }

    [DataContract]
    public class _PoslovniSubjekt
    {
        [DataMember]
        public int IDSubjekta { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string Osoba { get; set; }
        [DataMember]
        public string Mobitel { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string OIB { get; set; }
        [DataMember]
        public string Adresa { get; set; }

        public _PoslovniSubjekt(int id, string naziv, string osoba, string mobitel, string email, string oib, string adresa)
        {
            IDSubjekta = id;
            Naziv = naziv;
            Osoba = osoba;
            Mobitel = mobitel;
            Email = email;
            OIB = oib;
            Adresa = adresa;
        }
    }

}


//"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\svcutil.exe" http://servisi.debug-riing.net/pazigrad/pgparking.svc
