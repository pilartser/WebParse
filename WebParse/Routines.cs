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

        internal static long GetInt64(object value)
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

        internal static string GetProxyFromBitmap(string url)
        {
            var result = "";
            var proxyGif = new Bitmap(Connection.GetStream(url));
            for (var i = 0; i < 5; i++)
            {
                using (var digit = proxyGif.Clone(new Rectangle(0 + i * 6, 3, 5, 8), proxyGif.PixelFormat))
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
