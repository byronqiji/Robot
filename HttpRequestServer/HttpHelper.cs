using Robot.Model.WeChat;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Robot.Request
{
    public class HttpHelper
    {
        public static string GetResponseValue(string url)
        {
            BaseRequest request = new BaseRequest(url);

            return GetResponseValue(request.GetResponse());
        }

        public static WebResponse GetResponse(string url)
        {
            BaseRequest request = new BaseRequest(url);
            return request.GetResponse();
        }

        public static WebResponse GetResponse(string url, CookieCollection cookies)
        {
            return CreateRequest(url, cookies).GetResponse();
        }

        private static BaseRequest CreateRequest(string url, CookieCollection cookies)
        {
            BaseRequest request = new BaseRequest(url);
            request.SetCookies(cookies);

            return request;
        }

        public static string GetResponseValue(WebResponse response, int i = 0)
        {
            try
            {
                string responseValue = string.Empty;
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    responseValue = sr.ReadToEnd();
                }

                return responseValue;
            }
            catch
            {
                Thread.Sleep(500);

                if (i < 5)
                    return GetResponseValue(response, ++i);

                return string.Empty;
            }
        }

        public static string GetResponseValue(object loginUrl)
        {
            throw new NotImplementedException();
        }

        public static string GetResponseValue(string url, HttpMethod method, string data, CookieCollection cookies)
        {
            return GetResponseValue(CreateRequest(url, method, data, cookies).GetResponse());
        }

        public static BaseRequest CreateRequest(string url, HttpMethod method, string data, CookieCollection cookies)
        {
            BaseRequest request = new BaseRequest(url, method);
            request.SetCookies(cookies);
            if (method == HttpMethod.POST)
                request.SetData(data);
            return request;
        }

        public static string GetResponseValue(string url, CookieCollection cookies)
        {
            return GetResponseValue(CreateRequest(url, cookies).GetResponse());
        }
    }
}
