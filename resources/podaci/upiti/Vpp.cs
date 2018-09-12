using System;
using System.Linq;
using PG.Servisi.resources.cs;
using PG.Servisi.resources.podaci.baze;

namespace PG.Servisi.resources.podaci.upiti
{
    public class Vpp
    {
        public class _VppPrijenos
        {
            public int ID { get; set; }
            public int IDRedarstva { get; set; }
            public decimal Iznos { get; set; }
            public string PozivNaBroj { get; set; }
            public string Postupak { get; set; }

            public _VppPrijenos(int id, int idRedarstva, decimal iznos, string poziv, string postupak)
            {
                ID = id;
                IDRedarstva = idRedarstva;
                Iznos = iznos;
                PozivNaBroj = poziv;
                Postupak = postupak;
            }
        }

        public static bool DodajVPP(string grad, _VppPrijenos prekrsaj, int idAplikacije)
        {
            try
            {
                using (PazigradDataContext db = new PazigradDataContext(Sistem.ConnectionString(grad, idAplikacije)))
                {
                    //VPPPREKRSAJI nv = new VPPPREKRSAJI();

                    //int id = 1;
                    //if (db.VPPPREKRSAJIs.Any())
                    //{
                    //    id = db.VPPPREKRSAJIs.Max(i => i.ID) + 1;
                    //}

                    //nv.ID = id;
                    //nv.IDOrig = prekrsaj.ID;
                    //nv.IDPrekrsitelja = 0;
                    //nv.IDRedarstva = prekrsaj.IDRedarstva;
                    //nv.KaznaIznos = prekrsaj.Iznos;
                    //nv.KaznaOrig = prekrsaj.Iznos;
                    //nv.PNBOdobrenja = prekrsaj.PozivNaBroj;
                    //nv.PNBOdobrenjaTrosak = "";
                    //nv.Postupak = prekrsaj.Postupak;
                    //nv.Stanje = prekrsaj.Iznos;
                    //nv.Status = "Nije plaćeno";
                    //nv.TrosakIznos = 0;

                    //db.VPPPREKRSAJIs.InsertOnSubmit(nv);
                    //db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku(grad, ex, idAplikacije, "Dodaj VPP prekršaje");
                return false;
            }
        }

        //brisanje kod storna - komunikacija mogu - ne mogu mijenjati 

        //provjera neprenesenih te njihov prijenos
    }
}