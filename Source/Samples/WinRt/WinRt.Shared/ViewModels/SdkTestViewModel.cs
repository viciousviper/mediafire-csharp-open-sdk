using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using MediaFireSDK;
using MediaFireSDK.Core;
using MediaFireSDK.Model;

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
                                             User = await _agent.User.GetInfo();

                                             var fileContent = await _agent.Folder.GetFolderContent("", MediaFireFolderContentType.Files);

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
                                              await _agent.File.Download(file.QuickKey, fileStream, new Progress<MediaFireOperationProgress>(
                                                  (progress) =>
                                                  {
                                                      DownloadProgress = progress.Percentage.ToString("P");
                                                  }));

                                              Launcher.LaunchFileAsync(storageFile);
                                          }));
            }
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

                                             var uploadDetails = await _agent.File.Upload(
                                                      fileStream,
                                                      file.Name,
                                                      size: 0, // The SDK automatically gets the size of the fileStream 
                                                      folderKey: null,
                                                      progress: new Progress<MediaFireOperationProgress>(
                                                          (progress) =>
                                                          {
                                                              UploadProgress = progress.Percentage.ToString("P");
                                                          }),
                                                     actionOnDuplicate: MediaFireActionOnDuplicate.Replace);

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


