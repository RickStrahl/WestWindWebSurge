using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Westwind.Utilities.Configuration;

namespace WebSurge
{
    public class WebSurgeConfiguration : AppConfiguration
    {
        public string AppName { get; set; }

        [JsonIgnore]
        public StressTesterConfiguration StressTester { get; set; }

        public UrlCaptureConfiguration UrlCapture { get; set; }
        public WindowSettings WindowSettings { get; set; }
        public CheckForUpdates CheckForUpdates { get; set; }

        public List<string> RecentFiles 
        {
            get
            {
                return _recentFileList;
            }
            set
            {
                _recentFileList = value;
            }
        }
        private List<string> _recentFileList;

        public string LastFileName
        {
            get { return _lastFileName; }
            set
            {
                if (_recentFileList == null)
                    _recentFileList = new List<string>();

                _lastFileName = value;                
                RecentFiles.Insert(0, value);

                /* distinct */
                _recentFileList = RecentFiles
                    .Where(f=> File.Exists(f))
                    .GroupBy(f => f.ToLower())
                    .Select(f=> f.First())
                    .Take(10)
                    .ToList();
            }
        }
        private string _lastFileName;

        
       
        public WebSurgeConfiguration()
        {
            StressTester = new StressTesterConfiguration();
            UrlCapture = new UrlCaptureConfiguration();
            WindowSettings = new WindowSettings();
            CheckForUpdates = new CheckForUpdates();

            AppName = "West Wind WebSurge";
        }

        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            var provider = new JsonFileConfigurationProvider<WebSurgeConfiguration>()
            {
                JsonConfigurationFile = App.UserDataPath + "WebSurgeConfiguration.json",
                EncryptionKey = App.EncryptionMachineKey,
                PropertiesToEncrypt = "StressTester.Password"
            };
            Provider = provider;

            return provider;
        }


        protected override void OnInitialize(IConfigurationProvider provider, string sectionName, object configData)
        {
            base.OnInitialize(provider, sectionName, configData);

            if (UrlCapture.UrlFilterExclusions.Count < 1)
            {
                UrlCapture.UrlFilterExclusions = new List<string>()
                {
                    "analytics.com",
                    "google-syndication.com",
                    "google.com",
                    "live.com",
                    "microsoft.com",
                    "/chrome-sync/",
                    "client=chrome-omni",
                    "doubleclick.net",
                    "googleads.com"
                };
            }
            if (UrlCapture.ExtensionFilterExclusions.Count < 1)
            {
                UrlCapture.ExtensionFilterExclusions =
                    new List<string>(".css|.js|.png|.jpg|.gif|.ico|.svg|.fon".Split('|'));
            }
        }
    }
}
