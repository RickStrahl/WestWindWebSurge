using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSurge;

namespace SimpleStressTester.Tests
{
    [TestClass]
    public class SiteValidatorTests
    {
        [TestMethod]
        public void UriParsing()
        {
            var url = "http://www.microsoft.com/en-us/default.aspx";

            var builder = new UriBuilder(new Uri(url));
            builder.Path = string.Empty;

            Console.WriteLine(builder.ToString());

        }

        [TestMethod]
        public void BasicSiteValidatorTest()
        {
            var requestList = new List<HttpRequestData>()
            {
                new HttpRequestData
                {
                    Url = "http://www.microsoft.com/en-us/default.aspx"
                },
                new HttpRequestData
                {
                    Url = "http://localhost/WebLog"
                },
                              new HttpRequestData
                {
                    Url = "http://localhost/WebLog/posts"
                }

            };
            var stress = new StressTester();

            var validator = new SiteValidator(stress);
            bool result= validator.CheckAllServers(requestList);

            Assert.IsFalse(result);
            Console.WriteLine(validator.ErrorMessage);

        }

        [TestMethod]
        public void Basic2SiteValidatorTest()
        {
            var requestList = new List<HttpRequestData>()
            {

                // should work because it's local loopback
                new HttpRequestData
                {
                    Url = "http://localhost/WebLog"
                },
                              new HttpRequestData
                {
                    Url = "http://localhost/WebLog/posts"
                }

            };
            var stress = new StressTester();

            var validator = new SiteValidator(stress);
            bool result = validator.CheckAllServers(requestList);

            Assert.IsTrue(result);
            Console.WriteLine(validator.ErrorMessage);
        }
        
        [TestMethod]
        public void Basic3SiteValidatorTest()
        {

            var requestList = new List<HttpRequestData>()
            {

                // should work because it's local loopback
                new HttpRequestData
                {
                    Url = "http://rasxps/WebLog"
                },
                              new HttpRequestData
                {
                    Url = "http://rasxps/WebLog/posts"
                }

            };
            var stress = new StressTester();

            var validator = new SiteValidator(stress);
            bool result = validator.CheckAllServers(requestList);

            Console.WriteLine(validator.ErrorMessage);
            Assert.IsTrue(result);
            
        }

    }
}
