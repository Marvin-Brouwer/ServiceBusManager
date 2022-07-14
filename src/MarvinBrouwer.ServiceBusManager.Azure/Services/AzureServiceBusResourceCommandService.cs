using System.Runtime.Serialization;
using Azure.Messaging.ServiceBus;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Newtonsoft.Json.Linq;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public sealed class AzureServiceBusResourceCommandService : IAzureServiceBusResourceCommandService
{
	public Task QueueMessages(IAzureResource<IResource> selectedResource, IReadOnlyList<(BinaryData blob, string contentType)> messages,  CancellationToken cancellationToken)
	{
		if (selectedResource is QueueDeadLetter queueDeadLetter)
			return QueueMessagesToQueue(queueDeadLetter.Queue.InnerResource, ConvertMessages(messages), cancellationToken);
		if (selectedResource is TopicSubscription topicSubscription)
			return QueueMessagesToTopic(topicSubscription.Topic, ConvertMessages(messages), cancellationToken);
		if (selectedResource is TopicSubscriptionDeadLetter topicSubscriptionDeadLetter)
			return QueueMessagesToTopic(topicSubscriptionDeadLetter.Topic, ConvertMessages(messages), cancellationToken);

		throw new NotSupportedException(selectedResource.GetType().FullName);
	}

	public Task QueueMessages(IAzureResource<IResource> selectedResource, IReadOnlyList<ServiceBusReceivedMessage> messages, CancellationToken cancellationToken)
	{
		if (selectedResource is QueueDeadLetter queueDeadLetter)
			return QueueMessagesToQueue(queueDeadLetter.Queue.InnerResource, ConvertMessages(messages), cancellationToken);
		if (selectedResource is TopicSubscription topicSubscription)
			return QueueMessagesToTopic(topicSubscription.Topic, ConvertMessages(messages), cancellationToken);
		if (selectedResource is TopicSubscriptionDeadLetter topicSubscriptionDeadLetter)
			return QueueMessagesToTopic(topicSubscriptionDeadLetter.Topic, ConvertMessages(messages), cancellationToken);

		throw new NotSupportedException(selectedResource.GetType().FullName);
	}

	private IReadOnlyList<ServiceBusMessage> ConvertMessages(IReadOnlyList<ServiceBusReceivedMessage> messages)
	{
		return messages
			.Select(message => new ServiceBusMessage(message))
			.ToList();
	}

	private IReadOnlyList<ServiceBusMessage> ConvertMessages(IReadOnlyList<(BinaryData blob, string contentType)> messages)
	{
		return messages
			.Select(message => new ServiceBusMessage(message.blob){ ContentType = message.contentType })
			.ToList();
	}

	private Task QueueMessagesToQueue(IQueue queue, IReadOnlyList<ServiceBusMessage> messages, CancellationToken cancellationToken)
	{
		// TODO
		throw new NotImplementedException();
	}

	private Task QueueMessagesToTopic(ITopic topic, IReadOnlyList<ServiceBusMessage> messages, CancellationToken cancellationToken)
	{
		// TODO
		throw new NotImplementedException();
	}
}
