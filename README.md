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
We recommend that you use [nuget](nuget.org) to get your hands on the library.

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
                   useHttpV1:true //workaround on WinRt platform issue, should only be true on WinRt (Windows 8.1). 
               );
```

### Authenticating
```c#
const string Email = "";
const string Password = "";

var agent = new MediaFireAgent(config);

await agent.User.Authenticate(Email, Password);


```

### Downloading
```c#
Stream fileStream = null; // The destination stream.
string fileQuickKey= "[Target_File_QuickKey]";
 await agent.File.Download(
		fileQuickKey, 
		fileStream, 
		new Progress<MediaFireOperationProgress>(
									  (progress) =>
									  {
										  //Report Progress
									  })
		);

```

### Uploading
```c#
Stream fileStream = null; // The file stream to upload.
string fileName = "[Target_File_Name]";

var uploadDetails = await agent.File.Upload(
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
