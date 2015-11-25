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

    public interface IMediaFireFileApi
    {
        /// <summary>
        /// Downloads the file represented with <paramref name="quickKey"/> to <paramref name="destination"/>.
        /// </summary>
        /// <param name="quickKey">The quickkey that identifies the file.</param>
        /// <param name="destination">A stream where the content of the file will be writted.</param>
        /// <param name="token">The token to monitor for cancellation requests.</param>
        /// <param name="progress">A callback to receive progress updates.</param>
        Task Download(string quickKey, Stream destination, CancellationToken token, IProgress<MediaFireOperationProgress> progress = null);

        /// <summary>
        /// Downloads the file represented with <paramref name="quickKey"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="quickKey">The quickkey that identifies the file.</param>
        /// <param name="destination">A stream where the content of the file will be writted.</param>
        /// <param name="progress">A callback to receive progress updates.</param>
        Task Download(string quickKey, Stream destination, IProgress<MediaFireOperationProgress> progress = null);

        /// <summary>
        /// Uploads the content of <paramref name="fileStream"/> to a MediaFire file named <paramref name="fileName"/> on a folder which key is <paramref name="folderKey"/>.
        /// </summary>
        /// <param name="fileStream">The content stream to upload.</param>
        /// <param name="fileName">The name of the file to be created.</param>
        /// <param name="token">The token to monitor for cancellation requests.</param>
        /// <param name="size">The size of <paramref name="fileStream"/> in bytes. This parameter is optional when <paramref name="fileStream"/> supports the Length property.</param>
        /// <param name="folderKey">The destination folder to store the file. If it's not passed, then the file will be stored in the root folder.</param>
        /// <param name="progress">A callback to receive progress updates.</param>
        /// <param name="actionOnDuplicate">Specifies the action to take when the file already exists, by name, in the destination folder. skip ignores the upload, keep uploads the file and makes the file name unique by appending a number to it, and replace overwrites the old file, possibly adding to its version history.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        /// <returns>The upload information, if the serve is still processing the upload, the PollUpload method should be used in order to retrieve the file quickkey</returns>
        /// <example>
        /// var mfAgent = ...;
        /// var uploadInfo = await mfAgent.File.Upload(...);
        /// do
        /// {
        ///    if (uploadInfo.IsComplete)
        ///        break;
        ///
        ///    await Task.Delay(1000);
        ///
        ///    uploadInfo = await agent.Upload.PollUpload(uploadInfo.Key);
        /// } while (true);
        /// </example>
        Task<MediaFireUploadDetails> Upload(
            Stream fileStream,
            string fileName,
            CancellationToken token,
            long size = 0,
            string folderKey = null,
            IProgress<MediaFireOperationProgress> progress = null,
            MediaFireActionOnDuplicate actionOnDuplicate = MediaFireActionOnDuplicate.Keep,
            DateTime? modificationTime = null);

        /// <summary>
        /// Returns the file with <paramref name="quickKey"/> details.
        /// </summary>
        /// <param name="quickKey">The quickkey that identifies the file</param>
        Task<MediaFireFile> GetInfo(string quickKey);

        /// <summary>
        /// Returns the view link, normal download link, and, if possible, the direct download link of a file
        /// </summary>
        /// <param name="quickKey">The quickkey that identifies the file</param>
        /// <param name="linkType">Specify which link type to return. If not passed, all applicable link types are returned. view, edit, normal_download, direct_download, one_time_download, listen, watch, streaming.</param>
        Task<MediaFireLinkCollection> GetLinks(string quickKey, MediaFireLinkType linkType = MediaFireLinkType.All);

        /// <summary>
        /// Update a file's information
        /// </summary>
        /// <param name="quickKey">The quickkey that identifies the file. </param>
        /// <param name="fileName">The name and mimetype extensions of the file (Should have same file type as the old file). The filename should be 3 to 255 in length.</param>
        /// <param name="description">The description of the file.</param>
        /// <param name="truncate">Specifies if the content of the file should be deleted.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        /// <param name="privacy">Privacy of the file</param>
        Task Update(string quickKey, string fileName = null, string description = null, bool truncate = false, DateTime? modificationTime = null, MediaFirePrivacy? privacy = null);

        /// <summary>
        /// Moves the file to the trash can.
        /// </summary>
        /// <param name="quickKey">The quickkey that identifies the file. </param>
        Task Delete(string quickKey);

        /// <summary>
        /// Moves the file to the folder identified with <paramref name="folderKey"/>.
        /// </summary>
        /// <param name="quickKey">The quickkey that identifies the file. </param>
        /// <param name="folderKey">The destination folder key to move the file. If it's not passed, then the file will be moved to the root folder.</param>
        Task Move(string quickKey, string folderKey);
    }

    public static class MediaFireFileApiExtensions
    {
        /// <summary>
        /// Downloads the file represented with <paramref name="file"/> to <paramref name="destination"/>.
        /// </summary>
        /// <param name="file">The file to download.</param>
        /// <param name="destination">A stream where the content of the file will be writted.</param>
        /// <param name="token">The token to monitor for cancellation requests.</param>
        /// <param name="progress">A callback to receive progress updates.</param>
        public static Task Download(this IMediaFireFileApi fileApi, MediaFireFile file, Stream destination, CancellationToken token, IProgress<MediaFireOperationProgress> progress = null)
        {
            return fileApi.Download(file.QuickKey, destination, token, progress);
        }

        /// <summary>
        /// Downloads the file represented with <paramref name="file"/> to <paramref name="destination"/>.
        /// </summary>
        /// <param name="file">The file to download.</param>
        /// <param name="destination">A stream where the content of the file will be writted.</param>
        /// <param name="progress">A callback to receive progress updates.</param>
        public static Task Download(this IMediaFireFileApi fileApi, MediaFireFile file, Stream destination, IProgress<MediaFireOperationProgress> progress = null)
        {
            return fileApi.Download(file, destination, CancellationToken.None, progress);
        }

        /// <summary>
        /// Uploads the content of <paramref name="fileStream"/> to a MediaFire file named <paramref name="fileName"/> on a folder which key is <paramref name="folderKey"/>.
        /// </summary>
        /// <param name="fileStream">The content stream to upload.</param>
        /// <param name="fileName">The name of the file to be created.</param>
        /// <param name="size">The size of <paramref name="fileStream"/> in bytes. This parameter is optional when <paramref name="fileStream"/> supports the Length property.</param>
        /// <param name="folderKey">The destination folder to store the file. If it's not passed, then the file will be stored in the root folder.</param>
        /// <param name="progress">A callback to receive progress updates.</param>
        /// <param name="actionOnDuplicate">Specifies the action to take when the file already exists, by name, in the destination folder. skip ignores the upload, keep uploads the file and makes the file name unique by appending a number to it, and replace overwrites the old file, possibly adding to its version history.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        /// <returns>The upload information</returns>
        /// <example>
        /// var mfAgent = ...;
        /// var uploadInfo = await mfAgent.File.Upload(...);
        /// do
        /// {
        ///    if (uploadInfo.IsComplete)
        ///        break;
        ///
        ///    await Task.Delay(1000);
        ///
        ///    uploadInfo = await agent.Upload.PollUpload(uploadInfo.Key);
        /// } while (true);
        /// </example>
        public static Task<MediaFireUploadDetails> Upload(
            this IMediaFireFileApi fileApi,
            Stream fileStream,
            string fileName,
            long size = 0,
            string folderKey = null,
            IProgress<MediaFireOperationProgress> progress = null,
            MediaFireActionOnDuplicate actionOnDuplicate = MediaFireActionOnDuplicate.Keep,
            DateTime? modificationTime = null)
        {
            return fileApi.Upload(fileStream, fileName, CancellationToken.None, size, folderKey, progress, actionOnDuplicate, modificationTime);
        }

        /// <summary>
        /// Returns the view link, normal download link, and, if possible, the direct download link of a file.
        /// </summary>
        /// <param name="quickKey">The quickkey that identifies the file.</param>
        public static Task<MediaFireLinkCollection> GetLinks(this IMediaFireFileApi fileApi, string quickKey)
        {
            return fileApi.GetLinks(quickKey, MediaFireLinkType.All);
        }

        /// <summary>
        /// Changes the privacy.
        /// </summary>
        /// <param name="quickKey">The quickkey that identifies the file.</param>
        /// <param name="newPrivacy">The desired privacy.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        public static Task ChangePrivacy(this IMediaFireFileApi fileApi, string quickKey, MediaFirePrivacy newPrivacy, DateTime? modificationTime = null)
        {
            return fileApi.Update(quickKey, privacy: newPrivacy, modificationTime: modificationTime);
        }

        /// <summary>
        /// Changes the file privacy.
        /// </summary>
        /// <param name="file">The file to change the privacy.</param>
        /// <param name="newPrivacy">The desired privacy.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        public static Task ChangePrivacy(this IMediaFireFileApi fileApi, MediaFireFile file, MediaFirePrivacy newPrivacy, DateTime? modificationTime = null)
        {
            return fileApi.ChangePrivacy(file.QuickKey, newPrivacy, modificationTime);
        }

        /// <summary>
        /// Renames a file.
        /// </summary>
        /// <param name="quickKey">The quickkey that identifies the file.</param>
        /// <param name="newName">The new file name.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        public static Task Rename(this IMediaFireFileApi fileApi, string quickKey, string newName, DateTime? modificationTime = null)
        {
            return fileApi.Update(quickKey, fileName: newName, modificationTime: modificationTime);
        }

        /// <summary>
        /// Renames a file.
        /// </summary>
        /// <param name="file">The file to change the privacy.</param>
        /// <param name="newName">The new file name.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        public static Task Rename(this IMediaFireFileApi fileApi, MediaFireFile file, string newName, DateTime? modificationTime = null)
        {
            return fileApi.Rename(file.QuickKey, newName, modificationTime);
        }

        /// <summary>
        /// Moves the file to the trash can.
        /// </summary>
        /// <param name="file">The file.</param>
        public static Task Delete(this IMediaFireFileApi fileApi, MediaFireFile file)
        {
            return fileApi.Delete(file.QuickKey);
        }

        /// <summary>
        /// Moves the file to the folder identified with <paramref name="folderKey"/>.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="folder">The destination folder to move the file. If it's not passed, then the file will be moved to the root folder.</param>
        public static Task Move(this IMediaFireFileApi fileApi, MediaFireFile file, MediaFireFolder folder)
        {
            return fileApi.Move(file.QuickKey, folder.FolderKey);
        }
    }
}
