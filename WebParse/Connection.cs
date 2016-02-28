using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace WebParse
{
    class Connection
    {
        internal static HtmlDocument GetDoc(string url, Boolean isUseProxy = false)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.OptionWriteEmptyNodes = true;
            string html = GetStreamString(url, isUseProxy);
            if (html != "")
            {
                doc.LoadHtml(html);
                foreach (HtmlNode brNode in doc.DocumentNode.SelectNodes("//br"))
                {
                    brNode.Remove();
                }

                return doc;
            }
            else
                return null;
        }

        internal static HtmlDocument GetShowInfoDoc(string url)
        {
            HtmlDocument doc = GetDoc(url);
            if ((doc == null) || (doc.DocumentNode.SelectSingleNode("//div[@class=\"mid\"]/div[img]/div/p[@align=\"left\"]") != null)) //Плашка "Залочено на территории РФ" в инфе сериала
            {
                doc = GetDoc(url, true);
            }
            return doc;
        }

        internal static WebResponse GetResponse(string url, Boolean isUseProxy = false, string proxy = "")
        {
            try
            {
                System.Net.WebRequest myRequest = WebRequest.Create(url);
                if (isUseProxy)
                {
                    myRequest.Proxy = new WebProxy((proxy != "") ? proxy : Program.Pl.GetCurrentAddress());
                }
                return myRequest.GetResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        internal static Stream GetStream(string url, Boolean isUseProxy = false)
        {
            try
            {
                WebResponse myResponse = GetResponse(url, isUseProxy);
                return (myResponse != null) ? myResponse.GetResponseStream() : null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        

        internal static string GetStreamString(string url, Boolean isUseProxy = false)
        {
            Stream str = GetStream(url, isUseProxy);
            try
            {
                if (str != null)
                    return new StreamReader(str, Encoding.GetEncoding(1251)).ReadToEnd();
            }
            catch (Exception e)
            {
                //Program.pl.GetNextAddress();
                Console.WriteLine(e.Message);
                return "";
            }
            return "";
        }
    }
}
