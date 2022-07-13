using Azure.Messaging.ServiceBus;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public  sealed class AzureServiceBusResourceQueryService : IAzureServiceBusResourceQueryService
{
	public async Task<long> GetMessageCount(IResource selectedResource, bool countDeadLetter, CancellationToken cancellationToken)
	{
		if (selectedResource is IQueue queue)
		{
			queue = await queue.RefreshAsync(cancellationToken);
			return countDeadLetter
				? queue.DeadLetterMessageCount
				: queue.ActiveMessageCount;
		}
		if (selectedResource is ISubscription subscription)
		{
			subscription = await subscription.RefreshAsync(cancellationToken);
			return countDeadLetter
				? subscription.DeadLetterMessageCount
				: subscription.ActiveMessageCount;
		}

		throw new NotSupportedException(selectedResource.GetType().FullName);
	}


	//public async Task<MessageHandler> DownloadFullResource(AzureResource selectedResource, CancellationToken cancellationToken)
	//{
	//	var client = await _clientFactory.GetServiceBusClient(selectedResource.ServiceBus.Secret, cancellationToken);
	//	return await GetResourceResults(selectedResource, client, cancellationToken);
	//}

	/// <summary>
	/// Use ReceiveMessagesAsync instead of PeekMessagesAsync, because the ReceiveMode does not apply to peek
	/// When using peek we have no control over locking, releasing and completing
	/// </summary>
	//private async Task<MessageHandler> GetResourceResults(AzureResource selectedResource, ServiceBusClient client, CancellationToken cancellationToken)
	//{
	//	if (selectedResource is Queue || selectedResource is QueueDeadLetter)
	//		return await GetQueueResults(selectedResource, client, selectedResource.Path, cancellationToken);

	//	if (selectedResource is TopicSubscription topicSubscription)
	//		return await GetTopicResourceResults(selectedResource, client,
	//			topicSubscription.Topic.Path, topicSubscription.Path, cancellationToken);

	//	if (selectedResource is TopicSubscriptionDeadLetter topicSubscriptionDeadLetter)
	//		return await GetTopicResourceResults(selectedResource, client,
	//			topicSubscriptionDeadLetter.Topic.Path, topicSubscriptionDeadLetter.Path, cancellationToken);

	//	throw new NotSupportedException(selectedResource.GetType().FullName);
	//}

	//private async Task<MessageHandler> GetTopicResourceResults(
	//	AzureResource selectedResource,
	//	ServiceBusClient client,
	//	string topicPath,
	//	string topicSubscriptionPath,
	//	CancellationToken cancellationToken)
	//{
	//	var subscriptionClient = client.CreateReceiver(
	//		topicPath,
	//		topicSubscriptionPath,
	//		new ServiceBusReceiverOptions
	//		{
	//			ReceiveMode = ServiceBusReceiveMode.PeekLock
	//		});

	//	return new MessageHandler(
	//		client,
	//		selectedResource,
	//		subscriptionClient,
	//		await subscriptionClient.ReceiveMessagesAsync(AzureConstants.GetMessageCount, null, cancellationToken)
	//	);
	//}

	//private async Task<MessageHandler> GetQueueResults(
	//	AzureResource selectedResource,
	//	ServiceBusClient client,
	//	string path,
	//	CancellationToken cancellationToken)
	//{
	//	var queueClient = client.CreateReceiver(path, new ServiceBusReceiverOptions
	//	{
	//		ReceiveMode = ServiceBusReceiveMode.PeekLock
	//	});

	//	return new MessageHandler(
	//		client,
	//		selectedResource,
	//		queueClient,
	//		await ReceiveMessagesAsync(queueClient, cancellationToken)
	//	);
	//}

	//private async Task<IReadOnlyList<ServiceBusReceivedMessage>> ReceiveMessagesAsync(ServiceBusReceiver queueClient, CancellationToken cancellationToken)
	//{
	//	// Add synthetic delay to prevent locks persisting
	//	var lockWaitDelay = TimeSpan.FromMilliseconds(500);

	//	await Task.Delay(lockWaitDelay, cancellationToken);

	//	return await queueClient.ReceiveMessagesAsync(AzureConstants.MessageGetCount, null, cancellationToken);
	//}
}
