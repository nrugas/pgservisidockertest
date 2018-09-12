using System;
using System.Net;
using System.Text;
using PG.Servisi.resources.podaci.baze;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi.resources.cs.email
{
    public class SlanjeSMS
    {
        private static string korisnik = "RIING";
        private static string lozinka = "9EK4AV";
        private static string pin = "6282";
        private static string posiljatelj = "INFO";//"GRADSKO_OKO";

        private static string _broj, _poruka;

        public static void PosaljiSMS(string broj, string poruka)
        {
            broj = broj.TrimStart('+');

            if (broj == "385")
            {
                return;
            }

            if (broj.StartsWith("09"))
            {
                broj = broj.TrimStart('0');
                broj = "385" + broj;
            }

            if (!broj.StartsWith("3859"))
            {
                return;
            }

            string requestUri = string.Format("https://www.it1.hr/api/SMSSend.aspx?Username={0}&Password={1}&Pin={2}&Phone={3}&Sender={4}&Date={5:dd.MM.yy}&Msg={6}", korisnik, lozinka, pin, broj, posiljatelj, DateTime.Today, poruka);

            _broj = broj;
            _poruka = poruka;

            using (WebClient wc = new WebClient())
            {
                wc.DownloadStringCompleted += wc_DownloadStringCompleted;
                wc.Encoding = Encoding.UTF8;
                wc.DownloadStringAsync(new Uri(requestUri));
            }
        }

        private static void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    SMS_LOG po = new SMS_LOG();

                    po.Result = e.Result;
                    po.Error = e.Error != null ? e.Error.ToString() : "";
                    po.DateTime = DateTime.Now;
                    po.Broj = _broj;
                    po.Poruka = _poruka;

                    db.SMS_LOGs.InsertOnSubmit(po);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, 1, "POSLAN SMS");
            }
        }
    }
}