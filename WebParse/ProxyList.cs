using System;
using System.Collections.Generic;
using System.Linq;
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
            _dict = new Dictionary<string, List<string>>();
            _curCountry = "";
            _curAddress = "";
            FillProxyList();
            SetCurrentAddress();
        }

        private void FillProxyList()
        {
            Console.WriteLine("Proxy list refreshed!");
            var proxyPage = Connection.GetStreamString(Constants.ConstProxyListTemplate);
            var reProxies = new Regex(@"(?:(?<address>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,5})(?: )(?<country>[A-Z]{1,2})(?:(?<!RU)\-)(?:[N](?!\-S|!)))");
            foreach (Match m in reProxies.Matches(proxyPage))
            {
                var address = m.Groups["address"].Value;
                var country = m.Groups["country"].Value;
                if (!Constants.Countries["EU"].Contains(country) && country != "US") continue;
                if (!_dict.ContainsKey(country))
                    _dict.Add(country, new List<string>());
                _dict[country].Add(address);
            }
            PrintProxyList();
        }

        public void PrintProxyList()
        {
            var totalCount = 0;
            foreach (var key in _dict.Keys)
            {
                Console.WriteLine($"{key} contains {_dict[key].Count.ToString()} addresses");
                totalCount += _dict[key].Count;
                foreach (var address in _dict[key])
                {
                    Console.WriteLine(address);
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Total proxy count: {totalCount} \r\n");
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
            _curAddress = _curCountry != "" ? _dict[_curCountry].ElementAt(0) : "";
        }

        public string GetCurrentAddress()
        {
            var tryCount = 15;
            while ((!CheckProxy(_curAddress))&&(tryCount > 0))
            {
                Console.WriteLine($"Proxy {_curAddress} doesn't work!!!");
                GetNextAddress();
                tryCount--;
            }
            return tryCount == 0 ? "" : _curAddress;
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

        private static bool CheckProxy(string proxy)
        {
            try
            {
                var wr = (HttpWebResponse)Connection.GetResponse("http://www.google.com", true, proxy);
                var s = wr.GetResponseStream();
                if (s != null)
                {
                    return (wr.StatusCode == HttpStatusCode.OK) &&
                           (new StreamReader(s).ReadToEnd() != "");
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
