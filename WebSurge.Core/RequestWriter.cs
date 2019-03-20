using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;

namespace WebSurge
{
    


    /// <summary>
    /// Writes Request Data into a simple collection all in memory
    /// </summary>
    public class RequestWriter : IDisposable
    {
        /// <summary>
        /// List of result request objects with data filled in.
        /// </summary>
       protected List<HttpRequestData> Results { get; set; }
       
        public int RequestsFailed { get; set; }
        public int RequestsProcessed { get; set; }

        public int MaxSucessRequestsToCapture { get; set; } = 1_999_999_999;

        protected readonly StressTester _stressTester;
       

        private static object InsertLock = new object();

        public RequestWriter(StressTester stressTester, List<HttpRequestData> resultData = null)
        {
            if (resultData == null)
                Results = new List<HttpRequestData>();
            else
                Results = resultData;

            _stressTester = stressTester;

        }

        public virtual void Write(HttpRequestData result)
        {
            // don't log request detail data for non errors over a certain no of requests
            if (!result.IsError && RequestsProcessed > 30000)
            {
                // always clear response
                result.ResponseContent = null;

                // detail data only if we explicitly requested
                if (_stressTester.Options.CaptureMinimalResponseData)
                {
                    result.Headers = null;
                    result.ResponseHeaders = null;
                    result.FullRequest = null;
                    result.RequestContent = null;
                }
            }

            lock (InsertLock)
            {
                RequestsProcessed++;
                
                // log unless we are over the limit of requests we want to capture
                // Errors are ALWAYS blocked
                if (RequestsProcessed < MaxSucessRequestsToCapture || result.IsError)
                    Results.Add(result);

                if (result.IsError)
                    RequestsFailed++;
            }
        }

        public virtual void Clear()
        {
            RequestsProcessed = 0;
            RequestsFailed = 0;
           
            Results = new List<HttpRequestData>();
        }

        public virtual List<HttpRequestData> GetResults()
        {
            return Results;
        }

        public virtual void SetResults(List<HttpRequestData> requestData)
        {
            Results = requestData;
        }

        public virtual void Dispose()
        {
            Results = null;
        }
    }
}