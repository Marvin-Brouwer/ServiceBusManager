using Azure.Messaging.ServiceBus;

using ICSharpCode.SharpZipLib.Zip;

using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;

using MimeTypes;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarvinBrouwer.ServiceBusManager.Services;

public sealed class LocalStorageService : ILocalStorageService
{
	private const string TimeStampFormat = "yyyy-MM-dd HHmmss";
	private const string DownloadFolderName = "Downloads";

	private readonly string _applicationPath;
	public string DownloadFolderPath { get; }

	public LocalStorageService()
	{
		_applicationPath = Path.Join(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			Path.GetFileNameWithoutExtension(GetType().Module.Name)
		);
		DownloadFolderPath = Path.Join(_applicationPath, DownloadFolderName);
	}


	public void PrepareDownloadFolder()
	{
		if (!Directory.Exists(DownloadFolderPath)) Directory.CreateDirectory(DownloadFolderPath);
	}

	public void OpenDownloadFolder()
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = DownloadFolderName,
			Verb = "open",
			UseShellExecute = true,
			WorkingDirectory = _applicationPath
		});
	}
	public async Task StoreResourceDownload(
		DateTime timestamp, IAzureResource<IResource> resource,
		IReadOnlyList<ServiceBusReceivedMessage> messages, CancellationToken cancellationToken)
	{
		var archiveFileName = GetArchiveFileName(resource, timestamp);

		var tempDir = AcquireTemporaryStorageDirectory(archiveFileName);
		var destinationPath = Path.Combine(DownloadFolderPath, $"{archiveFileName}.zip");

		try
		{
			await GenerateArchive(messages, destinationPath, tempDir, cancellationToken);
		}
		finally
		{
			// Always remove temp, if it fails it's not because of the zip lib but a coding error
			Directory.Delete(tempDir, true);
		}
	}

	private static async Task GenerateArchive(
		IReadOnlyList<ServiceBusReceivedMessage> messages,
		string destinationPath,
		string tempDir,
		CancellationToken cancellationToken)
	{
		using var archive = ZipFile.Create(destinationPath);
		archive.BeginUpdate();
		foreach (var dataResult in messages)
		{
			if (cancellationToken.IsCancellationRequested) break;
			
			const string defaultContentType = ".txt";
			var extension = string.IsNullOrWhiteSpace(dataResult.ContentType)
				? defaultContentType
				: MimeTypeMap.GetExtension(dataResult.ContentType) ?? defaultContentType;
			var fileName = $"{dataResult.MessageId}{extension}";

			await using var dataStream = dataResult.Body.ToStream();
			await PushStreamToArchive(fileName, dataStream, tempDir, archive, cancellationToken);
		}

		if (cancellationToken.IsCancellationRequested) archive.Close();
		else archive.CommitUpdate();
	}

	private static async Task PushStreamToArchive(
		string fileName,
		Stream dataStream,
		string tempDir,
		ZipFile archive,
		CancellationToken cancellationToken)
	{
		var tempFilePath = Path.Join(tempDir, fileName);
		await using var fileStream = File.OpenWrite(tempFilePath);

		dataStream.Seek(0, SeekOrigin.Begin);
		await dataStream.CopyToAsync(fileStream, cancellationToken);
		fileStream.Close();

		archive.Add(tempFilePath, fileName);
	}

	private static string AcquireTemporaryStorageDirectory(string archiveFileName)
	{
		var tempDir = Path.Combine(Path.GetTempPath(), archiveFileName);
		if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
		Directory.CreateDirectory(tempDir);

		return tempDir;
	}

	private static string GetArchiveFileName(IAzureResource<IResource> resource, DateTime timestamp)
	{
		var timestampMarker = timestamp.ToString(TimeStampFormat);

		if (resource is IAzureResource<IQueue>)
			return $"{resource.ServiceBus.Name} queue-{resource.Path} {timestampMarker}";
		if (resource is TopicSubscription topicSubscription)
			return $"{resource.ServiceBus.Name} topic-{topicSubscription.Topic.Name} subscription-{resource.Path} {timestampMarker}";
		if (resource is TopicSubscriptionDeadLetter topicSubscriptionDeadLetter)
			return $"{resource.ServiceBus.Name} topic-{topicSubscriptionDeadLetter.Topic.Name} subscription-{resource.Path} {timestampMarker}";

		return $"{resource.ServiceBus.Name} {resource.Path} {timestampMarker}";
	}

	private static bool IsDataFilePath(string filePath) =>
		filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
		filePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) ||
		filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);

	public async IAsyncEnumerable<(BinaryData fileBlob, string contentType)> ReadFileData(string[] fileNames, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var rawFilePaths = fileNames.Where(IsDataFilePath);
		var rawFiles = ReadFileData(rawFilePaths, cancellationToken);
		var zipFilePaths = fileNames.Where(fileName => fileName.EndsWith(".zip"));
		var zipFiles = ReadZipData(zipFilePaths, cancellationToken);

		await foreach (var fileBlob in rawFiles.WithCancellation(cancellationToken))
		{
			yield return fileBlob;
		}

		await foreach (var fileBlob in zipFiles.WithCancellation(cancellationToken))
		{
			yield return fileBlob;
		}
	}

	private static async IAsyncEnumerable<(BinaryData fileBlob, string contentType)> ReadFileData(
		IEnumerable<string> filePaths, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		foreach (var filePath in filePaths)
		{
			if (cancellationToken.IsCancellationRequested) yield break;

			await using var fileStream = File.OpenRead(filePath);
			using var streamReader = new StreamReader(fileStream, Encoding.UTF8);

			if (cancellationToken.IsCancellationRequested) yield break;
			var fileBlob = new BinaryData(await streamReader.ReadToEndAsync());
			var contentType = MimeTypeMap.GetMimeType(Path.GetExtension(filePath));

			if (cancellationToken.IsCancellationRequested) yield break;
			yield return (fileBlob, contentType);
		}
	}

	private static async IAsyncEnumerable<(BinaryData fileBlob, string contentType)> ReadZipData(
		IEnumerable<string> zipFilePaths, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		foreach (var filePath in zipFilePaths)
		{
			if (cancellationToken.IsCancellationRequested) yield break;
			await using var fileStream = File.OpenRead(filePath);
			using var archive = new ZipFile(fileStream);

			if (cancellationToken.IsCancellationRequested) yield break;
			foreach (ZipEntry entry in archive)
			{
				if (entry.IsDirectory) continue;
				if (!IsDataFilePath(entry.Name)) continue;

				if (cancellationToken.IsCancellationRequested) yield break;
				await using var entryStream = archive.GetInputStream(entry);
				using var streamReader = new StreamReader(entryStream);

				if (cancellationToken.IsCancellationRequested) yield break;
				var fileBlob = new BinaryData(await streamReader.ReadToEndAsync());
				var contentType = MimeTypeMap.GetMimeType(Path.GetExtension(entry.Name));

				if (cancellationToken.IsCancellationRequested) yield break;
				yield return (fileBlob, contentType);
			}
		}
	}
}