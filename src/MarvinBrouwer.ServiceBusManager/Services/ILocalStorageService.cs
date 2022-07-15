using Azure.Messaging.ServiceBus;

using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarvinBrouwer.ServiceBusManager.Services;

public interface ILocalStorageService
{
	string DownloadFolderPath { get; }

	void PrepareDownloadFolder();

	void OpenDownloadFolder();

	Task StoreResourceDownload(
		DateTime timestamp, IAzureResource<IResource> resource,
		IReadOnlyList<ServiceBusReceivedMessage> messages, CancellationToken cancellationToken);

	IAsyncEnumerable<(BinaryData fileBlob, string contentType)> ReadFileData(
		string[] fileNames, CancellationToken cancellationToken);
}