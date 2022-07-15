using MarvinBrouwer.ServiceBusManager.Components;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarvinBrouwer.ServiceBusManager.Services;

public interface IAzureLandscapeRenderingService
{
	IAsyncEnumerable<SubscriptionTreeViewItem> LoadSubscriptions(CancellationToken cancellationToken);

	Task LoadSubscriptionContents(
		SubscriptionTreeViewItem subscriptionTreeViewItem, CancellationToken cancellationToken);

	Task LoadServiceBusResources(
		ServiceBusTreeViewItem serviceBusTreeViewItem, CancellationToken cancellationToken);

	Task LoadTopicSubscriptions(
		TopicTreeViewItem topicTreeViewItem, CancellationToken cancellationToken);
}