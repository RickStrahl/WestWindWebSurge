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
        [Option('u',"url",HelpText="A single URL to load test")]
        public string Url { get; set; }

        [ValueOption(1)]
        //[Option('f',"file",HelpText="File that contains HTTP Session data.")]
        public string SessionFile { get; set;  }

        [Option('s',"seconds",HelpText = "Length of time in seconds to run the test")]
        public int Time { get; set;  }

        [Option('t',"threads",HelpText = "Number of simultaneous threads to run")]
        public int Threads { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            var usage = new StringBuilder();
            usage.AppendLine("West Wind WebSurge v" + Environment.Version.Major + "." + Environment.Version.Minor);
            
            return usage.ToString();
        }

        public CommandLineOptions()
        {
            Time = 10;
            Threads = 2;
        }
    }
}
