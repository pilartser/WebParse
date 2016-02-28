using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Drawing;

namespace WebParse
{
    class Program
    {
        public static ProxyList Pl;

        static void Main(string[] args)
        {
            //Console.WriteLine(Routines.GetProxyFromBitmap("http://hideme.ru/images/proxylist_port_16127288.gif"));
            Pl = new ProxyList();
            //HtmlDocument doc = Routines.GetShowInfoDoc("http://www.lostfilm.tv/browse.php?cat=119");
            //Console.WriteLine(doc.DocumentNode.InnerHtml);

            //foreach (LostFilmShow show in GetSerialList())
            //{
            //    show.LoadInfo();
            //    show.PrintInfo();
            //}
            LostFilmShow show = new LostFilmShow(24, "BattleStar Galactica", "Звёздный крейсер Галактика");
            show.LoadInfo();
            show.PrintInfo();
            Console.WriteLine("It's ALL!!!");
            Console.ReadKey();
        }

        static IEnumerable<LostFilmShow> GetSerialList()
        {
            HtmlNode node = Connection.GetDoc(Constants.ConstLostfilmSerialList).DocumentNode.SelectSingleNode("//div[@class=\"mid\"]/div[@class=\"bb\"]");
            foreach (HtmlNode serial in node.SelectNodes("a"))
            {
                Int64 id = Routines.GetInt64(System.Web.HttpUtility.ParseQueryString(serial.GetAttributeValue("href", "").Replace("?", "&")).Get("cat"));
                string nameOriginal = serial.SelectSingleNode("span").InnerText.Trim(new char[] {'(', ')'});
                string nameTranslated = serial.SelectSingleNode("text()").InnerText;
                yield return new LostFilmShow(id, nameOriginal, nameTranslated);
            }
        }
        static void GetSerialInfo(HtmlNode node)
        {
            Console.WriteLine(node.OuterHtml);
            foreach (string key in Constants.ReParams.Keys)
            {
                Console.WriteLine(key);
                Console.WriteLine(Constants.ReParams[key].Match(node.InnerHtml).Value.ToString());
            }
        }



        static DateTime GetLastDate()
        {
            using (XmlReader reader = XmlReader.Create(Constants.ConstLostfilmRss))
            {
                reader.MoveToContent();
                // Parse the file and display each of the nodes. 
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "lastBuildDate")
                            {
                                XElement el = XNode.ReadFrom(reader) as XElement;
                                if (el != null)
                                {
                                    DateTime dt;
                                    return DateTime.TryParseExact(el.Value, "ddd, dd MMM yyyy H:mm:ss zzz",
                                                                    provider: System.Globalization.CultureInfo.CreateSpecificCulture("en-US"),
                                                                    style: System.Globalization.DateTimeStyles.None,
                                                                    result: out dt) ? 
                                                    dt : default(DateTime);
                                }
                            }
                            break;
                    }
                }
                return default(DateTime);
            }
        } 
    }
}
