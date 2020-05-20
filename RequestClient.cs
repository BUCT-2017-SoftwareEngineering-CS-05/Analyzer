using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace AnalyzerCrawler
{
    class RequestClient
    {
        public static void GetConfigrations()
        {
            string ip = ConfigurationManager.AppSettings["Server"];
            string pref = ConfigurationManager.AppSettings["Prefix"];
            string[] routes = ConfigurationManager.AppSettings["Postfixs"].Split(',');

            var baseurl = new Uri(new Uri(ip), pref);
            foreach (var r in routes)
            {
                var url = new Uri(baseurl, r).ToString();

                var result = HttpGet(url);

                var fs = new FileStream(r + ".json", FileMode.Create);
                fs.Write(Encoding.ASCII.GetBytes(Regex.Replace(result, @"[^\x00-\x7F]", c =>
    string.Format(@"\u{0:x4}", (int)c.Value[0]))).AsSpan());
                fs.Flush(); fs.Close();
            }
        }

        static string HttpGet(string url, List<KeyValuePair<string, string>> formData = null)
        {
            HttpClient httpClient = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
            };
            if (formData != null)
            {
                HttpContent content = new FormUrlEncodedContent(formData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "UTF-8"
                };
                for (int i = 0; i < formData.Count; i++)
                {
                    content.Headers.Add(formData[i].Key, formData[i].Value);
                    request.Headers.Add(formData[i].Key, formData[i].Value);
                }
            }
            
            var res = httpClient.SendAsync(request);
            res.Wait();
            var resp = res.Result;
            Task<string> temp = resp.Content.ReadAsStringAsync();
            temp.Wait();
            return temp.Result;
        }
        static string HttpPost(string url, string jsonstr)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonstr);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
        public static string NewsPost(string jsonstr)
        {
            string ip = ConfigurationManager.AppSettings["Server"];
            string route = ConfigurationManager.AppSettings["NewsPost"];
            var url = new Uri(new Uri(ip), route);

            return HttpPost(url.ToString(), jsonstr);
        }
    }
}
