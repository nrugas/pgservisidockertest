using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.cs.ispis;
using PG.Servisi.resources.podaci.baze;
using PG.Servisi.RESTApi;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Ispis
    {
        // TEST komentar zbog testiranja...
        public static bool IspisPredloska(string grad, string detaljiKazne, int qty, int idPredloska, int idJezika, out string Ispis, bool paymentinfo, int idAplikacije)
        {
            string NazivPredloska;

            int x = 70;
            int y = 1430; //1430

            const int height = 1500; //1500
            const string vert = "V";

            XElement TekstPredloska = Predlozak(grad, idPredloska, idJezika, idAplikacije, out NazivPredloska);

            if (NazivPredloska.Contains("#"))
            {
                return GO.Predlosci(grad, NazivPredloska, out Ispis, idAplikacije);
            }

            if (idJezika != 0)
            {
                detaljiKazne = Jezik.PromjeniJezikTeksta(grad, detaljiKazne, idJezika, idAplikacije);
            }

            if (TekstPredloska == null)
            {
                Ispis = "";
                return true;
            }

            #region DOHVAT TEKSTOVA

            const string grb =
                "000000000000003000000000000000000000004000007C00000C0000000000000000F00001B600003C00000000000000019C00037B0000760000000000000003060006FFC001C3000000000000000603801DFF600701800000000000000400E037FFB00E00C000020000C0000C00386FFFF8380060000E0001B80018000CDFFFF6600030007B00036E00300007BFFFFBC0001801CD0002FBC06000007FFFFF00000C0F7E8007FF70C01FFFFFFFFF0000063DFFC005FFDC81FFFFFFFFFF000003EFFF400FFFF70FFFFFF97D3E000001BFFFE00BFFFE7FFFFFF07E1E000001FFFFA017FFFFFFFFFFF07C0E07C201FFEFD037EFFFFFFFFFE07DAE0FFE03FE0FF82FE0FFFFFFFFC53806FFFC03FF07E87FC0FFFFFC00C038067FFF83FE01FC5F00FFFF0000C038067F3F03FC07F4FFC03FF00000E03C1E7E0007FE07FEBFC0FFC00000F0FF3EFE0067FF07FEBFC1FFC000007FFDFE7FFFE7FFBFFE5FDDFFE007FFFFABFE1FFFE1FFFFFE5FFFFFE0FFFFFF83FE3FFFC01FFFF46FFFFFEFFFFFFF81FC7FFFCC03FFFC2FFFFFFFFFFFFF2CFC3FFFDFC07FE817FFFFFFFFFFFF00FC3FFF9FF80FF817FFFFFFFFFFFF00FC3FFF9FFF83D01BFFFEFFFFFFFF01FC77FFDFFFE0700BFFFCFFFFFE7FC3FC6E3BBFFFFC2005FFF0FFFC007FFFFCEE733FFFFF6005FFC0FF80007FFFFCEEE73FF9FFC007E001F800007FFFFCDEEE07FFFFC002F007FC00007FFFFC1C0E00FFFF8001FE1FFC00003FFFFC0000701FFF80017FFFFC000020000800007F03FF0001FFFFFC000FFFFFFFE0007FE07F0000BFFFFE07FFC00007FFC0FFFC0E0000FFFFF9FF0000000001FF3FFF8600005FFF9FC0000000000007F3FFF400007FF9F8000000000000003F3FFC00002FDF800000000000000003E7F800003FF80000000000000000003EF80000178000000000000000000007D000000C00000000000000000000007000001FFFFFFFFFFFFFFFFFFFFFFFF0000010000000000000000000000010000017FFFFFFFFFFFFFFFFFFFFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFFFFFFFFFFFFFFFFFFFFD00000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE000050000017FFFF7FBFBFFFFBBFBDFFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFF00003FFFF80001FFFFD0000017FFFFFFFFFFFFFFFFFFFFFFD00000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000500000140000FFFFE0000FFFFE0000F000001E0000FFFFE0000FFFFE0000F000001E0000FFFFE0000FFFFE0000B000001A0000FFFFE0000FFFFE0000A000000A0000FFFFE0000FFFFE0001A000000B0000FFFFE0000FFFFE0001E000000D0000FFFFE0000FFFFE0001400000050000FFFFE0000FFFFE0003400000058000FFFFE0000FFFFE0003C00000068000FFFFE0000FFFFE000680000002C000FFFFE0000FFFFE0007800000034000FFFFE0000FFFFE0005000000016000FFFFE0000FFFFE000D00000001A000FFFFE0000FFFFE001B00000000B000FFFFE0000FFFFE001E00000000D000FFFFE0000FFFFE0034000000006800FFFFE0000FFFFE002C000000002FFFFFFFFFFFFFFFFFFFE80000000037FF00003FFFF80001FFD8000000001BFF00003FFFF80001FFB0000000000DFF00003FFFF80001FF600000000005FF00003FFFF80001FF400000000006FF00003FFFF80001FE8000000000037F00003FFFF80001FD800000000001BF00003FFFF80001FB000000000000DF00003FFFF80001F60000000000006780003FFFF80001EC0000000000001B00003FFFF80001980000000000000D80003FFFF80003600000000000000660003FFFF8000EC000000000000003B8003FFFF800198000000000000000CE003FFFF80077000000000000000077803FFFF801CC00000000000000001CE03FFFF807300000000000000000073C3FFFF83CE0000000000000000001CFBFFFF9E78000000000000000000079FFFFFF1E000000000000000000000F0FFFF0F00000000000000000000001F8001F8000000000000000000000001FFFF8000000000000";

            string Republika =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Republika", TekstPredloska), detaljiKazne);
            string Zupanija =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Zupanija", TekstPredloska), detaljiKazne);
            string Grad =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Grad", TekstPredloska), detaljiKazne);
            string Datum =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Datum", TekstPredloska), detaljiKazne);
            string BrojUpo =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$BrojUpozorenja", TekstPredloska), detaljiKazne);
            string Djela =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Djelatnik", TekstPredloska), detaljiKazne);
            string TekstPrekrsaja =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Prekrsaj", TekstPredloska), detaljiKazne);
            string Tekst1 =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Tekst1", TekstPredloska), detaljiKazne);
            string Tekst2 =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Tekst2", TekstPredloska), detaljiKazne);
            string Tekst3 =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Tekst3", TekstPredloska), detaljiKazne);
            string Odjel =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Odjel", TekstPredloska), detaljiKazne);
            string Direkcija =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Direkcija", TekstPredloska), detaljiKazne);
            string Naslov =
                ObradjivanjePodataka.ZamjenaKRzaVrijednosti(
                    ObradjivanjePodataka.TrazenjeDijelovaPredloska("$Naslov", TekstPredloska), detaljiKazne);

            #endregion

            #region PREDLOZAK

            string TekstNaplate = "";

            Regex r = new Regex(@"(,,).*");
            Match m = r.Match(TekstPrekrsaja);
            if (m.Success)
            {
                TekstPrekrsaja = TekstPrekrsaja.Replace(m.Value, ",");
                TekstNaplate = m.Value.Replace(",,", "");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("! 0 200 200 " + height + " " + qty + "\r\n");
            sb.Append("ON-FEED REPRINT\r\n");

            //BARKOD
            if (!string.IsNullOrEmpty(BrojUpo))
            {
                sb.Append(vert + "BARCODE 128 2 1 70 50 400 " + BrojUpo + "\r\n");
            }

            //REDAR
            sb.Append(vert + "TEXT " + Jezik.Fontovi(3) + " 0 120 400 " + Djela + "\r\n");

            //GRB
            sb.Append(vert + "EG 15 150 " + x + " " + (y += 10) + " " + grb + "\r\n");
            //REPUBLIKA
            sb.Append(vert + "TEXT " + Jezik.Fontovi(6) + " 0 " + (x -= 15) + " " + (y -= 140) + " " + Republika +
                      "\r\n");
            //ZUPANIJA
            sb.Append(vert + "TEXT " + Jezik.Fontovi(6) + " 0 " + (x += 30) + " " + y + " " + Zupanija + "\r\n");
            //GRAD
            sb.Append(vert + "TEXT " + Jezik.Fontovi(6) + " 0 " + (x += 30) + " " + y + " " + Grad + "\r\n");
            //ODJEL
            sb.Append(vert + "TEXT " + Jezik.Fontovi(2) + " 0 " + (x += 30) + " " + y + " " + Odjel + "\r\n");
            //DIREKCIJA
            sb.Append(vert + "TEXT " + Jezik.Fontovi(2) + " 0 " + (x += 30) + " " + y + " " + Direkcija + "\r\n");
            //DATUM
            sb.Append(vert + "TEXT " + Jezik.Fontovi(1) + " 0 " + (x += 30) + " " + y + " " + Datum + "\r\n");
            //NASLOV
            sb.Append(vert + "TEXT " + Jezik.Fontovi(4) + " 0 " + (x += 40) + " " + (y - 150) + " " + Naslov + "\r\n");
            //TEKST PREKRŠAJA
            x += 20;

            foreach (var c in ObradjivanjePodataka.WordWrap(TekstPrekrsaja, 1130))
            {
                sb.Append(vert + "TEXT " + Jezik.Fontovi(3) + " 0 " + (x += 30) + " 1440 " + c + "\r\n");
            }

            x += 10;
            sb.Append(vert + "TEXT " + Jezik.Fontovi(7) + " 0 " + (x += 20) + " 1100 " + TekstNaplate + "\r\n");

            x += 10;
            foreach (var c in ObradjivanjePodataka.WordWrap(Tekst1, 1130))
            {
                sb.Append(vert + "TEXT " + Jezik.Fontovi(3) + " 0 " + (x += 30) + " 1440 " + c + "\r\n");
            }

            x += 10;
            foreach (var c in ObradjivanjePodataka.WordWrap(Tekst2, 1130))
            {
                sb.Append(vert + "TEXT " + Jezik.Fontovi(3) + " 0 " + (x += 30) + " 1440 " + c + "\r\n");
            }

            x += 10;
            if (Naslov.StartsWith("OBAVIJEST"))
            {
                string[] Strano = Tekst3.Split('%');

                //sb.Append(vert + "TEXT " + Fontovi(3) + " 0 " + (x += 30) +
                //          " 1450 ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\r\n");
                //x += 10;

                foreach (var c in Strano)
                {
                    y = 1490;
                    foreach (var s in ObradjivanjePodataka.WordWrap(c, 1500))
                    {
                        sb.Append(vert + "TEXT 5 0 " + (x += 30) + " " + y + " " + s + "\r\n");
                        y = 1000;
                    }
                }
            }
            else
            {
                foreach (var c in ObradjivanjePodataka.WordWrap(Tekst3, 1130))
                {
                    sb.Append(vert + "TEXT " + Jezik.Fontovi(3) + " 0 " + (x += 30) + " 1440 " + c + "\r\n");
                }

                if (grad == "PROMETNIK_SPLIT")
                {
                    sb.Append(vert +
                              "TEXT 5 0 770 1440 tel. 021 682 576, fax. 021 682 577, e-mail: prometno.redarstvo@split.hr\r\n");
                }
            }

            sb.Append("PRINT\r\n");

            #endregion

            string pi = "";

            if (paymentinfo && Predlosci.Obavijest(grad, idPredloska, idAplikacije) == true)
            {
                XElement detalji = XElement.Parse(detaljiKazne);

                string kazna = detalji.Element("Kazna").Value;

                if (kazna == "")
                {
                    kazna = "0";
                }

                pi = PaymentInfo(grad, 0, Convert.ToDecimal(kazna), BrojUpo, 1, idAplikacije);
            }

            Ispis = ObradjivanjePodataka.MjenjanjeKvacica(sb) + pi;

            return true;
        }

        /*:: OBAVIJEST PAUKA ::*/

        public static string IspisObavijestiPauk(string grad, int idLokacije, string broj, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Prekrsaji p = db.Prekrsajis.First(i => i.IDLokacije == idLokacije);

                    try
                    {
                        string napomena = string.Format("IDPrekrsja: {0}, Broj dokumenta: {1}, Novi broj: {2}", p.IDPrekrsaja, p.BrojUpozorenja, broj);
                        Sustav.SpremiAkciju(grad, db.Zahtjevis.First(i => i.IDLokacije == idLokacije).IDPrijavitelja.Value, 57, napomena, 2,
                            idAplikacije);
                    }
                    catch (Exception ex)
                    {
                        Sustav.SpremiGresku(grad, ex, idAplikacije, "POKUŠAJ SPREMANJA AKCIJE - ISPIS OBAVIJESTI");
                    }

                    if (!string.IsNullOrEmpty(broj))
                    {
                        p.BrojUpozorenja = broj;
                        p.PozivNaBroj = broj;

                        db.SubmitChanges();
                    }

                    string ispis;
                    IspisPredloska(grad, ObavijestOPrekrsaju(Prekrsaj.DetaljiPrekrsaja(grad, idLokacije, idAplikacije)),
                        1, p.IDPredloskaIspisa.Value, 0, out ispis, string.IsNullOrEmpty(broj), idAplikacije);

                    return ispis;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ISPIS OBAVIJESTI PAUK");
                return null;
            }
        }



        public static string Predlozak(string grad, int idLokacije, int brKopija, int idJezika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    Prekrsaji p = db.Prekrsajis.First(i => i.IDLokacije == idLokacije);

                    IspisPredloska(grad, ObavijestOPrekrsaju(Prekrsaj.DetaljiPrekrsaja(grad, idLokacije, idAplikacije)), brKopija, p.IDPredloskaIspisa.Value, idJezika, out var ispis, false, idAplikacije);

                    return ispis;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ISPIS OBAVIJESTI");
                return null;
            }
        }

        /*:: DAN ::*/

        public static string IzvjestajSmjene(string grad, int idDjelatnika, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var nal = from n in db.NaloziPaukus
                              join p in db.Prekrsajis on n.IDNaloga equals p.IDNaloga
                              join s in db.StatusPaukas on n.IDStatusa equals s.IDStatusa
                              where n.IDDjelatnika == idDjelatnika &&
                                    n.DatumNaloga > DateTime.Now.Subtract(new TimeSpan(0, 12, 0, 0))
                              select new { n, p, s };

                    var zah = from n in db.Zahtjevis
                              where n.IDPrijaviteljaDjelatnik == idDjelatnika &&
                                    n.DatumVrijeme > DateTime.Now.Subtract(new TimeSpan(0, 12, 0, 0))
                              select new { n };

                    int y = 0;
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) + " IZVJEŠTAJ SMJENE");

                    y += 55;
                    sb.AppendLine("LEFT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " " +
                                  DateTime.Now.Subtract(new TimeSpan(0, 12, 0, 0)).ToString("dd.MM.yyyy"));

                    sb.AppendLine("RIGHT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " " +
                                  db.Djelatniks.First(i => i.IDDjelatnika == idDjelatnika).ImePrezime +
                                  "               ");

                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 20) +
                                  " ------------------------------------------------------------------------------------------------------------------------------------");

                    sb.AppendLine("LEFT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 40 " + (y += 25) + " Obrađenih naloga:");

                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 25) +
                                  " ------------------------------------------------------------------------------------------------------------------------------------");
                    sb.AppendLine("LEFT");

                    y += 20;
                    int rb = 1;
                    string autobus = "";

                    foreach (var n in nal)
                    {
                        if (grad.Contains("SPLIT"))
                        {
                            autobus = n.p.IDSkracenogOpisa == 22 ? " *" : "";
                        }

                        string txt = string.Format("{0:0#}.     {1}     {2}   -   {3:dd.MM.yy u HH:mm}   -   {4}{5}", rb++,
                            n.n.IDNaloga, n.p.RegistracijskaPlocica, n.n.DatumNaloga,
                            n.s.NazivStatusa, autobus);
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 40 " + (y += 25) + " " + txt);
                    }

                    string podignutihAS = "", pokusajaAS = "", ukupnoAS = "";
                    if (grad.Contains("SPLIT"))
                    {
                        ukupnoAS = " (Autobusna stanica*: " + nal.Count(i => i.p.IDSkracenogOpisa == 22) + ")";
                        podignutihAS = " (Autobusna stanica*: " +
                                       nal.Count(i => i.p.IDSkracenogOpisa == 22 && i.s.IDStatusa == 4) + ")";
                        pokusajaAS = " (Autobusna stanica*: " +
                                     nal.Count(i => i.p.IDSkracenogOpisa == 22 && i.s.IDStatusa == 3) + ")";
                    }

                    sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 40 " + (y += 45) + " Ukupno naloga: " + nal.Count() +
                                  ukupnoAS);
                    sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 40 " + (y += 25) + " Ukupno podignutih: " +
                                  nal.Count(i => i.s.IDStatusa == 4) + podignutihAS);
                    sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 40 " + (y += 25) + " Ukupno pokušaja: " +
                                  nal.Count(i => i.s.IDStatusa == 3) + pokusajaAS);

                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 40) +
                                  " ------------------------------------------------------------------------------------------------------------------------------------");
                    sb.AppendLine("LEFT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 40 " + (y += 25) +
                                  " Obrađenih zahtjeva za podizanjem:");

                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 25) +
                                  " ------------------------------------------------------------------------------------------------------------------------------------");
                    sb.AppendLine("LEFT");

                    rb = 1;
                    foreach (var n in zah)
                    {
                        if (grad.Contains("SPLIT"))
                        {
                            autobus = n.n.IDOpisa.Value == 22 ? " *" : "";
                        }

                        string txt = string.Format("{0:0#}.     {1}     {2}   -   {3:dd.MM.yy u HH:mm}   -   {4}{5}", rb++,
                            n.n.IDPrijave, n.n.Registracija, n.n.DatumVrijeme,
                            Zahtjev.Status(n.n.IDStatusa), autobus);
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 40 " + (y += 25) + " " + txt);
                    }

                    string odbijenihAS = "", odobrenihAS = "", zahtjevaAS = "";
                    if (grad.Contains("SPLIT"))
                    {
                        zahtjevaAS = " (Autobusna stanica*: " + zah.Count(i => i.n.IDOpisa == 22) + ")";
                        odobrenihAS = " (Autobusna stanica*: " + zah.Count(i => i.n.IDOpisa == 22 && i.n.IDStatusa == 3) +
                                      ")";
                        odbijenihAS = " (Autobusna stanica*: " + zah.Count(i => i.n.IDOpisa == 22 && i.n.IDStatusa != 3) +
                                      ")";
                    }

                    sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 40 " + (y += 45) + " Ukupno zahtjeva: " + zah.Count() +
                                  zahtjevaAS);
                    sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 40 " + (y += 25) + " Ukupno odobrenih: " +
                                  zah.Count(i => i.n.IDStatusa == 3) + odobrenihAS);
                    sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 40 " + (y += 25) + " Ukupno odbijenih: " +
                                  zah.Count(i => i.n.IDStatusa != 3) + odbijenihAS);

                    if (grad.Contains("SPLIT"))
                    {
                        sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 40 " + (y += 45) +
                                      "* - parkiran na djelu kolnika koji je kao stajalište za vozila javnog prijevoza");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 40 " + (y += 25) +
                                      "     putnika obilježen oznakama na kolniku ili prometnim znakom");
                    }

                    return "! 0 200 200 " + (y + 100) + " " + 1 + "\r\n" + ObradjivanjePodataka.MjenjanjeKvacica(sb) +
                           "\r\nPRINT\r\n";
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ISPIS IZVJEŠTAJA SMJENE");
                return "";
            }
        }

        /*:: RAČUN ::*/

        public static string Racun(string grad, _Racun racun, string registracija, string clanak, int idStatusa, DateTime datumNaloga, int idAplikacije)
        {
            try
            {
                _PoslovniProstor pp = PoslovniProstor.DohvatiPoslovniProstor(grad, racun.IDRedarstva, idAplikacije);

                if (pp == null)
                {
                    return "";
                }

                if (idStatusa == 1 || idStatusa == 2)
                {
                    Sustav.SpremiGresku(grad, new Exception("IDSTATUSA: " + idStatusa), idAplikacije, "ISPIS RACUNA");
                    idStatusa = 3;
                }

                _PostavkeIspisa p = pp.Postavke.First(i => i.IDStatusa == idStatusa && i.IDVrstePlacanja == racun.IDVrste);

                int y = 0;

                StringBuilder sb = new StringBuilder();

                #region HEADER

                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 10) + " " + pp.Naziv);
                if (!string.IsNullOrEmpty(pp.Podnaslov))
                {
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 45) + " " + pp.Podnaslov);
                }
                if (!string.IsNullOrEmpty(pp.USustavu))
                {
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 45) + " " + pp.USustavu);
                }
                if (!string.IsNullOrEmpty(pp.Web))
                {
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 45) + " " + pp.Web);
                }
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 30) + " " +
                              string.Format("{0} {1}, {2} {3}", pp.Ulica, pp.Broj, pp.Uplatnica.Posta, pp.Uplatnica.Mjesto));

                string kontakt = "";
                if (!string.IsNullOrEmpty(pp.Tel))
                {
                    kontakt = string.Format("Tel.: " + pp.Tel);
                }
                if (!string.IsNullOrEmpty(pp.Fax))
                {
                    kontakt += string.Format("; Fax.: " + pp.Fax);
                }
                if (!string.IsNullOrEmpty(pp.Email))
                {
                    kontakt += string.Format("; E-mail: " + pp.Email.Replace("@", "(at)"));
                }
                if (kontakt != "")
                {
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 30) + " " + kontakt);
                }
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 30) + " " + "OIB: " + pp.OIB);

                #endregion

                #region RACUN

                sb.AppendLine("TEXT " + Jezik.Fontovi(6) + " 0 0 " + (y += 40) + " Račun br: " + racun.BrojRacuna);

                y += 50;
                sb.AppendLine("LEFT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " " +
                              racun.DatumVrijeme.ToString("dd.MM.yy HH:mm:ss"));

                sb.AppendLine("RIGHT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " Operater: " + racun.Operater +
                              "               ");

                y += 60;
                sb.AppendLine("LEFT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " Opis");
                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " Količina");
                sb.AppendLine("RIGHT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " Iznos               ");

                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) +
                              " --------------------------------------------------------------------");

                foreach (var s in racun.Stavke)
                {
                    y += 50;
                    sb.AppendLine("LEFT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " " + s.OpisStavke);
                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " " + s.Kolicina);
                    sb.AppendLine("RIGHT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " " + s.Ukupno.ToString("n2") +
                                  "               ");
                }

                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) +
                              " --------------------------------------------------------------------");

                sb.AppendLine("RIGHT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 0 " + (y += 40) + " Osnovica: " +
                              racun.Osnovica.ToString("n2") + " kn" + "          ");

                sb.AppendLine("RIGHT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 0 " + (y += 40) + " PDV " + racun.PDVPosto + "%: " +
                              racun.PDV.ToString("n2") + " kn" + "          ");

                sb.AppendLine("RIGHT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 0 " + (y += 50) + " Ukupno: " + racun.Ukupno.ToString("n2") + " kn" + "          ");

                sb.AppendLine("LEFT");

                y += 20;
                string vlasnik = Osoba(racun.Osobe, true).Trim().TrimEnd(';').Trim();
                if (!string.IsNullOrEmpty(vlasnik))
                {
                    foreach (var q in ObradjivanjePodataka.LetterWrap(string.Format(" Vlasnik vozila: {0}", vlasnik), 72))
                    {
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 20) + " " + q);
                    }
                }

                y += 20;
                string preuzeo = Osoba(racun.Osobe, false).Trim().TrimEnd(';').Trim();
                if (!string.IsNullOrEmpty(preuzeo))
                {
                    foreach (var q in ObradjivanjePodataka.LetterWrap(string.Format("Vozilo preuzeo: {0}", preuzeo), 72))
                    {
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 20) + " " + q);
                    }
                }

                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " Način plaćanja: " + racun.NazivVrste);

                if (racun.IDVrste != 4 && racun.IDVrste != 5)
                {
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 50) + " ZKI: " + racun.ZKI);
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " JIR: " + racun.JIR);
                }

                #endregion

                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) + " --------------------------------------------------------------------");

                #region ZAPISNIK

                sb.AppendLine("CENTER");

                int z = 33;

                if (grad.ToUpper() == "PROMETNIK_RIJEKA")
                {
                    z = 35;
                }

                foreach (var q in ObradjivanjePodataka.LetterWrap(string.Format(p.Naslov, racun.IDReference, registracija), z))
                {
                    sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 40) + " " + q);
                }

                sb.AppendLine("LEFT");

                y += 45;

                try
                {
                    foreach (var q in ObradjivanjePodataka.LetterWrap(string.Format(p.Naredba, registracija, clanak), 90))
                    {
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
                    }
                }
                catch
                {
                    foreach (var q in ObradjivanjePodataka.LetterWrap(string.Format(p.Naredba, racun.IDReference, datumNaloga, registracija), 90))
                    {
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
                    }
                }

                y += 25;

                foreach (var q in ObradjivanjePodataka.LetterWrap(string.Format(p.Paragraf1, Math.Round(racun.Ukupno, 2), pp.Dosipijece, pp.Naziv, racun.DatumVrijeme), 90))
                {
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
                }

                y += 25;

                foreach (var q in ObradjivanjePodataka.LetterWrap(string.Format("{0} {1} {2}", p.Boldano, p.ZalbaRedarstva.Replace("@", "(at)"), p.Primjedba), 90))
                {
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
                }

                y += 70;
                sb.AppendLine("LEFT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " Operater: " + racun.Operater);

                sb.AppendLine("RIGHT");

                foreach (var q in ObradjivanjePodataka.LetterWrap(" Vozilo preuzeo: " + Preuzima(racun.Osobe) + "     ", 45))
                {
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 20) + " " + q);
                }

                y += 50;
                sb.AppendLine("LEFT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " _____________________________");

                sb.AppendLine("RIGHT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " _____________________________" + "               ");

                y += 20;
                sb.AppendLine("LEFT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 140 " + y + " potpis");

                sb.AppendLine("RIGHT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " potpis" + "                                             ");

                if ((racun.IDVrste == 5 || racun.IDVrste == 4) && (grad.ToUpper() == "PROMETNIK_RIJEKA" || grad == "Lokacije"))
                {
                    y += 50;
                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " Račun odobrio:");

                    y += 30;
                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " DIREKTOR");

                    y += 30;
                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " Željko Smojver");
                }

                sb.AppendLine("LEFT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 50) + " U " + p.Mjesto + ", dana  " + racun.DatumPreuzimanja.Value.ToString("dd.MM.yy"));

                if (!string.IsNullOrEmpty(p.TemeljniKapital))
                {
                    sb.AppendLine("CENTER");
                    foreach (string s in p.TemeljniKapital.Split(new[] { '%' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        y += 25;
                        foreach (var q in ObradjivanjePodataka.LetterWrap(s, 90))
                        {
                            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
                        }
                    }
                }

                #endregion

                if (racun.IDVrste == 4 || racun.IDVrste == 5)
                {
                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) +
                                  " --------------------------------------------------------------------");

                    #region PODACI ZA PLAĆANJE

                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 80) + " PODACI ZA PLAĆANJE ");

                    sb.AppendLine("LEFT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 80) +
                                  string.Format(" Primatelj: {0}, {1}, {2} {3}", pp.Uplatnica.Naziv, pp.Uplatnica.UlicaBroj, pp.Uplatnica.Posta, pp.Uplatnica.Mjesto));
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                                  string.Format(" Iznos: {0:n2} kn", racun.Ukupno));
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                                  string.Format(" IBAN: {0} ({1})", pp.Uplatnica.IBAN, pp.Banka).Replace(" ()", ""));
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                                  string.Format(" Model: {0}", pp.Uplatnica.Model));
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                                  string.Format(" SWIFT: {0}", pp.Uplatnica.Swift));
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                                  string.Format(" Poziv na broj: {0}", racun.PozivNaBr));
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                                  string.Format(" Opis: {0}", pp.Uplatnica.Opis));

                    #endregion

                    #region BARCODE

                    sb.AppendLine("B PDF-417 200 " + (y += 80) + " XD 2 YD 12 C 9 S 4");
                    sb.AppendLine("HRVHUB30");
                    sb.AppendLine("HRK");
                    sb.AppendLine(((int)(racun.Ukupno * 100)).ToString("000000000000000"));
                    //platitelj
                    sb.AppendLine("");
                    sb.AppendLine("");
                    sb.AppendLine("");
                    //primatelj
                    sb.AppendLine(pp.Uplatnica.Naziv);
                    sb.AppendLine(ObradjivanjePodataka.SkidanjeKvacica(string.Format("{0}", pp.Uplatnica.UlicaBroj).Trim(' ')).ToUpper());
                    sb.AppendLine(ObradjivanjePodataka.SkidanjeKvacica(pp.Uplatnica.Posta + " " + pp.Uplatnica.Mjesto).ToUpper());
                    sb.AppendLine(pp.Uplatnica.IBAN);
                    sb.AppendLine(pp.Uplatnica.Model);
                    sb.AppendLine(racun.PozivNaBr);
                    sb.AppendLine(""); //šifra namjene
                    sb.AppendLine(ObradjivanjePodataka.SkidanjeKvacica(pp.Uplatnica.Opis));
                    sb.AppendLine("ENDPDF");

                    #endregion

                    y = y + 130;

                    sb.AppendLine("LEFT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 80) +
                                  " Plaćanje možete izvršiti ispunjavanjem uplatnice pomoću \"PODATAKA ZA PLAĆANJE\" ili");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                                  " pomoću iznad ispisanog 2D barkoda kojim možete plaćanje izvršiti bez ispunavanja uplatnice.");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                                  " Plaćanje 2D barkodom možete izvršiti na prodajnom mjestu koje podržava takav oblik plaćanja,");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                                  " npr. u obližnjoj poslovnici FINE-e, u banci, moblinim bankarskim aplikacijama, na kioscima.");

                    if (p.Privola && racun.Osobe.Any())//privole na postavkama true i ima osobe
                    {
                        sb.AppendLine("CENTER");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) +
                                      " --------------------------------------------------------------------");
                        sb.AppendLine("CENTER");

                        sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 80) + " OBRAZAC PRIVOLE ZA ISPITANIKA ");

                        #region HRV

                        y = y + 50;
                        sb.AppendLine("LEFT");
                        foreach (var q in ObradjivanjePodataka.LetterWrap(string.Format("Ja svojim potpisom dajem privolu (suglasnost) da {0} može obrađivati moje osobne podatke u svrhu naplate računa i preuzimanja vozila.", pp.Naziv), 90))
                        {
                            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
                        }

                        y = y + 20;
                        sb.AppendLine("LEFT");
                        foreach (var q in ObradjivanjePodataka.LetterWrap(" Svjestan sam i obaviješten sam da, u bilo kojem trenutku, svoju privolu mogu povući putem: ", 91))
                        {
                            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
                        }

                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + "          - e-pošte: " + pp.Email.Replace("@", "(at)"));

                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + "          - poštom: " + string.Format("{0}, {1} {2}", pp.Uplatnica.UlicaBroj, pp.Uplatnica.Posta, pp.Uplatnica.Mjesto));

                        y = y + 50;

                        sb.AppendLine("RIGHT");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " " + PreuzimaPrivola(racun.Osobe) + "               ");
                        y += 30;

                        sb.AppendLine("RIGHT");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " _____________________________" + "               ");

                        y += 20;
                        sb.AppendLine("RIGHT");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " potpis" + "                                             ");

                        #endregion

                        #region ENG

                        //eng
                        sb.AppendLine("CENTER");

                        sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 80) + " CONSENT FORM ");

                        y = y + 50;
                        sb.AppendLine("LEFT");
                        foreach (var q in ObradjivanjePodataka.LetterWrap(string.Format(" I am herebywith my signature consenting that {0} can process my personal data for the purpose of issuing invoice and taking over the vehicle.", pp.Naziv), 90))
                        {
                            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
                        }

                        y = y + 20;
                        sb.AppendLine("LEFT");
                        foreach (var q in ObradjivanjePodataka.LetterWrap(" I am aware and I was informed that I may withdraw my consent at any time by using: ", 90))
                        {
                            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
                        }

                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + "          - e-mail: " + pp.Email.Replace("@", "(at)"));

                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + "          - postal service: " + string.Format("{0}, {1} {2}", pp.Uplatnica.UlicaBroj, pp.Uplatnica.Posta, pp.Uplatnica.Mjesto));

                        y = y + 50;

                        sb.AppendLine("RIGHT");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " " + PreuzimaPrivola(racun.Osobe) + "               ");
                        y += 30;

                        sb.AppendLine("RIGHT");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " _____________________________" + "               ");

                        y += 20;
                        sb.AppendLine("RIGHT");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " signature" + "                                             ");

                        #endregion
                    }
                }

                sb.AppendLine("PRINT");

                return "! 0 200 200 " + (y + 100) + " 1\r\nON-FEED REPRINT\r\n" +
                       ObradjivanjePodataka.MjenjanjeKvacica(sb);
            }
            catch (Exception ex)
            {
                string podaci = "REG: " + registracija + ", clanak: " + clanak + ", idStatusa: " + idStatusa;
                if (racun != null)
                {
                    podaci += ", idVrstePlacanja: " + racun.IDVrste;

                    if (racun.Stavke.Any())
                    {
                        podaci += ", brStavki: " + racun.Stavke.Count;
                    }
                }

                Sustav.SpremiGresku(grad, ex, idAplikacije, "ISPIS RACUNA - " + podaci);
                return "";
            }
        }

        /*:: DETALJI ::*/

        private static string PaymentInfo(string grad, int y, decimal iznos, string pozivnabroj, int idRedarstva, int idAplikacije)
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
                          string.Format(" Iznos: {0:n2} kn", iznos));
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + string.Format(" IBAN: {0}", up.IBAN));
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + string.Format(" Model: HR{0}", up.Model));
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
                          string.Format(" Poziv na broj: {0}-{1}-{2}", up.Poziv1, pozivnabroj, up.Poziv2));
            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + string.Format(" Opis: {0}", up.Opis));

            #endregion

            #region BARCODE

            sb.AppendLine("B PDF-417 200 " + (y += 80) + " XD 2 YD 12 C 9 S 4");
            sb.AppendLine("HRVHUB30");
            sb.AppendLine("HRK");
            sb.AppendLine(((int)(iznos * 100)).ToString("000000000000000"));
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
            sb.AppendLine(up.Poziv1 + "-" + pozivnabroj + "-" + up.Poziv2);
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

        private static string Privola(string grad, int y, string osoba, string tvrtka, string email, string adresa, int idRedarstva, int line, int idAplikacije)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("CENTER");
            sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) +
                          " --------------------------------------------------------------------");

            #region PODACI ZA PLAĆANJE

            sb.AppendLine("CENTER");

            sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 80) + " OBRAZAC PRIVOLE ZA ISPITANIKA ");

            y = y + 50;
            sb.AppendLine("LEFT");
            foreach (var q in ObradjivanjePodataka.LetterWrap(string.Format(" Ja, {0} na ovaj način dajem svoju privolu(suglasnost) da {1} može obrađivati moje osobne podatke u svrhu [odrediti svrhu obrade za koju se privola daje].", osoba, tvrtka), line)) //todo svrha
            {
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 30) + " " + q);
            }

            y = y + 20;
            sb.AppendLine("LEFT");
            foreach (var q in ObradjivanjePodataka.LetterWrap(string.Format(" Svjestan sam i obaviješten sam da, u bilo kojem trenutku, svoju privolu mogu povući putem \"OBRASCA ZA ODUSTAJANJA OD PRIVOLE ISPITANIKA\", putem e-pošte na {0} ili poštom na {1}.", email, adresa), line)) //todo svrha
            {
                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 30) + " " + q);
            }

            #endregion

            return "! 0 200 200 " + (y + 100) + " " + 1 + "\r\n" + ObradjivanjePodataka.MjenjanjeKvacica(sb) +
                   "\r\nPRINT\r\n";
        }

        /*:: PARKING ::*/

        static string bixolonLine = "".PadLeft(32, '-');

        public static string RacunParking(string grad, _Racun racun, bool info, int idAplikacije, int tipPrintera)
        {
            try
            {
                _PoslovniProstor pp = PoslovniProstor.DohvatiPoslovniProstor(grad, racun.IDRedarstva, idAplikacije);

                if (pp == null)
                {
                    return "";
                }

                int y = 0, fs = 2;

                StringBuilder sb = new StringBuilder();

                #region HEADER

                if (tipPrintera == 0) // ZEBRA
                {
                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " " + pp.Naziv);
                    if (!string.IsNullOrEmpty(pp.USustavu))
                    {
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 40) + " " + pp.USustavu);
                    }

                    if (!string.IsNullOrEmpty(pp.Web))
                    {
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 27) + " " + pp.Web);
                    }
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 27) + " " + string.Format("{0} {1} {2}", pp.Ulica, pp.Broj == "0" ? "" : pp.Broj, pp.Dodatak).Replace("  ", " "));
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 27) + " " + string.Format("{0} {1}", pp.Posta, pp.Mjesto));

                    string kontakt = "";
                    if (!string.IsNullOrEmpty(pp.Tel))
                    {
                        kontakt = string.Format("Tel.: " + pp.Tel);
                    }
                    if (!string.IsNullOrEmpty(pp.Fax))
                    {
                        kontakt += string.Format(" Fax.: " + pp.Fax);
                    }
                    if (kontakt != "")
                    {
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 27) + " " + kontakt);
                    }
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 27) + " " + "OIB: " + pp.OIB);
                }
                else
                {
                    ByteCollection bytes = new ByteCollection();
                    bytes.Add(BixolonUtils.PrintText(pp.Naziv, BixolonESC.ALIGNMENT_CENTER, 0, 0));
                    if (!string.IsNullOrEmpty(pp.USustavu))
                    {
                        bytes.Add(BixolonUtils.PrintText(pp.USustavu, BixolonESC.ALIGNMENT_CENTER, 0, 0));
                    }
                    bytes.Add(BixolonUtils.PrintText(pp.Web, BixolonESC.ALIGNMENT_CENTER, 0, 0));
                    bytes.Add(BixolonUtils.PrintText(string.Format("{0} {1}, {2} {3}", pp.Ulica, pp.Broj, pp.Posta, pp.Mjesto), BixolonESC.ALIGNMENT_CENTER, BixolonESC.TEXT_ATTRIBUTE_FONT_C, 0));

                    string kontakt = "";
                    if (!string.IsNullOrEmpty(pp.Tel))
                    {
                        kontakt = string.Format("Tel.: " + pp.Tel);
                    }
                    if (!string.IsNullOrEmpty(pp.Fax))
                    {
                        kontakt += string.Format(" Fax.: " + pp.Fax);
                    }
                    if (kontakt != "")
                    {
                        bytes.Add(BixolonUtils.PrintText(kontakt, BixolonESC.ALIGNMENT_CENTER, 0, 0));
                    }
                    bytes.Add(BixolonUtils.PrintText(" " + "OIB: " + pp.OIB, BixolonESC.ALIGNMENT_CENTER, 0, 0));
                    sb.AppendLine(Encoding.Default.GetString(bytes.GetBytes()));
                }

                #endregion

                #region RACUN
                if (tipPrintera == 0)
                {

                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 40) + " Račun br: " +
                                 racun.BrojRacuna);

                    y += 50;
                    sb.AppendLine("LEFT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " " +
                                  racun.DatumVrijeme.ToString("dd.MM.yy HH:mm:ss"));

                    sb.AppendLine("RIGHT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " Operater: " + racun.Operater + "   ");

                    y += 60;
                    sb.AppendLine("LEFT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " Opis");
                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " Količina");
                    sb.AppendLine("RIGHT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " Iznos" + "   ");

                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 15) +
                                  " ------------------------------------------------");

                    foreach (var s in racun.Stavke)
                    {
                        y += 30;
                        sb.AppendLine("LEFT");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " " + s.OpisStavke);
                        sb.AppendLine("CENTER");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " " + s.Kolicina);
                        sb.AppendLine("RIGHT");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " " + s.Ukupno.ToString("n2") +
                                      "   ");
                    }

                    sb.AppendLine("CENTER");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 30) +
                                  " ------------------------------------------------");

                    //sb.AppendLine("RIGHT");
                    //sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " " + fs + " 0 " + (y += 40) + " Osnovica: " + "7,5" + " kn");

                    //sb.AppendLine("RIGHT");
                    //sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " " + fs + " 0 " + (y += 35) + " PDV " + "25" + "%: " + "2,5" + " kn");

                    sb.AppendLine("RIGHT");
                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 40) + " UKUPNO: " +
                                  racun.Ukupno.ToString("n2") + " kn" + "   ");

                    sb.AppendLine("LEFT");

                    sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 50) + " Način plaćanja: " +
                                  racun.NazivVrste);

                    if (!string.IsNullOrEmpty(racun.Napomena))
                    {
                        sb.AppendLine("TEXT 7 " + fs + " 0 " + (y += 30) + " Registracija: " + racun.Napomena);
                    }
                    if (!string.IsNullOrEmpty(racun.ZKI))
                    {
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 70) + " ZKI: " + racun.ZKI);
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 30) + " JIR: " + racun.JIR);
                    }
                }
                else
                {
                    ByteCollection bytes = new ByteCollection();
                    bytes.Add(BixolonUtils.PrintText(" Račun br: " + racun.BrojRacuna, BixolonESC.ALIGNMENT_CENTER, BixolonESC.TEXT_ATTRIBUTE_EMPHASIZED, 0));
                    bytes.Add("\r\n");
                    bytes.Add(BixolonUtils.PrintText(racun.DatumVrijeme.ToString("dd.MM.yyyy HH:mm") + "\r", BixolonESC.ALIGNMENT_LEFT, 0, 0));
                    bytes.Add(BixolonUtils.PrintText(" Operater: " + racun.Operater, BixolonESC.ALIGNMENT_LEFT, 0, 0));

                    bytes.Add("\r\n");
                    //bytes.Add(BixolonUtils.PrintText("Opis", BixolonESC.ALIGNMENT_LEFT, 0, 0, false));
                    //bytes.Add(BixolonUtils.PrintText("Količina", BixolonESC.ALIGNMENT_CENTER, 0, 0, false));
                    bytes.Add(BixolonUtils.PrintText("Opis                  Kol  Iznos", BixolonESC.ALIGNMENT_LEFT, 0, 0));

                    bytes.Add(BixolonUtils.PrintText(bixolonLine, BixolonESC.ALIGNMENT_CENTER, 0, 0));


                    foreach (var s in racun.Stavke)
                    {
                        String iznos = s.Kolicina + "x" + s.Ukupno.ToString("n2").PadLeft(6);
                        String stavka = s.OpisStavke;
                        if (iznos.Length + stavka.Length <= bixolonLine.Length)
                        {
                            bytes.Add(BixolonUtils.PrintText(stavka.PadRight(bixolonLine.Length - iznos.Length) + iznos, BixolonESC.ALIGNMENT_LEFT, 0, 0, true));
                        }
                        else
                        {
                            bytes.Add(BixolonUtils.PrintText(stavka, BixolonESC.ALIGNMENT_LEFT, 0, 0, true));
                            bytes.Add(BixolonUtils.PrintText(iznos, BixolonESC.ALIGNMENT_RIGHT, 0, 0, true));
                        }
                        //bytes.Add(BixolonUtils.PrintText(" " + s.OpisStavke+ "\r", BixolonESC.ALIGNMENT_LEFT, 0, 0, true));
                        //bytes.Add(BixolonUtils.PrintText(" " + s.Kolicina.ToString()+"\r", BixolonESC.ALIGNMENT_CENTER, 0, 0, false));
                        //bytes.Add(BixolonUtils.PrintText(" " + s.Ukupno.ToString("n2") + "   ", BixolonESC.ALIGNMENT_RIGHT, 0, 0));

                    }

                    bytes.Add(BixolonUtils.PrintText(bixolonLine, BixolonESC.ALIGNMENT_CENTER, 0, 0));

                    bytes.Add(BixolonUtils.PrintText(" UKUPNO: " + racun.Ukupno.ToString("n2") + " kn", BixolonESC.ALIGNMENT_RIGHT, BixolonESC.TEXT_ATTRIBUTE_EMPHASIZED, 0));
                    bytes.Add("\r\n");
                    bytes.Add(BixolonUtils.PrintText(racun.NazivVrste, BixolonESC.ALIGNMENT_LEFT, 0, 0));

                    if (!string.IsNullOrEmpty(racun.ZKI))
                    {
                        bytes.Add(BixolonUtils.PrintText("ZKI:" + racun.ZKI, BixolonESC.ALIGNMENT_LEFT, BixolonESC.TEXT_ATTRIBUTE_FONT_C, 0));
                        bytes.Add(BixolonUtils.PrintText("JIR:" + racun.JIR, BixolonESC.ALIGNMENT_LEFT, BixolonESC.TEXT_ATTRIBUTE_FONT_C, 0));
                    }

                    if (!string.IsNullOrEmpty(racun.Napomena))
                    {
                        bytes.Add("\r\n");
                        bytes.Add(BixolonUtils.PrintText(racun.Napomena, BixolonESC.ALIGNMENT_CENTER, BixolonESC.TEXT_ATTRIBUTE_EMPHASIZED, BixolonESC.TEXT_SIZE_VERTICAL2 | BixolonESC.TEXT_SIZE_HORIZONTAL2));
                    }
                    sb.AppendLine(Encoding.Default.GetString(bytes.GetBytes()));
                }

                #endregion

                string pi = "";
                if (info)
                {
                    if (tipPrintera == 0) sb.AppendLine("POSTFEED 30");
                    else sb.AppendLine("\r\n");
                    pi = PaymentInfoParking(grad, racun.Ukupno, racun.PozivNaBr, racun.IDRedarstva, racun.Napomena,
                        idAplikacije, tipPrintera);
                }
                if (tipPrintera == 0)
                {
                    sb.AppendLine("PRINT");
                    return "! 0 200 200 " + (y + 50) + " 1\r\n" + ObradjivanjePodataka.SkidanjeKvacica(sb) + pi; //todo
                }
                else
                {
                    return ObradjivanjePodataka.SkidanjeKvacica(sb) + pi;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "ISPIS RACUNA PARKING");
                return "";
            }
        }

        private static string PaymentInfoParking(string grad, decimal iznos, string pozivnabroj, int idRedarstva, string registracija, int idAplikacije)
        {
            return PaymentInfoParking(grad, iznos, pozivnabroj, idRedarstva, registracija, idAplikacije, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grad"></param>
        /// <param name="iznos"></param>
        /// <param name="pozivnabroj"></param>
        /// <param name="idRedarstva"></param>
        /// <param name="registracija"></param>
        /// <param name="idAplikacije"></param>
        /// <param name="tipPrintera">0 = Zebra, 1 = Bixolon (ESC/P2)</param>
        /// <returns></returns>
        private static string PaymentInfoParking(string grad, decimal iznos, string pozivnabroj, int idRedarstva, string registracija, int idAplikacije, int tipPrintera)
        {
            _Uplatnica up = Gradovi.Uplatnica(grad, idRedarstva, idAplikacije);

            int y = 0;

            StringBuilder sb = new StringBuilder();
            if (tipPrintera == 0)
            {

                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + y + " ------------------------------------------------");

                #region PODACI ZA PLAĆANJE

                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + (y += 30) + " PODACI ZA PLAĆANJE ");

                sb.AppendLine("LEFT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + (y += 50) + " Primatelj: ");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + (y += 30) +
                              up.Naziv);
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + (y += 30) +
                              up.UlicaBroj);
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + (y += 30) +
                              string.Format("{0} {1}", up.Posta, up.Mjesto));

                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + (y += 30) +
                              string.Format(" Iznos: {0:n2} kn", iznos));
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + (y += 30) + string.Format(" IBAN: {0}", up.IBAN));
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + (y += 30) + string.Format(" Model: {0}", up.Model));
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + (y += 30) +
                              string.Format(" Poziv na broj: {0}", pozivnabroj));
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + (y += 30) + " SWIFT: " + up.Swift);
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 2 0 " + (y += 30) +
                              string.Format(" Opis: {0} ({1})", up.Opis, registracija).Replace(" ()", ""));

                #endregion

                #region BARCODE

                StringBuilder op = new StringBuilder();
                op.AppendLine("HRVHUB30");
                op.AppendLine("HRK");
                op.AppendLine(((int)(iznos * 100)).ToString("000000000000000"));
                //platitelj
                op.AppendLine("");
                op.AppendLine("");
                op.AppendLine("");
                //primatelj
                op.AppendLine(up.Naziv);
                op.AppendLine(
                    ObradjivanjePodataka.SkidanjeKvacica(string.Format("{0}", up.UlicaBroj).TrimEnd(' ')).ToUpper());
                op.AppendLine(ObradjivanjePodataka.SkidanjeKvacica(up.Posta + " " + up.Mjesto).ToUpper());
                op.AppendLine(up.IBAN);
                op.AppendLine(up.Model);
                op.AppendLine(pozivnabroj);
                op.AppendLine(""); //šifra namjene
                op.AppendLine(ObradjivanjePodataka.SkidanjeKvacica(up.Opis));

                sb.AppendLine("B PDF-417 40 " + (y += 50) + " XD 2 YD 6 C 5 R 0 S 6");
                sb.AppendLine(op.ToString());
                sb.AppendLine("ENDPDF");

                #endregion

            }
            else
            {

                sb.AppendLine(Encoding.Default.GetString(BixolonUtils.PrintText(bixolonLine, BixolonESC.ALIGNMENT_CENTER, 0, 0)));

                #region PODACI ZA PLAĆANJE


                sb.AppendLine(Encoding.Default.GetString(BixolonUtils.PrintText("", BixolonESC.ALIGNMENT_CENTER, 0, 0, false)));
                sb.Append(" PODACI ZA PLAĆANJE ");
                sb.AppendLine("\r\n");

                sb.AppendLine(Encoding.Default.GetString(BixolonUtils.PrintText(string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n", up.Naziv, up.UlicaBroj, up.Posta, up.Mjesto), BixolonESC.ALIGNMENT_CENTER, 0, 0)));

                sb.Append(Encoding.Default.GetString(BixolonUtils.PrintText(up.IBAN, BixolonESC.ALIGNMENT_CENTER, 0, 0)));

                sb.Append(Encoding.Default.GetString(BixolonUtils.PrintText(up.Model + " " + pozivnabroj, BixolonESC.ALIGNMENT_CENTER, 0, 0)));
                sb.Append(Encoding.Default.GetString(BixolonUtils.PrintText("SWIFT: " + up.Swift, BixolonESC.ALIGNMENT_CENTER, 0, 0)));

                sb.Append(Encoding.Default.GetString(BixolonUtils.PrintText(string.Format("{0} ({1})", up.Opis, registracija).Replace(" ()", ""), BixolonESC.ALIGNMENT_CENTER, BixolonESC.TEXT_ATTRIBUTE_FONT_C, 0)));
                sb.Append("\r\n");

                #endregion

                sb = new StringBuilder(ObradjivanjePodataka.SkidanjeKvacica(sb.ToString()));

                #region BARCODE

                StringBuilder op = new StringBuilder();
                op.AppendLine("HRVHUB30");
                op.AppendLine("HRK");
                op.AppendLine(((int)(iznos * 100)).ToString("000000000000000"));
                //platitelj
                op.AppendLine("");
                op.AppendLine("");
                op.AppendLine("");
                //primatelj
                op.AppendLine(up.Naziv);
                op.AppendLine();
                //ObradjivanjePodataka.SkidanjeKvacica(string.Format("{0}", up.UlicaBroj).TrimEnd(' ')).ToUpper());
                //op.AppendLine(ObradjivanjePodataka.SkidanjeKvacica(up.Posta + " " + up.Mjesto).ToUpper());
                op.AppendLine(ObradjivanjePodataka.SkidanjeKvacica(up.Mjesto).ToUpper());

                op.AppendLine(up.IBAN);
                op.AppendLine(up.Model);
                op.AppendLine(pozivnabroj);
                op.AppendLine(""); //šifra namjene
                op.AppendLine(ObradjivanjePodataka.SkidanjeKvacica("DPK")); // up.Opis));
                sb.AppendLine(Encoding.Default.GetString(BixolonUtils.getQrCode(Encoding.Default.GetBytes(op.ToString()), BixolonESC.QR_CODE_MODEL2, 6, BixolonESC.QR_CODE_ERROR_CORRECTION_LEVEL_L, 0)));
                sb.AppendLine("\r\n");
                sb.AppendLine("\r\n");
                //sb.AppendLine(Encoding.Default.GetString(BixolonUtils.getPdf417(Encoding.Default.GetBytes(op.ToString()), 3, 3)));
                #endregion
            }
            if (tipPrintera == 0)
            {
                return "! 0 200 200 " + (y + 200) + " " + 1 + "\r\n" + ObradjivanjePodataka.SkidanjeKvacica(sb) +
                       "\r\nPOSTFEED 100\r\nPRINT\r\n";
            }
            else
            {
                return sb.ToString();
            }
        }

        public static string IzvjestajSmjeneParking(string grad, int idKorisnika, int idRedarstva, DateTime datum, int idAplikacije, int tipPrintera)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    var nal =
                        db.RACUNIs.Where(
                            n =>
                                n.IDDjelatnika == idKorisnika && n.Datum.Date == datum.Date &&
                                n.IDRedarstva == idRedarstva);

                    int y = 0, fs = 2;
                    StringBuilder sb = new StringBuilder();
                    if (tipPrintera == 0)
                    {

                        sb.AppendLine("CENTER");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " IZVJEŠTAJ SMJENE");

                        y += 55;
                        sb.AppendLine("LEFT");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " " +
                                      DateTime.Now.ToString("dd.MM.yyyy u HH:mm"));

                        sb.AppendLine("RIGHT");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + y + " " +
                                      db.Djelatniks.First(i => i.IDDjelatnika == idKorisnika).ImeNaRacunu + "  ");

                        sb.AppendLine("CENTER");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 20) +
                                      " ----------------------------------------------");

                        sb.AppendLine("LEFT");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 40 " + (y += 25) + " Izdanih računa:");

                        sb.AppendLine("CENTER");
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                                      " ----------------------------------------------");
                        sb.AppendLine("LEFT");

                        y += 20;
                        int rb = 1;

                        foreach (var n in nal)
                        {
                            string storno = "";

                            if (n.Storniran)
                            {
                                storno = "{storno}";
                            }

                            string txt = string.Format("{0:0#}. {1} | {2:n2} kn | ({3:HH:mm}) {4}", rb++, n.BrojRacuna, n.Ukupno, n.Datum, storno);
                            sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " " + txt);
                        }

                        string ukupno = string.Format("Ukupno racuna: {0} ({1:n2} kn)", nal.Count(), nal.Sum(i => i.Ukupno));
                        sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 40 " + (y += 45) + " " + ukupno);

                        return "! 0 200 200 " + (y + 50) + " " + 1 + "\r\n" + ObradjivanjePodataka.SkidanjeKvacica(sb) +
                               "\r\nPOSTFEED 30\r\nPRINT\r\n";
                    }
                    else
                    {
                        ByteCollection b = new ByteCollection();
                        b.Add("\r\n");
                        b.Add(BixolonUtils.PrintText("IZVJESTAJ SMJENE", BixolonESC.ALIGNMENT_CENTER, BixolonESC.TEXT_ATTRIBUTE_UNDERLINE1, BixolonESC.TEXT_SIZE_VERTICAL2 | BixolonESC.TEXT_SIZE_HORIZONTAL2));
                        b.Add("\r\n");

                        var ime = ObradjivanjePodataka.SkidanjeKvacica(db.Djelatniks.First(i => i.IDDjelatnika == idKorisnika).ImeNaRacunu + "  ");

                        b.Add(BixolonUtils.PrintText(DateTime.Now.ToString("dd.MM.yyyy u HH:mm").PadRight(bixolonLine.Length - ime.Length) + ime, BixolonESC.ALIGNMENT_LEFT, 0, 0));
                        b.Add(BixolonUtils.PrintText(bixolonLine, BixolonESC.ALIGNMENT_LEFT, 0, 0));
                        b.Add(BixolonUtils.PrintText("       Izdanih racuna:", BixolonESC.ALIGNMENT_LEFT, 0, 0));
                        b.Add(BixolonUtils.PrintText(bixolonLine, BixolonESC.ALIGNMENT_LEFT, 0, 0));


                        int rb = 1;

                        foreach (var n in nal)
                        {
                            string storno = "";

                            if (n.Storniran)
                            {
                                storno = "{storno}";
                            }

                            string txt = string.Format("{0,2:0#}.{1,12} |{2,8:n2}kn {3} {4}", rb++, n.BrojRacuna, n.Ukupno, n.Datum.ToString("HH:mm"), storno);
                            b.Add(BixolonUtils.PrintText(txt, BixolonESC.ALIGNMENT_LEFT, BixolonESC.TEXT_ATTRIBUTE_FONT_B, 0));

                        }
                        b.Add(BixolonUtils.PrintText(bixolonLine, BixolonESC.ALIGNMENT_LEFT, 0, 0));
                        string ukupno = string.Format("Ukupno racuna: {0} ({1:n2} kn)", nal.Count(), nal.Sum(i => i.Ukupno));
                        b.Add(BixolonUtils.PrintText(ukupno, BixolonESC.ALIGNMENT_LEFT, BixolonESC.TEXT_ATTRIBUTE_EMPHASIZED, BixolonESC.TEXT_SIZE_VERTICAL1 | BixolonESC.TEXT_SIZE_HORIZONTAL1));
                        b.Add("\r\n");
                        b.Add("\r\n");

                        return Encoding.Default.GetString(b.GetBytes());
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "IZVJEŠTAJ SMJENE PARKING");
                return "";
            }
        }

        public static string VrijemeDolaska(int idGrada, int idSektora, string cijenaSat, string cijenaDnevne, string radnoVrijeme, int idAplikacije)
        {
            try
            {
                int y = 0, fs = 2;
                StringBuilder sb = new StringBuilder();

                DateTime dt = DateTime.Now;

                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 5) + " POTVRDA O VREMENU DOLASKA");
                sb.AppendLine("TEXT 7 " + fs + " 0 " + (y += 55) + " " + dt.ToString("dd.MM.yyyy"));
                sb.AppendLine("TEXT 7 " + fs + " 0 " + (y += 40) + " " + dt.ToString("HH:mm"));

                y += 60;

                sb.AppendLine("LEFT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + string.Format(" Cijena / price: 1h = {0}kn.", cijenaSat));

                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 45) +
                              " Pomoću ove potvrde vremena Vašeg dolaska vrši");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " se naplata parkiranja. Potvrdu je potrebno");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " čuvati do povratka u vozilo. Prije odlaska");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " potrebno je naplatničaru platiti parkiranje");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " ovisno o vremenu provedenom na parkiralištu.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " U slučaju gubitka ove potvrde biti će Vam");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " uručena dnevna parkirna karta. Ukoliko ne");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " izvršite plaćanje parkiranja do kraja radnog");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " vremena odnosno do 23:59 biti će Vam uručena");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " dnevna parkirna karta.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + string.Format(" Dnevna parkirna karta {0} kn.", cijenaDnevne));

                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 45) +
                              " This confirmation of your arrival time will be");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " used to charge your parking. The confirmation");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " should be kept until the return to the vehicle.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " Before leaving, it is necessary to pay for");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " parking, depending on time spent in the car park.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " In case of loss of this confirmation, daily");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " parking ticket will be charged. If you do not");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " make a parking payment by the end of working");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " hours (23:59), you will be charged with daily");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " parking ticket.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              string.Format(" Daily parking ticket: {0} kn.", cijenaDnevne));

                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 45) + " Preis des Parkplatzes : 1 Stunde = 7 Kn.");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Öffnungszeiten Parkplätze : 07 - 24 Stunden.");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 45) + " Mit dieser Bestätigung Ihrer Ankunftszeit auflädt");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Park getan. Das Zertifikat sollte bis zur Rückgabe");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " des Fahrzeugs gehalten werden. Bevor er ging, ist es");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " notwendig, für das Parken zahlen, abhängig von der");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Zeit auf dem Parkplatz verbracht. Bei Verlust dieser");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Urkunde wird täglich Parkticket vergeben. Wenn Sie ");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " einen Parkplatz Zahlung bis zum Ende der Arbeitszeit");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " oder auf 23:59 stellen wird Ihnen täglich Parkticket");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " geliefert werden.");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Tägliche Parkticket 100 kn.");

                y += 75;

                //BARKOD
                sb.AppendLine("BARCODE 128 1 2 20 0 0" + y + dt.ToString("dd.MM.yyyy HH:mm:ss"));

                sb.AppendLine("BARCODE QR 40 " + y + " M 2 U 8");
                sb.AppendLine("H4A," + dt.ToString("dd.MM.yyyy HH:mm:ss"));
                sb.AppendLine(idGrada.ToString());
                sb.AppendLine(idSektora.ToString());
                sb.AppendLine("ENDQR");

                y += 300;

                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " Radno vrijeme / Opening hours");
                sb.AppendFormat("TEXT 7 " + fs + " 0 " + (y += 35) + "  {0}", radnoVrijeme);

                return "! 0 200 200 " + (y + 100) + " " + 1 + "\r\n" + ObradjivanjePodataka.SkidanjeKvacica(sb) +
                       "\r\nPRINT\r\n";
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "VRIJEME DOLASKA - PARKING");
                return "";
            }
        }

        public static string VrijemeDolaskaESC(int idGrada, int idSektora, string cijenaSat, string cijenaDnevne, string radnoVrijeme, int idAplikacije)
        {
            try
            {
                int y = 0, fs = 2;
                ByteCollection i = new ByteCollection();

                StringBuilder sb = new StringBuilder();

                DateTime dt = DateTime.Now;
                i.Add(BixolonUtils.PrintText("POTVRDA O VREMENU DOLASKA", BixolonESC.ALIGNMENT_CENTER, 0, BixolonESC.TEXT_SIZE_VERTICAL2));
                i.Add("\r\n");
                i.Add(BixolonUtils.PrintText(dt.ToString("dd.MM.yyyy"), BixolonESC.ALIGNMENT_CENTER, BixolonESC.TEXT_ATTRIBUTE_EMPHASIZED, BixolonESC.TEXT_SIZE_VERTICAL2 | BixolonESC.TEXT_SIZE_HORIZONTAL2));
                i.Add(BixolonUtils.PrintText(dt.ToString("HH:mm"), BixolonESC.ALIGNMENT_CENTER, BixolonESC.TEXT_ATTRIBUTE_EMPHASIZED, BixolonESC.TEXT_SIZE_VERTICAL2 | BixolonESC.TEXT_SIZE_HORIZONTAL2));

                i.Add("\r\n");
                i.Add("\r\n");
                i.Add("\r\n");

                i.Add(
                    BixolonUtils.getQrCode(Encoding.Default.GetBytes(dt.ToString("dd.MM.yyyy HH:mm:ss") + "\r\n" + idGrada.ToString()), BixolonESC.QR_CODE_MODEL2, 8, 48, 0)
                );

                i.Add("\r\n");
                i.Add("\r\n");
                i.Add("\r\n");

                sb.AppendLine("Pomoću ove potvrde vremena Vašeg dolaska vrši se naplata parkiranja. Potvrdu je potrebno čuvati do povratka u vozilo. Prije odlaska potrebno je naplatničaru platiti parkiranje");
                sb.Append(" ovisno o vremenu provedenom na parkiralištu.");
                sb.AppendLine("U slučaju gubitka ove potvrde biti će Vam uručena dnevna parkirna karta.");
                sb.Append("Ukoliko ne izvršite plaćanje parkiranja do kraja radnog vremena odnosno do 23:59 biti će Vam uručena dnevna parkirna karta.");
                sb.AppendFormat("\r\nDnevna parkirna karta iznosi {0} kn.", cijenaDnevne);

                i.Add(BixolonUtils.PrintText(sb.ToString(), BixolonESC.ALIGNMENT_LEFT, 0, 0));

                i.Add("\r\n");
                i.Add("\r\n");
                i.Add("\r\n");


                return Encoding.Default.GetString(i.GetBytes());


                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 5) + " POTVRDA O VREMENU DOLASKA");
                sb.AppendLine("TEXT 7 " + fs + " 0 " + (y += 55) + " " + dt.ToString("dd.MM.yyyy"));
                sb.AppendLine("TEXT 7 " + fs + " 0 " + (y += 40) + " " + dt.ToString("HH:mm"));

                y += 60;

                sb.AppendLine("LEFT");
                sb.AppendFormat("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Cijena / price: 1h = {0}kn.", cijenaSat);

                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 10 0 " + (y += 45) +
                              " Pomoću ove potvrde vremena Vašeg dolaska vrši");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " 0 0 " + (y += 25) +
                              " se naplata parkiranja. Potvrdu je potrebno");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " čuvati do povratka u vozilo. Prije odlaska");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " potrebno je naplatničaru platiti parkiranje");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " ovisno o vremenu provedenom na parkiralištu.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " U slučaju gubitka ove potvrde biti će Vam");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " uručena dnevna parkirna karta. Ukoliko ne");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " izvršite plaćanje parkiranja do kraja radnog");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " vremena odnosno do 23:59 biti će Vam uručena");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " dnevna parkirna karta.");
                sb.AppendFormat("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Dnevna parkirna karta {0} kn.", cijenaDnevne);

                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 45) +
                              " This confirmation of your arrival time will be");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " used to charge your parking. The confirmation");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " should be kept until the return to the vehicle.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " Before leaving, it is necessary to pay for");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " parking, depending on time spent in the car park.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " In case of loss of this confirmation, daily");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " parking ticket will be charged. If you do not");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " make a parking payment by the end of working");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " hours (23:59), you will be charged with daily");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " parking ticket.");
                sb.AppendFormat("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " Daily parking ticket: {0} kn.", cijenaDnevne);

                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 45) + " Preis des Parkplatzes : 1 Stunde = 7 Kn.");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Öffnungszeiten Parkplätze : 07 - 24 Stunden.");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 45) + " Mit dieser Bestätigung Ihrer Ankunftszeit auflädt");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Park getan. Das Zertifikat sollte bis zur Rückgabe");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " des Fahrzeugs gehalten werden. Bevor er ging, ist es");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " notwendig, für das Parken zahlen, abhängig von der");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Zeit auf dem Parkplatz verbracht. Bei Verlust dieser");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Urkunde wird täglich Parkticket vergeben. Wenn Sie ");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " einen Parkplatz Zahlung bis zum Ende der Arbeitszeit");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " oder auf 23:59 stellen wird Ihnen täglich Parkticket");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " geliefert werden.");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Tägliche Parkticket 100 kn.");

                y += 75;

                //BARKOD
                sb.AppendLine("BARCODE 128 1 2 20 0 0" + y + dt.ToString("dd.MM.yyyy HH:mm:ss"));

                sb.AppendLine("BARCODE QR 40 " + y + " M 2 U 8");
                sb.AppendLine("H4A," + dt.ToString("dd.MM.yyyy HH:mm:ss"));
                sb.AppendLine(idGrada.ToString());
                sb.AppendLine(idSektora.ToString());
                sb.AppendLine("ENDQR");

                y += 300;

                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " Radno vrijeme / Opening hours");
                sb.AppendFormat("TEXT 7 " + fs + " 0 " + (y += 35) + "  {0}", radnoVrijeme);

                return "! 0 200 200 " + (y + 100) + " " + 1 + "\r\n" + ObradjivanjePodataka.SkidanjeKvacica(sb) +
                       "\r\nPRINT\r\n";
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "VRIJEME DOLASKA - PARKING");
                return "";
            }
        }

        public static string VrijemeDolaska(int idAplikacije)
        {
            try
            {
                int y = 0, fs = 2;
                StringBuilder sb = new StringBuilder();

                DateTime dt = DateTime.Now;

                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 5) + " POTVRDA O VREMENU DOLASKA");
                sb.AppendLine("TEXT 7 " + fs + " 0 " + (y += 55) + " " + dt.ToString("dd.MM.yyyy"));
                sb.AppendLine("TEXT 7 " + fs + " 0 " + (y += 40) + " " + dt.ToString("HH:mm"));

                y += 60;

                sb.AppendLine("LEFT");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Cijena / price: 1h = 7kn.");

                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 45) +
                              " Pomoću ove potvrde vremena Vašeg dolaska vrši");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " se naplata parkiranja. Potvrdu je potrebno");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " čuvati do povratka u vozilo. Prije odlaska");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " potrebno je naplatničaru platiti parkiranje");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " ovisno o vremenu provedenom na parkiralištu.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " U slučaju gubitka ove potvrde biti će Vam");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " uručena dnevna parkirna karta. Ukoliko ne");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " izvršite plaćanje parkiranja do kraja radnog");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " vremena odnosno do 23:59 biti će Vam uručena");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " dnevna parkirna karta.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Dnevna parkirna karta 70kn.");

                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 45) +
                              " This confirmation of your arrival time will be");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " used to charge your parking. The confirmation");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " should be kept until the return to the vehicle.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " Before leaving, it is necessary to pay for");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " parking, depending on time spent in the car park.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " In case of loss of this confirmation, daily");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " parking ticket will be charged. If you do not");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " make a parking payment by the end of working");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " hours (23:59), you will be charged with daily");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " parking ticket.");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " Daily parking ticket: 70 kn.");

                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 45) + " Preis des Parkplatzes : 1 Stunde = 7 Kn.");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Öffnungszeiten Parkplätze : 07 - 24 Stunden.");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 45) + " Mit dieser Bestätigung Ihrer Ankunftszeit auflädt");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Park getan. Das Zertifikat sollte bis zur Rückgabe");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " des Fahrzeugs gehalten werden. Bevor er ging, ist es");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " notwendig, für das Parken zahlen, abhängig von der");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Zeit auf dem Parkplatz verbracht. Bei Verlust dieser");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Urkunde wird täglich Parkticket vergeben. Wenn Sie ");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " einen Parkplatz Zahlung bis zum Ende der Arbeitszeit");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " oder auf 23:59 stellen wird Ihnen täglich Parkticket");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " geliefert werden.");
                //sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) + " Tägliche Parkticket 100 kn.");

                y += 75;

                //BARKOD
                sb.AppendLine("BARCODE 128 1 2 20 0 0" + y + dt.ToString("dd.MM.yyyy HH:mm:ss"));

                sb.AppendLine("BARCODE QR 80 " + y + " M 2 U 10");
                sb.AppendLine("L," + dt.ToString("dd.MM.yyyy HH:mm:ss"));
                sb.AppendLine("ENDQR");

                y += 230;

                sb.AppendLine("CENTER");
                sb.AppendLine("TEXT " + Jezik.Fontovi(0) + " " + fs + " 0 " + (y += 25) +
                              " Radno vrijeme / Opening hours");
                sb.AppendLine("TEXT 7 " + fs + " 0 " + (y += 35) + "  07 - 24");

                return "! 0 200 200 " + (y + 100) + " " + 1 + "\r\n" + ObradjivanjePodataka.SkidanjeKvacica(sb) +
                       "\r\nPRINT\r\n";
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "VRIJEME DOLASKA - PARKING");
                return "";
            }
        }

        /*:: POSTAVKE ::*/

        private static XElement Predlozak(string grad, int idPredloska, int idJezika, int idAplikacije, out string NazivPredloska)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    if (idJezika == 0)
                    {
                        var predlozak = db.PredlosciIspisas.First(i => i.IDPRedloska == idPredloska);

                        NazivPredloska = predlozak.NazivPredloska;
                        return predlozak.Predlozak;
                    }
                    else
                    {
                        var predlozak = from p in db.PrevedeniPredloscis
                                        where p.IDPredloska == idPredloska &&
                                              p.IDJezika == idJezika
                                        select p;

                        NazivPredloska = "";

                        if (!predlozak.Any())
                        {
                            return null;
                        }

                        return predlozak.First().Predlozak;
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "JEZIK PREDLOŠKA - NIJE PRONAĐEN");

                NazivPredloska = "";
                return null;
            }
        }

        private static string ObavijestOPrekrsaju(_Prekrsaj prekrsaj)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("<Elements>");
            sb.Append("<DanasnjiDatum>" + prekrsaj.DatumVrijeme + "</DanasnjiDatum>");
            sb.Append("<BrojUpozorenja>" + prekrsaj.BrojDokumenta + "</BrojUpozorenja>");
            sb.Append("<Datum>" + prekrsaj.DatumVrijeme.ToString("dd.MM.yy") + "</Datum>");
            sb.Append("<Vrijeme>" + prekrsaj.DatumVrijeme.ToString("HH:mm") + "</Vrijeme>");
            sb.Append("<Registracija>" + prekrsaj.Registracija + "</Registracija>");
            sb.Append("<Ulica>" + prekrsaj.Adresa + "</Ulica>");
            sb.Append("<KucniBroj></KucniBroj>");
            sb.Append("<Clanak>" + prekrsaj.ClanakPrekrsaja + "</Clanak>");
            sb.Append("<Kazna>" + prekrsaj.Kazna + "</Kazna>");
            sb.Append("<Djelatnik>" + prekrsaj.UID + "</Djelatnik>");
            sb.Append("<Prekrsaj>" + prekrsaj.OpisPrekrsaja + "</Prekrsaj>");
            sb.Append("</Elements>");

            return sb.ToString();
        }

        public static string Osoba(List<_Osoba> osobe, bool vlasnik)
        {
            try
            {
                if (!osobe.Any())
                {
                    return "";
                }

                //todo WTF
                if (vlasnik)
                {
                    if (osobe.Any(i => i.Vlasnik.Value && i.MUP == false))
                    {
                        _Osoba osoba = osobe.First(i => i.Vlasnik.Value);
                        return Preuzima(osoba);
                    }
                }
                else
                {
                    if (osobe.Any(i => i.Vlasnik == false && i.MUP == false))
                    {
                        _Osoba osoba = osobe.First(i => i.Vlasnik == false);
                        return Preuzima(osoba);
                    }
                }

                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string Preuzima(_Osoba osoba)
        {
            string preuzima = string.Format("{0} {1}", osoba.Ime, osoba.Prezime);

            if (osoba.Rodjen != null)
            {
                preuzima += string.Format(" ({0:dd.MM.yy})", osoba.Rodjen.Value);
            }

            if (osoba.Ulica != "")
            {
                preuzima +=
                    string.Format("; {0} {1}, {2} {3}", osoba.Ulica, osoba.KBr, osoba.Posta, osoba.Mjesto).TrimEnd(' ');
            }
            else
            {
                preuzima += "; " + osoba.Napomena.Trim();
            }

            if (osoba.OIB != "")
            {
                preuzima += string.Format("; OIB: {0}", osoba.OIB);
            }

            if (!string.IsNullOrEmpty(osoba.BrojDokumenta.Trim()))
            {
                preuzima += string.Format("; Br. Dokumenta: {0}", osoba.BrojDokumenta);
            }

            return Regex.Replace(preuzima, @"[\s+]", " ");
        }

        public static string Preuzima(List<_Osoba> osobe)
        {
            try
            {
                if (!osobe.Any())
                {
                    return "";
                }

                _Osoba osoba;

                if (osobe.Any(i => i.Vlasnik == false && i.MUP == false))
                {
                    osoba = osobe.First(i => i.Vlasnik == false && i.MUP == false);
                }
                else if(osobe.Any(i => i.MUP == false))
                {
                    osoba = osobe.First(i => i.MUP == false);
                }
                else
                {
                    osoba = null;
                }

                if (osoba != null)
                {
                    return string.Format("{0} {1} ({2})", osoba.Prezime, osoba.Ime, osoba.OIB).Replace(" ()", "");
                }

                return "            ";
                //_Osoba osoba = osobe.Any(i => i.Vlasnik == false) ? osobe.First(i => i.Vlasnik == false) : osobe.First();// todo && i.MUP == false
                //return string.Format("{0} {1} ({2})", osoba.Prezime, osoba.Ime, osoba.OIB).Replace(" ()", "");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string PreuzimaPrivola(List<_Osoba> osobe)
        {
            try
            {
                if (!osobe.Any())
                {
                    return "";
                }

                _Osoba osoba;

                if (osobe.Any(i => i.Vlasnik == false && i.MUP == false))
                {
                    osoba = osobe.First(i => i.Vlasnik == false && i.MUP == false);
                }
                else if (osobe.Any(i => i.MUP == false))
                {
                    osoba = osobe.First(i => i.MUP == false);
                }
                else
                {
                    osoba = null;
                }

                if (osoba != null)
                {
                    return string.Format("{0} {1} ({2})", osoba.Prezime, osoba.Ime, osoba.OIB).Replace(" ()", "");
                }

                return "";

                //_Osoba osoba = osobe.Any(i => i.Vlasnik == false) ? osobe.First(i => i.Vlasnik == false) : osobe.First();
                //return string.Format("{0} {1}", osoba.Prezime, osoba.Ime);
            }
            catch (Exception)
            {
                return "";
            }
        }

        //obrisi
        //public static string RacunOLD(string grad, _Racun racun, string registracija, string clanak, bool lisice, int idAplikacije)
        //{
        //    try
        //    {
        //        _PoslovniProstor pp = PoslovniProstor.DohvatiPoslovniProstor(grad, racun.IDRedarstva, idAplikacije);

        //        if (pp == null)
        //        {
        //            return "";
        //        }

        //        int y = 0;

        //        StringBuilder sb = new StringBuilder();

        //        #region HEADER

        //        sb.AppendLine("CENTER");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 10) + " " + pp.Naziv);
        //        if (!string.IsNullOrEmpty(pp.Podnaslov))
        //        {
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 45) + " " + pp.Podnaslov);
        //        }
        //        if (!string.IsNullOrEmpty(pp.USustavu))
        //        {
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 45) + " " + pp.USustavu);
        //        }
        //        if (!string.IsNullOrEmpty(pp.Web))
        //        {
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 30) + " " + pp.Web);
        //        }
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 30) + " " +
        //                      string.Format("{0} {1}, {2} {3}", pp.Ulica, pp.Broj, pp.Uplatnica.Posta, pp.Uplatnica.Mjesto));

        //        string kontakt = "";
        //        if (!string.IsNullOrEmpty(pp.Tel))
        //        {
        //            kontakt = string.Format("Tel.: " + pp.Tel);
        //        }
        //        if (!string.IsNullOrEmpty(pp.Fax))
        //        {
        //            kontakt += string.Format("; Fax.: " + pp.Fax);
        //        }
        //        if (!string.IsNullOrEmpty(pp.Email))
        //        {
        //            kontakt += string.Format("; E-mail: " + pp.Email.Replace("@", "(at)"));
        //        }
        //        if (kontakt != "")
        //        {
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 30) + " " + kontakt);
        //        }
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + (y += 30) + " " + "OIB: " + pp.OIB);

        //        #endregion

        //        #region RACUN

        //        sb.AppendLine("TEXT " + Jezik.Fontovi(6) + " 0 0 " + (y += 40) + " Račun br: " + racun.BrojRacuna);

        //        y += 50;
        //        sb.AppendLine("LEFT");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " " +
        //                      racun.DatumVrijeme.ToString("dd.MM.yy HH:mm:ss"));

        //        sb.AppendLine("RIGHT");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " Operater: " + racun.Operater +
        //                      "               ");

        //        y += 60;
        //        sb.AppendLine("LEFT");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " Opis");
        //        sb.AppendLine("CENTER");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " Količina");
        //        sb.AppendLine("RIGHT");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " Iznos               ");

        //        sb.AppendLine("CENTER");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) +
        //                      " --------------------------------------------------------------------");

        //        foreach (var s in racun.Stavke)
        //        {
        //            y += 50;
        //            sb.AppendLine("LEFT");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " " + s.OpisStavke);
        //            sb.AppendLine("CENTER");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " " + s.Kolicina);
        //            sb.AppendLine("RIGHT");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " " + s.Ukupno.ToString("n2") +
        //                          "               ");
        //        }

        //        sb.AppendLine("CENTER");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) +
        //                      " --------------------------------------------------------------------");

        //        sb.AppendLine("RIGHT");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 0 " + (y += 40) + " Osnovica: " +
        //                      racun.Osnovica.ToString("n2") + " kn" + "          ");

        //        sb.AppendLine("RIGHT");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 0 " + (y += 40) + " PDV " + racun.PDVPosto + "%: " +
        //                      racun.PDV.ToString("n2") + " kn" + "          ");

        //        sb.AppendLine("RIGHT");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(2) + " 0 0 " + (y += 50) + " Ukupno: " + racun.Ukupno.ToString("n2") + " kn" + "          ");

        //        sb.AppendLine("LEFT");

        //        y += 20;
        //        string vlasnik = Osoba(racun.Osobe, true).Trim().TrimEnd(';').Trim();
        //        if (!string.IsNullOrEmpty(vlasnik))
        //        {
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
        //                          string.Format(" Vlasnik vozila: {0}", vlasnik));
        //        }

        //        string preuzeo = Osoba(racun.Osobe, false).Trim().TrimEnd(';').Trim();
        //        if (!string.IsNullOrEmpty(preuzeo))
        //        {
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
        //                          string.Format(" Vozilo preuzeo: {0}", preuzeo));
        //        }

        //        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " Način plaćanja: " + racun.NazivVrste);

        //        if (racun.IDVrste != 4 && racun.IDVrste != 5)
        //        {
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 50) + " ZKI: " + racun.ZKI);
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " JIR: " + racun.JIR);
        //        }

        //        #endregion

        //        sb.AppendLine("CENTER");
        //        sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) +
        //                      " --------------------------------------------------------------------");

        //        #region ZAPISNIK

        //        sb.AppendLine("CENTER");

        //        if (lisice)
        //        {
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 80) + " ZAPISNIK O DEBLOKADI VOZILA");
        //        }
        //        else
        //        {
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 80) + " ZAPISNIK O POKUŠAJU PREMJEŠTANJA VOZILA");
        //        }

        //        sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 40) + " NALOG: " + racun.IDReference);
        //        sb.AppendLine("LEFT");

        //        y += 45;

        //        if (lisice)
        //        {
        //            string odluka = "";

        //            try
        //            {
        //                odluka = new PostavkeDataContext().GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad)).OdlukaLisice;
        //            }
        //            catch
        //            {
        //            }

        //            foreach (var q in ObradjivanjePodataka.LetterWrap("Na temelju članka 5. stavak 1. točka 8. Zakona o sigurnosti prometa na cestama i " + odluka + " izvršena je blokada vozila registarskih oznaka: " + registracija, 90))
        //            {
        //                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
        //            }

        //            y += 25;

        //            if (racun.IDVrste == 4)
        //            {
        //                foreach (
        //                    var q in
        //                    ObradjivanjePodataka.LetterWrap("Gore navedena stranka preuzima presliku ovog zapisnika te račun s uplatnicom, uz obvezu podmirenja troškova premještanja vozila u iznosu od " + Math.Round(racun.Ukupno, 2) + " kn u roku od " + pp.Dosipijece +
        //                        " dana od dana preuzimanja vozila sukladno čl. 86. st. 1. Zakona o sigurnosti prometa na cestama RH. Uplatu molimo izvršiti putem priložene opće uplatnice. U slučaju nepodmirenja novčane obveze u roku dospijeća po ovom računu, " +
        //                        pp.Naziv + " može zatražiti određivanje ovrhe na temelju vjerodostojne isprave.", 90))
        //                {
        //                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
        //                }

        //                y += 25;
        //            }

        //            string zalba;
        //            using (PostavkeDataContext db = new PostavkeDataContext())
        //            {
        //                zalba = db.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad)).ZalbaRedarstva;
        //            }

        //            foreach (var q in ObradjivanjePodataka.LetterWrap("Informacije o počinjenom prekršaju temeljem kojeg je naređeno blokiranje vozila možete dobiti kod izdavatelja naloga" + zalba.Replace("@", "(at)") +
        //                    ". Primjedbe na eventualna oštećenja navesti prilikom preuzimanja vozila. Naknadne primjedbe se ne  uvažavaju! Prigovor ne odlaže naplatu troškova premještanja vozila.", 90))
        //            {
        //                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
        //            }

        //            y += 70;
        //            sb.AppendLine("LEFT");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " Operater: " + racun.Operater);

        //            sb.AppendLine("RIGHT");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " Vozilo preuzeo: " + Preuzima(racun.Osobe) +
        //                          "               ");

        //            y += 50;
        //            sb.AppendLine("LEFT");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " _____________________________");

        //            sb.AppendLine("RIGHT");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " _____________________________" +
        //                          "               ");

        //            y += 20;
        //            sb.AppendLine("LEFT");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 140 " + y + " potpis");

        //            sb.AppendLine("RIGHT");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " potpis" + "                                             ");
        //        }
        //        else
        //        {
        //            foreach (
        //                var q in
        //                ObradjivanjePodataka.LetterWrap(
        //                    "Sukladno Zakonu o sigurnosti prometa na cestama (N.N. br. 67/2008, 48/2010 i 74/2011) izvršen je pokušaj premještanja vozila registarskih oznaka: " +
        //                    registracija + " temeljem - " + clanak, 90))
        //            {
        //                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
        //            }

        //            y += 25;

        //            string odlukaNaplate = "";
        //            //todo maknuti bolje riješiti - šta god
        //            try
        //            {
        //                if (Sistem.IDGrada(grad) == 5 || Sistem.IDGrada(grad) == 1)
        //                {
        //                    odlukaNaplate = new PostavkeDataContext().GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad)).ZalbaRedarstva.Trim('.').Trim();
        //                }
        //            }
        //            catch
        //            {
        //            }
        //            //nema vlasnika
        //            if (racun.IDVrste == 5)
        //            {
        //                foreach (var q in ObradjivanjePodataka.LetterWrap("Premještanje vozila nije bilo moguće te kako niste zatečeni na mjestu događaja sve primjedbe i žalbe možete podnijeti naknadno. Kontakti se nalaze u zaglavlju. Podnošenje primjedbi i žalbi ne odgađa obvezu plaćanja računa. " + odlukaNaplate, 90))
        //                {
        //                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
        //                }
        //            }
        //            else
        //            {
        //                if (racun.IDVrste == 4)
        //                {
        //                    foreach (var q in
        //                        ObradjivanjePodataka.LetterWrap("Gore navedena stranka preuzima presliku ovog zapisnika te račun s uplatnicom, uz obvezu podmirenja troškova premještanja vozila u iznosu od " + Math.Round(racun.Ukupno, 2) + " kn u roku od " + pp.Dosipijece +
        //                            " dana od dana preuzimanja vozila sukladno čl. 86. st. 1. Zakona o sigurnosti prometa na cestama RH. Uplatu molimo izvršiti putem priložene opće uplatnice. U slučaju nepodmirenja novčane obveze u roku dospijeća po ovom računu, " +
        //                            pp.Naziv + " može zatražiti određivanje ovrhe na temelju vjerodostojne isprave.", 90))
        //                    {
        //                        sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
        //                    }

        //                    y += 25;
        //                }

        //                string zalba;
        //                using (PostavkeDataContext db = new PostavkeDataContext())
        //                {
        //                    zalba = db.GRADOVIs.First(i => i.IDGrada == Sistem.IDGrada(grad)).ZalbaRedarstva;
        //                }

        //                foreach (var q in ObradjivanjePodataka.LetterWrap("Informacije o počinjenom prekršaju temeljem kojeg je naređeno premještanje vozila možete dobiti kod izdavatelja naloga" + zalba.Replace("@", "(at)") +
        //                        ". Primjedbe na eventualna oštećenja korisnik je dužan navesti prilikom preuzimanja vozila. Naknadne primjedbe se ne  uvažavaju! Prigovor ne odlaže naplatu troškova premještanja vozila.", 90))
        //                {
        //                    sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
        //                }

        //                y += 70;
        //                sb.AppendLine("LEFT");
        //                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " Operater: " + racun.Operater);

        //                sb.AppendLine("RIGHT");
        //                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " Vozilo preuzeo: " + Preuzima(racun.Osobe) +
        //                              "               ");

        //                y += 50;
        //                sb.AppendLine("LEFT");
        //                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + y + " _____________________________");

        //                sb.AppendLine("RIGHT");
        //                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " _____________________________" +
        //                              "               ");

        //                y += 20;
        //                sb.AppendLine("LEFT");
        //                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 140 " + y + " potpis");

        //                sb.AppendLine("RIGHT");
        //                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 0 " + y + " potpis" + "                                             ");
        //            }
        //        }

        //        //sb.AppendLine("LEFT");
        //        //sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 50) + " U " + Mjesto(pp.Mjesto) + ", dana  " + racun.DatumPreuzimanja.Value.ToString("dd.MM.yy"));

        //        //todo - temeljni kapital
        //        if (Sistem.IDGrada(grad) == 5 || Sistem.IDGrada(grad) == 1)
        //        {
        //            y += 25;
        //            foreach (var q in ObradjivanjePodataka.LetterWrap(
        //                "Društvo upisano u registar Trgovačkog suda u Rijeci pod brojem Tt-16/8538-2, Temeljni kapital iznosi 20.882.500,00 kn. Direktor: Željko Smojver",
        //                90))
        //            {
        //                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) + " " + q);
        //            }
        //        }

        //        #endregion

        //        if (racun.IDVrste == 4 || racun.IDVrste == 5)
        //        {
        //            sb.AppendLine("CENTER");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 15) +
        //                          " --------------------------------------------------------------------");

        //            #region PODACI ZA PLAĆANJE

        //            sb.AppendLine("CENTER");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(4) + " 0 0 " + (y += 80) + " PODACI ZA PLAĆANJE ");

        //            sb.AppendLine("LEFT");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 80) +
        //                          string.Format(" Primatelj: {0}, {1}, {2} {3}", pp.Uplatnica.Naziv, pp.Uplatnica.UlicaBroj, pp.Uplatnica.Posta, pp.Uplatnica.Mjesto));
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
        //                          string.Format(" Iznos: {0:n2} kn", racun.Ukupno));
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
        //                          string.Format(" IBAN: {0} ({1})", pp.Uplatnica.IBAN, pp.Banka).Replace(" ()", ""));
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
        //                          string.Format(" Model: {0}", pp.Uplatnica.Model));
        //            if (!string.IsNullOrEmpty(pp.Uplatnica.Swift))
        //            {
        //                sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
        //                              string.Format(" SWIFT: {0}", pp.Uplatnica.Swift));
        //            }
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
        //                          string.Format(" Poziv na broj: {0}", racun.PozivNaBr));
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
        //                          string.Format(" Opis: {0}", pp.Uplatnica.Opis));

        //            #endregion

        //            #region BARCODE

        //            sb.AppendLine("B PDF-417 200 " + (y += 80) + " XD 2 YD 12 C 9 S 4");
        //            sb.AppendLine("HRVHUB30");
        //            sb.AppendLine("HRK");
        //            sb.AppendLine(((int)(racun.Ukupno * 100)).ToString("000000000000000"));
        //            //platitelj
        //            sb.AppendLine("");
        //            sb.AppendLine("");
        //            sb.AppendLine("");
        //            //primatelj
        //            sb.AppendLine(pp.Naziv);
        //            sb.AppendLine(
        //                ObradjivanjePodataka.SkidanjeKvacica(
        //                    string.Format("{0}", pp.Uplatnica.UlicaBroj).Trim(' ')).ToUpper());
        //            sb.AppendLine(ObradjivanjePodataka.SkidanjeKvacica(pp.Uplatnica.Posta + " " + pp.Uplatnica.Mjesto).ToUpper());
        //            sb.AppendLine(pp.Uplatnica.IBAN);
        //            sb.AppendLine(pp.Uplatnica.Model);
        //            sb.AppendLine(racun.PozivNaBr);
        //            sb.AppendLine(""); //šifra namjene
        //            sb.AppendLine(ObradjivanjePodataka.SkidanjeKvacica(pp.Uplatnica.Opis));
        //            sb.AppendLine("ENDPDF");

        //            #endregion

        //            y = y + 130;

        //            sb.AppendLine("LEFT");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 80) +
        //                          " Plaćanje možete izvršiti ispunjavanjem uplatnice pomoću \"PODATAKA ZA PLAĆANJE\" ili");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
        //                          " pomoću iznad ispisanog 2D barkoda kojim možete plaćanje izvršiti bez ispunavanja uplatnice.");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
        //                          " Plaćanje 2D barkodom možete izvršiti naprodajnom mjestu koje podržava takav oblik plaćanja,");
        //            sb.AppendLine("TEXT " + Jezik.Fontovi(1) + " 0 50 " + (y += 30) +
        //                          " npr. u obližnjoj poslovnici FINE-e, u banci, moblinim bankarskim aplikacijama, na kioscima.");
        //        }

        //        sb.AppendLine("PRINT");

        //        //sb.Append("! 0 200 200 " + height + " " + qty + "\r\n");
        //        //sb.Append("ON-FEED REPRINT\r\n");

        //        return "! 0 200 200 " + (y + 100) + " 1\r\nON-FEED REPRINT\r\n" +
        //               ObradjivanjePodataka.MjenjanjeKvacica(sb);
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku(grad, ex, idAplikacije, "ISPIS RACUNA");
        //        return "";
        //    }
        //}
    }
}
