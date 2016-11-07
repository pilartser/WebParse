using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.Xml;
using System.Xml.Linq;

namespace WebParse
{
    class Program
    {
        public static ProxyList Pl;

        static void Main()
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
            var show = new LostFilmShow(24, "BattleStar Galactica", "Звёздный крейсер Галактика");
            show.LoadInfo();
            show.PrintInfo();
            Console.WriteLine("It's ALL!!!");
            Console.ReadKey();
        }

        static IEnumerable<LostFilmShow> GetSerialList()
        {
            var node = Connection.GetDoc(Constants.ConstLostfilmSerialList).DocumentNode.SelectSingleNode("//div[@class=\"mid\"]/div[@class=\"bb\"]");
            return node.SelectNodes("a").Select(
                            serial => new LostFilmShow(Routines.GetInt64(System.Web.HttpUtility.ParseQueryString(serial.GetAttributeValue("href", "").Replace("?", "&")).Get("cat")),
                                                        serial.SelectSingleNode("span").InnerText.Trim(new char[] { '(', ')' }), 
                                                        serial.SelectSingleNode("text()").InnerText));
        }

        static void GetSerialInfo(HtmlNode node)
        {
            Console.WriteLine(node.OuterHtml);
            foreach (var key in Constants.ReParams.Keys)
            {
                Console.WriteLine(key);
                Console.WriteLine(Constants.ReParams[key].Match(node.InnerHtml).Value);
            }
        }



        static DateTime GetLastDate()
        {
            using (var reader = XmlReader.Create(Constants.ConstLostfilmRss))
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
                                var el = XNode.ReadFrom(reader) as XElement;
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
