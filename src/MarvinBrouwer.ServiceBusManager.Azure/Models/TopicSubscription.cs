
using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record TopicSubscription : AzureResource<ISubscription>
{
	public TopicSubscription(ISubscription subscription) : base(subscription)
	{
		DeadLetter = new TopicSubscriptionDeadLetter(this);
	}
	
	public TopicSubscriptionDeadLetter DeadLetter { get; }
}