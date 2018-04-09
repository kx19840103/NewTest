using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using VirtualTrader;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace TradeClient
{
    public static partial class TradeClient
    {
        static readonly ILog Logger = log4net.LogManager.GetLogger(typeof(TradeClient));
        static IVirtualTrader Trader { get; }
        static Dictionary<string, string> Settings { get; set; }
        static List<StockData> LocalStockDataList { get; set; } = new List<StockData>();
        static List<dynamic> StockDataListFromTrader { get; set; } = new List<dynamic>();
        static List<StockData> UnusedStockDataList { get; set; } = new List<StockData>();

        static TradeClient()
        {
            Logger.Info("正在载入配置文件");
            LoadSettings("TradeClientSettings.json", "stocks.json");

            Trader = new VirtualTrader.VirtualTrader(Settings["UserId"], Settings["Password"]);
        }

        static void LoadSettings(string settingsFilename, string stocksFilename)
        {
            Logger.Info("正在载入settings.json");
            Settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(settingsFilename));

            Logger.Info("正在载入stocks.json");
            LocalStockDataList = JsonConvert.DeserializeObject<List<StockData>>(System.IO.File.ReadAllText(stocksFilename));
        }

        static void ResetTaskList()
        {
            Logger.Info("重置任务列表");
            foreach (var stock in UnusedStockDataList)
            {
                Logger.Info($"移除 {stock.Code}");
                LocalStockDataList.Remove(stock);
            }
            UnusedStockDataList.Clear();
        }

        static void RefreshTraderStockData()
        {
            Logger.Info($"更新持仓信息");
            var retryCount = 3;
            while(retryCount-- > 0)
            {
                var result = Trader.GetAllStockData();
                if (!result.IsSucceeded)
                {
                    Logger.Warn($"持仓信息更新失败");
                }
                else
                {
                    StockDataListFromTrader = new List<dynamic>();
                    foreach(var x in result.Extra)
                    {
                        StockDataListFromTrader.Add(x);
                        Logger.Info($"{x.Zqdm} {x.Zqmc}: 持仓数量 {x.Zqsl}, 可用数量 {x.Kysl}");
                    }
                    return;
                }
            }
        }

        static List<RevokeItem> GetRevokeItems()
        {
            var result = new List<RevokeItem>();

            dynamic revokeItems;
            var response = Trader.GetRevokeItems(out revokeItems);
            if (response.IsSucceeded)
            {
                foreach (var x in revokeItems)
                {
                    result.Add(new RevokeItem()
                    {
                        Id = $"{x.Wtrq}_{x.Wtbh}",
                        StockCode = x.Zqdm,
                        ItemType = ((string)(x.Mmbz)).Contains("S") ? RevokeItem.Type.Sell : RevokeItem.Type.Buy,
                        Price = x.Wtjg,
                        Amount = x.Wtsl
                    });
                }
            }

            return result;
        }

        static void Main(string[] args)
        {
            try
            {
                var loginResult = Trader.Login();
                if(!loginResult.IsSucceeded)
                {
                    Logger.Error("登录失败，程序退出");
                    Exit();
                }

                LocalStockDataList.ForEach(stock => Init(stock));

                var sleepTime = Int32.Parse(Settings["SleepTime"]) * 1000;
                var stopwatch = Stopwatch.StartNew();
                while (true)
                {
                    Logger.Info($"批量任务开始，共 {LocalStockDataList.Count} 对象");
                    RefreshTraderStockData();

                    LocalStockDataList.ForEach(stock =>
                    {
                        Logger.Info($"开始执行 {stock.Code} ");

                        ExecuteOneStock(stock);

                        Logger.Info($"结束执行 {stock.Code} ");
                    });

                    Logger.Info($"批量任务结束，共 {LocalStockDataList.Count} 对象");
                    ResetTaskList();

                    Logger.Info($"进入休眠");
                    if (stopwatch.ElapsedMilliseconds < sleepTime)
                    {
                        Thread.Sleep(sleepTime - (int)stopwatch.ElapsedMilliseconds);
                    }

                    stopwatch.Restart();
                }
            }
            catch (Exception e)
            {
                Logger.Info($"执行出错，退出执行。错误信息：{e.Message} StackTrace: {e.StackTrace}");
                Exit();
            }
        }

        static void Exit()
        {
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
