using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSurge.Cli;
using Westwind.Utilities;

namespace WebSurge.Cli

{
    class Program
    {
        static void Main(string[] args)
        {
            var origColor = Console.ForegroundColor;
            

            var options = new CommandLineOptions();

            if (!CommandLine.Parser.Default.ParseArguments(args, options))
                return;


            Console.ForegroundColor = ConsoleColor.White;

            // If SessionFile is a Url assign to Url so we run on a single URL
            if (!string.IsNullOrEmpty(options.SessionFile) &&
                (options.SessionFile.StartsWith("http://") || options.SessionFile.StartsWith("https://")))
            {
                options.Url = options.SessionFile;
                options.SessionFile = null;                
            }
            
            
            var stressTester = new StressTester();

            List<HttpRequestData> requests;
            if (!string.IsNullOrEmpty(options.SessionFile))
                requests = stressTester.ParseSessionFile(options.SessionFile);
            else
            {
                if (string.IsNullOrEmpty(options.Url))
                {
                    Console.WriteLine(options.GetUsage());
                    Console.ForegroundColor = origColor;
                    return;
                }

                requests = new List<HttpRequestData>();
                requests.Add(new HttpRequestData
                {
                    Url =options.Url
                });
            }

            if (options.Silent != 0 && options.Silent != 1)
            {
                Console.WriteLine("West Wind WebSurge v" + GetVersion());
                Console.WriteLine("------------------------");
            }

            int time = options.Time;
            int threads = options.Threads;

            stressTester.Options.DelayTimeMs = options.DelayTimeMs;
            stressTester.Options.RandomizeRequests = options.RandomizeRequests;
            stressTester.Options.WarmupSeconds = options.WarmupSeconds;

            if (options.Silent != 0 && options.Silent != 1)
                stressTester.RequestProcessed += stressTester_RequestProcessed;

            if (options.Silent != 0  && options.Silent != 2)            
                stressTester.Progress += stressTester_Progress;

            Console.ForegroundColor = ConsoleColor.Green;
            var results = stressTester.CheckAllSites(requests,threads,time);
            if (results == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + stressTester.ErrorMessage);
                Console.ForegroundColor = origColor;
                return;
            }
            Console.ForegroundColor = ConsoleColor.White;

            if (options.Json)
            {
                
                var result = stressTester.ResultsParser.GetResultReport(stressTester.Results, 
                    stressTester.TimeTakenForLastRunMs,
                    stressTester.ThreadsUsed);
                string json = JsonSerializationUtils.Serialize(result, formatJsonOutput: true);

                Console.WriteLine(json);
                Console.ForegroundColor = origColor;
                if (options.OutputFile != null)
                    File.WriteAllText(options.OutputFile, json);
                return;
            }

            string resultText = stressTester.ParseResults(results);

            if (options.Silent != 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                Console.WriteLine("Summary:");
                Console.WriteLine("--------");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (options.SessionFile != null)
                Console.WriteLine("Session: " + options.SessionFile);
            else
                Console.WriteLine("Url: " + options.Url);
                

            Console.WriteLine();
            Console.WriteLine(resultText);

            if (options.OutputFile != null)
            {
                File.WriteAllText(options.OutputFile, resultText);
            }

            Console.ForegroundColor = origColor;
        }

        private static object consoleLock = new object();
        static void stressTester_RequestProcessed(HttpRequestData req )
        {
            string output = req.StatusCode + " -  " + req.HttpVerb + " " + req.Url + "(" + req.TimeTakenMs.ToString("n0") + "ms)";
            lock (consoleLock)
            {
                if (req.IsError)
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(output);
                if (req.IsError)
                    Console.ForegroundColor = ConsoleColor.Green;
            }
        }

        private static object consoleLock2 = new object();
        private static int lastFailed = 0;
        static void stressTester_Progress(ProgressInfo progress)
        {            
            if (progress.SecondsProcessed < 1)
                return;
            
            string text = progress.RequestsProcessed.ToString("n0") +
                          " requests, " + progress.RequestsFailed.ToString("n0") + " failed | " +
                          progress.SecondsProcessed + " of " +
                          progress.TotalSecondsToProcessed + " secs " +            
                            "| " + (progress.RequestsProcessed / progress.SecondsProcessed).ToString("n0") + " requests/sec ";

            lock (consoleLock2)
            {
                if (progress.RequestsFailed > lastFailed)
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(text);

                if (progress.RequestsFailed > lastFailed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    lastFailed = progress.RequestsFailed;
                }
            }
        }

        static void WriteError(string message)
        {
            Console.WriteLine("  *** " + message + " ***\r\n");
            Console.WriteLine("Use 'WebSurgeCli -h' to get a list of valid commandline switches.");
        }

        public static string GetVersion()
        {
            var assmbly = Assembly.GetExecutingAssembly();
            var version = FileVersionInfo.GetVersionInfo(assmbly.Location);            
            return version.FileMajorPart + "." + version.FileMinorPart;         
        }

    }
}
