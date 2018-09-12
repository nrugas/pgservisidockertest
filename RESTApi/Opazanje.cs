using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PG.Servisi.RESTApi
{
    public class __Opazanje : _Opazanje
    {
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public int Total { get; set; }
        [DataMember]
        public string Adresa { get; set; }
        [DataMember]
        public List<string> Slike { get; set; }


        public __Opazanje(int idOpazanja, int? idLokacije, int? idSektora, int? idZone, int? idDjelatnika, int? idTerminala, int? idStatusa, int? idRacuna, string sektror, string djelatnik, string zona,
            string status, string registracija, string drzava, DateTime? vrijeme, DateTime? placenoDo, decimal? latitude, decimal? longitude, decimal? iznos, bool kaznjen, bool otisao, string brojRacuna, string opazanja) :
             base(idOpazanja, idLokacije, idSektora, idZone, idDjelatnika, idTerminala, idStatusa, idRacuna, sektror, djelatnik, zona,
             status, registracija, drzava, vrijeme, placenoDo, latitude, longitude, iznos, kaznjen, otisao, brojRacuna, opazanja)
        {

        }
    }
}