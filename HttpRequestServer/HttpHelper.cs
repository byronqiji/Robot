using Robot.Model.WeChat;
using System.IO;
using System.Net;

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
                if (i < 10)
                    return GetResponseValue(response, ++i);

                return string.Empty;
            }
        }

        public static string GetResponseValue(string url, HttpMethod method, string data)
        {
            return GetResponseValue(CreateRequest(url, method, data).GetResponse());
        }

        public static BaseRequest CreateRequest(string url, HttpMethod method, string data)
        {
            BaseRequest request = new BaseRequest(url, method);
            request.SetCookies(UserInfo.Instance.Cookies);
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
