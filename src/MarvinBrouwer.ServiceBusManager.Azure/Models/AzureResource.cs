using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Wrapper class for <see cref="IResource"/>s to make handling them more explicit
/// </summary>
public interface IAzureResource<out TResource>
	where TResource : IResource
{
	/// <summary>
	/// The original <see cref="IServiceBusNamespace"/> to which this resource belongs.
	/// </summary>
	IServiceBusNamespace ServiceBus { get; }

	/// <summary>
	/// The Fluent <typeparamref name="TResource"/> this wrapper refers to.
	/// </summary>
	TResource InnerResource { get; }

	/// <summary>
	/// The formatted Path, used for querying/modifying resources via the API.
	/// </summary>
	string Path { get; }
}

/// <inheritdoc cref="IAzureResource{TResource}" />
public abstract class AzureResource<TResource> : IAzureResource<TResource>
	where TResource : IResource
{
	/// <inheritdoc />
	public IServiceBusNamespace ServiceBus { get; protected init; } = default!;

	/// <inheritdoc />
	public TResource InnerResource { get; protected init; } = default!;

	/// <inheritdoc />
	public virtual string Path => InnerResource.Name;
}