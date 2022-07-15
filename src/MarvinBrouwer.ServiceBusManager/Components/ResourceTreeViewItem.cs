using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MarvinBrouwer.ServiceBusManager.Components;


public abstract class ResourceTreeViewItem : BaseTreeViewItem
{
	public IAzureResource<IResource> Resource { get; }

	protected ResourceTreeViewItem(IAzureResource<IResource> resource)
	{
		Resource = resource;
	}
}

