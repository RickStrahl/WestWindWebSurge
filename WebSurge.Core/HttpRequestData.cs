using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Newtonsoft.Json.Linq;
using Westwind.Utilities;
using Formatting = Newtonsoft.Json.Formatting;


namespace WebSurge
{

    /// <summary>
    /// This is the data point stored for each Web Surge request
    /// that captures all the required request information.
    /// </summary>
    public class HttpRequestData
    {
        /// <summary>
        /// Unique Id that identifies every request generated. Each
        /// request run has a unique id        
        /// </summary>
        public long Id { get; set; }


        ///// <summary>
        ///// Uniquely identifies the request URL.
        ///// </summary>
        //public long UrlId { get; set; }

        public DateTime Timestamp { get; set; }

        public int ThreadNumber { get; set; }

        public bool IsActive { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }
        public string Host { get; set; }
        public string HttpVerb { get; set; }
        public string ContentType { get; set; }
        
        public string FullRequest { get; set; }
        public string RequestContent { get; set; }
        
        public string Username { get; set; }
        public string Password { get; set; }
        public AuthenticationTypes AuthenticationType { get; set; }

        public List<HttpRequestHeader> Headers { get; set; }

        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }

        public string ResponseHeaders { get; set; }
        public string ResponseContent { get; set; }        
        
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public int ResponseLength { get; set; }

        public int TimeTakenMs { get; set; }
        public int TimeToFirstByteMs { get; set; }        

        public bool IsWarmupRequest { get; set; }
        public int SortOrder;

        private static Encoding WindowsEncoding = Encoding.GetEncoding(1252);
        public string TextEncoding { get; set; }

        public CookieContainer Cookies { get; set; }
         

        public HttpRequestData()
        {
            Id = DataUtils.GenerateUniqueNumericId();            
            IsActive = true;
            HttpVerb = "GET";
            Timestamp = DateTime.UtcNow;
            Headers = new List<HttpRequestHeader>();
            IsError = true;
            ErrorMessage = string.Empty;
            TextEncoding = "UTF-8";
        }
        public static HttpRequestData Copy(HttpRequestData req)
        {
            var rnew = req.MemberwiseClone() as HttpRequestData;
            rnew.Url = req.Url;
            rnew.HttpVerb = req.HttpVerb;
            rnew.Host = req.Host;
            rnew.Headers = new List<HttpRequestHeader>(req.Headers);
            rnew.ContentType = req.ContentType;
            rnew.RequestContent = req.RequestContent;
            rnew.Username = req.Username;
            rnew.Password = req.Password;

            rnew.Id = DataUtils.GenerateUniqueNumericId();
            rnew.Name = req.Name;
            rnew.Timestamp = DateTime.UtcNow;

            return rnew;     
        }

        public override string ToString()
        {
            return HttpVerb + " " + Url;
        }

        /// <summary>
        /// Returns a Request HTTP header.
        /// </summary>
        /// <param name="headerName">Request HTTP header to return. Not case sensitive.</param>
        /// <returns>header value or null</returns>
        public string GetHeader(string headerName)
        {
            headerName = headerName.ToLower();
            return Headers
                .Where(hd => hd.Name.ToLower() == headerName)
                .Select(hd => hd.Value)
                .LastOrDefault();
        }

        /// <summary>
        /// Retrieves the specified response HTTP header.
        /// Null if not found or the request hasn't been processed yet.
        /// </summary>
        /// <param name="headerName">Name of the HTTP header to retrieve. Not case sensitive</param>
        /// <returns>header value or null</returns>
        public string GetResponseHeader(string headerName)
        {
            if (ResponseHeaders == null)
                return null;

            string headerLine = StringUtils.ExtractString(ResponseHeaders, headerName + ": ", "\r\n", false, true, false);
            if (headerLine == null)
                return null;

            return headerLine;
        }

        /// <summary>
        /// Returns the output type of the response based on the content type.
        /// Supported types:
        /// Html, Xml, Json
        /// </summary>
        /// <returns></returns>
        public string GetTypeOfContent(string ct = null)
        {
            if (ct == null)
                ct = GetResponseHeader("Content-type");

            if (ct == null)
                return null;
            if (ct.Contains("text/html"))
                return "html";
            if (ct.Contains("text/xml") || ct.Contains("application/xml") || ct.Contains("application/soap+xml"))
                return "xml";
            if (ct.Contains("application/json"))
                return "json";
            if (ct.Contains("text/css"))
                return "css";
            if (ct.Contains("application/javascript") || ct.Contains("application/x-javascript"))
                return "javascript";
            if (ct.Contains("application/x-www-form-urlencoded"))
                return "urlencoded";
          
            return null;
        }
        

        /// <summary>
        /// Returns various result (or request) content to formatted
        /// content.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="outputType"></param>
        /// <returns></returns>
        public string GetFormattedContent(string data, string outputType)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            if (outputType == "json")
            {
                try
                {
                    return JValue.Parse(data).ToString(Formatting.Indented);
                }
                catch
                {
                    return "Invalid or partial JSON data cannot be formatted (try setting MaxResponseSize option to 0).\r\n" + data;                    
                }
            }
            if (outputType == "xml")
            {
                try
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(data);

                    using (var sw = new StringWriter())
                    {
                        using (var writer = new XmlTextWriter(sw))
                        {
                            writer.Formatting = System.Xml.Formatting.Indented;
                            doc.WriteTo(writer);
                        }

                        return sw.ToString();
                    }
                }
                catch 
                {
                    return "Invalid or partial XML data cannot be formatted (try setting  MaxResponseSize option to 0).\r\n" + data;
                }
            }
            if (outputType == "urlencoded")
            {
                return StringUtils.UrlDecode(data.Replace("&", "\r\n"));
            }

            return data;
        }

        /// <summary>
        /// Reliably returns the request content as a string.
        /// If the content is binary the result is returned
        /// binary markup characters are shown.
        /// </summary>
        /// <returns></returns>
        public string GetRequestContentAsString()
        {
            if (RequestContent == null)
                return null;

            if (RequestContent.StartsWith("b64_"))
            {
                var enc = Encoding.GetEncoding(1252);
                var data = Convert.FromBase64String(RequestContent.Replace("b64_",""));
                return enc.GetString(data);
            }
            return RequestContent;
        }

        

        /// <summary>
        /// Returns the content in the  encoding specified in the request
        /// or by the parameter passed.
        /// 
        /// Assumes that the content is a Unicode string - converts the
        /// content 
        /// </summary>
        /// <returns></returns>
        public string GetRequestContentAsEncodedString()
        {
            if (RequestContent == null || RequestContent.StartsWith("b64_"))
                return RequestContent;

            var bytes = GetRequestContentBytes();
            if (bytes == null)
                return null;

            return WindowsEncoding.GetString(bytes);            
        }

        /// <summary>
        /// Returns the Request content as raw, properly encoded bytes 
        /// based on the TextEncoding setting.
        ///                 
        /// Translates Unicode strings (except for binary data)
        /// to underlying encoding and Binary data (b64_ prefix) 
        /// is returned as raw data.
        /// </summary>
        /// <returns>byte[] data or null if no RequestContent</returns>
        public byte[] GetRequestContentBytes()
        {
            if (RequestContent == null)
                return null;

            if(RequestContent.StartsWith("b64_"))
                return Convert.FromBase64String(RequestContent.Replace("b64_",""));
            
            string textEncoding = TextEncoding.ToLower();
            if (textEncoding == "utf-8")
               return Encoding.UTF8.GetBytes(RequestContent);
            
            try
            {
                // try to load the encoding specified
                var enc = Encoding.GetEncoding(TextEncoding);
                return enc.GetBytes(RequestContent);
            }
            catch
            {}
        
            // fallback to 'as-is/no' encoding
            return WindowsEncoding.GetBytes(RequestContent);                        
        }

        /// <summary>
        /// Reliably returns a string from response content. Note string 
        /// may be truncated if binary data is in the result and it contains
        /// nulls.
        /// </summary>
        /// <returns></returns>
        public string GetResponseContentAsString()
        {
            if (ResponseContent == null)
                return null;
            
            if (ResponseContent.StartsWith("b64_"))
            {
                var enc = Encoding.GetEncoding(1252);
                var data = Convert.FromBase64String(ResponseContent.Replace("b64_",""));
                return enc.GetString(data);
            }
            return ResponseContent;
        }

        /// <summary>
        /// Parses Request HTTP headers from a string into the 
        /// Headers property of this class
        /// </summary>
        /// <param name="headerText">Full set of Http Request Headers</param>
        public void ParseHttpHeaders(string headerText)
        {
            if (string.IsNullOrEmpty(headerText))
            {
                Headers.Clear();
                return;
            }
            ParseHttpHeaders(StringUtils.GetLines(headerText));
        }

        /// <summary>
        /// Parses Request Http Headers from an array of strings
        /// into the Headers property of this class.
        /// </summary>
        /// <param name="lines">Array of Http Headers to parse in header: value format</param>
        public void ParseHttpHeaders(string[] lines)
        {
            Headers.Clear();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrEmpty(line))
                    continue;

                int idx = line.IndexOf(':');
                if (idx < 0 || line.Length < idx + 2)
                    continue;

                string header = line.Substring(0, idx);
                string value = line.Substring(idx + 1).Trim();

                var hd = new HttpRequestHeader
                {
                    Name = header,
                    Value = value
                };
                var name = hd.Name.ToLower();

                // ignore host header - host is part of url
                if (name == "host")
                {
                    Host = hd.Value;
                    continue;
                }
                if (name == "content-type")
                {
                    ContentType = hd.Value;

                    if (!string.IsNullOrEmpty(hd.Value))
                    {
                        TextEncoding = StringUtils.ExtractString(hd.Value, "charset=", ";", false, true);
                        
                        // Web Content defaults to UTF-8 
                        if (string.IsNullOrEmpty(TextEncoding))
                            TextEncoding = "UTF-8";
                    }
                }
                if (name == "content-length")
                    continue; // HTTP client adds this automatically
                if (name == "websurge-request-inactive")
                {
                    IsActive = false;
                    continue; // don't add header
                }
                if (name == "websurge-request-name")
                {
                    Name = hd.Value;
                    continue;
                }

                Headers.Add(hd);
            }
        }

        /// <summary>
        /// Parses a single HttpRequestData object to a string.
        /// Creates Request headers and content only - no response data.
        /// </summary>
        /// <param name="req">the request instance to turn into a string</param>
        /// <param name="noWebSurgeHeaders">Determines whether WebSurge specific headers are returned</param>
        /// <returns></returns>
        public string ToRequestHttpHeader(bool noWebSurgeHeaders = false)
        {
            var req = this;

            StringBuilder sb = new StringBuilder();

            var html = "{0} {1} HTTP/1.1\r\n";
            sb.AppendFormat(html, req.HttpVerb, req.Url);

            foreach (var header in req.Headers)
            {                
                sb.AppendLine(header.Name + ": " + header.Value);
            }

            if (!noWebSurgeHeaders)
            {
                if (!IsActive)
                    sb.AppendLine("Websurge-Request-Inactive: 1");
                if (!string.IsNullOrEmpty(Name))
                    sb.AppendLine("Websurge-Request-Name: " + Name);
            }

            if (!string.IsNullOrEmpty(req.RequestContent))
                sb.AppendLine("\r\n" + req.RequestContent);

            return sb.ToString();
        }

        public string ToResponseHttpHeader()
        {
            var req = this;

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("HTTP/1.1 {0} {1}\r\n", req.StatusCode, req.StatusDescription);
            sb.AppendLine(req.ResponseHeaders);

            if (!string.IsNullOrEmpty(req.ResponseContent))
            {                
                sb.AppendLine(req.ResponseContent);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Parses a single HttpRequestData object to HTML
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string ToHtml(bool asDocument = true)
        {
            
            //return TemplateRenderer.RenderTemplate("Request.cshtml", this);

            HttpRequestData req = this;

            StringBuilder sb = new StringBuilder();
            string html = "";

            if (!string.IsNullOrEmpty(req.ErrorMessage))
            {
                sb.AppendLine("<div class='error-display'>");
                sb.AppendLine(@"<div class='error-header'>Error Message</div>");
                sb.AppendLine(@"<div>" + req.ErrorMessage + "</div>");
                sb.AppendLine("</div>");
            }

            html = @"
<h3>Request Headers</h3>
<pre>
<b><span style='color: darkred;'>{0}</span> <a href='{1}'>{1}</a></b> HTTP/1.1
";
            sb.AppendFormat(html, req.HttpVerb, req.Url);

            if (!string.IsNullOrEmpty(req.ContentType))
                sb.AppendLine("Content-type: " + req.ContentType);

            if (req.Headers != null)
            {
                foreach (var header in req.Headers)
                {
                    sb.AppendLine(header.Name + ": " + header.Value);
                }
            }

            if (!string.IsNullOrEmpty(req.RequestContent))
            {
                sb.AppendLine();
                sb.AppendLine(HtmlUtils.HtmlEncode(req.RequestContent.Trim()));
            }

            sb.AppendLine("</pre>");

            if (req.TimeTakenMs > 0)
                sb.AppendFormat("<div class='timetaken'>{0}ms</div>", req.TimeTakenMs.ToString("n0"));

            if (!string.IsNullOrEmpty(req.StatusCode))
            {
                html = @"<h3>Http Response</h3>
<pre>";

                sb.AppendLine(html);


                string cssClass = req.StatusCode.CompareTo("399") > 0 ? "error-response" : "success-response";


                if (!string.IsNullOrEmpty(req.StatusCode))
                    sb.AppendFormat("<div class='{0}'>HTTP/1.1 {1} {2}</div>", cssClass, req.StatusCode, req.StatusDescription);

                sb.AppendLine(req.ResponseHeaders);

                if (req.ResponseContent != null)
                    sb.Append(HtmlUtils.HtmlEncode(req.ResponseContent.Trim()));
            }

            if (!asDocument)
                return sb.ToString();

            html = @"<!DOCTYPE HTML>
<html>
<head>
    <link href='css/WebSurge.css' type='text/css' rel='stylesheet' />
</head>
<body>
";
            return html + sb + "</body>\r\n</html>";
        }        

    }

    [DebuggerDisplay("{Name} - {Value}")]
    public class HttpRequestHeader
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public enum AuthenticationTypes
    {
        Windows,
        Basic,
        Digest
    }
}