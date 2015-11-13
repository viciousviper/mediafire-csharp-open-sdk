using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MediaFireSDK.Model
{
    public class MediaFireLikedAccountBase
    {
        public string Linked { get; set; }
        public DateTime DateCreated { get; set; }
        [JsonProperty("date_created_utc")]
        public DateTime CreatedUtc { get; set; }
        public string I18N { get; set; }
        public string Synced { get; set; }
        public string Name { get; set; }
    }

    public class FacebookAccount : MediaFireLikedAccountBase
    {
        [JsonProperty("facebook_id")]
        public string FacebookId { get; set; }
        [JsonProperty("service_id")]
        public string ServiceId { get; set; }
        [JsonProperty("token_for_business")]
        public string TokenForBusiness { get; set; }
        public string Email { get; set; }
        [JsonProperty("facebook_url")]
        public string FacebookUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Hometown { get; set; }
        public string Location { get; set; }
        public string Timezone { get; set; }
    }

    public class TwitterAccount : MediaFireLikedAccountBase
    {
        [JsonProperty("twitter_id")]
        public string TwitterId { get; set; }
        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }
    }

    public class GmailAccount : MediaFireLikedAccountBase
    {
        [JsonProperty("gmail_id")]
        public string GmailId { get; set; }
        public string Email { get; set; }
        [JsonProperty("gmail_url")]
        public string GmailUrl { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }

    }


    public class MediaFireUserDetails
    {
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        public string Gender { get; set; }

        [JsonProperty("birth_date")]
        public DateTime BirthDate { get; set; }

        public string Location { get; set; }

        public string Website { get; set; }

        public string Premium { get; set; }

        public long Bandwidth { get; set; }

        public DateTime Created { get; set; }

        public string Validated { get; set; }

        [JsonProperty("tos_accepted")]
        public string TosAccepted { get; set; }

        [JsonProperty("used_storage_size")]
        public long UsedStorageSize { get; set; }

        [JsonProperty("base_storage")]
        public long BaseStorage { get; set; }

        [JsonProperty("bonus_storage")]
        public long BonusStorage { get; set; }

        [JsonProperty("storage_limit")]
        public long StorageLimit { get; set; }

        [JsonProperty("storage_limit_exceeded")]
        public string StorageLimitExceeded { get; set; }

        [JsonProperty("used_revision_storage")]
        public string UsedRevisionStorage { get; set; }

        public string Options { get; set; }

        public FacebookAccount Facebook { get; set; }

        public GmailAccount Gmail { get; set; }

        public TwitterAccount Twitter { get; set; }

        [JsonProperty("one_time_key_request_max_count")]
        public long OneTimeKeyRequestMaxCount { get; set; }

    }
}
