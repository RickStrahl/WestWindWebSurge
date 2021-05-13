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
        public List<HttpRequestData> Results {
            get
            {
                if (RequestWriter == null)
                    return new RequestWriter(this).GetResults();

                return RequestWriter?.GetResults();
            }
        }

        /// <summary>
        /// Returns the the total time taken for the last set of 
        /// batch processing tasks.
        /// </summary>
        public int TimeTakenForLastRunMs { get; set; }

        /// <summary>
        /// Last number of threads used
        /// </summary>
        public int ThreadsUsed { get; set; }


        public RequestWriter RequestWriter { get; set; }



        /// <summary>
        /// Cookie Container used for the Interactive Session
        /// </summary>
        public static CookieContainer InteractiveSessionCookieContainer
        {
            get
            {
                if (App.Configuration.StressTester.TrackPerSessionCookies)
                    return _interactiveSessionCookieContainer ??
                           (_interactiveSessionCookieContainer = new CookieContainer());
                return null;
            }
            set => _interactiveSessionCookieContainer = value;
        }
        private static CookieContainer _interactiveSessionCookieContainer;

        /// <summary>
        /// Set this property to stop processing requests
        /// </summary>
        public bool CancelThreads
        {
            get { return _CancelThreads; }
            set
            {
                if (value && Running)
                    TimeTakenForLastRunMs = (int)DateTime.UtcNow.Subtract(StartTime).TotalMilliseconds;

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

        public bool IsWarmingUp { get; set; }

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

        public List<IWebSurgeExtensibility> Addins = new List<IWebSurgeExtensibility>();

        #region Initialization
        public StressTester(StressTesterConfiguration options = null)
        {
            Options = options;
            if (options == null)
                Options = new StressTesterConfiguration();
            Options.MaxResponseSize = 5000;
            StartTime = new DateTime(1900, 1, 1);

            RequestWriter = new RequestWriter(this);

        }

        #endregion

        #region CheckSite

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
        public List<HttpRequestData> CheckAllSites(List<HttpRequestData> requests,
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

            RequestWriter.Clear();

            requests = requests.Where(req => req.IsActive).ToList();

            foreach (var addin in App.Addins)
            {
                try
                {
                    if (!addin.OnLoadTestStarted(requests as List<HttpRequestData>))
                        return null;
                }
                catch (Exception ex)
                {
                    App.Log(addin.GetType().Name + " failed in OnLoadTestStarted(): " + ex.Message);
                }
            }

            Running = true;
            IsWarmingUp = true;

            var threads = new List<Thread>();
            CancelThreads = false;

            RequestWriter.RequestsProcessed = 0;
            RequestWriter.RequestsFailed = 0;

            // add warmup seconds to the request

            StartTime = DateTime.UtcNow;

            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread(RunSessions);
                thread.Start(new  RunSessionsThreadStartParameter
                {
                    Requests = requests,
                    Id = i
                });
                threads.Add(thread);
            }

            // Control loop/thread  that checks for test completion
            var lastProgress = DateTime.UtcNow.AddSeconds(-10);
            while (!CancelThreads)
            {
                // check for test done by time specified
                if (DateTime.UtcNow.Subtract(StartTime).TotalSeconds > seconds)
                {
                    TimeTakenForLastRunMs = (int)DateTime.UtcNow.Subtract(StartTime).TotalMilliseconds;

                    CancelThreads = true;

                    OnProgress(new ProgressInfo()
                    {
                        SecondsProcessed = seconds,
                        TotalSecondsToProcess = seconds,
                        RequestsProcessed = RequestWriter.RequestsProcessed,
                        RequestsFailed = RequestWriter.RequestsFailed,
                        IsWarmingUp = IsWarmingUp
                    });

                    // allows some time to catch up
                    Thread.Sleep(2000);

                    OnProgress(new ProgressInfo()
                    {
                        SecondsProcessed = seconds,
                        TotalSecondsToProcess = seconds,
                        RequestsProcessed = RequestWriter.RequestsProcessed,
                        RequestsFailed = RequestWriter.RequestsFailed,
                        IsWarmingUp = IsWarmingUp
                    });

                    Thread.Sleep(1000);

                    foreach (var thread in threads)
                        thread.Abort();
                    Thread.Sleep(1000);

                    break;
                }
                Thread.Sleep(100);

                if (DateTime.UtcNow.Subtract(lastProgress).TotalMilliseconds > 990)
                {
                    lastProgress = DateTime.UtcNow;

                    var secs = seconds;
                    if (secs < 0)
                        secs = 0;

                    OnProgress(new ProgressInfo()
                    {
                        SecondsProcessed = (int)DateTime.UtcNow.Subtract(StartTime).TotalSeconds,
                        TotalSecondsToProcess = secs,
                        RequestsProcessed = RequestWriter.RequestsProcessed,
                        RequestsFailed = RequestWriter.RequestsFailed,
                        IsWarmingUp = IsWarmingUp
                    });
                }
            }

            

            Running = false;

            var originalResults = RequestWriter.GetResults();

            // strip off WarmupSeconds
            var results = originalResults;

            var result = results.FirstOrDefault();
            var min = StartTime;
            if (result != null)
            {
                min = result.Timestamp;
                min = TimeUtils.Truncate(min, DateTimeResolution.Second);
            }
            var max = min.AddSeconds(seconds + 1).AddMilliseconds(-1);

            originalResults = results.Where(res => res.Timestamp > min && res.Timestamp < max).ToList();

            if (originalResults.Count > 0)
            {
                max = originalResults.Max(res => res.Timestamp);
                TimeTakenForLastRunMs = (int)TimeUtils.Truncate(max).Subtract(min).TotalMilliseconds;
            }
            else
                TimeTakenForLastRunMs = (int)TimeUtils.Truncate(DateTime.UtcNow).Subtract(min).TotalMilliseconds;

            foreach (var addin in App.Addins)
            {
                try
                {
                    addin.OnLoadTestCompleted(originalResults, TimeTakenForLastRunMs);
                }
                catch (Exception ex)
                {
                    App.Log(addin.GetType().Name + " failed in OnLoadTestCompleted(): " + ex.Message);
                }
            }

            return originalResults;
        }

        private static object _warmupLock = new object();

        /// <summary>
        /// Checks an individual site and returns a new HttpRequestData object
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="cookieContainer">Cookies cached for this session. Cookies are reset when an individual session (thread) restarts processing a squences of URLs</param>
        /// <param name="user">An optional user to login</param>
        /// <param name="threadNumber"></param>
        /// <returns>result summary data</returns>
        public HttpRequestData CheckSite(HttpRequestData reqData, 
            CookieContainer cookieContainer = null,             
            int threadNumber = 0)
        {
            if (CancelThreads)
                return null;

            // Important: create a new instance so we can overwrite properties
            //            without affecting original list data
            var result = HttpRequestData.Copy(reqData);
            result.ThreadNumber = threadNumber;

            result.ErrorMessage = "Request is incomplete"; // assume not going to make it

            result.IsWarmupRequest = StartTime.AddSeconds(Options.WarmupSeconds) > DateTime.UtcNow;
            if (IsWarmingUp && !result.IsWarmupRequest)
            {
                lock (_warmupLock)
                {
                    if (IsWarmingUp)
                    {
                        RequestWriter.Clear();
                        StartTime = DateTime.UtcNow;
                        IsWarmingUp = false;
                    }
                }
            }

            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(Options.ReplaceDomain) || !string.IsNullOrEmpty(Options.SiteBaseUrl) )
                        result.Url = FixupUrl(result.Url);

                    if (!string.IsNullOrEmpty(Options.ReplaceQueryStringValuePairs))
                        result.Url = ReplaceQueryStringValuePairs(result.Url, Options.ReplaceQueryStringValuePairs);

                    foreach (var addin in App.Addins)
                    {
                        try
                        {
                            if (!addin.OnBeforeRequestSent(result))
                                return result;
                        }
                        catch (Exception ex)
                        {
                            App.Log(addin.GetType().Name + " failed in OnBeforeRequestSent(): " + ex.Message);
                        }
                    }

                    client.CreateWebRequestObject(result.Url);
                    var webRequest = client.WebRequest;
                    if (webRequest == null)
                        throw new InvalidOperationException($"Couldn't create Webclient - {client.ErrorMessage}");


                    // TODO: Connection Groups might help with sharing connections more efficiently
                    // Initial tests show no improvements - more research required
                    //webRequest.ConnectionGroupName = "_WebSurge_" + Thread.CurrentContext.ContextID;

                    if (!string.IsNullOrEmpty(Options.Username))
                    {
                        client.Username = Options.Username;
                        webRequest.UnsafeAuthenticatedConnectionSharing = true;
                    }
                    if (!string.IsNullOrEmpty(Options.Password))
                        client.Password = Options.Password;

                    client.HttpVerb = result.HttpVerb;
                    
                    client.ContentType = result.ContentType;
                    client.PostMode = HttpPostMode.Raw;   // have to force raw data

                    client.TimeoutMs = Options.RequestTimeoutMs;
                    if (client.TimeoutMs < 10)
                        client.TimeoutMs = 10; // minimum of 10ms
                   
                    // don't auto-add gzip headers and don't decode by default
                    client.UseGZip = false;

                    if (Options.NoContentDecompression)
                        webRequest.AutomaticDecompression = DecompressionMethods.None;
                    else
                        webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                    if (!string.IsNullOrEmpty(result.RequestContent))
                    {
                        var data = result.GetRequestContentBytes();
                        client.AddPostKey(data);
                    }
                    //else
                    //    webRequest.ContentLength = 0;

                    HandleCookies(result, cookieContainer, client);

                    if (App.Configuration.StressTester.Users != null && App.Configuration.StressTester.Users.Count > 0)
                        HandleUser(result,client);

                    // *** Important! Get out as quickly as possible
                    if (CancelThreads)
                        return null;

                    // *** REQUEST RUNS
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    // Download the content and format the string data stored in the result
                    var httpOutput = DownloadAndFormatHttpOutput(client, result.Url);
                    
                    if (CancelThreads)
                        return null;

                    sw.Stop();
                    result.TimeTakenMs = (int)sw.ElapsedMilliseconds;  //(int)DateTime.UtcNow.Subtract(dt).TotalMilliseconds;

                    

                    if (client.Error || client.WebResponse == null)
                    {
                        result.ErrorMessage = client.ErrorMessage;
                        return result;
                    }
                    // *** REQUEST DONE


                    result.StatusCode = ((int)client.WebResponse.StatusCode).ToString();
                    result.StatusDescription = client.WebResponse.StatusDescription ?? string.Empty;
                    result.TimeToFirstByteMs = client.HttpTimings.TimeToFirstByteMs;

                    result.ResponseLength = (int)client.WebResponse.ContentLength;
                    if (result.ResponseLength < 1 && !string.IsNullOrEmpty(httpOutput))
                        result.ResponseLength = httpOutput.Length;
                    
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
                        result.Headers.Add(new HttpRequestHeader()
                        {
                            Name = key,
                            Value = webRequest.Headers[key]
                        });                        
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
                }

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
                result.IsError = true;
                result.ErrorMessage = "CheckSite Error: " + ex.GetBaseException().Message;

                if (!CancelThreads)
                    OnRequestProcessed(result);

                return result;
            }
        }

        public Task<HttpRequestData> CheckSiteAsync(HttpRequestData reqData,
            CookieContainer cookieContainer = null,
            int threadNumber = 0)
        {
            return Task.Run<HttpRequestData>( ()=> CheckSite(reqData, cookieContainer, threadNumber) );
        }

        public string DownloadAndFormatHttpOutput(HttpClient client, string url)
        {
            // using West Wind HttpClient
            //string httpOutput = client.DownloadString(result.Url);
            byte[] bytes = client.DownloadBytes(url, 8192);
            if (client.WebResponse == null)
                return string.Empty;

            string httpOutput = string.Empty;


            // assume content is string that needs to be decoded
            try
            {
                if (!string.IsNullOrEmpty(client.WebResponse.CharacterSet))
                {
                    string charset = client.WebResponse.CharacterSet.ToLower();
                    var encoding = Encoding.ASCII;

                    // special case UTF-8 since it's most common
                    if (charset.Contains("utf-8"))
                        encoding = Encoding.UTF8;
                    else if (charset.Contains("utf-16"))
                        encoding = Encoding.Unicode;
                    else if (charset.Contains("utf-7"))
                        encoding = Encoding.UTF7;
                    else if (charset.Contains("utf-32"))
                        encoding = Encoding.UTF32;
                    else
                        encoding = Encoding.GetEncoding(charset);

                    httpOutput = encoding.GetString(bytes);
                }
                else
                {
                    var contentType = client.WebResponse.ContentType;
                    if (bytes == null)
                        httpOutput = string.Empty;
                    else if (string.IsNullOrEmpty(contentType) ||
                             contentType.StartsWith("text/") ||
                             contentType.StartsWith("application/json") ||
                             contentType.StartsWith("application/xml"))
                        httpOutput = Encoding.UTF8.GetString(bytes); // no encoding - assume utf-8
                    else if (contentType.StartsWith("image/") ||
                             (contentType.StartsWith("application/") &&
                              !(contentType == "application/json" || contentType == "application/xml")))
                        httpOutput = $"data:{contentType};base64," + Convert.ToBase64String(bytes);
                    else // no content type or encoding assume UTF-8 text (may be wrong - best guess)
                        httpOutput = Encoding.UTF8.GetString(bytes); // default to UTF 8
                }
            }
            catch
            {
            } // ignore encoding assignment failures

            return httpOutput;
        }

        private void HandleCookies(HttpRequestData result,
            CookieContainer cookieContainer,
            HttpClient client)                        
        {            
            foreach (var header in result.Headers)
            {
                if (!header.Name.Equals("Cookies", StringComparison.InvariantCultureIgnoreCase))
                    SetHttpHeader(header, client);
            }

            // assign cookies if exist - Cookie Container clears existing cookies
            if (cookieContainer != null)
            {
                client.WebRequest.CookieContainer = cookieContainer;
                result.Cookies = cookieContainer;

                var cookie = result.Headers.FirstOrDefault(hd => hd.Name == "Cookie");
                string cookieValue = cookie?.Value;

                if (!string.IsNullOrEmpty(cookieValue) && !string.IsNullOrEmpty(result.Url))
                {
                    // check if the cookie already exists in the container
                    var cookieCollection = cookieContainer.GetCookies(new Uri(result.Url));

                    var fullCookies = cookieValue.Split(';');
                    for (var i = 0; i < fullCookies.Length; i++)
                    {
                        var fullCookie = fullCookies[i].Trim();

                        var ix = fullCookie.IndexOf('=');
                        if (ix < 0)
                            continue;

                        string key = fullCookie.Substring(0, ix);
                        string value = null;
                        if (fullCookie.Length > ix + 1)
                            value = fullCookie.Substring(ix + 1);

                        bool exists = false;
                        foreach (Cookie ck in cookieCollection)
                        {
                            if (ck.Name == key)
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (!exists)
                        {
                            cookieContainer.Add(new Cookie(key,
                                WebUtility.UrlEncode(value), "/",
                                new Uri(result.Url).DnsSafeHost));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handle user overrides for a provided Auth Cookie, or specific
        /// Login Urls
        /// </summary>
        /// <param name="user"></param>
        /// <param name="client"></param>
        private void HandleUser(HttpRequestData request, HttpClient client)
        {
            if (App.Configuration.StressTester.Users == null || 
                App.Configuration.StressTester.Users.Count < 1 || 
                (request.HttpVerb != "POST" && request.HttpVerb != "PUT"))
                return;

            // retrieve a user for the current thread
            var user = App.Configuration.StressTester.Users[request.ThreadNumber % App.Configuration.StressTester.Users.Count];
            if (user == null)
                return;

            // if AuthCookie is provided - just replace the cookie
            if (!string.IsNullOrEmpty(user.AuthCookie))
            {
                client.WebRequest.Headers["Cookie"] = user.AuthCookie;
                return;
            }

            // check login urls for a match
            if (user.LoginUrls == null || user.LoginUrls.Count == 0)
                return;

            LoginFormEntry login = null;
            foreach (var loginUrl in user.LoginUrls)
            {
                string url = loginUrl.Url;
                if (!string.IsNullOrEmpty(Options.ReplaceDomain) || !string.IsNullOrEmpty(Options.SiteBaseUrl))
                    url = FixupUrl(url);

                Debug.WriteLine(url + " " + request.Url + " - " + (url == request.Url));

                if (url.Equals(request.Url,StringComparison.InvariantCultureIgnoreCase))
                {
                    login = loginUrl;
                    break;
                }
            }

            if (login == null)
                return;

            //Debug.WriteLine(login.Url + " " + login.FormVariables.First(l => l.Key.Contains("Username")).Value + " " + login.FormVariables.First(l => l.Key.Contains("Password")).Value);

            foreach (var header in login.Headers)
            {                
                SetHttpHeader(header,client);
            }
            if (login.FormVariables != null)
            {
                client.ResetPostData();
                client.ContentType = login.ContentType;

                if (!string.IsNullOrEmpty(login.RawContent))
                    client.AddPostKey(login.RawContent);
                else
                {
                    foreach (var formVar in login.FormVariables)
                    {
                        if (formVar.BinaryValue != null)
                            client.AddPostKey(formVar.Key, formVar.BinaryValue);
                        else
                            client.AddPostKey(formVar.Key, formVar.Value);
                    }
                }

                // update the request info
                request.RequestContent = client.GetPostBuffer();
                //client.WebRequest.ContentLength = request.RequestContent.Length;
            }

        }

        private void SetHttpHeader(HttpRequestHeader header, HttpClient client)
        {
            var lheader = header.Name.ToLower(); 

            var webRequest = client.WebRequest;

            // Header Overrides that fail if you try to set them
            // directly in HTTP
            if (lheader == "cookie" && !string.IsNullOrWhiteSpace(Options.ReplaceCookieValue))
            {
                string cookie = Options.ReplaceCookieValue;
                webRequest.Headers.Add("Cookie", cookie);
                header.Value = cookie;
                return;
            }
            if (lheader == "authorization" && !string.IsNullOrEmpty(Options.ReplaceAuthorization))
            {
                webRequest.Headers.Add("Authorization", Options.ReplaceAuthorization);
                header.Value = Options.ReplaceAuthorization;                
                return;
            }
            if (lheader == "user-agent")
            {
                client.UserAgent = header.Value;
                return;
            }
            if (lheader == "accept")
            {
                webRequest.Accept = header.Value;
                return;
            }
            if (lheader == "referer")
            {
                webRequest.Referer = header.Value;
                return;
            }
            if (lheader == "connection")
            {
                if (header.Value.ToLower() == "keep-alive")
                    webRequest.KeepAlive = true; // this has no effect
                else if (header.Value.ToLower() == "close")
                    webRequest.KeepAlive = false;
                return;
            }
            // set above view property
            if (lheader == "content-type")
                return;
            // not handled at the moment
            if (lheader == "proxy-connection")
                return;

            // set explicitly via properties
            if (lheader == "transfer-encoding")
            {
                webRequest.TransferEncoding = header.Value;
                return;
            }
            if (lheader == "date")
                return;
            if (lheader == "expect")
            {
                //webRequest.Expect = header.Value;
                return;
            }
            if (lheader == "if-modified-since")
                return;

            webRequest.Headers[header.Name] = header.Value;
            //webRequest.Headers.Add(header.Name, header.Value);
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

        internal string FixupUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            if (!string.IsNullOrEmpty(Options.SiteBaseUrl))
            {
                if (!url.StartsWith("http"))
                   url = Options.SiteBaseUrl.TrimEnd('/') + "/" + url.TrimStart('/');
            }

            // TODO: Obsolete - Remove this eventually
            if (!string.IsNullOrEmpty(Options.ReplaceDomain))
            {
                var host = StringUtils.ExtractString(url, "://", "/", false, true);
                url = url.Replace(host, Options.ReplaceDomain);
            }

            return url;
        }
        #endregion

        #region Session Multi-Threading

        public void RunSessions(object requests)
        {
            var requestStart = requests as RunSessionsThreadStartParameter;
            RunSessions(requestStart.Requests, false, requestStart.Id);
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
        /// <param name="threadNumber">Thread number that identifies this thread. Id is used to map users to threads.</param>
        public void RunSessions(List<HttpRequestData> requests, bool runOnce, int threadNumber = 0)
        {
            List<HttpRequestData> reqs = null;
            
            if (Options.RandomizeRequests)
            {
                var rqs = requests;
                reqs = new List<HttpRequestData>();
                reqs.AddRange( rqs.Where( r2=> r2.SortNoRandmomize).OrderBy(r2=> r2.SortOrder) );

                var r = new Random();
                foreach (var req in rqs.Where(r2=> !r2.SortNoRandmomize) 
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
                CookieContainer cookieContainer = App.Configuration.StressTester.TrackPerSessionCookies
                    ? cookieContainer = new CookieContainer()
                    : null;
                
                foreach (var req in reqs)
                {
                    if (CancelThreads)
                        break;

                    //Debug.WriteLine("Thread: " + Thread.CurrentThread.ManagedThreadId + " - " + req.Url + " Cookies: " + (cookieContainer?.Count ?? -1) );
                                            
                    var result = CheckSite(req, cookieContainer, threadNumber);
                    
                    if (result != null)
                        RequestWriter.Write(result);

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

        #endregion

        #region Result Processing
        /// <summary>
        /// Writes the actual result to the storage container
        /// </summary>
        /// <param name="result"></param>
        public virtual void WriteResult(HttpRequestData result)
        {
            RequestWriter.Write(result);
        }

        /// <summary>
        /// Creates a string
        /// </summary>
        /// <param name="results">Results to process. If not passed uses internal Results</param>
        /// <param name="totalTime">Total request time in seconds</param>
        /// <returns></returns>
        public string ParseResults(RequestWriter writer, int totalTime = 0)
        {
            if (writer == null)
                writer = RequestWriter;
            if (totalTime == 0)
                totalTime = TimeTakenForLastRunMs / 1000;

            return ResultsParser.ParseResultsToString(writer, totalTime, ThreadsUsed);
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
            var requestDataList = parser.ParseFile(fileName, ref options);
            if (options != null)
                Options = options;            

            if (requestDataList == null)
            {
                SetError(parser.ErrorMessage);
                return null;
            }

            StressTester.InteractiveSessionCookieContainer = App.Configuration.StressTester.TrackPerSessionCookies ? 
                new CookieContainer() : 
                null;

            return requestDataList;
        }
#endregion


        #region Event Handling
        /// <summary>
        /// Event fired after each request that provides the
        /// current request.
        /// </summary>
        public event Action<HttpRequestData> RequestProcessed;

        public void OnRequestProcessed(HttpRequestData request)
        {
            foreach (var addin in App.Addins)
            {
                try
                {
                    addin.OnAfterRequestSent(request);
                }
                catch (Exception ex)
                {
                    App.Log(addin.GetType().Name + " failed in OnBeforeRequestSent(): " + ex.Message);
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
            Progress?.Invoke(progressInfo);
        }

        #endregion

        #region Error Messages
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
        #endregion
    }

    public class RunSessionsThreadStartParameter
    {
        public List<HttpRequestData> Requests { get; set; }
        public int Id { get; set; }
    }

    public class ProgressInfo
    {
        public int SecondsProcessed { get; set; }
        public int TotalSecondsToProcess { get; set; }
        public int RequestsProcessed { get; set; }
        public int RequestsFailed { get; set; }
        public bool IsWarmingUp { get; set; }
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
