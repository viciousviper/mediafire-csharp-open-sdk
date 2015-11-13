using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaFireSDK.Http.Error;
using MediaFireSDK.Model;
using Newtonsoft.Json;

namespace MediaFireSDK.Http
{
    internal class UploadWithProgressHttpContent : HttpContent
    {
        private readonly IProgress<MediaFireOperationProgress> _progress;
        private readonly MediaFireOperationProgress _data;
        private readonly Stream _file;
        private readonly int _bufferSize;
        private readonly CancellationToken _token;

        public UploadWithProgressHttpContent(
            IProgress<MediaFireOperationProgress> progress,
            MediaFireOperationProgress data,
            Stream file,
            int bufferSize,
            CancellationToken token)
        {
            _progress = progress;
            _data = data;
            _file = file;
            _bufferSize = bufferSize;
            _token = token;
        }



        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return HttpHelpers.CopyStreamWithProgress(_file, stream, _progress, _token, _data, _bufferSize);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _file.Length;
            return true;
        }
    }

    internal class HttpClientAgent : HttpRequestConfiguration
    {
        private readonly int _chunkBufferSize;
        private readonly bool _useHttpV1;

        public HttpClientAgent(string path, bool ignoreEmptyValues, int chunkBufferSize, bool useHttpV1)
            : base(path, ignoreEmptyValues)
        {
            _chunkBufferSize = chunkBufferSize;
            _useHttpV1 = useHttpV1;
        }


        public async override Task<T> SendAsync<T>(HttpMethod method)
        {

            var handler = new HttpClientHandler();


            var cli = new HttpClient(handler);

            foreach (var header in Headers)
            {
                cli.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            HttpContent content = null;

            if (Stream != null && IsUpload)
            {
                content = new UploadWithProgressHttpContent(ProgressOperation, ProgressData, Stream, _chunkBufferSize, Token);
            }


            if ((Parameters.Count != 0) && (content != null || method != HttpMethod.Post))
                Path = HttpHelpers.BuildQueryString(Parameters, Path);
            else
            {
                var parameters = new MultipartFormDataContent();
                foreach (var parameter in Parameters)
                {
                    parameters.Add(new StringContent(parameter.Value), parameter.Key);
                }
                content = parameters;
            }



            var req = new HttpRequestMessage(method, Path) { Content = content };

            if (content != null)
            {
                foreach (var contentHeader in ContentHeaders)
                {
                    content.Headers.Add(contentHeader.Key, contentHeader.Value);
                }

                req.Content = content;

            }

            //
            //  On WinRt this fixes the "The server committed a protocol violation. Section=ResponseStatusLine" Exception
            //
            if (IsUpload && _useHttpV1)
                req.Version = new Version(1, 0);


            var completionOption = HttpCompletionOption.ResponseContentRead;

            if (IsDownload)
                completionOption = HttpCompletionOption.ResponseHeadersRead;

            var resp = await cli.SendAsync(req, completionOption, Token).ConfigureAwait(false);

            if (IsDownload && resp.IsSuccessStatusCode)
            {
                await DownloadToStream(resp);
                return default(T);

            }

            return await DeserializeObject<T>(resp);
        }

        private async Task DownloadToStream(HttpResponseMessage resp)
        {
            var respStream = await resp.Content.ReadAsStreamAsync();
            ProgressData.TotalSize = resp.Content.Headers.ContentLength.Value;

            using (Stream)
            {
                await HttpHelpers.CopyStreamWithProgress(respStream, Stream, ProgressOperation, Token, ProgressData, _chunkBufferSize);
            }
        }

        private async Task<T> DeserializeObject<T>(HttpResponseMessage resp)
        {
            var resultString = await resp.Content.ReadAsStringAsync();

            try
            {
                resp.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestExtendedException(resp, e, resultString);
            }


            return DeserializeJson<T>(resultString);
        }

       
    }
}
