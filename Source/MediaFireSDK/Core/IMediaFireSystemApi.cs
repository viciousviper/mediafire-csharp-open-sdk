using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Responses;

namespace MediaFireSDK.Core
{
    public interface IMediaFireSystemApi
    {
        /// <summary>
        /// Returns all the configuration data about the MediaFire system.
        /// </summary>
        Task<MediaFireDetails> GetInfo();
    }
}
