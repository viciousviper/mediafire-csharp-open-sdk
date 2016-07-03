using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaFireSDK.Core;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Errors;
using MediaFireSDK.Model.Responses;

namespace MediaFireSDK.Tests
{
    [TestClass]
    public class MediaFireAgentTests
    {
        private const string AppId = "";
        private const string AppKey = "";

        private const string Email = "";
        private const string Password = "";

        private AuthenticationContext PrepareAuthenticationContest(MediaFireApiConfiguration config)
        {
            var authenticationContext = default(AuthenticationContext);
            using (var agent = new MediaFireAgent(config))
            {
                agent.User.GetSessionToken(Email, Password, TokenVersion.V2).Wait();

                authenticationContext = agent.User.GetAuthenticationContext();
            }

            Assert.IsNotNull(authenticationContext, "Authentication context is null");

            return authenticationContext;
        }

        [TestMethod]
        public void GetSessionToken_Version1_Succeeds()
        {
            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey);

            var token = default(string);
            using (var agent = new MediaFireAgent(config))
            {
                token = agent.User.GetSessionToken(Email, Password).Result;
            }

            Assert.IsNotNull(token, "SessionToken v1 is null");
        }

        [TestMethod]
        public void GetSessionToken_Version2_Succeeds()
        {
            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey);

            var token = default(string);
            using (var agent = new MediaFireAgent(config))
            {
                token = agent.User.GetSessionToken(Email, Password, TokenVersion.V2).Result;
            }

            Assert.IsNotNull(token, "SessionToken v2 is null");
        }

        [TestMethod]
        public void GetFilesAndFolders_WithSessionToken_Version1_Succeeds()
        {
            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey);

            IList<MediaFireFolder> folders;
            IList<MediaFireFile> files;
            using (var agent = new MediaFireAgent(config))
            {
                agent.User.GetSessionToken(Email, Password).Wait();

                folders = agent.GetAsync<MediaFireGetContentResponse>(MediaFireApiFolderMethods.GetContent, new Dictionary<string, object>
                {
                    { MediaFireApiParameters.FolderKey, string.Empty },
                    { MediaFireApiParameters.ContentType, MediaFireFolderContentType.Folders.ToApiParameter() }
                }).Result.FolderContent.Folders;

                files = agent.GetAsync<MediaFireGetContentResponse>(MediaFireApiFolderMethods.GetContent, new Dictionary<string, object>
                {
                    { MediaFireApiParameters.FolderKey, string.Empty },
                    { MediaFireApiParameters.ContentType, MediaFireFolderContentType.Files.ToApiParameter() }
                }).Result.FolderContent.Files;
            }

            Assert.IsNotNull(folders, "Returned folders collection is null");
            Assert.AreNotEqual(0, folders.Count, "No folders found");
            Assert.IsNotNull(files, "Returned files collection is null");
            Assert.AreNotEqual(0, files.Count, "No files found");
        }

        [TestMethod]
        public void GetFilesAndFolders_WithSessionToken_Version2_Succeeds()
        {
            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey);

            IList<MediaFireFolder> folders;
            IList<MediaFireFile> files;
            using (var agent = new MediaFireAgent(config))
            {
                agent.User.GetSessionToken(Email, Password, TokenVersion.V2).Wait();

                folders = agent.GetAsync<MediaFireGetContentResponse>(MediaFireApiFolderMethods.GetContent, new Dictionary<string, object>
                {
                    { MediaFireApiParameters.FolderKey, string.Empty },
                    { MediaFireApiParameters.ContentType, MediaFireFolderContentType.Folders.ToApiParameter() }
                }).Result.FolderContent.Folders;

                files = agent.GetAsync<MediaFireGetContentResponse>(MediaFireApiFolderMethods.GetContent, new Dictionary<string, object>
                {
                    { MediaFireApiParameters.FolderKey, string.Empty },
                    { MediaFireApiParameters.ContentType, MediaFireFolderContentType.Files.ToApiParameter() }
                }).Result.FolderContent.Files;
            }

            Assert.IsNotNull(folders, "Returned folders collection is null");
            Assert.AreNotEqual(0, folders.Count, "No folders found");
            Assert.IsNotNull(files, "Returned files collection is null");
            Assert.AreNotEqual(0, files.Count, "No files found");
        }

        [TestMethod]
        public void GetFilesAndFolders_WithRestoredSessionToken_Version2_Succeeds()
        {
            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey);

            var authenticationContext = PrepareAuthenticationContest(config);

            IList<MediaFireFolder> folders;
            IList<MediaFireFile> files;
            using (var agent = new MediaFireAgent(config))
            {
                agent.User.SetAuthenticationContext(authenticationContext);

                folders = agent.GetAsync<MediaFireGetContentResponse>(MediaFireApiFolderMethods.GetContent, new Dictionary<string, object>
                {
                    { MediaFireApiParameters.FolderKey, string.Empty },
                    { MediaFireApiParameters.ContentType, MediaFireFolderContentType.Folders.ToApiParameter() }
                }).Result.FolderContent.Folders;

                files = agent.GetAsync<MediaFireGetContentResponse>(MediaFireApiFolderMethods.GetContent, new Dictionary<string, object>
                {
                    { MediaFireApiParameters.FolderKey, string.Empty },
                    { MediaFireApiParameters.ContentType, MediaFireFolderContentType.Files.ToApiParameter() }
                }).Result.FolderContent.Files;
            }

            Assert.IsNotNull(folders, "Returned folders collection is null");
            Assert.AreNotEqual(0, folders.Count, "No folders found");
            Assert.IsNotNull(files, "Returned files collection is null");
            Assert.AreNotEqual(0, files.Count, "No files found");
        }

        private void DeleteFolderByPath(IMediaFireAgent agent, string folderPath, bool ignoreErrors = false)
        {
            try
            {
                agent.GetAsync<MediaFireEmptyResponse>(MediaFireApiFolderMethods.Delete, new Dictionary<string, object>() {
                    { MediaFireApiParameters.FolderPath, folderPath }
                }).Wait();
            }
            catch (AggregateException ex)
            {
                if (!ignoreErrors || ex.InnerExceptions.Count != 1 || !(ex.InnerExceptions[0] is MediaFireApiException))
                    throw;
            }
        }

        [TestMethod]
        public void CreateFolder_WithSessionToken_Version1_Succeeds()
        {
            const string folderName = "MediaFireSDKTestFolder";

            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey);

            var folderKey = default(string);
            using (var agent = new MediaFireAgent(config))
            {
                agent.User.GetSessionToken(Email, Password).Wait();

                DeleteFolderByPath(agent, Path.AltDirectorySeparatorChar + folderName, true);

                folderKey = agent.GetAsync<MediaFireCreateFolderResponse>(MediaFireApiFolderMethods.Create, new Dictionary<string, object>() {
                    { MediaFireApiParameters.ParentKey, string.Empty },
                    { MediaFireApiParameters.FolderName, folderName }
                }).Result.FolderKey;

                DeleteFolderByPath(agent, Path.AltDirectorySeparatorChar + folderName);
            }

            Assert.IsFalse(string.IsNullOrEmpty(folderKey), "Folder creation failed");
        }

        [TestMethod]
        public void CreateFolder_WithRestoredSessionToken_Version2_Succeeds()
        {
            const string folderName = "MediaFireSDKTestFolder";

            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey);

            var authenticationContext = PrepareAuthenticationContest(config);

            var folderKey = default(string);
            using (var agent = new MediaFireAgent(config))
            {
                agent.User.SetAuthenticationContext(authenticationContext);

                DeleteFolderByPath(agent, Path.AltDirectorySeparatorChar + folderName, true);

                folderKey = agent.GetAsync<MediaFireCreateFolderResponse>(MediaFireApiFolderMethods.Create, new Dictionary<string, object>() {
                    { MediaFireApiParameters.ParentKey, string.Empty },
                    { MediaFireApiParameters.FolderName, folderName }
                }).Result.FolderKey;

                DeleteFolderByPath(agent, Path.AltDirectorySeparatorChar + folderName);
            }

            Assert.IsFalse(string.IsNullOrEmpty(folderKey), "Folder creation failed");
        }

        private void DeleteFileByPath(IMediaFireAgent agent, string filePath, bool ignoreErrors = false)
        {
            try
            {
                agent.GetAsync<MediaFireEmptyResponse>(MediaFireApiFileMethods.Delete, new Dictionary<string, object>() {
                    { MediaFireApiParameters.FilePath, filePath }
                }).Wait();
            }
            catch (AggregateException ex)
            {
                if (!ignoreErrors || ex.InnerExceptions.Count != 1 || !(ex.InnerExceptions[0] is MediaFireApiException))
                    throw;
            }
        }

        [TestMethod]
        public void CreateFile_WithSessionToken_Version1_Succeeds()
        {
            const string fileName = "MediaFireSDKTestFile.ext";

            var content = Enumerable.Range(0, 20).Select(i => (byte)i).ToArray();

            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey, useHttpV1: true);

            var authenticationContext = PrepareAuthenticationContest(config);

            var fileKey = default(string);
            using (var agent = new MediaFireAgent(config))
            {
                agent.User.GetSessionToken(Email, Password).Wait();

                DeleteFileByPath(agent, Path.AltDirectorySeparatorChar + fileName, true);

                using (var contentStream = new MemoryStream(content))
                {
                    var uploadConfig = agent.Upload.GetUploadConfiguration(fileName, contentStream.Length, string.Empty, MediaFireActionOnDuplicate.Skip).Result;
                    var upload = agent.Upload.Simple(uploadConfig, contentStream, null).Result;

                    while (!(upload.IsComplete && upload.IsSuccess))
                    {
                        Task.Delay(100).Wait();
                        upload = agent.Upload.PollUpload(upload.Key).Result;
                    }

                    fileKey = agent.GetAsync<MediaFireGetFileInfoResponse>(MediaFireApiFileMethods.GetInfo, new Dictionary<string, object>() {
                        { MediaFireApiParameters.QuickKey, upload.QuickKey }
                    }).Result.FileInfo.Key;
                }

                DeleteFileByPath(agent, Path.AltDirectorySeparatorChar + fileName);
            }

            Assert.IsFalse(string.IsNullOrEmpty(fileKey), "File creation failed");
        }

        [TestMethod]
        public void CreateFile_WithRestoredSessionToken_Version2_Succeeds()
        {
            const string fileName = "MediaFireSDKTestFile.ext";

            var content = Enumerable.Range(0, 20).Select(i => (byte)i).ToArray();

            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey, useHttpV1: true);

            var authenticationContext = PrepareAuthenticationContest(config);

            var fileKey = default(string);
            using (var agent = new MediaFireAgent(config))
            {
                agent.User.SetAuthenticationContext(authenticationContext);

                DeleteFileByPath(agent, Path.AltDirectorySeparatorChar + fileName, true);

                using (var contentStream = new MemoryStream(content))
                {
                    var uploadConfig = agent.Upload.GetUploadConfiguration(fileName, contentStream.Length, string.Empty, MediaFireActionOnDuplicate.Skip).Result;
                    var upload = agent.Upload.Simple(uploadConfig, contentStream, null).Result;

                    while (!(upload.IsComplete && upload.IsSuccess))
                    {
                        Task.Delay(100).Wait();
                        upload = agent.Upload.PollUpload(upload.Key).Result;
                    }

                    fileKey = agent.GetAsync<MediaFireGetFileInfoResponse>(MediaFireApiFileMethods.GetInfo, new Dictionary<string, object>() {
                        { MediaFireApiParameters.QuickKey, upload.QuickKey }
                    }).Result.FileInfo.Key;
                }

                DeleteFileByPath(agent, Path.AltDirectorySeparatorChar + fileName);
            }

            Assert.IsFalse(string.IsNullOrEmpty(fileKey), "File creation failed");
        }

        [TestMethod]
        public void DownloadFile_WithSessionToken_Version1_Succeeds()
        {
            const string fileName = "MediaFireSDKTestFile.ext";

            var content = Enumerable.Range(0, 20).Select(i => (byte)i).ToArray();

            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey, useHttpV1: true);

            var result = default(byte[]);
            using (var agent = new MediaFireAgent(config))
            {
                agent.User.GetSessionToken(Email, Password).Wait();

                DeleteFileByPath(agent, Path.AltDirectorySeparatorChar + fileName, true);

                var fileKey = default(string);
                using (var contentStream = new MemoryStream(content))
                {
                    var uploadConfig = agent.Upload.GetUploadConfiguration(fileName, contentStream.Length, string.Empty, MediaFireActionOnDuplicate.Skip).Result;
                    var upload = agent.Upload.Simple(uploadConfig, contentStream, null).Result;

                    while (!(upload.IsComplete && upload.IsSuccess))
                    {
                        Task.Delay(100).Wait();
                        upload = agent.Upload.PollUpload(upload.Key).Result;
                    }

                    fileKey = agent.GetAsync<MediaFireGetFileInfoResponse>(MediaFireApiFileMethods.GetInfo, new Dictionary<string, object>() {
                        { MediaFireApiParameters.QuickKey, upload.QuickKey }
                    }).Result.FileInfo.Key;
                }

                var links = agent.GetAsync<MediaFireGetLinksResponse>(MediaFireApiFileMethods.GetLinks, new Dictionary<string, object>() {
                    { MediaFireApiParameters.QuickKey, fileKey },
                    { MediaFireApiParameters.LinkType, MediaFireApiConstants.LinkTypeDirectDownload }
                }).Result;

                if (!links.Links.Any() && string.IsNullOrEmpty(links.Links[0].DirectDownload))
                    throw new MediaFireException(MediaFireErrorMessages.FileMustContainADirectLink);

                var response = new HttpClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, links.Links[0].DirectDownload), HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).Result;
                response.EnsureSuccessStatusCode();

                var resultStream = response.Content.ReadAsStreamAsync().Result;
                var bufferStream = new MemoryStream();
                resultStream.CopyToAsync(bufferStream).Wait();

                result = bufferStream.GetBuffer().Take((int)bufferStream.Length).ToArray();

                DeleteFileByPath(agent, Path.AltDirectorySeparatorChar + fileName);
            }

            Assert.IsNotNull(result, "File creation failed");
            CollectionAssert.AreEqual(content, result, "File content mismatch");
        }

        [TestMethod]
        public void DownloadFile_WithRestoredSessionToken_Version2_Succeeds()
        {
            const string fileName = "MediaFireSDKTestFile.ext";

            var content = Enumerable.Range(0, 20).Select(i => (byte)i).ToArray();

            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey, useHttpV1: true);

            var authenticationContext = PrepareAuthenticationContest(config);

            var result = default(byte[]);
            using (var agent = new MediaFireAgent(config))
            {
                agent.User.SetAuthenticationContext(authenticationContext);

                DeleteFileByPath(agent, Path.AltDirectorySeparatorChar + fileName, true);

                var fileKey = default(string);
                using (var contentStream = new MemoryStream(content))
                {
                    var uploadConfig = agent.Upload.GetUploadConfiguration(fileName, contentStream.Length, string.Empty, MediaFireActionOnDuplicate.Skip).Result;
                    var upload = agent.Upload.Simple(uploadConfig, contentStream, null).Result;

                    while (!(upload.IsComplete && upload.IsSuccess))
                    {
                        Task.Delay(100).Wait();
                        upload = agent.Upload.PollUpload(upload.Key).Result;
                    }

                    fileKey = agent.GetAsync<MediaFireGetFileInfoResponse>(MediaFireApiFileMethods.GetInfo, new Dictionary<string, object>() {
                        { MediaFireApiParameters.QuickKey, upload.QuickKey }
                    }).Result.FileInfo.Key;
                }

                var links = agent.GetAsync<MediaFireGetLinksResponse>(MediaFireApiFileMethods.GetLinks, new Dictionary<string, object>() {
                    { MediaFireApiParameters.QuickKey, fileKey },
                    { MediaFireApiParameters.LinkType, MediaFireApiConstants.LinkTypeDirectDownload }
                }).Result;

                if (!links.Links.Any() && string.IsNullOrEmpty(links.Links[0].DirectDownload))
                    throw new MediaFireException(MediaFireErrorMessages.FileMustContainADirectLink);

                var response = new HttpClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, links.Links[0].DirectDownload), HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).Result;
                response.EnsureSuccessStatusCode();

                var resultStream = response.Content.ReadAsStreamAsync().Result;
                var bufferStream = new MemoryStream();
                resultStream.CopyToAsync(bufferStream).Wait();

                result = bufferStream.GetBuffer().Take((int)bufferStream.Length).ToArray();

                DeleteFileByPath(agent, Path.AltDirectorySeparatorChar + fileName);
            }

            Assert.IsNotNull(result, "File creation failed");
            CollectionAssert.AreEqual(content, result, "File content mismatch");
        }

        [TestMethod]
        public void RenewSecrectKey_RaisesEvent()
        {
            var config = new MediaFireApiConfiguration(appId: AppId, apiKey: AppKey);

            int eventRaised = 0;

            IList<MediaFireFolder> folders;
            IList<MediaFireFile> files;
            using (var agent = new MediaFireAgent(config))
            {
                agent.User.GetSessionToken(Email, Password, TokenVersion.V2).Wait();
                agent.User.AuthenticationContextChanged += (s, e) => ++eventRaised;

                folders = agent.GetAsync<MediaFireGetContentResponse>(MediaFireApiFolderMethods.GetContent, new Dictionary<string, object>
                {
                    { MediaFireApiParameters.FolderKey, string.Empty },
                    { MediaFireApiParameters.ContentType, MediaFireFolderContentType.Folders.ToApiParameter() }
                }).Result.FolderContent.Folders;

                files = agent.GetAsync<MediaFireGetContentResponse>(MediaFireApiFolderMethods.GetContent, new Dictionary<string, object>
                {
                    { MediaFireApiParameters.FolderKey, string.Empty },
                    { MediaFireApiParameters.ContentType, MediaFireFolderContentType.Files.ToApiParameter() }
                }).Result.FolderContent.Files;
            }

            Assert.AreEqual(2, eventRaised, "AuthenticationContextChange event not raised");
        }
    }
}
