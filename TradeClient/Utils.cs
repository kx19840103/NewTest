using System.Net;
using System.Threading;
using VirtualTrader;

namespace TradeClient
{
    public class Utils
    {
        public enum StockMarket
        {
            SH, SZ
        }

        static CookieWebClient webClient { get; } = new CookieWebClient();
        static Utils()
        {
            webClient.Encoding = System.Text.Encoding.UTF8;
        }

        public static StockStatus GetStockStatus(string stockCode, StockMarket market)
        {
            var queryUrl = $"https://hsmarket.eastmoney.com/api/SHSZQuoteSnapshot?id={stockCode}&market={market.ToString()}&callback=jQuery183020675270035713877_1515211248785";
            Thread.Sleep(200);
            return new StockStatus(webClient.DownloadString(queryUrl));
        }
    }
}
