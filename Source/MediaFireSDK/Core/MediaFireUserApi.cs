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

        public MediaFireUserApi(MediaFireRequestController requestController, MediaFireApiConfiguration configuration, ICryptoService cryptoService)
            : base(requestController,configuration)
        {
            _cryptoService = cryptoService;
        }


        public async Task<string> GetSessionToken(string email, string password)
        {
            RequestController.SessionBroker = null;

            var sessionBroker = new MediaFireSessionBroker(
                _cryptoService,
                Configuration,
                email,
                password,
                RequestController
                );


            var sessionToken = await sessionBroker.GetSessionToken();

            RequestController.SessionBroker = sessionBroker;


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
    }
}
