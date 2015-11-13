using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MediaFireSDK.Model.Responses
{
    internal class GetFileInfoResponse : MediaFireResponseBase
    {
        [JsonProperty("file_info")]
        public MediaFireFile FileInfo { get; set; }
    }

    internal class GetLinksResponse : MediaFireResponseBase
    {
        public MediaFireLinks[] Links { get; set; }
        [JsonProperty("one_time_download_request_count")]
        public int OneTimeDownloadRequestCount { get; set; }
        [JsonProperty("direct_download_free_bandwidth")]
        public int DirectDownloadFreeBandwidth { get; set; }
        [JsonProperty("one_time_key_request_count")]
        public int OneTimeKeyRequestCount { get; set; }
        [JsonProperty("one_time_key_request_max_count")]
        public int OneTimeKeyRequestMaxCount { get; set; }
    }
}
