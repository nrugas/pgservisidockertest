using System;
using System.Collections.Generic;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.cs.email;
using PG.Servisi.resources.pdf;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class MailLista
    {
        public static void PosaljiNaredbu(string grad, int idNaloga, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    // todo obrisi temp
                    if (db.EMAIL_LISTAs.Any(i => i.IDGrada == Sistem.IDGrada(grad) && i.Naredba))
                    {
                       _Prekrsaj prekrsaj = Prekrsaj.DetaljiPrekrsajaNalog(grad, idNaloga, idAplikacije);

                        if (prekrsaj == null)
                        {
                            Sustav.SpremiGresku(grad, new Exception("idn: " + idNaloga + ", grad: " + grad), idAplikacije, "POSALJI NAREDBU NA EMAIL");
                            return;
                        }

                        if (prekrsaj.Nalog == null)
                        {
                            Sustav.SpremiGresku(grad, new Exception("idn: " + idNaloga + ", grad: " + grad), idAplikacije, "POSALJI NAREDBU NA EMAIL");
                            return;
                        }

                        GRADOVI g = db.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad));

                        CreatePDF._tipJls = g.Tip;
                        CreatePDF._naziv = g.NazivGrada;
                        CreatePDF._grb = g.Grb;
                        CreatePDF._odlukaLisice = g.OdlukaLisice;

                        string putanja = CreatePDF.Naredba(grad, new List<_Prekrsaj>() { prekrsaj });//Prekrsaj(grad, prekrsaj, hub, idAplikacije);

                        if (putanja == "")
                        {
                            Sustav.SpremiGresku(grad, new Exception("putanja"), idAplikacije, "POSALJI NAREDBU NA EMAIL");
                        }

                        foreach (var email in db.EMAIL_LISTAs.Where(i => i.IDGrada == Sistem.IDGrada(grad) && i.Naredba))
                        {
                            //bool hub = email.Prilog && prekrsaj.Dokument == "OBAVIJEST"; //todo
                            string poruka = Pripremi.PopulateBodyNaredba(email.ImePrezime, idNaloga, prekrsaj.Nalog.Lisice);

                            string tip = "podizanje";

                            if (prekrsaj.Nalog.Lisice)
                            {
                                tip = "blokiranje";
                            }

                            Posalji.Email(grad, poruka, "Naredba za " + tip + " br. " + idNaloga, new List<string> { email.Email }, putanja, true, idAplikacije);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "POSALJI NAREDBU NA EMAIL");
            }
        }

        /*:: ADMINISTRACIJA ::*/

        public static List<_MailLista> DohvatiMailListu(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var ml = from m in db.EMAIL_LISTAs
                        where m.IDGrada == Sistem.IDGrada(grad)
                        select new _MailLista(m.IDListe, m.IDGrada, m.Email, m.ImePrezime, m.Naredba, m.Prilog);

                    return ml.ToList();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "DOHVATI EMAIL LISTU");
                return new List<_MailLista>();
            }
        }

        public static bool ObrisiMailListu(string grad, int idListe, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    db.EMAIL_LISTAs.DeleteOnSubmit(db.EMAIL_LISTAs.First(i => i.IDListe == idListe));
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "");
                return false;
            }
        }

        public static int DodajMailListu(string grad, _MailLista lista, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    EMAIL_LISTA el = new EMAIL_LISTA();

                    int id = 1;

                    if (db.EMAIL_LISTAs.Any())
                    {
                        id = db.EMAIL_LISTAs.Max(i => i.IDListe) + 1;
                    }

                    el.IDListe = id;
                    el.IDGrada = Sistem.IDGrada(grad);
                    el.Email = lista.Email;
                    el.ImePrezime = lista.ImePrezime;
                    el.Naredba = lista.Naredba;
                    el.Prilog = lista.PrilogHUB;

                    db.EMAIL_LISTAs.InsertOnSubmit(el);
                    db.SubmitChanges();

                    return id;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "");
                return -1;
            }
        }

        public static bool SaljiMailListu(string grad, int idListe, bool salji, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    EMAIL_LISTA el = db.EMAIL_LISTAs.First(i => i.IDListe == idListe);
                    el.Naredba = salji;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "");
                return false;
            }
        }

        public static bool PrilogMailListi(string grad, int idListe, bool hub, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    EMAIL_LISTA el = db.EMAIL_LISTAs.First(i => i.IDListe == idListe);
                    el.Prilog = hub;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "");
                return false;
            }
        }
    }
}