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
    public class FunctionalityTests_Limit
    {
        static string UserId = "";
        static string Password = "";
        static readonly VirtualTrader.VirtualTrader Trader = new VirtualTrader.VirtualTrader(UserId, Password);

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var ret = Trader.Login();
            Assert.IsTrue(ret.IsSucceeded);
        }

        [TestMethod]
        public void Login_BasicTest()
        {
            var ret = Trader.Login();
            Assert.IsTrue(ret.IsSucceeded);
        }

        [TestMethod]
        public void Buy_BasicTest()
        {
            var ret = Trader.Buy("127003", 0.1, 10, "海印转债");
            Assert.IsTrue(ret.IsSucceeded);
        }

        [TestMethod]
        public void GetRevokeList_BasicTest()
        {
            //dynamic revokeItems;
            //var ret = trader.GetRevokeList(out revokeItems);
            //Assert.IsNotNull((object)revokeItems);
        }

        [TestMethod]
        public void Revoke_BasicTest()
        {
            var ret = Trader.Buy("127003", 0.1, 10, "海印转债");
            Assert.IsTrue(ret.IsSucceeded);

            ret = Trader.Revoke("127003");
            Assert.IsTrue(ret.IsSucceeded);
        }

        [TestMethod]
        public void Sell_BasicTest()
        {
            var ret = Trader.Sell("600010", 2.52, 100, "包钢股份");
            Assert.IsTrue(ret.IsSucceeded);
        }
    }
}
