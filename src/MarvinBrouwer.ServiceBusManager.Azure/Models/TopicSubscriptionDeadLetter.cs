using MarvinBrouwer.ServiceBusManager.Azure.Helpers;

using Microsoft.Azure.Management.ServiceBus.Fluent;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;
using ITopicSubscription = Microsoft.Azure.Management.ServiceBus.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="ISubscription"/>'s dead-letter Queue
/// </summary>
public sealed class TopicSubscriptionDeadLetter : AzureResource<ITopicSubscription>
{
	/// <inheritdoc cref="TopicSubscriptionDeadLetter"/>
	public TopicSubscriptionDeadLetter(IAzureSubscription subscription, IServiceBusNamespace serviceBus, ITopic topic, TopicSubscription topicSubscription)
	{
		Subscription = subscription;
		ServiceBus = serviceBus;
		InnerResource = topicSubscription.InnerResource;

		Topic = topic;
	}

	/// <inheritdoc />
	public override string Path => DeadLetterNameHelper.FormatDeadLetterPath(InnerResource.Name);

	/// <summary>
	/// Reference to this <see cref="TopicSubscriptionDeadLetter"/>s parent <see cref="Models.Topic"/>
	/// </summary>
	public ITopic Topic { get; }
	
}