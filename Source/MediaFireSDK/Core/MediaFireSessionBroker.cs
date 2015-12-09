using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaFireSDK.Http;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Responses;
using MediaFireSDK.Services;

namespace MediaFireSDK.Core
{
    internal class MediaFireSessionBroker
    {
        private readonly ICryptoService _cryptoService;
        private readonly MediaFireApiConfiguration _configuration;
        private readonly string _email;
        private readonly string _password;
        private readonly MediaFireRequestController _requestController;


        internal string CurrentSessionToken { get; private set; }

        private Task _renewalTask;
        private Task _periodicRenewalTask;
        private CancellationTokenSource _periodicTokenSource;

        public MediaFireSessionBroker(
            ICryptoService cryptoService,
           MediaFireApiConfiguration configuration,
            string email,
            string password,
            MediaFireRequestController requestController
            )
        {
            _cryptoService = cryptoService;
            _configuration = configuration;
            _email = email;
            _password = password;
            _requestController = requestController;
        }


        public async Task<string> GetSessionToken()
        {
            var token = await GetSessionTokenInternal();
            LaunchPeriodicRenewal();
            return token;
        }

        public async Task Authenticate(HttpRequestConfiguration request)
        {
            await WaitForRenewal();

            request.AddOrReplaceParameter(ApiParameters.SessionToken, CurrentSessionToken);
        }

        public async Task RetrieveNewSessionToken()
        {
            var tcs = new TaskCompletionSource<bool>();

            if (Interlocked.CompareExchange(ref _renewalTask, tcs.Task, null) != null)
            {
                await WaitForRenewal();
                return;
            }

            try
            {
                await GetSessionTokenInternal();
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

        private async Task<string> GetSessionTokenInternal()
        {
            var httpRequest = await _requestController.CreateHttpRequest(ApiUserMethods.GetSessionToken, authenticate: false);

            httpRequest
                .Parameter(ApiParameters.Email, _email)
                .Parameter(ApiParameters.Password, _password)
                .Parameter(ApiParameters.Signature, GetMediaFireSignature(_email, _password));


            return await RetrieveSessionToken(httpRequest);
        }

        private async Task<string> RenewSessionTokenInternal()
        {
            var httpRequest = await _requestController.CreateHttpRequest(ApiUserMethods.RenewSessionToken, authenticate: false);
            httpRequest
                .Parameter(ApiParameters.SessionToken, CurrentSessionToken);

            return await RetrieveSessionToken(httpRequest);
        }

        private async Task<string> RetrieveSessionToken(HttpRequestConfiguration httpRequest)
        {
            var response = await _requestController.Post<SessionTokenResponse>(httpRequest);
            CurrentSessionToken = response.SessionToken;
            return response.SessionToken;
        }


        private void LaunchPeriodicRenewal()
        {
            if (_configuration.PeriodicallyRenewToken == false)
                return;

            var oldRenewalTask = _periodicRenewalTask;
            var oldCancellationTokenSource = _periodicTokenSource;

            _periodicTokenSource = new CancellationTokenSource();

            _periodicRenewalTask = Task.Run(async () =>
            {
                try
                {
                    await PeriodicRenewalRoutine(_configuration.SessionRenewPeriod, _periodicTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception e)
                {
                    //
                    //  Should not happen
                    //
#if DEBUG
                    if (Debugger.IsAttached)
                        Debugger.Break();
                    throw e;
#endif
                }
            }, _periodicTokenSource.Token);

            //
            //  Clean up other possible tasks.
            //
            if (oldRenewalTask != null)
                oldCancellationTokenSource.Cancel();

        }

        private async Task PeriodicRenewalRoutine(TimeSpan period, CancellationToken cancellationToken)
        {
            do
            {
                await Task.Delay(period, cancellationToken);

                var periodicRenewTask = new TaskCompletionSource<bool>();

                //
                //  Block further requests until the renew is done, if a renewal is already happening wait for it.
                //
                if (Interlocked.CompareExchange(ref _renewalTask, periodicRenewTask.Task, null) != null)
                {
                    if (await WaitForRenewal())
                        continue;
                }

                try
                {
                    var sessionToken = await RenewSessionTokenInternal();

                    cancellationToken.ThrowIfCancellationRequested();

                    CurrentSessionToken = sessionToken;

                    periodicRenewTask.SetResult(true);
                }
                catch (Exception e)
                {
                    //
                    //  Something went wrong try again in one minute.
                    //
                    period = TimeSpan.FromMinutes(1);
                    periodicRenewTask.SetException(e);


                    cancellationToken.ThrowIfCancellationRequested();
                    continue;
                }
                finally
                {
                    _renewalTask = null;
                }

                period = _configuration.SessionRenewPeriod;
            } while (true);
        }

        private string GetMediaFireSignature(string email, string password)
        {
            const string signatureFormat = "{0}{1}{2}{3}";
            var appKey = _configuration.ApiKey;
            var appId = _configuration.AppId;

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
