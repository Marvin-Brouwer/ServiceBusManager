using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using MarvinBrouwer.ServiceBusManager.Azure.Models;

using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using MimeTypes;

namespace MarvinBrouwer.ServiceBusManager.Services;

public sealed class LocalStorageService
{
	private const string TimeStampFormat = "yyyy-MM-dd HHmmss";
	private const string DownloadFolderName = "Downloads";

	private readonly string _applicationPath;
	private readonly string _downloadFolderPath;

	public LocalStorageService()
	{
		_applicationPath = Path.Join(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			Path.GetFileNameWithoutExtension(GetType().Module.Name)
		);
		_downloadFolderPath = Path.Join(_applicationPath, DownloadFolderName);
	}

	public void PrepareDownloadFolder()
	{
		if (!Directory.Exists(_downloadFolderPath)) Directory.CreateDirectory(_downloadFolderPath);
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
		var destinationPath = Path.Combine(_downloadFolderPath, $"{archiveFileName}.zip");

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

			var extension = MimeTypeMap.GetExtension(dataResult.ContentType) ?? ".file";
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

	// TODO, we can do better
	public static IAsyncEnumerable<string> ReadDownloads(string[] fileNames, CancellationToken cancellationToken)
	{
		var zipFiles = fileNames.Where(fileName => fileName.EndsWith(".zip"));
		var zipFileContents = ReadZipFiles(zipFiles);

		return ReadFileData(zipFileContents, cancellationToken);
	}

	private static async IAsyncEnumerable<string> ReadZipFiles(IEnumerable<string> zipFiles)
	{
		foreach (var filePath in zipFiles)
		{
			await using var fileStream = File.OpenRead(filePath);
			using var archive = new ZipFile(fileStream);
			foreach (ZipEntry entry in archive)
			{
				if (entry.IsDirectory) continue;
				if (!entry.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) continue;

				await using var entryStream = archive.GetInputStream(entry);
				using var stringReader = new StreamReader(entryStream);

				yield return await stringReader.ReadToEndAsync();
			}
		}
	}

	private static async IAsyncEnumerable<string> ReadFileData(IAsyncEnumerable<string> jsonFiles, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		await foreach (var fileName in jsonFiles.WithCancellation(cancellationToken))
		{
			if (cancellationToken.IsCancellationRequested) yield break;

			using var fileStream = File.OpenText(fileName);
			yield return await fileStream.ReadToEndAsync();
		}
	}
}