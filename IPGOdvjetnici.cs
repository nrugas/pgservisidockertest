using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace PG.Servisi
{
    [ServiceContract]
    public interface IPGOdvjetnici
    {
        [OperationContract]
        bool Dostupan();

        [OperationContract]
        _Djelatnik Prijava(string grad, string korisnickoIme, string zaporka, out bool blokiranaJLS);

        [OperationContract]
        List<_Drzava> Drzave();

        [OperationContract]
        bool DodajDrzavu(string kratica, string naziv);

        [OperationContract]
        _Uplatnica Uplatnica(string grad, int idRedarstva);

        /*:: AKTIVNOST ::*/

        [OperationContract]
        void Aktivan(string grad, string korisnik, string racunalo, string verzija, string os, int idKorisnika);

        [OperationContract]
        void SpremiError(string grad, Exception greska, string napomena, string korisnik);

        [OperationContract]
        void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena);

        [OperationContract]
        List<_Grad> GradoviOdvjetnika(bool obradjeni);

        /*:: POSTUPANJA ::*/

        [OperationContract]
        List<_VppPostupanja> DohvatiPostupanja(string grad, bool nepreuzeti, string drzava);

        [OperationContract]
        bool OznaciPreuzete(string grad, List<int> preuzeti);

        [OperationContract]
        _Prekrsaj DohvatiPrekrsaj(string grad, int idLokacije);

        [OperationContract]
        List<_Slika> SlikePrekrsaja(string grad, int idLokacije);

        [OperationContract]
        List<_VppPostupanja> PretraziPostupanja(string grad, string registracija, string poziv, int idStatusa, string drzava);

        [OperationContract]
        List<_VppPostupanja> PretraziPostupanjaZatvaranje(string grad, DateTime? datumOd, DateTime? datumDo, string drzava);

        /*:: REGISTRACIJA ::*/

        [OperationContract]
        List<_VppRegistracije> DohvatiProvjeruRegistracije(string grad, int max, out int br);

        [OperationContract]
        bool PromijeniRegistraciju(string grad, int idPrekrsaja, int idVppVanjsko, string registracija, string kratica, int idMarke);

        [OperationContract]
        bool OdbijRegistraciju(string grad, int idVppVanjsko, string status, string napomena);

        [OperationContract]
        List<byte[]> DohvatiSlike(string grad, int idLokacije);

        [OperationContract]
        bool OdobriSveRegistracije(string grad);

        /*:: MARKA ::*/

        [OperationContract]
        List<_2DLista> MarkaVozila();

        [OperationContract]
        bool DodajMarkuVozila(string marka);

        /*:: STATUS ::*/

        [OperationContract]
        List<_2DLista> Statusi(string grad);

        [OperationContract]
        bool IzmjeniStatus(string grad, List<_VppPostupanja> postupanja, int idStatusa, string napomena);

        [OperationContract]
        bool IzmjeniStatuse(string grad, List<_VppPostupanja> postupanja, string poziv, string napomena, DateTime datum, decimal iznos);

        [OperationContract]
        List<_Uplata> Uplate(int idGrada, DateTime? datumOd, DateTime? datumDo);

        [OperationContract]
        List<_VppPostupanja> UplacenaPostupanja(string grad, string poziv);

        /*:: PRILOG ::*/

        [OperationContract]
        byte[] Prilog(string grad, int idPostupanja);
    }

    [DataContract]
    public class _VppPostupanja
    {
        [DataMember]
        public int IDPreuzimanja { get; set; }
        [DataMember]
        public int IDPrekrsaja { get; set; }
        [DataMember]
        public int IDOpisaPrekrsaja { get; set; }
        [DataMember]
        public int? IDMarke { get; set; }
        [DataMember]
        public int? IDStatusa { get; set; }
        [DataMember]
        public DateTime DatumPrekrsaja { get; set; }
        [DataMember]
        public DateTime DatumOdobravanja { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string BrojDokumenta { get; set; }
        [DataMember]
        public string OpisPrekrsaja { get; set; }
        [DataMember]
        public string ClanakPrekrsaja { get; set; }
        [DataMember]
        public string Kazna { get; set; }
        [DataMember]
        public string Marka { get; set; }
        [DataMember]
        public string Drzava { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public _Koordinate Pozicija { get; set; }
        [DataMember]
        public bool? Preuzeto { get; set; }
        [DataMember]
        public bool Zatvoren { get; set; }
        [DataMember]
        public string Ikona { get; set; }
        [DataMember]
        public bool Ponisteno { get; set; }
        [DataMember]
        public string Napomena { get; set; }
        [DataMember]
        public bool Dokument { get; set; }

        public _VppPostupanja(int idpr, int idp, int idop, int? idm, int? idstat, DateTime dp, DateTime dao, string reg, string adresa, string bd,
            string op, string cp, string k, string marka, string drz, string st, _Koordinate poz, bool? pre, bool zatvoren, string ikona, bool ponisteno, string napomena, bool dokument)
        {
            IDPreuzimanja = idpr;
            IDPrekrsaja = idp;
            IDOpisaPrekrsaja = idop;
            IDMarke = idm;
            IDStatusa = idstat;
            DatumPrekrsaja = dp;
            DatumOdobravanja = dao;
            Registracija = reg;
            Adresa = adresa;
            BrojDokumenta = bd;
            OpisPrekrsaja = op;
            ClanakPrekrsaja = cp;
            Kazna = k;
            Marka = marka;
            Drzava = drz;
            Status = st;
            Pozicija = poz;
            Preuzeto = pre;
            Zatvoren = zatvoren;
            Ikona = ikona;
            Ponisteno = ponisteno;
            Napomena = napomena;
            Dokument = dokument;
        }
    }

    [DataContract]
    public class _VppRegistracije
    {
        [DataMember]
        public int IDVppPostupanja { get; set; }
        [DataMember]
        public int IDPrekrsaja { get; set; }
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public string Drzava { get; set; }
        [DataMember]
        public bool Obraden { get; set; }

        public _VppRegistracije(int idvpp, int idp, int idl, string reg, string drzava)
        {
            IDVppPostupanja = idvpp;
            IDPrekrsaja = idp;
            IDLokacije = idl;
            Registracija = reg;
            Drzava = drzava;
            Obraden = false;
        }
    }

    [DataContract]
    public class _Drzava
    {
        [DataMember]
        public string Drzava { get; set; }
        [DataMember]
        public string Kratica { get; set; }

        [DataMember]
        public string ISOKratica { get; set; }
        
        public _Drzava(string drz, string kra, string iso)
        {
            Drzava = drz;
            Kratica = kra;
            ISOKratica = iso;
        }
    }

    [DataContract]
    public class _Uplata
    {
        [DataMember]
        public int IDUplate { get; set; }
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public string Grad { get; set; }
        [DataMember]
        public string Poziv { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public decimal Iznos { get; set; }
        [DataMember]
        public int Broj { get; set; }

        public _Uplata(int idUplate, int idGrada, string grad, string poziv, DateTime datum, decimal iznos, int brojPrekrsaja)
        {
            IDUplate = idUplate;
            IDGrada = idGrada;
            Grad = grad;
            Poziv = poziv;
            Datum = datum;
            Iznos = iznos;
            Broj = brojPrekrsaja;
        }
    }
}
