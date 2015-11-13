using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Core;
using MediaFireSDK.Model.Responses;
using Newtonsoft.Json;

namespace MediaFireSDK.Model
{
    public class MediaFireOneTimeLink
    {
        public string Download { get; set; }
        public string View { get; set; }
        public string Watch { get; set; }
        public string Listen { get; set; }
    }

    public class MediaFireLinks
    {
        public string QuickKey { get; set; }
        public string View { get; set; }
        public string Read { get; set; }
        public string Edit { get; set; }
        public string Watch { get; set; }
        public string Listen { get; set; }
        public string Streaming { get; set; }
        [JsonProperty("normal_download")]
        public string NormalDownload { get; set; }
        [JsonProperty("direct_download")]
        public string DirectDownload { get; set; }
        [JsonProperty("one_time")]
        public MediaFireOneTimeLink OneTime { get; set; }
    }

    public class MediaFireLinkCollection : List<MediaFireLinks>
    {
        public MediaFireLinkCollection()
        {
        }

        public MediaFireLinkCollection(IEnumerable<MediaFireLinks> collection) : base(collection)
        {
        }

        public int OneTimeDownloadRequestCount { get; set; }
        public int DirectDownloadFreeBandwidth { get; set; }
        public int OneTimeKeyRequestCount { get; set; }
        public int OneTimeKeyRequestMaxCount { get; set; }

    }

  
}
