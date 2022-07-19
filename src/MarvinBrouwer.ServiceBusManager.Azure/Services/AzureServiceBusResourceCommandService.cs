using Azure.Messaging.ServiceBus;

using MarvinBrouwer.ServiceBusManager.Azure.Extensions;
using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent.Models;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <inheritdoc />
public sealed class AzureServiceBusResourceCommandService : IAzureServiceBusResourceCommandService
{
	/// <inheritdoc />
	public Task QueueMessages(IAzureResource<IResource> selectedResource, IReadOnlyList<(BinaryData blob, string contentType)> messages,  CancellationToken cancellationToken)
	{
		if (selectedResource is QueueDeadLetter queueDeadLetter)
			return QueueMessagesToQueue(queueDeadLetter.ServiceBus, queueDeadLetter.Queue.InnerResource, ConvertMessages(messages), cancellationToken);
		if (selectedResource is Topic topic)
			return QueueMessagesToTopic(topic.ServiceBus, topic.InnerResource, ConvertMessages(messages), cancellationToken);
		if (selectedResource is TopicSubscription topicSubscription)
			return QueueMessagesToTopic(topicSubscription.ServiceBus, topicSubscription.Topic, ConvertMessages(messages), cancellationToken);
		if (selectedResource is TopicSubscriptionDeadLetter topicSubscriptionDeadLetter)
			return QueueMessagesToTopic(topicSubscriptionDeadLetter.ServiceBus, topicSubscriptionDeadLetter.Topic, ConvertMessages(messages), cancellationToken);

		throw new NotSupportedException(selectedResource.GetType().FullName);
	}

	/// <inheritdoc />
	public Task QueueMessages(IAzureResource<IResource> selectedResource, IReadOnlyList<ServiceBusReceivedMessage> messages, CancellationToken cancellationToken)
	{
		if (selectedResource is QueueDeadLetter queueDeadLetter)
			return QueueMessagesToQueue(queueDeadLetter.ServiceBus, queueDeadLetter.Queue.InnerResource, ConvertMessages(messages), cancellationToken);
		if (selectedResource is Topic topic)
			return QueueMessagesToTopic(topic.ServiceBus, topic.InnerResource, ConvertMessages(messages), cancellationToken);
		if (selectedResource is TopicSubscription topicSubscription)
			return QueueMessagesToTopic(topicSubscription.ServiceBus, topicSubscription.Topic, ConvertMessages(messages), cancellationToken);
		if (selectedResource is TopicSubscriptionDeadLetter topicSubscriptionDeadLetter)
			return QueueMessagesToTopic(topicSubscriptionDeadLetter.ServiceBus, topicSubscriptionDeadLetter.Topic, ConvertMessages(messages), cancellationToken);

		throw new NotSupportedException(selectedResource.GetType().FullName);
	}

	private static IReadOnlyList<ServiceBusMessage> ConvertMessages(IReadOnlyList<ServiceBusReceivedMessage> messages)
	{
		return messages
			.Select(message => new ServiceBusMessage(message))
			.ToList();
	}

	private static IReadOnlyList<ServiceBusMessage> ConvertMessages(IReadOnlyList<(BinaryData blob, string contentType)> messages)
	{
		return messages
			.Select(message => new ServiceBusMessage(message.blob){ ContentType = message.contentType })
			.ToList();
	}

	private static async Task QueueMessagesToQueue(IServiceBusNamespace serviceBusNamespace, IQueue queue,
		IReadOnlyList<ServiceBusMessage> messages, CancellationToken cancellationToken)
	{
		var client = await serviceBusNamespace.CreateServiceBusClient(AccessRights.Send, cancellationToken);
		await PushMessage(queue.Name, messages, client, cancellationToken);
	}

	private static async Task QueueMessagesToTopic(IServiceBusNamespace serviceBusNamespace, ITopic topic,
		IReadOnlyList<ServiceBusMessage> messages, CancellationToken cancellationToken)
	{
		var client = await serviceBusNamespace.CreateServiceBusClient(AccessRights.Send, cancellationToken);
		await PushMessage(topic.Name, messages, client, cancellationToken);
	}

	private static async Task PushMessage(string resourcePath, IReadOnlyList<ServiceBusMessage> messages,
		ServiceBusClient client, CancellationToken cancellationToken)
	{
		var sender = client.CreateSender(resourcePath);

		try
		{
			await sender.SendMessagesAsync(messages, cancellationToken);
		}
		finally
		{
			if (!sender.IsClosed) await sender.CloseAsync(cancellationToken);
		}
	}
}
