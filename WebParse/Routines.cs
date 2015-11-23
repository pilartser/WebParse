using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Drawing;

namespace WebParse
{
    class Routines
    {

        internal static Int64 GetInt64(object value)
        {
            if (!Equals(value, null))
            {
                try
                {
                    return Convert.ToInt64(value);
                }
                catch
                {
                    return -1;
                }
            }
            else
                return -1;
        }

        internal static int GetInt(object value)
        {
            if (!Equals(value, null))
            {
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
                return 0;
        }

        internal static HtmlDocument GetDoc(string url, Boolean isUseProxy = false)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.OptionWriteEmptyNodes = true;
            doc.LoadHtml(GetStreamString(url, isUseProxy));
            foreach (HtmlNode brNode in doc.DocumentNode.SelectNodes("//br"))
            {
                brNode.Remove();
            }

            return doc;
        }

        internal static HtmlDocument GetShowInfoDoc(string url)
        {
            HtmlDocument doc = GetDoc(url);
            if (doc.DocumentNode.SelectSingleNode("//div[@class=\"mid\"]/div[img]/div/p[@align=\"left\"]") != null) //Плашка "Залочено на территории РФ" в инфе сериала
            {
                doc = GetDoc(url, true);
            }
            return doc;
        }

        internal static Stream GetStream(string url, Boolean isUseProxy = false)
        {
            /*HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.CookieContainer = new CookieContainer();
            myRequest.CookieContainer.Add(new Cookie("__utma", "27137956.1295951341.1448268557.1448268557.1448268557.1", "/", ".hideme.ru"));
            myRequest.CookieContainer.Add(new Cookie("__utmb", "27137956.2.9.1448270854583", "/", ".hideme.ru"));
            myRequest.CookieContainer.Add(new Cookie("__utmc", "27137956", "/", ".hideme.ru"));
            myRequest.CookieContainer.Add(new Cookie("__utmz", "27137956.1447224364.5.4.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided)", "/", ".hideme.ru"));
            myRequest.CookieContainer.Add(new Cookie("_ym_isad", "1", "/", ".hideme.ru"));
            myRequest.CookieContainer.Add(new Cookie("_ym_uid", "1448270861172438361", "/", ".hideme.ru"));
            myRequest.CookieContainer.Add(new Cookie("jv_enter_ts_PeHzjrJoSL", "1448270860901", "/", ".hideme.ru"));
            myRequest.CookieContainer.Add(new Cookie("jv_gui_state_PeHzjrJoSL", "WIDGET", "/", ".hideme.ru"));
            myRequest.CookieContainer.Add(new Cookie("jv_invitation_time_PeHzjrJoSL", "1448270854578", "/", ".hideme.ru"));
            myRequest.CookieContainer.Add(new Cookie("jv_pages_count_PeHzjrJoSL", "1", "/", ".hideme.ru"));
            myRequest.CookieContainer.Add(new Cookie("jv_refer_PeHzjrJoSL", "http%3A%2F%2Fhideme.ru%2Fproxy-list%2F%3Fcountry%3DUA%26maxtime%3D400%26type%3Dh", "/", ".hideme.ru"));
            myRequest.CookieContainer.Add(new Cookie("jv_visits_count_PeHzjrJoSL", "1", "/", ".hideme.ru"));
            myRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";*/
            System.Net.WebRequest myRequest = WebRequest.Create(url);
            if (isUseProxy)
                myRequest.Proxy = new WebProxy("http://195.138.78.222:8080"/*"http://37.53.91.4:3129"*/);
            WebResponse myResponse = myRequest.GetResponse();
            return myResponse.GetResponseStream();
        }

        internal static string GetStreamString(string url, Boolean isUseProxy = false)
        {
            return new StreamReader(GetStream(url, isUseProxy), Encoding.GetEncoding(1251)).ReadToEnd();
        }

        internal static string GetProxyFromBitmap(string url)
        {
            string result = "";
            Bitmap proxyGif = new Bitmap(Routines.GetStream(url));
            for (int i = 0; i < 5; i++)
            {
                using (Bitmap digit = proxyGif.Clone(new Rectangle(0 + i * 6, 3, 5, 8), proxyGif.PixelFormat))
                {
                    result += Routines.GetDigitFromBitmap(digit);
                }
            }
            return result;
        }

        internal static string GetDigitFromBitmap(Bitmap digit)
        {
            if (digit.GetPixel(0, 0).Name != "ffffff") //Верхний левый угол
            {
                //5,7
                if (digit.GetPixel(0, 1).Name != "ffffff")
                    return "5";
                else
                    return "7";
            }
            else if (digit.GetPixel(4, 0).Name != "ffffff") //Верхний правый угол, за исключением вышеперечисленных
            {
                //6
                return "6";
            }
            else if (digit.GetPixel(0, 7).Name != "ffffff") //Нижний левый угол, за исключением вышеперечисленных
            {
                //1,2,9
                if (digit.GetPixel(4, 7).Name != "ffffff") //Нижний правый угол
                {
                    if (digit.GetPixel(0, 6).Name != "ffffff")
                        return "2";
                    else
                        return "1";
                }
                else
                    return "9";
            }
            else if (digit.GetPixel(0, 1).Name != "ffffff")
            {
                //3,8
                if (digit.GetPixel(0, 2).Name != "ffffff")
                    return "8";
                else
                    return "3";
            }
            else if (digit.GetPixel(0, 3).Name != "ffffff")
            {
                //0,4
                if (digit.GetPixel(3, 0).Name != "ffffff")
                    return "4";
                else
                    return "0";
            }
            else
                return "";
        }
    }
}
