using System;
using Newtonsoft.Json;

namespace MediaFireSDK.Model.Responses
{
    public abstract class MediaFireResponseBase
    {
        public string Action { get; set; }

        public string PKey { get; set; }

        public string Message { get; set; }

        public int Error { get; set; }

        public string Result { get; set; }

        [JsonProperty("current_api_version")]
        public string CurrentApiVersion { get; set; }

        public string Deprecated { get; set; }

        public string Asynchronous { get; set; }

        [JsonProperty("new_key")]
        internal string NewKeyInternal { get; set; }

        public bool NewKey { get { return !string.IsNullOrEmpty(NewKeyInternal) &&  NewKeyInternal.FromMediaFireYesNo(); } }
    }

    internal class MediaFireResponseContainer<T> where T : MediaFireResponseBase
    {
        public T Response { get; set; }
    }

    public class MediaFireEmptyResponse : MediaFireResponseBase
    {
    }

    public class MediaFireErrorResponse : MediaFireResponseBase
    {
    }
}
