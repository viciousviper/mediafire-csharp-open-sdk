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

        private readonly EventHandler _authenticationContextChangedHandler;

        public MediaFireUserApi(MediaFireRequestController requestController, MediaFireApiConfiguration configuration, ICryptoService cryptoService)
            : base(requestController, configuration)
        {
            _cryptoService = cryptoService;
            _authenticationContextChangedHandler = (s, e) => AuthenticationContextChanged?.Invoke(this, e);
        }

        public async Task<string> GetSessionToken(string email, string password, TokenVersion tokenVersion = TokenVersion.V1)
        {
            if (RequestController.SessionBroker != null)
                RequestController.SessionBroker.AuthenticationContextChanged -= _authenticationContextChangedHandler;
            RequestController.SessionBroker = new MediaFireSessionBroker(_cryptoService, Configuration, email, password, RequestController);
            RequestController.SessionBroker.AuthenticationContextChanged += _authenticationContextChangedHandler;

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

            if (RequestController.SessionBroker != null)
                RequestController.SessionBroker.AuthenticationContextChanged -= _authenticationContextChangedHandler;
            RequestController.SessionBroker = new MediaFireSessionBroker(_cryptoService, Configuration, authenticationContext, RequestController);
            RequestController.SessionBroker.AuthenticationContextChanged += _authenticationContextChangedHandler;
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
            if (RequestController.SessionBroker != null)
                RequestController.SessionBroker.AuthenticationContextChanged -= _authenticationContextChangedHandler;
            RequestController.SessionBroker = null;
            return Task.FromResult(true);
        }

        public event EventHandler AuthenticationContextChanged;
    }
}
