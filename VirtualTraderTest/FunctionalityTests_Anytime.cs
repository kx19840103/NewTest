using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using VirtualTrader;

namespace VirtualTraderTest
{
    [TestClass]
    public class FunctionalityTests2
    {
        static string UserId = "xx";
        static string Password = "xx";
        static readonly VirtualTrader.VirtualTrader Trader = new VirtualTrader.VirtualTrader(UserId, Password);

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var ret = Trader.Login();
            Assert.IsTrue(ret.IsSucceeded);
        }

        [TestMethod]
        public void GetAllStockData_BasicTest()
        {
            var ret = Trader.GetAllStockData();
            Assert.IsTrue(ret.IsSucceeded);
        }

    }
}
