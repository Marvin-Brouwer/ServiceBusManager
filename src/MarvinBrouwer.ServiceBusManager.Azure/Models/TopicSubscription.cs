
using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record TopicSubscription : AzureResource<ISubscription>
{
	public TopicSubscription(IServiceBusNamespace ServiceBus, ITopic topic, ISubscription subscription) : base(ServiceBus, subscription)
	{
		DeadLetter = new TopicSubscriptionDeadLetter(ServiceBus, topic, this);
		Topic = topic;
	}
	
	public ITopic Topic { get; }
	public TopicSubscriptionDeadLetter DeadLetter { get; }
}