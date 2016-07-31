using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinHandlesQuerier.Core.Handlers;

namespace WinHandlesQuerier.ProcessStrategies.Tests
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