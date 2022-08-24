using Microsoft.Azure.Management.ServiceBus.Fluent;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="IServiceBusNamespace"/>
/// </summary>
public sealed class ServiceBus : AzureResource<IServiceBusNamespace>
{
	/// <inheritdoc cref="ServiceBus"/>
	public ServiceBus(IAzureSubscription subscription, IServiceBusNamespace serviceBus)
	{
		Subscription = subscription;
		ServiceBus = serviceBus;
		InnerResource = serviceBus;
	}
}