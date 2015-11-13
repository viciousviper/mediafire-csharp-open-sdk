using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Http;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Responses;

namespace MediaFireSDK.Core
{
    internal class MediaFireFolderApi : MediaFireApiBase, IMediaFireFolderApi
    {
        public MediaFireFolderApi(MediaFireRequestController requestController)
            : base(requestController)
        {
        }
        public async Task<MediaFireFolderContent> GetFolderContent(
            string folderKey,
            MediaFireFolderContentType contentType = MediaFireFolderContentType.Folders,
            MediaFireOrderDirection orderDirection = MediaFireOrderDirection.Asc,
            MediaFireOrderQuery orderQuery = MediaFireOrderQuery.Name,
            int chunk = 1,
            int chunkSize = 100,
            bool details = false,
            string deviceId = null,
            params MediaFireContentFilter[] filters)
        {
            var requestConfig = await RequestController.CreateHttpRequest(ApiFolderMethods.GetContent);

            requestConfig
                .Parameter(ApiParameters.FolderKey, folderKey)
                .Parameter(ApiParameters.ContentType, GetContentTypeString(contentType))
                .Parameter(ApiParameters.Filter, GetFilterString(filters))
                .Parameter(ApiParameters.DeviceId, deviceId)
                .Parameter(ApiParameters.OrderBy, orderQuery.ToString().ToLower())
                .Parameter(ApiParameters.OrderDirection, orderDirection.ToString().ToLower())
                .Parameter(ApiParameters.Chunk, chunk)
                .Parameter(ApiParameters.ChunkSize, chunkSize)
                .Parameter(ApiParameters.Details, details.ToMediaFireYesNo())
                ;

            var response = await RequestController.Get<GetContentResponse>(requestConfig);

            return response.FolderContent;

        }

        public async Task Update(string folderKey, string folderName = null, string description = null, DateTime? modificationTime = null,
            MediaFirePrivacy? privacy = null, bool? privacyRecursive = null)
        {
            var requestConfig = await RequestController.CreateHttpRequest(ApiFolderMethods.Update);
            requestConfig
              .Parameter(ApiParameters.FolderKey, folderKey)
              .Parameter(ApiParameters.FolderName, folderName)
              .Parameter(ApiParameters.Description, description)
              .Parameter(ApiParameters.ModificationTime, modificationTime)
              .Parameter(ApiParameters.Privacy, privacy.ToApiParameter())
              .Parameter(ApiParameters.PrivacyRecursive, privacyRecursive.ToMediaFireYesNo());

            await RequestController.Post<EmptyResponse>(requestConfig);
        }

        public async Task Delete(string folderKey)
        {
            var requestConfig = await RequestController.CreateHttpRequest(ApiFolderMethods.Delete);
            requestConfig.Parameter(ApiParameters.FolderKey, folderKey);
            await RequestController.Post<EmptyResponse>(requestConfig);
        }

        public async Task Move(string folderKey, string folderKeyDst)
        {
            var requestConfig = await RequestController.CreateHttpRequest(ApiFolderMethods.Move);
            requestConfig.Parameter(ApiParameters.FolderKeySource, folderKey);
            requestConfig.Parameter(ApiParameters.FolderKeyDestination, folderKeyDst);
            await RequestController.Post<EmptyResponse>(requestConfig);
        }



        public async Task<string> Create(
            string folderName,
            string parentKey = null,
            MediaFireActionOnDuplicate actionOnDuplicate = MediaFireActionOnDuplicate.Keep,
            DateTime? modificationTime = null)
        {
            var requestConfig = await RequestController.CreateHttpRequest(ApiFolderMethods.Create);
            requestConfig
                .Parameter(ApiParameters.FolderName, folderName)
                .Parameter(ApiParameters.ParentKey, parentKey)
                .Parameter(ApiParameters.ActionOnDuplicate, actionOnDuplicate.ToApiParamenter())
                .Parameter(ApiParameters.ModificationTime, modificationTime);

            var resp = await RequestController.Post<CreateFolderResponse>(requestConfig);

            return resp.FolderKey;

        }

        public async Task<MediaFireFolder> GetInfo(string folderKey, string deviceId = null, bool details = false)
        {
            var requestConfig = await RequestController.CreateHttpRequest(ApiFolderMethods.GetInfo);
            requestConfig
              .Parameter(ApiParameters.FolderKey, folderKey)
              .Parameter(ApiParameters.DeviceId, deviceId)
              .Parameter(ApiParameters.Details, details.ToMediaFireYesNo())
              ;

            var response = await RequestController.Get<GetFolderInfoResponse>(requestConfig);

            return response.FolderInfo;
        }

        public async Task<IEnumerable<MediaFireItem>> Search(
            string searchText,
            string folderKey = null,
            string deviceId = null,
            bool searchAll = false,
            bool details = false,
            params MediaFireContentFilter[] filters)
        {
            var requestConfig = await RequestController.CreateHttpRequest(ApiFolderMethods.Search);

            requestConfig
                .Parameter(ApiParameters.SearchText, searchText)
                .Parameter(ApiParameters.FolderKey, folderKey)
                .Parameter(ApiParameters.Filter, GetFilterString(filters))
                .Parameter(ApiParameters.DeviceId, deviceId)
                .Parameter(ApiParameters.SearchAll, searchAll.ToMediaFireYesNo())
                .Parameter(ApiParameters.Details, details.ToMediaFireYesNo())
                ;

            var response = await RequestController.Get<SearchResponse>(requestConfig);

            return response.Results.Select(Convert).ToList();
        }

       
        private string GetFilterString(MediaFireContentFilter[] filters)
        {
            if (filters == null || filters.Length == 0)
                return string.Empty;

            return string.Join(",", filters);
        }

        private string GetContentTypeString(MediaFireFolderContentType contentType)
        {
            return contentType == MediaFireFolderContentType.Files
                ? ApiParameters.ContentTypeFileType
                : ApiParameters.ContentTypeFolderType;
        }

        private MediaFireItem Convert(SearchResultItem searchItem)
        {
            MediaFireItem item = null;
            if (string.Equals(searchItem.Type, MediaFireApiConstants.MediaFireFolder, StringComparison.OrdinalIgnoreCase))
            {
                var folder = new MediaFireFolder();
                item = folder;

                folder.FolderKey = searchItem.FolderKey;
                folder.Name = searchItem.Name;
                folder.TotalFolders = searchItem.TotalFolders;
                folder.TotalFiles = searchItem.TotalFiles;
                folder.TotalSize = searchItem.TotalSize;
            }
            else if (string.Equals(searchItem.Type, MediaFireApiConstants.MediaFireFile, StringComparison.OrdinalIgnoreCase))
            {
                var file = new MediaFireFile();
                item = file;
                file.QuickKey = searchItem.QuickKey;
                file.Size = searchItem.Size;
                file.Name = searchItem.FileName;
                file.PasswordProtected = searchItem.PasswordProtected;
                file.MimeType = searchItem.MimeType;
                file.FileType = searchItem.FileType;
                file.View = searchItem.View;
                file.Edit = searchItem.Edit;
                file.Hash = searchItem.Hash;
                file.Flag = searchItem.Flag;
            }

            FillGenericProperties(item, searchItem);
            return item;
        }

        public void FillGenericProperties(MediaFireItem item, SearchResultItem searchResult)
        {
            item.Created = searchResult.Created;
            item.CreatedUtc = searchResult.CreatedUtc;
            item.Revision = searchResult.Revision;
            item.Privacy = searchResult.Privacy;
            item.Description = searchResult.Description;
            item.Flag = searchResult.Flag;
            item.ParentName = searchResult.ParentName;
            item.ParentFolderKey = searchResult.ParentFolderkey;
            item.Relevancy = searchResult.Relevancy;
        }


    }
}
