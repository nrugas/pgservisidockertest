using System;
using System.Collections.Generic;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi
{
    public class PGMup : IPGMup
    {
        private const int idAplikacije = 6;

        public bool Dostupan()
        {
            return true;
        }

        public _Grad AktivacijaAplikacije(string aktivacijskiKod)
        {
            return Sustav.AktivacijaAplikacije(aktivacijskiKod, idAplikacije);
        }

        public _Grad Grad(string grad)
        {
            return Gradovi.Grad(grad, idAplikacije);
        }

        public _Predlozak DohvatiPredlozak(string grad, int idPredloska)
        {
            return Predlosci.DohvatiPredlozakIspisa(grad, idPredloska, idAplikacije);
        }

        /*:: AKTIVNOST ::*/

        public void Aktivan(string grad, string korisnik, string racunalo, string verzija, string os, int idKorisnika)
        {
            Sustav.Aktivnost(grad, idKorisnika, racunalo, verzija, korisnik, os, true, idAplikacije);
        }

        public void SpremiError(string grad, Exception greska, string napomena, string korisnik)
        {
            Sustav.SpremiGresku(grad, greska, idAplikacije, napomena, korisnik);
        }

        public void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena)
        {
            Sustav.SpremiAkciju(grad, idKorisnika, idAkcije, napomena, 1, idAplikacije);
        }

        public _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS)
        {
            return Korisnici.Prijava(grad, korisnickoIme, zaporka, -1, out blokiranaJLS, idAplikacije);
        }

        public bool AktivniKorisnik(string grad)
        {
            return Zahtjev.AktivniKorisnik(grad, idAplikacije);
        }

        /*:: STATUS KAŽNJAVANJA VOZILA ::*/

        public List<_Kaznjavan> Kaznjavan(string grad, string registracija, string drzava, int dana)
        {
            return Mobile.Kaznjavan(grad, registracija, drzava, dana, false, idAplikacije);
        }

        public List<_Vozilo> VozilaPauka(string grad, bool obrisana)
        {
            return Vozila.VozilaPauka(grad, obrisana, idAplikacije);
        }

        /*:: ZAHTJEVI ::*/

        public List<_ZahtjevPauka> Zahtjevi(string grad, int idVozila, int idStatusa, DateTime? datumOd, DateTime? datumDo, int idRedarstva)
        {
            return Zahtjev.Zahtjevi(grad, idVozila, idStatusa, datumOd, datumDo, idRedarstva, idAplikacije);
        }

        /**/

        public List<_Drzava> Drzave()
        {
            return Sustav.Drzave(idAplikacije);
        }

        public List<_Zakon> DohvatiZakone(string grad, bool neaktivni)
        {
            return Zakoni.DohvatiZakoneS(grad, neaktivni, 1, false, idAplikacije);
        }

        public _Prekrsaj DetaljiPrekrsaja(string grad, int idLokacije)
        {
            return Prekrsaj.DetaljiPrekrsaja(grad, idLokacije, idAplikacije);
        }

        /*:: ZAHTJEVI ::*/

        public int Neobradjeni(string grad, int idDjelatnika, out _PrijavaPauk prijava)
        {
            return Zahtjev.Neobradjeni(grad, idDjelatnika, 0, out prijava, idAplikacije);
        }

        public List<_Slika> Slike(string grad, int idLokacije)
        {
            return Prekrsaj.SlikePrekrsaja(grad, idLokacije, idAplikacije);
        }

        public bool Preuzmi(string grad, int idZahtjeva, int idDjelatnika)
        {
            return Zahtjev.Preuzmi(grad, idZahtjeva, idDjelatnika, idAplikacije);
        }

        public void Odustani(string grad, int idZahtjeva, int idDjelatnika)
        {
            Zahtjev.Odustani(grad, idZahtjeva, idDjelatnika, idAplikacije);
        }

        public int BrojSlikaZahtjeva(string grad, int idLokacije)
        {
            return Zahtjev.BrojSlikaZahtjeva(grad, idLokacije, idAplikacije);
        }

        public bool StatusZahtjeva(string grad, int idZahtjeva)
        {
            return Zahtjev.StatusZahtjeva(grad, idZahtjeva, idAplikacije);
        }

        public void SpremiAkcijuZahtjeva(string grad, int idZahtjeva, int idDjelatnika, int idAkcije)
        {
            Zahtjev.SpremiAkcijuZahtjeva(grad, idZahtjeva, idDjelatnika, idAkcije, idAplikacije);
        }

        public int DodajPrekrsaj(string grad, _PrijavaPauk zahtjev, int idOpisa, decimal kazna, string registracija, string adresa, string drzava, bool kaznjava, bool nalogPauku, bool lisice, int idRedarstva)
        {
            return Zahtjev.DodajPrekrsaj(grad, zahtjev, idOpisa, kazna, registracija,adresa, drzava, kaznjava, nalogPauku, lisice, idRedarstva, idAplikacije);
        }

        public bool StatusZahtjevaPaukOdustao(string grad, int idZahtjeva)
        {
            return Zahtjev.StatusZahtjevaPaukOdustao(grad, idZahtjeva, idAplikacije);
        }

        public bool Zatvori(string grad, int idZahtjeva, int idStatusa, int idDjelatnika, string razlog, int idRedarstva)
        {
            return Zahtjev.Zatvori(grad, idZahtjeva, idStatusa, idDjelatnika, null, null, razlog, idRedarstva, idAplikacije);
        }

        public string GenerirajPozivNaBroj(string grad, decimal kazna)
        {
            return Prekrsaj.GenerirajPozivNaBroj(grad, true, DateTime.Now, kazna, 1);
        }

        /*:: PROVJERA ::*/

        public void Preuzeti(string grad, int idDjelatnika)
        {
            //Zahtjev.Preuzeti(grad, idDjelatnika, idAplikacije);
        }

        public _Vozilo Vozilo(string grad, int idVozila)
        {
            return Vozila.Vozilo(grad, idVozila, idAplikacije);
        }

        public List<_Predlozak> PredlosciIspisa(string grad)
        {
            return Predlosci.PredlosciIspisa(grad, 0, idAplikacije);
        }

        public _ZahtjevPauka DetaljiZahtjeva(string grad, int idZahtjeva)
        {
            return Zahtjev.DohvatiZahtjev(grad, idZahtjeva, idAplikacije);
        }
    }
}
