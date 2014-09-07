using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebSurge.Tests
{
    [TestClass]
    public class FiddlerParserTests
    {
        const string STR_TestFile = "1_Full.txt";

        [TestMethod]
        public void TestParser()
        {
            SessionParser parser = new SessionParser();
            var options = new StressTesterConfiguration();
            List<HttpRequestData> httpRequests = parser.ParseFile(STR_TestFile, ref options);

            Assert.IsNotNull(httpRequests);

            Console.WriteLine(httpRequests.Count);
            foreach (var req in httpRequests)
            {
                Console.WriteLine("--\r\n");
                Console.WriteLine(req.HttpVerb + " -- " + req.Url);
                Console.WriteLine(req.Host);
                foreach (var header in req.Headers)
                {
                    Console.WriteLine(" " + header.Name + ": " + header.Value);
                }
                Console.WriteLine(req.RequestContent);
            }
        }

        //[TestMethod]
        //public async Task TestCheckSiteAsync()
        //{
        //    SessionParser parser = new SessionParser();
        //    var httpRequests = parser.Parse();

        //    Assert.IsNotNull(httpRequests);

        //    foreach (var req in httpRequests)
        //    {
        //        var tester = new StressTester();
        //        var res = await tester.CheckSiteAsync(req);

        //        Assert.IsNotNull(res, "No Result Returned.");
        //        Assert.IsNotNull(res.IsError, res.ErrorMessage);

        //        Console.WriteLine(res.StatusCode + " " + res.Url + " " + res.TimeTakenSecs.ToString("n0") +  " " + res.ErrorMessage) ;
        //        //Console.WriteLine(res.ResponseContent);
        //    }

        //}

        [TestMethod]
        public void CheckSiteSyncTest()
        {
            SessionParser parser = new SessionParser();
            List<HttpRequestData> httpRequests = null;

            Assert.IsNotNull(httpRequests);

            foreach (var req in httpRequests)
            {
                var tester = new StressTester();
                var res = tester.CheckSite(req);

                Assert.IsNotNull(res, "No Result Returned.");
                Assert.IsNotNull(res.IsError, res.ErrorMessage);

                Console.WriteLine(res.StatusCode + " " + res.Url + " " + res.TimeTakenMs.ToString("n0") + " " + res.ErrorMessage);
                //Console.WriteLine(res.ResponseContent);
            }

        }

        //[TestMethod]
        //public async Task TestAllSitesAsyncTest()
        //{
        //    SessionParser parser = new SessionParser();
        //    var httpRequests = parser.Parse();

        //    var tester = new StressTester();

        //    var swatch = new Stopwatch();
        //    swatch.Start();

        //    var results = await tester.CheckAllSitesAysnc(httpRequests,5,5);

        //    swatch.Stop();

        //    long totalTime = swatch.ElapsedMilliseconds / 1000;

        //    Console.WriteLine("Total Requests: " + results.Count);
        //    Console.WriteLine("Total Time: " + totalTime.ToString("n2") + " secs");             
        //    Console.WriteLine("Failed: " + results.Count(req => !req.StatusCode.StartsWith("2")));
        //    Console.WriteLine("Req/Sec: " + ((decimal) results.Count / (decimal) totalTime).ToString("n2") );
        //    Console.WriteLine("\r\n--------------\r\n");

        //    int count = 0;
        //    foreach (var result in results.OrderByDescending( res => res.StatusCode))
        //    {
        //        count++;
        //        Console.WriteLine(count + ". " + result.Url);
        //        Console.WriteLine("\t" + result.StatusCode + "  " + result.TimeTakenSecs.ToString("n0") + "ms " + result.ErrorMessage);
        //    }
        //}


        [TestMethod]
        public void TestAllSitesThreadsTest()
        {
            SessionParser parser = new SessionParser();
            List<HttpRequestData> httpRequests = null;

            var tester = new StressTester();

            var swatch = new Stopwatch();
            swatch.Start();

            var results = tester.CheckAllSites(httpRequests, 40, 40);

            Assert.IsNotNull(results);

            swatch.Stop();

            long totalTime = swatch.ElapsedMilliseconds / 1000;

            Console.WriteLine(tester.ParseResults(results, (int) totalTime));

            int count = 0;
            foreach (var result in results.OrderByDescending(res => res.StatusCode))
            {
                count++;
                Console.WriteLine(count + ". " + result.Url);
                Console.WriteLine("\t" + result.StatusCode + "  " + result.TimeTakenMs.ToString("n0") + "ms " + result.ErrorMessage);
            }
        }

        [TestMethod]
        public void CheckSingleSiteWithGzipTest()
        {
            var tester = new StressTester();
            tester.Options.MaxResponseSize = 0;
            var resp = tester.CheckSite(new HttpRequestData()
            {
                HttpVerb = "GET",
                Url = "http://weblog.west-wind.com/posts/2014/Feb/22/Using-CSS-Transitions-to-SlideUp-and-SlideDown"                
            });


            Console.WriteLine(resp.StatusCode);
            Console.WriteLine(resp.TimeTakenMs);
            Console.WriteLine(resp.ResponseContent);
        }

    }
}
