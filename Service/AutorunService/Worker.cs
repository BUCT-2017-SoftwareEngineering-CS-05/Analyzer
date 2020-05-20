using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Diagnostics;

namespace AutorunService
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly ILogger<Worker> _logger;

        private Timer taskTimer;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
         public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service running.");

            taskTimer = new Timer(CallWorker, null, TimeSpan.Zero,
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }
         public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service is stopping.");

            taskTimer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            //taskTimer.Stop();
            taskTimer?.Dispose();
        }

        private void CallWorker(object state)
        {
            _logger.LogInformation("Crawler executed at: {time}", DateTimeOffset.Now);

            //var tcs = new TaskCompletionSource<int>();

            ProcessStartInfo PSI = new ProcessStartInfo
            {
                FileName = "dotnet.exe",
                Arguments = string.Format("\"{0}\" {1}", Path.Combine(Directory.GetCurrentDirectory(), @"Worker\AnalyzerCrawler.dll"), ""),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using (Process process = Process.Start(PSI))
            using (StreamReader reader = process.StandardOutput)
            {
                while (process.StandardOutput.Peek() > -1)
                {
                    _logger.LogInformation(process.StandardOutput.ReadLine());
                }
                _logger.LogInformation(process.StandardOutput.ReadToEnd());
                while (process.StandardError.Peek() > -1)
                {
                    _logger.LogError(process.StandardError.ReadLine());
                }

                process.WaitForExit();
            }

            _logger.LogInformation("Crawler finished at: {time}", DateTimeOffset.Now);
        }
    }
}
