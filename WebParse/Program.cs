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
        static void Main(string[] args)
        {
            //Console.WriteLine(Routines.GetProxyFromBitmap("http://hideme.ru/images/proxylist_port_16127288.gif"));
            ProxyList pl = new ProxyList();
            

            //HtmlDocument doc = Routines.GetShowInfoDoc("http://www.lostfilm.tv/browse.php?cat=119");
            //foreach (TVShow show in GetSerialList())
            //{
            //    show.LoadInfo();
            //    Console.WriteLine(String.Format("ID: {0}\r\nName Original: {1}\r\nName Translated: {2}\r\nRelise Year: {3}", show.Id, show.NameOriginal, show.NameTranslated, show.ReliseYear));
            //}
            Console.WriteLine("It's ALL!!!");
            Console.ReadKey();
        }

        static IEnumerable<TVShow> GetSerialList()
        {
            HtmlNode node = Routines.GetDoc(Constants.CONST_LOSTFILM_SERIAL_LIST).DocumentNode.SelectSingleNode("//div[@class=\"mid\"]/div[@class=\"bb\"]");
            foreach (HtmlNode serial in node.SelectNodes("a"))
            {
                Int64 id = Routines.GetInt64(System.Web.HttpUtility.ParseQueryString(serial.GetAttributeValue("href", "").Replace("?", "&")).Get("cat"));
                string NameOriginal = serial.SelectSingleNode("span").InnerText.TrimStart('(').TrimEnd(')');
                string NameTranslated = serial.SelectSingleNode("text()").InnerText;
                yield return new TVShow(id, NameOriginal, NameTranslated);
            }
        }
        static void GetSerialInfo(HtmlNode node)
        {
            Console.WriteLine(node.OuterHtml);
            foreach (string key in Constants.reParams.Keys)
            {
                Console.WriteLine(key);
                Console.WriteLine(Constants.reParams[key].Match(node.InnerHtml).Value.ToString());
            }
        }



        static DateTime GetLastDate()
        {
            using (XmlReader reader = XmlReader.Create(Constants.CONST_LOSTFILM_RSS))
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
                                    return DateTime.TryParseExact(el.Value,
                                    "ddd, dd MMM yyyy H:mm:ss zzz",
                                    provider: System.Globalization.CultureInfo.CreateSpecificCulture("en-US"),
                                    style: System.Globalization.DateTimeStyles.None,
                                    result: out dt) ? dt : default(DateTime);
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
