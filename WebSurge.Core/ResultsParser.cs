using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSurge
{
    public class ResultsParser
    {
        public string ParseResults(IEnumerable<HttpRequestData> resultData, int totalTime, int threads)
        {
            var results = resultData.ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Total Requests: " + results.Count.ToString("n0"));            
            if (threads > 0)
                sb.AppendLine("       Threads: " + threads);    
            sb.AppendLine("        Failed: " +
                              results.Count(req => string.IsNullOrEmpty(req.StatusCode) || !req.StatusCode.StartsWith("2")).ToString("n0"));

            if (totalTime > 0)
            {
                sb.AppendLine("    Total Time: " + totalTime.ToString("n2") + " secs");
                if (results.Count > 0)
                    sb.AppendLine("       Req/Sec: " + ((decimal)results.Count / (decimal)totalTime).ToString("n2") + "\r\n");
            }
            if (results.Count > 0)
            {
                sb.AppendLine(string.Format("      Avg Time: {0:n2} ms", results.Average(req => req.TimeTakenMs)));
                sb.AppendLine(string.Format("      Min Time: {0:n2} ms", results.Min(req => req.TimeTakenMs)));
                sb.AppendLine(string.Format("      Max Time: {0:n2} ms", results.Max(req => req.TimeTakenMs)));
            }

            return sb.ToString();
        }

        public IEnumerable<TimeTakenResult> TimeLineDataForIndividualRequest(IEnumerable<HttpRequestData> resultData, string url)
        {
            var count = 0;
            return resultData.Where(rd=> rd.Url.ToLower() == url.ToLower())
                             .OrderBy(rd => rd.Timestamp)
                             .Select(rd => new TimeTakenResult()
                             {                       
                                 OrigId = rd.Id,
                                 RequestNo = count++, 
                                 TimeTaken = rd.TimeTakenMs,
                                 IsError = rd.IsError
                             }).ToList();
        }

        public IEnumerable<RequestsPerSecondResult> RequestsPerSecond(IEnumerable<HttpRequestData> resultData, string url = null)
        {
            
            if (!string.IsNullOrEmpty(url))
                resultData = resultData.Where(rd => rd.Url.ToLower() == url.ToLower());


            DateTime startTime = resultData.First().Timestamp;
            var res = resultData.OrderBy(rd=> rd.Timestamp)
                            .GroupBy(rd => (int) rd.Timestamp.Subtract(startTime).TotalSeconds,rd => rd,
                                           (second, rd) => new RequestsPerSecondResult() { Second = second, Requests = rd.Count() });                                     
                        
            return res.ToList();
        }
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
        public int TimeTaken { get; set;  }
        public bool IsError { get; set;  }
    }
}
