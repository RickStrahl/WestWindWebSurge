using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebSurge
{
    /// <summary>
    /// Contains an individual user for a test. Each user
    /// can specify a dedicated AuthCookie (sets Cookies)
    /// or provide one or more LoginUrls with form variables
    /// and headers.
    /// </summary>    
    public class UserEntry
    {
        [Description("Any URLS that you need to send user specific data to.")]
        /// <summary>
        /// Any URLS that you need to send user specific data to
        /// </summary>
        public List<LoginFormEntry> LoginUrls;

        /// <summary>
        /// Complete Authorization Cookie that's associated with this user
        /// </summary>
        [Description("Optional - Complete Authorization Cookie that's associated with this user.")]
        public string AuthCookie { get; set; }

        public UserEntry()
        {
            LoginUrls = new List<LoginFormEntry>();
        }        
    }

    [DebuggerDisplay("{Url}")]
    public class LoginFormEntry
    {
        /// <summary>
        /// This is the URL that Headers and Formvariables are applied to
        /// </summary>
        public string Url { get; set; }

        public List<HttpRequestHeader> Headers { get; set; }

        public List<HttpFormVariable> FormVariables { get; set; }
        public string ContentType { get; set; }

        /// <summary>
        /// Optional Raw Content set to post to the server for login authentication
        /// </summary>
        public string RawContent { get; set; }

        /// <summary>
        /// HTTP Verb to apply to the content
        /// </summary>
        public string HttpVerb { get; set; }

        public LoginFormEntry()
        {
            Headers = new List<HttpRequestHeader>();
            FormVariables = new List<HttpFormVariable>();
            ContentType = "application/x-www-form-urlencoded";
            HttpVerb = "POST";
        }
    }

    [DebuggerDisplay("{Key} - {Value}")]
    public class HttpFormVariable
    {
        public string Key { get; set; }

        public string Value { get; set;  }

        public byte[] BinaryValue { get; set; }

        public HttpFormVariable()
        {            
        }

        public HttpFormVariable(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public HttpFormVariable(string key, byte[] binaryValue)
        {
            Key = key;
            BinaryValue = binaryValue;
        }
    }
}
