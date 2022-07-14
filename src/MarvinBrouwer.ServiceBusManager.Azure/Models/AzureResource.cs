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


public abstract record AzureResource<TResource>(IServiceBusNamespace ServiceBus, TResource InnerResource) : IAzureResource<TResource>
	where TResource : IResource
{
	public virtual string Path => InnerResource.Name;
}