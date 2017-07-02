using System.Net;

namespace Robot.Request
{
    public enum HttpMethod
    {
        GET = 1,
        POST = 2,
    }

    public abstract class HttpRequest
    {
        internal HttpWebRequest httpWebRequest;
    }
}
