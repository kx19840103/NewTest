using HtmlAgilityPack;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;

// window.addEventListener("beforeunload", function() { debugger; }, false)

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace VirtualTrader
{
    public enum TradeType
    {
        Buy, Sell
    }

    public class OperationResult
    {
        public bool IsSucceeded { get; set; } = true;
        public List<string> Messages { get; set; } = new List<string>();
        public dynamic Extra { get; set; }
        public void AddError(string errorMessage)
        {
            IsSucceeded = false;
            Messages.Add(errorMessage);
        }

    }

    public interface IVirtualTrader
    {
        //====order related
        OperationResult Buy(string stockCode, double price, int amount, string zqmc);
        OperationResult Sell(string stockCode, double price, int amount, string zqmc);

        //====revoke
        OperationResult Revoke(string stockCode);   // order id
        OperationResult RevokeByItemId(string ItemId);
        OperationResult GetRevokeItems(out dynamic revokeItems);

        //
        OperationResult GetAllStockData();

        OperationResult Login();
    }

    public class VirtualTrader : IVirtualTrader
    {
        public static readonly ILog Logger = log4net.LogManager.GetLogger(typeof(VirtualTrader));
        private CookieWebClient WebClient { get; } = new CookieWebClient();
        private string ValidateKey { get; set; } = null;
        private Dictionary<string, string> JsonSettings { get; set; } = new Dictionary<string, string>();
        private string UserId { get; set; }
        private string Password { get; set; }

        private VirtualTrader() { }

        public VirtualTrader(string userId, string password)
        {
            UserId = userId;
            Password = password;
            JsonSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText("VirtualTraderSettings.json"));

            WebClient.Headers[HttpRequestHeader.UserAgent] = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            WebClient.Encoding = System.Text.Encoding.UTF8;
        }

        public OperationResult Buy(string stockCode, double price, int amount, string zqmc)
        {
            try
            {
                var result = LoginIfNeeded();
                if (!result.IsSucceeded) return result;

                var buyUrl = JsonSettings["buyUrl"] + ValidateKey;
                var formParams = new Dictionary<string, string>
                {
                    { "stockCode",  stockCode},
                    { "price",      string.Format("{0:0.00}", price)},
                    { "amount",     amount.ToString()},
                    { "tradeType",  "B"},
                    { "zqmc",       zqmc}
                };

                var response = WebClient.PostRequest(buyUrl, formParams);
                if (!response.Contains("Wtbh"))
                {
                    Logger.Warn($"Buy {stockCode} failed: {response}");
                    result.AddError(response);
                }

                return result;
            }
            catch (Exception e)
            {
                var result = new OperationResult();
                result.AddError(e.Message);
                return result;
            }
        }

        public OperationResult Revoke(string stockCode)
        {
            dynamic revokeItems;
            var result = GetRevokeItems(out revokeItems);
            if(result.IsSucceeded)
            {
                foreach(var x in revokeItems)
                {
                    if (x.Zqdm == stockCode)
                    {
                        var ItemId = $"{x.Wtrq}_{x.Wtbh}";
                        if (!RevokeByItemId(ItemId).IsSucceeded)
                        {
                            var warning = $"revoke failed. {stockCode}: {ItemId}";
                            Logger.Warn(warning);
                            result.AddError(warning);
                        }
                    }
                }
            }
            else
            {
                var warning = $"GetRevokeItems failed while trying to revoke {stockCode}";
                Logger.Warn(warning);
                result.AddError(warning);
            }

            return result;
        }

        public OperationResult Sell(string stockCode, double price, int amount, string zqmc)
        {
            try
            {
                var result = LoginIfNeeded();
                if (!result.IsSucceeded) return result;
                var buyUrl = JsonSettings["sellUrl"] + ValidateKey;

                var formParams = new Dictionary<string, string>
                {
                    { "stockCode",  stockCode},
                    { "price",      string.Format("{0:0.00}", price)},
                    { "amount",     amount.ToString()},
                    { "tradeType",  "S"},
                    { "zqmc",       zqmc}
                };

                var response = WebClient.PostRequest(buyUrl, formParams);
                if (!response.Contains("Wtbh")) result.AddError(response);

                return result;
            }
            catch (Exception e)
            {
                var result = new OperationResult();
                result.AddError(e.Message);
                return result;
            }
        }

        public string GetValidateKeyFromHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc.DocumentNode.SelectSingleNode("//input[@id = 'em_validatekey']").Attributes["value"].Value;
        }

        public OperationResult Login()
        {
            Logger.Info("开始登录");
            try
            {
                var LoginRetryTimes = Int32.Parse(JsonSettings["commonRetryTimes"]);

                var html = "";
                var retried = 0;
                for (; retried <= LoginRetryTimes; ++retried)
                {
                    // 2. get vCode img from result
                    var randNum = Utils.GenerateRndDouble(16);
                    var vCodeImgInBytes = WebClient.DownloadData($"{JsonSettings["vCode"]}?randNum={Utils.GenerateRndDouble(16)}");

                    // 3. popup a window
                    using (var form = new VerificationCodeForm())
                    {
                        form.PictureBox.Image = new System.Drawing.Bitmap(new System.IO.MemoryStream(vCodeImgInBytes));
                        form.ShowDialog();

                        // 4. do login
                        if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            doLogin(form.VerficationCodeValueTextBox.Text, randNum);
                        }
                    }

                    html = WebClient.DownloadString(JsonSettings["homepage"]);
                    if (isAlreadyLoggedIn(html)) break;

                    Logger.Info($"login failed. {UserId}, {Password}");
                }

                var result = new OperationResult();
                if (retried > LoginRetryTimes)
                {
                    result.AddError("login failed");
                }
                else
                {
                    // 5. get validateKey
                    ValidateKey = GetValidateKeyFromHtml(html);
                    result.Messages.Add(ValidateKey);
                }

                return result;
            }
            catch (Exception e)
            {
                var result = new OperationResult();
                result.AddError(e.Message);
                return result;
            }
        }

        public OperationResult LoginIfNeeded()
        {
            var html = WebClient.DownloadString(JsonSettings["homepage"]);
            if (!isAlreadyLoggedIn(html)) return Login();

            ValidateKey = GetValidateKeyFromHtml(html);
            var ret = new OperationResult();
            ret.Messages.Add(ValidateKey);
            return ret;
        }

        private string doLogin(string vCode, string randNum)
        {
            var loginUrl = JsonSettings["login"];

            var loginParams = new Dictionary<string, string>
            {
                { "userId",         UserId},
                { "password",       Password},
                { "randNumber",     randNum},
                { "identifyCode",   vCode},
                { "duration",       JsonSettings["duration"]},
                { "authCode",       ""},
                { "type",           "Z"},
            };

            return WebClient.PostRequest(loginUrl, loginParams);
        }

        private bool isAlreadyLoggedIn(string html)
        {
            return html.Contains(JsonSettings["loggedinSign"]);
        }


        public OperationResult GetRevokeItems(out dynamic revokeItems)
        {
            revokeItems = null;
            var result = LoginIfNeeded();
            if (!result.IsSucceeded) return result;

            try
            {
                var str = WebClient.PostRequest($"{JsonSettings["getRevokeListUrl"]}{ValidateKey}", new Dictionary<string, string>());
                var response = JsonConvert.DeserializeObject<GetRevokeItemsResponse>(str);
                revokeItems = response.Data;
            }
            catch(Exception e)
            {
                Logger.Warn($"GetRevokeItems failed. {e.Message}");
                result.AddError($"GetRevokeItems failed. {e.Message}");
            }

            return result;
        }

        public OperationResult GetAllStockData()
        {
            var result = LoginIfNeeded();
            if (!result.IsSucceeded) return result;

            var str = WebClient.PostRequest($"{JsonSettings["getAllStockDataUrl"]}{ValidateKey}", new Dictionary<string, string>
            {
                { "qqhs", "1000" },
                { "dwc", ""}
            });

            dynamic response = JsonConvert.DeserializeObject(str);
            result.Extra = response.Data;

            return result;
        }

        public OperationResult RevokeByItemId(string ItemId)
        {
            var result = new OperationResult();

            // do revoke
            var param = new Dictionary<string, string>
                        {
                            {"revokes", ItemId}
                        };
            var response = WebClient.PostRequest($"{JsonSettings["doRevokeUrl"]}{ValidateKey}", param);

            if (!response.Contains("撤单已报"))
            {
                var warning = $"revoke by order id failed. ItemId: {ItemId}";
                Logger.Warn(warning);
                result.AddError(warning);
            }

            return result;
        }

        class GetRevokeItemsResponse
        {
            public string Message { get; set; }
            public int Status { get; set; }
            public List<dynamic> Data { get; set; }
        }
    }
}
