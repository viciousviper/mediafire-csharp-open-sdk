# MediaFire SDK

A portable class library to integrate with the [MediaFire API](http://www.mediafire.com/developers/core_api/1.4/getting_started/).
Supported Platforms:

- >= .NET Framework 4.5
- Windows 8
- Windows Phone Silverlight 8 and 8.1
- Windows Phone 8.1
- Xamarin (Android, iOS and iOS Classic)
- Universal Windows Platform 

## Nuget
We recommend that you use [nuget](https://www.nuget.org/packages/MediaFireSDK/) to get your hands on the library.

    PM> Install-Package MediaFireSDK

## Getting Started

### Configuring the Agent
```c#
var config = new MediaFireApiConfiguration
               (
                   appId: [Your_App_Id],
                   apiKey: [Your_App_Key],
                   apiVersion: "1.4", //desired api version
                   automaticallyRenewToken: true, // Lets the SDK automatically renew the session token.
                   chunkTransferBufferSize: 4096, // The buffer size to be used on Download and Upload operations.
                   useHttpV1:true //On some platforms, the client will throw the error "The server committed a protocol violation. Section=ResponseStatusLine". In that cases set this property to true. 
                   PeriodicallyRenewToken:true // The SDK will create a Task that will renew the token every **SessionRenewPeriod**.
               )
               {
	               SessionRenewPeriod = TimeSpan.FromMinutes(5)
			   };
```

### Authenticating
```c#
const string Email = "";
const string Password = "";

var agent = new MediaFireAgent(config);

await agent.User.Authenticate(Email, Password);


```

### Extending the SDK
Due to the constant API updates, the SDK does not implement every single method supported by the MediaFire API. To allow developers to use the SDK and still reach all available features, under the IMediaFireAgent  class, there are three generic methods available:

```c#
Task<T> GetAsync<T>(string path, IDictionary<string, object> parameters = null, bool attachSessionToken = true) where T : MediaFireResponseBase;

Task<T> PostAsync<T>(string path, IDictionary<string, object> parameters = null, bool attachSessionToken = true) where T : MediaFireResponseBase;

Task<T> PostStreamAsync<T>(
            string path, 
            Stream content, 
            IDictionary<string, object> parameters, 
            IDictionary<string, string> headers, 
            bool attachSessionToken = true,
            CancellationToken? token = null,
            IProgress<MediaFireOperationProgress> progress = null 
            ) where T : MediaFireResponseBase;
```

These methods take advantage of the automatic session renew system and exception handling that the SDK offers. The PostStreamAsync should only be used by upload operations. As an example here is the implementation of the [file/get_links](http://www.mediafire.com/developers/core_api/unversioned/file/#get_links) method:

```c#
public async Task<MediaFireLinkCollection> GetLinks(string quickKey, MediaFireLinkType linkType = MediaFireLinkType.All)
{
	var res = await agent.GetAsync<MediaFireGetLinksResponse>(MediaFireApiFileMethods.GetLinks, new Dictionary<string, object>
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
```

The class [MediaFireConstants](https://github.com/MediaFire/mediafire-csharp-open-sdk/blob/master/Source/MediaFireSDK/MediaFireConstants.cs) contains most of the API parameters (MediaFireApiParameters class) and methods (MediaFireApiFileMethods.XXX, MediaFireApiFolderMethods.XXX, etc). Under [MediaFireSDK.Model](https://github.com/MediaFire/mediafire-csharp-open-sdk/tree/master/Source/MediaFireSDK/Model) namespace there are some predefined responses of the API.

### Uploading
```c#
Stream fileStream = null; // The file stream to upload.
string fileName = "[Target_File_Name]";

var uploadDetails = await agent.Upload.Simple(
						  fileStream,
						  fileName,
						  size: 0, // The SDK automatically gets the size of the fileStream 
						  folderKey: null,
						  progress: new Progress<MediaFireOperationProgress>(
							  (progress) =>
							  {
								  //Report Progress
							  }),
						 actionOnDuplicate: MediaFireActionOnDuplicate.Replace);

do
{ 
  //
  //  Wait until the upload is finalized in the Server.
  //
	if (uploadDetails.IsComplete && uploadDetails.IsSuccess)
	{
	  return;
	}
  await Task.Delay(1000);
	uploadDetails = await _agent.Upload.PollUpload(uploadDetails.Key);
} while (true);
```

### Downloading
```c#
//quickKey the file to download
//destination A writable stream

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
				agent.Configuration.ChunkTransferBufferSize);
	}
}

```


