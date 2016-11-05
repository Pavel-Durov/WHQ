using Microsoft.VisualStudio.TestTools.UnitTesting;
using WHQ.ProcessStrategies;

namespace WHQ.ProcessStrategies.Tests
{
    [TestClass()]
    public class DumpFileProcessStrategyTests
    {
        const string FAKE_FILE_PATH = @"C:\temp\dump\dump.dmp";
        const string REAL_FILE_PATH = @"C:\ISO\dumps\Windows_Server_2012_R2\DumpTest.dmp";

        [TestMethod()]
        public void RunTest_Failed()
        {
            DumpFileProcessStrategy strategy = new DumpFileProcessStrategy(FAKE_FILE_PATH);

            Assert.IsNotNull(strategy.FilePath);
        }

        [TestMethod()]
        public void RunTest()
        {
            DumpFileProcessStrategy strategy = new DumpFileProcessStrategy(REAL_FILE_PATH);

            Assert.IsNotNull(strategy.FilePath);
        }
    }
}