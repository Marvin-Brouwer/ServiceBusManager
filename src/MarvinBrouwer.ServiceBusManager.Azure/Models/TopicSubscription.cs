using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed class TopicSubscription : AzureResource<ISubscription>
{
	public TopicSubscription(IServiceBusNamespace serviceBus, ITopic topic, ISubscription subscription)
	{
		ServiceBus = serviceBus;
		InnerResource = subscription;

		DeadLetter = new TopicSubscriptionDeadLetter(ServiceBus, topic, this);
		Topic = topic;
	}
	
	public ITopic Topic { get; }
	public TopicSubscriptionDeadLetter DeadLetter { get; }
}