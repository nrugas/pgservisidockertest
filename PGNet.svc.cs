using System;
using System.Collections.Generic;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi
{
    public class PGNet : IPGNet
    {
        private const int idAplikacije = 7;

        public List<_JLS> PopisGradova()
        {
            return Gradovi.GradoviWeb(idAplikacije);
        }

        public void SpremiError(string grad, Exception greska, string napomena, string korisnik)
        {
            Sustav.SpremiGresku(grad, greska, idAplikacije, napomena, korisnik);
        }

        public _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS)
        {
            return Korisnici.Prijava(grad, korisnickoIme, zaporka, 1, out blokiranaJLS, idAplikacije);
        }

        public bool IspunjeniPodaci(string grad, int idKorisnika)
        {
            return Sustav.IspunjeniPodaci(grad, idKorisnika, idAplikacije);
        }

        /*:: DJELATNICI ::*/

        public List<_Djelatnik> DohvatiDjelatnike(string grad)
        {
            return Korisnici.DohvatiDjelatnikeNET(grad, idAplikacije);
        }

        public int DodajNovogDjelatnika(string grad, _Djelatnik korisnik)
        {
            return Korisnici.DodajNovogDjelatnika(grad, korisnik, false, idAplikacije);
        }

        public List<_2DLista> DohvatiPrivilegije(string grad)
        {
            return Korisnici.DohvatiPrivilegije(grad, idAplikacije);
        }

        public bool IzmjeniZaporku(string grad, int  idDjelatnika, string lozinka)
        {
            return Korisnici.IzmjeniZaporku(grad, idDjelatnika, lozinka, false, idAplikacije);
        }

        /*:: UPLATNICA ::*/

        public _Uplatnica Uplatnica(string grad)
        {
            return Gradovi.Uplatnica(grad, 1, idAplikacije);
        }

        public int IzmjeniUplatnicu(string grad, _Uplatnica uplatnica)
        {
            return Gradovi.IzmjeniUplatnicu(grad, uplatnica, idAplikacije);
        }

        public decimal IznosNaUplatnici(int idGrada)
        {
            return Gradovi.IznosNaUplatnici(idGrada, idAplikacije);
        }
        
        /*:: MATERIJALI ::*/

        public List<_Narudzba> DohvatiNarudzbe(string grad)
        {
            return Sustav.DohvatiNarudzbe(grad, idAplikacije);
        }

        public bool NovaNarudzba(string grad, _Narudzba narudzba)
        {
            return Sustav.NaruciMaterijal(grad, narudzba, idAplikacije);
        }

        /*:: PODRSKA ::*/

        public List<_2DLista> Aplikacije()
        {
            return Sustav.Aplikacije(idAplikacije);
        }

        public bool NaruciMaterijal(string grad, _Narudzba narudzba)
        {
            return Sustav.NaruciMaterijal(grad, narudzba, idAplikacije);
        }

        public bool PrijaviProblem(string grad, _Problem problem, List<byte[]> slike)
        {
            return Sustav.PrijaviProblem(grad, problem, slike, idAplikacije);
        }

        public bool PostaviPitanje(string grad, int idKorisnika, int idPodrucja, string poruka, List<byte[]> slike)
        {
            return Sustav.PostaviPitanje(grad, idKorisnika, idPodrucja, poruka, slike, idAplikacije);
        }
    }
}
