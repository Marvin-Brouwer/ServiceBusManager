using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record Topic(IServiceBusNamespace ServiceBus, ITopic InnerResource) : AzureResource<ITopic>(ServiceBus, InnerResource)
{
	public IAsyncEnumerable<TopicSubscription> TopicSubscriptions { get; init; }

}