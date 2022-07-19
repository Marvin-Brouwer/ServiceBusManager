using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="IServiceBusNamespace"/>
/// </summary>
public sealed class ServiceBus : AzureResource<IServiceBusNamespace>
{
	/// <inheritdoc cref="ServiceBus"/>
	public ServiceBus(IServiceBusNamespace serviceBus)
	{
		ServiceBus = serviceBus;
		InnerResource = serviceBus;
	}
}