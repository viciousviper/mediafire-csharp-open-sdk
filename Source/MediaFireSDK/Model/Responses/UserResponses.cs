using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MediaFireSDK.Model.Responses
{
    internal class SessionTokenResponse : MediaFireResponseBase
    {
        [JsonProperty("session_token")]
        public string SessionToken { get; set; }
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
