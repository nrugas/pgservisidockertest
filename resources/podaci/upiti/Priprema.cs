using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Priprema
    {
        public static List<_Prekrsaj> PripremiPodatke(string grad, IQueryable<_Prekrsaj> tocke, int idAplikacije)
        {
            try
            {
                List<_Prekrsaj> nova = new List<_Prekrsaj>();

                foreach (var p in tocke)
                {
                    nova.Add(new _Prekrsaj(
                        p.IDPrekrsaja,
                        p.IDRedarstva,
                        p.IDTerminala,
                        p.IDOpisaPrekrsaja,
                        p.IDOpisaZakona,
                        p.IDLokacije,
                        p.IDDjelatnika,
                        p.IDDokumenta,
                        p.Latitude,
                        p.Longitude,
                        p.DatumVrijeme,
                        p.Registracija,
                        p.Redar,
                        p.BrojIskaznice,
                        p.UID,
                        p.Adresa,
                        p.BrojDokumenta,
                        p.Terminal,
                        p.Dokument,
                        p.OpisPrekrsaja,
                        p.KratkiOpis,
                        "",
                        //Zakon(grad, p.IDOpisaPrekrsaja, idAplikacije), //TODO - Promijeniti u  p.IDOpisaZakona, kada prebacimo na novo 
                        p.ClanakPrekrsaja,
                        "",
                        //Clanak(grad, p.IDOpisaPrekrsaja, idAplikacije),//TODO - Promijeniti u  p.IDOpisaZakona, kada prebacimo na novo 
                        p.ClanakPauka,
                        p.Kazna,
                        p.Pauk,
                        p.Zahtjev,
                        p.Storniran,
                        p.Test,
                        p.Trajanje,
                        p.StatusOcitanja,
                        p.OsobaStorna,
                        p.NapomenaStorna,
                        p.Komentar,
                        p.Vozilo,
                        p.StatusVpp,
                        p.Drzava,
                        p.IDRacuna,
                        p.Racun,
                        p.Nalog));
                }

                return nova;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "PRIPREMA PODATAKA");
                return new List<_Prekrsaj>();
            }
        }

        public static string Ocitanje(byte? ocitanje)
        {
            return (ocitanje & 3) == 3 ? "I" : (ocitanje & 3) == 1 ? "O" : "R";
        }

        public static string Registracija(string registracija, string drzava)
        {
            if (string.IsNullOrEmpty(drzava))
            {
                return registracija;
            }

            return drzava != "??" ? registracija + " (" + drzava + ")" : registracija;
        }

        public static _Nalog Nalog(int? idNaloga, VozilaPauka vozilo, StatusPauka status, RazloziNepodizanjaVozila razlog, NaloziPauku nalog, Pauk pauk, string brojRacuna, string vrstaPlacanja)
        {
            if (idNaloga == null || idNaloga == 0)
            {
                return null;
            }

            return new _Nalog(idNaloga,
                vozilo == null ? 0 : vozilo.IDVozila,
                vozilo == null ? 0 : vozilo.IDTerminala,
                vozilo == null ? "" : vozilo.NazivVozila,
                status == null ? 0 : status.IDStatusa,
                status == null ? "Izdao nalog" : status.NazivStatusa,
                razlog == null ? 0 : razlog.IDRazloga,
                razlog == null ? "" : razlog.NazivRazloga,
                nalog == null ? DateTime.Now : nalog.DatumNaloga,
                pauk == null ? DateTime.Now : pauk.DatumZaprimanja,
                pauk == null ? null : pauk.DatumPodizanja,
                pauk == null ? null : pauk.DatumDeponija,
                nalog == null ? false : nalog.StornoRedara,
                nalog == null ? false : nalog.NalogZatvoren,
                status == null ? "Black" : status.Boja,
                nalog == null ? null : nalog.IDRacuna,
                brojRacuna,
                vrstaPlacanja,
                nalog == null ? false : nalog.Lisice,
                nalog == null ? "" : nalog.Napomena);
        }

        /**/

        //TODO trenutno saljem idopisa a kasnije kada uskladimo sve ovdje treba ici idzakona
        public static string Zakon(string grad, int? idZakona, int idAplikacije)
        {
            try
            {
                if (idZakona == null)
                {
                    return "";
                }

                int? id;

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    id = db.OpisiPrekrsajas.First(i => i.IDOpisa == idZakona).IDNovog;
                }

                if (id == null)
                {
                    return "";
                }

                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    return db.ZAKONI_OPISIs.First(i => i.IDOpisaZakona == id).Opis;
                }
            }
            catch (Exception)
            {

                return "????";
            }
        }

        public static string Clanak(string grad, int? idZakona, int idAplikacije)
        {
            try
            {
                if (idZakona == null)
                {
                    return "";
                }

                int? id;

                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    id = db.OpisiPrekrsajas.First(i => i.IDOpisa == idZakona).IDNovog;
                }

                if (id == null)
                {
                    return "";
                }

                using (PostavkeDataContext db = new PostavkeDataContext())
                {

                    var zak = from zo in db.ZAKONI_OPISIs
                              join z in db.ZAKONIs on zo.IDZakona equals z.IDZakona
                              where zo.IDOpisaZakona == id
                              select z.Clanak;

                    return zak.First();
                }
            }
            catch (Exception)
            {
                return "????";
            }
        }
    }
}