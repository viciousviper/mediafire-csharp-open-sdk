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
    internal class MediaFireSystemApi : MediaFireApiBase, IMediaFireSystemApi
    {
        public MediaFireSystemApi(MediaFireRequestController requestController)
            : base(requestController)
        {
        }

        public Task<MediaFireDetails> GetInfo()
        {
            return Get<MediaFireDetails>(ApiSystemMethods.GetInfo, authenticate: false);
        }
    }
}
