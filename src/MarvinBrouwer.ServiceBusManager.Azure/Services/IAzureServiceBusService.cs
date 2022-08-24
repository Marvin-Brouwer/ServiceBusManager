using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <summary>
/// This service is responsible for listing ServiceBuses and related resources
/// </summary>
public interface IAzureServiceBusService
{
	/// <summary>
	/// List all the <see cref="ServiceBus"/>es the current authenticated user has access to
	/// </summary>
	IAsyncEnumerable<ServiceBus> ListServiceBuses(IAzure azure, IAzureSubscription subscription, CancellationToken cancellationToken);

	/// <summary>
	/// List all the <see cref="IAzureResource"/>s (<see cref="Queue"/>s and <see cref="Topic"/>s) that are part of this <paramref name="serviceBus"/>
	/// </summary>
	Task<(IAsyncEnumerable<Queue> queues, IAsyncEnumerable<Topic> topics)> ListServiceBusResources(IAzure azure, ServiceBus serviceBus, CancellationToken cancellationToken);

	/// <summary>
	/// List all the <see cref="TopicSubscription"/>s that are part of this <paramref name="topic"/>
	/// </summary>
	Task<IAsyncEnumerable<TopicSubscription>> ListTopicSubscriptions(IAzure azure, Topic topic, CancellationToken cancellationToken);
}