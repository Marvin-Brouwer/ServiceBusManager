using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MarvinBrouwer.ServiceBusManager.Components;

/// <summary>
/// <see cref="BaseTreeViewItem"/> item for a specific azure <see cref="IResource"/>
/// </summary>
public abstract class ServiceBusResourceTreeViewItem : AzureResourceTreeViewItem
{
	/// <summary>
	/// Selected resource ot use for actions
	/// </summary>
	public IAzureResource Resource { get; }

	/// <inheritdoc cref="ServiceBusResourceTreeViewItem"/>
	protected internal ServiceBusResourceTreeViewItem(IAzureResource resource): base(resource.AzureSubscription)
	{
		Resource = resource;
	}
}

