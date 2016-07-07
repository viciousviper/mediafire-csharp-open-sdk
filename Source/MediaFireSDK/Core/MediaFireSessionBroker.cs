using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediaFireSDK.Http;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Responses;
using MediaFireSDK.Services;

namespace MediaFireSDK.Core
{
    internal class MediaFireSessionBroker : IDisposable
    {
        private readonly ICryptoService _cryptoService;
        private readonly MediaFireApiConfiguration _configuration;
        private readonly string _email;
        private readonly string _password;
        private readonly MediaFireRequestController _requestController;

        internal string CurrentSessionToken { get; private set; }
        private long? _secretKey;
        private string _time;

        private Task _renewalTask;
        private Task _periodicRenewalTask;
        private CancellationTokenSource _periodicTokenSource;

        internal event EventHandler AuthenticationContextChanged;

        public MediaFireSessionBroker(ICryptoService cryptoService, MediaFireApiConfiguration configuration, string email, string password, MediaFireRequestController requestController)
        {
            _cryptoService = cryptoService;
            _configuration = configuration;
            _email = email;
            _password = password;
            _requestController = requestController;
        }

        public MediaFireSessionBroker(ICryptoService cryptoService, MediaFireApiConfiguration configuration, AuthenticationContext authenticationContext, MediaFireRequestController requestController)
        {
            _cryptoService = cryptoService;
            _configuration = configuration;
            CurrentSessionToken = authenticationContext.SessionToken;
            _secretKey = authenticationContext.SecretKey;
            _time = authenticationContext.Time;
            _requestController = requestController;
        }

        public AuthenticationContext GetAuthenticationContext()
        {
            if (!_secretKey.HasValue)
                throw new InvalidOperationException("Authentication context has not been initialized");

            return new AuthenticationContext(CurrentSessionToken, _secretKey.Value, _time);
        }

        public async Task<string> GetSessionToken(TokenVersion tokenVersion)
        {
            var token = await GetSessionTokenInternal(tokenVersion);

            if (tokenVersion == TokenVersion.V1)
                LaunchPeriodicRenewal();

            return token;
        }

        public async Task Authenticate(HttpRequestConfiguration request)
        {
            await WaitForRenewal();

            request
                .AddOrReplaceParameter(MediaFireApiParameters.SessionToken, CurrentSessionToken)
                .Parameter(MediaFireApiParameters.Signature, GetMediaFireCallSignature);
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
                await GetSessionTokenInternal(TokenVersion.V1);
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

        private async Task<string> GetSessionTokenInternal(TokenVersion tokenVersion)
        {
            var httpRequest = await _requestController.CreateHttpRequest(MediaFireApiUserMethods.GetSessionToken, authenticate: false);

            httpRequest
                .Parameter(MediaFireApiParameters.Email, _email)
                .Parameter(MediaFireApiParameters.Password, _password)
                .Parameter(MediaFireApiParameters.Signature, GetMediaFireSignature(_email, _password))
                .Parameter(MediaFireApiParameters.TokenVersion, tokenVersion == TokenVersion.V2 ? 2 : (int?)null);

            return await RetrieveSessionToken(httpRequest);
        }

        private async Task<string> RenewSessionTokenInternal()
        {
            var httpRequest = await _requestController.CreateHttpRequest(MediaFireApiUserMethods.RenewSessionToken, authenticate: false);
            httpRequest
                .Parameter(MediaFireApiParameters.SessionToken, CurrentSessionToken)
                .Parameter(MediaFireApiParameters.Signature, GetMediaFireCallSignature);

            return await RetrieveSessionToken(httpRequest);
        }

        private async Task<string> RetrieveSessionToken(HttpRequestConfiguration httpRequest)
        {
            var response = await _requestController.Post<SessionTokenResponse>(httpRequest);
            _secretKey = !string.IsNullOrEmpty(response.SecretKey) ? long.Parse(response.SecretKey) : (long?)null;
            _time = response.Time;
            return CurrentSessionToken = response.SessionToken;
        }

        private void LaunchPeriodicRenewal()
        {
            if (!_configuration.PeriodicallyRenewToken)
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
                    throw;
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

        private string GetMediaFireCallSignature(HttpRequestConfiguration request)
        {
            const string signatureFormat = "{0}{1}{2}";

            if (!_secretKey.HasValue)
                return null;

            var uri = new Uri(request.GetConfiguredPath());
            return _cryptoService.GetMd5Hash(string.Format(signatureFormat, _secretKey % 256, _time, uri.PathAndQuery)).ToLowerInvariant();
        }

        public void RenewSecretKey()
        {
            if (!_secretKey.HasValue)
                return;

            _secretKey = (_secretKey * 16807) % 2147483647;

            AuthenticationContextChanged?.Invoke(this, EventArgs.Empty);
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

        public void Dispose()
        {
            if (_periodicTokenSource != null)
            {
                _periodicTokenSource.Cancel();
            }
        }
    }
}
