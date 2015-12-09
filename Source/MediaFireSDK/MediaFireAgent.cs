using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Core;
using MediaFireSDK.Http;
using MediaFireSDK.Model;
using MediaFireSDK.Multimedia;
using MediaFireSDK.Services;

namespace MediaFireSDK
{
    public class MediaFireAgent : IMediaFireAgent
    {

        public MediaFireAgent(MediaFireApiConfiguration configuration)
        {
            Configuration = configuration;
            var requestController = new MediaFireRequestController(configuration);
            var cryptoService = new BouncyCastleCryptoService();
            User = new MediaFireUserApi(requestController, configuration,cryptoService);
            System = new MediaFireSystemApi(requestController);
            Folder = new MediaFireFolderApi(requestController);
            Image = new MediaFireImageApi(requestController);
            File = new MediaFireFileApi(requestController, configuration, this);
            Upload = new MediaFireUploadApi(requestController);
        }

        public MediaFireApiConfiguration Configuration { get; private set; }
        public IMediaFireUserApi User { get; private set; }
        public IMediaFireSystemApi System { get; private set; }
        public IMediaFireFolderApi Folder { get; private set; }
        public IMediaFireImageApi Image { get; private set; }
        public IMediaFireFileApi File { get; private set; }
        public IMediaFireUploadApi Upload { get; private set; }
    }
}
