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
        private Dictionary<string, List<string>> _dict;
        private string _curCountry;
        private string _curAddress;

        public ProxyList()
        {
            _dict = new Dictionary<string, List<string>> { };
            _curCountry = "";
            _curAddress = "";
            FillProxyList();
            SetCurrentAddress();
        }

        private void FillProxyList()
        {
            Console.WriteLine("Proxy list refreshed!");
            String proxyPage = Connection.GetStreamString(Constants.ConstProxyListTemplate);
            Regex reProxies = new Regex(@"(?:(?<address>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,5})(?: )(?<country>[A-Z]{1,2})(?:(?<!RU)\-)(?:[N](?!\-S|!)))");
            foreach (Match m in reProxies.Matches(proxyPage))
            {
                string address = m.Groups["address"].Value;
                string country = m.Groups["country"].Value;
                if (Constants.Countries["EU"].Contains(country) || country == "US")
                {
                    if (!_dict.ContainsKey(country))
                        _dict.Add(country, new List<string> { });
                    _dict[country].Add(address);
                }
            }
            PrintProxyList();
        }

        public void PrintProxyList()
        {
            int totalCount = 0;
            foreach (string key in _dict.Keys)
            {
                Console.WriteLine(String.Format("{0} contains {1} addresses", key, _dict[key].Count.ToString()));
                totalCount += _dict[key].Count;
                foreach (string address in _dict[key])
                {
                    Console.WriteLine(address);
                }
                Console.WriteLine();
            }
            Console.WriteLine(String.Format("Total proxy count: {0} \r\n", totalCount));
        }

        private void SetCurrentAddress()
        {
            if (_dict.Keys.Count > 0)
            {
                if (_dict.Keys.Contains("UA"))
                    _curCountry = "UA";
                else if (_dict.Keys.Contains("BY"))
                    _curCountry = "BY";
                else
                    _curCountry = _dict.Keys.ElementAt(0);
            }
            else
                _curCountry = "";
            if (_curCountry != "")
                _curAddress = _dict[_curCountry].ElementAt(0);
            else
                _curAddress = "";
        }

        public string GetCurrentAddress()
        {
            int tryCount = 15;
            while ((!CheckProxy(_curAddress))&&(tryCount > 0))
            {
                Console.WriteLine(String.Format("Proxy {0} doesn't work!!!", _curAddress));
                GetNextAddress();
                tryCount--;
            }
            if (tryCount == 0)
                return "";
            else
                return _curAddress;
        }

        private void GetNextAddress()
        {
            if ((_curCountry != "") && (_curAddress != ""))
            {
                _dict[_curCountry].Remove(_curAddress);
                if (_dict[_curCountry].Count == 0)
                    _dict.Remove(_curCountry);
                if (_dict.Keys.Count == 0)
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
