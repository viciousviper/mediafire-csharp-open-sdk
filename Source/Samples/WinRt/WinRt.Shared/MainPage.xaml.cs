using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MediaFireSDK;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Errors;
using WinRt.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WinRt
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SdkTestViewModel _viewModel;

        private const string AppId = "";
        private const string AppKey = "";

        private const string Email = "";
        private const string Password = "";

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
#if WINDOWS_APP
            var config = new MediaFireApiConfiguration
               (
                   appId: AppId,
                   apiKey: AppKey,
                   apiVersion: "1.4",
                   automaticallyRenewToken: true,
                   chunkTransferBufferSize: 1024,
                   useHttpV1:true //workaround on WinRt platform issue
               );
#else
             var config = new MediaFireApiConfiguration
               (
                   appId: AppId,
                   apiKey: AppKey,
                   apiVersion: "1.4",
                   automaticallyRenewToken: true,
                   chunkTransferBufferSize: 1024
               );
#endif

            var agent = new MediaFireAgent(config);

            try
            {
                await agent.User.Authenticate(Email, Password);
                _viewModel = new SdkTestViewModel(agent);
                DataContext = _viewModel;
                _viewModel.LoadUserAndRootFilesCommand.Execute(null);
            }
            catch (MediaFireApiException ex)
            {
                new MessageDialog(ex.Message, ex.Error.ToString()).ShowAsync();
            }

        }

        private void DownloadAndOpenFile(object sender, ItemClickEventArgs e)
        {
            _viewModel.DownloadFileCommand.Execute(e.ClickedItem);
        }
    }
}
