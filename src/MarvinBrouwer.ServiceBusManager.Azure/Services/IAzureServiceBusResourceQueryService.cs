using MarvinBrouwer.ServiceBusManager.Azure.Models;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

internal interface IAzureServiceBusResourceQueryService
{
	Task<long> GetMessageCount(ServiceBusResource selectedResource, CancellationToken cancellationToken);
	Task<MessageHandler> DownloadFullResource(ServiceBusResource selectedResource, CancellationToken cancellationToken);
}