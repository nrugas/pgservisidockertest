using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.cs.email
{
    public class Pripremi
    {
        //public static string PopulateBodyPrijavaProblema(string grad, int idPrijave, _Problem problem)
        //{
        //    string body;

        //    string apPath = HostingEnvironment.ApplicationPhysicalPath;

        //    using (StreamReader reader = new StreamReader(apPath + "/resources/templates/PrijavaProblema.html"))
        //    {
        //        body = reader.ReadToEnd();
        //    }

        //    body = body.Replace("{Grad}", grad);
        //    body = body.Replace("{Osoba}", problem.Djelatnik);
        //    body = body.Replace("{Broj}", idPrijave.ToString()); 
        //    body = body.Replace("{Datum}", DateTime.Now.ToString("dd.MM.yy u HH:mm"));
        //    body = body.Replace("{Aplikacija}", problem.Aplikacija);

        //    string opis = problem.Opis;

        //    if (problem.IDTerminala != null)
        //    {
        //        opis += string.Format("<br/><br/>Terminal: {0} (ID: {1})", problem.Terminal, problem.IDTerminala);
        //    }
        //    if (problem.Redar != null)
        //    {
        //        opis += string.Format("<br/><br/>Redar / Vozilo: {0} (ID: {1})", problem.Redar, problem.IDRedara);
        //    }

        //    body = body.Replace("{Opis}", opis);
        //    body = body.Replace("{Interval}", problem.Interval);
        //    body = body.Replace("{Radnja}", problem.Radnja);

        //    return body;
        //}

        public static string PopulateBodyNarudzba(string grad, _Narudzba narudzba, int idAplikacije)
        {
            string body = "";

            string apPath = HostingEnvironment.ApplicationPhysicalPath;

            if (narudzba.IDRedarstva == 2)
            {
                using (StreamReader reader = new StreamReader(apPath + "/resources/templates/NarudzbaMaterijalaPauk.html"))
                {
                    body = reader.ReadToEnd();
                }
            }
            else if (narudzba.IDRedarstva == 1)
            {
                using (StreamReader reader = new StreamReader(apPath + "/resources/templates/NarudzbaMaterijala.html"))
                {
                    body = reader.ReadToEnd();
                }
            }

            if (body == "")
            {
                return "";
            }

            string aplikacija;

            using (PostavkeDataContext db = new PostavkeDataContext())
            {
                aplikacija = db.APLIKACIJEs.First(i => i.IDAplikacije == idAplikacije).NazivAplikacije;
            }

            body = body.Replace("{Grad}", grad);
            body = body.Replace("{Osoba}", narudzba.Korisnik);
            body = body.Replace("{Broj}", narudzba.IDNarudzbe.ToString());
            body = body.Replace("{Datum}", DateTime.Now.ToString("dd.MM.yy u HH:mm"));
            body = body.Replace("{Aplikacija}", aplikacija);
            body = body.Replace("{Tristo}", narudzba.Tristo.ToString());
            body = body.Replace("{Petsto}", narudzba.Petsto.ToString());
            if (narudzba.IDRedarstva == 1)
            {
                body = body.Replace("{Sedamsto}", narudzba.Sedamsto.ToString());
            }
            body = body.Replace("{Etuia}", narudzba.Etuia.ToString());
            body = body.Replace("{Trake}", narudzba.Traka.ToString());
            body = body.Replace("{Isporuka}", narudzba.Dostava);
            body = body.Replace("{Napomena}", narudzba.Napomena);
            body = body.Replace("{Adresa}", narudzba.Adresa);

            return body;
        }

        //public static string PopulateBodyPitanje(string grad, string osoba, int idPrijave, string aplikacija, string poruka)
        //{
        //    string body;

        //    string apPath = HostingEnvironment.ApplicationPhysicalPath;

        //    using (StreamReader reader = new StreamReader(apPath + "/resources/templates/PitanjeZahtjev.html"))
        //    {
        //        body = reader.ReadToEnd();
        //    }

        //    body = body.Replace("{Grad}", grad);
        //    body = body.Replace("{Osoba}", osoba);
        //    body = body.Replace("{Broj}", idPrijave.ToString());
        //    body = body.Replace("{Datum}", DateTime.Now.ToString("dd.MM.yy u HH:mm"));
        //    body = body.Replace("{Aplikacija}", aplikacija);
        //    body = body.Replace("{Upit}", poruka);

        //    return body;
        //}

        public static string PopulateBodyLozinka(string userName, string uid, string lozinka)
        {
            string body;

            string apPath = HostingEnvironment.ApplicationPhysicalPath;

            using (StreamReader reader = new StreamReader(apPath + "/resources/templates/IzmjenaLozinke.html"))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{UserName}", userName);
            body = body.Replace("{UID}", uid);
            body = body.Replace("{Lozinka}", lozinka);

            return body;
        }

        public static string PopulateBodyPrijenos(string userName, string razdoblje)
        {
            string body;

            string apPath = HostingEnvironment.ApplicationPhysicalPath;

            using (StreamReader reader = new StreamReader(apPath + "/resources/templates/Prijenos.html"))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{UserName}", userName);
            body = body.Replace("{Razdoblje}", razdoblje);

            return body;
        }

        public static string PopulateBodyJLS(string jls, string web, string kod)
        {
            string body;

            string apPath = HostingEnvironment.ApplicationPhysicalPath;

            using (StreamReader reader = new StreamReader(apPath + "/resources/templates/NovaJLS.html"))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{JLS}", jls);
            body = body.Replace("{web}", web);
            body = body.Replace("{kod}", kod);

            return body;
        }

        public static string PopulateBodyDobrodoslica(string grad, string imePrezime, string korisnicko, string lozinka)
        {
            string body;

            string kod;

            using (PostavkeDataContext db = new PostavkeDataContext())
            {
                kod = db.GRADOVIs.First(i => i.Baza == grad).AktivacijskiKod;
            }

            string apPath = HostingEnvironment.ApplicationPhysicalPath;

            using (StreamReader reader = new StreamReader(apPath + "/resources/templates/PorukaDobrodoslice.html"))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{ImePrezime}", imePrezime);
            body = body.Replace("{Lozinka}", lozinka);
            body = body.Replace("{KorisnickoIme}", korisnicko);
            body = body.Replace("{Kod}", kod);

            return body;
        }

        public static string PopulateBodyNaredba(string imePrezime, int idNaloga, bool tip)
        {
            string body;

            string apPath = HostingEnvironment.ApplicationPhysicalPath;

            using (StreamReader reader = new StreamReader(apPath + "/resources/templates/Naredba.html"))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{ImePrezime}", imePrezime);
            body = body.Replace("{id}", idNaloga.ToString());
            body = body.Replace("{tip}", tip ? "blokiranje" : "podizanje");
            body = body.Replace("{tip}", tip ? "blokiranje" : "podizanje");

            return body;
        }
    }
}