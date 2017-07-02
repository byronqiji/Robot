using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;

namespace Robot.Request
{
    public class BaseRequest : HttpRequest
    {
        public BaseRequest(string url, HttpMethod method = HttpMethod.GET)
        {
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, errors) => { return true; });
            }

            httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            httpWebRequest.Method = method.ToString();
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
            httpWebRequest.Date = DateTime.Now;
        }

        internal WebResponse GetResponse()
        {
            return httpWebRequest.GetResponse();
        }

        public void SetData(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            httpWebRequest.ContentLength = buffer.Length;
            using (Stream stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        public void SetCookies(CookieCollection cookies)
        {
            httpWebRequest.CookieContainer = new CookieContainer();

            if (cookies != null)
            {
                foreach (Cookie cookie in cookies)
                {
                    httpWebRequest.CookieContainer.Add(cookie);
                }
            }
        }
    }
}
