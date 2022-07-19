using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public interface IAzureResource<out TResource>
	where TResource : IResource
{
	IServiceBusNamespace ServiceBus { get; }
	TResource InnerResource { get; }
	string Path { get; }
}


public abstract class AzureResource<TResource> : IAzureResource<TResource>
	where TResource : IResource
{
	public IServiceBusNamespace ServiceBus { get; protected init; }
	public TResource InnerResource { get; protected init; }

	public virtual string Path => InnerResource.Name;
}