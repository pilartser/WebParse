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
        public List<string> Country;
        public List<string> Genre;
        public Boolean isOpen;

        public TVShow(Int64 idShow, string nameShow, string nameTranslated)
        {
            Id = idShow;
            NameOriginal = nameShow;
            NameTranslated = nameTranslated;
            Country = new List<string> {};
            Genre = new List<string> {};
            isOpen = true;
        }

        public void LoadInfo()
        {
            HtmlDocument doc = Routines.GetShowInfoDoc(String.Format("http://www.lostfilm.tv/browse.php?cat={0}", this.Id));
            HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[@class=\"mid\"]/div[img]");
            this.ReliseYear = Routines.GetInt(Constants.reParams["relise_year"].Match(node.InnerHtml).Value);
        }
    }

    class ProxyList
    {
        public ProxyList()
        {
            HtmlDocument doc = Routines.GetDoc(String.Format(Constants.CONST_PROXY_LIST_TEMPLATE, "UA"));
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//html/body/table[@cellspacing=\"0\"]/tr/td/table/tr[td/font[@class=\"spy1\"]/text()=\"HTTP\"]"))
                Console.WriteLine(node.InnerHtml + "\r\n");
        }
    }

    class Constants
    {
        internal const string CONST_LOSTFILM_SERIAL_LIST = "http://www.lostfilm.tv/serials.php";
        internal const string CONST_LOSTFILM_RSS = "http://www.lostfilm.tv/rssdd.xml";
        internal const string CONST_PROXY_LIST_TEMPLATE = "http://spys.ru/proxys/{0}";

        internal static readonly Dictionary<string, Regex> reParams = new Dictionary<string, Regex>()
        {
            {"country", new Regex(@"((?<=^\tСтрана: )[A-Za-zА-Яа-я ,\/]+(?=\.\r$|\r$))", RegexOptions.Multiline)},
            {"relise_year", new Regex(@"((?<=^\tГод выхода: <span>)\d{4})", RegexOptions.Multiline)},
            {"genre", new Regex(@"((?<=^\tЖанр: <span>)[A-Za-zА-Яа-я ,\/]+(?=</span>))", RegexOptions.Multiline)},
            {"status", new Regex(@"((?<=^\tСтатус: )[A-Za-zА-Яа-я ,\/]+(?=\r$))", RegexOptions.Multiline)}
        };
    }
}
