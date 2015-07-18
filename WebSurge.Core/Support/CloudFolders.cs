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
                    _DropboxDirectory = _DropboxDirectory + WebSurgeFolder;
                    if (!Directory.Exists(_DropboxDirectory))
                        Directory.CreateDirectory(_DropboxDirectory + WebSurgeFolder);
                }                

                return _DropboxDirectory;
            }
        }
        private static string _DropboxDirectory;

        public static bool IsDropbox
        {
            get
            {
                if (_IsDropBox != null)
                    return _IsDropBox.Value;

                _DropboxDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Dropbox\\";
                _IsDropBox = Directory.Exists(_DropboxDirectory);
                return _IsDropBox.Value;
            }
        }
        private static bool? _IsDropBox;

        public static string OneDriveDirectory
        {
            get
            {
                _OneDriveDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\OneDrive\\";
                if (!Directory.Exists(_OneDriveDirectory))
                    _OneDriveDirectory = null;
                else
                {
                    _OneDriveDirectory = _OneDriveDirectory + WebSurgeFolder;
                    if (!Directory.Exists(_OneDriveDirectory))
                        Directory.CreateDirectory(_OneDriveDirectory);
                }
                return _OneDriveDirectory;
            }
        }
        private static string _OneDriveDirectory;

        public static bool IsOneDrive
        {
            get
            {
                if (_IsOneDrive != null)
                    return _IsOneDrive.Value;

                _DropboxDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Dropbox\\";
                _IsOneDrive = Directory.Exists(_DropboxDirectory);
                return _IsOneDrive.Value;
            }
        }
        private static bool? _IsOneDrive;

    }
}
