using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSurge;

namespace SimpleStressTester.Tests
{
    [TestClass]
    public class PercentileExtensionsTests
    {
        [TestMethod]
        public void TestPercentile()
        {
            var sequence = new[] {35m, 40m, 20m, 50m, 15m};

            Assert.AreEqual(46m, sequence.Percentile(0.9m));
            Assert.AreEqual(48m, sequence.Percentile(0.95m));
            Assert.AreEqual(49.6m, sequence.Percentile(0.99m));
            Assert.AreEqual(50m, sequence.Percentile(1m));
        }
    }
}