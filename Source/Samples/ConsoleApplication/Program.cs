using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Responses;

namespace ConsoleApplication
{
    class Program
    {

        private const string AppId = "";
        private const string AppKey = "";

        private const string Email = "";
        private const string Password = "";

        static void Main(string[] args)
        {
            var config = new MediaFireApiConfiguration
                (
                    appId: AppId,
                    apiKey: AppKey,
                    apiVersion: "1.4",
                    automaticallyRenewToken: true,
                    chunkTransferBufferSize:1024
                );

            var agent = new MediaFireAgent(config);

            Console.WriteLine("Signing in {0}...", Email);
            agent.User.GetSessionToken(Email, Password).Wait();


            Console.WriteLine("Getting root folder files and folders...", Email);

            var folderContent = agent.GetAsync<MediaFireGetContentResponse>(MediaFireApiFolderMethods.GetContent,
                new Dictionary<string, object>
                {
                    {MediaFireApiParameters.FolderKey, ""},
                    {MediaFireApiParameters.ContentType, MediaFireFolderContentType.Folders.ToApiParameter()}
                }).Result.FolderContent;

            var fileContent = agent.GetAsync<MediaFireGetContentResponse>(MediaFireApiFolderMethods.GetContent,
               new Dictionary<string, object>
                {
                    {MediaFireApiParameters.FolderKey, ""},
                    {MediaFireApiParameters.ContentType, MediaFireFolderContentType.Files.ToApiParameter()}
                }).Result.FolderContent;





            Console.WriteLine("Key | Name");
            foreach (var item in folderContent.Folders.Union<MediaFireItem>(fileContent.Files))
            {
                Console.WriteLine("{0} | {1}", item.Key, item.Name);
            }

           

        }
    }
}
