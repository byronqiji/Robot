using System;
using System.IO;
using System.Net;
using System.Net.Security;

namespace Robot.Business.Helper
{
    public class HttpHelper
    {
        internal static string GetResponseValue(string url)
        {
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, errors) => { return true; });
            } 

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
            request.Date = DateTime.Now;

            WebResponse response = request.GetResponse();

            string responseValue = string.Empty;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                responseValue = sr.ReadToEnd();
            }

            return responseValue;
        }
    }
}
