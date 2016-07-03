﻿using System;

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
        public string ApiKey { get; }

        /// <summary>
        /// The aplication id.
        /// </summary>
        public string AppId { get; }

        /// <summary>
        /// The api version to be used by the sdk.
        /// </summary>
        public string ApiVersion { get; }

        /// <summary>
        /// Configures the sdk to automatically renew the client if the session token expires.
        /// </summary>
        public bool AutomaticallyRenewToken { get; }

        /// <summary>
        /// The buffer size to be used on block operations, upload and download.
        /// </summary>
        public int ChunkTransferBufferSize { get; }

        /// <summary>
        /// Use version 1.0 of http
        /// <remarks>On some platforms, the client will throw the error "The server committed a protocol violation. Section=ResponseStatusLine". In that cases set this property to true</remarks>
        /// </summary>
        public bool UseHttpV1 { get; }

        /// <summary>
        /// Configures a timer so that every SessionRenewPeriod minutes the SDK can renew automatically the session.
        /// </summary>
        public bool PeriodicallyRenewToken { get; }

        public TimeSpan SessionRenewPeriod { get; set; }

        public MediaFireApiConfiguration(string apiKey, string appId, string apiVersion = "1.5", bool automaticallyRenewToken = true, int chunkTransferBufferSize = 4096, bool useHttpV1 = false, bool periodicallyRenewToken = false)
        {
            ApiKey = apiKey;
            AppId = appId;
            ApiVersion = apiVersion;
            AutomaticallyRenewToken = automaticallyRenewToken;
            ChunkTransferBufferSize = chunkTransferBufferSize;
            UseHttpV1 = useHttpV1;
            PeriodicallyRenewToken = periodicallyRenewToken;
            SessionRenewPeriod = TimeSpan.FromMinutes(7);
        }
    }
}
