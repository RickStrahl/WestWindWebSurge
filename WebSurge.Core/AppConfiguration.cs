using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Westwind.Utilities.Configuration;

namespace WebSurge
{

    public class App
    {
        public static StressTesterConfiguration Configuration { get; set; }
        public static UrlCaptureConfiguration CaptureConfiguration { get; set; }
        public static string AppDataPath { get; set; }

        static App()
        {
            AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
              "\\West Wind Technologies\\WebSurge\\";
            if (!Directory.Exists(App.AppDataPath))
                Directory.CreateDirectory(App.AppDataPath);

            Configuration = new StressTesterConfiguration();
            Configuration.Initialize();

            CaptureConfiguration = new UrlCaptureConfiguration();
            CaptureConfiguration.Initialize();

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
        public string AppName { get; set; }

        public StressTesterConfiguration()
        {
            FileName = Path.GetFullPath("1_Full.txt");
            MaxResponseSize = 5000;
            LastSecondsToRun = 10;
            LastThreads = 2;
            AppName = "West Wind WebSurge";
        }

        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            var provider = new ConfigurationFileConfigurationProvider<StressTesterConfiguration>()
            {
                ConfigurationFile = App.AppDataPath + "WebSurge.config",
                ConfigurationSection = sectionName
            };

            Provider = provider;
            
            return provider;
        }
    }

    public class UrlCaptureConfiguration : AppConfiguration
    {
        public bool IgnoreResources { get; set; }


        public int ProcessId;
        public string UrlFilterExclusions { get; set; }
        public string ExtensionFilterExclusions { get; set; }

        public UrlCaptureConfiguration()
        {
            UrlFilterExclusions =
                "analytics.com|google-syndication.com|google.com|live.com|microsoft.com|/chrome-sync/|client=chrome-omni";

            ExtensionFilterExclusions = ".css|.js|.png|.jpg|.gif|.ico";
        }

        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            var provider = new ConfigurationFileConfigurationProvider<StressTesterConfiguration>()
            {
                ConfigurationFile = App.AppDataPath + "WebSurge.config",
                ConfigurationSection = sectionName
            };

            Provider = provider;

            return provider;
        }

    }
}
