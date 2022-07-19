using MarvinBrouwer.ServiceBusManager.Azure.Helpers;

using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="ISubscription"/>'s dead-letter Queue
/// </summary>
public sealed class TopicSubscriptionDeadLetter : AzureResource<ISubscription>
{
	/// <inheritdoc cref="TopicSubscriptionDeadLetter"/>
	public TopicSubscriptionDeadLetter(IServiceBusNamespace serviceBus, ITopic topic, TopicSubscription subscription)
	{
		ServiceBus = serviceBus;
		InnerResource = subscription.InnerResource;

		Topic = topic;
	}

	/// <inheritdoc />
	public override string Path => DeadLetterNameHelper.FormatDeadLetterPath(InnerResource.Name);

	/// <summary>
	/// Reference to this <see cref="TopicSubscriptionDeadLetter"/>s parent <see cref="Models.Topic"/>
	/// </summary>
	public ITopic Topic { get; }
	
}