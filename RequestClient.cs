using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.IO;

namespace AnalyzerCrawler
{
    class RequestClient
    {
        public static void GetConfigrations()
        {
            string ip = ConfigurationManager.AppSettings["ServerIP"];
            string pref = ConfigurationManager.AppSettings["Prefix"];
            string[] routes = ConfigurationManager.AppSettings["Postfixs"].Split(',');

            var baseurl = new Uri(new Uri(ip), pref);
            foreach (var r in routes)
            {
                var url = new Uri(baseurl, r).ToString();

                var result = HttpGet(url);

                var fs = new FileStream(r + ".json", FileMode.Truncate);
                fs.Write(Encoding.ASCII.GetBytes(result).AsSpan());
                fs.Flush(); fs.Close();
            }
        }

        static string HttpGet(string url, List<KeyValuePair<string, string>> formData = null)
        {
            HttpClient httpClient = new HttpClient();
            HttpContent content = new FormUrlEncodedContent(formData);
            if (formData != null)
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "UTF-8"
                };
                for (int i = 0; i < formData.Count; i++)
                {
                    content.Headers.Add(formData[i].Key, formData[i].Value);
                }
            }
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
            };
            for (int i = 0; i < formData.Count; i++)
            {
                request.Headers.Add(formData[i].Key, formData[i].Value);
            }
            var res = httpClient.SendAsync(request);
            res.Wait();
            var resp = res.Result;
            Task<string> temp = resp.Content.ReadAsStringAsync();
            temp.Wait();
            return temp.Result;
        }
    }
}
