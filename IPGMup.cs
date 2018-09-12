using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace PG.Servisi
{
    [ServiceContract]
    public interface IPGMup
    {
        [OperationContract]
        bool Dostupan();

        [OperationContract]
        _Grad Grad(string grad);

        /*:: AKTIVNOST ::*/

        [OperationContract]
        _Grad AktivacijaAplikacije(string aktivacijskiKod);

        [OperationContract]
        void Aktivan(string grad, string korisnik, string racunalo, string verzija, string os, int idKorisnika);

        [OperationContract]
        void SpremiError(string grad, Exception greska, string napomena, string korisnik);

        [OperationContract]
        void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena);

        [OperationContract]
        _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS);

        [OperationContract]
        bool AktivniKorisnik(string grad);

        [OperationContract]
        _Predlozak DohvatiPredlozak(string grad, int idPredloska);

        /*:: STATUS KAŽNJAVANJA VOZILA ::*/

        [OperationContract]
        List<_Kaznjavan> Kaznjavan(string grad, string registracija, string drzava, int dana);

        [OperationContract]
        List<_Vozilo> VozilaPauka(string grad, bool obrisana);

        /*:: ZAHTJEVI ::*/

        [OperationContract]
        List<_ZahtjevPauka> Zahtjevi(string grad, int idVozila, int idStatusa, DateTime? datumOd, DateTime? datumDo, int idRedarstva);

        /**/

        [OperationContract]
        List<_Drzava> Drzave();

        [OperationContract]
        List<_Zakon> DohvatiZakone(string grad, bool neaktivni);

        [OperationContract]
        _Prekrsaj DetaljiPrekrsaja(string grad, int idLokacije);

        /*:: ZAHTJEVI ::*/

        [OperationContract]
        int Neobradjeni(string grad, int idDjelatnika, out _PrijavaPauk prijava);

        [OperationContract]
        List<_Slika> Slike(string grad, int idLokacije);

        [OperationContract]
        bool Preuzmi(string grad, int idZahtjeva, int idDjelatnika);

        [OperationContract]
        void Odustani(string grad, int idZahtjeva, int idDjelatnika);

        [OperationContract]
        int BrojSlikaZahtjeva(string grad, int idLokacije);

        [OperationContract]
        bool StatusZahtjeva(string grad, int idZahtjeva);

        [OperationContract]
        void SpremiAkcijuZahtjeva(string grad, int idZahtjeva, int idDjelatnika, int idAkcije);

        [OperationContract]
        int DodajPrekrsaj(string grad, _PrijavaPauk zahtjev, int idOpisa, decimal kazna, string registracija,
            string adresa, string drzava, bool kaznjava, bool nalogPauku, bool lisice, int idRedarstva);

        [OperationContract]
        bool StatusZahtjevaPaukOdustao(string grad, int idZahtjeva);

        [OperationContract]
        bool Zatvori(string grad, int idZahtjeva, int idStatusa, int idDjelatnika, string razlog, int idRedarstva);

        [OperationContract]
        string GenerirajPozivNaBroj(string grad, decimal kazna);

        /**/

        [OperationContract]
        void Preuzeti(string grad, int idDjelatnika);

        [OperationContract]
        _Vozilo Vozilo(string grad, int idVozila);

        [OperationContract]
        List<_Predlozak> PredlosciIspisa(string grad);

        [OperationContract]
        _ZahtjevPauka DetaljiZahtjeva(string grad, int idZahtjeva);
    }

    [DataContract]
    public class _PrijavaPauk
    {
        [DataMember]
        public int IDPrijave { get; set; }
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public int? IDPrijavitelja { get; set; }
        [DataMember]
        public int? IDOdobravatelja { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public decimal Lat { get; set; }
        [DataMember]
        public decimal Lng { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string Drzava { get; set; }
        [DataMember]
        public int IDStatusa { get; set; }
        [DataMember]
        public int? IDOpisa { get; set; }
        [DataMember]
        public List<_Slika> Slike { get; set; }

        public _PrijavaPauk(int idprijave, int idlokacije, int? idprijavitelja, int? idodobravatelja, DateTime datumvrijeme,
            string registracija, decimal lat, decimal lng, string adresa, string drzava, int idstatusa, int? idOpisa, List<_Slika> slike)
        {
            IDPrijave = idprijave;
            IDLokacije = idlokacije;
            IDPrijavitelja = idprijavitelja;
            IDOdobravatelja = idodobravatelja;
            DatumVrijeme = datumvrijeme;
            Registracija = registracija;
            Lat = lat;
            Lng = lng;
            Adresa = adresa;
            Drzava = drzava;
            IDStatusa = idstatusa;
            IDOpisa = idOpisa;
            Slike = slike;
        }
    }
}
