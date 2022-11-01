using Azure.Messaging.ServiceBus;

using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <summary>
/// This service is responsible for pushing message data that belongs to <see cref="IAzureResource"/>s
/// </summary>
public interface IAzureServiceBusResourceCommandService
{
	/// <summary>
	/// Queue these raw blobs to the <paramref name="selectedResource"/>
	/// </summary>
	Task QueueMessages(IAzure azure, IAzureResource selectedResource,
		IReadOnlyList<(BinaryData blob, string contentType)> messages, CancellationToken cancellationToken);

	/// <summary>
	/// Queue these messages to the <paramref name="selectedResource"/>
	/// </summary>
	Task QueueMessages(IAzure azure, IAzureResource selectedResource,
		IReadOnlyList<ServiceBusReceivedMessage> messages, CancellationToken cancellationToken);
}