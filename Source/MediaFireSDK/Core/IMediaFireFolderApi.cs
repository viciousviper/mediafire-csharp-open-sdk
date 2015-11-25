using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Model;

namespace MediaFireSDK.Core
{
    public interface IMediaFireFolderApi
    {
        /// <summary>
        /// Returns a collection of top-level folders or files for target folder.
        /// </summary>
        /// <param name="folderKey">A folder key.</param>
        /// <param name="contentType">Specifies the type of content to return.</param>
        /// <param name="orderDirection">Specifies the sort order of the results.</param>
        /// <param name="orderQuery">The order query.</param>
        /// <param name="chunkSize">The number of items to include in each chunk returned.</param>
        /// <param name="details">Specifies additional folder information to return in the results.</param>
        /// <param name="deviceId">An integer specifying on which user device to look for data.</param>
        /// <param name="filters">Filter by privacy and/or by filetype.</param>
        /// <param name="chunk">Specifies which segment of the results to return starting from 1.</param>
        /// <returns>The folder content for the selected <paramref name="contentType"/> with the respective <paramref name="folderKey"/>.</returns>
        Task<MediaFireFolderContent> GetFolderContent(
            string folderKey,
            MediaFireFolderContentType contentType = MediaFireFolderContentType.Folders,
            MediaFireOrderDirection orderDirection = MediaFireOrderDirection.Asc,
            MediaFireOrderQuery orderQuery = MediaFireOrderQuery.Name,
            int chunk = 1,
            int chunkSize = 100,
            bool details = false,
            string deviceId = null,
            params MediaFireContentFilter[] filters);

        /// <summary>
        /// Update a folder's information.
        /// </summary>
        /// <param name="folderKey">A folder key.</param>
        /// <param name="folderName">The name of the folder.</param>
        /// <param name="description">The description of the folder.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        /// <param name="privacy">Privacy of the folder ('public' or 'private').</param>
        /// <param name="privacyRecursive">Whether or not applying 'privacy' to sub-folders.</param>
        Task Update(string folderKey, string folderName = null, string description = null, DateTime? modificationTime = null, MediaFirePrivacy? privacy = null, bool? privacyRecursive = null);

        /// <summary>
        /// Deletes a folder.
        /// </summary>
        /// <param name="folderKey">The key that identifies the folder to be deleted.</param>
        Task Delete(string folderKey);

        /// <summary>
        /// Move one folder to a target destination.
        /// </summary>
        /// <param name="folderKey">A folder key.</param>
        /// <param name="folderKeyDst">The destination folderkey.</param>
        Task Move(string folderKey, string folderKeyDst);

        /// <summary>
        /// Creates a folder in a specified target destination. 
        /// </summary>
        /// <param name="folderName">The name of the folder to be created.</param>
        /// <param name="parentKey">The destination folder key.</param>
        /// <param name="actionOnDuplicate">Specifies the action to take when the file already exists, by name, in the destination folder. skip ignores the upload, keep uploads the file and makes the file name unique by appending a number to it, and replace overwrites the old file, possibly adding to its version history.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        /// <returns>The key of created folder</returns>
        Task<string> Create(string folderName, string parentKey = null, MediaFireActionOnDuplicate actionOnDuplicate = MediaFireActionOnDuplicate.Keep, DateTime? modificationTime = null);

        /// <summary>
        /// Returns the folder details.
        /// </summary>
        /// <param name="folderKey">The key that identifies a folder.</param>
        /// <param name="deviceId">An integer specifying on which user device to look for data.</param>
        /// <param name="details">Specifies additional folder information to return in the results.</param>
        Task<MediaFireFolder> GetInfo(string folderKey, string deviceId = null, bool details = false);

        /// <summary>
        /// Searches the the content of a given folder. If <paramref name="folderKey"/> is not passed, then the search will be performed on the root folder ("myfiles").
        /// </summary>
        /// <param name="searchText">The search keywords to look for in filenames, folder names, and descriptions. The string must be at least 3 characters in length.</param>
        /// <param name="folderKey">A folder key.</param>
        /// <param name="deviceId">An integer specifying on which user device to look for data.</param>
        /// <param name="searchAll">If <paramref name="folderKey"/> is not passed, <paramref name="searchAll"/> can be used to indicate whether to search the root folder only or the entire device</param>
        /// <param name="details">Specifies additional folder information to return in the results.</param>
        /// <param name="filters">Filter by privacy and/or by filetype.</param>
        Task<IEnumerable<MediaFireItem>> Search(string searchText, string folderKey = null, string deviceId = null, bool searchAll = false, bool details = false, params MediaFireContentFilter[] filters);
    }

    public static class MediaFireFolderApiExtensions
    {
        /// <summary>
        /// Returns a collection of top-level folders or files for the root folder.
        /// </summary>
        /// <param name="contentType">Specifies the type of content to return.</param>
        /// <returns>The folder content for the selected <paramref name="contentType"/>.</returns>
        public static Task<MediaFireFolderContent> GetRootFolderContent(this IMediaFireFolderApi folderApi, MediaFireFolderContentType contentType)
        {
            return folderApi.GetFolderContent(string.Empty, contentType);
        }

        /// <summary>
        /// Return an url of <paramref name="folder"/>
        /// </summary>
        /// <param name="folder">A folder</param>
        public static string GetFolderPublicUrl(this MediaFireFolder folder)
        {
            return string.Format(MediaFireApiConstants.PublicFolderUrlFormat, folder.Key);
        }

        /// <summary>
        /// Changes the privacy.
        /// </summary>
        /// <param name="folderKey">A folder key.</param>
        /// <param name="newPrivacy">The desired privacy.</param>
        /// <param name="privacyRecursive">Whether or not applying 'privacy' to sub-folders.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        public static Task ChangePrivacy(this IMediaFireFolderApi folderApi, string folderKey, MediaFirePrivacy newPrivacy, bool privacyRecursive = false, DateTime? modificationTime = null)
        {
            return folderApi.Update(folderKey, privacy: newPrivacy, privacyRecursive: privacyRecursive, modificationTime: modificationTime);
        }

        /// <summary>
        /// Changes the privacy.
        /// </summary>
        /// <param name="folder">A folder.</param>
        /// <param name="newPrivacy">The desired privacy.</param>
        /// <param name="privacyRecursive">Whether or not applying 'privacy' to sub-folders.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        public static Task ChangePrivacy(this IMediaFireFolderApi folderApi, MediaFireFolder folder, MediaFirePrivacy newPrivacy, bool privacyRecursive = false, DateTime? modificationTime = null)
        {
            return folderApi.ChangePrivacy(folder.FolderKey, newPrivacy, privacyRecursive, modificationTime);
        }

        /// <summary>
        /// Renames a folder.
        /// </summary>
        /// <param name="folderKey">A folder key.</param>
        /// <param name="newName">The new folder name.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        public static Task Rename(this IMediaFireFolderApi folderApi, string folderKey, string newName, DateTime? modificationTime = null)
        {
            return folderApi.Update(folderKey, folderName: newName, modificationTime: modificationTime);
        }

        /// <summary>
        /// Renames a folder.
        /// </summary>
        /// <param name="folder">A folder.</param>
        /// <param name="newName">The new folder name.</param>
        /// <param name="modificationTime">The date/time of the update. If not set, the current server time will be used.</param>
        public static Task Rename(this IMediaFireFolderApi folderApi, MediaFireFolder folder, string newName, DateTime? modificationTime = null)
        {
            return folderApi.Rename(folder.FolderKey, newName, modificationTime);
        }

        /// <summary>
        /// Deletes a folder.
        /// </summary>
        /// <param name="folder">A Folder.</param>
        public static Task Delete(this IMediaFireFolderApi folderApi, MediaFireFolder folder)
        {
            return folderApi.Delete(folder.FolderKey);
        }


        /// <summary>
        /// Move one folder to a target destination.
        /// </summary>
        /// <param name="folder">A folder.</param>
        /// <param name="destination">The destination folder.</param>
        public static Task Move(this IMediaFireFolderApi folderApi, MediaFireFolder folder, MediaFireFolder destination)
        {
            return folderApi.Move(folder.FolderKey, destination.FolderKey);
        }
    }

}
