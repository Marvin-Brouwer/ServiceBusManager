using Azure.Messaging.ServiceBus;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public interface IAzureServiceBusResourceQueryService
{
	Task<long> GetMessageCount<TResource>(IAzureResource<TResource> selectedResource, CancellationToken cancellationToken)
		where TResource : IResource;

	Task<IReadOnlyList<ServiceBusReceivedMessage>> ReadAllMessages<TResource>(
		IAzureResource<TResource> selectedResource, CancellationToken cancellationToken)
		where TResource : IResource;
}