namespace MediaFireSDK.Model
{
    /// <summary>
    /// The mediafire sdk configuration.
    /// </summary>
    public sealed class MediaFireApiConfiguration
    {
        /// <summary>
        /// The aplication key.
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// The aplication id.
        /// </summary>
        public string AppId { get; private set; }

        /// <summary>
        /// The api version to be used by the sdk.
        /// </summary>
        public string ApiVersion { get; private set; }

        /// <summary>
        /// Configures the sdk to automatically renew the client if the session token expires.
        /// </summary>
        public bool AutomaticallyRenewToken { get; private set; }

        /// <summary>
        /// The buffer size to be used on block operations, upload and download.
        /// </summary>
        public int ChunkTransferBufferSize { get; private set; }

        /// <summary>
        /// Use version 1.0 of http
        /// <remarks>This should be used on Windows 8.1 apps to workaround a bug while uploading.</remarks>
        /// </summary>
        public bool UseHttpV1 { get; private set; }


        public MediaFireApiConfiguration(string apiKey, string appId, string apiVersion = "1.4", bool automaticallyRenewToken = true, int chunkTransferBufferSize = 4096, bool useHttpV1 = false)
        {
            ApiKey = apiKey;
            AppId = appId;
            ApiVersion = apiVersion;
            AutomaticallyRenewToken = automaticallyRenewToken;
            ChunkTransferBufferSize = chunkTransferBufferSize;
            UseHttpV1 = useHttpV1;
        }
    }
}
