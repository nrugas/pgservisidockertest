using System;
using System.Collections.Generic;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi
{
    public class PGPauk : IPGPauk
    {
        private const int idAplikacije = 4;

        public bool Dostupan()
        {
            return true;
        }

        public List<_2DLista> Aplikacije()
        {
            return Sustav.Aplikacije(idAplikacije);
        }

        public List<_2DLista> Redarstva()
        {
            return Postavke.Redarstva(idAplikacije);
        }

        public List<_Drzava> Drzave()
        {
            return Sustav.Drzave(idAplikacije);
        }

        public List<_3DLista> DohvatiOpise(string grad)
        {
            return Zakoni.DohvatiOpise(grad, idAplikacije);
        }

        public string f_kontrolni(string broj)
        {
            return KontrolniBroj.f_kontrolni("HUBM11", broj, "", broj.Length);
        }

        public _Grad AktivacijaAplikacije(string aktivacijskiKod)
        {
            return Sustav.AktivacijaAplikacije(aktivacijskiKod, idAplikacije);
        }

        public _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS)
        {
            return Korisnici.Prijava(grad, korisnickoIme, zaporka, 2, out blokiranaJLS, idAplikacije);
        }

        public void SpremiError(string grad, Exception greska, string napomena, string korisnik)
        {
            Sustav.SpremiGresku(grad, greska, idAplikacije, napomena, korisnik);
        }

        public bool Aktivan(string grad, string korisnik, string racunalo, string verzija, string os, int idKorisnika)
        {
            bool reset = Sustav.Aktivnost(grad, idKorisnika, racunalo, verzija, korisnik, os, false, idAplikacije);

            if (reset)
            {
                Sustav.AktivnostReset(grad, racunalo, idAplikacije);
            }

            return reset;
        }

        public void Reset(string grad, int idAktivnosti)
        {
            Sustav.Reset(grad, idAktivnosti, idAplikacije);
        }

        public List<_2DLista> PopisGradova()
        {
            return Gradovi.PopisGradova(2, idAplikacije);
        }

        public _Predlozak DohvatiPredlozakIspisa(string grad, string koji)
        {
            return Predlosci.DohvatiPredlozakIspisa(grad, koji, idAplikacije);
        }

        public _Predlozak DohvatiPredlozak(string grad, int idPredloska)
        {
            return Predlosci.DohvatiPredlozakIspisa(grad, idPredloska, idAplikacije);
        }

        public bool IspisPredloska1(string baza, string detaljiKazne, int idPredloska, out string ispis)
        {
            return Ispis.IspisPredloska(baza, detaljiKazne, 1, idPredloska, 0, out ispis, false, idAplikacije);
        }

        public string IspisObavijestiPauk(string grad, int idLokacije, string broj)
        {
            return Ispis.IspisObavijestiPauk(grad, idLokacije, broj, idAplikacije);
        }

        public _Grad Grad(string grad)
        {
            return Gradovi.Grad(grad, idAplikacije);
        }

        public bool NaruciMaterijal(string grad, _Narudzba narudzba)
        {
            return Sustav.NaruciMaterijal(grad, narudzba, idAplikacije);
        }

        public List<_Predlozak> PredlosciIspisa(string grad)
        {
            return Predlosci.PredlosciIspisa(grad, 0, idAplikacije);
        }

        /*: GO ::*/

        public List<_3DLista> GradoviGO()
        {
            return GO.GradoviGO(idAplikacije);
        }

        #region KONTAKTIRANJE KORISNIKA

        public bool DodajPredlozak(string grad, string naziv, string predlozak)
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

        public bool IzmjeniMapu(string grad, string mapa)
        {
            return Gradovi.IzmjeniMapu(grad, mapa, idAplikacije);
        }

        #endregion

        /*:: GLAVNI MENU ::*/

        public List<_Pozicija> PozicijeRedara(string grad, int minuta)
        {
            return Mapa.PozicijeRedara(grad, minuta, 1, idAplikacije);
        }

        public List<_Pozicija> TrenutnePozicijeVozila(string grad, int minuta, int? idVozila)
        {
            return Mapa.TrenutnePozicijeVozila(grad, minuta, idVozila, idAplikacije);
        }

        public List<_Prekrsaj> PozicijeNaloga(string grad, int idVozila, DateTime datum, bool sviNalozi, string vrijeme)
        {
            return Mapa.PozicijeNaloga(grad, idVozila, datum, sviNalozi, vrijeme, idAplikacije);
        }

        public List<_Tocka> PutanjaPauka(string grad, DateTime datum, string vrijeme, int idVozila)
        {
            return Mapa.StvaranjePutanjePauka(grad, datum, vrijeme, idVozila, idAplikacije);
        }

        public List<_PutanjaVozila> PutanjaObradeNaloga(string grad, _Nalog nalog, out List<_PutanjaVozila> putanjaDoPrekrsaja, out List<_DogadjajiNaloga> dogadaji)
        {
            return Mapa.PutanjaObradeNaloga(grad, nalog, out putanjaDoPrekrsaja, out dogadaji, idAplikacije);
        }

        /*:: PREKRSAJ ::*/

        public List<_Slika> SlikePrekrsaja(string grad, int idLokacije)
        {
            return Prekrsaj.SlikePrekrsaja(grad, idLokacije, idAplikacije);
        }

        public int RotirajSliku(string grad, int idSlike, byte[] slika)
        {
            return Prekrsaj.RotirajSliku(grad, idSlike, slika, idAplikacije);
        }

        public List<byte[]> Slike(string grad, int idLokacije)
        {
            return Prekrsaj.Slike(grad, idLokacije, idAplikacije);
        }

        public List<_Prekrsaj> PretragaNaloga(string grad, int idStatusa, int idVozila, DateTime? datumOd, DateTime? datumDo, bool registracija, bool dokument, bool ulica, string pojam)
        {
            return Pretraga.PretragaNaloga(grad, idStatusa, idVozila, datumOd, datumDo, registracija, dokument, ulica, pojam, idAplikacije);
        }

        public List<_Prekrsaj> PretragaNalogaNaDeponiju(string grad)
        {
            return Pretraga.PretragaNalogaZaNaplatu(grad, 4, true, idAplikacije);
        }

        public List<_Prekrsaj> PretragaBlokiranih(string grad)
        {
            return Pretraga.PretragaNalogaZaNaplatu(grad, 22, true, idAplikacije);
        }

        public List<_2DLista> Statusi(string grad, bool zatvara)
        {
            return Spider.Statusi(grad, zatvara, idAplikacije);
        }

        public int DodajRucniPrekrsaj(string grad, _Prekrsaj prekrsaj, bool obavijest, List<byte[]> slike, int idRedarstva, bool lisice)
        {
            return Prekrsaj.DodajRucniPrekrsaj(grad, prekrsaj, obavijest, slike, idRedarstva, lisice, idAplikacije);
        }

        public List<_Zakon> DohvatiZakone(string grad, int idRedarstva, bool neaktivni)
        {
            return Zakoni.DohvatiZakoneS(grad, neaktivni, idRedarstva, true, idAplikacije);
        }

        public List<int> DodajSliku(string grad, int idLokacije, List<byte[]> slike)
        {
            return Prekrsaj.DodajSliku(grad, idLokacije, slike, 1, idAplikacije);
        }

        public List<_Predlozak> PredlosciIspisaLight(string grad, int idRedarstva)
        {
            return Predlosci.PredlosciIspisaLight(grad, idRedarstva, idAplikacije);
        }

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
            return Chat.PosaljiPoruku(grad, poruka, 2, idAplikacije);
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

        /*:: VOZILA ::*/

        #region VOZILA

        public List<_2DLista> PopisVozila(string grad)
        {
            return Vozila.PopisVozila(grad, idAplikacije);
        }

        public List<_Vozilo> VozilaPauka(string grad, bool obrisana)
        {
            return Vozila.VozilaPauka(grad, obrisana, idAplikacije);
        }

        //public List<_2DLista> Terminali(string grad)
        //{
        //    return Vozila.Terminali(grad, idAplikacije);
        //}

        public bool ObrisiVozilo(string grad, int idVozila)
        {
            return Vozila.ObrisiVozilo(grad, idVozila, idAplikacije);
        }

        public bool IzmijeniVozilo(string grad, _Vozilo vozilo)
        {
            return Vozila.IzmijeniVozilo(grad, vozilo, idAplikacije);
        }

        public int DodajVozilo(string grad, _Vozilo vozilo)
        {
            return Vozila.DodajVozilo(grad, vozilo, idAplikacije);
        }

        public bool AktivirajVozilo(string grad, int idVozila, bool aktivno)
        {
            return Vozila.AktivirajVozilo(grad, idVozila, aktivno, idAplikacije);
        }

        public bool VoziloObradjujeNaloge(string grad, int idVozila, bool obradjuje)
        {
            return Vozila.VoziloObradjujeNaloge(grad, idVozila, obradjuje, idAplikacije);
        }

        public bool VoziloObradjujeLisice(string grad, int idVozila, bool obradjuje)
        {
            return Vozila.VoziloObradjujeLisice(grad, idVozila, obradjuje, idAplikacije);
        }

        public _Vozilo Vozilo(string grad, int idVozila)
        {
            return Vozila.Vozilo(grad, idVozila, idAplikacije);
        }

        #endregion

        #region PODRSKA

        public bool PrijaviProblem(string grad, _Problem problem, List<byte[]> slike)
        {
            return Sustav.PrijaviProblem(grad, problem, slike, idAplikacije);
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

        /*:: DJELATNICI ::*/

        #region DJELATNICI

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
            return Korisnici.DohvatiDjelatnike(grad, 2, idAplikacije);
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

        public List<_Kontakt> DohvatiKontakte(string grad)
        {
            return Korisnici.DohvatiKontakte(grad, idAplikacije);
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

        /*:: AKCIJE KORISNIKA ::*/

        #region AKCIJE KORISNIKA

        public List<_AktivneAplikacije> DohvatiAktivne(string grad, bool aktivni)
        {
            return Sustav.DohvatiAktivne(grad, aktivni, idAplikacije);
        }

        public bool ObrisiAktivnost(string grad, int idAktivnosti)
        {
            return Sustav.ObrisiAktivnost(grad, idAktivnosti, idAplikacije);
        }

        public void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena)
        {
            Sustav.SpremiAkciju(grad, idKorisnika, idAkcije, napomena, 2, idAplikacije);
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

        /*:: AKCIJE ::*/

        public bool Storniraj(string baza, int idNaloga, int idRazloga, string napomena)
        {
            return Nalog.Storniraj(baza, idNaloga, idRazloga, false, napomena, idAplikacije);
        }

        public bool IzmjeniVoziloNaloga(string grad, int idNaloga, int idVozila)
        {
            return Nalog.IzmjeniVoziloNaloga(grad, idNaloga, idVozila, idAplikacije);
        }

        //dodijeli
        public List<_Nalog> DodijeljeniNalozi(string grad, int idVozila)
        {
            return Nalog.DodijeljeniNalozi(grad, idVozila, idAplikacije);
        }

        public bool? Dodjeli(string grad, int idNaloga, int idVozila)
        {
            return Nalog.Dodjeli(grad, idNaloga, idVozila, idAplikacije);
        }

        public bool UpDown(string grad, int idNaloga, int idVozila, bool up)
        {
            return Nalog.UpDown(grad, idNaloga, idVozila, up, idAplikacije);
        }

        //otkazi
        public bool Otkazi(string grad, int idNaloga)
        {
            return Nalog.Otkazi(grad, idNaloga, idAplikacije);
        }

        //deponiraj
        public bool Deponiraj(string grad, int idNaloga, int idTerminala)
        {
            return Nalog.Deponiraj(grad, idNaloga, idTerminala, idAplikacije);
        }

        public int? IDTerminala(string grad, int idVozila)
        {
            return Nalog.IDTerminala(grad, idVozila, idAplikacije);
        }

        //pokusaj
        public bool PokusajPodizanja(string grad, int idNaloga, int idTerminala)
        {
            return Nalog.PokusajPodizanja(grad, idNaloga, idTerminala, idAplikacije);
        }


        //blokiraj
        public bool Blokiraj(string grad, int idNaloga, int idTerminala)
        {
            return Nalog.Blokiraj(grad, idNaloga, idTerminala, idAplikacije);
        }

        //todo obrisi naplati
        //public int NoviBrojRacuna(string grad, int idNaloga, out decimal? iznos)
        //{
        //    return Financije.NoviBrojRacuna(grad, idNaloga, out iznos, idAplikacije);
        //}
        //

        public int? Pokusaj(string grad, _Racun racun, int idTerminala, ref string brrac)
        {
            return Nalog.Pokusaj(grad, racun, idTerminala, ref brrac, idAplikacije);
        }

        //vrati na pocetak
        public bool VratiNaPocetak(string grad, int idNaloga)
        {
            return Nalog.VratiNaPocetak(grad, idNaloga, idAplikacije);
        }

        public bool NaplatioIzvanSustava(string grad, int idNaloga, string napomena)
        {
            return Nalog.NaplatioIzvanSustava(grad, idNaloga, napomena, idAplikacije);
        }

        //napomena
        public bool Napomena(string grad, int idNaloga, string napomena)
        {
            return Nalog.Napomena(grad, idNaloga, napomena, idAplikacije);
        }

        /*:: IZMJENE NALOGA ::*/

        public bool Registracija(string grad, int idPrekrsaja, string registracija, string kratica)
        {
            return Prekrsaj.Registracija(grad, idPrekrsaja, registracija, kratica, false, idAplikacije);
        }

        public bool Adresa(string grad, int idPrekrsaja, string adresa)
        {
            return Prekrsaj.Adresa(grad, idPrekrsaja, adresa, idAplikacije);
        }

        /*:: IZMJENE STATUS ::*/

        public List<_Event> IzmjeneStatusa(string grad, int idVozila, DateTime datum)
        {
            return Nalog.IzmjeneStatusa(grad, idVozila, datum, idAplikacije);
        }

        public bool ObrisiIzmjenuStatusa(string grad, int id)
        {
            return Nalog.ObrisiIzmjenuStatusa(grad, id, idAplikacije);
        }

        /**/

        public List<_2DLista> Razlozi(string grad)
        {
            return Nalog.Razlozi(grad, idAplikacije);
        }

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

        /*:: AUTOPILOT ::*/

        public bool UkljuciAutoPilot(string grad, bool ukljucen)
        {
            return AutopilotPauk.UkljuciAutoPilot(grad, ukljucen, idAplikacije);
        }

        public bool Autopilot(string grad)
        {
            return AutopilotPauk.Autopilot(grad, idAplikacije);
        }

        /*:: STATISTIKA ::*/

        #region STATISTIKA

        public List<_Statistika> NaloziPauku(string grad, DateTime datumOd, DateTime datumDo, int IDVozila, bool zahtjevi, int idOpisa)
        {
            return Statistika.NaloziPauku(grad, datumOd, datumDo, IDVozila, zahtjevi, idOpisa, -1, idAplikacije);
        }

        public List<_Statistika> RazloziOstalog(string grad, DateTime datumOd, DateTime datumDo, int IDVozila)
        {
            return Statistika.RazloziOstalog(grad, datumOd, datumDo, IDVozila, idAplikacije);
        }

        public List<_Statistika> IzdanihRacuna(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.IzdanihRacuna(grad, datumOd, datumDo, 2, idAplikacije);
        }

        public List<_Statistika> NalogaPoPrekrsajima(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.NalogaPoPrekrsajima(grad, datumOd, datumDo, -1, idAplikacije);
        }

        public List<_Statistika> NalogaPoPrekrsajimaSatus(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.NalogaPoPrekrsajimaSatus(grad, datumOd, datumDo, idAplikacije);
        }

        public List<_Statistika> Kilometri(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.Kilometri(grad, datumOd, datumDo, idAplikacije);
        }

        public List<_Statistika> PoslanihZahtjeva(string grad, DateTime datumOd, DateTime datumDo, int IDVozila)
        {
            return Statistika.PoslanihZahtjeva(grad, datumOd, datumDo, IDVozila, -1, idAplikacije);
        }

        public List<_CentralnaLokacija> IntenzitetPostupanja(string grad, DateTime datumOd, DateTime datumDo, bool nalog, bool zahtjevi)
        {
            return Statistika.IntenzitetPostupanja(grad, datumOd, datumDo, -1, nalog, zahtjevi, idAplikacije);
        }

        public List<_Statistika> TrajanjeBaterije(string grad, DateTime datum, int idTerminala)
        {
            return Statistika.TrajanjeBaterije(grad, datum, idTerminala, idAplikacije);
        }

        public List<_Statistika> GPS(string grad, DateTime datum, int idTerminala)
        {
            return Statistika.GPS(grad, datum, idTerminala, idAplikacije);
        }

        #endregion

        /*:: IZVJEŠTAJI ::*/

        public List<_EvidencijaPlacanja> EvidencijaPlacanja(string grad, DateTime datum)
        {
            return Statistika.EvidencijaPlacanja(grad, datum, idAplikacije);
        }

        public List<_EvidencijaPlacanja> EvidencijaPlacanjaUkupno(string grad, DateTime datum)
        {
            return Statistika.EvidencijaPlacanjaUkupno(grad, datum, idAplikacije);
        }

        public List<_DnevniUtrzak> BlagajnickiIzvjestaj(string grad, DateTime datum, int idDjelatnika)
        {
            return Statistika.BlagajnickiIzvjestaj(grad, datum, idDjelatnika, 2, idAplikacije);
        }

        public DateTime? ZadnjiBlagajnickiIzvjestaj(string grad)
        {
            return Statistika.ZadnjiBlagajnickiIzvjestaj(grad, 2, idAplikacije);
        }

        public List<_DnevniUtrzak> BlagajnickiIzvjestajSintetika(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.BlagajnickiIzvjestajSintetika(grad, datumOd, datumDo, 2, idAplikacije);
        }

        /*:: PONAVLJACI ::*/

        public List<_Prekrsaj> DetaljiPonavljaca(string grad, int idPredloska, DateTime? datumOd, DateTime? datumDo, string registracija)
        {
            return Pretraga.DetaljiPonavljaca(grad, idPredloska, datumOd, datumDo, registracija, true, 1, idAplikacije);
        }

        public List<_2DLista> Ponavljaci(string grad, int idPredloska, DateTime? datumOd, DateTime? datumDo, int broj)
        {
            return Pretraga.Ponavljaci(grad, idPredloska, datumOd, datumDo, broj, true, 1, idAplikacije);
        }

        /*:: VREMENA PAUKA ::*/

        public List<_VremenaPauka> VremenaPauka(string grad, DateTime DatumOd, DateTime DatumDo)
        {
            return Pretraga.VremenaPauka(grad, DatumOd, DatumDo, idAplikacije);
        }

        /*:: ZAHTJEVI ::*/

        public List<_ZahtjevPauka> Zahtjevi(string grad, int idVozila, int idStatusa, DateTime? datumOd, DateTime? datumDo)
        {
            return Zahtjev.Zahtjevi(grad, idVozila, idStatusa, datumOd, datumDo, -1, idAplikacije);
        }

        public _ZahtjevPauka DohvatiZahtjev(string grad, int idZahtjeva)
        {
            return Zahtjev.DohvatiZahtjev(grad, idZahtjeva, idAplikacije);
        }

        public bool PonoviZahtjev(string grad, int idZahtjeva)
        {
            return Zahtjev.PonoviZahtjev(grad, idZahtjeva, idAplikacije);
        }

        /*:: POSTAVKE JLS ::*/

        public _CentralnaLokacija Deponij(string grad)
        {
            return Spider.Deponij(grad, idAplikacije);
        }

        public bool LokacijaDeponija(string grad, _CentralnaLokacija deponij)
        {
            return Spider.LokacijaDeponija(grad, deponij, idAplikacije);
        }

        public bool StatusLokacije(string grad)
        {
            return Spider.StatusLokacije(grad, idAplikacije);
        }

        public bool DodajStatusLokacije(string grad)
        {
            return Spider.DodajStatusLokacije(grad, idAplikacije);
        }

        public bool SpremiPostavke(string grad, bool dof, bool naplata, bool lisice, bool prijave, bool prilog, bool mup, string odlukaLisice, bool hub, string zalbaPrometnog, int danaLezarina, DateTime? datum)
        {
            return Sustav.PostavkePauka(grad, dof, naplata, lisice, prijave, prilog, mup, odlukaLisice, hub, zalbaPrometnog, danaLezarina, datum, idAplikacije);
        }

        /*:: REDARSTVO PAUK ::*/

        public List<Tuple<int, int, int>> RedarstvaPauka(string grad)
        {
            return Spider.RedarstvaPauka(grad, idAplikacije);
        }

        public List<Tuple<string, int, int>> DohvatiRedarstvaPauka(string grad)
        {
            return Spider.DohvatiRedarstvaPauka(grad, idAplikacije);
        }

        public bool ObrisiRedarstvoPauk(string grad, Tuple<int, int, int> podaci)
        {
            return Spider.ObrisiRedarstvoPauk(grad, podaci, idAplikacije);
        }

        public bool DodajRedarstvoPauk(string grad, Tuple<int, int, int> podaci)
        {
            return Spider.DodajRedarstvoPauk(grad, podaci, idAplikacije);
        }

        /*:: POSADA ::*/

        public bool Ukloni(string grad, _Posada posada)
        {
            return Posade.Ukloni(grad, posada, idAplikacije);
        }

        public bool Dodaj(string grad, _Posada posada)
        {
            return Posade.Dodaj(grad, posada, idAplikacije);
        }

        public List<_Posada> DohvatiPosadu(string grad, DateTime datum)
        {
            return Posade.DohvatiPosadu(grad, datum, idAplikacije);
        }

        public List<_2DLista> Pauk(string grad)
        {
            return Korisnici.Pauk(grad, idAplikacije);
        }

        /*:: NAPLATA ::*/

        #region NAPLATA

        public int NoviRacun(string grad, _Racun racun, out string brrac, out string poziv)
        {
            return Naplata.NoviRacun(grad, racun, out brrac, out poziv, idAplikacije);
        }

        public int StornirajRacun(string grad, _Racun racun, int idStatusa, byte[] prilog, string filename, out string brrac)
        {
            return Naplata.StornirajRacun(grad, racun, idStatusa, prilog, filename, out brrac, idAplikacije);
        }

        public byte[] PregledajPrilogStornu(string grad, int idRacuna, out string filename)
        {
            return Naplata.PregledajPrilogStornu(grad, idRacuna, out filename, idAplikacije);
        }

        public List<_2DLista> StatusiStorna()
        {
            return Naplata.StatusiStorna(2);
        }

        public string Naplati(string grad, _Racun racun, out int idRacuna, out string poziv)
        {
            string brrac;
            return Naplata.NaplatiPauk(grad, racun, -10, out idRacuna, out brrac, out poziv, idAplikacije);
        }

        public List<_2DLista> Stavka(string grad, int idStatusa)
        {
            return Naplata.Stavka(grad, idStatusa, idAplikacije);
        }

        public string IspisKopijeRacuna(string grad, int idRacuna)
        {
            return Naplata.IspisKopijeRacuna(grad, idRacuna, idAplikacije);
        }

        /*:: VRSTE PLACANJA ::*/

        public List<_VrstaPlacanja> VrstePlacanja(string grad)
        {
            return Naplata.VrstePlacanja(grad, null, 2, idAplikacije);
        }

        public List<_VrstaPlacanjaStatus> VrstePlacanjaStatusi(string grad, out bool definiranIznos)
        {
            return Naplata.VrstePlacanjaStatusi(grad, 2, out definiranIznos, idAplikacije);
        }

        public bool IzmjeniStstusVrstePlacanja(string grad, int idVrste, bool ukljuci)
        {
            return Naplata.IzmjeniStstusVrstePlacanja(grad, idVrste, ukljuci, 2, idAplikacije);
        }

        /*:: POSLOVNI PROSTOR ::*/

        public _PoslovniProstor DohvatiPoslovniProstor(string grad, int idRedarstva)
        {
            return PoslovniProstor.DohvatiPoslovniProstor(grad, idRedarstva, idAplikacije);
        }

        public bool? IzmjeniPoslovniProstor(string grad, _PoslovniProstor prostor)
        {
            return PoslovniProstor.IzmjeniPoslovniProstor(grad, prostor, idAplikacije);
        }

        public bool SpremiLogo(string grad, byte[] logo, int idRedarstva)
        {
            return PoslovniProstor.SpremiLogo(grad, logo, idRedarstva, idAplikacije);
        }

        public bool RegistrirajPoslovniProstor(string grad, _PoslovniProstor prostor)
        {
            return Fiskalizacija.RegistrirajPP(grad, prostor, idAplikacije);
        }

        public bool DodajCertifikat(string grad, string sifra, byte[] certifikat, int idVlasnika)
        {
            return Fiskalizacija.DodajCertifikat(grad, sifra, certifikat, idVlasnika, idAplikacije);
        }

        public List<_Posta> Poste()
        {
            return Sustav.Poste(idAplikacije);
        }

        public _Uplatnica Uplatnica(string grad, int idRedarstva)
        {
            return Gradovi.Uplatnica(grad, idRedarstva, idAplikacije);
        }

        /*:: NAPLATNA MJESTA ::*/

        public _NaplatnoMjesto NaplatnoMjesto(string grad, string oznaka, int idProstora)
        {
            return PoslovniProstor.NaplatnoMjesto(grad, oznaka, idProstora, idAplikacije);
        }

        public List<_NaplatnoMjesto> NaplatnaMjesta(string grad, int idProstora)
        {
            return PoslovniProstor.NaplatnaMjesta(grad, idProstora, idAplikacije);
        }

        /*:: POSTAVKE ISPISA ::*/

        public List<_PostavkeIspisa> DohvatiPostavkeIspisa(string grad, int idRedarstva)
        {
            return PoslovniProstor.DohvatiPostavkeIspisa(grad, idRedarstva, idAplikacije);
        }

        public bool IzmjeniPostavkeIspisa(string grad, _PostavkeIspisa postavke)
        {
            return PoslovniProstor.IzmjeniPostavkeIspisa(grad, postavke, idAplikacije);
        }

        public int DodajPostavkuIspisa(string grad, _PostavkeIspisa postavke)
        {
            return PoslovniProstor.DodajPostavkuIspisa(grad, postavke, idAplikacije);
        }

        public bool KopirajPostavkeIspisa(string grad, int idRedarstva)
        {
            return PoslovniProstor.KopirajPostavkeIspisa(grad, idRedarstva, idAplikacije);
        }

        /*:: OPISI STAVKI ::*/

        public List<_2DLista> StatusiKojiNaplacuju(string grad)
        {
            return Naplata.StatusiKojiNaplacuju(grad, idAplikacije);
        }

        public bool? ObrisiOpisStavke(string grad, int idOpisa)
        {
            return Naplata.ObrisiOpisStavke(grad, idOpisa, idAplikacije);
        }

        public bool IzmjeniOpisStavke(string grad, _OpisiStavki opis)
        {
            return Naplata.IzmjeniOpisStavke(grad, opis, idAplikacije);
        }

        public int DodajOpisStavke(string grad, _OpisiStavki opis)
        {
            return Naplata.DodajOpisStavke(grad, opis, idAplikacije);
        }

        public List<_OpisiStavki> DohvatiOpiseStavki(string grad, int idRedarstva)
        {
            return Naplata.DohvatiOpiseStavki(grad, idRedarstva, idAplikacije);
        }

        /*:: RACUN ::*/

        public _Prekrsaj DetaljiPrekrsaja(string grad, int idNaloga)
        {
            return Prekrsaj.DetaljiPrekrsajaNalog(grad, idNaloga, idAplikacije);
        }

        public _Prekrsaj DetaljiPrekrsajaNalog(string grad, int idNaloga)
        {
            return Prekrsaj.DetaljiPrekrsajaNalog(grad, idNaloga, idAplikacije);
        }

        public _Racun DohvatiRacun(string grad, int idRacuna)
        {
            return Naplata.DohvatiRacun(grad, idRacuna, true, idAplikacije);
        }

        public _Racun DohvatiRacunPoziv(string grad, string poziv, out int idStatusa)
        {
            return Naplata.DohvatiRacunPoziv(grad, poziv, out idStatusa, idAplikacije);
        }

        public List<_Racun> DohvatiPopisRacuna(string grad, DateTime? datumOd, DateTime? datumDo, int idDjelatnika, bool fisk, string brrac, string poziv, int idRedarstva)
        {
            return Naplata.DohvatiPopisRacuna(grad, datumOd, datumDo, idDjelatnika, fisk, brrac, poziv, idRedarstva, idAplikacije);
        }

        public List<_Racun> DohvatiPopisRacunaOsoba(string grad, string ime, string prezime, string oib, int idRedarstva)
        {
            return Naplata.DohvatiPopisRacunaOsoba(grad, ime, prezime, oib, idRedarstva, idAplikacije);
        }

        public List<_Racun> DohvatiPopisRacunaStorno(string grad, DateTime? datumOd, DateTime? datumDo, int idStatusa, int idRedarstva)
        {
            return Naplata.DohvatiPopisRacunaStorno(grad, datumOd, datumDo, idStatusa, idRedarstva, idAplikacije);
        }

        public List<_Racun> DohvatiPopisRacunaMUP(string grad, DateTime? datumOd, DateTime? datumDo, string brrac, int idRedarstva)
        {
            return Naplata.DohvatiPopisRacunaMUP(grad, datumOd, datumDo, brrac, idRedarstva, idAplikacije);
        }

        public List<_2DLista> VrstaKartica(string grad)
        {
            return Naplata.VrsteKartica(grad, idAplikacije);
        }

        public List<_2DLista> VrsteBanaka(string grad)
        {
            return Naplata.VrsteBanaka(grad, idAplikacije);
        }

        /*:: ZAKLJUCENJE ::*/

        public int ZakljuciBlagajnu(string grad, _Zakljucenje zakljucenje)
        {
            return Naplata.ZakljuciBlagajnu(grad, zakljucenje, idAplikacije);
        }

        public bool ProgramskoZakljucivanjeZaostalih(string grad, DateTime datumDo, int idRedarstva)
        {
            return Naplata.ProgramskoZakljucivanjeZaostalih(grad, datumDo, idRedarstva, idAplikacije);
        }

        public List<_Zakljucenje> DohvatiZakljucenja(string grad, DateTime? datumOd, DateTime? datumDo, int idRedarstva)
        {
            return Naplata.DohvatiZakljucenja(grad, datumOd, datumDo, idRedarstva, idAplikacije);
        }

        public List<_Racun> DohvatiPopisRacunaZakljucenja(string grad, int idZakljucenja)
        {
            return Naplata.DohvatiPopisRacunaZakljucenja(grad, idZakljucenja, idAplikacije);
        }

        public int Nezakljuceni(string grad, int idRedarstva)
        {
            return Naplata.Nezakljuceni(grad, idRedarstva, idAplikacije);
        }

        public List<_Racun> DohvatiPopisRacunaZakljucenje(string grad, int? idDjelatnika, string oznaka, int idRedarstva)
        {
            return Naplata.DohvatiPopisRacunaZakljucenje(grad, idDjelatnika, oznaka, idRedarstva, idAplikacije);
        }

        public bool? Prenesi(string grad, DateTime datumOd, DateTime datumDo, out string poruka)
        {
            return Naplata.Prenesi(grad, datumOd, datumDo, out poruka, idAplikacije);
        }

        public _Prijenos Pripremi(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Prijenos.Pripremi(grad, datumOd, datumDo, idAplikacije);
        }

        public List<_PovijestPrijenosa> DohvatiPovijestPrijenosa(string grad, DateTime? datumOd, DateTime? datumDo)
        {
            return Prijenos.DohvatiPovijestPrijenosa(grad, datumOd, datumDo, idAplikacije);
        }

        /*:: OSOBE ::*/

        public bool SpremiOsobe(string grad, List<_Osoba> osobe, int idRacuna)
        {
            return Osobe.SpremiOsobe(grad, osobe, idRacuna, true, false, idAplikacije);
        }

        public List<_Osoba> DohvatiOsobe(string grad, bool oib)
        {
            return Osobe.DohvatiOsobe(grad, oib, idAplikacije);
        }

        public List<_Dokument> Dokumenti(string grad, int idOsobe)
        {
            return Osobe.Dokumenti(grad, idOsobe, idAplikacije);
        }

        public bool IzmjeniOsobu(string grad, _Osoba osoba, int idRacuna)
        {
            return Osobe.IzmjeniOsobu(grad, osoba, idRacuna, idAplikacije);
        }

        public bool ObrisiOsobu(string grad, int idOsobe)
        {
            return Osobe.ObrisiOsobu(grad, idOsobe, idAplikacije);
        }

        public _Osoba DohvatiOsobu(string grad, int idOsobe)
        {
            return Osobe.DohvatiOsobu(grad, idOsobe, idAplikacije);
        }

        /*:: KARTICE ::*/

        public bool IzmjeniKarticuPlacanja(string grad, int idRacuna, int? idBanke, int idKartice, string odobrenje, bool? rate)
        {
            return Naplata.IzmjeniKarticuPlacanja(grad, idRacuna, idBanke, idKartice, odobrenje, rate, idAplikacije);
        }

        /*:: FISKALIZACIJA ::*/

        public string PonovnaFiskalizacija(string grad, int idRedarstva)
        {
            return Fiskalizacija.PonovnaFiskalizacija(grad, idRedarstva, idAplikacije);
        }

        public string IzvjestajSmjene(string grad, int idDjelatnika)
        {
            return Ispis.IzvjestajSmjene(grad, idDjelatnika, idAplikacije);
        }

        /*:: MUP ::*/

        public List<_Osoba> DohvatMUPa(string grad, _Racun racun, int idKorisnika)
        {
            return Naplata.DohvatMUPa(grad, racun, idKorisnika, idAplikacije);
        }

        public _OdgovorMUPVozilo MUPParkingVozilo(string oibustanove, string registracija, DateTime datumpreksaja, string adresaprekrsaja)
        {
            return Naplata.MUPParkingVozilo(oibustanove, registracija, datumpreksaja, adresaprekrsaja, idAplikacije);
        }
        #endregion

        /*:: EMAIL LISTE ::*/

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

        /*:: VANJSKE PRIJAVE ::*/

        public List<_Prijava> Prijave(string grad, DateTime datumOd, DateTime datumDo, bool nepregledane)
        {
            return VanjskaPrijava.Prijave(grad, datumOd, datumDo, nepregledane, 2, idAplikacije);
        }

        public void PregledaoPrijavu(string grad, int idPrijave)
        {
            VanjskaPrijava.PregledaoPrijavu(grad, idPrijave, idAplikacije);
        }

        public bool NalogPrijave(string grad, int idPrijave, int? idNaloga, int idPrekrsaja)
        {
            return VanjskaPrijava.NalogPrijave(grad, idPrijave, idNaloga, idPrekrsaja, idAplikacije);
        }

        public int NepregledanePrijave(string grad)
        {
            return VanjskaPrijava.NepregledanePrijave(grad, idAplikacije);
        }

        public void OdbijPrijavu(string grad, int idPrijave)
        {
            VanjskaPrijava.OdbijPrijavu(grad, idPrijave, idAplikacije);
        }

        public bool ObradiPrijavu(string grad, int idPrijave, string napomena)
        {
            return VanjskaPrijava.ObradiPrijavu(grad, idPrijave, napomena, idAplikacije);
        }

        public List<_2DLista> StatusiVP(string grad)
        {
            return VanjskaPrijava.StatusiVP(grad, idAplikacije);
        }
    }
}
