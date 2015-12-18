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
using Newtonsoft.Json;

namespace MediaFireSDK.Core
{
    class MediaFireUploadApi : MediaFireApiBase, IMediaFireUploadApi
    {
        public MediaFireUploadApi(MediaFireRequestController requestController)
            : base(requestController)
        {
        }

        public MediaFireUploadApi(MediaFireRequestController requestController, MediaFireApiConfiguration configuration)
            : base(requestController, configuration)
        {
        }

        public async Task<MediaFireUploadDetails> PollUpload(string key)
        {
            var requestConfig = await RequestController.CreateHttpRequest(MediaFireApiUploadMethods.PollUpload);
            requestConfig.Parameter(MediaFireApiParameters.Key, key);

            var response = await RequestController.Get<UploadResponse>(requestConfig);

            response.DoUpload.Key = key;
            return response.DoUpload;
        }


        public async Task<MediaFireUploadConfiguration> GetUploadConfiguration(
            string fileName,
            long size,
            string folderKey = null,
            MediaFireActionOnDuplicate actionOnDuplicate = MediaFireActionOnDuplicate.Keep,
            DateTime? modificationTime = null)
        {
            var descriptor = new MediaFireUploadConfiguration();

            var requestConfig = await RequestController.CreateHttpRequest(MediaFireApiUploadMethods.Simple);

            ConfigureForSimpleUpload(requestConfig, folderKey, actionOnDuplicate, modificationTime);

            descriptor.Endpoint = requestConfig.GetConfiguredPath();
            descriptor.RequestUrl = RequestController.RemoveBaseUrlFromUrl(descriptor.Endpoint);

            descriptor.RequestHeaders.Add(new KeyValuePair<string, string>(MediaFireApiConstants.FileNameHeader, fileName));
            descriptor.RequestHeaders.Add(new KeyValuePair<string, string>(MediaFireApiConstants.FileSizeHeader, size.ToString()));
            descriptor.RequestHeaders.Add(new KeyValuePair<string, string>(MediaFireApiConstants.ContentTypeHeader, MediaFireApiConstants.SimpleUploadContentTypeValue));

            descriptor.Size = size;
            return descriptor;
        }

        public Task<MediaFireUploadDetails> ProcessUploadResponse(string content)
        {
            var uploadInfo = RequestController.DeserializeObject<UploadResponse>(content).DoUpload;
            return PollUpload(uploadInfo.Key);
        }

        private void ConfigureForSimpleUpload(
            HttpRequestConfiguration requestConfig,
            string folderKey = null,
            MediaFireActionOnDuplicate actionOnDuplicate = MediaFireActionOnDuplicate.Keep,
            DateTime? modificationTime = null
            )
        {
            requestConfig
                .Parameter(MediaFireApiParameters.FolderKey, folderKey)
                .Parameter(MediaFireApiParameters.ActionOnDuplicate, actionOnDuplicate.ToApiParamenter())
                .Parameter(MediaFireApiParameters.ModificationTime, modificationTime);
        }

        public async Task<MediaFireUploadCheckDetails> Check(string fileName, long size = 0, string deviceId = null, string hash = null, string folderKey = null, bool resumable = false)
        {
            var requestConfig = await RequestController.CreateHttpRequest(MediaFireApiUploadMethods.Check);

            if (string.IsNullOrEmpty(fileName) && (size == 0 || string.IsNullOrEmpty(hash)))
            {
                throw new MediaFireException(MediaFireErrorMessages.CheckParamsError);
            }
            else
                requestConfig.Parameter(MediaFireApiParameters.FileName, fileName);

            if (string.IsNullOrEmpty(hash) == false)
            {
                requestConfig.Parameter(MediaFireApiParameters.Hash, hash);
            }

            if(size != 0)
                requestConfig.Parameter(MediaFireApiParameters.Size, size);

            requestConfig
                .Parameter(MediaFireApiParameters.DeviceId, deviceId)
                .Parameter(MediaFireApiParameters.FolderKey, folderKey)
                .Parameter(MediaFireApiParameters.Resumable, resumable.ToMediaFireYesNo());

            return await RequestController.Get<MediaFireUploadCheckDetails>(requestConfig);
        }

        public async Task<MediaFireUploadDetails> Simple(MediaFireUploadConfiguration uploadConfiguration, Stream content, IProgress<MediaFireOperationProgress> progress = null,
            CancellationToken? tokenParam = null)
        {
            var token = tokenParam ?? CancellationToken.None;

            if (progress == null)
                progress = new Progress<MediaFireOperationProgress>();

            var requestConfig = await RequestController.CreateHttpRequestForFullPath(uploadConfiguration.Endpoint, isChunkedOperation: true);

            requestConfig
                .Content(content, true)
                .WithProgress(progress, new MediaFireOperationProgress { TotalSize = uploadConfiguration.Size }, token);

            foreach (var headers in uploadConfiguration.RequestHeaders)
            {
                requestConfig.ContentHeader(headers.Key, headers.Value);
            }



            var res = await RequestController.Post<UploadResponse>(requestConfig);

            if (res.DoUpload.IsSuccess == false)
                throw new MediaFireException(string.Format(MediaFireErrorMessages.UploadErrorFormat, res.DoUpload.Result));


            var uploadInfo = await PollUpload(res.DoUpload.Key);

            uploadInfo.Key = res.DoUpload.Key;

            return uploadInfo;
        }
    }
}
