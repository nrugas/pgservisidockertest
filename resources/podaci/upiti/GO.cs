using System;
using System.Collections.Generic;
using System.Text;
using PG.Servisi.GOService;
using PG.Servisi.resources.cs.ispis;

namespace PG.Servisi.resources.podaci.upiti
{
    public class GO
    {
        public static List<_3DLista> GradoviGO(int idAplikacije)
        {
            try
            {
                using (GOPazigradClient sc = new GOPazigradClient())
                {
                    List<_3DLista> gradovi = new List<_3DLista>();

                    foreach (var g in sc.Gradovi())
                    {
                        gradovi.Add(new _3DLista(g.ID, g.Naziv, g.Opis));
                    }

                    return gradovi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Gradovi GO");
                return new List<_3DLista>();
            }
        }

        public static List<_3DLista> GrupeGO(string grad, int idAplikacije)
        {
            try
            {
                using (GOPazigradClient sc = new GOPazigradClient())
                {
                    List<_3DLista> grupe = new List<_3DLista>();

                    foreach (var g in sc.Grupe(grad))
                    {
                        grupe.Add(new _3DLista(g.ID, g.Naziv, g.Napomena));
                    }

                    return grupe;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "Grupe GO");
                return new List<_3DLista>();
            }
        }

        /*:: PREDLOŠCI ::*/

        public static bool Predlosci(string grad, string naziv, out string ispis, int idAplikacije)
        {
            try
            {
                switch (naziv)
                {
                    case "#javnepovrsine":
                        ispis = JavnePovrsine(grad, 0, 4, idAplikacije);
                        return true;
                    default:
                        ispis = "";
                        return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "PREDLOŠCI GO");
                ispis = "";
                return false;
            }
        }

        private static string JavnePovrsine(string grad, int y, int idRedarstva, int idAplikacije)
        {
            _Uplatnica up = Gradovi.Uplatnica(grad, idRedarstva, idAplikacije);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("CENTER");
            sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) +
                          " --------------------------------------------------------------------");

            #region PODACI ZA PLAĆANJE

            sb.AppendLine("CENTER");
            sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 80) + " PODACI ZA PLAĆANJE ");

            sb.AppendLine("LEFT");
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 80) +
                          string.Format(" Primatelj: {0}, {1}, {2} {3}", up.Naziv, up.UlicaBroj, up.Posta, up.Mjesto));
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                          string.Format(" Iznos: {0:n2} kn", "100"));
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + string.Format(" IBAN: {0}", up.IBAN));
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + string.Format(" Model: HR{0}", up.Model));
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                          string.Format(" Poziv na broj: {0}-{1}-{2}", up.Poziv1, "22222", up.Poziv2));
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + string.Format(" Opis: {0}", up.Opis));

            #endregion

            #region BARCODE

            sb.AppendLine("B PDF-417 200 " + (y += 80) + " XD 2 YD 12 C 9 S 4");
            sb.AppendLine("HRVHUB30");
            sb.AppendLine("HRK");
            sb.AppendLine(((int)(100 * 100)).ToString("000000000000000"));
            //platitelj
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            //primatelj
            sb.AppendLine(up.Naziv);
            sb.AppendLine(
                ObradjivanjePodataka.SkidanjeKvacica(string.Format("{0}", up.UlicaBroj).TrimEnd(' ')).ToUpper());
            sb.AppendLine(ObradjivanjePodataka.SkidanjeKvacica(up.Posta + " " + up.Mjesto).ToUpper());
            sb.AppendLine(up.IBAN);
            sb.AppendLine("HR" + up.Model);
            sb.AppendLine(up.Poziv1 + "-" + "222222" + "-" + up.Poziv2);
            sb.AppendLine(""); //šifra namjene
            sb.AppendLine(ObradjivanjePodataka.SkidanjeKvacica(up.Opis));
            sb.AppendLine("ENDPDF");

            #endregion

            y = y + 130;

            sb.AppendLine("LEFT");
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 80) +
                          " Plaćanje možete izvršiti ispunjavanjem uplatnice pomoću \"PODATAKA ZA PLAĆANJE\" ili");
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                          " pomoću iznad ispisanog 2D barkoda kojim možete plaćanje izvršiti bez ispunavanja uplatnice.");
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                          " Plaćanje 2D barkodom možete izvršiti naprodajnom mjestu koje podržava takav oblik plaćanja,");
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                          " npr. u obližnjoj poslovnici FINE-e, u banci, moblinim bankarskim aplikacijama, na kioscima.");

            return "! 0 200 200 " + (y + 100) + " " + 1 + "\r\n" + ObradjivanjePodataka.MjenjanjeKvacica(sb) +
                   "\r\nPRINT\r\n";
        }
    }
}