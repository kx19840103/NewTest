using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TradeClient
{
    public class RevokeItem
    {
        private static readonly ILog Logger = log4net.LogManager.GetLogger(typeof(StockStatus));

        public enum Type
        {
            Buy, Sell
        }

        #region Properties
        public string Id { get; set; }
        public string StockCode { get; set; }
        public Type ItemType { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        #endregion

    }
}
