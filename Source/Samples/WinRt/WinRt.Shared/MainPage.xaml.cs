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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MediaFireSDK;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Errors;
using MediaFireSDK.Multimedia;
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
        private IMediaFireAgent _agent;

        private const string AppId = "48092";
        private const string AppKey = "djufwhhhb92d7dfrtr83nir92tu6dyr6i36ittuc";

        private const string Email = "amazing.red@gmail.com";
        private const string Password = "testpwd";

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

            _agent = new MediaFireAgent(config);

            try
            {
                await _agent.User.GetSessionToken(Email, Password);
                _viewModel = new SdkTestViewModel(_agent);
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

        private void LoadThumbnail(object sender, RoutedEventArgs e)
        {
            var img = sender as Image;
            var file = img.DataContext as MediaFireFile;

            var imgUrl = file.GetThumbnail(MediaFireSupportedImageSize.Size107X80, _agent.Image);

            img.Source = new BitmapImage(new Uri(imgUrl));
        }
    }
}
