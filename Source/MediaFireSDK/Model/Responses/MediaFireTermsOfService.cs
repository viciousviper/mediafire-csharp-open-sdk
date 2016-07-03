﻿using System;
using Newtonsoft.Json;

namespace MediaFireSDK.Model.Responses
{
    public class MediaFireTermsOfService
    {
        public string Revision { get; set; }

        public string Terms { get; set; }

        public DateTime Date { get; set; }
    }

    public class UserTermsOfService : MediaFireTermsOfService
    {
        [JsonProperty("user_accepted_terms")]
        internal string UserAcceptedTermsString { get; set; }

        public bool UserAcceptedTerms { get { return UserAcceptedTermsString.FromMediaFireYesNo(); } }

        [JsonProperty("acceptance_token")]
        public string AcceptanceToken { get; set; }
    }
}
