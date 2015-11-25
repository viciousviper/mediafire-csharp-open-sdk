using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using MediaFireSDK.Model;
using MediaFireSDK.Multimedia;

namespace WinRt
{
    class MediaFireFileToThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var file = value as MediaFireFile;

            if (file == null) return string.Empty;

            return file.GetThumbnail(MediaFireSupportedImageSize.Size107X80);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
