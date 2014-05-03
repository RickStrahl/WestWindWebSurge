using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSurge
{
    public class ResultsParser
    {
        public string ParseResults(IEnumerable<HttpRequestData> resultData, int totalTime)
        {
            var results = resultData.ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Total Requests: " + results.Count);
            sb.AppendLine("        Failed: " +
                              results.Count(req => string.IsNullOrEmpty(req.StatusCode) || !req.StatusCode.StartsWith("2")));

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

        public IEnumerable<int> TimeLineDataForIndividualRequest(IEnumerable<HttpRequestData> resultData, string url)
        {
            return resultData.OrderBy(rd => rd.Timestamp)
                             .Select(rd => rd.TimeTakenMs);
        }

    }
}
