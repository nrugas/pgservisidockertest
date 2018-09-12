using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace PG.Servisi
{
    [ServiceContract]
    public interface IPGNet
    {
        [OperationContract]
        List<_JLS> PopisGradova();

        [OperationContract]
        void SpremiError(string grad, Exception greska, string napomena, string korisnik);

        [OperationContract]
        _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS);

        [OperationContract]
        bool IspunjeniPodaci(string grad, int idKorisnika);

        /*:: DJELATNICI ::*/

        [OperationContract]
        List<_Djelatnik> DohvatiDjelatnike(string grad);

        [OperationContract]
        int DodajNovogDjelatnika(string grad, _Djelatnik korisnik);

        [OperationContract]
        List<_2DLista> DohvatiPrivilegije(string grad);

        [OperationContract]
        bool IzmjeniZaporku(string grad, int idDjelatnika, string lozinka);

        /*:: UPLATNICA ::*/

        [OperationContract]
        _Uplatnica Uplatnica(string grad);

        [OperationContract]
        int IzmjeniUplatnicu(string grad, _Uplatnica uplatnica);

        [OperationContract]
        decimal IznosNaUplatnici(int idGrada);

        /*:: MATERIJALI ::*/

        [OperationContract]
        List<_Narudzba> DohvatiNarudzbe(string grad);

        [OperationContract]
        bool NovaNarudzba(string grad, _Narudzba narudzba);

        /*:: PODRSKA ::*/

        [OperationContract]
        List<_2DLista> Aplikacije();

        [OperationContract]
        bool NaruciMaterijal(string grad, _Narudzba narudzba);

        [OperationContract]
        bool PrijaviProblem(string grad, _Problem problem, List<byte[]> slike);

        [OperationContract]
        bool PostaviPitanje(string grad, int idKorisnika, int idPodrucja, string poruka, List<byte[]> slike);
    }

    [DataContract]
    public class _Narudzba
    {
        [DataMember]
        public int IDNarudzbe { get; set; }
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public int IDKorisnika { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }
        [DataMember]
        public int Tristo { get; set; }
        [DataMember]
        public int Petsto { get; set; }
        [DataMember]
        public int Sedamsto { get; set; }
        [DataMember]
        public int Etuia { get; set; }
        [DataMember]
        public int Traka { get; set; }
        [DataMember]
        public string Napomena { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string Dostava { get; set; }
        [DataMember]
        public string Korisnik { get; set; }
        [DataMember]
        public bool Isporuceno { get; set; }
        [DataMember]
        public int Godina { get; set; }

        public _Narudzba(int idnarudzbe, int idgrada, int idkorisnika, int idRedarstva, DateTime datumVrijeme,  int tristo, int petsto, 
            int sedamsto, int etuia, int traka, string napomena, string adresa, string dostava, string korisnik, bool isporuceno)
        {
            IDNarudzbe = idnarudzbe;
            IDGrada = idgrada;
            IDKorisnika = idkorisnika;
            IDRedarstva = idRedarstva;
            DatumVrijeme = datumVrijeme;
            Tristo = tristo;
            Petsto = petsto;
            Sedamsto = sedamsto;
            Etuia = etuia;
            Traka = traka;
            Napomena = napomena;
            Adresa = adresa;
            Dostava = dostava;
            Korisnik = korisnik;
            Isporuceno = isporuceno;
            Godina = datumVrijeme.Year;
        }
    }

    [DataContract]
    public class _JLS
    {
        [DataMember]
        public string Baza { get; set; }
        [DataMember]
        public string Naziv { get; set; }

        public _JLS(string  baza, string naziv)
        {
            Baza = baza;
            Naziv = naziv;
        }
    }
}
