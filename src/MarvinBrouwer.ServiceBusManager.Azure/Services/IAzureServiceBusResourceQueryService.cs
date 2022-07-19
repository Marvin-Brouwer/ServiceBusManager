using Azure.Messaging.ServiceBus;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <summary>
/// This service is responsible for querying message data that belongs to <see cref="IAzureResource{TResource}"/>s
/// </summary>
public interface IAzureServiceBusResourceQueryService
{
	/// <summary>
	/// Get the current message count for this <paramref name="selectedResource"/>
	/// </summary>
	Task<long> GetMessageCount(IAzureResource<IResource> selectedResource, CancellationToken cancellationToken);

	/// <summary>
	/// Read all the messages (max 100) <br />
	/// that are queued for this <see cref="IAzureResource{TResource}"/>
	/// (<see cref="Queue"/>/<see cref="QueueDeadLetter"/>/<see cref="TopicSubscription"/>/<see cref="TopicSubscriptionDeadLetter"/>)
	/// </summary>
	/// <param name="selectedResource"></param>
	/// <param name="receiveMode">
	/// Determine whether you want this read to be destructive or just peeking
	/// </param>
	/// <param name="cancellationToken"></param>
	Task<IReadOnlyList<ServiceBusReceivedMessage>> ReadAllMessages(
		IAzureResource<IResource> selectedResource, ServiceBusReceiveMode receiveMode, CancellationToken cancellationToken);
}