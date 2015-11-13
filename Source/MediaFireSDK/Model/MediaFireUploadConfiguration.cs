using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFireSDK.Model
{
    /// <summary>
    /// Represent the data necessary to make an upload to MediaFire
    /// </summary>
    public sealed class MediaFireUploadConfiguration
    {
        internal MediaFireUploadConfiguration()
        {
            RequestHeaders = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// The mandatory header list.
        /// </summary>
        public List<KeyValuePair<string, string>> RequestHeaders { get; set; }

        /// <summary>
        /// The upload request url
        /// </summary>
        public string  RequestUrl { get; set; }
    }
}
