using Azure.Messaging.ServiceBus;

using MarvinBrouwer.ServiceBusManager.Azure.Extensions;
using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;

using AccessRights = Microsoft.Azure.Management.ServiceBus.Fluent.Models.AccessRights;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <inheritdoc />
public sealed class AzureServiceBusResourceQueryService : IAzureServiceBusResourceQueryService
{
	private readonly IAzureAuthenticationService _authenticationService;

	/// <inheritdoc cref="AzureServiceBusResourceQueryService" />
	public AzureServiceBusResourceQueryService(IAzureAuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;
	}

	/// <inheritdoc />
	public async Task<long> GetMessageCount(
		IAzureResource<IResource> selectedResource, CancellationToken cancellationToken)
	{
		var credentials = await _authenticationService.Authenticate(selectedResource.Subscription, cancellationToken);
		var azure = credentials.WithSubscription(selectedResource.Subscription.SubscriptionId);

		var serviceBus = await azure.ServiceBusNamespaces
			.GetByIdAsync(selectedResource.ServiceBus.Id, cancellationToken);
		if (serviceBus is null) return 0;

		if (selectedResource is Queue)
		{
			var queue = await serviceBus.Queues
				.GetByNameAsync(selectedResource.InnerResource.Name, cancellationToken);

			return queue?.ActiveMessageCount ?? 0;
		}

		if (selectedResource is QueueDeadLetter)
		{
			var deadLetterQueue = await serviceBus.Queues
				.GetByNameAsync(selectedResource.InnerResource.Name, cancellationToken);

			return deadLetterQueue?.DeadLetterMessageCount ?? 0;
		}

		if (selectedResource is TopicSubscription topicSubscription)
		{
			var topic = await serviceBus.Topics
				.GetByNameAsync(topicSubscription.Topic.Name, cancellationToken);
			if (topic is null) return 0;

			var azureTopicSubscription = await topic.Subscriptions
				.GetByNameAsync(topicSubscription.InnerResource.Name, cancellationToken);

			return azureTopicSubscription?.ActiveMessageCount ?? 0;
		}

		if (selectedResource is TopicSubscriptionDeadLetter deadLetterSubscription)
		{
			var topic = await serviceBus.Topics
				.GetByNameAsync(deadLetterSubscription.Topic.Name, cancellationToken);
			if (topic is null) return 0;

			var azureTopicSubscription = await topic.Subscriptions
				.GetByNameAsync(deadLetterSubscription.InnerResource.Name, cancellationToken);

			return azureTopicSubscription?.DeadLetterMessageCount ?? 0;
		}

		throw new NotSupportedException(selectedResource.GetType().FullName);
	}

	/// <inheritdoc />
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
	
	private static ServiceBusReceiver CreateQueueReceiver(
		ServiceBusClient client,
		string path,
		ServiceBusReceiveMode receiveMode)
	{
		return client.CreateReceiver(path, new ServiceBusReceiverOptions
		{
			ReceiveMode = receiveMode
		});
	}

	private static ServiceBusReceiver CreateTopicReceiver(
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
