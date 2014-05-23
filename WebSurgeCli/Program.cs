using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSurge.Cli;

namespace WebSurge.Cli

{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new CommandLineOptions();

            if (!CommandLine.Parser.Default.ParseArguments(args, options))
                return;

            Console.WriteLine("West Wind WebSurge v" + GetVersion());

            
            //Console.WriteLine(ServicePointManager.DefaultConnectionLimit);

            var stressTester = new StressTester();

            List<HttpRequestData> requests;
            if (!string.IsNullOrEmpty(options.SessionFile))
                requests = stressTester.ParseFiddlerSessions(options.SessionFile);
            else
            {
                if (string.IsNullOrEmpty(options.Url))
                {
                    WriteError("No session file or url specified.");
                    return;
                }

                requests = new List<HttpRequestData>();
                requests.Add(new HttpRequestData
                {
                    Url =options.Url
                });
            }


            //int time = StringUtils.ParseInt(tbtTxtTimeToRun.Text,2)
            int time = options.Time;
            int threads = options.Threads;


            stressTester.Progress += stressTester_Progress;
            //stressTester.RequestProcessed += stressTester_RequestProcessed;
            var results = stressTester.CheckAllSites(requests,threads,time);



            string resultText = stressTester.ParseResults(results);
            
            Console.Clear();
            Console.WriteLine("West Wind WebSurge v" + GetVersion());
            Console.WriteLine("-----------------------------------");
            Console.WriteLine(resultText);

            Console.ReadLine();

        }

        static void stressTester_RequestProcessed(HttpRequestData req )
        {
            string output = req.StatusCode + " -  " + req.HttpVerb + " " + req.Url + "(" + req.TimeTakenMs.ToString("n0") + "ms)";
            Console.WriteLine(output);
        }

        static void stressTester_Progress(ProgressInfo progress)
        {
            int reqsec = 0;
            if (progress.SecondsProcessed < 1)
                return;

            string text = progress.RequestsProcessed.ToString("n0") +
                          " requests, " + progress.RequestsFailed.ToString("n0") + " failed | " +
                          progress.SecondsProcessed + " of " +
                          progress.TotalSecondsToProcessed + " secs ";
            if (progress.SecondsProcessed > 0)
                text += "| " + progress.RequestsProcessed / progress.SecondsProcessed + " request/sec ";

            Console.WriteLine(text);
            
            //Console.WriteLine("{0} requests in {1} seconds - {2} req/sec. {3} failed.",
            //    progress.RequestsProcessed,
            //    progress.SecondsProcessed,
            //    reqsec,
            //    progress.RequestsFailed);
        }

        static void WriteError(string message)
        {
            Console.WriteLine("  *** " + message + " ***");
        }

        static string GetVersion()
        {
            var assmbly = Assembly.GetExecutingAssembly();
            var version = FileVersionInfo.GetVersionInfo(assmbly.Location);            
            return version.FileMajorPart + "." + version.FileMinorPart;         
        }

    }
}
