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
        public string LastResponse { get; set; }
        
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public int ResponseLength { get; set; }

        public int TimeTakenMs { get; set; }
        

        public HttpRequestData()
        {
            Timestamp = DateTime.UtcNow;
            Headers = new List<HttpRequestHeader>();
            IsError = true;
            ErrorMessage = string.Empty;
        }
        public static HttpRequestData Copy(HttpRequestData req)
        {
            return req.MemberwiseClone() as HttpRequestData;
            
            //var reqData = new HttpRequestData();            
            //DataUtils.CopyObjectData(req, reqData);
            //return reqData;
        }

        public string ToString()
        {
            return HttpVerb + " " + Url;
        }

        public string GetHeader(string headerName)
        {
            headerName = headerName.ToLower();
            return Headers
                .Where(hd => hd.Name.ToLower() == headerName)
                .Select(hd => hd.Value).LastOrDefault();
        }


         /// <summary>
        /// Parses a single HttpRequestData object to HTML
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string ToHttpHeader()
        {
            var req = this;

            StringBuilder sb = new StringBuilder();
            
            var html = "{0} {1} HTTP/1.1\r\n";
            sb.AppendFormat(html, req.HttpVerb, req.Url);

            if (!string.IsNullOrEmpty(req.ContentType))
                sb.AppendLine("Content-type: " + req.ContentType);

            foreach (var header in req.Headers)
            {
                sb.AppendLine(header.Name + ": " + header.Value);
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
            var req = this;

            StringBuilder sb = new StringBuilder();
            string html = "";

            if (!string.IsNullOrEmpty(req.ErrorMessage))
            {
                sb.AppendLine(@"<label>Error Message</label>");
                sb.AppendLine(req.ErrorMessage);
            }

            html = @"
<h3>Request Headers</h3>
<pre>
<b><span style='color: darkred;'>{0}</span> <a href='{1}' target='top'>{1}</a></b> HTTP/1.1
";
            sb.AppendFormat(html, req.HttpVerb, req.Url);

            if (!string.IsNullOrEmpty(req.ContentType))
                sb.AppendLine("Content-type: " + req.ContentType);

            foreach (var header in req.Headers)
            {
                sb.AppendLine(header.Name + ": " + header.Value);
            }
            sb.AppendLine();

            if (!string.IsNullOrEmpty(req.RequestContent))
                sb.AppendLine(req.RequestContent);

            sb.AppendLine("</pre>");

            if (req.TimeTakenMs > 0)
                sb.AppendFormat("<div class='timetaken'>{0}ms</div>", req.TimeTakenMs.ToString("n0"));

            if (!string.IsNullOrEmpty(req.StatusCode))
            {
                html = @"<h3>Http Response</h3>
<pre>";

                sb.AppendLine(html);

                if (!string.IsNullOrEmpty(req.StatusCode))
                    sb.AppendLine("HTTP/1.1 " + req.StatusCode + " " + req.StatusDescription);

                sb.AppendLine(req.ResponseHeaders);
                sb.AppendLine();

                if (req.LastResponse != null)
                    sb.AppendLine(HtmlUtils.HtmlEncode(req.LastResponse.Trim()));
            }

            if (!asDocument)
                return sb.ToString();

            html = @"<!DOCTYPE HTML>
<html>
<head>
  <style>  
  html,body { 
    font-family: arial;
    font-size: 11pt;
    margin: 0;
    padding: 0;
    line-spacing: 1.55;
  }   
  body{
     padding: 5px;
  } 
  label {
    font-weight: bold;
    display: block;
    margin-top: 10px;
  }
  pre {
    background: #eeeeee;
    border: 1px solid silver;
    border-radius: 4px;
    font-family: Consolas,monospace;
    font-weight: normal;
    font-size: 12px;
    padding: 5px;
    margin: 5px 8px;
    overflow-x: hidden;
    border-radius: 2px;
  }
  .timetaken
  {
    float: right; 
    color: steelblue;
    font-size: smaller;
    padding-right: 10px;
  }
  h3 {
    color: steelblue;
    margin-top: 12px;
    margin-bottom: 3px;
  }
  </style>
</head>
<body>
";
            return html + sb + "</body></html>";
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