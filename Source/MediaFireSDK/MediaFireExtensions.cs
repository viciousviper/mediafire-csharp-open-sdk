using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK.Model;

namespace MediaFireSDK
{
    internal static class MediaFireExtensions
    {
        /// <summary>
        /// Transforms a yes/no string to a bool.
        /// </summary>
        /// <param name="value">yes or no strings</param>
        public static bool FromMediaFireYesNo(this string value)
        {
            return value.Equals(MediaFireApiConstants.MediaFireYes, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Transforms a bool to a yes/no string.
        /// </summary>
        /// <param name="value">yes or no strings</param>
        public static string ToMediaFireYesNo(this bool? value)
        {
            return value == null ? String.Empty : value.Value.ToMediaFireYesNo();
        }

        /// <summary>
        /// Transforms a bool to a yes/no string.
        /// </summary>
        /// <param name="value">yes or no strings</param>
        public static string ToMediaFireYesNo(this bool value)
        {
            return value ? MediaFireApiConstants.MediaFireYes : MediaFireApiConstants.MediaFireNo;
        }

        /// <summary>
        /// Transforms the enum MediaFireActionOnDuplicate to a MediaFire api parameter.
        /// </summary>
        /// <param name="actionOnDuplicate">The parameter to be converted.</param>
        internal static string ToApiParamenter(this MediaFireActionOnDuplicate actionOnDuplicate)
        {
            return actionOnDuplicate.ToString().ToLower();
        }

        /// <summary>
        /// Transforms the enum MediaFirePrivacy to a MediaFire api parameter.
        /// </summary>
        /// <param name="privacy">The parameter to be converted.</param>
        internal static string ToApiParameter(this MediaFirePrivacy? privacy)
        {
            if (privacy.HasValue == false)
                return String.Empty;

            return privacy.Value == MediaFirePrivacy.Public
                ? MediaFireApiConstants.PublicPrivacy
                : MediaFireApiConstants.PrivatePrivacy;
        }

        /// <summary>
        /// Transforms the enum MediaFireLinkType to a MediaFire api parameter.
        /// </summary>
        /// <param name="linkType">The parameter to be converted.</param>
        internal static string ToApiParameter(this MediaFireLinkType linkType)
        {
            var type = String.Empty;
            switch (linkType)
            {
                case MediaFireLinkType.DirectDownload:
                    type = MediaFireApiConstants.LinkTypeDirectDownload;
                    break;
                case MediaFireLinkType.Edit:
                    type = MediaFireApiConstants.LinkTypeEdit;
                    break;
                case MediaFireLinkType.Listen:
                    type = MediaFireApiConstants.LinkTypeListen;
                    break;
                case MediaFireLinkType.NormalDownload:
                    type = MediaFireApiConstants.LinkTypeNormalDownload;
                    break;
                case MediaFireLinkType.OneTimeDownload:
                    type = MediaFireApiConstants.LinkTypeOneTimeDownload;
                    break;
                case MediaFireLinkType.Streaming:
                    type = MediaFireApiConstants.LinkTypeStreaming;
                    break;
                case MediaFireLinkType.View:
                    type = MediaFireApiConstants.LinkTypeView;
                    break;
                case MediaFireLinkType.All:
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return type;
        }
    }
}
