using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MediaFireSDK.Model.Responses
{
    internal class UploadResponse : MediaFireResponseBase
    {
        public MediaFireUploadDetails DoUpload { get; set; }
    }

}
