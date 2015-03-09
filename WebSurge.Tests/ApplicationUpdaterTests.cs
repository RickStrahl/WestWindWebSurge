using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSurge;
using WebSurge.Core;

namespace SimpleStressTester.Tests
{
    [TestClass]
    public class ApplicationUpdaterTests
    {

        [TestMethod]
        public void CheckForNewVersion()
        {
            var updater = new ApplicationUpdater("0.01");           
            Assert.IsTrue(updater.NewVersionAvailable());

            updater = new ApplicationUpdater("1.0"); 
            updater.CurrentVersion = "9999.00";
            Assert.IsFalse(updater.NewVersionAvailable());


            updater = new ApplicationUpdater("0.78");
            Console.WriteLine(updater.NewVersionAvailable());

            updater = new ApplicationUpdater(typeof(Program));
            Console.WriteLine(updater.NewVersionAvailable());
        }


        [TestMethod]
        public void MyTestMethod()
        {
            var v = new Version("1.01.11");
            Console.WriteLine(v);
        }

        [TestMethod]
        public async Task DownloadFile()
        {
            var updater = new ApplicationUpdater(typeof(Program));
            updater.DownloadProgressChanged += updater_DownloadProgressChanged;

            File.Delete(updater.DownloadStoragePath);

            Assert.IsTrue(await updater.DownloadAsync());
            Assert.IsTrue(File.Exists(updater.DownloadStoragePath));
        }

        void updater_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine("{0}%" + "  {1} of  {2}", e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);
        }

        [TestMethod]
        public void DownloadFileAndRun()
        {
            var updater = new ApplicationUpdater(typeof(Program));

            File.Delete(updater.DownloadStoragePath);

            Assert.IsTrue(updater.Download());
            Assert.IsTrue(File.Exists(updater.DownloadStoragePath));

            updater.ExecuteDownloadedFile();
        }

        [TestMethod]
        public void DownloadFileWithEvents()
        {
            var updater = new ApplicationUpdater(typeof(Program));
            updater.DownloadProgressChanged += updater_DownloadProgressChanged;

            File.Delete(updater.DownloadStoragePath);

            Assert.IsTrue(updater.Download());
            Assert.IsTrue(File.Exists(updater.DownloadStoragePath));
        }

       

    }
}
