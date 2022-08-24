using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Microsoft.Azure.Management.ServiceBus.Fluent;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Wrapper class for <see cref="IResource"/>s to make handling them more explicit
/// </summary>
public interface IAzureResource
{
	/// <summary>
	/// The original <see cref="IAzureSubscription"/> to which this resource belongs.
	/// </summary>
	IAzureSubscription AzureSubscription { get; }

	/// <summary>
	/// The original <see cref="IServiceBusNamespace"/>'s Name to which this resource belongs.
	/// </summary>
	string ServiceBusId { get; }

	/// <summary>
	/// The original <see cref="IServiceBusNamespace"/>'s Name to which this resource belongs.
	/// </summary>
	string ServiceBusName { get; }

	/// <inheritdoc cref="IIndexable.Key"/>
	string Key { get; }

	/// <summary>
	/// The original resource Id
	/// </summary>
	string Id { get; }

	/// <summary>
	/// The original resource Name
	/// </summary>
	string Name { get; }

	/// <summary>
	/// The formatted Path, used for querying/modifying resources via the API.
	/// </summary>
	string Path { get; }
}

/// <inheritdoc cref="IAzureResource" />
public abstract class AzureResource : IAzureResource
{
	/// <inheritdoc />
	public IAzureSubscription AzureSubscription { get; protected init; } = default!;

	/// <inheritdoc />
	public string ServiceBusId { get; protected init; } = default!;

	/// <inheritdoc />
	public string ServiceBusName { get; protected init; } = default!;

	/// <inheritdoc />
	public string Key { get; protected init; } = default!;

	/// <inheritdoc />
	public string Id { get; protected init; } = default!;

	/// <inheritdoc />
	public string Name { get; protected init; } = default!;

	/// <inheritdoc />
	public virtual string Path => Name;
}