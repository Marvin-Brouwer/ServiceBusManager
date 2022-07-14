using MarvinBrouwer.ServiceBusManager.Azure.Helpers;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ServiceBus.Fluent;

public sealed record TopicSubscriptionDeadLetter(IServiceBusNamespace ServiceBus, TopicSubscription TopicSubscription) : AzureResource<ISubscription>(ServiceBus, TopicSubscription.InnerResource)
{
	public TopicSubscriptionDeadLetter(IServiceBusNamespace ServiceBus, ITopic topic, TopicSubscription subscription) : this(ServiceBus, subscription)
	{
		TopicPath = topic.Name;
	}

	public string TopicPath { get; }

	public override string Path => DeadLetterNameHelper.FormatDeadLetterPath(InnerResource.Name);
}