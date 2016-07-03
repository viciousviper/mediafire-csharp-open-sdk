using System;

namespace MediaFireSDK.Core
{
    public class AuthenticationContext
    {
        public string SessionToken { get; }

        public long SecretKey { get; }

        public string Time { get; }

        public AuthenticationContext(string sessionToken, long secretKey, string time)
        {
            if (string.IsNullOrEmpty(sessionToken))
                throw new ArgumentNullException("sessionToken");
            if (string.IsNullOrEmpty(time))
                throw new ArgumentNullException("time");

            SessionToken = sessionToken;
            SecretKey = secretKey;
            Time = time;
        }
    }
}
