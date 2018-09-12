using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Linq;

namespace PG.Servisi
{
    [ServiceContract]
    public interface IPGAdministracija
    {
        [OperationContract]
        bool Dostupan();

        [OperationContract]
        int KreirajNoviGrad(_Grad grad);

        [OperationContract]
        _Grad AktivacijaAplikacije(string aktivacijskiKod);

        [OperationContract]
        List<_3DLista> Redari(string grad);

        [OperationContract]
        List<_2DLista> Redarstva();

        [OperationContract]
        List<_2DLista> Aplikacije();

        [OperationContract]
        List<_Drzava> Drzave();

        [OperationContract]
        List<_2DLista> PopisPredlozaka(string grad);

        [OperationContract]
        List<_2DLista> PopisGradova();

        [OperationContract]
        List<_2DLista> PopisVozila(string grad);

        [OperationContract]
        _Grad Grad(string grad);

        [OperationContract]
        bool PostavkeGrada(string grad, _Grad postavke);

        [OperationContract]
        bool IzmjeniMapu(string grad, string mapa);

        [OperationContract]
        _Vozilo Vozilo(string grad, int idVozila);

        /*: GO ::*/

        [OperationContract]
        List<_3DLista> GradoviGO();

        [OperationContract]
        List<_3DLista> GrupeGO(string grad);

        /*:: NALOG ZA PLACANJE ::*/

        #region NALOG

        [OperationContract]
        _Uplatnica Uplatnica(string grad, int idRedarstva);

        [OperationContract]
        int IzmjeniUplatnicu(string grad, _Uplatnica nalog);

        [OperationContract]
        _PoslovniProstor DohvatiPoslovniProstor(string grad, int idRedarstva);

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
        List<_Prekrsaj> PrekrsajiIzvoz(string grad, int idDjelatnika, int idPredloska, DateTime? datumOd, DateTime? datumDo, bool storno);

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

        [OperationContract]
        void Posalji(List<string> brojevi, string poruka);

        #endregion

        /*:: AKTIVNOST ::*/

        #region AKTIVNOST

        [OperationContract]
        bool Aktivan(string grad, string korisnik, string racunalo, string verzija, string os, bool odobrava, int idKorisnika);

        [OperationContract]
        bool TrenutnoAktivan(string grad, int idKorisnika);

        [OperationContract]
        List<_AktivneAplikacije> DohvatiAktivne(string grad, bool aktivni);

        [OperationContract]
        bool ObrisiAktivnost(string grad, int idAktivnosti);

        [OperationContract]
        void Reset(string grad, int idAktivnosti);

        [OperationContract]
        void SpremiError(string grad, Exception greska, string napomena, string korisnik);

        #endregion

        /*:: DJELATNICI ::*/

        #region DJELATNICI

        [OperationContract]
        bool PosaljiEmailDdobrodoslice(string grad, int idKorisnika);

        [OperationContract]
        _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS);

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

        [OperationContract]
        List<_Kontakt> DohvatiSMS(string grad);

        /*:: GO ::*/

        [OperationContract]
        int PoveziSaGO(string grad, int idKorisnika);

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

        [OperationContract]
        bool Zabrani(string grad, int idDjelatnika, int idDozvole, bool dozvoljeno);

        #endregion

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

        /*:: ZAKONI ::*/

        #region ZAKONI

        [OperationContract]
        List<_2DLista> DohvatiJezike(string grad);

        [OperationContract]
        List<_Zakon> DohvatiZakone(string grad, int idRedarstva, bool neaktivni);

        [OperationContract]
        List<_Opis> DohvatiOpiseZakona(string grad, int idPrekrsaja);

        [OperationContract]
        int DodajZakon(string grad, _Zakon novi);

        [OperationContract]
        int DodajKratkiOpis(string grad, _Opis opis);

        [OperationContract]
        bool SpremiPrijevod(string grad, int idJezika, int idZakona, int idOpisa, string clanak, string opis);

        [OperationContract]
        bool IzmjeniZakon(string grad, _Zakon zakon, _Opis opis);

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
        int DodajPredlozak(string grad, string nazivPredloska, bool pauk, bool kaznjava, int idRedarstva, XElement tekstPredloska);

        [OperationContract]
        _Predlozak DohvatiPredlozakIspisa(string grad, string koji);

        [OperationContract]
        _Predlozak DohvatiPredlozak(string grad, int idPredloska);

        #endregion

        /*:: ISPIS PREDLOZAKA ::*/

        #region ISPIS PREDLOŽAKA

        //todo naziv
        [OperationContract]
        bool IspisPredloska(string grad, string detaljiKazne, int idPredloska, out string ispis);

        [OperationContract]
        bool IspisPredloska1(string baza, string detaljiKazne, int qty, int idPredloska, int idJezika, out string ispis);

        #endregion

        /*:: PORUKE ::*/

        #region PORUKE

        [OperationContract]
        List<_Poruka> DohvatiPoruke(string grad, int idDjelatnika, bool pauk);

        [OperationContract]
        bool PromijeniStatusPoruke(string grad, int idPrimatelja, int idPosiljatelja);

        [OperationContract]
        bool ImaNeprocitanihPoruka(string grad, int idPrimatelja, out bool vazno, out int brojPoruka);

        [OperationContract]
        int PosaljiPoruku(string grad, _Poruka poruka);

        [OperationContract]
        bool ObrisiPoruku(string grad, int IDPoruku, bool Primljena);

        [OperationContract]
        int Neprocitanih(string grad, int idPrimatelja, int idPosiljatelja, out bool aktivan);

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

        /*:: KAMERE ::*/

        #region WEB KAMERE

        [OperationContract]
        bool ImaKamere(string grad);

        [OperationContract]
        int DodajKameru(string grad, _WebKamere kamera);

        [OperationContract]
        bool IzmijeniKameru(string grad, _WebKamere kamera);

        [OperationContract]
        bool ObrisiKameru(string grad, int idKamere);

        [OperationContract]
        List<_WebKamere> DohvatiKamere(string grad);

        #endregion

        /*:: PREKRŠAJ GLAVNI MENU ::*/

        #region PREKRSAJ

        [OperationContract]
        List<_Prekrsaj> PretraziPrekrsaje(string grad, int idDjelatnika, DateTime datum, bool obavijesti, bool upozorenja);

        [OperationContract]
        _Prekrsaj DetaljiPrekrsaja(string grad, int idLokacije);

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
        List<_Tocka> StvaranjePutanjeRedara(string grad, DateTime datum, string vrijeme, int idRedara);

        [OperationContract]
        List<_Prekrsaj> PozicijePrekrsaja(string baza, int idZaposlenika, DateTime datum, string vrijeme);

        [OperationContract]
        List<_Pozicija> PozicijeRedara(string baza, int minuta);

        [OperationContract]
        _Pozicija PozicijaOdabranogRedara(string baza, int idDjelatnika);

        [OperationContract]
        List<_Pozicija> TrenutnePozicijeVozila(string grad, int minuta, int? idVozila);

        [OperationContract]
        List<_WebKamere> PozicijeKamera(string grad);

        /*:: RUČNO DODAVANJE PREKRSAJA ::*/

        [OperationContract]
        List<string> BrojPrekrsaja(string grad, string registracija);

        [OperationContract]
        int DodajRucniPrekrsaj(string grad, _Prekrsaj prekrsaj, bool obavijest, List<byte[]> slike, bool lisice);

        #endregion

        /*:: IZMJENE PREKRSAJA ::*/

        #region AKCIJE

        [OperationContract]
        bool Prenesen(string grad, int idPrekrsaja);

        [OperationContract]
        bool RelokacijaPrekrsaja(string grad, int idPrekrsaja, decimal latitude, decimal longitude);

        [OperationContract]
        bool Storniraj(string grad, int idPrekrsaja, string napomena, string osoba);

        [OperationContract]
        bool Test(string grad, int idPrekrsaja, bool test);

        [OperationContract]
        bool Registracija(string grad, int idPrekrsaja, string registracija, string kratica);

        [OperationContract]
        bool Adresa(string grad, int idPrekrsaja, string adresa);

        [OperationContract]
        string BrojDokumenta(string grad, _Prekrsaj prekrsaj);

        [OperationContract]
        int Vrsta(string grad, _Prekrsaj prekrsaj, string broj);

        [OperationContract]
        string ObrisiSveStornirane(string grad);

        [OperationContract]
        bool IzmjeniZakonPrekrsaja(string grad, int idPrekrsaja, int idOpisa, decimal kazna, out bool dodan);

        [OperationContract]
        int NalogPauku(string grad, int idPrekrsaja, DateTime datum, bool lisice);

        #endregion

        /*:: SUSTAV ::*/

        #region POSEBNA VOZILA

        [OperationContract]
        List<_Odobrenja> DohvatiOdobrenja(string grad);

        [OperationContract]
        bool ObrisiOdobrenje(string grad, int idOdobrenja);

        [OperationContract]
        int DodajOdobrenje(string grad, _Odobrenja odobrenje);

        [OperationContract]
        bool PromijeniStatusOdobrenja(string grad, int idOdobrenja, bool suspendirano);

        [OperationContract]
        bool IzmijeniOdobrenje(string grad, _Odobrenja odobrenje);

        //[OperationContract]
        //bool PokusajKaznjavanja(string grad, string registracija);

        #endregion

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

        /*:: PRETRAGA ::*/

        #region PRETRAGA

        [OperationContract]
        List<_Prekrsaj> ZabiljezeniPrekrsaji(string grad, int idDjelatnika, int idPredloska, DateTime? datumOd, DateTime? datumDo, bool pauk,
            bool registracija, bool dokument, bool ulica, bool storno, char? tipStorna, string pojam, bool test, bool hr);

        /*:: PONAVLJACI ::*/

        [OperationContract]
        List<_Prekrsaj> DetaljiPonavljaca(string grad, int idPredloska, DateTime? datumOd, DateTime? datumDo, string registracija);

        [OperationContract]
        List<_2DLista> Ponavljaci(string grad, int idPredloska, DateTime? datumOd, DateTime? datumDo, int broj);

        /*:: VREMENA PAUKA ::*/

        [OperationContract]
        List<_VremenaPauka> VremenaPauka(string grad, DateTime DatumOd, DateTime DatumDo);

        /*:: ZAHTJEVI ::*/

        [OperationContract]
        List<_ZahtjevPauka> Zahtjevi(string grad, int idVozila, int idStatusa, DateTime? datumOd, DateTime? datumDo, int idRedarstva);

        [OperationContract]
        bool PonoviZahtjev(string grad, int idZahtjeva);

        #endregion

        /*:: STATISTIKA ::*/

        #region STATISTIKA

        [OperationContract]
        _Aktivnost Aktivnost(string grad, DateTime datum);

        [OperationContract]
        List<_Statistika> IzdanihObavijesti(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> NaloziPauku(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> UpozorenjaObavijesti(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> UpozorenjaObavijestiMjesecno(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> UpozorenjaObavijestiPoPrekrsajima(string grad, DateTime datumOd, DateTime datumDo, bool nalozi);

        [OperationContract]
        List<_Statistika> NalogaPoPrekrsajima(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> NaglaseneUlice(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> ProsjekPoDanu(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> TrajanjePostupka(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> RedariPrekrsaji(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> PrekrsajaPoDrzavi(string grad, DateTime datumOd, DateTime datumDo, bool izuzmihr);

        [OperationContract]
        List<_Statistika> ObavijestiNaloziPauku(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> StatusVPP(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> IntenzitetRada(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> AktivnostTerminal(string grad, DateTime datumOd, DateTime datumDo, int idDjelatnika);

        [OperationContract]
        List<_Statistika> TrajanjeBaterije(string grad, DateTime datum, int idTerminala);

        [OperationContract]
        List<_Statistika> GPS(string grad, DateTime datum, int idTerminala);

        [OperationContract]
        List<_Statistika> VPStatusi(string grad);

        [OperationContract]
        List<_Statistika> VPNaplaceni(string grad);

        [OperationContract]
        List<_Stranci> VPRazradaDrzave(string grad);

        [OperationContract]
        List<_CentralnaLokacija> IntenzitetPostupanja(string grad, DateTime datumOd, DateTime datumDo, bool nalog, bool zahtjevi);

        #endregion

        #region POSTAVKE

        [OperationContract]
        List<_2DLista> PopisNaglasenihUlica(string grad);

        [OperationContract]
        bool ObrisiNaglasenuUlicu(string grad, int idUlice);

        [OperationContract]
        int DodajNaglasenuUlicu(string grad, string ulica);

        #endregion

        /*:: TERMINALI ::*/

        #region TERMINALI

        [OperationContract]
        List<_Terminal> PopisTerminala(string grad, bool neaktivni);

        [OperationContract]
        List<_2DLista> DohvatiTerminale(string grad);

        [OperationContract]
        bool IzmjeniTerminal(string grad, _Terminal terminal);

        [OperationContract]
        bool AkcijeNaTerminalima(string grad, _Terminal terminal);

        [OperationContract]
        List<_StatusTerminala> StatusTerminala(string grad);

        #endregion

        /*:: EMAIL LISTE ::*/

        #region EMAIL LISTE

        [OperationContract]
        List<_MailLista> DohvatiMailListu(string grad);

        [OperationContract]
        bool ObrisiMailListu(string grad, int idListe);

        [OperationContract]
        int DodajMailListu(string grad, _MailLista lista);

        [OperationContract]
        bool SaljiMailListu(string grad, int idListe, bool salji);

        [OperationContract]
        bool PrilogMailListi(string grad, int idListe, bool hub);

        #endregion

        /*:: ZAHTJEVI ::*/

        #region ZAHTJEVI

        [OperationContract]
        int Neobradjeni(string grad, int idDjelatnika, out _PrijavaPauk prijava);

        [OperationContract]
        bool Preuzmi(string grad, int idZahtjeva, int idDjelatnika);

        [OperationContract]
        void Odustani(string grad, int idZahtjeva, int idDjelatnika);

        [OperationContract]
        bool StatusZahtjeva(string grad, int idZahtjeva);

        [OperationContract]
        void SpremiAkcijuZahtjeva(string grad, int idZahtjeva, int idDjelatnika, int idAkcije);

        [OperationContract]
        List<_Kaznjavan> Kaznjavan(string grad, string registracija, string drzava, int dana);

        [OperationContract]
        int DodajPrekrsaj(string grad, _PrijavaPauk zahtjev, int idOpisa, decimal kazna, string registracija,
            string adresa, string drzava, bool obavijest, bool nalogPauku, bool lisice);

        [OperationContract]
        bool StatusZahtjevaPaukOdustao(string grad, int idZahtjeva);

        [OperationContract]
        bool Zatvori(string grad, int idZahtjeva, int idStatusa, int idDjelatnika, string razlog);

        [OperationContract]
        bool DodijeliPozivNaBroj(string grad, _Prekrsaj prekrsaj);

        #endregion

        /*:: RENTACAR ::*/

        #region RENT-A-CAR

        [OperationContract]
        List<_RentaCar> DohvatiRentaCar(string grad);

        [OperationContract]
        _RentaCar DodajRentaCar(string grad, _RentaCar renta);

        [OperationContract]
        bool IzmjeniRentaCar(string grad, _RentaCar renta);

        [OperationContract]
        bool ObrisiRentaCar(string grad, int idRente);

        [OperationContract]
        bool PoveziRentaCar(string grad, int idRente, string naziv, string email, string mobitel, out int idKorisnikaGO,
            out int idKlasifikacije);

        /*:: VOZILA ::*/

        [OperationContract]
        int DodajRCVozilo(string grad, int idRentaCar, string registracija);

        [OperationContract]
        bool DodajRCVozila(string grad, int idRentaCar, string[] registracije);

        [OperationContract]
        bool IzmjeniRCVozilo(string grad, int idVozila, string registracija);

        [OperationContract]
        bool ObrisiRCVozilo(string grad, int idVozila);

        #endregion
    }

    [DataContract]
    public class _2DLista
    {
        [DataMember]
        public int Value { get; set; }
        [DataMember]
        public string Text { get; set; }

        public _2DLista(int val, string txt)
        {
            Value = val;
            Text = txt;
        }
    }

    [DataContract]
    public class _Chat
    {
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public string ImePrezime { get; set; }
        [DataMember]
        public int IDPrivilegije { get; set; }
        [DataMember]
        public string Privilegija { get; set; }
        [DataMember]
        public bool Aktivan { get; set; }
        [DataMember]
        public int Neprocitanih { get; set; }

        public _Chat(int idDjelatnika, int idRedarstva, string imePrezime, int idPrivilegije, string privilegija, bool aktivan, int neprocitanih)
        {
            IDDjelatnika = idDjelatnika;
            IDRedarstva = idRedarstva;
            ImePrezime = imePrezime;
            IDPrivilegije = idPrivilegije;
            Privilegija = privilegija;
            Aktivan = aktivan;
            Neprocitanih = neprocitanih;
        }
    }

    [DataContract]
    public class _Grad
    {
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public string Baza { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public decimal IznosNaloga { get; set; }
        [DataMember]
        public int Zoom { get; set; }
        [DataMember]
        public bool Pauk { get; set; }
        [DataMember]
        public bool Aktivan { get; set; }
        [DataMember]
        public bool Vpp { get; set; }
        [DataMember]
        public bool Odvjetnici { get; set; }
        [DataMember]
        public string AktivacijskiKod { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string Grb { get; set; }
        [DataMember]
        public bool NaplataPauk { get; set; }
        [DataMember]
        public DateTime? DatumNaplate { get; set; }
        [DataMember]
        public bool DOF { get; set; }
        [DataMember]
        public bool PazigradNaIzvjestaju { get; set; }
        [DataMember]
        public string Zalba { get; set; }
        [DataMember]
        public int Zadrska { get; set; }
        [DataMember]
        public bool Chat { get; set; }
        [DataMember]
        public string Mapa { get; set; }
        [DataMember]
        public bool? DohvatPodataka { get; set; }
        [DataMember]
        public string GO { get; set; }
        [DataMember]
        public int? IDGrupePredmet { get; set; }
        [DataMember]
        public string Tip { get; set; }
        [DataMember]
        public bool Lisice { get; set; }
        [DataMember]
        public string OdlukaLisice { get; set; }
        [DataMember]
        public bool VanjskePrijave { get; set; }
        [DataMember]
        public bool PrilogObavijest { get; set; }
        [DataMember]
        public bool RacunHUB { get; set; }
        [DataMember]
        public int DanaLezarina { get; set; }
        [DataMember]
        public bool Parking { get; set; }

        public _Grad(int idg, string baza, string naziv, double lat, double lon, decimal nalog, int zoom, bool pauk, bool akt, bool vpp,
            bool odv, string ak, string adresa, string grb, bool naplatapauk, DateTime? datumNaplate, bool dof, bool pazigrad, string zalba, int zadrska, bool chat, 
            bool? dohvat, string mapa, string go, int? idgrupe, string tip, bool lisice, string odlukaLisice, bool vanjskePrijave, bool prilog, bool hub, int lezarina, bool parking)
        {
            IDGrada = idg;
            Baza = baza;
            Naziv = naziv;
            Latitude = lat;
            Longitude = lon;
            IznosNaloga = nalog;
            Zoom = zoom;
            Pauk = pauk;
            Aktivan = akt;
            Vpp = vpp;
            Odvjetnici = odv;
            AktivacijskiKod = ak;
            Adresa = adresa;
            Grb = grb;
            NaplataPauk = naplatapauk;
            DatumNaplate = datumNaplate;
            DOF = dof;
            PazigradNaIzvjestaju = pazigrad;
            Zalba = zalba;
            Zadrska = zadrska;
            Chat = chat;
            DohvatPodataka = dohvat;
            Mapa = mapa;
            GO = go;
            IDGrupePredmet = idgrupe;
            Tip = tip;
            Lisice = lisice;
            OdlukaLisice = odlukaLisice;
            VanjskePrijave = vanjskePrijave;
            PrilogObavijest = prilog;
            RacunHUB = hub;
            DanaLezarina = lezarina;
            Parking = parking;
        }
    }

    [DataContract]
    public class _Djelatnik
    {
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public int? IDRedarstva { get; set; }
        [DataMember]
        public int? IDSubjekta { get; set; }
        [DataMember]
        public string ImePrezime { get; set; }
        [DataMember]
        public string UID { get; set; }
        [DataMember]
        public string BrojIskaznice { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Mobitel { get; set; }
        [DataMember]
        public string Telefon { get; set; }
        [DataMember]
        public string Zaporka { get; set; }
        [DataMember]
        public int IDPrivilegije { get; set; }
        [DataMember]
        public string Privilegija { get; set; }
        [DataMember]
        public string OIB { get; set; }
        [DataMember]
        public string ImeNaRacunu { get; set; }
        [DataMember]
        public bool Prometni { get; set; }
        [DataMember]
        public bool Pauk { get; set; }
        [DataMember]
        public bool Statistika { get; set; }
        [DataMember]
        public bool Blokiran { get; set; }
        [DataMember]
        public bool Obrisan { get; set; }
        [DataMember]
        public bool TraziOdobrenje { get; set; }
        [DataMember]
        public bool MUP { get; set; }
        [DataMember]
        public bool ObradaZahtjeva { get; set; }
        [DataMember]
        public bool Blagajna { get; set; }
        [DataMember]
        public bool Pretplate { get; set; }
        [DataMember]
        public string GOGradID { get; set; }
        [DataMember]
        public int? IDGO { get; set; }
        [DataMember]
        private List<_Grad> Gradovi { get; set; }
        [DataMember]
        private List<int> Privilegije { get; set; }

        public _Djelatnik(int id, int? idRed, int? idSubjekta, string ip, string u, string bi, string em, string mob, string tel, string z, int idp, string pri, string oib, string imenarac,
            bool prometni, bool pauk, bool st, bool blo, bool obrisan, bool traziodobrenje, bool mup, bool obradazahtjeva, bool blagajna, bool pretplate, string goGradID, int? idgo, List<_Grad> gradovi, List<int> privilegije)
        {
            IDDjelatnika = id;
            IDRedarstva = idRed;
            IDSubjekta = idSubjekta;
            ImePrezime = ip;
            UID = u;
            BrojIskaznice = bi;
            Email = em;
            Mobitel = mob;
            Telefon = tel;
            Zaporka = z;
            IDPrivilegije = idp;
            Privilegija = pri;
            OIB = oib;
            ImeNaRacunu = imenarac;
            Prometni = prometni;
            Pauk = pauk;
            Statistika = st;
            Blokiran = blo;
            Obrisan = obrisan;
            TraziOdobrenje = traziodobrenje;
            MUP = mup;
            ObradaZahtjeva = obradazahtjeva;
            Blagajna = blagajna;
            Pretplate = pretplate;
            GOGradID = goGradID;
            IDGO = idgo;
            Gradovi = gradovi;
            Privilegije = privilegije;
        }
    }

    [DataContract]
    public class _Odobrenja
    {
        [DataMember]
        public int IDOdobrenja { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string Sjediste { get; set; }
        [DataMember]
        public string Kontakt { get; set; }
        [DataMember]
        public string Odobrenje { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public string Drzava { get; set; }
        [DataMember]
        public DateTime? DatumOd { get; set; }
        [DataMember]
        public TimeSpan? VrijemeOd { get; set; }
        [DataMember]
        public DateTime? DatumDo { get; set; }
        [DataMember]
        public TimeSpan? VrijemeDo { get; set; }
        [DataMember]
        public bool Suspendirano { get; set; }
        [DataMember]
        public int? IDDjelatnika { get; set; }
        [DataMember]
        public int? IDSubjekta { get; set; }
        [DataMember]
        public DateTime? Deaktiviran { get; set; }

        public _Odobrenja(int idOdobrenja, int idRedarstva, string naziv, string sjediste, string kontakt, string odobrenje, string registracija, string drzava, DateTime? datumOd, TimeSpan? vrijemeOd, DateTime? datumDo, TimeSpan? vrijemeDo, 
            bool suspendirano, int? idDjelatnika, int? idSubjekta, DateTime? deaktiviran)
        {
            IDOdobrenja = idOdobrenja;
            IDRedarstva = idRedarstva;
            Naziv = naziv;
            Sjediste = sjediste;
            Kontakt = kontakt;
            Odobrenje = odobrenje;
            Registracija = registracija;
            DatumOd = datumOd;
            VrijemeOd = vrijemeOd; 
            DatumDo = datumDo;
            VrijemeDo = vrijemeDo;
            Suspendirano = suspendirano;
            IDDjelatnika = idDjelatnika;
            IDSubjekta = idSubjekta;
            Deaktiviran = deaktiviran;
            Drzava = drzava;
        }
    }

    [DataContract]
    public class _Privilegije
    {
        [DataMember]
        public int IDPrivilegije { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string Opis { get; set; }
        [DataMember]
        public List<_Dozvola> Privilegije { get; set; }

        public _Privilegije(int id, string naziv, string opis, List<_Dozvola> priv)
        {
            IDPrivilegije = id;
            Naziv = naziv;
            Opis = opis;
            Privilegije = priv;
        }
    }

    [DataContract]
    public class _Dozvola
    {
        [DataMember]
        public int IDAkcije { get; set; }
        [DataMember]
        public int IDAplikacije { get; set; }
        [DataMember]
        public string Aplikacija { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string Opis { get; set; }
        [DataMember]
        public bool GrupaPrivilegija { get; set; }
        [DataMember]
        public bool Odabran { get; set; }
        [DataMember]
        public bool Dozvoljeno { get; set; }

        public _Dozvola(int id, int ida, string apl, string naziv, string opis, bool gp, bool odabran, bool dozvoljeno)
        {
            IDAkcije = id;
            IDAplikacije = ida;
            Aplikacija = apl;
            Naziv = naziv;
            Opis = opis;
            GrupaPrivilegija = gp;
            Odabran = odabran;
            Dozvoljeno = dozvoljeno;
        }
    }

    [DataContract]
    public class _Terminal
    {
        [DataMember]
        public int IDTerminala { get; set; }
        [DataMember]
        public int? IDGrada { get; set; }
        [DataMember]
        public string Grad { get; set; }
        [DataMember]
        public string IdentifikacijskiBroj { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string Verzija { get; set; }
        [DataMember]
        public XElement Parametri { get; set; }
        [DataMember]
        public bool ResetRequest { get; set; }
        [DataMember]
        public bool RestartRequest { get; set; }
        [DataMember]
        public bool ExitRequest { get; set; }
        [DataMember]
        public bool SuspendRequest { get; set; }
        [DataMember]
        public bool SelfDestructRequest { get; set; }
        [DataMember]
        public bool Aktivan { get; set; }
        [DataMember]
        public bool? Pauk { get; set; }
        [DataMember]
        public DateTime? DatumSpajanja { get; set; }

        public _Terminal(int id, int? idg, string grad, string ib, string n, string v, XElement p, bool rr, bool rest, bool ex,
            bool sr, bool sd, bool akt, bool? pauk, DateTime? datum)
        {
            IDTerminala = id;
            IDGrada = idg;
            Grad = grad;
            IdentifikacijskiBroj = ib;
            Naziv = n;
            Verzija = v;
            Parametri = p;
            ResetRequest = rr;
            RestartRequest = rest;
            ExitRequest = ex;
            SuspendRequest = sr;
            SelfDestructRequest = sd;
            Aktivan = akt;
            Pauk = pauk;
            DatumSpajanja = datum;
        }
    }

    [DataContract]
    public class _StatusTerminala
    {
        [DataMember]
        public int IDTerminala { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string Verzija { get; set; }
        [DataMember]
        public DateTime? DatumSpajanja { get; set; }
        [DataMember]
        public double? GPS { get; set; }
        [DataMember]
        public double? Baterija { get; set; }
        [DataMember]
        public bool? Pauk { get; set; }

        public _StatusTerminala(int idTerminala, string naziv, string verzija, DateTime? vrijeme, double? gpsAcc, double? battery, bool? pauk)
        {
            IDTerminala = idTerminala;
            Naziv = naziv;
            Verzija = verzija;
            DatumSpajanja = vrijeme;
            GPS = gpsAcc;
            Baterija = battery;
            Pauk = pauk;
        }
    }

    [DataContract]
    public class _Printer
    {
        [DataMember]
        public int IDPrintera { get; set; }
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public string Grad { get; set; }
        [DataMember]
        public string PIN { get; set; }
        [DataMember]
        public string MAC { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public int InterniBroj { get; set; }
        [DataMember]
        public int IDModela { get; set; }
        [DataMember]
        public string SerijskiBroj { get; set; }
        [DataMember]
        public DateTime? DatumUlaska { get; set; }
        [DataMember]
        public int? Jamstvo { get; set; }
        [DataMember]
        public DateTime? JamstvoDo { get; set; }
        [DataMember]
        public bool? Vlasnik { get; set; }
        [DataMember]
        public bool Ispravan { get; set; }

        public _Printer(int idprintera, int idgrada, int idRedarstva, string grad, string pin, string mac, string naziv, int interni, int idModela,
            string serijski, DateTime? usao, int? jamstvo, DateTime? jamstvoDo, bool? vlasnik, bool ispravan)
        {
            IDPrintera = idprintera;
            IDGrada = idgrada;
            IDRedarstva = idRedarstva;
            Grad = grad;
            PIN = pin;
            MAC = mac;
            Naziv = naziv;
            InterniBroj = interni;
            IDModela = idModela;
            SerijskiBroj = serijski;
            DatumUlaska = usao;
            Jamstvo = jamstvo;
            JamstvoDo = jamstvoDo;
            Vlasnik = vlasnik;
            Ispravan = ispravan;
        }
    }

    [DataContract]
    public class _Akcija
    {
        [DataMember]
        public int IDAkcijeKorisnika { get; set; }
        [DataMember]
        public int IDAkcije { get; set; }
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }
        [DataMember]
        public string Detalji { get; set; }
        [DataMember]
        public string Akcija { get; set; }
        [DataMember]
        public string Korisnik { get; set; }

        public _Akcija(int idak, int ida, int idd, DateTime datum, DateTime dv, string detalji, string akcija, string kor)
        {
            IDAkcijeKorisnika = idak;
            IDAkcije = ida;
            IDDjelatnika = idd;
            Datum = datum;
            DatumVrijeme = dv;
            Detalji = detalji;
            Akcija = akcija;
            Korisnik = kor;
        }
    }

    [DataContract]
    public class _AktivneAplikacije
    {
        [DataMember]
        public int IDAktivne { get; set; }
        [DataMember]
        public DateTime Kada { get; set; }
        [DataMember]
        public string Grad { get; set; }
        [DataMember]
        public string Korisnik { get; set; }
        [DataMember]
        public string Racunalo { get; set; }
        [DataMember]
        public string Aplikacija { get; set; }
        [DataMember]
        public string Verzija { get; set; }
        [DataMember]
        public string OS { get; set; }
        [DataMember]
        public string Aktivnost { get; set; }
        [DataMember]
        public bool Aktivan { get; set; }
        [DataMember]
        public bool Odobrava { get; set; }
        [DataMember]
        public bool Reset { get; set; }

        public _AktivneAplikacije(int idaktivne, DateTime kada, string grad, string korisnik, string racunalo, string aplikacija, 
            string verzija, string os, string aktivnost, bool aktivan, bool odobrava, bool reset)
        {
            IDAktivne = idaktivne;
            Kada = kada;
            Grad = grad;
            Korisnik = korisnik;
            Racunalo = racunalo;
            Aplikacija = aplikacija;
            Verzija = verzija;
            OS = os;
            Aktivnost = aktivnost;
            Aktivan = aktivan;
            Odobrava = odobrava;
            Reset = reset;
        }
    }

    [DataContract]
    public class _Zakon
    {
        [DataMember]
        public int IDZakona { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public string SkraceniOpis { get; set; }
        [DataMember]
        public decimal Kazna { get; set; }
        [DataMember]
        public string Clanak { get; set; }
        [DataMember]
        public bool Neaktivan { get; set; }
        [DataMember]
        public List<_Opis> Opisi { get; set; }
        [DataMember]
        public List<_Prijevod> Prijevodi { get; set; }

        public _Zakon(int idzakona, int idRedarstva, string opis, decimal kazna, string clanak, bool neaktivan, List<_Opis> opisi, List<_Prijevod> prijevodi)
        {
            IDZakona = idzakona;
            IDRedarstva = idRedarstva;
            SkraceniOpis = opis;
            Kazna = kazna;
            Clanak = clanak;
            Neaktivan = neaktivan;
            Opisi = opisi;
            Prijevodi = prijevodi;
        }
    }

    [DataContract]
    public class _Opis
    {
        [DataMember]
        public int IDOpisa { get; set; }
        [DataMember]
        public int IDZakona { get; set; }
        [DataMember]
        public string OpisPrekrsaja { get; set; }
        [DataMember]
        public string KratkiOpis { get; set; }
        [DataMember]
        public string ClanakPauka { get; set; }
        [DataMember]
        public bool Obrisan { get; set; }
        [DataMember]
        public List<_Prijevod> Prijevodi { get; set; }

        public _Opis(int idOpisa, int idzakona, string opis, string kratki, string pauk, bool obrisan, List<_Prijevod> prijevodi)
        {
            IDOpisa = idOpisa;
            IDZakona = idzakona;
            OpisPrekrsaja = opis;
            KratkiOpis = kratki;
            ClanakPauka = pauk;
            Prijevodi = prijevodi;
            Obrisan = obrisan;
        }
    }

    [DataContract]
    public class _Prijevod
    {
        [DataMember]
        public int IDPrijevoda { get; set; }
        [DataMember]
        public int IDZakona { get; set; }
        [DataMember]
        public int IDJezika { get; set; }
        [DataMember]
        public string Prijevod { get; set; }

        public _Prijevod(int idPrijevoda, int idZakona, int idJezika, string prijevod)
        {
            IDPrijevoda = idPrijevoda;
            IDZakona = idZakona;
            IDJezika = idJezika;
            Prijevod = prijevod;
        }
    }

    [DataContract]
    public class _Predlozak
    {
        [DataMember]
        public int IDPredloska { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public string NazivPredloska { get; set; }
        [DataMember]
        public bool Pauk { get; set; }
        [DataMember]
        public bool Kaznjava { get; set; }
        [DataMember]
        public XElement TekstPredloska { get; set; }
        [DataMember]
        public List<_TekstPredloska> Tekstovi { get; set; }

        public _Predlozak(int id, int idRedarstva, string naz, bool pauk, bool kaznjava, XElement tp, List<_TekstPredloska> teks)
        {
            IDPredloska = id;
            IDRedarstva = idRedarstva;
            NazivPredloska = naz;
            Pauk = pauk;
            Kaznjava = kaznjava;
            TekstPredloska = tp;
            Tekstovi = teks;
        }
    }

    [DataContract]
    public class _TekstPredloska
    {
        [DataMember]
        public int IDPrevedenogPredloska { get; set; }
        [DataMember]
        public int IDJezika { get; set; }
        [DataMember]
        public string Jezik { get; set; }
        [DataMember]
        public XElement TekstPredloska { get; set; }

        public _TekstPredloska(int id, int idj, string jez, XElement tekst)
        {
            IDPrevedenogPredloska = id;
            IDJezika = idj;
            Jezik = jez;
            TekstPredloska = tekst;
        }
    }

    [DataContract]
    public class _Poruka
    {
        [DataMember]
        public int IDPoruke { get; set; }
        [DataMember]
        public int IDPrimatelja { get; set; }
        [DataMember]
        public int IDPosiljatelja { get; set; }
        [DataMember]
        public int? IDLokacije { get; set; }
        [DataMember]
        public string Primatelj { get; set; }
        [DataMember]
        public string Posiljatelj { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public string Poruka { get; set; }
        [DataMember]
        public bool Procitano { get; set; }
        [DataMember]
        public bool Odlazna { get; set; }
        [DataMember]
        public bool Vazno { get; set; }
        [DataMember]
        public bool Slike { get; set; }

        public _Poruka(int idp, int idprim, int idpos, int? Idlok, string prim, string pos, DateTime dat, string por, bool proc, bool odl, bool vaz, bool sli)
        {
            IDPoruke = idp;
            IDPrimatelja = idprim;
            IDPosiljatelja = idpos;
            IDLokacije = Idlok;
            Primatelj = prim;
            Posiljatelj = pos;
            Datum = dat;
            Poruka = por;
            Procitano = proc;
            Odlazna = odl;
            Vazno = vaz;
            Slike = sli;
        }
    }

    [DataContract]
    public class _LokalneAdrese
    {
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public int Broj { get; set; }
        [DataMember]
        public List<_DetaljiLokalneAdrese> Detalji { get; set; }

        public _LokalneAdrese(string adresa, int broj, List<_DetaljiLokalneAdrese> detalji)
        {
            Adresa = adresa;
            Broj = broj;
            Detalji = detalji;
        }
    }

    [DataContract]
    public class _DetaljiLokalneAdrese
    {
        [DataMember]
        public int IDAdrese { get; set; }
        [DataMember]
        public decimal Latitude { get; set; }
        [DataMember]
        public decimal Longitude { get; set; }
        [DataMember]
        public string KucniBroj { get; set; }

        public _DetaljiLokalneAdrese(int idadrese, decimal latitude, decimal longitude, string kucnibroj)
        {
            IDAdrese = idadrese;
            Latitude = latitude;
            Longitude = longitude;
            KucniBroj = kucnibroj;
        }
    }

    [DataContract]
    public class _Nalog
    {
        [DataMember]
        public int? IDNaloga { get; set; }
        [DataMember]
        public int? IDVozila { get; set; }
        [DataMember]
        public int? IDTerminala { get; set; }
        [DataMember]
        public string Vozilo { get; set; }
        [DataMember]
        public int? IDStatusa { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public int? IDRazloga { get; set; }
        [DataMember]
        public string Razlog { get; set; }
        [DataMember]
        public DateTime? DatumNaloga { get; set; }
        [DataMember]
        public DateTime? DatumZaprimanja { get; set; }
        [DataMember]
        public DateTime? DatumPodizanja { get; set; }
        [DataMember]
        public DateTime? DatumDeponija { get; set; }
        [DataMember]
        public bool? Storno { get; set; }
        [DataMember]
        public bool? Zatvoren { get; set; }
        [DataMember]
        public string Boja { get; set; }
        [DataMember]
        public int? IDRacuna { get; set; }
        [DataMember]
        public string Racun { get; set; }
        [DataMember]
        public string Vrsta { get; set; }
        [DataMember]
        public bool Lisice { get; set; }
        [DataMember]
        public string Napomena { get; set; }

        public _Nalog(int? idNaloga, int? idVozila, int? idTerminala, string vozilo, int? idStatusa, string status, int? idRazloga, string razlog, DateTime? naloga, DateTime? zaprimanja,
            DateTime? podizanja, DateTime? deponija, bool? storno, bool? zatvoren, string boja, int? idRacuna, string racun, string vrsta, bool lisice, string napomena)
        {
            IDNaloga = idNaloga;
            IDVozila = idVozila;
            IDTerminala = idTerminala;
            Vozilo = vozilo;
            IDStatusa = idStatusa;
            Status = status;
            IDRazloga = idRazloga;
            Razlog = razlog;
            DatumNaloga = naloga;
            DatumZaprimanja = zaprimanja;
            DatumPodizanja = podizanja;
            DatumDeponija = deponija;
            Storno = storno;
            Zatvoren = zatvoren;
            Boja = boja;
            IDRacuna = idRacuna;
            Racun = racun;
            Vrsta = vrsta;
            Lisice = lisice;
            Napomena = napomena;
        }
    }

    [DataContract]
    public class _Prekrsaj
    {
        [DataMember]
        public int IDPrekrsaja { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public int IDTerminala { get; set; }
        [DataMember]
        public int? IDOpisaPrekrsaja { get; set; }
        [DataMember]
        public int? IDOpisaZakona { get; set; }
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public int IDDokumenta { get; set; }
        [DataMember]
        public decimal Latitude { get; set; }
        [DataMember]
        public decimal Longitude { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public string Redar { get; set; }
        [DataMember]
        public string BrojIskaznice { get; set; }
        [DataMember]
        public string UID { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string BrojDokumenta { get; set; }
        [DataMember]
        public string Terminal { get; set; }
        [DataMember]
        public string Dokument { get; set; }
        [DataMember]
        public string OpisPrekrsaja { get; set; }
        [DataMember]
        public string KratkiOpis { get; set; }
        [DataMember]
        public string OpisZakona { get; set; }
        [DataMember]
        public string ClanakPrekrsaja { get; set; }
        [DataMember]
        public string Clanak { get; set; }
        [DataMember]
        public string ClanakPauka { get; set; }
        [DataMember]
        public string Kazna { get; set; }
        [DataMember]
        public bool? Pauk { get; set; }
        [DataMember]
        public bool Zahtjev { get; set; }
        [DataMember]
        public bool? Storniran { get; set; }
        [DataMember]
        public bool? Test { get; set; }
        [DataMember]
        public int? Trajanje { get; set; }
        [DataMember]
        public string StatusOcitanja { get; set; }
        [DataMember]
        public string OsobaStorna { get; set; }
        [DataMember]
        public string NapomenaStorna { get; set; }
        [DataMember]
        public _KomentarPostupanja Komentar { get; set; }
        [DataMember]
        public string Vozilo { get; set; }
        [DataMember]
        public string StatusVpp { get; set; }
        [DataMember]
        public string Drzava { get; set; }
        [DataMember]
        public int? IDRacuna { get; set; }
        [DataMember]
        public string Racun { get; set; }
        [DataMember]
        public _Nalog Nalog { get; set; }
        [DataMember]
        public bool ZakonskaSankcija { get; set; }

        public _Prekrsaj(int idp, int idRedarstva, int idt, int? idopp, int? idoz, int idlok, int iddjel, int iddok, decimal lat, decimal lng, DateTime dv, string reg, string red, string bi, string uid, string adr,
            string bd, string ter, string dok, string op, string ko, string oz, string cp, string c, string cpauka, string kaz, bool? pa, bool zahtjev, bool? sto, bool? te, int? trajanje, string so, string os, string ns,
            _KomentarPostupanja kom, string voz, string svpp, string drzava, int? idRacuna, string racun, _Nalog nalog)
        {
            IDPrekrsaja = idp;
            IDRedarstva = idRedarstva;
            IDTerminala = idt;
            IDOpisaPrekrsaja = idopp;
            IDOpisaZakona = idoz;
            IDLokacije = idlok;
            IDDjelatnika = iddjel;
            IDDokumenta = iddok;
            Latitude = lat;
            Longitude = lng;
            Datum = dv.Date;
            DatumVrijeme = dv;
            Registracija = reg;
            Redar = red;
            BrojIskaznice = bi;
            UID = uid;
            Adresa = adr;
            BrojDokumenta = bd;
            Terminal = ter;
            Dokument = dok;
            OpisPrekrsaja = op;
            KratkiOpis = ko;
            OpisZakona = oz;
            ClanakPrekrsaja = cp;
            Clanak = c;
            ClanakPauka = cpauka;
            Kazna = kaz;
            Pauk = pa;
            Zahtjev = zahtjev;
            Storniran = sto;
            Test = te;
            Trajanje = trajanje;
            StatusOcitanja = so;
            OsobaStorna = os;
            NapomenaStorna = ns;
            Komentar = kom;
            Vozilo = voz;
            StatusVpp = svpp;
            Drzava = drzava;
            IDRacuna = idRacuna;
            Racun = racun;
            Nalog = nalog;
        }
    }

    [DataContract]
    public class _Slika
    {
        [DataMember]
        public int IDSlike { get; set; }
        [DataMember]
        public byte[] Slika { get; set; }

        public _Slika(int id, byte[] slika)
        {
            IDSlike = id;
            Slika = slika;
        }
    }

    [DataContract]
    public class _KomentarPostupanja
    {
        [DataMember]
        public int? IDKomentara { get; set; }
        [DataMember]
        public int? IDPrekrsaja { get; set; }
        [DataMember]
        public string Primjedba { get; set; }
        [DataMember]
        public string Obrazlozenje { get; set; }
        [DataMember]
        public string IspravnoPostupanje { get; set; }

        public _KomentarPostupanja(int? idk, int? idp, string prim, string obr, string isp)
        {
            IDKomentara = idk;
            IDPrekrsaja = idp;
            Primjedba = prim;
            Obrazlozenje = obr;
            IspravnoPostupanje = isp;
        }
    }

    [DataContract]
    public class _Koordinate
    {
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public int IDKorisnika { get; set; }
        [DataMember]
        public decimal Latitude { get; set; }
        [DataMember]
        public decimal Longitude { get; set; }
        [DataMember]
        public DateTime Date { get; set; }

        public _Koordinate(int id, int idkorisnika, decimal lat, decimal lng, DateTime dat)
        {
            IDLokacije = id;
            IDKorisnika = idkorisnika;
            Latitude = lat;
            Longitude = lng;
            Date = dat;
        }
    }

    [DataContract]
    public class _WebKamere
    {
        [DataMember]
        public int IDKamere { get; set; }
        [DataMember]
        public string NazivKamere { get; set; }
        [DataMember]
        public decimal LatKamere { get; set; }
        [DataMember]
        public decimal LngKamere { get; set; }
        [DataMember]
        public string AdresaKamere { get; set; }
        [DataMember]
        public string OpisKamere { get; set; }
        [DataMember]
        public bool PrikaziKamere { get; set; }

        public _WebKamere(int id, string nk, decimal lat, decimal lng, string ak, string ok, bool pri)
        {
            IDKamere = id;
            NazivKamere = nk;
            LatKamere = lat;
            LngKamere = lng;
            AdresaKamere = ak;
            OpisKamere = ok;
            PrikaziKamere = pri;
        }
    }

    [DataContract]
    public class _Kretanja
    {
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public string ImePrezime { get; set; }
        [DataMember]
        public double Lat { get; set; }
        [DataMember]
        public double Lon { get; set; }
        [DataMember]
        public string NazivTerminala { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }

        public _Kretanja(int id, string ip, double la, double lo, string nt, DateTime dv)
        {
            IDLokacije = id;
            ImePrezime = ip;
            Lat = la;
            Lon = lo;
            NazivTerminala = nt;
            DatumVrijeme = dv;
        }
    }

    [DataContract]
    public class _PutanjaVozila
    {
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public string Vozilo { get; set; }
        [DataMember]
        public double Lat { get; set; }
        [DataMember]
        public double Lon { get; set; }
        [DataMember]
        public string NazivTerminala { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }

        public _PutanjaVozila(int id, string voz, double la, double lo, string nt, DateTime dv)
        {
            IDLokacije = id;
            Vozilo = voz;
            Lat = la;
            Lon = lo;
            NazivTerminala = nt;
            DatumVrijeme = dv;
        }
    }

    [DataContract]
    public class _DogadjajiNaloga
    {
        [DataMember]
        public int IDStatusa { get; set; }
        [DataMember]
        public decimal LatDog { get; set; }
        [DataMember]
        public decimal LngDog { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }

        public _DogadjajiNaloga(int ids, decimal ladog, decimal lnDog, string st, DateTime dv)
        {
            IDStatusa = ids;
            LatDog = ladog;
            LngDog = lnDog;
            Status = st;
            DatumVrijeme = dv;
        }
    }

    [DataContract]
    public class _VremenaPauka
    {
        [DataMember]
        public DateTime? VrijemeNaloga { get; set; }
        [DataMember]
        public DateTime? Preuzet { get; set; }
        [DataMember]
        public DateTime? Podizanje { get; set; }
        [DataMember]
        public DateTime? Deponij { get; set; }
        [DataMember]
        public TimeSpan? PreuzetTrajanje { get; set; }
        [DataMember]
        public TimeSpan? PodizanjeTrajanje { get; set; }
        [DataMember]
        public TimeSpan? DeponijTrajanje { get; set; }
        [DataMember]
        public TimeSpan? Ukupno { get; set; }
        [DataMember]
        public string Status { get; set; }

        public _VremenaPauka(DateTime? vn, DateTime? preuzet, DateTime? podizanje, DateTime? deponij,
            TimeSpan? preuzetT, TimeSpan? podizanjeT, TimeSpan? deponijT, TimeSpan? ukupno, string status)
        {
            VrijemeNaloga = vn;
            Preuzet = preuzet;
            Podizanje = podizanje;
            Deponij = deponij;
            PreuzetTrajanje = preuzetT;
            PodizanjeTrajanje = podizanjeT;
            DeponijTrajanje = deponijT;
            Ukupno = ukupno;
            Status = status;
        }
    }

    [DataContract]
    public class _Statistika
    {
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public int Broj { get; set; }
        [DataMember]
        public decimal Posto { get; set; }
        [DataMember]
        public string PuniNaziv { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public List<_Statistika> Detalji { get; set; }

        public _Statistika(string naziv, int broj, decimal posto, string pn, DateTime datum, List<_Statistika> detalji)
        {
            Naziv = naziv;
            Broj = broj;
            Posto = posto;
            PuniNaziv = pn;
            Datum = datum;
            Detalji = detalji;
        }
    }

    [DataContract]
    public class _Aktivnost
    {
        [DataMember]
        public int Obavijesti { get; set; }
        [DataMember]
        public int Upozorenja { get; set; }
        [DataMember]
        public int? Naloga { get; set; }
        [DataMember]
        public int? Zahtjeva { get; set; }
        [DataMember]
        public int? Odobrenih { get; set; }
        [DataMember]
        public int? Lisica { get; set; }

        public _Aktivnost(int obavijesti, int upozorenja, int? naloga, int? ukupno, int? odobrenih, int? lisica)
        {
            Obavijesti = obavijesti;
            Upozorenja = upozorenja;
            Naloga = naloga;
            Zahtjeva = ukupno;
            Odobrenih = odobrenih;
            Lisica = lisica;
        }
    }

    [DataContract]
    public class _PovijestOpreme
    {
        [DataMember]
        public int IDPovijesti { get; set; }
        [DataMember]
        public int IDOpreme { get; set; }
        [DataMember]
        public int IDVrste { get; set; }
        [DataMember]
        public string Vrsta { get; set; }
        [DataMember]
        public int IDStatusa { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Napomena { get; set; }
        [DataMember]
        public string Interni { get; set; }
        [DataMember]
        public bool Servis { get; set; }

        public _PovijestOpreme(int idPovijesti, int idOpreme, int idVrste, string vrsta, int idStatusa, DateTime datum, string status, string napomena, string interni, bool servis)
        {
            IDPovijesti = idPovijesti;
            IDOpreme = idOpreme;
            IDVrste = idVrste;
            Vrsta = vrsta;
            IDStatusa = idStatusa;
            Datum = datum;
            Status = status;
            Napomena = napomena;
            Interni = interni;
            Servis = servis;
        }
    }

    [DataContract]
    public class _MailLista
    {
        [DataMember]
        public int IDListe { get; set; }
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string ImePrezime { get; set; }
        [DataMember]
        public bool Naredba { get; set; }
        [DataMember]
        public bool PrilogHUB { get; set; }

        public _MailLista(int idListe, int idGrada, string email, string imePrezime, bool naredba, bool hub)
        {
            IDListe = idListe;
            IDGrada = idGrada;
            Email = email;
            ImePrezime = imePrezime;
            Naredba = naredba;
            PrilogHUB = hub;
        }
    }

    [DataContract]
    public class _Stranci
    {
        [DataMember]
        public string JLS { get; set; }
        [DataMember]
        public int Stranaca { get; set; }
        [DataMember]
        public int Naplacenih { get; set; }
        [DataMember]
        public int Obustavljeni { get; set; }
        [DataMember]
        public int Djelomicno { get; set; }
        [DataMember]
        public int Ostalo { get; set; }
        [DataMember]
        public int PosalnihVP { get; set; }
        [DataMember]
        public int NaplacenihVP { get; set; }
        [DataMember]
        public int Nenaplaceni { get; set; }
        [DataMember]
        public List<_Stranci> Drzave { get; set; }

        public _Stranci(string jls, int stranaca, int naplacenih, int obustavljeni, int djelomicni, int ostalo, int poslanih, int naplacenivp, int nenaplaceni, List<_Stranci> drzave)
        {
            JLS = jls;
            Stranaca = stranaca;
            Naplacenih = naplacenih;
            Obustavljeni = obustavljeni;
            Djelomicno = djelomicni;
            Ostalo = ostalo;
            PosalnihVP = poslanih;
            NaplacenihVP = naplacenivp;
            Nenaplaceni = nenaplaceni;
            Drzave = drzave;
        }
    }

    [DataContract]
    public class _RentaCar
    {
        [DataMember]
        public int IDRentaCar { get; set; }
        [DataMember]
        public int? IDKlasifikacije { get; set; }
        [DataMember]
        public int? IDKorisnikaGO { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Mobitel { get; set; }
        [DataMember]
        public string Osoba { get; set; }
        [DataMember]
        public string Telefon { get; set; }
        [DataMember]
        public bool Aktivan { get; set; }
        [DataMember]
        public bool HUB { get; set; }
        [DataMember]
        public List<_2DLista> Vozila { get; set; }

        public _RentaCar(int id, int? idKlasifikacije, int? idKorisnikaGo, string naziv, string email, string mobitel, string osoba, string telefon, bool aktivan, bool hub, List<_2DLista> vozila)
        {
            IDRentaCar = id;
            IDKlasifikacije = idKlasifikacije;
            IDKorisnikaGO = idKorisnikaGo;
            Naziv = naziv;
            Email = email;
            Mobitel = mobitel;
            Osoba = osoba;
            Telefon = telefon;
            Aktivan = aktivan;
            Vozila = vozila;
            HUB = hub;
        }
    }

    //[DataContract]
    //public class _DrzaveStranaca
    //{
    //    [DataMember]
    //    public string Drzava { get; set; }
    //    [DataMember]
    //    public int Ukupno { get; set; }
    //    [DataMember]
    //    public int Placenih { get; set; }
    //    [DataMember]
    //    public int Posalnih { get; set; }
    //    [DataMember]
    //    public int Naplacenih { get; set; }
    //}
}
