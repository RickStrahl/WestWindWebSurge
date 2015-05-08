

namespace WebSurge
{
    /// <summary>
    /// WebSurge extensibility class that can be used to intercept each request before it
    /// is sent to the server. Allows for modification of the request data and custom
    /// logging or update oprations.
    /// 
    /// Please note that these events are asynchronous and multi-threaded so take care
    /// that you write thread safe code. You should also keep these methods as quick as
    /// possible as they are fired on each request and can result in decreased throughput
    /// as requests are fired.
    /// </summary>
    public interface IWebSurgeExtensibility
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
        bool OnBeforeRequestSent(HttpRequestData data);

        /// <summary>
        /// Post-request processing of the request that has
        /// been fired agains the server. The passed request data
        /// contains the full completed request data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        void OnAfterRequestSent(HttpRequestData data);
    }
}
