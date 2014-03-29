using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Westwind.Utilities;

namespace WebSurge
{
    public class FiddlerSessionParser
    {
        public const string STR_Separator = "\r\n------------------------------------------------------------------\r\n";


        public List<HttpRequestData> Parse(string fiddlerSessionFile = null)
        {
            if (fiddlerSessionFile == null)
                fiddlerSessionFile = Path.GetFullPath("1_Full.txt");

            if (!File.Exists(fiddlerSessionFile))
            {
                SetError("File doesn't exist.");
                return null;
            }

            var httpRequests = new List<HttpRequestData>();

            string file = File.ReadAllText(fiddlerSessionFile);

            string[] requests = file.Split(new string[1] {STR_Separator},StringSplitOptions.RemoveEmptyEntries);
            foreach (string request in requests)
            {
                string req = request.Trim();
                if (!string.IsNullOrEmpty(req))
                {
                    var httpRequest = ParseRequest(req);
                    if (httpRequest != null)
                        httpRequests.Add(httpRequest);
                }
            }

            return httpRequests;
        }

        public HttpRequestData ParseRequest(string requestData)
        {
            var reqHttp = new HttpRequestData();      
                        
            string fullHeader = StringUtils.ExtractString(requestData, "", "\r\n\r\nHTTP",false,true);
            reqHttp.FullRequest = fullHeader; 
            
            string header = StringUtils.ExtractString(fullHeader, "", "\r\n\r\n",false,true);
            
            var lines = StringUtils.GetLines(header);
            reqHttp.Url = StringUtils.ExtractString(lines[0], " ", " HTTP/");
            reqHttp.HttpVerb = StringUtils.ExtractString(lines[0], "", " ");

            if (reqHttp.HttpVerb == "CONNECT")
            {
                return null;
            }


            if (reqHttp.HttpVerb != "GET")
                reqHttp.RequestContent = StringUtils.ExtractString(fullHeader, "\r\n\r\n", "\r\nHTTP", false, true);            

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                var tokens = line.Split(new String[1] { ": " },StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 1)
                {
                    var hd = new HttpRequestHeader
                    {
                        Name = tokens[0],
                        Value = tokens[1]
                    };
                    var name = hd.Name.ToLower();

                    if (name == "host")
                    {
                        reqHttp.Host = hd.Value;
                        continue;
                    }
                    if (name == "content-type")
                    {
                        reqHttp.ContentType = hd.Value;
                        continue;
                    }                    
                    if (name == "content-length")                        
                        continue;  // client adds this

                    reqHttp.Headers.Add(hd);
                }
            }

            reqHttp.Host = reqHttp.Headers
                .Where(hd => hd.Name == "Host")
                .Select(hd => hd.Value)
                .FirstOrDefault();                

            return reqHttp;
        }


        public string ErrorMessage {get; set; }

        protected void SetError()
        {
            this.SetError("CLEAR");
        }

        protected void SetError(string message)
        {
            if (message == null || message=="CLEAR")
            {
                this.ErrorMessage = string.Empty;
                return;
            }
            this.ErrorMessage += message;
        }

        protected void SetError(Exception ex, bool checkInner = false)
        {
            if (ex == null)
                this.ErrorMessage = string.Empty;

            Exception e = ex;
            if (checkInner)
                e = e.GetBaseException();

            ErrorMessage = e.Message;
        }
    }

}
