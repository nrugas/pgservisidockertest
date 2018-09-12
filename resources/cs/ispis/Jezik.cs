using System.Linq;
using System.Text.RegularExpressions;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.cs.ispis
{
    public class Jezik
    {
        public static string PromjeniJezikTeksta(string connString, string tekst, int idJezika, int idAplikacije)
        {
            try
            {
                return SitniPrijevodi(PromijeniTekstPrekrsaja(connString, PromijeniTekstClanka(connString, tekst, idJezika, idAplikacije), idJezika, idAplikacije), idJezika);
            }
            catch
            {
                return tekst;
            }
        }

        private static string PromijeniTekstPrekrsaja(string connString, string tekst, int idJezika, int idAplikacije)
        {
            string tekstPrekrsaja = "";

            const string pat2 = @"(<Prekrsaj>)+.*(</Prekrsaj>)+";
            Regex r2 = new Regex(pat2);
            Match m2 = r2.Match(tekst);
            if (m2.Success)
            {
                tekstPrekrsaja = m2.Value.Replace("<Prekrsaj>", "").Replace("</Prekrsaj>", "").Trim();
            }

            using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(connString, idAplikacije)))
            {
                int idOpisa = 0;
                foreach (OpisiPrekrsaja opi in db.OpisiPrekrsajas)
                {
                    if (ObradjivanjePodataka.SkidanjeKvacica(opi.OpisPrekrsaja) == ObradjivanjePodataka.SkidanjeKvacica(tekstPrekrsaja))
                    {
                        idOpisa = opi.IDPrekrsaja;
                        break;
                    }
                }

                var pre = from p in db.OpisiPrekrsajaPrijevodis
                          where p.IDOpisa == idOpisa &&
                                p.IDJezika == idJezika
                          select p.Prijevod;

                return Regex.Replace(tekst, pat2, "<Prekrsaj>" + pre.First() + "</Prekrsaj>");
            }
        }

        private static string PromijeniTekstClanka(string connString, string tekst, int idJezika, int idAplikacije)
        {
            string tekstClanka = "";

            const string pat2 = @"(<Clanak>)+.*(</Clanak>)+";
            Regex r2 = new Regex(pat2);
            Match m2 = r2.Match(tekst);
            if (m2.Success)
            {
                tekstClanka = m2.Value.Replace("<Clanak>", "").Replace("</Clanak>", "").Trim();
            }

            using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(connString, idAplikacije)))
            {
                int idPrekrsaja = 0;
                foreach (PopisPrekrsaja prek in db.PopisPrekrsajas)
                {
                    if (ObradjivanjePodataka.SkidanjeKvacica(prek.MaterijalnaKaznjivaNorma) == ObradjivanjePodataka.SkidanjeKvacica(tekstClanka))
                    {
                        idPrekrsaja = prek.IDPrekrsaja;
                        break;
                    }
                }

                var pre = from p in db.PopisPrekrsajaPrijevodis
                    where p.IDPrekrsaja == idPrekrsaja &&
                          p.IDJezika == idJezika
                    select p.PrijevodClanka;

                return Regex.Replace(tekst, pat2, "<Clanak>" + pre.First() + "</Clanak>");
            }
        }

        private static string SitniPrijevodi(string tekst, int idJezika)
        {
            string prevedeni = tekst;

            if (idJezika == 1)
            {
                prevedeni = tekst.Replace("na ulici", "on street").Replace("kod kućnog broja", "").Replace("u blizini kućnog broja", "");
            }

            if (idJezika == 2)
            {
                prevedeni = tekst.Replace("na ulici", "Strasse").Replace("kod kućnog broja", "").Replace("u blizini kućnog broja", "");
            }

            if (idJezika == 3)
            {
                prevedeni = tekst.Replace("na ulici", "in via").Replace("kod kućnog broja", "").Replace("u blizini kućnog broja", "");
            }

            return prevedeni;
        }

        public static string PretvoriHR(string tekst)
        {
            return HrCharConverterLight.Converter.CPConvert(tekst, 1250, 437);
        }

        public static string Fontovi(int IDFonta)
        {
            string Font = "";

            switch (IDFonta)
            {
                case -2:
                    Font = "calibrib1.cpf";
                    break;
                case -1:
                    Font = "calibri9.cpf";
                    break;
                case 0:
                    Font = "0";
                    break;
                case 1:
                    Font = "AriN08p.cpf";
                    break;
                case 2:
                    Font = "AriN10p.cpf";
                    break;
                case 3:
                    Font = "AriN12p.cpf";
                    break;
                case 4:
                    Font = "AriN14p.cpf";
                    break;
                case 5:
                    Font = "Ari16Bpt.cpf";
                    break;
                case 6:
                    Font = "AriN10Bp.cpf";
                    break;
                case 7:
                    Font = "AriN12Bp.cpf";
                    break;
            }

            return Font;
        }

        public const string grb = "000000000000003000000000000000000000004000007C00000C0000000000000000F00001B600003C00000000000000019C00037B0000760000000000000003060006FFC001C3000000000000000603801DFF600701800000000000000400E037FFB00E00C000020000C0000C00386FFFF8380060000E0001B80018000CDFFFF6600030007B00036E00300007BFFFFBC0001801CD0002FBC06000007FFFFF00000C0F7E8007FF70C01FFFFFFFFF0000063DFFC005FFDC81FFFFFFFFFF000003EFFF400FFFF70FFFFFF97D3E000001BFFFE00BFFFE7FFFFFF07E1E000001FFFFA017FFFFFFFFFFF07C0E07C201FFEFD037EFFFFFFFFFE07DAE0FFE03FE0FF82FE0FFFFFFFFC53806FFFC03FF07E87FC0FFFFFC00C038067FFF83FE01FC5F00FFFF0000C038067F3F03FC07F4FFC03FF00000E03C1E7E0007FE07FEBFC0FFC00000F0FF3EFE0067FF07FEBFC1FFC000007FFDFE7FFFE7FFBFFE5FDDFFE007FFFFABFE1FFFE1FFFFFE5FFFFFE0FFFFFF83FE3FFFC01FFFF46FFFFFEFFFFFFF81FC7FFFCC03FFFC2FFFFFFFFFFFFF2CFC3FFFDFC07FE817FFFFFFFFFFFF00FC3FFF9FF80FF817FFFFFFFFFFFF00FC3FFF9FFF83D01BFFFEFFFFFFFF01FC77FFDFFFE0700BFFFCFFFFFE7FC3FC6E3BBFFFFC2005FFF0FFFC007FFFFCEE733FFFFF6005FFC0FF80007FFFFCEEE73FF9FFC007E001F800007FFFFCDEEE07FFFFC002F007FC00007FFFFC1C0E00FFFF8001FE1FFC00003FFFFC0000701FFF80017FFFFC000020000800007F03FF0001FFFFFC000FFFFFFFE0007FE07F0000BFFFFE07FFC00007FFC0FFFC0E0000FFFFF9FF0000000001FF3FFF8600005FFF9FC0000000000007F3FFF400007FF9F8000000000000003F3FFC00002FDF800000000000000003E7F800003FF80000000000000000003EF80000178000000000000000000007D000000C00000000000000000000007000001FFFFFFFFFFFFFFFFFFFFFFFF0000010000000000000000000000010000017FFFFFFFFFFFFFFFFFFFFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFFFFFFFFFFFFFFFFFFFFD00000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE000050000017FFFF7FBFBFFFFBBFBDFFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFFFFFFFFFFFFFFFFFFFFD00000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000F000001E0000FFFFE0000FFFFE0000F000001E0000FFFFE0000FFFFE0000B000001A0000FFFFE0000FFFFE0000A000000A0000FFFFE0000FFFFE0001A000000B0000FFFFE0000FFFFE0001E000000D0000FFFFE0000FFFFE0001400000050000FFFFE0000FFFFE0003400000058000FFFFE0000FFFFE0003C00000068000FFFFE0000FFFFE000680000002C000FFFFE0000FFFFE0007800000034000FFFFE0000FFFFE0005000000016000FFFFE0000FFFFE000D00000001A000FFFFE0000FFFFE001B00000000B000FFFFE0000FFFFE001E00000000D000FFFFE0000FFFFE0034000000006800FFFFE0000FFFFE002C000000002FFFFFFFFFFFFFFFFFFFE80000000037FF00003FFFF80001FFD8000000001BFF00003FFFF80001FFB0000000000DFF00003FFFF80001FF600000000005FF00003FFFF80001FF400000000006FF00003FFFF80001FE8000000000037F00003FFFF80001FD800000000001BF00003FFFF80001FB000000000000DF00003FFFF80001F60000000000006780003FFFF80001EC0000000000001B00003FFFF80001980000000000000D80003FFFF80003600000000000000660003FFFF8000EC000000000000003B8003FFFF800198000000000000000CE003FFFF80077000000000000000077803FFFF801CC00000000000000001CE03FFFF807300000000000000000073C3FFFF83CE0000000000000000001CFBFFFF9E78000000000000000000079FFFFFF1E000000000000000000000F0FFFF0F00000000000000000000001F8001F8000000000000000000000001FFFF8000000000000";
    }
}