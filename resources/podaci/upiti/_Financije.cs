using System;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Financije
    {
        //todo obrisi sve
        ///*:: RIJEKA ::*/

        //public static int NoviBrojRacuna(string grad, int idNaloga, out decimal? iznos, int idAplikacije)
        //{
        //    try
        //    {
        //        if (grad.ToUpper() == "PROMETNIK_RIJEKA")
        //        {
        //            using (PaukClient sc = new PaukClient())
        //            {
        //                return sc.PodaciORacunu(idNaloga, out iznos);
        //            }
        //        }

        //        iznos = 0;
        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "NOVI BROJ RAČUNA - FINANCIJE");

        //        iznos = 0;
        //        return -1;
        //    }
        //}

        //public static int SpremiNalog(string grad, string registracija, string obavjest, string idDjelatnika, string imePrezime, DateTime vrijemeNaloga,
        //    int idPrekrsaja, string opisPrekrsaja, string clanak, string adresa, int idAplikacije)
        //{
        //    try
        //    {
        //        using (PaukClient sc = new PaukClient())
        //        {
        //            return sc.SpremiNalog(registracija, obavjest, idDjelatnika, imePrezime, vrijemeNaloga, idPrekrsaja, opisPrekrsaja, clanak, adresa);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI NALOG - FINANCIJE");
        //        return -1;
        //    }
        //}

        //public static void Slike(string grad, int idNaloga, int idLokacije, int idAplikacije)
        //{
        //    try
        //    {
        //        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
        //        {
        //            using (PaukClient sc = new PaukClient())
        //            {
        //                foreach (var q in db.SlikaPrekrsajas.Where(i => i.IDLokacije == idLokacije))
        //                {
        //                    sc.SpremiSliku(idNaloga, q.Slika.ToArray());
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "SPREMI SLIKU - FINANCIJE");
        //    }
        //}

        //public static void StornirajNalog(string grad, int idNaloga, int? idRazloga, int idVozila, int idStatusa, int idAplikacije)
        //{
        //    try
        //    {
        //        if (grad.ToUpper() == "PROMETNIK_RIJEKA")
        //        {
        //            if (idStatusa == 10)
        //            {
        //                IzmjeniNalog(grad, idNaloga, 8, Nalog.NazivRazloga(grad, idRazloga, idAplikacije), 1, 0, null, null, null, "", idAplikacije);
        //            }
        //            else
        //            {
        //                IzmjeniStatus(grad, idNaloga, idStatusa, DateTime.Now, idRazloga, idVozila, "", idAplikacije);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATI - FINANCIJE");
        //    }
        //}

        //public static bool IzmjeniStatus(string grad, int idNaloga, int idStatusa, DateTime datumVrijeme, int? idRazloga, int idVozila, string poziv, int idAplikacije)
        //{
        //    try
        //    {
        //        DateTime? DatumPreuzimanja = null;
        //        DateTime? DatumPodizanja = null;
        //        DateTime? DatumDeponija = null;
        //        //int IDOperatera;

        //        string KomentraiOstalo = Nalog.NazivRazloga(grad, idRazloga, idAplikacije);

        //        if (idStatusa == 1)
        //        {
        //            DatumPreuzimanja = datumVrijeme;
        //        }
        //        if (idStatusa == 2)
        //        {
        //            DatumPodizanja = datumVrijeme;
        //        }
        //        if (idStatusa == 4)
        //        {
        //            DatumDeponija = datumVrijeme;
        //        }

        //        if (idStatusa == 10 || idStatusa == 11)
        //        {
        //            KomentraiOstalo = Nalog.NazivStatusaNaloga(grad, idStatusa, idAplikacije);
        //        }

        //        if (idStatusa == 11) idStatusa = 5;

        //        return IzmjeniNalog(grad, idNaloga, idStatusa, KomentraiOstalo, idVozila, IDOperatera(grad, idVozila, idAplikacije), DatumPreuzimanja,
        //            DatumPodizanja, DatumDeponija, poziv, idAplikacije);
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI STATUS RP - FINANCIJE");
        //        return false;
        //    }
        //}

        //public static bool Naplati(string grad, int idNaloga, int idTerminala, int brojRacuna, int idAplikacije)
        //{
        //    try
        //    {
        //        if (grad.ToUpper() == "PROMETNIK_RIJEKA")
        //        {
        //            using (PaukClient sc = new PaukClient())
        //            {
        //                int idVozila = Nalog.IDVozila(grad, idTerminala, idAplikacije);
        //                return sc.NaplatiNaLicuMjesta(idNaloga, IDOperatera(grad, idVozila, idAplikacije), 3, brojRacuna);
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "NAPLATI - FINANCIJE");
        //        return false;
        //    }
        //}

        //private static bool IzmjeniNalog(string grad, int idNaloga, int idStatusa, string StausKomentar, int PaukBr, int IDOperatera, DateTime? DatumPreuzimanja,
        //    DateTime? DatumPodizanja, DateTime? DatumDeponiranja, string poziv, int idAplikacije)
        //{
        //    try
        //    {
        //        using (PaukClient sc = new PaukClient())
        //        {
        //            sc.IzmjeniNalogNovi(idNaloga, idStatusa, StausKomentar, PaukBr, IDOperatera, DatumPreuzimanja, DatumPodizanja, DatumDeponiranja, poziv);
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "IZMJENI NALOG - FINANCIJE");
        //        return false;
        //    }
        //}

        //private static int IDOperatera(string grad, int idVozila, int idAplikacije)
        //{
        //    try
        //    {
        //        bool Jutarnja = !(DateTime.Now.Hour > 14);

        //        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
        //        {
        //            var iddj = from p in db.Posadas
        //                       join d in db.DjelatniciPaukas on p.IDDjelatnika1 equals d.IDDjelatnikaPauka
        //                       where p.IDVozila == idVozila &&
        //                             p.DatumVrijeme.Date == DateTime.Today.Date &&
        //                             p.JutarnjaSmjena == Jutarnja
        //                       select d.IDOperatera;

        //            return (int)iddj.First();
        //        }
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}
    }
}