//using Orderman;
using System;
using System.Collections;
using System.Text;

namespace PG.Servisi.resources.cs.ispis
{
    /// <summary>
    /// Summary description for HrCharConverter.
    /// </summary>
    public class HrCharConverterLight
    {
        public static string NoHr(string hrText)
        {
            return (hrText.Replace("»", "C").Replace("∆", "C").Replace("–", "DJ").Replace("é", "Z").Replace("ä", "S").Replace("Ë", "c").Replace("Ê", "c").Replace("", "dj").Replace("û", "z").Replace("ö", "s"));
        }

        public static string ToHr(string hrText)
        {
            return (hrText.Replace("»", "^").Replace("∆", "]").Replace("–", "\\").Replace("é", "@").Replace("ä", "[").Replace("Ë", "}").Replace("Ê", "~").Replace("", "|").Replace("û", "`").Replace("ö", "{"));
        }

        private static HrCharConverterLight converter;

        public static HrCharConverterLight Converter
        {
            get
            {
                if (converter == null) converter = new HrCharConverterLight();
                return converter;
            }
        }

        private HrCodePage[] cps;
        public HrCharConverterLight()
        {
            cps = new HrCodePage[4];
            cps[0] = new HrCodePage(1250, @"»Ë∆Ê–äöéû");
            //cps[0] = new HrCodePage(1250, new byte[] { 200, 232, 198, 230, 208, 240, 138, 154, 142, 158}); 
            //cps[0] = new HrCodePage(1250, new byte[] { 0xC8, 0xE8, 0xC6, 0xE6, 0xD0, 0xF0, 0x8A, 0x9A, 0x8E, 0x9E });
            cps[1] = new HrCodePage(437, @"^~]}\|[{@`");

            //cps[2] = new HrCodePage(852,   "\u00BC\u0192\u00C5\u00E5\u2564\u2568\u00B5\u03C4\u00AA\u00BA"); //  @"¨üèÜ—–ÊÁ¶ß");
            //cps[2] = new HrCodePage(852,   "\u00AC\u009F\u008F\u0086\u00D1\u00D0\u00E6\u00E7\u00A6\u00A7"); , new byte[] { 0xAC, 0x9F, 0x8F, 0x86, 0xD1, 0xD0, 0xE6, 0xE7, 0xA6, 0xA7 });
            cps[2] = new HrCodePage(852, new byte[] { 0xAC, 0x9F, 0x8F, 0x86, 0xD1, 0xD0, 0xE6, 0xE7, 0xA6, 0xA7 });
            cps[3] = new HrCodePage(0, "CcCcDdSsZz");
        }

        private ArrayList GetHrBytes(int cp)
        {
            ArrayList ret = null;
            foreach (HrCodePage h in cps)
            {
                if (h.cp == cp)
                {
                    ret = h.hrBytes;
                    break;
                }
            }
            return ret;
        }

        //public byte[] PrintConvert(byte[] msg, int cpTo)
        //{
        //    return CPConvert(msg, 1250, cpTo);
        //}

        public string CPConvert(string msg, int cpFrom, int cpTo)
        {
            //return Encoding.UTF8.GetString(CPConvert(Encoding.UTF8.GetBytes(msg), cpFrom, cpTo));
            return Encoding.Default.GetString(CPConvert(Encoding.UTF8.GetBytes(msg), cpFrom, cpTo));
        }

        public byte[] CPConvert(byte[] msg, int cpFrom, int cpTo)
        {
            //			if (cpFrom != 0 || (cpFrom = this.DetectCodePage(text)) != 0)
            //			{
            Encoding enc = Encoding.UTF8;
            ArrayList fromAL = GetHrBytes(cpFrom);
            ArrayList toAL = GetHrBytes(cpTo);

            ByteCollection ret = new ByteCollection(msg.Length);
            int i = 0;
            while (i < msg.Length)
            {
                if (msg[i] == 0x1b)
                {
                    ret.Add(msg[i]);
                    i++;
                }
                else
                {
                    bool fnd = false;
                    int j;
                    for (j = 0; j < fromAL.Count; j++)
                    {
                        byte[] ba = (byte[])fromAL[j];
                        if (ArrayCompare(msg, i, ba) == 0)
                        {
                            i += ba.Length;
                            fnd = true;
                            break;
                        }
                    }
                    if (fnd)
                    {
                        foreach (byte b in (byte[])toAL[j]) ret.Add(b);
                        continue;
                    }
                }
                ret.Add(msg[i++]);
            }
            return ret.GetBytes();
        }

        private int ArrayCompare(byte[] a1, int offset, byte[] a2)
        {
            int ret = 0;
            for (int i = 0; i < Math.Min(a1.Length - offset, a2.Length); i++)
            {
                ret = a1[offset + i].CompareTo(a2[i]);
                if (ret != 0) break;
            }
            return ret;
        }

        private class HrCodePage
        {
            public int cp;
            public ArrayList hrBytes = new ArrayList();

            public HrCodePage(int cp, string s)
            {
                this.cp = cp;
                for (int i = 0; i < s.Length; i++) hrBytes.Add(Encoding.UTF8.GetBytes(s.Substring(i, 1)));
            }

            public HrCodePage(int cp, byte[] bytes)
            {
                this.cp = cp;
                for (int i = 0; i < bytes.Length; i++) hrBytes.Add(new byte[1] { bytes[i] });
            }
        }

        public static byte[] CharsToBytes(char[] chars)
        {
            byte[] ret;
            ArrayList b = new ArrayList();
            for (int i = 0; i < chars.Length; i++)
            {
                short c = Convert.ToInt16(chars[i]);
                if (c > 0xff) b.Add(Convert.ToByte(c / 0x100));
                b.Add(Convert.ToByte(c % 0x100));
            }
            ret = new byte[b.Count];
            for (int i = 0; i < ret.Length; i++) ret[i] = (byte)b[i];
            return ret;
        }
    }

    public class HRDateTime
    {
        public static string LongDateTime(DateTime dt)
        {
            string[] mjes = new string[12] { "sijeËnja", "veljaËe", "oûujka", "travnja", "svibnja", "lipnja", "srpnja", "kolovoza", "rujna", "listaopada", "studenog", "prosinca" };
            string ret = dt.Day + "." + mjes[dt.Month] + " " + dt.Year;
            ret += " u " + dt.TimeOfDay;
            return ret;
        }
    }
}