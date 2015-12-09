using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Core;
using MediaFireSDK.Http;
using MediaFireSDK.Model;

namespace MediaFireSDK.Multimedia
{
    internal class MediaFireImageApi : MediaFireApiBase, IMediaFireImageApi
    {
        public MediaFireImageApi(MediaFireRequestController requestController)
            : base(requestController)
        {
        }

        private string GenerateImageThumbnailUrl(string quickKey, string hash, MediaFireSupportedImageSize size, string accessToken)
        {

            const string multimediaImageConversionApiUrlFormat = "http://www.mediafire.com/conversion_server.php?{0}&quickkey={1}&doc_type=i&size_id={2}&session_token={3}";
            const int requiredFirstHashBytes = 4;
            return string.Format(
                multimediaImageConversionApiUrlFormat,
                hash.Substring(0, requiredFirstHashBytes),
                quickKey,
                (char)size,
                accessToken
                );


        }

        public string GetConvertImageUrl(string quickKey, string hash, MediaFireSupportedImageSize size)
        {
            return GenerateImageThumbnailUrl(quickKey, hash, size, RequestController.SessionBroker.CurrentSessionToken);
        }

        public async Task<string> GetConvertImageUrlAsync(string quickKey, string hash, MediaFireSupportedImageSize size)
        {
            //
            //  Authenticate a dummy request, this ensures that the access token used after this will be valid.
            //
            await RequestController.SessionBroker.Authenticate(new HttpClientAgent("", false, -1, false));

            return GetConvertImageUrl(quickKey, hash, size);

        }
    }
}
