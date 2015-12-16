using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using MediaFireSDK;
using MediaFireSDK.Core;
using MediaFireSDK.Http;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Errors;
using MediaFireSDK.Model.Responses;

namespace WinRt.ViewModels
{
    public class SdkTestViewModel : INotifyPropertyChanged
    {
        private readonly IMediaFireAgent _agent;

        /// <summary>
        /// The <see cref="User" /> property's name.
        /// </summary>
        public const string UserPropertyName = "User";

        private MediaFireUserDetails _details;

        /// <summary>
        /// Sets and gets the User property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public MediaFireUserDetails User
        {
            get
            {
                return _details;
            }

            set
            {
                if (_details == value)
                {
                    return;
                }

                _details = value;
                RaisePropertyChanged(UserPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="DownloadProgress" /> property's name.
        /// </summary>
        public const string DownloadProgressPropertyName = "DownloadProgress";

        private string _progress;

        /// <summary>
        /// Sets and gets the DownloadProgress property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DownloadProgress
        {
            get
            {
                return _progress;
            }

            set
            {
                _progress = value;
                RaisePropertyChanged(DownloadProgressPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="UploadProgress" /> property's name.
        /// </summary>
        public const string UploadProgressPropertyName = "UploadProgress";

        private string _uprogress;

        /// <summary>
        /// Sets and gets the UploadProgress property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string UploadProgress
        {
            get
            {
                return _uprogress;
            }

            set
            {
                _uprogress = value;
                RaisePropertyChanged(UploadProgressPropertyName);
            }
        }



        public ObservableCollection<MediaFireFile> Files { get; private set; }


        private ICommand _load;

        /// <summary>
        /// Gets the MyCommand.
        /// </summary>
        public ICommand LoadUserAndRootFilesCommand
        {
            get
            {
                return _load
                    ?? (_load = new SafeCommand(
                                         async () =>
                                         {
                                             User = (await _agent.GetAsync<MediaFireGetUserInfoResponse>(MediaFireApiUserMethods.GetInfo)).UserDetails;


                                             var fileContent = (await _agent.GetAsync<MediaFireGetContentResponse>(
                                                 MediaFireApiFolderMethods.GetContent,
                                                 new Dictionary<string, object>
                                                 {
                                                     {MediaFireApiParameters.FolderKey, ""},
                                                     {MediaFireApiParameters.ContentType, MediaFireFolderContentType.Files.ToApiParameter()}
                                                 })).FolderContent;

                                             foreach (var mediaFireFile in fileContent.Files)
                                             {
                                                 Files.Add(mediaFireFile);
                                             }

                                         }));
            }
        }

        private ICommand _downloadFileCommand;

        /// <summary>
        /// Gets the DownloadFileCommand.
        /// </summary>
        public ICommand DownloadFileCommand
        {
            get
            {
                return _downloadFileCommand
                    ?? (_downloadFileCommand = new SafeCommand<MediaFireFile>(
                                          async (file) =>
                                          {
                                              var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(file.FileName, CreationCollisionOption.OpenIfExists);

                                              var fileStream = (await storageFile.OpenAsync(FileAccessMode.ReadWrite)).AsStreamForWrite();

                                              await
                                                  Download(file.QuickKey, fileStream, CancellationToken.None, (new Progress<MediaFireOperationProgress>((progress) =>
                                                  {
                                                      DownloadProgress = progress.Percentage.ToString("P");
                                                  })));

                                              Launcher.LaunchFileAsync(storageFile);
                                          }));
            }
        }


        public async Task Download(string quickKey, Stream destination, CancellationToken token, IProgress<MediaFireOperationProgress> progress = null)
        {
            var links = await GetLinks(quickKey, MediaFireLinkType.DirectDownload);
            if (links.Count != 0 && string.IsNullOrEmpty(links[0].DirectDownload))
                throw new MediaFireException(MediaFireErrorMessages.FileMustContainADirectLink);


            var fileLink = links[0].DirectDownload;

            if (progress == null)
                progress = new Progress<MediaFireOperationProgress>();


            var cli = new HttpClient();
            var resp = await cli.SendAsync(new HttpRequestMessage(HttpMethod.Get, fileLink), HttpCompletionOption.ResponseHeadersRead, token);

            resp.EnsureSuccessStatusCode();

            var stream = await resp.Content.ReadAsStreamAsync();

            var progressData = new MediaFireOperationProgress
            {
                TotalSize = resp.Content.Headers.ContentLength.Value
            };

            using (destination)
            {
                await
                    MediaFireHttpHelpers.CopyStreamWithProgress(stream, destination, progress, token, progressData,
                        _agent.Configuration.ChunkTransferBufferSize);
            }
        }

        public async Task<MediaFireLinkCollection> GetLinks(string quickKey, MediaFireLinkType linkType = MediaFireLinkType.All)
        {
            var res = await _agent.GetAsync<MediaFireGetLinksResponse>(MediaFireApiFileMethods.GetLinks, new Dictionary<string, object>
            {
                {MediaFireApiParameters.QuickKey, quickKey},
                {MediaFireApiParameters.LinkType, linkType.ToApiParameter()},
            });

            var col = new MediaFireLinkCollection(res.Links)
            {
                DirectDownloadFreeBandwidth = res.DirectDownloadFreeBandwidth,
                OneTimeDownloadRequestCount = res.OneTimeDownloadRequestCount,
                OneTimeKeyRequestCount = res.OneTimeKeyRequestCount,
                OneTimeKeyRequestMaxCount = res.OneTimeKeyRequestMaxCount
            };

            return col;
        }

        private ICommand _uploadCommand;

        /// <summary>
        /// Gets the UploadCommand.
        /// </summary>
        public ICommand UploadCommand
        {
            get
            {
                return _uploadCommand
                    ?? (_uploadCommand = new SafeCommand(
                                         async () =>
                                         {
                                             const string uploadFilePath = "ms-appx:///MediaFireLogo.psd";

                                             var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uploadFilePath));

                                             var fileStream = (await file.OpenReadAsync()).AsStreamForRead();


                                             var uploadConfig = await _agent.Upload.GetUploadConfiguration(file.Name, fileStream.Length, null, MediaFireActionOnDuplicate.Skip);

                                             var uploadDetails = await _agent.Upload.Simple(uploadConfig, fileStream,
                                                 new Progress<MediaFireOperationProgress>(
                                                     (progress) =>
                                                     {
                                                         UploadProgress = progress.Percentage.ToString("P");
                                                     }),
                                                 CancellationToken.None);

                                             
                                             do
                                             {
                                                 if (uploadDetails.IsComplete && uploadDetails.IsSuccess)
                                                 {
                                                     new MessageDialog("Upload Finished file key: " + uploadDetails.QuickKey).ShowAsync();
                                                     return;
                                                 }

                                                 uploadDetails = await _agent.Upload.PollUpload(uploadDetails.Key);
                                             } while (true);

                                         }));

            }
        }

        public SdkTestViewModel()
        {

        }
        public SdkTestViewModel(IMediaFireAgent agent)
        {
            _agent = agent;
            Files = new ObservableCollection<MediaFireFile>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    public class SafeCommand : ICommand
    {

        private readonly Func<Task> _action;

        public SafeCommand(Func<Task> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            try
            {
                await _action();
            }
            catch (Exception e)
            {
                new MessageDialog(e.ToString()).ShowAsync();
            }
        }

        public event EventHandler CanExecuteChanged;
    }

    public class SafeCommand<T> : ICommand
    {

        private readonly Func<T, Task> _action;

        public SafeCommand(Func<T, Task> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            try
            {
                await _action((T)parameter);
            }
            catch (Exception e)
            {
                new MessageDialog(e.ToString()).ShowAsync();
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}


