using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Westwind.Utilities;
using Westwind.Utilities.Configuration;

namespace WebSurge
{

    public class App
    {
        public static WebSurgeConfiguration Configuration { get; set; }        
        public static string AppDataPath { get; set; }

        static App()
        {
            AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
              "\\West Wind Technologies\\WebSurge\\";
            if (!Directory.Exists(App.AppDataPath))
                Directory.CreateDirectory(App.AppDataPath);

            Configuration = new WebSurgeConfiguration();
            Configuration.Initialize();
        }


        /// <summary>
        /// Logs exceptions in the applications
        /// </summary>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            var msg = ex.Message + "\r\n---\r\n" + ex.Source + "\r\n" + ex.StackTrace;
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
            StringUtils.LogString(msg, App.AppDataPath + "WebSurgeErrors.log");
        }
    }

    public class WebSurgeConfiguration : AppConfiguration
    {
        public string AppName { get; set; }
        public StressTesterConfiguration StressTester {get; set;}
        public UrlCaptureConfiguration UrlCapture {get; set;}
        public WindowSettings WindowSettings { get; set; }

        public WebSurgeConfiguration()
        {
            StressTester = new StressTesterConfiguration();
            UrlCapture = new UrlCaptureConfiguration();
            WindowSettings = new WindowSettings();

            AppName = "West Wind WebSurge";
        }

        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            var provider = new JsonFileConfigurationProvider<WebSurgeConfiguration>()
            {
                JsonConfigurationFile = App.AppDataPath + "WebSurgeConfiguration.json"
            };
            Provider = provider;

            return provider;
        }    
    }
    

    public class StressTesterConfiguration
    {
        public string LastFileName { get; set; }
        public string ReplaceCookieValue { get; set; }
        public int MaxResponseSize { get; set; }
        public int LastThreads { get; set; }
        public int LastSecondsToRun { get; set; }
        public int DelayTimeMs { get; set; }
        public bool RandomizeRequests { get; set; }
        public int RequestTimeoutMs { get; set; }
        

        public StressTesterConfiguration()
        {
            LastFileName = Path.GetFullPath("1_Full.txt");
            MaxResponseSize = 5000;
            LastSecondsToRun = 10;
            LastThreads = 2;            
        }    
    }

    public class UrlCaptureConfiguration 
    {
        public int ProcessId; 
        public bool IgnoreResources { get; set; }        
        public string UrlFilterExclusions { get; set; }
        public string ExtensionFilterExclusions { get; set; }

        public UrlCaptureConfiguration()
        {
            UrlFilterExclusions =
                "analytics.com|google-syndication.com|google.com|live.com|microsoft.com|/chrome-sync/|client=chrome-omni";

            ExtensionFilterExclusions = ".css|.js|.png|.jpg|.gif|.ico";
        }
    }

    public class WindowSettings
    {
        public int Left { get; set; }
        public int Top { get; set;  }
        public int Height { get; set; }
        public int Width { get; set; }

        public WindowSettings()
        {
            Left = -1;
            Top = -1;
            Width = 1000;
            Height = 700;
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
}
