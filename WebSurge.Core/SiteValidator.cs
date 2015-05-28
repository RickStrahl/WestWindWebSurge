using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            if (new Uri(serverRootUrl).IsLoopback)
                return true;

            var http = new HttpClient();
            if (!string.IsNullOrEmpty(StressTester.Options.Username))
                http.Username = StressTester.Options.Username;
            if (!string.IsNullOrEmpty(StressTester.Options.Password))
                http.Password = StressTester.Options.Password;


            try
            {
                var text = http.DownloadString(serverRootUrl + "websurge-allow.txt");
                if (http.WebResponse.StatusCode != System.Net.HttpStatusCode.OK || text.Length > 5)
                {
                    string robots = http.DownloadString(serverRootUrl + "robots.txt");
                    if (!robots.Contains("Allow: WebSurge"))
                        return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        string GetServerRootUrl(string url)
        {
            // fix up if stress tester is replacing the domain
            if (!string.IsNullOrEmpty(StressTester.Options.ReplaceDomain))
                url = StressTester.ReplaceDomain(url);

            var uri = new Uri(url);            
            var builder = new UriBuilder(uri);            
            
            builder.Path = string.Empty;
            builder.Query = string.Empty;
            return builder.ToString();
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
