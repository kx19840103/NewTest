using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrader
{
    public class Utils
    {
        public static string GenerateRndDouble(int length)
        {
            var randNum = "0.";
            var rnd = new Random();
            for (var i = 0; i < length; ++i)
            {
                var value = rnd.Next() % 10;
                if (i == length - 1 && value == 0) ++value;

                randNum += value;
            }
            return randNum;
        }
    }
}
