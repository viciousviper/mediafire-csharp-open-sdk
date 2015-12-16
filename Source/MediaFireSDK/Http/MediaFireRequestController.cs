using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Core;
using MediaFireSDK.Http.Error;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Errors;
using MediaFireSDK.Model.Responses;
using Newtonsoft.Json;

namespace MediaFireSDK.Http
{

    internal class MediaFireRequestController
    {
        public MediaFireApiConfiguration Configuration { get; private set; }
        private readonly string _baseApiPath;
        internal MediaFireSessionBroker SessionBroker { get; set; }


        public MediaFireRequestController(MediaFireApiConfiguration configuration)
        {
            Configuration = configuration;
            _baseApiPath = string.Format(MediaFireApiConstants.BaseUrlWithVersionFormat, MediaFireApiConstants.BaseUrl,
                configuration.ApiVersion);
        }

        public string GetApiFullPath(string path)
        {
            return _baseApiPath + path;
        }

        public string RemoveBaseUrlFromUrl(string url)
        {
            return url.Replace(_baseApiPath, string.Empty);
        }

        public async Task<HttpRequestConfiguration> CreateHttpRequest(string relativePath, bool authenticate = true,
            bool isChunkedOperation = false)
        {
            var fullPath = GetApiFullPath(relativePath);
            var request = GetConfiguration(fullPath, isChunkedOperation);

            return await ConfigureRequest(authenticate, request);
        }

        public async Task<HttpRequestConfiguration> CreateHttpRequestForFullPath(string fullPath, bool authenticate = true,
          bool isChunkedOperation = false)
        {
            var request = GetConfiguration(fullPath, isChunkedOperation);

            return await ConfigureRequest(authenticate, request);
        }

        private HttpRequestConfiguration GetConfiguration(string path, bool isChunkedOperation)
        {
            const bool ignoreEmptyParameters = true;

            return new HttpClientAgent(path, ignoreEmptyParameters, Configuration.ChunkTransferBufferSize, Configuration.UseHttpV1);
        }


        private async Task<HttpRequestConfiguration> ConfigureRequest(bool authenticate,
            HttpRequestConfiguration request)
        {
            request
                .Parameter(MediaFireApiParameters.AppId, Configuration.AppId)
                .Parameter(MediaFireApiParameters.ResponseFormat, MediaFireApiConstants.JsonFormat);

            if (authenticate == false)
                return request;

            if (SessionBroker == null)
                throw new MediaFireException(MediaFireErrorMessages.RequestMustBeAuthenticated);

            await AuthenticateRequest(request);


            return request;
        }


        public Task<T> Post<T>(HttpRequestConfiguration request) where T : MediaFireResponseBase
        {
            return SendAndUnwrap<MediaFireResponseContainer<T>, T>(request, HttpMethod.Post);
        }

        public Task<T> Get<T>(HttpRequestConfiguration request) where T : MediaFireResponseBase
        {
            return SendAndUnwrap<MediaFireResponseContainer<T>, T>(request, HttpMethod.Get);
        }

        private async Task<Q> SendAndUnwrap<T, Q>(HttpRequestConfiguration request, HttpMethod method)
            where Q : MediaFireResponseBase
            where T : MediaFireResponseContainer<Q>
        {
            var resp = await Send<T>(request, method);

            //
            //  Download Operations do not have responses.
            //
            if (request.IsDownload)
                return null;

            return resp.Response;
        }

        private async Task<T> Send<T>(HttpRequestConfiguration request, HttpMethod method)
        {
            HttpRequestExtendedException error;


            try
            {
                return await request.SendAsync<T>(method);
            }
            catch (HttpRequestExtendedException e)
            {
                if (string.IsNullOrEmpty(e.ResponseContent))
                    throw;

                error = e;
            }
             
            //
            //  Check if MediaFire returned any error.
            //
            MediaFireApiException detailedError;
            try
            {
                var errorDetails =
                    JsonConvert.DeserializeObject<MediaFireResponseContainer<MediaFireErrorResponse>>(error.ResponseContent);
                detailedError = new MediaFireApiException(error, request, errorDetails.Response);
            }
            catch (Exception)
            {
                //
                //   The content didn't have any useful information, just throw the original exception
                // 
                throw error;
            }

            //
            //  Check if the error was about an expired session token, if so try to renew it.
            //
            if (detailedError.Error != MediaFireApiErrorCodes.InvalidToken || Configuration.AutomaticallyRenewToken == false)
                throw detailedError;


            //
            //  Renew session token
            //
            await SessionBroker.RetrieveNewSessionToken();

            //
            //  Re authenticate the request with the renewed token
            //
            await AuthenticateRequest(request);

            //
            //  Retry the previous request
            //
            return await Send<T>(request, method);

        }


        private async Task AuthenticateRequest(HttpRequestConfiguration request)
        {
            await SessionBroker.Authenticate(request);
        }

        public T DeserializeObject<T>(string content) where T : MediaFireResponseBase
        {
            return JsonConvert.DeserializeObject<MediaFireResponseContainer<T>>(content).Response;
        }
    }
}
