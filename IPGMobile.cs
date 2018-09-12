using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Linq;

namespace PG.Servisi
{
    [ServiceContract]
    public interface IPGMobile
    {
        [OperationContract]
        void Aktivan(string grad, int idKorisnika, string terminalid, string hostname, string verzija, string os);

        [OperationContract]
        _Grad AktivacijaAplikacije(string aktivacijskiKod);

        [OperationContract]
        List<_Printer> Printeri(string grad, int idRedarstva);

        [OperationContract]
        List<_Drzava> Drzave();

        [OperationContract]
        List<_Zakon> DohvatiZakone(string grad, bool neaktivni);

        [OperationContract]
        _Operater Djelatnik(string grad, string uid, string lozinka);

        [OperationContract]
        bool IzmjeniLozinku(string grad, int idKorisnika, string lozinka);

        [OperationContract]
        bool SpremiLog(string grad, DateTime vrijeme, string naziv, string uid, string service, string info);

        [OperationContract]
        _2DLista Terminal(string grad, string ib);

        [OperationContract]
        void SpremiError(string grad, string greska, string napomena, string korisnik);

        [OperationContract]
        void SpremiAkciju(string grad, int idKorisnika, int idAkcije, string napomena);

        /*:: OPREMA ::*/

        [OperationContract]
        void DodajBiljesku(string id, int idStatusa, string napomena);

        //todo
        //[OperationContract]
        //_Oprema Terminal(string id);

        /*:: POSEBNA VOZILA ::*/

        [OperationContract]
        bool? PosebnoVozilo(string grad, string registracija);

        [OperationContract]
        string PostojiOdobrenje(string grad, string registracija);

        [OperationContract]
        _Odobrenja Odobrenje(string grad, string registracija, int idRedarstva);

        /*:: PREKRŠAJ ::*/

        #region PREKRSAJ

        [OperationContract]
        bool SpremiPrekrsajNovo(string grad, _Lokacija lokacija, ref _NoviPrekrsaj prekrsaj);

        [OperationContract]
        int SpremiLokaciju(string grad, _Lokacija lokacija, bool pauk);

        [OperationContract]
        bool SpremiFotografiju(string grad, int idLokacije, byte[] fotografija);

        [OperationContract]
        int SpremiPrekrsaj(string grad, _NoviPrekrsaj prekrsaj);

        [OperationContract]
        int PosaljiNalogPauku(string grad, int idPrekrsaja, DateTime datum, bool lisice);

        [OperationContract]
        _LokacijaPrekrsaja LokacijaPrekrsaja(string grad, int idLokacije);

        [OperationContract]
        bool? IskoristenaUplatnica(string grad, string poziv);

        [OperationContract]
        bool? IskoristenaUplatnicaRacun(string grad, string poziv);

        [OperationContract]
        bool NoviNalog(string grad, int idNaloga, DateTime vrijeme, bool lisice, int idLokacije);

        [OperationContract]
        _Prekrsaj GetPrekrsaj(string grad, int idLokacije);

        [OperationContract]
        _Prekrsaj GetPrekrsajBroj(string grad, string broj);

        #endregion

        /*:: ZAHTJEV ZA PODIZANJEM ::*/

        #region ZAHTJEV

        [OperationContract]
        int NoviZahtjev(string grad, _Zahtjev zahtjev, out bool aktivan);

        [OperationContract]
        bool StatusZahtjeva(string grad, int idZahtjeva, ref int idStatusa, out string poruka, out int? idNaloga, out decimal kazna, out bool obavijest);

        [OperationContract]
        _Operater PreuzeoZahtjev(string grad, int idZahtjeva, out string aplikacija);

        [OperationContract]
        bool AktivniKorisnik(string grad);

        [OperationContract]
        int IDVozila(string grad, int idTerminala, out string vozilo);

        #endregion

        /*:: STATUS KAŽNJAVANJA VOZILA ::*/

        [OperationContract]
        List<_Kaznjavan> Kaznjavan(string grad, string registracija, string drzava, int dana);

        /*:: PAUK ::*/

        #region PAUK

        [OperationContract]
        string NazivVozila(string grad, int idTerminala);

        [OperationContract]
        _Vozilo Vozilo(string grad, int idTerminala);

        [OperationContract]
        List<_2DLista> Razlozi(string grad, int idStatusa);

        [OperationContract]
        int SpremiLokacijuPauka(string grad, _PozicijaPauka pozicija);

        [OperationContract]
        List<_PozicijaDjelatnika> PozicijeRedara(string grad, int minuta);

        [OperationContract]
        int DohvatiIDNaloga(string grad, int idTerminala);

        [OperationContract]
        _NalogMobile DetaljiNaloga(string grad, int idNaloga);

        [OperationContract]
        List<_CentralnaLokacija> DohvatiCentralnuLokaciju(string grad, int idTerminala);

        [OperationContract]
        List<_NalogMobile> DohvatiNalogeDjeatnika(string grad, int idDjelatnika);

        [OperationContract]
        bool StornoRedara(string grad, int idNaloga);

        [OperationContract]
        int? DohvatiNoviBrojRacuna(string grad, int idNaloga, out decimal? iznos);

        [OperationContract]
        void IzmjeniStatus(string grad, int idNaloga, int idTerminala, int idStatusa, int? idRazloga, DateTime datum, decimal lat, decimal lng, string adresa, decimal brzina,
            int idStatusaLokacije, int idCentralneLokacije);

        [OperationContract]
        void IzmjeniStatusNaloga(string grad, int idNaloga, int idTerminala, int? idDjelatnika, int idStatusa, int? idRazloga, DateTime datum, decimal lat, decimal lng, string adresa, decimal brzina,
            int idStatusaLokacije, int idCentralneLokacije);

        [OperationContract]
        int NoviID(string grad);

        [OperationContract]
        int ProvijeriNaplativost(string grad, string brojObavijesti, out string poruka);

        [OperationContract]
        _Zakon DetaljiPrekrsaja(string grad, int idOpisa);

        //[OperationContract]
        //void DohvatiNoveNaloge(string grad);

        [OperationContract]
        bool NaplatiPauka(string grad, int idNaloga, int idTerminala, int brojRacuna, DateTime datum, decimal lat, decimal lng, string adresa, decimal brzina,
            int idStatusaLokacije, int idCentralneLokacije);

        #endregion

        /*:: ISPIS ::*/

        #region ISPIS

        [OperationContract]
        bool IspisPredloska(string grad, string detaljiKazne, int qty, int idPredloska, int idJezika, out string Ispis);

        [OperationContract]
        string IspisObavijestiPauk(string grad, int idLokacije, string broj);

        [OperationContract]
        string IspisKopijeRacuna(string grad, int idRacuna);

        [OperationContract]
        string IspisKopijeRacunaBroj(string grad, string broj);

        [OperationContract]
        string IzvjestajSmjene(string grad, int idDjelatnika);

        #endregion

        /*:: LOKACIJA ::*/

        [OperationContract]
        List<_DetaljiLokacije> DohvatiAdresu(double lat, double lng);

        [OperationContract]
        bool NaplatiPrekrsaj(string grad, int idPrekrsaja, int idRedara, int idVrstePlacanja, out string poruka);

        [OperationContract]
        bool DodajLokalnuAdresu(string grad, string adresa, string broj, decimal? hdop, decimal lat, decimal lng);

        /*:: AUTO PILOT ::*/

        #region Auto Pilot

        [OperationContract]
        bool PromijeniStatusVozila(string baza, int idTerminala, bool aktivan);

        [OperationContract]
        List<_NaloziPauku> IzdaniNalozi(string baza, DateTime datum, bool novi);

        [OperationContract]
        List<_StatusVozila> StatusVozila(string baza);

        [OperationContract]
        bool UkljuciAutoPilot(string baza, bool ukljucen);

        [OperationContract]
        bool MozePrimitiNalog(string baza, int idVozila, int idNaloga);

        [OperationContract]
        bool SetAutopilotID(string baza, string ID, bool force, out string autopilotID);

        [OperationContract]
        bool Autopilot(string baza);

        [OperationContract]
        void DodjeliPauku(string baza, int idNaloga, int idVozila);

        [OperationContract]
        bool PrivremenaObustava(string grad, int idVozila, bool obustavi);

        [OperationContract]
        DateTime? AutopilotUgasen(string grad);

        #endregion

        /*:: NAPLATA ::*/

        [OperationContract]
        List<_VrstaPlacanja> VrstePlacanja(string grad, int? idStatusa);

        [OperationContract]
        string Naplati(string grad, int idNaloga, int idStatusa, int idDjelatnika, int idVrstePlacanja, _Osoba osoba, decimal iznos, string poziv, out int idRacuna);

        [OperationContract]
        bool SpremiDokument(string grad, int idRacuna, byte[] dokument);
        
        /*:: OBRADA ZAHTJEVA ::*/

        #region OBRADA ZAHTJEVA

        [OperationContract]
        int Neobradjeni(string grad, int idDjelatnika, string uid, out _PrijavaPauk prijava);

        [OperationContract]
        List<_Slika> Slike(string grad, int idLokacije);

        [OperationContract]
        bool Preuzmi(string grad, int idZahtjeva, int idDjelatnika);

        [OperationContract]
        void Odustani(string grad, int idZahtjeva, int idDjelatnika);

        [OperationContract]
        int DodajPrekrsaj(string grad, _PrijavaPauk zahtjev, int idOpisa, string registracija,
            string adresa, string drzava, bool obavijest, bool nalogPauku);

        [OperationContract]
        bool StatusZahtjevaPaukOdustao(string grad, int idZahtjeva);

        [OperationContract]
        bool Odbij(string grad, int idZahtjeva, int idDjelatnika, string razlog);

        #endregion

        /*:: NEOČITANA REGISTRACIJA ::*/

        [OperationContract]
        bool NeocitanaRegistracija(string grad, _Neocitana registracija);

        [OperationContract]
        int DodanoLokacija(string grad, DateTime datumOd, DateTime datumDo);

        [OperationContract]
        int DodanoPrekrsaja(string grad, DateTime datumOd, DateTime datumDo);

        /*:: TERMINALI ::*/

        #region TERMINALI

        [OperationContract]
        _Terminal GetTerminala(string grad, string deviceId);

        [OperationContract]
        int InsertTerminal(string grad, string deviceId, string naziv);

        [OperationContract]
        bool ResetTerminals(string grad, string deviceId, int command);

        [OperationContract]
        string GetParametriTerminala(string grad, string deviceId, string naziv, bool startUp);

        [OperationContract]
        int? SetTerminalAccessTime(string grad, string deviceId);

        [OperationContract]
        bool ClearTerminalStatus(string grad, string deviceId);

        [OperationContract]
        void UpdateVerzija(string grad, string progVer, string romVer, string deviceId);//todo, bool pauk);

        #endregion

        /*:: PORUKE ::*/

        [OperationContract]
        _Poruka DohvatiPoruku(string grad, int idDjelatnika, out int neprocitanih);

        [OperationContract]
        bool ProcitaoPoruku(string grad, int idDjelatnika, int idPoruke);

        /*:: LISICE ::*/

        [OperationContract]
        List<_Prekrsaj> PretragaBlokiranih(string grad);

        [OperationContract]
        decimal IznosDeblokade(string grad, int idNaloga);

        [OperationContract]
        string DeblokirajVozilo(string grad, int idNaloga, int idDjelatnika, int idVrstePlacanja, _Osoba osoba, string poziv, out int idRacuna);

        /*:: MOBILE ::*/

        [OperationContract]
        bool NoviZahtjevM(string grad, decimal lat, decimal lng, string adresa, List<byte[]> slike);

        /*:: PARKING ::*/

        [OperationContract]
        bool SpremiOpazanje(string grad, ref _Opazanje opazanje, List<byte[]> slike);

        [OperationContract]
        List<_Opazanje> TraziOpazanje(string grad, string registracija, bool samoopazanja, int? idDjelatnika, int? idSektora);

        //[OperationContract]
        //string NaplatiParking(string grad, _Opazanje opazanje, int? tip, int idVrstePlacanja, int kolicina, out decimal iznos);

        [OperationContract]
        List<_Zone> Zone(string grad);

        [OperationContract]
        List<_Sektori> Sektori(string grad);
    }

    //[DataContract(Name = "TerminalCommand")]
    //public enum TerminalCommand
    //{
    //    [EnumMember]
    //    ResetTerminal = 0,
    //    [EnumMember]
    //    RestartProgram = 1,
    //    [EnumMember]
    //    ExitProgram = 2,
    //    [EnumMember]
    //    SuspendTerminal = 3
    //}

    [DataContract]
    public class _Lokacija
    {
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public int IDTerminala { get; set; }
        [DataMember]
        public int IDPozicioniranja { get; set; }
        [DataMember]
        public decimal Latitude { get; set; }
        [DataMember]
        public decimal Longitude { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }
        [DataMember]
        public int? CellTowerID { get; set; }
        [DataMember]
        public int? SignalStrength { get; set; }
        [DataMember]
        public float HDOP { get; set; }
        [DataMember]
        public float Brzina { get; set; }
        [DataMember]
        public float Preciznost { get; set; }
        [DataMember]
        public float Baterija { get; set; }
        [DataMember]
        public bool Punjac { get; set; }

        public _Lokacija(int idLokacije, int idDjelatnika, int idTerminala, int idPozicioniranja, decimal latitude, decimal longitude, string registracija, 
            DateTime datumVrijeme, int? cellTower, int? signal, float hdop, float brzina, float preciznost, float baterija, bool punjac)
        {
            IDLokacije = idLokacije;
            IDDjelatnika = idDjelatnika;
            IDTerminala = idTerminala;
            IDPozicioniranja = idPozicioniranja;
            Latitude = latitude;
            Longitude = longitude;
            Registracija = registracija;
            DatumVrijeme = datumVrijeme;
            CellTowerID = cellTower;
            SignalStrength = signal;
            HDOP = hdop;
            Brzina = brzina;
            Preciznost = preciznost;
            Baterija = baterija;
            Punjac = punjac;
        }
    }

    [DataContract]
    public class _NoviPrekrsaj
    {
        [DataMember]
        public int IDPrekrsaja { get; set; }
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public int IDOpisaPrekrsaja { get; set; }
        [DataMember]
        public int IDZakona { get; set; }
        [DataMember]
        public int IDPozicioniranja { get; set; }
        [DataMember]
        public int IDPredloska { get; set; }
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public DateTime Vrijeme { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public string BrojUpozorenja { get; set; }
        [DataMember]
        public decimal Latitude { get; set; }
        [DataMember]
        public decimal Longitude { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public decimal Kazna { get; set; }
        [DataMember]
        public string PozivNaBroj { get; set; }
        [DataMember]
        public bool Nalog { get; set; }
        [DataMember]
        public int IDNaloga { get; set; }
        [DataMember]
        public string Drzava { get; set; }
        [DataMember]
        public int StatusOcitanja { get; set; }
        [DataMember]
        public string Ocitanja { get; set; }
        [DataMember]
        public int Trajanje { get; set; }
        [DataMember]
        public bool ZakonskaSankcija { get; set; }

        public _NoviPrekrsaj(int idprekrsaja, int idlokacije, int idOpisaPrekrsaja, int idzakona, int idpozicioniranja, int idpredloska, int iddjelatnika, DateTime vrijeme,
            string registracija, string brojupozorenja, decimal latitude, decimal longitude, string adresa, decimal kazna, string pozivnabroj, bool nalog, int idnaloga, 
            string drzava, int statusocitanja, string ocitanja, int trajanje)
        {
            IDPrekrsaja = idprekrsaja;
            IDLokacije = idlokacije;
            IDOpisaPrekrsaja =  idOpisaPrekrsaja;
            IDZakona = idzakona;
            IDPozicioniranja = idpozicioniranja;
            IDPredloska = idpredloska;
            IDDjelatnika = iddjelatnika;
            Vrijeme = vrijeme;
            Registracija = registracija;
            BrojUpozorenja = brojupozorenja;
            Latitude = latitude;
            Longitude = longitude;
            Adresa = adresa;
            Kazna = kazna;
            PozivNaBroj = pozivnabroj;
            Nalog = nalog;
            IDNaloga = idnaloga;
            Drzava = drzava;
            StatusOcitanja = statusocitanja;
            Ocitanja = ocitanja;
            Trajanje = trajanje;
        }
    }

    [DataContract]
    public class _Kaznjavan
    {
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public string Vrsta { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }
        [DataMember]
        public decimal Kazna { get; set; }
        [DataMember]
        public decimal Lat { get; set; }
        [DataMember]
        public decimal Lng { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string Redar { get; set; }
        [DataMember]
        public string Opis { get; set; }
        [DataMember]
        public List<int> Slike { get; set; }

        public _Kaznjavan(int idLokacije, string vrsta, DateTime datumVrijeme, decimal kazna, decimal lat, decimal lng,
            string adresa, string redar, string opis, List<int> slike)
        {
            IDLokacije = idLokacije;
            Vrsta = vrsta;
            DatumVrijeme = datumVrijeme;
            Kazna = kazna;
            Lat = lat;
            Lng = lng;
            Adresa = adresa;
            Redar = redar;
            Opis = opis;
            Slike = slike;
        }
    }

    [DataContract]
    public class _Zahtjev
    {
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public int IDVozila { get; set; }
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public int? IDOpisa { get; set; }
        [DataMember]
        public DateTime Vrijeme { get; set; }
        [DataMember]
        public decimal Latitude { get; set; }
        [DataMember]
        public decimal Longitude { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string Drzava { get; set; }
        [DataMember]
        public byte? TipOcitanja { get; set; }
        [DataMember]
        public string Ocitanja { get; set; }
        [DataMember]
        public int Trajanje { get; set; }

        public _Zahtjev(int idlokacije, int idvozila, int idDjelatnika, int? idopisa, DateTime vrijeme, decimal lat, decimal lng, string registracija,
            string adresa, string drzava, byte? tipOcitanja, string ocitanja, int trajanje)
        {
            IDLokacije = idlokacije;
            IDVozila = idvozila;
            IDDjelatnika = idDjelatnika;
            IDOpisa = idopisa;
            Vrijeme = vrijeme;
            Latitude = lat;
            Longitude = lng;
            Registracija = registracija;
            Adresa = adresa;
            Drzava = drzava;
            TipOcitanja = tipOcitanja;
            Ocitanja = ocitanja;
            Trajanje = trajanje;
        }
    }

    [DataContract]
    public class _PozicijaPauka
    {
        [DataMember]
        public int IDLokacijePauka { get; set; }
        [DataMember]
        public int? IDVozila { get; set; }
        [DataMember]
        public int? IDNacinaPozicioniranja { get; set; }
        [DataMember]
        public int? IDTerminala { get; set; }
        [DataMember]
        public int? IDStatusaLokacije { get; set; }
        [DataMember]
        public int? IDCentralneLokacije { get; set; }
        [DataMember]
        public decimal LatPauka { get; set; }
        [DataMember]
        public decimal LngPauka { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }
        [DataMember]
        public float Brzina { get; set; }
        [DataMember]
        public float Preciznost { get; set; }
        [DataMember]
        public float Baterija { get; set; }

        public _PozicijaPauka(int idLokacije, int? idVozila, int? idNacinaPoz, int? idTerminala, int? idStatusaLok,
            int? idCentlok, decimal lat, decimal lng, DateTime datum, float brzina, float preciznost, float baterija)
        {
            IDLokacijePauka = idLokacije;
            IDVozila = idVozila;
            IDNacinaPozicioniranja = idNacinaPoz;
            IDTerminala = idTerminala;
            IDStatusaLokacije = idStatusaLok;
            IDCentralneLokacije = idCentlok;
            LatPauka = lat;
            LngPauka = lng;
            DatumVrijeme = datum;
            Brzina = brzina;
            Preciznost = preciznost;
            Baterija = baterija;
        }
    }

    [DataContract]
    public class _DetaljiLokacije
    {
        [DataMember]
        public string Ulica { get; set; }
        [DataMember]
        public string Broj { get; set; }
        [DataMember]
        public string NameCode { get; set; }
        [DataMember]
        public string Lokalitet { get; set; }
        [DataMember]
        public string Zupanija { get; set; }
        [DataMember]
        public int Accuracy { get; set; }

        public _DetaljiLokacije(string ul, string br, string lok, string zup, string nc, int acc)
        {
            Ulica = ul;
            Broj = br;
            NameCode = nc;
            Lokalitet = lok;
            Zupanija = zup;
            Accuracy = acc;
        }
    }

    [DataContract]
    public class _CentralnaLokacija
    {
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public decimal Latitude { get; set; }
        [DataMember]
        public decimal Longitude { get; set; }

        public _CentralnaLokacija(int id, decimal lat, decimal lon)
        {
            IDLokacije = id;
            Latitude = lat;
            Longitude = lon;
        }
    }

    [DataContract]
    public class _NaloziPauku
    {
        [DataMember]
        public int IDNaloga { get; set; }
        [DataMember]
        public int IDStatusa { get; set; }
        [DataMember]
        public string NazivStatusa { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }

        public _NaloziPauku(int id, int ids, string ns, DateTime dv)
        {
            IDNaloga = id;
            IDStatusa = ids;
            NazivStatusa = ns;
            DatumVrijeme = dv;
        }
    }

    //todo obrisi
    //[DataContract]
    //public class _NalogZdenko
    //{
    //    [DataMember]
    //    public int idNaloga { get; set; }
    //    [DataMember]
    //    public string Registracija { get; set; }
    //    [DataMember]
    //    public string BrojObavijesti { get; set; }
    //    [DataMember]
    //    public string IzdanOd { get; set; }
    //    [DataMember]
    //    public string idDjelatnika { get; set; }
    //    [DataMember]
    //    public string ImePrezime { get; set; }
    //    [DataMember]
    //    public string Adresa { get; set; }
    //    [DataMember]
    //    public DateTime? DatumVrijemeNaloga { get; set; }
    //    [DataMember]
    //    public string OpisPrekrsaja { get; set; }
    //    [DataMember]
    //    public string MaterijalnaKaznjivaNorma { get; set; }
    //    [DataMember]
    //    public int? PaukBr { get; set; }
    //    [DataMember]
    //    public DateTime? PreuzeoPauk { get; set; }
    //    [DataMember]
    //    public int? DizanjePokusaj { get; set; }
    //    [DataMember]
    //    public string DizanjePokusajNapomena { get; set; }
    //    [DataMember]
    //    public DateTime? DizanjePokusajDatumVrijeme { get; set; }
    //    [DataMember]
    //    public DateTime? Deponij { get; set; }

    //    public _NalogZdenko(int idn, string reg, string bn, string io, string idd, string ip, string a, DateTime? dvn,
    //        string op, string mkn, int? pbr, DateTime? pp, int? dp, string dpn, DateTime? dpdv, DateTime? dep)
    //    {
    //        idNaloga = idn;
    //        Registracija = reg;
    //        BrojObavijesti = bn;
    //        IzdanOd = io;
    //        idDjelatnika = idd;
    //        ImePrezime = ip;
    //        Adresa = a;
    //        DatumVrijemeNaloga = dvn;
    //        OpisPrekrsaja = op;
    //        MaterijalnaKaznjivaNorma = mkn;
    //        PaukBr = pbr;
    //        PreuzeoPauk = pp;
    //        DizanjePokusaj = dp;
    //        DizanjePokusajNapomena = dpn;
    //        DizanjePokusajDatumVrijeme = dpdv;
    //        Deponij = dep;
    //    }
    //}

    [DataContract]
    public class _Parking
    {
        [DataMember]
        public int IDParkinga { get; set; }
        [DataMember]
        public string Naziv { get; set; }
        [DataMember]
        public string Zona { get; set; }
        [DataMember]
        public decimal Cijena { get; set; }

        public _Parking(int idparkinga, string naziv, string zona, decimal cijena)
        {
            IDParkinga = idparkinga;
            Naziv = naziv;
            Zona = zona;
            Cijena = cijena;
        }
    }

    [DataContract]
    public class _StatusVozila
    {
        [DataMember]
        public int IDVozila { get; set; }
        [DataMember]
        public int? IDTerminala { get; set; }
        [DataMember]
        public string NazivVozila { get; set; }
        [DataMember]
        public bool Aktivan { get; set; }
        [DataMember]
        public decimal? Latitude { get; set; }
        [DataMember]
        public decimal? Longitude { get; set; }
        [DataMember]
        public int DodjeljenihNaloga { get; set; }
        [DataMember]
        public bool PrivremenoObustavljeno { get; set; }
        [DataMember]
        public bool StavljaLisice { get; set; }
        [DataMember]
        public DateTime? VrijemeZadnjeAktivnosti { get; set; }

        public _StatusVozila(int idv, int? idt, string nv, bool a, decimal? lat, decimal? lng, int br, bool sta, bool sl, DateTime? vza)
        {
            IDVozila = idv;
            IDTerminala = idt;
            NazivVozila = nv;
            Aktivan = a;
            Latitude = lat;
            Longitude = lng;
            DodjeljenihNaloga = br;
            PrivremenoObustavljeno = sta;
            StavljaLisice = sl;
            VrijemeZadnjeAktivnosti = vza;
        }
    }

    [DataContract]
    public class _Operater
    {
        [DataMember]
        public int IDRedara { get; set; }
        [DataMember]
        public int? IDRedarstva { get; set; }
        [DataMember]
        public int IDPrijaviteljaGO { get; set; }
        [DataMember]
        public string ImePrezime { get; set; }
        [DataMember]
        public string UID { get; set; }
        [DataMember]
        public string BrojIskaznice { get; set; }
        [DataMember]
        public int IDPrivilegije { get; set; }
        [DataMember]
        public XElement Parametri { get; set; }
        [DataMember]
        public bool Blokiran { get; set; }
        [DataMember]
        public bool Naplata { get; set; }
        [DataMember]
        public bool TraziOdobrenje { get; set; }
        [DataMember]
        public bool ObradujeZahtjeve { get; set; }
        [DataMember]
        public string GOGradID { get; set; }

        public _Operater(int idRedara, int? idRedarstva, int idPrijaviteljaGo, string ime, string uid, string bi, int idPrivilegije, XElement parametri, bool blokiran, bool naplata, bool traziodobrenje, bool obradjuje, string goGradID)
        {
            IDRedara = idRedara;
            IDRedarstva = idRedarstva;
            IDPrijaviteljaGO = idPrijaviteljaGo;
            ImePrezime = ime;
            UID = uid;
            BrojIskaznice = bi;
            IDPrivilegije = idPrivilegije;
            Parametri = parametri;
            Blokiran = blokiran;
            Naplata = naplata;
            TraziOdobrenje = traziodobrenje;
            ObradujeZahtjeve = obradjuje;
            GOGradID = goGradID;
        }
    }

    [DataContract]
    public class _LokacijaPrekrsaja
    {
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public int IDPrekrsaja { get; set; }
        [DataMember]
        public int? IDNaloga { get; set; }
        [DataMember]
        public int? IDNacinaPozicioniranja { get; set; }
        [DataMember]
        public int? IDDjelatnika { get; set; }
        [DataMember]
        public int? IDTerminla { get; set; }
        [DataMember]
        public decimal LatPrekrsaja { get; set; }
        [DataMember]
        public decimal LngPrekrsaja { get; set; }
        [DataMember]
        public decimal LatLokacije { get; set; }
        [DataMember]
        public decimal LngLokacije { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string Terminal { get; set; }
        [DataMember]
        public string ImePrezime { get; set; }
        [DataMember]
        public string UID { get; set; }
        [DataMember]
        public double? Brzina { get; set; }

        public _LokacijaPrekrsaja(int idLokacije, int idPrekrsaja, int? idNaloga, int? idNacinaPozicioniranja, int? idDjelatnika, int? idTerminala,
            decimal lat, decimal lng, decimal latl, decimal lngl, DateTime datumVrijeme, string adresa, string nazivTerminala, string imePrezime,
            string uid, double? brzina)
        {
            IDLokacije = idLokacije;
            IDPrekrsaja = idPrekrsaja;
            IDNaloga = idNaloga;
            IDNacinaPozicioniranja = idNacinaPozicioniranja;
            IDDjelatnika = idDjelatnika;
            IDTerminla = idTerminala;
            LatPrekrsaja = lat;
            LngPrekrsaja = lng;
            LatLokacije = latl;
            LngLokacije = lngl;
            Datum = datumVrijeme;
            Adresa = adresa;
            Terminal = nazivTerminala;
            ImePrezime = imePrezime;
            UID = uid;
            Brzina = brzina;
        }
    }

    [DataContract]
    public class _VrstaPlacanja
    {
        [DataMember]
        public int IDVrstePlacanja { get; set; }
        [DataMember]
        public string NazivVrstePlacanja { get; set; }
        [DataMember]
        public bool Uplatnica { get; set; }
        [DataMember]
        public bool Ispis { get; set; }
        [DataMember]
        public int Uplatitelj { get; set; }
        [DataMember]
        public decimal Iznos { get; set; }
        [DataMember]
        public string NaRacunu { get; set; }
        [DataMember]
        public string Kratica { get; set; }

        public _VrstaPlacanja(int idvrsteplacanja, string nazivvrsteplacanja, bool uplatnica, bool ispis, int uplatitelj, decimal iznos, 
            string naracunu, string kratica)
        {
            IDVrstePlacanja = idvrsteplacanja;
            NazivVrstePlacanja = nazivvrsteplacanja;
            Uplatnica = uplatnica;
            Ispis = ispis;
            Uplatitelj = uplatitelj;
            Iznos = iznos;
            NaRacunu = naracunu;
            Kratica = kratica;
        }
    }

    [DataContract]
    public class _VrstaPlacanjaStatus
    {
        [DataMember]
        public int IDVrstePlacanja { get; set; }
        [DataMember]
        public string NazivVrstePlacanja { get; set; }
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public int IDStatusa { get; set; }
        [DataMember]
        public decimal Iznos { get; set; }

        public _VrstaPlacanjaStatus(int idvrsteplacanja, string nazivvrsteplacanja, int idGrada, int idStatusa, decimal iznos)
        {
            IDVrstePlacanja = idvrsteplacanja;
            NazivVrstePlacanja = nazivvrsteplacanja;
            IDGrada = idGrada;
            IDStatusa = idStatusa;
            Iznos = iznos;
        }
    }

    [DataContract]
    public class _Neocitana
    {
        [DataMember]
        public byte[] Slika { get; set; }
        [DataMember]
        public DateTime? Datum { get; set; }
        [DataMember]
        public int? TrajanjePrijenosa { get; set; }
        [DataMember]
        public int? Duljina { get; set; }
        [DataMember]
        public int? TrajanjeOcitanja { get; set; }
        [DataMember]
        public string Ocitanja { get; set; }
        [DataMember]
        public int? Status { get; set; }
        [DataMember]
        public int? IDDjelatnika { get; set; }
        [DataMember]
        public XElement XMLData { get; set; }

        public _Neocitana(byte[] slika, DateTime? datum, int? prijenos, int? duljina, int? ocitanje, string ocitanja, int? status, int? iddjelatnika, XElement xml)
        {
            Slika = slika;
            Datum = datum;
            TrajanjePrijenosa = prijenos;
            Duljina = duljina;
            TrajanjeOcitanja = ocitanje;
            Ocitanja = ocitanja;
            Status = status;
            IDDjelatnika = iddjelatnika;
            XMLData = xml;
        }
    }

    /*:: PARKING ::*/

    [DataContract]
    public class _DnevnaKarta
    {
        [DataMember]
        public int IDKarte { get; set; }
        [DataMember]
        public int IDOpazanja { get; set; }
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public int IDLokacije { get; set; }
        [DataMember]
        public DateTime Vrijeme { get; set; }
        [DataMember]
        public decimal Kazna { get; set; }

        public _DnevnaKarta(int idKarte, int idOpazanja, int idDjelatnika, int idLokacije, DateTime vrijeme,
            decimal kazna)
        {
            IDKarte = idKarte;
            IDOpazanja = idOpazanja;
            IDDjelatnika = idDjelatnika;
            IDLokacije = idLokacije;
            Vrijeme = vrijeme;
            Kazna = kazna;
        }
    }

    [DataContract]
    public class _Opazanje
    {
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public int Total { get; set; }
        [DataMember]
        public int IDOpazanja { get; set; }
        [DataMember]
        public int? IDLokacije { get; set; }
        [DataMember]
        public int? IDSektora { get; set; }
        [DataMember]
        public int? IDZone { get; set; }
        [DataMember]
        public int? IDDjelatnika { get; set; }
        [DataMember]
        public int? IDTerminala { get; set; }
        [DataMember]
        public int? IDStatusa { get; set; }
        [DataMember]
        public string Sektor { get; set; }
        [DataMember]
        public string Djelatnik { get; set; }
        [DataMember]
        public string Zona { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public string Drzava { get; set; }
        [DataMember]
        public DateTime? Vrijeme { get; set; }
        [DataMember]
        public DateTime? PlacenoDo { get; set; }
        [DataMember]
        public decimal? Latitude { get; set; }
        [DataMember]
        public decimal? Longitude { get; set; }
        [DataMember]
        public decimal? Iznos { get; set; }
        [DataMember]
        public bool Kaznjen { get; set; }
        [DataMember]
        public bool Otisao { get; set; }
        [DataMember]
        public int? IDRacuna { get; set; }
        [DataMember]
        public string BrojRacuna { get; set; }
        [DataMember]
        public string Opazanja { get; set; }
        [DataMember]
        public string[] Slike { get; set; }

        public _Opazanje(int idOpazanja, int? idLokacije, int? idSektora, int? idZone, int? idDjelatnika, int? idTerminala, int? idStatusa, int? idRacuna, string sektror, string djelatnik, string zona, 
            string status, string registracija, string drzava, DateTime? vrijeme, DateTime? placenoDo, decimal? latitude, decimal? longitude, decimal? iznos, bool kaznjen, bool otisao, string brojRacuna, string opazanja)
        {
            IDOpazanja = idOpazanja;
            IDLokacije = idLokacije;
            IDSektora = idSektora;
            IDZone = idZone;
            IDDjelatnika = idDjelatnika;
            IDTerminala = idTerminala;
            IDStatusa = idStatusa;
            IDRacuna = idRacuna;
            Sektor = sektror;
            Djelatnik = djelatnik;
            Zona = zona;
            Status = status;
            Registracija = registracija;
            Drzava = drzava;
            Vrijeme = vrijeme;
            PlacenoDo = placenoDo;
            Latitude = latitude;
            Longitude = longitude;
            Iznos = iznos;
            Kaznjen = kaznjen;
            Otisao = otisao;
            BrojRacuna = brojRacuna;
            Opazanja = opazanja;
        }
    }

    /*:: OLD ::*/

    [DataContract]
    public class _PozicijaDjelatnika
    {
        [DataMember]
        public int IDDjelatnika { get; set; }
        [DataMember]
        public string ImePrezime { get; set; }
        [DataMember]
        public double Lat { get; set; }
        [DataMember]
        public double Lon { get; set; }
        [DataMember]
        public string NazivTerminala { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }
        [DataMember]
        public int Obavijest { get; set; }
        [DataMember]
        public int Upozorenje { get; set; }
        [DataMember]
        public int NalogaPauku { get; set; }
        [DataMember]
        public string Preciznost { get; set; }
        [DataMember]
        public string Baterija { get; set; }
        [DataMember]
        public string Kontakt { get; set; }
        [DataMember]
        public bool Aktivan { get; set; }

        public _PozicijaDjelatnika(int id, string ip, double la, double lo, string nt, DateTime dv, int ob,
            int up, int np, string prec, string bat, string kon, bool akt)
        {
            IDDjelatnika = id;
            ImePrezime = ip;
            Lat = la;
            Lon = lo;
            NazivTerminala = nt;
            DatumVrijeme = dv;
            Obavijest = ob;
            Upozorenje = up;
            NalogaPauku = np;
            Preciznost = prec;
            Baterija = bat;
            Kontakt = kon;
            Aktivan = akt;
        }
    }

    [DataContract]
    public class _NalogMobile
    {
        [DataMember]
        public int IDPrekrsaja { get; set; }
        [DataMember]
        public int IDNaloga { get; set; }
        [DataMember]
        public int? IDVozila { get; set; }
        [DataMember]
        public int? IDTerminala { get; set; }
        [DataMember]
        public int? IDStatusa { get; set; }
        [DataMember]
        public string Terminal { get; set; }
        [DataMember]
        public string Vozilo { get; set; }
        [DataMember]
        public string VoziloRegistracija { get; set; }
        [DataMember]
        public DateTime DatumVrijeme { get; set; }
        [DataMember]
        public string Registracija { get; set; }
        [DataMember]
        public string Redar { get; set; }
        [DataMember]
        public string UID { get; set; }
        [DataMember]
        public string BrojIskaznice { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public string BrojDokumenta { get; set; }
        [DataMember]
        public string OpisPrekrsaja { get; set; }
        [DataMember]
        public string ClanakPauka { get; set; }
        [DataMember]
        public string Clanak { get; set; }
        [DataMember]
        public string StatusPauka { get; set; }
        [DataMember]
        public string Grupa { get; set; }
        [DataMember]
        public _Koordinate Pozicija { get; set; }
        [DataMember]
        public string Boja { get; set; }
        [DataMember]
        public bool Zatvoren { get; set; }
        [DataMember]
        public bool Storniran { get; set; }
        [DataMember]
        public bool Redosljed { get; set; }
        [DataMember]
        public bool Lisice { get; set; }
        [DataMember]
        public int Prioritet { get; set; }

        public _NalogMobile(int idp, int idn, int? idv, int? idt, int idst, string ter, string voz, string vozReg, DateTime dv, string reg,
            string red, string uid, string bi, string adr, string bd, string op, string cp, string c, string sp, string gr, _Koordinate koo,
            string boj, bool zat, bool sto, bool re, bool lisice, int prio)
        {
            IDPrekrsaja = idp;
            IDNaloga = idn;
            IDVozila = idv;
            IDTerminala = idt;
            IDStatusa = idst;
            Terminal = ter;
            Vozilo = voz;
            VoziloRegistracija = vozReg;
            DatumVrijeme = dv;
            Registracija = reg;
            Redar = red;
            UID = uid;
            BrojIskaznice = bi;
            Adresa = adr;
            BrojDokumenta = bd;
            OpisPrekrsaja = op;
            ClanakPauka = cp;
            Clanak = c;
            StatusPauka = sp;
            Grupa = gr;
            Pozicija = koo;
            Boja = boj;
            Zatvoren = zat;
            Storniran = sto;
            Redosljed = re;
            Lisice = lisice;
            Prioritet = prio;
        }
    }
}
