using System;
using System.IO;
using System.Net.NetworkInformation;
using Westwind.Utilities;

namespace WebSurge
{
    public class UnlockKey
    {
        /// <summary>
        /// The key to unlock this application
        /// </summary>        
        static string ProKey = "Kuhela_100";  // "3bd0f6e";

        /// <summary>
        /// Determines whether the app is unlocked
        /// </summary>
        public static bool Unlocked
        {
            get
            {
                if (RegisteredCalled)
                    return _unlocked;

                return IsRegistered();
            }
        }
        static bool _unlocked = false;

        /// <summary>
        /// Determines whether the app is running the Pro Version
        /// </summary>
        /// <returns></returns>
        public static RegTypes RegType
        {
            get
            {
                if (RegisteredCalled)
                    return _eRegType;

                IsRegistered();
                return _eRegType;
            }
        }

        public static int FreeThreadLimit = 10;
        public static int FreeSitesLimit = 20;

        static RegTypes _eRegType = RegTypes.Free;

        private static readonly object LockKey = new Object();
        private static bool RegisteredCalled = false;

        /// <summary>
        /// Figures out if this copy is registered
        /// </summary>
        /// <returns></returns>
        public static bool IsRegistered()
        {
            RegisteredCalled = true;

            lock (LockKey)
            {
                _unlocked = false;
                _eRegType = RegTypes.Free;

                if (!File.Exists("Registered.key"))
                    return false;

                string Key = File.ReadAllText("Registered.key");                
                
                if (Key == EncodeKey(ProKey))
                {
                    _eRegType = RegTypes.Professional;
                    _unlocked = true;
                    return true;
                }

                //if (Key == UnlockKey.Key)
                //{
                //    _unlocked = true;
                //    return true;
                //}

                return false;
            }
        }


        /// <summary>
        /// Writes out the registration information
        /// </summary>
        public static bool Register(string Key)
        {
            lock (LockKey)
            {
                string RawKey = Key;
                Key = EncodeKey(Key);

                _eRegType = RegTypes.Free;
                _unlocked = false;

                if (RawKey != ProKey)
                    return false;
                _unlocked = true;

                File.WriteAllText("Registered.key",Key);
                
                //if (Key == UnlockKey.Key)
                //    _unlocked = true;
                if (RawKey == ProKey)
                    _eRegType = RegTypes.Professional;
            }
            return true;
        }

        /// <summary>
        /// Encodes the plain text key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        static string EncodeKey(string Key)
        {
            var Encoded = Encryption.EncryptString(Key, GetMacAddress());
            //string Encoded = Key; //  for now do nothing Key.GetHashCode().ToString("x");
            return Encoded;
        }

        static string GetMacAddress()
        {
            string macAddresses = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return macAddresses;
        }

    }

    public enum RegTypes
    {
        Free,
        Professional,
        Enterprise        
    }


}
