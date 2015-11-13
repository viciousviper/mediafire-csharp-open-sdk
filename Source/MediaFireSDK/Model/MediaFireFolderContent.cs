using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MediaFireSDK.Model
{
   



    public class MediaFireFolderContent
    {
        [JsonProperty("chunk_size")]
        public int ChunkSize { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("chunk_number")]
        public int ChunkNumber { get; set; }

        [JsonProperty("more_chunks")]
        internal string MoreChunks { get; set; }

        public bool HasMoreChunks { get { return string.Equals(MoreChunks, MediaFireApiConstants.MediaFireYes, StringComparison.OrdinalIgnoreCase); } }

        public List<MediaFireFolder> Folders { get; set; }
        public List<MediaFireFile> Files { get; set; } 
    }
}
