using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK;
using MediaFireSDK.Model;

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
            agent.User.Authenticate(Email, Password).Wait();


            Console.WriteLine("Getting root folder files and folders...", Email);
            var folderContent = agent.Folder.GetFolderContent("", MediaFireFolderContentType.Folders).Result;
            var fileContent = agent.Folder.GetFolderContent("", MediaFireFolderContentType.Files).Result;

            Console.WriteLine("Key | Name");
            foreach (var item in folderContent.Folders.Union<MediaFireItem>(fileContent.Files))
            {
                Console.WriteLine("{0} | {1}", item.Key, item.Name);
            }

           

        }
    }
}
