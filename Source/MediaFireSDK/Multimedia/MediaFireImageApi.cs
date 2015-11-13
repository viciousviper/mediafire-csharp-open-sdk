using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Model;

namespace MediaFireSDK.Multimedia
{
    public class MediaFireImageApi : IMediaFireImageApi
    {

        internal static string GenerateImageThumbnailUrl(string quickKey, string hash, MediaFireSupportedImageSize size)
        {
            const string multimediaImageConversionApiUrlFormat = "http://www.mediafire.com/conversion_server.php?{0}&quickkey={1}&doc_type=i&size_id={2}";
            const int requiredFirstHashBytes = 4;
            return string.Format(
                multimediaImageConversionApiUrlFormat,
                hash.Substring(0, requiredFirstHashBytes),
                quickKey,
                (char)size);


        }

        public string GetConvertImageUrl(string quickKey, string hash, MediaFireSupportedImageSize size)
        {
            return GenerateImageThumbnailUrl(quickKey, hash, size);
        }
    }
}
