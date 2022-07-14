using Azure.Messaging.ServiceBus;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public interface IAzureServiceBusResourceCommandService
{
	Task QueueMessages(
		IAzureResource<IResource> selectedResource,
		IReadOnlyList<(BinaryData blob, string contentType)> messages,
		CancellationToken cancellationToken);
	Task QueueMessages(
		IAzureResource<IResource> selectedResource,
		IReadOnlyList<ServiceBusReceivedMessage> messages,
		CancellationToken cancellationToken);
}