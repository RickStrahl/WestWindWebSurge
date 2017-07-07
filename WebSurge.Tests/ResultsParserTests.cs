using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSurge;
using Westwind.Utilities;


namespace SimpleStressTester.Tests
{
    [TestClass]
    public class ResultsParserTests
    {

        [TestMethod]
        public void ResultsReportTimeTakenTest()
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
            var timeTakenMs = 30000;

            var parser = new ResultsParser();
            var results = parser.GetResultReport(requests, timeTakenMs, 10);

            Assert.AreEqual(timeTakenMs / 1000, results.TestResult.TimeTakenSecs);

            Console.WriteLine(JsonSerializationUtils.Serialize(results, false, true));
        }

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

        [TestMethod]
        public void RequestSummaryTest()
        {
            var time = DateTime.UtcNow;

            var requests = new List<HttpRequestData>()
            {
                new HttpRequestData()
                {
                    Url = "http://localhost/",
                    Timestamp = time,
                    IsError = false,
                    TimeTakenMs = 10,
                    ErrorMessage = "Invalid Server Response",
                    StatusCode = "500"
                },
                new HttpRequestData()
                {
                    Url = "http://localhost/wconnect",
                    Timestamp = time.AddMilliseconds(20),
                    IsError = false,
                    TimeTakenMs = 95
                
                },
                new HttpRequestData
                {
                    Url = "http://localhost/",
                    Timestamp = time.AddMilliseconds(220),
                    IsError = false,
                    TimeTakenMs = 15,
                    ErrorMessage = "Bogus Invalid Server Response",
                    StatusCode = "500"
                },
                new HttpRequestData
                {
                    Url = "http://localhost/",
                    Timestamp = time.AddMilliseconds(1020),
                    TimeTakenMs = 20
                },
                new HttpRequestData
                {
                    Url = "http://localhost/wconnect",
                    Timestamp = time.AddMilliseconds(1050),
                    TimeTakenMs = 20
                },
                new HttpRequestData
                {
                    Url = "http://localhost/",
                    Timestamp = time.AddMilliseconds(1200),
                    TimeTakenMs = 20
                },
                new HttpRequestData
                {
                    Url = "http://localhost/",
                    Timestamp = time.AddMilliseconds(3020),
                    TimeTakenMs = 20
                },
                new HttpRequestData
                {
                    Url = "http://localhost/",
                    Timestamp = time.AddMilliseconds(3050),
                    TimeTakenMs = 20
                },
                new HttpRequestData
                {
                    Url = "http://localhost/wconnect",
                    Timestamp = time.AddMilliseconds(3200),
                    TimeTakenMs = 20
                },
                new HttpRequestData
                {
                    Url = "http://localhost/wconnect",
                    Timestamp = time.AddMilliseconds(3500),
                    TimeTakenMs = 50
                },
                new HttpRequestData
                {
                    Url = "http://localhost/wconnect/testpage",
                    Timestamp = time.AddMilliseconds(3100),
                    IsError = false,
                    TimeTakenMs = 50
                },
                new HttpRequestData
                {
                    Url = "http://localhost/wconnect/testpage",
                    IsError = false,
                    Timestamp = time.AddMilliseconds(3200),
                    TimeTakenMs = 57
                },
                new HttpRequestData
                {
                    Url = "http://localhost/wconnect/testpage2",
                    Timestamp = time.AddMilliseconds(3100),
                    TimeTakenMs = 50
                },
                new HttpRequestData
                {
                    Url = "http://localhost/wconnect/testpage2",
                    Timestamp = time.AddMilliseconds(3200),
                    TimeTakenMs = 57
                }

            };

            var parser = new ResultsParser();
            var res = parser.UrlSummary(requests, 200);

            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() > 0);

            foreach (var r in res)
            {
                Console.WriteLine(r.Url + ": " +  JsonSerializationUtils.Serialize(r.Results,false,true));
            }


            var html = parser.GetResultReportHtml(requests,10,2);            
            Console.WriteLine(html);

            var file = App.UserDataPath + "html\\_preview.html";

            File.WriteAllText(file, html);
            ShellUtils.GoUrl(file);
        }
    }
}
