using System.Collections.Generic;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi
{
    public class PGGO : IPGGO
    {
        private const int idAplikacije = 16;

        /*:: DJELATNICI ::*/

        public int DodajNovogDjelatnika(string grad, _Djelatnik korisnik)
        {
            return Korisnici.DodajNovogDjelatnika(grad, korisnik, false, idAplikacije);
        }

        /*:: PREDLOZAK ::*/

        public _Predlozak DohvatiPredlozak(string grad, int idPredloska)
        {
            return Predlosci.DohvatiPredlozakIspisa(grad, idPredloska, idAplikacije);
        }

        /*:: PREKRSAJ ::*/

        public _Prekrsaj Detalji(string grad, int idLokacije)
        {
            return Prekrsaj.DetaljiPrekrsaja(grad, idLokacije, idAplikacije);
        }

        public int DodajRucniPrekrsaj(string grad, _Prekrsaj prekrsaj, bool obavijest, List<byte[]> slike)
        {
            return Prekrsaj.DodajRucniPrekrsaj(grad, prekrsaj, obavijest, slike, 3, false, idAplikacije);
        }

        public void DodajFotografiju(string grad, int idLokacije, byte[] s)
        {
            Prekrsaj.DodajFotografijuGO(grad, idLokacije, s, idAplikacije);
        }

        public List<_Opis> DohvatiOpiseZakonaGOS(string grad)
        {
            return Zakoni.DohvatiOpiseZakonaGOS(grad, idAplikacije);
        }

        public int IznosKazneS(string grad, int idOpisa)
        {
            return Zakoni.IznosKazneS(grad, idOpisa, idAplikacije);
        }

        public _Vozilo Vozilo(string grad, int idVozila)
        {
            return Vozila.Vozilo(grad, idVozila, idAplikacije);
        }
    }
}
