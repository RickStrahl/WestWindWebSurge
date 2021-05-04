using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Westwind.RazorHosting;
using Westwind.Utilities;

namespace WebSurge
{
    public class ResultsParser
    {

        public TestResult ParseResults(RequestWriter requestWriter, int totalTimeSecs, int threads)
        {
            // avoid divide by zero errors
            if (totalTimeSecs < 1)
                totalTimeSecs = 1;

            var results = requestWriter.GetResults();
            var count = results.Count;

            TestResult res;
            if (count == 0)
                return res = new TestResult();

            res = new TestResult()
            {                
                TotalRequests = results.Count,
                ThreadCount = threads,
                TimeTakenSecs = totalTimeSecs,
                FailedRequests = results.Count(req => req.IsError),
                SuccessRequests = results.Count(req => !req.IsError),
                RequestsPerSecond = ((decimal) results.Count/(decimal) totalTimeSecs),
                AvgRequestTimeMs = (decimal) results.Average(req => req.TimeTakenMs),
                MinRequestTimeMs = results.Min(req => req.TimeTakenMs),
                MaxRequestTimeMs = results.Max(req => req.TimeTakenMs),
                TotalBytesSent = results.Sum(req=> (long) req.ResponseLength),
                TotalBytesPosted = results.Sum(req=> string.IsNullOrEmpty(req.RequestContent) ? 0 : (long) req.RequestContent.Length),

                ErrorMessages = results
                                    .GroupBy(x => x.ErrorMessage)
                                    .Where(g => g.Key != null)
                                    .Select(g => new ErrorMessage()
                {
                    Message = g.Key,
                    Count = g.Count()
                })
            };

            return res;
        }

        public string ParseResultsToString(RequestWriter writer, int totalTimeSecs, int threads)
        {
            var result = ParseResults(writer, totalTimeSecs, threads);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Total Requests: " + result.TotalRequests.ToString("n0"));

            if (threads > 0)
                sb.AppendLine("       Threads: " + result.ThreadCount);

            sb.AppendLine("        Failed: " + result.FailedRequests);

            if (result.TimeTakenSecs > 0)
            {
                sb.AppendLine("    Total Time: " + result.TimeTakenSecs.ToString("n2") + " secs");
                if (result.TotalRequests > 0)
                    sb.AppendLine("       Req/Sec: " +
                                  ((decimal) result.TotalRequests/(decimal) result.TimeTakenSecs).ToString("n2") +
                                  "\r\n");
            }

            

            if (result.TotalRequests > 0)
            {
                sb.AppendLine($"      Avg Time: {result.AvgRequestTimeMs:n2} ms");
                sb.AppendLine($"      Min Time: {result.MinRequestTimeMs:n2} ms");
                sb.AppendLine($"      Max Time: {result.MaxRequestTimeMs:n2} ms");
                sb.AppendLine($"     data sent: {result.ByteSizeString(result.TotalBytesSent)}");
                if (result.TotalBytesPosted > 0)
                    sb.AppendLine($"   data posted: {result.ByteSizeString(result.TotalBytesPosted)}");
            }

            return sb.ToString();
        }

        public IEnumerable<TimeTakenResult> TimeLineDataForIndividualRequest(IEnumerable<HttpRequestData> resultData,
            string url)
        {
            var count = 0;
            return resultData.Where(rd => rd.Url.ToLower() == url.ToLower())
                .OrderBy(rd => rd.Timestamp)
                .Select(rd => new TimeTakenResult()
                {
                    OrigId = rd.Id,
                    RequestNo = count++,
                    TimeTaken = rd.TimeTakenMs,
                    IsError = rd.IsError
                }).ToList();
        }

        public IEnumerable<RequestsPerSecondResult> RequestsPerSecond(RequestWriter requestWriter,
            string url = null)
        {
            IEnumerable<HttpRequestData> resultData = requestWriter.GetResults();

            if (!string.IsNullOrEmpty(url))
                resultData = resultData.Where(rd => rd.Url.ToLower() == url.ToLower());

            DateTime startTime = resultData.First().Timestamp;
            var res = resultData.OrderBy(rd => rd.Timestamp)
                .GroupBy(rd => (int) rd.Timestamp.Subtract(startTime).TotalSeconds,
                    rd => rd,
                    (second, rd) => new RequestsPerSecondResult() {Second = second, Requests = rd.Count()});

            return res.ToList();
        }

        public IEnumerable<UrlSummary> UrlSummary(RequestWriter requestWriter, int totalTimeTakenSecs)
        {
            // avoid divide by 0 error - assume at least 1 second
            if (totalTimeTakenSecs == 0)
                totalTimeTakenSecs = 1;

            IEnumerable<HttpRequestData> resultData = requestWriter.GetResults();
            var urls = resultData
                .GroupBy(res => res.HttpVerb + " " + res.Url + (string.IsNullOrEmpty(res.Name) ? "" : " • " + res.Name),
                    rs => rs, (key, uls) =>
                    {
                        // Prevent multiple enumerations
                        var results = uls as IList<HttpRequestData> ?? uls.ToList();

                        return new UrlSummary()
                        {
                            Url = key,
                            Results = new TestResult()
                            {
                                TimeTakenSecs = totalTimeTakenSecs,
                                TotalRequests = results.Count(),
                                FailedRequests = results.Count(u => u.IsError),
                                SuccessRequests = results.Count(u => !u.IsError),
                                RequestsPerSecond = ((decimal) results.Count() / (decimal) totalTimeTakenSecs),
                                MinRequestTimeMs = results.Min(u => u.TimeTakenMs),
                                MaxRequestTimeMs = results.Max(u => u.TimeTakenMs),
                                AvgRequestTimeMs = (decimal) results.Average(u => u.TimeTakenMs),
                                TotalBytesSent = results.Sum(req => (long) req.ResponseLength),
                                TotalBytesPosted = results.Sum(req => string.IsNullOrEmpty(req.RequestContent) ? 0 : (long) req.RequestContent.Length),

                                ErrorMessages = results
                                    .GroupBy(x => x.ErrorMessage)
                                    .Where(g => g.Key != null)
                                    .Select(g => new ErrorMessage()
                                    {
                                        Message = g.Key,
                                        Count = g.Count()
                                    })
                            }
                        };
                    });

            return urls.ToList();
        }

        public TestResultView GetResultReport(RequestWriter requestWriter,
            int totalTimeTakenMs,
            int threadCount)
        {
            // Convert milliseconds to seconds
            var totalTimeTaken = totalTimeTakenMs / 1000;

            var urlSummary = UrlSummary(requestWriter, totalTimeTaken);
            var testResult = ParseResults(requestWriter, totalTimeTaken, threadCount);

            var model = new TestResultView()
            {
                Timestamp = DateTime.UtcNow,
                TestResult = testResult,
                UrlSummary = urlSummary
            };

            return model;
        }

        public string GetResultReportHtml(RequestWriter writer, 
                                       int totalTimeTakenMs, 
                                       int threadCount)
        {
            var model = GetResultReport(writer, totalTimeTakenMs, threadCount);
            return TemplateRenderer.RenderTemplate("TestResult.cshtml",model);
        }

    }

    public class TestResultView
    {
        public DateTime Timestamp { get; set; }
        public TestResult TestResult { get; set; }
        public IEnumerable<UrlSummary> UrlSummary { get; set; }    
    }

    public class UrlSummary
    {
        public string Url { get; set; }
        public string HttpVerb { get; set;  }
        public TestResult Results { get; set; }
    }

    public class TestResult
    {        
        public int TotalRequests { get; set; }
        public int ThreadCount { get; set; }
        public int FailedRequests { get; set; }
        public int SuccessRequests { get; set; }
        public decimal RequestsPerSecond { get; set; }        
        public decimal AvgRequestTimeMs { get; set; }
        public decimal MinRequestTimeMs { get; set; }
        public decimal MaxRequestTimeMs { get; set; }
        public int TimeTakenSecs { get; set; }
        public IEnumerable<ErrorMessage> ErrorMessages { get; set; }
        public long TotalBytesSent { get; set; }
        public long TotalBytesPosted { get; set; }



        public string ByteSizeString(long byteCount)
        {
            string res = null;
            if (byteCount < 10_000)
                res = byteCount.ToString("n0") + " bytes";
            else if (byteCount < 1_500_000)
                res = ((decimal) byteCount / 1000).ToString("n0") + "kb";
            else if (byteCount < 9_000_000)
                res = ((decimal)byteCount / 1000000).ToString("n1") + "mb";
            else if (byteCount < 1_000_000_000)
                res = ((decimal) byteCount / 1000000).ToString("n0") + "mb";
            else
                res = ((decimal)byteCount / 1_000_000_000).ToString("n1") + "gb";

            return res;
        }
    }

    public class ErrorMessage
    {
        public string Message { get; set; }

        public int Count { get; set; }
    }

    public class RequestsPerSecondResult
    {
        public int Second { get; set; }
        public int Requests { get; set; }
    }

    public class TimeTakenResult
    {
        public long OrigId { get; set; }
        public int RequestNo { get; set; }
        public int TimeTaken { get; set; }
        public bool IsError { get; set; }
    }

}
