using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;



namespace WebParse
{
    class TVShow
    {
        public Int64 Id;
        public string NameOriginal;
        public string NameTranslated = "";
        public int ReliseYear = 0;
        public List<string> Countries;
        public List<string> Genres;
        public Boolean isOpen;

        public TVShow(Int64 idShow, string nameShow, string nameTranslated)
        {
            Id = idShow;
            NameOriginal = nameShow;
            NameTranslated = nameTranslated;
            Countries = new List<string> {};
            Genres = new List<string> {};
            isOpen = true;
        }

        public void LoadInfo()
        {
            HtmlDocument doc = Connection.GetShowInfoDoc(String.Format("http://www.lostfilm.tv/browse.php?cat={0}", this.Id));
            if (doc != null)
            {
                try
                {
                    HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[@class=\"mid\"]/div[img]");
                    this.ReliseYear = Routines.GetInt(Constants.reParams["reliseYear"].Match(node.InnerHtml).Value);
                    foreach (String country in Constants.reParams["countrySplit"].
                                                Split(Constants.reParams["country"].Match(node.InnerHtml).Value).
                                                Select(c => c.Trim()))
                    {
                        this.Countries.Add(country);
                    }
                    foreach (String genre in Constants.reParams["genreSplit"].
                                                Split(Constants.reParams["genre"].Match(node.InnerHtml).Value).
                                                Select(g => g.Trim()).Where(s => (s != String.Empty)))
                    {
                        this.Genres.Add(genre);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void PrintInfo()
        {
            Console.WriteLine(String.Format("ID: {0}\r\nName Original: {1}\r\nName Translated: {2}\r\nRelise Year: {3}",
                                                this.Id, this.NameOriginal, this.NameTranslated, this.ReliseYear));
            Console.Write(String.Format("Countries: {0}", (this.Countries.Count == 0)?"\r\n":""));
            for (int i = 0; i < this.Countries.Count; i++)
            {
                Console.WriteLine(String.Format("{0}{1}", "".PadLeft((i == 0) ? 0 : 11), this.Countries.ElementAt(i)));
            }
            Console.Write(String.Format("Genres: {0}", (this.Genres.Count == 0)?"\r\n":""));
            for (int i = 0; i < this.Genres.Count; i++)
            {
                Console.WriteLine(String.Format("{0}\"{1}\"", "".PadLeft((i == 0) ? 0 : 8), this.Genres.ElementAt(i)));
            }
            Console.WriteLine("-----------------------");
        }
    }

    
    class Constants
    {
        internal const string CONST_LOSTFILM_SERIAL_LIST = "http://www.lostfilm.tv/serials.php";
        internal const string CONST_LOSTFILM_RSS = "http://www.lostfilm.tv/rssdd.xml";
        internal const string CONST_PROXY_LIST_TEMPLATE = "http://txt.proxyspy.net/proxy.txt";
        internal const string CONST_COUNTRIES_LIST = "http://www.geonames.org/countries/";

        internal static readonly Dictionary<string, List<string>> countries = new Dictionary<string, List<string>>
        {
        {"EU", new List<string> {"AD", "AL", "AT", "AX", "BA", "BE", "BG", "BY", "CH", "CS", "CY", "CZ", "DE", "DK", "EE", "ES", "FI", "FO", "FR", "GB", "GG", "GI", "GR",
                                    "HR", "HU", "IE", "IM", "IS", "IT", "JE", "LI", "LT", "LU", "LV", "MC", "MD", "ME", "MK", "MT", "NL", "NO", "PL", "PT", "RO", "RS", "RU", 
                                    "SE", "SI", "SJ", "SK", "SM", "UA", "VA", "XK"}},
        {"AS", new List<string> {"AE", "AF", "AM", "AZ", "BD", "BH", "BN", "BT", "CC", "CN", "CX", "GE", "HK", "ID", "IL", "IN", "IO", "IQ", "IR", "JO", "JP", "KG", "KH", 
                                    "KP", "KR", "KW", "KZ", "LA", "LB", "LK", "MM", "MN", "MO", "MV", "MY", "NP", "OM", "PH", "PK", "PS", "QA", "SA", "SG", "SY", "TH", "TJ", 
                                    "TM", "TR", "TW", "UZ", "VN", "YE"}},
        {"NA", new List<string> {"AG", "AI", "AN", "AW", "BB", "BL", "BM", "BQ", "BS", "BZ", "CA", "CR", "CU", "CW", "DM", "DO", "GD", "GL", "GP", "GT", "HN", "HT", "JM", 
                                    "KN", "KY", "LC", "MF", "MQ", "MS", "MX", "NI", "PA", "PM", "PR", "SV", "SX", "TC", "TT", "US", "VC", "VG", "VI"}},
        {"AF", new List<string> {"AO", "BF", "BI", "BJ", "BW", "CD", "CF", "CG", "CI", "CM", "CV", "DJ", "DZ", "EG", "EH", "ER", "ET", "GA", "GH", "GM", "GN", "GQ", "GW", 
                                    "KE", "KM", "LR", "LS", "LY", "MA", "MG", "ML", "MR", "MU", "MW", "MZ", "NA", "NE", "NG", "RE", "RW", "SC", "SD", "SH", "SL", "SN", "SO", 
                                    "SS", "ST", "SZ", "TD", "TG", "TN", "TZ", "UG", "YT", "ZA", "ZM", "ZW"}},
        {"AN", new List<string> {"AQ", "BV", "GS", "HM", "TF"}},
        {"SA", new List<string> {"AR", "BO", "BR", "CL", "CO", "EC", "FK", "GF", "GY", "PE", "PY", "SR", "UY", "VE"}},
        {"OC", new List<string> {"AS", "AU", "CK", "FJ", "FM", "GU", "KI", "MH", "MP", "NC", "NF", "NR", "NU", "NZ", "PF", "PG", "PN", "PW", "SB", "TK", "TL", "TO", "TV", 
                                    "UM", "VU", "WF", "WS"}}
        };

        internal static readonly Dictionary<string, Regex> reParams = new Dictionary<string, Regex>()
        {
            {"country", new Regex(@"((?<=^\tСтрана: )[A-Za-zА-Яа-я ,\/]+(?=\.\r$|\r$))", RegexOptions.Multiline)},
            {"reliseYear", new Regex(@"((?<=^\tГод выхода: <span>)\d{4})", RegexOptions.Multiline)},
            {"genre", new Regex(@"((?<=^\tЖанр: <span>)[A-Za-zА-Яа-я ,\/\.]+(?=</span>))", RegexOptions.Multiline)},
            {"status", new Regex(@"((?<=^\tСтатус: )[A-Za-zА-Яа-я ,\/]+(?=\r$))", RegexOptions.Multiline)},
            {"countrySplit", new Regex(@"(?:(?:,)|(?:/))")},
            {"genreSplit", new Regex(@"(?:(?:,)|(?:/)|(?:\.))")}
        };
    }
}
