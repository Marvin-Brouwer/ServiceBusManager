using Azure.Messaging.ServiceBus;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public interface IAzureServiceBusResourceQueryService
{
	Task<long> GetMessageCount(IAzureResource<IResource> selectedResource, CancellationToken cancellationToken);
	Task<IReadOnlyList<ServiceBusReceivedMessage>> ReadAllMessages(IAzureResource<IResource> selectedResource, CancellationToken cancellationToken);
}