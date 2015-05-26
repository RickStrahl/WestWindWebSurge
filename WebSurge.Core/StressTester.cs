using System.Diagnostics;
using System.Threading;
using Westwind.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Westwind.Utilities.InternetTools;

namespace WebSurge
{
    public class StressTester
    {
        /// <summary>
        /// Options that determine how requests are setup and configured
        /// </summary>
        public StressTesterConfiguration Options { get; set; }
        
        /// <summary>
        /// List of result request objects with data filled in.
        /// </summary>
        public List<HttpRequestData> Results { get; set; }

        /// <summary>
        /// Returns the the total time taken for the last set of 
        /// batch processing tasks.
        /// </summary>
        public int TimeTakenForLastRunMs { get; set; }

        /// <summary>
        /// Last number of threads used
        /// </summary>
        public int ThreadsUsed { get; set; }
        
        /// <summary>
        /// Set this property to stop processing requests
        /// </summary>
        public bool CancelThreads
        {
            get { return _CancelThreads; }
            set
            {
                if (value && Running)
                    TimeTakenForLastRunMs = (int) DateTime.UtcNow.Subtract(StartTime).TotalMilliseconds;

                _CancelThreads = value;
            }
        }
        private bool _CancelThreads = false;
        
        
        /// <summary>
        /// StartTime when the request is starting.
        /// </summary> 
        DateTime StartTime { get; set; }

        /// <summary>
        /// Determines if a test is in progress
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// Number of requests processed
        /// </summary>
        private int RequestsProcessed;

        /// <summary>
        /// Number of requests processed
        /// </summary>
        private int RequestsFailed;

        /// <summary>
        /// Results list Insert Lock
        /// </summary>
        static readonly object InsertLock = new object();

        /// <summary>
        /// Instance of a results parser
        /// </summary>
        public ResultsParser ResultsParser
        {
            get
            {
                if (_parser == null)
                    _parser = new ResultsParser();
                return _parser;
            }
        }
        private ResultsParser _parser;

        public List<IWebSurgeExtensibility> PlugIns = new List<IWebSurgeExtensibility>();

        /// <summary>
        /// Event fired after each request that provides the
        /// current request.
        /// </summary>
        public event Action<HttpRequestData> RequestProcessed;
        public void OnRequestProcessed(HttpRequestData request)
        {
            foreach (var plugin in App.Plugins)
            {
                try
                {
                    plugin.OnAfterRequestSent(request);
                }
                catch (Exception ex)
                {
                    App.Log(plugin.GetType().Name + " failed in OnBeforeRequestSent(): " + ex.Message);
                }
            }

            if (!Options.NoProgressEvents && RequestProcessed != null)
                RequestProcessed(request);
        }

        /// <summary>
        /// Event fired in intervals to provides progress information
        /// on the test. Provides a progress structure with fields
        /// with summary request data.
        /// </summary>
        public event Action<ProgressInfo> Progress;
        public void OnProgress(ProgressInfo progressInfo)
        {
            if (Progress != null)
                Progress(progressInfo);
        }

        public StressTester(StressTesterConfiguration options = null)
        {
            Options = options;
            if (options == null)
                Options = new StressTesterConfiguration();
            Options.MaxResponseSize = 5000;
            StartTime = new DateTime(1900, 1,1);
            
            Results = new List<HttpRequestData>();
        }
                
        /// <summary>
        /// Checks an individual site and returns a new HttpRequestData object
        /// </summary>
        /// <param name="reqData"></param>
        /// <returns></returns>
        public HttpRequestData CheckSite(HttpRequestData reqData)
        {
            // create a new instance
            var result = HttpRequestData.Copy(reqData);

            result.ErrorMessage = "Request is incomplete"; // assume not going to make it

            result.IsWarmupRequest =  StartTime.AddSeconds(Options.WarmupSeconds) > DateTime.UtcNow;

            try
            {
                var client = new HttpClient();
                

                    if (!string.IsNullOrEmpty(Options.ReplaceDomain))
                        result.Url = ReplaceDomain(result.Url);

                    if (!string.IsNullOrEmpty(Options.ReplaceQueryStringValuePairs))
                        result.Url = ReplaceQueryStringValuePairs(result.Url, Options.ReplaceQueryStringValuePairs);

                    client.CreateWebRequestObject(result.Url);
                    var webRequest = client.WebRequest;

                    if (!string.IsNullOrEmpty(Options.Username))
                    {
                        client.Username = Options.Username;
                        webRequest.UnsafeAuthenticatedConnectionSharing = true;
                    }
                    if (!string.IsNullOrEmpty(Options.Password))
                        client.Password = Options.Password;

                    webRequest.Method = reqData.HttpVerb;

                    client.ContentType = reqData.ContentType;
                    client.Timeout = Options.RequestTimeoutMs/1000;                    
                
                    // don't auto-add gzip headers and don't decode by default
                    client.UseGZip = false;

                    if (Options.NoContentDecompression)
                        webRequest.AutomaticDecompression = DecompressionMethods.None;
                    else
                        webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                    if (!string.IsNullOrEmpty(reqData.RequestContent))
                    {
                        var data = reqData.GetRequestContentBytes();
                        client.AddPostKey(data);
                    }
                    else
                    {
                        webRequest.ContentLength = 0;
                    }

                    foreach (var header in result.Headers)
                    {
                        var lheader = header.Name.ToLower();

                        // Header Overrides that fail if you try to set them
                        // directly in HTTP
                        if (lheader == "cookie" && !string.IsNullOrEmpty(Options.ReplaceCookieValue))
                        {
                            string cookie = Options.ReplaceCookieValue;
                            webRequest.Headers.Add("Cookie", cookie);
                            header.Value = cookie;
                            continue;
                        }
                        if (lheader == "authorization" && !string.IsNullOrEmpty(Options.ReplaceAuthorization))
                        {
                            webRequest.Headers.Add("Authorization", Options.ReplaceAuthorization);
                            header.Value = Options.ReplaceAuthorization;
                            continue;
                        }
                        if (lheader == "user-agent")
                        {
                            client.UserAgent = header.Value;
                            continue;
                        }
                        if (lheader == "accept")
                        {
                            webRequest.Accept = header.Value;
                            continue;
                        }
                        if (lheader == "referer")
                        {
                            webRequest.Referer = header.Value;
                            continue;
                        }
                        if (lheader == "connection")
                        {
                            if (header.Value.ToLower() == "keep-alive")
                                webRequest.KeepAlive = true; // this has no effect
                            else if (header.Value.ToLower() == "close")
                                webRequest.KeepAlive = false;
                            continue;
                        }
                        // set above view property
                        if (lheader == "content-type")
                            continue;
                        // not handled at the moment
                        if (lheader == "proxy-connection")
                            continue; // TODO: Figure out what to do with this one

                        // set explicitly via properties
                        if (lheader == "transfer-encoding")
                        {
                            webRequest.TransferEncoding = header.Value;
                            continue;
                        }
                        if (lheader == "date")
                            continue;
                        if (lheader == "expect")
                        {
                            //webRequest.Expect = header.Value;
                            continue;
                        }
                        if (lheader == "if-modified-since")
                            continue;

                        webRequest.Headers.Add(header.Name, header.Value);
                    }

                    foreach (var plugin in App.Plugins)
                    {
                        try
                        {
                            if (!plugin.OnBeforeRequestSent(result))
                                return result;
                        }
                        catch (Exception ex)
                        {
                            App.Log(plugin.GetType().Name + " failed in OnBeforeRequestSent(): " + ex.Message);
                        }
                    }

                    DateTime dt = DateTime.UtcNow;


                    string httpOutput = client.DownloadString(result.Url);

                    result.TimeTakenMs = (int) DateTime.UtcNow.Subtract(dt).TotalMilliseconds;
                    // result.TimeToFirstByteMs = client.Timings.TimeToFirstByteMs;

                    if (client.Error || client.WebResponse == null)
                    {
                        result.ErrorMessage = client.ErrorMessage;
                        return result;
                    }

                    if (CancelThreads)
                        return null;

                    result.StatusCode = ((int) client.WebResponse.StatusCode).ToString();
                    result.StatusDescription = client.WebResponse.StatusDescription ?? string.Empty;
                    result.TimeToFirstByteMs = client.HttpTimings.TimeToFirstByteMs;

                    result.ResponseLength = (int) client.WebResponse.ContentLength;
                    result.ResponseContent = httpOutput;

                    StringBuilder sb = new StringBuilder();
                    foreach (string key in client.WebResponse.Headers.Keys)
                    {
                        sb.AppendLine(key + ": " + client.WebResponse.Headers[key]);
                    }



                    result.ResponseHeaders = sb.ToString();

                    // update to actual Http headers sent
                    result.Headers.Clear();
                    foreach (string key in webRequest.Headers.Keys)
                    {
                        result.Headers.Add(new HttpRequestHeader() {Name = key, Value = webRequest.Headers[key]});
                    }

                    char statusCode = result.StatusCode[0];
                    if (statusCode == '4' || statusCode == '5')
                    {
                        result.IsError = true;
                        result.ErrorMessage = client.WebResponse.StatusDescription;
                    }
                    else
                    {
                        result.IsError = false;
                        result.ErrorMessage = null;

                        if (Options.MaxResponseSize > 0 && result.ResponseContent.Length > Options.MaxResponseSize)
                            result.ResponseContent = result.ResponseContent.Substring(0, Options.MaxResponseSize);
                    }

                //} // using client
                client.Dispose();

                if (!CancelThreads)
                    OnRequestProcessed(result);

                return result;
                
            }

            // these will occur on shutdown - don't log since they will return
            // unstable results - just ignore
            catch (ThreadAbortException)
            {                
                return null;
            }
            catch (Exception ex)
            {                
                Console.WriteLine("Exception: " + ex.Message);
                result.IsError = true;
                result.ErrorMessage = "CheckSite Error: " + ex.GetBaseException().Message;
                
                if (!CancelThreads)
                    OnRequestProcessed(result);

                return result;
            }
        }

        internal string ReplaceQueryStringValuePairs(string url, string replaceKeys)
        {
            if (string.IsNullOrEmpty(replaceKeys))
                return url;

            var urlQuery = new UrlEncodingParser(url);
            var replaceQuery = new UrlEncodingParser(replaceKeys);

            foreach (string key in replaceQuery.Keys)
            {
                urlQuery[key] = replaceQuery[key];
            }

            return urlQuery.ToString();
        }

        internal string ReplaceDomain(string url)
        {
            if (!string.IsNullOrEmpty(Options.ReplaceDomain))
            {
                var host = StringUtils.ExtractString(url, "://", "/", false, true);
                url = url.Replace(host, Options.ReplaceDomain);                
            }

            return url;
        }


        /// <summary>
        /// This is the main Session processing routine. This routine creates the
        /// new threads to run each session on. It monitors for shutdown/cancel operation
        /// and then shuts down the worker threads and summarizes the results.
        /// 
        /// The worker method call for each Session request processing is 
        /// SessionThreadRunner().
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="threadCount"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public List<HttpRequestData> CheckAllSites(IEnumerable<HttpRequestData> requests, 
                                                   int threadCount = 2, 
                                                   int seconds = 60,
                                                   bool runOnce = false)
        {
     

            
            ThreadsUsed = threadCount;

            //if (UnlockKey.RegType == RegTypes.Free &&
            //    (threadCount > UnlockKey.FreeThreadLimit ||
            //    requests.Count() > UnlockKey.FreeSitesLimit))
            //{
            //    Running = false;
            //    SetError("The free version is limited to " + UnlockKey.FreeSitesLimit + " urls to check and " + UnlockKey.FreeThreadLimit + " simultaneous threads.\r\n\r\n" +
            //            "Please reduce the URL or thread counts, or consider purchasing the Professional version that includes unlimited sites and threads.");                    
            //    return null;
            //}

            if (!runOnce)
            {
                var validator = new SiteValidator(this);
                if (!validator.CheckAllServers(requests))
                {
                    SetError(validator.ErrorMessage);
                    Running = false;
                    return null;
                }
            }


            Results = new List<HttpRequestData>();
            requests = requests.Where(req => req.IsActive).ToList();


            foreach (var plugin in App.Plugins)
            {
                try
                {
                    if (!plugin.OnLoadTestStarted(requests as List<HttpRequestData>))
                        return null;
                }
                catch (Exception ex)
                {
                    App.Log(plugin.GetType().Name + " failed in OnLoadTestStarted(): " + ex.Message);
                }
            }

            Running = true;
             
            var threads = new List<Thread>();
            CancelThreads = false;
            RequestsProcessed = 0;
            RequestsFailed = 0;

            // add warmup seconds to the request
            seconds += Options.WarmupSeconds;

            StartTime = DateTime.UtcNow;
            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread(RunSessions);
                thread.Start(requests);
                threads.Add(thread);
            }            

            var lastProgress = DateTime.UtcNow.AddSeconds(-10);            
            while (!CancelThreads)
            {
                if (DateTime.UtcNow.Subtract(StartTime).TotalSeconds  > seconds + 1)
                {
                    TimeTakenForLastRunMs = (int) DateTime.UtcNow.Subtract(StartTime).TotalMilliseconds;
                    
                    CancelThreads = true;

                    Thread.Sleep(3000);                    
                    foreach (var thread in threads)
                        thread.Abort();
                    Thread.Sleep(1000);

                    break;
                }
                Thread.Sleep(100);

                if (DateTime.UtcNow.Subtract(lastProgress).TotalMilliseconds > 950)
                {
                    lastProgress = DateTime.UtcNow;
                    
                    OnProgress(new ProgressInfo() 
                    {
                         SecondsProcessed = (int) DateTime.UtcNow.Subtract(StartTime).TotalSeconds,
                         TotalSecondsToProcessed = seconds,
                         RequestsProcessed = RequestsProcessed,
                         RequestsFailed = RequestsFailed,                         
                    });
                    
                }
            }

            Running = false;

            seconds = seconds - Options.WarmupSeconds;

            // strip off WarmupSeconds
            var results = Results.Where(res => !res.IsWarmupRequest);

            var result = results.FirstOrDefault();
            var min =  StartTime;
            if (result != null)
            {
                min = result.Timestamp;                
                min = TimeUtils.Truncate(min, DateTimeResolution.Second);
            }
            var max = min.AddSeconds(seconds + 1).AddMilliseconds(-1);

            Results = results.Where(res => res.Timestamp > min && res.Timestamp < max).ToList();

            if (Results.Count > 0)
            {
                max = Results.Max(res => res.Timestamp);
                TimeTakenForLastRunMs = (int) TimeUtils.Truncate(max).Subtract(min).TotalMilliseconds;
            }
            else
                TimeTakenForLastRunMs = (int) TimeUtils.Truncate(DateTime.UtcNow).Subtract(min).TotalMilliseconds;

            foreach (var plugin in App.Plugins)
            {
                try
                {
                    plugin.OnLoadTestCompleted(Results, TimeTakenForLastRunMs);
                }
                catch (Exception ex)
                {
                    App.Log(plugin.GetType().Name + " failed in OnLoadTestCompleted(): " + ex.Message);
                }
            }

            return Results;   
        }


        public void RunSessions(object requests)
        {
            RunSessions(requests, false);
        }

        /// <summary>
        /// Checks an entire session in a loop until CancelThreads
        /// or runOnce is true.
        /// 
        /// This routine runs through all the requests in a session
        /// one after the other (may be randomized based on options)
        /// until CancelThreads or runOnce is true. 
        /// 
        /// Typically called from CheckAllSites which sets up multiple
        /// threads each running this method.
        /// </summary>
        /// <param name="requests">HttpRequests to run</param>
        /// <param name="runOnce">When set only fires once</param>
        public void RunSessions(object requests, bool runOnce = false)
        {
            List<HttpRequestData> reqs = null;
            bool isFirstRequest = true;

            if (Options.RandomizeRequests)
            {
                var rqs = requests as List<HttpRequestData>;
                reqs = new List<HttpRequestData>();
                
                var r = new Random();
                foreach (var req in rqs
                    .OrderBy(rq => r.NextDouble())
                    .ToList())
                {
                    reqs.Add(req);
                }
                rqs = null;
            }
            else
                reqs = requests as List<HttpRequestData>;
            
            while (!CancelThreads)
            {

                foreach (var req in reqs)
                {
                    if (CancelThreads)                                            
                        break;

                    var result = CheckSite(req);

                    if (result != null)
                        WriteResult(result);

                    if (Options.DelayTimeMs == 0)
                    {
                        Thread.Yield();
                    }
                    else if (Options.DelayTimeMs < 0)
                    {
                        // no yielding - can generate more requests but much more cpu usage
                    }
                    else
                        Thread.Sleep(Options.DelayTimeMs);  
                }

                if (runOnce)
                    break;
            }
        }

        /// <summary>
        /// Writes the actual result to the storage container
        /// </summary>
        /// <param name="result"></param>
        public virtual void WriteResult(HttpRequestData result)
        {
            // don't log request detail data for non errors over a certain no of requests
            if (!result.IsError && Results.Count > 3000)
            {
                // always clear response
                result.ResponseContent = null;

                // detail data only if we explicitly requested
                if (Options.CaptureMinimalResponseData)
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

        /// <summary>
        /// Creates a string
        /// </summary>
        /// <param name="results">Results to process. If not passed uses internal Results</param>
        /// <param name="totalTime">Total request time in seconds</param>
        /// <returns></returns>
        public string ParseResults(IEnumerable<HttpRequestData> resultData = null, int totalTime = 0)
        {
            if (resultData == null)
                resultData = Results;
            if (totalTime == 0)
                totalTime = TimeTakenForLastRunMs/1000;
            
            return ResultsParser.ParseResultsToString(resultData, totalTime, ThreadsUsed);
        }

        

        /// <summary>
        /// Parses Fiddler Session Trace files into a list of HttpRequestData
        /// objects.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<HttpRequestData> ParseSessionFile(string fileName)
        {
            var parser = new SessionParser();
            
            var options = Options;
            var requestDataList = parser.ParseFile(fileName,ref options);
            if (options != null)
                Options = options;

            if (requestDataList == null)
            {
                SetError(parser.ErrorMessage);
                return null;
            }

            return requestDataList;
        }


        public string ErrorMessage { get; set; }

        protected void SetError()
        {
            SetError("CLEAR");
        }

        protected void SetError(string message)
        {
            if (message == null || message == "CLEAR")
            {
                ErrorMessage = string.Empty;
                return;
            }
            ErrorMessage = message;
        }

        protected void SetError(Exception ex, bool checkInner = false)
        {
            if (ex == null)
                ErrorMessage = string.Empty;

            Exception e = ex;
            if (checkInner)
                e = e.GetBaseException();

            ErrorMessage = e.Message;
        }        
    }

    public class ProgressInfo
    {
        public int SecondsProcessed { get; set; }
        public int TotalSecondsToProcessed { get; set; }
        public int RequestsProcessed { get; set; }
        public int RequestsFailed { get; set; }        
    }
}

#if false
    
        public async Task<List<HttpRequestData>> CheckAllSitesAysnc(IEnumerable<HttpRequestData> requests, int iterations = 1, int loadFactor = 1)
        {
            Results.Clear();

            var tasks = new List<Task<HttpRequestData>>();

            if (iterations < 1)
                iterations = 1;


            for (int i = 0; i < iterations; i++)
            {
                foreach (var reqData in requests)
                {
                    for (int j = 0; j < loadFactor; j++)
                    {
                        // add right away so we can see incomplete requests
                        Results.Add(reqData);

                        var tsk = CheckSiteAsync(reqData).ContinueWith(task =>
                        {
                            var result = task.Result;
                            OnRequestProcessed(result);
                            //Results.Add(result); 
                            return result;
                        }, TaskContinuationOptions.ExecuteSynchronously);

                        tasks.Add(tsk);
                    }
                }
                var results = await Task.WhenAny(Task.WhenAll<HttpRequestData>(tasks), Task.Delay(10000));
            }

            return Results;
        }



        public async Task<HttpRequestData> CheckSiteAsync(HttpRequestData reqData = null)
        {
            var result = reqData;

            try
            {
                var mh = new HttpClientHandler()
                {
                    // turn off Cookie Container
                    UseCookies = false
                };

                if (!string.IsNullOrEmpty(reqData.Username)) ;
                mh.Credentials = new NetworkCredential(reqData.Username, reqData.Password);

                mh.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                var client = new HttpClient(mh);

                var swatch = new Stopwatch();
                swatch.Start();

                HttpResponseMessage response = null;
                StringContent content = null;


                if (!string.IsNullOrEmpty(reqData.RequestContent))
                {

                    if (string.IsNullOrEmpty(reqData.ContentType))
                    {
                        content = new StringContent(reqData.RequestContent, Encoding.UTF8);
                        content.Headers.ContentType = new MediaTypeHeaderValue(reqData.ContentType);
                    }
                }

                // set headers etc.                                
                client.Timeout = TimeSpan.FromMilliseconds(Options.RequestTimeoutMs);

                foreach (var header in reqData.Headers)
                {
                    // skip encoding
                    if (header.Name == "Cookie" && !string.IsNullOrEmpty(Options.ReplaceCookieValue))
                    {
                        string cookie = Options.ReplaceCookieValue;
                        client.DefaultRequestHeaders.Add("Cookie", cookie);
                        continue;
                    }

                    client.DefaultRequestHeaders.Add(header.Name, header.Value);
                }


                if (reqData.HttpVerb == "GET")
                {
                    // full read happens here
                    response = await client.GetAsync(reqData.Url, HttpCompletionOption.ResponseHeadersRead);
                }
                else if (reqData.HttpVerb == "HEAD")
                {
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(reqData.Url),
                        Method = HttpMethod.Head
                    };
                    response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                }
                else if (reqData.HttpVerb == "POST")
                {
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(reqData.Url),
                        Method = HttpMethod.Post,
                        Content = content
                    };
                    response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    //response = await client.PostAsync(reqData.Url, content);
                }
                else if (reqData.HttpVerb == "PUT")
                    response = await client.PutAsync(reqData.Url, content);
                else if (reqData.HttpVerb == "DELETE")
                    response = await client.DeleteAsync(reqData.Url);

                result.StatusCode = ((int)response.StatusCode).ToString();

                swatch.Stop();
                result.TimeTakenSecs = swatch.ElapsedMilliseconds;

                HttpResponseMessage msg;

                if (!response.IsSuccessStatusCode)
                {
                    result.IsError = true;
                    result.ErrorMessage = response.ReasonPhrase;
                    return result;
                }
                result.IsError = false;
                result.ErrorMessage = null;

                if (response.Content != null)
                    result.ResponseContent = await response.Content.ReadAsStringAsync();

                if (Options.MaxResponseSize > 0 && result.ResponseContent.Length > Options.MaxResponseSize)
                    result.ResponseContent = result.ResponseContent.Substring(0, Options.MaxResponseSize);

                OnRequestProcessed(result);

                return result;
            }
            catch (ThreadAbortException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.ErrorMessage = "CheckSiteAsync Error: " + ex.GetBaseException().Message;
                OnRequestProcessed(result);

                return result;
            }
        }
#endif
