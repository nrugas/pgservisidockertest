using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;

namespace TestPlateDefinition
{

    public class PlateDefinition
    {
        /// <summary>
        /// Basic character set (can be changed - inlude or exclude certain chars)
        /// </summary>
        private const string ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private const string NUMERIC = "01234567890";

        private static HybridDictionary plateBadges = new HybridDictionary();

        public static Image GetPlateBadge(string id)
        {
            return (Image)plateBadges[id.ToUpper()];
        }

        public static string[] euCountries = new[] { "" };

        public static string[] EUCountries
        {
            get { return euCountries; }
            private set { euCountries = value; }
        }

        public static bool IsEU(string countryAbb)
        {
            return PlateDefinition.EUCountries.Contains(countryAbb.ToUpper().Trim());
        }

        public static string[] preferredOrder = new string[0];

        public static string[] PreferredOrder
        {
            get { return preferredOrder; }
        }

        public static void SetPreferredOrder(string abbrevList)
        {
            preferredOrder = _MakeList(abbrevList).ToArray();
        }

        static readonly object lockCountryPlates = new object();
        private static List<PlateDefinition> countryPlates = null;

        public static PlateDefinition[] CountryPlates
        {
            get
            {
                lock (countryPlates)
                {
                    countryPlates.Sort(delegate (PlateDefinition x, PlateDefinition y)
                    {
                        return _CompareCountryCodesPreferring(x.CountryCode, y.CountryCode);
                    });
                    return countryPlates.ToArray();
                }
            }
            set { lock (lockCountryPlates) countryPlates = new List<PlateDefinition>(value); }
        }

        private static int _CompareCountryCodesPreferring(string c1, string c2)
        {
            int i, j;
            for (i = 0; i < PreferredOrder.Length; i++)
            {
                if (PreferredOrder[i].ToUpper() == c1) break;
            }
            for (j = 0; j < PreferredOrder.Length; j++)
            {
                if (PreferredOrder[j].ToUpper() == c2) break;
            }
            return i.CompareTo(j);
        }

        /// <summary>
        /// Make list from xml config string (delimited)
        /// </summary>
        /// <param name="delimitedStr"></param>
        /// <returns></returns>
        private static IEnumerable<string> _MakeList(string delimitedStr)
        {
            List<string> ret = new List<string>();
            if (!string.IsNullOrEmpty(delimitedStr))
            {

                List<string> llist = new List<string>();
                foreach (var l in delimitedStr.Split(new[] { ',', ';' }))
                {
                    string ll = l.ToUpper().Trim(new[] { ' ', '\n', '\r' });
                    if (!string.IsNullOrEmpty(ll))
                    {
                        _LoopChars(ll, 0, "", ret);
                    }
                }
            }
            return ret.ToArray();
        }


        private static void _LoopChars(string item, int depth, string result, List<string> ret)
        {
            if (depth < item.Length)
            {
                string loop = item[depth].ToString();
                if (item[depth] == '{')
                {
                    int closed = item.IndexOf('}', depth);
                    if (closed > 0)
                    {
                        loop = item.Substring(depth + 1, closed - depth - 1);
                        depth = closed;
                    }
                }
                for (int i = 0; i < loop.Length; i++)
                {
                    char c = loop[i];
                    if (i + 2 < loop.Length && loop[i + 1] == '-' &&
                        ((char.IsDigit(c) && char.IsDigit(loop[i + 2])) ||
                        ((char.IsLetter(c) && char.IsLetter(loop[i + 2]))
                        )))
                    {
                        string lloop = "";
                        if (char.IsNumber(c))
                        {
                            lloop = NUMERIC;
                        }
                        else
                        {
                            lloop = ALPHA;
                        }
                        int start = lloop.IndexOf(c);
                        int end = lloop.IndexOf(loop[i + 2]);
                        for (int j = start; j <= end; j += ((end > start) ? 1 : -1))
                        {
                            _LoopChars(item, depth + 1, result + lloop[j], ret);
                        }
                    }
                    else
                    {
                        if (char.IsLetterOrDigit(c)) _LoopChars(item, depth + 1, result + c.ToString(), ret);
                    }
                }
            }
            else if (!ret.Contains(result)) ret.Add(result);
        }



        /// <summary>
        /// Convert textual color name to Color
        /// </summary>
        /// <param name="colorName"></param>
        /// <returns></returns>
        private static Color xColorFromName(string colorName)
        {
            Color ret = Color.Transparent;
            try
            {
                ret = ColorTranslator.FromHtml(colorName);
            }
            catch (Exception e)
            {

            }
            return ret;
        }


        /// <summary>
        /// Convert textual color name to Color
        /// </summary>
        /// <param name="colorName"></param>
        /// <returns></returns>
        private static Color ColorFromName(string colorName)
        {
            Color ret = Color.Transparent;
            try
            {
                Type colorType = typeof(System.Drawing.Color);
                // We take only static property to avoid properties like Name, IsSystemColor ...
                PropertyInfo[] propInfos = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);

                foreach (var colorInfo in propInfos)
                {
                    if (colorInfo.Name.ToLower() == colorName.ToLower())
                    {
                        ret = (Color)colorInfo.GetValue(null, null);
                        break;
                    }
                    if (colorInfo.Name.ToLower().Contains(colorName.ToLower())) ret = (Color)colorInfo.GetValue(null, null);

                }

            }
            catch (Exception ex)
            {

            }
            return ret;
        }


        /// <summary>
        /// Get colors from xml config string ('black/white',...) 
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="backColor"></param>
        /// <param name="foreColor"></param>
        private static void GetColorsFromConfigString(string colors, ref Color backColor, ref Color foreColor)
        {

            string bckcol = "", forecol = "";
            if (!string.IsNullOrEmpty(colors))
            {
                try
                {
                    if (colors.Contains("/"))
                    {
                        bckcol = colors.Substring(colors.LastIndexOf("/")).Trim(new[] { ' ', '/' });
                        forecol = colors.Substring(0, colors.LastIndexOf("/")).Trim(new[] { ' ', '/' });
                    }
                    else
                    {
                        forecol = colors.Trim(new[] { ' ', '/' });
                    }
                    Color c = ColorFromName(bckcol);
                    if (c != Color.Transparent) backColor = c;
                    c = ColorFromName(forecol);
                    if (c != Color.Transparent) foreColor = c;
                }
                catch (Exception e)
                {

                }

            }
        }


        /// <summary>
        /// Make equal/similar chars lists
        /// </summary>
        /// <param name="configStr"></param>
        /// <param name="parentList"></param>
        /// <returns></returns>
        public static IEnumerable<string> _MakeCharsList(string configStr, IEnumerable<string> parentList)
        {
            List<string> ret = new List<string>(), addList = new List<string>();
            if (parentList != null) ret.AddRange(parentList);
            addList.AddRange(_MakeList(configStr));
            foreach (var item in addList)
            {
                bool newItem = true;
                for (int ii = 0; ii < item.Length; ii++)
                {
                    var c = item[ii];
                    for (int i = 0; i < ret.Count; i++)
                    {
                        if (ret[i].Contains(c))
                        {
                            newItem = false;
                            ii = 0;
                            while (ii < item.Length)
                            {
                                if (!ret[i].Contains(item[ii])) ret[i] += (item[ii]);
                                ii++;
                            }
                            break;
                        }
                    }
                }
                if (newItem) ret.Add(item);
            }
            return ret.ToArray();
        }


        public static IEnumerable<string> _MergeCharsList(IEnumerable<string> list, IEnumerable<string> parentList)
        {
            List<string> ret = new List<string>(), addList = new List<string>();
            if (parentList != null) ret.AddRange(parentList);
            addList.AddRange(list);
            foreach (var item in addList)
            {
                bool newItem = true;
                for (int ii = 0; ii < item.Length; ii++)
                {
                    var c = item[ii];
                    for (int i = 0; i < ret.Count; i++)
                    {
                        if (ret[i].Contains(c))
                        {
                            newItem = false;
                            ii = 0;
                            do
                            {
                                if (!ret[i].Contains(item[ii])) ret[i] += (item[ii]);
                            } while (++ii < item.Length);
                            break;
                        }
                    }
                }
                if (newItem) ret.Add(item);
            }
            StringBuilder add = new StringBuilder();
            for (int i = 0; i < ret.Count; i++)
            {
                for (int ii = ret.Count - 1; ii > i; ii--)
                {
                    add.Length = 0;
                    bool connected = false;
                    foreach (var c in ret[ii])
                    {
                        if (ret[i].Contains(c)) connected = true;
                        else add.Append(c);
                    }
                    if (connected)
                    {
                        ret[i] += add.ToString();
                        ret.RemoveAt(ii);
                    }
                }
            }
            return ret.ToArray();
        }


        public static string PriorityCountry
        {
            get { return "HR"; }
        }

        private static string[] priorityPlates = new string[0];


        /// <summary>
        /// 
        /// </summary>
        public static string[] PriorityPlates
        {
            get { return priorityPlates.ToArray(); }
            private set { priorityPlates = value; }
        }


        private List<char> alpha = new List<char>(ALPHA);
        private List<char> numeric = new List<char>(NUMERIC);


        private static string[] identicalChars = new string[0];


        /// <summary>
        /// Equal chars - static - common equal chars for all CountryCode plates
        /// </summary>
        public static string[] IdenticalChars
        {
            get { return identicalChars.ToArray(); }
            private set { identicalChars = value; }
        }

        private static string[] equalChars = new string[0];


        /// <summary>
        /// Equal chars - static - common equal chars for all CountryCode plates
        /// </summary>
        public static string[] EqualChars
        {
            get { return equalChars.ToArray(); }
            private set { equalChars = value; }
        }

        private static string[] similarChars = new string[0];

        public static string[] SimilarChars
        {
            get { return similarChars.ToArray(); }
            private set { similarChars = value; }
        }


        private string[] plateIdenticalChars = new string[0];


        /// <summary>
        /// Equal chars - common + CountryCode specific
        /// </summary>
        public string[] PlateIdenticalChars
        {
            get { return plateIdenticalChars.ToArray(); }
            private set { plateIdenticalChars = value; }
        }

        private string[] plateEqualChars = new string[0];


        /// <summary>
        /// Equal chars - common + CountryCode specific
        /// </summary>
        public string[] PlateEqualChars
        {
            get { return plateEqualChars.ToArray(); }
            private set { plateEqualChars = value; }
        }



        private string[] plateSimilarChars = new string[0];

        /// <summary>
        /// Similar chars (IL, KX,...)
        /// </summary>
        public string[] PlateSimilarChars
        {
            get { return plateSimilarChars.ToArray(); }
            private set { plateSimilarChars = value; }
        }



        private void IncExlChars(string configStr, bool include)
        {
            foreach (var str in _MakeList(configStr))
            {
                foreach (var c in str)
                {
                    if (!char.IsSeparator(c) && !char.IsPunctuation(c) &&
                        !char.IsWhiteSpace(c))
                    {
                        List<char> list = ((char.IsNumber(c)) ? numeric : alpha);
                        if (include)
                        {
                            if (!list.Contains(c)) list.Add(c);
                        }
                        else
                        {
                            if (list.Contains(c)) list.Remove(c);
                        }
                    }
                }
            }
        }


        private bool Check(string mask, char c, List<string> list, int listInd)
        {
            bool ret = false;
            switch (mask)
            {
                case "X":
                    int i = 0;
                    while (i < list.Count())
                    {
                        if (list[i].Length <= listInd || list[i][listInd] != c) list.RemoveAt(i);
                        else i++;
                    }
                    ret = i > 0;
                    break;
                case "A":
                    ret = alpha.Contains(c);
                    break;
                case "0":
                    ret = numeric.Contains(c);
                    break;
            }
            return ret;
        }



        public static CountryResultSet[] SearchResults(string plateRecognition)
        {
            List<CountryResultSet> ret = new List<CountryResultSet>();
            int i = 0;
            string c = "";
            foreach (var plateDefinition in CountryPlates)
            {
                c += plateDefinition.CountryCode + ",";
                Result[] r = plateDefinition.GetPlateVariations(plateRecognition);
                if (r.Length > 0)
                {
                    ret.Add(new CountryResultSet(plateDefinition, r));
                }
                i++;
            }
            ret.Sort(delegate (CountryResultSet x, CountryResultSet y)
            {
                int cmp = x.BestAccuracy.CompareTo(y.BestAccuracy);
                if (cmp == 0)
                {
                    cmp = _CompareCountryCodesPreferring(x.Country, y.Country);
                }
                return cmp;
            });
            return ret.ToArray();
        }




        public Result[] GetPlateVariations(string plateRecognition)
        {
            List<Result> ret = new List<Result>();
            foreach (var plate in plateRecognition.Split(new[] { ' ', ',' }))
            {
                if (!string.IsNullOrEmpty(plate.Trim()))
                {
                    foreach (var pattern in Patterns)
                    {
                        GetVariation(ret, "", plate, 0, pattern, null, -1, -1, 0);
                    }
                }
            }
            ret.Sort(delegate (Result x, Result y)
            {
                return x.Accuracy.CompareTo(y.Accuracy);
            });
            return ret.ToArray();
        }

        public static int WorstAccuracy = 3; // TODO odrediti optimum (bilo 6)

        private void GetVariation(List<Result> ret, string combination, string part, int depth,
            Pattern pattern,
            List<string> list, int listInd, int listOrder, int accuracy)
        {
            if (accuracy > WorstAccuracy) return;

            if (pattern.Mask.Length != part.Length) return;
            if (depth < part.Length)
            {
                bool ident = false;
                char c = part[depth];

                string mask = pattern.Mask[depth].ToString();
                if (mask == "X")
                {
                    if (list == null || ++listInd >= pattern.listsApplied[listOrder].Value)
                    {
                        listOrder++;
                        listInd = 0;
                        list = new List<string>((IEnumerable<string>)Lists[pattern.listsApplied[listOrder].Key]);
                    }
                }
                else
                {
                    list = null;
                    listInd = -1;
                }
                List<char> chechedChar = new List<char>();
                for (int iden = 0; iden < 3; iden++)
                {
                    IEnumerable<string> idlist = new List<string>();
                    switch (iden)
                    {
                        case 0:
                            idlist = PlateIdenticalChars;
                            break;
                        case 1:
                            idlist = _MergeCharsList(PlateEqualChars, PlateIdenticalChars);
                            break;
                        case 2:
                            idlist = _MergeCharsList(PlateSimilarChars, _MergeCharsList(PlateEqualChars, PlateIdenticalChars));
                            break;
                    }
                    int acc = 0;
                    foreach (var equalChars in idlist)
                    {
                        if (equalChars.Contains(c))
                        {
                            foreach (var ic in equalChars)
                            {
                                if (chechedChar.Contains(ic)) continue;
                                chechedChar.Add(ic);
                                // TODO - napravio izmjenu za test... hm!???
                                if (c.ToString().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D") == 
                                    ic.ToString().Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z").Replace("Š", "S").Replace("Đ", "D")) acc = 0;
                                else acc = (iden + 1);
                                List<string> xlist = null;
                                if (list != null) xlist = new List<string>(list);
                                if (Check(mask, ic, xlist, listInd))
                                    GetVariation(ret, combination + ic.ToString(), part, depth + 1, pattern, xlist,
                                        listInd, listOrder, accuracy + acc);
                            }
                            ident = true;
                            break;
                        }
                    }
                }
                if (!ident)
                {
                    if (Check(mask, c, list, listInd))
                        GetVariation(ret, combination + c.ToString(), part, depth + 1, pattern, list, listInd, listOrder,
                            accuracy);
                }
            }
            else
            {
                StringBuilder plate = new StringBuilder(combination);
                for (int i = pattern.DelimiterList.Length - 1; i >= 0; i--)
                {
                    plate.Insert(pattern.DelimiterList[i].Value, pattern.DelimiterList[i].Key);
                }
                string p = plate.ToString(); //.Replace("@", " ");
                var result = new Result(p, accuracy, CountryCode, pattern);
                if (!ret.Contains(result, new compareResults())) ret.Add(result);
            }
        }


        private class compareResults : IEqualityComparer<Result>
        {
            public bool Equals(Result x, Result y)
            {

                if (string.Compare(x.Plate, y.Plate) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(Result obj)
            {
                return obj.Plate.GetHashCode();
            }

        }



        public class Pattern
        {

            internal static Regex listsRead = new Regex(
                "{\\s*(?<listId>\\d+)\\s*:\\s*(?<length>\\d+)\\s*}",
                RegexOptions.IgnoreCase
                | RegexOptions.CultureInvariant
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                );


            public string Mask { get; private set; }

            private string format = "";


            private List<KeyValuePair<char, int>> delimiterList = new List<KeyValuePair<char, int>>();

            public KeyValuePair<char, int>[] DelimiterList
            {
                get { return delimiterList.ToArray(); }
            }

            public string Format
            {
                get { return format; }
                internal set
                {
                    StringBuilder mask = new StringBuilder();
                    format = value.ToUpper().Replace('|', '\x2006');
                    int i = 0;
                    foreach (Match m in listsRead.Matches(format))
                    {
                        if (m.Index > i)
                        {
                            foreach (var c in format.Substring(i, m.Index - i))
                            {
                                if (c == 'A' || c == '0') mask.Append(c);
                                else delimiterList.Add(new KeyValuePair<char, int>(c, mask.Length));
                            }
                        }
                        i = m.Index + m.Length;
                        try
                        {

                            string gid = m.Groups["listId"].Value.Trim();
                            int len = Convert.ToInt32(m.Groups["length"].Value.Trim());
                            for (int ii = 0; ii < len; ii++) mask.Append('X');
                            listsApplied.Add(new KeyValuePair<string, int>(gid, len));
                        }
                        catch (Exception)
                        {

                        }
                    }
                    if (i < format.Length)
                    {
                        foreach (var c in format.Substring(i))
                        {
                            if (c == 'A' || c == '0') mask.Append(c);
                            else delimiterList.Add(new KeyValuePair<char, int>(c, mask.Length));
                        }
                    }
                    Mask = mask.ToString();

                }
            }


            public Color PlateBackColor { get; set; }
            public Color PlateTextColor { get; set; }

            public bool LeftBar { get; set; }
            public bool RightBar { get; set; }

            public List<KeyValuePair<string, int>> listsApplied = new List<KeyValuePair<string, int>>();

            public Pattern()
            {
                PlateBackColor = Color.Transparent;
                PlateTextColor = Color.Transparent;
            }
        }








        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public bool EUbar { get; set; }
        public bool RightBar { get; set; }
        private Color plateBackColor = Color.White;

        public Color PlateBackColor
        {
            get { return plateBackColor; }
        }

        private Color plateTextColor = Color.Black;

        public Color PlateTextColor
        {
            get { return plateTextColor; }
        }

        public HybridDictionary Lists { get; private set; }

        private List<Pattern> Patterns { get; set; }





        public void AddList(string id, string list)
        {
            Lists.Add(id.Trim(), _MakeList(list));
        }


        public static Regex formatRegex = new Regex("\\[(\\w+)\\]",
            RegexOptions.IgnoreCase
            | RegexOptions.CultureInvariant
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled
            );

        private string[] GetFormats(string rawFormat)
        {
            List<string> actions = new List<string>();
            int i = 0;
            foreach (Match m in formatRegex.Matches(rawFormat))
            {
                if (m.Index > i) actions.Add(rawFormat.Substring(i, m.Index - i));
                actions.Add(m.ToString());
                i = m.Index + m.Length;
            }
            if (i < rawFormat.Length) actions.Add(rawFormat.Substring(i, rawFormat.Length - i));
            List<string> ret = new List<string>();
            GetFormat(ret, "", actions.ToArray(), 0);
            return ret.ToArray();
        }

        private void GetFormat(List<string> ret, string format, string[] action, int depth)
        {
            if (format == null) format = "";
            if (depth < action.Length)
            {
                if (!action[depth].StartsWith("[") || !action[depth].EndsWith("]"))
                    GetFormat(ret, format + action[depth], action, depth + 1);
                else
                {
                    string v = action[depth].Substring(1, action[depth].Length - 2);
                    for (int i = 0; i <= v.Length; i++)
                    {
                        string vv = "";
                        if (i > 0) vv = v.Substring(0, i);
                        GetFormat(ret, format + vv, action, depth + 1);
                    }
                }
            }
            else
            {
                if (!ret.Contains(format)) ret.Add(format);
            }
        }


        public void AddPattern(string format, string appliedLists, string colors, bool peubar, bool prightbar)
        {
            foreach (var frmt in GetFormats(format.Trim().ToUpper()))
            {
                Pattern p = new Pattern();
                p.Format = frmt;
                GetFormats(format);
                string bckcol = "", forecol = "";
                Color bc = PlateBackColor;
                Color fc = PlateTextColor;
                GetColorsFromConfigString(colors, ref bc, ref fc);
                p.PlateBackColor = bc;
                p.PlateTextColor = fc;
                p.LeftBar = peubar;
                p.RightBar = prightbar;
                Patterns.Add(p);
            }
        }




        public PlateDefinition()
        {
            Patterns = new List<Pattern>();
            Lists = new HybridDictionary();
        }

        public void SetColors(string configColorString)
        {
            GetColorsFromConfigString(configColorString, ref plateBackColor, ref plateTextColor);
        }


        private static string configFile = "RegistarskeOznake.xml";

        public static PlateDefinition[] LoadConfigFile()
        {
            List<PlateDefinition> ret = new List<PlateDefinition>();
            lock (configFile)
            {
                string file = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath,"bin//RestApi//"+configFile);

                XmlDocument xDoc = new XmlDocument();

                try
                {
                    if (File.Exists(file))
                    {

                        xDoc.Load(file);


                        string identicalChars = "", equalChars = "", similarChars = "", order = "", eu = "";
                        string priorityplates = "";
                        foreach (XmlAttribute a in xDoc.DocumentElement.Attributes)
                        {
                            switch (a.Name.ToLower())
                            {
                                case "identicalchars":
                                    identicalChars = a.Value.Trim();
                                    break;
                                case "equalchars":
                                    equalChars = a.Value.Trim();
                                    break;
                                case "similarchars":
                                    similarChars = a.Value.Trim();
                                    break;

                                case "order":
                                    order = a.Value.Trim();
                                    break;
                                case "eucountries":
                                    eu = a.Value.Trim();
                                    break;
                                case "priorityplates":
                                    priorityplates = a.Value.ToUpper().Trim();
                                    break;
                            }
                        }
                        PlateDefinition.IdenticalChars = _MakeCharsList(identicalChars, null).ToArray();
                        PlateDefinition.EqualChars = _MakeCharsList(equalChars, null).ToArray();
                        PlateDefinition.SimilarChars = _MakeCharsList(similarChars, null).ToArray();
                        PlateDefinition.SetPreferredOrder(order);
                        PlateDefinition.EUCountries = _MakeList(eu).ToArray();
                        PlateDefinition.PriorityPlates = _MakeList(priorityplates).ToArray();


                        XmlNodeList badges = xDoc.GetElementsByTagName("badge");
                        foreach (XmlNode n in badges)
                        {


                            string id = (n.Attributes["id"] != null) ? n.Attributes["id"].Value.ToUpper() : "";
                            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(n.InnerText))
                            {
                                Image badge = null;
                                try
                                {
                                    badge = new Bitmap(new MemoryStream(Convert.FromBase64String(n.InnerText)));
                                }
                                catch (Exception e)
                                {

                                }
                                if (badge != null)
                                {
                                    if (!plateBadges.Contains(id)) plateBadges.Add(id, badge);
                                }

                            }
                        }


                        XmlNodeList plates = xDoc.GetElementsByTagName("plate");

                        foreach (XmlNode n in plates)
                        {

                            PlateDefinition plate = new PlateDefinition();
                            ret.Add(plate);

                            string abb = "";
                            string country = "";
                            bool eubar = false;
                            bool rightbar = false;
                            string plateColors = "";
                            string badge = "";
                            similarChars = "";
                            equalChars = "";
                            identicalChars = "";
                            string include = "";
                            string exclude = "";


                            string pauk = "";
                            string goGradId = "";
                            List<string> wcfLocations = new List<string>();
                            List<string> goServiceLocations = new List<string>();
                            List<string> connStrings = new List<string>();
                            foreach (XmlAttribute a in n.Attributes)
                            {
                                switch (a.Name.ToLower())
                                {
                                    case "abb":
                                        abb = a.Value.ToUpper().Trim();
                                        break;

                                    case "CountryCode":
                                        country = a.Value.Trim();
                                        break;

                                    case "eubar":
                                        try
                                        {
                                            eubar = bool.Parse(a.Value);
                                        }
                                        catch
                                        {

                                        }
                                        break;
                                    case "rightbar":
                                        try
                                        {
                                            rightbar = bool.Parse(a.Value);
                                        }
                                        catch
                                        {

                                        }
                                        break;
                                    case "color":
                                        plateColors = a.Value.Trim();
                                        break;
                                    case "badge":
                                        badge = a.Value.Trim();
                                        break;
                                    case "identicalchars":
                                        identicalChars = a.Value.Trim();
                                        break;
                                    case "similarchars":
                                        similarChars = a.Value.Trim();
                                        break;
                                    case "equalchars":
                                        equalChars = a.Value.Trim();
                                        break;

                                    case "includechars":
                                        include = a.Value.ToUpper().Trim();
                                        break;
                                    case "excludechars":
                                        exclude = a.Value.ToUpper().Trim();
                                        break;



                                    default:

                                        break;
                                }
                            }

                            plate.CountryCode = abb;
                            plate.CountryName = country;
                            plate.EUbar = eubar;
                            plate.RightBar = rightbar;
                            plate.SetColors(plateColors);
                            plate.PlateIdenticalChars =
                                _MakeCharsList(identicalChars, PlateDefinition.IdenticalChars).ToArray();
                            plate.PlateEqualChars =
                                _MakeCharsList(equalChars, PlateDefinition.EqualChars).ToArray();
                            plate.PlateSimilarChars =
                                _MakeCharsList(similarChars, PlateDefinition.SimilarChars).ToArray();
                            plate.IncExlChars(include, true);
                            plate.IncExlChars(exclude, false);


                            foreach (XmlNode nn in n.ChildNodes)
                            {
                                identicalChars = "";
                                similarChars = "";
                                equalChars = "";
                                bool peubar = eubar;
                                bool prightbar = rightbar;
                                string list = "";
                                string id = "";

                                plateColors = "";
                                badge = "";
                                if (nn.Attributes == null) continue;
                                foreach (XmlAttribute a in nn.Attributes)
                                {
                                    switch (a.Name.ToLower())
                                    {
                                        case "list":
                                            list = a.Value.ToUpper().Trim();
                                            break;

                                        case "id":
                                            id = a.Value.Trim();
                                            break;

                                        case "color":
                                            plateColors = a.Value.Trim();
                                            break;


                                        case "badge":
                                            badge = a.Value.Trim();
                                            break;
                                        case "identicalchars":
                                            identicalChars = a.Value.Trim();
                                            break;
                                        case "similarchars":
                                            similarChars = a.Value.Trim();
                                            break;
                                        case "equalchars":
                                            equalChars = a.Value.Trim();
                                            break;
                                        case "eubar":
                                            try
                                            {
                                                peubar = bool.Parse(a.Value);
                                            }
                                            catch
                                            {

                                            }
                                            break;
                                        case "rightbar":
                                            try
                                            {
                                                prightbar = bool.Parse(a.Value);
                                            }
                                            catch
                                            {

                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                }
                                switch (nn.Name.ToLower())
                                {
                                    case "pattern":
                                        plate.AddPattern(nn.InnerText, list, plateColors, peubar, prightbar);
                                        break;
                                    case "list":
                                        plate.AddList(id, nn.InnerText);
                                        break;

                                }

                            }


                        }

                    }
                    //else CFMessageBox.MessageBoxCF.Show("Ne postoji datoteka " + file);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                }
            }



            CountryPlates = ret.ToArray();

            return CountryPlates;

        }


        public class CountryResultSet
        {
            public PlateDefinition CountryPlateDefinition { get; private set; }
            public Result[] Plates { get; private set; }
            public string Country { get { return CountryPlateDefinition.CountryCode; } }

            public int BestAccuracy { get; private set; }
            public int WorstAccuracy { get; private set; }
            public Result BestPlate { get; private set; }

            internal protected CountryResultSet(PlateDefinition pd, IEnumerable<Result> plates)
            {
                CountryPlateDefinition = pd;
                BestAccuracy = 99;
                WorstAccuracy = 0;
                BestPlate = null;
                List<Result> r = new List<Result>();
                foreach (var plate in plates)
                {
                    if (plate.CountryCode == pd.CountryCode)
                    {
                        r.Add(plate);
                        if (BestAccuracy >= 0)
                        {
                            if (plate.CountryCode == PlateDefinition.PriorityCountry)
                            {
                                foreach (var priorityPlate in PriorityPlates)
                                {
                                    if (plate.Plate.StartsWith(priorityPlate))
                                    {
                                        BestAccuracy = -1;
                                        BestPlate = plate;
                                        break;
                                    }
                                }
                            }
                        }
                        if (plate.Accuracy < BestAccuracy)
                        {
                            BestAccuracy = plate.Accuracy;
                            BestPlate = plate;
                        }
                        if (plate.Accuracy > WorstAccuracy) WorstAccuracy = plate.Accuracy;
                    }
                }
                Plates = r.ToArray();
            }

        }


        public class Result
        {


            public string Plate { get; private set; }
            public string CountryCode { get; private set; }
            public bool EU { get; private set; }
            public int Accuracy { get; private set; }

            public bool LeftBar { get; private set; }

            public bool RightBar { get; private set; }

            public Color PlateBackColor { get; set; }
            public Color PlateTextColor { get; set; }


            internal protected Result(string plate, int accuracy, string countryCode, Pattern pattern)
            {
                this.Plate = plate;
                this.Accuracy = accuracy;
                this.CountryCode = countryCode;
                this.LeftBar = pattern.LeftBar;
                this.RightBar = pattern.RightBar;
                this.PlateBackColor = pattern.PlateBackColor;
                this.PlateTextColor = pattern.PlateTextColor;
                this.EU = PlateDefinition.IsEU(countryCode);
            }




        }

        

    }
}