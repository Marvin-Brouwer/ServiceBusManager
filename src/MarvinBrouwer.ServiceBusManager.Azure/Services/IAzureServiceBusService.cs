using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <summary>
/// This service is responsible for listing ServiceBuses and related resources
/// </summary>
public interface IAzureServiceBusService
{
	/// <summary>
	/// List all the <see cref="ServiceBus"/>es the current authenticated user has access to
	/// </summary>
	IAsyncEnumerable<ServiceBus> ListServiceBuses(ISubscription subscription, CancellationToken cancellationToken);

	/// <summary>
	/// List all the <see cref="IAzureResource{TResource}"/>s (<see cref="Queue"/>s and <see cref="Topic"/>s) that are part of this <paramref name="serviceBus"/>
	/// </summary>
	(IAsyncEnumerable<Queue> queues, IAsyncEnumerable<Topic> topics) ListServiceBusResources(ServiceBus serviceBus, CancellationToken cancellationToken);

	/// <summary>
	/// List all the <see cref="TopicSubscription"/>s that are part of this <paramref name="topic"/>
	/// </summary>
	IAsyncEnumerable<TopicSubscription> ListTopicSubscriptions(Topic topic, CancellationToken cancellationToken);
}