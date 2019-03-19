using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;

namespace WebSurge
{

    public class MemoryCollectionRquestWriter : RequestWriterBase
    {
        public MemoryCollectionRquestWriter(StressTester tester) : base(tester)
        {
        }
    }


    /// <summary>
    /// Writes Request Data into a simple collection all in memory
    /// </summary>
    public class RequestWriterBase : IDisposable
    {
        /// <summary>
        /// List of result request objects with data filled in.
        /// </summary>
       protected List<HttpRequestData> Results { get; set; }
       
        public int RequestsFailed { get; set; }
        public int RequestsProcessed { get; set; }

        protected readonly StressTester _stressTester;
       

        private static object InsertLock = new object();

        public RequestWriterBase(StressTester stressTester)
        {
            Results = new List<HttpRequestData>();
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
                Results.Add(result);
                RequestsProcessed++;
                if (result.IsError)
                    RequestsFailed++;
            }
        }

        public virtual List<HttpRequestData> GetResults()
        {
            return Results;
        }

        public virtual void Dispose()
        {
            Results = null;
        }
    }
}