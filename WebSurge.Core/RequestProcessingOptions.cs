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

    /// <summary>
    /// Allows you to replace the domain and port number of the
    /// Http request with the one specified here. Allows you to 
    /// easily switch between multiple machines like dev, staging and live.
    /// </summary>
    [Description("Allows you to replace the domain and port number and optional base path of URL to handle running in different environments without changing the original captured URL. Strips url from :// to first / (or end of string) and replaces with your string. Lets you switch between multiple machines like dev, staging and live.")]
    public string ReplaceDomain { get; set; }

    public RequestProcessingOptions()
    {
        RequestTimeoutMs = 20000;
        MaxResponseSize = 2000;
        DelayTimeMs = 0;
        
    }
}