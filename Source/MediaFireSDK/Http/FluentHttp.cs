using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaFireSDK.Model;
using Newtonsoft.Json;

namespace MediaFireSDK.Http
{
    public abstract class HttpRequestConfiguration
    {
        protected string Path;
        private readonly bool _ignoreEmptyValues;
        protected readonly List<KeyValuePair<string, string>> Headers = new List<KeyValuePair<string, string>>();
        protected readonly List<KeyValuePair<string, string>> ContentHeaders = new List<KeyValuePair<string, string>>();
        protected readonly List<KeyValuePair<string, string>> Parameters = new List<KeyValuePair<string, string>>();
        protected Stream Stream;
        protected IProgress<MediaFireOperationProgress> ProgressOperation;
        protected MediaFireOperationProgress ProgressData;
        protected CancellationToken Token;

        internal bool IsUpload;
        internal bool IsDownload;

        protected HttpRequestConfiguration(string path, bool ignoreEmptyValues)
        {
            Path = path;
            _ignoreEmptyValues = ignoreEmptyValues;
            Token = CancellationToken.None;
            ProgressOperation = new Progress<MediaFireOperationProgress>();
            ProgressData = new MediaFireOperationProgress();
        }

        public HttpRequestConfiguration Parameter(string name, object value)
        {
            return value == null && _ignoreEmptyValues ? this : AddParameter(name, value.ToString());
        }

        public HttpRequestConfiguration Parameter(string name, Func<HttpRequestConfiguration, string> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            var value = func(this);
            return string.IsNullOrEmpty(value) && _ignoreEmptyValues ? this : AddParameter(name, value);
        }

        public HttpRequestConfiguration Parameter(string name, string value)
        {
            return string.IsNullOrEmpty(value) && _ignoreEmptyValues ? this : AddParameter(name, value);
        }

        private HttpRequestConfiguration AddParameter(string name, string value)
        {
            Parameters.Add(new KeyValuePair<string, string>(name, value));
            return this;
        }

        public HttpRequestConfiguration AddOrReplaceParameter(string name, string value)
        {
            Replace(Parameters, name, value);
            return this;
        }

        public HttpRequestConfiguration AddOrReplaceHeader(string name, string value)
        {
            Replace(Headers, name, value);
            return this;
        }

        public HttpRequestConfiguration Header(string name, string value)
        {
            if (_ignoreEmptyValues && string.IsNullOrEmpty(value))
                return this;

            Headers.Add(new KeyValuePair<string, string>(name, value));
            return this;
        }

        public HttpRequestConfiguration ContentHeader(string name, string value)
        {
            if (_ignoreEmptyValues && string.IsNullOrEmpty(value))
                return this;

            ContentHeaders.Add(new KeyValuePair<string, string>(name, value));
            return this;
        }

        public HttpRequestConfiguration Content(Stream s, bool forUpload)
        {
            Stream = s;
            IsUpload = forUpload;
            IsDownload = !forUpload;
            return this;
        }

        public HttpRequestConfiguration WithProgress(IProgress<MediaFireOperationProgress> progressAction, MediaFireOperationProgress progressData, CancellationToken token)
        {
            Token = token;
            ProgressData = progressData;
            ProgressOperation = progressAction;
            return this;
        }

        private void Replace(List<KeyValuePair<string, string>> list, string key, string value)
        {
            for (int index = 0; index < Parameters.Count; index++)
            {
                var item = Parameters[index];
                if (item.Key == key)
                {
                    Parameters.RemoveAt(index);
                    break;
                }
            }

            list.Add(new KeyValuePair<string, string>(key, value));
        }

        public Task<T> Get<T>()
        {
            return SendAsync<T>(HttpMethod.Get);
        }

        public Task<T> Post<T>()
        {
            return SendAsync<T>(HttpMethod.Post);
        }

        public Task<T> Put<T>()
        {
            return SendAsync<T>(HttpMethod.Put);
        }

        public Task<T> Delete<T>()
        {
            return SendAsync<T>(HttpMethod.Delete);
        }

        public abstract Task<T> SendAsync<T>(HttpMethod method);

        protected static T DeserializeJson<T>(string resultString)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)resultString;
            }

            return JsonConvert.DeserializeObject<T>(resultString);
        }

        public string GetConfiguredPath()
        {
            return MediaFireHttpHelpers.BuildQueryString(Parameters, Path);
        }
    }
}
