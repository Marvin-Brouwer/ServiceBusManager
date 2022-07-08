using MarvinBrouwer.ServiceBusManager.Azure.Models;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public interface IAzureServiceBusService
{
	IEnumerable<Task<ServiceBus>> ListServiceBuses(List<string> secrets, CancellationToken cancellationToken);
	Task<ServiceBus> GetServiceBus(string secret, CancellationToken cancellationToken);
}