using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record Topic(ITopic InnerResource) : AzureResource<ITopic>(InnerResource)
{
	public IAsyncEnumerable<TopicSubscription> TopicSubscriptions { get; init; }

}