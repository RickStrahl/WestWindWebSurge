using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSurge;
using WebSurge.Support;

namespace SimpleStressTester.Tests
{
    [TestClass]
    public class PostmanIntegrationTests
    {
        const string STR_WebSurgeSampleFile = "WebSurgeRequests.websurge";
        const string STR_PostmanCollectionFile = "1_Postman_Sample_Collection.json";

        [TestMethod]
        public void ExportPostman()
        {
            SessionParser parser = new SessionParser();
            var options = new StressTesterConfiguration();
            List<HttpRequestData> httpRequests = parser.ParseFile(STR_WebSurgeSampleFile, ref options);

            var postman = new PostmanIntegration();

            string result = postman.Export("Test", httpRequests, options);

            Assert.IsNotNull(result, "postman export failed: nothing returned");

            Console.WriteLine(result);

        }

        [TestMethod]
        public void ImportPostman()
        {

            var postman = new PostmanIntegration();

            var requests = postman.ImportFromFile(STR_PostmanCollectionFile);

            Assert.IsNotNull(requests, "Requests failed to import.");

            foreach (var req in requests.Requests)
            {
                Console.WriteLine(req.ToString());
                Console.WriteLine(" --- " + req.Username + " " + req.Password);
            }
        }

    }
}
