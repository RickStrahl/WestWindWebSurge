using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WebSurge
{
    public class StressTesterConfiguration
    {

        #region Domain Replacement
        /// <summary>
        /// Optional Website Base URL that allows you to use *site relative URLs* rather than
        /// fully qualified URLs for each request. Makes it easier to switch to different sites
        /// for testing by changing the SiteBaseUrl in one place for all tests.
        /// Works best if all URLs are to a single site.
        ///
        /// URL Examples:  
        /// https://mysite.com  
        /// http://localhost:5000    
        /// http://localhost/virtual  
        /// </summary>
        [Description(@"Optional Website Base URL that allows you to use *site relative URLs* rather than fully qualified URLs for each request. Makes it easier to switch to different sites for testing by changing the SiteBaseUrl in one place for all tests. Works best if all URLs are to a single site.

URL Examples:
https://mysite.com,
http://localhost:5000,
http://localhost/virtual
")]
        [Category("Domain Replacement")]
        public string SiteBaseUrl
        {
            get => _siteBaseUrl;
            set
            {
                _siteBaseUrl = value;
                if (SiteBaseUrl != null && !_siteBaseUrl.EndsWith("/"))
                    _siteBaseUrl += "/";
            }
        }
        private string _siteBaseUrl;


        /// <summary>
        /// Allows you to replace the domain and port number of the
        /// Http request with the one specified here. Allows you to 
        /// easily switch between multiple machines like dev, staging and live.
        /// </summary>
        [Obsolete]
        [Description(
            @"Obsolete - recommend you use `SiteBaseUrl` and relative URLs instead for easier site switching.

Optional domain replacement for fully qualified `http://` and `https://` URLs. Parses the hostname of the original URL and replaces it with a replacement domain you specify.

Example domains replacements: 
MyDomain.com
localhost:5001
localhost/virtual
")]
        [Category("Domain Replacement")]
        public string ReplaceDomain { get; set; }

#endregion

        /// <summary>
        /// If true no progress information events are fired
        /// </summary>
        [Description("If true no progress events are fired. Speeds up operation on high volume test that " +
                     "exceed a 500 requests per second, but provides no UI progress.")]
        [Category("Performance")]
        public bool NoProgressEvents { get; set; }

        /// <summary>
        /// Optional delay time between requests. 0 means
        /// no delay.
        /// </summary>
        [Description("Delay time added after each request to simulate user 'wait times' before going on. 0 doesn't wait but yields the active thread, -1 doesn't yield. Use -1 to get optimum performance at the cost of very high CPU load - best with 1 thread per Core in your machine.")]
        [Category("Performance")]
        public int DelayTimeMs { get; set; }

        [Description("When true gzip and deflate content is not decompressed, which can improve performance on high volume requests")]
        [Category("Performance")]
        public bool NoContentDecompression { get; set; }


        [Description("Use this option if you plan on capturing large numbers of requests - " +
                     "high transaction counts or long running tests. This option will capture full responses " + 
                     "for the first few thousand records and then capture only the basic request " + 
                     "information necesary to calculate results and discard headers and response body " +
                     "for sucess requests. Failures continue to capture in full size.")]
        [Category("Minimize Memory")]
        public bool CaptureMinimalResponseData { get; set; }

        /// <summary>
        /// Max size of the response that's retained.
        /// Default captures full response (0).
        /// </summary>
        [Description(
            "The maximum size of the response to capture. Use this to limit the byte size " +
            "of response captures to limit memory usage while capturing for large test runs. " + 
            "Set to 0 to capture the full response which is the default.")]
        [Category("Minimize Memory")]
        public int MaxResponseSize { get; set; }


        [Description(@"Replaces query string key value pairs on the URL when set. Use query string syntax for values to add or replace. Example: id=333123&format=json  - adds or replaces id and json query string values.")]
        [Category("Header Replacement")]
        public string ReplaceQueryStringValuePairs { get; set; }


        

        #region Header and URL Replacement


        /// <summary>
        /// A cookie value that is replaced instead of the 'real'
        /// cookie header sent in the request.
        /// 
        /// Use this to simulate user authentication cookies if
        /// necessary.
        /// </summary>
        [Description(
            @"A cookie value that is replaced instead of the captured cookie of the captured trace.

Use to force custom auth cookies to an existing session that has expired cookies."
        )]
        [Category("Header Replacement")]
        public string ReplaceCookieValue { get; set; }

        [Description(
            @"When set replaces or adds the Authentication header with this value.

Allows to add custom authentication to a request after you've captured say a bearer token."
        )]
        [Category("Header Replacement")]
        public string ReplaceAuthorization { get; set;  }

        /// <summary>
        /// Username to use for NTLM or Basic Authentication
        /// </summary>

        [Category("Authentication")]
        [Description("Global Username to use for NTLM or Basic Authentication.\r\nYou can also use 'AutoLogin' to use your current Windows Credentials for NTLM authentication and leave the password blank")]
        public string Username { get; set;  }

        /// <summary>
        /// Username to use for NTLM or Basic Authentication
        /// </summary>
        [Category("Authentication")]
        [PasswordPropertyText(true)]
        [Description("Global Password to use for NTLM or Basic Authentication. Important: This value is saved in the request configuration in encrypted format."
        )]
        public string Password { get; set; }


        [Browsable(false)]
        [Description("Optional specific users assigned to this test.")]
        [Category("Authentication")]
        public List<UserEntry> Users { get; set; }
        #endregion

        #region Test Operation

        /// <summary>
        /// The request timeout in milliseconds
        /// </summary>
        [Description("Max time a request can take before it's considered timed out. Value is in milliseconds.")]
        [Category("Test Operation")]
        public int RequestTimeoutMs
        {
            get => _requestTimeoutMs;
            set => _requestTimeoutMs = value;
        }
        private int _requestTimeoutMs;

        /// <summary>
        /// Determines whether requests are run in random
        /// order on each thread, or whether all threads
        /// run requests sequentially in the same order
        /// </summary>
        [Description(
            "Determines whether the captured session is played back in random order. " + 
            "If false session is played back in the order captured."
            )]
        [Category("Test Operation")]
        public bool RandomizeRequests { get; set; }


        /// <summary>
        /// Seconds to run requests before logging actual requests. Use to warm up the Web server.
        /// </summary>
        [Description("Seconds to run requests before logging actual requests. Use to warm up the Web server.")]
        [Category("Test Operation")]
        public int WarmupSeconds { get; set; }

        /// <summary>
        /// Determines if certificate errors are ignored. Must restart application for this change to take effect.
        /// </summary>
        [Description("Determines if certificate errors are ignored. Must restart application for this change to take effect.")]
        [Category("Test Operation")]
        public bool IgnoreCertificateErrors { get; set; }

        

        [Description(
            "Determines if cookies are tracked for requests in a single URL session. Initially cookies are empty but if you login cookies are then assigned and tracked which allows tracking an individual user for that session. Tip: Use in combination with Users for login Urls to simulate multiple **different** users.")]
        [Category("Test Operation")]
        public bool TrackPerSessionCookies
        {
            get { return _trackPerSessionCookies; }
            set
            {
                if (value == false)
                    StressTester.InteractiveSessionCookieContainer = null;

                _trackPerSessionCookies = value;
            }
        }
        private bool _trackPerSessionCookies;

        #endregion


        [Description("Determines whether templates are reloaded " +
                     "on each request or whether they are cached. The latter is more " + 
                     "efficient but won't reload templates if they're changed unless you restart. " + 
                     "The former is useful if you want to customize the templates " + 
                     "without restarting WebSurge.")]
        [Category("User Interface")]
        public bool ReloadTemplates { get; set; }

        [Category("User Interface")]
        [Description("Ace Editor theme used for viewing syntax highlighted content. Values come from the Ace Editor themes in the AppData/West Wind Web Surge/Html folder. Some themes avaialable: vscodelight,vscodedark,visualstudio,twilight,monokai,github,ambiance")]
        public string FormattedPreviewTheme { get; set; }

        [Browsable(false)]
        public int LastThreads { get; set; }


    


        [Browsable(false)]
        public int LastSecondsToRun { get; set; }

        


        public StressTesterConfiguration()
        {
            RequestTimeoutMs = 15000;
            MaxResponseSize = 0;
            DelayTimeMs = 0;

            MaxResponseSize = 0;            
            WarmupSeconds = 2;

            LastSecondsToRun = 10;
            LastThreads = 2;            

            IgnoreCertificateErrors = false;
            TrackPerSessionCookies = true;
            FormattedPreviewTheme = "vscodelight";

            Users = new List<UserEntry>();
        }
        
    }
}
