using MarvinBrouwer.ServiceBusManager.Azure.Helpers;

using Microsoft.Azure.Management.ServiceBus.Fluent;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;
using ITopicSubscription = Microsoft.Azure.Management.ServiceBus.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="IAzureSubscription"/>'s dead-letter Queue
/// </summary>
public sealed class TopicSubscriptionDeadLetter : AzureResource
{
	/// <inheritdoc cref="TopicSubscriptionDeadLetter"/>
	public TopicSubscriptionDeadLetter(IAzureSubscription subscription, IServiceBusNamespace serviceBus, string topicName, TopicSubscription topicSubscription)
	{
		AzureSubscription = subscription;
		ServiceBusId = serviceBus.Id;
		ServiceBusName = serviceBus.Name;

		Key = topicSubscription.Key;
		Id = topicSubscription.Id;
		Name = topicSubscription.Name;

		TopicName = topicName;
	}

	/// <inheritdoc />
	public override string Path => DeadLetterNameHelper.FormatDeadLetterPath(Name);

	/// <summary>
	/// Reference to this <see cref="TopicSubscriptionDeadLetter"/>s parent <see cref="Models.Topic"/>
	/// </summary>
	public string TopicName { get; }
	
}