using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Responses;

namespace MediaFireSDK.Core
{
    public interface IMediaFireUploadApi
    {
        /// <summary>
        /// Check for the status of a current Upload.
        /// </summary>
        /// <param name="key">An upload key, obtained using the upload methods.</param>
        /// <returns></returns>
        Task<MediaFireUploadDetails> PollUpload(string key);

        /// <summary>
        /// Returns the necessary information to perform an upload to mediafire. This method is used to enable the possibility of using third party upload frameworks without the necessity of implementing MediaFire authentication and session management.
        /// </summary>
        /// <param name="fileName">The name of the file to be created.</param>
        /// <param name="size">The size of the file, in bytes.</param>
        /// <param name="folderKey">The destination folder to store the file. If it's not passed, then the file will be stored in the root folder.</param>
        /// <param name="actionOnDuplicate">Specifies the action to take when the file already exists, by name, in the destination folder. skip ignores the upload, keep uploads the file and makes the file name unique by appending a number to it, and replace overwrites the old file, possibly adding to its version history.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        /// <returns></returns>
        Task<MediaFireUploadConfiguration> GetUploadConfiguration(
            string fileName,
            long size,
            string folderKey = null,
            MediaFireActionOnDuplicate actionOnDuplicate = MediaFireActionOnDuplicate.Keep,
            DateTime? modificationTime = null
            );

        /// <summary>
        /// Returns the upload info, of an upload performed outside the sdk.
        /// </summary>
        /// <param name="content">The raw json returned at the end of the upload configuration.</param>
        /// <returns></returns>
        Task<MediaFireUploadDetails> ProcessUploadResponse(string content);
    }
}
