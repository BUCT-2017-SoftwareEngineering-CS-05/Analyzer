using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;

namespace AnalyzerCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            try
            {
                RequestClient.GetConfigrations();

                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                ScriptSource script = engine.CreateScriptSourceFromFile(@"Crawler/crawler.py");

                var result = script.Execute(scope);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
