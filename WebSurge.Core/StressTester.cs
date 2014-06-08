using System.Diagnostics;
using System.Threading;
using Westwind.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSurge
{
    public class StressTester
    {

        /// <summary>
        /// Options that determine how requests are setup and configured
        /// </summary>
        public RequestProcessingOptions Options { get; set; }
        
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
        /// Event fired after each request that provides the
        /// current request.
        /// </summary>
        public event Action<HttpRequestData> RequestProcessed;
        public void OnRequestProcessed(HttpRequestData request)
        {
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

        public StressTester(RequestProcessingOptions options = null)
        {
            Options = options;
            if (options == null)
                Options = new RequestProcessingOptions();
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
                var client = new Westwind.Utilities.InternetTools.HttpClient();
                
                if (!string.IsNullOrEmpty(Options.ReplaceDomain))
                {
                    var host = StringUtils.ExtractString(result.Url, "://", "/", false, true);
                    result.Url = result.Url.Replace(host, Options.ReplaceDomain);
                }
                
                client.CreateWebRequestObject(result.Url);
                var webRequest = client.WebRequest;

                webRequest.Method = reqData.HttpVerb;
                client.UseGZip = true;                

                client.ContentType = reqData.ContentType;
                client.Timeout = Options.RequestTimeoutMs;           

                if (!string.IsNullOrEmpty(reqData.RequestContent))
                {
                    client.AddPostKey(reqData.RequestContent);
                }

                foreach (var header in reqData.Headers)
                {
                    var lheader = header.Name.ToLower();

                    // Header Overrides that fail if you try to set them
                    // directly in HTTP
                    if (lheader == "cookie" && !string.IsNullOrEmpty(Options.ReplaceCookieValue))
                    {
                        string cookie = Options.ReplaceCookieValue;
                        webRequest.Headers.Add("Cookie", cookie);
                        continue;
                    }
                    if (lheader == "user-agent")
                    {
                        webRequest.UserAgent = header.Value;
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
                            webRequest.KeepAlive = true;
                        else if (header.Value.ToLower() == "close")
                            webRequest.KeepAlive = false;                        
                        continue;
                    }
                    if (lheader == "if-modified-since")                        
                        continue;                    

                    webRequest.Headers.Add(header.Name, header.Value);
                }


                DateTime dt = DateTime.UtcNow;

                string http = client.GetUrl(reqData.Url);
                
                result.TimeTakenMs = (int) DateTime.UtcNow.Subtract(dt).TotalMilliseconds;                
                
                if (client.Error || client.WebResponse == null)
                {
                    result.ErrorMessage = client.ErrorMessage;
                    return result;
                }

                //// don't notify
                //if (result.IsWarmupRequest)
                //    return result;

                var webResponse = client.WebResponse;

                if (CancelThreads)
                    return null;

                result.StatusCode = ((int) client.WebResponse.StatusCode).ToString();
                result.StatusDescription = client.WebResponse.StatusDescription ?? string.Empty;
                    
                result.ResponseLength = result.ResponseLength;

                StringBuilder sb = new StringBuilder();
                foreach (string key in webResponse.Headers.Keys)
                {
                    sb.AppendLine( key + ": " + webResponse.Headers[key]);
                }

                result.ResponseHeaders = sb.ToString();

                char statusCode = result.StatusCode[0];
                if (statusCode == '4' || statusCode == '5') 
                {
                    result.IsError = true;
                    result.ErrorMessage = webResponse.StatusDescription;
                    
                    if (!CancelThreads)
                        OnRequestProcessed(result);

                    return result;
                }
                result.IsError = false;
                result.ErrorMessage = null;

                result.LastResponse = http;

                if (Options.MaxResponseSize > 0 && result.LastResponse.Length > Options.MaxResponseSize)
                    result.LastResponse = result.LastResponse.Substring(0, Options.MaxResponseSize);

                if (!CancelThreads)
                    OnRequestProcessed(result);

                return result;
            }
            // these will occur on shutdown - don't log since they will return
            // unstable results - just ignore
            catch (ThreadAbortException ex)
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

        

        /// <summary>
        /// Checks all sites using separate threads that run 
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="threadCount"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public List<HttpRequestData> CheckAllSites(IEnumerable<HttpRequestData> requests, 
                                                   int threadCount = 2, 
                                                   int seconds = 60)
        {

            this.ThreadsUsed = threadCount;

            if (UnlockKey.RegType == RegTypes.Free &&
                (threadCount > UnlockKey.FreeThreadLimit ||
                requests.Count() > UnlockKey.FreeSitesLimit))
            {
                Running = false;
                SetError("The free version is limited to " + UnlockKey.FreeSitesLimit + " urls to check and " + UnlockKey.FreeThreadLimit + " simultaneous threads.\r\n\r\n" +
                        "Please reduce the URL or thread counts, or consider purchasing the Professional version that includes unlimited sites and threads.");                    
                return null;
            }

            Running = true;            
            Results.Clear();
            
            var threads = new List<Thread>();
            CancelThreads = false;
            RequestsProcessed = 0;
            RequestsFailed = 0;

            // add warmup seconds to the request
            seconds += Options.WarmupSeconds;
            
            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread(CheckSiteThreadRunner);
                thread.Start(requests);
                threads.Add(thread);
            }

            StartTime = DateTime.UtcNow;

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

            return Results;   
        }

        

        private void CheckSiteThreadRunner(object requests)
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

                    if (isFirstRequest)
                    {
                        // don't record first request on thread
                        isFirstRequest = false;
                        continue;
                    }

                    // add before so  we can see incomplete requests
                    if (result != null)
                    {
                        lock (InsertLock)
                        {
                            Results.Add(result);
                            RequestsProcessed++;
                            if (result.IsError)
                                RequestsFailed++;
                        }
                    }

                    if (Options.DelayTimeMs == 0)                    
                        //Thread.Yield();
                        Thread.Sleep(1);                    
                    else
                        Thread.Sleep(Options.DelayTimeMs);  
                }
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

            var parser = new ResultsParser();
            return parser.ParseResults(resultData, totalTime, ThreadsUsed);
        }

   

        /// <summary>
        /// Parses Fiddler Session Trace files into a list of HttpRequestData
        /// objects.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<HttpRequestData> ParseFiddlerSessions(string fileName)
        {
            var parser = new FiddlerSessionParser();
            var requestDataList = parser.Parse(fileName);
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
                result.TimeTakenMs = swatch.ElapsedMilliseconds;

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
                    result.LastResponse = await response.Content.ReadAsStringAsync();

                if (Options.MaxResponseSize > 0 && result.LastResponse.Length > Options.MaxResponseSize)
                    result.LastResponse = result.LastResponse.Substring(0, Options.MaxResponseSize);

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
