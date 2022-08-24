using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Components;

/// <summary>
/// <see cref="BaseTreeViewItem"/> item for a specific azure resource
/// </summary>
public abstract class AzureResourceTreeViewItem : BaseTreeViewItem
{
	/// <summary>
	/// Selected resource ot use for actions
	/// </summary>
	public IAzureSubscription AzureSubscription { get; }

	/// <inheritdoc cref="ServiceBusResourceTreeViewItem"/>
	protected internal AzureResourceTreeViewItem(IAzureSubscription subscription)
	{
		AzureSubscription = subscription;
	}
}

