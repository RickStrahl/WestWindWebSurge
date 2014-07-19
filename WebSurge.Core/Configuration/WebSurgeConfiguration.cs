using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Westwind.Utilities.Configuration;

namespace WebSurge
{
    public class WebSurgeConfiguration : AppConfiguration
    {
        public string AppName { get; set; }
        public StressTesterConfiguration StressTester { get; set; }
        public UrlCaptureConfiguration UrlCapture { get; set; }
        public WindowSettings WindowSettings { get; set; }
        public List<string> RecentFiles { get; set; }
        public CheckForUpdates CheckForUpdates { get; set; }

        public string LastFileName
        {
            get { return _LastFileName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _LastFileName = null;
                    return;
                }
                _LastFileName = value;

                var match = RecentFiles.FirstOrDefault(s => s.ToLower() == value.ToLower());
                if (match != null)
                    RecentFiles.Remove(match);

                RecentFiles.Insert(0, value);

                RecentFiles = new List<string>(RecentFiles.Distinct().Take(10));


            }
        }

        private string _LastFileName;



        public WebSurgeConfiguration()
        {
            RecentFiles = new List<string>();
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
                JsonConfigurationFile = App.UserDataPath + "WebSurgeConfiguration.json"
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
