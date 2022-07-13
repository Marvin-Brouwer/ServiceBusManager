using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public interface IAzureServiceBusResourceQueryService
{
	Task<long> GetMessageCount(IResource selectedResource, bool countDeadLetter, CancellationToken cancellationToken);
	// todo change type
	// Task<object> DownloadFullResource(IResource selectedResource, CancellationToken cancellationToken);
}