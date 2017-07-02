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
        
        public static string GetResponseValue(WebResponse response)
        {
            string responseValue = string.Empty;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                responseValue = sr.ReadToEnd();
            }

            return responseValue;
        }

        public static string GetResponseValue(string url, HttpMethod method, string data)
        {
            BaseRequest request = new BaseRequest(url, method);
            request.SetData(data);

            return GetResponseValue(request.GetResponse());
        }

        public static string GetResponseValue(string url, CookieCollection cookies)
        {
            BaseRequest request = new BaseRequest(url);
            request.SetCookies(cookies);

            return GetResponseValue(request.GetResponse());
        }
    }
}
