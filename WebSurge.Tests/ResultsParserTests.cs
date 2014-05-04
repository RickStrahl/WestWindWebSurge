using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSurge;

namespace SimpleStressTester.Tests
{
    [TestClass]
    public class ResultsParserTests
    {
        [TestMethod]
        public void RequestPerSecondListTest()
        {
            var time = DateTime.UtcNow;

            var requests = new List<HttpRequestData>()
            {
                new HttpRequestData()
                {
                    Timestamp = time,
                    TimeTakenMs = 10
                },
                new HttpRequestData()
                {
                    Timestamp = time.AddMilliseconds(20),
                    TimeTakenMs = 15
                },
                new HttpRequestData
                {
                    Timestamp = time.AddMilliseconds(220),
                    TimeTakenMs = 15
                },
                new HttpRequestData
                {
                    Timestamp = time.AddMilliseconds(1020),
                    TimeTakenMs = 20
                },
                new HttpRequestData
                {
                    Timestamp = time.AddMilliseconds(1050),
                    TimeTakenMs = 20
                },
                new HttpRequestData
                {
                    Timestamp = time.AddMilliseconds(1200),
                    TimeTakenMs = 20
                },
                new HttpRequestData
                {
                    Timestamp = time.AddMilliseconds(3020),
                    TimeTakenMs = 20
                },
                new HttpRequestData
                {
                    Timestamp = time.AddMilliseconds(3050),
                    TimeTakenMs = 20
                },
                new HttpRequestData
                {
                    Timestamp = time.AddMilliseconds(3200),
                    TimeTakenMs = 20
                },  new HttpRequestData
                {
                    Timestamp = time.AddMilliseconds(3500),
                    TimeTakenMs = 50
                }
            };

            var parser = new ResultsParser();
            var res = parser.RequestsPerSecond(requests);

            Assert.IsNotNull(res);
            foreach (var r in res)
            {
                Console.WriteLine(r.Second + ": " + r.Requests);
            }



        }
    }
}
