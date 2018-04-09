using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TradeClient;

namespace VirtualTraderTest
{
    [TestClass]
    public class TradeClientTests
    {
        [TestMethod]
        public void Regex_BasicTest()
        {
            var text = @"jQuery183020675270035713877_1515211248785({""code"":""600000"",""name"":""浦发银行"",""topprice"":""13.93"",""bottomprice"":""11.39"",""fivequote"":{""yesClosePrice"":""12.66"",""openPrice"":""12.67"",""sale1"":""12.69"",""sale2"":""12.70"",""sale3"":""12.71"",""sale4"":""12.72"",""sale5"":""12.73"",""buy1"":""12.68"",""buy2"":""12.67"",""buy3"":""12.66"",""buy4"":""12.65"",""buy5"":""12.64"",""sale1_count"":3484,""sale2_count"":4947,""sale3_count"":5534,""sale4_count"":4508,""sale5_count"":4386,""buy1_count"":2564,""buy2_count"":3541,""buy3_count"":2431,""buy4_count"":4124,""buy5_count"":2337},""realtimequote"":{""open"":""12.67"",""high"":""12.71"",""low"":""12.62"",""avg"":""12.67"",""zd"":""0.03"",""zdf"":""0.24%"",""turnover"":""0.11%"",""currentPrice"":""12.69"",""volume"":""310267"",""amount"":""393058256"",""wp"":""146164"",""np"":""164104"",""time"":""15:10:45""}});";
            var pat = @"""code"":""([^""]*)""";
            var r = new Regex(pat, RegexOptions.IgnoreCase);
            var m = r.Match(text);
            var g = m.Groups[1];
        }

        [TestMethod]
        public void InitStockStatusWithString_BasicTest()
        {
            var text = @"jQuery183020675270035713877_1515211248785({""code"":""600000"",""name"":""浦发银行"",""topprice"":""13.93"",""bottomprice"":""11.39"",""fivequote"":{""yesClosePrice"":""12.66"",""openPrice"":""12.67"",""sale1"":""12.69"",""sale2"":""12.70"",""sale3"":""12.71"",""sale4"":""12.72"",""sale5"":""12.73"",""buy1"":""12.68"",""buy2"":""12.67"",""buy3"":""12.66"",""buy4"":""12.65"",""buy5"":""12.64"",""sale1_count"":3484,""sale2_count"":4947,""sale3_count"":5534,""sale4_count"":4508,""sale5_count"":4386,""buy1_count"":2564,""buy2_count"":3541,""buy3_count"":2431,""buy4_count"":4124,""buy5_count"":2337},""realtimequote"":{""open"":""12.67"",""high"":""12.71"",""low"":""12.62"",""avg"":""12.67"",""zd"":""0.03"",""zdf"":""0.24%"",""turnover"":""0.11%"",""currentPrice"":""12.69"",""volume"":""310267"",""amount"":""393058256"",""wp"":""146164"",""np"":""164104"",""time"":""15:10:45""}});";
            var stockStatus = new StockStatus(text);
            Assert.AreEqual("600000", stockStatus.Code);
            Assert.AreEqual("浦发银行", stockStatus.Name);
            Assert.AreEqual(13.93, stockStatus.TopPrice);
            Assert.AreEqual(11.39, stockStatus.BottomPrice);
            Assert.AreEqual(12.69, stockStatus.CurrentPrice);
        }

        [TestMethod]
        public void LoadSettings_BasicTest()
        {
            Assert.AreEqual("60", TradeClient.TradeClient.Settings["SleepTime"]);

            Assert.AreEqual(2, TradeClient.TradeClient.LocalStockDataList.Count);
            Assert.AreEqual("123456", TradeClient.TradeClient.LocalStockDataList[0].Code);
            Assert.AreEqual("654321", TradeClient.TradeClient.LocalStockDataList[1].Code);

        }

        [TestMethod]
        public void AdHoc()
        {
            var a = new List<StockData>() { new StockData() };
            var str = JsonConvert.SerializeObject(a, Formatting.Indented);
            ;

        }

        #region functional

        [TestMethod]
        public void GetStockStatus_BasicTest_Functional()
        {
            var stockStatus = Utils.GetStockStatus("600000", Utils.StockMarket.SH);
            Assert.AreEqual("600000", stockStatus.Code);
            Assert.AreEqual("浦发银行", stockStatus.Name);
        }

        [TestMethod]
        public void GetStockStatus_StockTypeNotEqualsToSHorSZ_Functional()
        {
            var stockStatus = Utils.GetStockStatus("113009", Utils.StockMarket.SH);
            Assert.AreEqual("113009", stockStatus.Code);
            Assert.AreEqual("广汽转债", stockStatus.Name);
            Assert.AreEqual(0.0, stockStatus.TopPrice);
            Assert.AreEqual(0.0, stockStatus.BottomPrice);
        }

        #endregion
    }
}
