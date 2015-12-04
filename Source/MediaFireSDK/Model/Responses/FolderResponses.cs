using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MediaFireSDK.Model.Responses
{
    internal class GetContentResponse : MediaFireResponseBase
    {
        [JsonProperty("folder_content")]
        public MediaFireFolderContent FolderContent { get; set; }
    }

    internal class CreateFolderResponse : MediaFireResponseBase
    {
        public string FolderKey { get; set; }
    }

    internal class GetFolderInfoResponse : MediaFireResponseBase
    {
        [JsonProperty("folder_info")]
        public MediaFireFolder FolderInfo { get; set; }
    }

    internal class SearchResultItem
    {
        public string Type { get; set; }
        public string QuickKey { get; set; }
        public string FileName { get; set; }
        [JsonProperty("parent_folderkey")]
        public string ParentFolderkey { get; set; }
        [JsonProperty("parent_name")]
        public string ParentName { get; set; }
        public DateTime Created { get; set; }
        public string Revision { get; set; }
        public long Size { get; set; }
        public string Description { get; set; }
        public string Privacy { get; set; }
        [JsonProperty("password_protected")]
        public string PasswordProtected { get; set; }
        public string MimeType { get; set; }
        public string FileType { get; set; }
        public int View { get; set; }
        public int Edit { get; set; }
        public string Hash { get; set; }
        public string Flag { get; set; }
        public int Relevancy { get; set; }
        [JsonProperty("created_utc")]
        public DateTime CreatedUtc { get; set; }
        public string FolderKey { get; set; }
        public string Name { get; set; }

        [JsonProperty("total_folders")]
        public int TotalFolders { get; set; }
        [JsonProperty("total_files")]
        public int TotalFiles { get; set; }
        [JsonProperty("total_size")]
        public long TotalSize { get; set; }
    }

    internal class SearchResponse : MediaFireResponseBase
    {
        [JsonProperty("results_count")]
        public int ResultsCount { get; set; }
        public SearchResultItem[] Results { get; set; }
    }
}
