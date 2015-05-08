using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSurge;
using Westwind.Utilities;

namespace WebSurgeSamplePlugin
{
    public class SamplePlugIn : IWebSurgeExtensibility
    {
        public static object syncLock = new object();

        public bool OnBeforeRequestSent(HttpRequestData data)
        {
            logRequests(data, 0);
            return true;
        }

        public void OnAfterRequestSent(HttpRequestData data)
        {
            logRequests(data, 1);
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

            lock (syncLock)
            {
                StreamWriter streamWriter = new StreamWriter(Environment.CurrentDirectory + "\\requestlog.txt", true);
                streamWriter.WriteLine(DateTime.Now.ToString() + " - " + output);
                streamWriter.Close();
            }
        }
    }
}
