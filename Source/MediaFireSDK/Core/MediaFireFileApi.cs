using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaFireSDK.Http;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Errors;
using MediaFireSDK.Model.Responses;

namespace MediaFireSDK.Core
{
    class MediaFireFileApi : MediaFireApiBase, IMediaFireFileApi
    {
        private readonly MediaFireAgent _mediaFireAgent;

        public MediaFireFileApi(MediaFireRequestController requestController, MediaFireApiConfiguration configuration, MediaFireAgent mediaFireAgent)
            : base(requestController, configuration)
        {
            _mediaFireAgent = mediaFireAgent;
        }

        public async Task Download(string quickKey, Stream destination, CancellationToken token, IProgress<MediaFireOperationProgress> progress = null)
        {
            var links = await GetLinks(quickKey, MediaFireLinkType.DirectDownload);
            if (links.Count != 0 && string.IsNullOrEmpty(links[0].DirectDownload))
                throw new MediaFireException(MediaFireErrorMessages.FileMustContainADirectLink);


            var fileLink = links[0].DirectDownload;

            if (progress == null)
                progress = new Progress<MediaFireOperationProgress>();


            var requestConfig = await RequestController.CreateHttpRequestForFullPath(fileLink, isChunkedOperation: true);

            requestConfig
                .WithProgress(progress, new MediaFireOperationProgress(), token)
                .Content(destination, false);


            await RequestController.Get<EmptyResponse>(requestConfig);
        }



        public Task Download(string quickKey, Stream destination, IProgress<MediaFireOperationProgress> progress = null)
        {
            return Download(quickKey, destination, CancellationToken.None, progress);
        }

      

        public async Task<MediaFireUploadDetails> Upload(
            Stream fileStream,
            string fileName,
            CancellationToken token,
            long size = 0,
            string folderKey = null,
            IProgress<MediaFireOperationProgress> progress = null,
            MediaFireActionOnDuplicate actionOnDuplicate = MediaFireActionOnDuplicate.Keep,
            DateTime? modificationTime = null)
        {
            size = size == 0 ? fileStream.Length : size;

            if (progress == null)
                progress = new Progress<MediaFireOperationProgress>();

            var requestConfig = await RequestController.CreateHttpRequest(ApiUploadMethods.Simple, isChunkedOperation: true);

            MediaFireUploadApi.ConfigureForSimpleUpload(requestConfig, folderKey, actionOnDuplicate, modificationTime);

            requestConfig
                .Content(fileStream, true)
                .ContentHeader(MediaFireApiConstants.FileNameHeader, fileName)
                .ContentHeader(MediaFireApiConstants.FileSizeHeader, size.ToString())
                .ContentHeader(MediaFireApiConstants.ContentTypeHeader, MediaFireApiConstants.SimpleUploadContentTypeValue)
                .WithProgress(progress, new MediaFireOperationProgress { TotalSize = size }, token);


            var res = await RequestController.Post<UploadResponse>(requestConfig);

            if (res.DoUpload.IsSuccess == false)
                throw new MediaFireException(string.Format(MediaFireErrorMessages.UploadErrorFormat, res.DoUpload.Result));


            var uploadInfo = await _mediaFireAgent.Upload.PollUpload(res.DoUpload.Key);

            uploadInfo.Key = res.DoUpload.Key;

            return uploadInfo;
        }



        public async Task<MediaFireFile> GetInfo(string quickKey)
        {
            var requestConfig = await GetFileHttpRequest(quickKey, ApiFileMethods.GetInfo);
            return (await RequestController.Get<GetFileInfoResponse>(requestConfig)).FileInfo;
        }

        public async Task<MediaFireLinkCollection> GetLinks(string quickKey, MediaFireLinkType linkType)
        {
            var requestConfig = await GetFileHttpRequest(quickKey, ApiFileMethods.GetLinks);
            requestConfig.Parameter(ApiParameters.LinkType, linkType.ToApiParameter());

            var response = await RequestController.Get<GetLinksResponse>(requestConfig);

            var col = new MediaFireLinkCollection(response.Links)
            {
                DirectDownloadFreeBandwidth = response.DirectDownloadFreeBandwidth,
                OneTimeDownloadRequestCount = response.OneTimeDownloadRequestCount,
                OneTimeKeyRequestCount = response.OneTimeKeyRequestCount,
                OneTimeKeyRequestMaxCount = response.OneTimeKeyRequestMaxCount
            };

            return col;
        }

        public async Task Update(string quickKey, string fileName = null, string description = null, bool truncate = false,
            DateTime? modificationTime = null, MediaFirePrivacy? privacy = null)
        {
            var requestConfig = await GetFileHttpRequest(quickKey, ApiFileMethods.Update);
            requestConfig
                .Parameter(ApiParameters.FileName, fileName)
                .Parameter(ApiParameters.Description, description)
                .Parameter(ApiParameters.Truncate, truncate.ToMediaFireYesNo())
                .Parameter(ApiParameters.ModificationTime, modificationTime)
                .Parameter(ApiParameters.Privacy, privacy.ToApiParameter());

            await RequestController.Post<EmptyResponse>(requestConfig);
        }

        public async Task Delete(string quickKey)
        {
            var requestConfig = await GetFileHttpRequest(quickKey, ApiFileMethods.Delete);
            await RequestController.Post<EmptyResponse>(requestConfig);
        }

        public async Task Move(string quickKey, string folderKey)
        {
            var requestConfig = await GetFileHttpRequest(quickKey, ApiFileMethods.Move);
            requestConfig.Parameter(ApiParameters.FolderKey, folderKey);
            //todo change this to receive new names.
            await requestConfig.Post<EmptyResponse>();
        }


        private async Task<HttpRequestConfiguration> GetFileHttpRequest(string quickKey, string path)
        {
            var requestConfig = await RequestController.CreateHttpRequest(path);
            requestConfig.Parameter(ApiParameters.QuickKey, quickKey);
            return requestConfig;
        }
    }
}
