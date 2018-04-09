using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeClient
{
    public class StockData
    {
        public string Code { get; set; }
        public Utils.StockMarket Market { get; set; }// 上海60 深圳00 30  基金15 51 
        public double CeilPrice { get; set; }
        public double FloorPrice { get; set; }
        public double BasePrice { get; set; }
        public double PriceFix { get; set; }
        public int Intervalnumber { get; set; }// 间隔数量
        public double Basebottom { get; set; } // 底仓数目
        public double Totalinvestment { get; set; } //总投资额
        public double Intervaltime { get; set; }//间隔时间
        //public double Position { get; set; }//当前仓位
        public int? Initialposition { get; set; }//当前仓位初始
        //public double CurrentAvailability { get; set; }//当前可用数量
    }

    static partial class TradeClient
    {
        static void Init(StockData stockData)
        {
            Logger.Info($"Initializing {stockData.Code} ");

            stockData.Initialposition = GetCurrentAmount(stockData.Code);
            Logger.Info($" {stockData.Code}当前持仓为 {stockData.Initialposition}");
            if (stockData.Initialposition == null)
            {
                Logger.Warn($"未找到 {stockData.Code} 持仓信息，终止执行");
                AddStockToUnusedList(stockData);
            }
            else
            {
                var stockStatuss = Utils.GetStockStatus(stockData.Code, stockData.Market);
                Logger.Info($" {stockData.Code} { stockStatuss.Name} 初始化！！");
                Trader.Revoke(stockData.Code);
                var Price = AssignPrice(stockData.Code, stockData.BasePrice, stockData.PriceFix);
                stockData.FloorPrice = Price[0];
                stockData.CeilPrice = Price[1];
                Logger.Info($"初始化赋值  {stockData.Code} {stockStatuss.Name} 基准：{stockData.BasePrice},上限：{stockData.CeilPrice},下限：{stockData.FloorPrice}");
                Trader.Buy(stockData.Code, stockData.FloorPrice, stockData.Intervalnumber, stockStatuss.Name);
                Trader.Sell(stockData.Code, stockData.CeilPrice, stockData.Intervalnumber, stockStatuss.Name);
            }

        }

        static void ExecuteOneStock(StockData stockData)
        {
            /*
            var revokeItems = GetRevokeItems();
            foreach(var item in revokeItems)
            {
                if(item.StockCode.Contains("xxxxxxx") // stockcode
                    || item.ItemType == RevokeItem.Type.Buy // item type
                    || item.Price > 0.00
                    || item.Amount > 10
                    )
                {
                    Trader.RevokeByItemId(item.Id);
                }
            }
            */

            var CurrentAvailability = GetCurrentAvailabilityAmount(stockData.Code);//当前可用数量
            if (CurrentAvailability == null)
            {
                Logger.Warn($"未找到 {stockData.Code} 持仓信息，终止执行");
                return;
            }
            var position = GetCurrentAmount(stockData.Code); //当前持仓数目
            Logger.Info($"{stockData.Code} 当前持仓数目: {position}");
            if (position < 0)
            {
                Logger.Warn($"未找到 {stockData.Code} 持仓信息，终止执行");
                return;
            }
            if (position <= stockData.Basebottom)// 如果某只股票的当前持仓数目小于设置的底舱数目，停止该只股票今日的所有操作
            {
                Logger.Info($"{stockData.Code} 当前仓持仓 <= 底仓数目，停止操作");
                AddStockToUnusedList(stockData);
            }
            else
            {
                if ((stockData.Totalinvestment - position * stockData.BasePrice) < stockData.FloorPrice * stockData.Intervalnumber)
                {
                    Logger.Info($"{stockData.Code} 满仓，停止操作");
                    AddStockToUnusedList(stockData);
                }
                else
                {
                    var stockStatus = Utils.GetStockStatus(stockData.Code, stockData.Market);
                    Logger.Info($"{stockData.Code} {stockStatus.Name} 当前股价: {stockStatus.CurrentPrice}  基准：{ stockData.BasePrice},上限：{ stockData.CeilPrice},下限：{ stockData.FloorPrice}");
                    Logger.Info($"{stockData.Code} {stockStatus.Name} 初始持仓：{stockData.Initialposition} 当前持仓数目：{position}");
                    var currentPrice = stockStatus.CurrentPrice;
                    if (currentPrice > stockData.CeilPrice)
                    {
                        Logger.Info($"{stockData.Code} {stockStatus.Name}  当前估价大于上限价格，判断为 卖出 成交一次");
                        //Logger.Info($"{stockData.Code} {stockStatus.Name}  卖出 成交一次");
                        Trader.Revoke(stockData.Code);
                        CurrentAvailability = GetCurrentAvailabilityAmount(stockData.Code);
                        Logger.Info($"{stockData.Code} {stockStatus.Name} 执行撤单  当前可用数量：{CurrentAvailability}");
                        if (CurrentAvailability == 0)// 如果某只股票的当前可用数量为0，停止该只股票今日的所有操作
                        {
                            Logger.Info($"{stockData.Code} 可用数量为0，停止操作");
                            AddStockToUnusedList(stockData);
                        }
                        else
                        {
                            stockData.Initialposition = position;
                            stockData.BasePrice = stockData.CeilPrice;
                            var Price = AssignPrice(stockData.Code, stockData.BasePrice, stockData.PriceFix);
                            stockData.FloorPrice = Price[0];
                            stockData.CeilPrice = Price[1];
                            Logger.Info($"初始化赋值  {stockData.Code} {stockStatus.Name} 基准：{stockData.BasePrice},上限：{stockData.CeilPrice},下限：{stockData.FloorPrice}");
                            Trader.Buy(stockData.Code, stockData.FloorPrice, stockData.Intervalnumber, stockStatus.Name);
                            Trader.Sell(stockData.Code, stockData.CeilPrice, stockData.Intervalnumber, stockStatus.Name);
                        }
                    }
                    else if (currentPrice < stockData.FloorPrice)
                    {
                        Logger.Info($"{stockData.Code} {stockStatus.Name}  当前股价小于下限价格，判断为 买入 成交一次");
                        //Logger.Info($"{stockData.Code} {stockStatus.Name}  买入 成交一次");
                        Trader.Revoke(stockData.Code);
                        CurrentAvailability = GetCurrentAvailabilityAmount(stockData.Code);
                        Logger.Info($"{stockData.Code} {stockStatus.Name} 执行撤单  当前可用数量：{CurrentAvailability}");
                        if (CurrentAvailability == 0)// 如果某只股票的当前可用数量为0，停止该只股票今日的所有操作
                        {
                            Logger.Info($"{stockData.Code} 可用数量为0，停止操作");
                            AddStockToUnusedList(stockData);
                        }
                        else
                        {
                            stockData.Initialposition = position;
                            stockData.BasePrice = stockData.FloorPrice;
                            var Price = AssignPrice(stockData.Code, stockData.BasePrice, stockData.PriceFix);
                            stockData.FloorPrice = Price[0];
                            stockData.CeilPrice = Price[1];
                            Logger.Info($"初始化赋值  {stockData.Code} {stockStatus.Name} 基准：{stockData.BasePrice},上限：{stockData.CeilPrice},下限：{stockData.FloorPrice}");
                            Trader.Buy(stockData.Code, stockData.FloorPrice, stockData.Intervalnumber, stockStatus.Name);
                            Trader.Sell(stockData.Code, stockData.CeilPrice, stockData.Intervalnumber, stockStatus.Name);
                        }
                    }
                    else //((currentPrice >= stockData.FloorPrice) && (currentPrice <= stockData.CeilPrice))
                    {
                        if (position == (stockData.Initialposition + stockData.Intervalnumber))
                        {
                            Logger.Info($"{stockData.Code} {stockStatus.Name} 持仓数量增加一个标准数量，判断为 买入 成交一次");
                            //Logger.Info($"{stockData.Code} {stockStatus.Name} 买入 成交一次");
                            Trader.Revoke(stockData.Code);
                            CurrentAvailability = GetCurrentAvailabilityAmount(stockData.Code);
                            Logger.Info($"{stockData.Code} {stockStatus.Name} 执行撤单  当前可用数量：{CurrentAvailability}");
                            if (CurrentAvailability == 0)// 如果某只股票的当前可用数量为0，停止该只股票今日的所有操作
                            {
                                Logger.Info($"{stockData.Code} 可用数量为0，停止操作");
                                AddStockToUnusedList(stockData);
                            }
                            else
                            {
                                stockData.Initialposition = position;
                                stockData.BasePrice = stockData.FloorPrice;
                                var Price = AssignPrice(stockData.Code, stockData.BasePrice, stockData.PriceFix);
                                stockData.FloorPrice = Price[0];
                                stockData.CeilPrice = Price[1];
                                Logger.Info($"初始化赋值  {stockData.Code} {stockStatus.Name} 基准：{stockData.BasePrice},上限：{stockData.CeilPrice},下限：{stockData.FloorPrice}");
                                Trader.Buy(stockData.Code, stockData.FloorPrice, stockData.Intervalnumber, stockStatus.Name);
                                Trader.Sell(stockData.Code, stockData.CeilPrice, stockData.Intervalnumber, stockStatus.Name);
                            }
                        }
                        if (position == (stockData.Initialposition - stockData.Intervalnumber))
                        {
                            Logger.Info($"{stockData.Code} {stockStatus.Name} 持仓数量 减少 一个标准数量，判断为 卖出 成交一次");
                            //Logger.Info($"{stockData.Code} {stockStatus.Name} 卖出 成交一次");
                            Trader.Revoke(stockData.Code);
                            CurrentAvailability = GetCurrentAvailabilityAmount(stockData.Code);
                            Logger.Info($"{stockData.Code} {stockStatus.Name} 执行撤单  当前可用数量：{CurrentAvailability}");
                            if (CurrentAvailability == 0)// 如果某只股票的当前可用数量为0，停止该只股票今日的所有操作
                            {
                                Logger.Info($"{stockData.Code} 可用数量为0，停止操作");
                                AddStockToUnusedList(stockData);
                            }
                            else
                            {
                                stockData.Initialposition = position;
                                stockData.BasePrice = stockData.CeilPrice;
                                var Price = AssignPrice(stockData.Code, stockData.BasePrice, stockData.PriceFix);
                                stockData.FloorPrice = Price[0];
                                stockData.CeilPrice = Price[1];
                                Logger.Info($"初始化赋值  {stockData.Code} {stockStatus.Name} 基准：{stockData.BasePrice},上限：{stockData.CeilPrice},下限：{stockData.FloorPrice}");
                                Trader.Buy(stockData.Code, stockData.FloorPrice, stockData.Intervalnumber, stockStatus.Name);
                                Trader.Sell(stockData.Code, stockData.CeilPrice, stockData.Intervalnumber, stockStatus.Name);
                            }
                        }
                    }
                }
            }
        }

        static int? GetCurrentAmount(string stockCode)// 获取当前持仓数量
        {
            RefreshTraderStockData();
            var ret = StockDataListFromTrader.Where(x => x.Zqdm == stockCode);
            return ret.Count() == 0 ? null : ret.First().Zqsl;
        }

        static int? GetCurrentAvailabilityAmount(string stockCode)// 获取当前可用数量
        {
            RefreshTraderStockData();
            var ret = StockDataListFromTrader.Where(x => x.Zqdm == stockCode);
            return ret.Count() == 0 ? -1 : ret.First().Kysl;
        }

        static void AddStockToUnusedList(StockData stockData)
        {
            //结束程序 自动关机 发个邮件提醒（因当当前可用数量为0，所以停止 stockData.Code 股票操作
            Logger.Info($"添加 {stockData} 至待移除列表");
            UnusedStockDataList.Add(stockData);
        }


        static double[] AssignPrice(string Code, double BasePrice, double PriceFix)// 上下限赋值，15 51 开头的为基金，基金的价格精确到 “厘”
        {
            double[] Price = new double[2];
            var Codetemp = Code.Substring(0, 2);
            if ((Codetemp == "15") || (Codetemp == "51"))
            {
                Price[0] = Math.Round(BasePrice * (1 - PriceFix), 3, MidpointRounding.AwayFromZero);
                Price[1] = Math.Round(BasePrice * (1 + PriceFix), 3, MidpointRounding.AwayFromZero);
                return Price;
            }
            else
            {
                Price[0] = Math.Round(BasePrice * (1 - PriceFix), 2, MidpointRounding.AwayFromZero);
                Price[1] = Math.Round(BasePrice * (1 + PriceFix), 2, MidpointRounding.AwayFromZero);
                return Price;
            }
        }
    }
}
