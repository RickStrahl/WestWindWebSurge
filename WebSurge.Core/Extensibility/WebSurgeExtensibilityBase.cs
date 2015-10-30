using System.Collections.Generic;

namespace WebSurge
{

    /// <summary>
    /// Empty base implementation of IWebSurgeExtensibility. Inherit from this 
    /// class to let you implement just the methods you are interested in 
    /// </summary>
    public abstract class WebSurgeExtensibilityBase : IWebSurgeExtensibility
    {
        /// <summary>
        /// Pre-request processing of the request that is
        /// fired against the server.
        /// </summary>
        /// <remarks>
        /// These events are fired asynchronously and have to be threadsafe. The data
        /// instance passed is thread-safe, but any other resources you use may not be.
        /// For example, if you write output to a file make sure you put a Sync lock on
        /// the file to avoid simultaneous access by multiple threads.
        /// </remarks>
        /// <param name="data"></param>
        /// <returns>return true to process the request, false to abort processing and skip this request</returns>
        public virtual bool OnBeforeRequestSent(HttpRequestData data)
        {
            return true;
        }

        /// <summary>
        /// Post-request processing of the request that has
        /// been fired agains the server. The passed request data
        /// contains the full completed request data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual void OnAfterRequestSent(HttpRequestData data)
        {
        }


        /// <summary>
        /// This method is fired when a test run is started. You are passed a list
        /// of the requests that are to be fired. These are raw requests as entered
        /// in the file without any rule processing applied (you can look at transformed
        /// requests in OnBeforeRequestSent).
        /// </summary>
        /// <param name="requests">The list of requests that will be processed</param>
        /// <returns>return true to run the test, false to stop processing</returns>
        public virtual bool OnLoadTestStarted(IList<HttpRequestData> requests)
        {
            return true;
        }

        /// <summary>
        /// This method is fired when a test has completed running or was cancelled. You
        /// are passed a large list of every request that was fired during the test. Note
        /// this list can be potentially VERY large.
        /// </summary>
        /// <param name="results">List of every request run in the test - can be very large!</param>
        /// <param name="timeTakenForTestMs">Milliseconds take for test</param>
        public virtual void OnLoadTestCompleted(IList<HttpRequestData> results, int timeTakenForTestMs)
        {
        }
    }
}
