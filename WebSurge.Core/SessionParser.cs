using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Westwind.Utilities;

namespace WebSurge
{
    public class SessionParser
    {
        public const string STR_Separator = "\r\n------------------------------------------------------------------\r\n";
        public const string STR_StartWebSurgeOptions = "\r\n----- Start WebSurge Options -----\r\n";
        public const string STR_EndWebSurgeOptions = "\r\n----- End WebSurge Options -----\r\n";

        /// <summary>
        /// This method parses a Fiddler Session file that's in basic HTTP header
        /// format. To see what this format looks like create a session in the
        /// UI then Save it to disk. The persistence format is plain text and
        /// using the simple to understand, natural HTTP header format.
        /// </summary>
        /// <param name="fiddlerSessionFile"></param>
        /// <returns></returns>
        public List<HttpRequestData> ParseFile(string fiddlerSessionFile, ref StressTesterConfiguration options)
        {
            if (fiddlerSessionFile == null)
                fiddlerSessionFile = Path.GetFullPath("1_Full.txt");

            if (!File.Exists(fiddlerSessionFile))
            {
                SetError("File doesn't exist.");
                return null;
            }
                        
            string fileText = File.ReadAllText(fiddlerSessionFile);
            if (string.IsNullOrEmpty(fileText))
            {
                SetError("Couldn't read Session file data.");
                return null;
            }
            
            return Parse(fileText,ref options);
        }


        /// <summary>
        /// Parses a Fiddler Session from a string.
        /// </summary>
        /// <param name="sessionString">Http Headers string for multiple requests</param>
        /// <returns>List of HTTP requests or null on failure</returns>
        public List<HttpRequestData> Parse(string sessionString, ref StressTesterConfiguration config)
        {
            if (config != null)
                config = ParseConfiguration(ref sessionString);

            //strip configuration data if it exists
            if (!string.IsNullOrEmpty(sessionString))
            {
                int index = sessionString.IndexOf(STR_StartWebSurgeOptions);
                if (index > -1)
                    sessionString = sessionString.Substring(0, index);
            }
                            
            var httpRequests = new List<HttpRequestData>();            

            string[] requests = Regex.Split(sessionString, @"\r\n-{5,100}\r\n");

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

        StressTesterConfiguration ParseConfiguration(ref string sessionString)
        {
            StressTesterConfiguration options = null;
            
            string json = StringUtils.ExtractString(sessionString, STR_StartWebSurgeOptions, STR_EndWebSurgeOptions);
            if (!string.IsNullOrEmpty(json))
            {
                options = JsonSerializationUtils.Deserialize(json,typeof(StressTesterConfiguration))
                    as StressTesterConfiguration;

                if (options == null)
                    options = new StressTesterConfiguration();                

                if(options.Password != null)
                    options.Password = Encryption.DecryptString(options.Password, App.EncryptionMachineKey);
            }

            return options;
        }



        /// <summary>
        /// Parses an individual requests in HTTP header format.
        /// Expects the URL to be part of the first HTTP header line:
        /// 
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
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
                reqHttp.ParseHttpHeaders(lines);
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
        /// <param name="options"></param>
        /// <returns></returns>
        public bool Save(List<HttpRequestData> requests, string filename, 
                          StressTesterConfiguration options = null)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var request in requests)
            {
                sb.Append(request.ToRequestHttpHeader());

                if (!string.IsNullOrEmpty(request.ResponseHeaders))
                    sb.Append(request.ToResponseHttpHeader());
                    
                sb.AppendLine(STR_Separator);                
            }

            if (options != null)
            {
                sb.AppendLine(STR_StartWebSurgeOptions);
                
                // Encrypt and write
                string password = options.Password;
                if(!string.IsNullOrEmpty(password))
                    options.Password = Encryption.EncryptString(options.Password, App.EncryptionMachineKey);
                sb.AppendLine(JsonSerializationUtils.Serialize(options,false,true));
                options.Password = password;

                sb.AppendLine("\r\n// This file was generated by West Wind WebSurge");
                sb.AppendLine("// Get your free copy at http://websurge.west-wind.com");
                sb.AppendLine("// to easily test or load test the requests in this file.");
                sb.AppendLine(STR_EndWebSurgeOptions);
            }

            File.WriteAllText(filename, sb.ToString());

            return true;
        }


        public string ErrorMessage {get; set; }

        protected void SetError()
        {
            SetError("CLEAR");
        }

        protected void SetError(string message)
        {
            if (message == null || message=="CLEAR")
            {
                ErrorMessage = string.Empty;
                return;
            }
            ErrorMessage += message;
        }

        protected void SetError(Exception ex, bool checkInner = false)
        {
            if (ex == null)
                ErrorMessage = string.Empty;

            Exception e = ex;
            if (checkInner)
                e = e.GetBaseException();

            ErrorMessage = e.Message;
        }
    }

}
