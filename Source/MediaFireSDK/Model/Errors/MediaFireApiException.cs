using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Http;
using MediaFireSDK.Model.Responses;

namespace MediaFireSDK.Model.Errors
{

    public class MediaFireException : Exception
    {
        internal MediaFireException()
        {
        }

        internal MediaFireException(string message) : base(message)
        {
        }

        internal MediaFireException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class MediaFireApiException : MediaFireException
    {


        public HttpRequestConfiguration RequestConfiguration { get; private set; }
        public HttpRequestException RequestException { get; private set; }

        public MediaFireResponseBase Response { get; set; }

        public int Error { get { return Response.Error; } }

        public MediaFireApiException(
            HttpRequestException requestException,
            HttpRequestConfiguration requestConfiguration,
            MediaFireResponseBase errorDetails)
            : base(errorDetails.Message)
        {
            RequestException = requestException;
            RequestConfiguration = requestConfiguration;
            Response = errorDetails;
        }

    }
}
