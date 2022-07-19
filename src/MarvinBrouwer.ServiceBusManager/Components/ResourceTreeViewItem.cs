using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MarvinBrouwer.ServiceBusManager.Components;

/// <summary>
/// <see cref="BaseTreeViewItem"/> item for a specific azure <see cref="IResource"/>
/// </summary>
internal abstract class ResourceTreeViewItem : BaseTreeViewItem
{
	/// <summary>
	/// Selected resource ot use for actions
	/// </summary>
	public IAzureResource<IResource> Resource { get; }

	/// <inheritdoc cref="ResourceTreeViewItem"/>
	protected ResourceTreeViewItem(IAzureResource<IResource> resource)
	{
		Resource = resource;
	}
}

