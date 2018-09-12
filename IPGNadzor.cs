using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace PG.Servisi
{
    [ServiceContract]
    public interface IPGNadzor
    {
        [OperationContract]
        bool Dostupan();

        [OperationContract]
        void SpremiError(string grad, Exception greska, string napomena, string korisnik);

        [OperationContract]
        List<_2DLista> PopisGradova();

        [OperationContract]
        List<_2DLista> Redarstva();

        [OperationContract]
        List<_Iznos> Iznosi();

        [OperationContract]
        _Uplatnica Uplatnica(string grad, int idGrada, int idRedarstva);

        [OperationContract]
        decimal IznosNaUplatnici(int idGrada);

        [OperationContract]
        List<_Iznos> IznosPokusaja(int idGrada);

        [OperationContract]
        List<_Projekt> DohvatiProjekte();

        /*:: POVIJEST ::*/

        [OperationContract]
        int ZadnjiNalog(int idGrada, int? idIznosa, int idRedarstva);

        [OperationContract]
        List<_Povijest> PosvijestIspisaNaloga(int idGrada);

        [OperationContract]
        bool SpremiPovijestIspisa(_Povijest povijest);

        [OperationContract]
        bool? ProvijeriGodinu();
    }

    [DataContract]
    public class _Projekt
    {
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public string NazivGrada { get; set; }
        [DataMember]
        public decimal Latitude { get; set; }
        [DataMember]
        public decimal Longitude { get; set; }
        [DataMember]
        public string Baza { get; set; }
        [DataMember]
        public string Projekt { get; set; }
        [DataMember]
        public bool? Status { get; set; }
        [DataMember]
        public string Postupanja { get; set; }
        [DataMember]
        public DateTime? Zadnje { get; set; }

        public _Projekt(int idgrada, string nazivgrada, decimal latitude, decimal longitude, string baza, string projekt, bool? status, string postupanja,
            DateTime? zadnje)
        {
            IDGrada = idgrada;
            NazivGrada = nazivgrada;
            Latitude = latitude;
            Longitude = longitude;
            Baza = baza;
            Projekt = projekt;
            Status = status;
            Postupanja = postupanja;
            Zadnje = zadnje;
        }
    }

    [DataContract]
    public class _Iznos
    {
        [DataMember]
        public int IDIznosa { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public decimal Iznos { get; set; }

        public _Iznos(int idiznosa, int idredarstva, decimal iznos)
        {
            IDIznosa = idiznosa;
            IDRedarstva = idredarstva;
            Iznos = iznos;
        }
    }

    [DataContract]
    public class _Povijest
    {
        [DataMember]
        public int IDPovijesti { get; set; }
        [DataMember]
        public int IDGrada { get; set; }
        [DataMember]
        public string Grad { get; set; }
        [DataMember]
        public int IDRedarstva { get; set; }
        [DataMember]
        public string Redarstvo { get; set; }
        [DataMember]
        public int? IDIznosa { get; set; }
        [DataMember]
        public decimal? Iznos { get; set; }
        [DataMember]
        public int Stranica { get; set; }
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public int? Nalog { get; set; }
        [DataMember]
        public decimal Ispis { get; set; }

        public _Povijest(int idPovijesti, int idGrada, string grad, int idRedarstva, string redarstvo, int? idIznosa, decimal? iznos, int stranica, DateTime datum, 
            int? nalog, decimal ispis)
        {
            IDPovijesti = idPovijesti;
            IDGrada = idGrada;
            Grad = grad;
            IDRedarstva = idRedarstva;
            Redarstvo = redarstvo;
            IDIznosa = idIznosa;
            Iznos = iznos;
            Stranica = stranica;
            Datum = datum;
            Nalog = nalog;
            Ispis = ispis;
        }
    }
}
