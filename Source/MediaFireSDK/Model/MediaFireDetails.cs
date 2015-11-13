using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Model.Responses;
using Newtonsoft.Json;

namespace MediaFireSDK.Model
{
    public class MediaFireFileExtensionContainer
    {
        public List<string> Extensions { get; set; }
    }

    public class MediaFireImageSize
    {
        public string Width { get; set; }
        public string Height { get; set; }
        public string Thumbnail { get; set; }
    }

    public class MediaFirePrintService
    {
        [JsonProperty("service_id")]
        public string ServiceId { get; set; }
        public string Name { get; set; }
    }

    public class MediaFireDetails : MediaFireResponseBase
    {
        public string Timezone { get; set; }

        [JsonProperty("timezone_offset")]
        public string TimezoneOffset { get; set; }

        [JsonProperty("image_sizes")]
        public List<MediaFireImageSize> ImageSizes { get; set; }

        [JsonProperty("viewable")]
        internal MediaFireFileExtensionContainer Viewable { get; set; }
        [JsonProperty("editable")]
        internal MediaFireFileExtensionContainer Editable { get; set; }

        public List<string> ViewableFiles { get { return Viewable.Extensions; } }
        public List<string> EditableFiles { get { return Editable.Extensions; } }

        [JsonProperty("terms_of_service")]
        public MediaFireTermsOfService TermsOfService { get; set; }
        [JsonProperty("privacy_policy")]
        public MediaFireTermsOfService PrivacyPolicy { get; set; }
        [JsonProperty("max_keys")]
        public string MaxKeys { get; set; }
        [JsonProperty("max_objects")]
        public string MaxObjects { get; set; }
        [JsonProperty("max_image_size")]
        public string MaxImageSize { get; set; }
        [JsonProperty("print_services")]
        public List<MediaFirePrintService> PrintServices { get; set; }
    }

}
