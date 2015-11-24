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
            if (doc.DocumentNode.SelectSingleNode("//div[@class=\"mid\"]/div[img]/div/p[@align=\"left\"]") != null) //Плашка "Залочено на территории РФ" в инфе сериала
            {
                doc = GetDoc(url, true);
            }
            return doc;
        }

        internal static Stream GetStream(string url, Boolean isUseProxy = false)
        {
            int tryCount = 0;
            while (tryCount < 15)
            {
                try
                {
                    System.Net.WebRequest myRequest = WebRequest.Create(url);
                    if (isUseProxy)
                    {
                        string address = (tryCount==0)?Program.pl.GetCurrentAddress():Program.pl.GetNextAddress();
                        if (address != "")
                        {
                            Console.WriteLine(String.Format("Use proxy address: {0}", address));
                            myRequest.Proxy = new WebProxy(address);
                        }
                        else
                            return null;
                    }
                    WebResponse myResponse = myRequest.GetResponse();
                    return myResponse.GetResponseStream();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    tryCount += 1;
                }
            }
            Console.WriteLine("!!!Connection failed!!!");
            return null;
        }

        internal static string GetStreamString(string url, Boolean isUseProxy = false)
        {
            Stream str = GetStream(url, isUseProxy);
            try
            {
                if (str != null)
                    return new StreamReader(str, Encoding.GetEncoding(1251)).ReadToEnd();
            }
            catch(Exception e)
            {
                Program.pl.GetNextAddress();
                Console.WriteLine(e.Message);
                return "";
            }
            return "";
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
