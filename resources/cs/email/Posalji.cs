using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using PG.Servisi.resources.podaci.baze;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi.resources.cs.email
{
    public class Posalji
    {
        public static bool Email(string grad, string poruka, string subject, List<string> primatelji, string prilog, bool html, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    POSTAVKE_EMAILA po = db.POSTAVKE_EMAILAs.First();

                    MailAddress addr1 = new MailAddress(po.Email, po.Naziv);

                    MailMessage msg = new MailMessage();

                    if (primatelji.Count == 1)
                    {
                        msg.To.Add(primatelji.First());
                    }
                    else
                    {
                        msg.To.Add("support@pazigrad.com");

                        foreach (var p in primatelji)
                        {
                            msg.CC.Add(p);
                        }
                    }

                    msg.From = addr1;

                    msg.IsBodyHtml = html;
                    msg.Subject = subject;
                    msg.Body = poruka;

                    if (prilog != null)
                    {
                        msg.Attachments.Add(new Attachment(prilog));
                    }

                    SmtpClient sm = new SmtpClient();
                    sm.Timeout = po.Timeout;
                    sm.Host = po.Host;
                    sm.UseDefaultCredentials = po.DefaultCredentials;
                    sm.EnableSsl = po.EnableSsl;
                    sm.Port = po.Port;
                    sm.Credentials = new NetworkCredential(po.UserName, po.Lozinka);
                    sm.Send(msg);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Email");
                return false;
            }
        }

        public static bool EmailPrilozi(string grad, string poruka, string subject, List<string> primatelji, List<string> prilozi, bool html, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    POSTAVKE_EMAILA po = db.POSTAVKE_EMAILAs.First();

                    MailAddress addr1 = new MailAddress(po.Email, po.Naziv);

                    MailMessage msg = new MailMessage();

                    foreach (var p in primatelji)
                    {
                        msg.CC.Add(p);
                    }

                    msg.From = addr1;

                    msg.IsBodyHtml = html;
                    msg.Subject = subject;
                    msg.Body = poruka;

                    foreach (var p in prilozi)
                    {
                        msg.Attachments.Add(new Attachment(p));
                    }

                    SmtpClient sm = new SmtpClient();
                    sm.Timeout = po.Timeout;
                    sm.Host = po.Host;
                    sm.UseDefaultCredentials = po.DefaultCredentials;
                    sm.EnableSsl = po.EnableSsl;
                    sm.Port = po.Port;
                    sm.Credentials = new NetworkCredential(po.UserName, po.Lozinka);
                    sm.Send(msg);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Email");
                return false;
            }
        }

        public static bool EmailHelpDesk(string grad, string poruka, string subject, List<byte[]> slike, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    POSTAVKE_EMAILA po = db.POSTAVKE_EMAILAs.First();

                    MailMessage msg = new MailMessage();

                    //msg.To.Add("podrska@pazigrad.com");
                    msg.To.Add("podrska@ri-ing.net");
                    msg.From = new MailAddress("podrska@ri-ing.net");//("ivo.opancar@ri-ing.net");//mora biti neki korisnik iz spiceworksa da bi tagovi bili dodijeljeni;
                    msg.IsBodyHtml = false;
                    msg.Subject = subject;
                    msg.Body = poruka;

                    if (slike != null)
                    {
                        int x = 0;
                        foreach (var s in slike)
                        {
                            msg.Attachments.Add(new Attachment(new MemoryStream(s), "Slika - " + x++ +".jpg"));
                        }
                    }

                    SmtpClient sm = new SmtpClient();
                    sm.Timeout = po.Timeout;
                    sm.Host = po.Host;
                    sm.UseDefaultCredentials = po.DefaultCredentials;
                    sm.EnableSsl = po.EnableSsl;
                    sm.Port = po.Port;
                    sm.Credentials = new NetworkCredential(po.UserName, po.Lozinka);
                    sm.Send(msg);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Email");
                return false;
            }
        }
    }
}