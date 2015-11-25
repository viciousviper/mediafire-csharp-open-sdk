using MediaFireSDK.Model.Responses;
using Newtonsoft.Json;

namespace MediaFireSDK.Model
{
    public class MediaFireUploadCheckDetails : MediaFireResponseBase
    {
        [JsonProperty("hash_exists")]
        internal string HashExistsInternal { get; set; }
        [JsonProperty("file_exists")]
        internal string FileExistsInternal { get; set; }

        public bool FileExists { get { return FileExistsInternal.FromMediaFireYesNo(); } }
        public bool HashExists { get { return HashExistsInternal.FromMediaFireYesNo(); } }

        [JsonProperty("duplicate_quickkey")]
        public string DuplicateQuickkey { get; set; }
        [JsonProperty("available_space")]
        public long AvailableSpace { get; set; }
        [JsonProperty("used_storage_size")]
        public long UsedStorageSize { get; set; }
        [JsonProperty("storage_limit")]
        public long StorageLimit { get; set; }
        [JsonProperty("storage_limit_exceeded")]
        internal string StorageLimitExceeded { get; set; }

        public bool IsStorageLimitExceeded { get { return StorageLimitExceeded.FromMediaFireYesNo(); } }
    }
}
