using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            string[] requests = Regex.Split(file, @"\r\n-{5,100}\r\n");
            
            //string[] requests = file.Split(new string[1] {STR_Separator},StringSplitOptions.RemoveEmptyEntries);
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

            if (lines.Length > 0)
            {
                lines[0] = string.Empty;
                reqHttp.ParseHttpHeader(lines);
            }

            reqHttp.Host = reqHttp.Headers
                .Where(hd => hd.Name == "Host")
                .Select(hd => hd.Value)
                .FirstOrDefault();                

            return reqHttp;
        }

       


        /// <summary>
        /// Writes out the request data to disk
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Save(List<HttpRequestData> requests,string filename)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var request in requests)
            {
                sb.Append(request.ToHttpHeader());
                sb.AppendLine(STR_Separator);                
            }

            File.WriteAllText(filename, sb.ToString());

            return true;
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
