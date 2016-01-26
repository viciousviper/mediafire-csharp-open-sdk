using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK;
using MediaFireSDK.Model;


namespace ResumableUpload
{
    class Sample
    {
        private const string AppId = "";
        private const string AppKey = "";

        private const string Email = "";
        private const string Password = "";


        //
        //  The file to be uploaded
        //
        private const string FilePath = @"C:\Users\amazi\Desktop\Down Boyl2.mp3";

        static void Main(string[] args)
        {
            ServicePointManager.Expect100Continue = false;
            var config = new MediaFireApiConfiguration
               (
                   appId: AppId,
                   apiKey: AppKey,
                   apiVersion: "1.5",
                   automaticallyRenewToken: true,
                   chunkTransferBufferSize: 1024
               );

            _agent = new MediaFireAgent(config);
            Main().Wait();
        }


        private static IMediaFireAgent _agent;
        static async Task Main()
        {
            Console.WriteLine("Signing in {0}...", Email);
            await _agent.User.GetSessionToken(Email, Password);

            var fileInfo = GetFile();

            var resumableController = new MfResumableFileUploadController(_agent, fileInfo.Length, fileInfo.OpenRead(), fileInfo.Name);
            resumableController.CalculateFileHash();


            var checkDetails = await _agent.Upload.Check(fileInfo.Name, resumableController.FileLength, hash: resumableController.FileHash, resumable: true);

            if (checkDetails.FileExists)
            {
                Console.WriteLine("File already exists in the server, please choose another one");
                return;
            }

            Console.WriteLine("Unit Size:\t{0}", checkDetails.ResumableUpload.UnitSize);
            Console.WriteLine("Nr Of Units:\t{0}", checkDetails.ResumableUpload.NumberOfUnits);



            await resumableController.Upload(checkDetails.ResumableUpload);

        }


        private static FileInfo GetFile()
        {
            var finfo = new FileInfo(FilePath);

            Console.WriteLine("File: {0}\t\n", finfo.Name);
            Console.WriteLine("\t\t(bytes)");

            Console.WriteLine("TotalSize:\t{0}\n", finfo.Length);

            return finfo;
        }

    }
}
