using System.ComponentModel;

public class RequestProcessingOptions
{
    
    /// <summary>
    /// A cookie value that is replaced instead of the 'real'
    /// cookie header sent in the request.
    /// 
    /// Use this to simulate user authentication cookies if
    /// necessary.
    /// </summary>
    [Description("A cookie value that is replaced instead of the captured cookie of the captured trace.\r\n\r\nUse to force custom auth cookies to an existing session that has expired cookies.")]
    public string ReplaceCookieValue { get; set;  }


    /// <summary>
    /// Allows you to replace the domain and port number of the
    /// Http request with the one specified here. Allows you to 
    /// easily switch between multiple machines like dev, staging and live.
    /// </summary>
    [Description("Allows you to replace the domain and port number and optional base path of URL to handle running in different environments without changing the original captured URL.\r\n" +
        "For example, say you captured original urls from 'YourLiveDomain.com', but now you want to test on 'YourTestDomain.com' - you can set  this property to 'YourTestDomain.com' and all testing will replace that domain. You can also inject a virtual path so a valid replacement might be 'localhost/myapp' for local testing under a virtual directory.")]
    public string ReplaceDomain { get; set; }

    /// <summary>
    /// Determines whether requests are run in random
    /// order on each thread, or whether all threads
    /// run requests sequentially in the same order
    /// </summary>
    [Description("Determines whether the captured session is played back in random order. If false session is played back in the order captured.")]
    public bool RandomizeRequests { get; set; }

    /// <summary>
    /// Max size of the response that's retained.
    /// Defaults to 2,000 bytes.
    /// </summary>
    [Description("The maximum size of the response to capture.")]
    public int MaxResponseSize { get; set; }
    
    /// <summary>
    /// The request timeout in milliseconds
    /// </summary>
    [Description("Max time a request can take before it's considered failed.")]
    public int RequestTimeoutMs { get; set;  }

    /// <summary>
    /// If true no progress information events are fired
    /// </summary>
    [Description("If true no progress events are fired. Slightly speeds up operation but provides no UI progress.")]
    public bool NoProgressEvents { get; set; }

    /// <summary>
    /// Optional delay time between requests. 0 means
    /// no delay.
    /// </summary>
    [Description("Delay time added after each request to simulate user 'wait times' before going on.")]
    public int DelayTimeMs { get; set;  }
    
    [Description("Use this option if you plan on capturing large numbers of requests - high transaction count or long running requests. This option will capture only the basic request information necesary to calculate results and toss out headers and response body.")]
    public bool CaptureMinimalResponseData { get; set; }

    /// <summary>
    /// Seconds to run requests before logging actual requests. Use to warm up the Web server.
    /// </summary>
    [Description("Seconds to run requests before logging actual requests. Use to warm up the Web server.")]
    public int WarmupSeconds { get; set;  }


    public RequestProcessingOptions()
    {
        RequestTimeoutMs = 20000;
        MaxResponseSize = 2000;
        DelayTimeMs = 0;        
    }
}