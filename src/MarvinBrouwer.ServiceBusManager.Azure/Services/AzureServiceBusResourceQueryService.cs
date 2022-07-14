using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent.Models;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Azure;
using AccessRights = Microsoft.Azure.Management.ServiceBus.Fluent.Models.AccessRights;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public  sealed class AzureServiceBusResourceQueryService : IAzureServiceBusResourceQueryService
{
	public async Task<long> GetMessageCount<TResource>(IAzureResource<TResource> selectedResource, CancellationToken cancellationToken)
		where TResource : IResource
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


	/// <summary>
	/// Use the <see cref="IServiceBusNamespace"/>s Authorization rules to get
	/// a connection string containing the correct access rights
	/// </summary>
	private static async Task<string> GetAccessConnectionString(
		IServiceBusNamespace serviceBusNamespace, AccessRights accessRights, CancellationToken cancellationToken)
	{
		var rootManageSharedAccessKey = await serviceBusNamespace.AuthorizationRules
			.GetByNameAsync("RootManageSharedAccessKey", cancellationToken);

		// Try the default access key first
		if (rootManageSharedAccessKey != null && rootManageSharedAccessKey.Rights.Contains(accessRights))
			return (await rootManageSharedAccessKey.GetKeysAsync(cancellationToken)).PrimaryConnectionString;

		// Find one with enough rights
		var accessKeyRecord = (await serviceBusNamespace.AuthorizationRules
				.ListAsync(true, cancellationToken))
			.FirstOrDefault(key => key.Rights.Contains(accessRights));

		// If you ever run into this.
		// Alternatively you can use the _authenticationService.AzureCredentials to pass to the ServiceBusClient
		// We're not sure if this works, it sure didn't with the RootManageSharedAccessKey in place.
		if (accessKeyRecord is null) throw new NotSupportedException(
			"Currently we don't support service buses without access keys");

		return (await accessKeyRecord.GetKeysAsync(cancellationToken)).PrimaryConnectionString;
	}

	public async Task<IReadOnlyList<ServiceBusReceivedMessage>> ReadAllMessages<TResource>(IAzureResource<TResource> selectedResource, CancellationToken cancellationToken)
		where TResource : IResource
	{
		var connectionString = await GetAccessConnectionString(selectedResource.ServiceBus, AccessRights.Listen, cancellationToken);
		var client = new ServiceBusClient(connectionString, new ServiceBusClientOptions
		{
			EnableCrossEntityTransactions = true,
			TransportType = ServiceBusTransportType.AmqpTcp
		});

		if (selectedResource is IAzureResource<IQueue>)
		{
			var queueReceiver = CreateQueueReceiver(client, selectedResource.Path, cancellationToken);
			return await ReceiveMessagesAsync(queueReceiver, cancellationToken);
		}

		if (selectedResource is TopicSubscription subscription)
		{
			var queueReceiver = CreateTopicReceiver(client,
				subscription.TopicPath,
				subscription.Path, cancellationToken);
			return await ReceiveMessagesAsync(queueReceiver, cancellationToken);
		}
		if (selectedResource is TopicSubscriptionDeadLetter deadLetterSubscription)
		{
			var queueReceiver = CreateTopicReceiver(client,
				deadLetterSubscription.TopicPath,
				deadLetterSubscription.Path, cancellationToken);
			return await ReceiveMessagesAsync(queueReceiver, cancellationToken);
		}

		throw new NotSupportedException(selectedResource.GetType().FullName);
	}

	private ServiceBusReceiver CreateQueueReceiver(
		ServiceBusClient client,
		string path,
		CancellationToken cancellationToken)
	{
		return client.CreateReceiver(path, new ServiceBusReceiverOptions
		{
			ReceiveMode = ServiceBusReceiveMode.PeekLock
		});
	}

	private ServiceBusReceiver CreateTopicReceiver(
		ServiceBusClient client,
		string topicPath,
		string topicSubscriptionPath,
		CancellationToken cancellationToken)
	{
		return client.CreateReceiver(topicPath, topicSubscriptionPath, new ServiceBusReceiverOptions
		{
			ReceiveMode = ServiceBusReceiveMode.PeekLock
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
