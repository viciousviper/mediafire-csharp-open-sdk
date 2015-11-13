using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaFireSDK.Http;
using MediaFireSDK.Model.Responses;
using MediaFireSDK.Services;

namespace MediaFireSDK.Core
{
    internal class MediaFireSessionBroker
    {
        private readonly ICryptoService _cryptoService;
        private readonly string _apiKey;
        private readonly string _appId;
        private readonly string _email;
        private readonly string _password;

        private string _currentSessionToken;

        private Task _renewalTask;

        public MediaFireSessionBroker(
            ICryptoService cryptoService,
            string appId,
            string apiKey,
            string email,
            string password

            )
        {
            _cryptoService = cryptoService;
            _appId = appId;
            _apiKey = apiKey;
            _email = email;
            _password = password;
        }

        public async Task<string> GetSessionToken(MediaFireRequestController requestController)
        {
            var httpRequest = await requestController.CreateHttpRequest(ApiUserMethods.GetSessionToken, authenticate: false);

            httpRequest
               .Parameter(ApiParameters.Email, _email)
               .Parameter(ApiParameters.Password, _password)
               .Parameter(ApiParameters.Signature, GetMediaFireSignature(_email, _password));


            var response = await requestController.Post<SessionTokenResponse>(httpRequest);
            _currentSessionToken = response.SessionToken;
            return response.SessionToken;
        }



        public async Task Authenticate(HttpRequestConfiguration request)
        {
            await WaitForRenewal();

            request.AddOrReplaceParameter(ApiParameters.SessionToken, _currentSessionToken);
        }

        public async Task<string> GetSessionToken()
        {
            await WaitForRenewal();
            return _currentSessionToken;
        }



        public async Task RenewSessionToken(MediaFireRequestController reqController)
        {
            var tcs = new TaskCompletionSource<bool>();

            if (Interlocked.CompareExchange(ref _renewalTask, tcs.Task, null) != null)
            {
                await WaitForRenewal();
                return;
            }

            try
            {
                await GetSessionToken(reqController);
                tcs.SetResult(true);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
                throw;
            }
            finally
            {
                _renewalTask = null;
            }

        }



        private string GetMediaFireSignature(string email, string password)
        {
            const string signatureFormat = "{0}{1}{2}{3}";
            var appKey = _apiKey;
            var appId = _appId;

            return _cryptoService.GetSha1Hash(string.Format(signatureFormat, email, password, appId, appKey));
        }

        private async Task<bool> WaitForRenewal()
        {
            //
            //  Check if there is any pending token renewal,
            //  if so wait for its termination
            //
            var renewalTask = _renewalTask;

            if (renewalTask == null)
            {
                return false;
            }

            await renewalTask;

            return true;

        }


    }
}
