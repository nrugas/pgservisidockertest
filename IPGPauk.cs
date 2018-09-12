using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace PG.Servisi
{
    [ServiceContract]
    public interface IPGPauk
    {
        [OperationContract]
        bool Dostupan();

        [OperationContract]
        List<_2DLista> Aplikacije();

        [OperationContract]
        List<_2DLista> Redarstva();

        [OperationContract]
        List<_Drzava> Drzave();

        [OperationContract]
        List<_3DLista> DohvatiOpise(string grad);

        [OperationContract]
        string f_kontrolni(string broj);

        [OperationContract]
        _Grad AktivacijaAplikacije(string aktivacijskiKod);

        [OperationContract]
        _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS);

        [OperationContract]
        void SpremiError(string grad, Exception greska, string napomena, string korisnik);

        [OperationContract]
        bool Aktivan(string grad, string korisnik, string racunalo, string verzija, string os, int idKorisnika);

        [OperationContract]
        void Reset(string grad, int idAktivnosti);

        [OperationContract]
        List<_2DLista> PopisGradova();

        [OperationContract]
        _Predlozak DohvatiPredlozakIspisa(string grad, string koji);

        [OperationContract]
        _Predlozak DohvatiPredlozak(string grad, int idPredloska);

        [OperationContract]
        string IspisObavijestiPauk(string grad, int idLokacije, string broj);

        [OperationContract]
        bool IspisPredloska1(string baza, string detaljiKazne, int idPredloska, out string ispis);

        [OperationContract]
        _Grad Grad(string grad);

        [OperationContract]
        bool IzmjeniMapu(string grad, string mapa);

        [OperationContract]
        bool NaruciMaterijal(string grad, _Narudzba narudzba);

        #region KONTAKTIRANJE KORISNIKA

        [OperationContract]
        bool DodajPredlozak(string grad, string naziv, string predlozak);

        [OperationContract]
        bool ObrisiPredlozakEmaila(string grad, int idPredloska);

        [OperationContract]
        List<_3DLista> DohvatiPredloskeEmaila(string grad);

        [OperationContract]
        _EmailPostavke PostavkeEmaila(string grad);

        [OperationContract]
        List<_Predlozak> PredlosciIspisa(string grad);

        /*: GO ::*/

        [OperationContract]
        List<_3DLista> GradoviGO();

        #endregion

        /*:: GLAVNI MENU ::*/

        #region GLAVNI MENU

        [OperationContract]
        List<_Pozicija> PozicijeRedara(string baza, int minuta);

        [OperationContract]
        List<_Pozicija> TrenutnePozicijeVozila(string grad, int minuta, int? idVozila);

        [OperationContract]
        List<_Prekrsaj> PozicijeNaloga(string grad, int idVozila, DateTime datum, bool sviNalozi, string vrijeme);

        [OperationContract]
        List<_PutanjaVozila> PutanjaObradeNaloga(string grad, _Nalog nalog, out List<_PutanjaVozila> putanjaDoPrekrsaja, out List<_DogadjajiNaloga> dogadaji);

        [OperationContract]
        List<_Tocka> PutanjaPauka(string grad, DateTime datum, string vrijeme, int idVozila);

        #endregion

        /*:: PREKRSAJ ::*/

        #region PREKRSAJ

        [OperationContract]
        List<_Slika> SlikePrekrsaja(string grad, int idLokacije);

        [OperationContract]
        List<byte[]> Slike(string grad, int idLokacije);

        [OperationContract]
        int RotirajSliku(string grad, int idSlike, byte[] slika);

        [OperationContract]
        List<_Prekrsaj> PretragaNaloga(string grad, int idStatusa, int idVozila, DateTime? datumOd, DateTime? datumDo,
            bool registracija, bool dokument, bool ulica, string pojam);

        [OperationContract]
        List<_Prekrsaj> PretragaNalogaNaDeponiju(string grad);

        [OperationContract]
        List<_Prekrsaj> PretragaBlokiranih(string grad);

        [OperationContract]
        List<_2DLista> Statusi(string grad, bool status);

        [OperationContract]
        int DodajRucniPrekrsaj(string grad, _Prekrsaj prekrsaj, bool obavijest, List<byte[]> slike, int idRedarstva, bool lisice);

        [OperationContract]
        List<_Zakon> DohvatiZakone(string grad, int idRedarstva, bool neaktivni);

        [OperationContract]
        List<int> DodajSliku(string grad, int idLokacije, List<byte[]> slike);

        [OperationContract]
        List<_Predlozak> PredlosciIspisaLight(string grad, int idRedarstva);

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

        /*:: VOZILA ::*/

        #region VOZILA

        [OperationContract]
        List<_2DLista> PopisVozila(string grad);

        [OperationContract]
        List<_Vozilo> VozilaPauka(string grad, bool obrisana);

        [OperationContract]
        bool ObrisiVozilo(string grad, int idVozila);

        [OperationContract]
        bool IzmijeniVozilo(string grad, _Vozilo vozilo);

        [OperationContract]
        int DodajVozilo(string grad, _Vozilo vozilo);

        [OperationContract]
        bool AktivirajVozilo(string grad, int idVozila, bool aktivno);

        [OperationContract]
        bool VoziloObradjujeNaloge(string grad, int idVozila, bool obradjuje);

        [OperationContract]
        bool VoziloObradjujeLisice(string grad, int idVozila, bool obradjuje);

        [OperationContract]
        _Vozilo Vozilo(string grad, int idVozila);

        #endregion

        /*:: PODRSKA ::*/

        #region PODRSKA

        [OperationContract]
        bool PrijaviProblem(string grad, _Problem problem, List<byte[]> slike);

        [OperationContract]
        bool PostaviPitanje(string grad, int idKorisnika, int idPodrucja, string poruka, List<byte[]> slike);

        [OperationContract]
        List<_Prekrsaj> PrekrsajiIzvoz(string grad, int idDjelatnika, int idPredloska, DateTime? datumOd,
            DateTime? datumDo, bool storno);

        #endregion

        /*:: DJELATNICI ::*/

        #region DJELATNICI

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

        [OperationContract]
        bool Zabrani(string grad, int idDjelatnika, int idDozvole, bool dozvoljeno);

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

        /*:: AKCIJE KORISNIKA ::*/

        #region AKCIJE KORISNIKA

        [OperationContract]
        List<_AktivneAplikacije> DohvatiAktivne(string grad, bool aktivni);

        [OperationContract]
        bool ObrisiAktivnost(string grad, int idAktivnosti);

        [OperationContract]
        void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena);

        [OperationContract]
        List<_Akcija> DohvatiAkcije(string grad, int? idDjelatnika, int? idAkcije, DateTime? odDatum, DateTime? doDatum, int idprivilegije, int? idRedarstva);

        [OperationContract]
        List<_2DLista> DohvatiVrsteAkcija(string grad);

        [OperationContract]
        bool ObrisiAkciju(string grad, int idAkcije);

        #endregion

        /*:: AKCIJE ::*/

        #region AKCIJE

        [OperationContract]
        bool Storniraj(string baza, int idNaloga, int idRazloga, string napomena);

        [OperationContract]
        bool IzmjeniVoziloNaloga(string grad, int idNaloga, int idVozila);

        //dodijeli
        [OperationContract]
        List<_Nalog> DodijeljeniNalozi(string grad, int idVozila);

        //dodijeli
        [OperationContract]
        bool? Dodjeli(string grad, int idNaloga, int idVozila);

        [OperationContract]
        bool UpDown(string grad, int idNaloga, int idVozila, bool up);

        [OperationContract]
        bool Otkazi(string grad, int idNaloga);

        //deponiraj
        [OperationContract]
        bool Deponiraj(string grad, int idNaloga, int idTerminala);

        [OperationContract]
        int? IDTerminala(string grad, int idVozila);

        //pokusaj
        [OperationContract]
        bool PokusajPodizanja(string grad, int idNaloga, int idTerminala);

        //blokiraj
        [OperationContract]
        bool Blokiraj(string grad, int idNaloga, int idTerminala);

        //naplati
        //[OperationContract]
        //int NoviBrojRacuna(string grad, int idNaloga, out decimal? iznos);

        [OperationContract]
        int? Pokusaj(string grad, _Racun racun, int idTerminala, ref string brrac);

        [OperationContract]
        bool VratiNaPocetak(string grad, int idNaloga);

        [OperationContract]
        bool NaplatioIzvanSustava(string grad, int idNaloga, string napomena);

        [OperationContract]
        bool Napomena(string grad, int idNaloga, string napomena);

        /*:: IZMJENE STATUS ::*/

        [OperationContract]
        List<_Event> IzmjeneStatusa(string grad, int idVozila, DateTime datum);

        [OperationContract]
        bool ObrisiIzmjenuStatusa(string grad, int id);

        /*:: IZMJENE NALOGA ::*/

        [OperationContract]
        bool Registracija(string grad, int idPrekrsaja, string registracija, string kratica);

        [OperationContract]
        bool Adresa(string grad, int idPrekrsaja, string adresa);

        #endregion

        /*::  ::*/

        [OperationContract]
        List<_2DLista> Razlozi(string grad);

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

        /*:: AUTOPILOT ::*/

        [OperationContract]
        bool UkljuciAutoPilot(string grad, bool ukljucen);

        [OperationContract]
        bool Autopilot(string grad);

        /*:: STATISTIKA ::*/

        #region STATISTIKA

        [OperationContract]
        List<_Statistika> NaloziPauku(string grad, DateTime datumOd, DateTime datumDo, int IDVozila, bool zahtjevi, int idOpisa);

        [OperationContract]
        List<_Statistika> RazloziOstalog(string grad, DateTime datumOd, DateTime datumDo, int IDVozila);

        [OperationContract]
        List<_Statistika> IzdanihRacuna(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> NalogaPoPrekrsajima(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> NalogaPoPrekrsajimaSatus(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> Kilometri(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_Statistika> PoslanihZahtjeva(string grad, DateTime datumOd, DateTime datumDo, int IDVozila);

        [OperationContract]
        List<_CentralnaLokacija> IntenzitetPostupanja(string grad, DateTime datumOd, DateTime datumDo, bool nalog, bool zahtjevi);

        [OperationContract]
        List<_Statistika> TrajanjeBaterije(string grad, DateTime datum, int idTerminala);

        [OperationContract]
        List<_Statistika> GPS(string grad, DateTime datum, int idTerminala);

        #endregion

        /*:: IZVJEŠTAJI ::*/

        [OperationContract]
        List<_EvidencijaPlacanja> EvidencijaPlacanja(string grad, DateTime datum);

        [OperationContract]
        List<_EvidencijaPlacanja> EvidencijaPlacanjaUkupno(string grad, DateTime datum);

        [OperationContract]
        List<_DnevniUtrzak> BlagajnickiIzvjestaj(string grad, DateTime datum, int idDjelatnika);

        [OperationContract]
        List<_DnevniUtrzak> BlagajnickiIzvjestajSintetika(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        DateTime? ZadnjiBlagajnickiIzvjestaj(string grad);

        #region PREGLED

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
        List<_ZahtjevPauka> Zahtjevi(string grad, int idVozila, int idStatusa, DateTime? datumOd, DateTime? datumDo);

        [OperationContract]
        _ZahtjevPauka DohvatiZahtjev(string grad, int idZahtjeva);

        [OperationContract]
        bool PonoviZahtjev(string grad, int idZahtjeva);

        #endregion

        /*:: POSTAVKE JLS ::*/

        #region POSTAVKE

        [OperationContract]
        _CentralnaLokacija Deponij(string grad);

        [OperationContract]
        bool LokacijaDeponija(string grad, _CentralnaLokacija deponij);

        [OperationContract]
        bool StatusLokacije(string grad);

        [OperationContract]
        bool DodajStatusLokacije(string grad);

        [OperationContract]
        bool SpremiPostavke(string grad, bool dof, bool naplata, bool lisice, bool prijave, bool prilog, bool mup, string odlukaLisice, bool hub, string zalbaPrometnog, int danaLezarina, DateTime? datum);

        #endregion

        /*:: REDARSTVO PAUK ::*/

        [OperationContract]
        List<Tuple<int, int, int>> RedarstvaPauka(string grad);

        [OperationContract]
        List<Tuple<string, int, int>> DohvatiRedarstvaPauka(string grad);

        [OperationContract]
        bool ObrisiRedarstvoPauk(string grad, Tuple<int, int, int> podaci);

        [OperationContract]
        bool DodajRedarstvoPauk(string grad, Tuple<int, int, int> podaci);

        /*:: POSADA ::*/

        #region POSADA

        [OperationContract]
        bool Ukloni(string grad, _Posada posada);

        [OperationContract]
        bool Dodaj(string grad, _Posada posada);

        [OperationContract]
        List<_Posada> DohvatiPosadu(string grad, DateTime datum);

        [OperationContract]
        List<_2DLista> Pauk(string grad);

        #endregion

        #region RACUNI

        /*:: NAPLATA ::*/

        [OperationContract]
        string Naplati(string grad, _Racun racun, out int idRacuna, out string poziv);

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
        _PoslovniProstor DohvatiPoslovniProstor(string grad, int idRedarstva);

        [OperationContract]
        bool? IzmjeniPoslovniProstor(string grad, _PoslovniProstor prostor);

        [OperationContract]
        bool SpremiLogo(string grad, byte[] logo, int idRedarstva);

        [OperationContract]
        bool RegistrirajPoslovniProstor(string grad, _PoslovniProstor prostor);

        [OperationContract]
        bool DodajCertifikat(string grad, string sifra, byte[] certifikat, int idVlasnika);

        [OperationContract]
        _Uplatnica Uplatnica(string grad, int idRedarstva);

        /*:: NAPLATNA MJESTA ::*/

        [OperationContract]
        _NaplatnoMjesto NaplatnoMjesto(string grad, string oznaka, int idProstora);

        [OperationContract]
        List<_NaplatnoMjesto> NaplatnaMjesta(string grad, int idProstora);

        /*:: POSTAVKE ISPISA ::*/

        [OperationContract]
        List<_PostavkeIspisa> DohvatiPostavkeIspisa(string grad, int idRedarstva);

        [OperationContract]
        bool IzmjeniPostavkeIspisa(string grad, _PostavkeIspisa postavke);

        [OperationContract]
        int DodajPostavkuIspisa(string grad, _PostavkeIspisa postavke);

        [OperationContract]
        bool KopirajPostavkeIspisa(string grad, int idRedarstva);

        /*:: OPISI STAVKI ::*/

        [OperationContract]
        List<_2DLista> StatusiKojiNaplacuju(string grad);

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
        _Racun DohvatiRacunPoziv(string grad, string poziv, out int idStatusa);

        [OperationContract]
        List<_Racun> DohvatiPopisRacuna(string grad, DateTime? datumOd, DateTime? datumDo, int idDjelatnika, bool fisk,
            string brrac, string poziv, int idRedarstva);

        [OperationContract]
        List<_Racun> DohvatiPopisRacunaOsoba(string grad, string ime, string prezime, string oib, int idRedarstva);

        [OperationContract]
        List<_Racun> DohvatiPopisRacunaStorno(string grad, DateTime? datumOd, DateTime? datumDo, int idStatusa, int idRedarstva);

        [OperationContract]
        List<_Racun> DohvatiPopisRacunaMUP(string grad, DateTime? datumOd, DateTime? datumDo, string brrac,
            int idRedarstva);

        [OperationContract]
        _Prekrsaj DetaljiPrekrsaja(string grad, int idNaloga);

        [OperationContract]
        _Prekrsaj DetaljiPrekrsajaNalog(string grad, int idNaloga);

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
        bool? Prenesi(string grad, DateTime datumOd, DateTime datumDo, out string poruka);

        [OperationContract]
        _Prijenos Pripremi(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        List<_PovijestPrijenosa> DohvatiPovijestPrijenosa(string grad, DateTime? datumOd, DateTime? datumDo);

        /*:: KARTICE ::*/

        [OperationContract]
        bool IzmjeniKarticuPlacanja(string grad, int idRacuna, int? idBanke, int idKartice, string odobrenje, bool? rate);

        /*:: FISKALIZACIJA ::*/

        [OperationContract]
        string PonovnaFiskalizacija(string grad, int idRedarstva);

        /*:: MUP ::*/

        [OperationContract]
        List<_Osoba> DohvatMUPa(string grad, _Racun racun, int idKorisnika);

        [OperationContract]
        _OdgovorMUPVozilo MUPParkingVozilo(string oibustanove, string registracija, DateTime datumpreksaja, string adresaprekrsaja);

        #endregion

        [OperationContract]
        string IzvjestajSmjene(string grad, int idDjelatnika);

        /*:: EMAIL LISTE ::*/

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

        /*:: VANJSKE PRIJAVE ::*/

        #region VANJSKE PRIJAVE

        [OperationContract]
        List<_Prijava> Prijave(string grad, DateTime datumOd, DateTime datumDo, bool nepregledane);

        [OperationContract]
        void PregledaoPrijavu(string grad, int idPrijave);

        [OperationContract]
        bool NalogPrijave(string grad, int idPrijave, int? idNaloga, int idPrekrsaja);

        [OperationContract]
        int NepregledanePrijave(string grad);

        [OperationContract]
        void OdbijPrijavu(string grad, int idPrijave);

        [OperationContract]
        bool ObradiPrijavu(string grad, int idPrijave, string napomena);

        [OperationContract]
        List<_2DLista> StatusiVP(string grad);
        
        #endregion
    }

    [DataContract]
    public class _3DLista
    {
        [DataMember]
        private int ID { get; set; }
        [DataMember]
        private string Naziv { get; set; }
        [DataMember]
        private string Opis { get; set; }

        public _3DLista(int id, string naziv, string opis)
        {
            ID = id;
            Naziv = naziv;
            Opis = opis;
        }
    }

    [DataContract]
    public class _Vozilo
    {
        [DataMember]
        public int IDVozila { get; set; }
        [DataMember]
        public string NazivVozila { get; set; }
        [DataMember]
        public int? IDTerminala { get; set; }
        [DataMember]
        public string Terminal { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public string Kontakt { get; set; }
        [DataMember]
        public string Napomena { get; set; }
        [DataMember]
        public string Oznaka { get; set; }
        [DataMember]
        public string OznakaPP { get; set; }
        [DataMember]
        public bool Autopilot { get; set; }
        [DataMember]
        public bool Obradjuje { get; set; }
        [DataMember]
        public bool Blokira { get; set; }
        [DataMember]
        public bool Obrisan { get; set; }

        public _Vozilo(int id, string nv, int? idt, string term, string reg, string kon, string nap, bool ap, string oznaka, string oznakaPP, bool obrad, bool blokira, bool obrisan)
        {
            IDVozila = id;
            NazivVozila = nv;
            IDTerminala = idt;
            Terminal = term;
            Registracija = reg;
            Kontakt = kon;
            Napomena = nap;
            Autopilot = ap;
            Oznaka = oznaka;
            OznakaPP = oznakaPP;
            Obradjuje = obrad;
            Blokira = blokira;
            Obrisan = obrisan;
        }
    }

    [DataContract]
    public class _Event
    {
        [DataMember]
        public int IDEventa { get; set; }
        [DataMember]
        public int IDNaloga { get; set; }
        [DataMember]
        public int IDStatusa { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public string Vozilo { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Boja { get; set; }
        [DataMember]
        public bool Nalog { get; set; }

        public _Event(int idEventa, int idNaloga, int idStatusa, DateTime datum, string vozilo, string status, string boja, bool nalog)
        {
            IDEventa = idEventa;
            IDNaloga = idNaloga;
            IDStatusa = idStatusa;
            Datum = datum;
            Vozilo = vozilo;
            Status = status == null ? "" : status.Replace("()", "").TrimEnd('\r', '\n', ' ');
            Boja = boja;
            Nalog = nalog;
        }
    }

    [DataContract]
    public class _Pozicija
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string Terminal { get; set; }
        [DataMember]
        public int Preciznost { get; set; }
        [DataMember]
        public string Baterija { get; set; }
        [DataMember]
        public double? Brzina { get; set; }
        [DataMember]
        public string Kontakt { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public DateTime? Vrijeme { get; set; }
        [DataMember]
        public bool AP { get; set; }
        [DataMember]
        public List<_Tocka> Lokacije { get; set; }

        public _Pozicija(int id, string naziv, string terminal, int prec, string bat, double? brzina, string kon, string reg, DateTime? vrijeme, bool ap, List<_Tocka> lokacije)
        {
            ID = id;
            Naziv = naziv;
            Terminal = terminal;
            Preciznost = prec;
            Baterija = bat;
            Brzina = brzina;
            Kontakt = kon;
            Registracija = reg;
            Vrijeme = vrijeme;
            AP = ap;
            Lokacije = lokacije;
        }
    }

    [DataContract]
    public class _Tocka
    {
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public DateTime Vrijeme { get; set; }

        public _Tocka(double lat, double lng, DateTime vrijeme)
        {
            Latitude = lat;
            Longitude = lng;
            Vrijeme = vrijeme;
        }
    }

    [DataContract]
    public class _Problem
    {
        [DataMember]
        public int IDProblema { get; set; }
        [DataMember]
        public DateTime Vrijeme { get; set; }
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public int IDAplikacije { get; set; }
        [DataMember]
        public string Djelatnik { get; set; }
        [DataMember]
        public string Aplikacija { get; set; }
        [DataMember]
        public string Interval { get; set; }
        [DataMember]
        public string Radnja { get; set; }
        [DataMember]
        public string Opis { get; set; }
        [DataMember]
        public int? IDTerminala { get; set; }
        [DataMember]
        public string Terminal { get; set; }
        [DataMember]
        public int? IDRedara { get; set; }
        [DataMember]
        public string Redar { get; set; }
        [DataMember]
        public bool Rijesen { get; set; }

        public _Problem(int idproblema, DateTime vrijeme, int iddjelatnika, int idaplikacije, string djelatnik, string aplikacija, string interval, string radnja, string opis,
            int? idterminala, string terminal, int? idredara, string redar, bool rijesen)
        {
            IDProblema = idproblema;
            Vrijeme = vrijeme;
            IDDjelatnika = iddjelatnika;
            IDAplikacije = idaplikacije;
            Djelatnik = djelatnik;
            Aplikacija = aplikacija;
            Interval = interval;
            Radnja = radnja;
            Opis = opis;
            IDTerminala = idterminala;
            Terminal = terminal;
            IDRedara = idredara;
            Redar = redar;
            Rijesen = rijesen;
        }
    }

    [DataContract]
    public class _ZahtjevPauka
    {
        [DataMember]
        public int IDZahtjeva { get; set; }
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public int? IDDjelatnika { get; set; }
        [DataMember]
        public string Djelatnik { get; set; }
        [DataMember]
        public int? IDNaloga { get; set; }
        [DataMember]
        public int? IDVozila { get; set; }
        [DataMember]
        public string Vozilo { get; set; }
        [DataMember]
        public int? IDOdobravatelja { get; set; }
        [DataMember]
        public string Odobrio { get; set; }
        [DataMember]
        public int? IDStatusa { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public int? IDOpisa { get; set; }
        [DataMember]
        public string OpisPrekrsaja { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string Razlog { get; set; }
        [DataMember]
        public TimeSpan Trajanje { get; set; }
        [DataMember]
        public int IDAplikacije { get; set; }
        [DataMember]
        public string Aplikacija { get; set; }

        public _ZahtjevPauka(int idzahtjeva, int idlokacije, int? iddjelatnika, string djelatnik, int? idnaloga, int? idvozila, string vozilo,
            int? idodobravatelja, string odobrio, int? idstatusa, string status, int? idOpisa, string opis, DateTime datum, string registracija, 
            string adresa, string razlog, TimeSpan trajanje, int idAplikacije, string aplikacija)
        {
            IDZahtjeva = idzahtjeva;
            IDLokacije = idlokacije;
            IDNaloga = idnaloga;
            IDVozila = idvozila;
            IDDjelatnika = iddjelatnika;
            Djelatnik = djelatnik; 
            Vozilo = vozilo;
            IDOdobravatelja = idodobravatelja;
            Odobrio = odobrio;
            IDStatusa = idstatusa;
            Status = status;
            IDOpisa = idOpisa;
            OpisPrekrsaja = opis;
            Datum = datum;
            Registracija = registracija;
            Adresa = adresa;
            Razlog = razlog;
            Trajanje = trajanje;
            IDAplikacije = idAplikacije;
            Aplikacija = aplikacija;
        }
    }

    [DataContract]
    public class _Posada
    {
        [DataMember]
        public int IDPosade { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }
        [DataMember]
        public int IDVozila { get; set; }
        [DataMember]
        public int IDDjelatnika1 { get; set; }
        [DataMember]
        public int IDDjelatnika2 { get; set; }
        [DataMember]
        public int? IDTerminala { get; set; }
        [DataMember]
        public bool? JutarnjaSmjena { get; set; }

        public _Posada(int idPosade, DateTime datumVrijeme, int idVozila, int idDjelatnika1, int idDjelatnika2, int? idTerminala, bool? jutarnjaSmjena)
        {
            IDPosade = idPosade;
            DatumVrijeme = datumVrijeme;
            IDVozila = idVozila;
            IDDjelatnika1 = idDjelatnika1;
            IDDjelatnika2 = idDjelatnika2;
            IDTerminala = idTerminala;
            JutarnjaSmjena = jutarnjaSmjena;
        }
    }

    [DataContract]
    public class _PoslovniProstor
    {
        [DataMember]
        public int IDPoslovnogProstora { get; set; }
        [DataMember]
        public int? IDFisklaizacije { get; set; }
        [DataMember]
        public int? IDVlasnikaFiskal { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string Podnaslov { get; set; }
        [DataMember]
        public string USustavu { get; set; }
        [DataMember]
        public string Web { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Opis { get; set; }
        [DataMember]
        public string Ulica { get; set; }
        [DataMember]
        public string Broj { get; set; }
        [DataMember]
        public string Dodatak { get; set; }
        [DataMember]
        public string Posta { get; set; }
        [DataMember]
        public string Mjesto { get; set; }
        [DataMember]
        public string Tel { get; set; }
        [DataMember]
        public string Fax { get; set; }
        [DataMember]
        public string Banka { get; set; }
        [DataMember]
        public string OIB { get; set; }
        [DataMember]
        public string Oznaka { get; set; }
        [DataMember]
        public string RadnoVrijeme { get; set; }
        [DataMember]
        public DateTime? DatumPrimjene { get; set; }
        [DataMember]
        public int PDV { get; set; }
        [DataMember]
        public int Dosipijece { get; set; }
        [DataMember]
        public byte[] Logo { get; set; }
        [DataMember]
        public _Uplatnica Uplatnica { get; set; }
        [DataMember]
        public List<_PostavkeIspisa> Postavke { get; set; }

        public _PoslovniProstor(int idPoslovnogProstora, int? idFisk, int? idVlasnikaFis, int idredarstva, string naziv, string podnaslov, string uSustavu, string web, 
            string email, string opis, string ulica, string broj, string dodatak, string posta, string mjesto, string tel, string fax, string banka,
            string oib, string oznaka, string radnoVrijeme, DateTime? datumPrimjene, int pdv, int dospijece, byte[] logo, _Uplatnica uplatnica, List<_PostavkeIspisa> postavke)
        {
            IDPoslovnogProstora = idPoslovnogProstora;
            IDFisklaizacije = idFisk;
            IDVlasnikaFiskal = idVlasnikaFis;
            IDRedarstva = idredarstva;
            Naziv = naziv;
            Podnaslov = podnaslov;
            USustavu = uSustavu;
            Web = web;
            Email = email;
            Opis = opis;
            Ulica = ulica;
            Broj = broj;
            Dodatak = dodatak;
            Posta = posta;
            Mjesto = mjesto;
            Tel = tel;
            Fax = fax;
            Banka = banka;
            OIB = oib;
            Oznaka = oznaka;
            RadnoVrijeme = radnoVrijeme;
            DatumPrimjene = datumPrimjene;
            PDV = pdv;
            Dosipijece = dospijece;
            Logo = logo;
            Uplatnica = uplatnica;
            Postavke = postavke;
        }
    }

    [DataContract]
    public class _NaplatnoMjesto
    {
        [DataMember]
        public int IDNaplatnogMjesta { get; set; }
        [DataMember]
        public int IDPoslovnogProstora { get; set; }
        [DataMember]
        public string Oznaka { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string Sifra { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string Posta { get; set; }
        [DataMember]
        public string Mjesto { get; set; }
        [DataMember]
        public bool Glavno { get; set; }

        public _NaplatnoMjesto(int idMjesta, int idProstora, string oznaka, string naziv, string sifra, string adresa,
            string posta, string mjesto, bool glavno)
        {
            IDNaplatnogMjesta = idMjesta;
            IDPoslovnogProstora = idProstora;
            Oznaka = oznaka;
            Naziv = naziv;
            Sifra = sifra;
            Adresa = adresa;
            Posta = posta;
            Mjesto = mjesto;
            Glavno = glavno;
        }
    }

    [DataContract]
    public class _PostavkeIspisa
    {
        [DataMember]
        public int IDPostavke { get; set; }
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public int IDVrstePlacanja { get; set; }
        [DataMember]
        public int IDStatusa { get; set; }
        [DataMember]
        public string ZalbaRedarstva { get; set; }
        [DataMember]
        public string OdlukaCjenik { get; set; }
        [DataMember]
        public string OdlukaLisice { get; set; }
        [DataMember]
        public string Paragraf1 { get; set; }
        [DataMember]
        public string Boldano { get; set; }
        [DataMember]
        public string Primjedba { get; set; }
        [DataMember]
        public string Naslov { get; set; }
        [DataMember]
        public string Naredba { get; set; }
        [DataMember]
        public string TemeljniKapital { get; set; }
        [DataMember]
        public string Mjesto { get; set; }
        [DataMember]
        public bool HUB { get; set; }
        [DataMember]
        public bool R1 { get; set; }
        [DataMember]
        public bool ZapisnikRacun { get; set; }
        [DataMember]
        public bool ZapisnikNovaStrana { get; set; }
        [DataMember]
        public bool StavkeNaZapisniku { get; set; }
        [DataMember]
        public bool Privola { get; set; }

        public _PostavkeIspisa(int idPostavke, int idGrada, int idRedarstva, int idVrstePlacanja, int idStatusa,  string zalba, string cjenik, string lisice, string paragraf, string boldano,
            string primjedba, string naslov, string naredba, string kapital, string mjesto, bool hub, bool r1, bool zapirac, bool novastrana, bool stavke, bool privola)
        {
            IDPostavke = idPostavke;
            IDGrada = idGrada;
            IDRedarstva = idRedarstva;
            IDVrstePlacanja = idVrstePlacanja;
            IDStatusa = idStatusa;
            ZalbaRedarstva = zalba;
            OdlukaCjenik = cjenik;
            OdlukaLisice = lisice;
            Paragraf1 = paragraf;
            Boldano = boldano;
            Primjedba = primjedba;
            Naslov = naslov;
            Naredba = naredba;
            TemeljniKapital = kapital;
            Mjesto = mjesto;
            HUB = hub;
            R1 = r1;
            ZapisnikRacun = zapirac;
            ZapisnikNovaStrana = novastrana;
            StavkeNaZapisniku = stavke;
            Privola = privola;
        }
    }

    [DataContract]
    public class _OpisiStavki
    {
        [DataMember]
        public int IDOpisaStavke { get; set; }
        [DataMember]
        public int? IDStatusa { get; set; }
        [DataMember]
        public int? IDZone { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public string NazivOpisaStavke { get; set; }
        [DataMember]
        public string NazivStatusa { get; set; }
        [DataMember]
        public decimal? Iznos { get; set; }
        [DataMember]
        public bool Lezarina { get; set; }
        [DataMember]
        public bool Obrisan { get; set; }
        [DataMember]
        public string Sifra { get; set; }
        [DataMember]
        public string KratkiOpis { get; set; }
        [DataMember]
        public string Zona { get; set; }

        public _OpisiStavki(int idOpisaStavke, int? idStatusa, int? idZone, int idRedarstva, string nazivOpisaStavke, string nazivStatusa, decimal? iznos, bool lezarina, bool obrisan, string sifra, string kratkiOpisa, string zona)
        {
            IDOpisaStavke = idOpisaStavke;
            IDStatusa = idStatusa;
            IDZone = idZone;
            IDRedarstva = idRedarstva;
            NazivOpisaStavke = nazivOpisaStavke;
            NazivStatusa = nazivStatusa;
            Iznos = iznos;
            Lezarina = lezarina;
            Obrisan = obrisan;
            Sifra = sifra;
            KratkiOpis = kratkiOpisa;
            Zona = zona;
        }
    }

    [DataContract]
    public class _Posta
    {
        [DataMember]
        public int IDPoste { get; set; }
        [DataMember]
        public string Posta { get; set; }
        [DataMember]
        public string Mjesto { get; set; }

        public _Posta(int idPoste, string posta, string mjesto)
        {
            IDPoste = idPoste;
            Posta = posta;
            Mjesto = mjesto;
        }
    }

    [DataContract]
    public class _Racun
    {
        [DataMember]
        public int IDRacuna { get; set; }
        [DataMember]
        public int IDReference { get; set; }
        [DataMember]
        public int? IDZakljucenja { get; set; }
        [DataMember]
        public int IDVrste { get; set; }
        [DataMember]
        public int? IDVrsteKartice { get; set; }
        [DataMember]
        public int? IDVrsteBanke { get; set; }
        [DataMember]
        public string NazivVrste { get; set; }
        [DataMember]
        public string NazivBanke { get; set; }
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public string Operater { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }
        [DataMember]
        public int RedniBroj { get; set; }
        [DataMember]
        public int Godina { get; set; }
        [DataMember]
        public decimal PDV { get; set; }
        [DataMember]
        public decimal Osnovica { get; set; }
        [DataMember]
        public decimal Ukupno { get; set; }
        [DataMember]
        public int PDVPosto { get; set; }
        [DataMember]
        public string OIB { get; set; }
        [DataMember]
        public int Blagajna { get; set; }
        [DataMember]
        public string BrojRacuna { get; set; }
        [DataMember]
        public bool Storniran { get; set; }
        [DataMember]
        public string Orginal { get; set; }
        [DataMember]
        public string Napomena { get; set; }
        [DataMember]
        public bool Uplacen { get; set; }
        [DataMember]
        public string JIR { get; set; }
        [DataMember]
        public string ZKI { get; set; }
        [DataMember]
        public string UUID { get; set; }
        [DataMember]
        public DateTime? DatumPreuzimanja { get; set; }
        [DataMember]
        public string OznakaPP { get; set; }
        [DataMember]
        public string PozivNaBr { get; set; }
        [DataMember]
        public string BrojOdobrenja { get; set; }
        [DataMember]
        public string VrsteKartica { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public bool Prenesen { get; set; }
        [DataMember]
        public bool? Rate { get; set; }
        [DataMember]
        public bool BrzaBlagajna { get; set; }
        [DataMember]
        public bool Prilog { get; set; }
        [DataMember]
        public string KlasifikacijaStorna { get; set; }
        [DataMember]
        public List<_Stavka> Stavke { get; set; }
        [DataMember]
        public List<_Osoba> Osobe { get; set; }

        public _Racun(int idracuna, int idreference, int? idzaklj, int idvrste, int? idvrstekartice, int? idvrstebanke, string vrsta, string banka, int iddjelatnika, string operater, int idredarstva, DateTime datumvrijeme,
            int rednibroj, int godina, decimal pdv, decimal osnovica, decimal ukupno, int pdvposto, string oib, int blagajna, string brojracuna, bool storniran, string orginal, string napomena, bool uplacen, string jir, string zki, 
            string uuid, DateTime? datumpreuzimanja, string oznakapp, string poziv, string brojodobrenja, string vrstakartice, string registracija, bool prenesen, bool? rate, bool brzaBlagajna, bool prilog, string klasifikacijaStorna, 
            List<_Stavka> stavke, List<_Osoba> osobe)
        {
            IDRacuna = idracuna;
            IDReference = idreference;
            IDZakljucenja = idzaklj;
            IDVrste = idvrste;
            IDVrsteKartice = idvrstekartice;
            IDVrsteBanke = idvrstebanke;
            NazivVrste = vrsta;
            NazivBanke = banka;
            IDDjelatnika = iddjelatnika;
            Operater = operater;
            IDRedarstva = idredarstva;
            DatumVrijeme = datumvrijeme;
            RedniBroj = rednibroj;
            Godina = godina;
            PDV = pdv;
            Osnovica = osnovica;
            Ukupno = ukupno;
            PDVPosto = pdvposto;
            OIB = oib;
            Blagajna = blagajna;
            BrojRacuna = brojracuna;
            Storniran = storniran;
            Orginal = orginal;
            Napomena = napomena;
            Uplacen = uplacen;
            JIR = jir;
            ZKI = zki;
            UUID = uuid;
            DatumPreuzimanja = datumpreuzimanja;
            OznakaPP = oznakapp;
            PozivNaBr = poziv;
            BrojOdobrenja = brojodobrenja;
            VrsteKartica = vrstakartice;
            Registracija = registracija;
            Prenesen = prenesen;
            Rate = rate;
            BrzaBlagajna = brzaBlagajna;
            Prilog = prilog;
            KlasifikacijaStorna = klasifikacijaStorna;
            Stavke = stavke;
            Osobe = osobe;
            //Prekrsaj = prekrsaj;
        }
    }

    [DataContract]
    public class _Stavka
    {
        [DataMember]
        public int IDStavke { get; set; }
        [DataMember]
        public int IDRacuna { get; set; }
        [DataMember]
        public int IDOpisaStavke { get; set; }
        [DataMember]
        public string OpisStavke { get; set; }
        [DataMember]
        public bool Lezarina { get; set; }
        [DataMember]
        public int Kolicina { get; set; }
        [DataMember]
        public decimal Cijena { get; set; }
        [DataMember]
        public decimal PDV { get; set; }
        [DataMember]
        public decimal Osnovica { get; set; }
        [DataMember]
        public decimal Ukupno { get; set; }
        [DataMember]
        public int PdvPosto { get; set; }
        [DataMember]
        public string Napomena { get; set; }

        public _Stavka(int idStavke, int idRacuna, int idOpisa, string opis, bool lezarina, int kolicina, decimal cijena, decimal pdv, decimal osnovica, decimal ukupno,
            int pdvposto, string napomena)
        {
            IDStavke = idStavke;
            IDRacuna = idRacuna;
            IDOpisaStavke = idOpisa;
            OpisStavke = opis;
            Lezarina = lezarina;
            Kolicina = kolicina;
            Cijena = cijena;
            PDV = pdv;
            Osnovica = osnovica;
            Ukupno = ukupno;
            PdvPosto = pdvposto;
            Napomena = napomena;
        }
    }

    [DataContract]
    public class _Osoba
    {
        [DataMember]
        public int IDOsobe { get; set; }
        [DataMember]
        public string Ime { get; set; }
        [DataMember]
        public string Prezime { get; set; }
        [DataMember]
        public string Ulica { get; set; }
        [DataMember]
        public string KBr { get; set; }
        [DataMember]
        public string Posta { get; set; }
        [DataMember]
        public string Mjesto { get; set; }
        [DataMember]
        public string Drzava { get; set; }
        [DataMember]
        public string OIB { get; set; }
        [DataMember]
        public string Napomena { get; set; }
        [DataMember]
        public string BrojDokumenta { get; set; }
        [DataMember]
        public DateTime? Rodjen { get; set; }
        [DataMember]
        public bool? Vlasnik { get; set; }
        [DataMember]
        public bool MUP { get; set; }

        public _Osoba(int idosobe, string ime, string prezime, string ulica, string kbr, string posta, string mjesto, string drzava, string oib, string napomena, string brojdokumenta, DateTime? rodjen, bool? vlasnik, bool mup)
        {
            IDOsobe = idosobe;
            Ime = ime;
            Prezime = prezime;
            Ulica = ulica;
            KBr = kbr;
            Posta = posta;
            Mjesto = mjesto;
            Drzava = drzava;
            OIB = oib;
            Napomena = napomena;
            BrojDokumenta = brojdokumenta;
            Rodjen = rodjen;
            Vlasnik = vlasnik;
            MUP = mup;
        }
    }

    [DataContract]
    public class _Zakljucenje
    {
        [DataMember]
        public int IDZakljucenja { get; set; }
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public string Djelatnik { get; set; }
        [DataMember]
        public DateTime Vrijeme { get; set; }
        [DataMember]
        public decimal Ukupno { get; set; }
        [DataMember]
        public int Broj { get; set; }
        [DataMember]
        public string Oznaka { get; set; }
        [DataMember]
        public List<int> Racuni { get; set; }

        public _Zakljucenje(int idZakljucenja, int idDjelatnika, int idRedarstva, string djelatnik, DateTime vrijeme, decimal ukupno, int broj, string oznaka, List<int> racuni)
        {
            IDZakljucenja = idZakljucenja;
            IDDjelatnika = idDjelatnika;
            IDRedarstva = idRedarstva;
            Djelatnik = djelatnik;
            Vrijeme = vrijeme;
            Ukupno = ukupno;
            Broj = broj;
            Oznaka = oznaka;
            Racuni = racuni;
        }
    }

    [DataContract]
    public class _Dokument
    {
        [DataMember]
        public int IDDokumenta { get; set; }
        [DataMember]
        public int IDOsobe { get; set; }
        [DataMember]
        public byte[] Dokument { get; set; }

        public _Dokument(int idDokumenta, int idOsobe, byte[] dokument)
        {
            IDDokumenta = idDokumenta;
            IDOsobe = idOsobe;
            Dokument = dokument;
        }
    }

    [DataContract]
    public class _EmailPostavke
    {
        [DataMember]
        public int Timeout { get; set; }
        [DataMember]
        public string Host { get; set; }
        [DataMember]
        public bool Credentials { get; set; }
        [DataMember]
        public bool SSL { get; set; }
        [DataMember]
        public int Port { get; set; }
        [DataMember]
        public string User { get; set; }
        [DataMember]
        public string Pass { get; set; }
        [DataMember]
        public string EmailGrada { get; set; }
        [DataMember]
        public string Posiljatelj { get; set; }
        [DataMember]
        public string Grb { get; set; }
        [DataMember]
        public string Link { get; set; }

        public _EmailPostavke(int time, string host, bool cred, bool ssl, int port, string user, string pass,
            string email, string pos, string grb, string link)
        {
            Timeout = time;
            Host = host;
            Credentials = cred;
            SSL = ssl;
            Port = port;
            User = user;
            Pass = pass;
            EmailGrada = email;
            Posiljatelj = pos;
            Grb = grb;
            Link = link;
        }
    }

    [DataContract]
    public class _Kontakt
    {
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public int IDPrivilegije { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public string ImePrezime { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Napomena { get; set; }

        public _Kontakt(int idDjelatnika, int idPrivilegije, int idRedarstva, int idGrada, string imePrezime, string email, string napomena)
        {
            IDDjelatnika = idDjelatnika;
            IDPrivilegije = idPrivilegije;
            IDRedarstva = idRedarstva;
            IDGrada = idGrada;
            ImePrezime = imePrezime;
            Email = email;
            Napomena = napomena;
        }
    }

    [DataContract]
    public class _Uplatnica
    {
        [DataMember]
        public int IDUplatnice { get; set; }
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string Model { get; set; }
        [DataMember]
        public string IBAN { get; set; }
        [DataMember]
        public string Poziv1 { get; set; }
        [DataMember]
        public string Poziv2 { get; set; }
        [DataMember]
        public string Opis { get; set; }
        [DataMember]
        public string Swift { get; set; }
        [DataMember]
        public string Sifra { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string UlicaBroj { get; set; }
        [DataMember]
        public string Posta { get; set; }
        [DataMember]
        public string Mjesto { get; set; }

        public _Uplatnica(int iduplatnice, int idgrada, int idredarstva, string adresa, string model, string iban, string poziv1, string poziv2,
            string opis, string swift, string sifra, string naziv, string ulica, string posta, string mjesto)
        {
            IDUplatnice = iduplatnice;
            IDGrada = idgrada;
            IDRedarstva = idredarstva;
            Adresa = adresa;
            Model = model;
            IBAN = iban;
            Poziv1 = poziv1;
            Poziv2 = poziv2;
            Opis = opis;
            Swift = swift;
            Sifra = sifra;
            Naziv = naziv;
            UlicaBroj = ulica;
            Posta = posta;
            Mjesto = mjesto;
        }
    }

    [DataContract]
    public class _Prijenos
    {
        [DataMember]
        public int BrRacuna { get; set; }
        [DataMember]
        public int BrKartica { get; set; }
        [DataMember]
        public int BrGotovina { get; set; }
        [DataMember]
        public int BrOdgoda { get; set; }
        [DataMember]
        public decimal Ukupno { get; set; }
        [DataMember]
        public decimal Kartice { get; set; }
        [DataMember]
        public decimal Gotovina { get; set; }
        [DataMember]
        public decimal Odgoda { get; set; }

        public _Prijenos(int brracuna, int brkartica, int brgotovina, int brodgoda, decimal ukupno, decimal kartice,
            decimal gotovina, decimal odgoda)
        {
            BrRacuna = brracuna;
            BrKartica = brkartica;
            BrGotovina = brgotovina;
            BrOdgoda = brodgoda;
            Ukupno = ukupno;
            Kartice = kartice;
            Gotovina = gotovina;
            Odgoda = odgoda;
        }
    }

    [DataContract]
    public class _PovijestPrijenosa
    {
        [DataMember]
        public int IDPrijenosa { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public int Broj { get; set; }
        [DataMember]
        public string Poruka { get; set; }
        [DataMember]
        public bool Uspjesno { get; set; }

        public _PovijestPrijenosa(int IdPrijenosa, DateTime datum, string id, int broj, string poruka, bool uspjesno)
        {
            IDPrijenosa = IdPrijenosa;
            Datum = datum;
            ID = id;
            Broj = broj;
            Poruka = poruka;
            Uspjesno = uspjesno;
        }
    }

    /*:: MUP ::*/

    [DataContract]
    public class _VoziloMUP
    {
        [DataMember]
        public string RegistarskaOznaka { get; set; }
        [DataMember]
        public string MarkaVozila { get; set; }
        [DataMember]
        public string VrstaVozila { get; set; }

        public _VoziloMUP(string registarskaOznaka, string markaVozila, string vrstaVozila)
        {
            RegistarskaOznaka = registarskaOznaka;
            MarkaVozila = markaVozila;
            VrstaVozila = vrstaVozila;
        }
    }

    [DataContract]
    public class _VlasnikMUP
    {
        [DataMember]
        public string IndikatorVK { get; set; }
        [DataMember]
        public string OIBVlasnika { get; set; }
        [DataMember]
        public string ImeVlasnika { get; set; }
        [DataMember]
        public string PrezimeVlasnika { get; set; }
        [DataMember]
        public string NazivVlasnika { get; set; }
        [DataMember]
        public string Drzava { get; set; }
        [DataMember]
        public string Opcina { get; set; }
        [DataMember]
        public string Mjesto { get; set; }
        [DataMember]
        public string UlicaBroj { get; set; }

        public _VlasnikMUP(string indikatorVK, string oIBVlasnika, string imeVlasnika, string prezimeVlasnika, string nazivVlasnika,
                string drzava, string opcina, string mjesto, string ulicaBroj)
        {
            IndikatorVK = indikatorVK;
            OIBVlasnika = oIBVlasnika;
            ImeVlasnika = imeVlasnika;
            PrezimeVlasnika = prezimeVlasnika;
            NazivVlasnika = nazivVlasnika;
            Drzava = drzava;
            Opcina = opcina;
            Mjesto = mjesto;
            UlicaBroj = ulicaBroj;
        }
        public _VlasnikMUP()
        {
            IndikatorVK = null;
            OIBVlasnika = null;
            ImeVlasnika = null;
            PrezimeVlasnika = null;
            NazivVlasnika = null;
            Drzava = null;
            Opcina = null;
            Mjesto = null;
            UlicaBroj = null;
        }
    }

    [DataContract]
    public class _OdgovorMUPVozilo
    {
        [DataMember]
        public _VoziloMUP Vozilo { get; set; }
        [DataMember]
        public List<_VlasnikMUP> Vlasnik { get; set; }

        public _OdgovorMUPVozilo(_VoziloMUP vozilo, List<_VlasnikMUP> vlasnik)
        {
            Vozilo = vozilo;
            Vlasnik = vlasnik;
        }

        public _OdgovorMUPVozilo(_VoziloMUP vozilo)
        {
            Vozilo = vozilo;
            Vlasnik = null;
        }

        public _OdgovorMUPVozilo()
        {
            Vozilo = null;
            Vlasnik = null;
        }
    }

    [DataContract]
    public class _Prijava
    {
        [DataMember]
        public int IDVanjskePrijave { get; set; }
        [DataMember]
        public int IDIzvora { get; set; }
        [DataMember]
        public string Izvor { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public int? IDPrekrsaja { get; set; }
        [DataMember]
        public int? IDNaloga { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public decimal Latitude { get; set; }
        [DataMember]
        public decimal Longitude { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string Opis { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public DateTime Vrijeme { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public bool Pregledano { get; set; }
        [DataMember]
        public bool? Odbijen { get; set; }
        [DataMember]
        public string Napomena { get; set; }

        public _Prijava(int idVanjske, int idIzvora, string izvor, int idRedarstva, int idLokacije, int? idNaloga, int? idPrekrsaja, string status, decimal latitude, decimal longitude, 
            string adresa, string opis, string registracija, DateTime vrijeme, bool pregledano, bool? odbijen, string napomena)
        {
            IDVanjskePrijave = idVanjske;
            IDIzvora = idIzvora;
            Izvor = izvor;
            IDRedarstva = idRedarstva;
            IDLokacije = idLokacije;
            IDNaloga = idNaloga;
            IDPrekrsaja = idPrekrsaja;
            Status = status;
            Latitude = latitude;
            Longitude = longitude;
            Adresa = adresa;
            Opis = opis;
            Registracija = registracija;
            Vrijeme = vrijeme;
            Datum = vrijeme.Date;
            Pregledano = pregledano;
            Odbijen = odbijen;
            Napomena = napomena;
        }
    }

    [DataContract]
    public class _EvidencijaPlacanja
    {
        [DataMember]
        public int Redoslijed { get; set; }
        [DataMember]
        public int IDVrstePlacanja { get; set; }
        [DataMember]
        public string VrstaPlacanja { get; set; }
        [DataMember]
        public string Kratica { get; set; }
        [DataMember]
        public string Opis { get; set; }
        [DataMember]
        public int Broj { get; set; }
        [DataMember]
        public decimal Iznos { get; set; }
        [DataMember]
        public List<_EvidencijaPlacanja> Detalji { get; set; }

        public _EvidencijaPlacanja(int redoslijed, int idVrste, string vrsta, string kratica, string opis, int broj, decimal iznos, List<_EvidencijaPlacanja> detalji)
        {
            Redoslijed = redoslijed;
            IDVrstePlacanja = idVrste;
            VrstaPlacanja = vrsta;
            Kratica = kratica;
            Opis = opis;
            Broj = broj;
            Iznos = iznos;
            Detalji = detalji;
        }
    }

    [DataContract]
    public class _DnevniUtrzak
    {
        [DataMember]
        public int IDDnevnika { get; set; }
        [DataMember]
        public string Sifra { get; set; }
        [DataMember]
        public string Opis { get; set; }
        [DataMember]
        public decimal Primitak { get; set; }
        [DataMember]
        public decimal Izdatak { get; set; }

        public _DnevniUtrzak(int iddnevnika, string sifra, string opis, decimal primitak, decimal izdatak)
        {
            IDDnevnika = iddnevnika;
            Sifra = sifra;
            Opis = opis;
            Primitak = primitak;
            Izdatak = izdatak;
        }
    }
}
