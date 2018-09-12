using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace PG.Servisi
{
    [ServiceContract]
    public interface IPGOprema
    {
        [OperationContract]
        bool Dostupan();

        [OperationContract]
        List<_Stanje> Stanje();

        [OperationContract]
        void SpremiError(string grad, Exception greska, string napomena, string korisnik);

        [OperationContract]
        List<_2DLista> Redarstva();

        [OperationContract]
        List<_2DLista> PopisGradova();

        [OperationContract]
        List<_Printer> DohvatiPrintere();

        /**/

        [OperationContract]
        List<_2DLista> VrstaOpreme();

        //model
        [OperationContract]
        int DodajModel(string naziv, int idVrste);

        [OperationContract]
        bool IzmjeniModel(int idModela, string naziv, int idVrste);

        [OperationContract]
        bool? ObrisiModel(int idModela);

        [OperationContract]
        List<_Model> Modeli(int? idVrste);

        //os
        [OperationContract]
        int DodajOS(string naziv);

        [OperationContract]
        bool IzmjeniOS(int idOs, string naziv);

        [OperationContract]
        bool? ObrisiOs(int idOs);

        [OperationContract]
        List<_2DLista> OS();

        //tarifa
        [OperationContract]
        int DodajTarifu(string naziv, string opis);

        [OperationContract]
        bool IzmjeniTarifu(int idTarife, string naziv, string opis);

        [OperationContract]
        bool? ObrisiTarifu(int idTarife);

        [OperationContract]
        List<_3DLista> Tarife();

        //memorija 
        [OperationContract]
        List<_2DLista> Memorija();

        /**/

        [OperationContract]
        List<_Oprema> PretraziOpremu(int idVrste, string broj, bool? tip);

        /*:: IZMJENA ::*/

        [OperationContract]
        bool Izmjeni(_Oprema oprema);

        [OperationContract]
        bool IzmjeniInteni(int idVrsteOpreme, int idOpreme, int interniBroj);

        /*:: POVIJEST ::*/

        [OperationContract]
        void PovijestOpreme(int idOpreme, int idVrste, int idStatusa, string napomena, DateTime datum);

        [OperationContract]
        bool ObrisiPovijestOpreme(int idPovijesti, bool servis);

        [OperationContract]
        List<_PovijestOpreme> DohvatiPovijestOpreme(int idOpreme, int idVrste, int idStatusa);

        [OperationContract]
        bool Servis(_Servis servis);

        [OperationContract]
        List<_2DLista> StatusiPovijesti();

        [OperationContract]
        int NoviStatus(string status);

        /*:: DODAJ ::*/

        [OperationContract]
        int InterniBroj();

        [OperationContract]
        int Dodaj(_Oprema oprema);

        /*:: PRILOG ::*/

        [OperationContract]
        bool DodajPrilog(List<_Prilog> prilog);

        [OperationContract]
        bool ObrisiPrilog(int idPriloga);

        [OperationContract]
        List<_Prilog> Prilozi(int idVrste, int idOpreme);

        [OperationContract]
        _Prilog Prilog(int idPriloga);
    }

    [DataContract]
    public class _Oprema
    {
        [DataMember]
        public int IDOpreme { get; set; }
        [DataMember]
        public int IDVrsteOpreme { get; set; }
        [DataMember]
        public int IDStavkeReversa { get; set; }
        [DataMember]
        public string VrstaOpreme { get; set; }
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public string NazivGrada { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public string NazivRedarstva { get; set; }
        [DataMember]
        public int? IDModela { get; set; }
        [DataMember]
        public string NazivModela { get; set; }
        [DataMember]
        public int IDOsa { get; set; }
        [DataMember]
        public string OS { get; set; }
        [DataMember]
        public int IDMemorije { get; set; }
        [DataMember]
        public string Memorija { get; set; }
        [DataMember]
        public string PinFirmware { get; set; }
        [DataMember]
        public string MacIDB { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public int? InterniBroj { get; set; }
        [DataMember]
        public string SerijskiBroj { get; set; }
        [DataMember]
        public DateTime? DatumUlaska { get; set; }
        [DataMember]
        public int? Jamstvo { get; set; }
        [DataMember]
        public DateTime? DatumJamstva { get; set; }
        [DataMember]
        public bool? Vlasnik { get; set; }
        [DataMember]
        public string NazivVlasnika { get; set; }
        [DataMember]
        public bool? Ispravan { get; set; }
        [DataMember]
        public string Broj { get; set; }
        [DataMember]
        public string VPN { get; set; }

        public _Oprema(int idOpreme, int idVrste, int idStavke, string vrsta, int idGrada, string nazivGrada, int idRedarstva, string nazivRedarstva, int? idModela, string nazivModela, int idOsa,
            string os, int idMemorije, string memorija, string pin, string mac, string naziv, int? interniBroj, string serijski, DateTime? ulazak, int? jamstvo, DateTime? datumJamstva, bool? vlasnik, string nazivVlasnika,
            bool? ispravan, string broj, string vpn)
        {
            IDOpreme = idOpreme;
            IDVrsteOpreme = idVrste;
            IDStavkeReversa = idStavke;
            VrstaOpreme = vrsta;
            IDGrada = idGrada;
            NazivGrada = nazivGrada;
            IDRedarstva = idRedarstva;
            NazivRedarstva = nazivRedarstva;
            IDModela = idModela;
            NazivModela = nazivModela;
            IDOsa = idOsa;
            OS = os;
            IDMemorije = idMemorije;
            Memorija = memorija;
            PinFirmware = pin;
            MacIDB = mac;
            Naziv = naziv;
            InterniBroj = interniBroj;
            SerijskiBroj = serijski;
            DatumUlaska = ulazak;
            Jamstvo = jamstvo;
            DatumJamstva = datumJamstva;
            Vlasnik = vlasnik;
            NazivVlasnika = nazivVlasnika;
            Ispravan = ispravan;
            Broj = broj;
            VPN = vpn;
        }
    }

    [DataContract]
    public class _Servis
    {
        [DataMember]
        public int IDServisa { get; set; }
        [DataMember]
        public int IDVrste { get; set; }
        [DataMember]
        public int IDOpreme { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public string OpisProblema { get; set; }
        [DataMember]
        public bool Vracen { get; set; }
        [DataMember]
        public string Napomena { get; set; }
        [DataMember]
        public DateTime DatumPovratka { get; set; }

        public _Servis(int idservisa, int idvrste, int idopreme, DateTime datum, string opis, bool vracen,
            string napomena, DateTime povratak)
        {
            IDServisa = idservisa;
            IDVrste = idvrste;
            IDOpreme = idopreme;
            Datum = datum;
            OpisProblema = opis;
            Vracen = vracen;
            Napomena = napomena;
            DatumPovratka = povratak;
        }
    }

    [DataContract]
    public class _Model
    {
        [DataMember]
        public int IDModela { get; set; }
        [DataMember]
        public int IDVrste { get; set; }
        [DataMember]
        public string Naziv { get; set; }

        public _Model(int idModela, int idVrste, string naziv)
        {
            IDModela = idModela;
            IDVrste = idVrste;
            Naziv = naziv;
        }
    }

    [DataContract]
    public class _Stanje
    {
        [DataMember]
        public int IDVrste { get; set; }
        [DataMember]
        public string NazivVrste { get; set; }
        [DataMember]
        public int Ispravni { get; set; }
        [DataMember]
        public int Neispravni { get; set; }
        [DataMember]
        public int Servis { get; set; }

        public _Stanje(int idVrste, string naziv, int ispravni, int neispravni, int servis)
        {
            IDVrste = idVrste;
            NazivVrste = naziv;
            Ispravni = ispravni;
            Neispravni = neispravni;
            Servis = servis;
        }
    }

    [DataContract]
    public class _Prilog
    {
        [DataMember]
        public int IDPriloga { get; set; }
        [DataMember]
        public int IDVrsteOpreme { get; set; }
        [DataMember]
        public int IDOpreme { get; set; }
        [DataMember]
        public DateTime Vrijeme { get; set; }
        [DataMember]
        public string Opis { get; set; }
        [DataMember]
        public byte[] Prilog { get; set; }
        [DataMember]
        public string Ekstenzija { get; set; }

        public _Prilog(int idPriloga, int idVrste, int idOpreme, DateTime vrijeme, string opis, byte[] prilog, string ekstenzija)
        {
            IDPriloga = idPriloga;
            IDVrsteOpreme = idVrste;
            IDOpreme = idOpreme;
            Vrijeme = vrijeme;
            Opis = opis;
            Prilog = prilog;
            Ekstenzija = ekstenzija;
        }
    }



    //[DataContract]
    //public class _Stanje
    //{
    //    [DataMember]
    //    public int Terminal { get; set; }
    //    [DataMember]
    //    public int TerminalNeispravan { get; set; }
    //    [DataMember]
    //    public int Printer { get; set; }
    //    [DataMember]
    //    public int PrinterNeispravan { get; set; }
    //    [DataMember]
    //    public int SIM { get; set; }
    //    [DataMember]
    //    public int SIMNeispravan { get; set; }
    //    [DataMember]
    //    public int BatTerminal { get; set; }
    //    [DataMember]
    //    public int BatTerminalNeispravan { get; set; }
    //    [DataMember]
    //    public int BatPrinter { get; set; }
    //    [DataMember]
    //    public int BatPrinterNeispravan { get; set; }
    //    [DataMember]
    //    public int AdaTerminal { get; set; }
    //    [DataMember]
    //    public int AdaTerminalNeispravan { get; set; }
    //    [DataMember]
    //    public int AdaPrinter { get; set; }
    //    [DataMember]
    //    public int AdaPrinterNeispravan { get; set; }
    //    [DataMember]
    //    public int Cradle { get; set; }
    //    [DataMember]
    //    public int CradleNeispravan { get; set; }

    //    public _Stanje(int ter, int terNe, int prin, int prinNe, int sim, int simNe, int bt, int btNe, int bp, int bpNe, int at, int atNe, int ap, int apNe,
    //        int cra, int craNe)
    //    {
    //        Terminal = ter;
    //        TerminalNeispravan = terNe;
    //        Printer = prin;
    //        PrinterNeispravan = prinNe;
    //        SIM = sim;
    //        SIMNeispravan = simNe;
    //        BatTerminal = bt;
    //        BatTerminalNeispravan = btNe;
    //        BatPrinter = bp;
    //        BatPrinterNeispravan = bpNe;
    //        AdaTerminal = at;
    //        AdaTerminalNeispravan = atNe;
    //        AdaPrinter = ap;
    //        AdaPrinterNeispravan = apNe;
    //        Cradle = cra;
    //        CradleNeispravan = craNe;
    //    }
    //}

}
