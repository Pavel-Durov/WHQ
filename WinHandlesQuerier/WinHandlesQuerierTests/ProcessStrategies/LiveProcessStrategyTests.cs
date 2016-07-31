using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinHandlesQuerier.ProcessStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHandlesQuerier.ProcessStrategies.Tests
{
    [TestClass()]
    public class LiveProcessStrategyTests
    {
        [TestMethod()]
        public void RunTest()
        {
            LiveProcessStrategy strategy = new LiveProcessStrategy(1);

            Assert.AreNotEqual<uint>(0, strategy.PID);
        }
    }
}