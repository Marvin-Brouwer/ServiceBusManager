
using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record TopicSubscription : AzureResource<ISubscription>
{
	public TopicSubscription(IServiceBusNamespace ServiceBus, ITopic topic, ISubscription subscription) : base(ServiceBus, subscription)
	{
		DeadLetter = new TopicSubscriptionDeadLetter(ServiceBus, topic, this);
		TopicPath = topic.Name;
	}

	public string TopicPath { get; }

	public TopicSubscriptionDeadLetter DeadLetter { get; }
}