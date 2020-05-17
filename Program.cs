using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AnalyzerCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            try
            {
                //RequestClient.GetConfigrations();

                var res = GoPython(@"Crawler/crawler.py");

                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static Tuple<String, String> GoPython(string pythonFile, string moreArgs = "")
        {
            ProcessStartInfo PSI = new ProcessStartInfo();
            PSI.FileName = "python.exe";
            PSI.Arguments = string.Format("\"{0}\" {1}", Path.Combine(Directory.GetCurrentDirectory(), pythonFile), moreArgs);
            PSI.CreateNoWindow = true;
            PSI.UseShellExecute = false;
            PSI.RedirectStandardError = true;
            PSI.RedirectStandardOutput = true;
            using (Process process = Process.Start(PSI))
            using (StreamReader reader = process.StandardOutput)
            {
                string stderr = process.StandardError.ReadToEnd(); // Error(s)!!
                string result = reader.ReadToEnd(); // What we want.
                return new Tuple<String, String>(result, stderr);
            }
        }
    }
}
