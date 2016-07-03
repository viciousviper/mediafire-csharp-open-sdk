using System;
using Newtonsoft.Json;

namespace MediaFireSDK.Model.Responses
{
    internal class SessionTokenResponse : MediaFireResponseBase
    {
        [JsonProperty("session_token")]
        public string SessionToken { get; set; }

        [JsonProperty("secret_key")]
        public string SecretKey { get; set; }

        public string Time { get; set; }
    }

    public class MediaFireGetUserInfoResponse : MediaFireResponseBase
    {
        [JsonProperty("user_info")]
        public MediaFireUserDetails UserDetails { get; set; }
    }

    internal class FetchTosResponse : MediaFireResponseBase
    {
        [JsonProperty("terms_of_service")]
        public UserTermsOfService TermsOfService { get; set; }
    }

    public class RegisterResponse : MediaFireResponseBase
    {
        public string Email { get; set; }

        public DateTime Created { get; set; }

        [JsonProperty("created_utc")]
        public DateTime CreatedUtc { get; set; }
    }
}
