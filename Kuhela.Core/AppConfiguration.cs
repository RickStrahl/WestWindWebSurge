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
        public static UrlCaptureOptions UrlCaptureOptions { get; set; }
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

    public class UrlCaptureOptions : AppConfiguration
    {
        public bool IgnoreResources { get; set; }
        public int ProcessId { get; set; }
        public string UrlFilterExclusions { get; set; }
        public string ExtensionFilterExclusions { get; set; }

        public UrlCaptureOptions()
        {
            UrlFilterExclusions =
                "analytics.com|google-syndication.com|google.com|live.com|microsoft.com|/chrome-sync/|client=chrome-omni";

            ExtensionFilterExclusions = ".css|.js|.png|.jpg|.gif|.ico";
        }

    }
}
