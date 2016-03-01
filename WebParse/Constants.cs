using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;



namespace WebParse
{
    public enum LostFilmSeasonCategory
    {
        None,
        Season,
        Film
    }

    class LostFilmEpisode
    {
        public string Num;
        public string NameOriginal;
        public string NameTranslated;

        public LostFilmEpisode()
        {
            Num = "";
        }
    }

    class LostFilmSeason
    {
        public LostFilmSeasonCategory Type;
        public string Name;
        public string Num;

        public LostFilmSeason()
        {
            Type = LostFilmSeasonCategory.None;
        }

        public LostFilmSeason(HtmlNode row, string nameShowTranslated)
        {
            Type = LostFilmSeasonCategory.None;
            var nodes = row.SelectNodes("div/h2 | div");
            if (nodes == null) return;
            Type = (nodes.Count == 1) ? LostFilmSeasonCategory.Film : LostFilmSeasonCategory.Season;
            Name = new Regex($@"(.+(?=\. {((Type == LostFilmSeasonCategory.Season)?"Сериал ":"") + nameShowTranslated}))").
                            Match(nodes.First().InnerText.Trim()).Value;
        }

        public void AddEpisode(HtmlNode row)
        {
            var node = row.SelectSingleNode("table/tr/td[@class=\"t_episode_title\"]");
            var m = Constants.ReParams["episode"].Match(node.GetAttributeValue("onClick", ""));
            if (m.Success)
            {
                Num = m.Groups["season"].Value;
            }
                Console.WriteLine($" {m.Groups["episode"].Value}");
        }

        public void Print()
        {
            Console.WriteLine($"Раздел {Name} тип {Type} номер {Num}");
        }
    }

    class LostFilmShow
    {
        public long Id;
        public string NameOriginal;
        public string NameTranslated;
        public int ReliseYear;
        public bool IsClose;
        public List<string> Countries;
        public List<string> Genres;
        public List<LostFilmSeason> Seasons;

        public LostFilmShow(long idShow, string nameShow, string nameTranslated)
        {
            Id = idShow;
            NameOriginal = nameShow;
            NameTranslated = nameTranslated;
            IsClose = false;
            Countries = new List<string>();
            Genres = new List<string>();
            Seasons = new List<LostFilmSeason>();
        }

        public void LoadInfo()
        {
            var doc = Connection.GetShowInfoDoc(string.Format(Constants.ConstLostfilmSerialPage, Id));
            if (doc == null) return;
            try
            {
                var node = doc.DocumentNode.SelectSingleNode("//div[@class=\"mid\"]/div[img]");
                if (node == null) return;
                ReliseYear = Routines.GetInt(Constants.ReParams["reliseYear"].Match(node.InnerHtml).Value);
                Countries = Constants.ReParams["countrySplit"].
                    Split(Constants.ReParams["country"].Match(node.InnerHtml).Value).
                    Select(c => c.Trim()).ToList();
                Genres = Constants.ReParams["genreSplit"].
                    Split(Constants.ReParams["genre"].Match(node.InnerHtml).Value).
                    Select(g => g.Trim().ToLower()).Where(s => (s != string.Empty)).ToList();
                IsClose = (Constants.ReParams["status"].Match(node.InnerHtml).Value.ToLower() != "закончен");
                LoadSeasons(doc.DocumentNode.SelectNodes("//div[@class=\"mid\"]/div[div/@class=\"t_row even\"]/div[@class=\"content\" or @class=\"t_row even\" or @class=\"t_row odd\"]"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void LoadSeasons(HtmlNodeCollection rows)
        {
            LostFilmSeason season = null;
            foreach (var row in rows)
            {
                switch (row.GetAttributeValue("class", ""))
                {
                    case "content":
                        if (season != null)
                            Seasons.Add(season);
                        season = new LostFilmSeason(row, NameTranslated);
                        break;
                    case "t_row even":
                    case "t_row odd":
                        season?.AddEpisode(row);
                        break;
                }
            }
            if (season != null)
                Seasons.Add(season);
        }

        public void PrintInfo()
        {
            Console.WriteLine(
                $"ID: {Id}\r\nName Original: {NameOriginal}\r\nName Translated: {NameTranslated}\r\nRelise Year: {ReliseYear}");
            Console.Write($"Countries: {((Countries.Count == 0) ? "\r\n" : "")}");
            for (var i = 0; i < Countries.Count; i++)
            {
                Console.WriteLine($"{"".PadLeft((i == 0) ? 0 : 11)}{Countries.ElementAt(i)}");
            }
            Console.Write($"Genres: {((Genres.Count == 0) ? "\r\n" : "")}");
            for (var i = 0; i < Genres.Count; i++)
            {
                Console.WriteLine($"{"".PadLeft((i == 0) ? 0 : 8)}{Genres.ElementAt(i)}");
            }
            Console.WriteLine($"Status: {(IsClose ? "close" : "open")}");
            Console.WriteLine("Seasons:");
            foreach (var season in Seasons)
            {
                season.Print();
            }
            Console.WriteLine("-----------------------");
        }
    }

    
    class Constants
    {
        internal const string ConstLostfilmSerialList = "http://www.lostfilm.tv/serials.php";
        internal const string ConstLostfilmSerialPage = "http://www.lostfilm.tv/browse.php?cat={0}";
        internal const string ConstLostfilmRss = "http://www.lostfilm.tv/rssdd.xml";
        internal const string ConstProxyListTemplate = "http://txt.proxyspy.net/proxy.txt";
        internal const string ConstCountriesList = "http://www.geonames.org/countries/";

        internal static readonly Dictionary<string, List<string>> Countries = new Dictionary<string, List<string>>
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

        internal static readonly Dictionary<string, Regex> ReParams = new Dictionary<string, Regex>()
        {
            {"reliseYear", new Regex(@"((?<=^\tГод выхода: <span>)\d{4})", RegexOptions.Multiline)},
            {"genre", new Regex(@"((?<=^\tЖанр: <span>)[A-Za-zА-Яа-я ,\/\.]+(?=</span>))", RegexOptions.Multiline)},
            {"status", new Regex(@"((?<=^\tСтатус: )[A-Za-zА-Яа-я ,\/]+(?=\r$))", RegexOptions.Multiline)},
            {"country", new Regex(@"((?<=^\tСтрана: )[A-Za-zА-Яа-я ,\/]+(?=\.\r$|\r$))", RegexOptions.Multiline)},
            {"countrySplit", new Regex(@"(?:(?:,)|(?:/))")},
            {"genreSplit", new Regex(@"(?:(?:,)|(?:/)|(?:\.))")},
            {"episode", new Regex(@"(?:ShowAllReleases\('\d{1,4}',')(?<season>[\d\.]{1,6})(?:',')(?<episode>[\d-]{1,8})")}
        };
    }
}
