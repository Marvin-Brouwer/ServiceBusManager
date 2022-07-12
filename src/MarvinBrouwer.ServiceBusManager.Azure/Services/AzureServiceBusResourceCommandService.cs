//using System.Runtime.Serialization;
//using Azure.Messaging.ServiceBus;
//using MarvinBrouwer.ServiceBusManager.Azure.Models;
//using Newtonsoft.Json.Linq;

//namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

//internal class AzureServiceBusResourceCommandService : IAzureServiceBusResourceCommandService
//{
//	private readonly IAzureServiceBusClientFactory _clientFactory;

//	public AzureServiceBusResourceCommandService(IAzureServiceBusClientFactory clientFactory)
//	{
//		_clientFactory = clientFactory;
//	}

//	public static async Task ClearResource(MessageHandler resourceData, CancellationToken cancellationToken)
//	{
//		var tasks = resourceData.Messages.Select(dataItem =>
//			resourceData.Receiver.CompleteMessageAsync(dataItem, cancellationToken));
//		await Task.WhenAll(tasks);
//	}

//	public static async Task ReleaseResource(MessageHandler resourceData, CancellationToken cancellationToken)
//	{
//		var tasks = resourceData.Messages.Select(dataItem =>
//			resourceData.Receiver.AbandonMessageAsync(dataItem, null, cancellationToken));
//		await Task.WhenAll(tasks);
//	}

//	public Task RequeueDistinctBy(MessageHandler resourceData, string propertyName, CancellationToken cancellationToken)
//	{
//		var convertedMessages = resourceData.Messages
//			.OrderBy(message => message.SequenceNumber)
//			.Select(message => ConvertMessage(message, propertyName))
//			.ToList();

//		if (convertedMessages.Any(message => string.IsNullOrWhiteSpace(message.id)))
//			throw new InvalidDataContractException($"The array contains an item without the {propertyName} property");

//		return RequeueMessages(resourceData, convertedMessages, cancellationToken);
//	}

//	private async Task RequeueMessages(
//		MessageHandler resourceData,
//		List<(ServiceBusReceivedMessage message, string? id)> convertedMessages,
//		CancellationToken cancellationToken)
//	{
//		foreach (var (message, _) in convertedMessages
//			.GroupBy(message => message.id)
//			.Select(group => @group.First()))
//		{
//			await using var sender = _clientFactory.CreateServiceBusSender(resourceData);
//			var sendMessage = new ServiceBusMessage(message.Body)
//			{
//				ContentType = message.ContentType
//			};
//			await sender.SendMessageAsync(sendMessage, cancellationToken);
//			await sender.CloseAsync(cancellationToken);
//		}
//	}

//	public async Task QueueMessages(
//		ServiceBusResource selectedResource,
//		List<string> messages,
//		CancellationToken cancellationToken)
//	{
//		var client = await _clientFactory.GetServiceBusClient(selectedResource.ServiceBus.Secret, cancellationToken);
//		await using var sender = _clientFactory.CreateServiceBusSender(selectedResource, client);

//		try
//		{
//			foreach (var message in messages)
//			{
//				var sendMessage = new ServiceBusMessage(message)
//				{
//					ContentType = "application/json"
//				};
//				await sender.SendMessageAsync(sendMessage, cancellationToken);
//			}
//		}
//		finally
//		{
//			await sender.CloseAsync(cancellationToken);
//		}
//	}


//	private static (ServiceBusReceivedMessage message, string? id) ConvertMessage(ServiceBusReceivedMessage message, string propertyName)
//	{
//		if (!message.ContentType.Contains("json"))
//			throw new NotSupportedException("This content type is not supported");

//		return (message, JObject.Parse(message.Body.ToString())[propertyName]?.Value<string>());
//	}
//}
