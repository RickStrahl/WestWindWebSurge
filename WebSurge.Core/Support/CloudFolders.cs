using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSurge
{
    public class CloudFolders
    {
        public static string WebSurgeFolder = "West Wind WebSurge\\";

        public static string DropboxDirectory
        {
            get
            {
                _DropboxDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Dropbox\\";
                if (!Directory.Exists(_DropboxDirectory))
                    _DropboxDirectory = null;
                else
                {
                    if (!Directory.Exists(_DropboxDirectory + WebSurgeFolder))
                        Directory.CreateDirectory(_DropboxDirectory + WebSurgeFolder);
                }                

                return _DropboxDirectory;
            }
        }
        private static string _DropboxDirectory;

        public static string OneDriveDirectory
        {
            get
            {
                _OneDriveDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\OneDrive";
                if (!Directory.Exists(_OneDriveDirectory))
                    _OneDriveDirectory = null;

                return _OneDriveDirectory;
            }
        }
        private static string _OneDriveDirectory;        


    }
}
