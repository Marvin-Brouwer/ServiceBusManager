using Azure.Messaging.ServiceBus;
using MarvinBrouwer.ServiceBusManager.Azure.Extensions;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using AccessRights = Microsoft.Azure.Management.ServiceBus.Fluent.Models.AccessRights;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public  sealed class AzureServiceBusResourceQueryService : IAzureServiceBusResourceQueryService
{
	public async Task<long> GetMessageCount(
		IAzureResource<IResource> selectedResource, CancellationToken cancellationToken)
	{
		if (selectedResource is Queue queue)
		{
			return (await queue.InnerResource.RefreshAsync(cancellationToken))
				.ActiveMessageCount;
		}
		if (selectedResource is QueueDeadLetter deadLetterQueue)
		{
			return (await deadLetterQueue.InnerResource.RefreshAsync(cancellationToken))
				.DeadLetterMessageCount;
		}
		if (selectedResource is TopicSubscription subscription)
		{
			return (await subscription.InnerResource.RefreshAsync(cancellationToken))
				.ActiveMessageCount;
		}
		if (selectedResource is TopicSubscriptionDeadLetter deadLetterSubscription)
		{
			return (await deadLetterSubscription.InnerResource.RefreshAsync(cancellationToken))
				.DeadLetterMessageCount;
		}

		throw new NotSupportedException(selectedResource.GetType().FullName);
	}
	
	public async Task<IReadOnlyList<ServiceBusReceivedMessage>> ReadAllMessages(
		IAzureResource<IResource> selectedResource, ServiceBusReceiveMode receiveMode, CancellationToken cancellationToken)
	{
		var client = await selectedResource.ServiceBus.CreateServiceBusClient(AccessRights.Listen, cancellationToken);

		if (selectedResource is IAzureResource<IQueue>)
		{
			var queueReceiver = CreateQueueReceiver(client, selectedResource.Path, receiveMode);
			return await ReceiveMessagesAsync(queueReceiver, cancellationToken);
		}

		if (selectedResource is TopicSubscription subscription)
		{
			var queueReceiver = CreateTopicReceiver(client,
				subscription.Topic.Name,
				subscription.Path,
				receiveMode);
			return await ReceiveMessagesAsync(queueReceiver, cancellationToken);
		}
		if (selectedResource is TopicSubscriptionDeadLetter deadLetterSubscription)
		{
			var queueReceiver = CreateTopicReceiver(client,
				deadLetterSubscription.Topic.Name,
				deadLetterSubscription.Path,
				receiveMode);
			return await ReceiveMessagesAsync(queueReceiver, cancellationToken);
		}

		throw new NotSupportedException(selectedResource.GetType().FullName);
	}


	private ServiceBusReceiver CreateQueueReceiver(
		ServiceBusClient client,
		string path,
		ServiceBusReceiveMode receiveMode)
	{
		return client.CreateReceiver(path, new ServiceBusReceiverOptions
		{
			ReceiveMode = receiveMode
		});
	}

	private ServiceBusReceiver CreateTopicReceiver(
		ServiceBusClient client,
		string topicPath,
		string topicSubscriptionPath,
		ServiceBusReceiveMode receiveMode)
	{
		return client.CreateReceiver(topicPath, topicSubscriptionPath, new ServiceBusReceiverOptions
		{
			ReceiveMode = receiveMode
		});
	}

	private static async Task<IReadOnlyList<ServiceBusReceivedMessage>> ReceiveMessagesAsync(ServiceBusReceiver receiver, CancellationToken cancellationToken)
	{
		// Add synthetic delay to prevent locks persisting
		var lockWaitDelay = TimeSpan.FromMilliseconds(500);
		await Task.Delay(lockWaitDelay, cancellationToken);

		return await receiver
			.ReceiveMessagesAsync(AzureConstants.ServiceBusResourceMaxItemCount, null, cancellationToken);
	}
}
