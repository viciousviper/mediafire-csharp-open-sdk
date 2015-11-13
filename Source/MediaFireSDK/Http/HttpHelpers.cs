using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaFireSDK.Model;

namespace MediaFireSDK.Http
{



    internal static class HttpHelpers
    {


        const string QueryStringFormat = "{0}?{1}";


        public static string GetString(this Encoding encoding, byte[] chars)
        {
            return encoding.GetString(chars, 0, chars.Length);
        }

        public static string BuildQueryString(IEnumerable<KeyValuePair<string, string>> parameters, string url)
        {
            var str = String.Join("&", parameters.Select(v => string.Concat(v.Key, "=", v.Value)).ToList());
            return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(string.Format(QueryStringFormat, url, str)));

        }

        public static async Task<string> Upload(
            Stream src,
            string url,
            CancellationToken token,
            IProgress<MediaFireOperationProgress> progress,
            Dictionary<string, string> parameters,
            Dictionary<string, string> headers,
            long size,
            MediaFireApiConfiguration configuration
            )
        {
            const string httpPostMethodString = "POST";

            if (parameters != null && parameters.Count != 0)
            {
                url = BuildQueryString(parameters, url);
            }

            var wr = (HttpWebRequest)WebRequest.Create(url);

            if (headers != null && headers.Count != 0)
            {
                foreach (var header in headers)
                {
                    if (header.Key == MediaFireApiConstants.ContentTypeHeader)
                        wr.ContentType = header.Value;
                    else
                    {
                        wr.Headers[header.Key] = header.Value;
                    }
                }
            }

            wr.Method = httpPostMethodString;

            var dstStream = await Task<Stream>.Factory.FromAsync(wr.BeginGetRequestStream, wr.EndGetRequestStream, null);

            await CopyStreamWithProgress(
                src,
                dstStream,
                progress,
                token,
                new MediaFireOperationProgress { TotalSize = size },
                configuration.ChunkTransferBufferSize);

            dstStream.Dispose();

            var wResp = await Task<WebResponse>.Factory.FromAsync(wr.BeginGetResponse, wr.EndGetResponse, null);

            var str = new StreamReader(wResp.GetResponseStream());
            var result = str.ReadToEnd();
            return result;
        }

        public static async Task<Stream> CopyStreamWithProgress(
            Stream source,
            Stream destination,
            IProgress<MediaFireOperationProgress> progress,
            CancellationToken token,
            MediaFireOperationProgress progressData,
            int bufferSize
            )
        {
            int read, offset = 0;
            var buffer = new byte[bufferSize];
            using (source)
            {
                do
                {
                    read = await source.ReadAsync(buffer, 0, bufferSize, token);

                    await destination.WriteAsync(buffer, 0, read, token);

                    offset += read;
                    progressData.CurrentSize = offset;
                    progress.Report(progressData);
                } while (read != 0);
            }

            return destination;
        }
    }
}
