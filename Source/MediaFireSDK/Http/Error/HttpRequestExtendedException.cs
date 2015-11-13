using System;
using System.Net.Http;

namespace MediaFireSDK.Http.Error
{
    public class HttpRequestExtendedException : HttpRequestException
    {
        public HttpRequestExtendedException()
        {
        }

        public HttpRequestExtendedException(string message)
            : base(message)
        {
        }

        public HttpRequestExtendedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public HttpRequestExtendedException(HttpResponseMessage response, HttpRequestException excep, string responseContent)
            : base(string.Empty, excep)
        {
            Response = response;
            ResponseContent = responseContent;
        }

        public HttpResponseMessage Response { get; private set; }
        public string ResponseContent { get;  private set; }
    }
}
