using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSurge;

namespace WebSurgeSamplePlugin
{
    public class LogPlugIn : IWebSurgeExtensibility
    {
        public bool OnBeforeRequestSent(HttpRequestData data)
        {
            logRequests(data, 0);
            return true;
        }

        public void OnAfterRequestSent(HttpRequestData data)
        {
            logRequests(data, 1);
        }

        public bool OnLoadTestStarted(IList<HttpRequestData> requests)
        {
            LogString("Starting test with " + requests.Count + " in Session.");
            return true;
        }

        public void OnLoadTestCompleted(IList<HttpRequestData> results, int timeTakenForTestMs)
        {
            LogString("Completed test with " + results.Count + " requests processed.");
        }

        private void logRequests(HttpRequestData data, int mode = 0)
        {
            string output;

            // Check for a non-success response/errors
            bool isErorr = data.IsError;

            if (mode != 0)
                output = data.StatusCode + " " + data.HttpVerb + " " + data.Url + " " + data.TimeTakenMs;
            else
                output = data.HttpVerb + " " + data.Url;

            LogString(output);
        }

        public static object syncLock = new object();

        private void LogString(string message)
        {
            lock (syncLock)
            {
                StreamWriter streamWriter = new StreamWriter(Environment.CurrentDirectory + "\\requestlog.txt", true);
                streamWriter.WriteLine(DateTime.Now.ToString() + " - " + message);
                streamWriter.Close();
            }
        }
    }
}
