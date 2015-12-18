using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        Task<MediaFireUploadDetails> PollUpload(string key);

        /// <summary>
        /// Returns the necessary information to perform an upload to mediafire. This method is used to enable the possibility of using third party upload frameworks without the necessity of implementing MediaFire authentication and session management.
        /// </summary>
        /// <param name="fileName">The name of the file to be created.</param>
        /// <param name="size">The size of the file, in bytes.</param>
        /// <param name="folderKey">The destination folder to store the file. If it's not passed, then the file will be stored in the root folder.</param>
        /// <param name="actionOnDuplicate">Specifies the action to take when the file already exists, by name, in the destination folder. skip ignores the upload, keep uploads the file and makes the file name unique by appending a number to it, and replace overwrites the old file, possibly adding to its version history.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
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
        Task<MediaFireUploadDetails> ProcessUploadResponse(string content);

        /// <summary>
        /// Checks if a duplicate filename exists in the destination folder and verifies folder permissions for non-owner uploads.
        /// </summary>
        /// <param name="fileName">The name of the file to be created.</param>
        /// <param name="size">The size of the file, in bytes.</param>
        /// <param name="deviceId">An integer specifying on which user device to look for data.</param>
        /// <param name="hash">The SHA256 hash of the file being uploaded</param>
        /// <param name="folderKey">The destination folder to store the file. If it's not passed, then the file will be stored in the root folder.</param>
        /// <param name="resumable">Specifies whether to make this upload resumable or not.</param>
        /// <returns></returns>
        Task<MediaFireUploadCheckDetails> Check(
            string fileName,
            long size = 0,
            string deviceId = null,
            string hash = null,
            string folderKey = null,
            bool resumable = false
            );

        /// <summary>
        /// Uploads <paramref name="content"/> to MediaFire given a <paramref name="uploadConfiguration"/> retrieved from GetUploadConfiguration.
        /// </summary>
        /// <param name="uploadConfiguration">An upload configuration previously retrieved from GetUploadConfiguration</param>
        /// <param name="content">The content stream.</param>
        /// <param name="progress">A callback to receive progress updates.</param>
        /// <param name="token">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        Task<MediaFireUploadDetails> Simple(
            MediaFireUploadConfiguration uploadConfiguration,
            Stream content,
            IProgress<MediaFireOperationProgress> progress = null,
            CancellationToken? token = null
            );

    }
}
