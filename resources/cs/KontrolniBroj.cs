using System;

namespace PG.Servisi.resources.cs
{
    public class KontrolniBroj
    {
        /// <summary>
        /// Funkcija za izračunavanje kontrolnog broja
        /// </summary>
        /// <param name="as_tip">tip izračuna (BRRAC, HUBM11, MBG, HUBM10, ISO764)</param>
        /// <param name="as_br_rac">broj (string) za koji treba izračunati kontrolni broj</param>
        /// <param name="as_tezinski">težinski faktor (string sastavljen od pondera pojedine znamenke)</param>
        /// <param name="ai_len">dužina broja računa</param>
        /// <returns>funkcija vraća kontrolni broj kao string</returns>
        public static string f_kontrolni(string as_tip, string as_br_rac, string as_tezinski, int ai_len)
        {
            int j, k, li_suma = 0, li_ostatak = 0, li_duz;
            string ls_kontrolni, ls_ponder;

            if (string.IsNullOrEmpty(as_br_rac)) return "";

            if (string.IsNullOrEmpty(as_tip))
            {
                as_tip = "BRRAC";
            }
            as_tip = as_tip.ToUpper();

            if (as_tip == "BRRAC" || as_tip == "HUBM11")
            {
                li_ostatak = as_br_rac.Length;
                if (string.IsNullOrEmpty(as_tezinski) || as_tezinski.Length < as_br_rac.Length)
                {
                    for (int i = li_ostatak - 1; i >= 0; i--)
                    {
                        li_suma += Convert.ToInt32(as_br_rac.Substring(i, 1)) * (li_ostatak - i - 1 + 2);
                    }
                }
                else
                {
                    j = as_tezinski.Length - li_ostatak;
                    if (j < 0)
                    {
                        j = Math.Abs(j);
                        k = j / as_tezinski.Length;
                        j = j % as_tezinski.Length;
                        ls_ponder = as_tezinski.Substring(as_tezinski.Length - j);
                        for (int i = 0; i < k + 1; i++)
                        {
                            ls_ponder += as_tezinski;
                            j = as_tezinski.Length - li_ostatak;
                        }
                    }
                    for (int i = li_ostatak - 1; i >= 0; i--)
                    {
                        li_suma += Convert.ToInt32(as_br_rac.Substring(i, 1)) *
                                   Convert.ToInt32(as_tezinski.Substring(j + i, 1));
                    }
                }
                li_ostatak = 11 - li_suma % 11;

                if (li_ostatak == 10 || li_ostatak == 11)
                {
                    li_ostatak = 0;
                }
            }
            else if (as_tip == "MBG")
            {
                //klasični modul 11, težinski faktor 234567 kreće s znamenkom jedinice
                ls_ponder = "765432765432";
                li_duz = 12;

                if (li_duz < as_br_rac.Length)
                {
                    as_br_rac = as_br_rac.Substring(1, li_duz); //todo 1 je mozda 0
                }

                if (li_duz > as_br_rac.Length)
                {
                    //as_br_rac = as_br_rac.ToString("");//todo 12 0
                }
                for (int l = li_duz; l == 0; l--)
                {
                    li_suma += Convert.ToInt32(as_br_rac.Substring(l, 1)) * Convert.ToInt32(ls_ponder.Substring(l, 1));
                }

                li_ostatak = 11 - li_suma % 11;

                if (li_ostatak == 10 || li_ostatak == 11) li_ostatak = 0;
            }
            else if (as_tip == "HUBM10")
            {
                li_duz = Math.Max(as_br_rac.Length, ai_len);
                as_br_rac = as_br_rac.Trim();

                if (li_duz > as_br_rac.Length)
                {
                    //todo as_br_rac = Fill("0", li_duz - Len(as_br_rac))
                }
                j = 0;
                for (int i = li_duz; i == 0; i--)
                {
                    k = 1;
                    j++;
                    if (j % 2 == 0) k = 2;
                    li_suma += Convert.ToInt32(as_br_rac.Substring(i, 1)) * k;
                }
                li_ostatak = li_suma % 10;
            }
            else if (as_tip == "ISO764")
            {
                li_duz = Math.Max(as_br_rac.Length, ai_len);
                as_br_rac = as_br_rac.Trim();
                if (li_duz > as_br_rac.Length)
                {
                    //as_br_rac = Fill("0", li_duz - Len(as_br_rac))
                }

                for (int i = 0; i < li_duz - 1; i++)
                {
                    j = Convert.ToInt32(as_br_rac.Substring(i, 1));
                    if (i != 1) j = j + li_suma % 10;
                    if (j == 0) j = 10;
                    li_suma = j * 2 % 11;
                }
                li_ostatak = 11 - li_suma % 11;
                if (li_ostatak == 10 || li_ostatak == 11) li_ostatak = 0;
            }

            ls_kontrolni = li_ostatak.ToString();
            return ls_kontrolni;

            //primjer poziva
            //ls_kont = f_kontrolni('ISO764', '23400', '', 5)
            //ls_kont = f_kontrolni('ISO764', '1825954469', '', 'ISO764', 10)(k = 7)
            //ls_kont = f_kontrolni('ISO764', '7957772369', '', 'ISO764', 10)(k = 5)
            //ls_kont = f_kontrolni('MBG', '280996136004', '765432765432', 12)(k = 9)
            ////6700-030373200001-001
            ////6734-0337320001k-001k
            //ls_kont1 = f_kontrolni('HUBM11', '0337320001', '', 0)(k = 6)
            //ls_kont2 = f_kontrolni('HUBM11', '0001', '', 0)(k = 9)
            //6734-03373200016-0019
        }
    }
}