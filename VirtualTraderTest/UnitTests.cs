using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using VirtualTrader;

namespace VirtualTraderTest
{
    [TestClass]
    public class UnitTests
    {
        static string UserId = "";
        static string Password = "";


        [TestMethod]
        public void GetValidateKeyFromHtml_BasicTest()
        {
            var client = new VirtualTrader.VirtualTrader(UserId, Password);
            var key = client.GetValidateKeyFromHtml(File.ReadAllText("homepage.html"));
            Assert.AreEqual(key, "9a2be776-b9ed-4997-807c-6c51e953e028");
        }

        [TestMethod]
        public void DoubleToStringWithTrailingZerosTest()
        {
            Assert.AreEqual("0.00", string.Format("{0:0.00}", 0));
            Assert.AreEqual("1.00", string.Format("{0:0.00}", 1));
            Assert.AreEqual("12.00", string.Format("{0:0.00}", 12));
            Assert.AreEqual("123.00", string.Format("{0:0.00}", 123));
            Assert.AreEqual("1234.00", string.Format("{0:0.00}", 1234));

            Assert.AreEqual("0.00", string.Format("{0:0.00}", 0.0));
            Assert.AreEqual("1.00", string.Format("{0:0.00}", 1.0));
            Assert.AreEqual("12.00", string.Format("{0:0.00}", 12.0));
            Assert.AreEqual("123.00", string.Format("{0:0.00}", 123.0));
            Assert.AreEqual("1234.00", string.Format("{0:0.00}", 1234.0));

            Assert.AreEqual("0.10", string.Format("{0:0.00}", 0.1));
            Assert.AreEqual("1.10", string.Format("{0:0.00}", 1.1));
            Assert.AreEqual("12.10", string.Format("{0:0.00}", 12.1));
            Assert.AreEqual("123.10", string.Format("{0:0.00}", 123.1));
            Assert.AreEqual("1234.10", string.Format("{0:0.00}", 1234.1));

            Assert.AreEqual("0.12", string.Format("{0:0.00}", 0.12));
            Assert.AreEqual("1.12", string.Format("{0:0.00}", 1.12));
            Assert.AreEqual("12.12", string.Format("{0:0.00}", 12.12));
            Assert.AreEqual("123.12", string.Format("{0:0.00}", 123.12));
            Assert.AreEqual("1234.12", string.Format("{0:0.00}", 1234.12));

            Assert.AreEqual("0.12", string.Format("{0:0.00}", 0.123));
            Assert.AreEqual("1.12", string.Format("{0:0.00}", 1.123));
            Assert.AreEqual("12.12", string.Format("{0:0.00}", 12.123));
            Assert.AreEqual("123.12", string.Format("{0:0.00}", 123.123));
            Assert.AreEqual("1234.12", string.Format("{0:0.00}", 1234.123));

            Assert.AreEqual("0.13", string.Format("{0:0.00}", 0.129));
            Assert.AreEqual("1.13", string.Format("{0:0.00}", 1.129));
            Assert.AreEqual("12.13", string.Format("{0:0.00}", 12.129));
            Assert.AreEqual("123.13", string.Format("{0:0.00}", 123.129));
            Assert.AreEqual("1234.13", string.Format("{0:0.00}", 1234.129));

            Assert.AreEqual("0.10", string.Format("{0:0.00}", 0.10));
            Assert.AreEqual("1.10", string.Format("{0:0.00}", 1.10));
            Assert.AreEqual("12.10", string.Format("{0:0.00}", 12.10));
            Assert.AreEqual("123.10", string.Format("{0:0.00}", 123.10));
            Assert.AreEqual("1234.10", string.Format("{0:0.00}", 1234.10));
        }
    }
}
