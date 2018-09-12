using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Xml.Linq;
using PG.Servisi.resources.cs.email;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi
{
    [ServiceBehavior(MaxItemsInObjectGraph = int.MaxValue)]
    public class PGAdministracija : IPGAdministracija
    {
        private const int idAplikacije = 2;

        public bool Dostupan()
        {
            return true;
        }

        public int KreirajNoviGrad(_Grad grad)
        {
            return Gradovi.KreirajNoviGrad(grad, idAplikacije);
        }

        public _Grad AktivacijaAplikacije(string aktivacijskiKod)
        {
            return Sustav.AktivacijaAplikacije(aktivacijskiKod, idAplikacije);
        }

        public List<_3DLista> Redari(string grad)
        {
            return Korisnici.Redari(grad, 1, idAplikacije);
        }

        public List<_2DLista> Redarstva()
        {
            return Postavke.Redarstva(idAplikacije);
        }

        public List<_2DLista> Aplikacije()
        {
            return Sustav.Aplikacije(idAplikacije);
        }

        public List<_Drzava> Drzave()
        {
            return Sustav.Drzave(idAplikacije);
        }

        public List<_2DLista> PopisPredlozaka(string grad)
        {
            return Predlosci.PopisPredlozaka(grad, 1, idAplikacije);
        }

        public List<_2DLista> PopisGradova()
        {
            return Gradovi.PopisGradova(1, idAplikacije);
        }

        public List<_2DLista> PopisVozila(string grad)
        {
            return Vozila.PopisVozila(grad, idAplikacije);
        }

        public _Grad Grad(string grad)
        {
            return Gradovi.Grad(grad, idAplikacije);
        }

        public bool PostavkeGrada(string grad, _Grad postavke)
        {
            return Gradovi.PostavkeGrada(grad, postavke, idAplikacije);
        }

        public bool IzmjeniMapu(string grad, string mapa)
        {
            return Gradovi.IzmjeniMapu(grad, mapa, idAplikacije);
        }

        public _Vozilo Vozilo(string grad, int idVozila)
        {
            return Vozila.Vozilo(grad, idVozila, idAplikacije);
        }

        /*: GO ::*/

        public List<_3DLista> GradoviGO()
        {
            return GO.GradoviGO(idAplikacije);
        }

        public List<_3DLista> GrupeGO(string grad)
        {
            return GO.GrupeGO(grad, idAplikacije);
        }

        /*:: NALOG ZA PLACANJE ::*/

        #region NALOG

        public _Uplatnica Uplatnica(string grad, int idRedarstva)
        {
            return Gradovi.Uplatnica(grad, idRedarstva, idAplikacije);
        }

        public int IzmjeniUplatnicu(string grad, _Uplatnica nalog)
        {
            return Gradovi.IzmjeniUplatnicu(grad, nalog, idAplikacije);
        }

        public _PoslovniProstor DohvatiPoslovniProstor(string grad, int idRedarstva)
        {
            return PoslovniProstor.DohvatiPoslovniProstor(grad, idRedarstva, idAplikacije);
        }

        #endregion

        /*:: PODRSKA ::*/

        #region PODRSKA

        public bool PrijaviProblem(string grad, _Problem problem, List<byte[]> slike)
        {
            return Sustav.PrijaviProblem(grad, problem, slike, idAplikacije);
        }

        public bool NaruciMaterijal(string grad, _Narudzba narudzba)
        {
            return Sustav.NaruciMaterijal(grad, narudzba, idAplikacije);
        }

        public bool PostaviPitanje(string grad, int idKorisnika, int idPodrucja, string poruka, List<byte[]> slike)
        {
            return Sustav.PostaviPitanje(grad, idKorisnika, idPodrucja, poruka, slike, idAplikacije);
        }

        public List<_Prekrsaj> PrekrsajiIzvoz(string grad, int idDjelatnika, int idPredloska, DateTime? datumOd, DateTime? datumDo, bool storno)
        {
            return Pretraga.PrekrsajiIzvoz(grad, idDjelatnika, idPredloska, datumOd, datumDo, storno, 1, idAplikacije);
        }

        #endregion

        #region KONTAKTIRANJE KORISNIKA

        public bool DodajPredlozakEmaila(string grad, string naziv, string predlozak)
        {
            return Sustav.DodajPredlozak(grad, naziv, predlozak, idAplikacije);
        }

        public bool ObrisiPredlozakEmaila(string grad, int idPredloska)
        {
            return Sustav.ObrisiPredlozakEmaila(grad, idPredloska, idAplikacije);
        }

        public List<_3DLista> DohvatiPredloskeEmaila(string grad)
        {
            return Sustav.DohvatiPredloskeEmaila(grad, idAplikacije);
        }

        public _EmailPostavke PostavkeEmaila(string grad)
        {
            return Sustav.PostavkeEmaila(grad, idAplikacije);
        }

        public void Posalji(List<string> brojevi, string poruka)
        {
            try
            {
                foreach (var b in brojevi)
                {
                    SlanjeSMS.PosaljiSMS(b, poruka);
                    Thread.Sleep(500);
                }
            }
            catch
            {

            }
        }

        #endregion

        /*:: AKTIVNOST ::*/

        #region AKTIVNOST

        public bool Aktivan(string grad, string korisnik, string racunalo, string verzija, string os, bool odobrava, int idKorisnika)
        {
            bool reset = Sustav.Aktivnost(grad, idKorisnika, racunalo, verzija, korisnik, os, odobrava, idAplikacije);

            if (reset)
            {
                Sustav.AktivnostReset(grad, racunalo, idAplikacije);
            }

            return reset;
        }

        public bool TrenutnoAktivan(string grad, int idKorisnika)
        {
            return Sustav.Aktivan(grad, idKorisnika, idAplikacije);
        }

        public List<_AktivneAplikacije> DohvatiAktivne(string grad, bool aktivni)
        {
            return Sustav.DohvatiAktivne(grad, aktivni, idAplikacije);
        }

        public bool ObrisiAktivnost(string grad, int idAktivnosti)
        {
            return Sustav.ObrisiAktivnost(grad, idAktivnosti, idAplikacije);
        }

        public void Reset(string grad, int idAktivnosti)
        {
            Sustav.Reset(grad, idAktivnosti, idAplikacije);
        }

        public void SpremiError(string grad, Exception greska, string napomena, string korisnik)
        {
            Sustav.SpremiGresku(grad, greska, idAplikacije, napomena, korisnik);
        }

        #endregion

        /*:: DJELATNICI ::*/

        #region DJELATNICI

        public bool PosaljiEmailDdobrodoslice(string grad, int idKorisnika)
        {
            return Korisnici.PosaljiEmailDdobrodoslice(grad, idKorisnika, idAplikacije);
        }

        public _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS)
        {
            return Korisnici.Prijava(grad, korisnickoIme, zaporka, 1, out blokiranaJLS, idAplikacije);
        }

        public int DodajNovogDjelatnika(string grad, _Djelatnik korisnik, bool email)
        {
            return Korisnici.DodajNovogDjelatnika(grad, korisnik, email, idAplikacije);
        }

        public bool ObrisiDjelatnika(string grad, int idKorisnika)
        {
            return Korisnici.ObrisiDjelatnika(grad, idKorisnika, idAplikacije);
        }

        public bool IzmjeniDjelatnika(string grad, _Djelatnik korisnik)
        {
            return Korisnici.IzmjeniDjelatnika(grad, korisnik, idAplikacije);
        }

        public List<_Djelatnik> DohvatiDjelatnike(string grad)
        {
            return Korisnici.DohvatiDjelatnike(grad, -1, idAplikacije);
        }

        public List<_Chat> DohvatiDjelatnikeChat(string grad, int idDjelatnika)
        {
            return Korisnici.DohvatiDjelatnikeChat(grad, idDjelatnika, idAplikacije);
        }

        public List<_2DLista> DohvatiPrivilegije(string grad)
        {
            return Korisnici.DohvatiPrivilegije(grad, idAplikacije);
        }

        public bool IzmjeniZaporku(string grad, int idKorisnika, string zaporka)
        {
            return Korisnici.IzmjeniZaporku(grad, idKorisnika, zaporka, false, idAplikacije);
        }

        public bool IzmjeniOtisak(string grad, int idKorisnika, byte[] otisak)
        {
            return Korisnici.IzmjeniOtisak(grad, idKorisnika, otisak, idAplikacije);
        }

        public bool IzmjeniGradoveDjelatnika(string grad, int idKorisnika, List<int> gradovi)
        {
            return Korisnici.IzmjeniGradoveDjelatnika(grad, idKorisnika, gradovi, idAplikacije);
        }

        //public void Kontakti()
        //{
        //    Korisnici.Kontakti();
        //}

        public List<_Kontakt> DohvatiKontakte(string grad)
        {
            return Korisnici.DohvatiKontakte(grad, idAplikacije);
        }

        public List<_Kontakt> DohvatiSMS(string grad)
        {
            return Korisnici.DohvatiSMS(grad, idAplikacije);
        }

        /*:: GO ::*/

        public int PoveziSaGO(string grad, int idKorisnika)
        {
            return Korisnici.PoveziSaGO(grad, idKorisnika, idAplikacije);
        }

        #endregion

        #region PRIVILEGIJE

        public List<_Privilegije> Privilegije(string grad)
        {
            return Korisnici.Privilegije(grad, idAplikacije);
        }

        public List<_Dozvola> DetaljiPrivilegije(string grad, int idKorisnika, int idPrivilegije)
        {
            return Korisnici.DetaljiPrivilegije(grad, idKorisnika, idPrivilegije, idAplikacije);
        }

        public bool IzmjeniDetaljePrivilegija(string grad, int idKorisnika, List<int> dodijeljene)
        {
            return Korisnici.IzmjeniDetaljePrivilegija(grad, idKorisnika, dodijeljene, idAplikacije);
        }

        public bool ResetirajPrivilegije(string grad, int idKorisnika)
        {
            return Korisnici.ResetirajPrivilegije(grad, idKorisnika, idAplikacije);
        }

        public bool Zabrani(string grad, int idDjelatnika, int idDozvole, bool dozvoljeno)
        {
            return Korisnici.Zabrani(grad, idDjelatnika, idDozvole, dozvoljeno, idAplikacije);
        }

        #endregion

        /*:: AKCIJE KORISNIKA ::*/

        #region AKCIJE KORISNIKA

        public void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena)
        {
            Sustav.SpremiAkciju(grad, idKorisnika, idAkcije, napomena, 1, idAplikacije);
        }

        public List<_Akcija> DohvatiAkcije(string grad, int? idDjelatnika, int? idAkcije, DateTime? odDatum, DateTime? doDatum, int idprivilegije, int? idRedarstva)
        {
            return Sustav.DohvatiAkcije(grad, idDjelatnika, idAkcije, odDatum, doDatum, idprivilegije, idRedarstva, idAplikacije);
        }

        public List<_2DLista> DohvatiVrsteAkcija(string grad)
        {
            return Sustav.DohvatiVrsteAkcija(grad, idAplikacije);
        }

        public bool ObrisiAkciju(string grad, int idAkcije)
        {
            return Sustav.ObrisiAkciju(grad, idAkcije, idAplikacije);
        }

        #endregion

        /*:: ZAKONI ::*/

        #region ZAKONI - TRENUTNO VADI IZ STARE BAZE PREBACITI NA NOVO

        public List<_2DLista> DohvatiJezike(string grad)
        {
            return Zakoni.DohvatiJezike(grad, idAplikacije);
        }

        public List<_Zakon> DohvatiZakone(string grad, int idRedarstva, bool neaktivni)
        {
            return Zakoni.DohvatiZakoneS(grad, neaktivni, idRedarstva, true, idAplikacije);
        }

        public List<_Opis> DohvatiOpiseZakona(string grad, int idPrekrsaja)
        {
            return Zakoni.DohvatiOpiseZakonaS(grad, idPrekrsaja, true, idAplikacije);
        }

        public int DodajZakon(string grad, _Zakon novi)
        {
            return Zakoni.DodajZakonS(grad, novi, 1, idAplikacije);
        }

        public int DodajKratkiOpis(string grad, _Opis opis)
        {
            return Zakoni.DodajKratkiOpisS(grad, opis, idAplikacije);
        }

        public bool SpremiPrijevod(string grad, int idJezika, int idZakona, int idOpisa, string clanak, string opis)
        {
            return Zakoni.SpremiPrijevodS(grad, idJezika, idZakona, idOpisa, clanak, opis, idAplikacije);
        }

        public bool IzmjeniZakon(string grad, _Zakon zakon, _Opis opis)
        {
            return Zakoni.IzmjeniZakonS(grad, zakon, opis, idAplikacije);
        }

        #endregion

        /*:: PREDLOŠCI ::*/

        #region PREDLOŠCI

        public List<_2DLista> JeziciPredlozaka(string grad)
        {
            return Predlosci.JeziciPredlozaka(grad, idAplikacije);
        }

        public List<_Predlozak> PredlosciIspisa(string grad)
        {
            return Predlosci.PredlosciIspisa(grad, 0, idAplikacije);
        }

        public bool ObrisiPredlozak(string grad, int idPredloska, int idJezika)
        {
            return Predlosci.ObrisiPredlozak(grad, idPredloska, idJezika, idAplikacije);
        }

        public bool IzmijeniPredlozak(string grad, int idPredloska, int idJezika, string nazivPredloska, bool pauk, bool kaznjava, XElement tekstPredloska)
        {
            return Predlosci.IzmijeniPredlozak(grad, idPredloska, idJezika, nazivPredloska, pauk, kaznjava, tekstPredloska, idAplikacije);
        }

        public int DodajPredlozak(string grad, string nazivPredloska, bool pauk, bool kaznjava, int idRedarstva, XElement tekstPredloska)
        {
            return Predlosci.DodajPredlozak(grad, nazivPredloska, pauk, kaznjava, tekstPredloska, idRedarstva, idAplikacije);
        }

        public _Predlozak DohvatiPredlozakIspisa(string grad, string koji)
        {
            return Predlosci.DohvatiPredlozakIspisa(grad, koji, idAplikacije);
        }

        public _Predlozak DohvatiPredlozak(string grad, int idPredloska)
        {
            return Predlosci.DohvatiPredlozakIspisa(grad, idPredloska, idAplikacije);
        }

        #endregion

        #region ISPIS PREDLOŽAKA

        public bool IspisPredloska(string grad, string detaljiKazne, int idPredloska, out string ispis)
        {
            return Ispis.IspisPredloska(grad, detaljiKazne, 1, idPredloska, 0, out ispis, false, idAplikacije);
        }

        public bool IspisPredloska1(string baza, string detaljiKazne, int qty, int idPredloska, int idJezika, out string ispis)
        {
            return Ispis.IspisPredloska(baza, detaljiKazne, qty, idPredloska, idJezika, out ispis, false, idAplikacije);
        }

        #endregion

        /*:: PORUKE ::*/

        #region PORUKE

        public List<_Poruka> DohvatiPoruke(string grad, int idDjelatnika, bool pauk)
        {
            return Chat.DohvatiPoruke(grad, idDjelatnika, pauk, idAplikacije);
        }

        public bool PromijeniStatusPoruke(string grad, int idPrimatelja, int idPosiljatelja)
        {
            return Chat.PromijeniStatusPoruke(grad, idPrimatelja, idPosiljatelja, idAplikacije);
        }

        public bool ImaNeprocitanihPoruka(string grad, int idPrimatelja, out bool vazno, out int brojPoruka)
        {
            return Chat.ImaNeprocitanihPoruka(grad, idPrimatelja, out vazno, out brojPoruka, idAplikacije);
        }

        public int PosaljiPoruku(string grad, _Poruka poruka)
        {
            return Chat.PosaljiPoruku(grad, poruka, 1, idAplikacije);
        }

        public bool ObrisiPoruku(string grad, int IDPoruku, bool Primljena)
        {
            return Chat.ObrisiPoruku(grad, IDPoruku, Primljena, idAplikacije);
        }

        public int Neprocitanih(string grad, int idPrimatelja, int idPosiljatelja, out bool aktivan)
        {
            return Chat.Neprocitanih(grad, idPrimatelja, idPosiljatelja, out aktivan, idAplikacije);
        }

        #endregion

        /*:: ADMINISTRACIJA ULICA ::*/

        #region ADMINISTRACIJA ULICA

        public List<_LokalneAdrese> PopisUlica(string grad)
        {
            return Sustav.PopisUlica(grad, idAplikacije);
        }

        public bool ObrisiLokalnuAdresu(string grad, int idAdrese)
        {
            return Sustav.ObrisiLokalnuAdresu(grad, idAdrese, idAplikacije);
        }

        public bool ObrisiGrupuUlica(string grad, string ulica)
        {
            return Sustav.ObrisiGrupuUlica(grad, ulica, idAplikacije);
        }

        public bool IzmjeniLokalnuAdresu(string grad, int idAdrese, string adresa, string kbr)
        {
            return Sustav.IzmjeniLokalnuAdresu(grad, idAdrese, adresa, kbr, idAplikacije);
        }

        public bool IzmjeniGrupuUlica(string grad, string ulica, string novaulica)
        {
            return Sustav.IzmjeniGrupuUlica(grad, ulica, novaulica, idAplikacije);
        }

        #endregion

        /*:: KAMERE ::*/

        #region WEB KAMERE

        public bool ImaKamere(string grad)
        {
            return Sustav.ImaKamere(grad, idAplikacije);
        }

        public int DodajKameru(string grad, _WebKamere kamera)
        {
            return Sustav.DodajKameru(grad, kamera, idAplikacije);
        }

        public bool IzmijeniKameru(string grad, _WebKamere kamera)
        {
            return Sustav.IzmijeniKameru(grad, kamera, idAplikacije);
        }

        public bool ObrisiKameru(string grad, int idKamere)
        {
            return Sustav.ObrisiKameru(grad, idKamere, idAplikacije);
        }

        public List<_WebKamere> DohvatiKamere(string grad)
        {
            return Sustav.DohvatiKamere(grad, idAplikacije);
        }

        #endregion

        /*:: PREKRŠAJ GLAVNI MENU ::*/

        #region PREKRSAJ

        public List<_Prekrsaj> PretraziPrekrsaje(string grad, int idDjelatnika, DateTime datum, bool obavijesti, bool upozorenja)
        {
            return Prekrsaj.PretraziPrekrsaje(grad, idDjelatnika, datum, 1, idAplikacije);
        }

        public _Prekrsaj DetaljiPrekrsaja(string grad, int idLokacije)
        {
            return Prekrsaj.DetaljiPrekrsaja(grad, idLokacije, idAplikacije);
        }

        /*:: SLIKE ::*/

        public List<byte[]> Slike(string grad, int idLokacije)
        {
            return Prekrsaj.Slike(grad, idLokacije, idAplikacije);
        }

        public List<int> DodajSliku(string grad, int idLokacije, List<byte[]> slike)
        {
            return Prekrsaj.DodajSliku(grad, idLokacije, slike, 1, idAplikacije);
        }

        public bool ObrisiSliku(string grad, int idSlike)
        {
            return Prekrsaj.ObrisiSliku(grad, idSlike, idAplikacije);
        }

        public List<_Slika> SlikePrekrsaja(string grad, int idLokacije)
        {
            return Prekrsaj.SlikePrekrsaja(grad, idLokacije, idAplikacije);
        }

        public int RotirajSliku(string grad, int idLokacije, byte[] slike)
        {
            return Prekrsaj.RotirajSliku(grad, idLokacije, slike, idAplikacije);
        }

        /*:: MAPA ::*/

        public List<_Tocka> StvaranjePutanjeRedara(string grad, DateTime datum, string vrijeme, int idRedara)
        {
            return Mapa.StvaranjePutanjeRedara(grad, datum, vrijeme, idRedara, 30, 0, idAplikacije);
        }

        public List<_Prekrsaj> PozicijePrekrsaja(string grad, int idZaposlenika, DateTime datum, string vrijeme)
        {
            return Mapa.PozicijePrekrsaja(grad, idZaposlenika, datum, vrijeme, 1, idAplikacije);
        }

        public List<_Pozicija> PozicijeRedara(string grad, int minuta)
        {
            return Mapa.PozicijeRedara(grad, minuta, 1, idAplikacije);
        }

        public _Pozicija PozicijaOdabranogRedara(string grad, int idDjelatnika)
        {
            return Mapa.PozicijaOdabranogRedara(grad, idDjelatnika, idAplikacije);
        }

        public List<_Pozicija> TrenutnePozicijeVozila(string grad, int minuta, int? idVozila)
        {
            return Mapa.TrenutnePozicijeVozila(grad, minuta, idVozila, idAplikacije);
        }

        public List<_WebKamere> PozicijeKamera(string grad)
        {
            return Mapa.PozicijeKamera(grad, idAplikacije);
        }

        /*:: RUČNO DODAVANJE PREKRSAJA ::*/

        public List<string> BrojPrekrsaja(string grad, string registracija)
        {
            return Prekrsaj.BrojPrekrsaja(grad, registracija, idAplikacije);
        }

        public int DodajRucniPrekrsaj(string grad, _Prekrsaj prekrsaj, bool obavijest, List<byte[]> slike, bool lisice)
        {
            return Prekrsaj.DodajRucniPrekrsaj(grad, prekrsaj, obavijest, slike, 1, lisice, idAplikacije);
        }

        /*:: IZMJENE PREKRSAJA ::*/

        public bool Prenesen(string grad, int idPrekrsaja)
        {
            return Prekrsaj.Prenesen(grad, idPrekrsaja, idAplikacije);
        }

        public bool RelokacijaPrekrsaja(string grad, int idPrekrsaja, decimal latitude, decimal longitude)
        {
            return Prekrsaj.RelokacijaPrekrsaja(grad, idPrekrsaja, latitude, longitude, false, idAplikacije);
        }

        public bool Storniraj(string grad, int idPrekrsaja, string napomena, string osoba)
        {
            return Prekrsaj.Storniraj(grad, idPrekrsaja, napomena, osoba, idAplikacije);
        }

        public bool Test(string grad, int idPrekrsaja, bool test)
        {
            return Prekrsaj.Test(grad, idPrekrsaja, test, idAplikacije);
        }

        public bool Registracija(string grad, int idPrekrsaja, string registracija, string kratica)
        {
            return Prekrsaj.Registracija(grad, idPrekrsaja, registracija, kratica, false, idAplikacije);
        }

        public bool Adresa(string grad, int idPrekrsaja, string adresa)
        {
            return Prekrsaj.Adresa(grad, idPrekrsaja, adresa, idAplikacije);
        }

        public string BrojDokumenta(string grad, _Prekrsaj prekrsaj)
        {
            return Prekrsaj.BrojDokumenta(grad, prekrsaj, idAplikacije);
        }

        public int Vrsta(string grad, _Prekrsaj prekrsaj, string broj)
        {
            return Prekrsaj.Vrsta(grad, prekrsaj, broj, idAplikacije);
        }

        public string ObrisiSveStornirane(string grad)
        {
            return Prekrsaj.ObrisiSveStornirane(grad, idAplikacije);
        }

        public bool IzmjeniZakonPrekrsaja(string grad, int idPrekrsaja, int idOpisa, decimal kazna, out bool dodan)
        {
            return Prekrsaj.IzmjeniZakonPrekrsaja(grad, idPrekrsaja, idOpisa, kazna, 1, out dodan, idAplikacije);
        }

        public int NalogPauku(string grad, int idPrekrsaja, DateTime datum, bool lisice)
        {
            return Prekrsaj.NalogPauku(grad, idPrekrsaja, datum, lisice, "", idAplikacije);
        }

        #endregion

        /*:: SUSTAV ::*/

        #region POSEBNA VOZILA

        public List<_Odobrenja> DohvatiOdobrenja(string grad)
        {
            return Postavke.DohvatiOdobrenja(grad, 1, idAplikacije);
        }

        public bool ObrisiOdobrenje(string grad, int idOdobrenja)
        {
            return Postavke.ObrisiOdobrenje(grad, idOdobrenja, idAplikacije);
        }

        public int DodajOdobrenje(string grad, _Odobrenja odobrenje)
        {
            return Postavke.DodajOdobrenje(grad, odobrenje, idAplikacije);
        }

        public bool PromijeniStatusOdobrenja(string grad, int idOdobrenja, bool suspendirano)
        {
            return Postavke.PromijeniStatusOdobrenja(grad, idOdobrenja, suspendirano, idAplikacije);
        }

        public bool IzmijeniOdobrenje(string grad, _Odobrenja odobrenje)
        {
            return Postavke.IzmijeniOdobrenje(grad, odobrenje, idAplikacije);
        }

        #endregion

        #region PRINTERI

        public List<_Printer> DohvatiPrintere(string grad, bool svi, int idRedarstva)
        {
            return Postavke.DohvatiPrintere(grad, svi, idRedarstva, idAplikacije);
        }

        public bool ObrisiPrinter(string grad, int idPrintera)
        {
            return Postavke.ObrisiPrinter(grad, idPrintera, idAplikacije);
        }

        public int DodajPrinter(string grad, _Printer printer)
        {
            return Postavke.DodajPrinter(grad, printer, "", idAplikacije);
        }

        public bool IzmjeniPrinter(string grad, _Printer printer)
        {
            return Postavke.IzmjeniPrinter(grad, printer, null, idAplikacije);
        }

        /*:: POVIJEST ::*/

        public void PovijestPrintera(int idPrintera, string status, string napomena)
        {
            Oprema.PovijestOpreme(idPrintera, 2, 2, napomena, DateTime.Now, idAplikacije);
        }

        public List<_PovijestOpreme> DohvatiPovijestPrintera(int idPrintera)
        {
            return Oprema.DohvatiPovijestOpreme(idPrintera, 2, 0, idAplikacije);
        }

        #endregion

        /*:: PRETRAGA ::*/

        #region  PRETRAGA

        public List<_Prekrsaj> ZabiljezeniPrekrsaji(string grad, int idDjelatnika, int idPredloska, DateTime? datumOd, DateTime? datumDo, bool pauk,
            bool registracija, bool dokument, bool ulica, bool storno, char? tipStorna, string pojam, bool test, bool hr)
        {
            return Pretraga.ZabiljezeniPrekrsaji(grad, idDjelatnika, idPredloska, datumOd, datumDo, pauk, registracija, dokument, ulica, storno,
                tipStorna, pojam, test, hr, 1, idAplikacije);
        }

        /*:: PONAVLJACI ::*/

        public List<_Prekrsaj> DetaljiPonavljaca(string grad, int idPredloska, DateTime? datumOd, DateTime? datumDo, string registracija)
        {
            return Pretraga.DetaljiPonavljaca(grad, idPredloska, datumOd, datumDo, registracija, false, 1, idAplikacije);
        }

        public List<_2DLista> Ponavljaci(string grad, int idPredloska, DateTime? datumOd, DateTime? datumDo, int broj)
        {
            return Pretraga.Ponavljaci(grad, idPredloska, datumOd, datumDo, broj, false, 1, idAplikacije);
        }

        /*:: VREMENA PAUKA ::*/

        public List<_VremenaPauka> VremenaPauka(string grad, DateTime DatumOd, DateTime DatumDo)
        {
            return Pretraga.VremenaPauka(grad, DatumOd, DatumDo, idAplikacije);
        }

        /*:: ZAHTJEVI ::*/

        public List<_ZahtjevPauka> Zahtjevi(string grad, int idVozila, int idStatusa, DateTime? datumOd, DateTime? datumDo, int idRedarstva)
        {
            return Zahtjev.Zahtjevi(grad, idVozila, idStatusa, datumOd, datumDo, idRedarstva, idAplikacije);
        }

        public bool PonoviZahtjev(string grad, int idZahtjeva)
        {
            return Zahtjev.PonoviZahtjev(grad, idZahtjeva, idAplikacije);
        }

        #endregion

        /*:: STATISTIKA ::*/

        #region STATISTIKA

        public _Aktivnost Aktivnost(string grad, DateTime datum)
        {
            return Statistika.Aktivnost(grad, datum, 1, idAplikacije);
        }

        public List<_Statistika> IzdanihObavijesti(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.IzdanihObavijesti(grad, datumOd, datumDo, idAplikacije);
        }

        public List<_Statistika> NaloziPauku(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.NaloziPauku(grad, datumOd, datumDo, 0, false, 0, 1, idAplikacije);
        }

        public List<_Statistika> UpozorenjaObavijesti(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.UpozorenjaObavijesti(grad, datumOd, datumDo, 1, idAplikacije);
        }

        public List<_Statistika> UpozorenjaObavijestiMjesecno(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.UpozorenjaObavijestiMjesecno(grad, datumOd, datumDo, 1, idAplikacije);
        }

        public List<_Statistika> UpozorenjaObavijestiPoPrekrsajima(string grad, DateTime datumOd, DateTime datumDo, bool nalozi)
        {
            return Statistika.UpozorenjaObavijestiPoPrekrsajima(grad, datumOd, datumDo, nalozi, 1, idAplikacije);
        }

        public List<_Statistika> NalogaPoPrekrsajima(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.NalogaPoPrekrsajima(grad, datumOd, datumDo, 1, idAplikacije);
        }

        public List<_Statistika> NaglaseneUlice(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.NaglaseneUlice(grad, datumOd, datumDo, 1, idAplikacije);
        }

        public List<_Statistika> ProsjekPoDanu(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.ProsjekPoDanu(grad, datumOd, datumDo, 1, idAplikacije);
        }

        public List<_Statistika> TrajanjePostupka(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.TrajanjePostupka(grad, datumOd, datumDo, 1, idAplikacije);
        }

        public List<_Statistika> RedariPrekrsaji(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.RedariPrekrsaji(grad, datumOd, datumDo, 1, idAplikacije);
        }

        public List<_Statistika> PrekrsajaPoDrzavi(string grad, DateTime datumOd, DateTime datumDo, bool izuzmihr)
        {
            return Statistika.PrekrsajaPoDrzavi(grad, datumOd, datumDo, izuzmihr, 1, idAplikacije);
        }

        public List<_Statistika> ObavijestiNaloziPauku(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.ObavijestiNaloziPauku(grad, datumOd, datumDo, 1, idAplikacije);
        }

        public List<_Statistika> StatusVPP(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.StatusVPP(grad, datumOd, datumDo, idAplikacije);
        }

        public List<_Statistika> IntenzitetRada(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.IntenzitetRada(grad, datumOd, datumDo, 1, idAplikacije);
        }

        public List<_Statistika> AktivnostTerminal(string grad, DateTime datumOd, DateTime datumDo, int idDjelatnika)
        {
            return Statistika.AktivnostTerminal(grad, datumOd, datumDo, idDjelatnika, idAplikacije);
        }

        public List<_Statistika> TrajanjeBaterije(string grad, DateTime datum, int idTerminala)
        {
            return Statistika.TrajanjeBaterije(grad, datum, idTerminala, idAplikacije);
        }

        public List<_Statistika> GPS(string grad, DateTime datum, int idTerminala)
        {
            return Statistika.GPS(grad, datum, idTerminala, idAplikacije);
        }

        public List<_Statistika> VPStatusi(string grad)
        {
            return Statistika.VPStatusi(grad, idAplikacije);
        }

        public List<_Statistika> VPNaplaceni(string grad)
        {
            return Statistika.VPNaplaceni(grad, idAplikacije);
        }

        public List<_Stranci> VPRazradaDrzave(string grad)
        {
            return Statistika.VPRazradaDrzave(grad, idAplikacije);
        }

        public List<_CentralnaLokacija> IntenzitetPostupanja(string grad, DateTime datumOd, DateTime datumDo, bool nalog, bool zahtjevi)
        {
            return Statistika.IntenzitetPostupanja(grad, datumOd, datumDo, 1, nalog, zahtjevi, idAplikacije);
        }

        #endregion

        #region POSTAVKE

        public List<_2DLista> PopisNaglasenihUlica(string grad)
        {
            return Statistika.PopisNaglasenihUlica(grad, idAplikacije);
        }

        public bool ObrisiNaglasenuUlicu(string grad, int idUlice)
        {
            return Statistika.ObrisiNaglasenuUlicu(grad, idUlice, idAplikacije);
        }

        public int DodajNaglasenuUlicu(string grad, string ulica)
        {
            return Statistika.DodajNaglasenuUlicu(grad, ulica, idAplikacije);

        }

        #endregion

        /*:: TERMINALI ::*/

        #region TERMINALI - TRENUTNO VADI IZ STARE BAZE PREBACITI NA NOVO

        public List<_Terminal> PopisTerminala(string grad, bool neaktivni)
        {
            return Postavke.PopisTerminalaS(grad, neaktivni, idAplikacije);
        }

        public List<_2DLista> DohvatiTerminale(string grad)
        {
            return Postavke.DohvatiTerminale(grad, idAplikacije);
        }

        public bool IzmjeniTerminal(string grad, _Terminal terminal)
        {
            return Postavke.IzmjeniTerminalS(grad, terminal, idAplikacije);
        }

        public bool AkcijeNaTerminalima(string grad, _Terminal terminal)
        {
            return Postavke.AkcijeNaTerminalima(grad, terminal, idAplikacije);
        }

        public List<_StatusTerminala> StatusTerminala(string grad)
        {
            return Postavke.StatusTerminala(grad, idAplikacije);
        }

        #endregion

        /*:: EMAIL LISTE ::*/

        #region EMAIL LISTE

        public List<_MailLista> DohvatiMailListu(string grad)
        {
            return MailLista.DohvatiMailListu(grad, idAplikacije);
        }

        public bool ObrisiMailListu(string grad, int idListe)
        {
            return MailLista.ObrisiMailListu(grad, idListe, idAplikacije);
        }

        public int DodajMailListu(string grad, _MailLista lista)
        {
            return MailLista.DodajMailListu(grad, lista, idAplikacije);
        }

        public bool SaljiMailListu(string grad, int idListe, bool salji)
        {
            return MailLista.SaljiMailListu(grad, idListe, salji, idAplikacije);
        }

        public bool PrilogMailListi(string grad, int idListe, bool hub)
        {
            return MailLista.PrilogMailListi(grad, idListe, hub, idAplikacije);
        }

        #endregion

        /*:: ZAHTJEVI ::*/

        #region ZAHTJEVI

        public int Neobradjeni(string grad, int idDjelatnika, out _PrijavaPauk prijava)
        {
            return Zahtjev.Neobradjeni(grad, idDjelatnika, 0, out prijava, idAplikacije);
        }

        public bool Preuzmi(string grad, int idZahtjeva, int idDjelatnika)
        {
            return Zahtjev.Preuzmi(grad, idZahtjeva, idDjelatnika, idAplikacije);
        }

        public void Odustani(string grad, int idZahtjeva, int idDjelatnika)
        {
            Zahtjev.Odustani(grad, idZahtjeva, idDjelatnika, idAplikacije);
        }

        public bool StatusZahtjeva(string grad, int idZahtjeva)
        {
            return Zahtjev.StatusZahtjeva(grad, idZahtjeva, idAplikacije);
        }

        public void SpremiAkcijuZahtjeva(string grad, int idZahtjeva, int idDjelatnika, int idAkcije)
        {
            Zahtjev.SpremiAkcijuZahtjeva(grad, idZahtjeva, idDjelatnika, idAkcije, idAplikacije);
        }

        public List<_Kaznjavan> Kaznjavan(string grad, string registracija, string drzava, int dana)
        {
            return Mobile.Kaznjavan(grad, registracija, drzava, dana, false, idAplikacije);
        }

        public int DodajPrekrsaj(string grad, _PrijavaPauk zahtjev, int idOpisa, decimal kazna, string registracija, string adresa, string drzava, bool obavijest, bool nalogPauku, bool lisice)
        {
            return Zahtjev.DodajPrekrsaj(grad, zahtjev, idOpisa, kazna, registracija, adresa, drzava, obavijest, nalogPauku, lisice, 1, idAplikacije);
        }

        public bool StatusZahtjevaPaukOdustao(string grad, int idZahtjeva)
        {
            return Zahtjev.StatusZahtjevaPaukOdustao(grad, idZahtjeva, idAplikacije);
        }

        public bool Zatvori(string grad, int idZahtjeva, int idStatusa, int idDjelatnika, string razlog)
        {
            return Zahtjev.Zatvori(grad, idZahtjeva, idStatusa, idDjelatnika, null, null, razlog, 1, idAplikacije);
        }

        public bool DodijeliPozivNaBroj(string grad, _Prekrsaj prekrsaj)
        {
            return Prekrsaj.DodijeliPozivNaBroj(grad, prekrsaj, idAplikacije);
        }

        #endregion

        /*:: RENTACAR ::*/

        #region RENT-A-CAR

        public List<_RentaCar> DohvatiRentaCar(string grad)
        {
            return RentaCar.DohvatiRentaCar(grad, idAplikacije);
        }

        public _RentaCar DodajRentaCar(string grad, _RentaCar renta)
        {
            return RentaCar.DodajRentaCar(grad, renta, idAplikacije);
        }

        public bool IzmjeniRentaCar(string grad, _RentaCar renta)
        {
            return RentaCar.IzmjeniRentaCar(grad, renta, idAplikacije);
        }

        public bool ObrisiRentaCar(string grad, int idRente)
        {
            return RentaCar.ObrisiRentaCar(grad, idRente, idAplikacije);
        }

        public bool PoveziRentaCar(string grad, int idRente, string naziv, string email, string mobitel, out int idKorisnikaGO,
            out int idKlasifikacije)
        {
            return RentaCar.PoveziRentaCar(grad, idRente, naziv, email, mobitel, out idKorisnikaGO,
                out idKlasifikacije, idAplikacije);
        }

        /*:: VOZILA ::*/

        public int DodajRCVozilo(string grad, int idRentaCar, string registracija)
        {
            return RentaCar.DodajRCVozilo(grad, idRentaCar, registracija, idAplikacije);
        }

        public bool DodajRCVozila(string grad, int idRentaCar, string[] registracije)
        {
            return RentaCar.DodajRCVozila(grad, idRentaCar, registracije, idAplikacije);
        }


        public bool IzmjeniRCVozilo(string grad, int idVozila, string registracija)
        {
            return RentaCar.IzmjeniRCVozilo(grad, idVozila, registracija, idAplikacije);
        }

        public bool ObrisiRCVozilo(string grad, int idVozila)
        {
            return RentaCar.ObrisiRCVozilo(grad, idVozila, idAplikacije);
        }

        #endregion
    }
}
