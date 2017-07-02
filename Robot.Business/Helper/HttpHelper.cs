using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;

namespace Robot.Business.Helper
{
    public enum HttpMethod
    {
        GET = 1,
        POST = 2,
    }

    public class HttpHelper
    {
        internal static string GetResponseValue(string url, CookieCollection cookies)
        {
            return GetResponseValue(GetResponse(url, cookies));
        }

        internal static string GetResponseValue(string url, HttpMethod method = HttpMethod.GET, string data = "")
        {
            return GetResponseValue(GetResponse(url, method, data));
        }

        internal static string GetResponseValue(WebResponse response)
        {
            string responseValue = string.Empty;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                responseValue = sr.ReadToEnd();
            }

            return responseValue;
        }

        internal static WebResponse GetResponse(string url, CookieCollection cookies)
        {
            return null;
        }

        internal static WebResponse GetResponse(string url, HttpMethod method = HttpMethod.GET, string data = "")
        {
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, errors) => { return true; });
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
            request.Date = DateTime.Now;

            if (method == HttpMethod.POST)
            {
                SetRequestData(request, data);
            }

            return request.GetResponse();
        }

        private static void SetRequestData(HttpWebRequest request, string data)
        {
            request.Method = HttpMethod.POST.ToString();
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            request.ContentLength = buffer.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
