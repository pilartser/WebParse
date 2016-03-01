using System;
using System.Text;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace WebParse
{
    class Connection
    {
        internal static HtmlDocument GetDoc(string url, bool isUseProxy = false)
        {
            var doc = new HtmlDocument {OptionWriteEmptyNodes = true};
            var html = GetStreamString(url, isUseProxy);
            if (html != "")
            {
                doc.LoadHtml(html);
                foreach (var brNode in doc.DocumentNode.SelectNodes("//br"))
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
            var doc = GetDoc(url);
            if ((doc == null) || (doc.DocumentNode.SelectSingleNode("//div[@class=\"mid\"]/div[img]/div/p[@align=\"left\"]") != null)) //Плашка "Залочено на территории РФ" в инфе сериала
            {
                doc = GetDoc(url, true);
            }
            return doc;
        }

        internal static WebResponse GetResponse(string url, bool isUseProxy = false, string proxy = "")
        {
            try
            {
                var myRequest = WebRequest.Create(url);
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

        internal static Stream GetStream(string url, bool isUseProxy = false)
        {
            try
            {
                var myResponse = GetResponse(url, isUseProxy);
                return myResponse?.GetResponseStream();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        

        internal static string GetStreamString(string url, bool isUseProxy = false)
        {
            var str = GetStream(url, isUseProxy);
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
