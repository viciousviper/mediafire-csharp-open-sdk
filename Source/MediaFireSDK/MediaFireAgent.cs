using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaFireSDK.Core;
using MediaFireSDK.Http;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Responses;
using MediaFireSDK.Multimedia;
using MediaFireSDK.Services;

namespace MediaFireSDK
{
    public class MediaFireAgent : IMediaFireAgent
    {
        private readonly MediaFireRequestController _requestController;

        public MediaFireAgent(MediaFireApiConfiguration configuration)
        {
            Configuration = configuration;
            _requestController = new MediaFireRequestController(configuration);
            var cryptoService = new BouncyCastleCryptoService();
            User = new MediaFireUserApi(_requestController, configuration, cryptoService);
            Image = new MediaFireImageApi(_requestController);
            Upload = new MediaFireUploadApi(_requestController);
        }

        public MediaFireApiConfiguration Configuration { get; private set; }
        public IMediaFireUserApi User { get; private set; }
        public IMediaFireImageApi Image { get; private set; }
        public IMediaFireUploadApi Upload { get; private set; }


        public async Task<T> GetAsync<T>(string path, IDictionary<string, object> parameters = null, bool attachSessionToken = true) where T : MediaFireResponseBase
        {
            var req = await ConfigureRequest(path, parameters, attachSessionToken);
            return await _requestController.Get<T>(req);
        }


        public async Task<T> PostAsync<T>(string path, IDictionary<string, object> parameters = null, bool attachSessionToken = true) where T : MediaFireResponseBase
        {
            var req = await ConfigureRequest(path, parameters, attachSessionToken);
            return await _requestController.Post<T>(req);
        }

        public async Task<T> PostStreamAsync<T>(
            string path,
            Stream content,
            IDictionary<string, object> parameters,
            IDictionary<string, string> headers,
            bool attachSessionToken = true,
            CancellationToken? token = null,
            IProgress<MediaFireOperationProgress> progress = null

            ) where T : MediaFireResponseBase
        {

            token = token ?? CancellationToken.None;
            progress = progress ?? new Progress<MediaFireOperationProgress>();
            var mfProgressData = new MediaFireOperationProgress();

            var req = await _requestController.CreateHttpRequest(path, attachSessionToken, isChunkedOperation: true);
            req.Content(content, true);

            foreach (var header in headers)
            {
                req.ContentHeader(header.Key, header.Value);
            }

            if (content.CanSeek)
            {
                mfProgressData.TotalSize = content.Length;
            }
            else
            {
                if (headers.ContainsKey(MediaFireApiConstants.FileSizeHeader))
                    mfProgressData.TotalSize = long.Parse(headers[MediaFireApiConstants.FileSizeHeader]);

                if (headers.ContainsKey(MediaFireApiConstants.UnitSizeHeader))
                    mfProgressData.TotalSize = long.Parse(headers[MediaFireApiConstants.UnitSizeHeader]);
            }


            req.WithProgress(progress, mfProgressData, token.Value);

            return await _requestController.Post<T>(req);
        }

        private async Task<HttpRequestConfiguration> ConfigureRequest(string path, IDictionary<string, object> parameters, bool attachSessionToken)
        {


            var requestConfig = await _requestController.CreateHttpRequest(path, attachSessionToken);
            if (parameters != null)
            {

                foreach (var parameter in parameters)
                {
                    requestConfig.Parameter(parameter.Key, parameter.Value);
                }
            }

            return requestConfig;
        }

        public void Dispose()
        {
            _requestController.SessionBroker.Dispose();
            User = null;
            Image = null;
            Upload = null;

        }
    }
}
