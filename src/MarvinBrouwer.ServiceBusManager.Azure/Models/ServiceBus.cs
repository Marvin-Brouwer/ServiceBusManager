using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed class ServiceBus : AzureResource<IServiceBusNamespace>
{
	public ServiceBus(IServiceBusNamespace serviceBus)
	{
		ServiceBus = serviceBus;
		InnerResource = serviceBus;
	}
}