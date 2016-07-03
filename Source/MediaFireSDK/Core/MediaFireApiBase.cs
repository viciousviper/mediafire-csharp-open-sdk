using System;
using System.Threading.Tasks;
using MediaFireSDK.Http;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Responses;

namespace MediaFireSDK.Core
{
    internal abstract class MediaFireApiBase
    {
        protected MediaFireApiConfiguration Configuration { get; }

        protected MediaFireRequestController RequestController { get; }

        protected MediaFireApiBase(MediaFireRequestController requestController)
        {
            RequestController = requestController;
        }

        protected MediaFireApiBase(MediaFireRequestController requestController, MediaFireApiConfiguration configuration)
        {
            Configuration = configuration;
            RequestController = requestController;
        }

        public async Task<T> Post<T>(string path, bool authenticate = true)
            where T : MediaFireResponseBase
        {
            return await RequestController.Post<T>(await RequestController.CreateHttpRequest(path, authenticate: authenticate));
        }

        public async Task<T> Get<T>(string path, bool authenticate = true)
            where T : MediaFireResponseBase
        {
            return await RequestController.Get<T>(await RequestController.CreateHttpRequest(path, authenticate: authenticate));
        }
    }
}
