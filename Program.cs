using System;
using System.Diagnostics;
using System.IO;

namespace AnalyzerCrawler
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                RequestClient.GetConfigrations();

                var res = GoPython(@"Crawler/crawler.py");

                if (res.Item1.Length == 0 && res.Item2.Length > 0)
                {
                    throw new Exception("Errors in script:\n" + res.Item2);
                }

                var r = RequestClient.NewsPost(res.Item1);
                if(int.TryParse(r,out int num))
                {
                    Console.WriteLine("News Posted "+num+ " piece(s).");

                }else throw new Exception("Error Response: \n" + r);

                if (res.Item2.Length > 0)
                {
                    throw new Exception("With errors in script:\n" + res.Item2);
                }
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
            
            }
        }

        public static Tuple<string, string> GoPython(string pythonFile, string moreArgs = "")
        {
            ProcessStartInfo PSI = new ProcessStartInfo
            {
                FileName = "py.exe",
                Arguments = string.Format("\"{0}\" {1}", Path.Combine(Directory.GetCurrentDirectory(), pythonFile), moreArgs),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            using (Process process = Process.Start(PSI))
            using (StreamReader reader = process.StandardOutput)
            {
                bool hasError = false;
                string stderr = "";
                process.ErrorDataReceived += (sender, e) =>
                {
                    hasError = true;
                };
                if (hasError)
                {
                    stderr = process.StandardError.ReadToEnd();

                }
                string result = reader.ReadToEnd();
                return new Tuple<string, string>(result, stderr);
            }
        }
    }
}
