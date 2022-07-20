using MarvinBrouwer.ServiceBusManager.Components;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarvinBrouwer.ServiceBusManager.Services;

/// <summary>
/// Service dedicated to rending the Tree view using the Azure resource data available.
/// </summary>
public interface IAzureLandscapeRenderingService
{
	/// <summary>
	/// Load all Azure subscriptions available to the current User
	/// </summary>
	IAsyncEnumerable<SubscriptionTreeViewItem> LoadSubscriptions(CancellationToken cancellationToken);

	/// <summary>
	/// Load all ServiceBuses for this subscription
	/// </summary>
	Task LoadSubscriptionContents(
		SubscriptionTreeViewItem subscriptionTreeViewItem, CancellationToken cancellationToken);

	/// <summary>
	/// Load all ServiceBus Resources for this ServiceBus
	/// </summary>
	Task LoadServiceBusResources(
		ServiceBusTreeViewItem serviceBusTreeViewItem, CancellationToken cancellationToken);

	/// <summary>
	/// Load all Subscriptions for this Topic
	/// </summary>
	Task LoadTopicSubscriptions(
		TopicTreeViewItem topicTreeViewItem, CancellationToken cancellationToken);
}