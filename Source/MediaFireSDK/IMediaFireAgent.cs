using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Core;
using MediaFireSDK.Model;
using MediaFireSDK.Multimedia;
using Org.BouncyCastle.Bcpg;

namespace MediaFireSDK
{
    /// <summary>
    /// The mediafire API agent.
    /// </summary>
    public interface IMediaFireAgent
    {
        /// <summary>
        /// The configuration passed to the agent in the instantiation phasse.
        /// </summary>
        MediaFireApiConfiguration Configuration { get; }
        
        /// <summary>
        /// The User api methods.
        /// </summary>
        IMediaFireUserApi User { get; }
        
        /// <summary>
        /// The System api methods.
        /// </summary>
        IMediaFireSystemApi System { get; }
        
        /// <summary>
        /// The Folder api methods.
        /// </summary>
        IMediaFireFolderApi Folder { get; }
        
        /// <summary>
        /// The File api methods.
        /// </summary>        
        IMediaFireFileApi File { get; }

        /// <summary>
        /// The Image api methods.
        /// </summary>        
        IMediaFireImageApi Image { get; }

        /// <summary>
        /// The Upload api methods.
        /// </summary>        
        IMediaFireUploadApi Upload { get; }
    }
}
