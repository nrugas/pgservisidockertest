using System.Collections.Generic;
using System.ServiceModel;

namespace PG.Servisi
{
    [ServiceContract]
    public interface IPGGO
    {
        [OperationContract]
        int DodajNovogDjelatnika(string grad, _Djelatnik korisni);

        /*:: PREDLOZAK ::*/

        [OperationContract]
        _Predlozak DohvatiPredlozak(string grad, int idPredloska);

        /*:: PREKRSAJ ::*/

        [OperationContract]
        _Prekrsaj Detalji(string grad, int idLokacije);

        [OperationContract]
        int DodajRucniPrekrsaj(string grad, _Prekrsaj prekrsaj, bool obavijest, List<byte[]> slike);

        [OperationContract]
        void DodajFotografiju(string grad, int idLokacije, byte[] s);

        [OperationContract]
        List<_Opis> DohvatiOpiseZakonaGOS(string grad);

        [OperationContract]
        int IznosKazneS(string grad, int idOpisa);

        [OperationContract]
        _Vozilo Vozilo(string grad, int idVozila);
    }
}
