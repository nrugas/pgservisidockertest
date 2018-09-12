using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using PG.Reports;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.upiti;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Prekrsaj = PG.Servisi.resources.podaci.upiti.Prekrsaj;

namespace PG.Servisi.resources.pdf
{
    class CreatePDF
    {
        public static string _tipJls = "", _naziv = "", _grb = "", _odlukaLisice = "";
        private static string _path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"resources\pdf\temp\";

        public static string Naredba(string grad, List<_Prekrsaj> _prekrsaji)
        {
            string path = Putanja(Path.Combine(_path, grad +".pdf"));

            Tuple<string, string> logo = Logo(2);//todo ako je od mupa?

            InstanceReportSource s = new InstanceReportSource();
            s.Parameters.Add("Header", string.Format("{0} {1} - {2}", _tipJls, _naziv, logo.Item2));
            s.Parameters.Add("Grb", string.Format("{0}", _grb));
            s.Parameters.Add("Logo", string.Format("{0}", logo.Item1));

            Naredba na = new Naredba();
            na.DataSource = Report(grad, _prekrsaji);

            s.ReportDocument = na;

            var reportProcessor = new ReportProcessor();
            var renderingResult = reportProcessor.RenderReport("PDF", s, new Hashtable());

            using (var destinationFileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                destinationFileStream.Write(renderingResult.DocumentBytes, 0, renderingResult.DocumentBytes.GetLength(0));
                destinationFileStream.Flush();
                destinationFileStream.Close();
            }

            return path;
        }

        public static Tuple<string, string> Logo(int idRedarstva)
        {
            try
            {
                string logo, tip;
                switch (idRedarstva)
                {
                    case 1:
                        logo = "logo_splash.png";
                        tip = "Prometno redarstvo";
                        break;
                    case 2:
                        logo = "logo.png";
                        tip = "Pauk služba";
                        break;
                    case 3:
                        logo = "parking.png";
                        tip = "Parking služba";
                        break;
                    case 4:
                        logo = "komunalno.png";
                        tip = "Komunalno redarstvo";
                        break;
                    case 5:
                        logo = "mup.png";
                        tip = "MUP";
                        break;
                    default:
                        logo = "";
                        tip = "";
                        break;
                }

                return new Tuple<string, string>(Environment.CurrentDirectory + @"\resources\images\backgrounds\" + logo, tip);
            }
            catch
            {
                return new Tuple<string, string>("", "");
            }
        }

        public static string Mjesto(string mjesto)
        {
            try
            {
                if (mjesto.ToUpper() == "SPLIT")
                {
                    return "Splitu";
                }

                if (mjesto.ToUpper() == "RIJEKA")
                {
                    return "Rijeci";
                }

                if (mjesto.ToUpper() == "CRIKVENICA")
                {
                    return "Crikvenici";
                }

                if (mjesto.ToUpper() == "TROGIR")
                {
                    return "Trogiru";
                }

                if (mjesto.ToUpper() == "NOVALJA")
                {
                    return "Novalji";
                }

                if (mjesto.ToUpper() == "PODGORA")
                {
                    return "Podgori";
                }

                if (mjesto.ToUpper() == "OPATIJA")
                {
                    return "Opatiji";
                }

                if (mjesto.ToUpper() == "HVAR")
                {
                    return "Hvaru";
                }

                if (mjesto.ToUpper() == "POREČ")
                {
                    return "Poreču";
                }

                if (mjesto.ToUpper() == "RI-ING NET")
                {
                    return "Rijeci";
                }

                return "________________";
            }
            catch (Exception)
            {
                return "________________";
            }
        }

        private static string Putanja(string putanjax, int x = 1)
        {
            if (File.Exists(putanjax))
            {
                var novaPutanja = putanjax.Replace(".pdf", "") + " (" + x + ").pdf";
                x++;
                return Putanja(novaPutanja, x);
            }

            return putanjax;
        }

        private static Stream PozicijaPrekrsaja(decimal latitude, decimal longitude, out string koordinate)
        {
            try
            {
                koordinate = Geocoordinates.DDtoDMS((double)latitude, Geocoordinates.CoordinateType.latitude) + ", " + Geocoordinates.DDtoDMS((double)longitude, Geocoordinates.CoordinateType.longitude);

                try
                {
                    WebRequest req = WebRequest.Create(Geocoordinates.PozicijaPrekrsajaGM(latitude.ToString().Replace(",", "."), longitude.ToString().Replace(",", ".")));
                    req.Timeout = 5000; //10sec
                    WebResponse response = req.GetResponse();
                    Stream stream = response.GetResponseStream();
                    return stream;
                }
                catch (Exception)
                {
                    WebRequest req = WebRequest.Create(Geocoordinates.PozicijaPrekrsajaOS(latitude.ToString().Replace(",", "."), longitude.ToString().Replace(",", ".")));
                    WebResponse response = req.GetResponse();
                    Stream stream = response.GetResponseStream();
                    return stream;
                }
            }
            catch (Exception ex)
            {
                koordinate = "";
                return null;
            }
        }

        private static List<Podaci> Report(string baza, List<_Prekrsaj> _prekrsaji)
        {
            #region PODACI
            List<Podaci> podaci = new List<Podaci>();

            foreach (var _prekrsaj in _prekrsaji)
            {
                #region SLIKE

                List<byte[]> slike = Prekrsaj.Slike(baza, _prekrsaj.IDLokacije, 1);

                List<Slika> foto = new List<Slika>();

                int x = 0;
                foreach (var sl in slike)
                {
                    foto.Add(new Slika(_prekrsaj.IDLokacije, x++, "", sl)); 
                }

                #endregion

                #region PAUK

                string kaznjivoPo = _prekrsaj.ClanakPrekrsaja + ", " + _prekrsaj.OpisPrekrsaja, pauk = "Izdana naredba za podizanje: NE";

                if (_prekrsaj.Pauk == true)
                {
                    if (_prekrsaj.Nalog.Lisice)
                    {
                        pauk = "Izdana naredba za blokiranje vozila: DA (" + _prekrsaj.Nalog.Status + ")";
                    }
                    else
                    {
                        pauk = "Izdana naredba za podizanje: DA (" + _prekrsaj.Nalog.Status + ")";
                    }
                }

                #endregion

                string republika = "", zupanija = "", grad = "", odjel = "", direkcija = "", diKada = "", naTemelju = "", naredba = "", clanakPauka = "", vozilo = "";

                if (_prekrsaj.Nalog != null)
                {
                    #region ZAGLAVLJE

                    XElement predlozak = Predlosci.PredlosciIspisa(baza, 0, 1).First(i => i.IDPredloska == _prekrsaj.IDDokumenta).TekstPredloska; //AdministracijaServiceHost._administracija.DohvatiPredlozak(Settings.Default.Baza, _prekrsaj.IDDokumenta).TekstPredloska;

                    republika = predlozak.Element("Republika").Value;
                    zupanija = predlozak.Element("Zupanija").Value;
                    grad = predlozak.Element("Grad").Value;
                    odjel = predlozak.Element("Odjel").Value;
                    direkcija = predlozak.Element("Direkcija").Value;

                    #endregion

                    #region NA_TEMELJU

                    if (_prekrsaj.IDRedarstva == 3)
                    {
                        naTemelju = "Na temelju " + _prekrsaj.ClanakPrekrsaja + ", izdaje se:";
                        clanakPauka = string.Format("OPIS: Vozilo je zatečeno u Gradu {0}, {1}, {2} ({3}).", Mjesto(_naziv), _prekrsaj.Adresa.Trim(','), _prekrsaj.OpisPrekrsaja, _prekrsaj.ClanakPrekrsaja);
                    }
                    else
                    {
                        if (_prekrsaj.Nalog.Lisice)
                        {
                            naTemelju = "Na temelju članka 5. stavak 1. točka 8. Zakona o sigurnosti prometa na cestama i" + _odlukaLisice + " izdaje se:";
                            clanakPauka = string.Format("OPIS: Vozilo je zatečeno parkirano u Gradu {0}, {1}, {2} ({3}).", Mjesto(_naziv), _prekrsaj.Adresa, _prekrsaj.OpisPrekrsaja, _prekrsaj.ClanakPrekrsaja);
                        }
                        else
                        {
                            naTemelju = "Na temelju članka 84. i 85. Zakona o sigurnosti prometa na cestama i članak 6. Pravilnika o uvjetima i načinu obavljanja poslova nadzora nepropisno zaustavljenih ili parkiranih vozila te uvjetima za obavljanje poslova premještanja nepropisno zaustavljenih ili parkiranih vozila, izdaje se:";
                            clanakPauka = string.Format("OPIS: Vozilo je zatečeno parkirano u Gradu {0}, {1}, {2} ({3}).", Mjesto(_naziv), _prekrsaj.Adresa, _prekrsaj.OpisPrekrsaja, _prekrsaj.ClanakPrekrsaja);
                        }
                    }

                    #endregion

                    naredba = _prekrsaj.Nalog.IDNaloga.ToString();
                    diKada = string.Format("U {0}, dana {1:dd.MM.yyyy u HH:mm} sati.", Mjesto(_naziv), _prekrsaj.DatumVrijeme);

                    #region VOZILO

                    _Vozilo voz = Vozila.Vozilo(baza, (int)_prekrsaj.Nalog.IDVozila, 1); //todo

                    if (voz != null)
                    {
                        vozilo = voz.NazivVozila + (voz.Registracija != "" ? " (" + voz.Registracija + ")" : "");
                    }

                    #endregion
                }

                Stream mapa = PozicijaPrekrsaja(_prekrsaj.Latitude, _prekrsaj.Longitude, out string koordinate);

                Podaci p = new Podaci(_prekrsaj.DatumVrijeme.ToString("dd.MM.yyyy u HH:mm"), _prekrsaj.Dokument, _prekrsaj.BrojDokumenta, _prekrsaj.Registracija, _prekrsaj.Kazna + " kn", _prekrsaj.Adresa, kaznjivoPo, pauk,
                    koordinate, (double)_prekrsaj.Latitude, (double)_prekrsaj.Longitude, republika, zupanija, grad, odjel, direkcija, diKada, naTemelju, naredba, clanakPauka, vozilo, _prekrsaj.BrojIskaznice, Slike.ReadFully(mapa), foto);

                podaci.Add(p);
            }

            Izvjestaj.Set(podaci);

            #endregion

            return podaci;
        }
    }
}
