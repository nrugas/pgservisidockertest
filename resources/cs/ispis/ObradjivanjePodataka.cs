using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace PG.Servisi.resources.cs.ispis
{
    public static class ObradjivanjePodataka
    {
        /// <summary>
        /// Lomi tekst na određenu dužinu da bi stao na stranicu
        /// </summary>
        /// <param name="text">tekst koji se prilagođuje</param>
        /// <param name="linLen">dužina reda kojem se prilagođuje</param>
        /// <returns>tekst podjeljenj na zadanu dužinu</returns>
        public static string[] WordWrap2(string text, int linLen)
        {
            List<string> ret = new List<string>();
            string[] TN = text.Split(' ');

            string tekst = "     ";

            foreach (var s in TN)
            {
                if (s != "" || s != " ")
                {
                    if ((tekst.Length + s.Length + 1) < linLen)
                    {
                        tekst += " " + s;
                    }
                    else
                    {
                        ret.Add(tekst);
                        tekst = s;
                    }
                }
            }

            ret.Add(tekst);

            return ret.ToArray();
        }

        private static List<string> ret;

        public static List<string> LetterWrap(string text, int line)
        {
            ret = new List<string>();

            if (text.Length > line)
            {
                Sub(text, line);
            }
            else
            {
                ret.Add(text);
            }

            return ret;
        }

        private static void Sub(string text, int line)
        {
            string li = text.Substring(0, line);
            ret.Add(li.Trim());

            text = text.Replace(li, "").Trim();
            if (text.Length > line)
            {
                Sub(text, line);
            }
            else
            {
                ret.Add(text);
            }
        }

        public static string[] WordWrap(string text, int linLen)
        {
            List<string> ret = new List<string>();
            char[] del = { ' ' };
            string[] TN = text.Split(del, StringSplitOptions.RemoveEmptyEntries);

            string tekst = "          ";

            foreach (var s in TN)
            {
                if (DuzinaTeksta(tekst + s) < linLen)
                {
                    tekst += s + " ";
                }
                else
                {
                    ret.Add(tekst.Replace(" ,", ","));
                    tekst = s + " ";
                }
            }

            ret.Add(tekst.Replace(" ,", ","));
            return ret.ToArray();
        }

        public static int DuzinaTeksta(string tekst)
        {
            Font font = new Font("Arial", 16);
            return TextRenderer.MeasureText(tekst, font).Width;
        }

        /// <summary>
        /// zamjenjuje naše znakove sa normalnim
        /// </summary>
        /// <param name="sb">tekst kojem želite maknut naše znakove</param>
        /// <returns>tekst bez naših znakova</returns>
        public static StringBuilder SkidanjeKvacica(StringBuilder sb)
        {
            sb.Replace('Ć', 'C');
            sb.Replace('Č', 'C');
            sb.Replace('Š', 'S');
            sb.Replace('Ž', 'Z');
            sb.Replace('Đ', 'D');
            sb.Replace('ć', 'c');
            sb.Replace('č', 'c');
            sb.Replace('š', 's');
            sb.Replace('ž', 'z');
            sb.Replace('đ', 'd');
            return sb;
        }

        public static string SkidanjeKvacica(string sb)
        {
            return sb.Replace('Ć', 'C').Replace('Č', 'C').Replace('Š', 'S').Replace('Ž', 'Z').Replace('Đ', 'D').Replace('ć', 'c').Replace('č', 'c').Replace('š', 's').Replace('ž', 'z').Replace('đ', 'd');
        }

        public static StringBuilder MjenjanjeKvacica(StringBuilder sb)
        {
            sb.Replace('Ć', ']');
            sb.Replace('Č', '^');
            sb.Replace('Š', '[');
            sb.Replace('Ž', '@');
            sb.Replace('Đ', '\\');
            sb.Replace('ć', '}');
            sb.Replace('č', '~');
            sb.Replace('š', '{');
            sb.Replace('ž', '`');
            sb.Replace('đ', '|');
            return sb;
        }

        /// <summary>
        /// Traži ključne riječi u tekstu (riječi koje počinju sa $)
        /// </summary>
        /// <param name="Text">tekst u kojem se traže ključne rječi</param>
        /// <returns>nađene ključne rječi</returns>
        public static List<string> TrazenjeKljucnihRijeci(string Text)
        {
            List<string> nova = new List<string>();

            char[] DeLi = { ' ' };
            string[] txt = Text.Split(DeLi);

            foreach (var c in txt)
            {
                if (c.Trim(' ', ',', '.', ':', ';').StartsWith("$"))
                {
                    nova.Add(c);
                }
            }

            return nova;
        }

        /// <summary>
        /// zamjenjuje ključnu riječ za vrijednost
        /// </summary>
        /// <param name="Text">tekst u kojem želim zamijenii ključne riječi</param>
        /// <param name="xml">xml u kojem se nalaze vrijednosti</param>
        /// <returns>tekst sa pravim vrijednostima</returns>
        public static string ZamjenaKRzaVrijednosti(string Text, string xml)
        {
            XmlTextReader rdr = new XmlTextReader(new StringReader(xml));
            rdr.WhitespaceHandling = WhitespaceHandling.None;

            while (rdr.Read())
            {
                foreach (var r in TrazenjeKljucnihRijeci(Text))
                {
                    if (r.ToUpper().TrimStart('$').TrimEnd(',').TrimEnd('.') == rdr.Name.ToUpper() && rdr.Name.ToUpper() != "")
                    {
                        if (rdr.Value == "")
                        {
                            rdr.Read();
                        }

                        Text = Text.Replace(r, rdr.Value);
                        break;
                    }
                }
            }

            foreach (var r in TrazenjeKljucnihRijeci(Text))
            {
                Text = Text.Replace(r, "?");
            }

            return Text;
        }

        /// <summary>
        /// zamjenjuje ključnu riječ za tekst predloska
        /// </summary>
        /// <param name="KljucnaRijec">kljucna rijec koju zelim zamijeniti sa tekstom</param>
        /// <param name="TekstPredloska">xml u kojem se nalazi tekst predloska</param>
        /// <returns>tekst sa pravim vrijednostima</returns>
        public static string TrazenjeDijelovaPredloska(string KljucnaRijec, XElement TekstPredloska)
        {
            XElement allData = XElement.Parse(TekstPredloska.ToString());

            string x = KljucnaRijec.TrimStart('$');

            var v = from page in allData.Descendants(x)
                    select page;

            string Predlozak = "";

            foreach (var e in v)
            {
                Predlozak = e.Value;
            }

            return Predlozak;
        }
    }
}
