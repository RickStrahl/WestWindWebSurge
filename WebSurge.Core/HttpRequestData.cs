using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Westwind.Utilities;


namespace WebSurge
{

    public class HttpRequestData
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }

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
        public bool IsWarmupRequest { get; set; }

        public HttpRequestData()
        {
            Id = DataUtils.GenerateUniqueNumericId();
            HttpVerb = "GET";
            Timestamp = DateTime.UtcNow;
            Headers = new List<HttpRequestHeader>();
            IsError = true;
            ErrorMessage = string.Empty;
        }
        public static HttpRequestData Copy(HttpRequestData req)
        {
            var rnew = req.MemberwiseClone() as HttpRequestData;
            rnew.Timestamp = DateTime.UtcNow;
            return rnew;     
        }

        public string ToString()
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

            string headerLine = StringUtils.ExtractString(ResponseHeaders, headerName + ": ", "\r\n", true, true, false);
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
        public string GetOutputType()
        {
            string ct = GetResponseHeader("Content-Type");
            if (ct == null)
                return null;
            if (ct.Contains("text/html"))
                return "html";
            if (ct.Contains("text/xml") || ct.Contains("application/xml"))
                return "xml";
            if (ct.Contains("application/json"))
                return "json";

            return null;
        }


        /// <summary>
        /// Reliably returns the request content as a string.
        /// If the content is binary the result is returned
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
                }
                if (name == "content-length")
                    continue; // HTTP client adds this automatically

                Headers.Add(hd);
            }
        }

        /// <summary>
        /// Parses a single HttpRequestData object to a string.
        /// Creates Request headers and content only - no response data.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string ToRequestHttpHeader()
        {
            var req = this;

            StringBuilder sb = new StringBuilder();

            var html = "{0} {1} HTTP/1.1\r\n";
            sb.AppendFormat(html, req.HttpVerb, req.Url);

            foreach (var header in req.Headers)
            {                
                sb.AppendLine(header.Name + ": " + header.Value);
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