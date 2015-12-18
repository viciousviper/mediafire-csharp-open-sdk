namespace MediaFireSDK
{

    /// Core Api Contants
    public static class MediaFireApiConstants
    {
        internal const string BaseUrl = "https://www.mediafire.com/api";
        public const string PublicFolderUrlFormat = "https://www.mediafire.com/folder/{0}";
        internal const string BaseUrlWithVersionFormat = "{0}/{1}/";
        internal const string JsonFormat = "json";

        public const string MediaFireNo = "no";
        public const string MediaFireYes = "yes";

        public const string LinkTypeView = "view";
        public const string LinkTypeEdit = "edit";
        public const string LinkTypeNormalDownload = "normal_download";
        public const string LinkTypeDirectDownload = "direct_download";
        public const string LinkTypeOneTimeDownload = "one_time_download";
        public const string LinkTypeListen = "listen";
        public const string LinkTypeWatch = "watch";
        public const string LinkTypeStreaming = "streaming";


        public const string MediaFireFile = "file";
        public const string MediaFireFolder = "folder";

        public const string ContentTypeHeader = "Content-Type";
        public const string SimpleUploadContentTypeValue = "application/octet-stream";
        public const string ResumableUploadContentTypeValue = SimpleUploadContentTypeValue;


        public const string FileNameHeader = "x-filename";
        public const string FileSizeHeader = "x-filesize";
        public const string FileHashHeader = "x-filehash";

        public const string UnitIdHeader = "x-unit-id";
        public const string UnitSizeHeader = "x-unit-size";
        public const string UnitHashHeader = "x-unit-hash";



        public const string UserAcceptedTermsValue = MediaFireYes;

        public const string CameraFolderName = "Camera";
        public const int PasswordMinimumChars = 6;

        public const string PublicPrivacy = "public";
        public const string PrivatePrivacy = "private";
    }

    /// 
    /// Api Methods contants.
    /// 

    public static class MediaFireApiBillingMethods
    {
        public const string BillingMethodsPath = "billing/";
        public const string CancelPlan = BillingMethodsPath + "cancel_plan.php";
        public const string ChangePlan = BillingMethodsPath + "change_plan.php";
        public const string GetInvoice = BillingMethodsPath + "get_invoice.php";
        public const string GetPlans = BillingMethodsPath + "get_plans.php";
        public const string GetProducts = BillingMethodsPath + "get_products.php";
        public const string PurchasePlan = BillingMethodsPath + "purchase_plan.php";

    }

    public static class MediaFireApiContactMethods
    {
        public const string ContactMethodsPath = "contact/";
        public const string Add = ContactMethodsPath + "add.php";
        public const string Delete = ContactMethodsPath + "delete.php";
        public const string Fetch = ContactMethodsPath + "fetch.php";
        public const string GetAvatar = ContactMethodsPath + "get_avatar.php";
        public const string GetSources = ContactMethodsPath + "get_sources.php";
        public const string SetAvatar = ContactMethodsPath + "set_avatar.php";
        public const string Summary = ContactMethodsPath + "summary.php";
    }

    public static class MediaFireApiDeviceMethods
    {

        public const string DeviceMethodsPath = "device/";
        public const string AddDevice = DeviceMethodsPath + "add_device.php";
        public const string EmptyTrash = DeviceMethodsPath + "empty_trash.php";
        public const string FollowResource = DeviceMethodsPath + "follow_resource.php";
        public const string GetChanges = DeviceMethodsPath + "get_changes.php";
        public const string GetForeignChanges = DeviceMethodsPath + "get_foreign_changes.php";
        public const string GetForeignResources = DeviceMethodsPath + "get_foreign_resources.php";
        public const string GetPatch = DeviceMethodsPath + "get_patch.php";
        public const string GetResourceShares = DeviceMethodsPath + "get_resource_shares.php";
        public const string GetStatus = DeviceMethodsPath + "get_status.php";
        public const string GetTrash = DeviceMethodsPath + "get_trash.php";
        public const string GetUpdates = DeviceMethodsPath + "get_updates.php";
        public const string GetUserShares = DeviceMethodsPath + "get_user_shares.php";
        public const string RemoveDevice = DeviceMethodsPath + "remove_device.php";
        public const string RequestForeignResource = DeviceMethodsPath + "request_foreign_resource.php";
        public const string ShareResources = DeviceMethodsPath + "share_resources.php";
        public const string UnfollowResource = DeviceMethodsPath + "unfollow_resource.php";
        public const string UnshareResources = DeviceMethodsPath + "unshare_resources.php";
    }

    public static class MediaFireApiFileMethods
    {

        public const string FileMethodsPath = "file/";
        public const string ConfigureOneTimeKey = FileMethodsPath + "configure_one_time_key.php";
        public const string Copy = FileMethodsPath + "copy.php";
        public const string Create = FileMethodsPath + "create.php";
        public const string Delete = FileMethodsPath + "delete.php";
        public const string GetInfo = FileMethodsPath + "get_info.php";
        public const string GetLinks = FileMethodsPath + "get_links.php";
        public const string GetVersions = FileMethodsPath + "get_versions.php";
        public const string Move = FileMethodsPath + "move.php";
        public const string OneTimeKey = FileMethodsPath + "one_time_key.php";
        public const string Purge = FileMethodsPath + "purge.php";
        public const string RecentlyModified = FileMethodsPath + "recently_modified.php";
        public const string Restore = FileMethodsPath + "restore.php";
        public const string SetArtwork = FileMethodsPath + "set_artwork.php";
        public const string Update = FileMethodsPath + "update.php";
        public const string UpdateFile = FileMethodsPath + "update_file.php";
        public const string Zip = FileMethodsPath + "zip.php";
    }


    public static class MediaFireApiFolderMethods
    {

        public const string FolderMethodsPath = "folder/";
        public const string ConfigureFiledrop = FolderMethodsPath + "configure_filedrop.php";
        public const string Copy = FolderMethodsPath + "copy.php";
        public const string Create = FolderMethodsPath + "create.php";
        public const string Delete = FolderMethodsPath + "delete.php";
        public const string GetContent = FolderMethodsPath + "get_content.php";
        public const string GetDepth = FolderMethodsPath + "get_depth.php";
        public const string GetInfo = FolderMethodsPath + "get_info.php";
        public const string GetSiblings = FolderMethodsPath + "get_siblings.php";
        public const string Move = FolderMethodsPath + "move.php";
        public const string Purge = FolderMethodsPath + "purge.php";
        public const string Search = FolderMethodsPath + "search.php";
        public const string Update = FolderMethodsPath + "update.php";
    }

    public static class MediaFireApiNotificationsMethods
    {

        public const string NotificationsMethodsPath = "notifications/";
        public const string GetCache = NotificationsMethodsPath + "get_cache.php";
        public const string PeekCache = NotificationsMethodsPath + "peek_cache.php";
        public const string SendMessage = NotificationsMethodsPath + "send_message.php";
        public const string SendNotification = NotificationsMethodsPath + "send_notification.php";
    }


    public static class MediaFireApiSystemMethods
    {
        public const string SystemMethodsPath = "system/";
        public const string GetInfo = SystemMethodsPath + "get_info.php";
        public const string GetEditableMedia = SystemMethodsPath + "get_editable_media.php";
        public const string GetLimits = SystemMethodsPath + "get_limits.php";
        public const string GetMimeTypes = SystemMethodsPath + "get_mime_types.php";
        public const string GetVersion = SystemMethodsPath + "get_version.php";
    }

    public static class MediaFireApiUploadMethods
    {
        public const string UploadMethodsPath = "upload/";
        public const string Simple = UploadMethodsPath + "simple.php";
        public const string PollUpload = UploadMethodsPath + "poll_upload.php";
        public const string Check = UploadMethodsPath + "check.php";
        public const string AddWebUpload = UploadMethodsPath + "add_web_upload.php";
        public const string GetOptions = UploadMethodsPath + "get_options.php";
        public const string GetWebUploads = UploadMethodsPath + "get_web_uploads.php";
        public const string Instant = UploadMethodsPath + "instant.php";
        public const string Patch = UploadMethodsPath + "patch.php";
        public const string Resumable = UploadMethodsPath + "resumable.php";
        public const string SetOptions = UploadMethodsPath + "set_options.php";
        public const string Update = UploadMethodsPath + "update.php";
    }

    public static class MediaFireApiUserMethods
    {
        public const string UserMethodsPath = "user/";
        public const string GetSessionToken = UserMethodsPath + "get_session_token.php";
        public const string RenewSessionToken = UserMethodsPath + "renew_session_token.php";
        public const string GetInfo = UserMethodsPath + "get_info.php";
        public const string Register = UserMethodsPath + "register.php";
        public const string FetchTos = UserMethodsPath + "fetch_tos.php";
        public const string AcceptTos = UserMethodsPath + "accept_tos.php";

        public const string DestroyActionToken = UserMethodsPath + "destroy_action_token.php";
        public const string GetActionToken = UserMethodsPath + "get_action_token.php";
        public const string GetAvatar = UserMethodsPath + "get_avatar.php";
        public const string GetDevices = UserMethodsPath + "get_devices.php";
        public const string GetLimits = UserMethodsPath + "get_limits.php";
        public const string GetSettings = UserMethodsPath + "get_settings.php";
        public const string SetAvatar = UserMethodsPath + "set_avatar.php";
        public const string SetSettings = UserMethodsPath + "set_settings.php";
        public const string Update = UserMethodsPath + "update.php";

    }








    public static class MediaFireApiParameters
    {
        public const string Signature = "signature";
        public const string ResponseFormat = "response_format";
        public const string SessionToken = "session_token";
        public const string AppId = "application_id";
        public const string Password = "password";
        public const string Email = "email";
        public const string FolderName = "foldername";
        public const string FileName = "filename";
        public const string Description = "description";
        public const string Truncate = "truncate";
        public const string ParentKey = "parent_key";
        public const string ActionOnDuplicate = "action_on_duplicate ";
        public const string Key = "key";
        public const string ContentType = "content_type";
        public const string ContentTypeFileType = "files";
        public const string ContentTypeFolderType = "folders";

        public const string FolderKey = "folder_key";
        public const string FolderKeySource = "folder_key_src";
        public const string FolderKeyDestination = "folder_key_dst";
        public const string QuickKey = "quick_key";
        public const string SearchText = "search_text";
        public const string FirstName = "first_name";
        public const string LastName = "last_name";
        public const string DisplayName = "display_name";

        public const string AcceptanceToken = "acceptance_token";

        public const string Filter = "filter";
        public const string DeviceId = "device_id";
        public const string OrderBy = "order_by";
        public const string OrderDirection = "order_direction";
        public const string Chunk = "chunk";
        public const string ChunkSize = "chunk_size";
        public const string Details = "details";
        public const string LinkType = "link_type";


        public const string ModificationTime = "mtime";
        public const string Privacy = "privacy";
        public const string PrivacyRecursive = "privacy_recursive ";

        public const string SearchAll = "search_all";

        public const string Hash = "hash";
        public const string Size = "size";
        public const string Resumable = "resumable";
    }

    public static class MediaFireErrorMessages
    {
        public const string RequestMustBeAuthenticated = "Please authenticate before making this request.";
        public const string FileMustContainADirectLink = "File must contain a direct link to be able to download.";
        public const string UploadErrorFormat = "Upload Error {0}";
        public const string CheckParamsError = "If the fileName parameter is not setted, the hash and file size are required.";
    }

    public static class MediaFireUploadResult
    {
        public const int Success = 0;
        public const int InvalidUploadKey = -20;
        public const int KeyNotFound = -80;

    }

    public static class MediaFireUploadStatus
    {
        public const int NoStatusAvailable = 0;
        public const int KeyReadyToUse = 2;
        public const int UploadInProgress = 3;
        public const int UploadCompleted = 4;
        public const int WaitingForVerification = 5;
        public const int VerifyingFile = 6;
        public const int FinishedVerification = 11;
        public const int UploadIsInProgress = 17;
        public const int WaitingForAssembly = 18;
        public const int AssemblingFile = 19;
        public const int NoMoreRequestsForThisKey = 99;
    }

    public static class MediaFireApiErrorCodes
    {
        public const int Internal = 101;
        public const int MissingKey = 102;
        public const int InvalidKey = 103;
        public const int MissingToken = 104;
        public const int InvalidToken = 105;
        public const int ChangeExtension = 106;
        public const int InvalidCredentials = 107;
        public const int InvalidUser = 108;
        public const int InvalidAppid = 109;
        public const int InvalidQuickkey = 110;
        public const int MissingQuickkey = 111;
        public const int InvalidFolderkey = 112;
        public const int MissingFolderkey = 113;
        public const int AccessDenied = 114;
        public const int FolderPathConflict = 115;
        public const int InvalidDate = 116;
        public const int MissingFoldername = 117;
        public const int InvalidFilename = 118;
        public const int NoMfEmail = 119;
        public const int EmailTaken = 120;
        public const int EmailRejected = 121;
        public const int EmailMisformatted = 122;
        public const int PasswordMisformatted = 123;
        public const int ApiVersionMissing = 124;
        public const int OldApiVersion = 125;
        public const int ApiCallDeprecated = 126;
        public const int InvalidSignature = 127;
        public const int MissingParams = 128;
        public const int InvalidParams = 129;
        public const int NonProLimitReached = 130;
        public const int AddOwnedFolder = 131;
        public const int RemoveOwnedFolder = 132;
        public const int AddAnonFolder = 133;
        public const int ContactAlreadyExists = 137;
        public const int ContactDoesNotExist = 138;
        public const int ContactGroupExists = 139;
        public const int UnknownContactGroup = 140;
        public const int UnknownDevice = 141;
        public const int InvalidFileType = 142;
        public const int FileAlreadyExists = 143;
        public const int FolderAlreadyExists = 144;
        public const int ApplicationDisabled = 145;
        public const int ApplicationSuspended = 146;
        public const int ZipMultipleOwners = 147;
        public const int ZipNonProDownload = 148;
        public const int ZipOwnerNotPro = 149;
        public const int ZipFileTooBig = 150;
        public const int ZipNoFilesSelected = 151;
        public const int ZipNoFilesZipped = 152;
        public const int ZipTotalSizeTooBig = 153;
        public const int ZipNumFilesExceeded = 154;
        public const int ZipOwnerInsufficientBandwidth = 155;
        public const int ZipRequesterInsufficientBandwidth = 156;
        public const int ZipAllInsufficientBandwidth = 157;
        public const int FileExists = 158;
        public const int FolderExists = 159;
        public const int InvalidAcceptanceToken = 160;
        public const int UserMustAcceptTos = 161;
        public const int LimitExceeded = 162;
        public const int AccessLimitReached = 163;
        public const int DmcaAlreadyReported = 164;
        public const int DmcaAlreadyRemoved = 165;
        public const int AddPrivateFolder = 166;
        public const int FolderDepthLimit = 167;
        public const int InvalidProductId = 168;
        public const int UploadFailed = 169;
        public const int TargetPlanNotInTheSameClass = 170;
        public const int BizPlanRestriction = 171;
        public const int ExpirationDateRestriction = 172;
        public const int NotPremiumUser = 173;
        public const int InvalidUrl = 174;
        public const int InvalidUploadKey = 175;
        public const int StorageLimitRestriction = 176;
        public const int DuplicateEntry = 177;
        public const int ProductIdMatch = 178;
        public const int NotCurrentProduct = 179;
        public const int BizDowngrade = 180;
        public const int BusinessUpgrade = 181;
        public const int ChangePlanCredit = 182;
        public const int BandwidthError = 183;
        public const int AlreadyLinked = 184;
        public const int InvalidFoldername = 185;
        public const int ZipPasswordBulk = 186;
        public const int ServerNotFound = 187;
        public const int NotLoggedIn = 188;
        public const int ResellerTos = 189;
        public const int BusinessSeat = 190;
        public const int BannedBuyer = 191;
        public const int ResellerCreditsError = 192;
        public const int PurchaseBannedError = 193;
        public const int SubdomainError = 194;
        public const int TooManyFailed = 195;
        public const int InvalidCard = 196;
        public const int RecentSubscriber = 197;
        public const int InvoiceFailed = 198;
        public const int DuplicateApiTransaction = 199;
        public const int CardccvError = 200;
        public const int TransactionDeclined = 201;
        public const int PrepaidCard = 202;
        public const int CardStoreFailed = 206;
        public const int CopyLimitExceeded = 207;
        public const int AsyncJobInProgress = 208;
        public const int FolderAlreadyDeleted = 209;
        public const int FileAlreadyDeleted = 210;
        public const int CantModifyDeletedItems = 211;
        public const int ChangeFromFree = 212;
        public const int InvalidFiledropKey = 214;
        public const int MissingSignature = 215;
        public const int EmailAddressTooShort = 216;
        public const int EmailAddressTooLong = 217;
        public const int FbEmailMissing = 218;
        public const int FbEmailExists = 219;
        public const int AuthFacebook = 220;
        public const int AuthTwitter = 221;
        public const int UnknownPatch = 222;
        public const int InvalidRevision = 223;
        public const int NoActiveInvoice = 224;
        public const int ApplicationNoLogging = 225;
        public const int InvalidInstallationId = 226;
        public const int IncidentMismatch = 227;
        public const int MissingFacebookToken = 228;
        public const int MissingTwitterToken = 229;
        public const int NoAvatar = 230;
        public const int InvalidSoftwareToken = 231;
        public const int EmailNotValidated = 232;
        public const int AuthGmail = 233;
        public const int FailedToSendMessage = 234;
        public const int UserIsOwner = 235;
        public const int UserIsFollower = 236;
        public const int UserNotFollower = 237;
        public const int PatchNoChange = 238;
        public const int ShareLimitReached = 239;
        public const int CannotGrantPerms = 240;
        public const int InvalidPrintService = 241;
        public const int FolderFilesExceeded = 242;
        public const int AccountTemporarilyLocked = 243;
        public const int NonUsUser = 244;
        public const int InvalidService = 245;
        public const int ChangeFromAffiliate = 246;
        public const int ChangeFromApple = 247;
        public const int AppNotAuthenticated = 248;
        public const int InvalidReceiptData = 249;
        public const int InvalidTransactionId = 250;
        public const int UsedTransactionId = 251;
        public const int TokenAlreadyUpgraded = 252;
        public const int UnknownApi = 253;
        public const int ListAlreadyExists = 254;
        public const int UnkownList = 255;
        public const int InvalidImportService = 256;
        public const int CardReuseForbidden = 257;

    }
}