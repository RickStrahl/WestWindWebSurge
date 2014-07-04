using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Westwind.Utilities;
using Westwind.Utilities.InternetTools;

namespace WebSurge.Core
{
    
    /// <summary>
    /// Checks for new versions and allows downloading of the latest version
    /// and installation.
    /// </summary>
    public class ApplicationUpdater
    {
        /// <summary>
        /// Version info captured by NewVersionAvailable
        /// </summary>
        public VersionInfo VersionInfo { get; set; }

        /// <summary>
        /// The current version we're checking for updates
        /// </summary>
        public string CurrentVersion { get; set; }

        /// <summary>
        /// The local file that identifies the local version
        /// </summary>
        public string VersionFile { get; set; }

        /// <summary>
        /// The URL on a remote server HTTP link that contains 
        /// the Version XML with the VersionInfo data
        /// </summary>
        public string VersionCheckUrl { get; set;  }

        /// <summary>
        /// The URL from which the installer is downloaded
        /// </summary>
        public string DownloadUrl { get; set; }


        /// <summary>
        /// Determines where the updated version is downloaded to
        /// </summary>
        public string DownloadStoragePath { get; set; }

        /// <summary>
        /// How frequently to check for updates
        /// </summary>
        public int CheckDays { get; set; }

        /// <summary>
        /// Last time updates were checked for
        /// </summary>
        public DateTime LastCheck { get; set; }

        
        
        /// <summary>
        /// Overload that requires a semantic versioning number
        /// as a string (0.56 or 9.44.44321)
        /// </summary>
        /// <param name="currentVersion"></param>
        public ApplicationUpdater(string currentVersion)
        {
            VersionInfo = new VersionInfo();
            CurrentVersion = currentVersion;
            Initialize();
        }

        /// <summary>
        /// Overload that accepts a type from an assembly that holds
        /// version information
        /// </summary>
        /// <param name="assemblyType"></param>
        public ApplicationUpdater(Type assemblyType)
        {
            VersionInfo = new VersionInfo(); 
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = v.Major + "." + v.Minor;
            Initialize();
        }

        void Initialize()
        {
            VersionFile = App.UserDataPath + "WebSurge_Version.xml";
            VersionCheckUrl = App.VersionCheckUrl;
            DownloadUrl = App.InstallerDownloadUrl;
            DownloadStoragePath = App.UserDataPath + "WebSurgeSetup.exe";

            CheckDays = App.Configuration.CheckForUpdates.Days;
            LastCheck = App.Configuration.CheckForUpdates.LastUpdateCheck; 
        }

        /// <summary>
        /// This is the do it all function that checks for a new version
        /// downloads if it doesn't exist and immediately executes it.
        /// </summary>
        public bool CheckDownloadExecute(bool executeImmediately = true)
        {
            if (!NewVersionAvailable())
                return true;

            if (!Download())
                return false;

            if (!executeImmediately)
                return true;

            ExecuteDownloadedFile();

            return true;
        }


        /// <summary>
        /// Checks to see if a new version is available at the 
        /// VersionCheckUrl
        /// </summary>
        /// <param name="checkDate"></param>
        /// <returns></returns>
        public bool NewVersionAvailable(bool checkDate = false)
        {
            if (checkDate)
            {
                if (LastCheck.Date > DateTime.UtcNow.Date.AddDays(CheckDays * -1))                
                    return false;                
            }

            string xml = null;
            try
            {
                var client = new WebClient();
                xml = client.DownloadString(App.VersionCheckUrl);
            }
            catch
            {
                // fail silently if no connection or invalid url
                return false;
            }

            if (!string.IsNullOrEmpty(xml))
            {
                var ver = SerializationUtils.DeSerializeObject(xml, typeof (VersionInfo)) as VersionInfo;                
                if (ver != null)
                {
                    VersionInfo = ver; 
                    
                    if (ver.Version.CompareTo(CurrentVersion) > 0)                    
                        return true;                   
                }
            }

            return false;
        }

        /// <summary>
        /// Downloads the update exe
        /// </summary>
        /// <returns></returns>
        public bool Download()
        {
            try
            {
                var client = new WebClient();
                //client.DownloadProgressChanged += client_DownloadProgressChanged; 
                // In order to get events we have to run this async and wait
                client.DownloadFile(DownloadUrl, DownloadStoragePath);
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DownloadAsync()
        {
            var client = new WebClient();
            client.DownloadProgressChanged += client_DownloadProgressChanged; 
            await client.DownloadFileTaskAsync(DownloadUrl, DownloadStoragePath);            

            return true;
        }

        /// <summary>
        /// Event you can use to get download progress information
        /// </summary>
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        /// <summary>
        /// Forwards the DownloadProgressChangedEventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
                DownloadProgressChanged(sender, e);
        }

        /// <summary>
        /// Executes the downloaded file in the download folder
        /// </summary>
        public void ExecuteDownloadedFile()
        {
            var proc = new Process();
            Process.Start(DownloadStoragePath);
        }

    }
    
    /// <summary>
    /// Version info class used to 
    /// </summary>
    public class VersionInfo
    {
        public string Version { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string DownloadUrl { get; set; }
        public int DownloadSize { get; set; }

        public VersionInfo()
        {
            Version = "0.01";
        }
    }
}
