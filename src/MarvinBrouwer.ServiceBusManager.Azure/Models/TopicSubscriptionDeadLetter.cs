using MarvinBrouwer.ServiceBusManager.Azure.Helpers;

using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed class TopicSubscriptionDeadLetter : AzureResource<ISubscription>
{
	public TopicSubscriptionDeadLetter(IServiceBusNamespace serviceBus, ITopic topic, TopicSubscription subscription)
	{
		ServiceBus = serviceBus;
		InnerResource = subscription.InnerResource;

		Topic = topic;
	}

	public ITopic Topic { get; }

	public override string Path => DeadLetterNameHelper.FormatDeadLetterPath(InnerResource.Name);
	
}