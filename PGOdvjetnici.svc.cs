using System;
using System.Collections.Generic;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi
{
    public class PGOdvjetnici : IPGOdvjetnici
    {
        private const int idAplikacije = 8;

        public bool Dostupan()
        {
            return true;
        }

        public _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS)
        {
            return Korisnici.Prijava(grad, korisnickoIme, zaporka, 0, out blokiranaJLS, idAplikacije);
        }

        public List<_Drzava> Drzave()
        {
            return Sustav.Drzave(idAplikacije);
        }

        public bool DodajDrzavu(string kratica, string naziv)
        {
            return Sustav.DodajDrzavu(kratica, naziv, idAplikacije);
        }

        public _Uplatnica Uplatnica(string grad, int idRedarstva)
        {
            return Gradovi.Uplatnica(grad, idRedarstva, idAplikacije);
        }

        /*:: AKTIVNOST ::*/

        public void Aktivan(string grad, string korisnik, string racunalo, string verzija, string os, int idKorisnika)
        {
            Sustav.Aktivnost(grad, idKorisnika, racunalo, verzija, korisnik, os, false, idAplikacije);
        }

        public void SpremiError(string grad, Exception greska, string napomena, string korisnik)
        {
            Sustav.SpremiGresku(grad, greska, idAplikacije, napomena, korisnik);
        }

        public void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena)
        {
            Sustav.SpremiAkciju(grad, idKorisnika, idAkcije, napomena, 1, idAplikacije);
        }

        public List<_Grad> GradoviOdvjetnika(bool obradjeni)
        {
            return Gradovi.GradoviOdvjetnika(obradjeni, idAplikacije);
        }

        /*:: POSTUPANJA ::*/

        public List<_VppPostupanja> DohvatiPostupanja(string grad, bool nepreuzeti, string drzava)
        {
            return Odvjetnici.DohvatiPostupanja(grad, nepreuzeti, drzava, idAplikacije);
        }

        public bool OznaciPreuzete(string grad, List<int> preuzeti)
        {
            return Odvjetnici.OznaciPreuzete(grad, preuzeti, idAplikacije);
        }

        public _Prekrsaj DohvatiPrekrsaj(string grad, int idLokacije)
        {
            return Prekrsaj.DetaljiPrekrsajaO(grad, idLokacije, idAplikacije);
        }

        public List<_Slika> SlikePrekrsaja(string grad, int idLokacije)
        {
            return Prekrsaj.SlikePrekrsaja(grad, idLokacije, idAplikacije);
        }

        public List<_VppPostupanja> PretraziPostupanja(string grad, string registracija, string poziv, int idStatusa, string drzava)
        {
            return Odvjetnici.PretraziPostupanja(grad, registracija, poziv, idStatusa, drzava, idAplikacije);
        }

        public List<_VppPostupanja> PretraziPostupanjaZatvaranje(string grad, DateTime? datumOd, DateTime? datumDo, string drzava)
        {
            return Odvjetnici.PretraziPostupanjaZatvaranje(grad, datumOd, datumDo, drzava, idAplikacije);
        }

        /*:: REGISTRACIJA ::*/

        public List<_VppRegistracije> DohvatiProvjeruRegistracije(string grad, int max, out int br)
        {
            return Odvjetnici.DohvatiProvjeruRegistracije(grad, max, out br, idAplikacije);
        }

        public bool PromijeniRegistraciju(string grad, int idPrekrsaja, int idVppVanjsko, string registracija, string kratica, int idMarke)
        {
            return Odvjetnici.PromijeniRegistraciju(grad, idPrekrsaja, idVppVanjsko, registracija, kratica, idMarke, idAplikacije);
        }

        public bool OdbijRegistraciju(string grad, int idVppVanjsko, string status, string napomena)
        {
            return Odvjetnici.OdbijRegistraciju(grad, idVppVanjsko, status, napomena, idAplikacije);
        }

        public List<byte[]> DohvatiSlike(string grad, int idLokacije)
        {
            return Prekrsaj.Slike(grad, idLokacije, idAplikacije);
        }

        public bool OdobriSveRegistracije(string grad)
        {
            return Odvjetnici.OdobriSveRegistracije(grad, idAplikacije);
        }

        /*:: MARKA ::*/

        public List<_2DLista> MarkaVozila()
        {
            return Odvjetnici.MarkaVozila(idAplikacije);
        }

        public bool DodajMarkuVozila(string marka)
        {
            return Odvjetnici.DodajMarkuVozila(marka, idAplikacije);
        }

        /*:: STATUS ::*/

        public List<_2DLista> Statusi(string grad)
        {
            return Odvjetnici.Statusi(grad, idAplikacije);
        }

        public bool IzmjeniStatus(string grad, List<_VppPostupanja> postupanja, int idStatusa, string napomena)
        {
            return Odvjetnici.IzmjeniStatus(grad, postupanja, idStatusa, napomena, idAplikacije);
        }

        public bool IzmjeniStatuse(string grad, List<_VppPostupanja> postupanja, string poziv, string napomena, DateTime datum, decimal iznos)
        {
            try
            {
                Odvjetnici.IzmjeniStatus(grad, postupanja, 7, napomena, idAplikacije);

                foreach (_VppPostupanja post in postupanja)
                {
                    Odvjetnici.ZavediUplatu(grad, post, napomena, poziv, datum, idAplikacije);
                }

                Odvjetnici.UkupnaUplata(grad, poziv, datum, iznos, postupanja.Count, idAplikacije);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<_Uplata> Uplate(int idGrada, DateTime? datumOd, DateTime? datumDo)
        {
            return Odvjetnici.Uplate(idGrada, datumOd, datumDo, idAplikacije);
        }

        public List<_VppPostupanja> UplacenaPostupanja(string grad, string poziv)
        {
            return Odvjetnici.UplacenaPostupanja(grad, poziv, idAplikacije);
        }

        /*:: PRILOG ::*/

        public byte[] Prilog(string grad, int idPostupanja)
        {
            return Odvjetnici.Prilog(grad, idPostupanja, idAplikacije);
        }
    }
}
