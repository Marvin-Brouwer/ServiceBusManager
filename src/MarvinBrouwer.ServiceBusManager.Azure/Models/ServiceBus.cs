using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="IServiceBusNamespace"/>
/// </summary>
public sealed class ServiceBus : AzureResource
{
	/// <inheritdoc cref="ServiceBus"/>
	public ServiceBus(IAzureSubscription subscription, IServiceBusNamespace serviceBus)
	{
		AzureSubscription = subscription;
		ServiceBusId = serviceBus.Id;
		ServiceBusName = serviceBus.Name;

		Key = serviceBus.Key;
		Id = serviceBus.Id;
		Name = serviceBus.Name;

		ResourceGroupName = serviceBus.ResourceGroupName;
	}

	/// <inheritdoc cref="IHasResourceGroup.ResourceGroupName"/>
	public string ResourceGroupName { get; }
}