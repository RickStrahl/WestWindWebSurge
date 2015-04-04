using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using WebSurge;
using Westwind.Utilities;

namespace SimpleStressTester.Tests
{
    [TestClass]
    public class MruListTests
    {
     
        [TestMethod]
        public void AddToRecentDocs()
        {
            MostRecentlyUsedList.AddToRecentlyUsedDocs(
                @"C:\Users\Rick\AppData\Roaming\West Wind WebSurge\WebLogLiveTest.websurge");
        }

        [TestMethod]
        public void EnumerateRecentDocs()
        {
            var recents = MostRecentlyUsedList.GetMostRecentDocs("*.websurge");
            foreach (var recent in recents)
            {
                Console.WriteLine(recent);
            }
        }



        [TestMethod]
        public void GetFirstWebSurgeValue()
        {
            RegistryKey registryKey =
                Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs\.websurge");
            byte[] bytes = registryKey.GetValue("1") as byte[];

            var ms = new MemoryStream(bytes);
            string value = ms.AsString();

            Console.WriteLine(value);

            value = StringUtils.ExtractString(value, "", "\0");

            Console.WriteLine(value);
        }



    }
}
