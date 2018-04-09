using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading;

namespace VirtualTrader
{
    public class CookieWebClient : WebClient
    {
        private static readonly ILog Logger = log4net.LogManager.GetLogger(typeof(CookieWebClient));
        public CookieContainer CookieContainer { get; private set; }

        /// <summary>
        /// This will instanciate an internal CookieContainer.
        /// </summary>
        public CookieWebClient()
        {
            this.CookieContainer = new CookieContainer();
        }

        /// <summary>
        /// Use this if you want to control the CookieContainer outside this class.
        /// </summary>
        public CookieWebClient(CookieContainer cookieContainer)
        {
            this.CookieContainer = cookieContainer;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request == null) return base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";

            // sleep a while
            Thread.Sleep(new Random().Next(200, 500));

            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var retryCount = 1;
            WebResponse response = null;
            while(true)
            {
                try
                {
                    response = base.GetWebResponse(request);
                    break;
                }
                catch(Exception ex)
                {
                    var tm = (int)(Math.Log(retryCount + 1) * 1000);
                    Logger.Info($"Retry {retryCount++}: {request.RequestUri}, Sleep: {tm} ms, Exception: {ex}");
                    Thread.Sleep(tm);
                }
            }

            return response;
        }

        public string PostRequest(string address, Dictionary<string, string> parameters)
        {
            Logger.Info($"Request: {address}");
            Logger.Info($"Param: {JsonConvert.SerializeObject(parameters)}");

            var nameValues = new NameValueCollection();
            foreach(var kvp in parameters)
            {
                nameValues.Add(kvp.Key, kvp.Value);
            }
            byte[] responsebytes = this.UploadValues(address, "POST", nameValues);
            string responsebody = System.Text.Encoding.UTF8.GetString(responsebytes);

            Logger.Info($"Response: {responsebody}");
            return responsebody;
        }
    }
}
