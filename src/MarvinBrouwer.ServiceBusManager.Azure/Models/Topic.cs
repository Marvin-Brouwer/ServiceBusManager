using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed class Topic : AzureResource<ITopic>
{
	public Topic(IServiceBusNamespace serviceBus, ITopic topic)
	{
		ServiceBus = serviceBus;
		InnerResource = topic;
	}

	public IAsyncEnumerable<TopicSubscription> TopicSubscriptions { get; init; }
	
}