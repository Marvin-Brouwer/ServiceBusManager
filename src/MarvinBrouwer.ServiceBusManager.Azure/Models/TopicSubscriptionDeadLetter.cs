using MarvinBrouwer.ServiceBusManager.Azure.Helpers;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record TopicSubscriptionDeadLetter(
	TopicSubscription Subscription,
	Topic Topic
) : ServiceBusResource(
	Subscription.ServiceBus,
	$"{Subscription.Name}_{AzureConstants.DeadLetterPathSegment}"
)
{
	public override string Path => DeadLetterNameHelper.FormatDeadLetterPath(Subscription.Path);
	public override ServiceBusResource GetServiceBusTarget() => Topic;
}