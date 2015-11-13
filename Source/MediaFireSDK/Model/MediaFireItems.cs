using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MediaFireSDK.Model
{
    public class MediaFireItem
    {
        public string Key { get; set; }
        public DateTime Created { get; set; }
        [JsonProperty("created_utc ")]
        public DateTime CreatedUtc { get; set; }
        public string Flag { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Privacy { get; set; }
        public string Revision { get; set; }

        [JsonProperty("owner_name")]
        public string OwnerName { get; set; }

        /// <summary>
        /// returned in search
        /// </summary>
        [JsonProperty("parent_name")]
        public string ParentName { get; set; }

        /// <summary>
        /// returned in search
        /// </summary>
        [JsonProperty("parent_folderkey")]
        public string ParentFolderKey { get; set; }

        /// <summary>
        /// returned in search
        /// </summary>
        public int Relevancy { get; set; }


        public override string ToString()
        {
            return string.Format("[{0}] {1}", Key, Name);
        }
    }

    public class MediaFireFolder : MediaFireItem
    {
        public MediaFireFolder()
        {
            TotalFiles = TotalFolders = TotalSize = -1;
        }
        public string FolderKey { get { return Key; } set { Key = value; } }
        [JsonProperty("file_count")]
        public int FileCount { get; set; }
        [JsonProperty("folder_count")]
        public string FolderCount { get; set; }
        [JsonProperty("dropbox_enabled")]
        internal string DropboxEnabled { get; set; }
        public bool IsDropboxEnabled
        {
            get
            {
                return DropboxEnabled.Equals(MediaFireApiConstants.MediaFireYes, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        /// <remarks>
        /// This property is only availabled if the request that generated it had the details set to true.
        /// </remarks>
        [JsonProperty("total_folders")]
        public long TotalFolders { get; set; }

        /// <remarks>
        /// This property is only availabled if the request that generated it had the details set to true.
        /// </remarks>
        [JsonProperty("total_files")]
        public long TotalFiles { get; set; }

        /// <remarks>
        /// This property is only availabled if the request that generated it had the details set to true.
        /// </remarks>
        [JsonProperty("total_size")]
        public long TotalSize { get; set; }
    }


    public class MediaFireFile : MediaFireItem
    {
        public string QuickKey { get { return Key; } set { Key = value; } }
        public string Hash { get; set; }
        public string FileName { get { return Name; } set { Name = value; } }
        public long Size { get; set; }
        [JsonProperty("password_protected")]
        internal string PasswordProtected { get; set; }
        public bool IsPasswordProtected
        {
            get
            {
                return PasswordProtected.Equals(MediaFireApiConstants.MediaFireYes, StringComparison.CurrentCultureIgnoreCase);
            }
        }
        public string MimeType { get; set; }
        public string FileType { get; set; }
        public int View { get; set; }
        public int Edit { get; set; }
        public long Downloads { get; set; }
        public long Views { get; set; }
        public MediaFireLinks Links { get; set; }



        [JsonProperty("ready")]
        internal string Ready { get; set; }

        public bool IsReady { get { return !(string.Equals(Ready, MediaFireApiConstants.MediaFireNo)); } }
    }

}
