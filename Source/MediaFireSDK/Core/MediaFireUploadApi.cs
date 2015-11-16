using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaFireSDK.Http;
using MediaFireSDK.Model;
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
            var requestConfig = await RequestController.CreateHttpRequest(ApiUploadMethods.PollUpload);

            requestConfig.Parameter(ApiParameters.Key, key);

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

            var requestConfig = await RequestController.CreateHttpRequest(ApiUploadMethods.Simple);

            ConfigureForSimpleUpload(requestConfig, folderKey, actionOnDuplicate, modificationTime);

            descriptor.RequestUrl = requestConfig.GetConfiguredPath();

            descriptor.RequestHeaders.Add(new KeyValuePair<string, string>(MediaFireApiConstants.FileNameHeader, fileName));
            descriptor.RequestHeaders.Add(new KeyValuePair<string, string>(MediaFireApiConstants.FileSizeHeader, size.ToString()));
            descriptor.RequestHeaders.Add(new KeyValuePair<string, string>(MediaFireApiConstants.ContentTypeHeader, MediaFireApiConstants.SimpleUploadContentTypeValue));

            return descriptor;
        }

        public Task<MediaFireUploadDetails> ProcessUploadResponse(string content)
        {
            var uploadInfo = RequestController.DeserializeObject<UploadResponse>(content).DoUpload;
            return PollUpload(uploadInfo.Key);
        }

        internal static void ConfigureForSimpleUpload(
            HttpRequestConfiguration requestConfig,
            string folderKey = null,
            MediaFireActionOnDuplicate actionOnDuplicate = MediaFireActionOnDuplicate.Keep,
            DateTime? modificationTime = null
            )
        {
            requestConfig
                .Parameter(ApiParameters.FolderKey, folderKey)
                .Parameter(ApiParameters.ActionOnDuplicate, actionOnDuplicate.ToApiParamenter())
                .Parameter(ApiParameters.ModificationTime, modificationTime);
        }
    }
}
