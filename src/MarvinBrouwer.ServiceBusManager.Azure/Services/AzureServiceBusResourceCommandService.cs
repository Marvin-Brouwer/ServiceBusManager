using Azure.Messaging.ServiceBus;

using MarvinBrouwer.ServiceBusManager.Azure.Extensions;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent.Models;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <inheritdoc />
public sealed class AzureServiceBusResourceCommandService : IAzureServiceBusResourceCommandService
{
	/// <inheritdoc />
	public Task QueueMessages(IAzure azure, IAzureResource selectedResource, IReadOnlyList<(BinaryData blob, string contentType)> messages,  CancellationToken cancellationToken)
	{
		return QueueMessages(azure, selectedResource, ConvertMessages(messages), cancellationToken);
	}

	/// <inheritdoc />
	public Task QueueMessages(IAzure azure, IAzureResource selectedResource, IReadOnlyList<ServiceBusReceivedMessage> messages, CancellationToken cancellationToken)
	{
		return QueueMessages(azure, selectedResource, ConvertMessages(messages), cancellationToken);
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

	private async Task QueueMessages(IAzure azure, IAzureResource selectedResource, IReadOnlyList<ServiceBusMessage> messages, CancellationToken cancellationToken)
	{
		if (selectedResource is not (QueueDeadLetter or Topic or TopicSubscription or TopicSubscriptionDeadLetter))
			throw new NotSupportedException(selectedResource.GetType().FullName);

		var serviceBusNamespace = await azure.ServiceBusNamespaces
			.GetByIdAsync(selectedResource.ServiceBusId, cancellationToken);
		var serviceBusClient = await serviceBusNamespace
			.CreateServiceBusClient(AccessRights.Send, cancellationToken);

		await PushMessage(selectedResource.Name, messages, serviceBusClient, cancellationToken);
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
