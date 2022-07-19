using Azure.Messaging.ServiceBus;

using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <summary>
/// This service is responsible for pushing message data that belongs to <see cref="IAzureResource{TResource}"/>s
/// </summary>
public interface IAzureServiceBusResourceCommandService
{
	/// <summary>
	/// Queue these raw blobs to the <paramref name="selectedResource"/>
	/// </summary>
	Task QueueMessages(
		IAzureResource<IResource> selectedResource,
		IReadOnlyList<(BinaryData blob, string contentType)> messages,
		CancellationToken cancellationToken);

	/// <summary>
	/// Queue these messages to the <paramref name="selectedResource"/>
	/// </summary>
	Task QueueMessages(
		IAzureResource<IResource> selectedResource,
		IReadOnlyList<ServiceBusReceivedMessage> messages,
		CancellationToken cancellationToken);
}