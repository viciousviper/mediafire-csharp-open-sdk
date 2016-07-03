﻿using System;
using System.Threading.Tasks;
using MediaFireSDK.Model.Responses;

namespace MediaFireSDK.Core
{
    public interface IMediaFireUserApi
    {
        /// <summary>
        /// Authenticates the SDK with the user credentials.
        /// </summary>
        /// <param name="email">The email address of the user's MediaFire account.</param>
        /// <param name="password">The password of the user's MediaFire account.</param>
        /// <param name="tokenVersion">The <see cref="TokenVersion"/> to retrieve.</param>
        /// <returns>The user session token</returns>
        /// <remarks>This method is not thread-safe.</remarks>
        Task<string> GetSessionToken(string email, string password, TokenVersion tokenVersion = TokenVersion.V1);

        /// <summary>
        /// Register an individual with MediaFire and create a user account.
        /// </summary>
        /// <param name="email">The email address to be used as a login to the user's MediaFire account.</param>
        /// <param name="password">The password of the user's MediaFire account.</param>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="displayName">The name to be displayed on the user's shared items.</param>
        Task<RegisterResponse> Register(string email, string password, string firstName = null, string lastName = null, string displayName = null);

        /// <summary>
        /// Gets the session token v2 and associated secret key and time.
        /// </summary>
        /// <returns>The authentication context.</returns>
        AuthenticationContext GetAuthenticationContext();

        /// <summary>
        /// Sets the session token v2 and associated secret key and time.
        /// </summary>
        /// <param name="authenticationContext">The authentication context.</param>
        void SetAuthenticationContext(AuthenticationContext authenticationContext);

        /// <summary>
        /// Returns the HTML format of the MediaFire Terms of Service and its revision number, date, whether the user has accepted it not not, and the acceptance token (if the user has not accepted the latest terms).
        /// </summary>
        /// <returns>An object with the necessary info about the terms of service </returns>
        Task<UserTermsOfService> FetchTermsOfService();

        /// <summary>
        /// Records that the session user has accepted the MediaFire Terms of Service by sending the acceptance token.
        /// </summary>
        /// <param name="acceptanceToken">The token returned by FetchTermsOfService.</param>
        Task AcceptTermsOfService(string acceptanceToken);

        /// <summary>
        /// Removes from the SDK all user information.
        /// </summary>
        Task Logout();

        /// <summary>
        /// Occurs when the authentication context changed.
        /// </summary>
        event EventHandler AuthenticationContextChanged;
    }
}
