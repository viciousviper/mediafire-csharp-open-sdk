using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Model;

namespace MediaFireSDK.Multimedia
{
    
    public interface IMediaFireImageApi
    {
        /// <summary>
        /// Allows you to convert image files to png images of various sizes. 
        /// </summary>
        /// <param name="quickKey">The quickkey of the file to be converted</param>
        /// <param name="hash">The file hash</param>
        /// <param name="size">The return image desired size</param>
        /// <returns>An url to the location of the converted image.</returns>
        string GetConvertImageUrl(string quickKey, string hash, MediaFireSupportedImageSize size);
    }

    public static class MediaFireImageApiExtensions
    {
        public static string GetFileThumbnail(this IMediaFireImageApi imageApi, MediaFireFile file, MediaFireSupportedImageSize size)
        {
            return imageApi.GetConvertImageUrl(file.QuickKey, file.Hash, size);
        }

        public static string GetThumbnail(this MediaFireFile file, MediaFireSupportedImageSize size)
        {
            return MediaFireImageApi.GenerateImageThumbnailUrl(file.QuickKey, file.Hash, size);
        }
    }
}
