using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSurge;

namespace SimpleStressTester.Tests
{
    [TestClass]
    public class RequestEncoding
    {
        [TestMethod]
        public void StringEncoding()
        {
            string text = "testé";
            byte[] b = Encoding.UTF8.GetBytes(text);            
            Console.WriteLine(Encoding.GetEncoding(1252).GetString(b));

            text = "Motörhead";
            b = Encoding.UTF8.GetBytes(text);
            Console.WriteLine(Encoding.GetEncoding(1252).GetString(b));
        }

        [TestMethod]
        public void HttpRequestDataEncoding()
        {
            string body =
                @"""{""Artist"":{""Id"":331,""ArtistName"":""Motörhead testé"" }"" ";

            var req = new HttpRequestData()
            {
                TextEncoding = "utf-8",
                RequestContent = body
            };

            Console.WriteLine("Raw: " + body);
            var encoded = req.GetRequestContentAsEncodedString();
            Console.WriteLine("Encoded: " + encoded);

            Assert.IsTrue(encoded.Contains("MotÃ¶rhead"));
        }
    }
}
