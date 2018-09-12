using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi
{
    public class PGParking : IPGParking
    {
        private const int idAplikacije = 11;

        public bool Dostupan()
        {
            return true;
        }

        public _Grad Aktivacija(string aktivacijskiKod)
        {
            return Sustav.AktivacijaAplikacije(aktivacijskiKod, idAplikacije);
        }

        public List<_2DLista> Aplikacije()
        {
            return Sustav.Aplikacije(idAplikacije);
        }

        public List<_2DLista> PopisGradova()
        {
            return Gradovi.PopisGradova(4, idAplikacije);
        }

        public List<_3DLista> Redari(string grad)
        {
            return Korisnici.Redari(grad, 4, idAplikacije);
        }

        public List<_2DLista> Redarstva()
        {
            return Postavke.Redarstva(idAplikacije);
        }

        public List<_Drzava> Drzave()
        {
            return Sustav.Drzave(idAplikacije);
        }

        public List<_2DLista> PopisPredlozaka(string grad)
        {
            return Predlosci.PopisPredlozaka(grad, 1, idAplikacije);
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

        #endregion

        /*:: AKTIVNOST ::*/

        public bool Aktivan(string grad, string korisnik, string racunalo, string verzija, string os, int idKorisnika)
        {
            bool reset = Sustav.Aktivnost(grad, idKorisnika, racunalo, verzija, korisnik, os, false, idAplikacije);

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

        /*:: AKCIJE KORISNIKA ::*/

        #region AKCIJE KORISNIKA

        public void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena)
        {
            Sustav.SpremiAkciju(grad, idKorisnika, idAkcije, napomena, 4, idAplikacije);
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
            return Pretraga.PrekrsajiIzvoz(grad, idDjelatnika, idPredloska, datumOd, datumDo, storno, 4, idAplikacije);
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

        #endregion

        /*:: DJELATNICI ::*/

        #region DJELATNICI

        public bool PosaljiEmailDdobrodoslice(string grad, int idKorisnika)
        {
            return Korisnici.PosaljiEmailDdobrodoslice(grad, idKorisnika, idAplikacije);
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
            return Korisnici.DohvatiDjelatnike(grad, 4, idAplikacije);
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

        #endregion

        #region POSLOVNI SUBJEKTI

        public List<_2DLista> DohvatiPopisSubjekta(string grad)
        {
            return Korisnici.DohvatiPopisSubjekta(grad, idAplikacije);
        }

        public List<_PoslovniSubjekt> DohvatiPoslovneSubjekte(string grad)
        {
            return Korisnici.DohvatiPoslovneSubjekte(grad, idAplikacije);
        }

        public int DodajPoslovnogSubjekta(string grad, _PoslovniSubjekt subjekt)
        {
            return Korisnici.DodajPoslovnogSubjekta(grad, subjekt, idAplikacije);
        }

        public bool IzmjeniPoslovnogSubjekta(string grad, _PoslovniSubjekt subjekt)
        {
            return Korisnici.IzmjeniPoslovnogSubjekta(grad, subjekt, idAplikacije);
        }

        public bool ObrisPoslovnogSubjekta(string grad, int idSubjekta)
        {
            return Korisnici.ObrisPoslovnogSubjekta(grad, idSubjekta, idAplikacije);
        }

        #endregion

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
            return Naplata.StatusiStorna(4);
        }

        public string StornirajKaznu(string grad, int idPrekrsaja, int idDjelatnika, string napomena, int idStatusa)
        {
            return Naplata.StornirajRacun(grad, idPrekrsaja, idDjelatnika, napomena, idStatusa, idAplikacije);
        }

        //public string Naplati(string grad, _Racun racun, out int idRacuna, out string poziv)
        //{
        //    string brrac;
        //    return Naplata.NaplatiPauk(grad, racun, out idRacuna, out brrac, out poziv, idAplikacije);
        //}

        public List<_2DLista> Stavka(string grad, int idStatusa)
        {
            return Naplata.Stavka(grad, idStatusa, idAplikacije);
        }

        public string IspisKopijeRacuna(string grad, int idRacuna)
        {
            return Naplata.IspisKopijeRacuna(grad, idRacuna, idAplikacije);
        }

        public List<_2DLista> Statusi()
        {
            return Parking.Statusi(false, idAplikacije);
        }

        /*:: VRSTE PLACANJA ::*/

        public List<_VrstaPlacanja> VrstePlacanja(string grad)
        {
            return Naplata.VrstePlacanja(grad, 0, 4, idAplikacije);
        }

        public List<_VrstaPlacanjaStatus> VrstePlacanjaStatusi(string grad, out bool definiranIznos)
        {
            return Naplata.VrstePlacanjaStatusi(grad, 4, out definiranIznos, idAplikacije);
        }

        public bool IzmjeniStstusVrstePlacanja(string grad, int idVrste, bool ukljuci)
        {
            return Naplata.IzmjeniStstusVrstePlacanja(grad, idVrste, ukljuci, 4, idAplikacije);
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

        /*:: OPISI STAVKI ::*/

        public List<_2DLista> StatusiKojiNaplacuju()
        {
            return Parking.Statusi(true, idAplikacije);
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

        public _Racun DohvatiRacun(string grad, int idRacuna)
        {
            return Naplata.DohvatiRacun(grad, idRacuna, true, idAplikacije);
        }

        public List<_Racun> DohvatiPopisRacuna(string grad, DateTime? datumOd, DateTime? datumDo, int idDjelatnika, bool fisk, string brrac, string poziv, int idRedarstva)
        {
            return Naplata.DohvatiPopisRacuna(grad, datumOd, datumDo, idDjelatnika, fisk, brrac, poziv, idRedarstva, idAplikacije);
        }

        public List<_Racun> DohvatiPopisRacunaOsoba(string grad, string ime, string prezime, string oib, int idRedarstva)
        {
            return Naplata.DohvatiPopisRacunaOsoba(grad, ime, prezime, oib, idRedarstva, idAplikacije);
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

        public bool ZakljuciZaostatke(string grad, int idRedarstva)
        {
            return Naplata.ZakljuciZaostatke(grad, idRedarstva, idAplikacije);
        }

        public bool Prenesi(string grad, List<DateTime> datumi)
        {
            return true; //Naplata.Prenesi(grad, datumi, idAplikacije);
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

        public string IzvjestajSmjene(string grad, int idKorisnika)
        {
            return Ispis.IzvjestajSmjeneParking(grad, idKorisnika, 4, DateTime.Today, idAplikacije, 0);
        }

        public string IzvjestajZakljucenja(string grad, int idKorisnika, DateTime datum)
        {
            return Ispis.IzvjestajSmjeneParking(grad, idKorisnika, 4, datum, idAplikacije, 0);
        }

        /*:: MUP ::*/

        public List<_Osoba> DohvatMUPa(string grad, _Racun racun, int idKorisnika)
        {
            return Naplata.DohvatMUPa(grad, racun, idKorisnika, idAplikacije);
        }

        #endregion

        /*:: SUSTAV ::*/

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

        #region TERMINALI

        public List<_Terminal> PopisTerminala(string grad, bool neaktivni)
        {
            return Postavke.PopisTerminalaS(grad, neaktivni, idAplikacije);
        }

        public bool IzmjeniTerminal(string grad, _Terminal terminal)
        {
            return Postavke.IzmjeniTerminalS(grad, terminal, idAplikacije);
        }

        public bool AkcijeNaTerminalima(string grad, _Terminal terminal)
        {
            return Postavke.AkcijeNaTerminalima(grad, terminal, idAplikacije);
        }

        #endregion

        #region POSEBNA VOZILA

        public List<_Odobrenja> DohvatiOdobrenja(string grad)
        {
            return Postavke.DohvatiOdobrenja(grad, 4, idAplikacije);
        }

        public bool ObrisiOdobrenje(string grad, int idOdobrenja)
        {
            return Postavke.ObrisiOdobrenje(grad, idOdobrenja, idAplikacije);
        }

        public int DodajOdobrenje(string grad, _Odobrenja vozilo)
        {
            return Postavke.DodajOdobrenje(grad, vozilo, idAplikacije);
        }

        public bool PromijeniStatusOdobrenja(string grad, int idOdobrenja, bool suspendirano)
        {
            return Postavke.PromijeniStatusOdobrenja(grad, idOdobrenja, suspendirano, idAplikacije);
        }

        public bool IzmijeniOdobrenje(string grad, _Odobrenja odobrenje)
        {
            return Postavke.IzmijeniOdobrenje(grad, odobrenje, idAplikacije);
        }

        //public bool PokusajKaznjavanja(string grad, string registracija)
        //{
        //    return Postavke.PokusajKaznjavanja(grad, registracija, idAplikacije);
        //}

        #endregion

        /*:: PREDLOŠCI ::*/

        #region PREDLOŠCI

        public List<_2DLista> JeziciPredlozaka(string grad)
        {
            return Predlosci.JeziciPredlozaka(grad, idAplikacije);
        }

        public List<_Predlozak> PredlosciIspisa(string grad)
        {
            return Predlosci.PredlosciIspisa(grad, 4, idAplikacije);
        }

        public bool ObrisiPredlozak(string grad, int idPredloska, int idJezika)
        {
            return Predlosci.ObrisiPredlozak(grad, idPredloska, idJezika, idAplikacije);
        }

        public bool IzmijeniPredlozak(string grad, int idPredloska, int idJezika, string nazivPredloska, bool pauk, bool kaznjava, XElement tekstPredloska)
        {
            return Predlosci.IzmijeniPredlozak(grad, idPredloska, idJezika, nazivPredloska, pauk, kaznjava, tekstPredloska, idAplikacije);
        }

        public int DodajPredlozak(string grad, string nazivPredloska, bool pauk, bool kaznjava, XElement tekstPredloska)
        {
            return Predlosci.DodajPredlozak(grad, nazivPredloska, pauk, kaznjava, tekstPredloska, 4, idAplikacije);
        }

        public _Predlozak DohvatiPredlozakIspisa(string grad, string koji)
        {
            return Predlosci.DohvatiPredlozakIspisa(grad, koji, idAplikacije);
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

        /*:: PREKRSAJ ::*/

        #region PREKRSAJ

        public _Opazanje TraziOpazanje(string grad, int idOpazanja)
        {
            return Parking.TraziOpazanje(grad, idOpazanja, idAplikacije);
        }

        public List<_Opazanje> TraziOpazanja(string grad, DateTime datum, string vrijeme, int? idDjelatnika)
        {
            return Parking.TraziOpazanja(grad, datum, vrijeme, idDjelatnika, idAplikacije);
        }

        public List<_Opazanje> PretraziOpazanja(string grad, int idStatusa, int idDjelatnika, int idSektora,
            int idzone, DateTime? datumOd, DateTime? datumDo, string registracija)
        {
            return Parking.PretraziOpazanja(grad, idStatusa, idDjelatnika, idSektora, idzone, datumOd, datumDo, registracija,
                idAplikacije);
        }

        //public List<_Prekrsaj> PretraziPrekrsaje(string grad, int idDjelatnika, DateTime datum, bool obavijesti, bool upozorenja)
        //{
        //    return Prekrsaj.PretraziPrekrsaje(grad, idDjelatnika, datum, 4, idAplikacije);
        //}

        //public _Prekrsaj DetaljiPrekrsaja(string grad, int idLokacije)
        //{
        //    return Prekrsaj.DetaljiPrekrsaja(grad, idLokacije, idAplikacije);
        //}

        public _Aktivnost Aktivnost(string grad, DateTime datum)
        {
            return Statistika.Aktivnost(grad, datum, 4, idAplikacije);
        }

        /*:: SLIKE ::*/

        public List<byte[]> Slike(string grad, int idLokacije)
        {
            return Prekrsaj.Slike(grad, idLokacije, idAplikacije);
        }

        public List<int> DodajSliku(string grad, int idLokacije, List<byte[]> slike)
        {
            return Prekrsaj.DodajSliku(grad, idLokacije, slike, 4, idAplikacije);
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

        public List<_Tocka> StvaranjePutanjeRedara(string grad, DateTime datum, string vrijeme, int idRedara, int gpsAcc, int speed)
        {
            return Mapa.StvaranjePutanjeRedara(grad, datum, vrijeme, idRedara, gpsAcc, speed, idAplikacije);
        }

        public List<_Prekrsaj> PozicijePrekrsaja(string grad, int idZaposlenika, DateTime datum, string vrijeme)
        {
            return Mapa.PozicijePrekrsaja(grad, idZaposlenika, datum, vrijeme, 4, idAplikacije);
        }

        public List<_Pozicija> PozicijeRedara(string grad, int minuta)
        {
            return Mapa.PozicijeRedara(grad, minuta, 4, idAplikacije);
        }

        public _Pozicija PozicijaOdabranogRedara(string grad, int idDjelatnika)
        {
            return Mapa.PozicijaOdabranogRedara(grad, idDjelatnika, idAplikacije);
        }

        /*:: RUČNO DODAVANJE PREKRSAJA ::*/

        public List<string> BrojPrekrsaja(string grad, string registracija)
        {
            return Prekrsaj.BrojPrekrsaja(grad, registracija, idAplikacije);
        }

        public int DodajRucniPrekrsaj(string grad, _Prekrsaj prekrsaj, bool obavijest, List<byte[]> slike)
        {
            return Prekrsaj.DodajRucniPrekrsaj(grad, prekrsaj, obavijest, slike, 4, false, idAplikacije);
        }

        /*:: IZMJENE PREKRSAJA ::*/

        public bool RelokacijaPrekrsaja(string grad, int idPrekrsaja, decimal latitude, decimal longitude)
        {
            return Prekrsaj.RelokacijaPrekrsaja(grad, idPrekrsaja, latitude, longitude, true, idAplikacije);
        }

        public bool Registracija(string grad, int idOpazanja, string registracija, string kratica)
        {
            return Prekrsaj.Registracija(grad, idOpazanja, registracija, kratica, true, idAplikacije);
        }

        public bool Adresa(string grad, int idPrekrsaja, string adresa)
        {
            return Prekrsaj.Adresa(grad, idPrekrsaja, adresa, idAplikacije);
        }

        public bool VoziloOtislo(string grad, int idOpazanja)
        {
            return Prekrsaj.VoziloOtislo(grad, idOpazanja, idAplikacije);
        }

        #endregion

        /*:: PRETRAGA ::*/

        #region  PRETRAGA

        public List<_Prekrsaj> ZabiljezeniPrekrsaji(string grad, int idDjelatnika, int idPredloska, DateTime? datumOd, DateTime? datumDo, bool pauk,
        bool registracija, bool dokument, bool ulica, bool storno, char? tipStorna, string pojam, bool test, bool hr)
        {
            return Pretraga.ZabiljezeniPrekrsaji(grad, idDjelatnika, idPredloska, datumOd, datumDo, pauk, registracija, dokument, ulica, storno,
                tipStorna, pojam, test, hr, 4, idAplikacije);
        }

        /*:: PONAVLJACI ::*/

        public List<_Opazanje> DetaljiPonavljaca(string grad, int idStatusa, DateTime? datumOd, DateTime? datumDo, string registracija, bool kaznjeni)
        {
            return Pretraga.DetaljiPonavljacaParking(grad, idStatusa, datumOd, datumDo, registracija, kaznjeni, idAplikacije);
        }

        public List<_2DLista> Ponavljaci(string grad, int idStatusa, DateTime? datumOd, DateTime? datumDo, int broj)
        {
            return Pretraga.PonavljaciParking(grad, idStatusa, datumOd, datumDo, broj, idAplikacije);
        }

        #endregion

        /*:: STATISTIKA ::*/

        public List<_Statistika> ObavljenihOpazanja(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.ObavljenihOpazanja(grad, datumOd, datumDo, idAplikacije);
        }

        public List<_Statistika> IzdaneKazne(string grad, DateTime datumOd, DateTime datumDo)
        {
            return Statistika.IzdaneKazne(grad, datumOd, datumDo, idAplikacije);
        }
        
        public List<_Statistika> IzdanePretplate(string grad, DateTime datumOd, DateTime datumDo, int idSubjekta)
        {
            return Statistika.IzdanePretplate(grad, datumOd, datumDo, idSubjekta, 4, idAplikacije);
        }

        /*:: ZONE ::*/

            public List<_Zone> Zone(string grad)
        {
            return Parking.Zone(grad, idAplikacije);
        }

        public int DodajZonu(string grad, _Zone zona)
        {
            return Parking.DodajZonu(grad, zona, idAplikacije);
        }

        public bool IzmjeniZonu(string grad, _Zone zona)
        {
            return Parking.IzmjeniZonu(grad, zona, idAplikacije);
        }

        public bool ObrisiZonu(string grad, int idZone)
        {
            return Parking.ObrisiZonu(grad, idZone, idAplikacije);
        }

        /*:: SEKTORI ::*/

        public List<_Sektori> Sektori(string grad)
        {
            return Parking.Sektori(grad, idAplikacije);
        }

        public int DodajSektor(string grad, _Sektori sektor)
        {
            return Parking.DodajSektor(grad, sektor, idAplikacije);
        }

        public bool IzmjeniSektor(string grad, _Sektori sektor)
        {
            return Parking.IzmjeniSektor(grad, sektor, idAplikacije);
        }

        public bool ObrisiSektor(string grad, int idSektora)
        {
            return Parking.ObrisiSektor(grad, idSektora, idAplikacije);
        }

        /*:: MOBILE ::*/

        public _Aktiviran AktivacijaAplikacije(string aktivacijskiKod)
        {
            _Grad grad = Sustav.AktivacijaAplikacije(aktivacijskiKod, idAplikacije);

            if (grad == null)
            {
                return null;
            }

            return new _Aktiviran(grad.Baza, grad.Naziv);
        }

        public void SpremiError(string grad, Exception greska, string napomena, string korisnik)
        {
            Sustav.SpremiGresku(grad, greska, idAplikacije, napomena, korisnik);
        }

        public _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS)
        {
            return Korisnici.Prijava(grad, korisnickoIme, zaporka, 4, out blokiranaJLS, idAplikacije);
        }

        public string Naplati(string grad, int idKorisnika, int kolicina)
        {
            try
            {
                int idVrstePlacanja = 1;

                string vrsta = Naplata.VrstaPlacanja(grad, idVrstePlacanja, idAplikacije);
                _PoslovniProstor pp = PoslovniProstor.DohvatiPoslovniProstor(grad, 4, idAplikacije);

                if (pp == null)
                {
                    return "Niste definirali poslovni prostor!";
                }

                _Djelatnik djel = Korisnici.DohvatiDjelatnika(grad, idKorisnika, idAplikacije);

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    List<_Stavka> stavke = new List<_Stavka>();
                    RACUNI_STAVKE_OPI st = db.RACUNI_STAVKE_OPIs.First(i => i.IDRedarstva == 4 && i.Obrisan == false && i.IDStatusa == null);

                    decimal ukupno = (decimal)(kolicina * st.Iznos);
                    _Stavka nova = new _Stavka(0, 0, st.IDOpisaStavke, st.NazivOpisaStavke, st.Lezarina, kolicina, (decimal)st.Iznos, 0, 0, ukupno, pp.PDV, "");
                    stavke.Add(nova);

                    List<_Osoba> osobe = new List<_Osoba>();

                    int blagajna = 1;

                    _Racun novi = new _Racun(0, -1, null, idVrstePlacanja, null, null, vrsta, "", idKorisnika, djel.ImeNaRacunu, 4, DateTime.Now, 0, 0, 0, 0, ukupno,
                        pp.PDV, djel.OIB ?? "", blagajna, "", false, "", "", true, "", "", "", DateTime.Now, pp.Oznaka, "", "", "", "", false, false, false, false, "", stavke, osobe);

                    return Naplata.NaplatiParking(grad, novi, false, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATA PARKINGA S UREĐAJA");
                return "";
            }
        }

        public string DnevnaKarta(string grad, int idKorisnika, string registracija, int? idLokacije, int idVrste)
        {
            try
            {
                string vrsta = Naplata.VrstaPlacanja(grad, idVrste, idAplikacije);
                _PoslovniProstor pp = PoslovniProstor.DohvatiPoslovniProstor(grad, 4, idAplikacije);

                if (pp == null)
                {
                    return "Niste definirali poslovni prostor!";
                }

                _Djelatnik djel = Korisnici.DohvatiDjelatnika(grad, idKorisnika, idAplikacije);

                List<_Stavka> stavke = new List<_Stavka>();
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RACUNI_STAVKE_OPI st = db.RACUNI_STAVKE_OPIs.First(i => i.IDRedarstva == 4 && i.Obrisan == false && i.IDStatusa == 1);

                    decimal ukupno = (decimal)(1 * st.Iznos);
                    _Stavka nova = new _Stavka(0, 0, st.IDOpisaStavke, st.NazivOpisaStavke, st.Lezarina, 1, (decimal)st.Iznos, 0, 0, ukupno, pp.PDV, "");
                    stavke.Add(nova);

                    List<_Osoba> osobe = new List<_Osoba>();

                    int blagajna = 1;

                    _Racun novi = new _Racun(0, idLokacije ?? -1, null, idVrste, null, null, vrsta, "", idKorisnika, djel.ImeNaRacunu, 4, DateTime.Now, 0, 0, 0, 0, ukupno,
                        pp.PDV, djel.OIB ?? "", blagajna, "", false, "", registracija, true, "", "", "", DateTime.Now, pp.Oznaka, "", "", "", "", false, false, false, false, "", stavke, osobe);

                    return Naplata.NaplatiParking(grad, novi, idVrste == 4, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATA PARKINGA S UREĐAJA");
                return "";
            }
        }

        public string VrijemeDolaska()
        {
            return Ispis.VrijemeDolaska(idAplikacije);
        }

        /**/

        public int SpremiLokaciju(string grad, string registracija, int idDjelatnika)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    GRADOVI grd = db.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad));

                    _Lokacija lokacija = new _Lokacija(0, idDjelatnika, 0, 3, grd.Latitude, grd.Longitude, "", DateTime.Now, null, null, 0, 0, 0, 0, false);

                    return Mobile.SpremiLokaciju(grad, lokacija, false, idAplikacije);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI LOKACIJU - PARKING");
                return -1;
            }
        }

        public bool SpremiFotografiju(string grad, int idLokacije, byte[] fotografija)
        {
            return Mobile.SpremiFotografiju(grad, idLokacije, fotografija, idAplikacije);
        }

        public bool Storno(string grad, int idKorisnkika)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    RACUNI dv = db.RACUNIs.OrderByDescending(i => i.IDRacuna).First(i => i.IDRedarstva == 4);

                    if (dv.Storniran)
                    {
                        return false;
                    }

                    string brrac;
                    _Racun racun = Naplata.DohvatiRacun(grad, dv.IDRacuna, true, idAplikacije);

                    racun.IDDjelatnika = idKorisnkika;
                    racun.DatumVrijeme = DateTime.Now;
                    racun.OIB = db.Djelatniks.First(i => i.IDDjelatnika == idKorisnkika).OIB;
                    racun.Operater = db.Djelatniks.First(i => i.IDDjelatnika == idKorisnkika).ImeNaRacunu;
                    racun.Orginal = racun.BrojRacuna;
                    racun.Storniran = true;
                    racun.Osnovica = racun.Osnovica * -1;
                    racun.Ukupno = racun.Ukupno * -1;
                    racun.PDV = racun.PDV * -1;
                    //racun.Blagajna = racun.Blagajna;
                    //racun.DatumPreuzimanja = racun.DatumPreuzimanja;
                    racun.Godina = DateTime.Now.Year;
                    //racun.IDRedarstva = 4;
                    //racun.IDReference = racun.IDReference;
                    //racun.IDVrste = racun.IDVrste;
                    //racun.Napomena = racun.Napomena;
                    //racun.NazivVrste = racun.NazivVrste;
                    //racun.Osobe = racun.Osobe;
                    //racun.OznakaPP = racun.OznakaPP;
                    //racun.PDVPosto = racun.PDVPosto;
                    //racun.PozivNaBr = racun.PozivNaBr;

                    int id = Naplata.StornirajRacun(grad, racun, 0, null, null, out brrac, idAplikacije);

                    if (id == -1)
                    {
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATA PARKINGA S UREĐAJA");
                return false;
            }
        }
    }
}
