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

                GoPython(@"Crawler/crawler.py");

            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
            
            }
        }

        public static void GoPython(string pythonFile, string moreArgs = "")
        {
            decimal count = 0;
            string err = "";
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
            {
                while (process.StandardOutput.Peek() > -1)
                {
                    var r = RequestClient.NewsPost(process.StandardOutput.ReadLine());
                    if (int.TryParse(r, out int num))
                    {
                        Console.WriteLine("News Posted " + num + " piece(s).");
                        count += num;
                    }
                    else throw new Exception("Error Response: \n" + r);
                }
                Console.WriteLine("In total: " + count);
                
                while (process.StandardError.Peek() > -1)
                {
                    err += process.StandardError.ReadLine();
                }
            }
            if (err.Length > 0)
            {
                throw new Exception(err);
            }
        }
    }
}
