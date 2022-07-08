using MarvinBrouwer.ServiceBusManager.Azure.Models;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

internal interface IAzureServiceBusResourceCommandService
{
	Task RequeueDistinctBy(MessageHandler resourceData, string propertyName, CancellationToken cancellationToken);

	Task QueueMessages(
		ServiceBusResource selectedResource,
		List<string> messages,
		CancellationToken cancellationToken);
}