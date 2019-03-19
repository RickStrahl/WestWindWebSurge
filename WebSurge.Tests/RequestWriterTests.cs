using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSurge;

namespace SimpleStressTester.Tests
{
    [TestClass]
    public class RequestWriterTests
    {

        [TestMethod]
        public void MemoryRequestWriterBasic()
        {
            var stressTester = new StressTester();
            var writer = new MemoryCollectionRquestWriter(stressTester);
            
            for (int i = 0; i < 50; i++)
            {
                var req = new HttpRequestData
                {
                    TimeTakenMs = 10,
                    Timestamp = DateTime.Now,
                    Url = "http://localhost"
                };
                
                writer.Write(req);
            }

            Assert.IsTrue(writer.RequestsProcessed == 50);
            Assert.IsTrue(writer.GetResults().Count == 50);
        }

        [TestMethod]
        public void FileRequestWriterBasic()
        {
            var stressTester = new StressTester();
            List<HttpRequestData> reqs;

            int requestCount = 10_000_001;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            using (var writer = new FileCollectionRequestWriter(stressTester)
            { MaxCollectionItems = 11_000_000})
            {
                for (int i = 0; i < requestCount; i++)
                {
                    var req = new HttpRequestData
                    {
                        TimeTakenMs = 10,
                        Timestamp = DateTime.Now,
                        Url = "http://localhost",
                        IsError = false
                    };

                    writer.Write(req);
                    //Thread.Sleep(0);
                }

                watch.Stop();
                Console.WriteLine("Time: " + watch.ElapsedMilliseconds.ToString("n0"));
                Console.WriteLine("Memory: " + Process.GetCurrentProcess().PrivateMemorySize.ToString("n0"));
                
                return;
                
                watch.Reset();
                watch.Start();


             
                reqs = writer.GetResults();

                watch.Stop();
                Console.WriteLine("Assemble Time: " + watch.ElapsedMilliseconds.ToString("n0"));


                return;

                Assert.IsNotNull(reqs, "Requests are null");

                Assert.IsTrue(writer.RequestsProcessed == requestCount);

                Assert.IsTrue(reqs.Count == requestCount);
            }

        }


    }
}