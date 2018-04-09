using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TradeClient
{
    public class StockStatus
    {
        private static readonly ILog Logger = log4net.LogManager.GetLogger(typeof(StockStatus));

        #region Properties
        public string Code { get; set; }
        public string Name { get; set; }
        public double TopPrice { get; set; }
        public double BottomPrice { get; set; }
        public double CurrentPrice { get; set; }
        #endregion

        private static readonly Regex codeRegex = new Regex(@"""code"":""([^""]*)""", RegexOptions.IgnoreCase);
        private static readonly Regex nameRegex = new Regex(@"""name"":""([^""]*)""", RegexOptions.IgnoreCase);
        private static readonly Regex topPriceRegex = new Regex(@"""topPrice"":""([^""]*)""", RegexOptions.IgnoreCase);
        private static readonly Regex bottomPriceRegex = new Regex(@"""bottomPrice"":""([^""]*)""", RegexOptions.IgnoreCase);
        private static readonly Regex currentPriceRegex = new Regex(@"""currentPrice"":""([^""]*)""", RegexOptions.IgnoreCase);

        StockStatus(){}

        // init properties using response string
        public StockStatus(string text)
        {
            Code = codeRegex.Match(text).Groups[1].ToString();
            Name = nameRegex.Match(text).Groups[1].ToString();

            Double price;
            if (Double.TryParse(topPriceRegex.Match(text).Groups[1].ToString(), out price))
                TopPrice = price;
            else
                Logger.Info("topPrice not found");

            if (Double.TryParse(bottomPriceRegex.Match(text).Groups[1].ToString(), out price))
                BottomPrice = price;
            else
                Logger.Info("bottomPrice not found");

            if (Double.TryParse(currentPriceRegex.Match(text).Groups[1].ToString(), out price))
                CurrentPrice = price;
            else
                Logger.Info("currentPrice not found");

        }
    }
}
