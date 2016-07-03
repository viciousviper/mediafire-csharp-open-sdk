using System;
using System.Threading.Tasks;
using MediaFireSDK.Http;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Responses;
using MediaFireSDK.Services;

namespace MediaFireSDK.Core
{
    internal class MediaFireUserApi : MediaFireApiBase, IMediaFireUserApi
    {
        private readonly ICryptoService _cryptoService;

        public MediaFireUserApi(MediaFireRequestController requestController, MediaFireApiConfiguration configuration, ICryptoService cryptoService)
            : base(requestController, configuration)
        {
            _cryptoService = cryptoService;
        }

        public async Task<string> GetSessionToken(string email, string password, TokenVersion tokenVersion = TokenVersion.V1)
        {
            RequestController.SessionBroker = new MediaFireSessionBroker(_cryptoService, Configuration, email, password, RequestController);
            RequestController.SessionBroker.AuthenticationContextChanged += (s, e) => AuthenticationContextChanged?.Invoke(this, e);

            var sessionToken = await RequestController.SessionBroker.GetSessionToken(tokenVersion);

            return sessionToken;
        }

        public async Task<RegisterResponse> Register(string email, string password, string firstName = null, string lastName = null, string displayName = null)
        {
            var requestConfig = await RequestController.CreateHttpRequest(MediaFireApiUserMethods.Register, authenticate: false);

            requestConfig
                .Parameter(MediaFireApiParameters.Email, email)
                .Parameter(MediaFireApiParameters.Password, password)
                .Parameter(MediaFireApiParameters.FirstName, firstName)
                .Parameter(MediaFireApiParameters.LastName, lastName)
                .Parameter(MediaFireApiParameters.DisplayName, displayName);

            return await RequestController.Post<RegisterResponse>(requestConfig);
        }

        public AuthenticationContext GetAuthenticationContext()
        {
            if (RequestController.SessionBroker == null)
                throw new InvalidOperationException("SessionBroker has not been initialized");

            return RequestController.SessionBroker.GetAuthenticationContext();
        }

        public void SetAuthenticationContext(AuthenticationContext authenticationContext)
        {
            if (authenticationContext == null)
                throw new ArgumentNullException("authenticationContext");

            RequestController.SessionBroker = new MediaFireSessionBroker(_cryptoService, Configuration, authenticationContext, RequestController);
        }

        public async Task<UserTermsOfService> FetchTermsOfService()
        {
            var resp = await Get<FetchTosResponse>(MediaFireApiUserMethods.FetchTos);
            return resp.TermsOfService;
        }

        public async Task AcceptTermsOfService(string acceptanceToken)
        {
            var requestConfig = await RequestController.CreateHttpRequest(MediaFireApiUserMethods.AcceptTos);

            requestConfig.Parameter(MediaFireApiParameters.AcceptanceToken, acceptanceToken);

            await RequestController.Post<MediaFireEmptyResponse>(requestConfig);
        }

        public Task Logout()
        {
            RequestController.SessionBroker = null;
            return Task.FromResult(true);
        }

        public event EventHandler AuthenticationContextChanged;
    }
}
