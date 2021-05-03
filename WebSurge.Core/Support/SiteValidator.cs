﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Westwind.Utilities;
using Westwind.Utilities.InternetTools;

namespace WebSurge
{
    /// <summary>
    /// Class that checks to see if the site that you are testing
    /// explicitly allows WebSurge to access the site.
    /// 
    /// Checks for WebSurge-Allow.txt or robots.txt by adding #Accept: WebSurge   
    /// </summary>
    /// <remarks>    
    /// We only check this on stress tests, not for individual URL tests
    /// or single run of a session.
    /// </remarks>
    public class SiteValidator
    {
        private StressTester StressTester;
        public string ErrorMessage;

        public SiteValidator(StressTester stressTester)
        {
            StressTester = stressTester;
        }

        public bool CheckAllServers(IEnumerable<HttpRequestData> requests)
        {
            if (requests == null)
            {
                ErrorMessage =  "There are no requests to process.";
                return false;
            }

            var uniqueServerUrls = new HashSet<string>();
            foreach (var request in requests)
            {
                if(!request.IsActive)
                    continue;

                var rootUrl = GetServerRootUrl(request.Url);

                // already checked
                if (uniqueServerUrls.Contains(rootUrl))
                    continue;
                
                if (IsWebSurgeAllowedForUrl(rootUrl))
                    uniqueServerUrls.Add(rootUrl);
                else
                {                    
                    ErrorMessage = string.Format(NotAllowedMessage, rootUrl);                                   
                    return false;
                }
            }

            return true;
        }

        public bool IsWebSurgeAllowedForUrl(string serverUrl)
        {        
            string serverRootUrl = GetServerRootUrl(serverUrl);
            try
            {
                var uri = new Uri(serverRootUrl);
                if (NetworkUtils.IsLocalIpAddress(uri.Host))
                    return true;
            }
            catch
            {
                return false;
            }


            var http = new HttpClient();
            if (!string.IsNullOrEmpty(StressTester.Options.Username))
                http.Username = StressTester.Options.Username;
            if (!string.IsNullOrEmpty(StressTester.Options.Password))
                http.Password = StressTester.Options.Password;

            try
            {
                var url = (serverRootUrl + "/websurge-allow.txt");
                var text = http.DownloadString(url);
                if (http.WebResponse.StatusCode != HttpStatusCode.OK && 
                    http.WebResponse.StatusCode != HttpStatusCode.NoContent)
                {
                    url = serverRootUrl + "/robots.txt";
                    string robots = http.DownloadString(url);
                    if (!robots.Contains("Allow: WebSurge"))
                        return false;
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.GetBaseException().Message;
                return false;
            }

            return true;
        }

        string GetServerRootUrl(string url)
        {
            if (!string.IsNullOrEmpty(StressTester.Options.ReplaceDomain) || !string.IsNullOrEmpty(StressTester.Options.SiteBaseUrl))
                url = StressTester.FixupUrl(url);

            var uri = new Uri(url);
            var builder = new UriBuilder(uri);

            builder.Path = string.Empty;
            builder.Query = string.Empty;

            var port = ":" + builder.Port;
            if ((builder.Scheme == "https" && builder.Port == 443) ||
                (builder.Scheme == "http" && builder.Port == 80))
                port = string.Empty;

            var path = $"{builder.Scheme}://{builder.Host}{port}";

            if (!string.IsNullOrEmpty(StressTester.Options.ReplaceDomain))
                path = StressTester.FixupUrl(path);

            return path;
        }

        public const string NotAllowedMessage =
@"Unable to run a stress test on the server at {0}. 

In order to run a load test on a server, you have to place either an empty file into the root of your Web server:

    websurge-allow.txt

or, you can add the following string to your robots.txt file (note it's meant to be a comment) also in the root of the Web server:

    #Allow: WebSurge

These settings ensure that you only load test sites that you have control over.
";
    }
}
