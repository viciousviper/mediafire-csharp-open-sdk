namespace MediaFireSDK.Model
{
    /// <summary>
    /// Represent MediaFire Privacy options
    /// </summary>
    public enum MediaFirePrivacy
    {
        Private,
        Public,
    }

    /// <summary>
    /// Represent the content to be returned on GetContent folder method.
    /// </summary>
    public enum MediaFireFolderContentType
    {
        Folders,
        Files
    }

    /// <summary>
    /// Represents which property should the API use to order the result.
    /// </summary>
    public enum MediaFireOrderQuery
    {
        Name,
        Created,
        Size,
        Downloads
    }

    /// <summary>
    /// Represents which type(s) should the API filter the result.
    /// </summary>
    public enum MediaFireContentFilter
    {
        Public,
        Private,
        Image,
        Video,
        Audio,
        Document,
        Spreadsheet,
        Presentation,
        Application,
        Archive,
        Data,
        Development,
    }

    /// <summary>
    /// The order direction of a query request.
    /// </summary>
    public enum MediaFireOrderDirection
    {
        Asc,
        Desc
    }

    /// <summary>
    /// MediaFire supported link types.
    /// </summary>
    public enum MediaFireLinkType
    {
        All,
        View,
        Edit,
        NormalDownload,
        DirectDownload,
        OneTimeDownload,
        Listen,
        Watch,
        Streaming
    }

    /// <summary>
    /// The action the API should take when two items have the same name.
    /// </summary>
    public enum MediaFireActionOnDuplicate
    {
        Keep,
        Skip,
        Replace
    }

    /// <summary>
    /// MediaFire image size for image convertion.
    /// </summary>
    public enum MediaFireSupportedImageSize : byte
    {
        Size32X32 = 0x30,
        Size107X80 = 0x31,
        Size191X145 = 0x32,
        Size335X251 = 0x33,
        Size515X386 = 0x34,
        Size800X600 = 0x35,
        Size1024X786 = 0x36,
        Size1280X800 = 0x37,
        Size1600X1200 = 0x38,
        Size1680X1050 = 0x39,
        Size1920X1080 = 0x61,
        Size2240X1680 = 0x62,
        Size2560X1920 = 0x63,
        Size3072X2304 = 0x64,
        Size3264X2448 = 0x65,
        Size4064X2704 = 0x66,
        Size4000X4000 = 0x7a,
    }

}
