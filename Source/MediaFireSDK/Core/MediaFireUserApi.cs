using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public MediaFireUserApi(MediaFireRequestController requestController, ICryptoService cryptoService)
            : base(requestController)
        {
            _cryptoService = cryptoService;
        }


        public async Task<string> Authenticate(string email, string password)
        {
            RequestController.SessionBroker = null;

            var sessionBroker = new MediaFireSessionBroker(
                _cryptoService,
                RequestController.Configuration.AppId,
                RequestController.Configuration.ApiKey,
                email,
                password
                );


            var sessionToken = await sessionBroker.GetSessionToken(RequestController);

            RequestController.SessionBroker = sessionBroker;

            return sessionToken;

        }

        public async Task<MediaFireUserDetails> GetInfo()
        {

            var requestConfig = await RequestController.CreateHttpRequest(ApiUserMethods.GetInfo);
            var response = await RequestController.Get<GetUserInfoResponse>(requestConfig);
            return response.UserDetails;
        }

        public async Task<RegisterResponse> Register(string email, string password, string firstName = null, string lastName = null, string displayName = null)
        {
            var requestConfig = await RequestController.CreateHttpRequest(ApiUserMethods.Register, authenticate: false);

            requestConfig
                .Parameter(ApiParameters.Email, email)
                .Parameter(ApiParameters.Password, password)
                .Parameter(ApiParameters.FirstName, firstName)
                .Parameter(ApiParameters.LastName, lastName)
                .Parameter(ApiParameters.DisplayName, displayName);

            return await RequestController.Post<RegisterResponse>(requestConfig);


        }

        public async Task<UserTermsOfService> FetchTermsOfService()
        {
            var resp = await Get<FetchTosResponse>(ApiUserMethods.FetchTos);
            return resp.TermsOfService;
        }

        public async Task AcceptTermsOfService(string acceptanceToken)
        {
            var requestConfig = await RequestController.CreateHttpRequest(ApiUserMethods.AcceptTos);

            requestConfig.Parameter(ApiParameters.AcceptanceToken, acceptanceToken);

            await RequestController.Post<EmptyResponse>(requestConfig);
        }

        public Task Logout()
        {
            RequestController.SessionBroker = null;
            return Task.FromResult(true);
        }
    }
}
