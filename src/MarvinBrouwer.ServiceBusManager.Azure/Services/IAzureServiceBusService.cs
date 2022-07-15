using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public interface IAzureServiceBusService
{
	IAsyncEnumerable<ServiceBus> ListServiceBuses(ISubscription subscription, CancellationToken cancellationToken);
	(IAsyncEnumerable<Queue> queues, IAsyncEnumerable<Topic> topics) ListServiceBusResources(ServiceBus serviceBus, CancellationToken cancellationToken);
	IAsyncEnumerable<TopicSubscription> ListTopicSubscriptions(Topic topic, CancellationToken cancellationToken);

}