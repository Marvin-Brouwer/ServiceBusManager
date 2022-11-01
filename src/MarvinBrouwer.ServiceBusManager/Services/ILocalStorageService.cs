using Azure.Messaging.ServiceBus;

using MarvinBrouwer.ServiceBusManager.Azure.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarvinBrouwer.ServiceBusManager.Services;

/// <summary>
/// This service is responsible for everything related to reading and writing files or blobs
/// </summary>
public interface ILocalStorageService
{
	/// <summary>
	/// Get the Application's download folder path
	/// </summary>
	string DownloadFolderPath { get; }

	/// <summary>
	/// Prepare the download folder (make sure it's there)
	/// </summary>
	void PrepareDownloadFolder();

	/// <summary>
	/// Open windows explorer in the Application's download folder
	/// </summary>
	void OpenDownloadFolder();

	/// <summary>
	/// Store all the <paramref name="messages"/> in the download folder.
	/// </summary>
	Task StoreResourceDownload(
		DateTime timestamp, IAzureResource resource,
		IReadOnlyList<ServiceBusReceivedMessage> messages, CancellationToken cancellationToken);

	/// <summary>
	/// Read all blob data for the <paramref name="fileNames"/>
	/// </summary>
	IAsyncEnumerable<(BinaryData fileBlob, string contentType)> ReadFileData(
		string[] fileNames, CancellationToken cancellationToken);
}