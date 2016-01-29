using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Model.Responses;
using Newtonsoft.Json;

namespace MediaFireSDK.Model
{
    public class MediaFireUploadDetails
    {
        public int Result { get; set; }
        public string Key { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public string FileError { get; set; }
        public string QuickKey { get; set; }
        public string Hash { get; set; }
        public string FileName { get; set; }

        [JsonProperty("size")]
        internal long? SizeContainer { get; set; }
        public long Size { get { return SizeContainer ?? 0; } }
        public DateTime Created { get; set; }
        public string Revision { get; set; }

        public bool IsComplete { get { return Status == MediaFireUploadStatus.NoMoreRequestsForThisKey; } }
        public bool IsSuccess { get { return Result == MediaFireUploadResult.Success; } }



        [JsonProperty("created_utc")]
        public string CreatedUtc { get; set; }
    }

}
