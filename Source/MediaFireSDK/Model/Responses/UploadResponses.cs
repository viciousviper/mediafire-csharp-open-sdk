using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MediaFireSDK.Model.Responses
{
    public class UploadResponse : MediaFireResponseBase
    {
        public MediaFireUploadDetails DoUpload { get; set; }

        [JsonProperty("resumable_upload")]
        public ResumableUploadDetails ResumableUpload { get; set; }
    }

    public class Bitmap
    {
        public int Count { get; set; }

        public List<int> Words { get; set; }
    }

    public class ResumableUploadDetails
    {
        [JsonProperty("all_units_ready")]
        internal string AllUnitsReadyInternal { get; set; }

        public bool AllUnitsReady { get { return AllUnitsReadyInternal.FromMediaFireYesNo(); } }

        [JsonProperty("number_of_units")]
        public int NumberOfUnits { get; set; }

        [JsonProperty("unit_size")]
        public long UnitSize { get; set; }

        public Bitmap Bitmap { get; set; }
    }
}
