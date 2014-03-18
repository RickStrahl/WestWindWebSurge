using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Westwind.Utilities.Configuration;

namespace Kuhela
{

    public class App
    {
        public static StressTesterConfiguration Configuration { get; set; }

        static App()
        {
            Configuration = new StressTesterConfiguration();
            Configuration.Initialize();
        }
    }


    public class StressTesterConfiguration : AppConfiguration
    {
        public string FileName { get; set; }
        public string ReplaceCookieValue { get; set; }
        public int MaxResponseSize { get; set; }
        public int LastThreads { get; set; }
        public int LastSecondsToRun { get; set; }
        public int DelayTimeMs { get; set; }
        public bool RandomizeRequests { get; set; }
        public int RequestTimeoutMs { get; set; }

        public StressTesterConfiguration()
        {
            FileName = Path.GetFullPath("1_Full.txt");
            MaxResponseSize = 5000;
            LastSecondsToRun = 10;
            LastThreads = 2;
        }
    }
}
