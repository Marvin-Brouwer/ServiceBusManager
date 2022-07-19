using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="ISubscription"/>
/// </summary>
public sealed class TopicSubscription : AzureResource<ISubscription>
{
	/// <inheritdoc cref="TopicSubscription"/>
	public TopicSubscription(IServiceBusNamespace serviceBus, ITopic topic, ISubscription subscription)
	{
		ServiceBus = serviceBus;
		InnerResource = subscription;

		DeadLetter = new TopicSubscriptionDeadLetter(ServiceBus, topic, this);
		Topic = topic;
	}

	/// <summary>
	/// Reference to this <see cref="TopicSubscription"/>s parent <see cref="Models.Topic"/>
	/// </summary>
	public ITopic Topic { get; }

	/// <summary>
	/// Instance of this <see cref="TopicSubscriptionDeadLetter"/>s dead-letter <see cref="TopicSubscriptionDeadLetter"/>
	/// </summary>
	public TopicSubscriptionDeadLetter DeadLetter { get; }
}