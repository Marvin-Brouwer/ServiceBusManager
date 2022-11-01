using Microsoft.Azure.Management.ServiceBus.Fluent;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;
using ITopicSubscription = Microsoft.Azure.Management.ServiceBus.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="ITopicSubscription"/>
/// </summary>
public sealed class TopicSubscription : AzureResource
{
	/// <inheritdoc cref="TopicSubscription"/>
	public TopicSubscription(IAzureSubscription subscription, IServiceBusNamespace serviceBus, string topicName, ITopicSubscription topicSubscription)
	{
		AzureSubscription = subscription;
		ServiceBusId = serviceBus.Id;
		ServiceBusName = serviceBus.Name;

		Key = topicSubscription.Key;
		Id = topicSubscription.Id;
		Name = topicSubscription.Name;

		DeadLetter = new TopicSubscriptionDeadLetter(subscription, serviceBus, topicName, this);
		TopicName = topicName;
	}

	/// <summary>
	/// Reference to this <see cref="TopicSubscription"/>s parent <see cref="Models.Topic"/>
	/// </summary>
	public string TopicName { get; }

	/// <summary>
	/// Instance of this <see cref="TopicSubscriptionDeadLetter"/>s dead-letter <see cref="TopicSubscriptionDeadLetter"/>
	/// </summary>
	public TopicSubscriptionDeadLetter DeadLetter { get; }
}