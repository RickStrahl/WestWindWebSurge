using System.Collections.Generic;
using System.Linq;
using System.Net;
using Westwind.Utilities;


namespace WebSurge
{

    public class HttpRequestData
    {
        public string FullRequest { get; set; }

        public string Url { get; set; }
        public string Host { get; set; }
        public string HttpVerb { get; set; }
        public string ContentType { get; set; }
        
        public string RequestContent { get; set; }


        public string Username { get; set; }
        public string Password { get; set; }

        public AuthenticationTypes AuthenticationType { get; set; }

        public List<HttpRequestHeader> Headers { get; set; }


        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }

        public string LastResponse { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public int ResponseLength { get; set; }

        public int TimeTakenMs { get; set; }
        public string ResponseHeaders { get; set; }


        public HttpRequestData()
        {
            Headers = new List<HttpRequestHeader>();
            IsError = true;
            ErrorMessage = string.Empty;
        }
        public static HttpRequestData Copy(HttpRequestData req)
        {
            var reqData = new HttpRequestData();
            DataUtils.CopyObjectData(req, reqData);
            return reqData;
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