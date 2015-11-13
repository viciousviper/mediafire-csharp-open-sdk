using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaFireSDK.Http;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Responses;

namespace MediaFireSDK.Core
{
    internal abstract class MediaFireApiBase
    {
        protected MediaFireApiConfiguration Configuration { get; private set; }
        protected MediaFireRequestController RequestController { get; private set; }

        protected MediaFireApiBase(MediaFireRequestController requestController)
        {
            RequestController = requestController;
        }

        protected MediaFireApiBase(MediaFireRequestController requestController, MediaFireApiConfiguration configuration)
        {
            Configuration = configuration;
            RequestController = requestController;
        }

        public async Task<T> Post<T>(string path, bool authenticate = true) where T : MediaFireResponseBase
        {
            return await RequestController.Post<T>(await RequestController.CreateHttpRequest(path, authenticate));
        }

        public async Task<T> Get<T>(string path, bool authenticate = true) where T : MediaFireResponseBase
        {
            return await RequestController.Get<T>(await RequestController.CreateHttpRequest(path, authenticate));
        }

      
    }
}
