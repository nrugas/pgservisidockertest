using System;
using System.Linq;
using System.Threading;
using PG.Servisi.resources.podaci.baze;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi.resources.cs
{
    public class Sistem
    {
        public static string master = "Data Source=192.168.222.6\\RAZVOJDB;Initial Catalog=master;Persist Security Info=True;User ID=razvoj;Password=razvoj@net";
        public static string _master = "Data Source=10.0.1.243;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=Pa$$w0rd";

        public static string ConnectionString(string grad, int idAplikacije)
        {
            try
            {
                using (PostavkeDataContext db = new PostavkeDataContext())
                {
                    var po = db.GRADOVIs.First(i => i.Baza == grad);

                    return string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}",
                        po.Instanca, po.Baza, po.Korisnik, po.Lozinka);
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Connection String");
                return "";
            }
        }

        public static string ConnectionStringGO(string grad, int idAplikacije)
        {
            try
            {
                GODataContext db = new GODataContext();

                var po = db.GRADOVI1s.First(i => i.Baza == grad);

                return string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}",
                    po.Instanca, po.Baza, po.Korisnik, po.Lozinka);
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Connection String GO");
                return "";
            }
        }

        public static int IDGrada(string grad)
        {
            using (PostavkeDataContext db = new PostavkeDataContext())
            {
                return db.GRADOVIs.First(i => i.Baza == grad).IDGrada;
            }
        }

        /*
         * 
         * System.Data.SqlClient.SqlException (0x80131904): 
         * A network-related or instance-specific error occurred while establishing a connection to SQL Server. 
         * The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server 
         * is configured to allow remote connections.
         * 
         */
    }

    public static class UICallbackTimer
    {
        public static void DelayExecution(TimeSpan delay, Action action)
        {
            Timer timer = null;
            SynchronizationContext context = SynchronizationContext.Current;

            timer = new Timer(
                (ignore) =>
                {
                    timer.Dispose();

                    context.Post(ignore2 => action(), null);
                }, null, delay, TimeSpan.FromMilliseconds(-1));
        }
    }
}