using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text.RegularExpressions;

namespace PG.Servisi
{
    [ServiceContract]
    public interface IPGStatistika
    {

        [OperationContract]
        void Sime();

        [OperationContract]
        string IspisKopijeRacunaParking();

        [OperationContract]
        string ProvjeraPoziva(string poziv);

        [OperationContract]
        string Kazni(string poziv);

        [OperationContract]
        string PostaviZaVpp();

        [OperationContract]
        bool Podesi(out string error);

        [OperationContract]
        void posaljiNalog();

        //[OperationContract]
        //void GenerirajPozivNaBroj1();

        //[OperationContract]
        //void uplatnice();

        //[OperationContract]
        //bool Dostupan();

        //[OperationContract]
        //string PozivNaBroj(string grad, decimal ukupno, int idRedarstva);

        //[OperationContract]
        //bool Placeni(string grad, DateTime datumProdaje, out string poruka);

        //[OperationContract]
        //bool Odgode(string grad, DateTime datumProdaje, out string poruka);

        //[OperationContract]
        //void PostojiRCVozilo(string registracija, int IDLokacije);

        ////[OperationContract]
        ////List<_Statistika> Prijavljen();

        //[OperationContract]
        //bool UrediStatuse();

        //[OperationContract]
        //bool Trazi();

        //[OperationContract]
        //bool KopirajTerminale();

        //[OperationContract]
        //string Pronadji();

        //[OperationContract]
        //void DodajRucniTerminal();

        //[OperationContract]
        //int KopirajDatumID();

        //[OperationContract]
        //bool? Prenesi();

        //[OperationContract]
        //string Pauk();

        //[OperationContract]
        //string GenerirajPozivNaBroj();

        //[OperationContract]
        //void Izmjena();

        //[OperationContract]
        //void Procitaj();

        //[OperationContract]
        //string Slike();

        //[OperationContract]
        //bool UskladiPozivRijeka();

        //[OperationContract]
        //void Posalji();

        [OperationContract]
        bool Baza(out string error);
    }

    [DataContract]
    public class _PaukStat
    {
        [DataMember]
        public int IDNaloga { get; set; }
        [DataMember]
        public int IDOpisa { get; set; }
        [DataMember]
        public string Adresa { get; set; }

        public _PaukStat(int id, int ido, string adr)
        {
            IDNaloga = id;
            IDOpisa = ido;
            Adresa = srediAdresu(adr);
        }

        public string srediAdresu(string adresa)
        {
            adresa = adresa.Replace("na ulici", "");
            adresa = adresa.Replace(" u blizini kućnog broja", "");
            adresa = adresa.Replace(" kod kućnog broja", "");
            adresa = adresa.Replace("na križanju", "");
            adresa = adresa.Replace("bb", "");

            adresa = adresa.Replace("Č", "C");
            adresa = adresa.Replace("Ć", "C");
            adresa = adresa.Replace("Ž", "Z");
            adresa = adresa.Replace("Đ", "D");
            adresa = adresa.Replace("Š", "S");

            adresa = adresa.Trim();

            string pattern = @"\d+\s?[abcdABCD]?$";
            Regex rgx = new Regex(pattern);
            return rgx.Replace(adresa, "").Trim();
        }
    }

    [DataContract]
    public class _UspjesnoOcitan
    {
        [DataMember]
        public string ImePrezime { get; set; }
        [DataMember]
        public int Ocitan { get; set; }
        [DataMember]
        public int Izmijenjen { get; set; }
        [DataMember]
        public int Rucni { get; set; }

        public _UspjesnoOcitan(string ip, int oc, int iz, int ru)
        {
            ImePrezime = ip;
            Ocitan = oc;
            Izmijenjen = iz;
            Rucni = ru;
        }
    }

    [DataContract]
    public class _Kilometri
    {
        [DataMember]
        public DateTime Datum { get; set; }
        [DataMember]
        public string Vozilo { get; set; }
        [DataMember]
        public double Kilometri { get; set; }

        public _Kilometri(DateTime dat, string voz, double km)
        {
            Datum = dat;
            Vozilo = voz;
            Kilometri = km;
        }
    }

    [DataContract]
    public class _Placeni
    {
        [DataMember]
        public _Zaglavlje zaglavlje { get; set; }
        [DataMember]
        public List<_StavkeP> stavke { get; set; }
        [DataMember]
        public List<_VrstaPla> vrsta_placanja { get; set; }

        public _Placeni(_Zaglavlje zag, List<_StavkeP> stav, List<_VrstaPla> vrsta)
        {
            zaglavlje = zag;
            stavke = stav;
            vrsta_placanja = vrsta;
        }
    }

    [DataContract]
    public class _Zaglavlje
    {
        [DataMember]
        public string broj { get; set; }
        [DataMember]
        public string rj_tax_index { get; set; }
        [DataMember]
        public string datum { get; set; }
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string napomena { get; set; }

        public _Zaglavlje(string brojrac, string skiTaxIndex, string dat, string txt, string nap)
        {
            broj = brojrac;
            rj_tax_index = skiTaxIndex;
            datum = dat;
            text = txt;
            napomena = nap;
        }
    }

    [DataContract]
    public class _VrstaPla
    {
        [DataMember]
        public char vp_tip { get; set; }
        [DataMember]
        public string vp_sifra { get; set; }
        [DataMember]
        public string vp_iznos { get; set; }

        public _VrstaPla(char vrsta, string vpSifra, string vpIznos)
        {
            vp_tip = vrsta;
            vp_sifra = vpSifra;
            vp_iznos = vpIznos;
        }

    }

    [DataContract]
    public class _StavkeP
    {
        [DataMember]
        public string tbr_stp { get; set; }
        [DataMember]
        public string osnovica_roba { get; set; }
        [DataMember]
        public string osnovica_usluga { get; set; }
        [DataMember]
        public string osnovica_avans { get; set; }
        [DataMember]
        public string osnovica_agencija { get; set; }
        [DataMember]
        public string pdvi_roba { get; set; }
        [DataMember]
        public string pdvi_usluga { get; set; }
        [DataMember]
        public string pdvi_avans { get; set; }
        [DataMember]
        public string pdvi_agencija { get; set; }
        [DataMember]
        public string prolazna_agencija { get; set; }
        [DataMember]
        public string naknada { get; set; }
        [DataMember]
        public string poreznapot { get; set; }
        [DataMember]
        public string boravisna { get; set; }

        public _StavkeP(string tarifa, string roba, string usluga, string avans, string agencija, string pdvroba, string pdvusluga, string pdvavans,
            string pdvagencija, string prolazna, string nak, string potrosnja, string bor)
        {
            tbr_stp = tarifa;
            osnovica_roba = roba;
            osnovica_usluga = usluga;
            osnovica_avans = avans;
            osnovica_agencija = agencija;
            pdvi_roba = pdvroba;
            pdvi_usluga = pdvusluga;
            pdvi_avans = pdvavans;
            pdvi_agencija = pdvagencija;
            prolazna_agencija = prolazna;
            naknada = nak;
            poreznapot = potrosnja;
            boravisna = bor;
        }
    }

    [DataContract]
    public class _Odgoda
    {
        [DataMember]
        public _ZaglavljeOdgoda zaglavlje { get; set; }
        [DataMember]
        public List<_StavkeOdgoda> stavke { get; set; }
        [DataMember]
        public List<_StopeOdgoda> po_stopama { get; set; }

        public _Odgoda(_ZaglavljeOdgoda zag, List<_StavkeOdgoda> stav, List<_StopeOdgoda> stope)
        {
            zaglavlje = zag;
            stavke = stav;
            po_stopama = stope;
        }
    }

    [DataContract]
    public class _ZaglavljeOdgoda
    {
        [DataMember]
        public string Rj_tax_index { get; set; }
        [DataMember]
        public string Datum { get; set; }
        [DataMember]
        public string Dat_dos { get; set; }
        [DataMember]
        public string Broj { get; set; }
        [DataMember]
        public string Tip { get; set; }
        [DataMember]
        public string Podtip { get; set; }
        [DataMember]
        public string Par_oib { get; set; }
        [DataMember]
        public string Par_naziv { get; set; }
        [DataMember]
        public string Par_adresa { get; set; }
        [DataMember]
        public string Par_ptt { get; set; }
        [DataMember]
        public string Par_mjesto { get; set; }
        [DataMember]
        public string Poziv_na_broj { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string Napomena { get; set; }
        [DataMember]
        public string Externi_id { get; set; }

        public _ZaglavljeOdgoda(string skiTaxIndex, string dat, string datdos, string brojrac, string tp, string ptp, string oib, string naziv, string adresa, string ptt,
            string mjesto, string poziv, string txt, string nap, string id)
        {
            Rj_tax_index = skiTaxIndex;
            Datum = dat;
            Dat_dos = datdos;
            Broj = brojrac;
            Tip = tp;
            Podtip = ptp;
            Par_oib = oib;
            Par_naziv = naziv;
            Par_adresa = adresa;
            Par_ptt = ptt;
            Par_mjesto = mjesto;
            Poziv_na_broj = poziv;
            Text = txt;
            Napomena = nap;
            Externi_id = id;
        }
    }

    [DataContract]
    public class _StavkeOdgoda
    {
        [DataMember]
        public string Nci { get; set; }
        [DataMember]
        public string Mari_vel { get; set; }
        [DataMember]
        public string Vpci { get; set; }
        [DataMember]
        public string Mari_mal { get; set; }
        [DataMember]
        public string Naknada { get; set; }
        [DataMember]
        public string Mpci { get; set; }
        [DataMember]
        public string Prolazna_stavka { get; set; }
        [DataMember]
        public string Porez_na_pot { get; set; }
        [DataMember]
        public string Pdvi { get; set; }
        [DataMember]
        public string Ukupno { get; set; }

        public _StavkeOdgoda(string nci, string marivel, string vpci, string marimal, string naknada, string mpci, string prolazna, string potrosnja, string pdvi, string ukupno)
        {
            Nci = nci;
            Mari_vel = marivel;
            Vpci = vpci;
            Mari_mal = marimal;
            Naknada = naknada;
            Mpci = mpci;
            Prolazna_stavka = prolazna;
            Porez_na_pot = potrosnja;
            Pdvi = pdvi;
            Ukupno = ukupno;
        }
    }

    [DataContract]
    public class _StopeOdgoda
    {
        [DataMember]
        public string Tbr_stp { get; set; }
        [DataMember]
        public string Osnovica_Roba { get; set; }
        [DataMember]
        public string Pdvi_Roba { get; set; }
        [DataMember]
        public string Osnovica_Usluga { get; set; }
        [DataMember]
        public string Pdvi_Usluga { get; set; }
        [DataMember]
        public string Osnovica_Agencija { get; set; }
        [DataMember]
        public string Pdvi_Agencija { get; set; }

        public _StopeOdgoda(string tarifa, string osn_roba, string pdvi_roba, string osn_usluga, string pdvi_usluga, string osn_agen, string pdv_agen)
        {
            Tbr_stp = tarifa;
            Osnovica_Roba = osn_roba;
            Pdvi_Roba = pdvi_roba;
            Osnovica_Usluga = osn_usluga;
            Pdvi_Usluga = pdvi_usluga;
            Osnovica_Agencija = osn_agen;
            Pdvi_Agencija = pdv_agen;
        }
    }

}
