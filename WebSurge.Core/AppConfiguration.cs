using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Westwind.Utilities;
using Westwind.Utilities.Configuration;

namespace WebSurge
{

    public class App
    {
        public static WebSurgeConfiguration Configuration { get; set; }        
        
        public static string UserDataPath { get; set; }
        public static string LogFile { get; set;  }
        public static string VersionCheckUrl { get; set; }
        public static string InstallerDownloadUrl { get; set; }
        public static string WebHomeUrl { get; set; }

        static App()
        {
            UserDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
              "\\West Wind Technologies\\WebSurge\\";
            if (!Directory.Exists(App.UserDataPath))
                Directory.CreateDirectory(App.UserDataPath);
            
            LogFile = UserDataPath + "WebSurgeErrors.log";
            VersionCheckUrl = "http://west-wind.com/files/WebSurge_Version.xml";
            InstallerDownloadUrl = "http://west-wind.com/files/WebsurgeSetup.exe";
            WebHomeUrl = "http://west-wind.com/websurge";

            Configuration = new WebSurgeConfiguration();
            Configuration.Initialize();       

        }


        /// <summary>
        /// Logs exceptions in the applications
        /// </summary>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            ex = ex.GetBaseException();

            var msg = ex.Message + "\r\n---\r\n" + ex.Source + "\r\n" + ex.StackTrace + "\r\n";
            Log(msg);
        }

        /// <summary>
        /// Logs messages to the log file
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg)
        {
            var text = msg + 
            "\r\n\r\n---------------------------\r\n\r\n";
            StringUtils.LogString(msg, App.UserDataPath + "WebSurgeErrors.log");
        }
    }

    public class WebSurgeConfiguration : AppConfiguration
    {
        public string AppName { get; set; }
        public StressTesterConfiguration StressTester {get; set;}
        public UrlCaptureConfiguration UrlCapture {get; set;}
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

                RecentFiles = new List<string>( RecentFiles.Distinct().Take(10) );

                
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
                UrlCapture.ExtensionFilterExclusions = new List<string>(".css|.js|.png|.jpg|.gif|.ico|.svg|.fon".Split('|'));
            }
        }
    }
    

    public class StressTesterConfiguration
    {
      
        public string ReplaceCookieValue { get; set; }        
        public string ReplaceDomain { get; set; }
        public int MaxResponseSize { get; set; }
        public int LastThreads { get; set; }
        public int LastSecondsToRun { get; set; }
        public int DelayTimeMs { get; set; }
        public bool RandomizeRequests { get; set; }
        public int RequestTimeoutMs { get; set; }
        public int WarmupSeconds { get; set;  }
        public bool CaptureMinimalResponseData { get; set; }
        public bool NoProgressEvents { get; set;  }
        

        public StressTesterConfiguration()
        {
            MaxResponseSize = 5000;
            LastSecondsToRun = 10;
            LastThreads = 2;
            RequestTimeoutMs = 5000;
            WarmupSeconds = 2;            
        }    
    }

    public class UrlCaptureConfiguration 
    {

        [XmlIgnore]   
        [JsonIgnore]
        public int ProcessId { get; set; } 
        public bool IgnoreResources { get; set; }
        public string CaptureDomain { get; set; }
        public List<string> UrlFilterExclusions { get; set; }
        public List<string> ExtensionFilterExclusions { get; set; }

        public UrlCaptureConfiguration()
        {
            UrlFilterExclusions = new List<string>();
            ExtensionFilterExclusions = new List<string>();
        }
    }

    public class WindowSettings
    {
        public int Left { get; set; }
        public int Top { get; set;  }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Split { get; set; }

        public WindowSettings()
        {
            Left = -1;
            Top = -1;
            Width = 1000;
            Height = 700;
            Split = 490;
        }

        /// <summary>
        /// Loads settings from this structure to a form.
        /// Applies Window settings to a Windows or WPF Form
        /// object - anything with Top, Left, Height, Width
        /// properties really
        /// </summary>
        /// <param name="form"></param>
        public void Load(dynamic form)
        {
            if (Left != -1)
            {
                form.Left = Left;
                form.Top = Top;                
            }
            form.Width = Width;
            form.Height = Height;

            try
            {
                form.BottomSplitContainer.SplitterDistance = Split;
            }
            catch { }
        }

        /// <summary>
        /// Saves settings from a form to this structure.
        /// Works with any form type object that has
        /// Top,Left,Height,Width properties
        /// </summary>
        /// <param name="form"></param>
        public void Save(dynamic form)
        {
            Top = form.Top;
            Left = form.Left;
            Width = form.Width;
            Height = form.Height;

            if (Top < 0)
                Top = 0;
            if (Left < 0)
                Left = 0;

            try
            {
                Split = form.BottomSplitContainer.SplitterDistance;
            }
            catch { }
        }

        //public override string ToString()
        //{
        //    string ser = StringSerializer.SerializeObject(this);
        //    return ser;
        //}

        //public static WindowSettings FromString(string text)
        //{
        //    if (string.IsNullOrEmpty(text))
        //        return new WindowSettings();

        //    return StringSerializer.Deserialize<WindowSettings>(text);
        //}
    }

    public class CheckForUpdates
    {
        public int Days { get; set; }
        public DateTime LastUpdateCheck { get; set; }

        public CheckForUpdates()
        {
            Days = 10;
            LastUpdateCheck = DateTime.UtcNow.Date;
        }
    }
}
