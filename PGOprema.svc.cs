using System;
using System.Collections.Generic;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi
{
    public class PGOprema : IPGOprema
    {
        private const int idAplikacije = 12;

        public bool Dostupan()
        {
            return true;
        }

        public List<_Stanje> Stanje()
        {
            return Oprema.Stanje(idAplikacije);
        }

        public void SpremiError(string grad, Exception greska, string napomena, string korisnik)
        {
            Sustav.SpremiGresku(grad, greska, idAplikacije, napomena, korisnik);
        }

        public List<_2DLista> Redarstva()
        {
            return Postavke.Redarstva(idAplikacije);
        }

        public List<_2DLista> PopisGradova()
        {
            return Gradovi.PopisGradova(0, idAplikacije);
        }

        public List<_Printer> DohvatiPrintere()
        {
            return Postavke.DohvatiPrintere("Lokacije", false, 0, idAplikacije);
        }

        /**/

        public List<_2DLista> VrstaOpreme()
        {
            return Oprema.VrstaOpreme(idAplikacije);
        }

        //model
        public int DodajModel(string naziv, int idVrste)
        {
            return Oprema.DodajModel(naziv, idVrste, idAplikacije);
        }

        public bool IzmjeniModel(int idModela, string naziv, int idVrste)
        {
            return Oprema.IzmjeniModel(idModela, naziv, idVrste, idAplikacije);
        }

        public bool? ObrisiModel(int idModela)
        {
            return Oprema.ObrisiModel(idModela, idAplikacije);
        }

        public List<_Model> Modeli(int? idVrste)
        {
            return Oprema.Modeli(idVrste, idAplikacije);
        }

        //os
        public int DodajOS(string naziv)
        {
            return Oprema.DodajOS(naziv, idAplikacije);
        }

        public bool IzmjeniOS(int idOs, string naziv)
        {
            return Oprema.IzmjeniOS(idOs, naziv, idAplikacije);
        }

        public bool? ObrisiOs(int idOs)
        {
            return Oprema.ObrisiOs(idOs, idAplikacije);
        }

        public List<_2DLista> OS()
        {
            return Oprema.OS(idAplikacije);
        }

        //tarifa
        public int DodajTarifu(string naziv, string opis)
        {
            return Oprema.DodajTarifu(naziv, opis, idAplikacije);
        }      
               
        public bool IzmjeniTarifu(int idTarife, string naziv, string opis)
        {
            return Oprema.IzmjeniTarifu(idTarife, naziv, opis, idAplikacije);
        }      
               
        public bool? ObrisiTarifu(int idTarife)
        {
            return Oprema.ObrisiTarifu(idTarife, idAplikacije);
        }

        public List<_3DLista> Tarife()
        {
            return Oprema.Tarife(idAplikacije);
        }

        //memorija
        public List<_2DLista> Memorija()
        {
            return Oprema.Memorija(idAplikacije);
        }

        /**/

        public List<_Oprema> PretraziOpremu(int idVrste, string broj, bool? tip)
        {
            return Oprema.PretraziOpremu(idVrste, broj, tip);
        }

        /*:: IZMJENA ::*/

        public bool Izmjeni(_Oprema oprema)
        {
            return Oprema.Izmjeni(oprema, idAplikacije);
        }

        public bool IzmjeniInteni(int idVrsteOpreme, int idOpreme, int interniBroj)
        {
            return Oprema.IzmjeniInterni(idVrsteOpreme, idOpreme, interniBroj, idAplikacije);
        }

        /*:: POVJEST ::*/

        public void PovijestOpreme(int idOpreme, int idVrste, int idStatusa, string napomena, DateTime datum)
        {
            Oprema.PovijestOpreme(idOpreme, idVrste, idStatusa, napomena, datum, idAplikacije);
        }

        public List<_PovijestOpreme> DohvatiPovijestOpreme(int idOpreme, int idVrste, int idStatusa)
        {
            return Oprema.DohvatiPovijestOpreme(idOpreme, idVrste, idStatusa, idAplikacije);
        }

        public int NoviStatus(string status)
        {
            return Oprema.NoviStatus(status, idAplikacije);
        }

        public bool ObrisiPovijestOpreme(int idPovijesti, bool servis)
        {
            return Oprema.ObrisiPovijestOpreme(idPovijesti, servis, idAplikacije);
        }

        public bool Servis(_Servis servis)
        {
            return Oprema.Servis(servis);
        }

        public List<_2DLista> StatusiPovijesti()
        {
            return Oprema.StatusiPovijesti();
        }

        /*:: DODAJ ::*/

        public int InterniBroj()
        {
            return Oprema.InterniBroj(idAplikacije);
        }

        public int Dodaj(_Oprema oprema)
        {
            return Oprema.Dodaj(oprema, idAplikacije);
        }

        /*:: PRILOG ::*/

        public bool DodajPrilog(List<_Prilog> prilog)
        {
            return Oprema.DodajPrilog(prilog, idAplikacije);
        }

        public bool ObrisiPrilog(int idPriloga)
        {
            return Oprema.ObrisiPrilog(idPriloga, idAplikacije);
        }

        public List<_Prilog> Prilozi(int idVrste, int idOpreme)
        {
            return Oprema.Prilozi(idVrste, idOpreme, idAplikacije);
        }

        public _Prilog Prilog(int idPriloga)
        {
            return Oprema.Prilog(idPriloga, idAplikacije);
        }
    }
}
