using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSurge;
using Westwind.Utilities.InternetTools;

namespace SimpleStressTester.Tests
{
    [TestClass]
    public class HttpPerfTests
    {
        private string testUrl = "http://localhost/aspnetperf/static.htm";
        private int counter = 0;
        private int counter2 = 0;
        private bool cancel = false;
        private bool cancel2 = false;


        [TestMethod]  
       public async Task PlainHTtpRequest()
        {
            var client = new HttpClient();
            string result = await client.DownloadStringAsync("http://west-wind.com");

            Console.WriteLine(result);
        }

        [TestMethod]
        public async Task ReusePlainHTtpRequest()
        {
            string url = "http://localhost/aspnetperf/static.htm";

            var client = new HttpClient();
            string result = client.DownloadString(url);

            var swatch = new Stopwatch();
            swatch.Start();

            for (int i = 0; i < 100000; i++)
            {
                result = client.DownloadString(url);
            }

            swatch.Stop();

            Console.WriteLine(swatch.ElapsedMilliseconds + " " + result);
        }

        [TestMethod]
        public async Task RecreatePlainHttpRequest()
        {
            string url = "http://localhost/aspnetperf/static.htm";

            var client = new HttpClient();
            string result = client.DownloadString(url);

            var swatch = new Stopwatch();
            swatch.Start();

            for (int i = 0; i < 100000; i++)
            {
                client = new HttpClient();
                result = client.DownloadString(url);
            }

            swatch.Stop();

            Console.WriteLine(swatch.ElapsedMilliseconds + " " + result);
        }


        [TestMethod]
        public void ThreadRequests()
        {
            int threadCount = 8;


            Console.WriteLine("Threads:" + threadCount);
            Console.WriteLine("Connection Limit: " + ServicePointManager.DefaultConnectionLimit);
            Console.WriteLine("Max Service Points: " + ServicePointManager.MaxServicePoints);

            for (int i = 0; i < threadCount; i++)
            {
                var t = new Thread(RunHttpRequests);
                t.Start();
            }

            // let threads start up            
            Thread.Sleep(5000);
            cancel2 = true;

            
            Console.WriteLine(counter2);
        }



        [TestMethod]
        public void ThreadRequestsAsync()
        {
            
            int threadCount = 8;

            Console.WriteLine("Threads:" + threadCount);
            Console.WriteLine("Connection Limit: " + ServicePointManager.DefaultConnectionLimit);
            Console.WriteLine("Max Service Points: " + ServicePointManager.MaxServicePoints);

            for (int i = 0; i < threadCount; i++)
            {
                var t = new Thread(RunHttpRequestsAsync);
                t.Start();
            }

            // let threads start up            
            Thread.Sleep(10000);
            cancel2 = true;

            Console.WriteLine(counter);
        }

        void RunHttpRequests()
        {
            while (!cancel2)
            {
                RunHttpRequest();
                //Thread.Sleep(0);
                Thread.Yield();
            }
        }

        void RunHttpRequestsAsync()
        {
            while (!cancel)
            {

                var t = RunHttpRequestAsync();
                t.Wait();
                int result = t.Result;
                //Thread.Sleep(0);
                Thread.Yield();
            }            
        }

        int RunHttpRequest()
        {
            using (var client = new HttpClient())
            {                
                string result =  client.DownloadString(testUrl);                
            }
            Interlocked.Increment(ref counter2);

            return 0;
        }



        async Task<int> RunHttpRequestAsync()
        {
            using (var client = new HttpClient())
            {
                client.CreateWebRequestObject(testUrl);                
                string result = await client.DownloadStringAsync(testUrl);
            }
            Interlocked.Increment(ref counter);

            return 0;
        }


    }
}
