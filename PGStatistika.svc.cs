using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi
{
    [ServiceBehavior(MaxItemsInObjectGraph = int.MaxValue)]
    public class PGStatistika : IPGStatistika
    {
        private int idAplikacije = 10;

        public void Sime()
        {
            PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString("PROMETNIK_RIJEKA", 1));

            var c = from n in db.NaloziPaukus
                    where n.DatumNaloga.Date >= new DateTime(2018, 1, 1)
                    select n;

            int prva = c.Count(i =>
                i.DatumNaloga.TimeOfDay > new TimeSpan(0, 6, 30, 0) &&
                i.DatumNaloga.TimeOfDay < new TimeSpan(0, 14, 0, 0));

            int druga = c.Count(i =>
                i.DatumNaloga.TimeOfDay > new TimeSpan(0, 14, 0, 0) &&
                i.DatumNaloga.TimeOfDay < new TimeSpan(0, 21, 0, 0));

        }

        public string ProvjeraPoziva(string poziv)
        {
            return Sustav.ProvjeraPoziva("Lokacije", poziv, 70, 4, idAplikacije).Item2;
        }

        public string Kazni(string poziv)
        {
            _Opazanje opazanje = Parking.TraziOpazanje("Lokacije", 1816, idAplikacije);
            return Parking.NaplatiParking("Lokacije", opazanje, 29, 4, 1, poziv, 0, idAplikacije).Item3;
        }

        public string IspisKopijeRacunaParking()
        {
            return Parking.IspisKopijeRacunaParking("PROMETNIK_PODGORA", 223, 10, 0);
        }

        public bool Baza(out string error)
        {
            error = "";

            try
            {
                foreach (var g in new PostavkeDataContext().GRADOVIs)
                {
                    if (g.IDGrada == 1)
                    {
                        continue;
                    }

                    try
                    {
                        //PazigradDataContext pg = new PazigradDataContext(Sistem.ConnectionString(g.Baza, 4));

                        using (DbConnection connection = new SqlConnection(Sistem.ConnectionString(g.Baza, 4)))
                        {
                            connection.Open();

                            try
                            {
                                using (DbCommand command = new SqlCommand("ALTER TABLE Djelatnik ADD IDGO int NULL;"))
                                {
                                    command.Connection = connection;
                                    command.ExecuteNonQuery();
                                }
                            }
                            catch (Exception e)
                            {
                                error += g.Baza + " (PopisPrekrsaja); ";
                            }
                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE ODOBRENJA ADD VrijemeUnosa datetime NULL;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (PopisPrekrsaja); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE PopisPrekrsaja ADD IDRedarstva int NOT NULL Default 1;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (PopisPrekrsaja); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE PredlosciIspisa ADD Pauk bit NOT NULL Default 1;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (PredlosciIspisa - Pauk); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE PredlosciIspisa ADD Kaznjava bit NOT NULL Default 0;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (PredlosciIspisa - Kaznjava); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE Zahtjevi ADD IDredarstva int NULL;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (Zahtjevi); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("CREATE TABLE RACUNI_STORNA (IDStorna int not null, IDOriginala int not null, IDStorniranog int not null, IDStatusa int not null, IDDjelatnika int not null, Datum datetime not null, Napomena nvarchar(MAX) not null, Prilog image null, PrilogNaziv nvarchar(50) null);"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (POSLOVNI_SUBJEKTI PK); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE Lokacije ADD Punjac bit NULL;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (ODOBRENJA); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE ODOBRENJA ADD Deaktiviran datetime NULL;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (ODOBRENJA); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE ODOBRENJA ADD Drzava nvarchar(10) NULL;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (ODOBRENJA); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE Djelatnik ADD IDPoslovnogSubjekta int NULL;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (DJELATNIK); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("CREATE TABLE POSLOVNI_SUBJEKTI (IDPoslovnogSubjekta int not null, NazivSubjekta nvarchar(150) not null, OdgovornaOsoba nvarchar(150) not null, Mobitel nvarchar(150) not null, Email nvarchar(150) not null, OIB nvarchar(150) not null, Adresa nvarchar(150) not null);"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (POSLOVNI_SUBJEKTI PK); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE POSLOVNI_SUBJEKTI add primary key (IDPoslovnogSubjekta);"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (POSLOVNI_SUBJEKTI PK); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE BLAGAJNICKI_DNEVNIK_STAVKE add primary key (IDStavkeDnevnika);"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    error += g.Baza + " (BLAGAJNICKI_DNEVNIK_STAVKE PK); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE PARKING_ZONE DROP COLUMN GracePeriod;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    error += g.Baza + " (PARKING_ZONE ); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE PARKING_ZONE ADD GracePeriod int NULL;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    error += g.Baza + " (PARKING_ZONE ); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE PARKING_OPAZANJA ADD IDRacuna int NULL;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " ( PARKING_OPAZANJA); ";
                            //}

                            //Table<NAPLATNA_MJESTA> tableObj = pg.GetTable<NAPLATNA_MJESTA>();

                            //try
                            //{
                            //    tableObj.Any();
                            //}
                            //catch
                            //{
                            //    using (DbCommand command = new SqlCommand("CREATE TABLE NAPLATNA_MJESTA (IDNaplatnogMjesta int not null, IDPoslovnogProstora int not null, OznakaNaplatnogMjesta nvarchar(50) not null, Naziv nvarchar(150) not null, Sifra nvarchar(50) not null, Adresa nvarchar(MAX) not null, Posta nvarchar(150) not null, Mjesto nvarchar(150) not null, Glavno bit not null DEFAULT (0), Neaktivno bit not null  DEFAULT (0));"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}

                            //Table<BLAGAJNICKI_DNEVNIK> bd = pg.GetTable<BLAGAJNICKI_DNEVNIK>();

                            //try
                            //{
                            //   bd.Any();
                            //}
                            //catch
                            //{
                            //    try
                            //    {
                            //        using (DbCommand command = new SqlCommand("CREATE TABLE BLAGAJNICKI_DNEVNIK (IDDnevnika int NOT NULL, IDRedarstva int NOT NULL, IDDjelatnika int NOT NULL, Datum date NOT NULL, Polog decimal(18, 3) NOT NULL, Promet decimal(18, 3) NOT NULL ); "))
                            //        {
                            //            command.Connection = connection;
                            //            command.ExecuteNonQuery();
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        error += g.Baza + " (LAGAJNICKI_DNEVNIK); ";
                            //    }
                            //}

                            //Table<BLAGAJNICKI_DNEVNIK_STAVKE> bds = pg.GetTable<BLAGAJNICKI_DNEVNIK_STAVKE>();

                            //try
                            //{
                            //    bds.Any();
                            //}
                            //catch
                            //{
                            //    try
                            //    {
                            //        using (DbCommand command = new SqlCommand("CREATE TABLE BLAGAJNICKI_DNEVNIK_STAVKE (IDStavkeDnevnika int NOT NULL, IDDnevnika int NOT NULL, IDBanke int NULL, IDVrsteKartice int NULL, RB int NOT NULL, Tip nvarchar (50) NOT NULL, Opis nvarchar(max) NOT NULL, Primitak decimal(18, 3) NOT NULL, Izdatak decimal(18, 3) NOT NULL, Rate bit NOT NULL DEFAULT(0), Redosljed int NULL, NaplatnoMjesto nvarchar (max) NULL);"))
                            //        {
                            //            command.Connection = connection;
                            //            command.ExecuteNonQuery();
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        error += g.Baza + " (LAGAJNICKI_DNEVNIK_STAVKE); ";
                            //    }
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE Djelatnik ADD VPP bit NOT NULL Default 0;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (DJELATNIK); ";
                            //}

                            //try
                            //{
                            //    using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_ZAKLJUCENJA ADD Oznaka nvarchar(50) NULL;"))
                            //    {
                            //        command.Connection = connection;
                            //        command.ExecuteNonQuery();
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    error += g.Baza + " (RACUNI_ZAKLJUCENJA); ";
                            //}

                            //using (
                            //    DbCommand command = new SqlCommand("ALTER TABLE RACUNI_STAVKE_OPIS ADD KratkiOpis nvarchar(10) NULL;"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery();
                            //}

                            //using (
                            //    DbCommand command = new SqlCommand("ALTER TABLE Prekrsaji ADD Vrijeme datetime  NULL;"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery();
                            //}

                            //using (DbCommand command = new SqlCommand("ALTER TABLE POSLOVNI_PROSTOR ADD IDRedarstva int NOT NULL DEFAULT (2);"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery();
                            //}

                            //using (DbCommand command = new SqlCommand("ALTER TABLE Djelatnik ADD IDRedarstva int NULL;"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery(); 
                            //}
                            //using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_ZAKLJUCENJA ADD IDRedarstva int NOT NULL DEFAULT (2);"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery();
                            //}

                            //using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_OSOBE ALTER COLUMN KucniBroj nvarchar(MAX);"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery();
                            //}

                            //using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_OSOBE ALTER COLUMN Posta nvarchar(MAX);"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery();
                            //}

                            //using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_OSOBE ALTER COLUMN Mjesto nvarchar(MAX);"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery();
                            //}
                            //using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_OSOBE ALTER COLUMN OIB nvarchar(MAX);"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery();
                            //}


                            //using (DbCommand command = new SqlCommand("ALTER TABLE Zahtjevi ADD Trajanje int NULL;"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery();
                            //}

                            //todo
                            //using (DbCommand command = new SqlCommand("ALTER TABLE PostavkePrograma DROP COLUMN PazigradNaIzvjestaju;"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery();
                            //}
                            //using (DbCommand command = new SqlCommand("ALTER TABLE PostavkePrograma DROP COLUMN ZalbaPrometnom;"))
                            //{
                            //    command.Connection = connection;
                            //    command.ExecuteNonQuery();
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        error += g.Baza + " (OPCENITO);";
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, 5, "UREDI STATUSE");
                return false;
            }
        }

        public bool Podesi(out string error)
        {
            error = "";
            try
            {
                foreach (var g in new PostavkeDataContext().GRADOVIs)
                {
                    try
                    {
                        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(g.Baza, idAplikacije)))
                        {
                            PredlosciIspisa pi = db.PredlosciIspisas.First(i => i.NazivPredloska == "OBAVIJEST");
                            pi.Kaznjava = true;
                            db.SubmitChanges();

                            if (db.PredlosciIspisas.Count() > 2)
                            {
                                error += "Vise od 2 (" + g.Baza + "); ";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        error += g.Baza + " (OPCENITO);";
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, 5, "USKLADI PODATKE");
                return false;
            }
        }

        public string PostaviZaVpp()
        {
            string greske = "";
            try
            {
                using (PostavkeDataContext pb = new PostavkeDataContext())
                {
                    foreach (var g in pb.GRADOVIs)
                    {
                        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(g.Baza, idAplikacije)))
                        {
                            try
                            {
                                UPLATNICE u = pb.UPLATNICEs.First(i => i.IDGrada == Sistem.IDGrada(g.Baza) && i.IDRedarstva == 1);

                                if (u == null)
                                {
                                    greske += g.Baza + " (nema postavljen nalog)";
                                    continue;
                                }

                                if (db.NALOZI_Predlozaks.Any())
                                {
                                    NALOZI_Predlozak np = db.NALOZI_Predlozaks.First();

                                    np.Grad = u.Mjesto;
                                    np.Adresa = u.Adresa;
                                    np.Model = u.Model.Replace("HR", "");
                                    np.BrojRacuna = "";
                                    np.PozivNaBroj1 = u.Poziv1;
                                    np.PozivNaBroj2 = u.Poziv2;
                                    np.OpisPlacanja = u.Opis;
                                    np.Sifra = u.Sifra;
                                    np.IBAN = u.IBAN;

                                    db.SubmitChanges();
                                }
                                else
                                {
                                    NALOZI_Predlozak np = new NALOZI_Predlozak();

                                    np.IDPredloska = 1;
                                    np.Grad = u.Mjesto;
                                    np.Adresa = u.Adresa;
                                    np.Model = u.Model.Replace("HR", "");
                                    np.BrojRacuna = "";
                                    np.PozivNaBroj1 = u.Poziv1;
                                    np.PozivNaBroj2 = u.Poziv2;
                                    np.OpisPlacanja = u.Opis;
                                    np.Sifra = u.Sifra;
                                    np.IBAN = u.IBAN;

                                    db.NALOZI_Predlozaks.InsertOnSubmit(np);
                                    db.SubmitChanges();
                                }
                            }
                            catch (Exception e)
                            {
                                greske += g.Baza + " ("+ e.Message + ")";

                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, idAplikacije, "NALOZI_Predlozak");
            }

            return greske;
        }

        public void posaljiNalog()
        {
            MailLista.PosaljiNaredbu("Lokacije", 3469, 1);
        }

        //public bool Dostupan()
        //{
        //    return true;
        //}

        //public bool Placeni(string grad, DateTime datumProdaje, out string poruka)
        //{
        //    poruka = "";
        //    return true; //Prijenos.Placeni(grad, datumProdaje, out poruka, idAplikacije);
        //}

        //public bool Odgode(string grad, DateTime datumProdaje, out string poruka)
        //{
        //    poruka = "";
        //    return true; //return Prijenos.Odgode(grad, datumProdaje, out poruka, idAplikacije);
        //}

        //public void PostojiRCVozilo(string registracija, int IDLokacije)
        //{
        //    RentaCar.PostojiRCVozilo("Lokacije", registracija, IDLokacije, 10, idAplikacije);
        //}

        //public List<_Statistika> Prijavljen()
        //{
        //   return Statistika.Prijavljen("PROMETNIK_OSIJEK",16, idAplikacije);
        //}

        //public void uplatnice()
        //{
        //    try
        //    {
        //        //using (OLDDataContext db = new OLDDataContext())
        //        //{
        //        //    using (PostavkeDataContext dbn = new PostavkeDataContext())
        //        //    {
        //        //        foreach (var z in db.ZadnjaStranicas.GroupBy(i => i.IDGrada))
        //        //        {
        //        //            foreach (var x in z)
        //        //            {
        //        //                if (x.ZadnjaStranica1 == -1)
        //        //                {
        //        //                    continue;
        //        //                }

        //        //                if (x.IDGrada == 44 || x.IDGrada == 1)
        //        //                {
        //        //                    continue;
        //        //                }

        //        //                int idIznosa = 1;

        //        //                switch ((int)x.Kazna)
        //        //                {
        //        //                    case 300:
        //        //                        idIznosa = 1;
        //        //                        break;
        //        //                    case 500:
        //        //                        idIznosa = 2;
        //        //                        break;
        //        //                    case 700:
        //        //                        idIznosa = 3;
        //        //                        break;
        //        //                }

        //        //                decimal ispis = dbn.GRADOVIs.First(i => i.IDGrada == x.IDGrada.Value).IznosNaloga;

        //        //                DateTime zadnji = db.PovijestIspisas
        //        //                    .Where(i => i.IDGrada == x.IDGrada && i.Iznos == x.Kazna)
        //        //                    .OrderByDescending(i => i.Datum).First().Datum;

        //        //                _Povijest povijest = new _Povijest(0, x.IDGrada.Value, "", 1, "", idIznosa,  x.Kazna, (int)((x.ZadnjaStranica1 +1) / 3), zadnji , x.ZadnjaStranica1 + 1, ispis);

        //        //                Gradovi.SpremiPovijestIspisa(povijest, idAplikacije);
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //    }
        //    catch (Exception e)
        //    {


        //    }
        //}

        //public string PozivNaBroj(string grad, decimal ukupno, int idRedarstva)
        //{
        //    try
        //    {
        //        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
        //        {
        //            string increment = "00001";

        //            if (db.RACUNIs.Any(i => i.PozivNaBroj.Contains("-" + DateTime.Today.ToString("yy") + "9") && i.IDRedarstva == idRedarstva))
        //            {
        //                int id = db.RACUNIs.Count(i => i.PozivNaBroj.Contains("-" + DateTime.Today.ToString("yy") + "9") && i.IDRedarstva == idRedarstva) + 1;
        //                increment = id.ToString("00000");
        //            }

        //            _Uplatnica n = Gradovi.Uplatnica(grad, idRedarstva, idAplikacije);

        //            if (n == null)
        //            {
        //                return "";
        //            }

        //            string iznos = "000";

        //            try
        //            {
        //                PostavkeDataContext pdb = new PostavkeDataContext();

        //                if (pdb.UPLATNICE_IZNOSIs.Any(i => i.Iznos == ukupno))
        //                {
        //                    iznos = pdb.UPLATNICE_IZNOSIs.First(i => i.Iznos == ukupno).IDIznosa.ToString("000");
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                iznos = "000";
        //            }

        //            string idjls = n.IDGrada.ToString("000");

        //            string godina = DateTime.Today.ToString("yy");
        //            string rucno = "9";

        //            string prvidio = iznos + idjls + idRedarstva;
        //            string drugidio = godina + rucno + increment;

        //            string poziv;

        //            if (n.Model == "HR01")
        //            {
        //                poziv = prvidio + "-" + drugidio + KontrolniBroj.f_kontrolni("HUBM11", prvidio + drugidio, "", (prvidio + drugidio).Length);
        //            }
        //            else
        //            {
        //                poziv = prvidio + KontrolniBroj.f_kontrolni("HUBM11", prvidio, "", prvidio.Length) + "-" + drugidio + KontrolniBroj.f_kontrolni("HUBM11", drugidio, "", drugidio.Length);
        //            }

        //            return (n.Poziv1 + "-" + poziv + "-" + n.Poziv2).Trim('-');
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return "";
        //    }
        //}

        //public bool KopirajTerminale()
        //{
        //    try
        //    {
        //        using (PostavkeDataContext pdb = new PostavkeDataContext())
        //        {
        //            foreach (var g in pdb.GRADOVIs.Where(i => i.IDGrada == 1))
        //            {
        //                using (
        //                    PazigradDataContext db =
        //                        new PazigradDataContext(Sistem.ConnectionString(g.Baza, idAplikacije)))
        //                {
        //                    foreach (
        //                        var q in
        //                        db.Terminalis.Where(i => i.Aktivan).OrderByDescending(i => i.VrijemeZadnjegPristupa))
        //                    {
        //                        if (pdb.TERMINALIs.Any(i => i.IdentifikacijskiBroj == q.IdentifikacijskiBroj))
        //                        {
        //                            continue;
        //                        }

        //                        if (q.IDTerminala == 0)
        //                        {
        //                            continue;
        //                        }
        //                        TERMINALI tr = new TERMINALI();

        //                        int id = 1;
        //                        if (pdb.TERMINALIs.Any())
        //                        {
        //                            id = pdb.TERMINALIs.Max(i => i.IDTerminala) + 1;
        //                        }

        //                        tr.IDTerminala = id;
        //                        tr.IDGrada = g.IDGrada;
        //                        tr.IDRedarstva = q.Pauk == true ? 2 : 1;
        //                        tr.IDModela = 1; //
        //                        tr.IDOsa = 1; //
        //                        tr.NazivTerminala = q.NazivTerminala;
        //                        tr.IdentifikacijskiBroj = q.IdentifikacijskiBroj;
        //                        tr.Verzija = q.Verzija;
        //                        tr.VrijemeZadnjegPristupa = null;
        //                        tr.Aktivan = q.Aktivan;
        //                        tr.Parametri = null;
        //                        tr.ResetRequest = false;
        //                        tr.RestartRequest = false;
        //                        tr.ProgramExit = false;
        //                        tr.SelfDestruct = false;
        //                        tr.TerminalSuspend = false;
        //                        tr.InterniBroj = interni(q.NazivTerminala);
        //                        tr.SerijskiBroj = ""; //iz opreme po intrnom naknadno
        //                        tr.DatumDolaska = null;
        //                        tr.Jamstvo = null;
        //                        tr.JamstvoDo = null;
        //                        tr.Vlasnik = true;
        //                        tr.Firmware = q.Verzija; //iz verzije

        //                        pdb.TERMINALIs.InsertOnSubmit(tr);
        //                        pdb.SubmitChanges();
        //                    }
        //                }
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //public bool Trazi()
        //{
        //    try
        //    {
        //        using (PostavkeDataContext pdb = new PostavkeDataContext())
        //        {
        //            foreach (var g in pdb.GRADOVIs)
        //            {
        //                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(g.Baza, idAplikacije)))
        //                {
        //                    foreach (var q in db.Terminalis.Where(i => i.IdentifikacijskiBroj == "31BFD7AF8533D1105A86D2DBE5D76548B4920420"))
        //                    {

        //                    }
        //                }
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //public int interni(string naziv)
        //{
        //    try
        //    {
        //        string itn =
        //            naziv.Replace("RI", "")
        //                .Replace("ING", "")
        //                .Replace("NET", "")
        //                .Replace("Terminal", "")
        //                .Replace("_1 ", "")
        //                .Replace("_2 ", "")
        //                .Replace("_3 ", "")
        //                .Replace("_4 ", "")
        //                .Replace("_5 ", "")
        //                .Replace("_6 ", "")
        //                .Replace("_7 ", "")
        //                .Replace("_8 ", "")
        //                .Replace("_9 ", "")
        //                .Replace("_10 ", "")
        //                .Replace("-", "")
        //                .Replace("_", "")
        //                .Replace("(", "")
        //                .Replace(")", "")
        //                .Replace(" ", "");

        //        return Convert.ToInt32(itn);
        //    }
        //    catch (Exception)
        //    {
        //        return 0;
        //    }
        //}

        //public bool KopirajPodatkeTerminala()
        //{
        //    try
        //    {
        //        using (PostavkeDataContext pdb = new PostavkeDataContext())
        //        {
        //            foreach (TERMINALI tr in pdb.TERMINALIs)
        //            {
        //                //todo dohvati podatke za interni broj terminala

        //                tr.IDModela = 1; //
        //                tr.IDOsa = 1; //
        //                tr.SerijskiBroj = ""; //iz opreme po intrnom naknadno
        //                tr.DatumDolaska = null;
        //                tr.Jamstvo = null;
        //                tr.JamstvoDo = null;
        //                tr.Vlasnik = true;
        //                tr.Firmware = ""; //iz verzije

        //                pdb.SubmitChanges();
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //public bool UrediStatuse()
        //{
        //    try
        //    {
        //        //string error = null;
        //        foreach (var g in new PostavkeDataContext().GRADOVIs)
        //        {
        //            try
        //            {
        //                //PazigradDataContext pg = new PazigradDataContext(Sistem.ConnectionString(g.Baza, 4));

        //                //var ima = from p in pg.Prekrsajis
        //                //          where p.PozivNaBroj == "030373603752"
        //                //          select p;

        //                //if (ima.Any())
        //                //{

        //                //}

        //                using (DbConnection connection = new SqlConnection(Sistem.ConnectionString(g.Baza, 4)))
        //                {
        //                    connection.Open();
        //                    using (DbCommand command = new SqlCommand("ALTER TABLE Djelatnik ADD Blagajna bit NOT NULL DEFAULT (0);"))
        //                    {
        //                        command.Connection = connection;
        //                        command.ExecuteNonQuery();
        //                    }

        //                    //using (
        //                    //    DbCommand command = new SqlCommand("ALTER TABLE RACUNI_STAVKE_OPIS ADD KratkiOpis nvarchar(10) NULL;"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}

        //                    //using (
        //                    //    DbCommand command = new SqlCommand("ALTER TABLE Prekrsaji ADD Vrijeme datetime  NULL;"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}

        //                    //using (DbCommand command = new SqlCommand("ALTER TABLE POSLOVNI_PROSTOR ADD IDRedarstva int NOT NULL DEFAULT (2);"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}

        //                    //using (DbCommand command = new SqlCommand("ALTER TABLE Djelatnik ADD IDRedarstva int NULL;"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery(); 
        //                    //}
        //                    //using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_ZAKLJUCENJA ADD IDRedarstva int NOT NULL DEFAULT (2);"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}

        //                    //using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_OSOBE ALTER COLUMN KucniBroj nvarchar(MAX);"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}

        //                    //using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_OSOBE ALTER COLUMN Posta nvarchar(MAX);"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}

        //                    //using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_OSOBE ALTER COLUMN Mjesto nvarchar(MAX);"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}
        //                    //using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_OSOBE ALTER COLUMN OIB nvarchar(MAX);"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}


        //                    //using (DbCommand command = new SqlCommand("ALTER TABLE Zahtjevi ADD Trajanje int NULL;"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}

        //                    //todo
        //                    //using (DbCommand command = new SqlCommand("ALTER TABLE PostavkePrograma DROP COLUMN PazigradNaIzvjestaju;"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}
        //                    //using (DbCommand command = new SqlCommand("ALTER TABLE PostavkePrograma DROP COLUMN ZalbaPrometnom;"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}
        //                }
        //            }
        //            catch
        //            {
        //                //error += g.Baza + ";";
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku("", ex, 5, "UREDI STATUSE");
        //        return false;
        //    }
        //}

        //public string Pronadji()
        //{
        //    string g = "";

        //    try
        //    {
        //        using (PostavkeDataContext db = new PostavkeDataContext())
        //        {
        //            foreach (var grad in db.GRADOVIs)
        //            {
        //                using (PazigradDataContext pdb = new PazigradDataContext(Sistem.ConnectionString(grad.Baza, idAplikacije)))
        //                {
        //                    var pr = from p in pdb.Djelatniks
        //                             where p.IDDjelatnika == 51
        //                             select p;

        //                    if (pr.Any())
        //                    {
        //                        g += grad.NazivGrada;
        //                    }
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception)
        //    {

        //    }

        //    return g;
        //}

        //public void DodajRucniTerminal()
        //{
        //    Prekrsaj.DodajRucniTerminal("Lokacije", idAplikacije);
        //}

        //public int KopirajDatumID()
        //{
        //    int x = 0;
        //    try
        //    {
        //        using (PostavkeDataContext pdb = new PostavkeDataContext())
        //        {
        //            foreach (var g in pdb.GRADOVIs)
        //            {
        //                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(g.Baza, 4)))
        //                {
        //                    foreach (Prekrsaji p in db.Prekrsajis.Where(i => i.Vrijeme == null))
        //                    {
        //                        try
        //                        {
        //                            Lokacije l = db.Lokacijes.First(i => i.IDLokacije == p.IDLokacije);

        //                            p.Vrijeme = l.DatumVrijeme;
        //                            p.IDDjelatnika = l.IDDjelatnika;
        //                            p.IDOpisaZakona =
        //                                db.OpisiPrekrsajas.First(i => i.IDOpisa == p.IDSkracenogOpisa).IDNovog;

        //                            db.SubmitChanges();
        //                        }
        //                        catch (Exception)
        //                        {
        //                            x++;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }

        //    return x;
        //}

        //public bool? Prenesi()
        //{
        //    using (
        //        PazigradDataContext db =
        //            new PazigradDataContext(Sistem.ConnectionString("PROMETNIK_SPLIT", idAplikacije)))
        //    {

        //        var racuni = from r in db.RACUNIs
        //                     join ro in db.RACUNI_OSOBE_RELACIJEs on r.IDRacuna equals ro.IDRacuna
        //                     join o in db.RACUNI_OSOBEs on ro.IDOsobe equals o.IDOsobe
        //                     where r.Datum.Date == DateTime.Today.AddDays(-1) &&
        //                           (r.IDVrstePlacanja == 4 || r.IDVrstePlacanja == 5)
        //                     select new { r, o };

        //        // int x = racuni.Count();

        //    }

        //    //return Naplata.Prenesi("PROMETNIK_SPLIT", new DateTime(2016, 9, 28), idAplikacije);
        //    return true;
        //}

        //public string Pauk()
        //{
        //    StringBuilder tw = new StringBuilder();

        //    //try
        //    //{
        //    //    tw.AppendLine("Adresa; Naloga;");

        //    //    using (
        //    //        PazigradDataContext pdb =
        //    //            new PazigradDataContext(Sistem.ConnectionString("PROMETNIK_SPLIT", idAplikacije)))
        //    //    {
        //    //        var pr = from p in pdb.Prekrsajis
        //    //                 where p.NalogPauka == true && p.IDNaloga > 1 &&
        //    //                       p.Vrijeme.Value.Date < new DateTime(2017, 1, 1)
        //    //                 select p; //new _PaukStat((int) p.IDNaloga, p.Adresa);

        //    //        List<_PaukStat> ps = new List<_PaukStat>();

        //    //        foreach (var adr in pr)
        //    //        {
        //    //            ps.Add(new _PaukStat(adr.IDNaloga.Value, adr.IDSkracenogOpisa, adr.Adresa));
        //    //        }

        //    //        foreach (var a in ps.GroupBy(i => i.Adresa).OrderByDescending(i => i.Count()))
        //    //        {
        //    //            if (a.Count() < 3)
        //    //            {
        //    //                continue;
        //    //            }

        //    //            int ukupno = a.Count();
        //    //            string adresa = a.Key;

        //    //            List<string> statusi = new List<string>();
        //    //            List<string> zakoni = new List<string>();

        //    //            foreach (var id in a)
        //    //            {
        //    //                var st = from p in pdb.NaloziPaukus
        //    //                         join s in pdb.StatusPaukas on p.IDStatusa equals s.IDStatusa
        //    //                         where p.IDNaloga == id.IDNaloga
        //    //                         select s.NazivStatusa;

        //    //                statusi.Add(st.First());

        //    //                var za = from z in pdb.OpisiPrekrsajas
        //    //                         where z.IDOpisa == id.IDOpisa
        //    //                         select z.OpisPrekrsaja;

        //    //                zakoni.Add(za.First());
        //    //            }

        //    //            List<_2DLista> status = new List<_2DLista>();
        //    //            List<_2DLista> zakon = new List<_2DLista>();

        //    //            foreach (var st in statusi.GroupBy(i => i))
        //    //            {
        //    //                status.Add(new _2DLista(st.Count(), st.Key));
        //    //            }

        //    //            foreach (var st in zakoni.GroupBy(i => i))
        //    //            {
        //    //                zakon.Add(new _2DLista(st.Count(), st.Key));
        //    //            }

        //    //            tw.AppendLine(adresa + ";" + ukupno + ";");

        //    //            foreach (var s in status.OrderByDescending(i => i.Value))
        //    //            {
        //    //                tw.AppendLine(s.Text + ";" + s.Value + ";");
        //    //            }

        //    //            tw.AppendLine("");

        //    //            foreach (var s in zakon.OrderByDescending(i => i.Value))
        //    //            {
        //    //                tw.AppendLine(s.Text + ";" + s.Value + ";");
        //    //            }

        //    //            tw.AppendLine("---------------------------------------------------");
        //    //        }
        //    //    }
        //    //}
        //    //catch
        //    //{

        //    //}

        //    return tw.ToString();
        //}

        //public string GenerirajPozivNaBroj()
        //{
        //    return Prekrsaj.GenerirajPozivNaBroj("PROMETNIK_SPLIT", true, DateTime.Today, 500, 1);
        //}

        //public void Izmjena()
        //{
        //    using (
        //        PazigradDataContext db =
        //            new PazigradDataContext(Sistem.ConnectionString("PROMETNIK_SPLIT", idAplikacije)))
        //    {
        //        int ii = db.Prekrsajis.Count(i => i.PozivNaBroj.Length > 12);
        //        foreach (var p in db.Prekrsajis.Where(i => i.PozivNaBroj.Length > 12))
        //        {
        //            //Prekrsaji pr = db.Prekrsajis.First(i => i.IDPrekrsaja == p.IDPrekrsaja);
        //            p.PozivNaBroj = p.PozivNaBroj.TrimStart('0');
        //            p.BrojUpozorenja = p.PozivNaBroj.TrimStart('0');
        //            db.SubmitChanges();
        //        }
        //    }

        //}

        //List<_2DLista> ulice = new List<_2DLista>();
        //List<_2DLista> mjesta = new List<_2DLista>();
        //List<_2DLista> zakoni = new List<_2DLista>();
        //List<int> greska = new List<int>();

        //public void Procitaj()
        //{
        //    using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(@"C:\Users\danie\Downloads\sifrarnik.xlsx")))
        //    {
        //        var ulicexls = xlPackage.Workbook.Worksheets.First(); //select sheet here
        //        var totalRows = ulicexls.Dimension.End.Row;
        //        var totalColumns = ulicexls.Dimension.End.Column;

        //        for (int rowNum = 1; rowNum <= totalRows; rowNum++) //selet starting row here
        //        {
        //            var row =
        //                ulicexls.Cells rowNum, 1, rowNum, totalColumns].Select(
        //                    c => c.Value == null ? string.Empty : c.Value.ToString());

        //            int res;
        //            bool ok = int.TryParse(row.ElementAt(0), out res);

        //            string ulica;
        //            try
        //            {
        //                ulica = row.ElementAt(1);
        //            }
        //            catch (Exception)
        //            {
        //                continue;
        //            }

        //            if (ok)
        //            {
        //                ulice.Add(new _2DLista(Convert.ToInt32(res), ulica));
        //            }
        //        }

        //        var mjestaxls = xlPackage.Workbook.Worksheets.ElementAt(1); //select sheet here
        //        var mjestatotalRows = mjestaxls.Dimension.End.Row;
        //        var mjestatotalColumns = mjestaxls.Dimension.End.Column;

        //        for (int rowNum = 1; rowNum <= mjestatotalRows; rowNum++) //selet starting row here
        //        {
        //            var row =
        //                mjestaxls.Cells rowNum, 1, rowNum, mjestatotalColumns].Select(
        //                    c => c.Value == null ? string.Empty : c.Value.ToString());

        //            int res;
        //            bool ok = int.TryParse(row.ElementAt(0), out res);

        //            string mjesto;
        //            try
        //            {
        //                mjesto = row.ElementAt(1);
        //            }
        //            catch (Exception)
        //            {
        //                continue;
        //            }

        //            if (ok)
        //            {
        //                mjesta.Add(new _2DLista(Convert.ToInt32(res), mjesto));
        //            }
        //        }

        //        var zakonxls = xlPackage.Workbook.Worksheets.ElementAt(3); //select sheet here
        //        var zakontotalRows = zakonxls.Dimension.End.Row;
        //        var zakontotalColumns = zakonxls.Dimension.End.Column;

        //        for (int rowNum = 1; rowNum <= zakontotalRows; rowNum++) //selet starting row here
        //        {
        //            var row =
        //                zakonxls.Cells rowNum, 1, rowNum, zakontotalColumns].Select(
        //                    c => c.Value == null ? string.Empty : c.Value.ToString());

        //            int res;
        //            bool ok = int.TryParse(row.ElementAt(0), out res);

        //            string id;
        //            try
        //            {
        //                id = row.ElementAt(1);
        //            }
        //            catch (Exception)
        //            {
        //                continue;
        //            }

        //            if (ok)
        //            {
        //                zakoni.Add(new _2DLista(Convert.ToInt32(res), id));
        //            }
        //        }
        //    }

        //    List<_Prekrsaj> prekrsaji = new List<_Prekrsaj>();

        //    using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(@"C:\Users\danie\Downloads\prometno.xlsx")))
        //    {
        //        var prekxls = xlPackage.Workbook.Worksheets.First(); //select sheet here
        //        var totalRows = prekxls.Dimension.End.Row;
        //        var totalColumns = prekxls.Dimension.End.Column;

        //        for (int rowNum = 1; rowNum <= totalRows; rowNum++) //selet starting row here
        //        {
        //            try
        //            {
        //                var row =
        //                    prekxls.Cells rowNum, 1, rowNum, totalColumns].Select(
        //                        c => c.Value == null ? string.Empty : c.Value.ToString());

        //                int id = Convert.ToInt32(row.ElementAt(0)),
        //                    idOpisa = IDOpisa(Convert.ToInt32(row.ElementAt(8))),
        //                    idDjelatnika = Convert.ToInt32(row.ElementAt(17));

        //                DateTime datum = Convert.ToDateTime(row.ElementAt(12) + " " + row.ElementAt(13));

        //                //DateTime datum = new DateTime(d.Year, d.Month, d.Day);
        //                string registracija = row.ElementAt(3),
        //                    geocoding,
        //                    adresa = Adresa(row.ElementAt(14), row.ElementAt(15), row.ElementAt(16), out geocoding),
        //                    poziv = Convert.ToInt32(row.ElementAt(0)).ToString("000000"),
        //                    drzava = row.ElementAt(2),
        //                    kazna = row.ElementAt(9),
        //                    klasa = row.ElementAt(22),
        //                    urb = row.ElementAt(23);

        //                decimal lat, lng;
        //                Geocoding(geocoding, out lat, out lng);

        //                if (idOpisa == -1)
        //                {
        //                    greska.Add(id);
        //                }
        //                else
        //                {
        //                    //prekrsaji.Add(new _Prekrsaj(id, 1, idOpisa, 0, 0, idDjelatnika, 2, null, lat, lng, datum, registracija, "", "", "", adresa, poziv,
        //                    //    "", "", "", "", "", "", "", "", kazna, false, false, false, false, "", "", "", null, "", klasa, urb, drzava, null));
        //                }

        //                if (prekrsaji.Count % 50 == 0)
        //                {
        //                    Thread.Sleep(1000);
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                continue;
        //            }
        //        }
        //    }

        //    foreach (var p in prekrsaji)
        //    {
        //        int id = Prekrsaj.DodajRucniPrekrsaj("PROMETNIK_PREKO", p, true, new List<byte ]>(), 1, false, 1);

        //        if (id == -1)
        //        {
        //            greska.Add(p.IDPrekrsaja);
        //        }
        //    }
        //}

        //public string Adresa(string idMjesta, string idUlice, string kbr, out string geocoding)
        //{
        //    string mjesto = "", m = "Preko", kbr1 = "";

        //    if (!string.IsNullOrEmpty(idMjesta) && idMjesta != "NULL")
        //    {
        //        if (Convert.ToInt32(idMjesta) != 159)
        //        {
        //            mjesto = string.Format(" ({0})", mjesta.First(i => i.Value == Convert.ToInt32(idMjesta)).Text);
        //            m = mjesta.First(i => i.Value == Convert.ToInt32(idMjesta)).Text;
        //        }
        //    }

        //    if (idUlice == "NULL")
        //    {
        //        geocoding = m + ", Croatia";
        //        return "Preko";
        //    }

        //    if (kbr != "0")
        //    {
        //        kbr1 = " u blizini kućnog broja " + kbr;
        //    }

        //    geocoding = ulice.First(i => i.Value == Convert.ToInt32(idUlice)).Text + ", " + m + ", Croatia";
        //    return string.Format("na ulici {0}{1}{2}", ulice.First(i => i.Value == Convert.ToInt32(idUlice)).Text,
        //        kbr1, mjesto);
        //}

        //public int IDOpisa(int idZakona)
        //{
        //    try
        //    {
        //        return Convert.ToInt32(zakoni.First(i => i.Value == idZakona).Text);
        //    }
        //    catch (Exception)
        //    {
        //        return -1;
        //    }
        //}

        //public void Geocoding(string adresa, out decimal lat, out decimal lng)
        //{
        //    //lat = (decimal)44.080452;
        //    //lng = (decimal)15.187538;
        //    //return;
        //    try
        //    {
        //        adresa = adresa.Replace("CENTAR", "").TrimStart(' ', ',');

        //        if (adresa == "Trg Hrvatske nezavisnosti, Preko, Croatia")
        //        {
        //            lat = (decimal)44.081134;
        //            lng = (decimal)15.187923;
        //            return;
        //        }

        //        if (adresa == "Trajektno pristanište, Preko, Croatia")
        //        {
        //            lat = (decimal)44.076339;
        //            lng = (decimal)15.194259;
        //            return;
        //        }

        //        var requestUri =
        //            string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", adresa);

        //        var request = WebRequest.Create(requestUri);
        //        var response = request.GetResponse();
        //        var xdoc = XDocument.Load(response.GetResponseStream());

        //        var result = xdoc.Element("GeocodeResponse").Element("result");
        //        var locationElement = result.Element("geometry").Element("location");
        //        lat = (decimal)locationElement.Element("lat");
        //        lng = (decimal)locationElement.Element("lng");
        //    }
        //    catch (Exception)
        //    {
        //        lat = (decimal)44.080452;
        //        lng = (decimal)15.187538;
        //    }
        //}

        //public string Slike()
        //{
        //    string id = "";
        //    try
        //    {
        //        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString("PROMETNIK_PREKO", idAplikacije)))
        //        {
        //            //string filepath = Environment.GetFolderPath();
        //            DirectoryInfo d = new DirectoryInfo(@"D:\Backup\Office 2016\Preko slike\");

        //            // int x = d.GetFiles().Count();

        //            foreach (var file in d.GetFiles())
        //            {
        //                try
        //                {
        //                    string pnb = ""; //1003.ToString("000000");//todo

        //                    int index = file.Name.IndexOf("_");
        //                    if (index > 0)
        //                        pnb = Convert.ToInt32(file.Name.Substring(0, index)).ToString("000000");

        //                    if (pnb == "")
        //                    {
        //                        id += file.Name;
        //                        continue;
        //                    }

        //                    Prekrsaji prek = db.Prekrsajis.First(i => i.PozivNaBroj == pnb);
        //                    string uid = db.Djelatniks.First(i => i.IDDjelatnika == prek.IDDjelatnika.Value).UID;

        //                    //1003_1.jpg
        //                    byte  sl = TimeStamp(file.FullName, prek.Vrijeme.Value, prek.Adresa, uid);

        //                    SlikaPrekrsaja slika = new SlikaPrekrsaja();

        //                    slika.IDLokacije = prek.IDLokacije;
        //                    slika.Slika = sl;

        //                    db.SlikaPrekrsajas.InsertOnSubmit(slika);
        //                    db.SubmitChanges();

        //                    Directory.Move(file.FullName, @"D:\Backup\Office 2016\Preko slike\prebaceno\" + file.Name);
        //                }
        //                catch (Exception)
        //                {
        //                    id += file.Name;
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }

        //    return id;
        //}

        //public static byte  SlikaFromFile(string slika)
        //{
        //    try
        //    {
        //        FileStream stream = File.OpenRead(slika);
        //        byte  fileBytes = new byte stream.Length];

        //        stream.Read(fileBytes, 0, fileBytes.Length);
        //        stream.Close();

        //        return fileBytes;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}


        //public static byte  TimeStamp(string putanja, DateTime picTime, string ulica, string uid)
        //{
        //    Bitmap bmp = new Bitmap(putanja);
        //    return TimeStamp(bmp, picTime, ulica, uid);
        //}

        //public static byte  TimeStamp(byte  slika, DateTime picTime, string ulica, string uid)
        //{
        //    MemoryStream ms = new MemoryStream(slika);
        //    Bitmap bmp = new Bitmap(ms);
        //    return TimeStamp(bmp, picTime, ulica, uid);
        //}

        //public static byte  TimeStamp(Bitmap bmp, DateTime picTime, string ulica, string uid)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    //Bitmap bmp = null;
        //    Random rnd = new Random();

        //    try
        //    {
        //        //bmp = new Bitmap(putanja);

        //        Graphics g = Graphics.FromImage(bmp);
        //        Font f = new Font(System.Drawing.FontFamily.GenericMonospace, 10);
        //        string text = string.Format("{0:dd.MM.yyyy u HH:mm:ss} ({2})\r\n{1}", picTime, ulica, uid);

        //        SizeF s = g.MeasureString(text, f, bmp.Width);
        //        RectangleF rec = new RectangleF(new PointF(0, bmp.Height - s.Height - 1), s);

        //        int i, crgb = 0;
        //        for (i = 0; i < Convert.ToInt32(rec.Width * rec.Height / 10); i++)
        //        {
        //            System.Drawing.Color c = bmp.GetPixel(rnd.Next((int)rec.Width),
        //                bmp.Height - 1 - rnd.Next((int)s.Height));
        //            crgb += (c.R + c.G + c.B) / 3;
        //        }

        //        crgb = crgb / i;

        //        g.FillRectangle(new SolidBrush(Color.FromArgb(120, crgb, crgb, crgb)), rec);
        //        g.DrawString(text, f,
        //            new SolidBrush(crgb >= 128 ? Color.Black : Color.White), rec);
        //        g.Dispose();

        //        bmp.Save(ms, ImageFormat.Jpeg);

        //        return ResizeImage(ms.ToArray(), new Size(800, 600));
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        ms.Dispose();
        //        bmp.Dispose();
        //    }
        //}

        //public static byte  ResizeImage(byte  img, Size size)
        //{
        //    Bitmap bmp;

        //    using (MemoryStream ms = new MemoryStream(img)) bmp = new Bitmap(ms);

        //    double f = Math.Min((double)size.Height / bmp.Height, (double)size.Width / bmp.Width);
        //    bmp = new Bitmap(bmp, Convert.ToInt32(bmp.Width * f), Convert.ToInt32(bmp.Height * f));
        //    return ImageToByte(bmp);
        //}

        //public static byte  ImageToByte(Image img)
        //{
        //    byte  byteArray;
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        img.Save(stream, ImageFormat.Jpeg);
        //        stream.Close();

        //        byteArray = stream.ToArray();
        //    }

        //    return byteArray;
        //}

        //public bool UskladiPozivRijeka()
        //{
        //    string idNaloga = "";
        //    try
        //    {
        //        using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString("PROMETNIK_RIJEKA", 1)))
        //        {
        //            foreach (var p in db.Prekrsajis.Where(i => i.Zahtjev && i.NalogPauka == true))
        //            {
        //                try
        //                {
        //                    using (PaukClient sc = new PaukClient())
        //                    {
        //                        bool ok = sc.UskladiPoziv(p.IDNaloga.Value, p.BrojUpozorenja);

        //                        if (!ok)
        //                        {
        //                            idNaloga += p.IDNaloga + "; ";
        //                        }
        //                    }
        //                }
        //                catch
        //                {
        //                    idNaloga += p.IDNaloga + "; ";
        //                }
        //            }
        //        }

        //        Sustav.SpremiGresku("PROMETNIK_RIJEKA", new Exception(idNaloga), 1, "USKLADI POZIVE - PAUK");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku("PROMETNIK_RIJEKA", ex, 1, "USKLADI POZIVE - PAUK");
        //        return false;
        //    }
        //}

        //public void GenerirajPozivNaBroj1()
        //{
        //    DateTime start = DateTime.Now;
        //    Prekrsaj.GenerirajPozivNaBroj("PROMETNIK_SPLIT", true, DateTime.Now, 300, 1);
        //    TimeSpan prvi = DateTime.Now.Subtract(start);

        //}

        //public void Posalji()
        //{
        //    try
        //    {
        //        List<string> brojevi = new List<string>();
        //        brojevi.Add("385989190468");
        //        brojevi.Add("385917844500");
        //        brojevi.Add("385915770635");
        //        brojevi.Add("385915437758");
        //        brojevi.Add("38598320522");
        //        brojevi.Add("38598403469");
        //        brojevi.Add("385911674843");
        //        brojevi.Add("385913945940");
        //        brojevi.Add("385917648950");
        //        brojevi.Add("385915007822");
        //        brojevi.Add("385959000471");
        //        brojevi.Add("385912123004");
        //        brojevi.Add("385912607984");
        //        brojevi.Add("385915461063");
        //        brojevi.Add("38598384228");
        //        brojevi.Add("385917876207");
        //        brojevi.Add("385955038490");
        //        brojevi.Add("385995269668");
        //        brojevi.Add("38598394560");
        //        brojevi.Add("385912509822");
        //        brojevi.Add("385959042226");
        //        brojevi.Add("385915056325");
        //        brojevi.Add("385916008205");
        //        brojevi.Add("385959119397");
        //        brojevi.Add("385959129555");
        //        brojevi.Add("385914641565");
        //        brojevi.Add("385992828628");
        //        brojevi.Add("385915147512");
        //        brojevi.Add("385915326111");
        //        brojevi.Add("385981761815");
        //        brojevi.Add("385953788657");
        //        brojevi.Add("385989867835");
        //        brojevi.Add("385922535071");
        //        brojevi.Add("385998036807");
        //        brojevi.Add("385911121053");
        //        brojevi.Add("385993124415");
        //        brojevi.Add("385915297120");
        //        brojevi.Add("385917648964");
        //        brojevi.Add("385915911111");
        //        brojevi.Add("385915127996");
        //        brojevi.Add("385955152786");
        //        brojevi.Add("38598406564");
        //        brojevi.Add("385914482135");
        //        brojevi.Add("38598800004");
        //        brojevi.Add("385912383512");
        //        brojevi.Add("38598329271");
        //        brojevi.Add("385993010292");
        //        brojevi.Add("38598257987");

        //        //foreach (var b in brojevi)
        //        //{
        //        //    try
        //        //    {
        //        //        // SlanjeSMS.PosaljiSMS(b, "Molio bih one koji nisu, a misle ići na stanično skijanje da mi se prijave do sutra u 9h. Prijava je na e-mailu. Hvala! Pozdrav Andrić");
        //        //    }
        //        //    catch (Exception exception)
        //        //    {


        //        //    }
        //        //}
        //    }
        //    catch
        //    {
        //    }
        //}

        //public bool Baza()
        //{
        //    try
        //    {
        //        string error = "";
        //        foreach (var g in new PostavkeDataContext().GRADOVIs)
        //        {
        //            try
        //            {
        //                if (g.IDGrada == 1)
        //                {
        //                    continue;
        //                }

        //                using (DbConnection connection = new SqlConnection(Sistem.ConnectionString(g.Baza, 4)))
        //                {
        //                    connection.Open();
        //                    //using (DbCommand command = new SqlCommand("ALTER TABLE VANJSKE_PRIJAVE ALTER COLUMN Odbijen bit NULL;"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}

        //                    using (DbCommand command = new SqlCommand("ALTER TABLE Prekrsaji ADD IDTipa int NULL;"))
        //                    {
        //                        command.Connection = connection;
        //                        command.ExecuteNonQuery();
        //                    }

        //                    using (DbCommand command = new SqlCommand("ALTER TABLE Prekrsaji ADD IDRacuna int NULL;"))
        //                    {
        //                        command.Connection = connection;
        //                        command.ExecuteNonQuery();
        //                    }

        //                    using (DbCommand command = new SqlCommand("ALTER TABLE RACUNI_STAVKE_OPIS ADD IDZone int NULL;"))
        //                    {
        //                        command.Connection = connection;
        //                        command.ExecuteNonQuery();
        //                    }

        //                    //using (DbCommand command = new SqlCommand("CREATE TABLE RENTACAR (IDRentaCar int NOT NULL, Registracija nvarchar(10) NOT NULL)"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}

        //                    //using (DbCommand command = new SqlCommand("CREATE TABLE RENTACAR_VOZILA (IDVozila int NOT NULL, IDRentaCar int NOT NULL, Registracija nvarchar(10) NOT NULL);"))
        //                    //{
        //                    //    command.Connection = connection;
        //                    //    command.ExecuteNonQuery();
        //                    //}
        //                }

        //                //using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(g.Baza, idAplikacije)))
        //                //{
        //                //    StatusPauka sp = new StatusPauka();
        //                //    sp.IDStatusa = 22;
        //                //    sp.NazivStatusa = "Blokirao vozilo";
        //                //    sp.Naplacuje = true;
        //                //    sp.Boja = "Green";
        //                //    sp.Zatvara = true;

        //                //    db.StatusPaukas.InsertOnSubmit(sp);
        //                //    db.SubmitChanges();
        //                //}
        //            }
        //            catch (Exception)
        //            {
        //                error += g.Baza + ";";
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Sustav.SpremiGresku("", ex, 5, "UREDI STATUSE");
        //        return false;
        //    }
        //}
    }
}
