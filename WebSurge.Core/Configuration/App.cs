using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using WebSurge.Extensibility;
using Westwind.Utilities;

namespace WebSurge
{
    public class App
    {
        public static WebSurgeConfiguration Configuration { get; set; }

        public static string UserDataPath { get; set; }
        public static string LogFile { get; set; }
        public static string VersionCheckUrl { get; set; }
        public static string InstallerDownloadUrl { get; set; }   
        public static string InstallerDownloadPage { get; set; }
        public static string WebHomeUrl { get; set; }
        public static string PurchaseUrl { get; set; }

        public static string DocsUrl { get; set; }

        public bool ReloadTemplates { get; set; }

        internal static string EncryptionMachineKey { get; set; }
        internal static string ProKey { get; set; }

        public static List<IWebSurgeExtensibility> Addins { get; set; }
        

        static App()
        {
            UserDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                           "\\West Wind WebSurge\\";
            if (!Directory.Exists(UserDataPath))
                Directory.CreateDirectory(UserDataPath);

            LogFile = UserDataPath + "WebSurgeErrors.log";
            VersionCheckUrl = "https://west-wind.com/files/WebSurge_Version.xml";
            InstallerDownloadUrl = "https://west-wind.com/files/WebsurgeSetup.exe";
            InstallerDownloadPage = "https://websurge.west-wind.com/download.aspx";
            WebHomeUrl = "https://websurge.west-wind.com";
            PurchaseUrl = "https://store.west-wind.com/product/order/websurge";
            DocsUrl = "https://websurge.west-wind.com/docs/";

            Configuration = new WebSurgeConfiguration();
            //Configuration.Initialize();
            try
            {
                Configuration.Initialize();
            }
            catch (Exception ex)
            {
                // Log but continue on with default settings
                App.Log(ex);
            }

            // Encryption key is only valid on the current machine
            EncryptionMachineKey = "22653K0U*He73kj4$JJ" + Environment.MachineName;
            ProKey = "Kuhela_100";  // "3bd0f6e";


            Addins = AddinLoader.LoadAddins();
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

        public static void OpenFileInExplorer(string filename)
        {
            
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,{filename}"
            });
        }

        /// <summary>
        /// Logs messages to the log file
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg)
        {
            var text = msg +
                       "\r\n\r\n---------------------------\r\n\r\n";
            StringUtils.LogString(msg, UserDataPath + "WebSurgeErrors.log");
        }
    }
}