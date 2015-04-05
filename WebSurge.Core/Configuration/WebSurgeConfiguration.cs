using System;
using System.Collections.Generic;
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
        public StressTesterConfiguration StressTester { get; set; }
        public UrlCaptureConfiguration UrlCapture { get; set; }
        public WindowSettings WindowSettings { get; set; }
        public CheckForUpdates CheckForUpdates { get; set; }

        [JsonIgnore]
        public List<string> RecentFiles
        {
            get
            {
                if (_recentFileList == null)
                {
                    try
                    {
                        _recentFileList = MostRecentlyUsedList.GetMostRecentDocs("*.websurge");
                    }
                    catch
                    {
                        _recentFileList = new List<string>();
                    }
                }

                return _recentFileList;
            }
        }

        private List<string> _recentFileList;

        public string LastFileName
        {
            get { return _LastFileName; }
            set
            {
                _LastFileName = value;
                try
                {
                    MostRecentlyUsedList.AddToRecentlyUsedDocs(value);

                    // reload recent file list
                    _recentFileList = MostRecentlyUsedList.GetMostRecentDocs("*.websurge");
                }
                catch
                {
                }
            }
        }

        private string _LastFileName;



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
