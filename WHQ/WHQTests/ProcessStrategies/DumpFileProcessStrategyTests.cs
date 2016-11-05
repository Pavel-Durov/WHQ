using Microsoft.VisualStudio.TestTools.UnitTesting;
using WHQ.Core.Handlers;
using WHQ.ProcessStrategies;

namespace WHQ.ProcessStrategies.Tests
{
    [TestClass()]
    public class DumpFileProcessStrategyTests
    {
        const string FILE_PATH = @"C:\temp\dump\dump.dmp";

        [TestMethod()]
        public void RunTest()
        {
            DumpFileProcessStrategy strategy = new DumpFileProcessStrategy(FILE_PATH);

            Assert.IsNotNull(strategy.FilePath);
        }
    }
}