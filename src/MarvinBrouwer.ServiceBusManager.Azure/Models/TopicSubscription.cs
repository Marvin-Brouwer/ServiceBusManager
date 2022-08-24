using Microsoft.Azure.Management.ServiceBus.Fluent;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;
using ITopicSubscription = Microsoft.Azure.Management.ServiceBus.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="ISubscription"/>
/// </summary>
public sealed class TopicSubscription : AzureResource<ITopicSubscription>
{
	/// <inheritdoc cref="TopicSubscription"/>
	public TopicSubscription(IAzureSubscription subscription, IServiceBusNamespace serviceBus, ITopic topic, ITopicSubscription topicSubscription)
	{
		Subscription = subscription;
		ServiceBus = serviceBus;
		InnerResource = topicSubscription;

		DeadLetter = new TopicSubscriptionDeadLetter(subscription, ServiceBus, topic, this);
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