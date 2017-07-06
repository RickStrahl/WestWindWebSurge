using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace WebSurge.Cli
{
    public class CommandLineOptions 
    {
        public string Url { get; set; }

        [ValueOption(1)]        
        public string SessionFile { get; set;  }

        [Option('s',"seconds",HelpText = "Length of time in seconds to run the test")]
        public int Time { get; set;  }

        [Option('t',"threads",HelpText = "Number of simultaneous threads to run")]
        public int Threads { get; set; }

        [Option('d', "delay", HelpText = "DelayTimeMs between individual requests in milliseconds. -1 no delay, 0 - delay with thread slice, 1-n millisecond delay")]
        public int DelayTimeMs { get; set; }

        [Option('r', "randomize", HelpText = "Randomize requests in the Session file.")]
        public bool RandomizeRequests { get; set; }
        [Option('w', "warmup", HelpText="Number of seconds used for warmup")]
        public int WarmupSeconds { get; set; }

        /// <summary>
        /// -1 not silent. 0 means silent, 1 means no detail 2 means no summary
        /// </summary>
        [Option('y', "silent", HelpText = "Silent operation - output only results. use y surpresses all, y1 surpresses detail only, y2 surpresses summary only",DefaultValue = 1)]
        public int Silent { get; set; }

        [Option("json", HelpText="Generates only JSON output.")]
        public bool Json {get; set;}

        [Option("outputFile", HelpText = "Saves the output to a file")]
        public string OutputFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("West Wind WebSurge v" + Program.GetVersion());            

            string options = @"------------------------
usage:   WebSurgeCli <SessionFile|Url> -sXX -wXX -tXX -dXX -r -yX --outputFile ""output.json""

Parameters:
-----------
SessionFile     Filename to a WebSurge/Fiddler HTTP session file
Url             Single URL to to hit

Commands:
---------
-h | -?      This help display           

Value Options:
--------------
-s          Number of seconds to run the test (10)
-w          Number of seconds used for warmup (0)
-t          Number of simultaneous threads to run (2)
-d          Delay in milliseconds after each request (0)
               1-n  Milliseconds of delay between requests
               0   No delay, but give up cpu time slice
               -1   No delay, no time slice (very high cpu usage)
-y          Display mode for progress (1)
               0 - No progress, 1 - no request detail, 
               2 - no progress summary, 3 - show all

Switches:
---------
-r          Randomize order of requests in Session file

Output:
-------
--json       Return results as JSON
--outputFile Save the output to a filename

Examples:
---------
WebSurgeCli http://localhost/testpage/  -s20 -t8
WebSurgeCli c:\temp\LoadTest.txt  -s20 -t8
WebSurgeCli c:\temp\LoadTest.txt  -s20 -t8 --json
";

            sb.AppendLine(options);            
            return sb.ToString();
        }
        

        public CommandLineOptions()
        {
            Silent = 1;
            Time = 10;
            Threads = 2;            
        }
    }
}
