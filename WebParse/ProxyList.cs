using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace WebParse
{
    class ProxyList
    {
        private Dictionary<string, List<string>> dict;
        private string curCountry;
        private string curAddress;

        public ProxyList()
        {
            dict = new Dictionary<string, List<string>> { };
            curCountry = "";
            curAddress = "";
            FillProxyList();
            SetCurrentAddress();
        }

        private void FillProxyList()
        {
            Console.WriteLine("Proxy list refreshed!");
            String proxyPage = Connection.GetStreamString(Constants.CONST_PROXY_LIST_TEMPLATE);
            Regex reProxies = new Regex(@"(?:(?<address>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,5})(?: )(?<country>[A-Z]{1,2})(?:(?<!RU)\-)(?:[N](?!\-S|!)))");
            foreach (Match m in reProxies.Matches(proxyPage))
            {
                string address = m.Groups["address"].Value;
                string country = m.Groups["country"].Value;
                if (Constants.countries["EU"].Contains(country) || country == "US")
                {
                    if (!dict.ContainsKey(country))
                        dict.Add(country, new List<string> { });
                    dict[country].Add(address);
                }
            }
            PrintProxyList();
        }

        public void PrintProxyList()
        {
            int totalCount = 0;
            foreach (string key in dict.Keys)
            {
                Console.WriteLine(String.Format("{0} contains {1} addresses", key, dict[key].Count.ToString()));
                totalCount += dict[key].Count;
                foreach (string address in dict[key])
                {
                    Console.WriteLine(address);
                }
                Console.WriteLine();
            }
            Console.WriteLine(String.Format("Total proxy count: {0} \r\n", totalCount));
        }

        private void SetCurrentAddress()
        {
            if (dict.Keys.Count > 0)
            {
                if (dict.Keys.Contains("UA"))
                    curCountry = "UA";
                else if (dict.Keys.Contains("BY"))
                    curCountry = "BY";
                else
                    curCountry = dict.Keys.ElementAt(0);
            }
            else
                curCountry = "";
            if (curCountry != "")
                curAddress = dict[curCountry].ElementAt(0);
            else
                curAddress = "";
        }

        public string GetCurrentAddress()
        {
            int tryCount = 15;
            while ((!CheckProxy(curAddress))&&(tryCount > 0))
            {
                Console.WriteLine(String.Format("Proxy {0} doesn't work!!!", curAddress));
                GetNextAddress();
                tryCount--;
            }
            if (tryCount == 0)
                return "";
            else
                return curAddress;
        }

        private void GetNextAddress()
        {
            if ((curCountry != "") && (curAddress != ""))
            {
                dict[curCountry].Remove(curAddress);
                if (dict[curCountry].Count == 0)
                    dict.Remove(curCountry);
                if (dict.Keys.Count == 0)
                    FillProxyList();
            }
            else
                FillProxyList();
            SetCurrentAddress();
        }

        private Boolean CheckProxy(string proxy)
        {
            try
            {
                WebResponse wr = Connection.GetResponse("http://www.google.com", true, proxy);
                StreamReader sr = new StreamReader(wr.GetResponseStream());
                return ((wr != null) && ((wr as HttpWebResponse).StatusCode == HttpStatusCode.OK) && (sr.ReadToEnd() != "")) ? true : false;
            }
            catch
            {
                return false;
            }
        }
    }
}
